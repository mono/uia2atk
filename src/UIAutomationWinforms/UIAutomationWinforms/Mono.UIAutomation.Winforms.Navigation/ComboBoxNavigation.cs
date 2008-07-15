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
	internal class ComboBoxNavigation : SimpleNavigation
	{

#region Constructor
		
		public ComboBoxNavigation (ComboBoxProvider provider)
			: base (provider)
		{
			this.provider = provider;
			combobox_control = (ComboBox) provider.Control;

			chain = new NavigationChain ();
			listbox_navigation = new ComboBoxListBoxNavigation (chain, provider);
			chain.AddFirst (listbox_navigation);

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
		
#region INavigable Interface
		
		public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
		{
			if (direction == NavigateDirection.FirstChild)
				return (IRawElementProviderFragment) chain.First.Value.Provider;
			else if (direction == NavigateDirection.LastChild)
				return (IRawElementProviderFragment) chain.Last.Value.Provider;
			else
				return base.Navigate (direction);
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
					chain.Remove (button_navigation);
					button_navigation = null;
				}
				if (textbox_navigation == null) {
					textbox_navigation = new ComboBoxTextBoxNavigation (chain,
					                                                    provider);
					chain.AddFirst (textbox_navigation);
				}
			} else if (combobox_control.DropDownStyle == ComboBoxStyle.DropDown) {
				if (button_navigation == null) {
					button_navigation = new ComboBoxButtonNavigation (chain,
					                                                  provider);
					chain.AddLast (button_navigation);
				}
				if (textbox_navigation == null) {
					textbox_navigation = new ComboBoxTextBoxNavigation (chain,
					                                                    provider);
					chain.AddFirst (textbox_navigation);
				}
			} else if (combobox_control.DropDownStyle == ComboBoxStyle.DropDownList) {
				if (button_navigation == null) {
					button_navigation = new ComboBoxButtonNavigation (chain,
					                                                  provider);
					chain.AddLast (button_navigation);
				}
				if (textbox_navigation != null) {
					chain.Remove (textbox_navigation);
					textbox_navigation = null;
				}
			}

		}
		
#endregion

#region Private members

		private NavigationChain chain;
		private ComboBox combobox_control;
		private ComboBoxProvider provider;
		private ComboBoxButtonNavigation button_navigation;
		private ComboBoxListBoxNavigation listbox_navigation;
		private ComboBoxTextBoxNavigation textbox_navigation;
		
#endregion
		
#region TextBox Navigation Class
		
		class ComboBoxTextBoxNavigation : SimpleNavigation 
		{
			public ComboBoxTextBoxNavigation (NavigationChain chain,
			                                  ComboBoxProvider provider)
				: base (provider)
			{
				this.chain = chain;
				this.provider = provider;
			}
			
			public override IRawElementProviderSimple Provider {
				get { return GetTextBoxProvider (); }
			}
			
			public override void Terminate ()
			{
				//Nothing to Terminate (we are using ListBoxProvider children providers).
			}

			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				if (direction == NavigateDirection.Parent)
					return provider;
				else if (direction == NavigateDirection.PreviousSibling)
					return GetPreviousSiblingProvider (chain);
				else if (direction == NavigateDirection.NextSibling)
					return GetNextSiblingProvider (chain);
				else
					return null;
			}
			
			private TextBoxProvider GetTextBoxProvider ()
			{
				if (textbox_provider == null) {
					textbox_provider = (TextBoxProvider) provider.GetChildTextBoxProvider ();
					if (textbox_provider != null)
						textbox_provider.Navigation = this;
				}

				return textbox_provider;
			}
			
			private NavigationChain chain;
			private ComboBoxProvider provider;
			private TextBoxProvider textbox_provider;
		}
		
#endregion
		
#region Button Navigation Class
		
		class ComboBoxButtonNavigation : SimpleNavigation
		{
			public ComboBoxButtonNavigation  (NavigationChain chain,
			                                  ComboBoxProvider provider)
				: base (provider)
			{
				this.chain = chain;
				this.provider = provider;
			}
			
			public override IRawElementProviderSimple Provider {
				get { return GetButtonProvider (); }
			}
			
			public override void Terminate ()
			{
				//Nothing to Terminate (we are using ListBoxProvider children providers).
			}

			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				if (direction == NavigateDirection.Parent)
					return provider;
				else if (direction == NavigateDirection.PreviousSibling)
					return GetPreviousSiblingProvider (chain);
				else if (direction == NavigateDirection.NextSibling)
					return GetNextSiblingProvider (chain);
				else
					return null;
			}
			
			private ComboBoxProvider.ComboBoxButtonProvider GetButtonProvider ()
			{
				if (button_provider == null) {
					button_provider = (ComboBoxProvider.ComboBoxButtonProvider) provider.GetChildButtonProvider ();
					if (button_provider != null)
						button_provider.Navigation = this;
				}

				return button_provider;
			}
			
			private NavigationChain chain;
			private ComboBoxProvider provider;
			private ComboBoxProvider.ComboBoxButtonProvider button_provider;
		}

#endregion

#region ListBox Navigation Class
		
		//TODO: This class should allow navigating 2 ScrollBars and n items.		
		class ComboBoxListBoxNavigation : SimpleNavigation
		{
			public ComboBoxListBoxNavigation (NavigationChain chain,
			                                  ComboBoxProvider provider)
				: base (provider)
			{
				this.chain = chain;
				this.provider = provider;
			}
			
			public override IRawElementProviderSimple Provider {
				get { return GetListBoxProvider (); }
			}
			
			public override void Terminate ()
			{
				//Nothing to Terminate (we are using ListBoxProvider children providers).
			}
			
			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				//TODO: We aware we are only returning the items, we are missing the scrollbars!
				if (direction == NavigateDirection.Parent)
					return provider;
				else if (direction == NavigateDirection.FirstChild)
					return GetListBoxProvider ().GetItemProvider (0);
				else if (direction == NavigateDirection.LastChild)
					return GetListBoxProvider ().GetItemProvider (GetListBoxProvider ().ItemsCount - 1);
				else if (direction == NavigateDirection.NextSibling)
					return GetNextSiblingProvider (chain);
				else if (direction == NavigateDirection.PreviousSibling)
					return GetPreviousSiblingProvider (chain);
				else
					return null;
			}
			
			private ComboBoxProvider.ComboBoxListBoxProvider GetListBoxProvider ()
			{
				if (listbox_provider == null) {
					listbox_provider = (ComboBoxProvider.ComboBoxListBoxProvider) provider.GetChildListBoxProvider ();
					listbox_provider.Navigation = this;
				}

				return listbox_provider; 
			}

			private NavigationChain chain;
			private ComboBoxProvider provider;
			private ComboBoxProvider.ComboBoxListBoxProvider listbox_provider;
		}	
		
#endregion

	}
}
