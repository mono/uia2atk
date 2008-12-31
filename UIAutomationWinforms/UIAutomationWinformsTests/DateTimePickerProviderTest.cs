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
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class DateTimePickerProviderTest : BaseProviderTest
	{
		[SetUp]
		public override void SetUp ()
		{
			base.SetUp ();
			
			oldCulture = Thread.CurrentThread.CurrentCulture;
			oldCulture.ToString ();

			// Ensure that we're in the US locale so that we can
			// make certian assumptions
			Thread.CurrentThread.CurrentCulture = new CultureInfo ("en-US");
			currentCalendar = Thread.CurrentThread.CurrentCulture.Calendar;
			currentCalendar.ToString ();

			picker = (DateTimePicker) GetControlInstance ();
			Form.Controls.Add (picker);
			Form.Show ();

			pickerProvider
				= ProviderFactory.GetProvider (picker);
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
			picker.ShowCheckBox = false;
			TestProperty (pickerProvider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Pane.Id);
			
			TestProperty (pickerProvider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "pane");
		}

		[Test]
		public void NavigationTest ()
		{
			picker.ShowUpDown = false;

			picker.Format = DateTimePickerFormat.Long;
			picker.Value = awesome;

			// Should be:
			string[] name_values = new string[] {
				"Tuesday",	// DayName
				",",		// Literal
				" ",		// Literal
				"January",	// Literal
				" ",		// Literal
				"20",		// Day
				",",		// Literal
				" ",		// Literal
				"2009"		// Year
			};

			IRawElementProviderSimple child
				= ((IRawElementProviderFragmentRoot) pickerProvider)
					.Navigate (NavigateDirection.FirstChild);
			int i = 0;
			do {
				TestProperty (child,
				              AutomationElementIdentifiers.NameProperty,
				              name_values[i]);

				if (long_pattern_part_is_spinner[i]) {
					TestProperty (child,
						      AutomationElementIdentifiers.ControlTypeProperty,
						      ControlType.Spinner.Id);
					TestProperty (child,
						      AutomationElementIdentifiers.IsKeyboardFocusableProperty,
						      true);

					((IRawElementProviderFragment) child).SetFocus ();
					TestProperty (child,
						      AutomationElementIdentifiers.HasKeyboardFocusProperty,
						      true);
				} else {
					TestProperty (child,
						      AutomationElementIdentifiers.ControlTypeProperty,
						      ControlType.Text.Id);
					TestProperty (child,
						      AutomationElementIdentifiers.IsKeyboardFocusableProperty,
						      false);

					((IRawElementProviderFragment) child).SetFocus ();
					TestProperty (child,
						      AutomationElementIdentifiers.HasKeyboardFocusProperty,
						      false);
				}
				
				i++;
				child = ((IRawElementProviderFragment) child)
					.Navigate (NavigateDirection.NextSibling);
			} while (child != null && i < long_pattern_num_parts);

			// Last control should be a button
			TestProperty (child,
				      AutomationElementIdentifiers.ControlTypeProperty,
				      ControlType.Button.Id);

			child = ((IRawElementProviderFragment) child)
				.Navigate (NavigateDirection.NextSibling);
			Assert.IsNull (child, "More than expected number of children");
		}

		[Test]
		public void RangeValueTest ()
		{
			// This format should have 8 parts
			picker.Format = DateTimePickerFormat.Long;
			picker.ShowUpDown = false;
			picker.Value = awesome;
	
			double[] awesome_values = new double[] {
				-1, -1, -1, -1, -1, 20, -1, -1, 2009
			};
	
			double[] awesome_min_values = new double[] {
				-1, -1, -1, -1, -1, 1, -1, -1, 1753
			};

			double[] awesome_max_values = new double[] {
				-1, -1, -1, -1, -1, 31, -1, -1, 9998
			};

			double[] set_value_value = new double[] {
				-1, -1, -1, -1, -1, 30, -1, -1, 2007
			};

			IRawElementProviderSimple child
				= ((IRawElementProviderFragmentRoot) pickerProvider)
					.Navigate (NavigateDirection.FirstChild);
			int i = 0;
			do {
				if (long_pattern_part_is_spinner[i]) {
					TestProperty (child,
						      AutomationElementIdentifiers.ControlTypeProperty,
						      ControlType.Spinner.Id);
		
					IRangeValueProvider rangeValueProvider
						= child.GetPatternProvider (
							RangeValuePatternIdentifiers.Pattern.Id)
								as IRangeValueProvider;
					Assert.IsNotNull (rangeValueProvider,
							  "Spinner control does not implement RangeValue");
					
					Assert.AreEqual (awesome_values[i], rangeValueProvider.Value,
					                 "Value returned by RangeValue is incorrect");
					Assert.AreEqual (awesome_min_values[i], rangeValueProvider.Minimum,
					                 "Minimum returned by RangeValue is incorrect");
					Assert.AreEqual (awesome_max_values[i], rangeValueProvider.Maximum,
					                 "Maximum returned by RangeValue is incorrect");

					bridge.ResetEventLists ();

					rangeValueProvider.SetValue (set_value_value[i]);

					Assert.AreEqual (set_value_value[i], rangeValueProvider.Value,
					                 "After setting, Value returned by RangeValue is incorrect");

					Assert.AreEqual (1, bridge.AutomationPropertyChangedEvents.Count, "Event count");

					rangeValueProvider.SetValue (awesome_values[i]);

					picker.ShowCheckBox = true;
					picker.Checked = true;
					Assert.IsFalse (rangeValueProvider.IsReadOnly,
					                "RangeValue is read only when Checked = true and ShowCheckBox = true");

					bridge.ResetEventLists ();

					picker.Checked = false;

					// Two events are generated because there are 2 active spinner parts
					Assert.AreEqual (2, bridge.GetAutomationPropertyEventCount (
						RangeValuePatternIdentifiers.IsReadOnlyProperty), "Event count");

					Assert.IsTrue (rangeValueProvider.IsReadOnly,
					               "RangeValue is not read only when Checked = false and ShowCheckBox = true");

					picker.ShowCheckBox = false;
					Assert.IsFalse (rangeValueProvider.IsReadOnly,
					                "RangeValue is read only when ShowCheckBox = false");
				}
				
				i++;
				child = ((IRawElementProviderFragment) child)
					.Navigate (NavigateDirection.NextSibling);
			} while (child != null && i < long_pattern_num_parts);
		}

		[Test]
		public void ButtonInvokeTest ()
		{
			picker.Format = DateTimePickerFormat.Long;
			picker.ShowUpDown = false;
			picker.Value = awesome;

			IRawElementProviderSimple child
				= ((IRawElementProviderFragmentRoot) pickerProvider)
					.Navigate (NavigateDirection.FirstChild);

			// Skip over the Part items
			int i = 0;
			do {
				i++;
				child = ((IRawElementProviderFragment) child)
					.Navigate (NavigateDirection.NextSibling);
			} while (child != null && i < long_pattern_num_parts);

			TestProperty (child,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Button.Id);

			IInvokeProvider invokeProvider
				= child.GetPatternProvider (
					InvokePatternIdentifiers.Pattern.Id) as IInvokeProvider;
			Assert.IsNotNull (invokeProvider,
			                  "DateTimePicker's drop down button does not implement IInvokeProvider");

			bridge.ResetEventLists ();

			invokeProvider.Invoke ();

			Assert.AreEqual (1, bridge.AutomationEvents.Count, "Event count");


			picker.ShowUpDown = true;

			child = ((IRawElementProviderFragmentRoot) pickerProvider)
				.Navigate (NavigateDirection.FirstChild);

			// Skip over the Part items
			i = 0;
			do {
				i++;
				child = ((IRawElementProviderFragment) child)
					.Navigate (NavigateDirection.NextSibling);
			} while (child != null && i < long_pattern_num_parts);
			
			Assert.IsNull (child,
			               "Button child still exists after setting ShowUpDown = true");
		}
		
		[Test]
		public void ToggleTest ()
		{
			picker.ShowCheckBox = false;
			IToggleProvider toggleProvider
				= pickerProvider.GetPatternProvider (
					TogglePatternIdentifiers.Pattern.Id) as IToggleProvider;
			Assert.IsNull (toggleProvider,
			               "With ShowCheckBox = false, DateTimePicker does not implement IToggleProvider");

			picker.ShowCheckBox = true;

			toggleProvider = pickerProvider.GetPatternProvider (
				TogglePatternIdentifiers.Pattern.Id) as IToggleProvider;
			Assert.IsNotNull (toggleProvider,
			                  "With ShowCheckBox = true, DateTimePicker does not implement IToggleProvider");

			picker.Checked = false;
			Assert.AreEqual (ToggleState.Off, toggleProvider.ToggleState,
			                 "With Checked = false, toggleProvider is not returning ToggleState.Off");

			picker.Checked = true;
			Assert.AreEqual (ToggleState.On, toggleProvider.ToggleState,
			                 "With Checked = true, toggleProvider is not returning ToggleState.On");

			toggleProvider.Toggle ();
			Assert.IsFalse (picker.Checked, "After toggling off, Checked is still true");
			Assert.AreEqual (ToggleState.Off, toggleProvider.ToggleState,
			                 "After toggling off, toggleProvider is not returning ToggleState.Off");

			toggleProvider.Toggle ();
			Assert.IsTrue (picker.Checked, "After toggling on, Checked is still false");
			Assert.AreEqual (ToggleState.On, toggleProvider.ToggleState,
			                 "After toggling on, toggleProvider is not returning ToggleState.On");
		}

		protected override Control GetControlInstance ()
		{
			return new DateTimePicker ();
		}

		private DateTimePicker picker;
		private IRawElementProviderSimple pickerProvider;

		private Calendar currentCalendar;
		private CultureInfo oldCulture;

		private int long_pattern_num_parts = 9;
		private bool[] long_pattern_part_is_spinner = new bool[] {
			false, false, false, false, false, true, false, false, true
		};

		private DateTime awesome = new DateTime (2009, 1, 20);
	}
}
