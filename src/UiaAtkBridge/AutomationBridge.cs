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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Collections.Generic;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Bridge;

namespace UiaAtkBridge
{
	public class AutomationBridge : IAutomationBridge
	{
#region Private Fields
		
		private bool applicationStarted = false;
		private Monitor appMonitor = null;
		private Dictionary<IntPtr, IRawElementProviderSimple>
			pointerProviderMapping;
		private Dictionary<IRawElementProviderSimple, Adapter>
			providerAdapterMapping;
		
#endregion

#region Public Constructor
		
		public AutomationBridge ()
		{
			bool newMonitor = false;
			if (appMonitor == null) {
				Console.WriteLine ("about to create monitor");
				appMonitor = new Monitor();
				Console.WriteLine ("just made monitor");
			}
			pointerProviderMapping =
				new Dictionary<IntPtr,IRawElementProviderSimple> ();
			providerAdapterMapping =
				new Dictionary<IRawElementProviderSimple, Adapter>();
		}
		
#endregion
		
#region IAutomationBridge Members
		
		public bool ClientsAreListening {
			get {
				// TODO: How with ATK?
				return true;
			}
		}
		
		public object HostProviderFromHandle (IntPtr hwnd)
		{
			if (!pointerProviderMapping.ContainsKey (hwnd))
				return null;
			return pointerProviderMapping [hwnd];
		}

		
		public void RaiseAutomationEvent (AutomationEvent eventId, object provider, AutomationEventArgs e)
		{
			// TODO: Find better way to pass PreRun event on to bridge
			//        (nullx3 is a magic value)
			//        (once bridge events are working, should be able to happen upon construction, right?)
			if (eventId == null && provider == null && e == null) {
				if (!applicationStarted && appMonitor != null)
					appMonitor.ApplicationStarts ();
				return;
			}
			
			IRawElementProviderSimple simpleProvider =
				(IRawElementProviderSimple) provider;
			if (!providerAdapterMapping.ContainsKey (simpleProvider))
				return;
			
			providerAdapterMapping [simpleProvider].RaiseAutomationEvent (eventId, e);
		}
		
		public void RaiseAutomationPropertyChangedEvent (object element, AutomationPropertyChangedEventArgs e)
		{
			IRawElementProviderSimple simpleProvider =
				(IRawElementProviderSimple) element;
			if (!providerAdapterMapping.ContainsKey (simpleProvider))
				return;
			
			providerAdapterMapping [simpleProvider].RaiseAutomationPropertyChangedEvent (e);
		}
		
		public void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			IRawElementProviderSimple simpleProvider =
				(IRawElementProviderSimple) provider;
			int controlTypeId = (int) simpleProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

			if (e.StructureChangeType == StructureChangeType.ChildrenBulkAdded) {
				if (controlTypeId == ControlType.Window.Id)
					HandleNewWindowControlType (provider);
				else if (controlTypeId == ControlType.Button.Id)
					// TODO: Consider generalizing...
					HandleNewButtonControlType (provider);
				// TODO: Other providers
			} else if (e.StructureChangeType == StructureChangeType.ChildrenBulkRemoved) {
				if (controlTypeId == ControlType.Window.Id)
					HandleWindowProviderRemoval ((IWindowProvider)provider);
			}
			
			// TODO: Other structure changes
		}
		
#endregion
		
#region Private Methods
		
		private void HandleNewWindowControlType (object provider)
		{
			IRawElementProviderSimple simpleProvider =
				(IRawElementProviderSimple) provider;
			
			Window newWindow = new Window (provider);			
			providerAdapterMapping [simpleProvider] = newWindow;
			
			TopLevelRootItem.Instance.AddOneChild (newWindow);
			
			IntPtr providerHandle = (IntPtr) simpleProvider.GetPropertyValue (AutomationElementIdentifiers.NativeWindowHandleProperty.Id);
			pointerProviderMapping [providerHandle] = simpleProvider;
		}
		
		private void HandleWindowProviderRemoval (IWindowProvider provider)
		{
			Console.WriteLine ("FormIsRemoved");
			TopLevelRootItem.Instance.RemoveChild (providerAdapterMapping [(IRawElementProviderSimple) provider]);
			providerAdapterMapping.Remove ((IRawElementProviderSimple) provider);
			
			IRawElementProviderSimple simpleProvider =
				(IRawElementProviderSimple) provider;
			IntPtr providerHandle = (IntPtr) simpleProvider.GetPropertyValue (AutomationElementIdentifiers.NativeWindowHandleProperty.Id);
			pointerProviderMapping.Remove (providerHandle);
		}
		
		private void HandleNewButtonControlType (object provider)
		{
			IRawElementProviderSimple simpleProvider =
				(IRawElementProviderSimple) provider;
			IRawElementProviderSimple parentProvider =
				simpleProvider.HostRawElementProvider;
			
			ParentAdapter parentObject =
				(ParentAdapter) providerAdapterMapping [parentProvider];
			
			Button atkButton = new Button (provider);
			providerAdapterMapping [simpleProvider] = atkButton;
			
			parentObject.AddOneChild (atkButton);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkButton);
		}
		
#endregion
	}
}
