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
	public class AutomationEventTest : BaseTest
	{
		[Test]
		public void StructureEventTest ()
		{
			List<AutomationElement> elementEventSenders = new List<AutomationElement> ();
			List<StructureChangeType> elementEventChangeTypes = new List<StructureChangeType> ();
			List<AutomationElement> childrenEventSenders = new List<AutomationElement> ();
			List<StructureChangeType> childrenEventChangeTypes = new List<StructureChangeType> ();
			StructureChangedEventHandler elementHandler = delegate (object sender, StructureChangedEventArgs args) {
				elementEventSenders.Add (sender as AutomationElement);
				elementEventChangeTypes.Add (args.StructureChangeType);
			};
			At.AddStructureChangedEventHandler(panel1Element, TreeScope.Element, elementHandler);
			StructureChangedEventHandler childrenHandler = delegate (object sender, StructureChangedEventArgs args) {
				childrenEventSenders.Add (sender as AutomationElement);
				childrenEventChangeTypes.Add (args.StructureChangeType);
			};
			At.AddStructureChangedEventHandler(panel1Element, TreeScope.Children, childrenHandler);
			InvokePattern addAction = (InvokePattern) btnAddTextboxElement.GetCurrentPattern (InvokePattern.Pattern);
			InvokePattern removeAction = (InvokePattern) btnRemoveTextboxElement.GetCurrentPattern (InvokePattern.Pattern);
			addAction.Invoke ();
			Thread.Sleep (1000);
			Assert.AreEqual (1, elementEventSenders.Count, "Check event count");
			Assert.AreEqual (panel1Element, elementEventSenders [0], "Check ChildrenInvalidated event sender");
			Assert.AreEqual (StructureChangeType.ChildrenInvalidated,
			                 elementEventChangeTypes [0], "Check ChildrenInvalidated event type");
			Assert.AreEqual (1, childrenEventSenders.Count, "Check event count");
			Assert.AreEqual (StructureChangeType.ChildAdded,
			                 childrenEventChangeTypes [0], "Check ChildAdded event type");
			removeAction.Invoke ();
			Thread.Sleep (1000);
			Assert.AreEqual (3, elementEventSenders.Count, "Check event count");
			Assert.AreEqual (panel1Element, elementEventSenders [1], "Check event sender");
			Assert.AreEqual (panel1Element, elementEventSenders [2], "Check event sender");
			Assert.IsTrue ((elementEventChangeTypes [1] == StructureChangeType.ChildRemoved
			               && elementEventChangeTypes [2] == StructureChangeType.ChildrenInvalidated) ||
			               (elementEventChangeTypes [1] == StructureChangeType.ChildrenInvalidated
			               && elementEventChangeTypes [2] == StructureChangeType.ChildRemoved),
			               "Check event type");
			addAction.Invoke ();
			Thread.Sleep (1000);
			Assert.AreEqual (4, elementEventSenders.Count, "Check event count");
			Assert.AreEqual (panel1Element, elementEventSenders [3], "Check ChildrenInvalidated event sender");
			Assert.AreEqual (StructureChangeType.ChildrenInvalidated,
			                 elementEventChangeTypes [3], "Check ChildrenInvalidated event type");
			Assert.AreEqual (2, childrenEventSenders.Count, "Check event count");
			Assert.AreEqual (StructureChangeType.ChildAdded,
			                 childrenEventChangeTypes [1], "Check ChildAdded event type");

			At.RemoveStructureChangedEventHandler (panel1Element, elementHandler);
			At.RemoveStructureChangedEventHandler (panel1Element, childrenHandler);
			addAction.Invoke ();
			Thread.Sleep (1000);
			Assert.AreEqual (4, elementEventSenders.Count, "Element event count didn't change");
			Assert.AreEqual (2, childrenEventSenders.Count, "Children event count didn't change");
		}
	}
}

