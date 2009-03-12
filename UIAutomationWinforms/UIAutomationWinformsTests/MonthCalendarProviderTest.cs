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
//      Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using System.Windows.Automation;
using System.Collections.Generic;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class MonthCalendarProviderTest : BaseProviderTest
	{
		[SetUp]
		public override void SetUp ()
		{
			base.SetUp ();
			
			oldCulture = Thread.CurrentThread.CurrentCulture;

			// Ensure that we're in the US locale so that we can
			// test Gregorian calendars.
			//
			// Regardless, Mono doesn't support anything else at
			// the moment, but let's just be cautious.
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			currentCalendar = Thread.CurrentThread.CurrentCulture.Calendar;

			daysInWeek = (currentCalendar.AddWeeks (anyGivenSunday, 1) - anyGivenSunday).Days;

			calendar = (MonthCalendar) GetControlInstance ();
			Form.Controls.Add (calendar);
			Form.Show ();

			calendarProvider
				= ProviderFactory.GetProvider (calendar);
		}

		[TearDown]
		public override void TearDown ()
		{
			base.TearDown ();	

			// Restore previously set culture
			Thread.CurrentThread.CurrentCulture = oldCulture;
		}

		[Test]
		public void BasicPropertiesTest ()
		{
			TestProperty (calendarProvider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Calendar.Id);
			
			TestProperty (calendarProvider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "calendar");

			TestProperty (calendarProvider,
			              AutomationElementIdentifiers.NameProperty,
			              calendar.SelectionStart.ToShortDateString ());
		}

		[Test]
		public void NamePropertyEventTest ()
		{	
			bridge.ResetEventLists ();

			object oldName = calendarProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			calendar.SelectionStart = new DateTime(2012, 2, 29);

			AutomationPropertyChangedEventTuple eventInfo = null;
			int eventCount = 0;
			
			foreach (AutomationPropertyChangedEventTuple evnt in bridge.AutomationPropertyChangedEvents) {
				if (evnt.e.Property.Id == AutomationElementIdentifiers.NameProperty.Id) {
					eventCount++;
					eventInfo = evnt;
				}
			}
			Assert.AreEqual (1, eventCount, "event count");

			Assert.AreEqual (oldName,
			                 eventInfo.e.OldValue,
			                 "event old value");

			TestProperty (calendarProvider,
			              AutomationElementIdentifiers.NameProperty,
			              eventInfo.e.NewValue);

			Assert.AreEqual (AutomationElementIdentifiers.AutomationPropertyChangedEvent.Id,
			                 eventInfo.e.EventId.Id,
			                 "event id");
			
			Assert.AreEqual (calendarProvider,
			                 eventInfo.element,
			                 "event element");
		}

		[Test]
		public void NavigationTest ()
		{
			IRawElementProviderFragmentRoot rootProvider
				= (IRawElementProviderFragmentRoot) calendarProvider;

			IRawElementProviderSimple child
				= rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (child, "MonthCalendar has no children");

			TestProperty (child,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.DataGrid.Id);
			TestProperty (child,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "data grid");

			IRawElementProviderSimple header
				= ((IRawElementProviderFragmentRoot) child).Navigate (
					NavigateDirection.FirstChild);
			Assert.IsNotNull (header, "MonthCalendarDataGrid has no children");

			TestProperty (header,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Header.Id);
			TestProperty (header,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "header");

			int numChildren = 0;

			IRawElementProviderSimple headerItem
				= ((IRawElementProviderFragmentRoot) header).Navigate (
						NavigateDirection.FirstChild);
			while (headerItem != null) {
				TestHeaderItem (headerItem, header, numChildren);
				
				numChildren++;
				headerItem = ((IRawElementProviderFragment) headerItem)
					.Navigate (NavigateDirection.NextSibling);
			}

			Assert.AreEqual (daysInWeek, numChildren, "Not returning the correct number of days in a week");

			int numItems = 0;
			int numButtons = 0;
			
			IRawElementProviderFragmentRoot dataGridProvider;
			IRawElementProviderFragment childItem;

			GetDataGridAndFirstListItem (out dataGridProvider, out childItem);

			List<IRawElementProviderSimple> buttons
				= new List<IRawElementProviderSimple> ();
			do {
				int controlType = (int) childItem.GetPropertyValue (
					AutomationElementIdentifiers.ControlTypeProperty.Id);
				if (controlType == ControlType.ListItem.Id) {
					numItems++;
				} else if (controlType == ControlType.Button.Id) {
					numButtons++;
					buttons.Add (childItem);
				}

				childItem = childItem.Navigate (NavigateDirection.NextSibling);
			} while (childItem != null);

			SelectionRange range = calendar.GetDisplayRange (false);
			Assert.AreEqual ((range.End - range.Start).Days + 1, numItems,
			                 "Don't have the correct number of list items");

			Assert.AreEqual (2, numButtons,
			                 "Don't have the correct number of buttons");

			IRawElementProviderSimple backButton = buttons[0];
			IInvokeProvider invokeProvider
				= backButton.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id)
					 as IInvokeProvider;
			Assert.IsNotNull (invokeProvider,
			                  "Button doesn't implement IInvoke");
			
			calendar.SelectionStart = new DateTime (2000, 1, 1);
			invokeProvider.Invoke ();

			Assert.AreEqual (new DateTime (1999, 12, 1), calendar.SelectionStart,
			                 "Calendar date is incorrect after going backward");
		}

		private void TestHeaderItem (IRawElementProviderSimple headerItem,
		                             IRawElementProviderSimple header,
		                             int index)
		{
			TestProperty (headerItem,
				      AutomationElementIdentifiers.ControlTypeProperty,
				      ControlType.HeaderItem.Id);
			TestProperty (headerItem,
				      AutomationElementIdentifiers.LocalizedControlTypeProperty,
				      "header item");
			TestProperty (headerItem,
				      AutomationElementIdentifiers.NameProperty,
			              anyGivenSunday.AddDays (index).ToString ("ddd"));

			Assert.AreEqual (header, ((IRawElementProviderFragment) headerItem).Navigate (
				NavigateDirection.Parent));
		}

		[Test]
		public void IGridProviderTest ()
		{
			TestGridProvider (calendarProvider);
		}

		[Test]
		public void DataGridIGridProviderTest ()
		{
			IRawElementProviderSimple provider
				= ((IRawElementProviderFragmentRoot) calendarProvider)
					.Navigate (NavigateDirection.FirstChild);
			TestGridProvider (provider);
		}

		public void TestGridProvider (IRawElementProviderSimple provider)
		{
			IGridProvider gridProvider = (IGridProvider)
				provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
			
			Assert.AreEqual (daysInWeek, gridProvider.ColumnCount);
			Assert.AreEqual (6, gridProvider.RowCount);

			DateTime date = calendar.GetDisplayRange (false).Start;
			for (int r = 0; r < 6; r++) {
				for (int c = 0; c < daysInWeek; c++) {
					IRawElementProviderSimple child
						= gridProvider.GetItem (r, c);

					TestProperty (child,
						      AutomationElementIdentifiers.ControlTypeProperty,
						      ControlType.ListItem.Id);
					TestProperty (child,
						      AutomationElementIdentifiers.NameProperty,
					              date.Day.ToString ());

					date = date.AddDays (1);
				}
			}
		}
		
		[Test]
		public void ListItemIGridItemProviderTest ()
		{
			IRawElementProviderFragmentRoot dataGridProvider;
			IRawElementProviderFragment child;

			GetDataGridAndFirstListItem (out dataGridProvider, out child);

			IGridProvider gridProvider = (IGridProvider)
				dataGridProvider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);

			SelectionRange range = calendar.GetDisplayRange (false);

			int numChildren = (range.End - range.Start).Days;
			for (int i = 0; i < numChildren && child != null; i++) {
				TestProperty (child,
					      AutomationElementIdentifiers.ControlTypeProperty,
					      ControlType.ListItem.Id);

				IGridItemProvider itemProvider = child.GetPatternProvider (
					GridItemPatternIdentifiers.Pattern.Id) as IGridItemProvider;
				Assert.IsNotNull (itemProvider,
				                  "Does not implement IGridItemProvider");

				int row = itemProvider.Row;
				int col = itemProvider.Column;
				Assert.AreEqual (gridProvider.GetItem (row, col),
				                 child, "Child found via navigation is reporting incorrect row and/or column values");

				Assert.AreEqual (1, itemProvider.RowSpan,
				                 "Child reporting larger RowSpan than expected");
				Assert.AreEqual (1, itemProvider.ColumnSpan,
				                 "Child reporting larger ColumnSpan than expected");

				Assert.AreEqual (dataGridProvider, itemProvider.ContainingGrid,
				                 "Child's ContainingGrid is not its DataGrid parent.");

				child = ((IRawElementProviderFragment) child)
					.Navigate (NavigateDirection.NextSibling);
			}
		}

		[Test]
		public void ITableProviderTest ()
		{
			IRawElementProviderFragmentRoot dataGridProvider;
			IRawElementProviderFragment child;
			
			GetDataGridAndFirstListItem (out dataGridProvider, out child);

			ITableProvider tableProvider
				= dataGridProvider.GetPatternProvider (TablePatternIdentifiers.Pattern.Id)
					 as ITableProvider;
			Assert.IsNotNull (tableProvider, "Does not implement ITableProvider");

			IRawElementProviderSimple header
				= dataGridProvider.Navigate (NavigateDirection.FirstChild);

			IRawElementProviderSimple[] headerItems
				= tableProvider.GetColumnHeaders ();

			Assert.IsNotNull (headerItems, "Returning null header items");
			Assert.AreEqual (daysInWeek, headerItems.Length,
			                 "Returning incorrect number of header items");

			for (int i = 0; i < headerItems.Length; i++) {
				TestHeaderItem (headerItems[i], header, i);
			}

			IRawElementProviderSimple[] rowHeaders
				= tableProvider.GetRowHeaders ();
			Assert.AreEqual (0, rowHeaders.Length);
		}
	
		[Test]
		public void ListItemITableItemProviderTest ()
		{
			IRawElementProviderFragmentRoot dataGridProvider;
			IRawElementProviderFragment child;

			GetDataGridAndFirstListItem (out dataGridProvider, out child);

			ITableProvider tableProvider
				= (ITableProvider) dataGridProvider.GetPatternProvider (
					TablePatternIdentifiers.Pattern.Id);

			IRawElementProviderSimple[] headerItems
				= tableProvider.GetColumnHeaders ();

			SelectionRange range = calendar.GetDisplayRange (false);

			int numChildren = (range.End - range.Start).Days;
			for (int i = 0; i < numChildren && child != null; i++) {
				TestProperty (child,
					      AutomationElementIdentifiers.ControlTypeProperty,
					      ControlType.ListItem.Id);

				ITableItemProvider itemProvider = child.GetPatternProvider (
					TableItemPatternIdentifiers.Pattern.Id) as ITableItemProvider;
				Assert.IsNotNull (itemProvider,
				                  "Does not implement ITableItemProvider");

				// GetColumnHeaderItems
				IRawElementProviderSimple [] itemHeaderItems
					= itemProvider.GetColumnHeaderItems ();

				Assert.IsNotNull (itemHeaderItems,
				                  "No header items returned");
				Assert.AreEqual (1, itemHeaderItems.Length,
				                 "Too many or too few header items returned");

				MonthCalendarHeaderItemProvider headerItem
					= itemHeaderItems[0] as MonthCalendarHeaderItemProvider;

				Assert.IsNotNull (headerItem,
				                  "Header item is not a MonthCalendarHeaderItemProvider");
				Assert.AreEqual (itemProvider.Column, headerItem.Index,
				                 "Header item returned does not have the same index as item's column");
				Assert.AreEqual (headerItems[itemProvider.Column], headerItem,
				                 "Header item and header item at that index are not equal");

				// GetRowHeaderItems
				IRawElementProviderSimple[] itemRowHeaderItems
					= itemProvider.GetRowHeaderItems ();

				Assert.IsNotNull (itemRowHeaderItems,
				                  "Item provider is returning null for row headers");
				Assert.AreEqual (0, itemRowHeaderItems.Length,
				                 "Item provider is returning more than 0 row headers");

				child = ((IRawElementProviderFragment) child)
					.Navigate (NavigateDirection.NextSibling);
			}
		}
	
		[Test]
		public void ListItemEditProviderTest ()
		{
			IRawElementProviderFragmentRoot dataGridProvider;
			IRawElementProviderFragment listItem;

			GetDataGridAndFirstListItem (out dataGridProvider, out listItem);

			SelectionRange range = calendar.GetDisplayRange (false);
			DateTime date = range.Start;

			int numChildren = (range.End - range.Start).Days;
			for (int i = 0; i < numChildren && listItem != null; i++) {
				// First test listItem itself
				IValueProvider valueProvider = listItem.GetPatternProvider (
					ValuePatternIdentifiers.Pattern.Id) as IValueProvider;
				TestListItemValueProvider (valueProvider, date.Day.ToString ());

				IRawElementProviderFragment textChild
					= ((IRawElementProviderFragmentRoot) listItem)
						.Navigate (NavigateDirection.FirstChild);
				Assert.IsNotNull (textChild, "ListItem has no children");
				TestProperty (textChild,
					      AutomationElementIdentifiers.ControlTypeProperty,
					      ControlType.Edit.Id);
				
				// Then test the text child of listItem
				valueProvider = textChild.GetPatternProvider (
					ValuePatternIdentifiers.Pattern.Id) as IValueProvider;
				TestListItemValueProvider (valueProvider, date.Day.ToString ());

				date = date.AddDays (1);
				listItem = listItem.Navigate (
					NavigateDirection.NextSibling);
			}
		}

		public void TestListItemValueProvider (IValueProvider valueProvider, string expected_val)
		{
			Assert.IsNotNull (valueProvider, "Does not support IValueProvider");
			Assert.AreEqual (expected_val, valueProvider.Value, "Value is not correct");
			Assert.IsTrue (valueProvider.IsReadOnly, "Value is not read only");
		}
	
		[Test]
		public void ISelectionProviderTest ()
		{
			IRawElementProviderFragmentRoot dataGridProvider;
			IRawElementProviderFragment listItem;

			GetDataGridAndFirstListItem (out dataGridProvider, out listItem);

			ISelectionProvider selectionProvider
				= dataGridProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id)
					 as ISelectionProvider;
			Assert.IsNotNull (selectionProvider, "Does not implement ISelectionProvider");

			// Test CanSelectMultiple
			calendar.MaxSelectionCount = 1;
			Assert.IsFalse (selectionProvider.CanSelectMultiple,
			                "CanSelectMultiple returns true when MaxSelectionCount is 1");

			bridge.ResetEventLists ();

			calendar.MaxSelectionCount = 5;
			Assert.IsTrue (selectionProvider.CanSelectMultiple,
			                "CanSelectMultiple returns false when MaxSelectionCount is 5");

			Assert.AreEqual (1,
			                 bridge.GetAutomationPropertyEventCount (SelectionPatternIdentifiers.CanSelectMultipleProperty),
			                 "Event count");

			// Test IsSelectionRequired
			calendar.MaxSelectionCount = 5;
			Assert.IsTrue (selectionProvider.IsSelectionRequired,
			               "IsSelectionRequired returns false when MaxSelectionCount is 5");

			// Test GetSelection and Selection event
			DateTime sel_start = new DateTime (2000, 12, 4);
			DateTime sel_end = new DateTime (2000, 12, 6);

			calendar.MinDate = new DateTime (2000, 12, 1);
			calendar.MaxSelectionCount = 5;

			bridge.ResetEventLists ();

			calendar.SelectionStart = sel_start;
			calendar.SelectionEnd = sel_end;

			Assert.AreEqual (2, bridge.GetAutomationPropertyEventCount (SelectionPatternIdentifiers.SelectionProperty),
			                 "Event count");

			IRawElementProviderSimple[] selectedItems
				= selectionProvider.GetSelection ();
			Assert.IsNotNull (selectedItems, "GetSelection returning null");

			Assert.AreEqual ((sel_end - sel_start).Days + 1,
			                 selectedItems.Length,
			                 "GetSelection Not returning the right number of items");
			int i = 0; 
			DateTime date = sel_start;
			while (date <= sel_end) {
				TestProperty (selectedItems[i],
					      AutomationElementIdentifiers.ControlTypeProperty,
					      ControlType.ListItem.Id);

				TestProperty (selectedItems[i],
				              AutomationElementIdentifiers.NameProperty,
				              date.Day.ToString ());

				date = date.AddDays (1);
				i++;
			}
		}

		[Test]
		public void ListItemISelectionItemProvider_Simple ()
		{
			DateTime sel_start = new DateTime (2000, 12, 4);
			DateTime sel_end = new DateTime (2000, 12, 6);

			calendar.MinDate = new DateTime (2000, 12, 1);
			calendar.MaxSelectionCount = 5;

			calendar.SelectionStart = sel_start;
			calendar.SelectionEnd = sel_end;

			IRawElementProviderFragmentRoot dataGridProvider;
			IRawElementProviderFragment listItem;

			GetDataGridAndFirstListItem (out dataGridProvider, out listItem);

			SelectionRange range = calendar.GetDisplayRange (false);
			DateTime date = range.Start;

			while (date <= range.End && listItem != null) {
				TestProperty (listItem,
					      AutomationElementIdentifiers.ControlTypeProperty,
					      ControlType.ListItem.Id);

				ISelectionItemProvider selectionItemProvider
					= listItem.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id)
						 as ISelectionItemProvider;
				Assert.IsNotNull (selectionItemProvider,
				                  "Does not implement ISelectionItemProvider");

				// Test get_IsSelected
				Assert.AreEqual (selectionItemProvider.IsSelected,
				                 (date >= sel_start && date <= sel_end),
				                 "Item thinks it's selected when it is not or vice versa");

				// Test SelectionContainer
				Assert.AreEqual (selectionItemProvider.SelectionContainer,
				                 dataGridProvider,
				                 "SelectionContainer is not the datagrid");
				
				date = date.AddDays (1);
				listItem = ((IRawElementProviderFragment) listItem)
					.Navigate (NavigateDirection.NextSibling);
			}
		}

		[Test]
		public void ListItemISelectionItemProvider_Complex ()
		{
			calendar.MinDate = new DateTime (2000, 12, 1);
			calendar.SelectionRange = new SelectionRange (new DateTime (2000, 12, 3),
			                                              new DateTime (2000, 12, 3));
			calendar.MaxSelectionCount = 5;

			IRawElementProviderFragmentRoot dataGridProvider;
			IRawElementProviderFragment listItem, secondItem;

			GetDataGridAndFirstListItem (out dataGridProvider, out listItem);

			// Advance until we hit a date in this month, so that
			// we don't jump and invalidate all of our items
			DateTime date = calendar.GetDisplayRange (false).Start;
			while (listItem != null) {
				if (date == calendar.MinDate) {
					break;
				}				

				date = date.AddDays (1);
				listItem = listItem.Navigate (
					NavigateDirection.NextSibling);
			}

			ISelectionItemProvider firstItemProvider
				= listItem.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id)
					 as ISelectionItemProvider;

			Assert.IsFalse (firstItemProvider.IsSelected,
			                "Item reporting to be selected when it shouldn't be");

			firstItemProvider.Select ();
			Assert.IsTrue (firstItemProvider.IsSelected,
			               "Item is reporting not selected when it should be");

			Assert.AreEqual (calendar.SelectionStart, date,
			                 "Calendar returning different start than we just set");
			Assert.AreEqual (calendar.SelectionEnd, date,
			                 "Calendar returning different end than we just set");

			secondItem = listItem.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (secondItem, "First item has no next sibling");

			ISelectionItemProvider secondItemProvider
				= secondItem.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id)
					 as ISelectionItemProvider;
			Assert.IsNotNull (secondItemProvider, "Second item does not implement ISelectionItemProvider");

			secondItemProvider.AddToSelection ();
			Assert.IsTrue (secondItemProvider.IsSelected,
			               "Second item not selected when it should be");

			firstItemProvider.RemoveFromSelection ();
			Assert.IsFalse (firstItemProvider.IsSelected,
			                "Item reporting to be selected when it shouldn't be");
			Assert.IsTrue (secondItemProvider.IsSelected,
			               "Second item not selected when it should be");
		}

		public void GetDataGridAndFirstListItem (out IRawElementProviderFragmentRoot dataGrid,
		                                         out IRawElementProviderFragment firstChild)
		{
			IRawElementProviderFragmentRoot rootProvider
				= (IRawElementProviderFragmentRoot) calendarProvider;
			
			// The datagrid is the MonthCalendar's first child
			dataGrid = (IRawElementProviderFragmentRoot)
				  rootProvider.Navigate (NavigateDirection.FirstChild);
			
			IRawElementProviderFragmentRoot child
				= (IRawElementProviderFragmentRoot) dataGrid.Navigate (
					NavigateDirection.FirstChild);

			// Header is next, but skip that
			firstChild = (IRawElementProviderFragment) child.Navigate (
					NavigateDirection.NextSibling);
		}

		[Test]
		public void IValueProviderTest ()
		{
			object valueProvider
				= calendarProvider.GetPatternProvider (
					ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNull (valueProvider,
			               "Incorrectly implements IValueProvider");
		}

		protected override Control GetControlInstance ()
		{
			return new MonthCalendar ();
		}

		public override void LabeledByAndNamePropertyTest ()
		{
			TestLabeledByAndName (true, false);
		}

		protected override void TestSelectionPattern_GetSelectionMethod (IRawElementProviderSimple provider)
		{
			// FIXME: Instead of overriding this method we should implement ISelectionItemProvider in children
		}

		protected override void TestTablePatternChild (IRawElementProviderSimple provider)
		{
			if (provider.GetType () == typeof (MonthCalendarDataGridProvider)) {
				
				// LAMESPEC:
				//     "The children of this element must implement ITableItemProvider."
				//     Internal DataGrid in Calendar is not implementing
				//     ITableItemProvider either.
				return;
			}
			
			base.TestTablePatternChild (provider);
		}

		[Test]
		public override void AmpersandsAndNameTest ()
		{
			// MonthCalendar doesn't use & in Text
		}

		private MonthCalendar calendar;
		private IRawElementProviderSimple calendarProvider;
		private Calendar currentCalendar;

		private CultureInfo oldCulture;
		private static int daysInWeek;

		private DateTime anyGivenSunday = new DateTime (2008, 12, 7);
	}
}
