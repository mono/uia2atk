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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;

namespace Mono.UIAutomation.Winforms.Navigation
{

	//Navigation tree has the following leafs:
	// 1. HScrollBar - ScrollBarProvider (may not be visible)
	// 2. VScrollBar - ScrollBarProvider (may not be visible)
	// 3. ListBoxItem - ListBoxItemProvider (0 to n) 
	internal class ListBoxNavigation : SimpleNavigation
	{

#region	Constructor
		
		public ListBoxNavigation (ListBoxProvider provider)
			: base (provider)
		{
			navigation = new NavigationChain ();
			itemroot_navigation = new ListBoxFirstItemNavigation (provider);
			
			ConnectNavigationEvents ((ListBox) provider.ListControl);
			UpdateNavigation ();
		}
		
#endregion
		
#region Public Methods
	
		public override void FinalizeProvider ()
		{
			base.FinalizeProvider ();
			
			DisconnectNavigationEvents ();
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
		
#region Private Methods
		
		private void ConnectNavigationEvents (ListBox listbox) 
		{
			//We are keeping a reference to internal ListBox fields to use them
			//later to remove/add the navigation class
			hscrollbar = (ScrollBar) Helper.GetPrivateField (typeof (ListBox), 
			                                                 listbox,
			                                                 "hscrollbar");
			vscrollbar = (ScrollBar) Helper.GetPrivateField (typeof (ListBox), 
			                                                 listbox, 
			                                                 "vscrollbar");
			
			ListBoxProvider provider = (ListBoxProvider) Provider;

			provider.HScrollbarNavigationUpdated 
				+= new ScrollbarNavigableEventHandler (OnHScrollbarNavigationUpdated);
			
			provider.VScrollbarNavigationUpdated 
				+= new ScrollbarNavigableEventHandler (OnVScrollbarNavigationUpdated);
		
			if (provider.HasHorizontalScrollbar == true)
				UpdateScrollBarNavigation (ref hscrollbar_navigation,
				                           "hscrollbar", 
				                           true);
			if (provider.HasVerticalScrollbar == true)
				UpdateScrollBarNavigation (ref vscrollbar_navigation,
				                           "vscrollbar", 
				                           true);
		}
		
		private void DisconnectNavigationEvents ()
		{
			((ListBoxProvider) Provider).HScrollbarNavigationUpdated 
				-= new ScrollbarNavigableEventHandler (OnHScrollbarNavigationUpdated);
			
			((ListBoxProvider) Provider).VScrollbarNavigationUpdated 
				-= new ScrollbarNavigableEventHandler (OnVScrollbarNavigationUpdated);
		}
		
		private void OnHScrollbarNavigationUpdated (object container,
		                                            ScrollBar scrollbar,
		                                            bool navigable)
		{		
			if (scrollbar == hscrollbar) {
				if (navigable == true && hscrollbar_navigation == null) {
					UpdateScrollBarNavigation (ref hscrollbar_navigation,
					                           "hscrollbar", 
					                           true);
				} else if (navigable == false && hscrollbar_navigation != null) {
					UpdateScrollBarNavigation (ref hscrollbar_navigation,
					                           "hscrollbar", 
					                           false);
				}
			}
		}
		
		private void OnVScrollbarNavigationUpdated (object container,
		                                            ScrollBar scrollbar,
		                                            bool navigable)
		{
			if (scrollbar == vscrollbar) {
				if (navigable == true && vscrollbar_navigation == null)
					UpdateScrollBarNavigation (ref vscrollbar_navigation,
					                           "vscrollbar", 
					                           true);
				else if (navigable == false && vscrollbar_navigation != null)
					UpdateScrollBarNavigation (ref vscrollbar_navigation,
					                           "vscrollbar", 
					                           false);
			}
		}

		private void UpdateNavigation ()
		{
			//FIXME: Improving missing
			navigation.Clear ();
			if (hscrollbar_navigation != null)
				navigation.AddLink (hscrollbar_navigation);
			if (vscrollbar_navigation != null)
				navigation.AddLink (vscrollbar_navigation);
			navigation.AddLink (itemroot_navigation);
		}
		
		private void UpdateScrollBarNavigation (ref ListBoxScrollBarNavigation navigation,
		                                        string fieldName, bool initialize)
		{
			if (initialize == true)
				navigation = new ListBoxScrollBarNavigation ((ListBoxProvider) Provider, 
				                                             fieldName);
			else {
				navigation.FinalizeProvider ();
				navigation = null;
			}
			UpdateNavigation ();
		}
		
#endregion

#region Private Fields

		private ScrollBar hscrollbar;
		private ScrollBar vscrollbar;
		private ListBoxFirstItemNavigation itemroot_navigation;
		private ListBoxScrollBarNavigation hscrollbar_navigation;
		private ListBoxScrollBarNavigation vscrollbar_navigation;
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
			
			public override void FinalizeProvider ()
			{
				if (scrollbar_provider != null)
					scrollbar_provider.Terminate ();
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
				//TODO: I'm sure we'll need a custom ListBoxScrollBarProvider 
				//instead of using the ScrollBar
				scrollbar = (ScrollBar) Helper.GetPrivateField (typeof (ListBox), 
				                                                provider.Control, 
				                                                field_name);
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
		
		class ListBoxFirstItemNavigation : SimpleNavigation
		{
			public ListBoxFirstItemNavigation (ListBoxProvider provider)
				: base (provider)
			{
				this.provider = provider;
				
				InitializeItemChangeEvents ();
			}
			
			public override IRawElementProviderSimple Provider {
				get { return GetListItemProvider (); }
			}
			
			public override void FinalizeProvider ()
			{
				if (first_item_provider != null) 
					first_item_provider.Terminate ();
				
				FinalizeItemChangeEvents ();
			}			
			
			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				if (direction == NavigateDirection.Parent)
					return provider;
				else if (direction == NavigateDirection.NextSibling)
					return first_item_provider == null ? null : navigation.Navigate (direction);
				else if (direction == NavigateDirection.PreviousSibling)
					return GetPreviousSiblingProvider ();
				else
					return null;
			}
			
			private ListItemProvider GetListItemProvider ()
			{
				if (first_item_provider == null) {
					first_item_provider = provider.GetItemProvider (0);
					if (first_item_provider != null) {
						navigation = first_item_provider.Navigation;
						first_item_provider.Navigation = this;
					}
				}

				return first_item_provider;
			}
			
			private void InitializeItemChangeEvents ()
			{
				provider.ChildRemoved += new StructureChangeEventHandler (OnChildRemoved);
			}
			
			private void FinalizeItemChangeEvents ()
			{
				provider.ChildRemoved -= new StructureChangeEventHandler (OnChildRemoved);
			}
			
			private void OnChildRemoved (object sender, ListItemProvider item, int index)
			{			
				if (index == 0) {
					first_item_provider = null;
					navigation = null;
				}
			}

			private INavigation navigation;			
			private ListBoxProvider provider;
			private ListItemProvider first_item_provider;
		}
		
#endregion
	}
}
