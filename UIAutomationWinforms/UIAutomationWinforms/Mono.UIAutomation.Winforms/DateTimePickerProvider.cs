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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Events.DateTimePicker;
using Mono.UIAutomation.Winforms.Behaviors.DateTimePicker;

using Mono.Unix;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (DateTimePicker))]
	internal class DateTimePickerProvider : FragmentRootControlProvider
	{
#region Public Methods
		public DateTimePickerProvider (DateTimePicker control)
			: base (control)
		{
			this.control = control;
		}

		public override void Initialize ()
		{
			base.Initialize ();

			SetEvent (ProviderEventType.AutomationElementNameProperty,
				  new Events.DateTimePicker.AutomationNamePropertyEvent (
					this, this));

			control.UIAShowCheckBoxChanged
				+= new EventHandler (OnUIAShowCheckBoxChanged);
			control.UIAShowUpDownChanged
				+= new EventHandler (OnUIAShowUpDownChanged);
		}
#endregion

#region FragmentControlProvider Implementation
		protected override void InitializeChildControlStructure ()
		{
			AddSegmentItems ();
			UpdateChildren ();
		}

		protected override void FinalizeChildControlStructure()
		{
			RemoveSegmentItems ();
			DestroyLocalChild (dropDownButton);
			dropDownButton = null;
		}

#endregion
		
#region Protected Methods
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Pane.Id;

			return base.GetProviderPropertyValue (propertyId);
		}
#endregion

#region Private Methods
		private void UpdateChildren ()
		{
			if (control.ShowCheckBox) {
				if (checkBox == null) {
					checkBox = new DateTimePickerCheckBoxProvider (this);
					InsertChildProviderBefore (checkBox, Navigation.GetAllChildren ().FirstOrDefault ());
				}
			} else {
				DestroyLocalChild (checkBox);
				checkBox = null;
			}

			if (control.ShowUpDown) {
				DestroyLocalChild (dropDownButton);
				dropDownButton = null;
			} else {
				if (dropDownButton == null) {
					dropDownButton
						= new DateTimePickerButtonProvider (this);
					AddChildProvider (dropDownButton);
				}
			}
		}

		private void DestroyLocalChild (FragmentControlProvider child)
		{
			if (child == null)
				return;
			RemoveChildProvider (child);
			child.Terminate ();
		}

		private void AddSegmentItems ()
		{
			for (int i = 0; i < control.part_data.Length; i++) {
				DateTimePicker.PartData part_data = control.part_data [i];
				FragmentControlProvider prov = null;

				switch (part_data.date_time_part) {
				case DateTimePicker.DateTimePart.Month:
				case DateTimePicker.DateTimePart.DayName:
				case DateTimePicker.DateTimePart.AMPMSpecifier:
					prov = new DateTimePickerListPartProvider (
						this, part_data, i
					);
					break;
				case DateTimePicker.DateTimePart.Day:
				case DateTimePicker.DateTimePart.Hour:
				case DateTimePicker.DateTimePart.Year:
				case DateTimePicker.DateTimePart.Seconds:
				case DateTimePicker.DateTimePart.Minutes:
				case DateTimePicker.DateTimePart.AMPMHour:
					prov = new DateTimePickerSpinnerPartProvider (
						this, part_data, i
					);
					break;
				default: // DateTimePicker.DateTimePart.Literal
					prov = new DateTimePickerPartProvider (
						this, part_data, i
					);
					break;
				}

				prov.Initialize ();
				AddChildProvider (prov);
				children.Add (prov);
			}
		}

		private void RemoveSegmentItems ()
		{
			foreach (FragmentControlProvider prov in children) {
				RemoveChildProvider (prov);
				prov.Terminate ();
			}

			children.Clear ();
		}

		private void OnUIAShowCheckBoxChanged (object o, EventArgs args)
		{
			UpdateChildren ();
		}
		
		private void OnUIAShowUpDownChanged (object o, EventArgs args)
		{
			UpdateChildren ();
		}
#endregion

#region Private Fields
		private DateTimePicker control;
		private DateTimePickerButtonProvider dropDownButton;
		private DateTimePickerCheckBoxProvider checkBox;
		private List<FragmentControlProvider> children = new List<FragmentControlProvider> ();
#endregion

