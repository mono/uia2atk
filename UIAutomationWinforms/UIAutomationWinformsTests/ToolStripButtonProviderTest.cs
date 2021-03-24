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
//      Brad Taylor <brad@getcoded.net>
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
	public class ToolStripButtonProviderTest : BaseProviderTest
	{
		[SetUp]
		public override void SetUp ()
		{
			base.SetUp ();

			strip = new ToolStrip ();
			button = new ToolStripButton ();
			strip.Items.Add (button);
			Form.Controls.Add (strip);
			Form.Show ();
		}

		[TearDown]
		public override void TearDown ()
		{
			base.TearDown ();

			Form.Controls.Remove (strip);
			strip = null;
			button = null;
		}

		[Test]
		public void BasicPropertiesTest ()
		{
			IRawElementProviderSimple provider
				= ProviderFactory.GetProvider (button);

			button.CheckOnClick = false;
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Button.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "button");

			button.CheckOnClick = true;
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.CheckBox.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "checkbox");
		}
		
		[Test]
		public void InvokeTest ()
		{
			IRawElementProviderSimple provider
				= ProviderFactory.GetProvider (button);

			IInvokeProvider invokeProvider = 
				provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id)
				as IInvokeProvider;
			Assert.IsNotNull (invokeProvider, "Does not implement IInvokeProvider");

			
			bool itemClicked = false;
			button.Click += delegate (object sender, EventArgs e) {
				itemClicked = true;
			};
			
			invokeProvider.Invoke ();			
			Assert.IsTrue (itemClicked,
			               "Click should fire when button is enabled");
			
			itemClicked = false;
			button.Enabled = false;
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

			button.Enabled = true;
		}
		
		[Test]
		public void InvokedEventTest ()
		{
			IRawElementProviderSimple provider
				= ProviderFactory.GetProvider (button);
			
			bridge.ResetEventLists ();
			
			button.PerformClick ();
			
			Assert.AreEqual (1,
			                 bridge.AutomationEvents.Count,
			                 "event count");
			
			AutomationEventTuple eventInfo =
				bridge.AutomationEvents [0];
			Assert.AreEqual (provider,
			                 eventInfo.provider,
			                 "event element");
			Assert.AreEqual (InvokePatternIdentifiers.InvokedEvent,
			                 eventInfo.e.EventId,
			                 "event args event type");
		}

		[Test]
		public void ToggleTest ()
		{
			IRawElementProviderSimple provider
				= ProviderFactory.GetProvider (button);

			IToggleProvider toggleProvider = 
				provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id)
				as IToggleProvider;

			// LAMESPEC: We're going against the spec here -- if
			// CheckOnClick is set, support Toggle provider.

			button.CheckOnClick = false;

			// Depends -> No
			Assert.IsNull (toggleProvider, "Implements IToggleProvider");


			button.CheckOnClick = true;

			toggleProvider = 
				provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id) as IToggleProvider;

			// Depends -> Yes
			Assert.IsNotNull (toggleProvider, "Should implement IToggleProvider");

			Assert.AreEqual (ToggleState.Off, toggleProvider.ToggleState, "ToggleState");

			bridge.ResetEventLists ();

			toggleProvider.Toggle ();

			Assert.AreEqual (ToggleState.On, toggleProvider.ToggleState, "ToggleState");

			Assert.AreEqual (1, bridge.AutomationPropertyChangedEvents.Count,
			                 "event count");
		}
		
		protected override Control GetControlInstance ()
		{
			return null;
		}
		
		protected override IRawElementProviderSimple GetProvider ()
		{
			return ProviderFactory.GetProvider (button);
		}

		private ToolStrip strip;
		private ToolStripButton button;
	}
}
