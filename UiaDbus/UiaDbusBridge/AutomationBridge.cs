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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
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

using DC = Mono.UIAutomation.UiaDbus;
using Mono.UIAutomation.UiaDbus.Interfaces;
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.UiaDbusBridge.Wrappers;

using NDesk.DBus;
using org.freedesktop.DBus;

namespace Mono.UIAutomation.UiaDbusBridge
{
	internal class AutomationBridge : IAutomationBridge
	{
		#region Private Members

		// TODO: Determine if HostProviderFromHandle would break if only
		//       one pointer/provider mapping were used
		private Dictionary<IntPtr, IRawElementProviderSimple> pointerWindowProviderMapping =
			new Dictionary<IntPtr, IRawElementProviderSimple> ();
		private Dictionary<IntPtr, IRawElementProviderSimple> pointerProviderMapping =
			new Dictionary<IntPtr, IRawElementProviderSimple> ();
		private Dictionary<IRawElementProviderSimple, ProviderElementWrapper> providerWrapperMapping =
			new Dictionary<IRawElementProviderSimple, ProviderElementWrapper> ();
		private int windowProviderCount = 0;
		
		private Application app = null;
		private bool mainLoopStarted = false;
		private static bool runMainLoop = false;
		private Thread mainLoop = null;
		
		private static volatile Bus sessionBus = null;
		private static volatile AutomationBridge instance = null;
		protected static object syncRoot = new object ();

		#endregion

		#region Constructor

		public AutomationBridge ()
		{
			lock (syncRoot) {
				if (instance != null)
					Log.Error ("Another UiaDbus AutomationBridge has been created" + Environment.StackTrace);
				else
					instance = this;
			}
		}

		#endregion

		#region Private Static Properties
		
		private static Bus SessionBus {
			get {
				lock (syncRoot) {
					if (sessionBus == null)
						sessionBus = Bus.Session;
					return sessionBus;
				}
			}
		}

		#endregion

		#region IAutomationBridge implementation
		
		public object HostProviderFromHandle (IntPtr hwnd)
		{
			IRawElementProviderSimple provider = null;
			pointerWindowProviderMapping.TryGetValue (hwnd, out provider);
			return provider;
		}

		public void RaiseAutomationEvent (AutomationEvent eventId, object provider, AutomationEventArgs e)
		{
			app.RaiseAutomationEvent (eventId, provider, e);
		}

