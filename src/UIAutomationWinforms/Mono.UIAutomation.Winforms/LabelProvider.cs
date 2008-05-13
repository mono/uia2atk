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
	public class LabelProvider : SimpleControlProvider
	{
#region Private Fields
		
		private Label label;
		
#endregion
		
#region Constructors
		
		public LabelProvider (Label label) : base (label)
		{
			this.label = label;
			
			label.TextChanged += OnTextChanged;
		}
		
#endregion
		
#region IRawElementProviderSimple Members
	
		public override object GetPatternProvider (int patternId)
		{
			return null;
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
				return label.Bounds.ToRect ();
			else if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Text.Id;
			else if (propertyId == AutomationElementIdentifiers.IsControlElementProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
				return false;
			else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
				return null;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "text";
			else
				return base.GetPropertyValue (propertyId);
		}

#endregion
		
#region Event Handlers
	
		private void OnTextChanged (object sender, EventArgs e)
		{
			if (AutomationInteropProvider.ClientsAreListening) {
				AutomationEventArgs eventArgs =
					new AutomationEventArgs (TextPatternIdentifiers.TextChangedEvent);
				AutomationInteropProvider.RaiseAutomationEvent (TextPatternIdentifiers.TextChangedEvent, this, eventArgs);
			}
		}
		
#endregion
	}
}
