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
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Behaviors
{
	
	public abstract class ListBoxItemProviderBehavior : ProviderBehavior
	{
#region Constructor
		
		protected ListBoxItemProviderBehavior (ListBoxItemProvider provider)
			: base (provider)
		{
			item_provider = provider;
		}

#endregion
		
#region IProviderBehavior Interface

		public override void Connect (Control control)
		{
			//Control's events aren't used to emit any UIA event
		}
		
		public override void Disconnect (Control control)
		{
			//Control's events aren't used to emit any UIA event
		}		
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.ListItem.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "list item";
			else if (propertyId == AutomationElementIdentifiers.AutomationIdProperty.Id)
				return ListBoxControl.GetHashCode () + ListBoxItemProvider.Index; // TODO: Ensure uniqueness
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return ListBoxControl.Items [ListBoxItemProvider.Index].ToString ();
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return ListBoxControl.Focused && ListBoxItemProvider.Index == ListBoxControl.SelectedIndex;
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return ListBoxProvider.GetPropertyValue (propertyId);
			//TODO: AutomationElementIdentifiers.IsOffscreenProperty.Id			
			//TODO: AutomationElementIdentifiers.BoundingRectangleProperty.Id
			//TODO: AutomationElementIdentifiers.ClickablePointProperty.Id
			else
				return base.GetPropertyValue (propertyId);
		}
		
#endregion
		
#region Protected Properties
		
		protected ListBox ListBoxControl {
			get { return item_provider.ListBoxControl; }
		}
		
		protected ListBoxItemProvider ListBoxItemProvider {
			get { return item_provider; }
		}
		
		protected ListBoxProvider ListBoxProvider {
			get { return item_provider.ListBoxProvider; }
		}
		
#endregion
			
#region Private Fields
			
		private ListBoxItemProvider item_provider;
			
#endregion
	}
}
