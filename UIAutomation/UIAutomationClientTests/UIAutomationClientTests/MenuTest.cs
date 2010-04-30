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
	public class MenuTest : BaseTest
	{
		#region Test Methods
		[Test]
		public void HierarchyTest ()
		{
			Assert.AreEqual ("ControlType.MenuBar",
				horizontalMenuStripElement.Current.ControlType.ProgrammaticName,
				"MenuBar control type");

			AutomationElement file = TreeWalker.RawViewWalker.GetFirstChild (horizontalMenuStripElement);
			Assert.IsNotNull (file, "File");
			Assert.AreEqual ("File", file.Current.Name, "File name");
			Assert.AreEqual ("ControlType.MenuItem",
				file.Current.ControlType.ProgrammaticName,
				"File control type");

			AutomationElement newElement = TreeWalker.RawViewWalker.GetFirstChild (file);
			Assert.IsNotNull (newElement, "New");
			Assert.AreEqual ("New",
				newElement.Current.Name,
				"New name");
			Assert.AreEqual ("ControlType.MenuItem",
				newElement.Current.ControlType.ProgrammaticName,
				"New control type");

			AutomationElement edit = TreeWalker.RawViewWalker.GetNextSibling (file);
			Assert.IsNotNull (edit, "Edit");
			Assert.AreEqual ("Edit", edit.Current.Name, "Edit name");
			Assert.AreEqual ("ControlType.MenuItem",
				edit.Current.ControlType.ProgrammaticName,
				"Edit control type");

			AutomationElement undo = TreeWalker.RawViewWalker.GetFirstChild (edit);
			AutomationElement separator = TreeWalker.RawViewWalker.GetNextSibling (undo);
			Assert.IsNotNull (separator, "Separator");
			Assert.AreEqual ("ControlType.Separator",
				separator.Current.ControlType.ProgrammaticName,
				"Separator control type");
			VerifyPatterns (separator);
		}
#endregion
	}
}
