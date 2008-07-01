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

using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{

	//TODO: This class should allow navigating 2 ScrollBars and n items.
	public class ComboBoxListBoxProvider : ListProvider
	{

#region Constructor

		public ComboBoxListBoxProvider (ComboBox control, 
		                                ComboBoxProvider provider)
			: base (control)
		{
			combobox_provider = provider;
			
			Navigation = new ComboBoxListBoxItemRootNavigation (this);
		}
 
#endregion
		
#region IProviderBehavior Interface

		public override object GetPropertyValue (int propertyId)
		{
			//TODO: Include: HelpTextProperty, LabeledByProperty, NameProperty
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.List.Id;
			//FIXME: According the documentation this is valid, however you can
			//focus only when control.CanFocus, this doesn't make any sense.
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "list";
			else
				return base.GetPropertyValue (propertyId);
		}

#endregion
		
#region FragmentControlProvider Overrides
		
		public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			return combobox_provider.ElementProviderFromPoint (x, y);
		}

#endregion
		
#region ListProvider Methods
		
		public override int SelectedItemsCount { 
			get { return combobox_provider.SelectedItemsCount; }
		}
		
		public override bool SupportsMulipleSelection { 
			get { return combobox_provider.SupportsMulipleSelection; }
		}
		
		public override int ItemsCount {
			get { return ((ComboBox) combobox_provider.Control).Items.Count; }
		}
		
		public override ListItemProvider[] GetSelectedItemsProviders ()
		{
			return combobox_provider.GetSelectedItemsProviders ();
		}
		
		public override string GetItemName (ListItemProvider item)
		{
			return combobox_provider.GetItemName (item);
		}
		
		public override void SelectItem (ListItemProvider item)
		{
			combobox_provider.SelectItem (item);
		}

		public override void UnselectItem (ListItemProvider item)
		{
			combobox_provider.UnselectItem (item);
		}
		
		public override bool IsItemSelected (ListItemProvider item)
		{
			return combobox_provider.IsItemSelected (item);
		}

#endregion

#region Private Fields
		
		private ComboBoxProvider combobox_provider;
		
#endregion
		
#region Navigation Class

#region ListBoxItem Navigation Class
		
		class ComboBoxListBoxItemRootNavigation : SimpleNavigation
		{
			public ComboBoxListBoxItemRootNavigation (ComboBoxListBoxProvider provider)
				: base (provider)
			{
				this.provider = provider;
			}
			
			public override bool SupportsNavigation {
				get { return !(((ComboBox) provider.ListControl).Items.Count == 0); }
			}
			
			public override IRawElementProviderSimple Provider {
				get { 
					if (first_item_provider == null) {
						first_item_provider = provider.GetItemProvider (0);
						navigation = first_item_provider.Navigation;
						first_item_provider.Navigation = this;
					}

					return first_item_provider;
				}
			}
			
			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				if (direction == NavigateDirection.Parent) {
					return provider;
				} else if (direction == NavigateDirection.NextSibling) {
					if (SupportsNavigation == false)
						return null;
					else
						return navigation.Navigate (direction);
				} else if (direction == NavigateDirection.PreviousSibling)
					return GetPreviousSiblingProvider ();
				else
					return null;
			}
			
			private ComboBoxListBoxProvider provider;
			private ListItemProvider first_item_provider;
			private INavigation navigation;
		}	
		
#endregion
		
#endregion

	}
}
