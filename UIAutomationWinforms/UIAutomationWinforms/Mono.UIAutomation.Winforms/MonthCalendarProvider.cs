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
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors.TabPage;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace Mono.UIAutomation.Winforms
{
	internal class MonthCalendarProvider : FragmentRootControlProvider
	{
		public MonthCalendarProvider (MonthCalendar control)
			: base (control)
		{
		}

		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			childDataGrid = new MonthCalendarDataGridProvider (this);
			childDataGrid.Initialize ();
			AddChildProvider (true, childDataGrid);
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

		private MonthCalendarDataGridProvider childDataGrid;
	}

	internal class MonthCalendarDataGridProvider : FragmentRootControlProvider
	{
		public MonthCalendarDataGridProvider (MonthCalendarProvider calendarProvider)
			: base (calendarProvider.Control)
		{
			this.calendarProvider = calendarProvider;

			// TODO TODO TODO
			this.calendarProvider.ToString ();
		}

		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			headerProvider = new MonthCalendarHeaderProvider (calendarProvider);
			headerProvider.Initialize ();
			AddChildProvider (true, headerProvider);
		}
		
		public override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.DataGrid.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return "data grid";

			return base.GetProviderPropertyValue (propertyId);
		}

		private MonthCalendarProvider calendarProvider;
		private MonthCalendarHeaderProvider headerProvider;
	}

	internal class MonthCalendarHeaderProvider : FragmentRootControlProvider
	{
		public MonthCalendarHeaderProvider (MonthCalendarProvider calendarProvider)
			: base (calendarProvider.Control)
		{
			this.calendarProvider = calendarProvider;
			this.calendar = (MonthCalendar) calendarProvider.Control;

			// TODO TODO TODO
			this.calendarProvider.ToString ();
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

			return base.GetProviderPropertyValue (propertyId);
		}

		// Logic copied and refactored from
		// ThemeWin32Classic::DrawSingleMonth.  No good way to share
		// this without refactoring MonthCalendar considerably.
		private DateTime[] GetDaysInWeek ()
		{
			DateTime[] days = new DateTime[DAYS_IN_WEEK];

			DateTime sunday = new DateTime (2006, 10, 1);
			DayOfWeek first_day_of_week = calendar.GetDayOfWeek (calendar.FirstDayOfWeek);

			for (int i = 0; i < days.Length; i++) {
				int position = i - (int) first_day_of_week;
				if (position < 0) {
					position = DAYS_IN_WEEK + position;
				}

				days[i] = sunday.AddDays (i + (int) first_day_of_week);
			}

			return days;
		}

		private MonthCalendar calendar;
		private MonthCalendarProvider calendarProvider;

		// Change this if we should ever need to support a
		// non-Gregorian calendar
		private const int DAYS_IN_WEEK = 7;

		private const string HEADER_ITEM_DAY_FORMAT = "ddd";
	}

	internal class MonthCalendarHeaderItemProvider : FragmentControlProvider
	{
		public MonthCalendarHeaderItemProvider (FragmentRootControlProvider rootProvider,
		                                        Control control, string label)
			: base (control)
		{
			this.rootProvider = rootProvider;
			this.label = label;
			
			// TODO TODO TODO
			this.rootProvider.ToString ();
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.HeaderItem.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return "header item";
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return label;

			return base.GetProviderPropertyValue (propertyId);
		}

		private string label;
		private FragmentRootControlProvider rootProvider;
	}
}
