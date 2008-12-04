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
			SetBehavior (TablePatternIdentifiers.Pattern,
			             new TableProviderBehavior (childDataGrid));
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
			this.calendar.DateChanged += OnDateChanged;
		}

		public override void Initialize ()
		{
			base.Initialize ();
			
			SetBehavior (GridPatternIdentifiers.Pattern,
			             new GridProviderBehavior (this));
			SetBehavior (TablePatternIdentifiers.Pattern,
			             new TableProviderBehavior (this));
		}

		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			headerProvider
				= new MonthCalendarHeaderProvider (
					this, calendarProvider, Control);
			headerProvider.Initialize ();
			AddChildProvider (true, headerProvider);

			AddChildren ();
		}
		
		public override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();

			RemoveChildProvider (true, headerProvider);
			headerProvider.Terminate ();
			
			RemoveChildren ();
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

		public MonthCalendarHeaderProvider HeaderProvider {
			get { return headerProvider; }
		}
		
		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { return calendarProvider; }
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

		private void AddChildren ()
		{
			MonthCalendarListItemProvider item;
			SelectionRange range = calendar.GetDisplayRange (false);

			for (DateTime d = range.Start;
			     d <= range.End; d = d.AddDays (1)) {
				int days = (d - range.Start).Days;
				int r = (int)System.Math.Floor ((double)days
					/ (double)MonthCalendarProvider.DaysInWeek);
				int c = days - (r * MonthCalendarProvider.DaysInWeek);

				item = new MonthCalendarListItemProvider (
					this, calendarProvider, Control, d, r, c);
				item.Initialize ();

				AddChildProvider (true, item);
				gridChildren.Add (d, item);
			}
		}

		private void RemoveChildren ()
		{
			foreach (MonthCalendarListItemProvider item
			         in gridChildren.Values) {
				RemoveChildProvider (true, item);
				item.Terminate ();
			}

			gridChildren.Clear ();
		}

		private void OnDateChanged (object o, DateRangeEventArgs args)
		{
			RemoveChildren ();
			AddChildren ();
		}

		private MonthCalendar calendar;
		private MonthCalendarProvider calendarProvider;
		private MonthCalendarHeaderProvider headerProvider;
		private Dictionary<DateTime, MonthCalendarListItemProvider> gridChildren
			= new Dictionary<DateTime, MonthCalendarListItemProvider> ();
	}

	internal class MonthCalendarListItemProvider : FragmentControlProvider
	{
		public MonthCalendarListItemProvider (MonthCalendarDataGridProvider dataGridProvider,
		                                      MonthCalendarProvider calendarProvider,
		                                      Control control, DateTime date,
		                                      int row, int col)
			: base (control)
		{
			this.dataGridProvider = dataGridProvider;
			this.calendarProvider = calendarProvider;
			this.date = date;
			this.row = row;
			this.col = col;
		}

		public MonthCalendarDataGridProvider DataGridProvider {
			get { return dataGridProvider; }
		}

		public MonthCalendarProvider MonthCalendarProvider {
			get { return calendarProvider; }
		}

		public int Column {
			get { return col; }
		}

		public int Row {
			get { return row; }
		}

		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { return dataGridProvider; }
		}

		public override void Initialize ()
		{
			base.Initialize ();

			SetBehavior (GridItemPatternIdentifiers.Pattern,
			             new ListItemGridItemProviderBehavior (this));
			SetBehavior (TableItemPatternIdentifiers.Pattern,
			             new ListItemTableItemProviderBehavior (this));
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

		private int row, col;
		private DateTime date;
		private MonthCalendarProvider calendarProvider;
		private MonthCalendarDataGridProvider dataGridProvider;
	}

	internal class MonthCalendarHeaderProvider : FragmentRootControlProvider
	{
		public MonthCalendarHeaderProvider (FragmentRootControlProvider rootProvider,
		                                    MonthCalendarProvider calendarProvider,
		                                    Control control)
			: base (control)
		{
			this.rootProvider = rootProvider;
			this.calendar = (MonthCalendar) control;
		}

		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			MonthCalendarHeaderItemProvider itemProvider;

			DateTime[] days = GetDaysInWeek ();
			for (int i = 0; i < days.Length; i++) {
				itemProvider = new MonthCalendarHeaderItemProvider (
					this, Control,
					days[i].ToString (HEADER_ITEM_DAY_FORMAT), i);

				itemProvider.Initialize ();
				AddChildProvider (true, itemProvider);
				headerItems.Add (itemProvider);
			}
		}

		public override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();
			
			foreach (MonthCalendarHeaderItemProvider item in headerItems) {
				RemoveChildProvider (true, item);
				item.Terminate ();
			}

			headerItems.Clear ();
		}

		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { return rootProvider; }
		}

		public IRawElementProviderSimple[] GetHeaderItems ()
		{
			return headerItems.ToArray ();
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
		private FragmentRootControlProvider rootProvider;
		private List<MonthCalendarHeaderItemProvider> headerItems
			= new List<MonthCalendarHeaderItemProvider> ();
		private const string HEADER_ITEM_DAY_FORMAT = "ddd";
	}

	internal class MonthCalendarHeaderItemProvider : FragmentControlProvider
	{
		public MonthCalendarHeaderItemProvider (FragmentRootControlProvider rootProvider,
		                                        Control control, string label, int index)
			: base (control)
		{
			this.rootProvider = rootProvider;
			this.label = label;
			this.index = index;
		}

		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { return rootProvider; }
		}
	
		public int Index {
			get { return index; }
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

		private int index;
		private string label;
		private FragmentRootControlProvider rootProvider;
	}
}
