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

	public class ListBoxNavigation : SimpleNavigation
	{

#region	 Constructor
		
		public ListBoxNavigation (ListBoxProvider provider)
			: base (provider)
		{
			listbox_provider = provider;
			
			navigation = new NavigationChain ();
			navigation.AddLink (new ListBoxScrollBar (listbox_provider, "hscrollbar"));
			navigation.AddLink (new ListBoxScrollBar (listbox_provider, "vscrollbar"));
			//navigation.AddLink (new ListBoxItemNavigation (listbox_provider));
		}
		
#endregion
		
#region Private Fields
		
		private ListBoxProvider listbox_provider;
		private NavigationChain navigation;
			
#endregion
		
#region ScrollBar Navigation Class
		
		class ListBoxScrollBar : SimpleNavigation
		{
			public ListBoxScrollBar (ListBoxProvider provider,
			                                 string field_name)
				: base (provider)
			{
				listbox_provider = provider;

				InitializeScrollBar (listbox_provider, field_name);
			}
			
			public override IRawElementProviderSimple Provider {
				get { return listbox_provider; }
			}
			
			public override bool SupportsNavigation  {
				get { 
					return true;
//					ComboBox control = (ComboBox) combobox_provider.Control;
//					return control.DropDownStyle == ComboBoxStyle.DropDown
//						|| control.DropDownStyle == ComboBoxStyle.DropDownList;
				}
			}
			
			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				if (direction == NavigateDirection.Parent)
					return (IRawElementProviderFragment) Provider;
				else if (direction == NavigateDirection.NextSibling)
					return GetNextSiblingProvider ();
				else
					return null; 
			}
			
			private void InitializeScrollBar (ListBoxProvider provider, 
			                                  string field_name)
			{
//				FieldInfo fieldInfo = typeof (ComboBox).GetField ("field_name",
//				                                                  BindingFlags.NonPublic
//				                                                  | BindingFlags.Instance);
//				ScrollBar scrollbar = (ScrollBar) fieldInfo.GetValue (Provider);
//				textbox_provider = (ScrollBarProvider) ProviderFactory.GetProvider (textbox);
//				textbox_provider.Navigation = this;
			}

			private ListBoxProvider listbox_provider;
			private ScrollBarProvider scrollbar_provider;
		}
		
#endregion

	}
}
