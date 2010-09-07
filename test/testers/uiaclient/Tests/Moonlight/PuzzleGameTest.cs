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
		public override string Sample {
			get { return "PuzzleGame"; }
		}
		
		public override string Uri {
			get { return "http://www.silverlightshow.net/showcase/PuzzleGame/PuzzleGame.html"; }
		}

		public override string Title {
			get { return "PuzzleGame - Mozilla Firefox"; }
		}

		[Test]
		public void RunTestCase301 ()
		{
			Run (TestCase301);
		}

		private void TestCase301 ()
		{
			// Click "Scramble" button
			var bScramble = MainWindow.Find<Button> ("Scramble");
			bScramble.Click ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("TextBlock \"Puzzle Not Solved.\" appears."));
			var lPuzzleStatus = MainWindow.Find<Text> ("Puzzle Not Solved.");
			Assert.IsNotNull (lPuzzleStatus);

			// Click "Reset" button
			var bReset = MainWindow.Find<Button> ("Reset");
			bReset.Click ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("TextBlock \"Puzzle Not Solved.\" disappears."));
			Assert.AreEqual (string.Empty, lPuzzleStatus.Name);

			// Click "Show Help" checkbox
			var cbShowHelp = MainWindow.Find<CheckBox> ("Show Help");
			cbShowHelp.Toggle ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("{0}'s ToggleState is \"On\".", cbShowHelp));
			Assert.AreEqual (ToggleState.On, cbShowHelp.ToggleState);

			// Click "Show Help" checkbox again
			cbShowHelp.Toggle ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult (string.Format ("{0}'s ToggleState is \"Off\".", cbShowHelp));
			Assert.AreEqual (ToggleState.Off, cbShowHelp.ToggleState);

			// Check current ExpandCollapseState
			//BUG608525
			//var cbPuzzleDimension = MainWindow.Find<ComboBox> ();
			//procedureLogger.Action (string.Format ("Check {0}'s ExpandCollapseState.", cbPuzzleDimension));
			//procedureLogger.ExpectedResult (string.Format ("{0}'s ExpandCollapseState is Collapsed.", cbPuzzleDimension));
			//Assert.AreEqual (ExpandCollapseState.Collapsed, cbPuzzleDimension.ExpandCollapseState);

			// Check current Selection
			//BUG629820
			//var li7X7 = cbPuzzleDimension.Find<ListItem> ("7 X 7");
			//var liChoice = cbPuzzleDimension.GetSelection ()[0];
			//Thread.Sleep (Config.Instance.ShortDelay);
			//procedureLogger.ExpectedResult (string.Format ("{0} selects {1}.", cbPuzzleDimension, li7X7));
			//Assert.AreEqual (li7X7, liChoice);

			// Expand "cbPuzzleDimension" 7X7 combobox
			//BUG635778
			//cbPuzzleDimension.Expand ();
			//Thread.Sleep (Config.Instance.ShortDelay);
			//procedureLogger.ExpectedResult ("The PuzzleDemension ComboBox should be expanded.");

			// Check current ExpandCollapseState
			//BUG635778
			//procedureLogger.Action (string.Format ("Check {0}'s ExpandCollapseState.", cbPuzzleDimension));
			//procedureLogger.ExpectedResult (string.Format ("{0}'s ExpandCollapseState is Expanded.", cbPuzzleDimension));
			//Assert.AreEqual (ExpandCollapseState.Expanded, cbPuzzleDimension.ExpandCollapseState);

			// Click "3X3" ListItem element to select it
			//BUG629820
			//var li3X3 = cbPuzzleDimension.Find<ListItem> ("3 X 3");
			//li3X3.Select ();
			//Thread.Sleep (Config.Instance.ShortDelay);
			//procedureLogger.ExpectedResult (string.Format ("{0}'s IsSelected is True.", li3X3));
			//Assert.IsTrue (li3X3.IsSelected);

			// Check current Selection of "ComboBox" element
			//BUG629820
			//var liChoice1 = cbPuzzleDimension.GetSelection ()[0];
			//Thread.Sleep (Config.Instance.ShortDelay);
			//procedureLogger.ExpectedResult (string.Format ("{0} selects {1}.", cbPuzzleDimension, li3X3));
			//Assert.AreEqual (li3X3, liChoice1);

			// Collapse "cbPuzzleDimension" 3X3 combobox
			//BUG635778
			//cbPuzzleDimension.Collapse ();
			//Thread.Sleep (Config.Instance.ShortDelay);
			//procedureLogger.ExpectedResult (string.Format ("{0}'s ExpandCollapseState is Collapsed.", cbPuzzleDimension));

			// Check current ExpandCollapseState
			//BUG635778
			//procedureLogger.Action (string.Format ("Check {0}'s ExpandCollapseState.", cbPuzzleDimension));
			//procedureLogger.ExpectedResult (string.Format ("{0}'s ExpandCollapseState is Expanded.", cbPuzzleDimension));
			//Assert.AreEqual (ExpandCollapseState.Collapsed, cbPuzzleDimension.ExpandCollapseState);

			// Check AddToSelection method
			//BUG629820
			//li7X7.AddToSelection ();
			//Thread.Sleep (Config.Instance.ShortDelay);
			//procedureLogger.Action (string.Format ("Check {0}'s IsSelected.", li7X7));
			//procedureLogger.ExpectedResult (string.Format ("{0}'s IsSelected is True.", li7X7));
			//Assert.IsTrue (li3X3.IsSelected);
		}
	}
}
