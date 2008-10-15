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
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class ToolStripMenuItemProviderTest : BaseProviderTest
	{
		[Test]
		public void ProviderPatternTest ()
		{
			ToolStripMenuItem menuItem = new ToolStripMenuItem ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (menuItem);

			// Should never support Toggle
			object toggleProvider = provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			Assert.IsNull (toggleProvider);

			// Should never support SelectionItem
			object selectionItemProvider = provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			Assert.IsNull (selectionItemProvider);

			// Should never support ExpandCollapse
			object expandCollapseProvider = provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);
			Assert.IsNull (expandCollapseProvider);
			
			object invokeProvider = provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (invokeProvider);
			Assert.IsTrue (invokeProvider is IInvokeProvider, "IInvokeProvider");
		}
		
		[Test]
		public void InvokeTest ()
		{
			ToolStripMenuItem menuItem = new ToolStripMenuItem ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (menuItem);
			IInvokeProvider invokeProvider = (IInvokeProvider)
				provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			
			bool itemClicked = false;
			menuItem.Click += delegate (object sender, EventArgs e) {
				itemClicked = true;
			};
			
			invokeProvider.Invoke ();			
			Assert.IsTrue (itemClicked,
			               "Click should fire when button is enabled");
			
			itemClicked = false;
			menuItem.Enabled = false;
			try {
				invokeProvider.Invoke ();
				Assert.Fail ("Expected ElementNotEnabledException");
			} catch (ElementNotEnabledException) {
				// Awesome, this is expected
			} catch (Exception e) {
				Assert.Fail ("Expected ElementNotEnabledException, " +
				             "but got exception with message: " +
				             e.Message);
			}
			Assert.IsFalse (itemClicked,
			                "Click should not fire when button is disabled");
		}
		
		[Test]
		public void InvokedEventTest ()
		{
			ToolStripMenuItem menuItem = new ToolStripMenuItem ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (menuItem);
			
			bridge.ResetEventLists ();
			
			menuItem.PerformClick ();
			
			Assert.AreEqual (1,
			                 bridge.AutomationEvents.Count,
			                 "event count");
			
			AutomationEventTuple eventInfo =
				bridge.AutomationEvents [0];
			Assert.AreEqual (InvokePatternIdentifiers.InvokedEvent,
			                 eventInfo.eventId,
			                 "event type");
			Assert.AreEqual (provider,
			                 eventInfo.provider,
			                 "event element");
			Assert.AreEqual (InvokePatternIdentifiers.InvokedEvent,
			                 eventInfo.e.EventId,
			                 "event args event type");
		}

		protected override Control GetControlInstance()
		{
			return null; // TODO: Lots of work...
		}

	}
}
