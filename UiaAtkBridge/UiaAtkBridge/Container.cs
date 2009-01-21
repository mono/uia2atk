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
//      Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class Container : ComponentParentAdapter
	{
		public Container (IRawElementProviderSimple provider): base (provider)
		{
			int controlTypeId = (int) Provider.GetPropertyValue (
			  AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (controlTypeId == ControlType.ToolBar.Id)
				Role = Atk.Role.ToolBar;
			else if (controlTypeId == ControlType.Group.Id || controlTypeId == ControlType.Pane.Id)
				Role = Atk.Role.Panel;
			else if (controlTypeId == ControlType.MenuItem.Id) {
				Role = Atk.Role.Panel;
				AddOneChild (new Button (provider));
			}
			else
				Role = Atk.Role.Filler;
		}

		public override int MdiZorder {
			get { return int.MinValue; }
		}

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == AutomationElementIdentifiers.AsyncContentLoadedEvent) {
				// TODO: Handle AsyncContentLoadedEvent
			} else if (eventId == AutomationElementIdentifiers.StructureChangedEvent) {
				// TODO: Handle StructureChangedEvent
			}
			else
				base.RaiseAutomationEvent (eventId, e);
		}

		public override void RaiseStructureChangedEvent (object childProvider, StructureChangedEventArgs e)
		{
			// TODO
		}
	}
}
