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
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms.Behaviors.DataGridView;

namespace Mono.UIAutomation.Winforms
{
		
	[MapsComponent (typeof (SWF.DataGridView))]
	internal class DataGridViewProvider : ListProvider
		//, IScrollBehaviorSubject
	{
		
		public DataGridViewProvider (SWF.DataGridView datagridView) 
			: base (datagridView)
		{
			this.datagridView = datagridView;
		}

		#region Overridden Methods

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
			throw new System.NotImplementedException();
		}

		public override void SelectItem (ListItemProvider item)
		{
			throw new System.NotImplementedException();
		}

		public override void ScrollItemIntoView (ListItemProvider item)
		{
		}

		public override ListItemProvider[] GetSelectedItems ()
		{
			throw new System.NotImplementedException ();
		}
		
		public override int SelectedItemsCount {
			get {
				throw new System.NotImplementedException();
			}
		}

		public override bool IsItemSelected (ListItemProvider item)
		{
			return false;
		}
		
		public override int ItemsCount {
			get {
				throw new System.NotImplementedException();
			}
		}

		public override int IndexOfObjectItem (object objectItem)
		{
			throw new System.NotImplementedException();
		}

		public override object GetItemPropertyValue (ListItemProvider item, int propertyId)
		{
			// FIXME: Implement
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
//				                                   listView, 
//				                                   HeaderProvider != null,
//				                                   listView.UIAHeaderControl,
//				                                   observer);
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
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

		#endregion

		#region Privaet Fields

		private SWF.DataGridView datagridView;

		#endregion

		#region Internal Class: Header Provider 
		
		internal class DataGridViewHeaderProvider : FragmentRootControlProvider
		{
			public DataGridViewHeaderProvider (SWF.DataGridView datagridview) : base (null)
			{
				this.datagridview = datagridview;
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { 
					return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (datagridview); 
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
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
					return Helper.RectangleToRect (SD.Rectangle.Empty);
					//return Helper.GetControlScreenBounds (listView.UIAHeaderControl, listView, true);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			public override void InitializeChildControlStructure ()
			{
				// FIXME: Add children
			}

			private SWF.DataGridView datagridview;
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
//					                                   ItemProvider.ListView, 
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
	}
}
