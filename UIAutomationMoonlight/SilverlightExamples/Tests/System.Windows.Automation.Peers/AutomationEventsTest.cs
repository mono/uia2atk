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
	public class AutomationEventsTest
	{
		[TestMethod]
		[Description("Tests AutomationEvents values")]
		public void EnumerationValuesTest()
		{
			Assert.AreEqual ((int) AutomationEvents.AsyncContentLoaded, 12, "AutomationEvents.AsyncContentLoaded");
			Assert.AreEqual ((int) AutomationEvents.AutomationFocusChanged, 4, "AutomationEvents.AutomationFocusChanged");
			Assert.AreEqual ((int) AutomationEvents.InvokePatternOnInvoked, 5, "AutomationEvents.InvokePatternOnInvoked");
			Assert.AreEqual ((int) AutomationEvents.MenuClosed, 3, "AutomationEvents.MenuClosed");
			Assert.AreEqual ((int) AutomationEvents.MenuOpened, 2, "AutomationEvents.MenuOpened");
			Assert.AreEqual ((int) AutomationEvents.PropertyChanged, 13, "AutomationEvents.PropertyChanged");
			Assert.AreEqual ((int) AutomationEvents.SelectionItemPatternOnElementAddedToSelection, 6, "AutomationEvents.SelectionItemPatternOnElementAddedToSelection");
			Assert.AreEqual ((int) AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection, 7, "AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection");
			Assert.AreEqual ((int) AutomationEvents.SelectionItemPatternOnElementSelected, 8, "AutomationEvents.SelectionItemPatternOnElementSelected");
			Assert.AreEqual ((int) AutomationEvents.SelectionPatternOnInvalidated, 9, "AutomationEvents.SelectionPatternOnInvalidated");
			Assert.AreEqual ((int) AutomationEvents.StructureChanged, 14, "AutomationEvents.StructureChanged");
			Assert.AreEqual ((int) AutomationEvents.TextPatternOnTextChanged, 11, "AutomationEvents.TextPatternOnTextChanged");
			Assert.AreEqual ((int) AutomationEvents.TextPatternOnTextSelectionChanged, 10, "AutomationEvents.TextPatternOnTextSelectionChanged");
			Assert.AreEqual ((int) AutomationEvents.ToolTipClosed, 1, "AutomationEvents.ToolTipClosed");
			Assert.AreEqual ((int) AutomationEvents.ToolTipOpened, 0, "AutomationEvents.ToolTipOpened");
		}
	}
}
