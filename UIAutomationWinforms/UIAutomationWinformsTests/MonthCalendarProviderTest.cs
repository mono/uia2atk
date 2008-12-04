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
			
			Assert.AreEqual (anyGivenSunday.AddDays (index).ToString ("ddd"),
					 headerItem.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id),
					 "Day name in header is incorrect");

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
					
					Assert.AreEqual (date.Day.ToString (),
							 child.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id),
							 "Day name for grid item is incorrect");

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

			while (child != null) {
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

			while (child != null) {
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

		protected override Control GetControlInstance ()
		{
			return new MonthCalendar ();
		}

		private MonthCalendar calendar;
		private IRawElementProviderSimple calendarProvider;
		private Calendar currentCalendar;

		private CultureInfo oldCulture;
		private static int daysInWeek;

		private DateTime anyGivenSunday = new DateTime (2008, 12, 7);
	}
}
