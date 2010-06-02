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
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class TableHierarchyTest : BaseTest
	{
		#region Test Methods
		[Test]
		public void TableHierarchy ()
		{
			AutomationElement headerElement;
			headerElement = table1Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.Header));
			Assert.IsNotNull (headerElement, "Header");
			AutomationElement headerItemElement;
			headerItemElement = headerElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.HeaderItem));
			Assert.IsNotNull (headerItemElement, "headerItem");
			Assert.AreEqual ("Gender",
				headerItemElement.Current.Name,
				"Header 1 name");
			Assert.AreEqual (headerElement,
				TreeWalker.RawViewWalker.GetParent (headerItemElement),
				"Header should be parent of its HeaderItem");
			Assert.IsNull (TreeWalker.RawViewWalker.GetFirstChild (headerItemElement),
				"headerItem should not have children");
			headerItemElement = TreeWalker.RawViewWalker.GetNextSibling (headerItemElement);
			Assert.IsNotNull (headerItemElement, "Header #2");
			Assert.AreEqual ("Name",
				headerItemElement.Current.Name,
				"Header 2 name");
			headerItemElement = TreeWalker.RawViewWalker.GetNextSibling (headerItemElement);
			Assert.IsNotNull (headerItemElement, "Header #3");
			Assert.AreEqual ("Age",
				headerItemElement.Current.Name,
				"Header 3 name");
			Assert.IsNull (TreeWalker.RawViewWalker.GetNextSibling (headerItemElement),
				"Should not have more than three header items");

			AutomationElement dataItemElement = TreeWalker.RawViewWalker.GetNextSibling (headerElement);
			Assert.IsNotNull (dataItemElement, "DataItem");
			Assert.AreEqual ("ControlType.DataItem",
				dataItemElement.Current.ControlType.ProgrammaticName,
				"DataItem ControlType");
			Assert.AreEqual (table1Element,
				TreeWalker.RawViewWalker.GetParent (dataItemElement),
				"DataItem parent should be Table");
			AutomationElement textElement = TreeWalker.RawViewWalker.GetFirstChild (dataItemElement);
			Assert.IsNotNull (textElement, "text");
			string expected = (Atspi? "Edit" : "CheckBox");
			Assert.AreEqual ("ControlType." + expected,
				textElement.Current.ControlType.ProgrammaticName,
				expected + "ControlType");
			Assert.AreEqual (dataItemElement,
				TreeWalker.RawViewWalker.GetParent (textElement),
				"Text parent should be DataItem");
				Assert.IsNull (TreeWalker.RawViewWalker.GetFirstChild (textElement),
				"TextElement FirstChild");
				Assert.IsNull (TreeWalker.RawViewWalker.GetPreviousSibling (textElement),
				"TextElement PreviousSibling");
			Assert.AreEqual ("False",
				textElement.Current.Name,
				"TextElement Name");
			//VerifyPatterns (textElement,
				//ValuePatternIdentifiers.Pattern);
			textElement = TreeWalker.RawViewWalker.GetNextSibling (textElement);
			Assert.IsNotNull (textElement, "item2");
			Assert.AreEqual ("Alice",
				textElement.Current.Name,
				"TextElement Name");
			textElement = TreeWalker.RawViewWalker.GetNextSibling (textElement);
			Assert.IsNotNull (textElement, "item2");
			Assert.AreEqual ("24",
				textElement.Current.Name,
				"TextElement Name");
			Assert.IsNull (TreeWalker.RawViewWalker.GetNextSibling (textElement),
				"next after item2");
		}

		[Test]
		public void TreeViewHierarchyTest ()
		{
			AutomationElement treeItemElement;
			treeItemElement = treeView1Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.TreeItem));
			Assert.IsNotNull (treeItemElement, "Should have a TreeItem");
			AutomationElement treeItem2Element;
			treeItem2Element = TreeWalker.RawViewWalker.GetNextSibling (treeItemElement);
			ExpandCollapsePattern pattern = (ExpandCollapsePattern) treeItem2Element.GetCurrentPattern (ExpandCollapsePatternIdentifiers.Pattern);
			pattern.Expand ();
			pattern = (ExpandCollapsePattern) treeItemElement.GetCurrentPattern (ExpandCollapsePatternIdentifiers.Pattern);
			TreeViewHierarchyTest (false);
			pattern.Expand ();
Thread.Sleep(500);
			TreeViewHierarchyTest (true);
			pattern.Collapse ();
Thread.Sleep(500);
			TreeViewHierarchyTest (false);
		}

		public void TreeViewHierarchyTest (bool item1Expanded)
		{
			AutomationElement treeItemElement;
			treeItemElement = treeView1Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.TreeItem));
			Assert.IsNotNull (treeItemElement, "Should have a TreeItem");
			Assert.AreEqual ("item 1",
				treeItemElement.Current.Name, "item 1");
			Assert.IsNull (TreeWalker.RawViewWalker.GetPreviousSibling (treeItemElement), "Item1 previous");
			AutomationElement treeItemSubElement;
			treeItemSubElement = treeItemElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.TreeItem));
			if (item1Expanded) {
				Assert.IsNotNull (treeItemSubElement, "TreeItemSub");
				Assert.AreEqual ("item 1a",
					treeItemSubElement.Current.Name, "item 1a");
				Assert.IsNull (TreeWalker.RawViewWalker.GetPreviousSibling (treeItemSubElement), "Item1a previous");
				Assert.IsNull (TreeWalker.RawViewWalker.GetNextSibling (treeItemSubElement), "Item1a next");
				Assert.IsNull (TreeWalker.RawViewWalker.GetFirstChild (treeItemSubElement), "Item1a child");
			} else {
				AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild (treeItemElement);
				if (child != null)
					Assert.IsTrue (child.Current.IsOffscreen, "Child should be of-screen when not expanded");
			}
			treeItemElement = TreeWalker.RawViewWalker.GetNextSibling (treeItemElement);
			Assert.AreEqual ("item 2",
				treeItemElement.Current.Name, "item 2");
			treeItemSubElement = treeItemElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.TreeItem));
			Assert.IsNotNull (treeItemSubElement, "SubElement should not be null");
			Assert.AreEqual ("item 2a",
				treeItemSubElement.Current.Name, "item 2a");
			Assert.IsNull (TreeWalker.RawViewWalker.GetPreviousSibling (treeItemSubElement), "Item2a previous");
			treeItemSubElement = TreeWalker.RawViewWalker.GetNextSibling (treeItemSubElement);
			Assert.AreEqual ("item 2b",
				treeItemSubElement.Current.Name, "item 2b");
			Assert.IsNull (TreeWalker.RawViewWalker.GetNextSibling (treeItemSubElement), "Item2b next");
			Assert.IsNull (TreeWalker.RawViewWalker.GetNextSibling (treeItemElement), "Item2 next");
			treeItemElement = TreeWalker.RawViewWalker.GetPreviousSibling (treeItemElement);
			Assert.AreEqual ("item 1",
				treeItemElement.Current.Name, "item 2 previous");
		}

		[Test]
		public void TreeViewPatternsTest ()
		{
			VerifyPatterns (treeView1Element,
				SelectionPatternIdentifiers.Pattern);
		}
		#endregion
	}
}
