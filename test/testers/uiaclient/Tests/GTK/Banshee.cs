// Banshee.cs: Tests for Banshee
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
// included in all copies or substantial por	tions of the Software.
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
using System;

namespace MonoTests.Mono.UIAutomation.UIAClientAPI.Gtk
{
	[TestFixture]
	public class Banshee : TestBase
	{
		Window window = null;
		Application app;

		protected override void LaunchSample ()
		{
			app = new Application ("banshee-1");
			app.Launch ("bash", "banshee-1");
		}

		protected override void OnSetup ()
		{
			base.OnSetup ();
			window = app.GetWindow ("Banshee Media Player");
		}

		protected override void OnQuit ()
		{
			base.OnQuit ();
			window.Close ();
		}

		//TestCase208 Init Sample, create a new account
		[Test]
		public void RunTestCase208 ()
		{
			Run (TestCase208);
		}

		private void TestCase208 ()
		{
			//208.1 Check "Banshee Media Player" WindowsPattern's property
			var bansheeWindow = window.Find<Window> ("Banshee Media Player");
			procedureLogger.Action ("Check the bansheeWindow is modal.");
			procedureLogger.ExpectedResult ("The bansheeWindow's Ismodal is true.");
			Assert.AreEqual (true, bansheeWindow.IsModal);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the bansheeWindow is topmost.");
			procedureLogger.ExpectedResult ("The bansheeWindow's IsTopmost is true.");
			Assert.AreEqual (true, bansheeWindow.IsTopmost);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the bansheeWindow's WindowInteractionState.");
			procedureLogger.ExpectedResult ("The bansheeWindow's WindowInteractionState is true.");
			Assert.AreEqual (WindowInteractionState.ReadyForUserInteraction, bansheeWindow.WindowInteractionState);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the bansheeWindow's WindowVisualState.");
			procedureLogger.ExpectedResult ("The bansheeWindow's WindowVisualState is true.");
			Assert.AreEqual (WindowVisualState.Normal, bansheeWindow.WindowVisualState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//208.2 Maximize the Banshee window
			procedureLogger.Action ("Check the if bansheeWindow can be maximized.");
			procedureLogger.ExpectedResult ("The bansheeWindow can be maximized.");
			Assert.AreEqual (true, bansheeWindow.CanMaximize);
			Thread.Sleep (Config.Instance.ShortDelay);

			bansheeWindow.SetWindowVisualState (WindowVisualState.Maximized);
			procedureLogger.ExpectedResult ("The bansheeWindow is Maximized.");
			Assert.AreEqual (WindowVisualState.Maximized, bansheeWindow.WindowVisualState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//208.3 Minimize the Banshee window
			procedureLogger.Action ("Check the bansheeWindow can be minimized.");
			procedureLogger.ExpectedResult ("The bansheeWindow can be minimized.");
			Assert.AreEqual (true, bansheeWindow.CanMinimize);
			Thread.Sleep (Config.Instance.ShortDelay);

			bansheeWindow.SetWindowVisualState (WindowVisualState.Minimized);
			procedureLogger.ExpectedResult ("The bansheeWindow is Minimized");
			Assert.AreEqual (WindowVisualState.Minimized, bansheeWindow.WindowVisualState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//208.4 Make the Banshee window to normal
			bansheeWindow.SetWindowVisualState (WindowVisualState.Normal);
			procedureLogger.ExpectedResult ("Make the bansheeWindow to normal.");
			Assert.AreEqual (WindowVisualState.Normal, bansheeWindow.WindowVisualState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//208.5 Check Banshee window's TransformPattern's property
			procedureLogger.Action ("Check CanMove.");
			procedureLogger.ExpectedResult ("The value of CanMove is true.");
			Assert.AreEqual (true, bansheeWindow.CanMove);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check CanSize.");
			procedureLogger.ExpectedResult ("The value of CanSize is false.");
			Assert.AreEqual (false, bansheeWindow.CanResize);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check CanRotate.");
			procedureLogger.ExpectedResult ("The value of CanRotate is false.");
			Assert.AreEqual (false, bansheeWindow.CanRotate);
			Thread.Sleep (Config.Instance.ShortDelay);

			//208.6 Move the Banshee Window to (400,400)
			bansheeWindow.Move (200, 200);
			procedureLogger.ExpectedResult ("Move \"Add Entry\" dialog to coordinates(200, 200 ).");

			Thread.Sleep (Config.Instance.ShortDelay);
		}

		//TestCase209 Init Sample, create a new account
		[Test]
		public void RunTestCase209 ()
		{
			Run (TestCase209);
		}

		private void TestCase209 ()
		{
			//209.1 Click "Media" Menu Item
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<Menu> ("Media").Click ();
			procedureLogger.ExpectedResult ("The \"Media\" sub menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//209.2 Select "Import Playlist" Menu Item
			var importPlaylistMenuItem = menuBar.Find<MenuItem> ("Import Playlist");
			importPlaylistMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Import Playlist\" dialog shows.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//209.3 Input "/usr/lib" in the "Location" text
			var importPlaylistDialog = window.Find<Window> ("Import Playlist");
			var locationEdit = importPlaylistDialog.Find<Edit> ("Location");
			locationEdit.SetValue ("/usr");
			procedureLogger.ExpectedResult ("the \"/user\" is entered in the \"Location\" Edit.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//209.4 Check the count of columns and rows of Datagrid
			var dataGrid = importPlaylistDialog.Find<DataGrid> ();
			procedureLogger.Action ("Check the count of columns of datagrid.");
			procedureLogger.ExpectedResult ("The count of columns of datagrid is 2.");
			Assert.AreEqual (2, dataGrid.ColumnCount);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the count of rows of datagrid.");
			procedureLogger.ExpectedResult ("The count of rows of datagrid is 2.");
			Assert.AreEqual ("11", dataGrid.RowCount);
			Thread.Sleep (Config.Instance.ShortDelay);

			//209.5 Get the (0,0) element of the Datagrid , check if it is "bin"
			var sampleText = dataGrid.Find<Edit> ("bin").AutomationElement;
			var entryText = dataGrid.GetItem (0, 0);
			procedureLogger.ExpectedResult ("The (0,0) item of the datagrid is \"bin\".");
			Assert.AreEqual (sampleText, entryText);
			Thread.Sleep (Config.Instance.ShortDelay);

			//209.6 Set current view of Datagrid
			dataGrid.SetCurrentView (0);
			procedureLogger.ExpectedResult ("The current view of dataGrid is 0.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//209.7 Check current view of Datagrid
			procedureLogger.Action ("Check the CurrentView of dataGrid.");
			procedureLogger.ExpectedResult ("The value of CurrentView property is 0.");
			Assert.AreEqual (0, dataGrid.CurrentView);
			Thread.Sleep (Config.Instance.ShortDelay);

			//209.8 Check the view name of Datagrid.
			var viewName = dataGrid.GetViewName (0);
			procedureLogger.ExpectedResult ("The current view name is \"Icons\".");
			Assert.AreEqual ("Icons", viewName);
			Thread.Sleep (Config.Instance.ShortDelay);

			//209.9 Check Column, ColumnSpan, Row, RowSpan ContainingGrid of each data Item
			var dataItems = dataGrid.FindAll<DataItem> ();
			for (int i = 0; i < dataItems.Length; i++) {
				procedureLogger.Action (string.Format ("Check Column of {0}.", dataItems[i].NameAndType));
				procedureLogger.ExpectedResult (string.Format ("The Column of {0} is 0.", dataItems[i].NameAndType));
				Assert.AreEqual (0, dataItems[i].Column);
				Thread.Sleep (Config.Instance.ShortDelay);

				procedureLogger.Action (string.Format ("Check ColumnSpan of {0}.", dataItems[i].NameAndType));
				procedureLogger.ExpectedResult (string.Format ("The ColumnSpan of {0} is 1.", dataItems[i].NameAndType));
				Assert.AreEqual (1, dataItems[i].ColumnSpan);
				Thread.Sleep (Config.Instance.ShortDelay);

				procedureLogger.Action (string.Format ("Check Row of {0}.", dataItems[i].NameAndType));
				procedureLogger.ExpectedResult (string.Format ("The Row of {0} is {1}.", dataItems[i].NameAndType, i.ToString ()));
				Assert.AreEqual (i, dataItems[i].Row);
				Thread.Sleep (Config.Instance.ShortDelay);

				procedureLogger.Action (string.Format ("Check RowSpan of {0}.", dataItems[i].NameAndType));
				procedureLogger.ExpectedResult (string.Format ("The RowSpan of {0} is 1.", dataItems[i].NameAndType));
				Assert.AreEqual (1, dataItems[i].RowSpan);
				Thread.Sleep (Config.Instance.ShortDelay);

				procedureLogger.Action (string.Format ("Check ContainingGrid of {0}.", dataItems[i].NameAndType));
				procedureLogger.ExpectedResult (string.Format ("The ContainingGrid of {0} is the AutomationElement of its parent.", dataItems[i].NameAndType));
				Assert.AreEqual (dataGrid.AutomationElement, dataItems[i].ContainingGrid);
				Thread.Sleep (Config.Instance.ShortDelay);
			}

			//209.10 Check ColumnHeaders of Datagrid
			Assert.AreEqual (dataGrid.ColumnHeaders, dataGrid.GetColumnHeaders ());
			procedureLogger.ExpectedResult (string.Format ("The ColumnHeaders is {0}.", dataGrid.ColumnHeaders));
			Thread.Sleep (Config.Instance.ShortDelay);

			//209.11 Check RowHeaders of Datagrid
			Assert.AreEqual (0, dataGrid.GetRowHeaders ().Length);
			procedureLogger.ExpectedResult ("The count of RowHeaders is 0.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//209.12 Check RowOrColumnMajor of Datagrid
			procedureLogger.Action ("Check RowOrColumnMajor.");
			procedureLogger.ExpectedResult ("The RowOrColumnMajor is \"RowMajor\".");
			Assert.AreEqual (RowOrColumnMajor.RowMajor, dataGrid.RowOrColumnMajor);
			Thread.Sleep (Config.Instance.ShortDelay);

			//209.13 Check the first item in datagrid's about its TableItemPattern
			var firstText = dataGrid.Find<Edit> ("bin");
			procedureLogger.Action ("Check Column.");
			procedureLogger.ExpectedResult ("The value of Colum is 0.");
			Assert.AreEqual (0, firstText.Column);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check ColumnSpan.");
			procedureLogger.ExpectedResult ("The value of ColumnSpan 1.");
			Assert.AreEqual (1, firstText.ColumnSpan);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check Row.");
			procedureLogger.ExpectedResult ("The value of Row is 0.");
			Assert.AreEqual (0, firstText.Row);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check RowSpan.");
			procedureLogger.ExpectedResult ("The value of RowSpan is 1.");
			Assert.AreEqual (1, firstText.RowSpan);
			Thread.Sleep (Config.Instance.ShortDelay);

			AutomationElement dataGridItem = dataGrid.GetItem (0, 0);
			AutomationElement firsttextItem = window.Find<Edit> ("bin").AutomationElement;
			procedureLogger.Action ("Check ContainingGrid.");
			procedureLogger.ExpectedResult ("The value of ContainingGrid is the AutomationElement of its parent.");
			Assert.AreEqual (dataGridItem, firsttextItem);
			Thread.Sleep (Config.Instance.ShortDelay);

			//209.14 Close the "Import Playlist" dialog
			importPlaylistDialog.Close ();
			procedureLogger.ExpectedResult ("The \"Import Playlist\" window is closed");
			Thread.Sleep (Config.Instance.ShortDelay);
		}

		//TestCase210 Init Sample, create a new account
		[Test]
		public void RunTestCase210 ()
		{
			Run (TestCase210);
		}

		private void TestCase210 ()
		{
			//210.1 Click "Edit" Menu Item
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<Menu> ("Edit").Click ();
			procedureLogger.ExpectedResult ("The \"Edit\" sub menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//210.2 Select "Preferences" Menu Item
			var importMenuItem = menuBar.Find<MenuItem> ("Preferences");
			importMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Import\" dialog shows.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//210.3 Select "Extensions" tab
			var preferencesDialog = window.Find<Window> ("Extensions");
			var extensionsTabItem = preferencesDialog.Find<TabItem> ();
			extensionsTabItem.Select ();
			procedureLogger.ExpectedResult ("The \"Extensions\" tab shows.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//210.4 Check HorizontallyScrollable and VerticallyScrollable of Datagrid
			var dataGrid = extensionsTabItem.Find<DataGrid> ();
			procedureLogger.Action ("Check the datagrid's VerticallyScrollable.");
			procedureLogger.ExpectedResult ("The value of VerticallyScrollable is true.");
			Assert.AreEqual (true, dataGrid.VerticallyScrollable);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the datagrid's HorizontallyScrollable.");
			procedureLogger.ExpectedResult ("The value of HorizontallyScrollable is false.");
			Assert.AreEqual (false, dataGrid.HorizontallyScrollable);
			Thread.Sleep (Config.Instance.ShortDelay);

			//210.5 Set the scroll bar horizontal percent to 0
			dataGrid.SetScrollPercent (0, -1);
			procedureLogger.ExpectedResult ("The horizontal percentage of scroll bar is set to 0.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//210.6 Check HorizontalScrollPercent and VerticalScrollPercent
			procedureLogger.Action ("Check the datagrid's HorizontalScrollPercent.");
			procedureLogger.ExpectedResult ("The value of HorizontalScrollPercent is 0.");
			Assert.AreEqual (0, dataGrid.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the datagrid's VerticalScrollPercent.");
			procedureLogger.ExpectedResult ("The value of VerticalScrollPercent is -1.");
			Assert.AreEqual (-1, dataGrid.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//210.7 Set the vertical scroll bar large increment
			dataGrid.Scroll (ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
			procedureLogger.ExpectedResult ("The vertical scroll bar scroll large increment.");
			Assert.AreEqual (24, dataGrid.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//210.8 Set the vertical scroll bar large decrement
			dataGrid.Scroll (ScrollAmount.NoAmount, ScrollAmount.LargeDecrement);
			procedureLogger.ExpectedResult ("The vertical scroll bar scroll large decrement.");
			Assert.AreEqual (0, dataGrid.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//210.9 Set the vertical scroll bar small increment
			dataGrid.ScrollVertical (ScrollAmount.SmallIncrement);
			procedureLogger.ExpectedResult ("The vertical scroll bar scroll small increment.");
			Assert.AreEqual ("", dataGrid.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//210.10 Set the vertical scroll bar small decrement
			dataGrid.ScrollVertical (ScrollAmount.SmallDecrement);
			procedureLogger.ExpectedResult ("The vertical scroll bar scroll small decrement.");
			Assert.AreEqual (0, dataGrid.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//210.11 Check HorizontalViewSize of Datagrid
			procedureLogger.Action ("Check the datagrid's HorizontalViewSize.");
			procedureLogger.ExpectedResult ("The value of HorizontalViewSize is.");
			Assert.AreEqual ("", dataGrid.HorizontalViewSize);
			Thread.Sleep (Config.Instance.MediumDelay);

			//210.12 Check VerticalViewSize of Datagrid
			procedureLogger.Action ("Check the datagrid's VerticalViewSize.");
			procedureLogger.ExpectedResult ("The value of VerticalViewSize is 100.");
			Assert.AreEqual (100, dataGrid.VerticalViewSize);
			Thread.Sleep (Config.Instance.MediumDelay);

			//210.13 Close the "Import Playlist" dialog
			preferencesDialog.Close ();
			procedureLogger.ExpectedResult ("The \"Import Playlist\" window is closed");
			Thread.Sleep (Config.Instance.ShortDelay);
		}
	}
}
