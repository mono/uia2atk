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
			/* BUG 573464 - [uiaclient-winforms]Some dialog's name has been changed in Linux 
			 * compares to in Windows 
			 * window.Find<Button> ("Discard changes").Click ();
			 */
			window.Find<Button> ("No").Click ();
			procedureLogger.ExpectedResult ("The window quit successfully");
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
			var fileNameComboBox = newPassDialog.Find<ComboBox> (Direction.Vertical, 1);
			fileNameComboBox.SetValue ("TestCase101");
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
			var masterPasswdEdit = createMasterKeyWindow.Find<Edit> (Direction.Vertical, 0);
			masterPasswdEdit.SetValue ("mono-a11y");
			procedureLogger.ExpectedResult ("\"mono-a11y\" entered in the \"Master password\" box.");
			Assert.AreEqual (false, masterPasswdEdit.IsReadOnly);
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.6  Re-Enter "mono-a11y" into "Repeat password" text box.
			var repeatPasswdEdit = createMasterKeyWindow.Find<Edit> (Direction.Vertical, 1);
			repeatPasswdEdit.SetValue ("mono-a11y");
			procedureLogger.ExpectedResult ("\"mono-a11y\" entered in the \"Repeat password\" box.");
			Assert.AreEqual (false, repeatPasswdEdit.IsReadOnly);
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.7 Check "Key file/option" CheckBox
			var keyfileCheckBox = createMasterKeyWindow.Find<CheckBox> ("Key file / provider:");
			keyfileCheckBox.Toggle ();
			procedureLogger.ExpectedResult ("\"Key file/option\" CheckBox chekced.");
			Assert.AreEqual (ToggleState.On, keyfileCheckBox.ToggleState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.8 Click the " Create..." button.
			createMasterKeyWindow.Find<Button> (" Create...").Click ();
			procedureLogger.ExpectedResult ("The \"Create a new key file\" dialog opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//101.9  Click the "Save" button of the dialog.
			var newKeyFileDialog = window.Find<Window> ("Create a new key file");
			newKeyFileDialog.Save ();

			/*
			 * BUG571799 - [uiaclient-Winforms]：The dialog
			 * who has parent has been found twice
			 * in case there is a TestCase101 key exist.
			 */

			/*
			 * BUG573464 - [uiaclient-winforms]Some dialog's name has been
			 * changed in Linux compares to in Windows
			 * var comfirmDialog = newKeyFileDialog.Find<Window> ("Confirm Save As");
			 */
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
			var fileNameComboBox = newPassDialog.Find<ComboBox> (Direction.Vertical, 1);
			fileNameComboBox.SetValue ("TestCase102");
			procedureLogger.ExpectedResult ("\"TestCase102\" entered in the \"File Name\" box.");
			Assert.AreEqual ("TestCase102", fileNameComboBox.Value);
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.3 Click the "Save" button of the dialog.
			newPassDialog.Save ();
			procedureLogger.ExpectedResult ("The \"Create New Password Database\" dialog disappears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.4 Enter "mono-a11y" into  "Master password" text box.
			var createMasterKeyWindow = window.Find<Window> ("Create Composite Master Key");
			var masterPasswdEdit = createMasterKeyWindow.Find<Edit> (Direction.Vertical, 0);
			masterPasswdEdit.SetValue ("mono-a11y");
			procedureLogger.ExpectedResult ("\"mono-a11y\" entered in the \"Master password\" box.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.5 Re-Enter "mono-a11y" into "Repeat password" text box.
			var repeatPasswdEdit = createMasterKeyWindow.Find<Edit> (Direction.Vertical, 1);
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
			editMenuItem.Click ();
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
			var standardIconList = iconPickerWindow.Find<List> (Direction.Vertical, 0);
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
			 * TODO write another test case to cover the ScrollIntoView() method.
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

			//102.16 Check ExpandCollapseState property.
			var searchCombobox = behaviorTabItem.Find<ComboBox> ("Searching entries in this group:");
			procedureLogger.Action ("Check ExpandCollapseState.");
			procedureLogger.ExpectedResult ("The value of ExpandCollapseState is Collapsed.");
			Assert.AreEqual (ExpandCollapseState.Collapsed, searchCombobox.ExpandCollapseState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.17 Expand the "Searching entries in this group" combo box.
			searchCombobox.Expand ();
			Thread.Sleep (Config.Instance.MediumDelay);
			procedureLogger.ExpectedResult ("\"Searching entries in this group\" combox box is expanded.");
			Assert.AreEqual (ExpandCollapseState.Expanded, searchCombobox.ExpandCollapseState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.18 Collapse the "Searching entries in this group" combo box.
			searchCombobox.Collapse ();
			Thread.Sleep (Config.Instance.MediumDelay);
			procedureLogger.ExpectedResult ("\"Searching entries in this group\" combox box is collapsed.");
			Assert.AreEqual (ExpandCollapseState.Collapsed, searchCombobox.ExpandCollapseState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.19 Select the "Enabled" from the "Searching entries in this group" combo box.
			//searchCombobox.Expand ();
			searchCombobox.Find<ListItem> ("Enabled").Select ();
			Thread.Sleep (Config.Instance.MediumDelay);
			procedureLogger.ExpectedResult ("The \"Enabled\" list item is selected.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//102.20 Click the "OK" button on the  "Edit Group" dialog.
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
			procedureLogger.ExpectedResult ("The \"Create New Password Database\" dialog opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.2 Click "Save" button on the dialog
			var newPassDialog = window.Find<Window> ("Create New Password Database");
			newPassDialog.Save ();
			procedureLogger.ExpectedResult ("The \"Create New Password Database\" dialog closes.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.3 Click "OK" button on the dialog
			var keyDialog = window.Find<Window> ("Create Composite Master Key");
			keyDialog.OK ();
			procedureLogger.ExpectedResult ("The \"KeePass\" dialog sppears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.4 Click "Yes" button on the dialog
			var createMasterKeyWindow = window.Find<Window> ("KeePass");
			createMasterKeyWindow.Yes ();
			procedureLogger.ExpectedResult ("The \"KeePass\" dialog closes.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.5 Click "OK" button on the dialog
			var newPassDialog2 = window.Find<Window> ("Create New Password Database - Step 2");
			newPassDialog2.OK ();
			procedureLogger.ExpectedResult ("The \"Create New Password Database - Step 2\" dialog closes.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.6 Click "Add Entry" button on the toolstripbar
			//BUG574620 :On linux a control who's control type is "SplitButton" on Windows is "Button"
			//BUG576050- [uiaclient-winforms]: The splitbutton's Invoke method doesn't work
			var addEntryButton = toolBar.Find<SplitButton> ("Add Entry");
			addEntryButton.Find<MenuItem> ().Click ();
			procedureLogger.ExpectedResult ("The \"Add Entry\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.7 Check "Add Entry" window's default WindowPattern Property
			var addEntryDialog = window.Find<Window> ("Add Entry");
			procedureLogger.Action ("Check CanMaximize.");
			procedureLogger.ExpectedResult ("The value of CanMaximize is false.");
			Assert.AreEqual (false, addEntryDialog.CanMaximize);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check CanMinimize.");
			procedureLogger.ExpectedResult ("The value of CanMinimize is false.");
			Assert.AreEqual (false, addEntryDialog.CanMinimize);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check IsModal.");
			procedureLogger.ExpectedResult ("The value of IsModal is true.");
			Assert.AreEqual (true, addEntryDialog.IsModal);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check IsTopmost.");
			procedureLogger.ExpectedResult ("The value of IsTopmost is false.");
			Assert.AreEqual (false, addEntryDialog.IsTopmost);
			Thread.Sleep (Config.Instance.ShortDelay);

			/*
			 * BUG576450 - [uiaclient-winforms] The dialog's Window Pattern' 
			 * WindowInteractionState will be different between Windows and Linux 
			 */
			/*
			 * procedureLogger.Action ("Check WindowInteractionState.");
			 * procedureLogger.ExpectedResult ("The value of WindowInteractionState is ReadyForUserInteraction.");
			 * Assert.AreEqual (WindowInteractionState.ReadyForUserInteraction, addEntryDialog.WindowInteractionState);
			 * Thread.Sleep (Config.Instance.ShortDelay);
			 */

			procedureLogger.Action ("Check WindowVisualState.");
			procedureLogger.ExpectedResult ("The value of WindowVisualState is Normal.");
			Assert.AreEqual (WindowVisualState.Normal, addEntryDialog.WindowVisualState);
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.8 move "add entry" window to (200,200 )
			procedureLogger.Action ("Check CanMove.");
			procedureLogger.ExpectedResult ("The value of CanMove is true.");
			Assert.AreEqual (true, addEntryDialog.CanMove);
			Thread.Sleep (Config.Instance.ShortDelay);

			addEntryDialog.Move (200, 200);
			procedureLogger.ExpectedResult ("Move \"Add Entry\" dialog to coordinates(200, 200 ).");
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check CanSize.");
			procedureLogger.ExpectedResult ("The value of CanSize is false.");
			Assert.AreEqual (false, addEntryDialog.CanResize);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check CanRotate.");
			procedureLogger.ExpectedResult ("The value of CanRotate is false.");
			Assert.AreEqual (false, addEntryDialog.CanRotate);
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.9 Click the "Auto-Type" tab item on the "Add Entry" Window
			var autoTypeTabItem = addEntryDialog.Find<TabItem> ("Auto-Type");
			autoTypeTabItem.Select ();
			procedureLogger.ExpectedResult ("The \"Auto-Type\" tab item appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.10 Click the "Add" button on the "Add Entry" Window
			autoTypeTabItem.Find<Button> ("Add").Click ();
			procedureLogger.ExpectedResult ("The \"Edit Auto-Type Item\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.12 Check the scroll bar's ReadOnly property
			//BUG571799 - [uiaclient-Winforms]：The dialog who has parent has been found twice
			//var editAutoTypeDialog = autoTypeTabItem.Find<Window> ("Edit Auto-Type Item");
			var editAutoTypeDialog = window.Find<Window> ("Edit Auto-Type Item");
			var scrollBar = editAutoTypeDialog.Find<ScrollBar> ();
			procedureLogger.Action ("Check IsReadOnly of the vertical scroll bar.");
			procedureLogger.ExpectedResult ("The value of IsReadOnly is false.");
			Assert.AreEqual (false, scrollBar.IsReadOnly);
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.13 Drag the scroll bar to move
			scrollBar.SetValue (300);
			procedureLogger.ExpectedResult ("The scroll bar scrolled 300.");
			Assert.AreEqual (300, (int) scrollBar.Value);
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.14 Drag the scroll bar to its maximum
			scrollBar.SetValue (scrollBar.Maximum);
			procedureLogger.ExpectedResult (string.Format("Set the value of scroll bar to maximum {0}.", scrollBar.Maximum));
			Assert.AreEqual (scrollBar.Maximum, scrollBar.Value);
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.15 Drag the scroll bar to its minimum
			scrollBar.SetValue (scrollBar.Minimum);
			procedureLogger.ExpectedResult (string.Format("Set the value of scroll bar to minimum {0}.", scrollBar.Minimum));
			Assert.AreEqual (scrollBar.Minimum, scrollBar.Value);
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.16 Drag the scroll bar to a LargeChange decrease
			scrollBar.SetValue (scrollBar.Maximum - scrollBar.LargeChange);
			procedureLogger.ExpectedResult ("Set the value of scroll bar to large decrement from the maxinum.");
			Assert.AreEqual (scrollBar.Maximum - scrollBar.Value, scrollBar.LargeChange);
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.17 Drag the scroll bar to a SmallChange increase
			scrollBar.SetValue (scrollBar.Minimum + scrollBar.SmallChange);
			procedureLogger.ExpectedResult ("Set the value of scroll bar to small increment from the mininum.");
			Assert.AreEqual (scrollBar.Value - scrollBar.Minimum, scrollBar.SmallChange);
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.18 Click "OK" button on the dialog
			editAutoTypeDialog.OK ();
			procedureLogger.ExpectedResult ("The \"Edit Auto-Type Item\" dialog closes.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.14 Click the "Advanced" tab item on the "Add Entry" Window
			var advancedTabItem = addEntryDialog.Find<TabItem> ("Advanced");
			advancedTabItem.Select ();
			procedureLogger.ExpectedResult ("The \"Advanced\" tab item appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.15 Click the "Add" button on the "Add Entry" Window
			advancedTabItem.Find<Button> ("Add").Click ();
			procedureLogger.ExpectedResult ("The \"Edit Entry String\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.16 Type the "a11y" into the "Name" edit
			var editEntryStringWindow = addEntryDialog.Find<Window> ("Edit Entry String");
			/* BUG569846 - [uiaclient-winforms]:UIA Client mathes wrong element 
			 * for LabeledByproperty on Linux
			 * nameCombobox = editEntryStringWindow.Find<ComboBox> ("Name:");
			 */
			var nameCombobox = editEntryStringWindow.Find<ComboBox> ("");
			nameCombobox.SetValue("a11y");
			procedureLogger.ExpectedResult ("\"a11y\" entered in the \"Name\" combo box.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.17 Click "OK" button on the "Edit Entry String" dialog
			editEntryStringWindow.OK ();
			procedureLogger.ExpectedResult ("The \"Edit Entry String\" window closes.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//103.18 Check the "a11y" text's TableItemPattern
			//BUG576455 All the "Text" controls are recognized as "Edit" on Linux
			//var notesDataGrid = advancedTabItem.Find<DataGrid> ("Notes:");
			//var a11yText = notesDataGrid.Find<Text> ("a11y");
			var a11yText = addEntryDialog.Find<Edit> ("a11y");

			procedureLogger.Action ("Check Column.");
			procedureLogger.ExpectedResult ("The value of Colum is 0.");
			Assert.AreEqual (0, a11yText.Column);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check ColumnSpan.");
			procedureLogger.ExpectedResult ("The value of ColumnSpan 1.");
			Assert.AreEqual (1, a11yText.ColumnSpan);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check Row.");
			procedureLogger.ExpectedResult ("The value of Row is 0.");
			Assert.AreEqual (0, a11yText.Row);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check RowSpan.");
			procedureLogger.ExpectedResult ("The value of RowSpan is 1.");
			Assert.AreEqual (1, a11yText.RowSpan);
			Thread.Sleep (Config.Instance.ShortDelay);

			/* 
			 * BUG576455 All the "Text" controls are recognized as "Edit" on Linux
			AutomationElement dataGridItem = notesDataGrid.GetItem (0, 0);
			AutomationElement a11ytextItem = window.Find<Edit> ("a11y").AutomationElement;
			procedureLogger.Action ("Check ContainingGrid.");
			procedureLogger.ExpectedResult ("The value of ContainingGrid is the AutomationElement of its parent.");
			Assert.AreEqual (dataGridItem, a11ytextItem);
			Thread.Sleep (Config.Instance.ShortDelay);
			 */

			//103.19 Close the "Add Entry" Window
			addEntryDialog.OK ();
			procedureLogger.ExpectedResult ("The \"Add Entry\" window closes.");
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
			//BUG574620: Button recognized as SplitButton on Linux, but it's Button on Windows
			//BUG576050: The splitbutton's Invoke method doesn't work
			var addEntryButton = toolBar.Find<SplitButton> ("Add Entry");
			addEntryButton.Find<MenuItem> ().Click ();
			procedureLogger.ExpectedResult ("The \"Add Entry\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.7  Input the "email" to the "Title" edit
			var addEntryDialog = window.Find<Window> ("Add Entry");
			addEntryDialog.Find<Edit> ("Title:").SetValue("email");
			procedureLogger.ExpectedResult ("\"email\" has been issued.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.8 Click "OK" button on the "Add Entry" dialog.
			addEntryDialog.OK ();
			procedureLogger.ExpectedResult ("The \"Add Entry\" dialog closes.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.9 Check the count of columns and rows of datagrid.
			// if pass TreeScope.Descendant to FindAll method as the first parameter,
			// it would failed due to "out of range", that's the reason why use TreeScope.Childen here.
			var rightPane = window.Find<Pane> ().Find<Pane> (Direction.Vertical, 0).Find<Pane> (Direction.Horizental, 0);
			var dataGrid = rightPane.Find<DataGrid> ();
			procedureLogger.Action ("Check the count of columns of datagrid.");
			procedureLogger.ExpectedResult ("The count of columns of datagrid is 11.");
			Assert.AreEqual (11, dataGrid.ColumnCount);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the count of rows of datagrid.");
			procedureLogger.ExpectedResult ("The count of rows of datagrid is 2.");
			Assert.AreEqual (2, dataGrid.RowCount);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.10 Click "Add Entry" button on the toolstripbar
			//BUG574620: Button recognized as SplitButton on Linux, but it's Button on Windows
			//BUG576050: The splitbutton's Invoke method doesn't work
			addEntryButton.Find<MenuItem> ().Click ();
			procedureLogger.ExpectedResult ("The \"Add Entry\" dialog appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.11 Input the "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" into the title edit
			addEntryDialog = window.Find<Window> ("Add Entry");
			var titleEdit = addEntryDialog.Find<Edit> ("Title:");
			titleEdit.SetValue("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
			procedureLogger.ExpectedResult ("\"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789\" has been issued.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.12 Click "OK" button on the "Add Entry" dialog
			//BUG571799 - [uiaclient-Winforms]：The dialog who has parent has been found twice
			addEntryDialog.OK ();
			procedureLogger.ExpectedResult ("The \"Add Entry\" dialog closes.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.13 Re-check the count of columns and rows of datagrid.
			procedureLogger.Action ("Check the count of columns of datagrid.");
			procedureLogger.ExpectedResult ("The count of columns of datagrid is 11.");
			Assert.AreEqual (11, dataGrid.ColumnCount);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check the count of rows of datagrid.");
			procedureLogger.ExpectedResult ("The count of rows of datagrid is 3.");
			Assert.AreEqual (3, dataGrid.RowCount);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.14 Get the (0,0) element of the datagrid , check if it is "Sample Entry"
			//BUG576455 All the "Text" controls are recognized as "Edit" on Linux
			//var sampleText = dataGrid.Find<Text> ("Sample Entry").AutomationElement;
			//var sampleText = dataGrid.Find<Edit> ("Sample Entry").AutomationElement;
			//BUG578244: The datagrid's GetItem method can't be run on Linux.
			//var entryText = dataGrid.GetItem (0, 0);
			//procedureLogger.ExpectedResult ("The (0,0) item of the datagrid is \"Sample Entry\".");
			//Assert.AreEqual (sampleText, entryText);
			//Thread.Sleep (Config.Instance.ShortDelay);

			//104.15 Resize window
			//Resize the window smaller in order to make horizontal scroll bar could be displayed.
			window.Resize (200, 300);
			procedureLogger.ExpectedResult ("The window is set to be 200 width, 300 height.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.16 Check HorizontallyScrollable and VerticallyScrollable of dataGrid
			procedureLogger.Action ("Check HorizontallyScrollable.");
			procedureLogger.ExpectedResult ("The value of HorizontallyScrollable property is true.");
			Assert.AreEqual (true, dataGrid.HorizontallyScrollable);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check VerticallyScrollable.");
			procedureLogger.ExpectedResult ("The value of VerticallyScrollable property is false.");
			Assert.AreEqual (false, dataGrid.VerticallyScrollable);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.17 Set the scroll bar horizontal percent to 0
			dataGrid.SetScrollPercent (0, -1);
			procedureLogger.ExpectedResult ("The horizontal percentage of scroll bar is set to 0.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.18 Check HorizontalScrollPercent and VerticalScrollPercent
			procedureLogger.Action ("Check HorizontalScrollPercent.");
			procedureLogger.ExpectedResult ("The value of HorizontalScrollPercent is 0.");
			Assert.AreEqual (0, dataGrid.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check VerticalScrollPercent.");
			procedureLogger.ExpectedResult ("The value of VerticalScrollPercent is -1.");
			Assert.AreEqual (-1, dataGrid.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.19 Set the horizontal scroll bar large increment
			//BUG580431: [UIAClient-Winforms] Scroll bar of datagrid could not be detected on Linux
			//dataGrid.Scroll (ScrollAmount.LargeIncrement, ScrollAmount.NoAmount);
			//procedureLogger.ExpectedResult ("The horizontal scroll bar scroll large increment.");
			//Assert.AreEqual (43, dataGrid.HorizontalScrollPercent);
			//Thread.Sleep (Config.Instance.ShortDelay);

			//104.20 Set the horizontal scroll bar large decrement
			dataGrid.Scroll (ScrollAmount.LargeDecrement, ScrollAmount.NoAmount);
			procedureLogger.ExpectedResult ("The horizontal scroll bar scroll large decrement.");
			Assert.AreEqual (0, dataGrid.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.21 Set the horizontal scroll bar small increment
			//BUG580431: [UIAClient-Winforms] Scroll bar of datagrid could not be detected on Linux
			//dataGrid.ScrollHorizontal (ScrollAmount.SmallIncrement);
			//procedureLogger.ExpectedResult ("The horizontal scroll bar scroll small increment.");
			//Assert.AreEqual (1, dataGrid.HorizontalScrollPercent);
			//Thread.Sleep (Config.Instance.ShortDelay);

			//104.22 Set the horizontal scroll bar small decrement
			dataGrid.ScrollHorizontal (ScrollAmount.SmallDecrement);
			procedureLogger.ExpectedResult ("The horizontal scroll bar scroll small decrement.");
			Assert.AreEqual (0, dataGrid.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.MediumDelay);

			//104.23 Check HorizontalViewSize of dataGrid
			//BUG580433: [UIAClient-Winforms] The window is same size on Window and Linux,
			//but the horizontal or vertical view size is not identical
			//procedureLogger.Action ("Check HorizontalViewSize.");
			//procedureLogger.ExpectedResult ("The value of HorizontalViewSize is.");
			//Assert.AreEqual (, dataGrid.HorizontalViewSize);
			//Thread.Sleep (Config.Instance.MediumDelay);

			//104.24 Check VerticalViewSize of dataGrid
			procedureLogger.Action ("Check VerticalViewSize.");
			procedureLogger.ExpectedResult ("The value of VerticalViewSize is 100.");
			Assert.AreEqual (100, dataGrid.VerticalViewSize);
			Thread.Sleep (Config.Instance.MediumDelay);

			//104.25 Set current view of dataGrid
			//BUG580447: The viewId is difference between on Linux and Windows
			dataGrid.SetCurrentView (0);
			procedureLogger.ExpectedResult ("The current view of dataGrid is 0.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.26 Check current view of dataGrid
			procedureLogger.Action ("Check CurrentView.");
			procedureLogger.ExpectedResult ("The value of CurrentView property is 0.");
			Assert.AreEqual (0, dataGrid.CurrentView);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.27 Check the view name of datagrid.
			var viewName = dataGrid.GetViewName (0);
			procedureLogger.ExpectedResult ("The current view name is \"Icons\".");
			Assert.AreEqual ("Icons", viewName);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.28 Check Column, ColumnSpan, Row, RowSpan ContainingGrid of each data item.
			var dataItems = dataGrid.FindAll<DataItem> ();
			for (int i = 0; i < dataItems.Length; i++) {
				//NOTE: Microsoft doesn't implement GridItemPattern for DataItem here, but you can call the methods
				//of GridItemPattern, does it intend to do, or a bug?
				procedureLogger.Action (string.Format("Check Column of {0}.", dataItems[i].NameAndType));
				procedureLogger.ExpectedResult (string.Format ("The Column of {0} is 0.", dataItems[i].NameAndType));
				Assert.AreEqual (0, dataItems[i].Column);
				Thread.Sleep (Config.Instance.ShortDelay);

				procedureLogger.Action (string.Format("Check ColumnSpan of {0}.", dataItems[i].NameAndType));
				procedureLogger.ExpectedResult (string.Format ("The ColumnSpan of {0} is 1.", dataItems[i].NameAndType));
				Assert.AreEqual (1, dataItems[i].ColumnSpan);
				Thread.Sleep (Config.Instance.ShortDelay);

				procedureLogger.Action (string.Format("Check Row of {0}.", dataItems[i].NameAndType));
				procedureLogger.ExpectedResult (string.Format ("The Row of {0} is {1}.",
				                                               dataItems[i].NameAndType, i.ToString ()));
				Assert.AreEqual (i, dataItems[i].Row);
				Thread.Sleep (Config.Instance.ShortDelay);

				procedureLogger.Action (string.Format("Check RowSpan of {0}.", dataItems[i].NameAndType));
				procedureLogger.ExpectedResult (string.Format ("The RowSpan of {0} is 1.", dataItems[i].NameAndType));
				Assert.AreEqual (1, dataItems[i].RowSpan);
				Thread.Sleep (Config.Instance.ShortDelay);

				procedureLogger.Action (string.Format("Check ContainingGrid of {0}.", dataItems[i].NameAndType));
				procedureLogger.ExpectedResult (string.Format("The ContainingGrid of {0} is the AutomationElement of its parent.",
				                                              dataItems[i].NameAndType));
				Assert.AreEqual (dataGrid.AutomationElement, dataItems[i].ContainingGrid);
				Thread.Sleep (Config.Instance.ShortDelay);
			}

			//104.29 Check Column, ColumnSpan, Row, RowSpan, ContainingGrid,
			// ColumnHeaderItems, RowHeaderItems of an Edit.
			//BUG576455 All the "Text" controls are recognized as "Edit" on Linux
			//var sampleText = dataGrid.Find<Text> ("Sample Entry");
			var sampleText = dataGrid.Find<Edit> ("Sample Entry");
			var sampleDataItem = dataGrid.Find<DataItem> ("Sample Entry");
			procedureLogger.Action ("Check Column.");
			procedureLogger.ExpectedResult ("The value of Column is 0.");
			Assert.AreEqual (0, sampleText.Column);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check ColumnSpan.");
			procedureLogger.ExpectedResult ("The value of ColumnSpan is 1.");
			Assert.AreEqual (1, sampleText.ColumnSpan);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check Row.");
			procedureLogger.ExpectedResult ("The value of Row is 0.");
			Assert.AreEqual (0, sampleText.Row);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check RowSpan.");
			procedureLogger.ExpectedResult ("The value of RowSpan is 1.");
			Assert.AreEqual (1, sampleText.RowSpan);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check ContainingGrid.");
			procedureLogger.ExpectedResult ("The value of ContainingGrid is the AutomationElement of its parent.");
			Assert.AreEqual (sampleDataItem.AutomationElement, sampleText.ContainingGrid);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check ColumnHeaderItems.");
			procedureLogger.ExpectedResult ("The value of ColumnHeaderItems is \"Title\".");
			Assert.AreEqual ("Title", sampleText.ColumnHeaderItems[0].Current.Name);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check RowHeaderItems.");
			procedureLogger.ExpectedResult ("The value of RowHeaderItems is null.");
			Assert.AreEqual (0, sampleText.RowHeaderItems.Length);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.30 Click "Tools" menu item on the menu bar
			var menubar = window.Find<MenuBar> ();
			menubar.Find<MenuItem> ("Tools").Click ();
			procedureLogger.ExpectedResult ("The \"Tools\" menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.31 Click "Generate Password.." menu item on the sub menu
			menubar.Find<MenuItem> ("Generate Password...").Click ();
			procedureLogger.ExpectedResult ("The \"Password Generator\" window appears.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.32 Select "Preview" Tab item
			var passwdWindow = window.Find<Window> ("Password Generator");
			var tabItemPreview = passwdWindow.Find<TabItem> ("Preview");
			tabItemPreview.Select ();
			procedureLogger.ExpectedResult ("The \"Preview\" tab selected.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.33 Check VerticallyScrollable and HorizontallyScrollable of passwordDocument
			var passwordDocument = tabItemPreview.Find<Document> ();
			procedureLogger.Action ("Check VerticallyScrollable.");
			procedureLogger.ExpectedResult ("The value of VerticallyScrollable is true.");
			Assert.AreEqual (true, passwordDocument.VerticallyScrollable);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check HorizontallyScrollable.");
			procedureLogger.ExpectedResult ("The value of HorizontallyScrollable is false.");
			Assert.AreEqual (false, passwordDocument.HorizontallyScrollable);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.34 Set percentage of the vertical scroll bar to 0
			passwordDocument.SetScrollPercent (-1, 0);
			procedureLogger.ExpectedResult ("The vertical percentage of scroll bar is set to 0.");;
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.35 Check VerticalScrollPercent and HorizontalScrollPercent of passwordDocument
			procedureLogger.Action ("Check VerticalScrollPercent.");
			procedureLogger.ExpectedResult ("The value of VerticalScrollPercent is 0.");
			Assert.AreEqual (0, passwordDocument.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			procedureLogger.Action ("Check HorizontalScrollPercent.");
			procedureLogger.ExpectedResult ("The value of HorizontalScrollPercent is -1.");
			Assert.AreEqual (-1, passwordDocument.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.36 Set the vertical scroll bar large increment
			//BUG580452: [UIAClient-Winforms] The window is same size on Window and Linux,
			//but the horizontal or vertical scroll percentage is not identical
			//passwordDocument.Scroll (ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
			//procedureLogger.ExpectedResult ("The vertical scroll bar scroll large increment.");
			//Assert.AreEqual (24, passwordDocument.VerticalScrollPercent);
			//Thread.Sleep (Config.Instance.ShortDelay);

			//104.37 Set the vertical scroll bar large decrement
			passwordDocument.Scroll (ScrollAmount.NoAmount, ScrollAmount.LargeDecrement);
			procedureLogger.ExpectedResult ("The vertical scroll bar scroll large decrement.");
			Assert.AreEqual (0, passwordDocument.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.38 Set the vertical scroll bar small increment
			//BUG580452: [UIAClient-Winforms] The window is same size on Window and Linux,
			//but the horizontal or vertical scroll percentage is not identical
			//passwordDocument.ScrollVertical (ScrollAmount.SmallIncrement);
			//procedureLogger.ExpectedResult ("The vertical scroll bar scroll small increment.");
			//Assert.AreEqual (, passwordDocument.VerticalScrollPercent);
			//Thread.Sleep (Config.Instance.ShortDelay);

			//104.39 Set the vertical scroll bar small decrement
			passwordDocument.ScrollVertical (ScrollAmount.SmallDecrement);
			procedureLogger.ExpectedResult ("The vertical scroll bar scroll small decrement.");
			Assert.AreEqual (0, passwordDocument.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.40 Set the Scroll vertica percent to 100
			passwordDocument.SetScrollPercent (-1, 100);
			procedureLogger.ExpectedResult ("The vertical percentage of scroll bar is set to 100.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.41 Check VerticalScrollPercent of passwordDocument
			procedureLogger.Action ("Check VerticalScrollPercent.");
			procedureLogger.ExpectedResult ("The value of VerticalScrollPercent is 100.");
			Assert.AreEqual (100, passwordDocument.VerticalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.42 Check VerticalViewSize of passwordDocument
			//BUG580433: [UIAClient-Winforms] The window is same size on Window and Linux,
			//but the horizontal or vertical view size is not identical
			//procedureLogger.Action ("Check VerticalViewSize.");
			//procedureLogger.ExpectedResult ("The value of VerticalViewSize is.");
			//Assert.AreEqual(, passwordDocument.VerticalViewSize);
			//Thread.Sleep (Config.Instance.ShortDelay);

			//104.43 Check HorizontalViewSize of passwordDocument
			procedureLogger.Action ("Check HorizontalViewSize.");
			procedureLogger.ExpectedResult ("The value of HorizontalViewSize is 100.");
			Assert.AreEqual(100, passwordDocument.HorizontalViewSize);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.44 Check ColumnHeaders of dataGrid
			Assert.AreEqual (dataGrid.ColumnHeaders, dataGrid.GetColumnHeaders ());
			procedureLogger.ExpectedResult (string.Format("The ColumnHeaders is {0}.", dataGrid.ColumnHeaders));
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.45 Check RowHeaders of dataGrid
			Assert.AreEqual (0, dataGrid.GetRowHeaders ().Length);
			procedureLogger.ExpectedResult ("The count of RowHeaders is 0.");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.46 Check RowOrColumnMajor of dataGrid
			procedureLogger.Action ("Check RowOrColumnMajor.");
			procedureLogger.ExpectedResult ("The RowOrColumnMajor is \"RowMajor\".");
			Assert.AreEqual (RowOrColumnMajor.RowMajor, dataGrid.RowOrColumnMajor);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.47 Close "Password Generator" window
			passwdWindow.OK ();
			procedureLogger.ExpectedResult ("The \"Password Generator\" window closes.");
			Thread.Sleep (Config.Instance.ShortDelay);
		}
	}
}