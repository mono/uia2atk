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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//  Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
//using System.Windows;
using System.Windows.Automation;
using At = System.Windows.Automation.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;
using MonoTests.System.Windows.Automation;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class RangeValuePatternTest : BaseTest
	{
		private AutomationElement hScrollBarElement = null;
		private AndCondition horizontalScrollCondition =
			new AndCondition (
				new PropertyCondition (AutomationElementIdentifiers.ControlTypeProperty,
					ControlType.ScrollBar),
				new PropertyCondition (AutomationElementIdentifiers.OrientationProperty,
					OrientationType.Horizontal));

		protected override void CustomFixtureSetUp ()
		{
			base.CustomFixtureSetUp ();
			if (!Atspi) {
				// to enable textBox3's horizontal scroll bar.
				RunCommand ("set textBox3 long text");
				hScrollBarElement = textbox3Element.FindFirst (TreeScope.Children, horizontalScrollCondition);
				Assert.IsNotNull (hScrollBarElement);
			}
		}

		#region Test Methods

		[Test]
		public void PropertiesTest ()
		{
			CheckPatternIdentifiers<RangeValuePattern> ();
		}

		[Test]
		public void RangeValueTest ()
		{
			RangeValuePattern pattern = null;
			if (Atspi) {
				pattern = (RangeValuePattern) numericUpDown1Element.GetCurrentPattern (RangeValuePatternIdentifiers.Pattern);
				Assert.AreEqual (0, pattern.Current.Minimum, "RangeValue Minimum");
				Assert.AreEqual (100, pattern.Current.Maximum, "RangeValue Maximum");
				Assert.AreEqual (10, pattern.Current.SmallChange, "RangeValue SmallChange");
			}
			else {
				//on Windows Winform, NumericUpDown's RangeValuePattern isn't well implemented
				//So we choose to test RangeValuePattern on textBox3's Horzontal Scroll Bar
				pattern = (RangeValuePattern) hScrollBarElement.GetCurrentPattern (RangeValuePattern.Pattern);
				var min = pattern.Current.Minimum;
				var max = pattern.Current.Maximum;
				Assert.AreEqual (0, min, "RangeValue Minimum");
				Assert.Greater (max, min, "Maximum > Minimum");
				Assert.AreEqual (0, pattern.Current.Value, "RangeValue Value");
				Assert.AreEqual (1, pattern.Current.SmallChange, "RangeValue SmallChange");
			}
			pattern.SetValue (50);
			Assert.AreEqual (50, pattern.Current.Value, "RangeValue Value after set");

			try {
				pattern.SetValue (500);
				Assert.Fail ("Should throw ArgumentOutOfRangeException");
			}
			catch (ArgumentOutOfRangeException) { }
			Assert.AreEqual (50, pattern.Current.Value, "RangeValue Value after OOR set");
		}

		[Test]
		public void Z_EventTest ()
		{
			AutomationElement element = (Atspi
				? numericUpDown1Element : hScrollBarElement);
			RangeValuePattern pattern;
				pattern = (RangeValuePattern) element.GetCurrentPattern (RangeValuePatternIdentifiers.Pattern);
			var automationEventsArray = new [] {
				new {Sender = (object) null, Args = (AutomationPropertyChangedEventArgs) null}};
			var automationEvents = automationEventsArray.ToList ();
			automationEvents.Clear ();

			AutomationPropertyChangedEventHandler handler =
				(o, e) => automationEvents.Add (new { Sender = o, Args = e });
			pattern.SetValue (20);

			At.AddAutomationPropertyChangedEventHandler (element,
			                                             TreeScope.Element, handler,
			                                             RangeValuePatternIdentifiers.ValueProperty);
			pattern.SetValue (25);
			if (Atspi) {
				Assert.AreEqual (1, automationEvents.Count, "event count");
				Assert.AreEqual (element, automationEvents [0].Sender, "event sender");
				Assert.AreEqual (RangeValuePattern.ValueProperty, automationEvents [0].Args.Property, "property");
				Assert.AreEqual (20, automationEvents [0].Args.OldValue, "old value");
				Assert.AreEqual (25, automationEvents [0].Args.NewValue, "new value");
			} else {
				Assert.AreEqual (0, automationEvents.Count, "event count");
			}

			At.RemoveAutomationPropertyChangedEventHandler (element,
			                                             handler);
			automationEvents.Clear ();
		}

		[Test]
		public void Z_NotEnabledTest ()
		{
			RangeValuePattern pattern = null;

			if (Atspi) {
				DisableControls ();
				pattern = (RangeValuePattern) numericUpDown1Element.GetCurrentPattern (RangeValuePatternIdentifiers.Pattern);
				try {
					pattern.SetValue (50);
					Assert.Fail ("Should throw ElementNotEnabledException");
				} catch (ElementNotEnabledException) { }
				EnableControls ();
			}
			else {
				RunCommand ("disable textBox3");
				try {
					hScrollBarElement = textbox3Element.FindFirst (TreeScope.Children, horizontalScrollCondition);
					Assert.IsNotNull (hScrollBarElement);
					pattern = (RangeValuePattern) hScrollBarElement.GetCurrentPattern (RangeValuePattern.Pattern);
					Assert.Fail ("Should throw InvalidOperationException " +
					             "to indicate that disabled scrollBar dosen't support RangeValuePattern");
				} catch (InvalidOperationException) { }
			}
		}

		#endregion
	}
}
