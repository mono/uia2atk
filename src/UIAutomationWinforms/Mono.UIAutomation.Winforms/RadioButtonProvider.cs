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

namespace Mono.UIAutomation.Winforms
{
	public class RadioButtonProvider : SimpleControlProvider, ISelectionItemProvider
	{
#region Private Fields
		
		private RadioButton radioButton;
		
#endregion
		
#region Constructors
		
		public RadioButtonProvider (RadioButton radioButton) :
			base (radioButton)
		{
			this.radioButton = radioButton;
			
			radioButton.CheckedChanged += OnCheckedChanged;
		}
		
#endregion
		
#region IRawElementProviderSimple Members
		
		public override object GetPatternProvider (int patternId)
		{
			if (patternId == SelectionItemPatternIdentifiers.Pattern.Id)
				return this;
			
			return null;
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ClassNameProperty.Id)
				return "WindowsForms10.BUTTON.app.0.bf7d44";
			else if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.RadioButton.Id;
			else if (propertyId == AutomationElementIdentifiers.IsPasswordProperty.Id)
				return false; // TODO: ???
			else if (propertyId == AutomationElementIdentifiers.IsControlElementProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
				return true;
			else
				return base.GetPropertyValue (propertyId);
		}
#endregion
		
#region ISelectionItem Members
	
		public void AddToSelection ()
		{
			Select ();
		}

		public bool IsSelected {
			get { return radioButton.Checked; }
		}

		public void RemoveFromSelection ()
		{
			return;
		}

		public void Select ()
		{
			radioButton.Checked = true;
		}

		public IRawElementProviderSimple SelectionContainer {
			get { return null; }
		}

#endregion
		
#region Event Handlers
		
		private void OnCheckedChanged (object sender, EventArgs e)
		{
			if (AutomationInteropProvider.ClientsAreListening) {
				AutomationEventArgs args =
					new AutomationEventArgs (SelectionItemPatternIdentifiers.ElementSelectedEvent);
				AutomationInteropProvider.RaiseAutomationEvent (SelectionItemPatternIdentifiers.ElementSelectedEvent,
				                                                this,
				                                                args);
				// TODO: Many other events to fire, including
				//       property change events!  This is also
				//       true for other providers, methinks.
			}
		}
		
#endregion
	}
}
