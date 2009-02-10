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
//      Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Windows.Automation;
using Mono.UIAutomation.Services;
using System.Collections.Generic;

using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	internal class TableImplementorHelper
	{
		public TableImplementorHelper (ComponentParentAdapter resource)
		{
			this.resource = resource;
			
			tableProvider = (ITableProvider) resource.Provider.GetPatternProvider (TablePatternIdentifiers.Pattern.Id);;
		}

		private Adapter							resource;
		private ITableProvider					tableProvider = null;
		private TableGroupAggregator aggregator = null;
		private Atk.Object caption = null;
		private Atk.Object summary = null;

		internal IGridProvider GridProvider {
			get {
				if (tableProvider != null)
					return tableProvider;
				IGridProvider grid = (IGridProvider) resource.Provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
				if (grid != null)
					return grid;
				if (aggregator == null) {
					IRawElementProviderFragment fragment = resource.Provider as IRawElementProviderFragment;
					aggregator = new TableGroupAggregator (fragment);
				}
				return aggregator;
			}
		}

		public Atk.Object RefAt (int row, int column)
		{
			if (!AreRowColInBounds (row, column))
				return null;

			IRawElementProviderSimple[] headers = null;
			if (tableProvider != null) {
				headers = tableProvider.GetColumnHeaders ();
			}

			// Some controls will have column headers that need to
			// be mapped to row 0
			if (row == 0 && headers != null) {
				if (column >= headers.Length)
					return null;

				return AutomationBridge.GetAdapterForProviderLazy (
					headers [column]);
			}

			// GetItem indexes through only items, not headers, so
			// we need to remap the row number
			if (headers != null)
				row -= 1;

			IRawElementProviderSimple item
				= GridProvider.GetItem (row, column);
			if (item == null)
				return null;

			return AutomationBridge.GetAdapterForProviderLazy (item);
		}

		public int GetIndexAt (int row, int column)
		{
			Adapter child = RefAt (row, column) as Adapter;
			if (child == null)
				return -1;

			return child.IndexInParent + 1;
		}

		public int GetColumnAtIndex (int index)
		{
			if (index <= 0)
				return 0;

			// Map from Atk's 1-based system to UIA's 0-based
			// indicies
			index -= 1;

			Adapter child = resource.RefAccessibleChild (index) as Adapter;
			if (child != null && child.Provider != null) {
				IGridItemProvider g = (IGridItemProvider) child.Provider.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id);
				if (g == null)	// ie, if a group header
					return 0;
				return g.Column;
			}
			return -1;
		}

		public int GetRowAtIndex (int index)
		{
			if (index <= 0)
				return -1;

			// Map from Atk's 1-based system to UIA's 0-based
			// indicies
			index -= 1;

			int ret = 0;
			if (tableProvider != null) {
				IRawElementProviderSimple[] headers
					= tableProvider.GetColumnHeaders ();
				if (headers != null && headers.Length > 0) {
					if (index >= headers.Length)
						ret = 1;
				}
			}

			Adapter child = resource.RefAccessibleChild (index) as Adapter;
			if (child != null && child.Provider != null) {
				IGridItemProvider g = (IGridItemProvider) child.Provider.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id);
				if (g != null)
					ret += g.Row;
				return ret + RowAdjustment (child.Provider);
			}
			return -1;
		}

		public int NColumns
		{
			get {
				if (GridProvider != null)
					return GridProvider.ColumnCount;
				return -1;
			}
		}

		public int NRows
		{
			get {
				if (GridProvider != null)
					return GridProvider.RowCount;
				return -1;
			}
		}

		public int GetColumnExtentAt (int row, int column)
		{
			if (!AreRowColInBounds (row, column))
				return -1;

			IGridItemProvider g = null;

			if (tableProvider != null) {
				IRawElementProviderSimple[] headers
					= tableProvider.GetColumnHeaders ();
				if (headers != null && headers.Length > 0) {
					if (row == 0) {
						g = headers [column].GetPatternProvider (
							GridItemPatternIdentifiers.Pattern.Id)
								as IGridItemProvider;
						return (g == null) ? 0 : g.ColumnSpan;
					}
					row -= 1;
				}
			}

			IRawElementProviderSimple item = GridProvider.GetItem (row, column);
			int controlTypeId = (int) item.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (controlTypeId == ControlType.Group.Id)
				return NColumns;
			g = (IGridItemProvider) item.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id);
			if (g != null)
				return g.ColumnSpan;
			return -1;
		}

		public int GetRowExtentAt (int row, int column)
		{
			if (!AreRowColInBounds (row, column))
				return -1;

			IGridItemProvider g = null;

			if (tableProvider != null) {
				IRawElementProviderSimple[] headers
					= tableProvider.GetColumnHeaders ();
				if (headers != null && headers.Length > 0) {
					if (row == 0) {
						g = headers [column].GetPatternProvider (
							GridItemPatternIdentifiers.Pattern.Id)
								as IGridItemProvider;
						return (g == null) ? 0 : g.RowSpan;
					}
					row -= 1;
				}
			}

			IRawElementProviderSimple item = GridProvider.GetItem (row, column);
			g = (IGridItemProvider) item.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id);
			if (g != null)
				return g.RowSpan;
			return 1;
		}

		public Atk.Object Caption
		{
			get { return caption; }
			set { caption = value; }
		}

		public string GetColumnDescription (int column)
		{
			if (tableProvider == null)
				return null;

			IRawElementProviderSimple [] items = tableProvider.GetColumnHeaders ();
			if (column < 0 || column >= items.Length)
				return null;

			return (string) items [column].GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
		}

		public Atk.Object GetColumnHeader (int column)
		{
			if (tableProvider == null)
				return null;
			
			IRawElementProviderSimple [] items = tableProvider.GetColumnHeaders ();
			if (column < 0 || column >= items.Length)
				return null;
			return AutomationBridge.GetAdapterForProviderLazy (items [column]);
		}

		public string GetRowDescription (int row)
		{
			if (tableProvider == null)
				return null;

			IRawElementProviderSimple [] items = tableProvider.GetRowHeaders ();
			if (row < 0 || row >= items.Length)
				return null;

			return (string) items [row].GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
		}

		public Atk.Object GetRowHeader (int row)
		{
			if (tableProvider == null)
				return null;
			
			IRawElementProviderSimple [] items = tableProvider.GetRowHeaders ();
			if (row < 0 || row >= items.Length)
				return null;
			return AutomationBridge.GetAdapterForProviderLazy (items [row]);
		}

		public Atk.Object Summary
		{
			get { return summary; }
			set { summary = value; }
		}

		public void SetColumnDescription (int column, string description)
		{
			Log.Warn ("TableImplementorHelper: SetColumnDescription not implemented.");
		}

		public void SetColumnHeader (int column, Atk.Object header)
		{
			Log.Warn ("TableImplementorHelper: SetColumnHeader not implemented.");
		}

		public void SetRowDescription (int row, string description)
		{
			Log.Warn ("TableImplementorHelper: SetRowDescription not implemented.");
		}

		public void SetRowHeader (int row, Atk.Object header)
		{
			Log.Warn ("TableImplementorHelper: SetRowHeader not implemented.");
		}

		public int GetSelectedColumns (out int [] selected)
		{
			Log.Warn ("TableImplementorHelper: GetSelectedColumns not implemented.");
			selected = null;
			return 0;
		}

		public int GetSelectedRows (out int [] selected)
		{
			ISelectionProvider selection 
				= (ISelectionProvider) resource.Provider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);
			if (selection == null) {
				selected = new int [0];
				return 0;
			}

			IRawElementProviderSimple []items = selection.GetSelection ();
			List<int> selectedItems = new List <int> ();
			foreach (IRawElementProviderSimple item in items) {
				ISelectionItemProvider selectionItem 
					= (ISelectionItemProvider) item.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
				IGridItemProvider gridItem 
					= (IGridItemProvider) item.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id);
				if (selectionItem != null && gridItem != null) {
					if (selectionItem.IsSelected)
						selectedItems.Add (gridItem.Row);
				}
			}

			selected = selectedItems.ToArray ();
			return selectedItems.Count;
		}

		// The below two functions should go away as soon as the
		// atk-sharp api is fixed.
		public int GetSelectedColumns (out int selected)
		{
			Log.Warn ("TableImplementorHelper: GetSelectedColumns not implemented.");
			selected = 0;
			return 0;
		}

		public int GetSelectedRows (out int selected)
		{
			// TODO: Logic should be the same as GetSelectedRows (out int [] selected)
			Log.Warn ("TableImplementorHelper: GetSelectedRows not implemented.");
			selected = 0;
			return 0;
		}

		public bool IsColumnSelected (int column)
		{
			// TODO: There's no UIA API to get selected columns
			Log.Warn ("TableImplementorHelper: IsColumnSelected not implemented.");
			return false;
		}

		public bool IsRowSelected (int row)
		{
			if (row < 0 || row >= NRows)
				return false;

			if (tableProvider != null) {
				IRawElementProviderSimple[] headers
					= tableProvider.GetColumnHeaders ();
				if (headers != null && headers.Length > 0) {
					// In UIA, header rows cannot be selected
					if (row == 0)
						return false;
					row -= 1;
				}
			}

			IRawElementProviderSimple item;
			try {
				item = GridProvider.GetItem (row, 0);
			} catch (ArgumentOutOfRangeException) {
				return false;
			}

			ISelectionItemProvider selectionItem 
				= (ISelectionItemProvider) item.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			if (selectionItem == null)
				return false;

			return selectionItem.IsSelected;
		}

		public bool IsSelected (int row, int column)
		{
			if (!AreRowColInBounds (row, column))
				return false;

			if (GridProvider == null)
				return false;

			if (tableProvider != null) {
				IRawElementProviderSimple[] headers
					= tableProvider.GetColumnHeaders ();
				if (headers != null && headers.Length > 0) {
					// In UIA, header rows cannot be selected
					if (row == 0)
						return false;
					row -= 1;
				}
			}

			IRawElementProviderSimple item = GridProvider.GetItem (row, column);
			if (item == null)
				return false;
			ISelectionItemProvider selectionItemProvider 
				= (ISelectionItemProvider) item.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			if (selectionItemProvider == null)
				return false;
			
			return selectionItemProvider.IsSelected;
		}

		public bool AddRowSelection (int row)
		{
			if (row < 0 || row >= NRows)
				return false;

			if (GridProvider == null)
				return false;

			if (tableProvider != null) {
				IRawElementProviderSimple[] headers
					= tableProvider.GetColumnHeaders ();
				if (headers != null && headers.Length > 0) {
					// In UIA, header rows cannot be selected
					if (row == 0)
						return false;
					row -= 1;
				}
			}
			
			IRawElementProviderSimple item = GridProvider.GetItem (row, 0);
			if (item == null)
				return false;

			ISelectionItemProvider selectionItem
				= (ISelectionItemProvider) item.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			if (selectionItem == null)
				return false;
			
			try {
				selectionItem.AddToSelection ();
			} catch (InvalidOperationException) {
				return false;
			}
			
			return true;
		}

		public bool RemoveRowSelection (int row)
		{
			if (row < 0 || row >= NRows)
				return false;

			if (GridProvider == null)
				return false;

			if (tableProvider != null) {
				IRawElementProviderSimple[] headers
					= tableProvider.GetColumnHeaders ();
				if (headers != null && headers.Length > 0) {
					// In UIA, header rows cannot be selected
					if (row == 0)
						return false;
					row -= 1;
				}
			}
			
			IRawElementProviderSimple item = GridProvider.GetItem (row, 0);
			if (item == null)
				return false;

			ISelectionItemProvider selectionItem
				= (ISelectionItemProvider) item.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			if (selectionItem == null)
				return false;
			
			try {
				selectionItem.RemoveFromSelection ();
			} catch (InvalidOperationException) {
				return false;
			}
			
			return true;
		}

		public bool AddColumnSelection (int column)
		{
			// TODO: There's no UIA API to selected columns
			Log.Warn ("TableImplementorHelper: AddColumnSelection not implemented.");
			return false;
		}

		public bool RemoveColumnSelection (int column)
		{
			// TODO: There's no UIA API to unselected columns
			Log.Warn ("TableImplementorHelper: RemoveColumnSelection not implemented.");
			return false;
		}

		internal int RowAdjustment (IRawElementProviderSimple provider)
		{
			if (GridProvider is TableGroupAggregator)
				return ((TableGroupAggregator)GridProvider).RowAdjustment ((IRawElementProviderFragment)provider);
			return 0;
		}

		private bool AreRowColInBounds (int row, int col)
		{
			return (row >= 0 && row < NRows)
			       && (col >= 0 && col < NColumns);
		}
	}

	internal class TableGroupAggregator : IGridProvider
	{
		private IRawElementProviderFragment provider;

		public TableGroupAggregator (IRawElementProviderFragment provider)
		{
			this.provider = provider;
		}

		public IRawElementProviderFragment Provider {
			get { return provider; }
		}

		public int ColumnCount {
			get {
				int adj;
				IRawElementProviderSimple g = GetGridForRow (0, out adj);
				IGridProvider grid = null;
				if (g != null)
					grid = (IGridProvider) g.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
				if (g != null)
					return grid.ColumnCount;
				return -1;
			}
		}

		public int RowCount {
			get {
				int rows = 0;
				for (IRawElementProviderFragment child = Provider.Navigate (NavigateDirection.FirstChild); child != null; child = child.Navigate (NavigateDirection.NextSibling)) {
					IGridProvider grid = (IGridProvider) child.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
					if (grid != null)
						rows += grid.RowCount + 1;
				}
				return rows;
			}
		}

		public IRawElementProviderSimple GetItem (int row, int column)
		{
			int adj;
			IRawElementProviderSimple g = GetGridForRow (row, out adj);
			if (g == null)
				return null;
			if (adj == -1)
				return g;
			IGridProvider grid = (IGridProvider) g.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
			if (grid == null)
				return null;
			int localRow = row - adj;
			if (localRow < 0 || localRow >= grid.RowCount || column < 0 || column >= grid.ColumnCount)
				return null;
			return grid.GetItem (localRow, column);
		}

		internal IRawElementProviderSimple GetGridForRow (int row, out int adj)
		{
			IRawElementProviderFragment child;
			adj = 0;
			for (child = Provider.Navigate (NavigateDirection.FirstChild); child != null; child = child.Navigate (NavigateDirection.NextSibling)) {
				IGridProvider grid = (IGridProvider) child.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
				if (grid == null)
				continue;
				if (adj + grid.RowCount >= row) {
					if (adj == row)
						adj = -1;	// header
					adj++;	// skip group header
					return child;
				}
				adj += grid.RowCount + 1;
			}
			return null;
		}

		internal int RowAdjustment (IRawElementProviderFragment provider)
		{
			IRawElementProviderFragment cur = provider;
			int controlTypeId = (int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			int adj = 0;
			IRawElementProviderFragment parent = provider.Navigate (NavigateDirection.Parent);
			int parentControlTypeId = (int) parent.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (controlTypeId == ControlType.DataItem.Id) {
				if (parentControlTypeId == ControlType.DataGrid.Id)
					return 0;
				cur = parent;
				adj = 1;
			}
			for (cur = cur.Navigate (NavigateDirection.PreviousSibling); cur != null; cur = cur.Navigate (NavigateDirection.PreviousSibling)) {
				IGridProvider grid = (IGridProvider) cur.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
				if (grid != null)
					adj += grid.RowCount + 1;
			}
			return adj;
		}
	}
}
