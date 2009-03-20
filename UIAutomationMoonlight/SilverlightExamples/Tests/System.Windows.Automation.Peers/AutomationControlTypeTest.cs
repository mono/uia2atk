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
	public class AutomationControlTypeTest
	{
		[TestMethod]
		[Description ("Tests AutomationControlType values")]
		public void EnumerationValuesTest ()
		{
			Assert.AreEqual ((int) AutomationControlType.Button, 0, "AutomationControlType.Button");
			Assert.AreEqual ((int) AutomationControlType.Calendar, 1, "AutomationControlType.Calendar");
			Assert.AreEqual ((int) AutomationControlType.CheckBox, 2, "AutomationControlType.CheckBox");
			Assert.AreEqual ((int) AutomationControlType.ComboBox, 3, "AutomationControlType.ComboBox");
			Assert.AreEqual ((int) AutomationControlType.Custom, 25, "AutomationControlType.Custom");
			Assert.AreEqual ((int) AutomationControlType.DataGrid, 28, "AutomationControlType.DataGrid");
			Assert.AreEqual ((int) AutomationControlType.DataItem, 29, "AutomationControlType.DataItem");
			Assert.AreEqual ((int) AutomationControlType.Document, 30, "AutomationControlType.Document");
			Assert.AreEqual ((int) AutomationControlType.Edit, 4, "AutomationControlType.Edit");
			Assert.AreEqual ((int) AutomationControlType.Group, 26, "AutomationControlType.Group");
			Assert.AreEqual ((int) AutomationControlType.Header, 34, "AutomationControlType.Header");
			Assert.AreEqual ((int) AutomationControlType.HeaderItem, 35, "AutomationControlType.HeaderItem");
			Assert.AreEqual ((int) AutomationControlType.Hyperlink, 5, "AutomationControlType.Hyperlink");
			Assert.AreEqual ((int) AutomationControlType.Image, 6, "AutomationControlType.Image");
			Assert.AreEqual ((int) AutomationControlType.List, 8, "AutomationControlType.List");
			Assert.AreEqual ((int) AutomationControlType.ListItem, 7, "AutomationControlType.ListItem");
			Assert.AreEqual ((int) AutomationControlType.Menu, 9, "AutomationControlType.Menu");
			Assert.AreEqual ((int) AutomationControlType.MenuBar, 10, "AutomationControlType.MenuBar");
			Assert.AreEqual ((int) AutomationControlType.MenuItem, 11, "AutomationControlType.MenuItem");
			Assert.AreEqual ((int) AutomationControlType.Pane, 33, "AutomationControlType.Pane");
			Assert.AreEqual ((int) AutomationControlType.ProgressBar, 12, "AutomationControlType.ProgressBar");
			Assert.AreEqual ((int) AutomationControlType.RadioButton, 13, "AutomationControlType.RadioButton");
			Assert.AreEqual ((int) AutomationControlType.ScrollBar, 14, "AutomationControlType.ScrollBar");
			Assert.AreEqual ((int) AutomationControlType.Separator, 38, "AutomationControlType.Separator");
			Assert.AreEqual ((int) AutomationControlType.Slider, 15, "AutomationControlType.Slider");
			Assert.AreEqual ((int) AutomationControlType.Spinner, 16, "AutomationControlType.Spinner");
			Assert.AreEqual ((int) AutomationControlType.SplitButton, 31, "AutomationControlType.SplitButton");
			Assert.AreEqual ((int) AutomationControlType.StatusBar, 17, "AutomationControlType.StatusBar");
			Assert.AreEqual ((int) AutomationControlType.Tab, 18, "AutomationControlType.Tab");
			Assert.AreEqual ((int) AutomationControlType.TabItem, 19, "AutomationControlType.TabItem");
			Assert.AreEqual ((int) AutomationControlType.Table, 36, "AutomationControlType.Table");
			Assert.AreEqual ((int) AutomationControlType.Text, 20, "AutomationControlType.Text");
			Assert.AreEqual ((int) AutomationControlType.Thumb, 27, "AutomationControlType.Thumb");
			Assert.AreEqual ((int) AutomationControlType.TitleBar, 37, "AutomationControlType.TitleBar");
			Assert.AreEqual ((int) AutomationControlType.ToolBar, 21, "AutomationControlType.ToolBar");
			Assert.AreEqual ((int) AutomationControlType.ToolTip, 22, "AutomationControlType.ToolTip");
			Assert.AreEqual ((int) AutomationControlType.Tree, 23, "AutomationControlType.Tree");
			Assert.AreEqual ((int) AutomationControlType.TreeItem, 24, "AutomationControlType.TreeItem");
			Assert.AreEqual ((int) AutomationControlType.Window, 32, "AutomationControlType.Window");
		}
	}
}
