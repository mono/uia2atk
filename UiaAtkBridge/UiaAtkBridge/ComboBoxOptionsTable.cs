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
using Mono.UIAutomation.Services;

namespace UiaAtkBridge
{
	public class ComboBoxOptionsTable : ComboBoxOptions, Atk.ITableImplementor
	{
		//FIXME: only used for untested behaviours:
		TableImplementorHelper tableExpert = null;
		
		public ComboBoxOptionsTable (IRawElementProviderSimple provider) : base (provider)
		{
			Role = Atk.Role.TreeTable;
			tableExpert = new TableImplementorHelper (this);
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			if (states.ContainsState (Atk.StateType.Defunct))
				return states;

			states.RemoveState (Atk.StateType.Focused);
			states.AddState (Atk.StateType.ManagesDescendants);
			
			//TODO: figure out why the gail side doesn't like this state:
			states.RemoveState (Atk.StateType.Selectable);
			
			return states;
		}
		
		#region TableImplementor implementation 
		
		public Atk.Object RefAt (int row, int column)
		{
			return tableExpert.RefAt (row, column);
		}
		
		public string GetColumnDescription (int column)
		{
			return tableExpert.GetColumnDescription (column);
		}

		public bool AddRowSelection (int row)
		{
			return tableExpert.AddRowSelection (row);
		}
		
		public bool RemoveRowSelection (int row)
		{
			return tableExpert.RemoveRowSelection (row);
		}
		
		public Atk.Object GetColumnHeader (int column)
		{
			return tableExpert.GetColumnHeader (column);
		}
		
		public string GetRowDescription (int row)
		{
			return tableExpert.GetRowDescription (row);
		}
		
		public Atk.Object GetRowHeader (int row)
		{
			return tableExpert.GetRowHeader (row);
		}
		
		public void SetColumnDescription (int column, string description)
		{
			tableExpert.SetColumnDescription (column, description);
		}
		
		public void SetColumnHeader (int column, Atk.Object header)
		{
			tableExpert.SetColumnHeader (column, header);
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
		
		public void SetRowDescription (int row, string description)
		{
			tableExpert.SetRowDescription (row, description);
		}
		
		public void SetRowHeader (int row, Atk.Object header)
		{
			tableExpert.SetRowHeader (row, header);
		}

		public int [] SelectedColumns {
			get { return new int [0]; }
		}

		//TODO: return the selected items
		public int [] SelectedRows {
			get { return new int [0]; }
		}

		//FIXME: remove this once we expose the correct overload (BNC#512477)
		public int GetSelectedColumns (out int selected)
		{
			selected = 0;
			return 0;
		}

		//FIXME: remove this once we expose the correct overload (BNC#512477)
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
		
		public bool AddColumnSelection (int column)
		{
			return false;
		}
		
		public bool RemoveColumnSelection (int column)
		{
			return false;
		}
		
		public int NColumns {
			get { return 1; }
		}
		
		public int NRows {
			get { return NAccessibleChildren; }
		}
		
		public Atk.Object Caption { get; set; }
		
		public Atk.Object Summary { get; set; }
		
		#endregion 
		
	}
}
