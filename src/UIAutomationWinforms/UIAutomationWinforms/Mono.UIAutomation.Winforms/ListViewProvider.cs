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
using System.ComponentModel;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.ListView;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	
	internal class ListViewProvider : ListProvider, IScrollBehaviorSubject
	{
		
		#region Constructors

		public ListViewProvider (SWF.ListView listView) : base (listView)
		{
			this.listView = listView;
			
			try { //TODO: Remove try-cath when SWF patch applied
			
			SWF.ScrollBar vscrollbar 
					= Helper.GetPrivateProperty<SWF.ListView, SWF.ScrollBar> (typeof (SWF.ListView), 
					                                                          listView,
					                                                          "UIAVScrollBar");
			SWF.ScrollBar hscrollbar 
					= Helper.GetPrivateProperty<SWF.ListView, SWF.ScrollBar> (typeof (SWF.ListView),
					                                                          listView,
					                                                          "UIAHScrollBar");
				
			//ListScrollBehaviorObserver updates Navigation
			observer = new ScrollBehaviorObserver (this, hscrollbar, vscrollbar);			
			observer.ScrollPatternSupportChanged += OnScrollPatternSupportChanged;
			UpdateScrollBehavior ();
			
			} catch (Exception) {}
			
			lastView = listView.View;
			groups = new Dictionary<SWF.ListViewGroup, ListViewGroupProvider> ();
		}
		
		#endregion
		
		#region IScrollBehaviorSubject specialization
		
		public bool SupportsHorizontalScrollbar { 
			get { return listView.Scrollable; } 
		}
		
		public bool SupportsVerticalScrollbar { 
			get { return listView.Scrollable; }
		}
		
		public IScrollBehaviorObserver ScrollBehaviorObserver { 
			get { return observer; }
		}
		
		public FragmentControlProvider GetScrollbarProvider (SWF.ScrollBar scrollbar)
		{
			return new ListViewScrollBarProvider (scrollbar, listView);
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
		
		#region ListProvider: Internal Methods: Get Behaviors
		
		internal override IProviderBehavior GetBehaviorRealization (AutomationPattern behavior)
		{
			if (MultipleViewPatternIdentifiers.Pattern == behavior)
				return new MultipleViewProviderBehavior (this);
			else if (behavior == SelectionPatternIdentifiers.Pattern) 
				return new SelectionProviderBehavior (this);
			else if (GridPatternIdentifiers.Pattern == behavior) {         
//				if (listView.View == SWF.View.Details || listView.View == SWF.View.List)
//					return null; //TODO: Return realization
//				else
					return null;
			} else
				return null;
		}
		
		#endregion
		
		#region ListItem: Properties Methods
		
		public override object GetItemPropertyValue (ListItemProvider item,
		                                             int propertyId)
		{
			if (ContainsItem (item) == false)
				return null;
			
			if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return listView.Items [item.Index].ToString ();
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return listView.Focused && listView.SelectedIndices.Contains (item.Index); //TODO: OK?
			else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
				System.Drawing.Rectangle itemRec = listView.GetItemRect (item.Index);
				System.Drawing.Rectangle rectangle = listView.Bounds;
				
				itemRec.X += rectangle.X;
				itemRec.Y += rectangle.Y;
				
				if (listView.FindForm () == listView.Parent)
					itemRec = listView.TopLevelControl.RectangleToScreen (itemRec);
				else
					itemRec = listView.Parent.RectangleToScreen (itemRec);
	
				return Helper.RectangleToRect (itemRec);
			} else
				return null;
		}
		
		#endregion 
		
		#region ListProvider specializations
		
		public override void Terminate ()
		{
			base.Terminate ();
			
			observer.ScrollPatternSupportChanged -= OnScrollPatternSupportChanged;
		}
		
		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();
			
			try {
				Helper.AddPrivateEvent (typeof (SWF.ListView), 
				                        listView,
				                        "UIAViewChanged",
				                        this, 
				                        "OnUIAViewChanged");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: UIAViewChanged not defined", GetType ());
			}
			
			observer.InitializeScrollBarProviders ();

			// Children initialization
			UpdateChildrenStructure ();
		}
		
		public override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();
			
			try {
				Helper.RemovePrivateEvent (typeof (SWF.ListView), 
				                           listView,
				                           "UIAViewChanged",
				                           this, 
				                           "OnUIAViewChanged");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: UIAViewChanged not defined", GetType ());
			}			
		}
		
		public override int ItemsCount { 
			get { return listView.Items.Count; }
		}
		
		#endregion
			
		#region ListItem: Selection Methods and Properties
		
		public override int SelectedItemsCount {
			get { return listView.SelectedItems.Count; }
		}
		
		public override bool IsItemSelected (ListItemProvider item)
		{
			return listView.SelectedIndices.Contains (item.Index);
		}
		
		public override ListItemProvider[] GetSelectedItems ()
		{
			return null;
		}
		
		public override void SelectItem (ListItemProvider item)
		{
			if (ContainsItem (item) == true)
				listView.Items [item.Index].Selected = true;
		}

		public override void UnselectItem (ListItemProvider item)
		{
			if (ContainsItem (item) == true)
				listView.Items [item.Index].Selected = false;
		}
		
		#endregion
		
		#region ListItem: Toggle Methods
		
		public override ToggleState GetItemToggleState (ListItemProvider item)
		{
			if (listView.CheckBoxes == false)
				return ToggleState.Indeterminate;
			
			if (ContainsItem (item) == true)
				return listView.Items [item.Index].Checked ? ToggleState.On : ToggleState.Off;
			else
				return ToggleState.Indeterminate;
		}
		
		public override void ToggleItem (ListItemProvider item)
		{
			if (listView.CheckBoxes == false)
				return;
			
			if (ContainsItem (item) == true)
				listView.Items [item.Index].Checked = !listView.Items [item.Index].Checked;
		}
		
		#endregion
		
		public override IConnectable GetListItemHasKeyboardFocusEvent (ListItemProvider provider)
		{
			return null;
		}
		
		#region ListProvider: ListItem: Scroll Methods
		
		public override void ScrollItemIntoView (ListItemProvider item)
		{
		}
		
		#endregion
		
		#region ListProvider: Protected Methods
		
		protected override Type GetTypeOfObjectCollection ()
		{
			return typeof (SWF.ListView.ListViewItemCollection);
		}
		
		protected override object GetInstanceOfObjectCollection ()	
		{
			return listView.Items;
		}
			
		protected override void OnCollectionChanged (object sender, 
		                                             CollectionChangeEventArgs args)
		{
			if (args.Action == CollectionChangeAction.Add) {
				InitializeProviderAtIndex ((int) args.Element, true);
//				OnNavigationChildAdded (true, item);
			} else if (args.Action == CollectionChangeAction.Remove) {
				ListItemProvider item = RemoveItemAt ((int) args.Element);
//				OnNavigationChildRemoved (true, item);
			} else
				base.OnCollectionChanged (sender, args);
		}
		
		#endregion		
		
		#region Private Methods
		
		private void InitializeProviderAtIndex (int index, bool raiseEvent)
		{			
			if (index < 0 || index >= ItemsCount)
				return;
			
			// SWF.View.List: Adds items, nothing else
			
			if (listView.View == SWF.View.SmallIcon 
			    || listView.View == SWF.View.LargeIcon) {

				ListViewGroupProvider groupProvider = null;
				SWF.ListViewGroup listViewGroup = null;
				
				if (listView.Items [index].Group == null) {
					if (listViewNullGroup == null)
						listViewNullGroup = new SWF.ListViewGroup ("Default");

					listViewGroup = listViewNullGroup;
				} else
					listViewGroup = listView.Items [index].Group;
				
				if (groups.TryGetValue (listViewGroup, 
				                        out groupProvider) == false) {
					groupProvider = new ListViewGroupProvider (listViewGroup,
					                                           listView);
					groups [listViewGroup] = groupProvider;
						
					OnNavigationChildAdded (raiseEvent, groupProvider);
				}
				
				ListItemProvider item = GetItemProviderAt (groupProvider, index);
				groupProvider.AddChildProvider (raiseEvent, item);
			} else {
				ListItemProvider item = GetItemProviderAt (this, index);
				OnNavigationChildAdded (raiseEvent, item);
			}
		}
		
		private void OnUIAViewChanged (object sender, EventArgs args)
		{
			// Selection Pattern always supported
			// Scroll Pattern depends on visible/enable scrollbars

			if (listView.View == SWF.View.Details || listView.View == SWF.View.List)
				SetBehavior (GridPatternIdentifiers.Pattern,
				             GetBehaviorRealization (GridPatternIdentifiers.Pattern));
			else
				SetBehavior (GridPatternIdentifiers.Pattern,
				             null);
			
			UpdateChildrenStructure ();
			
			//SmallIcon = MultipleView, Selection
			//LargeIcon = Multipleview, Scroll, Selection
			//Tile: MultipleView, Scroll, Selection
			//---
			//Details = MultipleView, Scroll, Selection, Grid
			//List = MultipleView, Scroll, Selection, Grid
		}
		
		private void OnScrollPatternSupportChanged (object sender, EventArgs args)
		{
			UpdateScrollBehavior ();
		}
		
		private void UpdateScrollBehavior ()
		{
			if (observer.SupportsScrollPattern == true)
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             new ScrollProviderBehavior (this));
			else
				SetBehavior (ScrollPatternIdentifiers.Pattern, null);
		}
		
		private void UpdateChildrenStructure ()
		{
			bool updateView = lastView != listView.View;
			
			if (updateView == true) {			
				foreach (ListViewGroupProvider groupProvider in groups.Values)
					groupProvider.Terminate ();
				groups.Clear ();
				
				ClearItemsList ();
				OnNavigationChildrenCleared (true);
			}
			
			for (int index = 0; index < ItemsCount; index++)
				InitializeProviderAtIndex (index, updateView);
		}
		
		#endregion
		
		#region Private Fields
		
		private SWF.View lastView;
		private SWF.ListView listView;
		private Dictionary<SWF.ListViewGroup, ListViewGroupProvider> groups;
		private ScrollBehaviorObserver observer;
		private SWF.ListViewGroup listViewNullGroup;
		
		#endregion
		
		#region Internal Class: ScrollBar provider

		internal class ListViewScrollBarProvider : ScrollBarProvider
		{
			public ListViewScrollBarProvider (SWF.ScrollBar scrollbar,
			                                  SWF.ListView listView)
				: base (scrollbar)
			{
				this.listView = listView;
				//TODO: i18n?
				name = scrollbar is SWF.HScrollBar ? "Horizontal Scroll Bar"
					: "Vertical Scroll Bar";
			}
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { 
					return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (listView);
				}
			}			
			
			public override object GetPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return name;
				else
					return base.GetPropertyValue (propertyId);
			}
			
			private SWF.ListView listView;
			private string name;
		}
		
		#endregion
		
		#region Internal Class: Group Provider 
		
		internal class ListViewGroupProvider : GroupBoxProvider
		{
			
			public ListViewGroupProvider (SWF.ListViewGroup group,
			                              SWF.ListView listView) : base (null)
			{
				this.group = group;
				this.listView = listView;
				
				// TODO: Implement ExpandCollapse
				// TODO: Implement Grid
			}
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { 
					return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (listView);
				}
			}
		
			public override object GetPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Group.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return group.Header;
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else
					return base.GetPropertyValue (propertyId);
			}
			
			private SWF.ListView listView;
			private SWF.ListViewGroup group;
		}
		
		#endregion
		
	}
}
