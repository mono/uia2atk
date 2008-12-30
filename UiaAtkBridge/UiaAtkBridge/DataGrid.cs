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

	public class DataGrid : ComponentParentAdapter , Atk.TableImplementor
	{
		private TableImplementorHelper tableExpert = null;
		
		public DataGrid (IRawElementProviderSimple provider): base (provider)
		{
			tableExpert = new TableImplementorHelper (this);
			Role = Atk.Role.TreeTable;
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == GridPatternIdentifiers.ColumnReorderedEvent)
				GLib.Signal.Emit (this, "column_reordered");
			else
				base.RaiseAutomationEvent (eventId, e);
			// TODO
		}

		public override void RaiseStructureChangedEvent (object childProvider, StructureChangedEventArgs e)
		{
			// TODO
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == AutomationElementIdentifiers.ControlTypeProperty) {
				//We remove the current Adapter and add it again to reflect the ControlType change
				StructureChangedEventArgs args 
					= new StructureChangedEventArgs (StructureChangeType.ChildRemoved, 
					                                 new int[] { 0 }); //TODO: Fix ?
				AutomationInteropProvider.RaiseStructureChangedEvent (Provider,
				                                                      args);

				args = new StructureChangedEventArgs (StructureChangeType.ChildAdded,
				                                      new int[] { 0 }); //TODO: Fix ?
				AutomationInteropProvider.RaiseStructureChangedEvent (Provider,
				                                                      args);
			} else
				base.RaiseAutomationPropertyChangedEvent (e);
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
			get { return tableExpert.Summary; } set { tableExpert.Summary = value; }
		}


		public void SetColumnDescription (int column, string description)
		{
			Console.WriteLine ("UiaAtkBridge: SetColumnDescription unimplemented");
		}

		public void SetColumnHeader (int column, Atk.Object header)
		{
			Console.WriteLine ("UiaAtkBridge: SetColumnHeader unimplemented");
		}

		public void SetRowDescription (int row, string description)
		{
			Console.WriteLine ("UiaAtkBridge: SetRowDescription unimplemented");
		}

		public void SetRowHeader (int row, Atk.Object header)
		{
			Console.WriteLine ("UiaAtkBridge: SetRowHeader unimplemented");
		}

		// TODO: Remove next two methods when atk-sharp is fixed
		public int GetSelectedColumns (out int selected)
		{
			return tableExpert.GetSelectedColumns (out selected);
		}

		public int GetSelectedColumns (out int [] selected)
		{
			return tableExpert.GetSelectedColumns (out selected);
		}

		public int GetSelectedRows (out int selected)
		{
			return tableExpert.GetSelectedRows (out selected);
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

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			states.AddState (Atk.StateType.ManagesDescendants);
			states.AddState (Atk.StateType.Focusable);
			return states;
		}
	}
}
