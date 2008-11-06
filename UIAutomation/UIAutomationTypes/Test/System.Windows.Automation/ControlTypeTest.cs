
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
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation {
	
	[TestFixture]
	public class ControlTypeTest {

		[Test]
		public void ButtonTest ()
		{
			ControlType myButton = ControlType.Button;
			Assert.IsNotNull (
				myButton,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50000,
				myButton.Id,
				"Id");
			Assert.AreEqual (
				"button",
				myButton.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Button",
				myButton.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myButton,
				ControlType.LookupById (myButton.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myButton.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myButton.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {InvokePatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myButton.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void CalendarTest ()
		{
			ControlType myCalendar = ControlType.Calendar;
			Assert.IsNotNull (
				myCalendar,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50001,
				myCalendar.Id,
				"Id");
			Assert.AreEqual (
				"calendar",
				myCalendar.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Calendar",
				myCalendar.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myCalendar,
				ControlType.LookupById (myCalendar.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myCalendar.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myCalendar.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {GridPatternIdentifiers.Pattern, ValuePatternIdentifiers.Pattern, SelectionPatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myCalendar.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void CheckBoxTest ()
		{
			ControlType myCheckBox = ControlType.CheckBox;
			Assert.IsNotNull (
				myCheckBox,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50002,
				myCheckBox.Id,
				"Id");
			Assert.AreEqual (
				"check box",
				myCheckBox.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.CheckBox",
				myCheckBox.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myCheckBox,
				ControlType.LookupById (myCheckBox.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myCheckBox.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myCheckBox.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {TogglePatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myCheckBox.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void ComboBoxTest ()
		{
			ControlType myComboBox = ControlType.ComboBox;
			Assert.IsNotNull (
				myComboBox,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50003,
				myComboBox.Id,
				"Id");
			Assert.AreEqual (
				"combo box",
				myComboBox.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.ComboBox",
				myComboBox.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myComboBox,
				ControlType.LookupById (myComboBox.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myComboBox.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myComboBox.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {SelectionPatternIdentifiers.Pattern, ExpandCollapsePatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myComboBox.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void EditTest ()
		{
			ControlType myEdit = ControlType.Edit;
			Assert.IsNotNull (
				myEdit,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50004,
				myEdit.Id,
				"Id");
			Assert.AreEqual (
				"edit",
				myEdit.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Edit",
				myEdit.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myEdit,
				ControlType.LookupById (myEdit.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myEdit.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myEdit.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {ValuePatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myEdit.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void HyperlinkTest ()
		{
			ControlType myHyperlink = ControlType.Hyperlink;
			Assert.IsNotNull (
				myHyperlink,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50005,
				myHyperlink.Id,
				"Id");
			Assert.AreEqual (
				"hyperlink",
				myHyperlink.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Hyperlink",
				myHyperlink.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myHyperlink,
				ControlType.LookupById (myHyperlink.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myHyperlink.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myHyperlink.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {InvokePatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myHyperlink.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void ImageTest ()
		{
			ControlType myImage = ControlType.Image;
			Assert.IsNotNull (
				myImage,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50006,
				myImage.Id,
				"Id");
			Assert.AreEqual (
				"image",
				myImage.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Image",
				myImage.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myImage,
				ControlType.LookupById (myImage.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myImage.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myImage.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myImage.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void ListItemTest ()
		{
			ControlType myListItem = ControlType.ListItem;
			Assert.IsNotNull (
				myListItem,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50007,
				myListItem.Id,
				"Id");
			Assert.AreEqual (
				"list item",
				myListItem.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.ListItem",
				myListItem.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myListItem,
				ControlType.LookupById (myListItem.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myListItem.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myListItem.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {SelectionItemPatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myListItem.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void ListTest ()
		{
			ControlType myList = ControlType.List;
			Assert.IsNotNull (
				myList,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50008,
				myList.Id,
				"Id");
			Assert.AreEqual (
				"list view",
				myList.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.List",
				myList.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myList,
				ControlType.LookupById (myList.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myList.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myList.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {SelectionPatternIdentifiers.Pattern, TablePatternIdentifiers.Pattern, GridPatternIdentifiers.Pattern, MultipleViewPatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myList.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void MenuTest ()
		{
			ControlType myMenu = ControlType.Menu;
			Assert.IsNotNull (
				myMenu,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50009,
				myMenu.Id,
				"Id");
			Assert.AreEqual (
				"menu",
				myMenu.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Menu",
				myMenu.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myMenu,
				ControlType.LookupById (myMenu.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myMenu.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myMenu.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myMenu.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void MenuBarTest ()
		{
			ControlType myMenuBar = ControlType.MenuBar;
			Assert.IsNotNull (
				myMenuBar,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50010,
				myMenuBar.Id,
				"Id");
			Assert.AreEqual (
				"menu bar",
				myMenuBar.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.MenuBar",
				myMenuBar.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myMenuBar,
				ControlType.LookupById (myMenuBar.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myMenuBar.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myMenuBar.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myMenuBar.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void MenuItemTest ()
		{
			ControlType myMenuItem = ControlType.MenuItem;
			Assert.IsNotNull (
				myMenuItem,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50011,
				myMenuItem.Id,
				"Id");
			Assert.AreEqual (
				"menu item",
				myMenuItem.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.MenuItem",
				myMenuItem.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myMenuItem,
				ControlType.LookupById (myMenuItem.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myMenuItem.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myMenuItem.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {InvokePatternIdentifiers.Pattern}, 
				new AutomationIdentifier [] {ExpandCollapsePatternIdentifiers.Pattern}, 
				new AutomationIdentifier [] {TogglePatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myMenuItem.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void ProgressBarTest ()
		{
			ControlType myProgressBar = ControlType.ProgressBar;
			Assert.IsNotNull (
				myProgressBar,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50012,
				myProgressBar.Id,
				"Id");
			Assert.AreEqual (
				"progress bar",
				myProgressBar.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.ProgressBar",
				myProgressBar.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myProgressBar,
				ControlType.LookupById (myProgressBar.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myProgressBar.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myProgressBar.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {ValuePatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myProgressBar.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void RadioButtonTest ()
		{
			ControlType myRadioButton = ControlType.RadioButton;
			Assert.IsNotNull (
				myRadioButton,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50013,
				myRadioButton.Id,
				"Id");
			Assert.AreEqual (
				"radio button",
				myRadioButton.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.RadioButton",
				myRadioButton.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myRadioButton,
				ControlType.LookupById (myRadioButton.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myRadioButton.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myRadioButton.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myRadioButton.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void ScrollBarTest ()
		{
			ControlType myScrollBar = ControlType.ScrollBar;
			Assert.IsNotNull (
				myScrollBar,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50014,
				myScrollBar.Id,
				"Id");
			Assert.AreEqual (
				"scroll bar",
				myScrollBar.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.ScrollBar",
				myScrollBar.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myScrollBar,
				ControlType.LookupById (myScrollBar.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myScrollBar.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myScrollBar.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myScrollBar.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void SliderTest ()
		{
			ControlType mySlider = ControlType.Slider;
			Assert.IsNotNull (
				mySlider,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50015,
				mySlider.Id,
				"Id");
			Assert.AreEqual (
				"slider",
				mySlider.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Slider",
				mySlider.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				mySlider,
				ControlType.LookupById (mySlider.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				mySlider.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				mySlider.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {RangeValuePatternIdentifiers.Pattern}, 
				new AutomationIdentifier [] {SelectionPatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				mySlider.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void SpinnerTest ()
		{
			ControlType mySpinner = ControlType.Spinner;
			Assert.IsNotNull (
				mySpinner,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50016,
				mySpinner.Id,
				"Id");
			Assert.AreEqual (
				"spinner",
				mySpinner.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Spinner",
				mySpinner.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				mySpinner,
				ControlType.LookupById (mySpinner.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				mySpinner.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				mySpinner.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {RangeValuePatternIdentifiers.Pattern}, 
				new AutomationIdentifier [] {SelectionPatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				mySpinner.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void StatusBarTest ()
		{
			ControlType myStatusBar = ControlType.StatusBar;
			Assert.IsNotNull (
				myStatusBar,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50017,
				myStatusBar.Id,
				"Id");
			Assert.AreEqual (
				"status bar",
				myStatusBar.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.StatusBar",
				myStatusBar.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myStatusBar,
				ControlType.LookupById (myStatusBar.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myStatusBar.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myStatusBar.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myStatusBar.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void TabTest ()
		{
			ControlType myTab = ControlType.Tab;
			Assert.IsNotNull (
				myTab,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50018,
				myTab.Id,
				"Id");
			Assert.AreEqual (
				"tab",
				myTab.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Tab",
				myTab.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myTab,
				ControlType.LookupById (myTab.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myTab.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myTab.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myTab.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void TabItemTest ()
		{
			ControlType myTabItem = ControlType.TabItem;
			Assert.IsNotNull (
				myTabItem,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50019,
				myTabItem.Id,
				"Id");
			Assert.AreEqual (
				"tab item",
				myTabItem.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.TabItem",
				myTabItem.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myTabItem,
				ControlType.LookupById (myTabItem.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myTabItem.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myTabItem.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myTabItem.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void TextTest ()
		{
			ControlType myText = ControlType.Text;
			Assert.IsNotNull (
				myText,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50020,
				myText.Id,
				"Id");
			Assert.AreEqual (
				"text",
				myText.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Text",
				myText.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myText,
				ControlType.LookupById (myText.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myText.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myText.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myText.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void ToolBarTest ()
		{
			ControlType myToolBar = ControlType.ToolBar;
			Assert.IsNotNull (
				myToolBar,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50021,
				myToolBar.Id,
				"Id");
			Assert.AreEqual (
				"tool bar",
				myToolBar.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.ToolBar",
				myToolBar.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myToolBar,
				ControlType.LookupById (myToolBar.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myToolBar.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myToolBar.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myToolBar.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void ToolTipTest ()
		{
			ControlType myToolTip = ControlType.ToolTip;
			Assert.IsNotNull (
				myToolTip,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50022,
				myToolTip.Id,
				"Id");
			Assert.AreEqual (
				"tool tip",
				myToolTip.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.ToolTip",
				myToolTip.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myToolTip,
				ControlType.LookupById (myToolTip.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myToolTip.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myToolTip.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myToolTip.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void TreeTest ()
		{
			ControlType myTree = ControlType.Tree;
			Assert.IsNotNull (
				myTree,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50023,
				myTree.Id,
				"Id");
			Assert.AreEqual (
				"tree view",
				myTree.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Tree",
				myTree.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myTree,
				ControlType.LookupById (myTree.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myTree.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myTree.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myTree.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void TreeItemTest ()
		{
			ControlType myTreeItem = ControlType.TreeItem;
			Assert.IsNotNull (
				myTreeItem,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50024,
				myTreeItem.Id,
				"Id");
			Assert.AreEqual (
				"tree view item",
				myTreeItem.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.TreeItem",
				myTreeItem.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myTreeItem,
				ControlType.LookupById (myTreeItem.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myTreeItem.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myTreeItem.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myTreeItem.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void CustomTest ()
		{
			ControlType myCustom = ControlType.Custom;
			Assert.IsNotNull (
				myCustom,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50025,
				myCustom.Id,
				"Id");
			Assert.AreEqual (
				"custom",
				myCustom.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Custom",
				myCustom.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myCustom,
				ControlType.LookupById (myCustom.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myCustom.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myCustom.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myCustom.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void GroupTest ()
		{
			ControlType myGroup = ControlType.Group;
			Assert.IsNotNull (
				myGroup,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50026,
				myGroup.Id,
				"Id");
			Assert.AreEqual (
				"group",
				myGroup.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Group",
				myGroup.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myGroup,
				ControlType.LookupById (myGroup.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myGroup.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myGroup.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myGroup.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void ThumbTest ()
		{
			ControlType myThumb = ControlType.Thumb;
			Assert.IsNotNull (
				myThumb,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50027,
				myThumb.Id,
				"Id");
			Assert.AreEqual (
				"thumb",
				myThumb.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Thumb",
				myThumb.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myThumb,
				ControlType.LookupById (myThumb.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myThumb.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myThumb.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myThumb.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void DataGridTest ()
		{
			ControlType myDataGrid = ControlType.DataGrid;
			Assert.IsNotNull (
				myDataGrid,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50028,
				myDataGrid.Id,
				"Id");
			Assert.AreEqual (
				"datagrid",
				myDataGrid.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.DataGrid",
				myDataGrid.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myDataGrid,
				ControlType.LookupById (myDataGrid.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myDataGrid.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myDataGrid.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {GridPatternIdentifiers.Pattern}, 
				new AutomationIdentifier [] {SelectionPatternIdentifiers.Pattern}, 
				new AutomationIdentifier [] {TablePatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myDataGrid.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void DataItemTest ()
		{
			ControlType myDataItem = ControlType.DataItem;
			Assert.IsNotNull (
				myDataItem,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50029,
				myDataItem.Id,
				"Id");
			Assert.AreEqual (
				"dataitem",
				myDataItem.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.DataItem",
				myDataItem.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myDataItem,
				ControlType.LookupById (myDataItem.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myDataItem.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myDataItem.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {SelectionItemPatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myDataItem.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void DocumentTest ()
		{
			ControlType myDocument = ControlType.Document;
			Assert.IsNotNull (
				myDocument,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50030,
				myDocument.Id,
				"Id");
			Assert.AreEqual (
				"document",
				myDocument.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Document",
				myDocument.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myDocument,
				ControlType.LookupById (myDocument.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{ValuePatternIdentifiers.Pattern};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myDocument.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myDocument.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {ScrollPatternIdentifiers.Pattern}, 
				new AutomationIdentifier [] {TextPatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myDocument.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void SplitButtonTest ()
		{
			ControlType mySplitButton = ControlType.SplitButton;
			Assert.IsNotNull (
				mySplitButton,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50031,
				mySplitButton.Id,
				"Id");
			Assert.AreEqual (
				"split button",
				mySplitButton.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.SplitButton",
				mySplitButton.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				mySplitButton,
				ControlType.LookupById (mySplitButton.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				mySplitButton.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				mySplitButton.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {InvokePatternIdentifiers.Pattern}, 
				new AutomationIdentifier [] {ExpandCollapsePatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				mySplitButton.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void WindowTest ()
		{
			ControlType myWindow = ControlType.Window;
			Assert.IsNotNull (
				myWindow,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50032,
				myWindow.Id,
				"Id");
			Assert.AreEqual (
				"window",
				myWindow.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Window",
				myWindow.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myWindow,
				ControlType.LookupById (myWindow.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myWindow.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myWindow.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {TransformPatternIdentifiers.Pattern}, 
				new AutomationIdentifier [] {WindowPatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myWindow.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void PaneTest ()
		{
			ControlType myPane = ControlType.Pane;
			Assert.IsNotNull (
				myPane,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50033,
				myPane.Id,
				"Id");
			Assert.AreEqual (
				"pane",
				myPane.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Pane",
				myPane.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myPane,
				ControlType.LookupById (myPane.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myPane.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myPane.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {TransformPatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myPane.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void HeaderTest ()
		{
			ControlType myHeader = ControlType.Header;
			Assert.IsNotNull (
				myHeader,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50034,
				myHeader.Id,
				"Id");
			Assert.AreEqual (
				"header",
				myHeader.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Header",
				myHeader.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myHeader,
				ControlType.LookupById (myHeader.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myHeader.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myHeader.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myHeader.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void HeaderItemTest ()
		{
			ControlType myHeaderItem = ControlType.HeaderItem;
			Assert.IsNotNull (
				myHeaderItem,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50035,
				myHeaderItem.Id,
				"Id");
			Assert.AreEqual (
				"header item",
				myHeaderItem.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.HeaderItem",
				myHeaderItem.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myHeaderItem,
				ControlType.LookupById (myHeaderItem.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myHeaderItem.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myHeaderItem.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myHeaderItem.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void TableTest ()
		{
			ControlType myTable = ControlType.Table;
			Assert.IsNotNull (
				myTable,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50036,
				myTable.Id,
				"Id");
			Assert.AreEqual (
				"table",
				myTable.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Table",
				myTable.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myTable,
				ControlType.LookupById (myTable.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myTable.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myTable.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {
				new AutomationIdentifier [] {GridPatternIdentifiers.Pattern}, 
				new AutomationIdentifier [] {SelectionPatternIdentifiers.Pattern}, 
				new AutomationIdentifier [] {TablePatternIdentifiers.Pattern}};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myTable.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void TitleBarTest ()
		{
			ControlType myTitleBar = ControlType.TitleBar;
			Assert.IsNotNull (
				myTitleBar,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50037,
				myTitleBar.Id,
				"Id");
			Assert.AreEqual (
				"title bar",
				myTitleBar.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.TitleBar",
				myTitleBar.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myTitleBar,
				ControlType.LookupById (myTitleBar.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				myTitleBar.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				myTitleBar.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				myTitleBar.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		[Test]
		public void SeparatorTest ()
		{
			ControlType mySeparator = ControlType.Separator;
			Assert.IsNotNull (
				mySeparator,
				"Pattern field must not be null.");
			Assert.AreEqual (
				50038,
				mySeparator.Id,
				"Id");
			Assert.AreEqual (
				"separator",
				mySeparator.LocalizedControlType,
				"LocalizedControlType");
			Assert.AreEqual (
				"ControlType.Separator",
				mySeparator.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				mySeparator,
				ControlType.LookupById (mySeparator.Id),
				"LookupById");
			AutomationIdentifier [] expectedNeverSupportedPatternIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedNeverSupportedPatternIds,
				mySeparator.GetNeverSupportedPatterns (),
				"NeverSupportedPatterns: ");
			AutomationIdentifier [] expectedRequiredPropertyIds =
				{};
			VerifyAutomationIdentifierLists (
				expectedRequiredPropertyIds,
				mySeparator.GetRequiredProperties (),
				"RequiredProperties: ");
			AutomationIdentifier [] [] expectedRequiredPatternSets = new AutomationIdentifier [] [] {};
			VerifyAutomationIdentifierListLists (
				expectedRequiredPatternSets,
				mySeparator.GetRequiredPatternSets (),
				"RequiredPatternSets: ");

		}

		private static void VerifyAutomationIdentifierListLists (AutomationIdentifier [] [] expectedIdentifierIdsArray, Array actualIdentifiers, string testLabel)
		{
			string expectedIdentifiers = "{";
			foreach (AutomationIdentifier [] idArray in expectedIdentifierIdsArray) {
				if (idArray.Length > 0) {
					expectedIdentifiers += " {";
					List<string> arrayIds = new List<string> ();
					//IEnumerable<string> arrayIds = from x in idArray where x != null select x.ProgrammaticName;
					foreach (AutomationIdentifier autoId in idArray) {
						if (autoId != null)
							arrayIds.Add (autoId.ProgrammaticName);
						else
							arrayIds.Add ("[unknown]");
					}
					expectedIdentifiers += GetCommaSeparatedList (arrayIds);
					expectedIdentifiers += "} ";
				}
			}
			expectedIdentifiers += "}";
			
			List<AutomationIdentifier []> expectedIdentifierIdArrays =
				new List<AutomationIdentifier []> (expectedIdentifierIdsArray);
			Assert.AreEqual (expectedIdentifierIdsArray.Length, actualIdentifiers.Length, testLabel + "Length mismatch.  Expected arrays: " + expectedIdentifiers);
			foreach (Array identifierArray in actualIdentifiers) {
				int matchingIndex = -1;
				for (int i = 0; i < expectedIdentifierIdArrays.Count; i++) {
					AutomationIdentifier [] ids = expectedIdentifierIdArrays [i];
					try {
						VerifyAutomationIdentifierLists (ids, identifierArray, testLabel);
						matchingIndex = i;
						break;
					} catch (AssertionException) { }
				}
				if (matchingIndex >= 0) {
					expectedIdentifierIdArrays.RemoveAt (matchingIndex);
				} else {
					IEnumerable<string> expectedIds = from AutomationIdentifier x in identifierArray select x.ProgrammaticName;	
					string expectedIdList = string.Empty;
					if (identifierArray.Length != 0)
						expectedIdList = GetCommaSeparatedList (expectedIds);
					Assert.Fail (testLabel + "Did not expect array: " + expectedIdList + Environment.NewLine + "Expected arrays:" + expectedIdentifiers);
				}
			}

			if (expectedIdentifierIdArrays.Count > 0)
				Assert.Fail (
					testLabel + "Missed {0} expected arrays.  Here's the first one: {1}",
					expectedIdentifierIdArrays.Count,
					GetCommaSeparatedList (expectedIdentifierIdArrays [0]));
		}

		private static void VerifyAutomationIdentifierLists (AutomationIdentifier [] expectedIdentifierIdsArray, Array actualIdentifiers, string testLabel)
		{
			List<AutomationIdentifier> expectedIdentifierIds =
				new List<AutomationIdentifier> (expectedIdentifierIdsArray);
			
			string expectedIdentifiers = "{";
			if (expectedIdentifierIds.Count > 0) {
				List<string> arrayIds = new List<string> ();
				foreach (AutomationIdentifier autoId in expectedIdentifierIds) {
					if (autoId != null)
						arrayIds.Add (autoId.ProgrammaticName);
					else
						arrayIds.Add ("[unknown]");
				}
				expectedIdentifiers += GetCommaSeparatedList (arrayIds);
			}
			expectedIdentifiers += "}";

			Assert.AreEqual (
				expectedIdentifierIds.Count,
				actualIdentifiers.Length,
				testLabel + "Length mismatch. Expected: " + expectedIdentifiers);
			foreach (AutomationIdentifier identifier in actualIdentifiers) {
				int matchingIndex = -1;
				for (int i = 0; i < expectedIdentifierIds.Count; i++) {
					AutomationIdentifier id = expectedIdentifierIds [i];
					if (identifier.Id == id.Id) {
						matchingIndex = i;
						break;
					}
				}
				if (matchingIndex >= 0)
					expectedIdentifierIds.RemoveAt (matchingIndex);
				else
					Assert.Fail (testLabel + string.Format (
						"Did not expect \"{0}\" with id \"{1}\"",
						identifier.ProgrammaticName,
						identifier.Id));
			}

			if (expectedIdentifierIds.Count > 0) {
				string ids = string.Empty;
				for (int i = 0; i < expectedIdentifierIds.Count; i++) {
					ids += expectedIdentifierIds [i].ToString ();
					if (i < expectedIdentifierIds.Count - 1)
						ids += ", ";
				}
				Assert.Fail (testLabel + "Expected the following additional identifiers: {0}", ids);
			}
		}

		private static string GetCommaSeparatedList<T> (IEnumerable<T> collection)
		{
			string output = string.Empty;
			IList<T> itemList = new List<T> (collection);
			for (int i = 0; i < itemList.Count; i++) {
				output += itemList [i].ToString ();
				if (i < itemList.Count - 1)
					output += ", ";
			}
			return output;
		}

	}
}

