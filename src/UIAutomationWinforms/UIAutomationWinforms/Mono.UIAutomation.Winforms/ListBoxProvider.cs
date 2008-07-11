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

#region Delegates
	
	public delegate void StructureChangeEventHandler (object sender, 
	                                                  ListItemProvider item, 
	                                                  int index);
	
	public delegate void ScrollbarNavigableEventHandler (object container,
	                                                     ScrollBar scrollbar,
	                                                     bool navigable);

#endregion

	public class ListBoxProvider : ListProvider
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
		
		public event StructureChangeEventHandler ChildAdded;
		
		public event StructureChangeEventHandler ChildRemoved;
		
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
		
#endregion
		
#region IProviderBehavior Interface

		public override object GetPropertyValue (int propertyId)
		{
			//TODO: Include: HelpTextProperty, LabeledByProperty, NameProperty
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.List.Id;
			//FIXME: According the documentation this is valid, however you can
			//focus only when control.CanFocus, this doesn't make any sense.
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "list";
			else
				return base.GetPropertyValue (propertyId);
		}
		
#endregion
		
#region FragmentControlProvider Overrides
		
		public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			throw new NotImplementedException ();
		}
		
#endregion
		
#region FragmentRootControlProvider Overrides
		
		public override void InitializeChildControlStructure ()
		{
			Helper.AddPrivateEvent (typeof (ListBox.ObjectCollection), 
			                        listbox_control.Items, 
			                        "ChildAdded",
			                        this, 
			                        "OnChildAdded");
			Helper.AddPrivateEvent (typeof (ListBox.ObjectCollection), 
			                        listbox_control.Items, 
			                        "ChildRemoved", 
			                        this, 
			                        "OnChildRemoved");

			for (int index = 0; index < listbox_control.Items.Count; index++)
				OnChildAdded (listbox_control, index);
		}
		
		public override void FinalizeChildControlStructure ()
		{		
			Helper.RemovePrivateEvent (typeof (ListBox.ObjectCollection), 
			                           listbox_control.Items, 
			                           "ChildAdded",
			                           this, 
			                           "OnChildAdded");
			Helper.RemovePrivateEvent (typeof (ListBox.ObjectCollection), 
			                           listbox_control.Items, 
			                           "ChildRemoved", 
			                           this, 
			                           "OnChildRemoved");
			
			ClearItemsList ();
		}

#endregion
		
#region ListProvider Methods
		
		public override bool SupportsMulipleSelection { 
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
			((ListBox) ListControl).SetSelected (item.Index, true);
		}

		public override void UnselectItem (ListItemProvider item)
		{
			((ListBox) ListControl).SetSelected (item.Index, false);
		}
		
		public override bool IsItemSelected (ListItemProvider item)
		{
			return ((ListBox) ListControl).SelectedIndices.Contains (item.Index);
		}
		
#endregion
		
#region Private Methods - Items 

		private void OnChildAdded (object sender, int index)
		{
			ListItemProvider item = GetItemProvider (index);

			Console.WriteLine ("ListBox: Child added.");
			
			Helper.RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
			                                   item);
			Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated,
			                                   this);
			
			if (ChildAdded != null)
				ChildAdded (this, item, index);
		}
		
		private void OnChildRemoved (object sender, int index)
		{
			ListItemProvider item = RemoveItemAt (index);
			
			Console.WriteLine ("ListBox: Child removed: {0}",
			                   item.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id));
				
			Helper.RaiseStructureChangedEvent (StructureChangeType.ChildRemoved,
			                                   item);
			Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated,
			                                   this);
			
			if (ChildRemoved != null)
				ChildRemoved (this, item, index);
		}
		
#endregion

#region Private Methods - Scroll Pattern

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
				    && hscrollbar.Enabled == true)
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
					HScrollbarNavigationUpdated  (this, hscrollbar, true);
				else
					HScrollbarNavigationUpdated  (this, hscrollbar, false);
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
				    && vscrollbar.Enabled == true)
					VScrollbarNavigationUpdated  (this, vscrollbar, true);
				else
					VScrollbarNavigationUpdated  (this, vscrollbar, false);
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
				    && vscrollbar.Visible == true)
					VScrollbarNavigationUpdated  (this, vscrollbar, true);
				else
					VScrollbarNavigationUpdated  (this, vscrollbar, false);
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
		
#endregion
	}
}
