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
	public class SL2WithPrismTests : TestBase
	{
		private Application app = null;
		private Window window = null;
		private string sample = "SL2WithPrism";

		public SL2WithPrismTests ()
		{
		}

		protected override void LaunchSample ()
		{
			app = new Application (sample);
			app.Launch ("/usr/local/bin/firefox",
				    "http://www.xentree.com/SL2WithPrism/");
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
			var eUserName = window.Find<Edit> ("User Name");
			eUserName.SetValue ("a11y");
			procedureLogger.ExpectedResult ("User Name should be entered in the TextBox.");
			Thread.Sleep (Config.Instance.ShortDelay);

			// 304.2: Enter "a11ya11y" into "Password" textbox on the top of main page
			var ePassword = window.Find<Edit> ("Password");
			ePassword.SetValue ("a11ya11y");
			procedureLogger.ExpectedResult ("Password should be entered in the TextBox, and is displayed as dots.");
			Thread.Sleep (Config.Instance.ShortDelay);

			// 304.3: Click "LogIn" button
			var bLogIn = window.Find<Button> ("LogIn");
			bLogIn.Click ();
			procedureLogger.ExpectedResult ("Account should be logged in successfully.");
			Thread.Sleep (Config.Instance.ShortDelay);
		}

		// 305: Move ScrollBar To Invoke HyperLink
		private void TestCase305 ()
		{
			// 305.1: Move ScrollBar to the Maximum value
			var sbRight = window.Find<ScrollBar> ();
			sbRight.SetScrollPercent (0, 100.0);
			procedureLogger.ExpectedResult ("The web page should be scrolled to bottom.");
			Thread.Sleep (Config.Instance.ShortDelay);

			// 305.2: Click "http://www.codeplex.com/SL2WithPrism" hyperlink
			var hProject = window.Find<Hyperlink> ();
			hProject.Click ();
			procedureLogger.ExpectedResult ("The link should be opened in the current page.");
			Thread.Sleep (Config.Instance.ShortDelay);
		}
	}
}
