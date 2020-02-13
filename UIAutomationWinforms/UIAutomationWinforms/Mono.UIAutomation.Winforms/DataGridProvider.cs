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
using Mono.Unix;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.DataGrid;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.DataGrid;

namespace Mono.UIAutomation.Winforms
{

	[MapsComponent (typeof (SWF.DataGrid))]
	internal class DataGridProvider 
		: FragmentRootControlProvider, IListProvider, IScrollBehaviorSubject
	{
		#region Public Constructors
		
		public DataGridProvider (SWF.DataGrid datagrid) : base (datagrid)
		{
			this.datagrid = datagrid;
			items = new List<ListItemProvider> ();
		}

		#endregion

		#region Public Properties

		public SWF.DataGridTableStyle CurrentTableStyle {
			get { return datagrid.UIACurrentTableStyle; }
		}

		public SWF.CurrencyManager CurrencyManager {
			get { return lastCurrencyManager; }
		}		

		public SWF.DataGrid DataGrid {
			get { return datagrid; }
		}

		public DataGridHeaderProvider HeaderProvider {
			get { return header; }
		}

		#endregion

		#region Public Methods

		public IRawElementProviderSimple[] GetSelectedItems ()
		{
			if (SelectedItemsCount == 0)
				return new IRawElementProviderSimple [0];
			else {
				List<IRawElementProviderSimple> selection = new List<IRawElementProviderSimple> ();
				foreach (ListItemProvider item in items) {
					if (IsItemSelected (item)) {
						// We are returning the children of the items
						foreach (IRawElementProviderSimple child in item.Navigation.GetAllChildren ())
							selection.Add (child);
					}
				}
				return selection.ToArray ();
			}
		}

		public IRawElementProviderSimple GetChildProviderAt (int row, int column)
		{
			int rowCount = CurrencyManager == null ? 0 : CurrencyManager.Count;
			int columnCount = CurrentTableStyle == null ? 0 : CurrentTableStyle.GridColumnStyles.Count;

			//According to http://msdn.microsoft.com/en-us/library/ms743401.aspx
			if (row < 0 || column < 0 || row >= rowCount || column >= columnCount)
			    throw new ArgumentOutOfRangeException ();

			return items [row].Navigation.GetChild (column);
		}

		#endregion

		#region IScrollBehaviorSubject specialization

		public IScrollBehaviorObserver ScrollBehaviorObserver { 
			get { return observer; }
		}
		
		public FragmentControlProvider GetScrollbarProvider (SWF.ScrollBar scrollbar)
		{
			return new DataGridScrollBarProvider (scrollbar, this);
		}

		#endregion

		#region SimpleControlProvider specialization

		public override void Initialize ()
		{
			base.Initialize ();

			//ListScrollBehaviorObserver updates Navigation
			observer = new ScrollBehaviorObserver (this, 
			                                       datagrid.UIAHScrollBar, 
			                                       datagrid.UIAVScrollBar);
			observer.ScrollPatternSupportChanged += OnScrollPatternSupportChanged;
			UpdateScrollBehavior ();

			SetBehavior (SelectionPatternIdentifiers.Pattern,
			             new SelectionProviderBehavior (this));
			SetBehavior (GridPatternIdentifiers.Pattern,
			             new GridProviderBehavior (this));
			
			// Table Pattern is *only* supported when Header exists
			if (datagrid.CurrentTableStyle.GridColumnStyles.Count > 0
			    && datagrid.ColumnHeadersVisible)
				CreateHeader (datagrid.CurrentTableStyle);
		}

		public override void Terminate ()
		{
			base.Terminate ();

			observer.ScrollPatternSupportChanged -= OnScrollPatternSupportChanged;
		}

		protected override void InitializeChildControlStructure ()
		{
			datagrid.DataSourceChanged += OnDataSourceChanged;
			UpdateChildren ();
			
			datagrid.UIACollectionChanged += OnUIACollectionChanged;
		}

		protected override void FinalizeChildControlStructure()
		{
			OnNavigationChildrenCleared ();
			datagrid.DataSourceChanged -= OnDataSourceChanged;
			datagrid.UIACollectionChanged -= OnUIACollectionChanged;
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			// NOTDOTNET: Using DataGrid instead of Table
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.DataGrid.Id;
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else
				return base.GetProviderPropertyValue (propertyId);
		}

		#endregion

		#region ScrollBehaviorObserver Methods
		
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
			if (ScrollPatternSupportChanged != null)
				ScrollPatternSupportChanged ();
		}

