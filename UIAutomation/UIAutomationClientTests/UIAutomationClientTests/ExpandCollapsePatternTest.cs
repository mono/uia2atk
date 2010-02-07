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
//  Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using System.Linq;
using At = System.Windows.Automation.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;
using MonoTests.System.Windows.Automation;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class ExpandCollapsePatternTest : BaseTest
	{
		#region Test Methods
		[Test]
		public void ExpandCollapseTest ()
		{
			AutomationElement parentElement, childElement;
			parentElement = treeView1Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.TreeItem));
			ExpandCollapsePattern pattern = (ExpandCollapsePattern) parentElement.GetCurrentPattern (ExpandCollapsePatternIdentifiers.Pattern);
			ExpandCollapsePattern.ExpandCollapsePatternInformation current = pattern.Current;
			Assert.AreEqual (ExpandCollapseState.Collapsed, current.ExpandCollapseState, "ExpandCollapseState before Expand");
			pattern.Expand ();
			if (Atspi)
				Thread.Sleep (500);
			Assert.AreEqual (ExpandCollapseState.Expanded, current.ExpandCollapseState, "ExpandCollapseState after Expand");
			childElement = parentElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.TreeItem));
			Assert.IsNotNull (childElement, "Should have a TreeItem after expand");

			pattern.Collapse ();
			if (Atspi)
				Thread.Sleep (500);
			Assert.AreEqual (ExpandCollapseState.Collapsed, current.ExpandCollapseState, "ExpandCollapseState after Collapse");
		}

		[Test]
		[ExpectedException("System.Windows.Automation.ElementNotEnabledException")]
		public void Z_NotEnabledTest ()
		{
			DisableControls ();

			AutomationElement parentElement;
			parentElement = treeView1Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.TreeItem));
			ExpandCollapsePattern pattern = (ExpandCollapsePattern) parentElement.GetCurrentPattern (ExpandCollapsePatternIdentifiers.Pattern);
			ExpandCollapsePattern.ExpandCollapsePatternInformation current = pattern.Current;
			Assert.AreEqual (ExpandCollapseState.Collapsed, current.ExpandCollapseState, "ExpandCollapseState before Collapse");
			pattern.Expand ();
		}

		[Test]
		public void Z_EventTest ()
		{
			AutomationElement parentElement;
			parentElement = treeView1Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.TreeItem));
			ExpandCollapsePattern pattern = (ExpandCollapsePattern) parentElement.GetCurrentPattern (ExpandCollapsePatternIdentifiers.Pattern);
			pattern.Collapse ();
			var propertyEventsArray = new [] {
				new {Sender = (object) null, Args = (AutomationPropertyChangedEventArgs) null}};
			var propertyEvents = propertyEventsArray.ToList ();
			propertyEvents.Clear ();

			AutomationPropertyChangedEventHandler propertyHandler =
				(o, e) => propertyEvents.Add (new { Sender = o, Args = e });
			At.AddAutomationPropertyChangedEventHandler (parentElement,
			                                             TreeScope.Element, propertyHandler,
			                                             ExpandCollapsePattern.ExpandCollapseStateProperty);

			pattern.Expand ();
			Thread.Sleep (100);
			Assert.AreEqual (1, propertyEvents.Count, "event count");
			Assert.AreEqual (parentElement, propertyEvents [0].Sender, "event sender");
			Assert.AreEqual (ExpandCollapsePattern.ExpandCollapseStateProperty, propertyEvents [0].Args.Property, "property");
			if (Atspi) {
				Assert.AreEqual (ExpandCollapseState.Collapsed, propertyEvents [0].Args.OldValue, "old value");
				Assert.AreEqual (ExpandCollapseState.Expanded, propertyEvents [0].Args.NewValue, "new value");
			} else {
				Assert.AreEqual (null, propertyEvents [0].Args.OldValue, "old value");
				Assert.AreEqual (1, propertyEvents [0].Args.NewValue, "new value");
			}

			propertyEvents.Clear ();
			pattern.Collapse ();
			Thread.Sleep (100);
			Assert.AreEqual (1, propertyEvents.Count, "event count");
			Assert.AreEqual (parentElement, propertyEvents [0].Sender, "event sender");
			Assert.AreEqual (ExpandCollapsePattern.ExpandCollapseStateProperty, propertyEvents [0].Args.Property, "property");
			if (Atspi) {
				Assert.AreEqual (ExpandCollapseState.Expanded, propertyEvents [0].Args.OldValue, "old value");
				Assert.AreEqual (ExpandCollapseState.Collapsed, propertyEvents [0].Args.NewValue, "new value");
			} else {
				Assert.AreEqual (null, propertyEvents [0].Args.OldValue, "old value");
				Assert.AreEqual (0, propertyEvents [0].Args.NewValue, "new value");
			}

			propertyEvents.Clear ();
			pattern.Collapse ();
			Thread.Sleep (100);
			Assert.AreEqual (0, propertyEvents.Count, "event count when collapsing but already collapsed");
		}
		#endregion
	}
}
