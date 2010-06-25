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
//  Matt Guo <matt@mattguo.com>
// 

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;
using At = System.Windows.Automation.Automation;
using NUnit.Framework;
using MonoTests.System.Windows.Automation;

namespace MonoTests.System.Windows.Automation
{
	//This TestFixture will test GridPattern, TablePattern, GridItemPattern and TableItemPattern
	[TestFixture]
	public class TableTest : BaseTest
	{
		private GridPattern gridPattern = null;
		private TablePattern tablePattern = null;

		protected override void CustomFixtureSetUp ()
		{
			base.CustomFixtureSetUp ();
			gridPattern = (GridPattern) table1Element.GetCurrentPattern (GridPattern.Pattern);
			tablePattern = (TablePattern) table1Element.GetCurrentPattern (TablePattern.Pattern);
		}

		[Test]
		public void IdentifierTest ()
		{
			Assert.AreEqual (GridPatternIdentifiers.Pattern.Id,
			                 GridPattern.Pattern.Id,
			                 "GridPattern.Id");
			Assert.AreEqual (GridPatternIdentifiers.Pattern.ProgrammaticName,
			                 GridPattern.Pattern.ProgrammaticName,
			                 "GridPattern.ProgrammaticName");
			Assert.AreEqual (GridItemPatternIdentifiers.Pattern.Id,
			                 GridItemPattern.Pattern.Id,
			                 "GridItemPattern.Id");
			Assert.AreEqual (GridItemPatternIdentifiers.Pattern.ProgrammaticName,
			                 GridItemPattern.Pattern.ProgrammaticName,
			                 "GridItemPattern.ProgrammaticName");
			Assert.AreEqual (TablePatternIdentifiers.Pattern.Id,
			                 TablePattern.Pattern.Id,
			                 "TablePattern.Id");
			Assert.AreEqual (TablePatternIdentifiers.Pattern.ProgrammaticName,
			                 TablePattern.Pattern.ProgrammaticName,
			                 "TablePattern.ProgrammaticName");
			Assert.AreEqual (TableItemPatternIdentifiers.Pattern.Id,
			                 TableItemPattern.Pattern.Id,
			                 "TableItemPattern.Id");
			Assert.AreEqual (TableItemPatternIdentifiers.Pattern.ProgrammaticName,
			                 TableItemPattern.Pattern.ProgrammaticName,
			                 "TableItemPattern.ProgrammaticName");
		}

		[Ignore]
		//todo see bug# 549115
		[Test]
		public void GridPatternTest ()
		{
			GridPatternTestInternal (gridPattern);
			GridPatternTestInternal (tablePattern);
		}

		[Test]
		public void TablePatternTest ()
		{
			RowOrColumnMajor expectedMajor = (Atspi
				? RowOrColumnMajor.Indeterminate
				: RowOrColumnMajor.RowMajor);
			Assert.AreEqual (expectedMajor, tablePattern.Current.RowOrColumnMajor,
			                 "RowOrColumnMajor");
			Assert.AreEqual (0, tablePattern.Current.GetRowHeaders ().Length, "row headers");
			var colHeaders = tablePattern.Current.GetColumnHeaders ();
			VerifyTableColumnHeaders (colHeaders);
		}

		[Test]
		public void GridItemPatternTest ()
		{
			var pattern = (GridItemPattern) gridPattern.GetItem (1, 2).
				GetCurrentPattern (GridItemPattern.Pattern);
			Assert.AreEqual (2, pattern.Current.Column, "Column");
			Assert.AreEqual (1, pattern.Current.Row, "Row");
			Assert.AreEqual (1, pattern.Current.ColumnSpan, "ColumnSpan");
			Assert.AreEqual (1, pattern.Current.RowSpan, "RowSpan");
			/*var tmp = pattern.Current.ContainingGrid.Current;
			Console.WriteLine ("[{0}], [{1}], [{2}]", tmp.Name, tmp.ControlType.ProgrammaticName, tmp.BoundingRectangle);*/
			//todo Currently This test case fails, see #bug 549109
			Assert.AreEqual (table1Element, pattern.Current.ContainingGrid,
			                 "ContainingGrid");
		}

