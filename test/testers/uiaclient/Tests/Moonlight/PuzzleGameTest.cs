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
	public class PuzzleGameTest : MoonlightTestBase
	{
		private Window window;

		public PuzzleGameTest (Window window) : base (window)
		{
			this.window = window;
		}

		public override string Sample {
			get { return "PuzzleGameTest"; }
		}

		[Test]
		public void RunTestCase301 ()
		{
			Run (TestCase301);
		}

		[Test]
		public void RunTestCase302 ()
		{
			Run (TestCase302);
		}

		[Test]
		public void RunTestCase303 ()
		{
			Run (TestCase303);
		}

		// 301: Scramble and Reset Puzzle
		private void TestCase301 ()
		{
			// 301.1: Click "Scramble" button
			var bScramble = window.Find<Button> ("Scramble");
			bScramble.Click ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("TextBlock \"Puzzle Not Solved.\" appears."));
			var lPuzzleStatus = window.Find<Text> ("Puzzle Not Solved.");
			Assert.IsNotNull (lPuzzleStatus);

			// 301.2: Click "Reset" button
			var bReset = window.Find<Button> ("Reset");
			bReset.Click ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("TextBlock \"Puzzle Not Solved.\" disappears."));
			Assert.AreEqual (string.Empty, lPuzzleStatus.Name);
		}

		// 302: Show Help
		private void TestCase302 ()
		{
			// 302.1: Click "Show Help" checkbox
			var cbShowHelp = window.Find<CheckBox> ("Show Help");
			cbShowHelp.Toggle ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("{0}'s ToggleState is \"On\".", cbShowHelp));
			Assert.AreEqual ("On", cbShowHelp.ToggleState);

			// 302.2: Click "Show Help" checkbox again
			cbShowHelp.Toggle ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("{0}'s ToggleState is \"Off\".", cbShowHelp));
			Assert.AreEqual ("Off", cbShowHelp.ToggleState);
		}

		// 303: Change Puzzle Dimension
		private void TestCase303 ()
		{
			// 303.1: Check current ExpandCollapseState
			var cbPuzzleDimension = window.Find<ComboBox> ();
			procedureLogger.Action (string.Format ("Check {0}'s ExpandCollapseState.", cbPuzzleDimension));
			procedureLogger.ExpectedResult (string.Format ("{0}'s ExpandCollapseState is Collapsed.", cbPuzzleDimension));
			Assert.AreEqual (ExpandCollapseState.Collapsed, cbPuzzleDimension.ExpandCollapseState);

			// 303.2: Check current Selection
			var li7X7 = cbPuzzleDimension.Find<ListItem> ("7 X 7");
			var liChoice = cbPuzzleDimension.GetSelection ()[0];
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("{0} selects {1}.", cbPuzzleDimension, li7X7));
			Assert.AreEqual (li7X7, liChoice);

			// 303.3: Expand "cbPuzzleDimension" 7X7 combobox
			cbPuzzleDimension.Expand ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("The PuzzleDemension ComboBox should be expanded.");

			// 303.4: Check current ExpandCollapseState
			procedureLogger.Action (string.Format ("Check {0}'s ExpandCollapseState.", cbPuzzleDimension));
			procedureLogger.ExpectedResult (string.Format ("{0}'s ExpandCollapseState is Expanded.", cbPuzzleDimension));
			Assert.AreEqual (ExpandCollapseState.Expanded, cbPuzzleDimension.ExpandCollapseState);

			// 303.5: Click "3X3" ListItem element to select it
			var li3X3 = cbPuzzleDimension.Find<ListItem> ("3 X 3");
			li3X3.Select ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("{0}'s IsSelected is True.", li3X3));
			Assert.IsTrue (li3X3.IsSelected);

			// 303.6: Check current Selection of "ComboBox" element
			var liChoice1 = cbPuzzleDimension.GetSelection ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("{0} selects {1}.", cbPuzzleDimension, li3X3));
			Assert.AreEqual (li3X3, liChoice1);

			// 303.7: Collapse "cbPuzzleDimension" 3X3 combobox
			cbPuzzleDimension.Collapse ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("{0}'s ExpandCollapseState is Collapsed.", cbPuzzleDimension));
			Assert.AreEqual (ExpandCollapseState.Collapsed, cbPuzzleDimension.ExpandCollapseState);

			// 303.8: Check AddToSelection method
			li7X7.AddToSelection ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.Action (string.Format ("Check {0}'s IsSelected.", li7X7));
			procedureLogger.ExpectedResult (string.Format ("{0}'s IsSelected is True.", li7X7));
			Assert.IsTrue (li3X3.IsSelected);
		}
	}
}
