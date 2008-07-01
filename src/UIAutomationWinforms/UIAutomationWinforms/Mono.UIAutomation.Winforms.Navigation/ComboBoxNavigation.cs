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
using System.Reflection;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms;

namespace Mono.UIAutomation.Winforms.Navigation
{
	
	//Navigation tree has the following leafs:
	// 1. TextBox - TextBox Provider (may not visible)
	// 2. ComboBoxItem - ComboBoxItemProvider (0 to n) 
	// 3. Button - ButtonProvider (may not be visible)
	internal class ComboBoxNavigation : SimpleNavigation
	{

#region Constructor
		
		public ComboBoxNavigation (ComboBoxProvider provider)
			: base (provider)
		{		
			navigation = new NavigationChain ();
			//TODO: Add Edit provider and Button provider
			itemroot_navigation = new ComboBoxListBoxNavigation (provider);
			navigation.AddLink (itemroot_navigation);
		}

#endregion
		
#region INavigable Interface
		
		public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
		{
			if (direction == NavigateDirection.FirstChild) {
				INavigation first = navigation.GetFirstLink ();
				return first != null ? (IRawElementProviderFragment) first.Provider : null;
			} else if (direction == NavigateDirection.LastChild) {
				INavigation last = navigation.GetLastLink ();
				if (last == itemroot_navigation) {
					ComboBoxProvider provider = (ComboBoxProvider) Provider;
					ComboBox combobox = (ComboBox) provider.Control;
					
					if (combobox.Items.Count > 1)
						return provider.GetItemProvider (combobox.Items.Count - 1);
					else
						return (IRawElementProviderFragment) last.Provider;
				} else
					return last != null ? (IRawElementProviderFragment) last.Provider : null;
			} else
				return base.Navigate (direction);
		}

#endregion

#region Private members

		private NavigationChain navigation;
		private ComboBoxListBoxNavigation itemroot_navigation;
		
#endregion

#region ListBox Navigation Class
		
		class ComboBoxListBoxNavigation : SimpleNavigation
		{
			public ComboBoxListBoxNavigation (ComboBoxProvider provider)
				: base (provider)
			{
				this.provider = provider;
			}
			
			public override bool SupportsNavigation  {
				get { return !(((ComboBox) provider.Control).Items.Count == 0); }
			}
			
			public override IRawElementProviderSimple Provider {
				get { 
					if (first_item_provider == null) {
						first_item_provider = provider.GetItemProvider (0);
						listbox_navigation = first_item_provider.Navigation;
						first_item_provider.Navigation = this;
					}

					return first_item_provider;
				}
			}

			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				//TODO: This MUST BE REFACTORED once the support to StructureEvent is enabled
				if (direction == NavigateDirection.Parent)
					return provider;
				//TODO: This must be changed to: GetNextSiblingProvider and 
				//GetPreviousSiblingProvider
				else if (direction == NavigateDirection.NextSibling
				         || direction == NavigateDirection.PreviousSibling)
					return listbox_navigation.Navigate (direction); 
				else
					return base.Navigate (direction);
			}

			private ComboBoxProvider provider;
			private ListItemProvider first_item_provider;
			private INavigation listbox_navigation;
		}
		
#endregion

	}
}
