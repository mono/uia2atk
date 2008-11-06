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
using System.Windows.Automation.Text;
using NUnit.Framework;

namespace MonoTests.System.Windows.Automation.Text
{
    [TestFixture]
    public class AnimationStyleTest
    {

        [Test]
        public void ValuesTest()
        {
            Assert.AreEqual (0,  (int) AnimationStyle.None, "None");
            Assert.AreEqual (1,  (int) AnimationStyle.LasVegasLights, "LasVegasLights");
            Assert.AreEqual (2,  (int) AnimationStyle.BlinkingBackground, "BlinkingBackground");
            Assert.AreEqual (3,  (int) AnimationStyle.SparkleText, "SparkleText");
            Assert.AreEqual (4,  (int) AnimationStyle.MarchingBlackAnts, "MarchingBlackAnts");
            Assert.AreEqual (5,  (int) AnimationStyle.MarchingRedAnts, "MarchingRedAnts");
            Assert.AreEqual (6,  (int) AnimationStyle.Shimmer, "Shimmer");
            Assert.AreEqual (-1, (int) AnimationStyle.Other, "Other");
        }

    }
}
