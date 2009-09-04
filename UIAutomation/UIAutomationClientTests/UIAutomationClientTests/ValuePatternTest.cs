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
//  Matt Guo <matt@mattguo.com>
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;
using At = System.Windows.Automation.Automation;
using NUnit.Framework;
using MonoTests.System.Windows.Automation;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class ValuePatternTest : BaseTest
	{
		[Test]
		public void ValueTest ()
		{
			string magicStr = "Hello, ValuePatternTest.ValueTest!";
			ValuePattern pattern = (ValuePattern) textbox1Element.GetCurrentPattern (ValuePatternIdentifiers.Pattern);
			pattern.SetValue (magicStr);
			//We add following sleep to make sure the test passes, since
			//"pattern.SetValue (magicStr)" may execute in another thread.
			Thread.Sleep (500);
			string str1 = pattern.Current.Value;
			string str2 = (string)(textbox1Element.GetCurrentPropertyValue (ValuePatternIdentifiers.ValueProperty));
			Assert.AreEqual (magicStr, str1, "Check pattern.Current.Value.");
			Assert.AreEqual (magicStr, str2, "Check ValuePatternIdentifiers.ValueProperty.");
		}

		[Test]
		public void PropertyEventTest ()
		{
			int eventCount = 0;
			string magicStr1 = "ValuePatternTest.PropertyEventTest.m1";
			string magicStr2 = "ValuePatternTest.PropertyEventTest.m2";
			ValuePattern pattern = (ValuePattern) textbox1Element.GetCurrentPattern (ValuePatternIdentifiers.Pattern);
			pattern.SetValue (magicStr1);
			AutomationPropertyChangedEventHandler handler = delegate (object sender, AutomationPropertyChangedEventArgs args) {
				Assert.AreEqual (textbox1Element, sender, "Check event sender");
				Assert.AreEqual (magicStr1, args.OldValue, "Check old Value");
				Assert.AreEqual (magicStr2, args.NewValue, "Check new Value");
				eventCount++;
			};
			At.AddAutomationPropertyChangedEventHandler (textbox1Element, TreeScope.Element, handler,
				new AutomationProperty [] {ValuePattern.ValueProperty, ValuePattern.IsReadOnlyProperty}
			);
			pattern.SetValue (magicStr2);
			Thread.Sleep (500);
			Assert.AreEqual (1, eventCount, "AutomationPropertyChangedEvent is fired.");

			At.RemoveAutomationPropertyChangedEventHandler (textbox1Element, handler);
			pattern.SetValue (magicStr1);
			Thread.Sleep (500);
			Assert.AreEqual (1, eventCount, "AutomationPropertyChangedEvent is not fired.");
		}

		[Test]
		public void ReadOnlyTest ()
		{
			ValuePattern pattern = (ValuePattern) textbox1Element.GetCurrentPattern (ValuePatternIdentifiers.Pattern);
			Assert.AreEqual (false, pattern.Current.IsReadOnly, "Check textbox1's ReadOnly property");

			pattern = (ValuePattern) textbox2Element.GetCurrentPattern (ValuePatternIdentifiers.Pattern);
			Assert.AreEqual (true, pattern.Current.IsReadOnly, "Check textbox2's ReadOnly property");
		}

		[Test]
		[ExpectedException("System.Windows.Automation.ElementNotEnabledException")]
		public void NotEnabledTest ()
		{
			ValuePattern pattern = (ValuePattern) textbox2Element.GetCurrentPattern (ValuePatternIdentifiers.Pattern);
			pattern.SetValue ("123");
		}
	}
}
