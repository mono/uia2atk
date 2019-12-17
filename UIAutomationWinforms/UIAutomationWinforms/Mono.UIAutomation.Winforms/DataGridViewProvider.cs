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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Mario Carrion <mcarrion@novell.com>
//
using Mono.Unix;
using System;
using System.ComponentModel;
using SD = System.Drawing;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.DataGridView;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.DataGridView;

namespace Mono.UIAutomation.Winforms
{
	// Good reference:
	// http://www.devolutions.net/articles/DataGridViewFAQ.htm
		
	[MapsComponent (typeof (SWF.DataGridView))]
	internal class DataGridViewProvider : ListProvider
	{
		#region Constructors
		
		public DataGridViewProvider (SWF.DataGridView datagridview) 
			: base (datagridview)
		{
			this.datagridview = datagridview;
		}

		#endregion

		#region Overridden Methods

		// NOTE:
		//       SWF.DataGridView.VerticalScrollBar and 
		//       SWF.DataGridView.HorizontalScrollBar are protected 
		//       properties part of the public API.
		protected override SWF.ScrollBar HorizontalScrollBar { 
			get {
				return Helper.GetPrivateProperty<SWF.DataGridView, SWF.ScrollBar> (datagridview,
				                                                                   "HorizontalScrollBar");
			}
		}

		protected override SWF.ScrollBar VerticalScrollBar { 
			get {
				return Helper.GetPrivateProperty<SWF.DataGridView, SWF.ScrollBar> (datagridview,
				                                                                   "VerticalScrollBar");
			}
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.DataGrid.Id;
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else
				return base.GetProviderPropertyValue (propertyId);
		}

		public override void UnselectItem (ListItemProvider item)
		{
			if (!ContainsItem (item))
				return;
			((DataGridDataItemProvider) item).Row.Cells [0].Selected = false;
		}

		public override void SelectItem (ListItemProvider item)
		{
			if (!ContainsItem (item))
				return;

			((DataGridDataItemProvider) item).Row.Cells [0].Selected = true;
		}

		public override void ScrollItemIntoView (ListItemProvider item)
		{
			if (!ContainsItem (item))
				return;

			DataGridDataItemProvider dataItem = (DataGridDataItemProvider) item;
			datagridview.FirstDisplayedCell = dataItem.Row.Cells [0];
		}

		public override IRawElementProviderSimple[] GetSelectedItems ()
		{
			List<DataGridViewDataItemChildProvider> items = new List<DataGridViewDataItemChildProvider> ();
			
			foreach (SWF.DataGridViewCell cell in datagridview.SelectedCells) {
				DataGridDataItemProvider itemProvider
					= (DataGridDataItemProvider) GetItemProviderFrom (this,
					                                                  datagridview.Rows [cell.RowIndex],
					                                                  false);
				if (itemProvider == null) //Not yet initialized
					break;
				items.Add (itemProvider.GetChildItem (cell.ColumnIndex));
			}

			return items.ToArray ();
		}
		
		public override int SelectedItemsCount {
			get { return datagridview.SelectedRows.Count; }
		}

		public override bool IsItemSelected (ListItemProvider item)
		{
			if (!ContainsItem (item))
				return false;

			return ((DataGridDataItemProvider) item).Row.Cells [0].Selected;
		}
		
		public override int ItemsCount {
			get { return datagridview.Rows.Count; }
		}

		public override int IndexOfObjectItem (object objectItem)
		{
			SWF.DataGridViewRow row = objectItem as SWF.DataGridViewRow;
			if (row == null)
				return -1;

			return datagridview.Rows.IndexOf (row);
		}

		public override void FocusItem (object objectItem)
		{
			SWF.DataGridViewRow row = objectItem as SWF.DataGridViewRow;
			if (row == null || !datagridview.Rows.Contains (row))
				return;

			datagridview.CurrentCell = row.Cells [0];
		}