		public void RaiseAutomationPropertyChangedEvent (object element, AutomationPropertyChangedEventArgs e)
		{
			IRawElementProviderSimple simpleProvider =
				element as IRawElementProviderSimple;

			ProviderElementWrapper wrapper = null;
			if (!providerWrapperMapping.TryGetValue (simpleProvider, out wrapper))
				return;

			if (e.NewValue != null && e.NewValue.Equals (false)) {
				int patternId = -1;

				if (e.Property == AutomationElementIdentifiers.IsDockPatternAvailableProperty)
					patternId = DockPatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty)
					patternId = ExpandCollapsePatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsGridItemPatternAvailableProperty)
					patternId = GridItemPatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsGridPatternAvailableProperty)
					patternId = GridPatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsInvokePatternAvailableProperty)
					patternId = InvokePatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsMultipleViewPatternAvailableProperty)
					patternId = MultipleViewPatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty)
					patternId = RangeValuePatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty)
					patternId = ScrollItemPatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsScrollPatternAvailableProperty)
					patternId = ScrollPatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty)
					patternId = SelectionItemPatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsSelectionPatternAvailableProperty)
					patternId = SelectionPatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsTableItemPatternAvailableProperty)
					patternId = TableItemPatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsTablePatternAvailableProperty)
					patternId = TablePatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsTextPatternAvailableProperty)
					patternId = TextPatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsTogglePatternAvailableProperty)
					patternId = TogglePatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsTransformPatternAvailableProperty)
					patternId = TransformPatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsValuePatternAvailableProperty)
					patternId = ValuePatternIdentifiers.Pattern.Id;
				else if (e.Property == AutomationElementIdentifiers.IsWindowPatternAvailableProperty)
					patternId = WindowPatternIdentifiers.Pattern.Id;

				if (patternId != -1)
					wrapper.UnregisterPattern (patternId);
			}
			app.RaiseAutomationPropertyChangedEvent (element, e);
		}

		public void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			IRawElementProviderSimple simpleProvider =
				provider as IRawElementProviderSimple;
			
			if (e.StructureChangeType == StructureChangeType.ChildAdded) {
				if (simpleProvider == null)
					return;

				object providerHandleObj =
					simpleProvider.GetPropertyValue (AEIds.NativeWindowHandleProperty.Id);
				IntPtr providerHandle = providerHandleObj != null ?
					(IntPtr) providerHandleObj :
					IntPtr.Zero;

				bool isWindow = false;
				if (ControlType.Window.Id == (int)
				    simpleProvider.GetPropertyValue (AEIds.ControlTypeProperty.Id)) {
					isWindow = true;
					pointerWindowProviderMapping [providerHandle] = simpleProvider;
					windowProviderCount++;
				}

				ProviderElementWrapper element = new ProviderElementWrapper (simpleProvider);
				element.Register (SessionBus);
				providerWrapperMapping [simpleProvider] = element;
				if (providerHandle != IntPtr.Zero)
					pointerProviderMapping [providerHandle] = simpleProvider;
				if (isWindow)
					app.AddRootElement (element);

				//The event shall be raised after the provider is added to providerWrapperMapping
				app.RaiseStructureChangedEvent (provider, e);
			} else if (e.StructureChangeType == StructureChangeType.ChildRemoved) {
				//The event shall be raised before the provider is deleted from providerWrapperMapping
				app.RaiseStructureChangedEvent (provider, e);
				HandleTotalElementRemoval (simpleProvider);
			} else {
				app.RaiseStructureChangedEvent (provider, e);
			}
		}

		public void Initialize ()
		{
			CheckMainLoop ();
		}

		public void Terminate ()
		{
			lock (syncRoot) {
				AbortMainLoop ();
				if (instance == this)
					instance = null;
			}
		}
		
		public bool IsAccessibilityEnabled {
			get {
				return true; // TODO?
			}
		}
		
		public bool ClientsAreListening {
			get {
				return true; // TODO
			}
		}
		
		#endregion

		#region Internal Methods and Properties

		internal ProviderElementWrapper GetProviderWrapper (IRawElementProviderSimple provider)
		{
			ProviderElementWrapper wrapper = null;
			if (providerWrapperMapping.TryGetValue (provider, out wrapper))
				return wrapper;
			return null;
		}

		internal static AutomationBridge Instance
		{
			get {
				return instance;
			}
		}

		internal string [] GetElementPaths (IRawElementProviderSimple[] elements)
		{
			string [] paths = new string [elements.Length];
			for (int i = 0; i < elements.Length; i++)
				paths [i] = FindWrapperByProvider (elements [i]).Path;
			return paths;
		}

		internal IRawElementProviderSimple FindProviderByPath (string path)
		{
			foreach (var entry in providerWrapperMapping)
			{
				if (entry.Value.Path == path)
						return entry.Key;
			}
			return null;
		}

		internal IRawElementProviderSimple FindProviderByRuntimeId (int [] runtimeId)
		{
			foreach (var provider in providerWrapperMapping.Keys)
			{
				int [] rid = (int []) provider.GetPropertyValue (
					AutomationElementIdentifiers.RuntimeIdProperty.Id);
				if (Automation.Compare (rid, runtimeId))
					return provider;
			}
			return null;
		}

		internal ProviderElementWrapper FindWrapperByProvider (IRawElementProviderSimple provider)
		{
			ProviderElementWrapper wrapper;
			if (providerWrapperMapping.TryGetValue (provider, out wrapper))
				return wrapper;
			return null;
		}

		internal ProviderElementWrapper FindWrapperByHandle (int handle)
		{
			IRawElementProviderSimple provider = null;
			pointerProviderMapping.TryGetValue (new IntPtr (handle),
			                                    out provider);
			if (provider != null)
				return FindWrapperByProvider (provider);
			return null;
		}

		#endregion

		#region Private Methods - Element Management

		private bool HandleTotalElementRemoval (IRawElementProviderSimple provider)
		{
			bool lastWindowProvider = false;
			
			IRawElementProviderFragment providerFragment =
				provider as IRawElementProviderFragment;
			if (providerFragment == null)
				return false;
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
			bool lastWindowProvider = false;
			
			ProviderElementWrapper element;
			if (!providerWrapperMapping.TryGetValue (provider, out element))
				return false;

			int controlTypeId = (int) provider.GetPropertyValue (
				AutomationElementIdentifiers.ControlTypeProperty.Id);

			IntPtr providerHandle = IntPtr.Zero;
			if (controlTypeId == ControlType.Window.Id) {
				app.RemoveRootElement (element);
				foreach (IntPtr pointer in pointerWindowProviderMapping.Keys) {
					if (provider == pointerWindowProviderMapping [pointer]) {
						providerHandle = pointer;
						break;
					}
				}
				pointerWindowProviderMapping.Remove (providerHandle);
				windowProviderCount--;
				if (windowProviderCount == 0)
					lastWindowProvider = true;
			} else {
				foreach (IntPtr pointer in pointerProviderMapping.Keys) {
					if (provider == pointerProviderMapping [pointer]) {
						providerHandle = pointer;
						break;
					}
				}
			}
			if (providerHandle != IntPtr.Zero)
				pointerProviderMapping.Remove (providerHandle);

			element.Unregister ();
			providerWrapperMapping.Remove (provider);
			app.RemoveProvider (provider);

			return lastWindowProvider;
		}

		#endregion

		#region Private Methods - DBus Main Loop

		private void CheckMainLoop ()
		{
			if (mainLoopStarted)
				return;
			runMainLoop = true;
			
			Bus bus = SessionBus;
			app = new Application ();
			bus.Register (new ObjectPath (DC.Constants.ApplicationPath),
			              app);
			mainLoop = new Thread (new ThreadStart (MainLoop));
			mainLoop.IsBackground = true;
			mainLoop.Start ();
			mainLoopStarted = true;
		}

		private void AbortMainLoop ()
		{
			runMainLoop = false;
			if (mainLoop != null) {// && mainLoop.IsAlive) {
				SessionBus.Unregister (new ObjectPath (DC.Constants.ApplicationPath));
				foreach (ProviderElementWrapper wrapper in providerWrapperMapping.Values)
					wrapper.Unregister ();
				Log.Info ("Stopping dbus bridge main loop");
				mainLoop.Abort ();
			}
			mainLoopStarted = false;
			mainLoop = null;
		}

		private void MainLoop ()
		{
			Bus bus = SessionBus;
			Log.Info ("About to start iterating main loop thread");
			while (runMainLoop)
				bus.Iterate ();
		}

		#endregion
	}
}
