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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class ComboBoxOptionsTable : ComboBoxOptions, Atk.TableImplementor
	{
		
		public ComboBoxOptionsTable (IRawElementProviderSimple provider) : base (provider)
		{
			Role = Atk.Role.TreeTable;
		}
		
		#region TableImplementor implementation 
		
		public Atk.Object RefAt (int row, int column)
		{
			throw new System.NotImplementedException ();
		}
		
		public int GetIndexAt (int row, int column)
		{
			if ((column != 0) || (row < 0) || (row >= NAccessibleChildren))
				return -1;

			return row + 1;
		}
		
		public int GetColumnAtIndex (int index)
		{
			index = 0;
			return 0;
		}
		
		public int GetRowAtIndex (int index)
		{
			if ((index < 0) || (index > NAccessibleChildren))
				return -1;
			return index - 1;
		}
		
		public int GetColumnExtentAt (int row, int column)
		{
			return 0;
		}
		
		public int GetRowExtentAt (int row, int column)
		{
			return 0;
		}
		
		public string GetColumnDescription (int column)
		{
			throw new System.NotImplementedException();
		}
		
		public Atk.Object GetColumnHeader (int column)
		{
			throw new System.NotImplementedException ();
		}
		
		public string GetRowDescription (int row)
		{
			throw new System.NotImplementedException ();
		}
		
		public Atk.Object GetRowHeader (int row)
		{
			throw new System.NotImplementedException();
		}
		
		public void SetColumnDescription (int column, string description)
		{
			throw new System.NotImplementedException ();
		}
		
		public void SetColumnHeader (int column, Atk.Object header)
		{
			throw new System.NotImplementedException ();
		}
		
		public void SetRowDescription (int row, string description)
		{
			throw new System.NotImplementedException ();
		}
		
		public void SetRowHeader (int row, Atk.Object header)
		{
			throw new System.NotImplementedException ();
		}
		
		public int GetSelectedColumns (out int selected)
		{
			selected = 0;
			return 0;
		}
		
		public int GetSelectedRows (out int selected)
		{
			selected = 0;
			return 0;
		}
		
		public bool IsColumnSelected (int column)
		{
			return false;
		}
		
		public bool IsRowSelected (int row)
		{
			return IsChildSelected (row);
		}
		
		public bool IsSelected (int row, int column)
		{
			if (column != 0)
				return false;
			return IsRowSelected (row);
		}
		
		public bool AddRowSelection (int row)
		{
			throw new System.NotImplementedException();
		}
		
		public bool RemoveRowSelection (int row)
		{
			throw new System.NotImplementedException();
		}
		
		public bool AddColumnSelection (int column)
		{
			return false;
		}
		
		public bool RemoveColumnSelection (int column)
		{
			return false;
		}
		
		public int NColumns {
			get {
				return 1;
			}
		}
		
		public int NRows {
			get {
				return NAccessibleChildren;
			}
		}
		
		public Atk.Object Caption { get; set; }
		
		public Atk.Object Summary { get; set; }
		
		#endregion 
		
	}
}