		public override object GetItemPropertyValue (ListItemProvider item, int propertyId)
		{
			DataGridDataItemProvider provider = (DataGridDataItemProvider) item;
			
			if (propertyId == AutomationElementIdentifiers.NameProperty.Id) {
				object cellvalue = provider.Row.Cells [0].FormattedValue;
				return cellvalue != null ? cellvalue.ToString () : String.Empty;
			} else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return datagridview.CurrentCell == provider.Row.Cells [0] && datagridview.Focused;
			else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
				SD.Rectangle rectangle = datagridview.GetRowDisplayRectangle (provider.Row.Index, false);
				if (datagridview.RowHeadersVisible)
					rectangle.X += datagridview.RowHeadersWidth;

				return Helper.GetControlScreenBounds (rectangle, 
				                                      datagridview,
				                                      true);
			} else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
				return Helper.IsListItemOffScreen (item.BoundingRectangle,
				                                   datagridview, 
				                                   datagridview.ColumnHeadersVisible,
				                                   header.Size,
				                                   ScrollBehaviorObserver);
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return !provider.Row.Cells [0].ReadOnly;
			else
				return null;
		}

		internal override IProviderBehavior GetBehaviorRealization (AutomationPattern behavior)
		{
			if (behavior == SelectionPatternIdentifiers.Pattern)
				return new SelectionProviderBehavior (this);
			else if (behavior == GridPatternIdentifiers.Pattern)
			    return new GridProviderBehavior (this);
			else if (behavior == TablePatternIdentifiers.Pattern)
				return new TableProviderBehavior (this);
			else if (behavior == ScrollPatternIdentifiers.Pattern)
				return new ScrollProviderBehavior (this);
			else
				return null;
		}

		public override IProviderBehavior GetListItemBehaviorRealization (AutomationPattern behavior,
		                                                                  ListItemProvider listItem)
		{
			if (behavior == SelectionItemPatternIdentifiers.Pattern)
				return new DataItemSelectionItemProviderBehavior (listItem);
			else if (behavior == GridItemPatternIdentifiers.Pattern)
				return new DataItemGridItemProviderBehavior (listItem);
			else if (behavior == TableItemPatternIdentifiers.Pattern)
				return new DataItemTableItemProviderBehavior (listItem);
			else
				return base.GetListItemBehaviorRealization (behavior, listItem);
		}

		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			header = new DataGridViewHeaderProvider (this);
			header.Initialize ();
			AddChildProvider (header);

			datagridview.Rows.CollectionChanged += OnCollectionChanged;