		[Test]
		public void TableItemPatternTest ()
		{
			var pattern = (TableItemPattern) tablePattern.GetItem (1, 2).
				GetCurrentPattern (TableItemPattern.Pattern);
			Assert.AreEqual (2, pattern.Current.Column, "Column");
			Assert.AreEqual (1, pattern.Current.Row, "Row");
			Assert.AreEqual (1, pattern.Current.ColumnSpan, "ColumnSpan");
			Assert.AreEqual (1, pattern.Current.RowSpan, "RowSpan");
			//todo comment below line to make following code run
			//Assert.AreEqual (table1Element, pattern.Current.ContainingGrid,
			//                 "ContainingGrid");
			Assert.AreEqual (0, pattern.Current.GetRowHeaderItems ().Length, "row headers");
			VerifyTableColumnHeaders (pattern.Current.GetColumnHeaderItems ());
		}

		[Test]
		//The "Z_" prefix ensures this test runs last, since it will change the column/row count of the data grid
		//todo Currently This test case fails, see #bug 549112
		public void Z_DynamicTest ()
		{
			var automationEventsArray = new [] {
				new {Sender = (object) null, Args = (AutomationPropertyChangedEventArgs) null}};
			var automationEvents = automationEventsArray.ToList ();
			automationEvents.Clear ();

			AutomationPropertyChangedEventHandler handler =
				(o, e) => automationEvents.Add (new { Sender = o, Args = e });
			At.AddAutomationPropertyChangedEventHandler (table1Element,
			                                             TreeScope.Element, handler,
			                                             GridPattern.RowCountProperty,
			                                             GridPattern.ColumnCountProperty);

			RunCommand ("add table row");
			Assert.AreEqual (1, automationEvents.Count, "event count");
			Assert.AreEqual (table1Element, automationEvents [0].Sender, "event sender");
			Assert.AreEqual (GridPattern.RowCountProperty, automationEvents [0].Args.Property, "property");
			int oldValue = (Atspi? 2: 3);
			Assert.AreEqual (oldValue, automationEvents [0].Args.OldValue, "old value");
			Assert.AreEqual (oldValue + 1, automationEvents [0].Args.NewValue, "new value");
			automationEvents.Clear ();

			RunCommand ("add table column");
			Assert.AreEqual (1, automationEvents.Count, "event count");
			Assert.AreEqual (table1Element, automationEvents [0].Sender, "event sender");
			Assert.AreEqual (GridPattern.ColumnCountProperty, automationEvents [0].Args.Property, "property");
			Assert.AreEqual (3, automationEvents [0].Args.OldValue, "old value");
			Assert.AreEqual (4, automationEvents [0].Args.NewValue, "new value");
		}

		private void GridPatternTestInternal (GridPattern pattern)
		{
			Assert.AreEqual (3, pattern.Current.ColumnCount, "ColumnCount");
			//Besides the 2 data rows, there is an additional new data row.
			Assert.AreEqual (3, pattern.Current.RowCount, "RowCount");
			var child = pattern.GetItem (0, 1);
			Assert.AreEqual ("Alice", child.Current.Name, "Item [1,1].Name");
			child = pattern.GetItem (1, 0);
			Assert.AreEqual (ControlType.CheckBox, child.Current.ControlType, "Item [0,0].ControlType");
			var toggle = (TogglePattern ) child.GetCurrentPattern (TogglePattern.Pattern);
			Assert.AreEqual (ToggleState.On, toggle.Current.ToggleState, "Item [0,0].ToggleState");
		}

		private void VerifyTableColumnHeaders (AutomationElement[] colHeaders)
		{
			Assert.AreEqual (3, colHeaders.Length, "col_headers.Length");
			Assert.AreEqual ("Gender", colHeaders [0].Current.Name, "colHeaders[0].Name");
			Assert.AreEqual ("Name", colHeaders [1].Current.Name, "colHeaders[1].Name");
			Assert.AreEqual ("Age", colHeaders [2].Current.Name, "colHeaders[2].Name");
			Assert.AreEqual (ControlType.HeaderItem, colHeaders [0].Current.ControlType, "colHeaders[0].ControlType");
		}
	}
}
