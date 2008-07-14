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
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using System.Windows;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{

	public class ComboBoxProvider : ListProvider
	{

#region Constructor
		
		public ComboBoxProvider (ComboBox combobox) : base (combobox)
		{
			combobox.DropDownStyleChanged += delegate (object obj, 
			                                           EventArgs args) {
				UpdateBehaviors (combobox);
			};
			
			UpdateBehaviors (combobox);
		}
		
#endregion
		
#region Public Properties

		public override INavigation Navigation {
			get { 
				if (navigation == null)
					navigation = new ComboBoxNavigation (this);

				return navigation;
			}
		}
		
#endregion
		
#region IProviderBehavior Interface

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.ComboBox.Id;
			//FIXME: According the documentation this is valid, however you can
			//focus only when control.CanFocus, this doesn't make any sense.
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "combo box";
			else
				return base.GetPropertyValue (propertyId);
		}
		
#endregion		

#region FragmentRootControlProvider Overrides
		
		public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			throw new NotImplementedException ();
		}

#endregion
		
#region ListProvider Overrides
		
		public override bool SupportsMultipleSelection { 
			get { return false; } 
		}
		
		public override int SelectedItemsCount { 
			get { return ListControl.SelectedIndex == -1 ? 0 : 1; }
		}
		
		public override int ItemsCount {
			get { return ((ComboBox) ListControl).Items.Count; }
		}
		
		public override ListItemProvider[] GetSelectedItemsProviders ()
		{
			if (((ComboBox) ListControl).SelectedIndex == -1)
				return null;
			else
				return new ListItemProvider [] { (ListItemProvider) GetFocus () };
		}

		public override string GetItemName (ListItemProvider item)
		{
			if (ContainsItem (item) == true)
				return ((ComboBox) ListControl).Items [item.Index].ToString ();
			else
				return string.Empty;
		}
		
		public override void SelectItem (ListItemProvider item)
		{
			if (ContainsItem (item) == true)
				ListControl.SelectedIndex = item.Index;
		}

		public override void UnselectItem (ListItemProvider item)
		{
		}
		
		public override bool IsItemSelected (ListItemProvider item)
		{
			return ContainsItem (item) == false ? false : item.Index == ListControl.SelectedIndex;
		}
		
#endregion
		
#region Private Methods
		
		private void UpdateBehaviors (ComboBox combobox) {
			if (combobox.DropDownStyle == ComboBoxStyle.Simple) {
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern, 
				             null);
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new ComboBoxValueProviderBehavior (this));
			} else if (combobox.DropDownStyle == ComboBoxStyle.DropDown) {
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
				             new ComboBoxExpandCollapseProviderBehavior (this));
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new ComboBoxValueProviderBehavior (this));
			} else if (combobox.DropDownStyle == ComboBoxStyle.DropDownList) {
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
				             new ComboBoxExpandCollapseProviderBehavior (this));
				SetBehavior (ValuePatternIdentifiers.Pattern, 
				             null);
			} 
		}
		
#endregion

	}
}
