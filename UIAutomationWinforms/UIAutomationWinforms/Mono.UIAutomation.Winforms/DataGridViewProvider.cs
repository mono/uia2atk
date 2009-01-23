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
using Mono.UIAutomation.Winforms.Behaviors.DataGridView;

namespace Mono.UIAutomation.Winforms
{
		
	[MapsComponent (typeof (SWF.DataGridView))]
	internal class DataGridViewProvider : ListProvider, IScrollBehaviorSubject
	{
		
		public DataGridViewProvider (SWF.DataGridView datagridView) 
			: base (datagridView)
		{
			this.datagridView = datagridView;
		}

		#region IScrollBehaviorSubject specialization
		
		public IScrollBehaviorObserver ScrollBehaviorObserver { 
			get { return observer; }
		}
		
		public FragmentControlProvider GetScrollbarProvider (SWF.ScrollBar scrollbar)
		{
			return new DataGridViewScrollBarProvider (scrollbar, this);
		}

		#endregion

		#region Overridden Methods

		public override void Initialize ()
		{
			base.Initialize ();

			// NOTE:
			//       SWF.DataGridView.VerticalScrollBar and 
			//       SWF.DataGridView.HorizontalScrollBar are protected 
			//       properties part of the public API.
			SWF.ScrollBar hscrollbar
				= Helper.GetPrivateProperty<SWF.DataGridView, SWF.ScrollBar> (typeof (SWF.DataGridView),
				                                                              this.datagridView,
				                                                              "HorizontalScrollBar");
			SWF.ScrollBar vscrollbar
				= Helper.GetPrivateProperty<SWF.DataGridView, SWF.ScrollBar> (typeof (SWF.DataGridView),
				                                                              this.datagridView,
				                                                              "VerticalScrollBar");
			
			//ListScrollBehaviorObserver updates Navigation
			observer = new ScrollBehaviorObserver (this, hscrollbar, vscrollbar);
			observer.ScrollPatternSupportChanged += OnScrollPatternSupportChanged;
			UpdateScrollBehavior ();
		}

		public override void Terminate ()
		{
			base.Terminate ();
			
			observer.ScrollPatternSupportChanged -= OnScrollPatternSupportChanged;
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.DataGrid.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return Catalog.GetString ("data grid");
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else
				return base.GetProviderPropertyValue (propertyId);
		}

		public override void UnselectItem (ListItemProvider item)
		{
			if (!ContainsItem (item))
				return;
			
			DataGridDataItemProvider dataItem = (DataGridDataItemProvider) item;
			dataItem.Row.Selected = false;
		}

		public override void SelectItem (ListItemProvider item)
		{
			if (!ContainsItem (item))
				return;
			
			DataGridDataItemProvider dataItem = (DataGridDataItemProvider) item;
			dataItem.Row.Selected = true;
		}

		public override void ScrollItemIntoView (ListItemProvider item)
		{
		}

		public override ListItemProvider[] GetSelectedItems ()
		{
			List<ListItemProvider> items = new List<ListItemProvider> ();
			
			foreach (SWF.DataGridViewRow row in datagridView.SelectedRows)
				items.Add (GetItemProviderFrom (this, row, false));

			return items.ToArray ();
		}
		
		public override int SelectedItemsCount {
			get { return datagridView.SelectedRows.Count; }
		}

		public override bool IsItemSelected (ListItemProvider item)
		{
			if (!ContainsItem (item))
				return false;

			DataGridDataItemProvider dataItem = (DataGridDataItemProvider) item;
			return dataItem.Row.Selected;
		}
		
		public override int ItemsCount {
			get { return datagridView.Rows.Count; }
		}

		public override int IndexOfObjectItem (object objectItem)
		{
			SWF.DataGridViewRow row = objectItem as SWF.DataGridViewRow;
			if (row == null)
				return -1;

			return datagridView.Rows.IndexOf (row);
		}

