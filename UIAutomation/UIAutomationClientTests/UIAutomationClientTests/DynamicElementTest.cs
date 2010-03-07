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
using System.Windows.Automation;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;
using SW = System.Windows;
using At = System.Windows.Automation.Automation;
using NUnit.Framework;
using MonoTests.System.Windows.Automation;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class DynamicElementTest : BaseTest
	{
		[Test]
		public void AddRemoveElementTest ()
		{
			InvokePattern addAction = (InvokePattern) btnAddTextboxElement.GetCurrentPattern (InvokePattern.Pattern);
			InvokePattern removeAction = (InvokePattern) btnRemoveTextboxElement.GetCurrentPattern (InvokePattern.Pattern);
			addAction.Invoke ();
			Thread.Sleep (1000);
			var newEditElement = panel1Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Edit));
			var vp = (ValuePattern) newEditElement.GetCurrentPattern (ValuePattern.Pattern);
			Action getControlTypeAction = () => Console.WriteLine (newEditElement.Current.BoundingRectangle);
			Action appendDotAction = () => vp.SetValue (vp.Current.Value + ".");
			AssertWontRaise<Exception> (getControlTypeAction, "get control type");
			AssertWontRaise<Exception> (getControlTypeAction, "append '.'");
			removeAction.Invoke ();
			Thread.Sleep (1000);
			AssertRaises<ElementNotAvailableException> (getControlTypeAction, "get control type");
			AssertRaises<ElementNotAvailableException> (getControlTypeAction, "append '.'");
		}

		[Test]
		public void Z_CloseAppTest ()
		{
			if (p != null) {
				p.Kill ();
				p = null;
			}
			AssertRaises<ElementNotAvailableException> (
				() => Console.WriteLine (label1Element.Current.Name),
				"get testFormElement.Name");
			AssertRaises<ElementNotAvailableException> (
				() => Console.WriteLine (testFormElement.Current.Name),
				"get testFormElement.Name");
		}

		[Test]
		public void DynamicPatternTest ()
		{
			var tp = (TransformPattern) testFormElement.GetCurrentPattern (TransformPattern.Pattern);
			Action moveAction = () => tp.Move (25, 25);
			AssertWontRaise<Exception> (moveAction, "move form");

			//Disable the TransformPattern
			RunCommand ("toggle form border");
			AssertRaises<InvalidOperationException> (
				() => testFormElement.GetCurrentPattern (TransformPattern.Pattern),
				"get TransformPattern on a borderless form");
			AssertRaises<InvalidOperationException> (moveAction, "move form");

			//Enable the TransformPattern again
			RunCommand ("toggle form border");
		}
	}
}
