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
	public class AtspiInvokePatternTest : InvokePatternTest
	{
		public override bool Atspi {
			get {
				return true;
			}
		}

		[Test]
		public void SpinnerTest ()
		{
			AutomationElement spinnerElement;
			object pattern;
			spinnerElement = groupBoxElement.FindFirst (TreeScope.Children,
			        new PropertyCondition (AEIds.ControlTypeProperty,
			                ControlType.Spinner));
			Assert.IsFalse (spinnerElement.TryGetCurrentPattern (
				InvokePattern.Pattern, out pattern),
				"Spinner should not support InvokePattern");
		}

		[Test]
		public void EditTest ()
		{
			object pattern;
			Assert.IsFalse (textbox1Element.TryGetCurrentPattern (
				InvokePattern.Pattern, out pattern),
				"TextBox should not support InvokePattern");
		}

		[Test]
		public void ToggleButtonTest ()
		{
			AutomationElement toggleButtonElement;
			object pattern;
			toggleButtonElement = groupBoxElement.FindFirst (TreeScope.Children,
			        new PropertyCondition (AEIds.NameProperty,
			                "ToggleButton"));
			Assert.IsFalse (toggleButtonElement.TryGetCurrentPattern (
				InvokePattern.Pattern, out pattern),
				"ToggleButton should not support InvokePattern");
		}

		[Test]
		public void ScrollBarTest ()
		{
			object pattern;
			AutomationElement scrollBarElement = groupBoxElement.FindFirst (TreeScope.Children,
			        new PropertyCondition (AEIds.ControlTypeProperty,
			                ControlType.ScrollBar));
			Assert.IsFalse (scrollBarElement.TryGetCurrentPattern (
				InvokePattern.Pattern, out pattern),
				"ScrollBar should not support InvokePattern");
		}
	}
}