		public override object GetItemPropertyValue (ListItemProvider item, int propertyId)
		{
			DataGridDataItemProvider provider = (DataGridDataItemProvider) item;
			
			if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return provider.Row.Cells [0].Value as string;
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return false;
			else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
				SD.Rectangle rectangle = datagridView.GetRowDisplayRectangle (provider.Row.Index, false);
				if (datagridView.RowHeadersVisible)
					rectangle.X += datagridView.RowHeadersWidth;

				return Helper.GetControlScreenBounds (rectangle, 
				                                      datagridView, 
				                                      true);
			} else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
				return false;
//				return Helper.IsListItemOffScreen ((Rect) item.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id),
//				                                   datagridView, 
//				                                   datagridView.ColumnHeadersVisible,
//				                                   listView.UIAHeaderControl,
//				                                   observer);
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true; // FIXME: Is this always OK?
			else
				return null;
		}

		internal override Behaviors.IProviderBehavior GetBehaviorRealization (AutomationPattern behavior)
		{
			if (behavior == SelectionPatternIdentifiers.Pattern)
				return new SelectionProviderBehavior (this);
			else
				return null;
		}

		public override void InitializeChildControlStructure ()
		{
			datagridView.Rows.CollectionChanged += OnCollectionChanged;
			datagridView.Columns.CollectionChanged += OnColumnsCollectionChanged;

			foreach (SWF.DataGridViewRow row in datagridView.Rows) {
				ListItemProvider itemProvider = GetItemProviderFrom (this, row);
				OnNavigationChildAdded (false, itemProvider);
			}

			header = new DataGridViewHeaderProvider (datagridView);
			header.Initialize ();
			OnNavigationChildAdded (false, header);
		}

		public override void FinalizeChildControlStructure ()
		{
			datagridView.Rows.CollectionChanged -= OnCollectionChanged;
			datagridView.Columns.CollectionChanged -= OnColumnsCollectionChanged;
		}

		protected override ListItemProvider GetNewItemProvider (FragmentRootControlProvider rootProvider,
		                                                        ListProvider provider,
		                                                        SWF.Control control,
		                                                        object objectItem)
		{
			// TODO: Supports Groups??
			return new DataGridDataItemProvider (this, 
			                                     datagridView, 
			                                     (SWF.DataGridViewRow) objectItem);
		}

		#endregion

		#region Event Handlers

		private void OnColumnsCollectionChanged (object sender,
		                                         CollectionChangeEventArgs args)
		{
			// FIXME: Add/remove HeaderItems
		}

		private void OnScrollPatternSupportChanged (object sender, EventArgs args)
		{
			UpdateScrollBehavior ();
		}

		#endregion

		#region Private Methods

		private void UpdateScrollBehavior ()
		{
//			if (observer.SupportsScrollPattern == true)
//				SetBehavior (ScrollPatternIdentifiers.Pattern,
//				             new ScrollProviderBehavior (this));
//			else
				SetBehavior (ScrollPatternIdentifiers.Pattern, null);
		}

		#endregion

		#region Private Fields

		private SWF.DataGridView datagridView;
		private DataGridViewHeaderProvider header;
		private ScrollBehaviorObserver observer;

		#endregion

		#region Internal Class: Header Provider 
		
		internal class DataGridViewHeaderProvider : FragmentRootControlProvider
		{
			public DataGridViewHeaderProvider (SWF.DataGridView datagridview) : base (null)
			{
				this.datagridview = datagridview;
				headers = new Dictionary<SWF.DataGridViewColumn, DataGridViewHeaderItemProvider> ();
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { 
					return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (datagridview); 
				}
			}

