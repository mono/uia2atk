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
//  Mario Carrion <mcarrion@novell.com>
//
using System;
using System.Windows.Automation;
using NUnit.Framework;

namespace MonoTests.System.Windows.Automation
{
    [TestFixture]
	public class DockPatternIdentifiersTest
    {

		[Test]
		public void PatternTest ()
		{
			AutomationPattern pattern = DockPatternIdentifiers.Pattern;
			Assert.IsNotNull (pattern, "Pattern field must not be null");
			Assert.AreEqual (10011, pattern.Id, "Id");
			Assert.AreEqual ("DockPatternIdentifiers.Pattern", pattern.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (pattern, AutomationPattern.LookupById (pattern.Id), "LookupById");
		}

		[Test]
		public void ToggleStatePropertyTest ()
		{
			AutomationProperty property = DockPatternIdentifiers.DockPositionProperty;
			Assert.IsNotNull (property, "Property field must not be null");
			Assert.AreEqual (30069, property.Id, "Id");
			Assert.AreEqual ("DockPatternIdentifiers.DockPositionProperty", property.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (property, AutomationProperty.LookupById (property.Id), "LookupById");
		}

    }
}
