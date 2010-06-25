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
// Copyright (c) 2010 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//  Matt Guo <matt@mattguo.com>
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
	public class CustomControlTest : BaseTest
	{
		private AutomationElement myControlElement;

		protected override void CustomFixtureSetUp ()
		{
			base.CustomFixtureSetUp ();
			if (!Atspi) {
				myControlElement = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.NameProperty, "My Control"));
				Assert.IsNotNull (myControlElement, "myControlElement is not null");
			}
		}

		[Test]
		public void PropertyTest ()
		{
			if (!Atspi) {
				Assert.AreEqual ("My Control", myControlElement.Current.Name);
				Assert.AreEqual (ControlType.Pane.ProgrammaticName, myControlElement.Current.ControlType.ProgrammaticName);
				Assert.AreEqual (new Size (30.0, 15.0), myControlElement.Current.BoundingRectangle.Size);
			}
		}
	}
}

