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
using System.Windows;
using System.Windows.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;
using MonoTests.System.Windows.Automation;

namespace AtspiUiaClientTests
{
	[TestFixture]
	public class AtspiDialogTest : BaseTest
	{
		#region Test Methods
		[Test]
		public void Bug590768 ()
		{
			RunCommand ("open FileChooser");
			AutomationElement chooser;
			chooser = AutomationElement.RootElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.NameProperty,
					"FileChooser"));
			Assert.IsNull (chooser,
				"Chooser should not be child of RootElement");
			chooser = testFormElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.NameProperty,
					"FileChooser"));
			Assert.IsNotNull (chooser,
				"FileChooser should be child of form");
			Assert.AreEqual (chooser,
				TreeWalker.RawViewWalker.GetLastChild (testFormElement),
				"Chooser should be last TestForm child");
			AutomationElement prev = TreeWalker.RawViewWalker.GetPreviousSibling (chooser);
			Assert.IsNotNull (prev,
				"Chooser PreviousSibling");
			Assert.AreEqual (chooser,
				TreeWalker.RawViewWalker.GetNextSibling (prev),
				"chooser GetPrev -> getNext");
			Assert.IsNull (TreeWalker.RawViewWalker.GetNextSibling (chooser),
				"Chooser GetNext");
			Assert.AreEqual (testFormElement,
				TreeWalker.RawViewWalker.GetParent (chooser),
				"chooser parent");
			RunCommand ("close FileChooser");
			chooser = testFormElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.NameProperty,
					"FileChooser"));
			Assert.IsNull (chooser,
				"FileChooser should go away when closed");
		}
#endregion

		public override bool Atspi {
			get {
				return true;
			}
		}
	}
}
