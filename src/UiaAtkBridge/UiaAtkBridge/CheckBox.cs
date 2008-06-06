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
	
	public class CheckBox : Button
	{
		ToggleState currentState;
		
		public CheckBox (IRawElementProviderSimple provider) : base (provider)
		{
			Role = Atk.Role.CheckBox;
			
			bool enabled = (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			if (enabled)
				RefStateSet ().AddState (Atk.StateType.Enabled);
			else
				RefStateSet ().RemoveState (Atk.StateType.Enabled);
			
			ToggleState state = (ToggleState)((IToggleProvider)provider).ToggleState;
			
			switch (state) {
			case ToggleState.On:
				currentState = ToggleState.On;
				RefStateSet ().AddState (Atk.StateType.Checked);
				break;
			case ToggleState.Off:
				currentState = ToggleState.Off;
				RefStateSet ().RemoveState (Atk.StateType.Checked);
				break;
			case ToggleState.Indeterminate:
				currentState = ToggleState.Indeterminate;
				break;
			default:
				throw new NotSupportedException ("Unknown toggleState " + state.ToString ());
			}
		}
		
		public override string GetName (int action)
		{
			if (action != 0)
				return null;

			return "click";
		}
		
		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == TogglePatternIdentifiers.ToggleStateProperty) {
				ToggleState state = (ToggleState)e.NewValue;
				
				switch (state) {
				case ToggleState.On:
					RefStateSet ().AddState (Atk.StateType.Checked);
					break;
				case ToggleState.Off:
					RefStateSet ().RemoveState (Atk.StateType.Checked);
					break;
				case ToggleState.Indeterminate:
					if (currentState == ToggleState.On)
						RefStateSet ().RemoveState (Atk.StateType.Checked);
					break;
				default:
					throw new NotSupportedException ("Unknown toggleState " + state.ToString ());
				}
			} else {
				base.RaiseAutomationPropertyChangedEvent (e);
			}
		}
	}
}
