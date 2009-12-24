// DockPattern.cs: Tests for DockPattern
//
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
//	Ray Wang <rawang@novell.com>
//	Felicia Mu <fxmu@novell.com>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Automation;
using System.Diagnostics;
using Mono.UIAutomation.TestFramework;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class DockPatternTests : TestBase
	{
		Window window = null;

		protected override void LaunchSample ()
		{
			string sample = Path.Combine (System.AppDomain.CurrentDomain.BaseDirectory, "DockPatternProvider.exe");
			Application app = new Application (sample);
			app.Launch ();
		}

		protected override void OnSetup ()
		{
			base.OnSetup ();
			window = GetWindow ("DockPattern Test");
		}

		protected override void OnQuit ()
		{
			base.OnQuit ();
			window.Close ();
			window.Find<Button> ("Discard changes").Click ();
		}

		[Test]
		public void RunTestCase105 ()
		{
			Run (TestCase105);
		}

		private void TestCase105 ()
		{
			//105.1 Move the dock to the Left
			var dock = window.Find<Pane> ("Top");
			dock.SetDockPosition (DockPosition.Left);
			procedureLogger.ExpectedResult ("The Dock control is docked to the left.");
			Assert.AreEqual (dock.DockPosition, DockPosition.Left);
			Thread.Sleep (Config.Instance.ShortDelay);

			//105.2 Move the dock to the Right
			dock.SetDockPosition (DockPosition.Right);
			procedureLogger.ExpectedResult ("The Dock control is docked to the right.");
			Assert.AreEqual (dock.DockPosition, DockPosition.Right);
			Thread.Sleep (Config.Instance.ShortDelay);

			//105.3 Move the dock to the Bottom
			dock.SetDockPosition (DockPosition.Bottom);
			procedureLogger.ExpectedResult ("The Dock control is docked to the bottom.");
			Assert.AreEqual (dock.DockPosition, DockPosition.Bottom);
			Thread.Sleep (Config.Instance.ShortDelay);

			//105.4 Move the dock to the Top
			dock.SetDockPosition (DockPosition.Top);
			procedureLogger.ExpectedResult ("The Dock control is docked to the top.");
			Assert.AreEqual (dock.DockPosition, DockPosition.Top);
			Thread.Sleep (Config.Instance.ShortDelay);

			//105.5 Move the dock to be Filled.
			dock.SetDockPosition (DockPosition.Fill);
			procedureLogger.ExpectedResult ("The Dock control is docked to be filled.");
			Assert.AreEqual (dock.DockPosition, DockPosition.Fill);
			Thread.Sleep (Config.Instance.ShortDelay);

			//105.6 Move the dock to None.
			dock.SetDockPosition (DockPosition.None);
			procedureLogger.ExpectedResult ("The Dock control is docked to none.");
			Assert.AreEqual (dock.DockPosition, DockPosition.None);
			Thread.Sleep (Config.Instance.ShortDelay);
		}
	}
}
