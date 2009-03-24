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
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Services;

using NDesk.DBus;
using org.freedesktop.DBus;

namespace UiaDbusCoreBridge
{
	public class AutomationBridge : IAutomationBridge
	{
		private const string Path = "/org/mono-a11y/Uia/AutomationElement";
		private const string Namespace = "org.mono-a11y.Uia";
		private bool mainLoopStarted = false;
		private static bool runMainLoop = false;
		private Thread mainLoop = null;
		private Dictionary<IntPtr, IRawElementProviderSimple> pointerProviderMapping =
			new Dictionary<IntPtr, IRawElementProviderSimple> ();
		private int windowProviderCount = 0;
		
		private static volatile Bus sessionBus = null;
		protected static object syncRoot = new object ();

		private static Bus SessionBus {
			get {
				lock (syncRoot) {
					if (sessionBus == null)
						sessionBus = Bus.Session;
					return sessionBus;
				}
			}
		}

		#region IAutomationBridge implementation
		public object HostProviderFromHandle (IntPtr hwnd)
		{
			IRawElementProviderSimple provider = null;
			pointerProviderMapping.TryGetValue (hwnd, out provider);
			return provider;
		}
		
		public void RaiseAutomationEvent (AutomationEvent eventId, object provider, AutomationEventArgs e)
		{
			// TODO
//			if (eventId == null && provider == null && e == null) {
//				BusG.Init ();
//			}
		}
		
		public void RaiseAutomationPropertyChangedEvent (object element, AutomationPropertyChangedEventArgs e)
		{
			// TODO
		}

		private bool registeredOne = false;
		
		public void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			IRawElementProviderSimple simpleProvider =
				provider as IRawElementProviderSimple;
			
			if (e.StructureChangeType == StructureChangeType.ChildAdded) {
				CheckMainLoop ();
				if (registeredOne)
					return;
				
				if (ControlType.Window.Id == (int)
				    simpleProvider.GetPropertyValue (AEIds.ControlTypeProperty.Id)) {
					IntPtr providerHandle = (IntPtr)
						simpleProvider.GetPropertyValue (AEIds.NativeWindowHandleProperty.Id);
					pointerProviderMapping [providerHandle] = simpleProvider;
					windowProviderCount++;
				}
				
				if (simpleProvider != null) {
					DbusProviderWrapper element = new DbusProviderWrapper (simpleProvider);
					Console.WriteLine ("Made an element");
					#pragma warning disable 0618
					SessionBus.Register (Namespace,
					                     new ObjectPath (Path),
					                     element);
					#pragma warning restore 0618
					Console.WriteLine ("Registered an element: " + element.Name);
					registeredOne = true;
					if (SessionBus.RequestName (Namespace)
					    != RequestNameReply.PrimaryOwner) {
						Console.WriteLine ("not primary owner");
					} else {
						Console.WriteLine ("primary owner");
					}
				}
			} else if (e.StructureChangeType == StructureChangeType.ChildRemoved) {
				if (HandleTotalElementRemoval (simpleProvider)) {
					Console.WriteLine ("Stopping dbus bridge main loop");
					runMainLoop = false;
					mainLoop.Abort ();
				}
			}
		}

		private bool HandleTotalElementRemoval (IRawElementProviderSimple provider)
		{
			bool lastWindowProvider = false;
			
			int controlTypeId = (int)provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (controlTypeId == ControlType.DataItem.Id) {
				// No adapter created for it, but adapters
				// created for its children
				IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
				if (fragment == null)
					return false;
				for (IRawElementProviderFragment child = fragment.Navigate (NavigateDirection.FirstChild); 
				     child != null; 
				     child = child.Navigate (NavigateDirection.NextSibling))
					HandleTotalElementRemoval (child);
				return false;
			}

//			Atk.Object obj;
//			if (providerAdapterMapping.TryGetValue (provider, out obj) == false)
//				return false;
//			Adapter adapter = obj as Adapter;
//			if (adapter == null)
//				return false;

//			ParentAdapter parentAdapter = adapter.Parent as ParentAdapter;
//			if (parentAdapter != null)
//				parentAdapter.PreRemoveChild (adapter);

			//foreach (Atk.Object atkObj in GetAdaptersDescendantsFamily (adapter)) {
			IRawElementProviderFragment providerFragment =
				provider as IRawElementProviderFragment;
			Console.WriteLine ("Total check frag");
			if (providerFragment == null)
				return false;
			Console.WriteLine ("Total handle desc");
			if (HandleDescendantElementRemoval (providerFragment))
				lastWindowProvider = true;
			if (HandleElementRemoval (providerFragment))
				lastWindowProvider = true;

			return lastWindowProvider;
		}

