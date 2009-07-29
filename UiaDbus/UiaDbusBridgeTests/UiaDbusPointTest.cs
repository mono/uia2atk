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
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using SW = System.Windows;

using DC = Mono.UIAutomation.UiaDbus;

using NUnit;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.UiaDbusBridge
{
	[TestFixture]
	public class UiaDbusPointTest
	{
		[Test]
		public void BasicTest ()
		{
			SW.Point nonEmptyPoint =
				new SW.Point (1,2);
			SW.Point emptyPoint =
				new SW.Point (double.NegativeInfinity, double.NegativeInfinity);

			DC.Point nonEmptyCorePoint = new DC.Point (nonEmptyPoint);
			Assert.AreEqual (nonEmptyPoint.X,
			                 nonEmptyCorePoint.x,
			                 "x from Point");
			Assert.AreEqual (nonEmptyPoint.Y,
			                 nonEmptyCorePoint.y,
			                 "y from Point");

			DC.Point emptyCorePoint = new DC.Point (emptyPoint);
			Assert.AreEqual (emptyPoint.X,
			                 emptyCorePoint.x,
			                 "x from Point");
			Assert.AreEqual (emptyPoint.Y,
			                 emptyCorePoint.y,
			                 "y from Point");

			nonEmptyCorePoint.x = 5;
			Assert.AreEqual (5,
			                 nonEmptyCorePoint.x,
			                 "x set manually");
			nonEmptyCorePoint.y = 6;
			Assert.AreEqual (6,
			                 nonEmptyCorePoint.y,
			                 "y set manually");
		}

		[Test]
		public void ToStringTest ()
		{
			SW.Point nonEmptyPoint =
				new SW.Point (1,2);
			SW.Point emptyPoint =
				new SW.Point (double.NegativeInfinity, double.NegativeInfinity);

			Assert.AreEqual ("1,2",
			                 new DC.Point (nonEmptyPoint).ToString (),
			                 "Typical non-empty Point");
			Assert.AreEqual ("-Infinity,-Infinity",
			                 new DC.Point (emptyPoint).ToString (),
			                 "Empty Point");
		}
	}
}
