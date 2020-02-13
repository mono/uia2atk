// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions: 
//  
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software. 
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// 
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Services;
using Mono.Unix;

namespace Mono.UIAutomation.Winforms
{
	internal delegate IRawElementProviderFragment ComponentProviderMapperHandler (Component component);

	internal static class ProviderFactory
	{	
		#region Static Fields
		
		// NOTE: This may not be the best place to track this...however
		//       I forsee this factory class evolving into a builder
		//       class that takes raw providers and attaches provider
		//       behaviors depending on the control type, and maybe
		//       it makes sense for the builder to keep track of
		//       this mapping?
		private static readonly Dictionary<Component, IRawElementProviderFragment> componentProviders
			 = new Dictionary<Component,IRawElementProviderFragment> ();
		
		private static readonly  List<IRawElementProviderFragmentRoot> formProviders
			= new List<IRawElementProviderFragmentRoot> ();

		private static readonly  Dictionary<Type, Type> providerComponentMap
			= new Dictionary<Type, Type> ();

		private static readonly Dictionary<Type, ComponentProviderMapperHandler> componentProviderMappers
			= new Dictionary<Type, ComponentProviderMapperHandler> ();

		public static readonly DesktopProvider DesktopProvider;

		#endregion
	
		#region Static Methods

		static ProviderFactory ()
		{
			Catalog.Init (Globals.CatalogName, Globals.LocalePath);
			InitializeProviderHash ();
			DesktopProvider = InitializeDesktopProvider ();
		}

		private static DesktopProvider InitializeDesktopProvider ()
		{
			var desktopProvider = (DesktopProvider) GetProvider (DesktopComponent.Instance);
			Helper.RaiseStructureChangedEvent (StructureChangeType.ChildAdded, desktopProvider);
			return desktopProvider;
		}

		private static void InitializeProviderHash ()
		{
			foreach (Type t in typeof (ProviderFactory).Assembly.GetTypes ()) {
				object[] attrs = t.GetCustomAttributes (
					typeof (MapsComponentAttribute), false);

				foreach (Attribute attr in attrs) {
					MapsComponentAttribute mca
						= attr as MapsComponentAttribute;
					if (mca == null) {
						continue;
					}

					if (mca.ProvidesMapper) {
						MethodInfo mi = t.GetMethod ("RegisterComponentMappings",
						                             BindingFlags.Static
						                             | BindingFlags.Public,
						                             null, new Type[0], null);
						if (mi == null) {
							Log.Warn ("{0} is a ProvidesMapper but does not implement RegisterComponentMappings.", t);
							continue;
						}

						// Allow the class to register its mappings
						mi.Invoke (null, null);
						continue;
					}

					if (providerComponentMap.ContainsKey (mca.From)) {
						Log.Warn ("Component map already contains a provider for {0}.  Ignoring.", mca.From);
						continue;
					}

					Log.Info ("Creating provider map from {0} to {1}", mca.From, t);
					providerComponentMap.Add (mca.From, t);
				}
			}
		}

		internal static FragmentControlProvider GetWrapper (Component component, IRawElementProviderSimple componentProvider)
		{
			var asFragment = componentProvider as IRawElementProviderFragment;
			var asFragmentRoot = asFragment != null ? componentProvider as IRawElementProviderFragmentRoot : null;

			if (asFragmentRoot != null)
				return new FragmentRootProviderWrapper (component, asFragmentRoot);

			if (asFragment != null)
				return new FragmentProviderWrapper (component, asFragment);

			return new SimpleProviderWrapper (component, componentProvider);
		}

		#endregion
		
		#region Static Public Methods
		
		public static List<IRawElementProviderFragmentRoot> GetFormProviders () 
		{
			return formProviders;
		}
		
		public static IRawElementProviderFragment GetProvider (Component component)
		{
			return GetProvider (component, true, false);
		}
		
		public static IRawElementProviderFragment GetProvider (Component component,
		                                                       bool initialize)
		{
			return GetProvider (component, initialize, false);
		}

