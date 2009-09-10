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
	public class UiaDbusRectTest
	{
		[Test]
		public void BasicTest ()
		{
			SW.Rect nonEmptyRect = new SW.Rect (1,2,3,4);
			SW.Rect emptyRect = SW.Rect.Empty;

			DC.Rect nonEmptyCoreRect = new DC.Rect (nonEmptyRect);
			Assert.AreEqual (nonEmptyRect.X,
			                 nonEmptyCoreRect.x,
			                 "x from rect");
			Assert.AreEqual (nonEmptyRect.Y,
			                 nonEmptyCoreRect.y,
			                 "y from rect");
			Assert.AreEqual (nonEmptyRect.Width,
			                 nonEmptyCoreRect.width,
			                 "width from rect");
			Assert.AreEqual (nonEmptyRect.Height,
			                 nonEmptyCoreRect.height,
			                 "height from rect");

			DC.Rect emptyCoreRect = new DC.Rect (emptyRect);
			Assert.AreEqual (emptyRect.X,
			                 emptyCoreRect.x,
			                 "x from rect");
			Assert.AreEqual (emptyRect.Y,
			                 emptyCoreRect.y,
			                 "y from rect");
			Assert.AreEqual (emptyRect.Width,
			                 emptyCoreRect.width,
			                 "width from rect");
			Assert.AreEqual (emptyRect.Height,
			                 emptyCoreRect.height,
			                 "height from rect");

			nonEmptyCoreRect.x = 5;
			Assert.AreEqual (5,
			                 nonEmptyCoreRect.x,
			                 "x set manually");
			nonEmptyCoreRect.y = 6;
			Assert.AreEqual (6,
			                 nonEmptyCoreRect.y,
			                 "y set manually");
			nonEmptyCoreRect.width = 7;
			Assert.AreEqual (7,
			                 nonEmptyCoreRect.width,
			                 "width set manually");
			nonEmptyCoreRect.height = 8;
			Assert.AreEqual (8,
			                 nonEmptyCoreRect.height,
			                 "height set manually");
		}

		[Test]
		public void IsEmptyTest ()
		{
			SW.Rect nonEmptyRect = new SW.Rect (1,2,3,4);
			SW.Rect emptyRect = SW.Rect.Empty;

			Assert.IsFalse (new DC.Rect (nonEmptyRect).IsEmpty,
			                "Non-empty rect should not be considered empty");
			Assert.IsTrue (new DC.Rect (emptyRect).IsEmpty,
			               "Empty rect should be considered empty");
		}

		[Test]
		public void ToSWRectTest ()
		{
			SW.Rect nonEmptyRect = new SW.Rect (1,2,3,4);
			DC.Rect nonEmptyDCRect= new DC.Rect (nonEmptyRect);
			Assert.AreEqual (nonEmptyDCRect.ToSWRect (), nonEmptyRect, "Convert non-empty rect");
			DC.Rect emptyDCRect= new DC.Rect (SW.Rect.Empty);
			Assert.AreEqual (emptyDCRect.ToSWRect (), SW.Rect.Empty, "Convert empty rect");
		}

		[Test]
		public void ToStringTest ()
		{
			SW.Rect nonEmptyRect = new SW.Rect (1,2,3,4);
			SW.Rect emptyRect = SW.Rect.Empty;

			Assert.AreEqual ("1,2,3,4",
			                 new DC.Rect (nonEmptyRect).ToString (),
			                 "Typical non-empty rect");
			Assert.AreEqual ("Empty",
			                 new DC.Rect (emptyRect).ToString (),
			                 "Empty rect");
		}
	}
}