			foreach (SWF.DataGridViewRow row in datagridview.Rows) {
				ListItemProvider itemProvider = GetItemProviderFrom (this, row);
				AddChildProvider (itemProvider);
			}
		}

		public override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();
			
			datagridview.Rows.CollectionChanged -= OnCollectionChanged;
		}

		protected override ListItemProvider GetNewItemProvider (FragmentRootControlProvider rootProvider,
		                                                        ListProvider provider,
		                                                        SWF.Control control,
		                                                        object objectItem)
		{
			return new DataGridDataItemProvider (this, 
			                                     datagridview, 
			                                     (SWF.DataGridViewRow) objectItem);
		}

		#endregion

		#region Public Properties

		public DataGridViewHeaderProvider Header {
			get { return header; }
		}

		public SWF.DataGridView DataGridView {
			get { return datagridview; }
		}

		#endregion

		#region Private Fields

		private SWF.DataGridView datagridview;
		private DataGridViewHeaderProvider header;

		#endregion

		#region Internal Class: Header Provider 
		
		internal class DataGridViewHeaderProvider : FragmentRootControlProvider
		{
			public DataGridViewHeaderProvider (DataGridViewProvider provider) : base (null)
			{
				viewProvider = provider;
				headers = new Dictionary<SWF.DataGridViewColumn, DataGridViewHeaderItemProvider> ();
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get {  return viewProvider; }
			}

			public SWF.DataGridView DataGridView {
				get { return viewProvider.DataGridView; }
			}

			public override SWF.Control AssociatedControl {
				get { return DataGridView; }
			}

			public SD.Rectangle Size {
				get {
					if (!DataGridView.ColumnHeadersVisible)
						return new SD.Rectangle (0, 0, 0, 0);
					else {
						SD.Rectangle bounds = SD.Rectangle.Empty;
						bounds.Height = DataGridView.ColumnHeadersHeight;
						bounds.Width = DataGridView.RowHeadersWidth;
						for (int index = 0; index < DataGridView.Columns.Count; index++)
							bounds.Width += DataGridView.Columns [index].Width;
						
						return bounds;
					}
				}
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
				get { return Helper.GetControlScreenBounds (Size, DataGridView, true); }
			}

			public IRawElementProviderSimple[] GetHeaderItems ()
			{
				IRawElementProviderSimple []items = new IRawElementProviderSimple [headers.Count];

				int index = 0;
				foreach (DataGridViewHeaderItemProvider item in headers.Values) {
					items [index] = item;
					index++;
				}
				
				return items;
			}

			public override void InitializeChildControlStructure ()
			{
				DataGridView.Columns.CollectionChanged += OnColumnsCollectionChanged;
				
				foreach (SWF.DataGridViewColumn column in DataGridView.Columns)
					UpdateCollection (column, CollectionChangeAction.Add);
			}

			public override void FinalizeChildControlStructure ()
			{
				base.FinalizeChildControlStructure ();

				foreach (DataGridViewHeaderItemProvider item in headers.Values)
					item.Terminate ();

				headers.Clear ();
			}

			private void OnColumnsCollectionChanged (object sender,
			                                         CollectionChangeEventArgs args)
			{
				UpdateCollection ((SWF.DataGridViewColumn) args.Element,
				                  args.Action);
			}

			private void UpdateCollection (SWF.DataGridViewColumn column, 
			                               CollectionChangeAction change)
			{
				if (change == CollectionChangeAction.Remove) {
					DataGridViewHeaderItemProvider headerItem = headers [column];
					RemoveChildProvider (headerItem);
					headers.Remove (column);
				} else if (change == CollectionChangeAction.Add) {
					DataGridViewHeaderItemProvider headerItem 
						= new DataGridViewHeaderItemProvider (this, column);
					headerItem.Initialize ();
					AddChildProvider (headerItem);
					headers [column] = headerItem;
				}
			}

			private DataGridViewProvider viewProvider;
			private Dictionary<SWF.DataGridViewColumn, DataGridViewHeaderItemProvider> headers;
		}

		#endregion

		#region Internal Class: Header Item Provider

		internal class DataGridViewHeaderItemProvider : FragmentControlProvider
		{
			public DataGridViewHeaderItemProvider (DataGridViewHeaderProvider headerProvider,
			                                       SWF.DataGridViewColumn column)
				: base (null)
			{
				this.headerProvider = headerProvider;
				this.column = column;
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return headerProvider; }
			}

			public SWF.DataGridViewColumn Column {
				get { return column; }
			}

			public DataGridViewHeaderProvider HeaderProvider {
				get { return headerProvider; }
			}

			public override SWF.Control AssociatedControl {
				get { return headerProvider.DataGridView; }
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
					return column.HeaderText;
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
				else if (propertyId == AutomationElementIdentifiers.HelpTextProperty.Id)
					return column.ToolTipText;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					Rect bounds 
						= (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					return Helper.IsOffScreen (bounds, headerProvider.DataGridView, true);
				} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get {
					if (column == null || column.Index < 0)
						return Rect.Empty;

					Rect headerBounds
						= (Rect) headerProvider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					for (int index = 0; index < column.Index; index++)
						headerBounds.X += headerProvider.DataGridView.Columns [index].Width;

					headerBounds.X += headerProvider.DataGridView.RowHeadersWidth;
					headerBounds.Width = headerProvider.DataGridView.Columns [column.Index].Width;
					
					return headerBounds;
				}
			}

			private SWF.DataGridViewColumn column;
			private DataGridViewHeaderProvider headerProvider;
		}

		#endregion

		#region Internal Class: Data Item Provider

		internal class DataGridDataItemProvider : ListItemProvider
		{
			public DataGridDataItemProvider (DataGridViewProvider datagridViewProvider,
			                                 SWF.DataGridView datagridview,
		                                     SWF.DataGridViewRow row) 
				: base (datagridViewProvider, datagridViewProvider, datagridview, row)
			{
				this.datagridviewProvider = datagridViewProvider;
				this.datagridview = datagridview;
				this.row = row;

				columns = new Dictionary<SWF.DataGridViewColumn, DataGridViewDataItemChildProvider> ();
			}

			public DataGridViewProvider DataGridViewProvider {
				get { return datagridviewProvider; }
			}

			public SWF.DataGridView DataGridView {
				get { return datagridview; }
			}

			public SWF.DataGridViewRow Row {
				get { return row; }
			}

			public override IRawElementProviderFragment GetFocus ()
			{
				if (DataGridView.CurrentCell == null 
				    || DataGridView.CurrentCell.RowIndex != Row.Index)
					return null;
				else {
					if (DataGridView.CurrentCell.ColumnIndex < 0 
					    || DataGridView.CurrentCell.ColumnIndex >= DataGridView.Columns.Count)
						return null;

					DataGridViewDataItemChildProvider provider = null;
					columns.TryGetValue (DataGridView.Columns [DataGridView.CurrentCell.ColumnIndex], 
					                     out provider);
					return provider;
				}
			}

			public DataGridViewDataItemChildProvider GetChildItem (SWF.DataGridViewColumn column)
			{
				DataGridViewDataItemChildProvider provider = null;
				columns.TryGetValue (column, out provider);
				
				return provider;
			}

			public DataGridViewDataItemChildProvider GetChildItem (int columnIndex)
			{
				if (columnIndex < 0 || columnIndex >= columns.Count)
					return null;

				return GetChildItem (datagridview.Columns [columnIndex]);
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.DataItem.Id;
				else if (propertyId == AutomationElementIdentifiers.NativeWindowHandleProperty.Id)
					return null;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			public override void InitializeChildControlStructure ()
			{
				foreach (SWF.DataGridViewColumn column in datagridview.Columns)
					UpdateCollection (column, CollectionChangeAction.Add);

				datagridview.Columns.CollectionChanged += OnColumnsCollectionChanged;
			}

			public override void FinalizeChildControlStructure ()
			{
				base.FinalizeChildControlStructure ();

				datagridview.Columns.CollectionChanged -= OnColumnsCollectionChanged;
			}

			private void OnColumnsCollectionChanged (object sender, 
			                                         CollectionChangeEventArgs args)
			{
				UpdateCollection ((SWF.DataGridViewColumn) args.Element,
				                  args.Action);
			}

			private void UpdateCollection (SWF.DataGridViewColumn column, 
			                               CollectionChangeAction change)
			{
				if (change == CollectionChangeAction.Remove) {
					DataGridViewDataItemChildProvider child = columns [column];
					RemoveChildProvider (child);
					child.Terminate ();
					columns.Remove (child.Column);
				} else if (change == CollectionChangeAction.Add) {
					DataGridViewDataItemChildProvider child;

					if ((column as SWF.DataGridViewButtonColumn) != null)
						child = new DataGridViewDataItemButtonProvider (this, column);
					else if ((column as SWF.DataGridViewCheckBoxColumn) != null)
						child = new DataGridViewDataItemCheckBoxProvider (this, column);
					else if ((column as SWF.DataGridViewComboBoxColumn) != null)
						child = new DataGridViewDataItemComboBoxProvider (this, column);
					else if ((column as SWF.DataGridViewImageColumn) != null)
						child = new DataGridViewDataItemImageProvider (this, column);
					else if ((column as SWF.DataGridViewLinkColumn) != null)
						child = new DataGridViewDataItemLinkProvider (this, column);
					else if ((column as SWF.DataGridViewTextBoxColumn) != null)
						child = new DataGridViewDataItemEditProvider (this, column);
					else
						child = new DataGridViewDataItemChildProvider (this, column);

					child.Initialize ();
					AddChildProvider (child);
					columns [child.Column] = child;
				}
			}

			private Dictionary<SWF.DataGridViewColumn, DataGridViewDataItemChildProvider> columns;
			private DataGridViewProvider datagridviewProvider;
			private SWF.DataGridView datagridview;
			private SWF.DataGridViewRow row;
		}

		#endregion

		#region Internal Class: Data Item Child Provider

		internal class DataGridViewDataItemChildProvider 
			: FragmentRootControlProvider, ISelectableItem
		{
			public DataGridViewDataItemChildProvider (DataGridDataItemProvider itemProvider,
			                                          SWF.DataGridViewColumn column) : base (null)
			{
				this.itemProvider = itemProvider;
				this.column = column;

				cell = itemProvider.Row.Cells [column.Index];
				gridProvider = (DataGridViewProvider) itemProvider.ListProvider;
			}

			public SWF.DataGridViewCell Cell {
				get { return cell; }
			}

			public SWF.DataGridViewColumn Column {
				get { return column; }
			}

			public DataGridDataItemProvider ItemProvider {
				get { return itemProvider; }
			}

			public DataGridViewProvider DataGridViewProvider {
				get { return gridProvider; }
			}

			public override SWF.Control AssociatedControl {
				get { return gridProvider.DataGridView; }
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return itemProvider; }
			}

			public override void SetFocus ()
			{
				itemProvider.DataGridView.Focus ();
				itemProvider.DataGridView.CurrentCell = cell;
			}

			public override void Initialize ()
			{
				base.Initialize ();

				SetBehavior (GridItemPatternIdentifiers.Pattern,
				             new DataItemChildGridItemProviderBehavior (this));
				SetBehavior (TableItemPatternIdentifiers.Pattern,
				             new DataItemChildTableItemProviderBehavior (this));
				SetBehavior (SelectionItemPatternIdentifiers.Pattern,
				             new DataItemChildSelectionItemProviderBehavior (this));

//				// Automation Events
//				SetEvent (ProviderEventType.AutomationElementIsOffscreenProperty,
//				          new ListItemEditAutomationIsOffscreenPropertyEvent (this));

				// Automation Events
				SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
				          new DataItemChildHasKeyboardFocusPropertyEvent (this));
			}

			public SWF.Control ContainerControl {
				get { return itemProvider.DataGridView; }
			}
	
			public IRawElementProviderSimple SelectionContainer { 
				get { return gridProvider; }
			}
			
			public bool Selected { 
				get { return cell.Selected; }
			}

			public void Select () 
			{
				cell.Selected = true;
			}

			public void Unselect ()
			{
				cell.Selected = false;
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id) {
					object cellvalue = cell.FormattedValue;
					return cellvalue != null ? cellvalue.ToString () : String.Empty;
				} else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return gridProvider.DataGridView.Enabled;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return gridProvider.DataGridView.CurrentCell == cell && gridProvider.DataGridView.Focused;
				else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
					return ContainerControl.Enabled;
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.HelpTextProperty.Id)
					return cell.ToolTipText;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
					return cell.Displayed;
				else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get { 
					Rect itemBounds = itemProvider.BoundingRectangle;

					for (int index = 0; index < cell.ColumnIndex; index++)
						itemBounds.X += itemProvider.DataGridView.Columns [index].Width;

					itemBounds.Width = itemProvider.DataGridView.Columns [cell.ColumnIndex].Width;

					return itemBounds;
				}
			}

			private SWF.DataGridViewCell cell;
			private SWF.DataGridViewColumn column;
			private DataGridViewProvider gridProvider;
			private DataGridDataItemProvider itemProvider;
		}

		#endregion

		#region Internal Class: Data Item Button Provider

		internal class DataGridViewDataItemButtonProvider : DataGridViewDataItemChildProvider
		{
			public DataGridViewDataItemButtonProvider (DataGridDataItemProvider itemProvider,
			                                           SWF.DataGridViewColumn column)
				: base (itemProvider, column)
			{
				buttonCell = (SWF.DataGridViewButtonCell) Cell;
			}

			public override void Initialize ()
			{
				base.Initialize ();
	
				SetBehavior (InvokePatternIdentifiers.Pattern, 
				             new DataItemChildInvokeProviderBehavior (this));
			}

			public SWF.DataGridViewButtonCell ButtonCell {
				get { return buttonCell; }
			}
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Button.Id;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private SWF.DataGridViewButtonCell buttonCell;
		}

		#endregion

		#region Internal Class: Data Item CheckBox Provider

		internal class DataGridViewDataItemCheckBoxProvider : DataGridViewDataItemChildProvider
		{
			public DataGridViewDataItemCheckBoxProvider (DataGridDataItemProvider itemProvider,
			                                             SWF.DataGridViewColumn column)
				: base (itemProvider, column)
			{
				checkBoxCell = (SWF.DataGridViewCheckBoxCell) Cell;
			}

			public override void Initialize ()
			{
				base.Initialize ();
	
				SetBehavior (TogglePatternIdentifiers.Pattern,
				             new DataItemChildToggleProviderBehavior (this));
			}

			public SWF.DataGridViewCheckBoxCell CheckBoxCell {
				get { return checkBoxCell; }
			}
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.CheckBox.Id;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private SWF.DataGridViewCheckBoxCell checkBoxCell;
		}

		#endregion

		#region Internal Class: Data Item Link Provider

		internal class DataGridViewDataItemLinkProvider : DataGridViewDataItemChildProvider
		{
			public DataGridViewDataItemLinkProvider (DataGridDataItemProvider itemProvider,
			                                         SWF.DataGridViewColumn column)
				: base (itemProvider, column)
			{
			}

			public override void Initialize ()
			{
				base.Initialize ();
	
				SetBehavior (InvokePatternIdentifiers.Pattern, 
				             new DataItemChildInvokeProviderBehavior (this));
			}
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Hyperlink.Id;
				else
					return base.GetProviderPropertyValue (propertyId);
			}
		}

		#endregion

		#region Internal Class: Data Item Image Provider

		internal class DataGridViewDataItemImageProvider
			: DataGridViewDataItemChildProvider
		{
			public DataGridViewDataItemImageProvider (DataGridDataItemProvider itemProvider,
			                                          SWF.DataGridViewColumn column)
				: base (itemProvider, column)
			{
				imageCell = (SWF.DataGridViewImageCell) Cell;
			}

			public SWF.DataGridViewImageCell ImageCell {
				get { return imageCell; }
			}

			public override void Initialize ()
			{
				base.Initialize ();
	
				SetBehavior (EmbeddedImagePatternIdentifiers.Pattern, 
				             new DataItemChildEmbeddedImageProviderBehavior (this));
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Image.Id;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private SWF.DataGridViewImageCell imageCell;
		}

		#endregion

		#region Internal Class: Data Item Edit Provider

		internal class DataGridViewDataItemEditProvider : DataGridViewDataItemChildProvider
		{
			public DataGridViewDataItemEditProvider (DataGridDataItemProvider itemProvider,
			                                         SWF.DataGridViewColumn column)
				: base (itemProvider, column)
			{
				textBoxCell = (SWF.DataGridViewTextBoxCell) Cell;
			}

			public SWF.DataGridViewTextBoxCell TextBoxCell {
				get { return textBoxCell; }
			}

			public override void Initialize ()
			{
				base.Initialize ();

				//LAMESPEC: Vista does not support Text Pattern.
				//http://msdn.microsoft.com/en-us/library/ms748367.aspx
				
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new DataItemChildValueProviderBehavior (this));
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Edit.Id;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private SWF.DataGridViewTextBoxCell textBoxCell;
		}

		#endregion

		#region Internal Class: Data Item ComboBox Provider

		internal class DataGridViewDataItemComboBoxProvider : DataGridViewDataItemChildProvider
		{
			public DataGridViewDataItemComboBoxProvider (DataGridDataItemProvider itemProvider,
			                                             SWF.DataGridViewColumn column)
				: base (itemProvider, column)
			{
				comboBoxCell = (SWF.DataGridViewComboBoxCell) Cell;
			}

			public SWF.DataGridViewComboBoxCell ComboBoxCell {
				get { return comboBoxCell; }
			}

			public ListProvider ListProvider {
				get { return listboxProvider; }
			}

			public override void Initialize ()
			{
				base.Initialize ();

				SetBehavior (SelectionPatternIdentifiers.Pattern,
				             new DataItemComboBoxSelectionProviderBehavior (this));
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
				             new DataItemComboBoxExpandCollapseProviderBehavior (this));
			}

			public override void InitializeChildControlStructure ()
			{
				if (listboxProvider == null) {
					listboxProvider = new DataGridViewDataItemComboBoxListBoxProvider (this);
					listboxProvider.Initialize ();
					AddChildProvider (listboxProvider);
				}
				if (buttonProvider == null) {
					buttonProvider = new DataGridViewDataItemComboBoxButtonProvider (this);
					buttonProvider.Initialize ();
					AddChildProvider (buttonProvider);
				}
			}

			public override void FinalizeChildControlStructure ()
			{
				if (listboxProvider != null) {
					listboxProvider.Terminate ();
					RemoveChildProvider (listboxProvider);
					listboxProvider = null;
				}
				if (buttonProvider != null) {
					buttonProvider.Terminate ();
					RemoveChildProvider (buttonProvider);
					buttonProvider = null;
				}
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.ComboBox.Id;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private DataGridViewDataItemComboBoxButtonProvider buttonProvider;
			private SWF.DataGridViewComboBoxCell comboBoxCell;
			private DataGridViewDataItemComboBoxListBoxProvider listboxProvider;
		}

		#endregion

		#region Internal Class: Data Item ComboBox ListBox Provider

		internal class DataGridViewDataItemComboBoxListBoxProvider : ListProvider
		{
			public DataGridViewDataItemComboBoxListBoxProvider (DataGridViewDataItemComboBoxProvider comboboxProvider)
				: base (comboboxProvider.ItemProvider.DataGridView)
			{
				this.comboboxProvider = comboboxProvider;
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return comboboxProvider; }
			}

			public DataGridViewDataItemComboBoxProvider ComboboxProvider {
				get { return comboboxProvider; }
			}

			public override void Initialize ()
			{
				SetEvent (ProviderEventType.AutomationElementControlTypeProperty,
				          new AutomationControlTypePropertyEvent (this));
				SetEvent (ProviderEventType.AutomationElementIsPatternAvailableProperty,
				          new AutomationIsPatternAvailablePropertyEvent (this));

				SetBehavior (SelectionPatternIdentifiers.Pattern,
				             GetBehaviorRealization (SelectionPatternIdentifiers.Pattern));
			}

			public override void InitializeChildControlStructure ()
			{
				base.InitializeChildControlStructure ();

				foreach (object obj in comboboxProvider.ComboBoxCell.Items) {
					ListItemProvider itemProvider = GetItemProviderFrom (this, obj);
					AddChildProvider (itemProvider);
				}
			}

			public string GetDisplayMemberFromValue (object value)
			{
				return GetDataBindingValueFromObjectItem (value, 
				                                          DataSourceBindingMemberType.DisplayMember) as string;
			}

			public object GetValueMemberFromValue (object value)
			{
				return GetDataBindingValueFromObjectItem (value, 
				                                          DataSourceBindingMemberType.ValueMember);
			}


			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.List.Id;
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.IsScrollPatternAvailableProperty.Id)
					return IsBehaviorEnabled (ScrollPatternIdentifiers.Pattern);
				else if (propertyId == AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
//					if (expanded)
//						return true;
					return false;
//					IExpandCollapseProvider pattern 
//						= comboboxProvider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id) as IExpandCollapseProvider;
//					return pattern != null && pattern.ExpandCollapseState == ExpandCollapseState.Collapsed;
				} else
					return comboboxProvider.GetPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get {
					// If we are expanded we return the value of the show rectangle
					// else we return the comboboxProvider
//					if (expanded)
//						return value;
//					else
						return (Rect) comboboxProvider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
				}
			}

			public override int SelectedItemsCount {
				get { return comboboxProvider.ComboBoxCell.Value == null ? 0 : 1; }
			}

			public override int ItemsCount { 
				get { return comboboxProvider.ComboBoxCell.Items.Count; }
			}

			protected override SWF.ScrollBar HorizontalScrollBar { 
				get { return null; }
			}
	
			protected override SWF.ScrollBar VerticalScrollBar { 
				get { return null; }
			}

			internal override IProviderBehavior GetBehaviorRealization (AutomationPattern behavior)
			{
				if (behavior == SelectionPatternIdentifiers.Pattern)
					return new DataItemComboBoxListBoxSelectionProviderBehavior (this);
//				//FIXME: Implement ScrollPattern
				else 
					return null;
			}

			public override IProviderBehavior GetListItemBehaviorRealization (AutomationPattern behavior,
			                                                                  ListItemProvider listItem)
			{
				if (behavior == SelectionItemPatternIdentifiers.Pattern)
					return new DataItemComboBoxListItemSelectionItemProviderBehavior (listItem);
				else
					return base.GetListItemBehaviorRealization (behavior, listItem);
			}

			public override IRawElementProviderSimple[] GetSelectedItems ()
			{
				if (SelectedItemsCount == 0)
					return new ListItemProvider [0];
				else {
					if (comboboxProvider.ComboBoxCell.DataSource == null)  {
						ListItemProvider itemProvider = GetItemProviderFrom (this,
						                                                     comboboxProvider.ComboBoxCell.Value,
						                                                     false);
						if (itemProvider == null) // Dropping button down usually sets current value to null
							return new ListItemProvider [0];
							
						return new ListItemProvider [1] { itemProvider };
					} else {
						ListItemProvider itemProvider = null;
						foreach (ListItemProvider item in Items) {
							object value = GetValueMemberFromValue (item.ObjectItem);
							if (object.Equals (value, comboboxProvider.ComboBoxCell.Value)) {
								itemProvider = item;
								break;
							}
						}

						if (itemProvider == null) // Happens when initializing
							return new ListItemProvider [0];

						return new ListItemProvider [1] { itemProvider };
					}
				}
			}
			
			public override bool IsItemSelected (ListItemProvider item)
			{
				if (comboboxProvider.ComboBoxCell.Value == null)
					return false;
				
				return GetValueMemberFromValue (item.ObjectItem) == comboboxProvider.ComboBoxCell.Value;
			}
			
			public override void SelectItem (ListItemProvider item)
			{
				if (!ContainsItem (item))
					return;

				SWF.DataGridViewCell oldCell = comboboxProvider.ComboBoxCell.DataGridView.CurrentCell;
				comboboxProvider.ComboBoxCell.DataGridView.CurrentCell = comboboxProvider.ComboBoxCell;
				comboboxProvider.ComboBoxCell.Value = GetValueMemberFromValue (item.ObjectItem);
				comboboxProvider.ComboBoxCell.DataGridView.CurrentCell = oldCell;
			}
			
			public override void UnselectItem (ListItemProvider item)
			{
				comboboxProvider.ComboBoxCell.Value = null;
			}

			public override int IndexOfObjectItem (object objectItem)
			{
				if (objectItem == null)
					return -1;

				for (int index = 0; index < comboboxProvider.ComboBoxCell.Items.Count; index++) {
					if (comboboxProvider.ComboBoxCell.Items [index] == objectItem)
						return index;
				}

				return -1;
			}

			public override void ScrollItemIntoView (ListItemProvider item)
			{
			}
		
			public override object GetItemPropertyValue (ListItemProvider item,
			                                             int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return GetDisplayMemberFromValue (item.ObjectItem);
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
					return comboboxProvider.GetPropertyValue (propertyId);
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return comboboxProvider.ComboBoxCell.DataGridView.Focused
						&& comboboxProvider.ComboBoxCell.DataGridView.CurrentCell == comboboxProvider.ComboBoxCell;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
					// What when combobox is visible and scroll is hiden cell?
					return IsItemSelected (item);
				else
					return null;
			}

			protected override void InitializeScrollBehaviorObserver ()
			{
			}

			private object GetDataBindingValueFromObjectItem (object objectItem,
			                                                  DataSourceBindingMemberType memberType)
			{
				SWF.DataGridViewComboBoxColumn column = (SWF.DataGridViewComboBoxColumn) comboboxProvider.Column;
				
				if (column.DataSource == null)
					return objectItem;
				else {
					string valueType = null;
					if (memberType == DataSourceBindingMemberType.ValueMember)
						valueType = column.ValueMember;
					else
						valueType = column.DisplayMember;
					
					SWF.BindingContext bindingContext = new SWF.BindingContext ();
					SWF.CurrencyManager manager = (SWF.CurrencyManager) bindingContext [column.DataSource];
					PropertyDescriptorCollection col = manager.GetItemProperties ();
	
					PropertyDescriptor prop = col.Find (valueType, true);
					if (prop == null)
						return null;

					return prop.GetValue (objectItem);
				}
			}

			private enum DataSourceBindingMemberType {
				DisplayMember,
				ValueMember
			}

			private DataGridViewDataItemComboBoxProvider comboboxProvider;
		}

		#endregion

		#region Internal Class: Data Item ComboBox Button Provider

		internal class DataGridViewDataItemComboBoxButtonProvider : FragmentControlProvider
		{
			public DataGridViewDataItemComboBoxButtonProvider (DataGridViewDataItemComboBoxProvider comboboxProvider)
				: base (null)
			{
				this.comboboxProvider = comboboxProvider;
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return comboboxProvider; }
			}

			public DataGridViewDataItemComboBoxProvider ComboBoxProvider {
				get { return comboboxProvider; }
			}

			public override void Initialize ()
			{
				base.Initialize ();

				SetBehavior (InvokePatternIdentifiers.Pattern, 
				             new DataItemComboBoxButtonInvokeProviderBehavior (this));
			}				

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Button.Id;
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
					// FIXME: Implement ?
					return comboboxProvider.GetPropertyValue (propertyId);
				} else
					return comboboxProvider.GetPropertyValue (propertyId);
			}

			private DataGridViewDataItemComboBoxProvider comboboxProvider;
		}

		#endregion
	}
}
