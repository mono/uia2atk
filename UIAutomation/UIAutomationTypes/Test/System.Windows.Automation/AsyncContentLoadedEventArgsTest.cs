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
//      Stephen Shaw <sshaw@decriptor.com>
// 

using System;
using System.Windows.Automation;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation {

    [TestFixture]
    public class AsyncContentLoadedEventArgsTest {

        [Test]
        public void PercentCompleteTest ()
        {
            AsyncContentLoadedEventArgs args = new AsyncContentLoadedEventArgs (AsyncContentLoadedState.Beginning, 0.0);
            Assert.AreEqual (0.0, args.PercentComplete, "0.0");
            args = new AsyncContentLoadedEventArgs (AsyncContentLoadedState.Beginning, 50.0);
            Assert.AreEqual (50.0, args.PercentComplete, "50.0");
            args = new AsyncContentLoadedEventArgs (AsyncContentLoadedState.Beginning, 100.0);
            Assert.AreEqual (100.0, args.PercentComplete, "100.0");
            args = new AsyncContentLoadedEventArgs (AsyncContentLoadedState.Beginning, 101.0);
            Assert.AreEqual (101.0, args.PercentComplete, "101.0");
            args = new AsyncContentLoadedEventArgs (AsyncContentLoadedState.Beginning, -1.0);
            Assert.AreEqual (-1.0, args.PercentComplete, "-1.0");
        }

        [Test]
        public void AsyncContentLoadedStateTest ()
        {
            AsyncContentLoadedEventArgs args = new AsyncContentLoadedEventArgs (AsyncContentLoadedState.Beginning, 0.0);
            Assert.AreEqual (AsyncContentLoadedState.Beginning, args.AsyncContentLoadedState, "Beginning");
            args = new AsyncContentLoadedEventArgs (AsyncContentLoadedState.Progress, 0.0);
            Assert.AreEqual (AsyncContentLoadedState.Progress, args.AsyncContentLoadedState, "Progress");
            args = new AsyncContentLoadedEventArgs (AsyncContentLoadedState.Completed, 0.0);
            Assert.AreEqual (AsyncContentLoadedState.Completed, args.AsyncContentLoadedState, "Completed");
        }

        [Test]
        public void EventIdTest ()
        {
            AsyncContentLoadedEventArgs args = new AsyncContentLoadedEventArgs (AsyncContentLoadedState.Progress, 0.0);
            Assert.AreEqual (AutomationElementIdentifiers.AsyncContentLoadedEvent, args.EventId, "EventId");
        }

    }
}