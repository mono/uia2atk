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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
//using System.Windows;
using System.Windows.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;
using MonoTests.System.Windows.Automation;
using At = System.Windows.Automation.Automation;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class TogglePatternTest : BaseTest
	{
		#region Test Methods

		[Test]
		public void PropertiesTest ()
		{
			CheckPatternIdentifiers<TogglePattern> ();
		}

		[Test]
		public void ToggleTest ()
		{
			TogglePattern pattern = (TogglePattern) checkbox1Element.GetCurrentPattern (TogglePattern.Pattern);
			Assert.AreEqual (ToggleState.Off, pattern.Current.ToggleState, "ToggleState before Toggle");
			pattern.Toggle ();
			// TODO: Enable this after resolving at-spi-sharp threading/MainLoop issues
			if (!Atspi)
				Assert.AreEqual (ToggleState.On, pattern.Current.ToggleState, "ToggleState after Toggle");
		}

		[Test]
		public void NotEnabledTest ()
		{
			TogglePattern pattern = null;
			if (!Atspi) {
				RunCommand ("disable checkBox1");
				pattern = (TogglePattern) checkbox1Element.GetCurrentPattern (TogglePattern.Pattern);
			} else {
				pattern = (TogglePattern) checkbox2Element.GetCurrentPattern (TogglePattern.Pattern);
			}

			try {
				pattern.Toggle ();
				Assert.Fail ("Should throw ElementNotEnabledException");
			} catch (ElementNotEnabledException) { }

			if (!Atspi)
				RunCommand ("enable checkBox1");
		}

		[Test]
		public void Z_EventTest ()
		{
			int eventCount = 0;
			AutomationPropertyChangedEventHandler handler = (o, e) => eventCount++;
			At.AddAutomationPropertyChangedEventHandler (checkbox1Element, TreeScope.Element, handler,
			                                             TogglePattern.ToggleStateProperty);

			TogglePattern pattern = (TogglePattern) checkbox1Element.GetCurrentPattern (TogglePattern.Pattern);
			pattern.Toggle ();
			//We should expect an AutomationPropertyChangedEvent here,
			//But since no such event fired on Windows Winforms,
			//then we assert no event fired here
			int expectedEventCount = (Atspi? 1: 0);
			Assert.AreEqual (expectedEventCount, eventCount, "ToggleState changed event");
		}
		#endregion
	}
}
