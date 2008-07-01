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
using System.Windows.Forms;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.Navigation
{

	//Navigation tree has the following leafs:
	// 1. HScrollBar - ScrollBarProvider (may not be visible)
	// 2. VScrollBar - ScrollBarProvider (may not be visible)
	// 3. ListBoxItem - ListBoxItemProvider (0 to n) 
	internal class ListBoxNavigation : SimpleNavigation
	{

#region	 Constructor
		
		public ListBoxNavigation (ListBoxProvider provider)
			: base (provider)
		{
			navigation = new NavigationChain ();		
			navigation.AddLink (new ListBoxScrollBarNavigation (provider, 
			                                                    "hscrollbar"));
			navigation.AddLink (new ListBoxScrollBarNavigation (provider, 
			                                                    "vscrollbar"));
			itemroot_navigation = new ListBoxItemRootNavigation (provider);
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
					ListBoxProvider provider = (ListBoxProvider) Provider;
					ListBox listbox = (ListBox) provider.Control;
					
					if (listbox.Items.Count > 1)
						return provider.GetItemProvider (listbox.Items.Count - 1);
					else
						return (IRawElementProviderFragment) last.Provider;
				} else
					return last != null ? (IRawElementProviderFragment) last.Provider : null;
			} else
				return base.Navigate (direction);
		}

#endregion

#region Private Fields

		private ListBoxItemRootNavigation itemroot_navigation;
		private NavigationChain navigation;
			
#endregion
		
#region ScrollBar Navigation Class
		
		class ListBoxScrollBarNavigation : SimpleNavigation
		{
			public ListBoxScrollBarNavigation (ListBoxProvider provider, 
			                                   string field_name)
				: base (provider)
			{
				this.provider = provider;
				this.field_name = field_name;
			}
			
			public override IRawElementProviderSimple Provider {
				get { 
					if (scrollbar_provider == null)
						InitializeScrollBarProvider (provider, field_name);
					
					return scrollbar_provider; 
				}
			}
			
			public override bool SupportsNavigation  {
				get {
					if (scrollbar_provider == null)
						InitializeScrollBarProvider (provider, field_name);
					
					ListBox listbox = (ListBox) provider.Control;
					if (listbox.ScrollAlwaysVisible 
					    && field_name.Equals ("vscrollbar"))
						return scrollbar.Enabled;
					else
						return scrollbar.Visible;
				}
			}
			
			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				if (direction == NavigateDirection.Parent)
					return provider;
				else if (direction == NavigateDirection.NextSibling)
					return GetNextSiblingProvider ();
				else if (direction == NavigateDirection.PreviousSibling)
					return GetPreviousSiblingProvider ();
				else
					return scrollbar_navigation.Navigate (direction); 
			}
			
			private void InitializeScrollBarProvider (ListBoxProvider provider, 
			                                          string field_name)
			{
				FieldInfo fieldInfo = typeof (ListBox).GetField (field_name,
				                                                  BindingFlags.NonPublic
				                                                  | BindingFlags.Instance);
				scrollbar = (ScrollBar) fieldInfo.GetValue (provider.Control);
				scrollbar_provider = (ScrollBarProvider) ProviderFactory.GetProvider (scrollbar);
				//We need this navigation to use it later.
				scrollbar_navigation = scrollbar_provider.Navigation;
				scrollbar_provider.Navigation = this;
			}

			private string field_name;
			private ListBoxProvider provider;
			private ScrollBar scrollbar;
			private ScrollBarProvider scrollbar_provider;
			private INavigation scrollbar_navigation;
		}
		
#endregion

#region ListBoxItem Navigation Class
		
		class ListBoxItemRootNavigation : SimpleNavigation
		{
			public ListBoxItemRootNavigation (ListBoxProvider provider)
				: base (provider)
			{
				this.provider = provider;
			}
			
			public override bool SupportsNavigation  {
				get { return !(((ListBox) provider.Control).Items.Count == 0); }
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
			
			private ListBoxProvider provider;
			private ListItemProvider first_item_provider;
			private INavigation navigation;
		}	
		
#endregion
	}
}
