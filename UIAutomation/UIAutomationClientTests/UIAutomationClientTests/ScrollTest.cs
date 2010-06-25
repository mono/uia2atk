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
	//This TestFixture will test ScrollPattern and ScrollItemPattern
	[TestFixture]
	public class ScrollTest : BaseTest
	{
		private ScrollPattern scrollPattern = null;
		private AutomationElement listItem0Element = null;
		private AutomationElement listItem9Element = null;
		private double lastHoriPos = 0.0;
		private double lastVertPos = 0.0;
		private double horiOffset = 0.0;
		private double vertOffset = 0.0;

		private void UpdatePosition ()
		{
			var currentHoriPos = scrollPattern.Current.HorizontalScrollPercent;
			horiOffset = currentHoriPos - lastHoriPos;
			lastHoriPos = currentHoriPos;
			var currentVertPos = scrollPattern.Current.VerticalScrollPercent;
			vertOffset = currentVertPos - lastVertPos;
			lastVertPos = currentVertPos;
		}

		private void ResetPosition ()
		{
			scrollPattern.SetScrollPercent (0.0, 0.0);
			Assert.AreEqual (0.0, scrollPattern.Current.VerticalScrollPercent, "Vertically reset");
			Assert.AreEqual (0.0, scrollPattern.Current.HorizontalScrollPercent, "Horizontally reset");
			lastHoriPos = 0.0;
			lastVertPos = 0.0;
			horiOffset = 0.0;
			vertOffset = 0.0;
		}

		private void ScrollToEnd ()
		{
			scrollPattern.SetScrollPercent (100.0, 100.0);
			Assert.AreEqual (100.0, scrollPattern.Current.VerticalScrollPercent, "Vertically scroll to end");
			Assert.AreEqual (100.0, scrollPattern.Current.HorizontalScrollPercent, "Horizontally scroll to end");
			UpdatePosition ();
		}

		private void AssertNotScrolled ()
		{
			UpdatePosition ();
			Assert.AreEqual (0, vertOffset, "It's not scrolled vertically");
			Assert.AreEqual (0, horiOffset, "It's not scrolled horizontally");
		}

		private void IneffectiveScrollTestInternal (ScrollAmount smallChange, ScrollAmount largeChange)
		{
			scrollPattern.Scroll (ScrollAmount.NoAmount, ScrollAmount.NoAmount);
			AssertNotScrolled ();
			scrollPattern.Scroll (largeChange, largeChange);
			AssertNotScrolled ();
			scrollPattern.Scroll (smallChange, smallChange);
			AssertNotScrolled ();
			scrollPattern.ScrollHorizontal (smallChange);
			AssertNotScrolled ();
			scrollPattern.ScrollHorizontal (largeChange);
			AssertNotScrolled ();
			scrollPattern.ScrollVertical (smallChange);
			AssertNotScrolled ();
			scrollPattern.ScrollVertical (largeChange);
			AssertNotScrolled ();
		}

		private void EffectiveScrollTestInternal (ScrollAmount smallChange, ScrollAmount largeChange, bool isOffsetPositive)
		{
			scrollPattern.Scroll (smallChange, smallChange);
			UpdatePosition ();
			var smallVertOffset = vertOffset;
			var smallHoriOffset = horiOffset;
			scrollPattern.Scroll (largeChange, largeChange);
			UpdatePosition ();
			var largeVertOffset = vertOffset;
			var largeHoriOffset = horiOffset;
			if (isOffsetPositive) {
				Assert.Greater (smallHoriOffset, 0.0, "Vertically scrolled");
				Assert.Greater (smallHoriOffset, 0.0, "Horizontally It's scrolled");
				Assert.Greater (largeVertOffset, smallVertOffset, "Vertically 'large' is larger than 'small'");
				Assert.Greater (largeHoriOffset, smallHoriOffset, "Horizontally 'large' is larger than 'small'");
			} else {
				Assert.Less (smallHoriOffset, 0.0, "Vertically scrolled");
				Assert.Less (smallHoriOffset, 0.0, "Horizontally It's scrolled");
				Assert.Less (largeVertOffset, smallVertOffset, "Vertically 'large' is larger than 'small'");
				Assert.Less (largeHoriOffset, smallHoriOffset, "Horizontally 'large' is larger than 'small'");
			}
		}

		private AutomationElement FindItem (string name)
		{
			var cond = new AndCondition(
			                            new PropertyCondition (
			                                                   AutomationElementIdentifiers.NameProperty,
			                                                   name),
			                            new PropertyCondition (
			                                                   AutomationElementIdentifiers.ControlTypeProperty,
			                                                   ControlType.DataItem));
			return listView1Element.FindFirst (TreeScope.Children, cond);
		}

		protected override void CustomFixtureSetUp ()
		{
			base.CustomFixtureSetUp ();
			scrollPattern = (ScrollPattern)
				listView1Element.GetCurrentPattern (ScrollPattern.Pattern);
			listItem0Element = FindItem ("Item 0");
			listItem9Element = FindItem ("Item 9");
		}

		[Test]
		public void PropertiesTest ()
		{
			CheckPatternIdentifiers<ScrollPattern> ();
			CheckPatternIdentifiers<ScrollItemPattern> ();
			Assert.AreEqual (-1.0, ScrollPattern.NoScroll, "Check ScrollPattern.NoScroll value");
			Assert.AreEqual (-1.0, ScrollPatternIdentifiers.NoScroll,
			                 "Check ScrollPatternIdentifiers.NoScroll value");
		}

		[Test]
		public void SetScrollPercentTest ()
		{
			ResetPosition ();
			//Double.NaN takes no effect on Windows
			scrollPattern.SetScrollPercent (double.NaN, double.NaN);
			AssertNotScrolled ();
			scrollPattern.SetScrollPercent (ScrollPattern.NoScroll, ScrollPattern.NoScroll);
			AssertNotScrolled ();

			scrollPattern.SetScrollPercent (ScrollPattern.NoScroll, 50.0);
			UpdatePosition ();
			Assert.Greater (lastVertPos, 0.0, "Vertical position bottom");
			Assert.Less (lastVertPos, 100.0, "Vertical position  top");
			scrollPattern.SetScrollPercent (ScrollPattern.NoScroll, 100.0);
			UpdatePosition ();
			Assert.AreEqual (100.0, lastVertPos, "Vertical position");

			AssertRaises<ArgumentOutOfRangeException> (
				() => scrollPattern.SetScrollPercent (ScrollPattern.NoScroll, 101.0),
				"Vertical out of range, top");

			AssertRaises<ArgumentOutOfRangeException> (
				() => scrollPattern.SetScrollPercent (ScrollPattern.NoScroll, -10.0),
				"Vertical out of range, bottom");

			scrollPattern.SetScrollPercent (50.0, ScrollPattern.NoScroll);
			UpdatePosition ();
			Assert.Greater (lastHoriPos, 0.0, "Horizontal position bottom");
			Assert.Less (lastHoriPos, 100.0, "Horizontal position  top");
			scrollPattern.SetScrollPercent (100.0, ScrollPattern.NoScroll);
			UpdatePosition ();
			Assert.AreEqual (100.0, lastHoriPos, "Horizontal position");

			AssertRaises<ArgumentOutOfRangeException> (
				() => scrollPattern.SetScrollPercent (101.0, ScrollPattern.NoScroll),
				"Horizontal out of range, top");

			AssertRaises<ArgumentOutOfRangeException> (
				() => scrollPattern.SetScrollPercent (-10.0, ScrollPattern.NoScroll),
				"Horizontal out of range, bottom");
		}

		[Test]
		public void ScrollableTest ()
		{
			Assert.IsTrue (scrollPattern.Current.HorizontallyScrollable,
			               "listView1.HorizontallyScrollable");
			Assert.IsTrue (scrollPattern.Current.VerticallyScrollable,
			               "listView1.VerticallyScrollable");
			RunCommand ("set textBox3 long text");
			var scrollPattern2 = (ScrollPattern)
				textbox3Element.GetCurrentPattern (ScrollPattern.Pattern);
			Assert.IsTrue (scrollPattern2.Current.HorizontallyScrollable,
			               "listView1.HorizontallyScrollable");
			Assert.IsFalse (scrollPattern2.Current.VerticallyScrollable,
			               "listView1.VerticallyScrollable");

			AssertRaises<InvalidOperationException> (
				() => scrollPattern2.SetScrollPercent (ScrollPattern.NoScroll, 50.0),
				"Vertical scroll is disabled on textBox3");
		}

		[Test]
		public void Z2_EventTest()
		{
			var automationEventsArray = new [] {
				new {Sender = (object) null, Args = (AutomationPropertyChangedEventArgs) null}};
			var automationEvents = automationEventsArray.ToList ();
			automationEvents.Clear ();
			AutomationPropertyChangedEventHandler handler = 
				(o, e) => automationEvents.Add (new { Sender = o, Args = e });
			ResetPosition ();
			At.AddAutomationPropertyChangedEventHandler (listView1Element,
				TreeScope.Element, handler,
				ScrollPattern.VerticalScrollPercentProperty,
				ScrollPattern.HorizontalScrollPercentProperty,
				ScrollPattern.VerticallyScrollableProperty,
				ScrollPattern.HorizontallyScrollableProperty,
				ScrollPattern.VerticalViewSizeProperty,
				ScrollPattern.HorizontalViewSizeProperty);

			scrollPattern.Scroll (ScrollAmount.LargeIncrement,
			                      ScrollAmount.LargeIncrement);
			UpdatePosition ();
			Assert.Greater (vertOffset, 0, "vert scrolled");
			Assert.Greater (horiOffset, 0, "hori scrolled");
			Thread.Sleep (500);
			bool vertChanged = false;
			bool horiChanged = false;
			foreach (var evt in automationEvents) {
				var args = evt.Args;
				Assert.AreEqual (listView1Element, evt.Sender, "event sender");
				//On Windows, args.OldValue is always null
				Assert.IsNull (args.OldValue, "event.OldValue");
				if (args.Property == ScrollPattern.VerticalScrollPercentProperty) {
					Assert.Greater ((double)args.NewValue, 0.0, "vert scrolled, event.NewValue");
					vertChanged = true;
				} else if (args.Property == ScrollPattern.HorizontalScrollPercentProperty) {
					Assert.Greater ((double)args.NewValue, 0.0, "hori scrolled, event.NewValue");
					horiChanged = true;
				}
			}
			Assert.IsTrue (vertChanged, "vert position event fired");
			Assert.IsTrue (horiChanged, "hori position event fired");

			automationEvents.Clear ();
			var oldVertViewSize = scrollPattern.Current.VerticalViewSize;
			RunCommand ("add listView1 item");
			var newVertViewSize = scrollPattern.Current.VerticalViewSize;
			Assert.Greater (oldVertViewSize, newVertViewSize, "size expanded, so view shrinked");
			vertChanged = false;
			bool viewSizeChanged = false;
			Thread.Sleep (500);
			foreach (var evt in automationEvents) {
				var args = evt.Args;
				Assert.AreEqual (listView1Element, evt.Sender, "event sender");
				//On Windows, args.OldValue is always null
				Assert.IsNull (args.OldValue, "event.OldValue");
				if (args.Property == ScrollPattern.VerticalViewSizeProperty)
					viewSizeChanged = true;
				else if (args.Property == ScrollPattern.VerticalScrollPercentProperty)
					vertChanged = true;
			}
			//On Windows, though viewSize changes, no event is fired
			Assert.IsFalse (viewSizeChanged, "viewSize changed event fired");
			Assert.IsTrue (vertChanged, "vert position event fired");

			automationEvents.Clear ();
			Assert.IsTrue (scrollPattern.Current.VerticallyScrollable);
			RunCommand ("make listView1 higher");
			Assert.IsFalse (scrollPattern.Current.VerticallyScrollable);
			vertChanged = false;
			viewSizeChanged = false;
			bool scrollableChanged = false;
			Thread.Sleep (500);
			foreach (var evt in automationEvents) {
				var args = evt.Args;
				Assert.AreEqual (listView1Element, evt.Sender, "event sender");
				//On Windows, args.OldValue is always null
				Assert.IsNull (args.OldValue, "event.OldValue");
				if (args.Property == ScrollPattern.VerticalViewSizeProperty)
					viewSizeChanged = true;
				else if (args.Property == ScrollPattern.VerticalScrollPercentProperty)
					vertChanged = true;
				else if (args.Property == ScrollPattern.VerticallyScrollableProperty)
					scrollableChanged = true;
			}
			//On Windows, though all Vertical*Property changes, no event is fired
			Assert.IsFalse (viewSizeChanged, "viewSize changed event fired");
			Assert.IsFalse (scrollableChanged, "viewSize changed event fired");
			Assert.IsFalse (vertChanged, "vert position event fired");

			At.RemoveAutomationPropertyChangedEventHandler (listView1Element, handler);
		}

		[Test]
		public void IneffectiveScrollFromTop ()
		{
			ResetPosition ();
			IneffectiveScrollTestInternal (ScrollAmount.SmallDecrement,
			                               ScrollAmount.LargeDecrement);
		}

		[Test]
		public void IneffectiveScrollTestFromBottom ()
		{
			ScrollToEnd ();
			IneffectiveScrollTestInternal (ScrollAmount.SmallIncrement,
			                               ScrollAmount.LargeIncrement);
		}

		[Test]
		public void EffectiveScrollTestFromTop ()
		{
			ResetPosition ();
			EffectiveScrollTestInternal (ScrollAmount.SmallIncrement,
			                             ScrollAmount.LargeIncrement,
			                             true);
		}

		[Test]
		public void EffectiveScrollTestFromBottom ()
		{
			ScrollToEnd ();
			EffectiveScrollTestInternal (ScrollAmount.SmallDecrement,
			                             ScrollAmount.LargeDecrement,
			                             false);
		}

		[Test]
		public void EffectiveScrollTestFromEnd ()
		{
			ResetPosition ();
			scrollPattern.Scroll (ScrollAmount.SmallIncrement, ScrollAmount.SmallIncrement);
			UpdatePosition ();
			var smallVertOffset = vertOffset;
			var smallHoriOffset = horiOffset;
			scrollPattern.Scroll (ScrollAmount.LargeIncrement, ScrollAmount.LargeIncrement);
			UpdatePosition ();
			var largeVertOffset = vertOffset;
			var largeHoriOffset = horiOffset;
			Assert.Greater (smallHoriOffset, 0.0, "Vertically scrolled");
			Assert.Greater (smallHoriOffset, 0.0, "Horizontally It's scrolled");
			Assert.Greater (largeVertOffset, smallVertOffset, "Vertically 'large' is greater than 'small'");
			Assert.Greater (largeHoriOffset, smallHoriOffset, "Horizontally 'large' is greater than 'small'");
		}

		[Test]
		public void ScrollItemTest ()
		{
			var item0Pattern = (ScrollItemPattern)
				listItem0Element.GetCurrentPattern (ScrollItemPattern.Pattern);
			item0Pattern.ScrollIntoView ();
			UpdatePosition ();
			var lastItemElement = FindItem ("Item Extra");
			if (lastItemElement == null)
				lastItemElement = listItem9Element;
			var lastItemPattern = (ScrollItemPattern)
				lastItemElement.GetCurrentPattern (ScrollItemPattern.Pattern);
			lastItemPattern.ScrollIntoView ();
			UpdatePosition ();
			Assert.AreEqual (100.0, lastVertPos, "Vertically scroll to end");
			Assert.AreEqual (100.0, vertOffset, "Vertically fully scrolled ");
		}

		[Test]
		public void Z1_NotEnabledtest ()
		{
			ResetPosition ();
			RunCommand ("disable list view");

			AssertRaises<InvalidOperationException> (
				() => scrollPattern.SetScrollPercent (50.0, 50.0),
				"Disabled #1");

			AssertRaises<InvalidOperationException> (
				() => scrollPattern.SetScrollPercent (ScrollPattern.NoScroll, ScrollPattern.NoScroll),
				"Disabled #2");

			AssertRaises<InvalidOperationException> (
				() => scrollPattern.Scroll (ScrollAmount.LargeIncrement, ScrollAmount.LargeIncrement),
				"Disabled #3");

			listItem9Element = FindItem ("Item 9");
			Assert.IsNotNull (listItem9Element, "listItem9Element");
			AssertRaises<InvalidOperationException> (
				() => listItem9Element.GetCurrentPattern (ScrollItemPattern.Pattern),
				"Disabled #4");

			RunCommand ("enable list view");
		}
	}
}