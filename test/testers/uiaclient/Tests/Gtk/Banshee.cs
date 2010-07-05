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
//	Felicia Mu <fxmu@novell.com>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Automation;
using NUnit.Framework;
using Mono.UIAutomation.TestFramework;
using SWF = System.Windows.Forms;

namespace MonoTests.Mono.UIAutomation.UIAClientAPI.Gtk
{
	[TestFixture]
	public class Banshee : TestBase
	{
		Window window = null;
		Application app = null;

		protected override void LaunchSample ()
		{
			app = new Application ("Banshee");
			app.Launch ("banshee-1");
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

		//TestCase213 Banshee Media Player dialog
		[Test]
		public void RunTestCase213 ()
		{
			Run (TestCase213);
		}

		private void TestCase213 ()
		{
			//213.1 Check the properties of WindowPattern for banshee window
			//BUG595149 WindowPattern is not finished?
			/*
			procedureLogger.Action ("Check IsModal.");
			procedureLogger.ExpectedResult ("The value of Ismodal is false.");
			Assert.IsFalse (window.IsModal);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check IsTopmost.");
			procedureLogger.ExpectedResult ("The value of IsTopmost is false.");
			Assert.IsFalse (window.IsTopmost);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check WindowInteractionState."); 
			procedureLogger.ExpectedResult ("The value of WindowInteractionState is ReadyForUserInteraction.");
			Assert.AreEqual (WindowInteractionState.ReadyForUserInteraction, window.WindowInteractionState);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check WindowVisualState.");
			procedureLogger.ExpectedResult ("The value of WindowVisualState is Normal.");
			Assert.AreEqual (WindowVisualState.Normal, window.WindowVisualState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//213.2 Maximize the banshee window
			procedureLogger.Action ("Check CanMaximize.");
			procedureLogger.ExpectedResult ("The value of CanMaximize is true.");
			Assert.IsTrue (window.CanMaximize);
			Thread.Sleep (Config.Instance.ShortDelay);

			window.SetWindowVisualState (WindowVisualState.Maximized);
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("The window is maximized.");
			Assert.AreEqual (WindowVisualState.Maximized, window.WindowVisualState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//213.3 Minimize the banshee window
			procedureLogger.Action ("Check CanMinimize.");
			procedureLogger.ExpectedResult ("The value of CanMinimize is true.");
			Assert.IsTrue (window.CanMinimize);
			Thread.Sleep (Config.Instance.ShortDelay);

			window.SetWindowVisualState (WindowVisualState.Minimized);
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("The window is minimized");
			Assert.AreEqual (WindowVisualState.Minimized, window.WindowVisualState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//213.4 Restore the banshee window to normal
			window.SetWindowVisualState (WindowVisualState.Normal);
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("Restore the banshee window to normal.");
			Assert.AreEqual (WindowVisualState.Normal, window.WindowVisualState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//213.5 Check the properties of TransformPattern for banshee window
			procedureLogger.Action ("Check CanMove.");
			procedureLogger.ExpectedResult ("The value of CanMove is true.");
			Assert.IsTrue (window.CanMove);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check CanReSize.");
			procedureLogger.ExpectedResult ("The value of CanReSize is true.");
			Assert.IsTrue (window.CanResize);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check CanRotate.");
			procedureLogger.ExpectedResult ("The value of CanRotate is true.");
			Assert.IsTrue (window.CanRotate);
			Thread.Sleep (Config.Instance.ShortDelay);

			//213.6 Move the banshee window to (200,200)
			window.Move (200, 200);
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("Move \"Add Entry\" dialog to coordinates(200, 200 ).");
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			//213.7 Check the "Repeat Off" button's pattern
			/*
			 * BUG608190 - [uiaclient-GTKs]:The Toggle button should not support Invoke pattern
			 */
			/*
			var repeatButton = window.Find<Button> ("Repeat Off");
			SWF.Button buttonControl = new SWF.Button ();
			AutomationElement buttonAe = AutomationElement.FromHandle (buttonControl.Handle);
			AutomationPattern[] addPatterns = {InvokePattern.Pattern};
			helper.PatternChcek (buttonControl, buttonAe, addPatterns, null);
			*/
		}

		//TestCase214 Import Playlist
		[Test]
		public void RunTestCase214 ()
		{
			Run (TestCase214);
		}

		private void TestCase214 ()
		{
			//214.1 Click "Media" Menu Item
			var menuBar = window.Find<MenuBar> ();
			// here is MenuItem on Windows, but is Menu on Linux
			var mediaMenuItem = menuBar.Find<MenuItem> ("Media");
			mediaMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Media\" sub menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//214.2 Select "Import Playlist" Menu Item
			var importPlaylistMenuItem = mediaMenuItem.Find<MenuItem> ("Import Playlist...");
			importPlaylistMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Import Playlist\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//214.3 Input "/usr" in the "Location" text
			//BUG593973: GTK sub window can not be refreshed
			var importPlaylistDialog = app.FindGtkSubWindow (window, "Import Playlist");
			//BUG595149 WindowPattern is not finished?
			//procedureLogger.Action ("Check IsModal of the dialog.");
			//procedureLogger.ExpectedResult ("The value of Ismodal is true.");
			//Assert.IsTrue (importPlaylistDialog.IsModal);
			//Thread.Sleep (Config.Instance.ShortDelay);

			var locationEdit = importPlaylistDialog.Find<Edit> ();
			locationEdit.SetValue ("/usr");
			procedureLogger.ExpectedResult ("The \"/usr\" is entered in the \"Location\" edit.");
			Assert.AreEqual("/usr", locationEdit.Value);
			Thread.Sleep (Config.Instance.ShortDelay);

			//214.4 Check the count of columns and rows of Datagrid
			var dataGrid = importPlaylistDialog.Find<DataGrid> ();
			procedureLogger.Action ("Check the count of columns of datagrid.");
			procedureLogger.ExpectedResult ("The count of columns of datagrid is 2.");
			//Assert.AreEqual (3, dataGrid.ColumnCount);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the count of rows of datagrid.");
			procedureLogger.ExpectedResult ("The count of rows of datagrid is 2.");
			Assert.AreEqual ("11", dataGrid.RowCount);
			Thread.Sleep (Config.Instance.ShortDelay);

			//214.5 Get the (0,0) element of the Datagrid , check if it is "bin"
			var sampleText = dataGrid.Find<Edit> ("bin").AutomationElement;
			var entryText = dataGrid.GetItem (0, 0);
			procedureLogger.ExpectedResult ("The (0,0) item of the datagrid is \"bin\".");
			Assert.AreEqual (sampleText, entryText);
			Thread.Sleep (Config.Instance.ShortDelay);

			//214.6 Set current view of Datagrid
			dataGrid.SetCurrentView (1);
			procedureLogger.ExpectedResult ("The current view of dataGrid is 0.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//214.7 Check current view of Datagrid
			procedureLogger.Action ("Check the CurrentView of dataGrid.");
			procedureLogger.ExpectedResult ("The value of CurrentView property is 0.");
			Assert.AreEqual (1, dataGrid.CurrentView);
			Thread.Sleep (Config.Instance.ShortDelay);

			//214.8 Check the view name of Datagrid.
			var viewName = dataGrid.GetViewName (1);
			procedureLogger.ExpectedResult ("The current view name is \"Icons\".");
			Assert.AreEqual ("Details", viewName);
			Thread.Sleep (Config.Instance.ShortDelay);

			//214.9 Check Column, ColumnSpan, Row, RowSpan ContainingGrid of each data Item
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

			//214.10 Check ColumnHeaders of Datagrid
			procedureLogger.Action ("Check ColumnHeaders.");
			procedureLogger.ExpectedResult (string.Format ("The ColumnHeaders is {0}.", dataGrid.ColumnHeaders));
			Assert.AreEqual (dataGrid.ColumnHeaders, dataGrid.GetColumnHeaders ());
			Thread.Sleep (Config.Instance.ShortDelay);

			//214.11 Check RowHeaders of Datagrid
			procedureLogger.Action ("Check RowHeaders.");
			procedureLogger.ExpectedResult ("The count of RowHeaders is 0.");
			Assert.AreEqual (0, dataGrid.GetRowHeaders ().Length);
			Thread.Sleep (Config.Instance.ShortDelay);

			//214.12 Check RowOrColumnMajor of Datagrid
			procedureLogger.Action ("Check RowOrColumnMajor.");
			procedureLogger.ExpectedResult ("The RowOrColumnMajor is \"RowMajor\".");
			Assert.AreEqual (RowOrColumnMajor.RowMajor, dataGrid.RowOrColumnMajor);
			Thread.Sleep (Config.Instance.ShortDelay);

			//214.13 Check the first item in datagrid's about its TableItemPattern
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
			procedureLogger.Action ("Check the (0,0) item of the datagrid.");
			procedureLogger.ExpectedResult ("The (0,0) item of the datagrid is \"bin\".");
			Assert.AreEqual (dataGridItem, firsttextItem);
			Thread.Sleep (Config.Instance.ShortDelay);

			//214.14 Close the "Import Playlist" dialog
			importPlaylistDialog.Close ();
			procedureLogger.ExpectedResult ("The \"Import Playlist\" window is closed");
			Thread.Sleep (Config.Instance.ShortDelay);
		}

		//TestCase215 Init Sample, create a new account
		[Test]
		public void RunTestCase215 ()
		{
			Run (TestCase215);
		}

		private void TestCase215 ()
		{
			//215.1 Click "Edit" Menu Item
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<MenuItem> ("Edit").Click ();
			procedureLogger.ExpectedResult ("The \"Edit\" sub menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//215.2 Select "Preferences" Menu Item
			menuBar.Find<MenuItem> ("Preferences").Click ();
			procedureLogger.ExpectedResult ("The \"Preferences\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//215.3 Select "Extensions" tab
			var preferencesDialog = app.FindGtkSubWindow (window, "Preferences");
			var extensionsTabItem = preferencesDialog.Find<TabItem> ("Extensions");
			extensionsTabItem.Select ();
			procedureLogger.ExpectedResult ("The \"Extensions\" tab item is displayed.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//215.4 Check HorizontallyScrollable and VerticallyScrollable of Datagrid
			//BUG595158 Scroll Pane without ScrollPattern support 
			var dataGrid = extensionsTabItem.Find<DataGrid> ();
			procedureLogger.Action ("Check the datagrid's VerticallyScrollable.");
			procedureLogger.ExpectedResult ("The value of VerticallyScrollable is true.");
			Assert.AreEqual (true, dataGrid.VerticallyScrollable);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the datagrid's HorizontallyScrollable.");
			procedureLogger.ExpectedResult ("The value of HorizontallyScrollable is false.");
			Assert.AreEqual (false, dataGrid.HorizontallyScrollable);
			Thread.Sleep (Config.Instance.ShortDelay);

			//215.5 Set the scroll bar horizontal percent to 0
			dataGrid.SetScrollPercent (0, -1);
			procedureLogger.ExpectedResult ("The horizontal percentage of scroll bar is set to 0.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//215.6 Check HorizontalScrollPercent and VerticalScrollPercent
			procedureLogger.Action ("Check the datagrid's HorizontalScrollPercent.");
			procedureLogger.ExpectedResult ("The value of HorizontalScrollPercent is 0.");
			Assert.AreEqual (0, dataGrid.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the datagrid's VerticalScrollPercent.");
			procedureLogger.ExpectedResult ("The value of VerticalScrollPercent is -1.");
			Assert.AreEqual (-1, dataGrid.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//215.7 Set the vertical scroll bar large increment
			dataGrid.Scroll (ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
			procedureLogger.ExpectedResult ("The vertical scroll bar scroll large increment.");
			Assert.AreEqual (24, dataGrid.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//215.8 Set the vertical scroll bar large decrement
			dataGrid.Scroll (ScrollAmount.NoAmount, ScrollAmount.LargeDecrement);
			procedureLogger.ExpectedResult ("The vertical scroll bar scroll large decrement.");
			Assert.AreEqual (0, dataGrid.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//215.9 Set the vertical scroll bar small increment
			dataGrid.ScrollVertical (ScrollAmount.SmallIncrement);
			procedureLogger.ExpectedResult ("The vertical scroll bar scroll small increment.");
			Assert.AreEqual ("", dataGrid.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//215.10 Set the vertical scroll bar small decrement
			dataGrid.ScrollVertical (ScrollAmount.SmallDecrement);
			procedureLogger.ExpectedResult ("The vertical scroll bar scroll small decrement.");
			Assert.AreEqual (0, dataGrid.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//215.11 Check HorizontalViewSize of Datagrid
			procedureLogger.Action ("Check the datagrid's HorizontalViewSize.");
			procedureLogger.ExpectedResult ("The value of HorizontalViewSize is.");
			Assert.AreEqual ("", dataGrid.HorizontalViewSize);
			Thread.Sleep (Config.Instance.MediumDelay);

			//215.12 Check VerticalViewSize of Datagrid
			procedureLogger.Action ("Check the datagrid's VerticalViewSize.");
			procedureLogger.ExpectedResult ("The value of VerticalViewSize is 100.");
			Assert.AreEqual (100, dataGrid.VerticalViewSize);
			Thread.Sleep (Config.Instance.MediumDelay);

			//215.13 Close the "Import Playlist" dialog
			preferencesDialog.Close ();
			procedureLogger.ExpectedResult ("The \"Import Playlist\" window is closed");
			Thread.Sleep (Config.Instance.ShortDelay);
		}
		
		//TestCase216 New Smart Playlist
		[Test]
		public void RunTestCase216 ()
		{
			Run (TestCase216);
		}

		private void TestCase216 ()
		{
			//216.1 Click "Media" Menu Item
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<MenuItem> ("Media").Click ();
			procedureLogger.ExpectedResult ("The \"Media\" sub menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//216.2 Select "New Smart Playlist..." Menu Item
			menuBar.Find<MenuItem> ("New Smart Playlist...").Click ();
			procedureLogger.ExpectedResult ("The \"New Smart Playlist\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//216.3 Click "Predefined Smart Playlists" button
			var playlistDialog = window.Find<Window> ("New Smart Playlist");
			playlistDialog.Find<Button> ("Predefined Smart Playlists").Click ();
			procedureLogger.ExpectedResult ("A tree appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//216.4 Check the patterns of the tree control which has a scroll in it 
			var tree = playlistDialog.Find<Tree> ();
			
			/*
			 * BUG608200 - [uiaclient-GTKs]:The Tree who has a scrollbar should support ScrollPattern
			 */
			SWF.TreeView treeControl = new SWF.TreeView ();
			AutomationElement treeAe = AutomationElement.FromHandle (treeControl.Handle);
			AutomationPattern[] addPatterns = {SelectionPattern.Pattern, ScrollPattern.Pattern};
			helper.PatternChcek (tree, treeAe, addPatterns, null);
			
			//216.5 Close the window
			var cancelButton = playlistDialog.Find<Button> ("Cancel");
			cancelButton.Click ();
			procedureLogger.ExpectedResult ("The \"New Smart Playlist\" dialog disappears.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
		}
	}
}