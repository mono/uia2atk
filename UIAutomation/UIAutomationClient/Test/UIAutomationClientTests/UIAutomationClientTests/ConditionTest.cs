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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Collections.Generic;
using System.Windows;
using SWA = System.Windows.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class ConditionTest
	{
		[Test]
		public void TrueConditionTest ()
		{
			SWA.Condition trueCond = SWA.Condition.TrueCondition;
			Assert.IsNotNull (trueCond, "TrueCondition");

			SWA.PropertyCondition truePropCond = trueCond as SWA.PropertyCondition;
			Assert.IsNull (truePropCond, "TrueCondition is not a PropertyCondition");

			SWA.AndCondition trueAndCond = trueCond as SWA.AndCondition;
			Assert.IsNull (trueAndCond, "TrueCondition is not a AndCondition");

			SWA.OrCondition trueOrCond = trueCond as SWA.OrCondition;
			Assert.IsNull (trueOrCond, "TrueCondition is not a OrCondition");

			SWA.NotCondition trueNotCond = trueCond as SWA.NotCondition;
			Assert.IsNull (trueNotCond, "TrueCondition is not a NotCondition");
		}

		[Test]
		public void FalseConditionTest ()
		{
			SWA.Condition falseCond = SWA.Condition.FalseCondition;
			Assert.IsNotNull (falseCond, "FalseCondition");

			SWA.PropertyCondition falsePropCond = falseCond as SWA.PropertyCondition;
			Assert.IsNull (falsePropCond, "FalseCondition is not a PropertyCondition");

			SWA.AndCondition falseAndCond = falseCond as SWA.AndCondition;
			Assert.IsNull (falseAndCond, "FalseCondition is not a AndCondition");

			SWA.OrCondition falseOrCond = falseCond as SWA.OrCondition;
			Assert.IsNull (falseOrCond, "FalseCondition is not a OrCondition");

			SWA.NotCondition falseNotCond = falseCond as SWA.NotCondition;
			Assert.IsNull (falseNotCond, "FalseCondition is not a NotCondition");
		}
	}
}
