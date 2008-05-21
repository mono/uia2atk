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
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms
{

	// TODO: Supposly this control should support Edit and Document control 
	// types (according to http://www.mono-project.com/Accessibility:_Control_Status)
	// however right now only Edit control type is being implemented.
	public class TextBoxProvider : SimpleControlProvider
	{
#region Private section
		
		private TextBox textbox;
	
#endregion
	
#region Constructors

		public TextBoxProvider (TextBox textbox) : base (textbox)
		{
			this.textbox = textbox;
		}

#endregion
		
#region Protected Methods

		protected override void InitializeEvents ()
		{
			base.InitializeEvents ();

			// NameProperty. uses control.Name to emit changes.
			SetEvent (EventStrategyType.NameProperty, null);
			SetEvent (EventStrategyType.TextChangedEvent, 
			          new DefaultTextChangedEvent (this, control));
			SetEvent (EventStrategyType.HasKeyboardFocusProperty, 
			          new TextBoxHasKeyBoardFocusPropertyEvent (this, textbox));
			
			// TODO: InvalidatedEvent
			// TODO: TextSelectionChangedEvent: using textbox.SelectionLength != 0?	
			// TODO: NameProperty property-changed event.
			// TODO: ValuePatternIdentifiers.ValueProperty property-changed event.
			// TODO: RangeValuePatternIdentifiers.ValueProperty property-changed event.
		}

#endregion
	
#region IRawElementProviderSimple Members
	
		public override object GetPatternProvider (int patternId)
		{
			return null;
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			// Edit Control Type properties
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Edit.Id;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return control.Name;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "edit";
			else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id) {
				// TODO: We are using TabIndex to evaluate whether the previous control
				// is Label (that way we know if this label is associated to the TextBox.
				// Right?)
				if (textbox.Parent != null) {
					Label associatedLabel = textbox.Parent.GetNextControl (textbox, true) as Label;
					if (associatedLabel != null)
						return associatedLabel;
				}
				return null;
			} else if (propertyId == AutomationElementIdentifiers.IsPasswordProperty.Id)
				return (textbox.UseSystemPasswordChar || (int) textbox.PasswordChar != 0);
			else
				return base.GetPropertyValue (propertyId);
		}

#endregion

	}

}
