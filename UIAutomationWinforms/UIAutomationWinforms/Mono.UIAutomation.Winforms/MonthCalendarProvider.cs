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
//	Brad Taylor <brad@getcoded.net>
//

using System;
using System.Threading;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Automation;
using System.Collections.Generic;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors.MonthCalendar;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace Mono.UIAutomation.Winforms
{
	internal class MonthCalendarProvider : FragmentRootControlProvider
	{
		public MonthCalendarProvider (MonthCalendar control)
			: base (control)
		{
		}

		static MonthCalendarProvider ()
		{
			// XXX: Mono's MonthCalendar control seems to only
			// support the Gregorian calendar.

			// Calendar cal = Thread.CurrentThread.CurrentCulture.Calendar;
			Calendar cal = new CultureInfo ("en-US").Calendar;
			numDaysInWeek = (cal.AddWeeks (fixedDate, 1) - fixedDate).Days;
		}

		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			childDataGrid = new MonthCalendarDataGridProvider (this);
			childDataGrid.Initialize ();
			AddChildProvider (true, childDataGrid);
			
			// Don't ask me why, but Calendar needs to implement
			// Grid as well as the DataGrid child...
			SetBehavior (GridPatternIdentifiers.Pattern,
			             new GridProviderBehavior (childDataGrid));
		}
		
		public override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Calendar.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return "calendar";

			return base.GetProviderPropertyValue (propertyId);
		}

		static internal int DaysInWeek {
			get { return numDaysInWeek; }
		}

		private MonthCalendarDataGridProvider childDataGrid;

		private static int numDaysInWeek = 0;
		private static DateTime fixedDate = new DateTime (2001, 01, 01);
	}

	internal class MonthCalendarDataGridProvider : FragmentRootControlProvider
	{
		public MonthCalendarDataGridProvider (MonthCalendarProvider calendarProvider)
			: base (calendarProvider.Control)
		{
			this.calendarProvider = calendarProvider;
			this.calendar = (MonthCalendar) calendarProvider.Control;

			// TODO TODO TODO
			this.calendarProvider.ToString ();
		}

		public override void Initialize ()
		{
			base.Initialize ();
			
			SetBehavior (GridPatternIdentifiers.Pattern,
			             new GridProviderBehavior (this));
		}

		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			headerProvider = new MonthCalendarHeaderProvider (calendarProvider);
			headerProvider.Initialize ();
			AddChildProvider (true, headerProvider);

			MonthCalendarListItemProvider item;
			SelectionRange range = calendar.GetDisplayRange (false);
			for (DateTime d = range.Start;
			     d <= range.End; d = d.AddDays (1)) {
				item = new MonthCalendarListItemProvider (
					this, Control, d);
				item.Initialize ();
				AddChildProvider (true, item);
				gridChildren.Add (d, item);
			}
		}
		
		public override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();
		}

		public int RowCount {
			get {
				return (int) System.Math.Ceiling (
					(double) gridChildren.Count / (double) MonthCalendarProvider.DaysInWeek);
			}
		}

		public int ColumnCount {
			get { return MonthCalendarProvider.DaysInWeek; }
		}
		
		public IRawElementProviderSimple GetItem (int row, int col)
		{
			if (row < 0 || row >= RowCount) {
				throw new ArgumentException ("row");
			}

			if (col < 0 || col >= ColumnCount) {
				throw new ArgumentException ("col");
			}

			SelectionRange range = calendar.GetDisplayRange (false);
			DateTime date = range.Start.AddDays (
				col + (row * MonthCalendarProvider.DaysInWeek)
			);

			if (!gridChildren.ContainsKey (date)) {
				throw new ArgumentException ();
			}

			return gridChildren[date];
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.DataGrid.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return "data grid";

			return base.GetProviderPropertyValue (propertyId);
		}

		private MonthCalendar calendar;
		private MonthCalendarProvider calendarProvider;
		private MonthCalendarHeaderProvider headerProvider;
		private Dictionary<DateTime, MonthCalendarListItemProvider> gridChildren
			= new Dictionary<DateTime, MonthCalendarListItemProvider> ();
	}

	internal class MonthCalendarListItemProvider : FragmentControlProvider
	{
		public MonthCalendarListItemProvider (FragmentRootControlProvider rootProvider,
		                                      Control control, DateTime date)
			: base (control)
		{
			this.date = date;
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.ListItem.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return "list item";
			else if (propertyId == AEIds.NameProperty.Id)
				return GetDateString ();

			return base.GetProviderPropertyValue (propertyId);
		}

		private string GetDateString ()
		{
			return date.Day.ToString ();
		}

		private DateTime date;
	}

	internal class MonthCalendarHeaderProvider : FragmentRootControlProvider
	{
		public MonthCalendarHeaderProvider (MonthCalendarProvider calendarProvider)
			: base (calendarProvider.Control)
		{
			this.calendar = (MonthCalendar) calendarProvider.Control;
		}

		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			MonthCalendarHeaderItemProvider itemProvider;

			DateTime[] days = GetDaysInWeek ();
			foreach (DateTime day in days) {
				itemProvider = new MonthCalendarHeaderItemProvider (
					this, Control,
					day.ToString (HEADER_ITEM_DAY_FORMAT));

				itemProvider.Initialize ();
				AddChildProvider (true, itemProvider);
			}
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Header.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return "header";
			else if (propertyId == AEIds.OrientationProperty.Id)
				return OrientationType.Horizontal;
			else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
				return false;
			else if (propertyId == AEIds.IsContentElementProperty.Id)
				return false;

			return base.GetProviderPropertyValue (propertyId);
		}

		// Logic copied and refactored from
		// ThemeWin32Classic::DrawSingleMonth.  No good way to share
		// this without refactoring MonthCalendar considerably.
		private DateTime[] GetDaysInWeek ()
		{
			int days_in_week = MonthCalendarProvider.DaysInWeek;
			DateTime[] days = new DateTime[days_in_week];

			DateTime sunday = new DateTime (2006, 10, 1);
			DayOfWeek first_day_of_week = calendar.GetDayOfWeek (calendar.FirstDayOfWeek);

			for (int i = 0; i < days.Length; i++) {
				int position = i - (int) first_day_of_week;
				if (position < 0) {
					position = days_in_week + position;
				}

				days[i] = sunday.AddDays (i + (int) first_day_of_week);
			}

			return days;
		}

		private MonthCalendar calendar;
		private const string HEADER_ITEM_DAY_FORMAT = "ddd";
	}

	internal class MonthCalendarHeaderItemProvider : FragmentControlProvider
	{
		public MonthCalendarHeaderItemProvider (FragmentRootControlProvider rootProvider,
		                                        Control control, string label)
			: base (control)
		{
			this.label = label;
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.HeaderItem.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return "header item";
			else if (propertyId == AEIds.NameProperty.Id)
				return label;

			return base.GetProviderPropertyValue (propertyId);
		}

		private string label;
	}
}
