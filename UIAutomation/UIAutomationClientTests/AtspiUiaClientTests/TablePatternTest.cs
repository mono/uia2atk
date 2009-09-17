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
//  Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
//using System.Windows;
using System.Windows.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;
using MonoTests.System.Windows.Automation;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class TableTest : BaseTest
	{
		#region Test Methods
		[Test]
		public void GridPatternTest ()
		{
			GridPattern pattern = (GridPattern) table1Element.GetCurrentPattern (GridPatternIdentifiers.Pattern);
			Assert.AreEqual (2, pattern.Current.RowCount, "RowCount");
			Assert.AreEqual (2, pattern.Current.ColumnCount, "ColumnCount");
		}

		[Test]
		public void GridItemPatternTest ()
		{
			GridPattern gridPattern = (GridPattern) table1Element.GetCurrentPattern (GridPatternIdentifiers.Pattern);
			AutomationElement item = gridPattern.GetItem (1, 0);
			Assert.AreEqual ("item 3", item.Current.Name, "GetItem (1, 0).Name");
			GridItemPattern gridItemPattern = (GridItemPattern) item.GetCurrentPattern (GridItemPatternIdentifiers.Pattern);
			Assert.AreEqual (1, gridItemPattern.Current.Row, "row");
			Assert.AreEqual (0, gridItemPattern.Current.Column, "row");

			Assert.AreEqual (table1Element, gridItemPattern.Current.ContainingGrid, "ContainingGrid");
		}

		[Test]
		public void TableGridBase ()
		{
			TablePattern pattern = (TablePattern) table1Element.GetCurrentPattern (TablePatternIdentifiers.Pattern);
			Assert.AreEqual (2, pattern.Current.RowCount, "RowCount");
			Assert.AreEqual (2, pattern.Current.ColumnCount, "ColumnCount");
		}

		[Test]
		public void TableItemGridItemBase ()
		{
			TablePattern tablePattern = (TablePattern) table1Element.GetCurrentPattern (TablePatternIdentifiers.Pattern);
			AutomationElement item = tablePattern.GetItem (1, 0);
			Assert.AreEqual ("item 3", item.Current.Name, "GetItem (1, 0).Name");
			TableItemPattern tableItemPattern = (TableItemPattern) item.GetCurrentPattern (TableItemPatternIdentifiers.Pattern);
			Assert.AreEqual (1, tableItemPattern.Current.Row, "row");
			Assert.AreEqual (0, tableItemPattern.Current.Column, "row");

			Assert.AreEqual (table1Element, tableItemPattern.Current.ContainingGrid, "ContainingGrid");
		}

		[Test]
		public void TableRowHeaderTest ()
		{
			TablePattern tablePattern = (TablePattern) table1Element.GetCurrentPattern (TablePatternIdentifiers.Pattern);
			AutomationElement [] rowHeaders = tablePattern.Current.GetRowHeaders ();
			Assert.AreEqual (0, rowHeaders.Length, "RowHeaders Length");
		}

		[Test]
		public void TableColumnHeaderTest ()
		{
			TablePattern tablePattern = (TablePattern) table1Element.GetCurrentPattern (TablePatternIdentifiers.Pattern);
			AutomationElement [] columnHeaders = tablePattern.Current.GetColumnHeaders ();
			Assert.AreEqual (2, columnHeaders.Length, "ColumnHeaders Length");
			Assert.AreEqual ("column 1", columnHeaders [0].Current.Name, "GetColumnHeaders () [0].Name");
			Assert.AreEqual ("column 2", columnHeaders [1].Current.Name, "GetColumnHeaders () [1].Name");
		}
		#endregion

		public override bool Atspi {
			get {
				return true;
			}
		}
	}
}
