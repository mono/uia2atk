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
using System.Collections.Generic;

using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	internal class TableImplementorHelper
	{
		public TableImplementorHelper (ComponentParentAdapter resource)
		{
			this.resource = resource;
			
			if (resource.Provider is ITableProvider)
				tableProvider = (ITableProvider)resource.Provider;
			if (resource.Provider is IGridProvider)
				gridProvider = (ITableProvider)resource.Provider;
		}

		private Adapter								resource;
		private ITableProvider					tableProvider = null;
		private IGridProvider					gridProvider = null;

		public Atk.Object RefAt (int row, int column)
		{
			IRawElementProviderSimple item = gridProvider.GetItem (row, column);
			return AutomationBridge.GetAdapterForProvider (item);
		}

		public int GetIndexAt (int row, int column)
		{
			throw new NotImplementedException();
		}

		public int GetColumnAtIndex (int index)
		{
			ITableItemProvider t = (ITableItemProvider) resource.RefAccessibleChild (index);
			return t.Column;
		}

		public int GetRowAtIndex (int index)
		{
			ITableItemProvider t = (ITableItemProvider) resource.RefAccessibleChild (index);
			return t.Row;
		}

		public int NColumns
		{
			get {
				if (gridProvider != null)
					return gridProvider.RowCount;
				return -1;
			}
		}

		public int NRows
		{
			get {
				if (gridProvider != null)
					return gridProvider.RowCount;
				return -1;
			}
		}

		public int GetColumnExtentAt (int row, int column)
		{
			IRawElementProviderSimple item = gridProvider.GetItem (row, column);
			return ((IGridItemProvider)item).ColumnSpan;
		}

		public int GetRowExtentAt (int row, int column)
		{
			IRawElementProviderSimple item = gridProvider.GetItem (row, column);
			return ((IGridItemProvider)item).RowSpan;
		}

		public Atk.Object Caption
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public string GetColumnDescription (int column)
		{
			throw new NotImplementedException();
		}

		public Atk.Object GetColumnHeader (int column)
		{
			// TODO: UIA supports multiple headers, but ATK doesn't
			IRawElementProviderSimple item = tableProvider.GetColumnHeaders()[0];
			return AutomationBridge.GetAdapterForProvider (item);
		}

		public string GetRowDescription (int row)
		{
			throw new NotImplementedException();
		}

		public Atk.Object GetRowHeader (int row)
		{
			// TODO: UIA supports multiple headers, but ATK doesn't
			IRawElementProviderSimple item = tableProvider.GetRowHeaders()[0];
			return AutomationBridge.GetAdapterForProvider (item);
		}

		public Atk.Object Summary
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public void SetColumnDescription (int column, string description)
		{
			throw new NotImplementedException();
		}

		public void SetColumnHeader (int column, Atk.Object header)
		{
			throw new NotImplementedException();
		}

		public void SetRowDescription (int row, string description)
		{
			throw new NotImplementedException();
		}

		public void SetRowHeader (int row, Atk.Object header)
		{
			throw new NotImplementedException();
		}

		public int GetSelectedColumns (out int selected)
		{
			throw new NotImplementedException();
		}

		public int GetSelectedRows (out int selected)
		{
			throw new NotImplementedException();
		}

		public bool IsColumnSelected (int column)
		{
			throw new NotImplementedException();
		}

		public bool IsRowSelected (int row)
		{
			throw new NotImplementedException();
		}

		public bool IsSelected (int row, int column)
		{
			throw new NotImplementedException();
		}

		public bool AddRowSelection (int row)
		{
			throw new NotImplementedException();
		}

		public bool RemoveRowSelection (int row)
		{
			throw new NotImplementedException();
		}

		public bool AddColumnSelection (int column)
		{
			throw new NotImplementedException();
		}

		public bool RemoveColumnSelection (int column)
		{
			throw new NotImplementedException();
		}

	}
}
