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
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Automation;
using System.Collections.Generic;
using System.Windows.Automation.Provider;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.UIAutomation.Winforms.Behaviors.DateTimePicker;

namespace Mono.UIAutomation.Winforms
{
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

			control.UIAShowCheckBoxChanged
				+= new EventHandler (OnUIAShowCheckBoxChanged);
			control.UIAShowUpDownChanged
				+= new EventHandler (OnUIAShowUpDownChanged);

			UpdateBehaviors ();
		}
#endregion

#region FragmentControlProvider Implementation
		public override void InitializeChildControlStructure ()
		{
			AddSegmentItems ();

			UpdateChildren ();
		}

		public override void FinalizeChildControlStructure ()
		{
			RemoveSegmentItems ();

			if (dropDownButton != null) {
				dropDownButton.Terminate ();
				dropDownButton = null;
			}
		}
#endregion
		
#region Protected Methods
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return control.ShowCheckBox ? ControlType.CheckBox.Id : ControlType.Pane.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return control.ShowCheckBox ? "check box" : "pane";

			return base.GetProviderPropertyValue (propertyId);
		}
#endregion

#region Private Methods
		private void UpdateChildren ()
		{
			if (control.ShowUpDown) {
				if (dropDownButton != null) {
					OnNavigationChildRemoved (false, dropDownButton);
					dropDownButton.Terminate ();
					dropDownButton = null;
				}
			} else {
				if (dropDownButton == null) {
					dropDownButton
						= new DateTimePickerButtonProvider (this);
					OnNavigationChildAdded (false, dropDownButton);
				}
			}
		}

		private void UpdateBehaviors ()
		{
			if (control.ShowCheckBox) {
				SetBehavior (TogglePatternIdentifiers.Pattern,
				             new ToggleProviderBehavior (this));
			} else {
				SetBehavior (TogglePatternIdentifiers.Pattern,
				             null);
			}
		}

		private void AddSegmentItems ()
		{
			for (int i = 0; i < control.part_data.Length; i++) {
				DateTimePickerPartProvider prov
					= new DateTimePickerPartProvider (
						this, control.part_data[i], i
				);
				prov.Initialize ();
				AddChildProvider (true, prov);
				children.Add (prov);
			}
		}

		private void RemoveSegmentItems ()
		{
			foreach (FragmentControlProvider prov in children) {
				RemoveChildProvider (true, prov);
				prov.Terminate ();
			}

			children.Clear ();

		}

		private void OnUIAShowCheckBoxChanged (object o, EventArgs args)
		{
			UpdateBehaviors ();
		}
		
		private void OnUIAShowUpDownChanged (object o, EventArgs args)
		{
			UpdateChildren ();
		}
#endregion

#region Private Fields
		private DateTimePicker control;
		private DateTimePickerButtonProvider dropDownButton;
		private List<FragmentControlProvider> children = new List<FragmentControlProvider> ();
#endregion

#region Internal Classes
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
					return "Drop Down"; // Localize me!
				else if (propertyId == AEIds.IsContentElementProperty.Id)
					return false;
				else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AEIds.ControlTypeProperty.Id)
					return ControlType.Button.Id;
				else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
					return "button";
				return base.GetProviderPropertyValue (propertyId);
			}

			private DateTimePickerProvider rootProvider;
		}

		internal class DateTimePickerPartProvider : FragmentControlProvider
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

				if (!IsStringValue) {
					SetBehavior (RangeValuePatternIdentifiers.Pattern,
						     new PartRangeValueProviderBehavior (this));
				}
			}

			public override void SetFocus ()
			{
				if (IsStringValue) {
					return;
				}

				((DateTimePicker) rootProvider.Control).SelectPart (part_index);
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AEIds.NameProperty.Id)
					return Text;
				else if (propertyId == AEIds.IsContentElementProperty.Id)
					return true;
				else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
					return !IsStringValue;
				else if (propertyId == AEIds.HasKeyboardFocusProperty.Id)
					return (part_data.Selected && !IsStringValue);
				else if (propertyId == AEIds.ControlTypeProperty.Id)
					return IsStringValue ? ControlType.Text.Id : ControlType.Spinner.Id;
				else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
					return IsStringValue ? "text" : "spinner";
				return base.GetProviderPropertyValue (propertyId);
			}

			private bool IsStringValue {
				get { 
					return (part_data.date_time_part == DateTimePicker.DateTimePart.Literal
					        || part_data.date_time_part == DateTimePicker.DateTimePart.DayName
				                || part_data.date_time_part == DateTimePicker.DateTimePart.Month);
				}
			}

			private int part_index;
			private DateTimePicker.PartData part_data;
			private DateTimePickerProvider rootProvider;
		} 
#endregion
	}
}
