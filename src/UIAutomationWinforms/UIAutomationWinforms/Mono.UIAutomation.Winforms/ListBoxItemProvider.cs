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
using Mono.UIAutomation.Winforms.Behaviors;

namespace Mono.UIAutomation.Winforms
{

	public class ListBoxItemProvider : FragmentControlProvider
	{

#region Constructor
		
		public ListBoxItemProvider (ListBoxProvider provider, ListBox control) 
			: this (provider, control, control.SelectedIndex)
		{
		}

		public ListBoxItemProvider (ListBoxProvider provider, ListBox control, 
		                            int index) : base (control)
		{
			listbox_provider = provider;
			listbox_control = control;
			this.index = index;
			
			SetBehavior (SelectionItemPatternIdentifiers.Pattern, 
			             new ListBoxItemSelectionProviderBehavior (this));
		}

#endregion
		
#region Public properties

		public int Index {
			get { return index; }
		}
		
		public ListBox ListBoxControl {
			get { return listbox_control; }
		}

		public ListBoxProvider ListBoxProvider {
			get { return listbox_provider; }
		}
		
#endregion
		
#region Public Methods
		
		public virtual object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.AutomationIdProperty.Id)
				return Control.GetHashCode () + Index; // TODO: Ensure uniqueness
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return ListBoxControl.Items [Index].ToString ();
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return ListBoxControl.Focused && Index == ListBoxControl.SelectedIndex;
			//TODO: AutomationElementIdentifiers.IsOffscreenProperty.Id			
			//TODO: AutomationElementIdentifiers.BoundingRectangleProperty.Id
			//TODO: AutomationElementIdentifiers.ClickablePointProperty.Id
			else
				return base.GetPropertyValue (propertyId);
		}		
		
#endregion

#region IRawElementProviderFragment Interface 

		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { return ListBoxProvider; }			
		}

		public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
		{
			if (direction == NavigateDirection.Parent)
				return FragmentRoot;
			else if (direction == NavigateDirection.NextSibling)
				return ListBoxProvider.GetListBoxItem (index + 1);
			else if (direction == NavigateDirection.PreviousSibling)
				return ListBoxProvider.GetListBoxItem (index - 1);
			else
				return null;
		}

#endregion

#region Private fields
		
		private ListBox listbox_control;
		private ListBoxProvider listbox_provider;
		private int index;		
		
#endregion

	}
}
