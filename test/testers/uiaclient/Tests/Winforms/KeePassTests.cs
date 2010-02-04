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
			Assert.AreEqual ("30", standardIconList.GetSelection ()[0].Current.Name);
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
			procedureLogger.ExpectedResult ("\"mono-a11y\" entered in the \"Master password\" box");
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
			 */			
			
						/*
			 * BUG576050- [uiaclient-winforms]: 
			 * The splitbutton's Invoke method doesn't work
			 */
			procedureLogger.ExpectedResult ("the \"Add Entry\" window appears");
			Thread.Sleep (Config.Instance.ShortDelay);
			
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
			
			/*
			 * BUG576450 - [uiaclient-winforms] The dialog's Window Pattern' 
			 * WindowInteractionState will be different between Windows and Linux 
			 */
			/*
			 * procedureLogger.Action ("Check \"Add Entry\" window's WindowInteractionStateProperty");
			 * Assert.AreEqual (WindowInteractionState.ReadyForUserInteraction, addEntryDialog.WindowInteractionState);
			 * procedureLogger.ExpectedResult ("The window's CanMaximizeProperty should be False");
			 * Thread.Sleep (Config.Instance.ShortDelay);
			 */
			
			procedureLogger.Action ("Check \"Add Entry\" window's WindowVisualStateProperty");
			Assert.AreEqual (WindowVisualState.Normal, addEntryDialog.WindowVisualState);
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
			
			//ssssssssssssssss
			procedureLogger.Action ("Check scroll bar's LargeChangeProperty");
			//Assert.AreEqual (131, (int)scrollBar.LargeChange);
			Assert.AreEqual (146, (int)scrollBar.LargeChange);
			procedureLogger.ExpectedResult ("The scroll bar's large chaged value should be 131");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			procedureLogger.Action ("Check scroll bar's LargeChangeProperty");
			Assert.AreEqual (1, (int)scrollBar.SmallChange);
			procedureLogger.ExpectedResult ("The scroll bar's large chaged value should be 131");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			procedureLogger.Action ("Check scroll bar's Maximum value");
			Assert.AreEqual (362, (int)scrollBar.Maximum);
			procedureLogger.ExpectedResult ("The scroll bar's Maximum value shoule be 362");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			procedureLogger.Action ("Check scroll bar's Minimum value");
			Assert.AreEqual (0, (int)scrollBar.Minimum);
			procedureLogger.ExpectedResult ("The scroll bar's minimum value shoule be 0");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			procedureLogger.Action ("Check scroll bar's value whether equals to 300");
			Assert.AreEqual (300, (int)scrollBar.Value);
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
			//BUG576050 The splitbutton's Invoke method doesn't work
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
			var dataGrid = rightPane.Find<DataGrid> ();
			procedureLogger.Action ("Check the count of columns of datagrid.");
			procedureLogger.ExpectedResult ("The count of columns of datagrid is 11.");
			Assert.AreEqual (11, dataGrid.ColumnCount);
			Thread.Sleep (Config.Instance.ShortDelay);
			
			procedureLogger.Action ("Check the count of columns of datagrid.");
			procedureLogger.ExpectedResult ("The count of columns of datagrid is 2.");
			Assert.AreEqual (2, dataGrid.RowCount);
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.10 Click "Add Entry" button on the toolstripbar
			addEntryButton.Click ();
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
			
			//104.16 Scroll horizontal scrollbar for a large increment.
			dataGrid.Scroll (ScrollAmount.LargeIncrement, ScrollAmount.NoAmount);
			procedureLogger.ExpectedResult ("The horizontal scrollbar scroll large increment.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.17 Scroll horizontal scrollbar for a large decrement.
			dataGrid.Scroll (ScrollAmount.SmallDecrement, ScrollAmount.NoAmount);
			procedureLogger.ExpectedResult ("The horizontal scrollbar scroll large decrement.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.18 Check HorizontalScrollPercent.
			procedureLogger.Action ("Check HorizontalScrollPercent.");
			//procedureLogger.ExpectedResult ("The value of HorizontalScrollPercent is.");
			//Assert.AreEqual (100.0d, dataGrid.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.19 Scroll horizontal scrollbar for a small increment.
			dataGrid.ScrollHorizontal (ScrollAmount.SmallIncrement);
			procedureLogger.ExpectedResult ("The horizontal scrollbar scroll small increment.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.20 Check HorizontalScrollPercent.
			//procedureLogger.Action ("Check HorizontalScrollPercent.");
			//procedureLogger.ExpectedResult ("The value of HorizontalScrollPercent is.");
			//Assert.AreEqual (0.0d, dataGrid.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.21 Scroll horizontal scrollbar for a small decrement.
			dataGrid.ScrollHorizontal (ScrollAmount.SmallDecrement);
			procedureLogger.ExpectedResult ("The horizotal Scrollbar decrease large");
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.22 Set the percentage of horizontal scrollbar to 50%.
			dataGrid.SetScrollPercent (50.0, 0.0);
			procedureLogger.ExpectedResult ("The percentage of horizontal scrollbar is 50%.");
			Assert.AreEqual (50.0, dataGrid.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.22.5 Check HorizontalScrollPercent.
			//procedureLogger.Action ("Check HorizontalScrollPercent.");
			//procedureLogger.ExpectedResult ("The value of HorizontalScrollPercent is 50%..");
			//Assert.AreEqual (50.0, dataGrid.HorizontalScrollPercent);
			Thread.Sleep (Config.Instance.ShortDelay);

			//104.23 Check the data grid's MultipleViewPattern property
			procedureLogger.Action ("Check CurrentView.");
			procedureLogger.ExpectedResult ("The value of CurrentView property is 0.");
			Assert.AreEqual (0, dataGrid.CurrentView);
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.24 Retrieve the view name for the datagrid.
			var viewName = dataGrid.GetViewName (0);
			procedureLogger.ExpectedResult ("The current view name is \"Icon\"");
			Assert.AreEqual ("Icon", viewName);
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.25 Check Column, ColumnSpan, Row, RowSpan ContainingGrid of each data item.
			var dataItems = dataGrid.FindAll<DataItem> ();
			for (int i = 0; i < dataItems.Length; i++) {
				procedureLogger.Action ("Check the column of each data item.");
				procedureLogger.ExpectedResult (string.Format ("The column of {0} is 0.", dataItems[i].NameAndType));
				Assert.AreEqual (0, dataItems[i].Column);
				Thread.Sleep (Config.Instance.ShortDelay);
			
				procedureLogger.Action ("Check ColumnSpan of each data item.");
				procedureLogger.ExpectedResult (string.Format ("The column of {0} is 1.", dataItems[i].NameAndType));
				Assert.AreEqual (1, dataItems[i].ColumnSpan);
				Thread.Sleep (Config.Instance.ShortDelay);
			
				procedureLogger.Action ("Check the row of each data item.");;
				procedureLogger.ExpectedResult (string.Format ("The row of {0} is {1}.",
				                                               dataItems[i].NameAndType, i.ToString ()));
				Assert.AreEqual (i, dataItems[i].Row);
				Thread.Sleep (Config.Instance.ShortDelay);
			
				procedureLogger.Action ("Check RowSpan of each data item.");
				procedureLogger.ExpectedResult (string.Format ("The column of {0} is 1.", dataItems[i].NameAndType));
				Assert.AreEqual (1, dataItems[i].RowSpan);
				Thread.Sleep (Config.Instance.ShortDelay);
			
				procedureLogger.Action ("Check ContainingGrid of each data item.");
				procedureLogger.ExpectedResult ("The ContainingGrid of each data item is the AutomationElement of its parent.");
				Assert.AreEqual (dataGrid.AutomationElement, dataItems[i].ContainingGrid);
				Thread.Sleep (Config.Instance.ShortDelay);
			}
			
			//104.26 Check Column, ColumnSpan, Row, RowSpan, ContainingGrid,
			// ColumnHeaderItems, RowHeaderItems of an Edit.
			//BUG576455 All the "Text" controls are recognized as "Edit" on Linux
			//var sampleText = dataGrid.Find<Text> ("Sample Entry");
			var sampleText = dataGrid.Find<Edit> ("Sample Entry");
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
			Assert.AreEqual (dataGrid.AutomationElement, sampleText.ContainingGrid);
			Thread.Sleep (Config.Instance.ShortDelay);
			
			procedureLogger.Action ("Check ColumnHeaderItems.");
			procedureLogger.ExpectedResult ("The value of ColumnHeaderItems is \"Title\".");
			Assert.AreEqual ("Title", sampleText.ColumnHeaderItems[0].Current.Name);
			Thread.Sleep (Config.Instance.ShortDelay);
			
			procedureLogger.Action ("Check RowHeaderItems.");
			procedureLogger.ExpectedResult ("The value of RowHeaderItems is null.");
			Assert.AreEqual ("", sampleText.RowHeaderItems[0].Current.Name);
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.29 Click "Tools" menu item on the menu bar
			var menubar = window.Find<MenuBar> ();
			menubar.Find<MenuItem> ("Tools").Click ();
			procedureLogger.ExpectedResult ("The \"Tools\" menu opens.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.30 Click "Generate Password.." menu item on the sub menu
			menubar.Find<MenuItem> ("Generate Password...").Click ();
			procedureLogger.ExpectedResult ("The \"Password Generator\" window appears.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.31 Select "Preview" Tab item
			var passwdWindow = window.Find<Window> ("Password Generator");
			var tabItemPreview = passwdWindow.Find<TabItem> ("Preview");
			tabItemPreview.Select ();
			procedureLogger.ExpectedResult ("The \"Preview\" tab selected.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.32 Scroll vertical scrollbar LargeIncrement
			var passwordDocument = tabItemPreview.Find<Document> ();
			passwordDocument.Scroll (ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
			procedureLogger.ExpectedResult ("The vertical scrollbar scroll large increment.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.36 Scroll vertical scrollbar LargeDecrement
			passwordDocument.Scroll (ScrollAmount.NoAmount, ScrollAmount.LargeDecrement);
			procedureLogger.ExpectedResult ("The vertical scrollbar scroll large decrement.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.33 Scroll vertical scrollbar SmallIncrement
			passwordDocument.ScrollVertical (ScrollAmount.SmallIncrement);
			procedureLogger.ExpectedResult ("The vertical scrollbar scroll small increment.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.34 Check the percentage of vertical scrollbar
			//procedureLogger.Action ("Checkthe VerticalScrollPercent.");
			//Assert.AreEqual (, passwordDocument.VerticalScrollPercent);
			//procedureLogger.ExpectedResult ("The value of VerticalScrollPercent is.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.35 Scroll vertical scrollbar SmallDecrement
			passwordDocument.ScrollVertical (ScrollAmount.SmallDecrement);
			procedureLogger.ExpectedResult ("The vertical scrollbar scroll small decrement.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.38 Use SetScrollPercent method make the vertical scrollbar move to (-1, 50)
			passwordDocument.SetScrollPercent (0.0, 50.0);
			procedureLogger.ExpectedResult ("The percentage of vertical scrollbar is 50%.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.39 Check the vertical scrollbar's position
			procedureLogger.Action ("Check VerticalScrollPercent.");
			Assert.AreEqual (50.0, passwordDocument.VerticalScrollPercent);
			procedureLogger.ExpectedResult ("The value of VerticallyScrollable is 50%.");
			Thread.Sleep (Config.Instance.ShortDelay);
			
			//104.40 Close "Password Generator" window
			passwdWindow.OK ();
			procedureLogger.ExpectedResult ("The \"Password Generator\" window closes.");
			Thread.Sleep (Config.Instance.ShortDelay);
		}
	}
}
