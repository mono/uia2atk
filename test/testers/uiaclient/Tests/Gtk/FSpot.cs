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
//	Felicia Mu <fxmu@novell.com>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Mono.UIAutomation.TestFramework;
using NUnit.Framework;
using System.Windows.Automation;
using SWF = System.Windows.Forms;


namespace MonoTests.Mono.UIAutomation.UIAClientAPI.Gtk
{
	[TestFixture]
	public class FSpot : TestBase
	{
		Window window = null;
		Application app = null;

		protected override void LaunchSample ()
		{
			// Log the filename.
			app = new Application ("GtkFspotTests");
			app.Launch ("f-spot");
		}

		protected override void OnSetup ()
		{
			base.OnSetup ();
			window = app.GetWindow ("F-Spot");
		}

		protected override void OnQuit ()
		{
			base.OnQuit ();
			window.Close ();
		}

		protected void ImportPictures ()
		{
			//Click "photo" menu
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<Menu> ("Photo").Click ();
			Thread.Sleep (Config.Instance.ShortDelay);

			//Select the "Import" menu item.
			var importMenuItem = menuBar.Find<MenuItem> ("Import...");
			importMenuItem.Click ();
			Thread.Sleep (Config.Instance.ShortDelay);

			//Select the "Select Folder" menu item in its combo box.
			var importSourceCombobox = window.Find<ComboBox> ("Select Folder");
			importSourceCombobox.Find<MenuItem> ("Select Folder").Click ();
			Thread.Sleep (Config.Instance.ShortDelay);

			var importDialog = window.Find<Window> ("Import");
			var locationEdit = importDialog.Find<Edit> ("Location:");
			locationEdit.SetValue ("/usr/share/pixmaps/");
			Thread.Sleep (Config.Instance.ShortDelay);

			//Click "Open" Button
			var openButton = importDialog.Find<Button> ("Open");
			openButton.Click ();
			Thread.Sleep (Config.Instance.LongDelay);

			//var list = importDialog.Find<List> ();
			//var listItem = list.Find<ListItem> ();
			//Thread.Sleep (Config.Instance.ShortDelay);

			//Click "Import" Button
			var importButton = importDialog.Find<Button> ("Import");
			importButton.Click ();
			Thread.Sleep (Config.Instance.ShortDelay);

			//var fspotList = window.Find<List> ();
			//var fspotListItem = fspotList.Find<ListItem> ();
			//Thread.Sleep (Config.Instance.ShortDelay);
		}

		//TestCase201 Import the pictures into the F-Spot Editor
		[Test]
		public void RunTestCase201 ()
		{
			Run (TestCase201);
		}

