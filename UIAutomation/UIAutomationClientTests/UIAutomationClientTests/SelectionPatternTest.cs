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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
//using System.Windows;
using System.Windows.Automation;
using At = System.Windows.Automation.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;
using MonoTests.System.Windows.Automation;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class SelectionPatternTest : BaseTest
	{
		AutomationElement child1Element;
		AutomationElement child2Element;

		public override void FixtureSetUp ()
		{
			base.FixtureSetUp ();
			child1Element = treeView1Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.TreeItem));
			Assert.IsNotNull (child1Element, "Child element should not be null");
			child2Element = TreeWalker.RawViewWalker.GetNextSibling (child1Element);
			Assert.IsNotNull (child2Element, "Child element should not be null");
		}

		#region Test Methods
		[Test]
		public void SelectionTest ()
		{
			SelectionPattern pattern = (SelectionPattern) treeView1Element.GetCurrentPattern (SelectionPatternIdentifiers.Pattern);
			Assert.IsNotNull (pattern, "selectionPattern should not be null");
			SelectionPattern.SelectionPatternInformation current = pattern.Current;
			AutomationElement [] selection = current.GetSelection ();
			Assert.AreEqual (0, selection.Length, "Selection length");

			SelectionItemPattern selectionItemPattern = (SelectionItemPattern) child1Element.GetCurrentPattern (SelectionItemPatternIdentifiers.Pattern);
			selectionItemPattern.Select ();
			if (Atspi)
				Thread.Sleep (500);
			selection = current.GetSelection ();
			Assert.AreEqual (1, selection.Length, "Selection length");
			Assert.AreEqual (child1Element, selection [0], "Selection should contain child1Element");

			if (Atspi) {
				Assert.IsFalse (current.IsSelectionRequired,
					"IsSelectionRequired");
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

			selectionItemPattern = (SelectionItemPattern) child2Element.GetCurrentPattern (SelectionItemPatternIdentifiers.Pattern);
			selectionItemPattern.Select ();
			selection = current.GetSelection ();
			Assert.AreEqual (1, selection.Length, "Selection length");
			Assert.AreEqual (child2Element, selection [0], "Selection should contain childElement");
		}

		[Test]
		public void Z_PropertyEventTest ()
		{
			var automationEventsArray = new [] {
				new {Sender = (object) null, Args = (AutomationPropertyChangedEventArgs) null}};
			var automationEvents = automationEventsArray.ToList ();
			automationEvents.Clear ();

			AutomationPropertyChangedEventHandler handler =
				(o, e) => automationEvents.Add (new { Sender = o, Args = e });

			SelectionItemPattern item1 = (SelectionItemPattern) child1Element.GetCurrentPattern (SelectionItemPatternIdentifiers.Pattern);
			item1.Select ();
			At.AddAutomationPropertyChangedEventHandler (treeView1Element,
				TreeScope.Subtree, handler,
				SelectionItemPattern.IsSelectedProperty,
				SelectionPattern.SelectionProperty);

			SelectionItemPattern item2 = (SelectionItemPattern) child2Element.GetCurrentPattern (SelectionItemPatternIdentifiers.Pattern);
			item2.Select ();
			Thread.Sleep (500);
			At.RemoveAutomationPropertyChangedEventHandler (treeView1Element, handler);
			if (Atspi) {
				Assert.AreEqual (2, automationEvents.Count, "event count");
				Assert.AreEqual (child1Element, automationEvents [0].Sender, "event sender");
				Assert.AreEqual (false, automationEvents [0].Args.NewValue, "new Value");
				Assert.AreEqual (true, automationEvents [0].Args.OldValue, "old Value");
				Assert.AreEqual (child2Element, automationEvents [1].Sender, "event sender");
				Assert.AreEqual (true, automationEvents [1].Args.NewValue, "new Value");
				Assert.AreEqual (false, automationEvents [1].Args.OldValue, "old Value");
			} else {
				// TODO: This all seems wrong; test again with Windows 7
				Assert.AreEqual (1, automationEvents.Count, "event count");
				Assert.AreEqual (child2Element, automationEvents [0].Sender, "event sender");
				Assert.AreEqual (true, automationEvents [0].Args.NewValue, "new Value");
				Assert.IsNull (automationEvents [0].Args.OldValue, "old Value");
			}
			automationEvents.Clear ();

			item1.Select ();
			Thread.Sleep (500);
			Assert.AreEqual (0, automationEvents.Count, "event count");
		}

		[Test]
		public void Z_AutomationEventTest ()
		{
			var automationEventsArray = new [] {
				new {Sender = (object) null, Args = (AutomationEventArgs) null}};
			var automationEvents = automationEventsArray.ToList ();
			automationEvents.Clear ();

			AutomationEventHandler handler =
				(o, e) => automationEvents.Add (new { Sender = o, Args = e });

			SelectionItemPattern item1 = (SelectionItemPattern) child1Element.GetCurrentPattern (SelectionItemPatternIdentifiers.Pattern);
			item1.Select ();

			AutomationEvent eventId = SelectionItemPattern.ElementSelectedEvent;
			At.AddAutomationEventHandler (eventId,
				treeView1Element, TreeScope.Descendants, handler);

			SelectionItemPattern item2 = (SelectionItemPattern) child2Element.GetCurrentPattern (SelectionItemPatternIdentifiers.Pattern);
			item2.Select ();
			Thread.Sleep (500);
			At.RemoveAutomationEventHandler (eventId, treeView1Element, handler);
			Assert.AreEqual (1, automationEvents.Count, "event count");
			Assert.AreEqual (child2Element, automationEvents [0].Sender, "event sender");
			Assert.AreEqual (SelectionItemPattern.ElementSelectedEvent, automationEvents [0].Args.EventId, "EventId");
			automationEvents.Clear ();

			item1.Select ();
			Thread.Sleep (500);
			Assert.AreEqual (0, automationEvents.Count, "event count");
		}
		#endregion
	}
}
