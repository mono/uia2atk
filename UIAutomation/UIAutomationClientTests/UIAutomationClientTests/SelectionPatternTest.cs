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
	public class SelectionPatternTest : BaseTest
	{
		#region Test Methods
		[Test]
		public void SelectionTest ()
		{
			SelectionPattern pattern = (SelectionPattern) treeView1Element.GetCurrentPattern (SelectionPatternIdentifiers.Pattern);
			Assert.IsNotNull (pattern, "selectionPattern should not be null");
			SelectionPattern.SelectionPatternInformation current = pattern.Current;
			AutomationElement [] selection = current.GetSelection ();
			Assert.AreEqual (0, selection.Length, "Selection length");

			AutomationElement childElement = treeView1Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.TreeItem));
			Assert.IsNotNull (childElement, "Child element should not be null");
			SelectionItemPattern selectionItemPattern = (SelectionItemPattern) childElement.GetCurrentPattern (SelectionItemPatternIdentifiers.Pattern);
			selectionItemPattern.Select ();
			selection = current.GetSelection ();
			Assert.AreEqual (1, selection.Length, "Selection length");
			Assert.AreEqual (childElement, selection [0], "Selection should contain childElement");

			if (Atspi) {
				Assert.IsTrue (current.IsSelectionRequired,
					"IsSelectionRequired");
				selectionItemPattern.RemoveFromSelection ();
				selection = current.GetSelection ();
				Assert.AreEqual (0, selection.Length, "Selection length");
			} else {
				Assert.IsTrue (current.IsSelectionRequired,
					"IsSelectionRequired");
				try {
					selectionItemPattern.RemoveFromSelection ();
					Assert.Fail ("expected InvalidOperationException");
				} catch (InvalidOperationException) {
					// expected
				}
			}
		}
		#endregion
	}
}
