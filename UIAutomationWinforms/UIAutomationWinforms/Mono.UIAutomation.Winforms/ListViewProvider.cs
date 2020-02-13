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
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.Unix;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.ListView;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ListView;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	
	[MapsComponent (typeof (SWF.ListView))]
	internal class ListViewProvider : ListProvider
	{
		#region Constructors

		public ListViewProvider (SWF.ListView listView) : base (listView)
		{
			this.listView = listView;

			lastView = listView.View;
			showGroups = listView.ShowGroups;
			groups = new Dictionary<SWF.ListViewGroup, ListViewGroupProvider> ();
		}
		
		#endregion
		
		#region SimpleControlProvider: Specializations

		public override void Initialize ()
		{
			base.Initialize ();

			//Event used to verify if groups is enabled so when can generate
			//a valid UIA tree
			listView.UIAShowGroupsChanged += OnUIAShowGroupsChanged;
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id) {
				if (listView.View == SWF.View.Details)
					return ControlType.DataGrid.Id;
				else
					return ControlType.List.Id;
			} else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else
				return base.GetProviderPropertyValue (propertyId);
		}
		
		#endregion
		
		#region ListProvider: Internal Methods: Get Behaviors
		
		internal override IProviderBehavior GetBehaviorRealization (AutomationPattern behavior)
		{
			// See comment in OnUIAViewChanged method
			// NOTE: observer sets/unsets ScrollPattern
			
			if (behavior == MultipleViewPatternIdentifiers.Pattern)
				return new MultipleViewProviderBehavior (this);
			else if (behavior == SelectionPatternIdentifiers.Pattern) 
				return new SelectionProviderBehavior (this);
			else if (behavior == GridPatternIdentifiers.Pattern) {
				if (listView.ShowGroups == false || SWF.Application.VisualStylesEnabled == false
				    || listView.View == SWF.View.List || listView.View == SWF.View.Details)
				    return new GridProviderBehavior (this);
				else
					return null;
			} else if (behavior == TablePatternIdentifiers.Pattern) {
				if (listView.View == SWF.View.Details)
					return new TableProviderBehavior (this);
				else
					return null;
			} else if (behavior == ScrollPatternIdentifiers.Pattern)
				return new ScrollProviderBehavior (this);
			else
				return null;
		}

		internal ListViewListItemProvider GetItem (SWF.ListViewItem item)
		{
			foreach (var provider in Navigation.GetAllChildren ()) {
				var itemProvider = provider as ListViewListItemProvider;
				if (itemProvider != null &&
				    itemProvider.ListViewItem == item)
					return itemProvider;
			}
			return null;
		}

		internal SWF.View View {
			get { return lastView; }
			set {
				if (listView.View != value) {
					listView.View = value;
					OnUIAViewChanged (this, EventArgs.Empty);
				}
			}
		}

		public override IProviderBehavior GetListItemBehaviorRealization (AutomationPattern behavior,
		                                                                  ListItemProvider listItem)
		{
			if (behavior == SelectionItemPatternIdentifiers.Pattern)
				return new ListItemSelectionItemProviderBehavior (listItem);
			else if (behavior == GridItemPatternIdentifiers.Pattern) {
				// LAMESPEC: GridItem implemented *only* when: listView.View != SWF.View.Details
				if (listView.View != SWF.View.Details
				    || IsBehaviorEnabled (GridPatternIdentifiers.Pattern))
					return new ListItemGridItemProviderBehavior (listItem);
				else
					return null;
			} else if (behavior == InvokePatternIdentifiers.Pattern && Control is SWF.MWFFileView) {
				return new ListItemInvokeProviderBehavior (listItem);
			} else if (behavior == ValuePatternIdentifiers.Pattern) {
				if (listView.LabelEdit == true)
					return new ListItemValueProviderBehavior (listItem);
				else
					return null;
			} else if (behavior == TogglePatternIdentifiers.Pattern) {
				if (listView.CheckBoxes == true)
					return new ListItemToggleProviderBehavior (listItem);
				else
					return null;
			} else if (behavior == TableItemPatternIdentifiers.Pattern) {
				if (listView.View == SWF.View.Details)
					return new ListItemTableItemProviderBehavior (listItem);
				else
					return null;
			} else if (behavior == EmbeddedImagePatternIdentifiers.Pattern)
				return new ListItemEmbeddedImageProviderBehavior (listItem);
			else if (behavior == ClipboardPatternIdentifiers.Pattern)
				return new ListItemClipboardProviderBehavior (listItem);
			else
				return base.GetListItemBehaviorRealization (behavior, listItem);
		}
		
		#endregion
		
		#region ListItem: Properties Methods

		public override object GetItemPropertyValue (ListItemProvider item,
		                                             int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return ((SWF.ListViewItem) item.ObjectItem).Text;
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return listView.Focused && ((SWF.ListViewItem)item.ObjectItem).Focused;
			else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
				int index = item.Index;
				if (index == -1 || index >= listView.UIAItemsLocationLength)
					return Helper.RectangleToRect (SD.Rectangle.Empty);

				SD.Rectangle itemRec = listView.GetItemRect (index);
				SD.Rectangle rectangle = listView.Bounds;
				
				itemRec.X += rectangle.X;
				itemRec.Y += rectangle.Y;
				
				itemRec = listView.Parent.RectangleToScreen (itemRec);
				
				return Helper.RectangleToRect (itemRec);
			} else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
				return Helper.IsListItemOffScreen ((Rect) item.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id),
				                                   listView, 
				                                   HeaderProvider != null,
				                                   listView.UIAHeaderControl,
				                                   ScrollBehaviorObserver);
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else
				return null;
		}
		
		#endregion 
		
		#region ListProvider specializations
		
		public override void Terminate ()
		{
			base.Terminate ();

			listView.UIAShowGroupsChanged -= OnUIAShowGroupsChanged;
		}
		
		protected override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			listView.Items.UIACollectionChanged += OnCollectionChanged;

			// Use to regenerate children when view changes
			listView.UIAViewChanged += OnUIAViewChanged;
			UpdateChildrenStructure (listView.Items.Count > 0);
		}
		
		protected override void FinalizeChildControlStructure()
		{
			base.FinalizeChildControlStructure ();

			listView.Items.UIACollectionChanged -= OnCollectionChanged;

			listView.UIAViewChanged -= OnUIAViewChanged;
		}
		
		public override int ItemsCount { 
			get { return listView.Items.Count; }
		}

		public override int IndexOfObjectItem (object objectItem)
		{
			return listView.Items.IndexOf (objectItem as SWF.ListViewItem);
		}
		
		public override void FocusItem (object objectItem)
		{
			((SWF.ListViewItem)objectItem).Focused = true;
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
		
		public override IRawElementProviderSimple[] GetSelectedItems ()
		{
			if (listView.SelectedIndices.Count == 0)
				return new ListItemProvider [0];
			else {
				ListItemProvider []providers = new ListItemProvider [listView.SelectedItems.Count];

				for (int index = 0; index < listView.SelectedItems.Count; index++)
					providers [index] = GetItemProviderFrom (this, listView.SelectedItems [index], false);
			
				return providers;
			}
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
			if (listView.CheckBoxes == false || item.Index == -1)
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

		public override IConnectable GetListItemEventRealization (ProviderEventType eventType, 
		                                                          ListItemProvider provider)
		{
			if (eventType == ProviderEventType.AutomationElementHasKeyboardFocusProperty)
			    return new ListItemAutomationHasKeyboardFocusPropertyEvent (provider);
			else if (eventType == ProviderEventType.AutomationElementIsOffscreenProperty)
			    return new ListItemAutomationIsOffscreenPropertyEvent (provider);
			// FIXME: ProviderEventType.AutomationElementBoundingRectangleProperty: 
			// When setting/unsetting image
			else
				return base.GetListItemEventRealization (eventType, provider);
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

		protected override void OnCollectionChanged (object sender, 
		                                             CollectionChangeEventArgs args)
		{		
			if (args.Action == CollectionChangeAction.Add)
				InitializeProviderFrom (args.Element);
			else if (args.Action == CollectionChangeAction.Remove)
				FinalizeProviderFrom (args.Element);
			else
				base.OnCollectionChanged (sender, args);
		}

		protected override ListItemProvider GetNewItemProvider (FragmentRootControlProvider rootProvider,
		                                                        ListProvider provider,
		                                                        SWF.Control control,
		                                                        object objectItem)
		{
			return new ListViewListItemProvider (rootProvider,
			                                     this,
			                                     listView,
			                                     (SWF.ListViewItem) objectItem);
		}
		
		#endregion

		#region Scroll Methods and Properties
		
		protected override SWF.ScrollBar HorizontalScrollBar { 
			get { return listView.UIAHScrollBar; }
		}

		protected override SWF.ScrollBar VerticalScrollBar { 
			get { return listView.UIAVScrollBar; }
		}

		#endregion

		#region Public Methods

		public ListViewHeaderProvider HeaderProvider {
			get { return header; }
		}

		public bool IsDefaultGroup (SWF.ListViewGroup group)
		{
			return GetDefaultGroup () == group;
		}

		public SWF.ListViewGroup GetDefaultGroup ()
		{
			if (listViewNullGroup == null)
				listViewNullGroup = listView.UIADefaultListViewGroup;

			return listViewNullGroup;
		}

		public SWF.ListViewGroup GetGroupFrom (SWF.ListViewItem item)
		{
			if (item.Group == null)
				return GetDefaultGroup ();
			else
				return item.Group;
		}

		public ListViewGroupProvider GetGroupProviderFrom (SWF.ListViewGroup group)
		{
			ListViewGroupProvider provider = null;
			groups.TryGetValue (group ?? GetDefaultGroup (), out provider);

			return provider;
		}

		#endregion
		
		#region Private Methods
		
		private void InitializeProviderFrom (object objectItem)
		{
			// FIXME: In Vista when listView.Groups == 0 && View.Details 
			// no Groups are added.

			// Using groups
			if (showGroups == true && listView.View != SWF.View.List
			    && SWF.Application.VisualStylesEnabled == true) {

				SWF.ListViewItem listViewItem = (SWF.ListViewItem) objectItem;
				SWF.ListViewGroup listViewGroup = GetGroupFrom (listViewItem);
				ListViewGroupProvider groupProvider = null;

				if (groups.TryGetValue (listViewGroup, 
				                        out groupProvider) == false) {
					groupProvider = new ListViewGroupProvider (listViewGroup,
					                                           listView);
					groups [listViewGroup] = groupProvider;
					groupProvider.Initialize ();
					AddChildProvider (groupProvider);
				}

				ListItemProvider item = GetItemProviderFrom (this, objectItem, false);
				if (item != null) {
					// Probably an item switching groups
					ListViewGroupProvider oldGroupProvider = (ListViewGroupProvider)item.Navigate (NavigateDirection.Parent);
					if (oldGroupProvider != groupProvider) {
						oldGroupProvider.RemoveItemFrom (item);
						RemoveItemFrom (objectItem);
						if (oldGroupProvider.ChildrenCount == 0) {
							RemoveChildProvider (oldGroupProvider);
							groups.Remove (oldGroupProvider.Group);
							oldGroupProvider.Terminate ();
						}
					}
				}

				item = GetItemProviderFrom (groupProvider, objectItem);
				groupProvider.AddChildProvider (item);
			// Not using groups
			} else {
				ListItemProvider item = GetItemProviderFrom (this, objectItem);
				AddChildProvider (item);
			}
		}

		private void FinalizeProviderFrom (object objectItem)
		{
			// Using groups
			if (showGroups == true && listView.View != SWF.View.List
			    && SWF.Application.VisualStylesEnabled == true) {

				SWF.ListViewItem listViewItem = (SWF.ListViewItem) objectItem;
				SWF.ListViewGroup listViewGroup = GetGroupFrom (listViewItem);

				ListViewGroupProvider groupProvider = null;

				if (groups.TryGetValue (listViewGroup, 
				                        out groupProvider) == true) {
					ListItemProvider item = GetItemProviderFrom (groupProvider, 
					                                             objectItem);
					groupProvider.RemoveChildProvider (item);

					if (groupProvider.ChildrenCount == 0) {
						RemoveChildProvider (groupProvider);
						groups.Remove (listViewGroup);
						groupProvider.Terminate ();
					}
				}
			// Not using groups
			} else {
				ListItemProvider item = RemoveItemFrom (objectItem);
				RemoveChildProvider (item);
			}
		}

		private void UpdateChildrenStructure (bool forceUpdate)
		{
			bool updateView = lastView != listView.View;
			
			if (updateView == true || forceUpdate == true) {			
				foreach (ListViewGroupProvider groupProvider in groups.Values) {
					RemoveChildProvider (groupProvider);
					groupProvider.Terminate ();
				}
				groups.Clear ();

				if (lastView == SWF.View.Details && header != null) {
					RemoveChildProvider (header);
					header.Terminate ();
					header = null;
				}

				foreach (ListItemProvider itemProvider in Items)
					RemoveChildProvider (itemProvider);
				
				ClearItemsList ();
			}

			if (listView.View == SWF.View.Details) {
				if (header == null) {
					header = new ListViewHeaderProvider (listView);
					header.Initialize ();
					AddChildProvider (header, updateView || forceUpdate);
				}
			}
			
			foreach (object objectItem in listView.Items)
				InitializeProviderFrom (objectItem);
		}

		private void OnUIAViewChanged (object sender, EventArgs args)
		{
			// Behaviors supported no matter the View:
			// - Selection Behavior: Always supported
			// - MultipleView Behavior: Always supported
			// - Scroll Behavior: Set/Unset by ScrollBehaviorObserver.

			// Behaviors supported only by specific View:
			SetBehavior (GridPatternIdentifiers.Pattern,
			             GetBehaviorRealization (GridPatternIdentifiers.Pattern));
			SetBehavior (TablePatternIdentifiers.Pattern,
			             GetBehaviorRealization (TablePatternIdentifiers.Pattern));
			
			UpdateChildrenStructure (false);

			lastView = listView.View;
		}

		private void OnUIAShowGroupsChanged (object sender, EventArgs args)
		{
			bool oldValue = showGroups;
			showGroups = listView.ShowGroups;
			
			//We will have to regenerate children
			if (listView.ShowGroups != oldValue && listView.View != SWF.View.List) {
				UpdateChildrenStructure (true);
				if (SWF.Application.VisualStylesEnabled == true)
					SetBehavior (GridPatternIdentifiers.Pattern,
					             GetBehaviorRealization (GridPatternIdentifiers.Pattern));
			}
		}

		#endregion
		
		#region Private Fields
		
		private SWF.View lastView;
		private SWF.ListView listView;
		private Dictionary<SWF.ListViewGroup, ListViewGroupProvider> groups;
		private SWF.ListViewGroup listViewNullGroup;
		private ListViewHeaderProvider header;
		private bool showGroups;
		
		#endregion
		
		#region Internal Class: ScrollBar provider

		internal class ListViewScrollBarProvider : ScrollBarProvider
		{
			public ListViewScrollBarProvider (SWF.ScrollBar scrollbar,
			                                  SWF.ListView listView)
				: base (scrollbar)
			{
				this.listView = listView;
				name = scrollbar is SWF.HScrollBar ? Catalog.GetString ("Horizontal Scroll Bar")
					: Catalog.GetString ("Vertical Scroll Bar");
			}
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { 
					return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (listView);
				}
			}			
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return name;
				else
					return base.GetProviderPropertyValue (propertyId);
			}
			
			private SWF.ListView listView;
			private string name;

		} // ListViewScrollBarProvider
		
		#endregion
		
		#region Internal Class: Group Provider 
		
		internal class ListViewGroupProvider : GroupBoxProvider
		{
			
			public ListViewGroupProvider (SWF.ListViewGroup group,
			                              SWF.ListView listView) : base (null)
			{
				this.group = group;
				this.listView = listView;
			}

			public override void Initialize ()
			{
				base.Initialize ();

				SetBehavior (GridPatternIdentifiers.Pattern,
				             new GroupGridProviderBehavior (this));
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
				             new GroupExpandCollapseProviderBehavior (this));
			}

			public SWF.ListViewGroup Group {
				get { return group; }
			}

			public SWF.ListView ListView {
				get { return listView; }
			}
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { 
					return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (ListView);
				}
			}

			public ListViewListItemProvider GetItem (SWF.ListViewItem item) 
			{
				foreach (ListViewListItemProvider provider in Navigation.GetAllChildren ()) {
					if (provider.ListViewItem == item)
						return provider;
				}
				return null;
			}
		
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return group.Header;
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
					return Helper.IsOffScreen (ItemsBoundingRectangle, listView, true);
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
					return true;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get { return ItemsBoundingRectangle; }
			}

			private Rect HeaderRectangle {
				get {
					// Lets Union the Header Bounds			
					SD.Rectangle headerRec = listView.UIAGetHeaderBounds (group);
					SD.Rectangle rectangle = listView.Bounds;
					
					headerRec.X += rectangle.X;
					headerRec.Y += rectangle.Y;
					
					if (listView.FindForm () == listView.Parent)
						headerRec = listView.TopLevelControl.RectangleToScreen (headerRec);
					else
						headerRec = listView.Parent.RectangleToScreen (headerRec);
					
					return Helper.RectangleToRect (headerRec);
				}
			}

			private Rect ItemsBoundingRectangle {
				get {
					Rect rect = Rect.Empty;
	
					foreach (ListViewListItemProvider itemProvider in Navigation.GetAllChildren ()) {
						Rect rectangle 
							= (Rect) itemProvider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
						rect.Union (rectangle);
					}
	
					Rect headerRect = HeaderRectangle;
					
					headerRect.Union (rect);
	
					return headerRect;
				}
			}						

			internal void RemoveItemFrom (ListItemProvider child)
			{
				RemoveChildProvider (child);
				child.Terminate ();
			}

			private SWF.ListView listView;
			private SWF.ListViewGroup group;

		} // ListViewGroupProvider
		
		#endregion

		#region Internal Class: Header Provider 
		
		internal class ListViewHeaderProvider : FragmentRootControlProvider
		{
			public ListViewHeaderProvider (SWF.ListView view) : base (null)
			{
				listView = view;
				headerItems = new Dictionary<SWF.ColumnHeader, ListViewHeaderItemProvider> ();
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { 
					return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (listView); 
				}
			}

			public SWF.ListView ListView {
				get { return listView; }
			}

			public override SWF.Control AssociatedControl {
				get { return ListView; }
			}

			public ListViewHeaderItemProvider GetHeaderItemFrom (SWF.ColumnHeader column)
			{
				ListViewHeaderItemProvider headerItem;
				headerItems.TryGetValue (column, out headerItem);
				return headerItem;
			}

			public IRawElementProviderSimple[] GetHeaderItems ()
			{
				if (ChildrenCount == 0)
					return new IRawElementProviderSimple [0];

				IRawElementProviderSimple []headerItems = new IRawElementProviderSimple [ChildrenCount];
				for (int index = 0; index < ChildrenCount; index++)
					headerItems [index] = Navigation.GetVisibleChild (index);

				return headerItems;
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Header.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return "Header";
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.OrientationProperty.Id)
					return OrientationType.Horizontal;
				else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
					return true;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get {
					return Helper.GetControlScreenBounds (listView.UIAHeaderControl, listView, true);
				}
			}

			protected override void InitializeChildControlStructure ()
			{
				base.InitializeChildControlStructure ();

				//Event used to update columns in ListItem when View.Details
				listView.Columns.UIACollectionChanged += OnUIAColumnsCollectionChanged;

				foreach (SWF.ColumnHeader column in listView.Columns) {
					ListViewHeaderItemProvider item 
						= new ListViewHeaderItemProvider (this, column);
					item.Initialize ();
					AddChildProvider (item);
					headerItems [column] = item;
				}
			}

			protected override void FinalizeChildControlStructure()
			{
				base.FinalizeChildControlStructure ();

				//Event used to update columns in ListItem when View.Details
				listView.Columns.UIACollectionChanged -= OnUIAColumnsCollectionChanged;

				foreach (ListViewHeaderItemProvider item in headerItems .Values)
					item.Terminate ();
			}

			private void OnUIAColumnsCollectionChanged (object sender, 
			                                            CollectionChangeEventArgs args)
			{				
				SWF.ColumnHeader column = (SWF.ColumnHeader) args.Element;
				
				if (args.Action == CollectionChangeAction.Add) {
					ListViewHeaderItemProvider itemProvider
						= new ListViewHeaderItemProvider (this, column);
					itemProvider.Initialize ();
					headerItems [column] = itemProvider;
					AddChildProvider (itemProvider);
				} else if (args.Action == CollectionChangeAction.Remove) {
					ListViewHeaderItemProvider itemProvider;
					if (headerItems.TryGetValue (column, out itemProvider)) {
						RemoveChildProvider (itemProvider);
						itemProvider.Terminate ();
						headerItems.Remove (column);
					}
				} else {
					foreach (ListViewHeaderItemProvider item in headerItems .Values)
						item.Terminate ();
					headerItems.Clear ();

					OnNavigationChildrenCleared ();
				}
			}

			private Dictionary<SWF.ColumnHeader, ListViewHeaderItemProvider> headerItems;
			private SWF.ListView listView;
			
		} // ListViewHeaderProvider

		#endregion

		#region Internal Class: Header Provider 
		
		internal class ListViewHeaderItemProvider : FragmentControlProvider
		{
			public ListViewHeaderItemProvider (ListViewHeaderProvider headerProvider,
			                                   SWF.ColumnHeader columnHeader) 
				: base (columnHeader)
			{
				this.headerProvider = headerProvider;
				this.columnHeader = columnHeader;
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return headerProvider; }
			}

			public ListViewHeaderProvider HeaderProvider {
				get { return headerProvider; }
			}

			public SWF.ColumnHeader ColumnHeader {
				get { return columnHeader; }
			}

			public override SWF.Control AssociatedControl {
				get { return HeaderProvider.ListView; }
			}
			
			public override void Initialize ()
			{
				base.Initialize ();

				SetBehavior (InvokePatternIdentifiers.Pattern,
				             new HeaderItemInvokeProvider (this));

				// Automation Events
				SetEvent (ProviderEventType.AutomationElementIsOffscreenProperty,
				          new HeaderItemAutomationIsOffScreenPropertyEvent (this));
				SetEvent (ProviderEventType.AutomationElementBoundingRectangleProperty,
				          new HeaderItemAutomationBoundingRectanglePropertyEvent (this));
				SetEvent (ProviderEventType.AutomationElementNameProperty,
				          new HeaderItemAutomationNamePropertyEvent (this));
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.HeaderItem.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return columnHeader.Text;
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.OrientationProperty.Id)
					return OrientationType.Horizontal;
				else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					Rect bounds 
						= (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					return Helper.IsOffScreen (bounds, headerProvider.ListView, true);
				} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get {
					int indexOf = headerProvider.ListView.Columns.IndexOf (columnHeader);
					Rect headerBounds
						= (Rect) headerProvider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					if (headerBounds.IsEmpty)
						return headerBounds;
					
					for (int index = 0; index < indexOf; index++)
						headerBounds.X += headerProvider.ListView.Columns [index].Width;
					
					headerBounds.Width = headerProvider.ListView.Columns [indexOf].Width;
					
					return headerBounds;
				} 
			}

			private ListViewHeaderProvider headerProvider;
			private SWF.ColumnHeader columnHeader;
			
		} // ListViewHeaderItemProvider

		#endregion

		#region Internal Class: ListItem Provider

		internal class ListViewListItemProvider : ListItemProvider
		{
			public ListViewListItemProvider (FragmentRootControlProvider rootProvider,
			                                 ListViewProvider listViewProvider, 
			                                 SWF.ListView listView,
			                                 SWF.ListViewItem listViewItem)
				: base (rootProvider, listViewProvider, listView, listViewItem)
			{
				this.listView = listView;
				this.listViewProvider = listViewProvider;
				lastView = listView.View;
				this.item = listViewItem;

				providers = new Dictionary<SWF.ColumnHeader, ListViewListItemEditProvider> ();
			}

			public SWF.ListView ListView {
				get { return listView; }
			}

			public ListViewProvider ListViewProvider {
				get { return listViewProvider; }
			}

			public SWF.ListViewItem ListViewItem {
				get { return item; }
			}

			public ListViewListItemEditProvider GetEditProviderAtColumn (int column)
			{
				if (column < 0 || column >= listView.Columns.Count)
					return null;

				ListViewListItemEditProvider editProvider = null;				
				providers.TryGetValue (listView.Columns [column], out editProvider);

				return editProvider;
			}

			public IRawElementProviderFragment CheckboxProvider {
				get { return checkboxProvider; }
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (ListView.View == SWF.View.Details) {
					//According to: http://msdn.microsoft.com/en-us/library/ms742561.aspx
					if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
						return ControlType.DataItem.Id;
					//FIXME: What about ItemTypeProperty & ItemStatusProperty ?
					else
						return base.GetProviderPropertyValue (propertyId);
				} else
					return base.GetProviderPropertyValue (propertyId);
			}

			public override void Initialize ()
			{
				base.Initialize ();

				//Event handlers to update Toggle and Value Behavior in ListItem
				listView.UIACheckBoxesChanged += OnUIACheckBoxesChanged;
				listView.UIALabelEditChanged += OnUIALabelEditChanged;

				//Event used to update children in ListItem when View.Details
				listView.Columns.UIACollectionChanged += OnUIAColumnsCollectionChanged;

				//Used to add children when View.Details
				listView.UIAViewChanged += OnUIAViewChanged;

				//Use to update our GridItem and TableItem providers
				listViewProvider.ProviderBehaviorSet += OnProviderBehaviorSet;
			}

			protected override void InitializeChildControlStructure ()
			{
				base.InitializeChildControlStructure ();

				if (lastView == SWF.View.Details)
					AddEditChildren ();

				if (listView.CheckBoxes == true) {
					checkboxProvider = new ListViewListItemCheckBoxProvider (this);
					checkboxProvider.Initialize ();
					AddChildProvider (checkboxProvider);
				}
			}

			public override void Terminate ()
			{
				base.Terminate ();

				listView.UIACheckBoxesChanged -= OnUIACheckBoxesChanged;
				listView.UIALabelEditChanged -= OnUIALabelEditChanged;

				listView.Columns.UIACollectionChanged -= OnUIAColumnsCollectionChanged;
				
				listView.UIAViewChanged -= OnUIAViewChanged;

				listViewProvider.ProviderBehaviorSet -= OnProviderBehaviorSet;
			}

			private void AddEditChildren ()
			{
				foreach (SWF.ColumnHeader column in listView.Columns) {
					ListViewListItemEditProvider editProvider 
						= new ListViewListItemEditProvider (column, this);
					editProvider.Initialize ();
					providers [column] = editProvider;

					AddChildProvider (editProvider);
				}
			}
	
			private void OnUIACheckBoxesChanged (object sender, EventArgs args)
			{
				UpdateBehavior (TogglePatternIdentifiers.Pattern);

				if (checkboxProvider == null) {
					checkboxProvider = new ListViewListItemCheckBoxProvider (this);
					checkboxProvider.Initialize ();
					AddChildProvider (checkboxProvider);
				} else {
					checkboxProvider.Terminate ();
					RemoveChildProvider (checkboxProvider);
					checkboxProvider = null;
				}
			}
	
			private void OnUIALabelEditChanged (object sender, EventArgs args)
			{
				UpdateBehavior (ValuePatternIdentifiers.Pattern);
				UpdateBehavior (ClipboardPatternIdentifiers.Pattern);
			}

			private void OnUIAViewChanged (object sender, EventArgs args)
			{
				if (lastView == SWF.View.Details) {
					providers.Clear ();
					OnNavigationChildrenCleared ();
				} else if (listView.View == SWF.View.Details)
					AddEditChildren ();
	
				lastView = listView.View;
			}

			private void OnUIAColumnsCollectionChanged (object sender, 
			                                            CollectionChangeEventArgs args)
			{
				if (listView.View != SWF.View.Details)
					return;

				SWF.ColumnHeader column = (SWF.ColumnHeader) args.Element;

				if (args.Action == CollectionChangeAction.Add) {
					ListViewListItemEditProvider editProvider 
						= new ListViewListItemEditProvider (column, this);
					editProvider.Initialize ();
					providers [column] = editProvider;
					AddChildProvider (editProvider);
				} else if (args.Action == CollectionChangeAction.Remove) {
					ListViewListItemEditProvider editProvider;
					if (providers.TryGetValue (column, out editProvider)) {
						RemoveChildProvider (editProvider);
						editProvider.Terminate ();
						providers.Remove (column);
					}
				} else {
					foreach (ListViewListItemEditProvider provider in providers.Values)
						provider.Terminate ();
					providers.Clear ();

					OnNavigationChildrenCleared ();
				}
			}

			private void OnProviderBehaviorSet (object sender, ProviderBehaviorEventArgs args)
			{
				AutomationPattern pattern = null;
				if (args.Pattern == TablePatternIdentifiers.Pattern)
					pattern = TableItemPatternIdentifiers.Pattern;
				else if (args.Pattern == GridPatternIdentifiers.Pattern)
					pattern = GridItemPatternIdentifiers.Pattern;
				else
					return;

				SetBehavior (pattern,
				             ListProvider.GetListItemBehaviorRealization (pattern, 
				                                                          this));
			}

			private SWF.ListViewItem item;
			private Dictionary<SWF.ColumnHeader, ListViewListItemEditProvider> providers;
			private SWF.View lastView;
			private SWF.ListView listView;
			private ListViewProvider listViewProvider;
			private ListViewListItemCheckBoxProvider checkboxProvider;
		} //ListViewListItemProvider

		#endregion

		#region Internal Class: ListItem Edit Provider 

		internal class ListViewListItemEditProvider : FragmentControlProvider
		{
			public ListViewListItemEditProvider (SWF.ColumnHeader header,
			                                     ListViewListItemProvider itemProvider)
				: base (null)
			{
				this.header = header;
				this.itemProvider = itemProvider;
			}

			public SWF.ColumnHeader ColumnHeader {
				get { return header; }
			}

			public ListViewListItemProvider ItemProvider {
				get { return itemProvider; }
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return itemProvider; }
			}

			public override SWF.Control AssociatedControl {
				get { return ItemProvider.ListView; }
			}

			public override void Initialize ()
			{
				base.Initialize ();

				//LAMESPEC: Vista does not support Text Pattern.
				//http://msdn.microsoft.com/en-us/library/ms748367.aspx

				SetBehavior (GridItemPatternIdentifiers.Pattern,
				             new ListItemEditGridItemProviderBehavior (this));
				SetBehavior (TableItemPatternIdentifiers.Pattern,
				             new ListItemEditTableItemProviderBehavior (this));
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new ListItemEditValueProviderBehavior (this));

				// Automation Events
				SetEvent (ProviderEventType.AutomationElementIsOffscreenProperty,
				          new ListItemEditAutomationIsOffscreenPropertyEvent (this));
				SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
				          new ListItemEditAutomationHasKeyboardFocusPropertyEvent (this));
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Edit.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id) {
					IValueProvider valueProvider = (IValueProvider) GetBehavior (ValuePatternIdentifiers.Pattern);
					return valueProvider.Value;
				} else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return IsFirstColumn || itemProvider.ListView.FullRowSelect;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id) {
					if (IsFirstColumn)
						return itemProvider.GetPropertyValue (propertyId);
					else
						return false;
				} else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.HelpTextProperty.Id) {
					if (ItemProvider.ListView.Columns.Count == 0 || ItemProvider.ListView.Columns [0] == header)
						return string.Empty;
					else
						return itemProvider.ListViewItem.ToolTipText;
				} else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
					return Helper.IsListItemOffScreen ((Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id),
					                                   ItemProvider.ListView, 
					                                   true,
					                                   ItemProvider.ListView.UIAHeaderControl,
					                                   ItemProvider.ListViewProvider.ScrollBehaviorObserver);
				else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get {
					int indexOf = itemProvider.ListView.Columns.IndexOf (header);
					Rect itemBounds
						= (Rect) itemProvider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					if (itemBounds.IsEmpty)
						return itemBounds;

					for (int index = 0; index < indexOf; index++)
						itemBounds.X += itemProvider.ListView.Columns [index].Width;
					
					itemBounds.Width = itemProvider.ListView.Columns [indexOf].Width;
					
					return itemBounds;
				}
			}

			bool IsFirstColumn {
				get { return header.Index == 0; }
			}
			private ListViewListItemProvider itemProvider;
			private SWF.ColumnHeader header;
		} //ListViewListItemEditProvider

		#endregion

		#region Internal Class: ListItem CheckBox Provider

		internal class ListViewListItemCheckBoxProvider : FragmentControlProvider
		{
			public ListViewListItemCheckBoxProvider (ListViewListItemProvider itemProvider)
				: base (null)
			{
				this.itemProvider = itemProvider;
			}

			public ListViewListItemProvider ItemProvider {
				get { return itemProvider; }
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return itemProvider; }
			}

			public override SWF.Control AssociatedControl {
				get { return ItemProvider.ListView; }
			}

			public override void Initialize ()
			{
				base.Initialize ();

				SetBehavior (TogglePatternIdentifiers.Pattern,
				             new ListItemCheckBoxToggleProviderBehavior (this));

				//Automation Events
				SetEvent (ProviderEventType.AutomationElementIsOffscreenProperty,
				          new ListItemCheckBoxAutomationIsOffscreenPropertyEvent (this));
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.CheckBox.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return itemProvider.GetPropertyValue (propertyId);
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					return Helper.IsListItemOffScreen ((Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id),
					                                   ItemProvider.ListView, 
					                                   true,
					                                   ItemProvider.ListView.UIAHeaderControl,
					                                   ItemProvider.ListViewProvider.ScrollBehaviorObserver);
//					Rect bounds 
//						= (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
//					return Helper.IsOffScreen (bounds, ItemProvider.ListView, true); 
				} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get {
					SD.Size checkBoxSize = ItemProvider.ListView.CheckBoxSize;
					Rect itemSize 
						= (Rect) ItemProvider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					if (itemSize.IsEmpty)
						return itemSize;
					
					Rect checkBoxRect = itemSize;
					checkBoxRect.Y = itemSize.Y + (itemSize.Height / 2) - (checkBoxSize.Height / 2);
					checkBoxRect.Width = checkBoxSize.Width;
					checkBoxRect.Height = checkBoxSize.Height;
					return checkBoxRect;
				}
			}

			private ListViewListItemProvider itemProvider;
		} //ListViewListItemEditProvider

		#endregion
		
	}
}
