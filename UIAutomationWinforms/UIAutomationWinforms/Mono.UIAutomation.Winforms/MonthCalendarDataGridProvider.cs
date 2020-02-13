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
	internal class MonthCalendarDataGridProvider : FragmentRootControlProvider
	{
#region Constructors
		public MonthCalendarDataGridProvider (MonthCalendarProvider calendarProvider)
			: base (calendarProvider.Control)
		{
			this.calendarProvider = calendarProvider;
			this.calendar = (MonthCalendar) calendarProvider.Control;
			this.calendar.DateChanged += OnDateChanged;
			this.displayRange = calendar.GetDisplayRange (false);
		}
#endregion

#region Public Properties
		public MonthCalendarHeaderProvider HeaderProvider {
			get { return headerProvider; }
		}
#endregion

#region SimpleControlProvider Overrides
		public override void Initialize ()
		{
			base.Initialize ();
			
			SetBehavior (GridPatternIdentifiers.Pattern,
			             new GridProviderBehavior (this));
			SetBehavior (TablePatternIdentifiers.Pattern,
			             new TableProviderBehavior (this));
			SetBehavior (SelectionPatternIdentifiers.Pattern,
			             new SelectionProviderBehavior (this));
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.DataGrid.Id;
			else if (propertyId == AEIds.NativeWindowHandleProperty.Id)
				return null;

			return base.GetProviderPropertyValue (propertyId);
		}
#endregion

#region FragmentControlProvider Overrides
		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { return calendarProvider; }
		}
	
		protected override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			headerProvider
				= new MonthCalendarHeaderProvider (
					this, calendarProvider, Control);
			headerProvider.Initialize ();
			AddChildProvider (headerProvider);

			AddChildren ();
		}
		
		protected override void FinalizeChildControlStructure()
		{
			base.FinalizeChildControlStructure ();

			RemoveChildProvider (headerProvider);
			headerProvider.Terminate ();
			
			RemoveChildren ();
		}
#endregion

#region Grid Helper Methods
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
#endregion

#region Selection Helper Methods
		public bool CanSelectMultiple {
			get { return calendar.MaxSelectionCount > 1; }
		}

		public int SelectedItemCount {
			get {
				SelectionRange range = calendar.SelectionRange;
				return (range.End - range.Start).Days;
			}
		}

		public IRawElementProviderSimple[] GetSelection ()
		{
			SelectionRange range = calendar.SelectionRange;
			List<IRawElementProviderSimple> selectedItems
				= new List<IRawElementProviderSimple> ();

			DateTime date = range.Start;
			while (date <= range.End) {
				if (!gridChildren.ContainsKey (date)) {
					date = date.AddDays (1);
					continue;
				}

				selectedItems.Add (gridChildren[date]);
				date = date.AddDays (1);
			}

			return selectedItems.ToArray ();
		}

		// The behavior of this will be a little wonky as selection
		// must be continuous.
		public void AddToSelection (MonthCalendarListItemProvider itemProvider)
		{
			if (!CanSelectMultiple) {
				throw new InvalidOperationException ();
			}
			
			if (IsItemSelected (itemProvider)) {
				return;
			}

			SelectionRange range = calendar.SelectionRange;

			int proposedDays = (range.End - itemProvider.Date).Days;
			if (range.Start > itemProvider.Date) {
				if (proposedDays > calendar.MaxSelectionCount) {
					throw new InvalidOperationException ();
				}

				range.Start = itemProvider.Date;
			}

			proposedDays = (itemProvider.Date - range.Start).Days;
			if (range.End < itemProvider.Date) {
				if (proposedDays > calendar.MaxSelectionCount) {
					throw new InvalidOperationException ();
				}

				range.End = itemProvider.Date;
			}
			
			calendar.SelectionRange = range;
		}

		// We can only allow removing from selection on the endpoints.
		// Winforms GUI doesn't even allow this.
		public void RemoveFromSelection (MonthCalendarListItemProvider itemProvider)
		{
			if (!CanSelectMultiple) {
				throw new InvalidOperationException ();
			}

			SelectionRange range = calendar.SelectionRange;
			if (range.Start == range.End) {
				throw new InvalidOperationException ();
			}
			
			if (range.Start == itemProvider.Date) {
				range.Start = range.Start.AddDays (1);
				return;
			}
			
			if (range.End == itemProvider.Date) {
				range.End = range.End.AddDays (-1);
				return;
			}

			throw new InvalidOperationException ();
		}

		public void SelectItem (MonthCalendarListItemProvider itemProvider)
		{
			calendar.SelectionRange
				= new SelectionRange (itemProvider.Date,
			                              itemProvider.Date);
		}

		public bool IsItemSelected (MonthCalendarListItemProvider itemProvider)
		{
			SelectionRange range = calendar.SelectionRange;
			return (range.Start <= itemProvider.Date
			        && range.End >= itemProvider.Date);
		}
