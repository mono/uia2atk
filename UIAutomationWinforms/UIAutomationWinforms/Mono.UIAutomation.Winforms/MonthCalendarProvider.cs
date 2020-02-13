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
using Mono.Unix;
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
	[MapsComponent (typeof (MonthCalendar))]
	internal class MonthCalendarProvider : FragmentRootControlProvider
	{
		public MonthCalendarProvider (MonthCalendar monthCalendar)
			: base (monthCalendar)
		{
			this.monthCalendar = monthCalendar; 
		}

		static MonthCalendarProvider ()
		{
			// XXX: Mono's MonthCalendar control seems to only
			// support the Gregorian calendar.

			// Calendar cal = Thread.CurrentThread.CurrentCulture.Calendar;
			Calendar cal = new CultureInfo ("en-US").Calendar;
			numDaysInWeek = (cal.AddWeeks (fixedDate, 1) - fixedDate).Days;
		}

		public override void Initialize ()
		{
			base.Initialize ();

			SetEvent (Events.ProviderEventType.AutomationElementNameProperty,
				  new Events.MonthCalendar.AutomationNamePropertyEvent (
					this));
		}

		protected override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			childDataGrid = new MonthCalendarDataGridProvider (this);
			childDataGrid.Initialize ();
			AddChildProvider (childDataGrid);
			
			// Don't ask me why, but Calendar needs to implement
			// Grid as well as the DataGrid child...
			SetBehavior (GridPatternIdentifiers.Pattern,
			             new GridProviderBehavior (childDataGrid));
			SetBehavior (TablePatternIdentifiers.Pattern,
			             new TableProviderBehavior (childDataGrid));
			SetBehavior (SelectionPatternIdentifiers.Pattern,
			             new SelectionProviderBehavior (childDataGrid));
		}
		
		protected override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Calendar.Id;
			else if (propertyId == AEIds.NameProperty.Id)
				return monthCalendar.SelectionStart.ToShortDateString ();
			return base.GetProviderPropertyValue (propertyId);
		}

		static internal int DaysInWeek {
			get { return numDaysInWeek; }
		}

		private MonthCalendarDataGridProvider childDataGrid;
		private MonthCalendar monthCalendar;

		private static int numDaysInWeek = 0;
		private static DateTime fixedDate = new DateTime (2001, 01, 01);
	}
}