		public static IRawElementProviderFragment GetProvider (Component component,
		                                                       bool initialize,
		                                                       bool forceInitializeChildren)
		{
			if (component == null)
				//FIXME: we should throw new ArgumentNullException ("component");
				return null;

			// First check if we've seen this component before
			IRawElementProviderFragment provider = FindProvider (component);
			if (provider != null)
				return provider;

			// Send a WndProc message to see if the control
			// implements it's own provider.
			if (component is SWF.Control
				// Sending WndProc to a form is broken for some reason
				&& !(component is SWF.Form)) {

				var control = (SWF.Control) component;

				IntPtr result = SWF.NativeWindow.WndProc (
					control.Handle,
					SWF.Msg.WM_GETOBJECT,
					IntPtr.Zero,
					new IntPtr (AutomationInteropProvider.RootObjectId));

				if (result != IntPtr.Zero) {
					IRawElementProviderSimple iSimpleProvider =
						AutomationInteropProvider.RetrieveAndDeleteProvider (result);
					provider = (iSimpleProvider as FragmentControlProvider) ?? GetWrapper (component, iSimpleProvider);
				}
			}
			else if (component is UserCustomComponent control) {
				provider = control.Provider;
			}

			ComponentProviderMapperHandler handler = null;
			Type providerType = null;

			if (provider == null) {
				Type typeIter = component.GetType ();

				// Chain up the type hierarchy until we find
				// either a type or handler for mapping, or we
				// hit Control or Component.
				do {
					// First see if there's a mapping handler
					if (componentProviderMappers.TryGetValue (typeIter,
					                                          out handler))
						break;

					// Next, see if we have a type mapping
					if (providerComponentMap.TryGetValue (typeIter,
					                                      out providerType))
						break;

					typeIter = typeIter.BaseType;
				} while (typeIter != null
				         && typeIter != typeof (System.ComponentModel.Component)
				         && typeIter != typeof (SWF.Control));
			}

			if (handler != null) {
				provider = handler (component);
			}

			// Create the provider if we found a mapping type
			if (provider == null) {
				// We meet a unknown custom control type,
				// then we count it as a Pane
				if (providerType == null) {
					var dialog = component as SWF.CommonDialog;
					if (dialog != null)
						return GetProvider (dialog.form, initialize, forceInitializeChildren);
					providerType = typeof (PaneProvider);
				}
				try {
					provider = (FragmentControlProvider)
						Activator.CreateInstance (providerType,
									  new object [] { component });
				} catch (MissingMethodException) {
					Log.Error (
						"Provider {0} does not have a valid single parameter constructor to handle {1}.",
						providerType, component.GetType ()
					);
					return null;
				}
			}

			if (provider != null) {
				// TODO: Abstract this out?
				if (component is SWF.Form) {
					formProviders.Add ((IRawElementProviderFragmentRoot) provider);
				}

				// TODO: Make tracking in dictionary optional
				componentProviders [component] = provider;
				if (provider is FragmentControlProvider frag) {
					if (initialize)
						frag.Initialize ();
					if (forceInitializeChildren)
						frag.InitializeChildControlStructure ();
				}
			} else {
				//FIXME: let's not throw while we are developing, a big WARNING will suffice
				//throw new NotImplementedException ("Provider not implemented for control " + component.GetType().Name);
				Log.Warn ("Provider not implemented for component " + component.GetType ());
				return null;
			}

			return provider;
		}

		public static void ReleaseProvider (Component component)
		{
			if (componentProviders.TryGetValue (component, out IRawElementProviderFragment provider)) {
				componentProviders.Remove (component);
				((FragmentControlProvider) provider).Terminate ();
				if (provider is FormProvider formProvider)
					formProviders.Remove (formProvider);
			}
		}
		
		public static IRawElementProviderFragment FindProvider (Component component)
		{
			IRawElementProviderFragment provider;
			
			if (component == null)
				//FIXME: we should throw instead of returning null
				//throw new ArgumentNullException ("component");
				return null;
			
			else if (componentProviders.TryGetValue (component, out provider))
				return provider;

			return null;
		}

		internal static void RegisterComponentProviderMapper (System.Type componentType,
		                                                      ComponentProviderMapperHandler handler)
		{
			componentProviderMappers.Add (componentType, handler);
		}
		
		#endregion
	}
}