#endregion

#region Private Methods
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

				AddChildProvider (item);
				gridChildren.Add (d, item);
			}

			backButton = new MonthCalendarButtonProvider (
				this, MonthCalendarButtonProvider.ButtonDirection.Back);
			backButton.Initialize ();
			AddChildProvider (backButton);

			forwardButton = new MonthCalendarButtonProvider (
				this, MonthCalendarButtonProvider.ButtonDirection.Forward);
			forwardButton.Initialize ();
			AddChildProvider (forwardButton);
		}

		private void RemoveChildren ()
		{
			RemoveChildProvider (backButton);
			backButton.Terminate ();

			RemoveChildProvider (forwardButton);
			forwardButton.Terminate ();

			foreach (MonthCalendarListItemProvider item
			         in gridChildren.Values) {
				RemoveChildProvider (item);
				item.Terminate ();
			}

			gridChildren.Clear ();
		}

		private void OnDateChanged (object o, DateRangeEventArgs args)
		{
			SelectionRange range = calendar.GetDisplayRange (false);
			if (displayRange != null
			    && range.Start == displayRange.Start
			    && range.End == displayRange.End)
				return;

			RemoveChildren ();
			AddChildren ();
			displayRange = range;
		}
#endregion

#region Private Fields
		private MonthCalendar calendar;
		private SelectionRange displayRange;

		private MonthCalendarProvider calendarProvider;
		private MonthCalendarHeaderProvider headerProvider;

		private MonthCalendarButtonProvider backButton, forwardButton;
		private Dictionary<DateTime, MonthCalendarListItemProvider> gridChildren
			= new Dictionary<DateTime, MonthCalendarListItemProvider> ();
#endregion
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

		protected override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			MonthCalendarHeaderItemProvider itemProvider;

			DateTime[] days = GetDaysInWeek ();
			for (int i = 0; i < days.Length; i++) {
				itemProvider = new MonthCalendarHeaderItemProvider (
					this, Control,
					days[i].ToString (HEADER_ITEM_DAY_FORMAT), i);

				itemProvider.Initialize ();
				AddChildProvider (itemProvider);
				headerItems.Add (itemProvider);
			}
		}

		protected override void FinalizeChildControlStructure()
		{
			base.FinalizeChildControlStructure ();
			
			foreach (MonthCalendarHeaderItemProvider item in headerItems) {
				RemoveChildProvider (item);
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
			else if (propertyId == AEIds.OrientationProperty.Id)
				return OrientationType.Horizontal;
			else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
				return false;
			else if (propertyId == AEIds.IsContentElementProperty.Id)
				return false;
			else if (propertyId == AEIds.LabeledByProperty.Id)
				return null;
			else if (propertyId == AEIds.NativeWindowHandleProperty.Id)
				return null;

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

		public override void Initialize ()
		{
			base.Initialize ();

			SetBehavior (GridItemPatternIdentifiers.Pattern,
			             new HeaderItemGridItemProviderBehavior (this));
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.HeaderItem.Id;
			else if (propertyId == AEIds.NameProperty.Id)
				return label;
			else if (propertyId == AEIds.LabeledByProperty.Id)
				return null;
			else if (propertyId == AEIds.NativeWindowHandleProperty.Id)
				return null;

			return base.GetProviderPropertyValue (propertyId);
		}

		private int index;
		private string label;
		private FragmentRootControlProvider rootProvider;
	}

	internal class MonthCalendarButtonProvider : FragmentControlProvider
	{
		public enum ButtonDirection {
			Back,
			Forward
		}

		public MonthCalendarButtonProvider (MonthCalendarDataGridProvider dataGridProvider,
		                                    ButtonDirection direction)
			: base (dataGridProvider.Control)
		{
			this.dataGridProvider = dataGridProvider;
			this.direction = direction;
		}

		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { return dataGridProvider; }
		}

		public MonthCalendarDataGridProvider DataGridProvider {
			get { return dataGridProvider; }
		}

		public ButtonDirection Direction {
			get { return direction; }
		}
	
		public override void Initialize ()
		{
			base.Initialize ();
			
			SetBehavior (InvokePatternIdentifiers.Pattern,
			             new ButtonInvokeProviderBehavior (this));
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id) {
				return ControlType.Button.Id;
			} else if (propertyId == AEIds.NameProperty.Id) {
				if (direction == ButtonDirection.Back) {
					return Catalog.GetString ("Back by one month");
				} else if (direction == ButtonDirection.Forward) {
					return Catalog.GetString ("Forward by one month");
				}
			} else if (propertyId == AEIds.LabeledByProperty.Id)
				return null;
			else if (propertyId == AEIds.NativeWindowHandleProperty.Id)
				return null;

			return base.GetProviderPropertyValue (propertyId);
		}

		private ButtonDirection direction;
		private MonthCalendarDataGridProvider dataGridProvider;
	}
}
