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
	public class SL2WithPrismTest : MoonlightTestBase
	{
		public override string Sample {
			get { return "SL2WithPrism"; }
		}

		[Test]
		public void RunTestCase304 ()
		{
			Run (TestCase304);
		}

		[Test]
		public void RunTestCase305 ()
		{
			Run (TestCase305);
		}

		// 304: LogIn With "a11y" User
		private void TestCase304 ()
		{
			// 304.1: Enter "a11y" into "User Name" textbox on the top of main page
			var eUserName = MainWindow.Find<Edit> (Direction.Vertical, 0);
			eUserName.SetValue ("a11y");
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("User Name should be entered in the TextBox.");

			procedureLogger.Action (string.Format ("Check {0}'s Value.", eUserName));
			procedureLogger.ExpectedResult (string.Format ("{0}'s Value is \"a11y\".", eUserName));
			Assert.AreEqual ("a11y", eUserName.Value);

			// 304.2: Enter "a11ya11y" into "Password" textbox on the top of main page
			var ePassword = MainWindow.Find<Edit> (Direction.Vertical, 1);
			ePassword.SetValue ("a11ya11y");
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("Password should be entered in the TextBox, and is displayed as dots.");

			procedureLogger.Action (string.Format ("Check {0}'s Value.", ePassword));
			procedureLogger.ExpectedResult (string.Format ("{0}'s Value is \"a11ya11y\".", ePassword));
			Assert.AreEqual ("a11ya11y", ePassword.Value);

			// 304.3: Click "LogIn" button
			var bLogIn = MainWindow.Find<Button> ("LogIn");
			bLogIn.Click ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("Account should be logged in successfully.");
			Assert.IsTrue (bLogIn.IsOffscreen);
		}

		// 305: Move ScrollBar To Invoke HyperLink
		private void TestCase305 ()
		{
			// 305.1: Move ScrollBar to the Maximum value
			var sbRight = MainWindow.Find<ScrollBar> ();
			sbRight.SetValue (sbRight.Maximum);
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.Action (string.Format ("Check {0}'s Maximum.", sbRight));
			procedureLogger.ExpectedResult ("The web page should be scrolled to bottom.");
			Assert.AreEqual (sbRight.Maximum, sbRight.Value);

			// 305.2: Click "http://www.codeplex.com/SL2WithPrism" hyperlink
			var hProject = MainWindow.Find<Hyperlink> ();
			hProject.Click ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.Action (string.Format ("Check if {0} exists.", hProject));
			procedureLogger.ExpectedResult ("The link should be opened in the current page.");
			Assert.IsNull (hProject);
		}
	}
}
