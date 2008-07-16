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

	//TODO: Should support: MultipleView Pattern and Grid Pattern?
	internal class ListBoxProvider : ListProvider
	{
		
#region Constructor 

		public ListBoxProvider (ListBox listbox) : base (listbox)
		{
			listbox_control = listbox;

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
		
		public bool HasHorizontalScrollbar
		{
			get { return hscrollbar.Visible == true && hscrollbar.Enabled == true; }
		}
		
		public bool HasVerticalScrollbar
		{
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
				? hscrollbar_provider : vscrollbar_provider;
		}
		
#endregion
		
#region SimpleControlProvider: Specializations

		public override object GetPropertyValue (int propertyId)
		{
			//TODO: Include: HelpTextProperty, LabeledByProperty, NameProperty
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.List.Id;
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "list";
			else if (propertyId == AutomationElementIdentifiers.IsScrollPatternAvailableProperty.Id)
				return IsBehaviorEnabled (ScrollPatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id)
				return false;
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

			for (int index = 0; index < listbox_control.Items.Count; index++)
				GenerateChildAddedEvent (index);
			
			//Scrollbars
			if (HasHorizontalScrollbar)
				RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
				                            ref hscrollbar_provider,
				                            hscrollbar);
			if (HasVerticalScrollbar)
				RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
				                            ref vscrollbar_provider,
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
			
			if (hscrollbar_provider != null)
				hscrollbar_provider.Terminate ();
			if (vscrollbar_provider != null)
				vscrollbar_provider.Terminate ();
		}

#endregion
		
#region ListProvider: Specializations
		
		public override bool SupportsMultipleSelection { 
			get { 
				return listbox_control.SelectionMode == SelectionMode.MultiExtended 
					|| listbox_control.SelectionMode == SelectionMode.MultiSimple;
			} 
		}
		
		public override int SelectedItemsCount { 
			get { return listbox_control.SelectedItems.Count; }
		}
		
		public override int ItemsCount {
			get { return listbox_control.Items.Count;  }
		}
		
		public override ListItemProvider[] GetSelectedItemsProviders ()
		{
			ListItemProvider []items;

			if (listbox_control.SelectedIndices.Count == 0)
				return null;
			
			items = new ListItemProvider [listbox_control.SelectedIndices.Count];			
			for (int index = 0; index < items.Length; index++) 
				items [index] = GetItemProvider (listbox_control.SelectedIndices [index]);
			
			return items;
		}
		
		public override string GetItemName (ListItemProvider item)
		{
			return listbox_control.Items [item.Index].ToString ();
		}
		
		public override void SelectItem (ListItemProvider item)
		{
			listbox_control.SetSelected (item.Index, true);
		}

		public override void UnselectItem (ListItemProvider item)
		{
			listbox_control.SetSelected (item.Index, false);
		}
		
		public override bool IsItemSelected (ListItemProvider item)
		{
			return listbox_control.SelectedIndices.Contains (item.Index);
		}
		
		protected override Type GetTypeOfObjectCollection ()
		{
			return typeof (ListBox.ObjectCollection);
		}
		
		protected override object GetInstanceOfObjectCollection ()
		{
			return listbox_control.Items;
		}
		
#endregion
		
#region Private Methods: StructureChangedEvent
		
		private void OnScrollbarNavigationUpdated (object container,
		                                           ScrollBar scrollbar,
		                                           bool navigable)
		{
			if (scrollbar == vscrollbar) {
				if (navigable == false && vscrollbar_provider != null) {
					RaiseStructureChangedEvent (StructureChangeType.ChildRemoved,
					                            ref vscrollbar_provider,
					                            vscrollbar);
				} else if (navigable == true && vscrollbar_provider == null) {
					RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
					                            ref vscrollbar_provider,
					                            vscrollbar);
				}
			} else if (scrollbar == hscrollbar) {
				if (navigable == false && hscrollbar_provider != null) {
					RaiseStructureChangedEvent (StructureChangeType.ChildRemoved,
					                            ref hscrollbar_provider,
					                            hscrollbar);
				} else if (navigable == true && hscrollbar_provider == null) {
					RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
					                            ref hscrollbar_provider,
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
			vscrollbar = (VScrollBar) Helper.GetPrivateField (listbox_control.GetType (), 
			                                                  listbox_control, 
			                                                  "vscrollbar");
			hscrollbar = (HScrollBar) Helper.GetPrivateField (listbox_control.GetType (), 
			                                                  listbox_control, 
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
				if (listbox_control.HorizontalScrollbar == true 
				    && hscrollbar.Visible == true && hscrollbar.Enabled == true)
					HScrollbarNavigationUpdated  (this, hscrollbar, true);
				else
					HScrollbarNavigationUpdated  (this, hscrollbar, false);
			}
			
			//Updating Behavior
			if (hscrollbar.Enabled == true) {
				if (scrollpattern_set == false) {
					if (listbox_control.HorizontalScrollbar == true) {
						SetScrollPatternBehavior ();
					}
				}
			} else {
				if (scrollpattern_set == true) {
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
				if (listbox_control.HorizontalScrollbar == true 
				    && hscrollbar.Visible == true && hscrollbar.Enabled == true)
					HScrollbarNavigationUpdated (this, hscrollbar, true);
				else
					HScrollbarNavigationUpdated (this, hscrollbar, false);
			}
			
			//Updating Behavior
			if (scrollpattern_set == false) {
				if (hscrollbar.Visible == true) {
					if (hscrollbar.Enabled == false 
					    || listbox_control.HorizontalScrollbar == false)
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
				if (listbox_control.ScrollAlwaysVisible == true
				    && vscrollbar.Visible == true && vscrollbar.Enabled == true)
					VScrollbarNavigationUpdated (this, vscrollbar, true);
				else
					VScrollbarNavigationUpdated (this, vscrollbar, false);
			}
			
			//Updating Behavior
			if (vscrollbar.Visible == true && scrollpattern_set == false) {
				SetScrollPatternBehavior ();
			} else if (vscrollbar.Visible == true && scrollpattern_set == true) {
				if (hscrollbar.Visible == true && hscrollbar.Enabled == true)
					return;
				UnsetScrollPatternBehavior ();
			}
		}		
		
		private void UpdateVScrollBehaviorVisible (object sender, EventArgs args)
		{
			//Event used to update Navigation chain
			if (VScrollbarNavigationUpdated != null) {
				if (listbox_control.ScrollAlwaysVisible == false
				    && vscrollbar.Enabled == true && vscrollbar.Visible == true)
					VScrollbarNavigationUpdated (this, vscrollbar, true);
				else
					VScrollbarNavigationUpdated (this, vscrollbar, false);
			}
			
			//Updating Behavior
			if (scrollpattern_set == false) {
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
			if (scrollpattern_set == false) {
				scrollpattern_set = true;
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             new ListBoxScrollProviderBehavior (this, 
				                                                hscrollbar,
				                                                vscrollbar));
				//TODO: Generate IsScroll Event
			}
		}
		
		private void UnsetScrollPatternBehavior ()
		{
			if (scrollpattern_set == true) {
				scrollpattern_set = false;
				SetBehavior (ScrollPatternIdentifiers.Pattern, null);
			}
		}

#endregion
		
#region Private Fields
		
		private HScrollBar hscrollbar;
		private ListBox listbox_control;
		private bool scrollpattern_set;
		private VScrollBar vscrollbar;
		private ScrollBarProvider hscrollbar_provider;
		private ScrollBarProvider vscrollbar_provider;
		
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
