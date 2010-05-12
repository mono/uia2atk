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
using At = System.Windows.Automation.Automation;
using NUnit.Framework;
using MonoTests.System.Windows.Automation;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	// Note: this TestFixture will fail on Windows 7, see comment of
	// "ListViewProviderTest.MultipleView_GetSupportedViewsTest" in UIAutomationWinformsTest
	public class MultipleViewPatternTest : BaseTest
	{
		private MultipleViewPattern pattern = null;

		public override void FixtureSetUp ()
		{
			base.FixtureSetUp ();
			pattern = (MultipleViewPattern) listView1Element.GetCurrentPattern (MultipleViewPattern.Pattern);
			Assert.IsNotNull (pattern);
		}

		[Test]
		public void PropertiesTest ()
		{
			CheckPatternIdentifiers<MultipleViewPattern> ();
		}

		[Test]
		public void ViewTest ()
		{
			var currentView = pattern.Current.CurrentView;
			var supportedViews = pattern.Current.GetSupportedViews ();
			Assert.Greater (supportedViews.Length, 1, "GetSupportedViews.Length");
			Assert.AreEqual (1, currentView, "GetSupportedViews.Value");
			Assert.AreEqual ("Details", pattern.GetViewName (currentView), "Current view name" );
			int argumentExceptionCount = 0;
			AssertRaises<ArgumentException> (() => pattern.GetViewName (-1), "get view name from -1");
			AssertRaises<ArgumentException> (() => pattern.GetViewName (supportedViews.Length),
				"get view name from supportedViews.Length");
		}

		[Test]
		public void SetErrorViewTest ()
		{
			var oldView = pattern.Current.CurrentView;
			AssertRaises<ArgumentException> (() => pattern.SetCurrentView (123456), "Set invalid viewId");
			var newView = pattern.Current.CurrentView;
			Assert.AreEqual (newView, oldView, "Current view isn't changed");
		}

		[Test]
		//todo We ignore this test case on Linux to make other test cases pass.
		[Ignore]
		public void PatternLifeTest ()
		{
			RunCommand ("change list view mode list");
			//todo Currently our implementation will fail unless
			//re-find the "listView1Element" with following code:
//			listView1Element = testFormElement.FindFirst (TreeScope.Children,
//				new PropertyCondition (AEIds.NameProperty, "listView1"));
//			Assert.IsNotNull (listView1Element);
//			pattern = (MultipleViewPattern) listView1Element.GetCurrentPattern (MultipleViewPattern.Pattern);

			var currentView = pattern.Current.CurrentView;
			Assert.AreEqual ("List", pattern.GetViewName (currentView), "Current view name" );
			RunCommand ("change list view mode details");
		}

		[Test]
		//The "Z1_" prefix ensures the test case's execution sequence
		public void Z1_DynamicTest ()
		{
			var automationEventsArray = new [] {
				new {Sender = (object) null, Args = (AutomationPropertyChangedEventArgs) null}};
			var automationEvents = automationEventsArray.ToList ();
			automationEvents.Clear ();

			AutomationPropertyChangedEventHandler handler =
				(o, e) => automationEvents.Add (new { Sender = o, Args = e });
			At.AddAutomationPropertyChangedEventHandler (listView1Element,
			                                             TreeScope.Element, handler,
			                                             MultipleViewPattern.CurrentViewProperty,
			                                             MultipleViewPattern.SupportedViewsProperty);

			RunCommand ("change list view mode list");

			//var currentView = pattern.Current.CurrentView;

			// We should expect an AutomationPropertyChangedEvent here,
			// But since on Windows Winforms didn't fire such a event, then we also assert no event fired.
			Assert.AreEqual (0, automationEvents.Count, "event count");
			/*
			Assert.AreEqual (1, automationEvents.Count, "event count");
			Assert.AreEqual (listView1Element, automationEvents [0].Sender, "event sender");
			Assert.AreEqual (MultipleViewPattern.CurrentViewProperty, automationEvents [0].Args.Property, "property");
			Assert.AreEqual (3, automationEvents [0].Args.NewValue, "new value");
			Assert.AreEqual (1, automationEvents [0].Args.OldValue, "old value");
			Assert.AreEqual ("List", pattern.GetViewName (currentView), "Current view name" );*/
		}

		[Test]
		public void NotEnabledTest ()
		{
			RunCommand ("disable list view");
			listView1Element = testFormElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.NameProperty, "listView1"));
			Assert.IsNotNull (listView1Element);
			pattern = (MultipleViewPattern) listView1Element.GetCurrentPattern (MultipleViewPattern.Pattern);
			// We should expect and ElementNotEnabledException here
			// But since on Windows Winforms there isn't expcetion fired, then we also assert no exception here
			pattern.SetCurrentView (1);
			RunCommand ("enable list view");
			pattern = (MultipleViewPattern) listView1Element.GetCurrentPattern (MultipleViewPattern.Pattern);
		}
	}
}
