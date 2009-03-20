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
//      Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Collections.Generic;
using System.Windows.Automation.Peers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Automation.Peers.Tests
{
	[TestClass]
	public class PatternInterfaceTest
	{
		[TestMethod]
		[Description("Tests PatternInterface values")]
		public void EnumerationValuesTest()
		{
			Assert.AreEqual ((int)PatternInterface.Dock, 12, "PatternInterface.Dock");
			Assert.AreEqual ((int)PatternInterface.ExpandCollapse, 6, "PatternInterface.ExpandCollapse");
			Assert.AreEqual ((int)PatternInterface.Grid, 7, "PatternInterface.Grid");
			Assert.AreEqual ((int)PatternInterface.GridItem, 8, "PatternInterface.GridItem");
			Assert.AreEqual ((int)PatternInterface.Invoke, 0, "PatternInterface.Invoke");
			Assert.AreEqual ((int)PatternInterface.MultipleView, 9, "PatternInterface.MultipleView");
			Assert.AreEqual ((int)PatternInterface.RangeValue, 3, "PatternInterface.RangeValue");
			Assert.AreEqual ((int)PatternInterface.Scroll, 4, "PatternInterface.Scroll");
			Assert.AreEqual ((int)PatternInterface.ScrollItem, 5, "PatternInterface.ScrollItem");
			Assert.AreEqual ((int)PatternInterface.Selection, 1, "PatternInterface.Selection");
			Assert.AreEqual ((int)PatternInterface.SelectionItem, 11, "PatternInterface.SelectionItem");
			Assert.AreEqual ((int)PatternInterface.Table, 13, "PatternInterface.Table");
			Assert.AreEqual ((int)PatternInterface.TableItem, 14, "PatternInterface.TableItem");
			Assert.AreEqual ((int)PatternInterface.Toggle, 15, "PatternInterface.Toggle");
			Assert.AreEqual ((int)PatternInterface.Transform, 16, "PatternInterface.Transform");
			Assert.AreEqual ((int)PatternInterface.Value, 2, "PatternInterface.Value");
			Assert.AreEqual ((int)PatternInterface.Window, 10, "PatternInterface.Window");
		}
	}
}
