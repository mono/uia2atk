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
// 	Neville Gao <nevillegao@gmail.com>
//

using System;
using System.Threading;
using System.Windows.Automation;

using NUnit.Framework;
using Mono.UIAutomation.TestFramework;

namespace MonoTests.Mono.UIAutomation.UIAClientAPI.Moonlight
{
	[TestFixture]
	public class DiggSearchTest : MoonlightTestBase
	{
		public override string Sample {
			get { return "DiggSearch"; }
		}

		[Test]
		public void RunTestCase306 ()
		{
			Run (TestCase306);
		}

		[Test]
		public void RunTestCase307 ()
		{
			Run (TestCase307);
		}

		[Test]
		public void RunTestCase308 ()
		{
			Run (TestCase308);
		}

		[Test]
		public void RunTestCase309 ()
		{
			Run (TestCase309);
		}

		// 306: Check List
		private void TestCase306 ()
		{
			// 306.1: Run the application, there is not listitems on the page
			var lResults = MainWindow.Find<List> ();
			procedureLogger.Action (string.Format ("Check {0}'s HorizontallyScrollable.", lResults));
			procedureLogger.ExpectedResult (string.Format ("{0}'s HorizontallyScrollable is False.", lResults));
			Assert.IsFalse (lResults.HorizontallyScrollable);

			procedureLogger.Action (string.Format ("Check {0}'s HorizontalScrollPercent.", lResults));
			procedureLogger.ExpectedResult (string.Format ("{0}'s HorizontalScrollPercent is -1.", lResults));
			Assert.AreEqual (-1, lResults.HorizontalScrollPercent);

			procedureLogger.Action (string.Format ("Check {0}'s VerticallyScrollable.", lResults));
			procedureLogger.ExpectedResult (string.Format ("{0}'s VerticallyScrollable is False.", lResults));
			Assert.IsFalse (lResults.VerticallyScrollable);

			procedureLogger.Action (string.Format ("Check {0}'s VerticalScrollPercent.", lResults));
			procedureLogger.ExpectedResult (string.Format ("{0}'s VerticalScrollPercent is -1.", lResults));
			Assert.AreEqual (-1, lResults.VerticalScrollPercent);

			// 306.2: Check the current selection of "list" element
			procedureLogger.Action (string.Format ("Check {0}'s CanSelectMultiple.", lResults));
			procedureLogger.ExpectedResult (string.Format ("{0}'s CanSelectMultiple is False.", lResults));
			Assert.IsFalse (lResults.CanSelectMultiple);

			procedureLogger.Action (string.Format ("Check {0}'s IsSelectionRequired.", lResults));
			procedureLogger.ExpectedResult (string.Format ("{0}'s IsSelectionRequired is False.", lResults));
			Assert.IsFalse (lResults.IsSelectionRequired);
		}
		// 307: Search For Items
		private void TestCase307 ()
		{
			// 307.1: Enter "basketball" into textbox on the top right side of the main page
			var eSearch = MainWindow.Find<Edit> ();
			eSearch.SetValue ("basketball");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action (string.Format ("Check {0}'s Value.", eSearch));
			procedureLogger.ExpectedResult (string.Format ("{0}'s Value is \"basketball\".", eSearch));
			Assert.AreEqual ("basketball", eSearch.Value);

			// 307.2: Click "Search" button
			var bSearch = MainWindow.Find<Button> ("Search");
			bSearch.Click ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("Some contents should display on the page.");

			// 307.3: Check if the list is scrollable
			var lResults = MainWindow.Find<List> ();
			procedureLogger.Action (string.Format ("Check {0}'s HorizontallyScrollable.", lResults));
			procedureLogger.ExpectedResult (string.Format ("{0}'s HorizontallyScrollable is True.", lResults));
			Assert.IsFalse (lResults.HorizontallyScrollable);

			procedureLogger.Action (string.Format ("Check {0}'s VerticallyScrollable.", lResults));
			procedureLogger.ExpectedResult (string.Format ("{0}'s VerticallyScrollable is True.", lResults));
			Assert.IsTrue (lResults.VerticallyScrollable);

			procedureLogger.Action (string.Format ("Check {0}'s VerticalScrollPercent.", lResults));
			procedureLogger.ExpectedResult (string.Format ("{0}'s VerticalScrollPercent is 0.", lResults));
			Assert.AreEqual (0, lResults.VerticalScrollPercent);
		}

