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
	public class TransformPatternIdentifiersTest {

		[Test]
		public void PatternTest ()
		{
			AutomationPattern pattern = TransformPatternIdentifiers.Pattern;
			Assert.IsNotNull (pattern, "Pattern field must not be null");
			Assert.AreEqual (10016, pattern.Id, "Id");
			Assert.AreEqual ("TransformPatternIdentifiers.Pattern", pattern.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (pattern, AutomationPattern.LookupById (pattern.Id), "LookupById");
		}

		[Test]
		public void CanMovePropertyTest ()
		{
			AutomationProperty property = TransformPatternIdentifiers.CanMoveProperty;
			Assert.IsNotNull (property, "Property field must not be null");
			Assert.AreEqual (30087, property.Id, "Id");
			Assert.AreEqual ("TransformPatternIdentifiers.CanMoveProperty", property.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (property, AutomationProperty.LookupById (property.Id), "LookupById");	
		}

		[Test]
		public void CanResizePropertyTest ()
		{
			AutomationProperty property = TransformPatternIdentifiers.CanResizeProperty;
			Assert.IsNotNull (property, "Property field must not be null");
			Assert.AreEqual (30088, property.Id, "Id");
			Assert.AreEqual ("TransformPatternIdentifiers.CanResizeProperty", property.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (property, AutomationProperty.LookupById (property.Id), "LookupById");
		}

		[Test]
		public void CanRotatePropertyTest ()
		{
			AutomationProperty property = TransformPatternIdentifiers.CanRotateProperty;
			Assert.IsNotNull (property, "Property field must not be null");
			Assert.AreEqual (30089, property.Id, "Id");
			Assert.AreEqual ("TransformPatternIdentifiers.CanRotateProperty", property.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (property, AutomationProperty.LookupById (property.Id), "LookupById");
		}

	}
}
