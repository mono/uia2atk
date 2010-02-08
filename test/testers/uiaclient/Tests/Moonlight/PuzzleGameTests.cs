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
	public class PuzzleGameTests : TestBase
	{
		private Application app = null;
		private Window window = null;
		private string sample = "PuzzleGame";

		public PuzzleGameTests ()
		{
		}

		protected override void LaunchSample ()
		{
			app = new Application (sample);
			//app.Launch ("/usr/local/bin/firefox",
				    //"http://www.silverlightshow.net/showcase/PuzzleGame/PuzzleGame.html");
			app.Launch ("firefox",
				    "http://www.silverlightshow.net/showcase/PuzzleGame/PuzzleGame.html");
		}

		protected override void OnSetup ()
		{
			base.OnSetup ();
			window = app.GetWindow (string.Format ("{0} - Mozilla Firefox", sample));
		}

		protected override void OnQuit ()
		{
			base.OnQuit ();
			procedureLogger.Save ();
			window.Close ();
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
			procedureLogger.ExpectedResult ("The pictures should be placed disordered.");
			Thread.Sleep (Config.Instance.ShortDelay);

			// 301.2: Click "Reset" button
			var bReset = window.Find<Button> ("Reset");
			bReset.Click ();
			procedureLogger.ExpectedResult ("The pictures should be placed to default position.");
			Thread.Sleep (Config.Instance.ShortDelay);
		}

		// 302: Show Help
		private void TestCase302 ()
		{
			// 302.1: Click "Show Help" checkbox
			var cbShowHelp = window.Find<CheckBox> ("Show Help");
			cbShowHelp.Toggle ();
			procedureLogger.ExpectedResult ("Every picture should be marked with a number.");
			Thread.Sleep (Config.Instance.ShortDelay);

			// 302.2: Click "Show Help" checkbox again
			cbShowHelp.Toggle ();
			procedureLogger.ExpectedResult ("The number on each picture should disappeared.");
			Thread.Sleep (Config.Instance.ShortDelay);
		}

		// 303: Change Puzzle Dimension
		private void TestCase303 ()
		{
			// 303.1: Check current ExpandCollapseState
			var cbPuzzleDimension = window.Find<ComboBox> ("7 X 7");
			Assert.AreEqual (cbPuzzleDimension.ExpandCollapseState, ExpandCollapseState.Collapsed);
			Thread.Sleep (Config.Instance.ShortDelay);

			// 303.2: Check current Selection
			// TODO:
			//Assert.AreEqual ();
			Thread.Sleep (Config.Instance.ShortDelay);

			// 303.3: Expand "cbPuzzleDimension" 7X7 combobox
			cbPuzzleDimension.Expand ();
			procedureLogger.ExpectedResult ("The PuzzleDemension ComboBox should be expanded.");
			Thread.Sleep (Config.Instance.ShortDelay);

			// 303.4: Check current ExpandCollapseState
			Assert.AreEqual (cbPuzzleDimension.ExpandCollapseState, ExpandCollapseState.Expanded);
			Thread.Sleep (Config.Instance.ShortDelay);

			// 303.5: Click "3X3" ListItem element to select it
			var li3X3 = cbPuzzleDimension.Find<ListItem> ("3 X 3");
			li3X3.Select ();
			procedureLogger.ExpectedResult ("The \"3 X 3\" item in ComboBox should be selected.");
			Thread.Sleep (Config.Instance.ShortDelay);

			// 303.6: Check "3X3" ListItem element current states
			Assert.IsTrue (li3X3.IsSelected);
			Thread.Sleep (Config.Instance.ShortDelay);

			// 303.7: Check current Selection of "ComboBox" element
			// TODO:
			//Assert.AreEqual ();
			Thread.Sleep (Config.Instance.ShortDelay);

			// 303.8: Collapse "cbPuzzleDimension" 3X3 combobox
			cbPuzzleDimension.Collapse ();
			procedureLogger.ExpectedResult ("The PuzzleDemension ComboBox should be collapsed.");
			Thread.Sleep (Config.Instance.ShortDelay);

			// 303.9: Check current ExpandCollapseState
			Assert.AreEqual (cbPuzzleDimension.ExpandCollapseState, ExpandCollapseState.Collapsed);
			Thread.Sleep (Config.Instance.ShortDelay);
		}
	}
}
