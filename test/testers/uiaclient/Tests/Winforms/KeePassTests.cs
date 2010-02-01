// KeePassTests.cs: Tests for KeePass
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

namespace MonoTests.Mono.UIAutomation.UIAClientAPI.Winforms
{
	[TestFixture]
	public class KeePassTests : TestBase
	{
		Window window = null;
		Application app;

		protected override void LaunchSample ()
		{
			//SingleInstance ("KeePass Password Safe");
			app = new Application ("KeePass");
			app.Launch ("mono", "KeePass.exe");
		}

		protected override void OnSetup ()
		{
			base.OnSetup ();
			window = app.GetWindow ("KeePass Password Safe");
		}

		protected override void OnQuit ()
		{
			base.OnQuit ();
			procedureLogger.Save ();
			window.Close ();
			// BUG 573464 - [uiaclient-winforms]Some dialog's name has been changed in Linux compares to in Windows
			// window.Find<Button> ("Discard changes").Click ();
			window.Find<Button> ("No").Click ();
			procedureLogger.ExpectedResult ("The  window quit successfully");
		}

		//TestCase101 Init Sample, create a new account
		[Test]
		public void RunTestCase101 ()
		{
			Run (TestCase101);
		}

		private void TestCase101 ()
		{
			//101.1 Click the "New..." button on the toolbar.
			var toolBar = window.Find<ToolBar> ();
			toolBar.Find<Button> ("New...").Click ();
			procedureLogger.ExpectedResult ("The \"Create New Password Database\" dialog opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.2 Enter "TestCase101" in the "File Name" combo box of the dailog.
			var newPassDialog = window.Find<Window> ("Create New Password Database");
			//BUG569846 [uiaclient-winforms]:UIA Client mathes wrong element
			//for LabeledByproperty on Linux
			var fileNameComboBox = newPassDialog.Find<ComboBox>(Direction.Vertical, 1);
			fileNameComboBox.SetValue("TestCase101");
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("\"TestCase101\" entered in the \"File Name\" box.");
			Assert.AreEqual ("TestCase101", fileNameComboBox.Value);
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.3 Change the view of list to "Extra Large Icons"
			var itemViewList = newPassDialog.Find<List> ();
			if (itemViewList.GetSupportedViews ().Contains<int> (0))
				itemViewList.SetCurrentView (0);
			Thread.Sleep (Config.Instance.ShortDelay);
			procedureLogger.ExpectedResult ("The current view of the dialog is \"Large Icons\"");
			/*
			 * Bug 571577 - [uiaclient-Winforms]: the Openfiledialog's itemViewList.GetSupportedViews()
			 * method can't be shown as expected
			 * Assert.AreEqual (itemViewList.GetViewName(itemViewList.CurrentView), "Large Icons");
			 * Thread.Sleep (Config.Instance.ShortDelay);
			 */

			//101.4 Click the "Save" button of the dialog.
			newPassDialog.Save ();
			procedureLogger.ExpectedResult ("The \"Create New Password Database\" dialog disappears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.5 Enter "mono-a11y" into  "Master password" text box.
			var createMasterKeyWindow = window.Find<Window> ("Create Composite Master Key");
			var masterPasswdEdit = createMasterKeyWindow.Find<Edit>(Direction.Vertical, 0);
			masterPasswdEdit.SetValue ("mono-a11y");
			procedureLogger.ExpectedResult ("\"mono-a11y\" entered in the \"Master password\" box.");
			Assert.AreEqual (false, masterPasswdEdit.IsReadOnly);
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.6  Re-Enter "mono-a11y" into "Repeat password" text box.
			var repeatPasswdEdit = createMasterKeyWindow.Find<Edit>(Direction.Vertical, 1);
			repeatPasswdEdit.SetValue ("mono-a11y");
			procedureLogger.ExpectedResult ("\"mono-a11y\" entered in the \"Repeat password\" box.");
			Assert.AreEqual (false, repeatPasswdEdit.IsReadOnly);
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.7 Check "Key file/option" CheckBox
			var keyfileCheckBox = createMasterKeyWindow.Find<CheckBox> ("Key file / provider:");
			keyfileCheckBox.Toggle();
			procedureLogger.ExpectedResult ("\"Key file/option\" CheckBox chekced.");
			Assert.AreEqual (ToggleState.On, keyfileCheckBox.ToggleState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.8 Click the " Create..." button.
			createMasterKeyWindow.Find<Button> (" Create...").Click ();
			procedureLogger.ExpectedResult ("The \"Create a new key file\" dialog opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.9  Click the "Save" button of the dialog.
			var newKeyFileDialog = window.Find<Window> ("Create a new key file");
			newKeyFileDialog.Save();

			/*
			 * BUG571799 - [uiaclient-Winforms]：The dialog
			 * who has parent has been found twice
			 * in case there is a TestCase101 key exist.
			 */

			/*
			 * BUG573464 - [uiaclient-winforms]Some dialog's name has been
			 * changed in Linux compares to in Windows
			 */
			//var comfirmDialog = newKeyFileDialog.Find<Window> ("Confirm Save As");
			var comfirmDialog = window.Find<Window> ("Save");
			if (comfirmDialog != null) {
				procedureLogger.ExpectedResult ("The \"Save\" dialog opens.");
				Thread.Sleep (Config.Instance.ShortDelay);

				comfirmDialog.OK ();
				procedureLogger.ExpectedResult ("The \"Save\" dialog disappears.");
				Thread.Sleep (Config.Instance.ShortDelay);
			} else {
				procedureLogger.ExpectedResult ("The \"Entropy Collection\" window opens.");
				Thread.Sleep (Config.Instance.ShortDelay);
			}

			//101.10 Click the "OK" button of the dialog.
			createMasterKeyWindow.Find<Window> ("Entropy Collection").OK ();
			procedureLogger.ExpectedResult ("The \"Entropy Collection\" window disappears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.11 Click the "OK" button on the "Create Master Key" Window
			createMasterKeyWindow.OK ();
			procedureLogger.ExpectedResult ("The \"Create Master Key\" window disappears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.12 Select the "Compression" Tab item.
			var newPassDialog2 = window.Find<Window> ("Create New Password Database - Step 2");
			var compressionTabItem = newPassDialog2.Find<TabItem> ("Compression");
			compressionTabItem.Select ();
			procedureLogger.ExpectedResult ("The \"Compression\" tab item opened.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.13 Check the "None" RadioButton.
			compressionTabItem.Find<RadioButton> ("None").Select ();
			procedureLogger.ExpectedResult ("The \"None\" radio button selected.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.14 Click the "OK" button to close the dialog.
			newPassDialog2.OK ();
			procedureLogger.ExpectedResult ("The \"Create New Password Database - Step 2\" window disappears.");
			Thread.Sleep (Config.Instance.ShortDelay);
		}

		//TestCase102 Organize the group
		[Test]
		public void RunTestCase102 ()
		{
			Run (TestCase102);
		}

		private void TestCase102 ()
		{
			//102.1 Click the "New..." button on the toolbar.
			var toolBar = window.Find<ToolBar> ();
			toolBar.Find<Button> ("New...").Click ();
			procedureLogger.ExpectedResult ("The \"Create New Password Database\" dialog opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.2 Enter "TestCase102" in the "File Name" combo box of the dailog.
			var newPassDialog = window.Find<Window> ("Create New Password Database");
			var fileNameComboBox = newPassDialog.Find<ComboBox>(Direction.Vertical, 1);
			fileNameComboBox.SetValue("TestCase102");
			procedureLogger.ExpectedResult ("\"TestCase102\" entered in the \"File Name\" box.");
			Assert.AreEqual ("TestCase102", fileNameComboBox.Value);
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.3 Click the "Save" button of the dialog.
			newPassDialog.Save ();
			procedureLogger.ExpectedResult ("The \"Create New Password Database\" dialog disappears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.4 Enter "mono-a11y" into  "Master password" text box.
			var createMasterKeyWindow = window.Find<Window> ("Create Composite Master Key");
			var masterPasswdEdit = createMasterKeyWindow.Find<Edit>(Direction.Vertical, 0);
			masterPasswdEdit.SetValue ("mono-a11y");
			procedureLogger.ExpectedResult ("\"mono-a11y\" entered in the \"Master password\" box.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.5 Re-Enter "mono-a11y" into "Repeat password" text box.
			var repeatPasswdEdit = createMasterKeyWindow.Find<Edit>(Direction.Vertical, 1);
			repeatPasswdEdit.SetValue ("mono-a11y");
			procedureLogger.ExpectedResult ("\"mono-a11y\" entered in the \"Repeat password\" box.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.6 Click the "OK" button on the "Create Master Key" Window
			createMasterKeyWindow.OK ();
			procedureLogger.ExpectedResult ("The \"Create Master Key\" window disappears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.7 Click the "OK" button to close the dialog.
			var newPassDialog2 = window.Find<Window> ("Create New Password Database - Step 2");
			newPassDialog2.OK ();
			procedureLogger.ExpectedResult ("\"Create New Password Database - Step 2\" dialog disappears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.8 Click the "Edit" menu item on the menu bar.
			var menuBar = window.Find<MenuBar> ();
			var editMenuItem = menuBar.Find<MenuItem> ("Edit");
			editMenuItem.Click();
			procedureLogger.ExpectedResult ("The \"Edit \" menu is shown.");
			Thread.Sleep (Config.Instance.ShortDelay);

			editMenuItem.Find<MenuItem> ("Edit Group").Click ();
			procedureLogger.ExpectedResult ("The \"Edit Group\" dialog opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.9 Click the "Icon" button on the "Edit Group" dialog.
			var editGroupWindow = window.Find<Window> ("Edit Group");
			var generalTabItem = editGroupWindow.Find<TabItem> ("General");

			/*
			 * BUG 574226 - [uiaclient-winforms]The name of Button is "Icon" 
			 * in Windows but in linux is ""
			 * generalTabItem.Find<Button> ("Icon:").Click ();
			 */
			generalTabItem.Find<Button> (Direction.Vertical, 0).Click ();
			procedureLogger.ExpectedResult ("The \"Icon Picker\" dialog opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.10 Select list item "30" on the "Icon Picker" dialog.
			var iconPickerWindow = editGroupWindow.Find<Window> ("Icon Picker");
			var standardIconList = iconPickerWindow.Find<List>(Direction.Vertical, 0);
			var listItem30 = standardIconList.Find<ListItem> ("30");
			listItem30.Select ();
			procedureLogger.ExpectedResult ("The \"30\" list item is selected.");
			// In standardIconList List, we only allow single selection, so we
			// assert [0] is reasonable.
			Assert.AreEqual (false, standardIconList.CanSelectMultiple);
			Assert.AreEqual (false, standardIconList.IsSelectionRequired);
			Assert.AreEqual ("30", standardIconList.GetSelection () [0].Current.Name);
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.11 Unselect list item "30" on the "Icon Picker" dialog.
			listItem30.RemoveFromSelection ();
			procedureLogger.ExpectedResult ("The \"30\" list item is removed from selection.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.12 Select list item "30" on the "Icon Picker" dialog again.
			listItem30.AddToSelection ();
			procedureLogger.ExpectedResult ("The \"30\" list item is added to selection.");
			Thread.Sleep (Config.Instance.ShortDelay);

			/*
			 * The list items in the list is different from the windows, the listitem "68"
			 * is always shown.
			 * TODO write anther test case to cover the ScrollIntoView() method.
			 * 102.13 Click list item "68" on the "Icon Picker" dialog.
			 * standardIconList.Find<ListItem> ("68").ScrollIntoView ();
			 * procedureLogger.ExpectedResult ("The \"68\" list item is showed in the view.");
			 * Thread.Sleep (Config.Instance.ShortDelay);
			 */

			//102.14 Click the "OK" button on the "Icon Picker" dialog.
			iconPickerWindow.OK ();
			procedureLogger.ExpectedResult ("The \"Icon Picker\" dialog disappears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.15 Select the "Behavior" Tab Item from the Tab.
			var behaviorTabItem = editGroupWindow.Find<TabItem> ("Behavior");
			behaviorTabItem.Select ();
			procedureLogger.ExpectedResult ("The \"Behavior\" tab item opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//ToReview
			//102.16 Expand the "Searching entries in this group" combo box.
			var searchCombobox = behaviorTabItem.Find<ComboBox> ("Searching entries in this group:");
			searchCombobox.Expand ();
			procedureLogger.ExpectedResult ("\"Searching entries in this group\" combox box is expanded.");
			//Assert.AreEqual (searchCombobox.ExpandCollapseState, ExpandCollapseState.Expanded);
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.17 Collapse the "Searching entries in this group" combo box.
			searchCombobox.Collapse ();
			procedureLogger.ExpectedResult ("\"Searching entries in this group\" combox box is collapsed.");
			//Assert.AreEqual (searchCombobox.ExpandCollapseState, ExpandCollapseState.Collapsed);
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.18 Select the "Enabled" from the "Searching entries in this group" combo box.
			searchCombobox.Expand ();
			Thread.Sleep (Config.Instance.ShortDelay);
			searchCombobox.Find<ListItem> ("Enabled").Select ();
			procedureLogger.ExpectedResult ("The \"Enabled\" list item is selected.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.19 Click the "OK" button on the  "Edit Group" dialog.
			editGroupWindow.OK ();
			procedureLogger.ExpectedResult ("The \"Edit Group\" dialog disappears.");
			Thread.Sleep (Config.Instance.ShortDelay);
		}

		//TestCase103 test the "Add Entry" dialog
		[Test]
		public void RunTestCase103 ()
		{
			Run (TestCase103);
		}

		private void TestCase103 ()
		{
			//103.1 Click "new" button on the toolstripbar
			var toolBar = window.Find<ToolBar> ();
			toolBar.Find<Button> ("New...").Click ();
			procedureLogger.ExpectedResult ("The \"Create New Password Database\" dialog opens");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.2 Click "Save" button on the dialog
			procedureLogger.Action ("Click \"Save\" button of the dialog");
			var newPassDialog = window.Find<Window> ("Create New Password Database");
			newPassDialog.Save (false);
			procedureLogger.ExpectedResult ("The \"Create New Password Database\" dialog closes");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.3 Click "OK" button on the dialog
			procedureLogger.Action ("Click \"OK\" button of the dialog");
			var keyDialog = window.Find<Window> ("Create Composite Master Key");
			keyDialog.OK (false);
			procedureLogger.ExpectedResult ("The \"Create New Password Database\" dialog closes");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.4 Click "Yes" button on the dialog
			var createMasterKeyWindow = window.Find<Window> ("KeePass");
			createMasterKeyWindow.Yes (false);
			procedureLogger.ExpectedResult("\"mono-a11y\" entered in the \"Master password\" box");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.5 Click "OK" button on the dialog
			procedureLogger.Action ("Click \"OK\" button of the dialog");
			var newPassDialog2 = window.Find<Window> ("Create New Password Database - Step 2");
			newPassDialog2.OK (false);
			procedureLogger.ExpectedResult ("The \"Create New Password Database\" dialog closes");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.6 Click "Add Entry" button on the toolstripbar
			procedureLogger.Action ("Click \"Add Entry\" button on the toolstripbar");
			/*
			 * BUG574620 - [uiaclient-winforms]:On linux a control who's control 
			 * type is "SplitButton" on Windows is "Button"
			 * toolBar.Find<Button> ("Add Entry").Click (false);

			Console.WriteLine("the SplitButton is {0}.......", toolBar.Find<SplitButton> ("Add Entry"));
			Console.WriteLine("sssssssssssssssssssssssss");
			toolBar.Find<SplitButton> ("Add Entry").Click();
			Thread.Sleep (10000000);
			procedureLogger.ExpectedResult ("the \"Add Entry\" window appears");
			Thread.Sleep (Config.Instance.ShortDelay);
			 */

			//103.7 Check "Add Entry" window's default WindowPattern Property
			procedureLogger.Action ("Check \"Add Entry\" window's CanMaximizeProperty");
			Window addEntryDialog = window.Find<Window> ("Add Entry");
			Assert.AreEqual (false, addEntryDialog.CanMaximize);
			procedureLogger.ExpectedResult ("The window's CanMaximizeProperty should be False");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check \"Add Entry\" window's CanMinimizeProperty");
			Assert.AreEqual (false, addEntryDialog.CanMinimize);
			procedureLogger.ExpectedResult ("The window's CanMaximizeProperty should be False");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check \"Add Entry\" window's IsModalProperty");
			Assert.AreEqual (true, addEntryDialog.IsModal);
			procedureLogger.ExpectedResult ("The window's CanMaximizeProperty should be False");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check \"Add Entry\" window's IsTopmostProperty");
			Assert.AreEqual (false, addEntryDialog.IsTopmost);
			procedureLogger.ExpectedResult ("The window's CanMaximizeProperty should be False");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check \"Add Entry\" window's WindowInteractionStateProperty");
			Assert.AreEqual (WindowInteractionState.ReadyForUserInteraction, addEntryDialog.WindowInteractionState);
			procedureLogger.ExpectedResult ("The window's CanMaximizeProperty should be False");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check \"Add Entry\" window's WindowVisualStateProperty");
			Assert.AreEqual (WindowVisualState.Minimized, addEntryDialog.WindowVisualState);
			procedureLogger.ExpectedResult ("The window's CanMaximizeProperty should be False");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.8 move "add entry" window to (200,200 )
			procedureLogger.Action ("move \"add entry\" window to (200,200 )");
			addEntryDialog.Move (200, 200);
			procedureLogger.ExpectedResult ("the \"add entry\" window is moved to (200,200 )");
			Thread.Sleep (Config.Instance.ShortDelay);

			//check the transformpattern's property
			procedureLogger.Action ("Check \"Add Entry\" window's CanMoveProperty");
			Assert.AreEqual (true, addEntryDialog.CanMove);
			procedureLogger.ExpectedResult ("The window's CanMoveProperty should be False");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check \"Add Entry\" window's CanSizeProperty");
			Assert.AreEqual (false, addEntryDialog.CanResize);
			procedureLogger.ExpectedResult ("The window's CanSizeProperty should be False");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check \"Add Entry\" window's CanRotateProperty");
			Assert.AreEqual (false, addEntryDialog.CanRotate);
			procedureLogger.ExpectedResult ("The window's CanRotateProperty should be False");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.9 Click the "Auto-Type" tab item on the "Add Entry" Window
			procedureLogger.Action ("Click the \"Auto-Type\" tab item on the \"Add Entry\" Window");
			var tabItemAuto = window.Find<TabItem> ("Auto-Type");
			tabItemAuto.Select ();
			procedureLogger.ExpectedResult ("The \"Auto-Type\" tab item appears");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.10 Click the "Add" button on the "Add Entry" Window
			procedureLogger.Action ("Click the \"Add\" button on the \"Add Entry\" Window");
			addEntryDialog.Find<Button> ("Add").Click ();
			procedureLogger.ExpectedResult ("The \"Edit Auto-Type Item\" window appears");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.11 Drag the scroll bar to the bottom on the "Edit Auto-Type Item" window
			procedureLogger.Action ("drag the scroll bar to the 300 position");
			var autoItemDialog = window.Find<Window> ("Edit Auto-Type Item");
			ScrollBar scrollBar = window.Find<ScrollBar> ();
			scrollBar.SetValue (300);
			procedureLogger.ExpectedResult ("the scroll bar is draged to the 413 position");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.12 Check the scroll bar's property
			procedureLogger.Action ("Check scroll bar's IsReadOnlyProperty");
			Assert.AreEqual (false, scrollBar.IsReadOnly);
			procedureLogger.ExpectedResult ("The scroll bar's IsReadOnlyProperty should be False");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check scroll bar's LargeChangeProperty");
			Assert.AreEqual (131, (int) scrollBar.LargeChange);
			procedureLogger.ExpectedResult ("The scroll bar's large chaged value should be 131");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check scroll bar's LargeChangeProperty");
			Assert.AreEqual (1, (int) scrollBar.SmallChange);
			procedureLogger.ExpectedResult ("The scroll bar's large chaged value should be 131");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check scroll bar's Maximum value");
			Assert.AreEqual (362, (int) scrollBar.Maximum);
			procedureLogger.ExpectedResult ("The scroll bar's Maximum value shoule be 362");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check scroll bar's Minimum value");
			Assert.AreEqual (0, (int) scrollBar.Minimum);
			procedureLogger.ExpectedResult ("The scroll bar's minimum value shoule be 0");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check scroll bar's value whether equals to 300");
			Assert.AreEqual (300, (int) scrollBar.Value);
			procedureLogger.ExpectedResult ("The scroll bar's value should be 300");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.13 Click "OK" button on the dialog
			procedureLogger.Action ("Click \"OK\" button on the dialog");
			autoItemDialog.OK ();
			procedureLogger.ExpectedResult ("\"None\" radio button selected");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.14 Click the "Advanced" tab item on the "Add Entry" Window
			procedureLogger.Action ("Click the \"Advanced\" tab item on the \"Add Entry\" Window");
			var tabItemAdvanced = window.Find<TabItem> ("Advanced");
			tabItemAdvanced.Select ();
			procedureLogger.ExpectedResult ("The \"Advanced\" tab item appears");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.15 Click the "Add" button on the "Add Entry" Window
			var notesDatagrid = addEntryDialog.Find<DataGrid> ("Notes:");
			procedureLogger.Action ("Click the \"Add\" button on the \"Add Entry\" Window");
			addEntryDialog.Find<Button> ("Add").Click ();
			procedureLogger.ExpectedResult ("The \"Edit Entry String\" dialog appears");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.16 Type the "aa" into the "Name" edit
			procedureLogger.Action ("Type the \"aa\" into the \"Name\" edit");
			var editEntryStringWindow = window.Find<Window> ("Edit Entry String");
			SWF.SendKeys.SendWait ("aa");
			procedureLogger.ExpectedResult ("the \"name\" edit 's value is \"aa\"");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.17 Click "OK" button on the "Edit Entry String" dialog
			procedureLogger.Action ("Click \"OK\" button on the \"Edit Entry String\" dialog");
			editEntryStringWindow.OK ();
			procedureLogger.ExpectedResult ("The \"Edit Entry String\" window closes");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.18 Check the "aa" text's TableItemPattern
			var aaText = addEntryDialog.Find<Text> ("aa");

			procedureLogger.Action ("Check \"aa\" text's TableItemPattern's Column property");
			Assert.AreEqual (0, aaText.Column);
			procedureLogger.ExpectedResult ("The \"aa\" text's Colum should be 0");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check \"aa\" text's TableItemPattern's ColumnSpan property");
			Assert.AreEqual (1, aaText.ColumnSpan);
			procedureLogger.ExpectedResult ("The \"aa\" text's ColumnSpan should be 1");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check \"aa\" text's TableItemPattern's Row property");
			Assert.AreEqual (0, aaText.Row);
			procedureLogger.ExpectedResult ("The \"aa\" text's Row should be 0");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check \"aa\" text's TableItemPattern's RowSpan property");
			Assert.AreEqual (1, aaText.RowSpan);
			procedureLogger.ExpectedResult ("The \"aa\" text's RowSpan should be 1");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check \"aa\" text's TableItemPattern's ContainingGrid property");
			AutomationElement dataGridItem = notesDatagrid.GetItem (0, 0);
			AutomationElement aatextItem = window.Find<Text> ("aa").AutomationElement;
			Assert.AreEqual (dataGridItem, aatextItem);
			procedureLogger.ExpectedResult ("The \"aa\" text's ContainingGrid should be 0");
			Thread.Sleep (Config.Instance.ShortDelay);

			newPassDialog2.OK ();
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.19 Close the "Add Entry" Window
			procedureLogger.Action ("Close the \"Add Entry\" Window");
			addEntryDialog.OK (false);
			procedureLogger.ExpectedResult ("The \"Create New Password Database - Step 2\" window closes");
			Thread.Sleep (Config.Instance.ShortDelay);
		}

		//TestCase104 test the "Password Generator" dialog
		[Test]
		public void RunTestCase104 ()
		{
			Run (TestCase104);
		}

		private void TestCase104 ()
		{
			//104.1 Click "new" button on the toolstripbar
			var toolBar = window.Find<ToolBar> ();
			toolBar.Find<Button> ("New...").Click ();
			procedureLogger.ExpectedResult ("The \"Create New Password Database\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.2 Click "Save" button on the dialog
			var newPassDialog = window.Find<Window> ("Create New Password Database");
			newPassDialog.Save ();
			procedureLogger.ExpectedResult ("The \"Create New Password Database\" dialog closes.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.3 Click "OK" button on the dialog
			var keyDialog = window.Find<Window> ("Create Composite Master Key");
			keyDialog.OK ();
			procedureLogger.ExpectedResult ("The \"KeePass\" dialog sppears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.4 Click "Yes" button on the dialog
			var createMasterKeyWindow = window.Find<Window> ("KeePass");
			createMasterKeyWindow.Yes ();
			procedureLogger.ExpectedResult ("The \"KeePass\" dialog closes.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.5  Click "OK" button on the dialog
			var newPassDialog2 = window.Find<Window> ("Create New Password Database - Step 2");
			newPassDialog2.OK ();
			procedureLogger.ExpectedResult ("The \"Create New Password Database - Step 2\" dialog closes.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.6  Click "Add Entry" button on the toolstripbar
			//BUGXXXXX: Button recognized as SplitButton on Linux, but it's Button on Windows
			var addEntryButton = toolBar.Find<SplitButton> ("Add Entry");
			addEntryButton.Click ();
			procedureLogger.ExpectedResult ("The \"Add Entry\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.7  Input the "email" into the title  edit
			procedureLogger.Action ("Input \"email\" into the \"Title\" edit.");
			SWF.SendKeys.SendWait ("email");
			procedureLogger.ExpectedResult ("\"email\" has been issued.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.8 Click "OK" button on the "Add Entry" dialog.
			//BUG571799 - [uiaclient-Winforms]：The dialog who has parent has been found twice
			//var addEntryDialog = window.Find<Window> ("Add Entry");
			//addEntryDialog.OK ();
			window.Find<Button> ("OK").Click ();
			procedureLogger.ExpectedResult ("The \"Add Entry\" dialog closes.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.9 Check the count of columns and rows of datagrid.
			var rightPane = window.Find<Pane> ().Find<Pane> (Direction.Vertical, 0).Find<Pane> (Direction.Horizental, 0);
			var dataGrid =  rightPane.Find<DataGrid> ();
			procedureLogger.Action ("Check the count of columns of datagrid.");
			procedureLogger.ExpectedResult ("The count of columns of datagrid is 11.");
			Assert.AreEqual (11, dataGrid.ColumnCount);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the count of columns of datagrid.");
			procedureLogger.ExpectedResult ("The count of columns of datagrid is 2.");
			Assert.AreEqual (2, dataGrid.RowCount);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.10 Click "Add Entry" button on the toolstripbar
			addEntryButton.Click();
			procedureLogger.ExpectedResult ("The \"Add Entry\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.11 Input the "shopping" into the title edit
			procedureLogger.Action ("Input \"shopping\" into the \"Title\" edit.");
			SWF.SendKeys.SendWait ("shopping");
			procedureLogger.ExpectedResult ("\"shopping\" has been issued.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.12 Click "OK" button on the "Add Entry" dialog
			//BUG571799 - [uiaclient-Winforms]：The dialog who has parent has been found twice
			//addEntryDialog.OK ();
			window.Find<Button> ("OK").Click ();
			procedureLogger.ExpectedResult ("The \"Add Entry\" dialog closes.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.13 Re-check the count of columns and rows of datagrid.
			procedureLogger.Action ("Check the count of columns of datagrid.");
			procedureLogger.ExpectedResult ("The count of columns of datagrid is 11.");
			Assert.AreEqual (11, dataGrid.ColumnCount);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the count of columns of datagrid.");
			procedureLogger.ExpectedResult ("The count of columns of datagrid is 3.");
			Assert.AreEqual (3, dataGrid.RowCount);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.14 Get the (0,0) element of the datagrid , check if it is "Sample Entry"
			var textItem = window.Find<Text> ("Sample Entry").AutomationElement;
			var dataGridItem = dataGrid.GetItem (0, 0);
			procedureLogger.ExpectedResult ("The (0,0) item of the datagrid is \"Sample Entry\".");
			Assert.AreEqual (textItem, dataGridItem);
			Thread.Sleep (Config.Instance.ShortDelay);

			// Resize the window smaller in order to make horizontal scroll bar could be displayed.
			window.Resize (500, 600);

			//104.15 Check the ScrollPattern's default property of datagrid
			procedureLogger.Action ("Check HorizontallyScrollable.");
			procedureLogger.ExpectedResult ("The value of HorizontallyScrollable property is true.");
			Assert.AreEqual (true, dataGrid.HorizontallyScrollable);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check HorizontalScrollPercent.");
			procedureLogger.ExpectedResult ("The value of HorizontalScrollPercent property is 0.0.");
			Assert.AreEqual (0.0, dataGrid.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check HorizontalViewSize.");
			//procedureLogger.ExpectedResult ("The value of HorizontalViewSize property is 75.711159737417944");
			//Assert.AreEqual (92.266666666666666d, dataGrid.HorizontalViewSize);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check VerticallyScrollable.");
			procedureLogger.ExpectedResult ("The value of VerticallyScrollable property is false.");
			Assert.AreEqual (false, dataGrid.VerticallyScrollable);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check VerticalScrollPercent.");
			procedureLogger.ExpectedResult ("The value of VerticalScrollPercent property is -1.");
			Assert.AreEqual (-1, dataGrid.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check VerticalViewSize.");
			//procedureLogger.ExpectedResult ("The value of VerticalViewSize property is 100.");
			//Assert.AreEqual (100, dataGrid.VerticalViewSize);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.16  Use Scroll method give the horizotal Scrollbar a LargeIncrement
			dataGrid.Scroll (ScrollAmount.LargeIncrement, ScrollAmount.NoAmount);
			procedureLogger.ExpectedResult ("The horizotal Scrollbar increase large");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.17 Use Scroll method give the horizotal Scrollbar a SmallIncrement
			procedureLogger.Action ("Use Scroll method give the horizotal Scrollbar a SmallIncrement");
			dataGrid.Scroll (ScrollAmount.SmallIncrement, ScrollAmount.NoAmount);
			procedureLogger.ExpectedResult ("The horizotal Scrollbar increase small");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.18 Check the horizotal scrollbar's position should be 100.0d
			procedureLogger.Action ("after the LargeIncrement and SmallIncrement the scroll's position is 100.0d ");
			Assert.AreEqual (100.0d, dataGrid.HorizontalScrollPercent);
			procedureLogger.ExpectedResult ("the scroll's position is 100.0d ");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.19 Use ScrollHorizontalmethod give the horizotal Scrollbar a  SmalIDecrement
			procedureLogger.Action ("Use Scroll method give the horizotal Scrollbar a SmalIDecrement");
			dataGrid.ScrollHorizontal (ScrollAmount.SmallDecrement);
			procedureLogger.ExpectedResult ("The horizotal Scrollbar decrease small");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.20 Use ScrollHorizontalmethod give the horizotal Scrollbar a LargeDecrement
			procedureLogger.Action ("Use Scroll method give the horizotal Scrollbar a LargeDecrement");
			dataGrid.Scroll (ScrollAmount.LargeDecrement, ScrollAmount.NoAmount);
			procedureLogger.ExpectedResult ("The horizotal Scrollbar decrease large");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.21 Check the horizotal scrollbar's position should be 0.0d
			procedureLogger.Action ("after the LargeDecrement and SmallDecrement the scroll's position is 0.0d");
			Assert.AreEqual (0.0d, dataGrid.HorizontalScrollPercent);
			procedureLogger.ExpectedResult ("the scroll's position is 0.0d");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.22 Use SetScrollPercent method make the horizotal scrollbar move to (50, -1)
			procedureLogger.Action ("Use SetScrollPercent method make the horizotal scrollbar move to (50, -1)");
			dataGrid.SetScrollPercent (50, -1);
			procedureLogger.ExpectedResult ("The horizotal Scrollbar move to (50, -1)");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.23 Check the horizotal scrollbar's position should be  54.054054054054056d
			procedureLogger.Action ("after the SetScrollPercent action the scroll's position is  54.054054054054056d");
			Assert.AreEqual (51.724137931034484d, dataGrid.HorizontalScrollPercent);
			procedureLogger.ExpectedResult ("The Horizontal scroll bar's VerticallyScrollable should be  54.054054054054056d");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.24 Check the data grid's MultipleViewPattern property
			procedureLogger.Action ("Check the data grid's CurrentView is 1");
			Assert.AreEqual (1, dataGrid.CurrentView);
			procedureLogger.ExpectedResult ("the data grid 's CurrentViewProperty should be 1");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.25 Retrieves the first name "Icons" of the view pattern of the data grid
			procedureLogger.Action ("Retrieves the first name \"Icons\" of the view pattern of the data grid");
			dataGrid.GetViewName (1);
			procedureLogger.ExpectedResult ("The Horizontal scroll bar's VerticallyScrollable should be  54.054054054054056d");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.26 Retrieves the supported name of the view pattern of the data grid, Assert its value is one
			procedureLogger.Action ("Retrieves the supported name of the view pattern of the data grid, Assert its value is one");
			Assert.AreEqual (1, dataGrid.CurrentView);
			procedureLogger.ExpectedResult ("The supported name's value of the view pattern of the data grid is one");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.27 Check the GridItemPattern's property of text in data grid
			procedureLogger.Action ("Check the column of data grid");
			Assert.AreEqual (0, dataGrid.Column);
			procedureLogger.ExpectedResult ("the column of data grid should be 0");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the column span of data grid");
			procedureLogger.ExpectedResult ("the column span of data grid should be 1");
			Assert.AreEqual (1, dataGrid.ColumnSpan);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the row of data grid");
			Assert.AreEqual (0, dataGrid.Row);
			procedureLogger.ExpectedResult ("the row of data grid should be 0");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the row span of data grid");
			Assert.AreEqual (1, dataGrid.RowSpan);
			procedureLogger.ExpectedResult ("the row span of data grid should be 1");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the ContainingGrid of data grid");
			Assert.AreEqual (null, dataGrid.ContainingGrid);
			procedureLogger.ExpectedResult ("the ContainingGrid of data grid should be null");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.28 Check the TableItem's property of text in data grid
			procedureLogger.Action ("Check the TableItemColumn of the text in datagrid");
			var sampleText = window.Find<Text> ("Sample Entry");
			Assert.AreEqual (0, sampleText.TableItemColumn);
			procedureLogger.ExpectedResult ("the TableItemColumn of the text in datagrid should be 0");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the TableItemColumnSpan of the text in datagrid");
			Assert.AreEqual (1, sampleText.TableItemColumnSpan);
			procedureLogger.ExpectedResult ("the TableItemColumnSpan of the text in datagrid should be 1");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the TableItemContainingGrid of the text in datagrid");
			Assert.AreEqual (dataGrid.AutomationElement, sampleText.TableItemContainingGrid);
			procedureLogger.ExpectedResult ("the TableItemContainingGrid of the text in datagrid should be dataGrid.AutomationElement");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the TableItemRow of the text in datagrid");
			Assert.AreEqual (0, sampleText.TableItemRow);
			procedureLogger.ExpectedResult ("the TableItemRow of the text in datagrid should be 0");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the TableItemRowHeaderItems of the text in datagrid");
			Assert.AreEqual ("", sampleText.TableItemRowHeaderItems);
			procedureLogger.ExpectedResult ("the TableItemRowHeaderItems of the text in datagrid should be none");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the TableItemColumnHeaderItems of the text in datagrid");
			Assert.AreEqual (sampleText.AutomationElement.GetCurrentPropertyValue (GridItemPattern.ContainingGridProperty),
				dataGrid.AutomationElement);
			procedureLogger.ExpectedResult ("the TableItemColumnHeaderItems of the text in datagrid should be ");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the TableItemRowSpan of the text in datagrid");
			Assert.AreEqual (1, sampleText.TableItemRowSpan);
			procedureLogger.ExpectedResult ("the TableItemRowSpan of the text in datagrid should be 1");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.29 Click "Tools" menu item on the menu bar
			procedureLogger.Action ("Click \"Tools\" menu item on the menu bar");
			window.Find<MenuItem> ("Tools").Click (false);
			procedureLogger.ExpectedResult ("The sub menu of \"Tools\" appears");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.30 Click "Generate Password.." menu item on the sub menu
			procedureLogger.Action ("Click \"Generate Password..\" menu item on the sub menu");
			window.Find<MenuItem> ("Generate Password...").Click (false);
			procedureLogger.ExpectedResult ("The Password Generator dialog appears");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.31 Select "Preview" Tab item
			procedureLogger.Action ("Select \"Preview\" Tab item");
			var tabItemPreview = window.Find<TabItem> ("Preview");
			tabItemPreview.Select ();
			procedureLogger.ExpectedResult ("The \"Preview\" tab item appears");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.32 Use Scroll method give the vertical Scrollbar a LargeIncrement
			procedureLogger.Action ("Use Scroll method give the vertical Scrollbar a LargeIncrement");
			var passwordDocument = tabItemPreview.Find<Document> ();
			passwordDocument.Scroll (ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
			procedureLogger.ExpectedResult ("The vertical Scrollbar increase large");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.33 Use Scroll method give the vertical Scrollbar a SmallIncrement
			procedureLogger.Action ("Use Scroll method give the vertical Scrollbar a LargeIncrement");
			passwordDocument.Scroll (ScrollAmount.NoAmount, ScrollAmount.SmallIncrement);
			procedureLogger.ExpectedResult ("The vertical Scrollbar increase large");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.34 Check the vertical scrollbar's position
			procedureLogger.Action ("after the SetScrollPercent action the scroll's position is 100.0d");
			Assert.AreEqual (100.0d, passwordDocument.VerticalScrollPercent);
			procedureLogger.ExpectedResult ("The vertical scroll bar's Vertically Scrollable should be 100.0d");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.35 Use ScrollHorizontal method give the vertical Scrollbar a SmalIDecrement
			procedureLogger.Action ("Use Scroll method give the vertical Scrollbar a LargeIncrement");
			passwordDocument.ScrollVertical (ScrollAmount.SmallDecrement);
			procedureLogger.ExpectedResult ("The vertical Scrollbar increase large");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.36 Use ScrollVertical method give the vertical Scrollbar a LargeDecrement
			procedureLogger.Action ("Use Scroll method give the vertical Scrollbar a LargeIncrement");
			passwordDocument.Scroll (ScrollAmount.NoAmount, ScrollAmount.LargeDecrement);
			procedureLogger.ExpectedResult ("The horizotal vertical increase large");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.37 Check the vertical scrollbar's position
			procedureLogger.Action ("after the SetScrollPercent action the scroll's position is 0.0d");
			Assert.AreEqual (0.0d, passwordDocument.VerticalScrollPercent);
			procedureLogger.ExpectedResult ("The vertical scroll bar's VerticallyScrollable should be 0.0d");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.38 Use SetScrollPercent method make the vertical scrollbar move to (-1, 50)
			procedureLogger.Action ("Use SetScrollPercent method make the horizotal scrollbar move to (-1, 50)");
			passwordDocument.SetScrollPercent (-1, 50);
			procedureLogger.ExpectedResult ("The horizotal Scrollbar move to (-1, 50)");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.39 Check the vertical scrollbar's position
			procedureLogger.Action ("after the SetScrollPercent action the scroll's position is 50.0d");
			Assert.AreEqual (50.0d, passwordDocument.VerticalScrollPercent);
			procedureLogger.ExpectedResult ("The Horizontal scroll bar's VerticallyScrollable should be 50.0d");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.40 Minimize "NewDatabase.kdbx*-KeePass Password Safe" Window to (50,50)
			procedureLogger.Action ("click the Close button");
			window.Find<Button> ("Close").Click (false);
			procedureLogger.ExpectedResult ("The \"Password Generator\" dialogue disappears");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Minimize \"NewDatabase.kdbx*-KeePass Password Safe\" Window to (50, 50)");
			window.Resize (50, 50);
			procedureLogger.ExpectedResult ("NewDatabase.kdbx*-KeePass Password Safe\" Window is minimize to (50, 50)");
			Thread.Sleep (Config.Instance.ShortDelay);
		}
	}
}
