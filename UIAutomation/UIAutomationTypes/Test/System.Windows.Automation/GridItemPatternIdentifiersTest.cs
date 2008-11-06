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
//	Neville Gao <nevillegao@gmail.com>
// 

using System;
using System.Windows.Automation;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation
{

	[TestFixture]
	public class GridItemPatternIdentifiersTest
	{

		[Test]
		public void PatternTest ()
		{
			AutomationPattern pattern = GridItemPatternIdentifiers.Pattern;
			Assert.IsNotNull (pattern,
					"Pattern field must not be null");
			Assert.AreEqual (10007,
					pattern.Id,
					"Id");
			Assert.AreEqual ("GridItemPatternIdentifiers.Pattern",
					pattern.ProgrammaticName,
					"ProgrammaticName");
			Assert.AreEqual (pattern,
					AutomationPattern.LookupById (pattern.Id),
					"LookupById");
		}

		[Test]
		public void RowPropertyTest ()
		{
			AutomationProperty property = GridItemPatternIdentifiers.RowProperty;
			Assert.IsNotNull (property,
					"Property field must not be null");
			Assert.AreEqual (30064,
					property.Id,
					"Id");
			Assert.AreEqual ("GridItemPatternIdentifiers.RowProperty",
					property.ProgrammaticName,
					"ProgrammaticName");
			Assert.AreEqual (property,
					AutomationProperty.LookupById (property.Id),
					"LookupById");
		}

		[Test]
		public void ColumnPropertyTest ()
		{
			AutomationProperty property = GridItemPatternIdentifiers.ColumnProperty;
			Assert.IsNotNull (property,
					"Property field must not be null");
			Assert.AreEqual (30065,
					property.Id,
					"Id");
			Assert.AreEqual ("GridItemPatternIdentifiers.ColumnProperty",
					property.ProgrammaticName,
					"ProgrammaticName");
			Assert.AreEqual (property,
					AutomationProperty.LookupById (property.Id),
					"LookupById");
		}

		[Test]
		public void RowSpanPropertyTest ()
		{
			AutomationProperty property = GridItemPatternIdentifiers.RowSpanProperty;
			Assert.IsNotNull (property,
					"Property field must not be null");
			Assert.AreEqual (30066,
					property.Id,
					"Id");
			Assert.AreEqual ("GridItemPatternIdentifiers.RowSpanProperty",
					property.ProgrammaticName,
					"ProgrammaticName");
			Assert.AreEqual (property,
					AutomationProperty.LookupById (property.Id),
					"LookupById");
		}

		[Test]
		public void ColumnSpanPropertyTest ()
		{
			AutomationProperty property = GridItemPatternIdentifiers.ColumnSpanProperty;
			Assert.IsNotNull (property,
					"Property field must not be null");
			Assert.AreEqual (30067,
					property.Id,
					"Id");
			Assert.AreEqual ("GridItemPatternIdentifiers.ColumnSpanProperty",
					property.ProgrammaticName,
					"ProgrammaticName");
			Assert.AreEqual (property,
					AutomationProperty.LookupById (property.Id),
					"LookupById");
		}

		[Test]
		public void ContainingGridPropertyTest ()
		{
			AutomationProperty property = GridItemPatternIdentifiers.ContainingGridProperty;
			Assert.IsNotNull (property,
					"Property field must not be null");
			Assert.AreEqual (30068,
					property.Id,
					"Id");
			Assert.AreEqual ("GridItemPatternIdentifiers.ContainingGridProperty",
					property.ProgrammaticName,
					"ProgrammaticName");
			Assert.AreEqual (property,
					AutomationProperty.LookupById (property.Id),
					"LookupById");
		}

	}
	
}