		private bool HandleDescendantElementRemoval (IRawElementProviderFragment providerFragment)
		{
			bool lastWindowProvider = false;
			var current = providerFragment.Navigate (NavigateDirection.FirstChild);
			while (current != null) {
				if (HandleElementRemoval (current))
					lastWindowProvider = true;
				if (HandleDescendantElementRemoval (current))
					lastWindowProvider = true;
				current = current.Navigate (NavigateDirection.NextSibling);
			}
			return lastWindowProvider;
		}
		
		private bool HandleElementRemoval (IRawElementProviderSimple provider)
		{
			Console.WriteLine ("HandleElementRemoval");
			bool lastWindowProvider = false;
			
//			Atk.Object obj;
//			if (providerAdapterMapping.TryGetValue (provider, out obj) == false)
//				return false;
//			Adapter adapter = (Adapter)obj;

			int controlTypeId = (int) provider.GetPropertyValue (
				AutomationElementIdentifiers.ControlTypeProperty.Id);
			
			if (controlTypeId == ControlType.ToolTip.Id) {
				//TopLevelRootItem.Instance.RemoveChild (adapter);
			} else if (controlTypeId == ControlType.Window.Id) {
				// We should do the following, but it would
				// reintroduce bug 427857.
				//GLib.Signal.Emit (adapter, "deactivate");
				//GLib.Signal.Emit (adapter, "destroy");
//				TopLevelRootItem.Instance.RemoveChild (adapter);
				IntPtr providerHandle = IntPtr.Zero;
				foreach (IntPtr pointer in pointerProviderMapping.Keys) {
					if (provider == pointerProviderMapping [pointer]) {
						providerHandle = pointer;
						break;
					}
				}
				pointerProviderMapping.Remove (providerHandle);
				windowProviderCount--;
				if (windowProviderCount == 0)
					lastWindowProvider = true;
		 	} else {
//				ParentAdapter parent = adapter.Parent as ParentAdapter;
//				if (parent != null)
//					parent.RemoveChild (adapter);
			}

//			pointerProviderMapping.Remove (provider);

			return lastWindowProvider;
		}
		
		public void Initialize ()
		{
			// TODO
		}
		
		public void Terminate ()
		{
			// TODO
		}
		
		public bool IsAccessibilityEnabled {
			get {
				return true; // TODO
			}
		}
		
		public bool ClientsAreListening {
			get {
				return true; // TODO
			}
		}
		#endregion

		private void CheckMainLoop ()
		{
			if (mainLoopStarted)
				return;
			runMainLoop = true;
			mainLoop = new Thread (new ThreadStart (MainLoop));
			mainLoop.Start ();
			mainLoopStarted = true;
		}

		private void MainLoop ()
		{
			while (runMainLoop)
				SessionBus.Iterate ();
		}
	}

	[Interface ("org.mono-a11y.Uia.AutomationElement")]
	public class DbusProviderWrapper : MarshalByRefObject
	{
		private IRawElementProviderSimple provider;
		
		public DbusProviderWrapper (IRawElementProviderSimple provider)
		{
			this.provider = provider;
		}

		public string Name {
			get {
				return provider.GetPropertyValue (AEIds.NameProperty.Id) as string;
			}
		}

		public string WhatControlType ()
		{
			return ControlType.LookupById ((int) provider.GetPropertyValue (AEIds.ControlTypeProperty.Id)).ProgrammaticName;
		}
	}
}
