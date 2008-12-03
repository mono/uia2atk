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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Navigation;
using SWF = System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{

	internal abstract class FragmentControlProvider 
		: SimpleControlProvider, IRawElementProviderFragment, IEnumerable<FragmentControlProvider>
	{
		#region Private Fields
		
		private List<FragmentControlProvider> children;
		private INavigation navigation;
		
		#endregion
		
		#region Internal Data
		
		internal IDictionary<Component, FragmentControlProvider>
			componentProviders;  // TODO: Fix this...
		
		#endregion

		#region Constructor	
		
		protected FragmentControlProvider (Component component) : base (component)
		{
			componentProviders =
				new Dictionary<Component, FragmentControlProvider> ();
			children = new List<FragmentControlProvider> ();
		}
		
		#endregion
		
		#region IEnumerable Specializations
		
		public IEnumerator<FragmentControlProvider> GetEnumerator ()
		{
			foreach (FragmentControlProvider childProvider in children)
				yield return childProvider;
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		#endregion
		
		#region Public Events
		
		public NavigationEventHandler NavigationUpdated;
		
		#endregion

		#region Public Properties
		
		public INavigation Navigation {
			get { return navigation; }
			set { 
				if (value != navigation && navigation != null)
					navigation.Terminate ();

				navigation = value; 
			}
		}

		public int ChildrenCount {
			get { return children.Count; }
		}

		#endregion
		
		#region Public Methods

		public int GetChildProviderIndexOf (FragmentControlProvider provider)
		{
			return provider == null ? -1 : children.IndexOf (provider);
		}

		public FragmentControlProvider GetChildProviderAt (int index)
		{
			if (index < 0 || index >= children.Count)
				return null;
			else
				return children [index];
		}
		
		public void AddChildProvider (bool raiseEvent, 
		                              FragmentControlProvider provider) 
		{
			OnNavigationChildAdded (raiseEvent, provider);
		}

		public void RemoveChildProvider (bool raiseEvent, 
		                                 FragmentControlProvider provider) 
		{
			OnNavigationChildRemoved (raiseEvent, provider);
		}

		public override void Terminate ()
		{
			base.Terminate ();
			
			FinalizeChildControlStructure ();
		}
	
		//TODO: Are the generated events duplicated? Because we're already
		//supporting StructureChangeType when Children are added to Controls.
		//See: SimpleControlProvider.InitializeEvents
		public virtual void InitializeChildControlStructure ()
		{
			// HACK: This is just to make sure control providers
			//       aren't sent to bridge until the parent's already
			//       there.  There are about 100 ways to do this
			//       better.
			if (Control != null) {				
				Control.ControlAdded += OnControlAdded;
				Control.ControlRemoved += OnControlRemoved;
				
				foreach (SWF.Control childControl in Control.Controls) {
					FragmentControlProvider childProvider =
						CreateProvider (childControl);
					
					if (childProvider == null)
						continue;
					// TODO: Null check, compound, etc?

					componentProviders [childControl] = childProvider;
					OnNavigationChildAdded (false,
					                        (FragmentControlProvider) childProvider);
				}
			}
		}
		
		public virtual void FinalizeChildControlStructure ()
		{
			for (; children.Count > 0; )
				OnNavigationChildRemoved (false, 
				                          (FragmentControlProvider) children [0]);

			children.Clear ();
			componentProviders.Clear ();
		}
		
		#endregion
		
		#region Protected Methods
		
		protected virtual void OnNavigationUpdated (NavigationEventArgs args)
		{
			if (NavigationUpdated != null)
				NavigationUpdated (this, args);
		}

		protected virtual void OnNavigationChildAdded (bool raiseEvent, 
		                                               FragmentControlProvider childProvider)
		{
			if (children.Contains (childProvider) == true)
				return;
			
			childProvider.Navigation = NavigationFactory.CreateNavigation (childProvider, this);
			childProvider.Navigation.Initialize ();

			children.Add (childProvider);

			childProvider.InitializeChildControlStructure ();

			OnNavigationUpdated (new NavigationEventArgs (raiseEvent, 
			                                              StructureChangeType.ChildAdded, 
			                                              childProvider));
		}
		
		protected virtual void OnNavigationChildRemoved (bool raiseEvent, 
		                                                 FragmentControlProvider childProvider)
		{
			if (children.Contains (childProvider) == false)
				return;
			
			OnNavigationUpdated (new NavigationEventArgs (raiseEvent, 
			                                              StructureChangeType.ChildRemoved, 
			                                              childProvider));

			childProvider.Navigation.Terminate ();
			childProvider.Navigation = null;
			
			children.Remove (childProvider);
		}
			
		protected virtual void OnNavigationChildrenCleared (bool raiseEvent)
		{
			OnNavigationUpdated (new NavigationEventArgs (raiseEvent,
			                                              StructureChangeType.ChildrenReordered, 
			                                              null));
		}
               
		#endregion
		
		#region Private Methods: Event Handlers
	
		private void OnControlAdded (object sender, SWF.ControlEventArgs args)
		{
			SWF.Control childControl = args.Control;
			FragmentControlProvider childProvider = CreateProvider (childControl);
			if (childProvider == null)
				return;

			componentProviders [childControl] = childProvider;	
			OnNavigationChildAdded (true, 
			                        (FragmentControlProvider) childProvider);
		}
	
		private void OnControlRemoved (object sender, SWF.ControlEventArgs args)
		{
			FragmentControlProvider removedProvider;
			
			if (componentProviders.TryGetValue (args.Control, out removedProvider) == true) {
				
				// TODO: Some sort of disposal
				
				componentProviders.Remove (args.Control);
				// StructureChangedEvent
				// TODO: Use correct arguments, and fix bridge
				//       to handle them!!!
				//       Event source: Parent of removed child
				//       runtimeId: The child that was removed.
				//       (pg 6 of fxref_uiautomationtypes_p2.pdf)
				OnNavigationChildRemoved (true, 
				                          (FragmentControlProvider) removedProvider);
				ProviderFactory.ReleaseProvider (args.Control);
			}
		}

		#endregion

		#region Protected Methods
	
		protected FragmentControlProvider CreateProvider (SWF.Control control)
		{
			if (Mono.UIAutomation.Winforms.ErrorProvider.InstancesTracker.IsControlFromErrorProvider (control))
				return null;

			return (FragmentControlProvider) ProviderFactory.GetProvider (control);
		}
		
		protected FragmentControlProvider GetProvider (SWF.Control control)
		{
			if (componentProviders.ContainsKey (control))
				return componentProviders [control];
			else
				return null;
		}

		#endregion
		
		#region IRawElementProviderFragment Interface 

		public virtual System.Windows.Rect BoundingRectangle {
			get {
				return (Rect)
					GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
			}
		}
		
		public virtual IRawElementProviderFragmentRoot FragmentRoot {
			get { 
				return (IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (Container);
			}
		}
		
		public virtual IRawElementProviderSimple[] GetEmbeddedFragmentRoots () 
		{
			return null;
		}
		
		public int[] GetRuntimeId ()
		{
			return new int [] { AutomationInteropProvider.AppendRuntimeId, 
				(int) GetPropertyValue (AutomationElementIdentifiers.AutomationIdProperty.Id) };
		}
		
		public IRawElementProviderFragment Navigate (NavigateDirection direction) 
		{
			return Navigation == null ? null : Navigation.Navigate (direction);
		}
		
		public virtual void SetFocus ()
		{
			if (Control != null)
				Control.Focus ();
		}

		#endregion
		
	}
}
