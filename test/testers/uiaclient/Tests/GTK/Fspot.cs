// F-Spot.cs: Tests for KeePass
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
using System.Diagnostics;
using NUnit.Framework;
using System.Windows.Automation;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.TestFramework;

namespace MonoTests.Mono.UIAutomation.UIAClientAPI.GTk
{
	[TestFixture]
	public class FSpot : TestBase
	{
		Window window = null;

		protected override void LaunchSample ()
		{
			Application app = new Application ("f-spot");
			app.Launch ();
			Console.WriteLine("launch is successful.............................!");
		}

		protected override void OnSetup ()
		{
			base.OnSetup ();
			window = GetWindow("F-Spot");		}

		protected override void OnQuit ()
		{
			base.OnQuit ();
//			window.Close (false);
//			window.Find<Button> ("Discard changes").Click (false);		
		}

		//TestCase201 Init Sample, create a new account
		[Test]
		public void RunTestCase201 ()
		{
			Run (TestCase201);
		}

		private void TestCase201 ()
		{
			Console.WriteLine("0000000000000000000000");
			//201.1 Click "photo" menu.
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<Menu> ("photo").Click (false);
			procedureLogger.ExpectedResult ("The \"Import\" dialog opens.");
			Thread.Sleep(Config.Instance.ShortDelay);

			//201.2 Click "Import" menu item.
			var newPassDialog = window.Find<Window> ("Create New Password Database");
			var fileNameComboBox = newPassDialog.FindAll<ComboBox>(ControlType.ComboBox)[1];
			fileNameComboBox.SetValue("TestCase201");
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("\"TestCase201\" entered in the \"File Name\" box.");
			Assert.AreEqual (fileNameComboBox.Value, "TestCase201");
			Thread.Sleep(Config.Instance.ShortDelay);

			
			//201.3 : click "Select Folder menu item.
			var itemViewList = newPassDialog.Find<List> ("Items View");
			if (itemViewList.GetSupportedViews ().Contains<int> (0))
				itemViewList.SetCurrentView (0);
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("The current view of the dialog is \"Extra Large Icons\"");
			Assert.AreEqual (itemViewList.GetViewName(itemViewList.CurrentView), "Extra Large Icons");
			Thread.Sleep (Config.Instance.ShortDelay);

			//201.4 Invoke "Cancel" button.
			newPassDialog.Save ();
			procedureLogger.ExpectedResult ("The \"Create New Password Database\" dialog disappears.");
			Thread.Sleep(Config.Instance.ShortDelay);

			//201.5 Invoke "Cancel" button.
			var createMasterKeyWindow = window.Find<Window> ("Create Composite Master Key");
			var masterPasswdEdit = createMasterKeyWindow.Finder.ByName("Repeat password:").ByAutomationId("m_tbPassword").Find<Edit> ();
			Assert.AreEqual (masterPasswdEdit.IsReadOnly, false);
			masterPasswdEdit.SetValue ("mono-a11y");
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("\"mono-a11y\" entered in the \"Master password\" box.");
			Thread.Sleep(Config.Instance.ShortDelay);

			//201.6  Close the Window.
			var repeatPasswdEdit = createMasterKeyWindow.Finder.ByName ("Repeat password:").ByAutomationId ("m_tbRepeatPassword").Find<Edit> ();
			Assert.AreEqual (masterPasswdEdit.IsReadOnly, false);
			repeatPasswdEdit.SetValue ("mono-a11y");
			procedureLogger.ExpectedResult ("\"mono-a11y\" entered in the \"Repeat password\" box.");
			Thread.Sleep(Config.Instance.ShortDelay);

			//201.7 Check "Key file/option" CheckBox
			var keyfileCheckBox = createMasterKeyWindow.Find<CheckBox> ("Key file / provider:");
			keyfileCheckBox.Toggle();
			procedureLogger.ExpectedResult ("\"Key file/option\" CheckBox chekced.");
			Assert.AreEqual (keyfileCheckBox.ToggleState, ToggleState.On);
			Thread.Sleep (Config.Instance.ShortDelay);
		}
	}
}
