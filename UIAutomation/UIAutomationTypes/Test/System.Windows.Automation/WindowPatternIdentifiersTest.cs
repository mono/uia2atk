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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Windows.Automation;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation {

	[TestFixture]
	public class WindowPatternIdentifiersTest {

		[Test]
		public void PatternTest ()
		{
			AutomationPattern pattern = WindowPatternIdentifiers.Pattern;
			Assert.IsNotNull (pattern, "Pattern field must not be null");
			Assert.AreEqual (10009, pattern.Id, "Id");
			Assert.AreEqual ("WindowPatternIdentifiers.Pattern", pattern.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (pattern, AutomationPattern.LookupById (pattern.Id), "LookupById");
		}

		[Test]
		public void WindowClosedEventTest ()
		{
			AutomationEvent automationEvent = WindowPatternIdentifiers.WindowClosedEvent;
			Assert.IsNotNull (automationEvent, "Property field must not be null");
			Assert.AreEqual (20017, automationEvent.Id, "Id");
			Assert.AreEqual ("WindowPatternIdentifiers.WindowClosedProperty", automationEvent.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (automationEvent, AutomationEvent.LookupById (automationEvent.Id), "LookupById");
		}

		[Test]
		public void WindowOpenedEventTest ()
		{
			AutomationEvent automationEvent = WindowPatternIdentifiers.WindowOpenedEvent;
			Assert.IsNotNull (automationEvent, "Property field must not be null");
			Assert.AreEqual (20016, automationEvent.Id, "Id");
			Assert.AreEqual ("WindowPatternIdentifiers.WindowOpenedProperty", automationEvent.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (automationEvent, AutomationEvent.LookupById (automationEvent.Id), "LookupById");
		}

		[Test]
		public void CanMaximizePropertyTest ()
		{
			AutomationProperty property = WindowPatternIdentifiers.CanMaximizeProperty;
			Assert.IsNotNull (property, "Property field must not be null");
			Assert.AreEqual (30073, property.Id, "Id");
			Assert.AreEqual ("WindowPatternIdentifiers.CanMaximizeProperty", property.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (property, AutomationProperty.LookupById (property.Id), "LookupById");
		}

		[Test]
		public void CanMinimizePropertyPropertyTest ()
		{
			AutomationProperty property = WindowPatternIdentifiers.CanMinimizeProperty;
			Assert.IsNotNull (property, "Property field must not be null");
			Assert.AreEqual (30074, property.Id, "Id");
			Assert.AreEqual ("WindowPatternIdentifiers.CanMinimizeProperty", property.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (property, AutomationProperty.LookupById (property.Id), "LookupById");
		}

		[Test]
		public void IsModalPropertyPropertyTest ()
		{
			AutomationProperty property = WindowPatternIdentifiers.IsModalProperty;
			Assert.IsNotNull (property, "Property field must not be null");
			Assert.AreEqual (30077, property.Id, "Id");
			Assert.AreEqual ("WindowPatternIdentifiers.IsModalProperty", property.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (property, AutomationProperty.LookupById (property.Id), "LookupById");
		}

		[Test]
		public void IsTopmostPropertyTest ()
		{
			AutomationProperty property = WindowPatternIdentifiers.IsTopmostProperty;
			Assert.IsNotNull (property, "Property field must not be null");
			Assert.AreEqual (30078, property.Id, "Id");
			Assert.AreEqual ("WindowPatternIdentifiers.IsTopmostProperty", property.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (property, AutomationProperty.LookupById (property.Id), "LookupById");
		}

		[Test]
		public void WindowInteractionStatePropertyTest ()
		{
			AutomationProperty property = WindowPatternIdentifiers.WindowInteractionStateProperty;
			Assert.IsNotNull (property, "Property field must not be null");
			Assert.AreEqual (30076, property.Id, "Id");
			Assert.AreEqual ("WindowPatternIdentifiers.WindowInteractionStateProperty", property.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (property, AutomationProperty.LookupById (property.Id), "LookupById");
		}

		[Test]
		public void WindowVisualStatePropertyTest ()
		{
			AutomationProperty property = WindowPatternIdentifiers.WindowVisualStateProperty;
			Assert.IsNotNull (property, "Property field must not be null");
			Assert.AreEqual (30075, property.Id, "Id");
			Assert.AreEqual ("WindowPatternIdentifiers.WindowVisualStateProperty", property.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (property, AutomationProperty.LookupById (property.Id), "LookupById");
		}

	}
}
