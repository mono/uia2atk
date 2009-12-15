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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using SWA = System.Windows.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;
using MonoTests.System.Windows.Automation;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class WindowPatternTest : BaseTest
	{
		#region Test Methods
		[Test]
		public void PropertiesTest ()
		{
			Assert.AreEqual (WindowPatternIdentifiers.Pattern.Id,
				WindowPattern.Pattern.Id,
				"PatternTest.Id");

			Assert.AreEqual (WindowPatternIdentifiers.Pattern.ProgrammaticName,
				WindowPattern.Pattern.ProgrammaticName,
				"Pattern.ProgrammaticName");
			Assert.AreEqual (WindowPatternIdentifiers.CanMaximizeProperty.Id,
				WindowPattern.CanMaximizeProperty.Id,
				"CanMaximizeProperty.Id");
			Assert.AreEqual (WindowPatternIdentifiers.CanMaximizeProperty.ProgrammaticName,
				WindowPattern.CanMaximizeProperty.ProgrammaticName,
				"CanMaximizeProperty.ProgrammaticName");
			Assert.AreEqual (WindowPatternIdentifiers.CanMinimizeProperty.Id,
				WindowPattern.CanMinimizeProperty.Id,
				"CanMinimizeProperty.Id");
			Assert.AreEqual (WindowPatternIdentifiers.CanMinimizeProperty.ProgrammaticName,
				WindowPattern.CanMinimizeProperty.ProgrammaticName,
				"CanMinimizeProperty.ProgrammaticName");
			Assert.AreEqual (WindowPatternIdentifiers.IsModalProperty.Id,
				WindowPattern.IsModalProperty.Id,
				"IsModalProperty.Id");
			Assert.AreEqual (WindowPatternIdentifiers.IsModalProperty.ProgrammaticName,
				WindowPattern.IsModalProperty.ProgrammaticName,
				"IsModalProperty.ProgrammaticName");
			Assert.AreEqual (WindowPatternIdentifiers.IsTopmostProperty.Id,
				WindowPattern.IsTopmostProperty.Id,
				"IsTopmostProperty.Id");
			Assert.AreEqual (WindowPatternIdentifiers.IsTopmostProperty.ProgrammaticName,
				WindowPattern.IsTopmostProperty.ProgrammaticName,
				"IsTopmostProperty.ProgrammaticName");
			Assert.AreEqual (WindowPatternIdentifiers.WindowInteractionStateProperty.Id,
				WindowPattern.WindowInteractionStateProperty.Id,
				"WindowInteractionStateProperty.Id");
			Assert.AreEqual (WindowPatternIdentifiers.WindowInteractionStateProperty.ProgrammaticName,
				WindowPattern.WindowInteractionStateProperty.ProgrammaticName,
				"WindowInteractionStateProperty.ProgrammaticName");
			Assert.AreEqual (WindowPatternIdentifiers.WindowVisualStateProperty.Id,
				WindowPattern.WindowVisualStateProperty.Id,
				"WindowVisualStateProperty.Id");
			Assert.AreEqual (WindowPatternIdentifiers.WindowVisualStateProperty.ProgrammaticName,
				WindowPattern.WindowVisualStateProperty.ProgrammaticName,
				"WindowVisualStateProperty.ProgrammaticName");

			Assert.AreEqual (WindowPatternIdentifiers.WindowClosedEvent.Id,
				WindowPattern.WindowClosedEvent.Id,
				"WindowClosedEvent.Id");
			Assert.AreEqual (WindowPatternIdentifiers.WindowClosedEvent.ProgrammaticName,
				WindowPattern.WindowClosedEvent.ProgrammaticName,
				"WindowClosedEvent.ProgrammaticName");
			Assert.AreEqual (WindowPatternIdentifiers.WindowOpenedEvent.Id,
				WindowPattern.WindowOpenedEvent.Id,
				"WindowOpenedEvent.Id");
			Assert.AreEqual (WindowPatternIdentifiers.WindowOpenedEvent.ProgrammaticName,
				WindowPattern.WindowOpenedEvent.ProgrammaticName,
				"WindowOpenedEvent.ProgrammaticName");
		}

		[Test]
		public void WaitForInputIdleTest ()
		{
			var window = (WindowPattern)
				testFormElement.GetCurrentPattern (WindowPattern.Pattern);

			bool result = window.WaitForInputIdle (500);

			Assert.IsTrue (result, "No wait when input is idle");

			RunCommand ("Open.ModalChildWindow");
			Thread.Sleep (500);
			var childWindowElement = testFormElement.FindFirst (
				TreeScope.Children,
				new PropertyCondition (AutomationElement.NameProperty,
					"TestForm1.ModalForm1"));

			result = window.WaitForInputIdle (500);

			Assert.IsTrue (result, "No wait even when showing a modal child window");
			( (WindowPattern) childWindowElement.GetCurrentPattern (WindowPattern.Pattern) ).Close ();

			RunCommand ("Sleep.2000");
			result = window.WaitForInputIdle (200);
			Assert.IsFalse (result, "Need to wait longer for idle if thread is sleeping");

			RunCommand ("Sleep.2000");
			result = window.WaitForInputIdle (4000);
			Assert.IsTrue (result, "Long enough wait for idle when thread is sleeping");

			// TODO: Other cases?
			// TODO: To get False result, test with another child
			//       window element that we get a ref to on open,
			//       but which is not immediately idle (how?)
			// TODO: Also test WindowInteractionState more as part of this?
		}

		[Test]
		public void IsModalTest ()
		{
			var window = (WindowPattern)
				testFormElement.GetCurrentPattern (WindowPattern.Pattern);

			Assert.IsFalse (window.Current.IsModal, "Main form is not modal");
			Assert.AreEqual (WindowInteractionState.ReadyForUserInteraction,
				window.Current.WindowInteractionState,
				"When no modal child form showing, main form is in ready state");

			RunCommand ("Open.ModalChildWindow");
			Thread.Sleep (500);

			var childWindowElement = testFormElement.FindFirst (
				TreeScope.Children,
				new PropertyCondition (AutomationElement.NameProperty,
					"TestForm1.ModalForm1"));
			Assert.IsNotNull (childWindowElement, "Unable to find AutomationElement for modal child window");

			var childWindow = (WindowPattern)
				childWindowElement.GetCurrentPattern (WindowPattern.Pattern);

			Assert.IsTrue (childWindow.Current.IsModal, "Child form is modal");
			// NOTE: Not the BlockedByModalWindow value you might expect
			Assert.AreEqual (WindowInteractionState.Running,
				window.Current.WindowInteractionState,
				"When modal child form showing, main form is in running state");

			childWindow.Close ();
			Assert.AreEqual (WindowInteractionState.ReadyForUserInteraction,
				window.Current.WindowInteractionState,
				"When no modal child form showing, main form is in ready state");
		}

		[Test]
		public void IsTopmostTest ()
		{
			var automationEventsArray = new [] {
				new {Sender = (object) null, Args = (AutomationPropertyChangedEventArgs) null}};
			var automationEvents = automationEventsArray.ToList ();
			automationEvents.Clear ();

			SWA.Automation.AddAutomationPropertyChangedEventHandler (testFormElement,
				TreeScope.Element,
				(o, e) => automationEvents.Add (new { Sender = o, Args = e }),
				WindowPattern.IsTopmostProperty);

			var window = (WindowPattern)
				testFormElement.GetCurrentPattern (WindowPattern.Pattern);

			Assert.IsFalse (window.Current.IsTopmost, "Initial val");
			var current = window.Current;

			RunCommand ("Toggle.Window.IsTopmost");
			Thread.Sleep (200);
			Assert.IsTrue (window.Current.IsTopmost, "Val after toggling on");
			Assert.AreEqual (window.Current.IsTopmost, current.IsTopmost,
				"Even old WindowPatternInformation instances return correct value");

			RunCommand ("Toggle.Window.IsTopmost");
			Thread.Sleep (200);
			Assert.IsFalse (window.Current.IsTopmost, "Val after toggling off");

			Assert.AreEqual (0, automationEvents.Count,
				"No property changed events expected");
		}

		[Test]
		public void WindowVisualStateTest ()
		{
			var automationEventsArray = new [] {
				new {Sender = (object) null, Args = (AutomationPropertyChangedEventArgs) null}};
			var automationEvents = automationEventsArray.ToList ();
			automationEvents.Clear ();

			SWA.Automation.AddAutomationPropertyChangedEventHandler (testFormElement,
				TreeScope.Element,
				(o, e) => automationEvents.Add (new { Sender = o, Args = e }),
				WindowPattern.CanMaximizeProperty,
				WindowPattern.CanMinimizeProperty,
				WindowPattern.WindowVisualStateProperty);

			var window = (WindowPattern)
				testFormElement.GetCurrentPattern (WindowPattern.Pattern);

			Assert.IsTrue (window.Current.CanMaximize, "CanMaximize");
			Assert.IsTrue (window.Current.CanMinimize, "CanMinimize");
			Assert.AreEqual (WindowVisualState.Normal,
				window.Current.WindowVisualState,
				"WindowVisualState");

			window.SetWindowVisualState (WindowVisualState.Maximized);
			Thread.Sleep (1000);

			Assert.IsTrue (window.Current.CanMaximize, "CanMaximize");
			Assert.IsTrue (window.Current.CanMinimize, "CanMinimize");
			Assert.AreEqual (WindowVisualState.Maximized,
				window.Current.WindowVisualState,
				"WindowVisualState");
			Assert.AreEqual (1, automationEvents.Count, "event count");
			Assert.AreEqual (WindowPattern.WindowVisualStateProperty,
				automationEvents [0].Args.Property, "event property");
			Assert.AreEqual (testFormElement,
				automationEvents [0].Sender, "event sender");
			Assert.AreEqual (null,
				automationEvents [0].Args.OldValue, "event old val");
			Assert.AreEqual (WindowVisualState.Maximized,
				automationEvents [0].Args.NewValue, "event new val");

			automationEvents.Clear ();
			window.SetWindowVisualState (WindowVisualState.Minimized);
			Thread.Sleep (1000);

			Assert.IsTrue (window.Current.CanMaximize, "CanMaximize");
			Assert.IsTrue (window.Current.CanMinimize, "CanMinimize");
			Assert.AreEqual (WindowVisualState.Minimized,
				window.Current.WindowVisualState,
				"WindowVisualState");
			Assert.AreEqual (1, automationEvents.Count, "event count");
			Assert.AreEqual (WindowPattern.WindowVisualStateProperty,
				automationEvents [0].Args.Property, "event property");
			Assert.AreEqual (testFormElement,
				automationEvents [0].Sender, "event sender");
			Assert.AreEqual (null,
				automationEvents [0].Args.OldValue, "event old val");
			Assert.AreEqual (WindowVisualState.Minimized,
				automationEvents [0].Args.NewValue, "event new val");

			automationEvents.Clear ();
			window.SetWindowVisualState (WindowVisualState.Normal);
			Thread.Sleep (1000);

			Assert.IsTrue (window.Current.CanMaximize, "CanMaximize");
			Assert.IsTrue (window.Current.CanMinimize, "CanMinimize");
			Assert.AreEqual (WindowVisualState.Normal,
				window.Current.WindowVisualState,
				"WindowVisualState");
			Assert.AreEqual (1, automationEvents.Count, "event count");
			Assert.AreEqual (WindowPattern.WindowVisualStateProperty,
				automationEvents [0].Args.Property, "event property");
			Assert.AreEqual (testFormElement,
				automationEvents [0].Sender, "event sender");
			Assert.AreEqual (null,
				automationEvents [0].Args.OldValue, "event old val");
			Assert.AreEqual (WindowVisualState.Normal,
				automationEvents [0].Args.NewValue, "event new val");

			// CanMaximize=True, CanMinimize=False
			automationEvents.Clear ();
			RunCommand ("Toggle.Window.CanMinimize");
			Thread.Sleep (1000);

			// NOTE: Here and below, not property change events for CanMinimize/CanMaximize
			Assert.AreEqual (0, automationEvents.Count, "event count");

			Assert.IsTrue (window.Current.CanMaximize, "CanMaximize");
			Assert.IsFalse (window.Current.CanMinimize, "CanMinimize");
			AssertRaises<InvalidOperationException> (
				() => window.SetWindowVisualState (WindowVisualState.Minimized),
				"trying to Minimize when CanMinimize is False");
			Thread.Sleep (500);
			Assert.AreEqual (WindowVisualState.Normal,
				window.Current.WindowVisualState,
				"WindowVisualState");

			window.SetWindowVisualState (WindowVisualState.Maximized);
			Thread.Sleep (500);
			Assert.AreEqual (WindowVisualState.Maximized,
				window.Current.WindowVisualState,
				"WindowVisualState");

			// Reset window to normal state
			window.SetWindowVisualState (WindowVisualState.Normal);
			Thread.Sleep (500);

			// CanMaximize=False, CanMinimize=True
			automationEvents.Clear ();
			RunCommand ("Toggle.Window.CanMinimize");
			RunCommand ("Toggle.Window.CanMaximize");
			Thread.Sleep (1000);

			Assert.AreEqual (0, automationEvents.Count, "event count");

			Assert.IsFalse (window.Current.CanMaximize, "CanMaximize");
			Assert.IsTrue (window.Current.CanMinimize, "CanMinimize");
			AssertRaises<InvalidOperationException> (
				() => window.SetWindowVisualState (WindowVisualState.Maximized),
				"trying to Maximize when CanMaximize is False");
			Thread.Sleep (500);
			Assert.AreEqual (WindowVisualState.Normal,
				window.Current.WindowVisualState,
				"WindowVisualState");

			window.SetWindowVisualState (WindowVisualState.Minimized);
			Thread.Sleep (500);
			Assert.AreEqual (WindowVisualState.Minimized,
				window.Current.WindowVisualState,
				"WindowVisualState");

			// Set form back to normal
			RunCommand ("Toggle.Window.CanMaximize");
		}

		[Test]
		public void OpenCloseTest ()
		{
			// Fun with lists of anonymous types
			var automationEventsArray = new [] {
				new {Sender = (object) null, Args = (AutomationEventArgs) null}};
			var automationEvents = automationEventsArray.ToList ();
			automationEvents.Clear ();

			SWA.Automation.AddAutomationEventHandler (WindowPattern.WindowOpenedEvent,
				AutomationElement.RootElement,
				TreeScope.Children,
				(o, e) => automationEvents.Add (new { Sender = o, Args = e }));

			// Open child window
			RunCommand ("Open.ChildWindow");
			Thread.Sleep (500);

			AutomationElement childWindow = AutomationElement.RootElement.FindFirst (
				TreeScope.Children,
				new PropertyCondition (AutomationElement.NameProperty,
					"TestForm1.ChildForm1"));
			Assert.IsNotNull (childWindow, "Unable to find AutomationElement for child window");

			// TODO: Figure out why multiple events sometimes occur,
			// and check that sender/arg values are correct in *all*
			Assert.IsTrue (automationEvents.Count > 0, "Check that event was raised");
			Assert.AreEqual (WindowPattern.WindowOpenedEvent,
				automationEvents [0].Args.EventId, "EventId");
			Assert.AreEqual (childWindow, automationEvents [0].Sender, "sender");

			automationEvents.Clear ();

			// TODO: Also test on element itself?
			SWA.Automation.AddAutomationEventHandler (WindowPattern.WindowClosedEvent,
				AutomationElement.RootElement,
				TreeScope.Subtree,
				(o, e) => automationEvents.Add (new { Sender = o, Args = e }));

			// Close window
			RunCommand ("Close.ChildWindow");

			Thread.Sleep (500);
			Assert.AreEqual (2, automationEvents.Count, "Check that only one even (close) was raised");
			Assert.AreEqual (WindowPattern.WindowClosedEvent,
				automationEvents [0].Args.EventId, "EventId");
			Assert.AreEqual (null, automationEvents [0].Sender, "sender");

			AssertRaises<ElementNotAvailableException> (
				() => childWindow.Current.Name.ToString (),
				"trying to access Current.Name for closed window element");

			automationEvents.Clear ();

			// Open window again
			RunCommand ("Open.ChildWindow");
			Thread.Sleep (500);

			childWindow = AutomationElement.RootElement.FindFirst (
				TreeScope.Children,
				new PropertyCondition (AutomationElement.NameProperty,
					"TestForm1.ChildForm1"));
			Assert.IsNotNull (childWindow, "Unable to find AutomationElement for child window");

			Assert.IsTrue (automationEvents.Count > 0, "Check that event was raised");
			Assert.AreEqual (WindowPattern.WindowOpenedEvent,
				automationEvents [0].Args.EventId, "EventId");
			Assert.AreEqual (childWindow, automationEvents [0].Sender, "sender");

			automationEvents.Clear ();

			// Close window
			( (WindowPattern) childWindow.GetCurrentPattern (WindowPattern.Pattern) ).Close ();

			Thread.Sleep (500);
			Assert.AreEqual (1, automationEvents.Count, "Check that only one even (close) was raised");
			Assert.AreEqual (WindowPattern.WindowClosedEvent,
				automationEvents [0].Args.EventId, "EventId");
			Assert.AreEqual (null, automationEvents [0].Sender, "sender");

			AssertRaises<ElementNotAvailableException> (
				() => ( (WindowPattern) childWindow.GetCurrentPattern (WindowPattern.Pattern) ).Close (),
				"trying to access Current.Name for closed window element");

			automationEvents.Clear ();


			// Exceptions adding event handlers
			// NOTE: TreeScope.Descendants is also not supported, but doesn't trigger the ArgumentException
			foreach (var scope in new TreeScope [] { TreeScope.Ancestors, TreeScope.Children, /*TreeScope.Descendants,*/ TreeScope.Element, TreeScope.Parent }) {
				AssertRaises<ArgumentException> (
					() => SWA.Automation.AddAutomationEventHandler (WindowPattern.WindowClosedEvent,
						AutomationElement.RootElement,
						scope,
						(o, e) => automationEvents.Add (new { Sender = o, Args = e })),
						"adding WindowClosedEvent handler on RootElement with TreeScope " + scope.ToString ());
			}

			AssertRaises<ArgumentException> (
				() => SWA.Automation.AddAutomationEventHandler (WindowPattern.WindowClosedEvent,
					button1Element,
					TreeScope.Element,
					(o, e) => automationEvents.Add (new { Sender = o, Args = e })),
					"adding WindowClosedEvent handler on non-Window element");
		}
		#endregion
	}
}