#region Internal Classes
		internal class DateTimePickerCheckBoxProvider
			: FragmentControlProvider
		{
			public DateTimePickerCheckBoxProvider (DateTimePickerProvider rootProvider)
				: base (rootProvider.Control)
			{
				this.rootProvider = rootProvider;

				SetBehavior (TogglePatternIdentifiers.Pattern,
				             new ToggleProviderBehavior (this));

				SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
					  new CheckboxAutomationHasKeyboardFocusPropertyEvent (this));
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return rootProvider; }
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AEIds.NameProperty.Id)
					return String.Empty;
				else if (propertyId == AEIds.IsContentElementProperty.Id)
					return false;
				else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
					return true;
				else if (propertyId == AEIds.HasKeyboardFocusProperty.Id)
					return (bool) rootProvider.GetPropertyValue (AEIds.HasKeyboardFocusProperty.Id)
					       && ((DateTimePicker) rootProvider.Control).UIAIsCheckBoxSelected;
				else if (propertyId == AEIds.ControlTypeProperty.Id)
					return ControlType.CheckBox.Id;
				return base.GetProviderPropertyValue (propertyId);
			}

			private DateTimePickerProvider rootProvider;
		}

		internal class DateTimePickerButtonProvider : FragmentControlProvider
		{
			public DateTimePickerButtonProvider (DateTimePickerProvider rootProvider)
				: base (rootProvider.Control)
			{
				this.rootProvider = rootProvider;

				SetBehavior (InvokePatternIdentifiers.Pattern,
				             new ButtonInvokeProviderBehavior (
				                     this, rootProvider));
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return rootProvider; }
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AEIds.NameProperty.Id)
					return Catalog.GetString ("Drop Down");
				else if (propertyId == AEIds.IsContentElementProperty.Id)
					return false;
				else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AEIds.ControlTypeProperty.Id)
					return ControlType.Button.Id;
				return base.GetProviderPropertyValue (propertyId);
			}

			private DateTimePickerProvider rootProvider;
		}

		internal class DateTimePickerPartProvider : FragmentRootControlProvider
		{
			public DateTimePickerPartProvider (DateTimePickerProvider rootProvider,
			                                   DateTimePicker.PartData part_data,
			                                   int part_index)
				: base (rootProvider.Control)
			{
				this.rootProvider = rootProvider;
				this.part_data = part_data;
				this.part_index = part_index;
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return rootProvider; }
			}

			public DateTimePickerProvider PickerProvider {
				get { return rootProvider; }
			}

			public DateTimePicker.PartData PartData {
				get { return part_data; }
			}

			public string Text {
				get { return PartData.GetText (((DateTimePicker) rootProvider.Control).Value); }
			}

			public override void Initialize ()
			{
				base.Initialize ();

				SetEvent (ProviderEventType.AutomationElementNameProperty,
					  new Events.DateTimePicker.AutomationNamePropertyEvent (
						this, rootProvider));

				SetEvent (ProviderEventType.AutomationElementIsEnabledProperty,
					  new PartAutomationIsEnabledPropertyEvent (this));

				SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
					  new PartAutomationHasKeyboardFocusPropertyEvent (this));
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AEIds.NameProperty.Id)
					return Text;
				else if (propertyId == AEIds.IsContentElementProperty.Id)
					return true;
				else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AEIds.HasKeyboardFocusProperty.Id)
					return false;
				else if (propertyId == AEIds.ControlTypeProperty.Id)
					return ControlType.Text.Id;
				else if (propertyId == AEIds.IsEnabledProperty.Id) {
					DateTimePicker picker = (DateTimePicker) Control;
					return Control.Enabled
					       && !(picker.ShowCheckBox && !picker.Checked);
				}
				return base.GetProviderPropertyValue (propertyId);
			}

			protected int part_index;
			protected DateTimePicker.PartData part_data;
			protected DateTimePickerProvider rootProvider;
		} 

		internal class DateTimePickerSpinnerPartProvider
			: DateTimePickerPartProvider
		{
			private DateTimePicker dateTimePicker;

			public DateTimePickerSpinnerPartProvider (DateTimePickerProvider rootProvider,
			                                          DateTimePicker.PartData part_data,
			                                          int part_index)
				: base (rootProvider, part_data, part_index)
			{
				dateTimePicker = (DateTimePicker) rootProvider.Control;
			}

			public override void Initialize ()
			{
				base.Initialize ();

				SetBehavior (RangeValuePatternIdentifiers.Pattern,
					     new PartRangeValueProviderBehavior (this));
			}

			public override void SetFocus ()
			{
				if (dateTimePicker.ShowCheckBox && !dateTimePicker.Checked)
					return;

				dateTimePicker.SelectPart (part_index);
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
					return true;
				else if (propertyId == AEIds.HasKeyboardFocusProperty.Id)
					return (bool) rootProvider.GetPropertyValue (AEIds.HasKeyboardFocusProperty.Id)
					       && part_data.Selected;
				else if (propertyId == AEIds.ControlTypeProperty.Id)
					return ControlType.Spinner.Id;
				return base.GetProviderPropertyValue (propertyId);
			}
		}

		internal class DateTimePickerListPartProvider
			: DateTimePickerPartProvider, IListProvider
		{
			public DateTimePickerListPartProvider (DateTimePickerProvider rootProvider,
			                                       DateTimePicker.PartData part_data,
			                                       int part_index)
				: base (rootProvider, part_data, part_index)
			{
				this.dateTimePicker = (DateTimePicker) rootProvider.Control;
			}

			public override void Initialize ()
			{
				base.Initialize ();

				SetBehavior (SelectionPatternIdentifiers.Pattern,
					     new PartSelectionProviderBehavior (this));
			}

			public override void SetFocus ()
			{
				if (dateTimePicker.ShowCheckBox && !dateTimePicker.Checked)
					return;

				((DateTimePicker) rootProvider.Control).SelectPart (part_index);
			}

			protected override void InitializeChildControlStructure ()
			{
				base.InitializeChildControlStructure ();

				Calendar cal = Thread.CurrentThread.CurrentCulture.Calendar;
				DateTime cur = dateTimePicker.Value;

				switch (part_data.date_time_part) {
				case DateTimePicker.DateTimePart.Month:
					int numMonths = cal.GetMonthsInYear (cur.Year);
					for (int i = 1; i <= numMonths; i++) {
						AddChildDate (new DateTime (cur.Year, i, 1));
					}
					break;
				case DateTimePicker.DateTimePart.DayName:
					DateTime week = cal.AddWeeks (monday, 1);
					for (DateTime d = monday; d < week; d = d.AddDays (1)) {
						AddChildDate (d);
					}
					break;
				case DateTimePicker.DateTimePart.AMPMSpecifier:
					// In this case, ObjectItem is just a marker
					AddChildDate (amDate);
					AddChildDate (pmDate);
					break;
				}
			}

			public int IndexOfObjectItem (object o)
			{
				return children.IndexOf (childrenData [(DateTime)o]);
			}

			public void FocusItem (object o)
			{
				if (!childrenData.ContainsKey ((DateTime)o)) {
					return;
				}
				
				SelectItem (childrenData [(DateTime)o]);
			}

			public IProviderBehavior GetListItemBehaviorRealization (AutomationPattern pattern,
			                                                         ListItemProvider prov)
			{
				DateTimePickerListPartItemProvider itemProvider
					= (DateTimePickerListPartItemProvider)prov;
				if (pattern == SelectionItemPatternIdentifiers.Pattern) {
					return new PartListItemSelectionItemProviderBehavior (
						itemProvider);
				} else if (pattern == ValuePatternIdentifiers.Pattern) {
					return new PartListItemValueProviderBehavior (itemProvider);
				}
				return null;
			}

			public IConnectable GetListItemEventRealization (ProviderEventType eventType,
			                                                 ListItemProvider prov)
			{
				if (eventType == ProviderEventType.AutomationElementHasKeyboardFocusProperty) {
					return new PartListItemAutomationHasKeyboardFocusPropertyEvent (
						prov, this);
				}
				return null;
			}

			public object GetItemPropertyValue (ListItemProvider prov, int propertyId)
			{
				DateTimePickerListPartItemProvider itemProv
					= (DateTimePickerListPartItemProvider)prov;
				if (propertyId == AEIds.NameProperty.Id)
					return itemProv.Text;
				else if (propertyId == AEIds.HasKeyboardFocusProperty.Id)
					return (bool) rootProvider.GetPropertyValue (AEIds.HasKeyboardFocusProperty.Id)
					       && IsItemSelected (prov);
				else if (propertyId == AEIds.BoundingRectangleProperty.Id)
					return GetProviderPropertyValue (AEIds.BoundingRectangleProperty.Id);
				return null;
			}

			public ToggleState GetItemToggleState (ListItemProvider prov)
			{
				throw new NotSupportedException ();
			}

			public void ToggleItem (ListItemProvider item)
			{
				throw new NotSupportedException ();
			}

			public bool IsItemSelected (ListItemProvider prov)
			{
				Calendar cal = Thread.CurrentThread.CurrentCulture.Calendar;
				DateTime cur = dateTimePicker.Value;
				DateTime date = (DateTime) prov.ObjectItem;

				switch (part_data.date_time_part) {
				case DateTimePicker.DateTimePart.Month:
					return (date.Month == cur.Month);
				case DateTimePicker.DateTimePart.DayName:
					return (cal.GetDayOfWeek (date)
						== cal.GetDayOfWeek (cur));
				case DateTimePicker.DateTimePart.AMPMSpecifier:
					return (date == amDate && !IsTimePM (cur))
					        || (date == pmDate && IsTimePM (cur));
				}
				return false;
			}
			
			public int SelectedItemsCount {
				get { return 1; }
			}
			
			public void SelectItem (ListItemProvider prov)
			{
				if (dateTimePicker.ShowCheckBox && !dateTimePicker.Checked)
					return;

				DateTime cur = dateTimePicker.Value;
				Calendar cal = Thread.CurrentThread.CurrentCulture.Calendar;
				DateTime date = (DateTime) prov.ObjectItem;

				switch (part_data.date_time_part) {
				case DateTimePicker.DateTimePart.Month:
					cur = cur.AddMonths (date.Month - cur.Month);
					break;
				case DateTimePicker.DateTimePart.DayName:
					cur = cur.AddDays (cal.GetDayOfWeek (date)
					                   - cal.GetDayOfWeek (dateTimePicker.Value));
					break;
				case DateTimePicker.DateTimePart.AMPMSpecifier:
					if (date == amDate && IsTimePM (cur)) {
						cur = cur.AddHours (-12);
					} else if (date == pmDate && !IsTimePM (cur)) {
						cur = cur.AddHours (12);
					}
					break;
				}
				dateTimePicker.Value = cur;
			}

			public void UnselectItem (ListItemProvider prov)
			{
				throw new NotSupportedException ();
			}
			
			public void ScrollItemIntoView (ListItemProvider item)
			{
				throw new NotSupportedException ();
			}

			public event Action ScrollPatternSupportChanged;

			public ListItemProvider GetSelectedItem ()
			{
				DateTime cur = dateTimePicker.Value;
				Calendar cal = Thread.CurrentThread.CurrentCulture.Calendar;
				DateTime searchDate = DateTime.MinValue;

				switch (part_data.date_time_part) {
				case DateTimePicker.DateTimePart.Month:
					searchDate = new DateTime (cur.Year, cur.Month, 1);
					break;
				case DateTimePicker.DateTimePart.DayName:
					searchDate = monday.AddDays (cal.GetDayOfWeek (cur)
					                             - cal.GetDayOfWeek (monday));
					break;
				case DateTimePicker.DateTimePart.AMPMSpecifier:
					searchDate = IsTimePM (cur) ? pmDate : amDate;
					break;
				}

				if (searchDate != DateTime.MinValue
				    && childrenData.ContainsKey (searchDate))
					return childrenData [searchDate];
				return null;
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
					return true;
				else if (propertyId == AEIds.HasKeyboardFocusProperty.Id)
					return (bool) rootProvider.GetPropertyValue (AEIds.HasKeyboardFocusProperty.Id)
					       && part_data.Selected;
				else if (propertyId == AEIds.NameProperty.Id)
					return Text;
				else if (propertyId == AEIds.ControlTypeProperty.Id)
					return ControlType.List.Id;
				return base.GetProviderPropertyValue (propertyId);
			}
			
			private void AddChildDate (DateTime date)
			{
				DateTimePickerListPartItemProvider item
					= new DateTimePickerListPartItemProvider (
						rootProvider, this, date);
				item.Initialize ();

				children.Add (item);
				childrenData [date] = item;
				AddChildProvider (item);
			}

			private bool IsTimePM (DateTime date)
			{
				return (date.Hour > 12)
				        || (date.Hour == 12 && date.Minute > 0);
			}

			private DateTimePicker dateTimePicker;
			private List<DateTimePickerListPartItemProvider> children
				= new List<DateTimePickerListPartItemProvider> ();
			private Dictionary<DateTime, DateTimePickerListPartItemProvider> childrenData
				= new Dictionary<DateTime, DateTimePickerListPartItemProvider> ();

			private readonly DateTime monday = new DateTime (2008, 12, 1); 
			private readonly DateTime amDate = new DateTime (2008, 12, 1, 1, 0, 0); 
			private readonly DateTime pmDate = new DateTime (2008, 12, 1, 13, 0, 0); 
		}

		internal class DateTimePickerListPartItemProvider
			: ListItemProvider
		{
			public DateTimePickerListPartItemProvider (FragmentRootControlProvider rootProvider,
							           DateTimePickerListPartProvider provider,
			                                           object objectItem)
				: base (rootProvider, provider, null, objectItem)
			{
				this.partProvider = provider;
			}
			
			public string Text {
				get { return partProvider.PartData.GetText ((DateTime) ObjectItem); }
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AEIds.IsEnabledProperty.Id) {
					return true;
				} else if (propertyId == AEIds.IsOffscreenProperty.Id) {
					// Item is onscreen only when selected
					return !ListProvider.IsItemSelected (this);
				}
				return base.GetProviderPropertyValue (propertyId);
			}

			private DateTimePickerListPartProvider partProvider;
		}
#endregion
	}
}
