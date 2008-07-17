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
	internal class ListBoxNavigation : ParentNavigation
	{

#region	Constructor
		
		public ListBoxNavigation (ListBoxProvider provider)
			: base (provider)
		{
			this.provider = provider;

			itemroot_navigation = new ListBoxFirstItemNavigation (provider, Chain);
			ConnectNavigationEvents ((ListBox) provider.ListControl);

			//TODO: Should we use ChildAdded and ChildRemoved to update items navigation?
			Chain.AddLast (itemroot_navigation);
		}
		
#endregion
		
#region ParentNavigation: Specializations
	
		public override void Terminate ()
		{
			base.Terminate ();
			
			DisconnectNavigationEvents ();
		}
		
		public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
		{
			if (direction == NavigateDirection.LastChild)
				return provider.GetItemProvider (provider.ItemsCount - 1);
			else
				return base.Navigate (direction);
		}

#endregion
		
#region Private Methods
		
		private void ConnectNavigationEvents (ListBox listbox) 
		{
			//Keeping ListBox's scrollbar references to later remove/add the navigation class
			hscrollbar = (ScrollBar) Helper.GetPrivateField (typeof (ListBox), 
			                                                 listbox,
			                                                 hscrollbar_field_name);
			vscrollbar = (ScrollBar) Helper.GetPrivateField (typeof (ListBox), 
			                                                 listbox, 
			                                                 vscrollbar_field_name);
			//ScrollBars
			provider.HScrollbarNavigationUpdated 
				+= new ScrollbarNavigableEventHandler (OnHScrollbarNavigationUpdated);
			
			provider.VScrollbarNavigationUpdated 
				+= new ScrollbarNavigableEventHandler (OnVScrollbarNavigationUpdated);
		
			if (provider.HasHorizontalScrollbar == true)
				UpdateScrollBarNavigation (ref hscrollbar_navigation,
				                           hscrollbar_field_name, 
				                           true);
			if (provider.HasVerticalScrollbar == true)
				UpdateScrollBarNavigation (ref vscrollbar_navigation,
				                           vscrollbar_field_name, 
				                           true);
		}
		
		private void DisconnectNavigationEvents ()
		{
			//ScrollBars
			provider.HScrollbarNavigationUpdated 
				-= new ScrollbarNavigableEventHandler (OnHScrollbarNavigationUpdated);
			provider.VScrollbarNavigationUpdated 
				-= new ScrollbarNavigableEventHandler (OnVScrollbarNavigationUpdated);			
		}
		
		private void OnHScrollbarNavigationUpdated (object container,
		                                            ScrollBar scrollbar,
		                                            bool navigable)
		{		
			if (navigable == true && hscrollbar_navigation == null) {
				UpdateScrollBarNavigation (ref hscrollbar_navigation,
				                           hscrollbar_field_name, 
				                           true);
			} else if (navigable == false && hscrollbar_navigation != null) {
				UpdateScrollBarNavigation (ref hscrollbar_navigation,
				                           hscrollbar_field_name, 
				                           false);
			}
		}

		private void OnVScrollbarNavigationUpdated (object container,
		                                            ScrollBar scrollbar,
		                                            bool navigable)
		{
			if (navigable == true && vscrollbar_navigation == null)
				UpdateScrollBarNavigation (ref vscrollbar_navigation,
				                           vscrollbar_field_name, 
				                           true);
			else if (navigable == false && vscrollbar_navigation != null)
				UpdateScrollBarNavigation (ref vscrollbar_navigation,
				                           vscrollbar_field_name, 
				                           false);
		}

		private void UpdateScrollBarNavigation (ref ListBoxScrollBarNavigation navigation,
		                                        string fieldName, 
		                                        bool initialize)
		{
			if (initialize == true) {
				navigation = new ListBoxScrollBarNavigation (Provider,
				                                             Chain, 
				                                             fieldName);
				//TODO: Makes sense keeping always same order: Hor, Vert, NItems?
				if (fieldName == hscrollbar_field_name)
					Chain.AddFirst (navigation);
				else {
					if (hscrollbar_navigation != null)
						Chain.AddAfter (Chain.Find (hscrollbar_navigation), navigation);
					else
						Chain.AddFirst (navigation);
				}
			} else {
				Chain.Remove (navigation);
				navigation.Terminate ();
				navigation = null;
			}
		}
		
#endregion

#region Private Fields

		private ScrollBar hscrollbar;
		private ScrollBar vscrollbar;
		private ListBoxProvider provider;
		private ListBoxFirstItemNavigation itemroot_navigation;
		private ListBoxScrollBarNavigation hscrollbar_navigation;
		private ListBoxScrollBarNavigation vscrollbar_navigation;
		private const string vscrollbar_field_name = "vscrollbar";
		private const string hscrollbar_field_name = "hscrollbar";
			
#endregion

#region Internal Class: ScrollBar Navigation
		
		class ListBoxScrollBarNavigation : ChildNavigation
		{
			public ListBoxScrollBarNavigation (IRawElementProviderSimple provider,
			                                   NavigationChain chain,
			                                   string field_name)
				: base (provider, chain)
			{
				this.field_name = field_name;
			}
			
			protected override IRawElementProviderSimple GetChildProvider ()
			{
				if (scrollbar_provider == null) {
					scrollbar_provider = ((ListBoxProvider) ParentProvider).GetChildScrollBarProvider (GetOrientationFromString ());
					//We need this navigation to use it later.
					scrollbar_navigation = scrollbar_provider.Navigation;
					if (scrollbar_provider != null)
						scrollbar_provider.Navigation = this;
				}
				
				return scrollbar_provider; 
			}
			
			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				if (direction == NavigateDirection.FirstChild)
					return scrollbar_navigation == null ? null : scrollbar_navigation.Navigate (direction);
				else
					return base.Navigate (direction);
			}
			
			private Orientation GetOrientationFromString ()
			{
				return field_name == "hscrollbar" ? Orientation.Horizontal 
					: Orientation.Vertical;
			}

			private string field_name;
			private ScrollBarProvider scrollbar_provider;
			private INavigation scrollbar_navigation;
		}
		
#endregion

#region Internal Class: ListBoxItem Navigation
		
		class ListBoxFirstItemNavigation : ChildNavigation
		{
			public ListBoxFirstItemNavigation (ListBoxProvider provider,
			                                   NavigationChain chain)
				: base (provider, chain)
			{
				this.provider = provider;
				
				provider.ChildRemoved += new StructureChangeEventHandler (OnChildRemoved);
				provider.ChildrenClear += new EventHandler (OnChildrenClear);
			}
			
			public override void Terminate ()
			{
				if (first_item_provider != null) {
					first_item_provider.Terminate ();
					first_item_provider = null;
				}
				
				provider.ChildRemoved -= new StructureChangeEventHandler (OnChildRemoved);
				provider.ChildrenClear -= new EventHandler (OnChildrenClear);
			}			
			
			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				if (direction == NavigateDirection.NextSibling)
					return first_item_provider == null ? null : navigation.Navigate (direction);
				else
					return base.Navigate (direction);
			}
			
			protected override IRawElementProviderSimple GetChildProvider ()
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
			
			private void OnChildRemoved (object sender, ListItemProvider item, int index)
			{			
				if (index == 0) {
					first_item_provider = null;
					navigation = null;
				}
			}
			
			private void OnChildrenClear (object sender, EventArgs args)
			{
				first_item_provider = null;
				navigation = null;
			}

			private INavigation navigation;
			private ListBoxProvider provider;
			private ListItemProvider first_item_provider;
		}
		
#endregion
	}
}
