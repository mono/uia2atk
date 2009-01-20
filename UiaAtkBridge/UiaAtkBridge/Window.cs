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
		private Splitter splitter = null;
		
		public Window (IRawElementProviderSimple provider) : base (provider)
		{
			rootProvider = (IRawElementProviderFragmentRoot) provider;
			if (rootProvider != null) {
				Name = (string) rootProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);

				//FIXME: change this not to use Navigation when we fix the FIXME in Adapter ctor.
				IRawElementProviderFragment rootOfRootProvider 
					= (IRawElementProviderFragment) rootProvider.Navigate (NavigateDirection.Parent);
				//NavigateDirection.Parent in IRawElementProviderFragmentRoot-based provider 
				//return FragmentRoot and FragmentRoot is the same reference.
				if (rootOfRootProvider != null && rootOfRootProvider != rootProvider)
					Role = Atk.Role.Dialog;
				else
					Role = Atk.Role.Frame;
			} else
				Role = Atk.Role.Frame;
		}

		internal Window () : base (null)
		{
			Role = Atk.Role.Window;
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
			if (eventId == AutomationElementIdentifiers.AutomationFocusChangedEvent)
				GainActiveState ();
			if (eventId == AutomationElementIdentifiers.WindowDeactivatedEvent)
				LoseActiveState ();
			else
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

		internal override void AddOneChild (Atk.Object child)
		{
			if (splitter != null) {
				splitter.AddOneChild (child);
				return;
			}

			base.AddOneChild (child);
			if (child is Splitter) {
				splitter = (Splitter)child;
				// Remove any other existing children and
				// add them to the splitter
				int count = NAccessibleChildren;
				for (int i = 0; i < count;) {
					Atk.Object obj = RefAccessibleChild (i);
					if (obj == child) {
						i++;
						continue;
					}
					RemoveChild (obj);
					obj.Parent = child;
					splitter.AddOneChild (obj);
					count--;
				}
			}
		}

		internal override void RemoveChild (Atk.Object childToRemove)
		{
			if (splitter != null && childToRemove == splitter) {
				int count = splitter.NAccessibleChildren;
				while (count > 0) {
					Atk.Object obj = childToRemove.RefAccessibleChild (0);
					splitter.RemoveChild (obj);
					obj.Parent = this;
					AddOneChild (obj);
					count--;
				}
				splitter = null;
				}
			base.RemoveChild (childToRemove);
			}

		private void NewActiveState (bool active)
		{
			this.active = active;
			NotifyStateChange (Atk.StateType.Active, active);
		}
		
		internal void LoseActiveState ()
		{
			NewActiveState (false);
		}

		internal void GainActiveState ()
		{
			NewActiveState (true);
		}
	}
}
