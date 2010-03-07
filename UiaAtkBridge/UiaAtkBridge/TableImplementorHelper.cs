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
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Services;
using System.Collections.Generic;

namespace UiaAtkBridge
{
	internal class TableImplementorHelper
	{
		public TableImplementorHelper (ComponentParentAdapter resource)
		{
			this.resource = resource;
			
			tableProvider = (ITableProvider) resource.Provider.GetPatternProvider (TablePatternIdentifiers.Pattern.Id);
		}

		private Adapter						resource;
		private ITableProvider				tableProvider = null;
		private IGridProvider				gridProvider = null;
		private Atk.Object caption = null;
		private Atk.Object summary = null;

		internal IGridProvider GridProvider {
			get {
				if (tableProvider != null)
					return tableProvider;
				if (gridProvider == null)
					gridProvider = (IGridProvider) resource.Provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
				return gridProvider;
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

				return AutomationBridge.GetAdapterForProviderSemiLazy (
					headers [column]);
			}

			// GetItem indexes through only items, not headers, so
			// we need to remap the row number
			if (headers != null)
				row -= 1;

			IRawElementProviderSimple item;
			try {
				item = GridProvider.GetItem (row, column);
			} catch (ArgumentOutOfRangeException e) {
				Log.Debug (e);	
				return null;
			}

			if (item == null)
				return null;

			return AutomationBridge.GetAdapterForProviderSemiLazy (item);
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
			index--;

			Adapter child = RefProviderChildByDepthSearch (index);
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
			index--;

			int ret = 0;
			if (tableProvider != null) {
				IRawElementProviderSimple[] headers
					= tableProvider.GetColumnHeaders ();
				if (headers != null && headers.Length > 0) {
					if (index >= headers.Length)
						ret = 1;
				}
			}

			Adapter child = RefProviderChildByDepthSearch (index);
			if (child != null && child.Provider != null) {
				IGridItemProvider g = (IGridItemProvider) child.Provider.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id);
				if (g != null)
					ret += g.Row;
				return ret;
			}
			return -1;
		}

		//this kind of search will work regardless of the technique used for the hierarchy layout used for
		//the children (mono-level or multi-level), because in the provider side it's multi-level always
		private Adapter RefProviderChildByDepthSearch (int pos)
		{
			Adapter adapter = null;
			var frag = resource.Provider as IRawElementProviderFragment;
			var parent = frag;

			while (pos >= 0) {
				if (frag == null)
					return null;
				frag = frag.Navigate (NavigateDirection.FirstChild);
				if (frag == null)
					frag = parent.Navigate (NavigateDirection.NextSibling);

				if (frag != null)
					parent = frag;
				else {
					if (parent != resource.Provider) {
						parent = parent.Navigate (NavigateDirection.Parent);
						frag = parent.Navigate (NavigateDirection.NextSibling);
					} else
						return null;
				}

				adapter = AutomationBridge.GetAdapterForProviderSemiLazy (frag);
				//we need this check because there are some children providers that don't have an Adapter
				if (adapter != null)
					pos--;
			}
			return adapter;
		}

		public int NColumns {
			get {
				if (GridProvider != null)
					return GridProvider.ColumnCount;
				return -1;
			}
		}

		public int NRows {
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

			IRawElementProviderSimple item;
			try {
				item = GridProvider.GetItem (row, column);
			} catch (ArgumentOutOfRangeException e) {
				Log.Debug (e);
				return -1;
			}

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

			IRawElementProviderSimple item;
			try {
				item = GridProvider.GetItem (row, column);
			} catch (ArgumentOutOfRangeException e) {
				Log.Debug (e);
				return -1;
			}

			g = (IGridItemProvider) item.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id);
			if (g != null)
				return g.RowSpan;
			return 1;
		}

		public Atk.Object Caption {
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

		public Atk.Object Summary {
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
			selected = new int [0];
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

		public int [] SelectedColumns {
			get {
				int [] selected;
				GetSelectedColumns (out selected);
				return selected;
			}
		}

		public int [] SelectedRows {
			get {
				int [] selected;
				GetSelectedRows (out selected);
				return selected;
			}
		}

		// The below function should go away as soon as the atk-sharp api is fixed (BNC#512477)
		public int GetSelectedColumns (out int selected)
		{
			Log.Warn ("TableImplementorHelper: GetSelectedColumns not implemented.");
			selected = 0;
			return 0;
		}

		// The below function should go away as soon as the atk-sharp api is fixed (BNC#512477)
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
			} catch (ArgumentOutOfRangeException e) {
				Log.Debug (e);
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

			IRawElementProviderSimple item;
			try {
				item = GridProvider.GetItem (row, column);
			} catch (ArgumentOutOfRangeException e) {
				Log.Debug (e);
				return false;
			}

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
			
			IRawElementProviderSimple item;
			try {
				// UIA doesn't support row selection, so we select the first cell
				item = GridProvider.GetItem (row, 0);
			} catch (ArgumentOutOfRangeException e) {
				Log.Debug (e);
				return false;
			}

			if (item == null)
				return false;

			ISelectionItemProvider selectionItem
				= (ISelectionItemProvider) item.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			if (selectionItem == null)
				return false;
			
			try {
				selectionItem.AddToSelection ();
			} catch (InvalidOperationException e) {
				Log.Debug (e);
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
			
			IRawElementProviderSimple item;
			try {
				item = GridProvider.GetItem (row, 0);
			} catch (ArgumentOutOfRangeException e) {
				Log.Debug (e);
				return false;
			}
			
			if (item == null)
				return false;

			ISelectionItemProvider selectionItem
				= (ISelectionItemProvider) item.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			if (selectionItem == null)
				return false;
			
			try {
				selectionItem.RemoveFromSelection ();
			} catch (InvalidOperationException e) {
				Log.Debug (e);
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

		private bool AreRowColInBounds (int row, int col)
		{
			return (row >= 0 && row < NRows)
			       && (col >= 0 && col < NColumns);
		}
	}
}
