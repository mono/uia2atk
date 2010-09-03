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
			//Click "photo" menu item
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<MenuItem> ("Photo").Click ();
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
			//201.1 Click "photo" menu item
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<MenuItem> ("Photo").Click ();	
			procedureLogger.ExpectedResult ("The \"Photo\" sub menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//201.2 Select the "Import" menu item.
			var importMenuItem = menuBar.Find<MenuItem> ("Import...");
			importMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Import\" dialog appears.");
			Thread.Sleep (Config.Instance.MediumDelay);

			//201.3 Select the "Select Folder" menu item in its combo box.
			var importDialog = window.Find<Window> ("Import");
			var selectCombobox = importDialog.Find<ComboBox> ("Select Folder");
			var seleceMenuItem = selectCombobox.Find<MenuItem> ("Select Folder");
			seleceMenuItem.Click ();
			procedureLogger.ExpectedResult("The \"Import\" dialog appears.");
			Thread.Sleep (Config.Instance.LongDelay);
			
			
			
			
			/*
			 * BUG619425 - [uiaclient-GTKs]:The hierarchy of the window who has two subwindows is different from accerciser.
			 */
			/*
			var subImportDialogs = app.FindAllGtkSubWindow (window, "Import");
			Assert.AreEqual(2, subImportDialogs.Length);
			procedureLogger.Action ("Another \"Import\" dialog shows.");
			procedureLogger.ExpectedResult ("The second \"Import\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//201.4 Input text "/usr/share/pixmaps/" into "Location:" Edit
			var locationEdit = subImportDialogs[1].Find<Edit> ();
			locationEdit.SetValue ("/usr/share/pixmaps/");
			procedureLogger.ExpectedResult ("\"/usr/share/pixmaps\" has been entered.");
			Thread.Sleep (Config.Instance.ShortDelay);
			Thread.Sleep (Config.Instance.ShortDelay);

			//201.5 Click "Open" Button
			var openButton = subImportDialogs[1].Find<Button> ("Open");
			openButton.Click ();
			procedureLogger.ExpectedResult ("another \"Import\" dialog shows up");
			Thread.Sleep (Config.Instance.LongDelay);

			var newImportDialog = app.FindGtkSubWindow (window, "Import");
			*/
			/*
			 * BUG596461 - [uiaclient-GTKs]: The TableItems can not be found by UIAClient.
			 */ 
			/*
			procedureLogger.Action ("Check if the pictures are loaded");
			var list = newImportDialog.Find<List> ();
			var listItem = list.Find<ListItem> ();
			procedureLogger.ExpectedResult ("The picture(s) in \"/usr/share/pixmaps/\" is(are) loaded.");
			Thread.Sleep (Config.Instance.LongDelay);
			Assert.IsNotNull (listItem);
			*/
			
			//201.6 Test the Progressbar control's supported patterns & properties
			/*
			 * BUG607790 - [uiaclient-GTKs]:The Progressbar control 's RangeValue Pattern 
			 * is not implemented completely
			 */
			/*
			var progressbar = newImportDialog.Find<Process> ();
			SWF.ProgressBar progressbarControl = new SWF.ProgressBar ();
			AutomationElement progressbarAe = new AutomationElement.FromHandle (progressbarControl.Handle);
			AutomationPattern[] addPatterns = {RangeValuePattern.Pattern};
			helper.PatternChcek (progressbar, progressbarAe, addPatterns, null);
			
			procedureLogger.Action ("Test the LargeChange of progressbar");
			procedureLogger.ExpectedResult ("The LargeChange of progressbar should be 20");
			Assert.AreEqual (20, progressbar.LargeChange);
			Thread.Sleep (Config.Instance.ShortDelay);
			
			procedureLogger.Action ("Test the SmallChange of progressbar");
			procedureLogger.ExpectedResult ("The SmallChange of progressbar should be false");
			Assert.AreEqual (1, progressbar.SmallChange);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			//201.7 Click "Import" Button
			// if the bug 619425 is fixed, uncomment below codes.
			/*
			Thread.Sleep (Config.Instance.LongDelay);
			var importButton = newImportDialog.Find<Button> ("Import");
			importButton.Click ();
			Thread.Sleep (Config.Instance.LongDelay);
			*/
			
			/*
			 * BUG596461 - [uiaclient-GTKs]: The TableItems can not be found by UIAClient.
			 */ 
			/*
			var fspotList = window.Find<List> ();
			Thread.Sleep (Config.Instance.ShortDelay);
			var fspotListItem = fspotList.Find<ListItem> ();
			procedureLogger.ExpectedResult ("The pictures in \"/usr/share/pixmaps/\" are imported.");
			Thread.Sleep (Config.Instance.ShortDelay);
			Assert.IsNotNull (fspotListItem);
			*/
			
			//201.8 Check the pane control's pattern
			/*
			 * BUG609780 - [uiaclient-GTKs]:The Pane control supports wrong pattern
			 */
			/*
			var pane = window.Find<Pane> ();
			SWF.Panel paneControl = new SWF.Panel ();
			AutomationElement paneAe = AutomationElement.FromHandle (paneControl.Handle);
			AutomationPattern[] addPatterns = {TransformPattern.Pattern};
			helper.PatternChcek (pane, paneAe, addPatterns, null);
			*/
		}

		//TestCase202 Find a pic Item
		[Test]
		public void RunTestCase202 ()
		{
			Run (TestCase202);
		}

		private void TestCase202 ()
		{
			
			//202.1 Check the button on toolbar's can be focusd
			var browseButton = window.Find<Button> ("Browse");
			
			var editButton = window.Find<Button> ("Edit Image");
			
			/*
			 * BUG619448 - [uiaclient-GTKs]: The button can't be focused by SetFocus() method.
			 */
			/*
			procedureLogger.Action ("Set focus on browseButton.");
			browseButton.AutomationElement.SetFocus();
			procedureLogger.ExpectedResult ("The browseButton has been focused.");
			Assert.IsTrue (browseButton.AutomationElement.Current.HasKeyboardFocus);
			Thread.Sleep (Config.Instance.ShortDelay);
				
			procedureLogger.Action ("Set focus on editButton.");
			editButton.AutomationElement.SetFocus();
			procedureLogger.ExpectedResult ("The editButton has been focused.");
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			browseButton.Click();
			Thread.Sleep (Config.Instance.LongDelay);
			procedureLogger.ExpectedResult ("The browseButton has been invoked and focused.");
			/*
			 * BUG597263 [uiaclient-GTKs]: The Invoke method can't make button 's 
			 * HasKeyboardFocus property to be true
			 */ 
			//Assert.IsTrue (browseButton.AutomationElement.Current.HasKeyboardFocus);
			
			//If the BUG619425 has been fixed, uncomment the below codes.
			//Do the action to load the list items.
			/*
			ImportPictures ();

			//202.2 Check the List's SelectionPattern's property
			var List = window.Find<List> ();
			procedureLogger.Action ("Check the List Can be Selected Multiple.");
			procedureLogger.ExpectedResult ("The List can select multiple.");
			Assert.IsTrue (List.CanSelectMultiple);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the List Is Selection Required.");
			procedureLogger.ExpectedResult ("The List requires selection .");
			Assert.AreEqual (false, List.IsSelectionRequired);
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.3 Select the List's first icon
			var firstListItem = window.FindAll<ListItem> () [0];
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("The first icon is selected.");
			Assert.AreEqual (firstListItem.Name, List.GetSelection () [0].Current.Name);
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.4 Make the fifty-third icon in List visible
			var fiftyThirdListItem = window.FindAll<ListItem> () [53];
			fiftyThirdListItem.ScrollIntoView ();
			procedureLogger.ExpectedResult ("The fifty-third icon is shown.");
			Assert.AreEqual (false, fiftyThirdListItem.IsOffscreen);
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.5 Select and Unselect the fifty-third icon in List
			fiftyThirdListItem.Select ();
			procedureLogger.ExpectedResult ("The fifty-third icon is selected.");
			Assert.IsTrue (fiftyThirdListItem.IsSelected);
			Thread.Sleep (Config.Instance.ShortDelay);

			fiftyThirdListItem.RemoveFromSelection ();
			procedureLogger.ExpectedResult ("The fifty-third icon is unselected.");
			Assert.AreEqual (false, fiftyThirdListItem.IsSelected);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/

			//202.6 Click "Edit Image" menu item on the toolbar
			editButton.Click ();
			procedureLogger.ExpectedResult ("Only the selected icon is stayed on the window.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.7 Maximum the picture
			var slider = window.Find<Slider> ();

			/*
			 * BUG607040 - [uiaclient-GTKs]:The slider control should not support Invoke Pattern
			 */
			slider.SetValue (slider.Maximum);
			procedureLogger.ExpectedResult ("The picture is maximumed.");
			Assert.AreEqual (1.0, slider.Maximum);
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.8 Minimize the picture
			slider.SetValue (slider.Minimum);
			procedureLogger.ExpectedResult ("The picture is minimized.");
			Assert.AreEqual (0, slider.Minimum);
			Thread.Sleep (Config.Instance.ShortDelay);

			/*
			 * BUG600360 - [uiaclient-GTKs]: The SmallChangeProperty & LargeChangeProperty 
			 * of RangeValuePattern is not implemented .
			 */
			/*
			//202.9 Give the picture a small enlarge
			slider.SetValue (slider.Minimum + slider.SmallChange);
			procedureLogger.ExpectedResult ("Make the picture a small change large.");
			Assert.AreEqual (slider.SmallChange - slider.Minimum, slider.Value);
			Thread.Sleep (Config.Instance.ShortDelay);

			//202.10 Make the picture a big reduce
			slider.SetValue (slider.Maximum - slider.LargeChange);
			procedureLogger.ExpectedResult ("Make the picture a big change large.");
			Assert.AreEqual (slider.LargeChange, 1 - slider.Value);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/

			//202.11 Give a comment "the last one" to the picture
			var firstGroup = window.Find<Group> ();
			var subGroups = firstGroup.FindAll<Group> ();
			var pane = subGroups[1].Find<Pane> ();
			var groups = pane.FindAll<Group> ();
			var tab = groups[1].Find<Tab> ();
			var edit = tab.Find<Edit> ();

			edit.SetValue ("the last one");
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("The \"the last one\" is inputed into the edit.");
			Assert.AreEqual ("the last one", edit.Value);
			Thread.Sleep (Config.Instance.ShortDelay);

			/*
			 * BUG595149 WindowPattern is not finished
			 */
			/*
			//202.12 Close the F-Spot window
			window.Close ();
			procedureLogger.ExpectedResult ("The F-Spot window is closed.");
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
		}

		//TestCase203 Adjust the time of picture
		[Test]
		public void RunTestCase203 ()
		{
			Run (TestCase203);
		}

		private void TestCase203 ()
		{
			//If the BUG619425 has been fixed, uncomment the below codes.
			//Do the action to load the list items.
			/*
			ImportPictures ();

			//203.1 Select the third icon
			var thirdItem = window.FindAll<ListItem> () [3];
			thirdItem.Select ();
			procedureLogger.ExpectedResult ("The third icon is selected.");
			Assert.IsTrue (thirdItem.IsSelected);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/

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
			var adjustTimeDialog = app.FindGtkSubWindow (window, "Adjust Time");
			var selectButton = adjustTimeDialog.Find<Button> ("Select Date");
			selectButton.Click ();
			procedureLogger.ExpectedResult ("The \"Calendar\" is shown.");
			Thread.Sleep (Config.Instance.ShortDelay);

			/*
			 * BUG609377 - [uiaclient-GTKs]:The Calendar' children can't be found by UIA Client
			 */
			/*
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
			*/

			/*
			 * BUG595149 WindowPattern is not finished
			 */
			/*
			//203.8 Close the "Adjust Time" window
			adjustTimeDialog.Close ();
			procedureLogger.ExpectedResult ("The \"Adjust Time\" window is closed.");
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
		}

		//TestCase204 Screensaver Configuration
		[Test]
		public void RunTestCase204 ()
		{
			Run (TestCase204);
		}

		private void TestCase204 ()
		{
			//204.1 Select "Tools" Menu item
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
			//var configureDialog = window.Find<Window> ("Screensaver Configuration");
			var configureDialog = app.FindGtkSubWindow (window, "Screensaver Configuration");
			var allImageRadioButton = configureDialog.Find<RadioButton> ("All Images");
			/*
			 * BUG597696 - [uiaclient-GTKs]: RadioButton doesn't support the SelectionItemPattern
			 * BUG604197 - [uiaclient-GTKs]:The RadioButton control should not support the InvokePattern
			 */
			/*
			SWF.RadioButton radioButtonControl = new SWF.RadioButton ();
			AutomationElement radioButtonAe = AutomationElement.FromHandle (radioButtonControl.Handle);
			helper.PatternChcek (allImageRadioButton, radioButtonAe, null, null);
			*/
			
			/*
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
			*/

			/*
			 * BUG595149 WindowPattern is not finished
			 */ 
			/*
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
			*/
		}

		//TestCase205 Print the pictures
		[Test]
		public void RunTestCase205 ()
		{
			Run (TestCase205);
		}

		private void TestCase205 ()
		{
			//205.1 Click "Edit Image" button
			var editButton = window.Find<ToolBar> ().Find<Button> ("Edit Image");
			editButton.Click ();
			procedureLogger.ExpectedResult ("One picture is selected.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//205.2 "Photo" Menu item
			var menuBar = window.Find<MenuBar> (); 
			var photoMenuItem = menuBar.Find<MenuItem> ("Photo");
			photoMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Photo\" sub menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.3 "Print" Menu Item
			var printMenuItem = menuBar.Find<MenuItem> ("Print");
			printMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Print\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.4 Select "Image Settings" tab item
			//var printDialog = app.FindGtkSubWindow (window, "Print");
			var printDialog = window.Find<Window> ("Print");
			var tab = printDialog.Find<Tab> ();
			var tabItems = tab.FindAll<TabItem> ();
			var imageSettingTab = tabItems[2];
			imageSettingTab.Select ();
			procedureLogger.ExpectedResult ("The \"Image Settings\"'s tab item is shown.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.5 Select the "Print file name" check box
			var printFileCheckBox = imageSettingTab.Find<CheckBox> ("Print file name");
			
			//Check the checkbox's Toggle state
			procedureLogger.Action ("The unselected check box's ToggleState should be off");
			Assert.AreEqual (ToggleState.Off, printFileCheckBox.ToggleState);
			procedureLogger.ExpectedResult ("The unselected check box's ToggleState is off");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			printFileCheckBox.Toggle ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("The \"Print file name\" check box is toggle.");
			Assert.AreEqual (ToggleState.On, printFileCheckBox.ToggleState);

			//205.6 Select "General" tab
			var generalTabItem = tabItems[0];
			generalTabItem.Select ();
			procedureLogger.ExpectedResult ("The \"General\" tab item is shown.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.7 Check the Spin Button's supported pattern
			var firstGroup = generalTabItem.Find<Group> ();
			var groups = firstGroup.FindAll<Group> ();
			var subGroups = groups[1].FindAll<Group> ();
			var pane = subGroups[1].Find<Pane> ();
			var subPane = pane.Find<Pane> ();
			var spinner = subPane.Find<Spinner> ();

			/*
			 * BUG598413 - [uiaclient-GTKs]: the Spinner has extra Invoke pattern
			 */
			//TODO
			procedureLogger.Action ("Check IsReadOnly property of Spin Button.");
			procedureLogger.ExpectedResult ("IsReadOnly is False.");
			Assert.AreEqual (false, spinner.IsReadOnly);
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.8 Set the Spin Button's value to its Maximum
			spinner.SetValue (100);
			procedureLogger.ExpectedResult ("The Spin Button's value is set to 100.");
			Assert.AreEqual (100, spinner.Maximum);
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.9 Set the Spin Button's value to its Minimum
			spinner.SetValue (1);
			procedureLogger.ExpectedResult ("The Spin Button's value is set to 0.");
			Assert.AreEqual (1, spinner.Minimum);
			Thread.Sleep (Config.Instance.ShortDelay);

			//205.10 Set the Spin Button's value to "50.0"
			spinner.SetValue (50);
			procedureLogger.ExpectedResult ("The Spin Button's value is set to 50.");
			Assert.AreEqual (50, spinner.Value);
			Thread.Sleep (Config.Instance.ShortDelay);
			
			/*
			 * BUG595149 WindowPattern is not finished
			 */ 
			/*
			//205.11 Close the "Print" dialog
			printDialog.Close ();
			procedureLogger.ExpectedResult ("The \"Print\" window is closed.");
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
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

			/*
			 * BUG598803 - [uiaclient-GTKs]:the menu item who can 
			 * pop up the sub menu is recognized as menu
			 */
			/*
			//206.2 Select "Components" Menu Item
			var ComponentsMenuItem = menuBar.Find<MenuItem> ("Components");
			ComponentsMenuItem.Click ();
			procedureLogger.ExpectedResult ("The \"Components\" sub menu appears.");
			Thread.Sleep (Config.Instance.ShortDelay);
			*/

			/*
			 * BUG599140 - [uiaclient-GTKs]: check menu item is translated into Custom
			 */ 
			/*
			//206.3 Select "Sidebar" Menu Item
			var SidebarMenuItem = ComponentsMenuItem.Find<MenuItem> ("Sidebar");
			SidebarMenuItem.Invoke ();
			procedureLogger.ExpectedResult ("The \"Filesystem\" tree appears.");
			Thread.Sleep (Config.Instance.ShortDelay);
			*/

			/*
			 * BUG599600 - [uiaclient-GTKs]:the Combobox can't be found by UIA Client
			 */ 
			/*
			//206.4 Select the "Folders" item in the combobox
			var combobox = window.Find<ComboBox> ();
			var listItem = combobox.Find<ListItem> ("Folders");
			listItem.Select ();
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("The \"Folders\" list item has been selected.");
			Assert.IsTrue (listItem.IsSelected);
			*/

			/*
			 * BUG599589 - [uiaclient-GTKs]:the tree item can't be found by UIA Client
			 * BUG599598 - [uiaclient-GTKs]:The tree control should not support the Grid pattern
			 */
			/*
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
			*/
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

			/*
			 * BUG600432 - [uiaclient-GTKs]:A document whose content can be scrollable 
			 * should support scroll pattern
			 */
			/*
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
			*/
		}

		//TestCase208 Set Rating filter...
		//Mainly testing the patterns about menu item	
		[Test]
		public void RunTestCase208 ()
		{
			Run (TestCase208);
		}

		private void TestCase208 ()
		{
			//208.1 Test the menu bar's supported patterns.
			/*
			 * BUG600816 - [uiaclient-GTKs]:The menubar should not support SelectionPattern
			 */
			var menuBar = window.Find<MenuBar> ();
			/*
			SWF.MenuStrip menuStrip= new SWF.MenuStrip ();
			AutomationElement ae = AutomationElement.FromHandle (menuStrip.Handle);
			AutomationPattern[] addPatterns = {ExpandCollapsePattern.Pattern};
			helper.PatternChcek (menuBar, ae, addPatterns, null);
			*/
			
			//208.2 Select "Find" Menu 
			var findMenu = menuBar.Find<MenuItem> ("Find");
			findMenu.Click ();
			procedureLogger.ExpectedResult ("The \"Help\" sub menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//208.3 Collapse the "Find" menu item. 
			/*
			 * BUG597997 - [uiaclient-GTKs]: MenuItem doesn't support the ExpandCollapsePattern			 */
			/*
			findMenu.Find<MenuItem> ("Find").Collapse ();
			procedureLogger.ExpectedResult ("The \"Find\" sub menu item collapses.");
			Thread.Sleep (Config.Instance.ShortDelay);
			Assert.AreEqual (ExpandCollapseState.Collapsed, findMenu.ExpandCollapseState);
			*/
			
			//208.4 Expand the "Find" menu item.
			/*
			 * BUG597997 - [uiaclient-GTKs]: MenuItem doesn't support the ExpandCollapsePattern			 */
			/*
			findMenu.Find<MenuItem> ("By Rating").Expand ();
			procedureLogger.ExpectedResult ("The \"Find\" sub menu item expands.");
			Thread.Sleep (Config.Instance.ShortDelay);
			Assert.AreEqual (ExpandCollapseState.Expanded, findMenu.ExpandCollapseState);
			*/
			
			//208.5 Select the "Set Rating filter..." menu item.
			var setMenu = menuBar.Find<MenuItem> ("Set Rating filter...");
			setMenu.Click ();
			procedureLogger.ExpectedResult ("The \"Set Rating filter\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//208.6 Find the image
			/*
			 * BUG610269 - [uiaclient-GTKs]:The Image control can't be found by UIA Client
			 */
			/*
			var subwindows = window.FindAll<Window> ();
			var image = subwindows[0].Find<Image>();
			Assert.IsNotNull (image);
			procedureLogger.Action ("Find the image on \"Set Rating filter\" dialog .");
			procedureLogger.ExpectedResult ("The image can be found by uia.");
			*/
		}
		
		//TestCase209 Extension Manager
		[Test]
		public void RunTestCase209 ()
		{
			Run (TestCase209);
		}

		private void TestCase209 ()
		{
			//209.1 Select "Edit" menu item
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<MenuItem> ("Edit").Click ();
			procedureLogger.ExpectedResult ("The \"Edit\" menu item's sub menu is shown.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//209.2 Select "Manage Extensions" Menu item 
			menuBar.Find<MenuItem> ("Manage Extensions").Click ();
			procedureLogger.ExpectedResult ("The \"Extension Manager\" window is shown.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//209.3 Test the Window's supported patterns
			/*
			 * BUGBUG606666 - [uiaclient-GTKs]:The Window control supports none Patterns
			 * BUG595149 WindowPattern is not finished?		
			*/
			//TODO-Testing
			/*
			var extensionWindow = window.Find<Window> ("Extension Manager");
			SWF.Form window = new SWF.Form ();
			AutomationElement windowAe = AutomationElement.FromHandle (window.Handle);
			helper.PatternChcek (extensionWindow, windowAe, null, null);
			*/
			
			//209.4 Test the disable button's IsKeyboardFocusableProperty 
			/*
			 * BUG604598 - [uiaclient-GTKs]:The IsKeyboardFocusableProperty of disable button should be false
			*/
			/*
			var disableButton = extensionWindow.Find<Button> ("uninstall...");
			procedureLogger.Action ("Test the IsKeyboardFocusableProperty of disable button");
			procedureLogger.ExpectedResult ("The IsKeyboardFocusableProperty of disable button should be false");
			Assert.IsFalse(disableButton.AutomationElement.Current.IsKeyboardFocusable);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			//209.5 Test the GTK controls's ClassName property
			/*
			 * BUG604219 - [uiaclient-GTKs]:For all the GTK controls,the ClassName property is not implemented
			*/
			/*
			procedureLogger.Action ("Test the IsKeyboardFocusableProperty of disable button");
			procedureLogger.ExpectedResult ("The IsKeyboardFocusableProperty of disable button should be false");
			Assert.AreEqual ("Button", disableButton.AutomationElement.Current.ClassName);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			//209.6 Test the separator's supportd patterns
			//TODO
			/*
			var separator = window.Find<Separator> ();
			SWF.ToolStripSeparator stripSeparator = new SWF.ToolStripSeparator ();
			AutomationElement separatorAe = AutomationElement.FromHandle (stripSeparator.);
			helper.PatternChcek (menuBar, separatorAe, null, null);
			*/
		}
		
		//TestCase210 The Table control
		[Test]
		public void RunTestCase210 ()
		{
			Run (TestCase210);
		}

		private void TestCase210 ()
		{
			//210.1 Select "Edit" menu item
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<MenuItem> ("Edit").Click ();
			procedureLogger.ExpectedResult ("The \"Edit\" menu item's sub menu is shown.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//210.2 Select "Manage Extensions" Menu item 
			menuBar.Find<MenuItem> ("Manage Extensions").Click ();
			procedureLogger.ExpectedResult ("The \"Extension Manager\" dialog is shown.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//210.3 Test the Table control's supported patterns & property
			var extensionWindow = app.FindGtkSubWindow (window, "Extension Manager");
			var table = extensionWindow.Find<Table> ();
			/*
			 * BUG604660 - [uiaclient-GTKs]:The Table control should not support Selection pattern
			*/
			/*
			SWF.DataGrid table = new SWF.DataGrid ();
			AutomationElement tableAe = AutomationElement.FromHandle (table.Handle);
			AutomationPattern[] delPatterns = {GridItemPattern.Pattern.Pattern, SelectionPattern.Pattern};
			helper.PatternChcek (menuBar, ae, null, delPatterns);
			*/
			
			/*
			 *  BUG605087 - [uiaclient-GTKs]:The Table has not implemented RowOrColumnMajorProperty
			 */
			/*
			procedureLogger.Action ("Test the RowOrColumnMajorProperty of table");
			procedureLogger.ExpectedResult ("The RowOrColumnMajorProperty of table should be Indeterminate");
			Assert.AreEqual (RowOrColumnMajor.Indeterminate, TablePattern.RowOrColumnMajorProperty);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			//210.4 Test the Header control's supported patterns & property
			/*
			 * BUG604649 - [uiaclient-GTKs]:The Header control supports the wrong patterns
			*/
			var header = table.Find<Header> ();
		
			/*
			 * BUG605093 - [uiaclient-GTKs]:The Header control's IsContentElementProperty should be false
			 */
			/*
			procedureLogger.Action ("Test the IsContentElementProperty of header");
			procedureLogger.ExpectedResult ("The IsContentElementProperty of header should be false");
			Assert.AreEqual (false, header.AutomationElement.Current.IsContentElement);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			//210.5 Test the Header Item control's supported patterns
			/*
			 * BUG605114 - [uiaclient-GTKs]:The uia-explore can't find the Header Item control
			*/
			var headerItem = table.Find<HeaderItem> ();
			
			//210.6 Test the Data Item control's supportd patterns
			/*
			 * BUG606293 - [uiaclient-GTKs]:The Data Item control should not support Selection ,Grid & Table Pattern
			 * BUG606297 - [uiaclient-GTKs]:All Data Item should support SelectionItem Pattern
			 * BUG606300 - [uiaclient-GTKs]:The Data Item can can typically be traversed by using 
			 * the keyboard should support GridItem pattern
			 * BUG606301 - [uiaclient-GTKs]:The Data Item which is Table control's child should support TableItem pattern	
			 * BUG606305 - [uiaclient-GTKs]:The Data Item contains a state that can be cycled through should support Toggle Pattern
			*/
			var dataItem = window.Find<DataItem> ();
			
			//210.7 Expand the Data Item control
			/*
			 * BUG606295 - [uiaclient-GTKs]:The Data Item can be expanded or collapsed should support ExpandCollapse Pattern
			 */
			/*
			dataItem.Expand ();
			procedureLogger.ExpectedResult ("The data grid is expanded.");
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			//210.8 Test the control type of Data Item control's child
			/*
			 * BUG606656 - [uiaclient-GTKs]:The child of Data Item is recognized as edit and custom 
			 */ 
			//var subDataItem = window.Find<DataItem> ();
		}
		
		//TestCase211 About F-Spot dialog
		[Test]
		public void RunTestCase211 ()
		{
			Run (TestCase211);
		}

		private void TestCase211 ()
		{
			//211.1 Select "Help" menu item
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<MenuItem> ("Help").Click ();
			procedureLogger.ExpectedResult ("the \"Help\"'s sub menu is shown");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//211.2 Select "About" Menu item 
			menuBar.Find<MenuItem> ("About").Click ();
			procedureLogger.ExpectedResult ("the \"About F-Spot\" window is shown");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//211.3 Click "Credits" button
			var aboutDialog = window.Find<Window> ("About F-Spot");
			var creditsButton = aboutDialog.Find<Button> ("Credits");
			creditsButton.Click ();
			procedureLogger.ExpectedResult ("the \"Credits\" window is shown");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//211.4 Test the Document control's supported patterns & property
			var creditsDialog = window.Find<Window> ("Credits");
			var writtenByTabItem = creditsDialog.Find<TabItem> ("Written by");
			var writtenByDocument = writtenByTabItem.Find<Document> ();
			/*
			 * BUG600432 - [uiaclient-GTKs]:A document whose content can be scrollable should support scroll pattern
			 * BUG600430 - [uiaclient-GTKs]:A Document should not support value pattern
			*/
			/*
			SWF.TextBox document = new SWF.TextBox ();
			AutomationElement documentAe = AutomationElement.FromHandle (document.Handle);
			AutomationPattern[] addPatterns = {ScrollPattern.Pattern};
			helper.PatternChcek (document, documentAe, addPatterns, null);
			*/
			
			var header = creditsDialog.Find<Header> ();
			
			/*
			 * BUG600805 - [uiaclient-GTKs]:The Document has not implemented IsControlElementProperty
			 */
			/*
			procedureLogger.Action ("Test the IsControlElementProperty of document");
			procedureLogger.ExpectedResult ("The IsControlElementProperty of document should be false");
			Assert.AreEqual (false, writtenByDocument.AutomationElement.Current.IsControlElement);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			/*
			 *BUG600803 - [uiaclient-GTKs]:The Document has not implemented IsContentElementProperty 
			 */
			/*
			procedureLogger.Action ("Test the IsContentElementProperty of document");
			procedureLogger.ExpectedResult ("The IsContentElementProperty of document should be false");
			Assert.AreEqual (false, writtenByDocument.AutomationElement.Current.IsContentElement);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/

			//211.5 Test scrollbar control's supported patterns & properties
			var scrollbar = window.Find<ScrollBar> ();
			/*
			SWF.ScrollableControl scrollbarControl = new SWF.ScrollableControl ();
			AutomationElement scrollbarAe = AutomationElement.FromHandle (scrollbarControl.Handle);
			AutomationPattern[] addPatterns = {InvokePattern.Pattern, RangeValuePattern.Pattern};
			helper.PatternChcek (scrollbar, scrollbarAe, addPatterns, null);
			*/
			/*
			 * BUG600360 - [uiaclient-GTKs]: The ScrollBar does not support SmallChangeProperty
			*/
			/*
			procedureLogger.Action ("Test the SmallChange of scrollbar");
			procedureLogger.ExpectedResult ("The SmallChange of scrollbar should be 5");
			Assert.AreEqual ("5", scrollbar.SmallChange);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			/*
			 * BUG600360 - [uiaclient-GTKs]: The ScrollBar does not support SmallChangeProperty
			*/
			/*
			procedureLogger.Action ("Test the LargeChange of scrollbar");
			procedureLogger.ExpectedResult ("The LargeChange of scrollbar should be 30");
			Assert.AreEqual ("30", scrollbar.LargeChange);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			//211.6 Set the scrollbar to its maximum value
			/*
			 * 599993 - [uiaclient-GTKs]: The ScrollBar's value can't set to its maximum value
			 * & LargeChangeProperty for RangeValuePattern
			*/
			/*
			scrollbar.SetValue (752);
			procedureLogger.ExpectedResult ("the scrollbar is set to its maximum.");
			Assert.AreEqual ("752", scrollbar.Maximum);
			*/
			/*
			 * BUG600365 - [uiaclient-GTKs]:A scrollbar's IsreadOnly property should be false, 
			 * if the scrollbar's value can be set
			 */
			/*
			procedureLogger.Action ("Test the IsreadOnly of scrollbar");
			procedureLogger.ExpectedResult ("The IsreadOnly of scrollbar should be false,if the scrollbar's value can be set.");
			Assert.AreEqual (false, scrollbar.IsReadOnly);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			//211.7 Test the tab control's pattern & property
			var tab = creditsDialog.Find<Tab> ();
			/*
			SWF.TabControl tabControl = new SWF.TabControl ();
			AutomationElement tabAe = AutomationElement.FromHandle (tabControl.Handle);
			helper.PatternChcek (tab, tabAe, null, null);
			*/
			/*
			 * BUG600420 - [uiaclient-GTKs]:Tab 's IsSelectionRequired Property should be
			 * true if there is at one child being selected
			 */
			/*
			procedureLogger.Action ("Test the IsSelectionRequired of tab");
			procedureLogger.ExpectedResult ("The IsSelectionRequired of tab shoulld be true.");
			Assert.AreEqual (true, tab.IsSelectionRequired);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
		}
		
		//TestCase212 Create New Tag
		[Test]
		public void RunTestCase212 ()
		{
			Run (TestCase212);
		}

		private void TestCase212 ()
		{
			//212.1 Select "Tags" menu item
			var menuBar = window.Find<MenuBar> ();
			menuBar.Find<MenuItem> ("Tags").Click ();
			procedureLogger.ExpectedResult ("the \"Tags\"'s sub menu is shown");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//212.2 Select "Create New Tag..." Menu item 
			menuBar.Find<MenuItem> ("Create New Tag...").Click ();
			procedureLogger.ExpectedResult ("the \"Create New Tag\" window is shown");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//212.3 Test the check box control's supported patterns & property
			var tagDialog = window.Find<Window> ("Create New Tag");
			var subGroup = tagDialog.Find<Group> ();
			var panes = subGroup.FindAll<Pane> ();
			var checkBox = panes[2].Find<CheckBox> ();
			//TODO
			/*
			System.Windows.Forms.CheckBox checkboxControl = new System.Windows.Forms.CheckBox ();
			AutomationElement checkboxAe = AutomationElement.FromHandle (checkboxControl.Handle);
			helper.PatternChcek (menuBar, checkboxAe, null, null);
			*/
			/*
			 * BUG602294 - [uiaclient-GTKs]:The The CheckBox has not implemented IsContentElementProperty
			 */
			/*
			procedureLogger.Action ("Test the IsContentElementProperty of check box");
			procedureLogger.ExpectedResult ("The IsContentElementProperty of check box should be false");
			Assert.AreEqual (false, checkBox.AutomationElement.Current.IsContentElement);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			/*
			 * BUG602296 - [uiaclient-GTKs]:The CheckBox has not implemented IsControlElementProperty 
			 */
			/*
			procedureLogger.Action ("Test the IsControlElementProperty of check box");
			procedureLogger.ExpectedResult ("The IsControlElementProperty of check box should be false");
			Assert.AreEqual (false, checkBox.AutomationElement.Current.IsContentElement);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			//212.4 Test the combobox control's supported patterns & property
			var combobox = window.Find<ComboBox> ();
			
			/*
			 * BUG602699 - [uiaclient-GTKs]:The ComboBox control should support the ExpandCollapsePatte
			 * BUG602710 - [uiaclient-GTKs]:The ComboBox control should support the SelectionPattern
			 */
			/*
			SWF.ComboBox comboboxControl= new SWF.ComboBox ();
			AutomationElement comboboxAe = AutomationElement.FromHandle (comboboxControl.Handle);
			AutomationPattern[] addPatterns = {SelectionPattern.Pattern};
			helper.PatternChcek (comboboxControl, comboboxAe, addPatterns, null);
			*/
			
			/*
			 * BUG602716 - [uiaclient-GTKs]:The ComboBox's IsContentElementProperty should be true
			 */
			/*
			procedureLogger.Action ("Test the IsControlElementProperty of combobox");
			procedureLogger.ExpectedResult ("The IsControlElementProperty of combobox should be false");
			Assert.AreEqual (true, combobox.AutomationElement.Current.IsContentElement);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			/*
			 * BUG602721 - [uiaclient-GTKs]:The ComboBox's IsControlElementProperty should be true
			 */
			/*
			procedureLogger.Action ("Test the IsControlElementProperty of combobox");
			procedureLogger.ExpectedResult ("The IsControlElementProperty of combobox should be false");
			Assert.AreEqual (true, combobox.AutomationElement.Current.IsControlElement);
			Thread.Sleep (Config.Instance.ShortDelay);
			*/
			
			//212.5 Find the combobox's sub menu
			/*
			 * BUG602676 - [uiaclient-GTKs]:ComboBox can dropdown menu not menuitem
			*/
			//var subMenu = combobox.Find<Menu> ();
			
			//212.6 Test the edit control's supported patterns & property
			var edit = window.Find<Edit> ();
			/*
			 * BUG603639 - [uiaclient-GTKs]:The Edit control should support the InvokePattern
			*/
			/*
			SWF.TextBox editControl = new SWF.TextBox ();
			AutomationElement editAe = AutomationElement.FromHandle (editControl.Handle);
			AutomationPattern[] addPatterns = {TextPattern.Pattern, ValuePattern.Pattern};
			helper.PatternChcek (editControl, editAe, addPatterns, null);	
			*/
			
			procedureLogger.Action ("Test the IsControlElementProperty of edit");
			procedureLogger.ExpectedResult ("The IsControlElementProperty of edit should be false");
			Assert.AreEqual (true, edit.AutomationElement.Current.IsContentElement);
			Thread.Sleep (Config.Instance.ShortDelay);
			
			procedureLogger.Action ("Test the IsControlElementProperty of edit");
			procedureLogger.ExpectedResult ("The IsControlElementProperty of edit should be false");
			Assert.AreEqual (true, edit.AutomationElement.Current.IsControlElement);
			Thread.Sleep (Config.Instance.ShortDelay);
		}
	}
}
