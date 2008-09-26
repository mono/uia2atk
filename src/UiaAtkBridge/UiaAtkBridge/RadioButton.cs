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

	public class RadioButton : Button
	{
		ISelectionItemProvider selProvider;
		
		public RadioButton (IRawElementProviderSimple provider) : base (provider)
		{
			Role = Atk.Role.RadioButton;
			selProvider = (ISelectionItemProvider)provider.GetPatternProvider(SelectionItemPatternIdentifiers.Pattern.Id);
			if (selProvider == null)
				throw new ArgumentException ("The provider for RadioButton should implement the SelectionItem pattern");
			actionName = "click";
		}
		
		public override bool DoAction (int action)
		{
			if (selProvider != null) {
				try {
					if (action != 0)
						return false;
					selProvider.Select();
					return true;
				} catch (ElementNotEnabledException e) {
					// TODO: handle this exception? maybe returning false is good enough
				}
			}
			return false;
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			if (selProvider.IsSelected)
				states.AddState (Atk.StateType.Checked);
			else
				states.RemoveState (Atk.StateType.Checked);
			
			return states;
		}

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs args)
		{
			if (eventId == SelectionItemPatternIdentifiers.ElementSelectedEvent)
				NotifyStateChange (Atk.StateType.Checked, true);
			else
				base.RaiseAutomationEvent (eventId, args);
		}
		
	}
	
}
