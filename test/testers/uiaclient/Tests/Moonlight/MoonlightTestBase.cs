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
	public abstract class MoonlightTestBase : TestBase
	{
		private Application application;
		private Window window;

		protected override void LaunchSample ()
		{
			string browser = Environment.GetEnvironmentVariable ("MOON_A11Y_BROWSER");
			if (browser == null) {
				browser = "firefox";
				Console.WriteLine ("** MOON_A11Y_BROWSER environment variable not found. Defaulting to {0}.", browser);
			}

			string profile = Environment.GetEnvironmentVariable ("MOON_A11Y_BROWSER_PROFILE");
			if (profile == null) {
				profile = "default";
				Console.WriteLine ("** MOON_A11Y_BROWSER_PROFILE environment variable not found. Defaulting to {0}.", profile);
			}

			//string uri = string.Format ("samples/{0}/{0}.html", Sample);
			//string uri = "http://147.2.207.213/moonlight_apps/DiggSample/TestPage.html";

			application = new Application (Sample);
			application.Launch (browser, "-no-remote", "-P", profile, Uri);
		}

		protected override void OnSetup ()
		{
			base.OnSetup ();

			string name = Environment.GetEnvironmentVariable ("MOON_A11Y_BROWSER_NAME");
			if (name == null) {
				name = "Mozilla Firefox";
				Console.WriteLine ("** MOON_A11Y_BROWSER_NAME environment variable not found. Defaulting to {0}.", name);
			}

			window = application.GetWindow ("DiggSample - Mozilla Firefox");
		}

		protected override void OnQuit ()
		{
			base.OnQuit ();

			//procedureLogger.Save ();
			//window.Close ();
		}

		public Window MainWindow {
			get { return window; }
		}

		public abstract string Sample {
			get;
		}

		public abstract string Uri {
			get;
		}
	}
}
