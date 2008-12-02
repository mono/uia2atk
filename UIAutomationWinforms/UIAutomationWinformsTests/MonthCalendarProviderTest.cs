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
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class MonthCalendarProviderTest : BaseProviderTest
	{
		private MonthCalendar calendar;
		private IRawElementProviderSimple calendarProvider;

		[SetUp]
		public override void SetUp ()
		{
			base.SetUp ();

			calendar = (MonthCalendar) GetControlInstance ();
			Form.Controls.Add (calendar);
			Form.Show ();

			calendarProvider
				= ProviderFactory.GetProvider (calendar);
		}

		[Test]
		public void BasicPropertiesTest ()
		{
			TestProperty (calendarProvider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Calendar.Id);
			
			TestProperty (calendarProvider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "calendar");
		}

		[Test]
		public void NavigationTest ()
		{
			IRawElementProviderFragmentRoot rootProvider
				= (IRawElementProviderFragmentRoot) calendarProvider;

			IRawElementProviderSimple child
				= rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (child, "MonthCalendar has no children");

			TestProperty (child,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.DataGrid.Id);
			TestProperty (child,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "data grid");

			IRawElementProviderSimple header
				= ((IRawElementProviderFragmentRoot) child).Navigate (
					NavigateDirection.FirstChild);
			Assert.IsNotNull (header, "MonthCalendarDataGrid has no children");

			TestProperty (header,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Header.Id);
			TestProperty (header,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "header");

			int numChildren = 0;

			IRawElementProviderSimple headerItem
				= ((IRawElementProviderFragmentRoot) header).Navigate (
						NavigateDirection.FirstChild);
			while (headerItem != null) {
				TestProperty (headerItem,
					      AutomationElementIdentifiers.ControlTypeProperty,
					      ControlType.HeaderItem.Id);
				TestProperty (headerItem,
					      AutomationElementIdentifiers.LocalizedControlTypeProperty,
					      "header item");
				
				Assert.AreEqual (any_given_sunday.AddDays (numChildren).ToString ("ddd"),
				                 headerItem.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id),
				                 "Day name in header is incorrect");
				
				numChildren++;
				headerItem = ((IRawElementProviderFragment) headerItem)
					.Navigate (NavigateDirection.NextSibling);
			}

			Assert.AreEqual (DAYS_IN_WEEK, numChildren, "Not returning the correct number of days in a week");
		}

		protected override Control GetControlInstance ()
		{
			return new MonthCalendar ();
		}

		// XXX: This will only work in a Gregorian calendar.
		private const int DAYS_IN_WEEK = 7;

		private DateTime any_given_sunday = new DateTime (2008, 12, 7);
	}
}