			public SWF.DataGridView DataGridView {
				get { return datagridview; }
			}			

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Header.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return "Header";
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return Catalog.GetString ("header");
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
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
					if (!datagridview.ColumnHeadersVisible)
						return Helper.GetControlScreenBounds (new SD.Rectangle (0, 0, 0, 0), datagridview, true);
					else {
						SD.Rectangle bounds = SD.Rectangle.Empty;
						bounds.Height = datagridview.ColumnHeadersHeight;
						bounds.Width = datagridview.RowHeadersWidth;
						for (int index = 0; index < datagridview.Columns.Count; index++)
							bounds.Width += datagridview.Columns [index].Width;
						
						return Helper.GetControlScreenBounds (bounds, datagridview, true);
					}
				} else
					return base.GetProviderPropertyValue (propertyId);
			}

			public override void InitializeChildControlStructure ()
			{
				datagridview.Columns.CollectionChanged += OnColumnsCollectionChanged;
				
				foreach (SWF.DataGridViewColumn column in datagridview.Columns) {
					DataGridViewHeaderItemProvider headerItem 
						= new DataGridViewHeaderItemProvider (this, column);
					headerItem.Initialize ();
					OnNavigationChildAdded (false, headerItem);
					headers [column] = headerItem;
				}
			}

			private void OnColumnsCollectionChanged (object sender,
			                                         CollectionChangeEventArgs args)
			{
				SWF.DataGridViewColumn column = (SWF.DataGridViewColumn) args.Element;
				
				if (args.Action == CollectionChangeAction.Remove) {
					DataGridViewHeaderItemProvider headerItem = headers [column];
					OnNavigationChildRemoved (true, headerItem);
					headers.Remove (column);
				} else if (args.Action == CollectionChangeAction.Add) {
					DataGridViewHeaderItemProvider headerItem 
						= new DataGridViewHeaderItemProvider (this, column);
					headerItem.Initialize ();
					OnNavigationChildAdded (true, headerItem);
					headers [column] = headerItem;
				}
			}

			private Dictionary<SWF.DataGridViewColumn, DataGridViewHeaderItemProvider> headers;
			private SWF.DataGridView datagridview;
		}

		#endregion

		#region

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

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.HeaderItem.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return column.HeaderText;
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return Catalog.GetString ("header item");
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
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
					if (column == null || column.Index < 0)
						return Rect.Empty;

					Rect headerBounds
						= (Rect) headerProvider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					for (int index = 0; index < column.Index; index++)
						headerBounds.X += headerProvider.DataGridView.Columns [index].Width;

					headerBounds.X += headerProvider.DataGridView.RowHeadersWidth;
					headerBounds.Width = headerProvider.DataGridView.Columns [column.Index].Width;
					
					return headerBounds;
				} else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					Rect bounds 
						= (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					return Helper.IsOffScreen (bounds, headerProvider.DataGridView, true);
				} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private SWF.DataGridViewColumn column;
			private DataGridViewHeaderProvider headerProvider;
		}

		#endregion

		#region Internal Class: Data Item

		internal class DataGridDataItemProvider : ListItemProvider
		{
			public DataGridDataItemProvider (DataGridViewProvider datagridProvider,
			                                 SWF.DataGridView datagridview,
		                                     SWF.DataGridViewRow row) 
				: base (datagridProvider, datagridProvider, datagridview, row)
			{
				this.datagridview = datagridview;
				this.row = row;
			}

			public SWF.DataGridView DataGridView {
				get { return datagridview; }
			}

			public SWF.DataGridViewRow Row {
				get { return row; }
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.DataItem.Id;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return Catalog.GetString ("data item");
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			public override void InitializeChildControlStructure ()
			{
				for (int index = 0; index < datagridview.Columns.Count; index++) {
					DataGridViewDataItemChildProvider child
						= new DataGridViewDataItemChildProvider (this, 
						                                         datagridview.Columns [index],
						                                         Row.Cells [index]);
					child.Initialize ();
					OnNavigationChildAdded (false, child);
				}
			}

			private SWF.DataGridView datagridview;
			private SWF.DataGridViewRow row;
		}

		#endregion

		#region Internal Class: Data Item Child Provider

		internal class DataGridViewDataItemChildProvider : FragmentControlProvider
		{
			public DataGridViewDataItemChildProvider (DataGridDataItemProvider itemProvider,
			                                          SWF.DataGridViewColumn column,
			                                          SWF.DataGridViewCell cell) : base (null)
			{
				this.itemProvider = itemProvider;
				this.column = column;
				this.cell = cell;
			}

			public SWF.DataGridViewColumn Column {
				get { return column; }
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return itemProvider; }
			}

			public override void Initialize ()
			{
				base.Initialize ();

				//LAMESPEC: Vista does not support Text Pattern.
				//http://msdn.microsoft.com/en-us/library/ms748367.aspx

//				SetBehavior (GridItemPatternIdentifiers.Pattern,
//				             new DataItemChildGridItemProviderBehavior (this));
//				SetBehavior (TableItemPatternIdentifiers.Pattern,
//				             new DataItemChildTableItemProviderBehavior (this));
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new DataItemChildValueProviderBehavior (this));

//				// Automation Events
//				SetEvent (ProviderEventType.AutomationElementIsOffscreenProperty,
//				          new ListItemEditAutomationIsOffscreenPropertyEvent (this));
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				// TODO: Generalize?
				
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id) {
					if (typeof (SWF.DataGridViewButtonColumn) == column.GetType ())
						return ControlType.Button.Id;
					else if (typeof (SWF.DataGridViewCheckBoxColumn) == column.GetType ())
						return ControlType.CheckBox.Id;
					else if (typeof (SWF.DataGridViewComboBoxColumn) == column.GetType ())
						return ControlType.ComboBox.Id;
					else if (typeof (SWF.DataGridViewImageColumn) == column.GetType ())
						return ControlType.Image.Id;
					else if (typeof (SWF.DataGridViewLinkColumn) == column.GetType ())
						return ControlType.Hyperlink.Id;
					else // SWF.DataGridViewTextBoxColumn or something else
						return ControlType.Edit.Id;
				} else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id) {
					if (typeof (SWF.DataGridViewButtonColumn) == column.GetType ())
						return Catalog.GetString ("button");
					else if (typeof (SWF.DataGridViewCheckBoxColumn) == column.GetType ())
						return Catalog.GetString ("checkbox");
					else if (typeof (SWF.DataGridViewComboBoxColumn) == column.GetType ())
						return Catalog.GetString ("combobox");
					else if (typeof (SWF.DataGridViewImageColumn) == column.GetType ())
						return Catalog.GetString ("image");
					else if (typeof (SWF.DataGridViewLinkColumn) == column.GetType ())
						return Catalog.GetString ("hyperlink");
					else // SWF.DataGridViewTextBoxColumn or something else
						return Catalog.GetString ("edit");
				} else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return cell.Value as string;
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.HelpTextProperty.Id)
					return cell.ToolTipText;
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
					Rect itemBounds
						= (Rect) itemProvider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);

					for (int index = 0; index < cell.ColumnIndex; index++)
						itemBounds.X += itemProvider.DataGridView.Columns [index].Width;

					itemBounds.Width = itemProvider.DataGridView.Columns [cell.ColumnIndex].Width;

					return itemBounds;
				} else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
					return false;
//					return Helper.IsListItemOffScreen ((Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id),
//					                                   itemProvider.DataGridView, 
//					                                   true,
//					                                   ItemProvider.ListView.UIAHeaderControl,
//					                                   ItemProvider.ListViewProvider.ScrollBehaviorObserver);
				else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private SWF.DataGridViewCell cell;
			private SWF.DataGridViewColumn column;
			private DataGridDataItemProvider itemProvider;
		}

		#endregion

		#region Internal Class: ScrollBar provider

		internal class DataGridViewScrollBarProvider : ScrollBarProvider
		{
			public DataGridViewScrollBarProvider (SWF.ScrollBar scrollbar,
			                                      DataGridViewProvider provider)
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
			
			private DataGridViewProvider provider;
			private string name;

		} // DataGridViewScrollBarProvider

		#endregion
	}
}
