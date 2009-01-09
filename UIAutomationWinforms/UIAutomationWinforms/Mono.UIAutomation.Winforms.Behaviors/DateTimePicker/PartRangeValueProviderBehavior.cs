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
using System.Windows.Automation;
using SWF = System.Windows.Forms;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.DateTimePicker;

namespace Mono.UIAutomation.Winforms.Behaviors.DateTimePicker
{
	internal class PartRangeValueProviderBehavior
		: ProviderBehavior, IRangeValueProvider
	{
#region Constructor
		public PartRangeValueProviderBehavior (
			DateTimePickerProvider.DateTimePickerPartProvider partProvider)
			: base (partProvider)
		{
			this.partProvider = partProvider;
			this.dateTimePicker = (SWF.DateTimePicker) partProvider.Control;
		}
#endregion
		
#region IProviderBehavior Interface
		public override AutomationPattern ProviderPattern { 
			get { return RangeValuePatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			// NOTE: SmallChange Property NEVER changes.
			// NOTE: LargeChange Property NEVER changes.
			DateTimePickerProvider pickerProvider
				= partProvider.PickerProvider;

			Provider.SetEvent (ProviderEventType.RangeValuePatternIsReadOnlyProperty,
			                   new PartRangeValuePatternIsReadOnlyEvent (partProvider, pickerProvider));
			Provider.SetEvent (ProviderEventType.RangeValuePatternValueProperty,
			                   new PartRangeValuePatternValueEvent (partProvider, pickerProvider));
			Provider.SetEvent (ProviderEventType.RangeValuePatternMinimumProperty,
			                   new PartRangeValuePatternMinimumEvent (partProvider, pickerProvider));
			Provider.SetEvent (ProviderEventType.RangeValuePatternMaximumProperty,
			                   new PartRangeValuePatternMaximumEvent (partProvider, pickerProvider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.RangeValuePatternIsReadOnlyProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.RangeValuePatternMinimumProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.RangeValuePatternMaximumProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.RangeValuePatternLargeChangeProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.RangeValuePatternSmallChangeProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.RangeValuePatternValueProperty,
			                   null);
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == RangeValuePatternIdentifiers.IsReadOnlyProperty.Id)
				return IsReadOnly;
			else if (propertyId == RangeValuePatternIdentifiers.MinimumProperty.Id)
				return Minimum;
			else if (propertyId == RangeValuePatternIdentifiers.MaximumProperty.Id)
				return Maximum;
			else if (propertyId == RangeValuePatternIdentifiers.LargeChangeProperty.Id)
				return LargeChange;
			else if (propertyId == RangeValuePatternIdentifiers.SmallChangeProperty.Id)
				return SmallChange;
			else if (propertyId == RangeValuePatternIdentifiers.ValueProperty.Id)
				return Value;
			else
				return base.GetPropertyValue (propertyId);
		}
#endregion
	
#region IRangeValueProvider Members
		public bool IsReadOnly {
			get {
				return (dateTimePicker.ShowCheckBox
				        && !dateTimePicker.Checked);
			}
		}
		
		public double Minimum {
			get {
				DateTime min = dateTimePicker.MinDate;
				DateTime cur = dateTimePicker.Value;
				switch (partProvider.PartData.date_time_part) {
				case SWF.DateTimePicker.DateTimePart.Seconds:
					return IsDateEqual (min, cur) ? min.Second : 0;
				case SWF.DateTimePicker.DateTimePart.Minutes:
					return IsDateEqual (min, cur) ? min.Minute : 0;
				case SWF.DateTimePicker.DateTimePart.AMPMHour:
					return IsDateEqual (min, cur) ? min.Hour : 0;
				case SWF.DateTimePicker.DateTimePart.Hour:
					int hour = 1;
					if (IsDateEqual (min, cur)) {
						hour = min.Hour;
						if (hour > 12)
							hour -= 12;
					}
					return hour;
				case SWF.DateTimePicker.DateTimePart.Day:
					if (cur.Year == min.Year
					    && cur.Month == min.Month) {
						return min.Day;
					}
					return 1;
				case SWF.DateTimePicker.DateTimePart.Month:
					if (cur.Year == min.Year) {
						return min.Month;
					}
					return 1;
				case SWF.DateTimePicker.DateTimePart.Year:
					return min.Year;
				}
				return 0;
			}
		}
		
		public double Maximum {
			get {
				DateTime max = dateTimePicker.MaxDate;
				DateTime cur = dateTimePicker.Value;
				switch (partProvider.PartData.date_time_part) {
				case SWF.DateTimePicker.DateTimePart.Seconds:
					return IsDateEqual (max, cur) ? max.Second : 60;
				case SWF.DateTimePicker.DateTimePart.Minutes:
					return IsDateEqual (max, cur) ? max.Minute : 60;
				case SWF.DateTimePicker.DateTimePart.AMPMHour:
					return IsDateEqual (max, cur) ? max.Hour : 23;
				case SWF.DateTimePicker.DateTimePart.Hour:
					int hour = 12;
					if (IsDateEqual (max, cur)) {
						hour = max.Hour;
						if (hour > 12)
							hour -= 12;
					}
					return hour;
				case SWF.DateTimePicker.DateTimePart.Day:
					if (cur.Year == max.Year
					    && cur.Month == max.Month) {
						return max.Day;
					}
					return DateTime.DaysInMonth (cur.Year, cur.Month);
				case SWF.DateTimePicker.DateTimePart.Month:
					if (cur.Year == max.Year) {
						return max.Month;
					}
					return Thread.CurrentThread.CurrentCulture
						.Calendar.GetMonthsInYear (cur.Year);
				case SWF.DateTimePicker.DateTimePart.Year:
					return max.Year;
				}
				return 0;
			}
		}
		
		public double LargeChange {
			get { return 1; }
		}
		public double SmallChange {
			get { return 1; }
		}

		public double Value {
			get { return Convert.ToDouble (partProvider.Text); }
		}
		
		public void SetValue (double value)
		{
			if (value < Minimum || value > Maximum)
				throw new ArgumentOutOfRangeException ();

			PerformSetValue (value);
		}
#endregion
		
#region Private Methods
		private void PerformSetValue (double value) 
		{
			if (dateTimePicker.InvokeRequired) {
				dateTimePicker.BeginInvoke (
					new DateTimePickerPartSetValueDelegate (PerformSetValue),
					new object [] { value }
				);
				return;
			}

			dateTimePicker.SetPart ((int) value, partProvider.PartData.date_time_part);
		}

		private bool IsDateEqual (DateTime date1, DateTime date2)
		{
			return date1.Date.Equals (date2.Date);
		}
#endregion
		
#region Private Fields
		private SWF.DateTimePicker dateTimePicker;
		private DateTimePickerProvider.DateTimePickerPartProvider partProvider;
#endregion
	}

	public delegate void DateTimePickerPartSetValueDelegate (double value);
}
