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
// 

using System;
using System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms
{
	public class CheckBoxProvider : SimpleControlProvider, IToggleProvider
	{
#region Private Fields
		
		private CheckBox checkbox;
		
#endregion		
	
#region Constructors
		
		public CheckBoxProvider(CheckBox checkbox) : base (checkbox)
		{
			this.checkbox = checkbox;
		}
		
#endregion
		
#region Protected Methods

		protected override void InitializeEvents ()
		{
			base.InitializeEvents ();

			SetEvent (EventStrategyType.ToggleStateProperty,
			          new DefaultToggleStatePropertyEvent (this, checkbox));
		}
		
#endregion
		
#region IRawElementProviderSimple Members
	
		public override object GetPatternProvider (int patternId)
		{
			if (patternId == TogglePatternIdentifiers.Pattern.Id)
				return this;
			
			return null;
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.CheckBox.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "check box";
			else
				return base.GetPropertyValue (propertyId);
		}

#endregion
		
#region IToggleProvider Members
	
		public void Toggle ()
		{
			switch (checkbox.CheckState) {
			case CheckState.Checked:
				checkbox.CheckState = CheckState.Unchecked;
				break;
			case CheckState.Unchecked:
				if (checkbox.ThreeState)
					checkbox.CheckState = 
						CheckState.Indeterminate;
				else
					checkbox.CheckState = 
						CheckState.Checked;
				break;
			// Control could still have been set to intermediate
			// programatically, regardless of ThreeState value.
			case CheckState.Indeterminate:
			default:
				checkbox.CheckState = CheckState.Checked;
				break;
			}
		}

		public ToggleState ToggleState {
			get {
				switch (checkbox.CheckState) {
				case CheckState.Checked:
					return ToggleState.On;
				case CheckState.Unchecked:
					return ToggleState.Off;
				case CheckState.Indeterminate:
				default:
					return ToggleState.Indeterminate;
				}
			}
		}

#endregion
	}
}