		// 308: Select and View Items
		private void TestCase308 ()
		{
			// 308.1: Check the  "list item" is not selected, listitem[3] for example
			var liResult = MainWindow.FindAll<ListItem> ()[3];
			procedureLogger.Action (string.Format ("Check {0}'s IsSelected.", liResult));
			procedureLogger.ExpectedResult (string.Format ("{0}'s IsSelected is False.", liResult));
			Assert.IsFalse (liResult.IsSelected);

			// 308.2: Select the "list item" with "Select" method, listitem[3] for example
			var bClose = MainWindow.Find<Button> ("Close");
			liResult.Select ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("{0}'s IsOffscreen is False.", bClose));
			Assert.IsFalse (bClose.IsOffscreen);

			// 308.3: Check the selection state of listitem[3] again
			procedureLogger.Action (string.Format ("Check {0}'s IsSelected.", liResult));
			procedureLogger.ExpectedResult (string.Format ("{0}'s IsSelected is True.", liResult));
			Assert.IsTrue (liResult.IsSelected);

			// 308.4: Close the small dialog by clicking "Close" button
			bClose.Click ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("{0}'s IsOffscreen is True.", bClose));
			Assert.IsTrue (bClose.IsOffscreen);

			// Check the selection state of listitem[3] again
			procedureLogger.Action (string.Format ("Check {0}'s IsSelected.", liResult));
			procedureLogger.ExpectedResult (string.Format ("{0}'s IsSelected is True.", liResult));
			Assert.IsTrue (liResult.IsSelected);

			// 308.5: Check the current selection of "list" element
			var lResults = MainWindow.Find<List> ();
			var liResult1 = lResults.GetSelection ();
			procedureLogger.ExpectedResult (string.Format ("{0} selects {1}.", lResults, liResult1));
			Assert.AreEqual (liResult, liResult1);

			// 308.6: Remove the selection of listitem[3]
			liResult.RemoveFromSelection ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("{0}'s IsSelected is False.", liResult));
			Assert.IsFalse (liResult.IsSelected);
		}

		// 309: Select and View Items
		private void TestCase309 ()
		{
			// 309.1: Move the vertical scrollbar to bottom(Select the last list item, use "ScrollIntoView" method)
			var lResults = MainWindow.Find<List> ();
			double prevVerticalScrollPercent = lResults.VerticalScrollPercent;

			var liResult = MainWindow.FindAll<ListItem> ()[9];
			liResult.ScrollIntoView ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("Window should scroll down. The ListItem which was out-of-screen appear now.");

			// 309.2: Check if the list is scrolled
			procedureLogger.Action (string.Format ("Check {0}'s HorizontallyScrollable.", lResults));
			procedureLogger.ExpectedResult (string.Format ("{0}'s HorizontallyScrollable is False.", lResults));
			Assert.IsFalse (lResults.HorizontallyScrollable);

			procedureLogger.Action (string.Format ("Check {0}'s HorizontalScrollPercent.", lResults));
			procedureLogger.ExpectedResult (string.Format ("{0}'s HorizontalScrollPercent is -1.", lResults));
			Assert.AreEqual (-1, lResults.HorizontalScrollPercent);

			procedureLogger.Action (string.Format ("Check {0}'s VerticallyScrollable.", lResults));
			procedureLogger.ExpectedResult (string.Format ("{0}'s VerticallyScrollable is True.", lResults));
			Assert.IsTrue (lResults.VerticallyScrollable);

			procedureLogger.Action (string.Format ("Check {0}'s VerticalScrollPercent.", lResults));
			procedureLogger.ExpectedResult (string.Format ("{0}'s VerticalScrollPercent changed.", lResults));
			Assert.AreNotEqual (prevVerticalScrollPercent, lResults.VerticalScrollPercent);
		}
	}
}
