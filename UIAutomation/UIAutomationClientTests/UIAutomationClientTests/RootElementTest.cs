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
	public class RootElementTest : BaseTest
	{
		Condition findTestForm = null;

		public RootElementTest ()
		{
			findTestForm = new AndCondition (new PropertyCondition (
				AEIds.NameProperty, "TestForm1"),
				new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Window));
		}

		public override void FixtureSetUp ()
		{
			var firstChild =AutomationElement.RootElement.FindFirst (
				TreeScope.Children, findTestForm);
			Assert.IsNull (firstChild);
			base.FixtureSetUp ();
		}

		[Test]
		public void NewlyAddedChildTest ()
		{
			var firstChild =AutomationElement.RootElement.FindFirst (
				TreeScope.Children, findTestForm);
			Assert.IsNotNull (firstChild);
		}

		// Note: Don't play other applications while the following test cases are running,
		// Since this test could be affected by any change of any child elements
		// of AutomationElement.RootElement
		[Test]
		public void PropertyEventTest ()
		{
			int eventCount = 0;
			AutomationProperty changedProperty = null;
			object newValue = null;
			object sender = null;
			AutomationPropertyChangedEventHandler handler = (o, e) =>
			{
				eventCount++;
				changedProperty = e.Property;
				newValue = e.NewValue;
				sender = o;
			};
			At.AddAutomationPropertyChangedEventHandler(
				AutomationElement.RootElement, TreeScope.Children,
				handler, AutomationElement.NameProperty);
			RunCommand ("change title:title 1");
			Assert.AreEqual (1, eventCount, "count of AutomationPropertyChangedEvent");
			Assert.AreEqual (AutomationElement.NameProperty, changedProperty);
			Assert.AreEqual ("title 1", newValue);
			Assert.AreEqual (testFormElement, sender);
			At.RemoveAutomationPropertyChangedEventHandler (
				AutomationElement.RootElement, handler);
			RunCommand ("change title:title 2");
			Assert.AreEqual (1, eventCount);
		}

		[Test]
		public void StructureEventTest ()
		//this test also tested WindowPattern.WindowOpenedEvent (i.e. AddAutomationEventHandler)
		{
			int automationEventCount = 0;
			int structureEventCount = 0;
			AutomationEvent eventId = null;
			AutomationEventHandler automationEventHandler = (o, e) =>
			{
				automationEventCount++;
				eventId = e.EventId;
			};
			At.AddAutomationEventHandler(
				WindowPattern.WindowOpenedEvent,
				AutomationElement.RootElement, TreeScope.Children,
				automationEventHandler);
			StructureChangedEventHandler structureEventHandler = (o, e) =>
			{
				if (e.StructureChangeType == StructureChangeType.ChildAdded)
					structureEventCount++;
			};
			At.AddStructureChangedEventHandler (
				AutomationElement.RootElement, TreeScope.Children, structureEventHandler);
			int pid = OpenForm ();
			Thread.Sleep (3000);
			Assert.AreEqual (1, structureEventCount, "[OpenForm] count of StructureChangedEvent");
			Assert.AreEqual (1, automationEventCount, "[OpenForm] count of WindowOpenedEvent");
			Assert.AreEqual (WindowPattern.WindowOpenedEvent, eventId);

			automationEventCount = 0;
			structureEventCount = 0;
			At.RemoveAllEventHandlers ();
			int pid2 = OpenForm ();
			Thread.Sleep (3000);
			Assert.AreEqual (0, structureEventCount);
			Assert.AreEqual (0, automationEventCount);

			structureEventHandler = (o, e) =>
			{
				structureEventCount++;
			};
			At.AddStructureChangedEventHandler (
				AutomationElement.RootElement, TreeScope.Children, structureEventHandler);
			CloseForm (pid);
			CloseForm (pid2);
			Thread.Sleep (3000);
			// Note: I expect 2 events here (whose StructureChangeType are both ChildRemoved)
			// But as tested on Win 7, we'll actually get no event,
			// And with our current implementation, we'll get 4 events (i.e. besides the 2 expected events, we
			// get other 2 ChildRemoved events, whose sender is the "testFormElement")
			Assert.AreEqual (0, structureEventCount, "[CloseForm] count of StructureChangedEvent");
		}

		private int OpenForm ()
		{
			Process p;
			if (Atspi)
				p = StartApplication ("GtkForm.exe", String.Empty);
			else
				p = StartApplication ("SampleForm.exe", String.Empty);
			return p.Id;
		}

		private void CloseForm (int pid)
		{
			var findFormByPid = new AndCondition (
				new PropertyCondition (
					AEIds.NameProperty, "TestForm1"),
				new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Window),
				new PropertyCondition (AEIds.ProcessIdProperty, pid));
			var firstChild = AutomationElement.RootElement.FindFirst (
				TreeScope.Children, findFormByPid);
			if (firstChild == null)
				return;
			var windowPattern = (WindowPattern) firstChild.GetCurrentPattern (
				WindowPattern.Pattern);
			if (windowPattern == null)
				return;
			windowPattern.Close ();
		}
	}
}