		#endregion

		#region IListProvider realization

		public int IndexOfObjectItem (object objectItem)
		{
			if (lastCurrencyManager == null)
				return -1;

			return (int) objectItem;
		}

		public void FocusItem (object objectItem)
		{
			int row = (int) objectItem;
			datagrid.Focus ();
			datagrid.CurrentCell = new SWF.DataGridCell (row, 0);
		}

		public int SelectedItemsCount {
			get { return datagrid.UIASelectedRows; }
		}

		public void ScrollItemIntoView (ListItemProvider item)
		{
			// FIXME: Implement:
		}

		public bool IsItemSelected (ListItemProvider item)
		{
			if (!items.Contains (item))
				return false;
			
			return datagrid.IsSelected (item.Index);
		}

		public void SelectItem (ListItemProvider item)
		{
			if (items.Contains (item))
				datagrid.Select (item.Index);
		}

		public void UnselectItem (ListItemProvider item)
		{
			if (items.Contains (item))
				datagrid.UnSelect (item.Index);
		}

		public object GetItemPropertyValue (ListItemProvider item,
		                                    int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
				int index = item.Index;
				// We need to union last column and first column
				SD.Rectangle rect0 = datagrid.GetCellBounds (index, 0);
				SD.Rectangle rectN = datagrid.GetCellBounds (index, header.ChildrenCount);
				return Helper.GetControlScreenBounds (SD.Rectangle.Union (rect0, rectN), datagrid);
			} else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
				Rect bounds 
					= (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
				return Helper.IsOffScreen (bounds, datagrid, true);
			} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
				return Helper.GetClickablePoint (this);
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id) {
				SWF.DataGridCell currentCell = datagrid.CurrentCell;
				return currentCell.ColumnNumber == 0 && currentCell.RowNumber == item.Index
					&& datagrid.Focused;
			} else
				return null;
		}

		public ToggleState GetItemToggleState (ListItemProvider item)
		{
			if (items.Contains (item)) {
				try {
					if ((bool) datagrid [item.Index, 0])
						return ToggleState.On;
					else
						return ToggleState.Off;
				} catch (InvalidCastException) {
					return ToggleState.Indeterminate;
				}
			} else
				return ToggleState.Indeterminate;
		}
		
		public void ToggleItem (ListItemProvider item)
		{
			if (items.Contains (item)) {
				try {
					bool value = (bool) datagrid [item.Index, 0];
					datagrid [item.Index, 0] = !value;
				} catch (InvalidCastException) {}
			}
		}

		public IProviderBehavior GetListItemBehaviorRealization (AutomationPattern behavior,
		                                                         ListItemProvider listItem)
		{
			if (behavior == SelectionItemPatternIdentifiers.Pattern)
				return new DataItemSelectionItemProviderBehavior (listItem);
			else if (behavior == GridItemPatternIdentifiers.Pattern)
				return new DataItemGridItemProviderBehavior (listItem);
			else if (behavior == ScrollItemPatternIdentifiers.Pattern)
				return new DataItemScrollItemProviderBehavior (listItem);
			else if (behavior == ValuePatternIdentifiers.Pattern)
				return new DataItemValueProviderBehavior (listItem);
			else if (behavior == TableItemPatternIdentifiers.Pattern)
				return new DataItemTableItemProviderBehavior (listItem);
			else
				return null;
		}

		public IConnectable GetListItemEventRealization (ProviderEventType eventType, 
		                                                 ListItemProvider provider)
		{
			if (eventType == ProviderEventType.AutomationElementHasKeyboardFocusProperty)
				return new DataItemAutomationHasKeyboardFocusPropertyEvent (provider);
			else if (eventType == ProviderEventType.AutomationFocusChangedEvent)
				return new DataItemAutomationFocusChangedEvent (provider);
			else if (eventType == ProviderEventType.AutomationElementIsOffscreenProperty)
				return new DataItemAutomationIsOffscreenPropertyEvent (provider);
			else
				return null;
		}

		public event Action ScrollPatternSupportChanged;

		#endregion

		#region Private Methods

		private void OnUIACollectionChanged (object sender, CollectionChangeEventArgs args)
		{
			if (lastCurrencyManager == null || args.Action == CollectionChangeAction.Refresh)
				return;

			// FIXME: Remove CurrentTableStyle property after committing SWF patch
			SWF.DataGridTableStyle tableStyle = CurrentTableStyle;
			CreateHeader (tableStyle);
			
			if (args.Action == CollectionChangeAction.Add) {
				if (tableStyle.GridColumnStyles.Count == 0)
					return;

				for (int index = items.Count; index < lastCurrencyManager.Count; index++)
					CreateListItem (index, tableStyle);
			} else if (args.Action == CollectionChangeAction.Remove) {
				// TODO: Is there a better way to do this?
				int toRemove = items.Count - lastCurrencyManager.Count;
				while (toRemove > 0) {
					ListItemProvider item = items [items.Count - 1];
					RemoveChildProvider (item);
					item.Terminate ();
					items.Remove (item);
					toRemove--;
				}
			}
		}

		private void UpdateChildren ()
		{
			if (lastDataSource != null) {
				OnNavigationChildrenCleared ();
				items.Clear ();
			}

			if (header != null) {
				RemoveChildProvider (header);
				header.Terminate ();
				header = null;

				SetBehavior (TablePatternIdentifiers.Pattern, null);
			}

			if (lastCurrencyManager == null) { 
				// First time, otherwise we can't do anything.
				lastCurrencyManager = RequestCurrencyManager ();
				if (lastCurrencyManager == null)
					return;
			}

			// Is showing "+" to expand, this usually happens when DataSource is
			// DataSet and has more than one DataTable.
			if (datagrid.CurrentTableStyle.GridColumnStyles.Count == 0) {
				DataGridCustomProvider customProvider 
					= new DataGridCustomProvider (this, 0, string.Empty);
				customProvider.Initialize ();
				AddChildProvider (customProvider);
			} else {
				CreateHeader (datagrid.CurrentTableStyle);

				for (int row = 0; row < lastCurrencyManager.Count; row++)
					CreateListItem (row, datagrid.CurrentTableStyle);
			}

			lastDataSource = datagrid.DataSource;
		}
		
		private void OnDataSourceChanged (object sender, EventArgs args)
		{
			bool refreshChildren = false;
			SWF.CurrencyManager manager = null;

			// Happens when Navigating DataGrid
			if (lastDataSource == datagrid.DataSource) {
				manager = RequestCurrencyManager ();
				// Only when rendering something different we refresh children
				if (manager != null && manager != lastCurrencyManager)
					refreshChildren = true;
			} else {
				manager = RequestCurrencyManager ();
				refreshChildren = true;
			}

			if (refreshChildren) {
				lastCurrencyManager = manager;
				UpdateChildren ();
			}
		}

		private SWF.CurrencyManager RequestCurrencyManager () 
		{
			if (datagrid.DataSource == null)
			    return null;
			
			return (SWF.CurrencyManager) datagrid.BindingContext [datagrid.DataSource,
			                                                      datagrid.DataMember];
		}

		private void CreateHeader (SWF.DataGridTableStyle tableStyle)
		{
			if (!datagrid.ColumnHeadersVisible || header != null)
				return;
	
			header = new DataGridHeaderProvider (this, tableStyle.GridColumnStyles);
			header.Initialize ();
			AddChildProvider (header);

			SetBehavior (TablePatternIdentifiers.Pattern,
			             new TableProviderBehavior (this));
		}

		private void CreateListItem (int row, SWF.DataGridTableStyle tableStyle)
		{
			DataGridDataItemProvider item = new DataGridDataItemProvider (this,
			                                                              row,
			                                                              datagrid,
			                                                              tableStyle);
			item.Initialize ();
			AddChildProvider (item);
			items.Add (item);
		}

		#endregion Private Methods

		#region Private Fields

		private SWF.DataGrid datagrid;
		private DataGridHeaderProvider header;
		private List<ListItemProvider> items;
		private SWF.CurrencyManager lastCurrencyManager; 
		private object lastDataSource;
		private ScrollBehaviorObserver observer;

		#endregion

		#region Internal Class: DataGridHeaderProvider

		internal class DataGridHeaderProvider : FragmentRootControlProvider
		{
			public DataGridHeaderProvider (DataGridProvider provider,
			                               SWF.GridColumnStylesCollection styles) : base (null)
			{
				this.provider = provider;
				this.styles = styles;

				dictionary = new Dictionary <SWF.DataGridColumnStyle, DataGridHeaderItemProvider> ();
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return provider; }
			}

			public DataGridProvider DataGridProvider {
				get { return provider; }
			}

			public SWF.GridColumnStylesCollection GridColumnStyles {
				get { return styles; }
			}

			public override SWF.Control AssociatedControl {
				get { return provider.DataGrid; }
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
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					Rect bounds 
						= (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					return Helper.IsOffScreen (bounds, provider.DataGrid, true);
				} else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get {
					SD.Rectangle rectangle = provider.DataGrid.UIAColumnHeadersArea;
					rectangle.X += provider.DataGrid.Bounds.X;
					rectangle.Y += provider.DataGrid.Bounds.Y;

					return Helper.GetControlScreenBounds (rectangle, provider.DataGrid);
				} 
			}

			public IRawElementProviderSimple[] GetHeaderItems ()
			{
				if (ChildrenCount == 0)
					return new IRawElementProviderSimple [0];
				else {
					IRawElementProviderSimple []children = new IRawElementProviderSimple [ChildrenCount];
					for (int index = 0; index < ChildrenCount; index++)
						children [index] = Navigation.GetChild (index);

					return children;
				}
			}

			public DataGridHeaderItemProvider GetHeaderItem (SWF.DataGridColumnStyle column)
			{
				DataGridHeaderItemProvider provider;
				dictionary.TryGetValue (column, out provider);
				return provider;
			}

			protected override void InitializeChildControlStructure ()
			{
				foreach (SWF.DataGridColumnStyle style in styles) {
					DataGridHeaderItemProvider headerItem
						= new DataGridHeaderItemProvider (this, style);
					headerItem.Initialize ();
					AddChildProvider (headerItem);
					dictionary [style] = headerItem;
				}

				styles.CollectionChanged += OnColumnsCollectionChanged;
			}

			protected override void FinalizeChildControlStructure()
			{
				base.FinalizeChildControlStructure ();
				
				styles.CollectionChanged -= OnColumnsCollectionChanged;
			}

			private void OnColumnsCollectionChanged (object sender, CollectionChangeEventArgs args)
			{
				SWF.DataGridColumnStyle column = (SWF.DataGridColumnStyle) args.Element;
				
				if (args.Action == CollectionChangeAction.Add) {
					DataGridHeaderItemProvider headerItem
						= new DataGridHeaderItemProvider (this, column);
					headerItem.Initialize ();
					AddChildProvider (headerItem);
					dictionary [column] = headerItem;
				} else if (args.Action == CollectionChangeAction.Remove) {
					DataGridHeaderItemProvider headerItem = null;
					if (!dictionary.TryGetValue (column, out headerItem))
						return;
					headerItem.Terminate ();
					RemoveChildProvider (headerItem);
					dictionary.Remove (column);
				} else if (args.Action == CollectionChangeAction.Refresh) {
					foreach (DataGridHeaderItemProvider headerItem in dictionary.Values)
						headerItem.Terminate ();
					OnNavigationChildrenCleared ();
				}  
			}

			private SWF.GridColumnStylesCollection styles;
			private Dictionary <SWF.DataGridColumnStyle, DataGridHeaderItemProvider> dictionary;
			private DataGridProvider provider;			
		} // DataGridHeaderProvider

		#endregion

		#region Internal Class: Header Item

		internal class DataGridHeaderItemProvider : FragmentControlProvider
		{
			public DataGridHeaderItemProvider (DataGridHeaderProvider header, 
			                                   SWF.DataGridColumnStyle style) : base (null)
			{
				this.header = header;
				this.style = style;
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return header; }
			}
			
			public DataGridHeaderProvider HeaderProvider {
				get { return header; }
			}

			public SWF.DataGridColumnStyle ColumnStyle {
				get { return style; }
			}

			public override SWF.Control AssociatedControl {
				get { return header.AssociatedControl; }
			}

			public override void Initialize ()
			{
				base.Initialize ();

				SetBehavior (InvokePatternIdentifiers.Pattern,
				             new HeaderItemInvokeProviderBehavior (this));
			}
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.HeaderItem.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return style.HeaderText;
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
					return Helper.IsOffScreen (bounds, style.DataGridTableStyle.DataGrid, true);
				} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get {
					Rect bounds
						= (Rect) header.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);

					int indexOf = header.GridColumnStyles.IndexOf (style);
					for (int index = 0; index < indexOf; index++)
						bounds.X += header.GridColumnStyles [index].Width;

					bounds.Width = header.GridColumnStyles [indexOf].Width;

					return bounds;
				}
			}

			private SWF.DataGridColumnStyle style;
			private DataGridHeaderProvider header;
		} // DataGridHeaderItemProvider

		#endregion

		#region Internal Class: Data Item

		internal class DataGridDataItemProvider : ListItemProvider
		{
			public DataGridDataItemProvider (DataGridProvider provider,
			                                 int row,
			                                 SWF.DataGrid dataGrid,
			                                 SWF.DataGridTableStyle style)
				: base (provider, provider, dataGrid, row)
			{
				this.provider = provider;
				this.style = style;
				name = string.Empty;

				columns = new Dictionary<SWF.DataGridColumnStyle, DataGridDataItemEditProvider> ();
			}

			public DataGridProvider DataGridProvider {
				get { return provider; }
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return provider; }
			}

			public int GetColumnIndexOf (DataGridDataItemEditProvider custom)
			{
				return provider.CurrentTableStyle.GridColumnStyles.IndexOf (custom.Column);
			}

			public string GetName (int columnIndex)
			{
				var columnsCount = provider.DataGrid.CurrentTableStyle.GridColumnStyles.Count;
				if (columnIndex <= -1 || columnIndex >= columnsCount) {
					Log.Error($"DataGridDataItemProvider.GetName: wrong columnIndex={columnIndex}. columnsCount={columnsCount}");
					return string.Empty;
				}

				var data = provider.DataGrid [Index, columnIndex];
				var dataAsStr = data?.ToString ();
				var name = string.IsNullOrEmpty (dataAsStr)
					? style.GridColumnStyles [columnIndex].NullText
					: dataAsStr;
				return name;
			}

			public object Value {
				get { return provider.DataGrid [Index, 0]; }
				set { provider.DataGrid [Index, 0] = value; }
			}

			public void SetEditValue (DataGridDataItemEditProvider edit, object value)
			{
				int column = provider.CurrentTableStyle.GridColumnStyles.IndexOf (edit.Column);
				int row = Index;
				if (column == -1 || row == -1)
					return;

				provider.DataGrid [row, column] = value;
			}

			protected override void InitializeChildControlStructure ()
			{
				for (int column = 0; column < provider.CurrentTableStyle.GridColumnStyles.Count; column++) {
					SWF.DataGridColumnStyle columnStyle =
						provider.CurrentTableStyle.GridColumnStyles [column];
					
					var edit = new DataGridDataItemEditProvider (this, columnStyle);
					edit.Initialize ();
					AddChildProvider (edit);

					if (column == 0)
						name = GetName (column);
					
					columns [columnStyle] = edit;
				}

				// To keep track of columns
				DataGridProvider.CurrentTableStyle.GridColumnStyles.CollectionChanged += OnColumnsCollectionChanged;
			}

			protected override void FinalizeChildControlStructure()
			{
				base.FinalizeChildControlStructure ();
				
				DataGridProvider.CurrentTableStyle.GridColumnStyles.CollectionChanged -= OnColumnsCollectionChanged;
			}

			public override void SetFocus ()
			{
				DataGridProvider.DataGrid.Focus ();
				DataGridProvider.DataGrid.CurrentCell = new SWF.DataGridCell (Index, 0);
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				//FIXME: What about ItemTypeProperty & ItemStatusProperty ?
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return name;
				// NOTDOTNET: Using DataItem instead, the spec says that we should use text, 
				// however the implementation uses ListItem.
				else if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.DataItem.Id;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					return IsOffScreen (DataGridProvider.DataGrid,
					                    (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id));
				} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else if (propertyId == AutomationElementIdentifiers.NativeWindowHandleProperty.Id)
					return null;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get {
					return Helper.GetControlScreenBounds (DataGridProvider.DataGrid.GetCellBounds (Index, 0),
					                                      DataGridProvider.DataGrid);
				}
			}

			public bool IsOffScreen (SWF.DataGrid datagrid, Rect bounds)
			{
				SD.Rectangle rectangle = datagrid.Bounds;
				rectangle.Height -= datagrid.UIACellsArea.Y - rectangle.Y - datagrid.UIARowHeight;
				rectangle.Y = datagrid.UIACellsArea.Y;
				if (datagrid.ColumnHeadersVisible) {
					rectangle.X += datagrid.RowHeaderWidth;
					rectangle.Y += datagrid.UIAColumnHeadersArea.Height;
					rectangle.Height -= datagrid.UIACaptionArea.Height;
				}
				if (datagrid.CaptionVisible)
					rectangle.X += datagrid.UIACaptionArea.Height;

				if (rectangle.Width < 0)
					rectangle.Width = 0;
				if (rectangle.Height < 0)
					rectangle.Height = 0;

				Rect screen = Helper.RectangleToRect (datagrid.Parent.RectangleToScreen (rectangle));
				return !bounds.IntersectsWith (screen);
			}

			private void OnColumnsCollectionChanged (object sender, CollectionChangeEventArgs args)
			{
				SWF.DataGridColumnStyle column = (SWF.DataGridColumnStyle) args.Element;

				if (args.Action == CollectionChangeAction.Remove) {
					DataGridDataItemEditProvider edit = columns [column];
					edit.Terminate ();
					RemoveChildProvider (edit);

					columns.Remove (column);
				} else if (args.Action == CollectionChangeAction.Add) {
					DataGridDataItemEditProvider edit 
						= new DataGridDataItemEditProvider (this, column);
					edit.Initialize ();
					AddChildProvider (edit);

					columns [column] = edit;
				} else if (args.Action == CollectionChangeAction.Refresh) {
					foreach (DataGridDataItemEditProvider edit in columns.Values)
						edit.Terminate ();
					OnNavigationChildrenCleared ();
				} 
			}

			private Dictionary<SWF.DataGridColumnStyle, DataGridDataItemEditProvider> columns;
			private string name;			
			private DataGridProvider provider;
			private SWF.DataGridTableStyle style;
		} //DataGridListItemProvider

		#endregion

		#region Internal Class: Data Item Edit

		internal class DataGridDataItemEditProvider : FragmentRootControlProvider, ISelectableItem
		{
			public DataGridDataItemEditProvider (DataGridDataItemProvider provider,
			                                     SWF.DataGridColumnStyle column) : base (null)
			{
				this.provider = provider;
				this.column = column;
			}

			public SWF.DataGridColumnStyle Column {
				get { return column; }
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return provider; }
			}

			public DataGridDataItemProvider ItemProvider {
				get { return provider; }
			}

			public override SWF.Control AssociatedControl {
				get { return ContainerControl; }
			}

			public override void Initialize ()
			{
				base.Initialize ();

				//TODO: Support Invoke pattern
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new DataItemEditValueProviderBehavior (this));
				SetBehavior (GridItemPatternIdentifiers.Pattern,
				             new DataItemEditGridItemProviderBehavior (this));
				SetBehavior (TableItemPatternIdentifiers.Pattern,
				             new DataItemEditTableItemProviderBehavior (this));
				SetBehavior (SelectionItemPatternIdentifiers.Pattern,
				             new DataItemEditSelectionItemProviderBehavior (this));

				//Events
				SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
				          new DataItemEditAutomationHasKeyboardFocusPropertyEvent (this));
				SetEvent (ProviderEventType.AutomationFocusChangedEvent,
				          new DataItemEditAutomationFocusChangedEvent (this));
				SetEvent (ProviderEventType.AutomationElementIsOffscreenProperty,
				          new DataItemEditAutomationIsOffscreenPropertyEvent (this));
			}

			public override void SetFocus ()
			{
				ItemProvider.DataGridProvider.DataGrid.Focus ();
				ItemProvider.DataGridProvider.DataGrid.CurrentCell = new SWF.DataGridCell (ItemProvider.Index, 
				                                                                           ItemProvider.GetColumnIndexOf (this));
			}

			public SWF.Control ContainerControl { 
				get { return ItemProvider.DataGridProvider.DataGrid; }
			}
	
			public IRawElementProviderSimple SelectionContainer { 
				get { return ItemProvider.DataGridProvider; }
			}
			
			public bool Selected { 
				get { return ItemProvider.Selected; }
			}
	
			public void Select ()
			{
				// SWF.DataGrid doesn't support cell-by-cell selection
				// so we are selecting all row anyway.
				ItemProvider.Select ();
			}
			
			public void Unselect ()
			{
				// SWF.DataGrid doesn't support cell-by-cell unselection
				// so we are unselecting all row anyway.
				ItemProvider.Unselect ();
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Text.Id;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					// ItemProvider.DataGridProvider.DataGrid.Focused should be used here, but seems SWF.DataGrid loses this state randomly
					return ItemProvider.DataGridProvider.DataGrid.CurrentCell.RowNumber == ItemProvider.Index
						&& ItemProvider.DataGridProvider.DataGrid.CurrentCell.ColumnNumber == ItemProvider.GetColumnIndexOf (this);
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return ItemProvider.DataGridProvider.DataGrid.Enabled;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return provider.GetName (provider.GetColumnIndexOf (this));
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					var bounds = (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					return provider.IsOffScreen (provider.DataGridProvider.DataGrid, bounds);
				} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get { 
					SD.Rectangle rectangle = provider.DataGridProvider.DataGrid.GetCellBounds (provider.Index,
					                                                                           provider.GetColumnIndexOf (this));
					Rect rect = Helper.GetControlScreenBounds (rectangle,
					                                           provider.DataGridProvider.DataGrid,
					                                           true);
					return rect;
				}
			}

			private readonly DataGridDataItemProvider provider;
			private readonly SWF.DataGridColumnStyle column;
		} //DataGridListItemCustomProvider

		#endregion
		
		#region Internal Class: ScrollBar provider

		class DataGridScrollBarProvider : ScrollBarProvider
		{
			public DataGridScrollBarProvider (SWF.ScrollBar scrollbar,
			                                  DataGridProvider provider)
				: base (scrollbar)
			{
				this.provider = provider;
				name = scrollbar is SWF.HScrollBar ? Catalog.GetString ("Horizontal Scroll Bar")
					: Catalog.GetString ("Vertical Scroll Bar");
			}
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return provider; }
			}			
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return name;
				else
					return base.GetProviderPropertyValue (propertyId);
			}
			
			private DataGridProvider provider;
			private string name;
		} // DataGridScrollBarProvider
		
		#endregion

		#region Internal Class: Custom 

		internal class DataGridCustomProvider : FragmentRootControlProvider
		{
			public DataGridCustomProvider (DataGridProvider provider,
			                               int row,
			                               string name) : base (null)
			{
				this.provider = provider;
				this.row = row;
				this.name = name;
			}

			public int Row {
				get { return row; }
			}

			public DataGridProvider DataGridProvider {
				get { return provider; }
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return DataGridProvider; }
			}

			public override void Initialize ()
			{
				base.Initialize ();

				//TODO: Support Value Pattern ?
				SetBehavior (InvokePatternIdentifiers.Pattern, 
				             new CustomInvokeProviderBehavior (this));
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Custom.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return name;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private string name;
			private DataGridProvider provider;
			private int row;
		} //DataGridCustomProvider

		#endregion
	}
}
