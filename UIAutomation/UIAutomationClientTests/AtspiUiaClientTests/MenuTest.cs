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

namespace AtspiUiaClientTests
{
	[TestFixture]
	public class AtspiMenuTest : MenuTest
	{
		[Test]
		public void CheckMenuItemTest ()
		{
			AutomationElement file = TreeWalker.RawViewWalker.GetFirstChild (horizontalMenuStripElement);
			Assert.IsNotNull (file, "File");
			Assert.AreEqual ("File", file.Current.Name, "File name");

			AutomationElement check = file.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.NameProperty,
					"Check"));
			Assert.IsNotNull (check, "Check MenuItem found");
			Assert.AreEqual ("ControlType.MenuItem",
				check.Current.ControlType.ProgrammaticName,
				"ControlType");
			VerifyPatterns (check,
				InvokePatternIdentifiers.Pattern,
				SelectionItemPatternIdentifiers.Pattern,
				TogglePatternIdentifiers.Pattern);
		}

		public override bool Atspi {
			get {
				return true;
			}
		}
}
}
