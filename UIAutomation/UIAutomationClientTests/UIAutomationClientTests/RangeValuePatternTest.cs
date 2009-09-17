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

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class RangeValuePatternTest : BaseTest
	{
		#region Test Methods
		[Test]
		public void RangeValueTest ()
		{
			RangeValuePattern pattern = (RangeValuePattern) numericUpDown1Element.GetCurrentPattern (RangeValuePatternIdentifiers.Pattern);
			Assert.AreEqual (0, pattern.Current.Minimum, "RangeValue Minimum");
			Assert.AreEqual (100, pattern.Current.Maximum, "RangeValue Maximum");
			Assert.AreEqual (10, pattern.Current.SmallChange, "RangeValue SmallChange");
			pattern.SetValue (50);
			Assert.AreEqual (50, pattern.Current.Value, "RangeValue Value after set");
			pattern.SetValue (500);
			Assert.AreEqual (50, pattern.Current.Value, "RangeValue Value after OOR set");
		}

		[Test]
		public void NotEnabledTest ()
		{
			DisableControls ();

			RangeValuePattern pattern = (RangeValuePattern) numericUpDown1Element.GetCurrentPattern (RangeValuePatternIdentifiers.Pattern);
			try {
				pattern.SetValue (50);
				Assert.Fail ("Should throw ElementNotEnabledException");
			} catch (ElementNotEnabledException) { }
			EnableControls ();
		}
		#endregion
	}
}
