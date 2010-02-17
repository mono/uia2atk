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
//  Matt Guo <matt@mattguo.com>
// 

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;
using NUnit.Framework;
using MonoTests.System.Windows.Automation;
using At = System.Windows.Automation.Automation;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class FocusTest : BaseTest
	{
		List<AutomationElement> actualFocusedElements = new List<AutomationElement> ();

		[Test]
		public void BasicFocusTest ()
		{
			// TODO:
			//Currently FocusTest.BasicFocusTest can pass on Windows, but can't on Linux. The failure reason is that:
			//on Windows, call InvokePattern.Invoke will make corresponding button focused, so we also assert this behavior in the test,
			//however our implementation will not make the button focused after InvokePattern.Invoke.

			AutomationElement [] expectedFocusedElements;
			if (Atspi)
				expectedFocusedElements = new AutomationElement [] {
					btnRunElement, textbox3Element,
					btnRunElement, button2Element,
				};
			else
				expectedFocusedElements = new AutomationElement [] {
					txtCommandElement, textbox3Element,
					txtCommandElement, button2Element,
				};

			AutomationFocusChangedEventHandler handler = (s,e) => actualFocusedElements.Add ((AutomationElement) s);
			At.AddAutomationFocusChangedEventHandler (handler);
			RunCommand ("focus textBox3");
			Assert.AreEqual (textbox3Element, AutomationElement.FocusedElement, "FocusedElement");
			RunCommand ("focus button2");
			Assert.AreEqual (button2Element, AutomationElement.FocusedElement, "FocusedElement");
			At.RemoveAutomationFocusChangedEventHandler (handler);
			RunCommand ("focus textBox3");
			Assert.AreEqual (textbox3Element, AutomationElement.FocusedElement, "FocusedElement");

			At.AddAutomationFocusChangedEventHandler (handler);
			At.RemoveAllEventHandlers ();
			RunCommand ("focus button2");
			Assert.AreEqual (button2Element, AutomationElement.FocusedElement, "FocusedElement");

			Assert.AreEqual (expectedFocusedElements.Length, actualFocusedElements.Count, "Event handler count");
			for (int i = 0; i < actualFocusedElements.Count; i++) {
				Assert.AreEqual (expectedFocusedElements [i], actualFocusedElements [i], "Event handler sender #" + i);
			}
		}

		[Test]
		public void SetFocusTest ()
		{
			AutomationElement [] expectedFocusedElements = new AutomationElement [] {
				textbox3Element, button2Element
			};

			button2Element.SetFocus ();
			AutomationFocusChangedEventHandler handler = (s,e) => actualFocusedElements.Add ((AutomationElement) s);
			At.AddAutomationFocusChangedEventHandler (handler);
			actualFocusedElements.Clear ();
			textbox3Element.SetFocus ();
			Thread.Sleep (100);
			Assert.AreEqual (textbox3Element, AutomationElement.FocusedElement, "FocusedElement");
			button2Element.SetFocus ();
			Thread.Sleep (100);
			Assert.AreEqual (button2Element, AutomationElement.FocusedElement, "FocusedElement");
			Thread.Sleep(1000);
			At.RemoveAutomationFocusChangedEventHandler (handler);
			Assert.AreEqual (expectedFocusedElements.Length, actualFocusedElements.Count, "Event handler count");
			for (int i = 0; i < actualFocusedElements.Count; i++) {
				Assert.AreEqual (expectedFocusedElements [i], actualFocusedElements [i], "Event handler sender #" + i);
			}
		}

		[Test]
		public void AutomationFocusChangedEventArgsTest ()
		{
			var args = new AutomationFocusChangedEventArgs (0, 0);
			Assert.AreEqual (AutomationElementIdentifiers.AutomationFocusChangedEvent,
			                 args.EventId);
		}

		//todo need to write more test on multiple sources, after the focusedElement
		//is supported by other sources such as Atspi
	}
}
