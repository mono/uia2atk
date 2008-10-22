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
	
	public class CheckBoxButton : Button
	{
		private IToggleProvider toggleProvider;
		
		public CheckBoxButton (IRawElementProviderSimple provider) : base (provider)
		{
			toggleProvider = (IToggleProvider) provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			imageProvider = toggleProvider;
			if (toggleProvider == null)
				throw new ArgumentException ("The provider for CheckBox should implement the Toggle pattern");

			Role = Atk.Role.CheckBox;
		}
		
		public override string GetName (int action)
		{
			if (action != 0)
				return null;

			return "click";
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			if (toggleProvider != null) {
				ToggleState state = toggleProvider.ToggleState;
				
				if (IsChecked (state))
					states.AddState (Atk.StateType.Checked);
				else
					states.RemoveState (Atk.StateType.Checked);
			}
			
			return states;
		}

		public override bool DoAction (int action)
		{
			if (toggleProvider != null) {
				try {
					if (action != 0)
						return false;
					toggleProvider.Toggle();
					return true;
				} catch (ElementNotEnabledException) {
					// TODO: handle this exception? maybe returning false is good enough
				}
			}
			return false;
		}
		
		
		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == TogglePatternIdentifiers.ToggleStateProperty)
				NotifyStateChange (Atk.StateType.Checked, IsChecked ((ToggleState)e.NewValue));
			else
				base.RaiseAutomationPropertyChangedEvent (e);
		}
		
		private bool IsChecked (ToggleState state)
		{
			switch (state) {
			case ToggleState.On:
				return true;
			case ToggleState.Indeterminate:
			case ToggleState.Off:
				return false;
			default:
				throw new NotSupportedException ("Unknown toggleState " + state.ToString ());
			}
		}
	}
}
