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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class Window : ComponentParentAdapter
	{
		private IRawElementProviderFragmentRoot rootProvider;
		
		public Window (IRawElementProviderSimple provider) : base (provider)
		{
			rootProvider = (IRawElementProviderFragmentRoot) provider;
			Role = Atk.Role.Frame;
			if (rootProvider != null)
				Name = (string) rootProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
		}
		
		public override void RaiseStructureChangedEvent (object childProvider, StructureChangedEventArgs e)
		{
			/*IRawElementProviderSimple simpleChildProvider =
				(IRawElementProviderSimple) childProvider;
			//TODO: remove elements
			if (e.StructureChangeType == StructureChangeType.ChildrenBulkAdded) {
				int controlTypeId = (int) simpleChildProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
				if (controlTypeId == ControlType.Button.Id) {
					// TODO: Consider generalizing...
					Button button = new Button ((IInvokeProvider) childProvider);
					AddOneChild (button);
					AddRelationship (Atk.RelationType.Embeds, button);
					//TODO: add to mappings
				}
			}*/
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			base.RaiseAutomationEvent (eventId, e);
		}
		
		public override Atk.Layer Layer {
			get { return Atk.Layer.Window; }
		}
		
		public override int MdiZorder {
			get { return -1; }
		}

		private bool active = false;
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			if (active)
				states.AddState (Atk.StateType.Active);
			else
				states.RemoveState (Atk.StateType.Active);
			
			return states;
		}

		private void NewActiveState (bool active)
		{
			this.active = active;
			NotifyStateChange (Atk.StateType.Active, active);
		}
		
		internal void LooseActiveState ()
		{
			NewActiveState (false);
		}

		internal void GainActiveState ()
		{
			NewActiveState (true);
		}
	}
}
