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

namespace UiaAtkBridge
{

	public class ListGroup: ComponentParentAdapter, Atk.SelectionImplementor, Atk.TableImplementor
	{

		private ISelectionProvider					selectionProvider;
		private SelectionProviderUserHelper	selectionHelper;

		private TableImplementorHelper tableExpert = null;

		public ListGroup (IRawElementProviderFragment provider): base (provider)
		{
			tableExpert = new TableImplementorHelper (this);

			IRawElementProviderFragment listProvider = provider.Navigate (NavigateDirection.Parent);
			selectionProvider = (ISelectionProvider)listProvider.GetPatternProvider(SelectionPatternIdentifiers.Pattern.Id);
			if (selectionProvider == null)
				throw new ArgumentException ("List should always implement ISelectionProvider");
			
			selectionHelper = new SelectionProviderUserHelper (provider, selectionProvider);
			Role = Atk.Role.LayeredPane;
		}

		public override void RaiseStructureChangedEvent (object childProvider, StructureChangedEventArgs e)
		{
			// TODO
		}

		public int SelectionCount
		{
			get { return selectionHelper.SelectionCount; }
		}
		public bool AddSelection (int i)
		{
			selectionHelper.AddSelection (i);
		 	//FIXME: currently unit-tests force this to always true, we may be interested in changing them when we report the gail bug about this (see ComboBox.cs)
			return true;
		}
		public bool ClearSelection ()
		{
			return selectionHelper.ClearSelection ();
		}
		public bool IsChildSelected (int i)
		{
			return selectionHelper.IsChildSelected (i);
		}
		public Atk.Object RefSelection (int i)
		{
			return selectionHelper.RefSelection (i);
		}
		public bool RemoveSelection (int i)
		{
			return selectionHelper.RemoveSelection (i);
		}
		public bool SelectAllSelection ()
		{
			return selectionHelper.SelectAllSelection ();
		}

		public Atk.Object RefAt (int row, int column)
		{
			return tableExpert.RefAt (row, column);
		}

		public int GetIndexAt (int row, int column)
		{
			return tableExpert.GetIndexAt (row, column);
		}

		public int GetColumnAtIndex (int index)
		{
			return tableExpert.GetColumnAtIndex (index);
		}

		public int GetRowAtIndex (int index)
		{
			return tableExpert.GetRowAtIndex (index);
		}

		public int NColumns { get { return tableExpert.NColumns; } }
		public int NRows { get { return tableExpert.NRows; } }
			
		public int GetColumnExtentAt (int row, int column)
		{
			return tableExpert.GetColumnExtentAt (row, column);
		}

		public int GetRowExtentAt (int row, int column)
		{
			return tableExpert.GetRowExtentAt (row, column);
		}

		public Atk.Object Caption
		{
			get { return tableExpert.Caption; } set { tableExpert.Caption = value; }
		}

		public string GetColumnDescription (int column)
		{
			return string.Empty;
		}

		public Atk.Object GetColumnHeader (int column)
		{
			return null;
		}

		public string GetRowDescription (int row)
		{
			return String.Empty;
		}

		public Atk.Object GetRowHeader (int row)
		{
			return null;
		}

		public Atk.Object Summary
		{
			get { return tableExpert.Summary; } 
			set { tableExpert.Summary = value; }
		}


		public void SetColumnDescription (int column, string description)
		{
			tableExpert.SetColumnDescription (column, description);
		}

		public void SetColumnHeader (int column, Atk.Object header)
		{
			tableExpert.SetColumnHeader (column, header);
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
			get { return tableExpert.SelectedColumns; }
		}

		public int [] SelectedRows {
			get { return tableExpert.SelectedRows; }
		}

		// TODO: Remove next methods when atk-sharp is fixed (BNC#512477)
		public int GetSelectedColumns (out int selected)
		{
			return tableExpert.GetSelectedColumns (out selected);
		}

		public int GetSelectedRows (out int selected)
		{
			return tableExpert.GetSelectedRows (out selected);
		}

		public int GetSelectedColumns (out int [] selected)
		{
			return tableExpert.GetSelectedColumns (out selected);
		}

		public int GetSelectedRows (out int [] selected)
		{
			return tableExpert.GetSelectedRows (out selected);
		}

		public bool IsColumnSelected (int column)
		{
			return tableExpert.IsColumnSelected (column);
		}

		public bool IsRowSelected (int row)
		{
			return tableExpert.IsRowSelected (row);
		}

		public bool IsSelected (int row, int column)
		{
			return tableExpert.IsSelected (row, column);
		}

		public bool AddRowSelection (int row)
		{
			return tableExpert.AddRowSelection (row);
		}

		public bool RemoveRowSelection (int row)
		{
			return tableExpert.RemoveRowSelection (row);
		}

		public bool AddColumnSelection (int column)
		{
			return tableExpert.AddColumnSelection (column);
		}

		public bool RemoveColumnSelection (int column)
		{
			return tableExpert.RemoveColumnSelection (column);
		}
	}
}
