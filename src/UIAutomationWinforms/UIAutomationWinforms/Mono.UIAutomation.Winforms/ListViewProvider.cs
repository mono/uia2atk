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
using System.Reflection;
using SWF = System.Windows.Forms;
using System.Windows;
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
			// See comment in OnUIAViewChanged method
			
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

		internal override IProviderBehavior GetListItemBehaviorRealization (AutomationPattern behavior,
		                                                                    ListItemProvider listItem)
		{
			if (behavior == SelectionItemPatternIdentifiers.Pattern)
				return new ListItemSelectionItemProviderBehavior (listItem);
			else if (behavior == GridItemPatternIdentifiers.Pattern)
				return new ListItemGridItemProviderBehavior (listItem);
			else
				return base.GetListItemBehaviorRealization (behavior, listItem);
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
				SD.Rectangle itemRec = listView.GetItemRect (item.Index);
				SD.Rectangle rectangle = listView.Bounds;
				
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

		public override int IndexOfObjectItem (object objectItem)
		{
			return listView.Items.IndexOf (objectItem as SWF.ListViewItem);
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
			if (ContainsItem (item) == false)
				return;
			
			// According to http://msdn.microsoft.com/en-us/library/system.windows.forms.listview.topitem.aspx
			if (listView.View == SWF.View.LargeIcon 
			    || listView.View == SWF.View.SmallIcon
			    || listView.View == SWF.View.Tile)
				return;
			
			listView.TopItem = listView.Items [item.Index];
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
			if (args.Action == CollectionChangeAction.Add)
				InitializeProviderFrom (args.Element, true);
			else if (args.Action == CollectionChangeAction.Remove)
				FinalizeProviderFrom (args.Element, true);
			else
				base.OnCollectionChanged (sender, args);
		}
		
		#endregion

		#region Public Methods

		public SWF.ListViewGroup GetGroupFrom (SWF.ListViewItem item)
		{
			if (item.Group == null) {
				if (listViewNullGroup == null)
						listViewNullGroup = Helper.GetPrivateProperty<SWF.ListView, SWF.ListViewGroup> (typeof (SWF.ListView), 
						                                                                                listView,
						                                                                                "UIADefaultListViewGroup");
				return listViewNullGroup;
			} else
				return item.Group;
		}

		#endregion
		
		#region Private Methods
		
		private void InitializeProviderFrom (object objectItem, bool raiseEvent)
		{			
			// View.SmallIcon and View.LargeIcon use Groups
			if (listView.View == SWF.View.SmallIcon 
			    || listView.View == SWF.View.LargeIcon) {

				SWF.ListViewItem listViewItem = (SWF.ListViewItem) objectItem;
				SWF.ListViewGroup listViewGroup = GetGroupFrom (listViewItem);
				ListViewGroupProvider groupProvider = null;

				if (groups.TryGetValue (listViewGroup, 
				                        out groupProvider) == false) {
					groupProvider = new ListViewGroupProvider (listViewGroup,
					                                           listView);
					groups [listViewGroup] = groupProvider;
						
					OnNavigationChildAdded (raiseEvent, groupProvider);
				}

				ListItemProvider item = GetItemProviderFrom (groupProvider, objectItem);
				groupProvider.AddChildProvider (raiseEvent, item);
			} else { // When View is SWF.View.List
				ListItemProvider item = GetItemProviderFrom (this, objectItem);
				OnNavigationChildAdded (raiseEvent, item);
			}
		}

		private void FinalizeProviderFrom (object objectItem, bool raiseEvent)
		{
			// View.SmallIcon and View.LargeIcon use Groups
			if (listView.View == SWF.View.SmallIcon 
			    || listView.View == SWF.View.LargeIcon) {

				SWF.ListViewItem listViewItem = (SWF.ListViewItem) objectItem;
				SWF.ListViewGroup listViewGroup = GetGroupFrom (listViewItem);

				ListViewGroupProvider groupProvider = null;

				if (groups.TryGetValue (listViewGroup, 
				                        out groupProvider) == true) {
					ListItemProvider item = GetItemProviderFrom (groupProvider, 
					                                             objectItem);
					groupProvider.RemoveChildProvider (raiseEvent, item);

					if (groupProvider.Navigate (NavigateDirection.FirstChild)
					    == null) {
						OnNavigationChildRemoved (raiseEvent, groupProvider);
						groups.Remove (listViewGroup);
						groupProvider.Terminate ();
					}
				}
			} else {
				ListItemProvider item = RemoveItemFrom (objectItem);
				OnNavigationChildRemoved (true, item);
			}
		}
		
		private void OnUIAViewChanged (object sender, EventArgs args)
		{
			// Behaviors supported no matter the View:
			// - Selection Behavior: Always supported
			// - MultipleView Behavior: Always supported
			// - Scroll Behavior: Set/Unset by ScrollBehaviorObserver.

			// Behaviors supported only by specific View:
			if (listView.View == SWF.View.Details || listView.View == SWF.View.List)
				SetBehavior (GridPatternIdentifiers.Pattern,
				             GetBehaviorRealization (GridPatternIdentifiers.Pattern));
			else
				SetBehavior (GridPatternIdentifiers.Pattern,
				             null);
			
			UpdateChildrenStructure ();
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
			
			foreach (object objectItem in listView.Items)
				InitializeProviderFrom (objectItem, updateView);
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
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					Rect bounds = GetItemsBoundingRectangle ();				
					Rect screen = Helper.RectangleToRect (SWF.Screen.GetWorkingArea (listView));
					// True if the *entire* control is off-screen
					return !screen.Contains (bounds.Left, bounds.Bottom) 
						&& !screen.Contains (bounds.Left, bounds.Top) 
						&& !screen.Contains (bounds.Right, bounds.Bottom) 
						&& !screen.Contains (bounds.Right, bounds.Top);
				} else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)// FIXME: Implement
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)// FIXME: Implement
					return false;
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
					return GetItemsBoundingRectangle ();
				else
					return base.GetPropertyValue (propertyId);
			}

			private Rect GetItemsBoundingRectangle ()
			{
				SWF.ListViewGroup defaultGroup
					= Helper.GetPrivateProperty<SWF.ListView, SWF.ListViewGroup> (typeof (SWF.ListView), 
					                                                              listView,
					                                                              "UIADefaultListViewGroup");
				ListViewProvider provider = (ListViewProvider) ProviderFactory.FindProvider (listView);
				Rect rect = Rect.Empty;
				
				if (defaultGroup == group) {
					for (int index = 0; index < listView.Items.Count; index++) {
						SWF.ListViewItem item = listView.Items [index];
						if (item.Group == null)							
							GetItemBoundingRectangle (provider, item, ref rect);
					}
				} else {					
					for (int index = 0; index < group.Items.Count; index++) {
						SWF.ListViewItem item = group.Items [index];
							
						GetItemBoundingRectangle (provider, item, ref rect);
					}
				}

				Rect headerRect = GetHeaderRectangle (group);
				headerRect.Union (rect);

				return headerRect;
			}
			
			private Rect GetHeaderRectangle (SWF.ListViewGroup group)
			{
				// Lets Union the Header Bounds
				MethodInfo methodInfo = typeof (SWF.ListView).GetMethod ("UIAGetHeaderBounds",
				                                                         BindingFlags.NonPublic
				                                                         | BindingFlags.Instance);
				Func<SWF.ListView, SWF.ListViewGroup, SD.Rectangle> method
					= (Func<SWF.ListView, SWF.ListViewGroup, SD.Rectangle>) Delegate.CreateDelegate (typeof (Func<SWF.ListView, SWF.ListViewGroup, SD.Rectangle>),
					                                                                                 methodInfo);
				
				
				SD.Rectangle headerRec = method (listView, group);
				SD.Rectangle rectangle = listView.Bounds;
				
				headerRec.X += rectangle.X;
				headerRec.Y += rectangle.Y;
				
				if (listView.FindForm () == listView.Parent)
					headerRec = listView.TopLevelControl.RectangleToScreen (headerRec);
				else
					headerRec = listView.Parent.RectangleToScreen (headerRec);
				
				return Helper.RectangleToRect (headerRec);
			}

			private Rect GetItemBoundingRectangle (ListViewProvider provider,
			                                       SWF.ListViewItem item, 
			                                       ref Rect rect)
			{
				//FIXME
//				ListItemProvider itemProvider 
//					= provider.GetItemProviderAt (this, item.Index);		
//				Rect rectangle 
//					= (Rect) itemProvider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
//				
//				rect.Union (rectangle);
//				
//				return rect;
				return Rect.Empty;
			}
			
			private SWF.ListView listView;
			private SWF.ListViewGroup group;
		}
		
		#endregion
		
	}
}
