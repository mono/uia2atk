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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;
using At = System.Windows.Automation.Automation;
using SW = System.Windows;
using NUnit.Framework;
using MonoTests.System.Windows.Automation;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class AutomationEventTest : BaseTest
	{
		[Test]
		public void ArgumentExceptionTest ()
		{
			Action action = () => {
				At.AddAutomationEventHandler (InvokePattern.InvokedEvent,
					null, TreeScope.Element, (o, e) => {});
			};
			AssertRaises<ArgumentNullException>(action,
				"Pass null as element to AddAutomationEventHandler");

			action = () => {
				At.AddAutomationPropertyChangedEventHandler (
					null, TreeScope.Element, (o, e) => {}, AEIds.NameProperty);
			};
			AssertRaises<ArgumentNullException>(action,
				"Pass null as element to AddAutomationPropertyChangedEventHandler");

			action = () => {
				At.AddStructureChangedEventHandler (
					null, TreeScope.Element, (o, e) => {});
			};
			AssertRaises<ArgumentNullException>(action,
				"Pass null as element to AddStructureChangedEventHandler");

			action = () => {
				At.AddAutomationEventHandler (InvokePattern.InvokedEvent,
					button1Element, TreeScope.Element, null);
			};
			AssertRaises<ArgumentNullException>(action,
				"Pass null as handler to AddAutomationEventHandler");

			action = () => {
				At.AddAutomationPropertyChangedEventHandler (
					button1Element, TreeScope.Element, null, AEIds.NameProperty);
			};
			AssertRaises<ArgumentNullException>(action,
				"Pass null as handler to AddAutomationPropertyChangedEventHandler");

			action = () => {
				At.AddStructureChangedEventHandler (
					button1Element, TreeScope.Element, null);
			};
			AssertRaises<ArgumentNullException>(action,
				"Pass null as handler to AddStructureChangedEventHandler");

			action = () => {
				At.AddAutomationFocusChangedEventHandler (null);
			};
			AssertRaises<ArgumentNullException>(action,
				"Pass null as handler to AddAutomationFocusChangedEventHandler");

			action = () => {
				At.RemoveAutomationEventHandler (InvokePattern.InvokedEvent,
					null, (o, e) => {});
			};
			AssertRaises<ArgumentNullException>(action,
				"Pass null as element to RemoveAutomationEventHandler");

			action = () => {
				At.RemoveAutomationPropertyChangedEventHandler (
					null, (o, e) => {});
			};
			AssertRaises<ArgumentNullException>(action,
				"Pass null as element to RemoveAutomationPropertyChangedEventHandler");

			action = () => {
				At.RemoveStructureChangedEventHandler (
					null, (o, e) => {});
			};
			AssertRaises<ArgumentNullException>(action,
				"Pass null as element to RemoveStructureChangedEventHandler");

			action = () => {
				At.RemoveAutomationEventHandler (InvokePattern.InvokedEvent,
					button1Element, null);
			};
			AssertRaises<ArgumentNullException>(action,
				"Pass null as handler to RemoveAutomationEventHandler");

			action = () => {
				At.RemoveAutomationPropertyChangedEventHandler (
					button1Element, null);
			};
			AssertRaises<ArgumentNullException>(action,
				"Pass null as handler to RemoveAutomationPropertyChangedEventHandler");

			action = () => {
				At.RemoveStructureChangedEventHandler (
					button1Element, null);
			};
			AssertRaises<ArgumentNullException>(action,
				"Pass null as handler to RemoveStructureChangedEventHandler");

			action = () => {
				At.RemoveAutomationFocusChangedEventHandler (null);
			};
			AssertRaises<ArgumentNullException>(action,
				"Pass null as handler to RemoveAutomationFocusChangedEventHandler");

			//Assert removing a non-existent handler won't fire any exception
			At.RemoveAutomationEventHandler (InvokePattern.InvokedEvent,
				button1Element, (o, e) => { Console.Write("nop"); } );
			At.RemoveAutomationPropertyChangedEventHandler (
				button1Element, (o, e) => { Console.Write("nop"); } );
			At.RemoveStructureChangedEventHandler (
				button1Element, (o, e) => { Console.Write("nop"); } );
			At.RemoveAutomationFocusChangedEventHandler (
				(o, e) => { Console.Write("nop"); } );

		}

		[Test]
		public void StructureEventTest ()
		{
			List<AutomationElement> elementEventSenders = new List<AutomationElement> ();
			List<StructureChangeType> elementEventChangeTypes = new List<StructureChangeType> ();
			List<AutomationElement> childrenEventSenders = new List<AutomationElement> ();
			List<StructureChangeType> childrenEventChangeTypes = new List<StructureChangeType> ();
			StructureChangedEventHandler elementHandler = delegate (object sender, StructureChangedEventArgs args) {
				elementEventSenders.Add (sender as AutomationElement);
				elementEventChangeTypes.Add (args.StructureChangeType);
			};
			At.AddStructureChangedEventHandler(panel1Element, TreeScope.Element, elementHandler);
			StructureChangedEventHandler childrenHandler = delegate (object sender, StructureChangedEventArgs args) {
				childrenEventSenders.Add (sender as AutomationElement);
				childrenEventChangeTypes.Add (args.StructureChangeType);
			};
			At.AddStructureChangedEventHandler(panel1Element, TreeScope.Children, childrenHandler);
			InvokePattern addAction = (InvokePattern) btnAddTextboxElement.GetCurrentPattern (InvokePattern.Pattern);
			InvokePattern removeAction = (InvokePattern) btnRemoveTextboxElement.GetCurrentPattern (InvokePattern.Pattern);
			addAction.Invoke ();
			Thread.Sleep (1000);
			Assert.AreEqual (1, elementEventSenders.Count, "Check event count");
			Assert.AreEqual (panel1Element, elementEventSenders [0], "Check ChildrenInvalidated event sender");
			Assert.AreEqual (StructureChangeType.ChildrenInvalidated,
			                 elementEventChangeTypes [0], "Check ChildrenInvalidated event type");
			Assert.AreEqual (1, childrenEventSenders.Count, "Check event count");
			Assert.AreEqual (StructureChangeType.ChildAdded,
			                 childrenEventChangeTypes [0], "Check ChildAdded event type");
			removeAction.Invoke ();
			Thread.Sleep (1000);
			Assert.AreEqual (3, elementEventSenders.Count, "Check event count");
			Assert.AreEqual (panel1Element, elementEventSenders [1], "Check event sender");
			Assert.AreEqual (panel1Element, elementEventSenders [2], "Check event sender");
			Assert.IsTrue ((elementEventChangeTypes [1] == StructureChangeType.ChildRemoved
			               && elementEventChangeTypes [2] == StructureChangeType.ChildrenInvalidated) ||
			               (elementEventChangeTypes [1] == StructureChangeType.ChildrenInvalidated
			               && elementEventChangeTypes [2] == StructureChangeType.ChildRemoved),
			               "Check event type");
			addAction.Invoke ();
			Thread.Sleep (1000);
			Assert.AreEqual (4, elementEventSenders.Count, "Check event count");
			Assert.AreEqual (panel1Element, elementEventSenders [3], "Check ChildrenInvalidated event sender");
			Assert.AreEqual (StructureChangeType.ChildrenInvalidated,
			                 elementEventChangeTypes [3], "Check ChildrenInvalidated event type");
			Assert.AreEqual (2, childrenEventSenders.Count, "Check event count");
			Assert.AreEqual (StructureChangeType.ChildAdded,
			                 childrenEventChangeTypes [1], "Check ChildAdded event type");

			At.RemoveStructureChangedEventHandler (panel1Element, elementHandler);
			At.RemoveStructureChangedEventHandler (panel1Element, childrenHandler);
			addAction.Invoke ();
			Thread.Sleep (1000);
			Assert.AreEqual (4, elementEventSenders.Count, "Element event count didn't change");
			Assert.AreEqual (2, childrenEventSenders.Count, "Children event count didn't change");
		}

		/*
		 * From MSDN:
		 "Not all property changes raise events; that is entirely up to the implementation of the UI Automation provider
		  for the element. For example, the standard proxy providers for list boxes do not raise an event when the
		   SelectionProperty changes. In this case, the application instead must listen for an ElementSelectedEvent."
		 * */
		[Test]
		public void SelectionPropertyChangedEventTest ()
		{
			int propertyEventCount = 0;
			int selectionEventCount = 0;
			AutomationPropertyChangedEventHandler propertyHandler =
				(o, e) => { propertyEventCount++; };
			AutomationEventHandler selectionEventHandler =
				(o, e) => { selectionEventCount++; };

			var childElement = treeView1Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.TreeItem));

			At.AddAutomationPropertyChangedEventHandler (treeView1Element, TreeScope.Subtree,
				propertyHandler, SelectionPattern.SelectionProperty);
			At.AddAutomationEventHandler (SelectionItemPattern.ElementSelectedEvent,
				childElement, TreeScope.Element, selectionEventHandler);

			var pattern = (SelectionPattern)
				treeView1Element.GetCurrentPattern (SelectionPatternIdentifiers.Pattern);
			Assert.IsNotNull (pattern, "selectionPattern should not be null");
			var current = pattern.Current;
			AutomationElement [] selection = current.GetSelection ();
			Assert.AreEqual (0, selection.Length, "Selection length");

			var selectionItemPattern = (SelectionItemPattern)
				childElement.GetCurrentPattern (SelectionItemPatternIdentifiers.Pattern);
			selectionItemPattern.Select ();
			Thread.Sleep (1000);

			selection = current.GetSelection ();
			Assert.AreEqual (1, selection.Length, "Selection length");
			Assert.AreEqual (0, propertyEventCount, "# of SelectionProperty changed event");
			Assert.AreEqual (1, selectionEventCount, "# of selection event");
		}

		[Test]
		public void BoundingRectanglePropertyChangedEventTest ()
		{
			int eventCount = 0;
			object newValue = null;
			AutomationPropertyChangedEventHandler propertyHandler = (o, e) => {
				eventCount++;
				newValue = e.NewValue;
			};
			At.AddAutomationPropertyChangedEventHandler (testFormElement, TreeScope.Element,
				propertyHandler, AEIds.BoundingRectangleProperty);
			RunCommand ("change form size 800x600");
			Thread.Sleep (1000);
			Assert.AreEqual (1, eventCount);
			SW.Rect rect = (SW.Rect) newValue;
			Assert.AreEqual (800, rect.Width, "rect.Width");
			Assert.AreEqual (600, rect.Height, "rect.Height");
		}

		[Test]
		public void ControlTypePropertyChangedEventTest ()
		{
			int eventCount = 0;
			AutomationPropertyChangedEventHandler propertyHandler = 
				(o, e) => { eventCount++; };

			// textBox3's ControlType will change from "Document" to "Edit" if we set Multiline = false
			// but on Windows we don't have corresponding property changed event.
			At.AddAutomationPropertyChangedEventHandler (textbox3Element, TreeScope.Element,
				propertyHandler, AEIds.ControlTypeProperty);
			var ct = textbox3Element.Current.ControlType;
			Assert.AreEqual (ControlType.Document, ct, ct.ProgrammaticName);

			RunCommand ("textBox3 singleline");
			Thread.Sleep (500);

			ct = textbox3Element.Current.ControlType;
			Assert.AreEqual (ControlType.Document, ct, ct.ProgrammaticName);
			textbox3Element = testFormElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.NameProperty, "textBox3"));

			ct = textbox3Element.Current.ControlType;
			Assert.AreEqual (ControlType.Edit, ct, ct.ProgrammaticName);
			Assert.AreEqual (0, eventCount);

			// listView1's ControlType will change from "DataGrid" to "List" if we set View = View.Tile
			// but on Windows we don't have corresponding property changed event.
			At.AddAutomationPropertyChangedEventHandler (listView1Element, TreeScope.Element,
				propertyHandler, AEIds.ControlTypeProperty);
			ct = listView1Element.Current.ControlType;
			Assert.AreEqual (ControlType.DataGrid, ct, ct.ProgrammaticName);

			RunCommand ("change list view mode tile");
			Thread.Sleep (500);

			ct = listView1Element.Current.ControlType;
			Assert.AreEqual (ControlType.DataGrid, ct, ct.ProgrammaticName);
			listView1Element = testFormElement.FindFirst (TreeScope.Descendants,
				new PropertyCondition (AEIds.NameProperty, "listView1"));

			ct = listView1Element.Current.ControlType;
			Assert.AreEqual (ControlType.List, ct, ct.ProgrammaticName);
			Assert.AreEqual (0, eventCount);
		}
	}
}

