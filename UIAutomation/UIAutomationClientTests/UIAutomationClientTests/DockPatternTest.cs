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
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using SWA = System.Windows.Automation;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;
using NUnit.Framework;
using MonoTests.System.Windows.Automation;
using At = System.Windows.Automation.Automation;

namespace MonoTests.System.Windows.Automation
{

	[TestFixture]
	public class DockPatternTest : BaseTest
	{
		#region Test Methods

		[Test]
		public void DockPositionTest ()
		{
			DockPattern dockPattern = (DockPattern) splitter1Element.GetCurrentPattern (DockPattern.Pattern);
			VerifyDockPosition (dockPattern, DockPosition.Left);
			dockPattern.SetDockPosition (DockPosition.Top);
			Thread.Sleep (200);
			VerifyDockPosition (dockPattern, DockPosition.Top);
			dockPattern.SetDockPosition (DockPosition.Right);
			Thread.Sleep (200);
			VerifyDockPosition (dockPattern, DockPosition.Right);
			dockPattern.SetDockPosition (DockPosition.Bottom);
			Thread.Sleep (200);
			VerifyDockPosition (dockPattern, DockPosition.Bottom);
			dockPattern.SetDockPosition (DockPosition.Left);
			Thread.Sleep (200);
			VerifyDockPosition (dockPattern, DockPosition.Left);
			RunCommand ("set splitter dock top");
			VerifyDockPosition (dockPattern, DockPosition.Top);
			RunCommand ("set splitter dock right");
			VerifyDockPosition (dockPattern, DockPosition.Right);
			RunCommand ("set splitter dock bottom");
			VerifyDockPosition (dockPattern, DockPosition.Bottom);
			RunCommand ("set splitter dock left");
			VerifyDockPosition (dockPattern, DockPosition.Left);
		}
#endregion

		private void VerifyDockPosition (DockPattern pattern, DockPosition dockPosition)
		{
			Assert.AreEqual (dockPosition,
				pattern.Current.DockPosition,
				"DockPosition " + dockPosition);
			Assert.AreEqual (dockPosition,
				splitter1Element.GetCurrentPropertyValue (DockPattern.DockPositionProperty),
				"DockPositionProperty " + dockPosition);
		}

		[Test]
		public void EventTest ()
		{
			var automationEventsArray = new [] {
				new {Sender = (object) null, Args = (AutomationPropertyChangedEventArgs) null}};
			var automationEvents = automationEventsArray.ToList ();
			automationEvents.Clear ();

			var dock = (DockPattern)
				splitter1Element.GetCurrentPattern (DockPattern.Pattern);

			dock.SetDockPosition (DockPosition.Top);

			SWA.Automation.AddAutomationPropertyChangedEventHandler (splitter1Element,
				TreeScope.Element,
				(o, e) => automationEvents.Add (new { Sender = o, Args = e }),
				DockPattern.DockPositionProperty);

			dock.SetDockPosition (DockPosition.Right);
			Thread.Sleep (200);
			Assert.AreEqual (1, automationEvents.Count, "event count");
			Assert.AreEqual (DockPattern.DockPositionProperty,
				automationEvents [0].Args.Property, "event property");
			Assert.AreEqual (splitter1Element,
				automationEvents [0].Sender, "event sender");
			Assert.AreEqual (3,
				automationEvents [0].Args.NewValue, "event new val");
		}
	}
}
