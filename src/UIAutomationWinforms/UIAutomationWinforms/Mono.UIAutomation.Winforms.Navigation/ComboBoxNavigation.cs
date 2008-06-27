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
	
	public class ComboBoxNavigation : SimpleNavigation
	{

#region Constructor
		
		public ComboBoxNavigation (ComboBoxProvider provider)
			: base (provider)
		{		
			navigation = new NavigationChain ();
			navigation.AddLink (new ComboBoxTextBoxNavigation (provider));
			navigation.AddLink (new ComboBoxListBoxNavigation (provider));
			navigation.AddLink (new ComboBoxButtonNavigation (provider));
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
				return last != null ? (IRawElementProviderFragment) last.Provider : null;
			} else
				return base.Navigate (direction);
		}

#endregion

#region Private members

		private NavigationChain navigation;
		
#endregion
		
#region TextBox Navigation Class
		
		class ComboBoxTextBoxNavigation : SimpleNavigation
		{
			public ComboBoxTextBoxNavigation (ComboBoxProvider provider)
				: base (provider)
			{
				combobox_provider = provider;

				InitializeTextBoxProvider ();
			}
			
			public override IRawElementProviderSimple Provider {
				get { return textbox_provider; }
			}
			
			public override bool SupportsNavigation  {
				get { 
					ComboBox control = (ComboBox) combobox_provider.Control;
					return control.DropDownStyle == ComboBoxStyle.DropDown
						|| control.DropDownStyle == ComboBoxStyle.DropDownList;
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
			
			private void InitializeTextBoxProvider ()
			{
				Type type = combobox_provider.Control.GetType ();
				FieldInfo fieldInfo = type.GetField ("textbox_ctrl",
				                                     BindingFlags.NonPublic
				                                     | BindingFlags.Instance);
				TextBox textbox = (TextBox) fieldInfo.GetValue (combobox_provider.Control);
				textbox_provider = (TextBoxProvider) ProviderFactory.GetProvider (textbox);
				textbox_provider.Navigation = this;
			}

			private ComboBoxProvider combobox_provider;			
			private TextBoxProvider textbox_provider;
		}
		
#endregion

#region ListBox Navigation Class
		
		class ComboBoxListBoxNavigation : SimpleNavigation
		{
			public ComboBoxListBoxNavigation (ComboBoxProvider provider)
				: base (provider)
			{
				InitializeListBoxProvider (provider);
			}

			public override IRawElementProviderSimple Provider {
				get { return listbox_provider; }
			}
			
			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				if (direction == NavigateDirection.Parent)
					return (IRawElementProviderFragment) Provider;
				else if (direction == NavigateDirection.NextSibling)
					return GetNextSiblingProvider ();
				else if (direction == NavigateDirection.PreviousSibling)
					return GetPreviousSiblingProvider ();
				else
					return listbox_navigation.Navigate (direction); 
			}
			
			private void InitializeListBoxProvider (ComboBoxProvider provider)
			{
				Type type = provider.Control.GetType ();

				FieldInfo fieldInfo = type.GetField ("listbox_ctrl",
				                                     BindingFlags.NonPublic
				                                     | BindingFlags.Instance);
				ListBox listbox = (ListBox) fieldInfo.GetValue (provider.Control);
				
				Console.WriteLine ("Is null? "+(listbox == null));
				
				listbox_provider = (ListBoxProvider) ProviderFactory.GetProvider (listbox);
				listbox_provider.Navigation = this;
				listbox_navigation = new ListBoxNavigation (listbox_provider);
			}
			
			private ListBoxProvider listbox_provider;
			private INavigation listbox_navigation;
		}
		
#endregion
		

#region Button Navigation Class
		
		class ComboBoxButtonNavigation : SimpleNavigation
		{
			public ComboBoxButtonNavigation (ComboBoxProvider provider)
				: base (provider)
			{
				InitializeButtonProvider ();
			}
			
			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				if (direction == NavigateDirection.Parent)
					return (IRawElementProviderFragment) Provider;
				else if (direction == NavigateDirection.PreviousSibling)
					return GetPreviousSiblingProvider ();
				else
					return null;
			}
			
			private void InitializeButtonProvider ()
			{
				//TODO: How to get a valid ButtonProvider?
				ButtonProvider provider = new ButtonProvider (null);
				provider.Navigation = this;
			}
		}
		
#endregion

	}
}
