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
// Copyright (c) 2008,2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 


using System;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class ToolStripLabelProviderTest : BaseProviderTest
	{
		[SetUp]
		public override void SetUp ()
		{
			base.SetUp ();

			strip = new ToolStrip ();
			item = new ToolStripLabel ();
			strip.Items.Add (item);
			Form.Controls.Add (strip);
			Form.Show ();
		}

		[TearDown]
		public override void TearDown ()
		{
			base.TearDown ();

			Form.Controls.Remove (strip);
			strip = null;
			item = null;
		}

		[Test]
		public void ProviderPatternTest ()
		{
			ToolStripLabel menuItem = new ToolStripLabel ();
			VerifyPatterns (menuItem, false);

			menuItem = new ToolStripLabel ();
			menuItem.IsLink = true;
			VerifyPatterns (menuItem, true);
		}

		private void VerifyPatterns (ToolStripLabel menuItem, bool expectInvokeSupport)
		{
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (menuItem);
			
			// Should never support Text
			object textProvider = provider.GetPatternProvider (TextPatternIdentifiers.Pattern.Id);
			Assert.IsNull (textProvider);

			// Should never support TableItem
			object tableItemProvider = provider.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id);
			Assert.IsNull (tableItemProvider);

			// Should never support RangeValue
			object rangeValueProvider = provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNull (rangeValueProvider);

			// Should never support Value
			object valueProvider = provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNull (valueProvider);

			// Support Invoke conditionally
			object invokeProvider = provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			if (expectInvokeSupport) {
				Assert.IsNotNull (invokeProvider);
				Assert.IsTrue (invokeProvider is IInvokeProvider, "IInvokeProvider");
			} else
				Assert.IsNull (invokeProvider);
		}
		
		[Test]
		public void InvokeTest ()
		{
			ToolStripLabel menuItem = item;
			menuItem.IsLink = true;
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
			ToolStripLabel menuItem = new ToolStripLabel ();
			menuItem.IsLink = true;
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

		[Test]
		public override void LabeledByAndNamePropertyTest()
		{
			ToolStripLabel menuItem = new ToolStripLabel ();
			menuItem.Text = "My menu item";
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (menuItem);

			Assert.AreEqual (menuItem.Text,
			                 provider.GetPropertyValue (AEIds.NameProperty.Id) as string,
			                 "Name");
			Assert.IsNull (provider.GetPropertyValue (AEIds.LabeledByProperty.Id),
			               "LabeledBy");
		}

		[Test]
		public void IsLinkChangeTest ()
		{
			ToolStripLabel menuItem = new ToolStripLabel ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (menuItem);
			
			menuItem.IsLink = true;
			
			object invokeProvider = provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (invokeProvider,
			                  "Invoke support when Islink changes to true");
			Assert.IsTrue (invokeProvider is IInvokeProvider,
			               "Returned Invoke provider should implement IInvokeProvider");

			menuItem.IsLink = false;
			
			invokeProvider = provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			Assert.IsNull (invokeProvider,
			               "No Invoke support when IsLink changes to false");
		}

		protected override Control GetControlInstance ()
		{
			return null; // TODO: Lots of work...
		}

		protected override IRawElementProviderSimple GetProvider ()
		{
			return ProviderFactory.GetProvider (item);
		}

		private ToolStrip strip;
		private ToolStripLabel item;
	}
}