		private void TestCase201 ()
		{
			//201.1 Click "photo" menu
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<Menu> ("Photo").Click ();
			procedureLogger.ExpectedResult ("The \"Photo\" sub menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//201.2 Select the "Import" menu item.
			var importMenuItem = menuBar.Find<MenuItem> ("Import...");
			importMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Import\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//201.3 Select the "Select Folder" menu item in its combo box.
			var importSourceCombobox = window.Find<ComboBox> ("Select Folder");
			importSourceCombobox.Find<MenuItem> ("Select Folder").Click ();
			Thread.Sleep (Config.Instance.ShortDelay);

			var importDialog = window.Find<Window> ("Import");
			procedureLogger.Action ("Find the \"Import\" dialog.");
			procedureLogger.ExpectedResult ("The \"Import\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//201.4 Input text "/usr/share/pixmaps/" into "Location:" Edit
			var locationEdit = importDialog.Find<Edit> ("Location:");
			locationEdit.SetValue ("/usr/share/pixmaps/");
			procedureLogger.ExpectedResult ("\"/usr/share/pixmaps\" has been entered.");
			Thread.Sleep (Config.Instance.ShortDelay);
			Assert.AreEqual ("/usr/share/pixmaps/", locationEdit.Value);
			Thread.Sleep (Config.Instance.ShortDelay);

			//201.5 Click "Open" Button
			var openButton = importDialog.Find<Button> ("Open");
			openButton.Click ();
			Thread.Sleep (Config.Instance.LongDelay);

			var list = importDialog.Find<List> ();
			var listItem = list.Find<ListItem> ();
			procedureLogger.ExpectedResult ("The picture(s) in \"/usr/share/pixmaps/\" is(are) loaded.");
			Thread.Sleep (Config.Instance.ShortDelay);
			Assert.IsNotNull (listItem);

			//201.6 Click "Import" Button
			var importButton = importDialog.Find<Button> ("Import");
			importButton.Click ();
			Thread.Sleep (Config.Instance.ShortDelay);

			var fspotList = window.Find<List> ();
			Thread.Sleep (Config.Instance.ShortDelay);
			var fspotListItem = fspotList.Find<ListItem> ();
			procedureLogger.ExpectedResult ("The pictures in \"/usr/share/pixmaps/\" are imported.");
			Thread.Sleep (Config.Instance.ShortDelay);
			Assert.IsNotNull (fspotListItem);
		}

		//TestCase202 Find a pic Item
		[Test]
		public void RunTestCase202 ()
		{
			Run (TestCase202);
		}

		private void TestCase202 ()
		{
			//Do the action to load the list items.
			ImportPictures ();

			//202.1 Check the List's SelectionPattern's property
			var List = window.Find<List> ();
			procedureLogger.Action ("Check the List Can be Selected Multiple.");
			procedureLogger.ExpectedResult ("The List can select multiple.");
			Assert.IsTrue (List.CanSelectMultiple);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the List Is Selection Required.");
			procedureLogger.ExpectedResult ("The List requires selection .");
			Assert.AreEqual (false, List.IsSelectionRequired);
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.2 Select the List's first icon
			var firstListItem = window.FindAll<ListItem> () [0];
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("The first icon is selected.");
			Assert.AreEqual (firstListItem.Name, List.GetSelection () [0].Current.Name);
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.3 Make the fifty-third icon in List visible
			var fiftyThirdListItem = window.FindAll<ListItem> () [53];
			fiftyThirdListItem.ScrollIntoView ();
			procedureLogger.ExpectedResult ("The fifty-third icon is shown.");
			Assert.AreEqual (false, fiftyThirdListItem.IsOffscreen);
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.4 Select and Unselect the fifty-third icon in List
			fiftyThirdListItem.Select ();
			procedureLogger.ExpectedResult ("The fifty-third icon is selected.");
			Assert.IsTrue (fiftyThirdListItem.IsSelected);
			Thread.Sleep (Config.Instance.ShortDelay);

			fiftyThirdListItem.RemoveFromSelection ();
			procedureLogger.ExpectedResult ("The fifty-third icon is unselected.");
			Assert.AreEqual (false, fiftyThirdListItem.IsSelected);
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.5 Click "Edit Image" menu item on the toolbar
			var editButton = window.Find<Button> ("Edit Image");
			editButton.Click ();
			procedureLogger.ExpectedResult ("Only the selected icon is stayed on the window.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.6 Maximum the picture
			var slider = window.Find<Slider> ();
			slider.SetValue (slider.Maximum);
			procedureLogger.ExpectedResult ("The picture is maximumed.");
			Assert.AreEqual (1.0, slider.Maximum);
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.7 Minimize the picture
			slider.SetValue (slider.Minimum);
			procedureLogger.ExpectedResult ("The picture is minimized.");
			Assert.AreEqual (0, slider.Minimum);
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.8 Give the picture a small enlarge
			slider.SetValue (slider.Minimum + slider.SmallChange);
			procedureLogger.ExpectedResult ("Make the picture a small change large.");
			Assert.AreEqual (slider.SmallChange - slider.Minimum, slider.Value);
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.9 Make the picture a big reduce
			slider.SetValue (slider.Maximum - slider.LargeChange);
			procedureLogger.ExpectedResult ("Make the picture a big change large.");
			Assert.AreEqual (slider.LargeChange, 1 - slider.Value);
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.10 Give a comment "the last one" to the picture
			var edit = window.Find<Edit> ("Comment:");
			edit.SetValue ("the last one");
			procedureLogger.ExpectedResult ("The \"the last one\" is inputed into the edit.");
			Assert.AreEqual ("the last one", edit.Value);
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.11 Close the F-Spot window
			window.Close ();
			procedureLogger.ExpectedResult ("The F-Spot window is closed.");
			Thread.Sleep (Config.Instance.ShortDelay);
		}

		//TestCase203 Adjust the time of picture
		[Test]
		public void RunTestCase203 ()
		{
			Run (TestCase203);
		}

		private void TestCase203 ()
		{
			//Do the action to load the list items.
			ImportPictures ();

			//203.1 Select the third icon
			var thirdItem = window.FindAll<ListItem> () [3];
			thirdItem.Select ();
			procedureLogger.ExpectedResult ("The third icon is selected.");
			Assert.IsTrue (thirdItem.IsSelected);
			Thread.Sleep (Config.Instance.ShortDelay);

			//203.2 Select the "Edit" Menu
			var editMenuItem = window.Find<MenuItem> ("Edit");
			editMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Edit\" sub menu is poped out.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//203.3 Select the "Adjust Time" Menu Item
			var adjustTimeMenuItem = editMenuItem.Find<MenuItem> ("Adjust Time...");
			adjustTimeMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Adjust Time\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//203.4 Click the "Calendar" button
			var adjustTimeDialog = window.Find<Window> ("Adjust Time");
			adjustTimeDialog.Find<Button> ("Select date").Click ();
			procedureLogger.ExpectedResult ("The \"Calendar\" is shown.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//203.5 Select "10" Day of the Calendar
			adjustTimeDialog.Find<Button> ("10").Click ();
			procedureLogger.ExpectedResult ("The \"10\" day is selected.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//203.6 Expand the "Select Time" combobox
			var selectTiemCombox = adjustTimeDialog.Find<ComboBox> ("Select Time");
			selectTiemCombox.Expand ();
			procedureLogger.ExpectedResult ("The \"Select Time\" combobox is expanded.");
			Assert.AreEqual (ExpandCollapseState.Expanded, selectTiemCombox.ExpandCollapseState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//203.7 Select the "08:30" list Item
			var timeListItem = selectTiemCombox.Find<ListItem> ("08:30");
			timeListItem.Select ();
			procedureLogger.ExpectedResult ("The \"08:30\" listitem is selected.");
			Assert.IsTrue (timeListItem.IsSelected);
			Thread.Sleep (Config.Instance.ShortDelay);

			//203.8 Close the "Adjust Time" window
			adjustTimeDialog.Close ();
			procedureLogger.ExpectedResult ("The \"Adjust Time\" window is closed.");
			Thread.Sleep (Config.Instance.ShortDelay);
		}

		//TestCase204 Screensaver Configuration
		[Test]
		public void RunTestCase204 ()
		{
			Run (TestCase204);
		}

		private void TestCase204 ()
		{
			//204.1 Select "Tools" Menu Item
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<MenuItem> ("Tools").Click ();
			procedureLogger.ExpectedResult ("The \"Tools\" sub menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//204.2 Select "Configure Screensaver" Menu Item
			var configureMenuItem = menuBar.Find<MenuItem> ("Configure Screensaver");
			configureMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Configure Screensaver\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//204.3 Select "All Image" radio button on the "Screensaver Configure" dialog
			var configureDialog = window.Find<Window> ("Screensaver Configuration");
			var allImageRadioButton = configureDialog.Find<RadioButton> ("All Images");
			allImageRadioButton.Select ();
			procedureLogger.ExpectedResult ("The \"All Images\" radio button is selected.");
			Assert.IsTrue (allImageRadioButton.IsSelected);
			Thread.Sleep (Config.Instance.ShortDelay);

			//204.4 Unselect "Images tagged with:" radio button on the "Screensaver Configure" dialog
			configureDialog.Find<RadioButton> ("Images tagged with:").AddToSelection ();
			procedureLogger.ExpectedResult ("The \"All Image\" radio button is unselected.");
			Assert.AreEqual (false, allImageRadioButton.IsSelected);
			Thread.Sleep (Config.Instance.ShortDelay);

			//204.5 Select "All Image" radio button "on the Screensaver Configure" dialog
			configureDialog.Find<RadioButton> ("All Image").AddToSelection ();
			procedureLogger.ExpectedResult ("The \"All Image\" radio button is selected again.");
			Assert.IsTrue (allImageRadioButton.IsSelected);
			Thread.Sleep (Config.Instance.ShortDelay);

			//204.6 Move the dialog to (x,y)
			procedureLogger.Action ("Check CanMove.");
			procedureLogger.ExpectedResult ("The value of CanMove is true.");
			Assert.IsTrue ( configureDialog.CanMove);
			Thread.Sleep (Config.Instance.ShortDelay);

			configureDialog.Move (200, 200);
			procedureLogger.ExpectedResult ("Move \"Configure Screensaver\" dialog to coordinates(200, 200 ).");
			Thread.Sleep (Config.Instance.ShortDelay);

			//204.7 Click "Close" Button
			configureDialog.Close ();
			procedureLogger.ExpectedResult ("The \"Configure Screensaver\" window is closed.");
			Thread.Sleep (Config.Instance.ShortDelay);
		}

		//TestCase205 Print the pictures
		[Test]
		public void RunTestCase205 ()
		{
			Run (TestCase205);
		}

		private void TestCase205 ()
		{
			//205.1 "Photo" Menu Item
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<MenuItem> ("Photo").Click ();
			procedureLogger.ExpectedResult ("The \"Import\" sub menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.2 "Print" Menu Item
			var printMenuItem = menuBar.Find<MenuItem> ("Print");
			printMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Print\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.3 Select "Image Settings" tab item
			var printDialog = window.Find<Window> ("Print");
			var settingTabItem = printDialog.Find<TabItem> ("Image Settings");
			settingTabItem.Select ();
			procedureLogger.ExpectedResult ("The \"Image Settings\"'s tab item is shown.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.4 Select the "Print file name" check box
			var printFileCheckBox = printDialog.Find<CheckBox> ("Print file name");
			printFileCheckBox.Toggle ();
			procedureLogger.ExpectedResult ("The \"Print file name\" check box is checked.");
			Assert.AreEqual (ToggleState.On, printFileCheckBox.ToggleState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.5 Select "General" tab
			var generalTabItem = printDialog.Find<TabItem> ("General");
			generalTabItem.Select ();
			procedureLogger.ExpectedResult ("The \"General\" tab item is shown.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.6 Check the if Spin Button can be operated
			var spinButton = printDialog.Find<Spinner> ();
			procedureLogger.Action ("Check IsReadOnly property of Spin Button.");
			procedureLogger.ExpectedResult ("IsReadOnly is False.");
			Assert.AreEqual (false, spinButton.IsReadOnly);
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.6 Set the Spin Button's value to its Maximum
			spinButton.SetValue (100);
			procedureLogger.ExpectedResult ("The Spin Button's value is set to 100.");
			Assert.AreEqual (100, spinButton.Maximum);
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.7 Set the Spin Button's value to its Minimum
			spinButton.SetValue (0);
			procedureLogger.ExpectedResult ("The Spin Button's value is set to 0.");
			Assert.AreEqual (0, spinButton.Minimum);
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.10 Set the Spin Button's value to "50.0"
			spinButton.SetValue (50);
			procedureLogger.ExpectedResult ("The Spin Button's value is set to 50.");
			Assert.AreEqual (50, spinButton.Value);
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.11 Close the "Print" dialog
			printDialog.Close ();
			procedureLogger.ExpectedResult ("The \"Print\" window is closed.");
			Thread.Sleep (Config.Instance.ShortDelay);
		}

		//TestCase206 Sidebar
		[Test]
		public void RunTestCase206 ()
		{
			Run (TestCase206);
		}

		private void TestCase206 ()
		{
			//206.1 Select "View" Menu Item
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<MenuItem> ("View").Click ();
			procedureLogger.ExpectedResult ("The \"View\" sub menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//206.2 Select "Components" Menu Item
			var ComponentsMenuItem = menuBar.Find<MenuItem> ("Components");
			ComponentsMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Components\" sub menu appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//206.3 Select "Sidebar" Menu Item
			var SidebarMenuItem = ComponentsMenuItem.Find<MenuItem> ("Sidebar");
			SidebarMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Filesystem\" tree appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//206.4 Select the "Folders" item in the combobox
			var combobox = window.Find<ComboBox> ();
			var listItem = combobox.Find<ListItem> ("Folders");
			listItem.Select ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("The \"Folders\" list item has been selected.");
			Assert.IsTrue (listItem.IsSelected);

			//206.5 Expand "Filesystem" tree Item
			var filesystemTreeItem = window.Find<TreeItem> ("Filesystem");
			procedureLogger.Action ("Check the tree's ExpandCollapseState.");
			procedureLogger.ExpectedResult ("The tree's ExpandCollapseState is Collapsed.");
			Assert.AreEqual (ExpandCollapseState.Collapsed, filesystemTreeItem.ExpandCollapseState);
			Thread.Sleep (Config.Instance.ShortDelay);

			filesystemTreeItem.Expand ();
			procedureLogger.ExpectedResult ("The tree is expanded.");
			Assert.AreEqual (ExpandCollapseState.Expanded, filesystemTreeItem.ExpandCollapseState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//206.6 Expand "home" tree Item
			var homeTreeItem = window.Find<TreeItem> ("home");
			homeTreeItem.Expand ();
			procedureLogger.ExpectedResult ("The tree is expanded twice.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//206.7 Select "2010" tree Item
			var treeItem2010 = window.Find<TreeItem> ("2010");
			treeItem2010.Select ();
			procedureLogger.ExpectedResult ("The \"2010\"tree item is selected.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//206.8 Collapse "Filesystem" tree Item
			filesystemTreeItem.Collapse ();
			procedureLogger.ExpectedResult ("The tree is collapsed.");
			Assert.AreEqual (ExpandCollapseState.Collapsed, filesystemTreeItem.ExpandCollapseState);
			Thread.Sleep (Config.Instance.ShortDelay);
		}

		//TestCase207 About F-Spot
		[Test]
		public void RunTestCase207 ()
		{
			Run (TestCase207);
		}

		private void TestCase207 ()
		{
			//207.1 Select "Help" Menu Item
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<MenuItem> ("Help").Click ();
			procedureLogger.ExpectedResult ("The \"Help\" sub menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//207.2 Select "About" Menu Item
			var AboutMenuItem = menuBar.Find<MenuItem> ("About");
			AboutMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"About\" tree appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//207.3 Click "Credits" button on the "About Spot" dialog
			var aboutDialog = window.Find<Window> ("About F-Spot");
			aboutDialog.Find<Button> ("Credits").Click ();
			procedureLogger.ExpectedResult ("The \"About Spot\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//207.4 Check HorizontalScrollPercent and VerticalScrollPercent
			var document = window.Find<Document> ();
			procedureLogger.Action ("Check HorizontalScrollPercent of the document.");
			procedureLogger.ExpectedResult ("The value of HorizontalScrollPercent is 0.");
			Assert.AreEqual (0, document.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check VerticalScrollPercent of the document.");
			procedureLogger.ExpectedResult ("The value of VerticalScrollPercent is -1.");
			Assert.AreEqual (-1, document.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//207.5 Set the horizontal scroll bar large increment
			document.Scroll (ScrollAmount.LargeIncrement, ScrollAmount.NoAmount);
			procedureLogger.ExpectedResult ("The horizontal scroll bar scroll large increment.");
			Assert.AreEqual (43, document.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//207.6 Set the horizontal scroll bar large decrement
			document.Scroll (ScrollAmount.LargeDecrement, ScrollAmount.NoAmount);
			procedureLogger.ExpectedResult ("The horizontal scroll bar scroll large decrement.");
			Assert.AreEqual (0, document.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//207.7 Set the horizontal scroll bar small increment
			document.ScrollHorizontal (ScrollAmount.SmallIncrement);
			procedureLogger.ExpectedResult ("The horizontal scroll bar scroll small increment.");
			Assert.AreEqual (1, document.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//207.8 Set the horizontal scroll bar small decrement
			document.ScrollHorizontal (ScrollAmount.SmallDecrement);
			procedureLogger.ExpectedResult ("The horizontal scroll bar scroll small decrement.");
			Assert.AreEqual (0, document.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.MediumDelay);

			//207.9 Check HorizontalViewSize of document
			procedureLogger.Action ("Check HorizontalViewSize.");
			procedureLogger.ExpectedResult ("The value of HorizontalViewSize is.");
			Assert.AreEqual (100, document.HorizontalViewSize);
			Thread.Sleep (Config.Instance.MediumDelay);
		}
	}
}
