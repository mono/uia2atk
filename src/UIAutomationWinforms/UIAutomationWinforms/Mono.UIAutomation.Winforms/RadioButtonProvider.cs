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
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms
{
	public class RadioButtonProvider : FragmentControlProvider, ISelectionItemProvider
	{
#region Private Fields
		
		private RadioButton radioButton;
		
#endregion
		
#region Constructors
		
		public RadioButtonProvider (RadioButton radioButton) :
			base (radioButton)
		{
			this.radioButton = radioButton;
		
			//TODO: Use SetEventStrategy
			radioButton.CheckedChanged += OnCheckedChanged;
		}
		
#endregion
		
#region Public Methods

		public override void InitializeEvents ()
		{
			base.InitializeEvents ();
			
			// TODO: Add events...
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
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.RadioButton.Id;
			else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
				return null;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "radio button";
			else
				return base.GetPropertyValue (propertyId);
		}
#endregion
		
#region ISelectionItem Members
	
		public void AddToSelection ()
		{
			IEnumerable<RadioButton> otherButtons =
				from Control c in radioButton.Parent.Controls
					where c is RadioButton && c != radioButton
					select (RadioButton)c;
			
			foreach (RadioButton button in otherButtons)
				if (button.Checked)
					// Assuming CanSelectMultiple==false...
					throw new InvalidOperationException ("RadioButton");
			Select ();
		}

		public bool IsSelected {
			get { return radioButton.Checked; }
		}

		public void RemoveFromSelection ()
		{
			// Assuming IsSelectionRequired==true and CanSelectMultiple==false...
			throw new InvalidOperationException ("RadioButton");
		}

		public void Select ()
		{
			radioButton.Checked = true;
		}

		public IRawElementProviderSimple SelectionContainer {
			get {
				IRawElementProviderSimple parentProvider =
					ProviderFactory.FindProvider (radioButton.Parent);
				if (parentProvider != null && parentProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id) != null)
					return parentProvider;
				return null;
			}
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
