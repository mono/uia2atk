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
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{

	internal class ListBoxProvider : ListProvider
	{
		
		#region Constructor 

		public ListBoxProvider (ListBox listbox) : base (listbox)
		{
			listboxControl = listbox;

			InitializeScrollBehavior ();
		}

		#endregion
		
		#region Public Properties
		
		public override INavigation Navigation {
			get { 
				if (navigation == null)
					navigation = new ListBoxNavigation (this);

				return navigation;
			}
		}
		
		public bool HasHorizontalScrollbar {
			get { return hscrollbar.Visible == true && hscrollbar.Enabled == true; }
		}
		
		public bool HasVerticalScrollbar {
			get { return vscrollbar.Visible == true && vscrollbar.Enabled == true; }
		}		
		
		#endregion
		
		#region Public Events
		
		public event ScrollbarNavigableEventHandler HScrollbarNavigationUpdated;
		
		public event ScrollbarNavigableEventHandler VScrollbarNavigationUpdated;
		
		#endregion
		
		#region Public Methods
		
		public override void Terminate ()
		{
			base.Terminate ();
			
			vscrollbar.VisibleChanged -= new EventHandler (UpdateVScrollBehaviorVisible);
			vscrollbar.EnabledChanged -= new EventHandler (UpdateVScrollBehaviorEnable);

			hscrollbar.VisibleChanged -= new EventHandler (UpdateHScrollBehaviorVisible);
			hscrollbar.EnabledChanged -= new EventHandler (UpdateHScrollBehaviorEnable);
		}
		
		public ScrollBarProvider GetChildScrollBarProvider (Orientation orientation)
		{
			return orientation == Orientation.Horizontal 
				? hscrollbarProvider : vscrollbarProvider;
		}
		
		#endregion
		
		#region SimpleControlProvider: Specializations

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.List.Id;
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "list";
			else
				return base.GetPropertyValue (propertyId);
		}
		
		#endregion
		
		#region FragmentControlProvider: Specializations
		
		public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			throw new NotImplementedException ();
		}
		
		#endregion
		
		#region FragmentRootControlProvider: Specializations
		
		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			for (int index = 0; index < listboxControl.Items.Count; index++)
				GenerateChildAddedEvent (index);
			
			//Scrollbars
			if (HasHorizontalScrollbar)
				RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
				                            ref hscrollbarProvider,
				                            hscrollbar);
			if (HasVerticalScrollbar)
				RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
				                            ref vscrollbarProvider,
				                            vscrollbar);

			HScrollbarNavigationUpdated += new ScrollbarNavigableEventHandler (OnScrollbarNavigationUpdated);
			VScrollbarNavigationUpdated += new ScrollbarNavigableEventHandler (OnScrollbarNavigationUpdated);
		}
		
		public override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();

			//ScrollBars
			HScrollbarNavigationUpdated -= new ScrollbarNavigableEventHandler (OnScrollbarNavigationUpdated);
			VScrollbarNavigationUpdated -= new ScrollbarNavigableEventHandler (OnScrollbarNavigationUpdated);
			
			if (hscrollbarProvider != null)
				hscrollbarProvider.Terminate ();
			if (vscrollbarProvider != null)
				vscrollbarProvider.Terminate ();
		}

		#endregion
		
		#region ListProvider: Specializations
		
		public override bool SupportsMultipleSelection { 
			get { 
				return listboxControl.SelectionMode == SelectionMode.MultiExtended 
					|| listboxControl.SelectionMode == SelectionMode.MultiSimple;
			} 
		}
		
		public override int SelectedItemsCount { 
			get { return listboxControl.SelectedItems.Count; }
		}
		
		public override int ItemsCount {
			get { return listboxControl.Items.Count;  }
		}
		
		public override ListItemProvider[] GetSelectedItemsProviders ()
		{
			ListItemProvider []items;

			if (listboxControl.SelectedIndices.Count == 0)
				return null;
			
			items = new ListItemProvider [listboxControl.SelectedIndices.Count];			
			for (int index = 0; index < items.Length; index++) 
				items [index] = GetItemProvider (listboxControl.SelectedIndices [index]);
			
			return items;
		}
		
		public override string GetItemName (ListItemProvider item)
		{
			return listboxControl.Items [item.Index].ToString ();
		}
		
		public override void SelectItem (ListItemProvider item)
		{
			listboxControl.SetSelected (item.Index, true);
		}

		public override void UnselectItem (ListItemProvider item)
		{
			listboxControl.SetSelected (item.Index, false);
		}
		
		public override bool IsItemSelected (ListItemProvider item)
		{
			return listboxControl.SelectedIndices.Contains (item.Index);
		}
		
		protected override Type GetTypeOfObjectCollection ()
		{
			return typeof (ListBox.ObjectCollection);
		}
		
		protected override object GetInstanceOfObjectCollection ()
		{
			return listboxControl.Items;
		}
		
		public override void ScrollItemIntoView (ListItemProvider item)
		{
			if (ContainsItem (item) == true)
				listboxControl.TopIndex = item.Index;
		}
		
		#endregion
		
		#region Private Methods: StructureChangedEvent
		
		private void OnScrollbarNavigationUpdated (object container,
		                                           ScrollBar scrollbar,
		                                           bool navigable)
		{
			if (scrollbar == vscrollbar) {
				if (navigable == false && vscrollbarProvider != null) {
					RaiseStructureChangedEvent (StructureChangeType.ChildRemoved,
					                            ref vscrollbarProvider,
					                            vscrollbar);
				} else if (navigable == true && vscrollbarProvider == null) {
					RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
					                            ref vscrollbarProvider,
					                            vscrollbar);
				}
			} else if (scrollbar == hscrollbar) {
				if (navigable == false && hscrollbarProvider != null) {
					RaiseStructureChangedEvent (StructureChangeType.ChildRemoved,
					                            ref hscrollbarProvider,
					                            hscrollbar);
				} else if (navigable == true && hscrollbarProvider == null) {
					RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
					                            ref hscrollbarProvider,
					                            hscrollbar);
				}
			}
		}
		
		private void RaiseStructureChangedEvent (StructureChangeType type,
		                                         ref ScrollBarProvider provider,
		                                         ScrollBar scrollbar)
		{
			if (type == StructureChangeType.ChildAdded) {
				provider = (ScrollBarProvider) ProviderFactory.GetProvider (scrollbar);
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildAdded, provider);
			} else {
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildRemoved, provider);
				provider.Terminate ();
				provider = null;
			}

			Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated,
			                                   this);
		}
		
		#endregion

		#region Private Methods: Scroll Pattern

		private void InitializeScrollBehavior () 
		{
			vscrollbar = (VScrollBar) Helper.GetPrivateField (typeof (ListBox), 
			                                                  listboxControl, 
			                                                  "vscrollbar");
			hscrollbar = (HScrollBar) Helper.GetPrivateField (typeof (ListBox), 
			                                                  listboxControl, 
			                                                  "hscrollbar");
			
			vscrollbar.VisibleChanged += new EventHandler (UpdateVScrollBehaviorVisible);
			vscrollbar.EnabledChanged += new EventHandler (UpdateVScrollBehaviorEnable);

			hscrollbar.VisibleChanged += new EventHandler (UpdateHScrollBehaviorVisible);
			hscrollbar.EnabledChanged += new EventHandler (UpdateHScrollBehaviorEnable);
			
			if ((vscrollbar.Visible == true && vscrollbar.Enabled == true)
			    || (hscrollbar.Visible == true && hscrollbar.Enabled == true))
				SetScrollPatternBehavior ();
		}
		
		private void UpdateHScrollBehaviorEnable (object sender, EventArgs args)
		{
			//Event used to update Navigation chain
			if (HScrollbarNavigationUpdated != null) {
				if (listboxControl.HorizontalScrollbar == true 
				    && hscrollbar.Visible == true && hscrollbar.Enabled == true)
					HScrollbarNavigationUpdated  (this, hscrollbar, true);
				else
					HScrollbarNavigationUpdated  (this, hscrollbar, false);
			}
			
			//Updating Behavior
			if (hscrollbar.Enabled == true) {
				if (scrollpatternSet == false) {
					if (listboxControl.HorizontalScrollbar == true) {
						SetScrollPatternBehavior ();
					}
				}
			} else {
				if (scrollpatternSet == true) {
					if (vscrollbar.Visible == true && vscrollbar.Enabled == true)
						return;

					UnsetScrollPatternBehavior ();
				}
			}
		}
		
		private void UpdateHScrollBehaviorVisible (object sender, EventArgs args)
		{
			//Event used to update Navigation chain
			if (HScrollbarNavigationUpdated != null) {
				if (listboxControl.HorizontalScrollbar == true 
				    && hscrollbar.Visible == true && hscrollbar.Enabled == true)
					HScrollbarNavigationUpdated (this, hscrollbar, true);
				else
					HScrollbarNavigationUpdated (this, hscrollbar, false);
			}
			
			//Updating Behavior
			if (scrollpatternSet == false) {
				if (hscrollbar.Visible == true) {
					if (hscrollbar.Enabled == false 
					    || listboxControl.HorizontalScrollbar == false)
						return;

					SetScrollPatternBehavior ();
				}
			} else {
				if (hscrollbar.Visible == false) {
					if (vscrollbar.Visible == true && vscrollbar.Enabled)
						return;

					UnsetScrollPatternBehavior ();
				}
			}
		}
		
		private void UpdateVScrollBehaviorEnable (object sender, EventArgs args)
		{
			//Event used to update Navigation chain
			if (VScrollbarNavigationUpdated != null) {
				if (listboxControl.ScrollAlwaysVisible == true
				    && vscrollbar.Visible == true && vscrollbar.Enabled == true)
					VScrollbarNavigationUpdated (this, vscrollbar, true);
				else
					VScrollbarNavigationUpdated (this, vscrollbar, false);
			}
			
			//Updating Behavior
			if (vscrollbar.Visible == true && scrollpatternSet == false) {
				SetScrollPatternBehavior ();
			} else if (vscrollbar.Visible == true && scrollpatternSet == true) {
				if (hscrollbar.Visible == true && hscrollbar.Enabled == true)
					return;
				UnsetScrollPatternBehavior ();
			}
		}		
		
		private void UpdateVScrollBehaviorVisible (object sender, EventArgs args)
		{
			//Event used to update Navigation chain
			if (VScrollbarNavigationUpdated != null) {
				if (listboxControl.ScrollAlwaysVisible == false
				    && vscrollbar.Enabled == true && vscrollbar.Visible == true)
					VScrollbarNavigationUpdated (this, vscrollbar, true);
				else
					VScrollbarNavigationUpdated (this, vscrollbar, false);
			}
			
			//Updating Behavior
			if (scrollpatternSet == false) {
				if (vscrollbar.Visible == true) {
					if (vscrollbar.Enabled == false)
						return;
					SetScrollPatternBehavior ();
				}
			} else {
				if (vscrollbar.Visible == false) {
					if (hscrollbar.Visible == true && hscrollbar.Enabled == true)
						return;
					UnsetScrollPatternBehavior ();					
				}
			}
		}
		
		private void SetScrollPatternBehavior ()
		{
			if (scrollpatternSet == false) {
				scrollpatternSet = true;
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             new ListBoxScrollProviderBehavior (this, 
				                                                hscrollbar,
				                                                vscrollbar));
				//TODO: Generate IsScroll Event
			}
		}
		
		private void UnsetScrollPatternBehavior ()
		{
			if (scrollpatternSet == true) {
				scrollpatternSet = false;
				SetBehavior (ScrollPatternIdentifiers.Pattern, null);
			}
		}

		#endregion
		
		#region Private Fields
		
		private HScrollBar hscrollbar;
		private ListBox listboxControl;
		private bool scrollpatternSet;
		private VScrollBar vscrollbar;
		private ScrollBarProvider hscrollbarProvider;
		private ScrollBarProvider vscrollbarProvider;
		
		#endregion

		#region Internal Class: ScrollBar provider

		internal class ListBoxScrollBarProvider : ScrollBarProvider
		{
			public ListBoxScrollBarProvider (ScrollBar scrollbar)
				: base (scrollbar)
			{
				//TODO: i18n?
				name = scrollbar is HScrollBar ? "Horizontal Scroll Bar"
					: "Vertical Scroll Bar";
			}
			
			public override object GetPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return name;
				else
					return base.GetPropertyValue (propertyId);
			}
			
			private string name;
		}
		
		#endregion
	}

}
