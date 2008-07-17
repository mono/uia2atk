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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms;

namespace Mono.UIAutomation.Winforms.Navigation
{
	
	//Navigation tree has the following leafs:
	// 1. TextBox - TextBox Provider (may not visible)
	// 2. ComboBoxItem - ComboBoxItemProvider (0 to n) 
	// 3. Button - ButtonProvider (may not be visible)
	internal class ComboBoxNavigation : ParentNavigation
	{

#region Constructor
		
		public ComboBoxNavigation (ComboBoxProvider provider)
			: base (provider)
		{
			this.provider = provider;
			combobox_control = (ComboBox) provider.Control;

			listbox_navigation = new ComboBoxListBoxNavigation (provider, Chain);
			Chain.AddFirst (listbox_navigation);

			combobox_control.DropDownStyleChanged += new EventHandler (OnDropDownStyleChanged);
			UpdateNavigation ();
		}

#endregion
		
#region Public Methods
		
		public override void Terminate ()
		{
			base.Terminate (); 
			
			combobox_control.DropDownStyleChanged -= new EventHandler (OnDropDownStyleChanged);
		}
		
#endregion
		
#region Private Methods

		private void OnDropDownStyleChanged (object sender, EventArgs args)
		{
			UpdateNavigation ();
		}		
		
		private void UpdateNavigation ()
		{
			if (combobox_control.DropDownStyle == ComboBoxStyle.Simple) {
				if (button_navigation != null) {
					Chain.Remove (button_navigation);
					button_navigation = null;
				}
				if (textbox_navigation == null) {
					textbox_navigation = new ComboBoxTextBoxNavigation (provider,
					                                                    Chain);
					Chain.AddFirst (textbox_navigation);
				}
			} else if (combobox_control.DropDownStyle == ComboBoxStyle.DropDown) {
				if (button_navigation == null) {
					button_navigation = new ComboBoxButtonNavigation (provider,
					                                                  Chain);
					Chain.AddLast (button_navigation);
				}
				if (textbox_navigation == null) {
					textbox_navigation = new ComboBoxTextBoxNavigation (provider,
					                                                    Chain);
					Chain.AddFirst (textbox_navigation);
				}
			} else if (combobox_control.DropDownStyle == ComboBoxStyle.DropDownList) {
				if (button_navigation == null) {
					button_navigation = new ComboBoxButtonNavigation (provider,
					                                                  Chain);
					Chain.AddLast (button_navigation);
				}
				if (textbox_navigation != null) {
					Chain.Remove (textbox_navigation);
					textbox_navigation = null;
				}
			}

		}
		
#endregion

#region Private members

		private ComboBox combobox_control;
		private ComboBoxProvider provider;
		private ComboBoxButtonNavigation button_navigation;
		private ComboBoxListBoxNavigation listbox_navigation;
		private ComboBoxTextBoxNavigation textbox_navigation;
		
#endregion
		
#region Internal Class: TextBox Navigation
		
		class ComboBoxTextBoxNavigation : ChildNavigation 
		{
			public ComboBoxTextBoxNavigation (ComboBoxProvider provider,
			                                  NavigationChain chain)
				: base (provider, chain)
			{
			}
			
			protected override IRawElementProviderSimple GetChildProvider ()
			{
				if (textbox_provider == null) {
					textbox_provider = ((ComboBoxProvider) ParentProvider).GetChildTextBoxProvider ();
					if (textbox_provider != null)
						textbox_provider.Navigation = this;
				}

				return textbox_provider;
			}
			
			private FragmentControlProvider textbox_provider;
		}
		
#endregion
		
#region Internal Class: Button Navigation
		
		class ComboBoxButtonNavigation : ChildNavigation
		{
			public ComboBoxButtonNavigation (ComboBoxProvider provider,
			                                 NavigationChain chain)
				: base (provider, chain)
			{
			}
			
			protected override IRawElementProviderSimple GetChildProvider ()
			{
				if (button_provider == null) {
					button_provider = ((ComboBoxProvider) ParentProvider).GetChildButtonProvider ();
					if (button_provider != null)
						button_provider.Navigation = this;
				}

				return button_provider;
			}

			private FragmentControlProvider button_provider;
		}

#endregion

#region Internal Class: ListBox Navigation
		
		//TODO: This class should allow navigating 2 ScrollBars and n items.		
		class ComboBoxListBoxNavigation : ChildNavigation
		{
			public ComboBoxListBoxNavigation (ComboBoxProvider provider,
			                                  NavigationChain chain)
				: base (provider, chain)
			{
			}
			
			protected override IRawElementProviderSimple GetChildProvider ()
			{
				return GetListBoxProvider ();
			}
			
			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				//TODO: We aware we are only returning the items, we are missing the scrollbars!
				if (direction == NavigateDirection.Parent)
					return ParentProvider;
				else if (direction == NavigateDirection.FirstChild)
					return GetListBoxProvider ().GetItemProvider (0);
				else if (direction == NavigateDirection.LastChild)
					return GetListBoxProvider ().GetItemProvider (GetListBoxProvider ().ItemsCount - 1);
				else
					return base.Navigate (direction);
			}
			
			private ComboBoxProvider.ComboBoxListBoxProvider GetListBoxProvider ()
			{
				if (listbox_provider == null) {
					listbox_provider = ((ComboBoxProvider) ParentProvider).GetChildListBoxProvider ();
					listbox_provider.Navigation = this;
				}

				return (ComboBoxProvider.ComboBoxListBoxProvider) listbox_provider;
			}

			private FragmentRootControlProvider listbox_provider;
		}	
		
#endregion

	}
}
