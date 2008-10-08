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
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using System.Windows;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.ListBox;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ListBox;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{

	internal class ListBoxProvider : ListProvider, IScrollBehaviorSubject
	{		
	
		#region Constructor 

		public ListBoxProvider (ListBox listbox) : base (listbox)
		{
			listboxControl = listbox;
			
			vscrollbar = Helper.GetPrivateProperty<ListBox, ScrollBar> (typeof (ListBox), 
			                                                            listboxControl,
			                                                            "UIAVScrollBar");
			hscrollbar = Helper.GetPrivateProperty<ListBox, ScrollBar> (typeof (ListBox),
			                                                            listboxControl,
			                                                            "UIAHScrollBar");
			
			scrollObserver = new ScrollBehaviorObserver (this,
			                                             hscrollbar,
			                                             vscrollbar);
			
			scrollObserver.HorizontalNavigationUpdated += new NavigationEventHandler (OnHorizontalNavigationUpdated);
			scrollObserver.VerticalNavigationUpdated += new NavigationEventHandler (OnVerticalNavigationUpdated);
			scrollObserver.ScrollPatternSupportChanged += new EventHandler (OnScrollPatternSupportChanged);
			
			if (scrollObserver.SupportsScrollPattern == true)
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             new ScrollProviderBehavior (this, 
				                                         hscrollbar, 
				                                         vscrollbar));
		}

		#endregion
		
		#region IScrollBehaviorSubject specialization
		
		public bool SupportsHorizontalScrollbar { 
			get { return listboxControl.HorizontalScrollbar; } 
		}
		
		public bool SupportsVerticalScrollbar { 
			get { return true; }
		}		
		
		#endregion

		#region Public Properties
		
		public bool HasHorizontalScrollbar {
			get { return scrollObserver.HasHorizontalScrollbar; }
		}
		
		public bool HasVerticalScrollbar {
			get { return scrollObserver.HasVerticalScrollbar; }
		}
		
		#endregion
		
		#region Public Methods
		
		public ScrollBar GetInternalScrollBar (Orientation orientation)
		{
			return orientation == Orientation.Horizontal ? hscrollbar : vscrollbar;
		}
		
		public override void Terminate ()
		{
			base.Terminate ();
			
			scrollObserver.HorizontalNavigationUpdated -= new NavigationEventHandler (OnHorizontalNavigationUpdated);
			scrollObserver.VerticalNavigationUpdated -= new NavigationEventHandler (OnVerticalNavigationUpdated);
			scrollObserver.ScrollPatternSupportChanged -= new EventHandler (OnScrollPatternSupportChanged);
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
		
		public override IRawElementProviderFragment GetFocus ()
		{
			return GetItemProviderAt (listboxControl.SelectedIndex);
		}
		
		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();
			
			for (int index = 0; index < listboxControl.Items.Count; index++) {
				ListItemProvider item = GetItemProviderAt (index);
				OnNavigationChildAdded (false, item);
			}
			
			if (HasHorizontalScrollbar == true)
				RaiseNavigationEvent (StructureChangeType.ChildAdded,
				                      ref hscrollbarProvider,
				                      hscrollbar,
				                      false);
			if (HasVerticalScrollbar == true)
				RaiseNavigationEvent (StructureChangeType.ChildAdded,
				                      ref vscrollbarProvider,
				                      vscrollbar,
				                      false);			
		}
		
		public override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();
			
			if (hscrollbarProvider != null) {
				OnNavigationChildRemoved (false, hscrollbarProvider);
				hscrollbarProvider.Terminate ();
				hscrollbarProvider = null;
			}

			if (vscrollbarProvider != null) {
				OnNavigationChildRemoved (false, vscrollbarProvider);
				vscrollbarProvider.Terminate ();
				vscrollbarProvider = null;
			}
		}

		#endregion
		
		#region ListItem: Properties Methods
		
		public override object GetItemPropertyValue (ListItemProvider item,
		                                             int propertyId)
		{
			if (ContainsItem (item) == false)
				return null;
			
			if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return listboxControl.Items [item.Index].ToString ();
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return listboxControl.Focused && item.Index == listboxControl.SelectedIndex;
			else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
				System.Drawing.Rectangle itemRec = listboxControl.GetItemRectangle (item.Index);
				System.Drawing.Rectangle rectangle = listboxControl.Bounds;
				
				itemRec.X += rectangle.X;
				itemRec.Y += rectangle.Y;
				
				if (listboxControl.FindForm () == listboxControl.Parent)
					itemRec = listboxControl.TopLevelControl.RectangleToScreen (itemRec);
				else
					itemRec = listboxControl.Parent.RectangleToScreen (itemRec);
	
				return Helper.RectangleToRect (itemRec);
			} else
				return null;
		}
		
		#endregion
		
		#region ListProvider: Specializations
		
		public override int SelectedItemsCount { 
			get { return listboxControl.SelectedItems.Count; }
		}
		
		public override int ItemsCount {
			get { return listboxControl.Items.Count;  }
		}
		
		public override ListItemProvider[] GetSelectedItems ()
		{
			ListItemProvider []items;

			if (listboxControl == null || listboxControl.SelectedIndices.Count == 0)
				return null;
			
			items = new ListItemProvider [listboxControl.SelectedIndices.Count];			
			for (int index = 0; index < items.Length; index++) 
				items [index] = GetItemProviderAt (listboxControl.SelectedIndices [index]);
			
			return items;
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

		public override IConnectable GetListItemHasKeyboardFocusEvent (ListItemProvider provider)
		{
			return new ListItemAutomationHasKeyboardFocusPropertyEvent (provider);
		}
		
		public override void ScrollItemIntoView (ListItemProvider item)
		{
			if (ContainsItem (item) == true)
				listboxControl.TopIndex = item.Index;
		}
		
		#endregion
		
		#region Internal Methods: Get Behaviors
		
		internal override IProviderBehavior GetBehaviorRealization (AutomationPattern behavior)
		{
			if (behavior == SelectionPatternIdentifiers.Pattern)
				return new SelectionProviderBehavior (this);
			else 
				return null;
		}		
		
		internal override IProviderBehavior GetListItemBehaviorRealization (AutomationPattern behavior,
		                                                                    ListItemProvider listItem)
		{
			if (behavior == SelectionItemPatternIdentifiers.Pattern)
				return new ListItemSelectionItemProviderBehavior (listItem);
			else
				return base.GetListItemBehaviorRealization (behavior, listItem);
		}
		
		#endregion
		
		#region Private Methods: Navigation
		
		private void UpdateScrollbarNavigation (ScrollBar scrollbar,
		                                        bool navigable)
		{
			if (scrollbar == vscrollbar) {
	           if (navigable == false && vscrollbarProvider != null)
					RaiseNavigationEvent (StructureChangeType.ChildRemoved,
					                      ref vscrollbarProvider,
					                      vscrollbar,
					                      true);
	           else if (navigable == true && vscrollbarProvider == null)
					RaiseNavigationEvent (StructureChangeType.ChildAdded,
					                      ref vscrollbarProvider,
					                      vscrollbar,
					                      true);
			} else if (scrollbar == hscrollbar) {
	           if (navigable == false && hscrollbarProvider != null)
					RaiseNavigationEvent (StructureChangeType.ChildRemoved,
					                      ref hscrollbarProvider,
					                      hscrollbar,
					                      true);
	           else if (navigable == true && hscrollbarProvider == null)
					RaiseNavigationEvent (StructureChangeType.ChildAdded,
					                      ref hscrollbarProvider,
					                      hscrollbar,
					                      true);
			}
		}
		
		private void RaiseNavigationEvent (StructureChangeType type,
		                                   ref ScrollBarProvider provider,
		                                   ScrollBar scrollbar,
		                                   bool generateEvent)
		{
			if (type == StructureChangeType.ChildAdded) {
				provider = (ScrollBarProvider) ProviderFactory.GetProvider (scrollbar);
				OnNavigationChildAdded (generateEvent, provider);
			} else {
				OnNavigationChildRemoved (generateEvent, provider);
				provider.Terminate ();
				provider = null;
			}
		}
		
		#endregion

		#region ScrollBehaviorObserver Methods
		
		private void OnScrollPatternSupportChanged (object sender, EventArgs args)
		{
			if (scrollObserver.SupportsScrollPattern == true)
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             new ScrollProviderBehavior (this, 
				                                         hscrollbar, 
				                                         vscrollbar));
			else
				SetBehavior (ScrollPatternIdentifiers.Pattern, null);
		}
		
		private void OnHorizontalNavigationUpdated (object sender,
		                                            NavigationEventArgs args)
		{
			UpdateScrollbarNavigation (hscrollbar,
			                           args.ChangeType == StructureChangeType.ChildAdded);
		}
		
		private void OnVerticalNavigationUpdated (object sender,
		                                          NavigationEventArgs args)
		{
			UpdateScrollbarNavigation (vscrollbar,
			                           args.ChangeType == StructureChangeType.ChildAdded);
		}
		
		#endregion
		
		#region Private Fields
		
		private ScrollBar hscrollbar;
		private ListBox listboxControl;
		private ScrollBar vscrollbar;
		private ScrollBarProvider hscrollbarProvider;
		private ScrollBarProvider vscrollbarProvider;
		private ScrollBehaviorObserver scrollObserver;
		
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
