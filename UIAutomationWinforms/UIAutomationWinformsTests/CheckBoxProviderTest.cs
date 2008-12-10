//// Permission is hereby granted, free of charge, to any person obtaining 
//// a copy of this software and associated documentation files (the 
//// "Software"), to deal in the Software without restriction, including 
//// without limitation the rights to use, copy, modify, merge, publish, 
//// distribute, sublicense, and/or sell copies of the Software, and to 
//// permit persons to whom the Software is furnished to do so, subject to 
//// the following conditions: 
////  
//// The above copyright notice and this permission notice shall be 
//// included in all copies or substantial portions of the Software. 
////  
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
//// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
//// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
//// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
//// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
//// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//// 
//// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
//// 
//// Authors: 
////      Sandy Armstrong <sanfordarmstrong@gmail.com>
//// 
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
	public class CheckBoxProviderTest : BaseProviderTest
	{
#region Test Methods
		
		[Test]
		public void BasicPropertiesTest ()
		{
			CheckBox checkbox = new CheckBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (checkbox);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.CheckBox.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "check box");
			
			// TODO: Test properties implemented by SimpleControlProvider
			//       (should those be in BaseProviderTest perhaps?)
		}
		
		[Test]
		public void ProviderPatternTest ()
		{
			CheckBox checkbox = new CheckBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (checkbox);
			
			object toggleProvider = provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (toggleProvider);
			Assert.IsTrue (toggleProvider is IToggleProvider, "IToggleProvider");
		}
		
		[Test]
		public override void LabeledByAndNamePropertyTest ()
		{
			TestLabeledByAndName (false, false, true, true, false);
		}

		[Test]
		public void NamePropertyTest ()
		{
			CheckBox checkbox = new CheckBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (checkbox);
			checkbox.Text = "first";
			
			bridge.ResetEventLists ();
			
			string oldState = checkbox.Text;;
			string newState = "second";
			
			checkbox.Text = newState;
			
			// Test NameProperty
			Assert.AreEqual (newState,
			                 provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id), 
			                 "NameProperty");
			
			// Test event was fired as expected
			Assert.AreEqual (1,
			                 bridge.AutomationPropertyChangedEvents.Count,
			                 "event count");
			
			AutomationPropertyChangedEventArgs eventArgs =
				bridge.AutomationPropertyChangedEvents [0].e;
			Assert.AreEqual (AutomationElementIdentifiers.NameProperty,
			                 eventArgs.Property,
			                 "event args property");
			Assert.AreEqual (oldState,
			                 eventArgs.OldValue,
			                 "old value");
			Assert.AreEqual (newState,
			                 eventArgs.NewValue,
			                 "new value");
		}
		
		[Test]
		public void ToggleStatePropertyChangedEventTest ()
		{
			CheckBox checkbox = new CheckBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (checkbox);
			IToggleProvider toggleProvider = (IToggleProvider)
				provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			checkbox.Checked = false;
			
			TestToggleEvent (checkbox, 
			                 toggleProvider,
			                 CheckState.Checked,
			                 ToggleState.On);
			TestToggleEvent (checkbox,
			                 toggleProvider,
			                 CheckState.Unchecked,
			                 ToggleState.Off);
			TestToggleEvent (checkbox,
			                 toggleProvider,
			                 CheckState.Indeterminate,
			                 ToggleState.Indeterminate);
		}
		
		[Test]
		public void ToggleTest ()
		{
			CheckBox checkbox = new CheckBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (checkbox);
			IToggleProvider toggleProvider = (IToggleProvider)
				provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			
			// Test two-state toggling
			Assert.AreEqual (CheckState.Unchecked, checkbox.CheckState, "Start two-state Unchecked");
			TestToggleEventWithToggle (toggleProvider, ToggleState.On);
			Assert.AreEqual (CheckState.Checked, checkbox.CheckState, "First two-state toggle: Checked");
			TestToggleEventWithToggle (toggleProvider, ToggleState.Off);
			Assert.AreEqual (CheckState.Unchecked, checkbox.CheckState, "Second two-state toggle: Unchecked");
			TestToggleEventWithToggle (toggleProvider, ToggleState.On);
			Assert.AreEqual (CheckState.Checked, checkbox.CheckState, "Third two-state toggle: Checked");
			
			checkbox.ThreeState = true;
			
			// Test three-state toggling
			Assert.AreEqual (CheckState.Checked, checkbox.CheckState, "Start three-state Checked");
			TestToggleEventWithToggle (toggleProvider, ToggleState.Off);
			Assert.AreEqual (CheckState.Unchecked, checkbox.CheckState, "First three-state toggle: Unchecked");
			TestToggleEventWithToggle (toggleProvider, ToggleState.Indeterminate);
			Assert.AreEqual (CheckState.Indeterminate, checkbox.CheckState, "Second three-state toggle: Intermediate");
			TestToggleEventWithToggle (toggleProvider, ToggleState.On);
			Assert.AreEqual (CheckState.Checked, checkbox.CheckState, "Third three-state toggle: Checked");

			checkbox.Enabled = false;

			// Test that an exception is thrown when not enabled
			try {
				toggleProvider.Toggle ();
				Assert.Fail ("Should throw ElementNotEnabledException");
			} catch (ElementNotEnabledException) { }
		}
		
#endregion
		
#region Private Methods
			
		private void TestToggleEvent (CheckBox checkbox,
		                              IToggleProvider provider,
		                              CheckState newState,
		                              ToggleState expectedState)
		{
			bridge.ResetEventLists ();
			
			object oldState = provider.ToggleState;
			
			checkbox.CheckState = newState;
			
			// Test IToggleProvider.ToggleState
			Assert.AreEqual (expectedState, provider.ToggleState, "ToggleState");
			
			// Test event was fired as expected
			Assert.AreEqual (1,
			                 bridge.AutomationPropertyChangedEvents.Count,
			                 "event count");
			
			AutomationPropertyChangedEventArgs eventArgs =
				bridge.AutomationPropertyChangedEvents [0].e;
			Assert.AreEqual (TogglePatternIdentifiers.ToggleStateProperty,
			                 eventArgs.Property,
			                 "event args property");
			Assert.AreEqual (oldState,
			                 eventArgs.OldValue,
			                 "old value");
			Assert.AreEqual (expectedState,
			                 eventArgs.NewValue,
			                 "new value");
		}
			
		private void TestToggleEventWithToggle (IToggleProvider provider,
		                              ToggleState expectedState)
		{
			bridge.ResetEventLists ();
			
			object oldState = provider.ToggleState;
			
			provider.Toggle ();
			
			// Test IToggleProvider.ToggleState
			Assert.AreEqual (expectedState, provider.ToggleState, "ToggleState");
			
			// Test event was fired as expected
			Assert.AreEqual (1,
			                 bridge.AutomationPropertyChangedEvents.Count,
			                 "event count");
			
			AutomationPropertyChangedEventArgs eventArgs =
				bridge.AutomationPropertyChangedEvents [0].e;
			Assert.AreEqual (TogglePatternIdentifiers.ToggleStateProperty,
			                 eventArgs.Property,
			                 "event args property");
			Assert.AreEqual (oldState,
			                 eventArgs.OldValue,
			                 "old value");
			Assert.AreEqual (expectedState,
			                 eventArgs.NewValue,
			                 "new value");
		}
		
#endregion
		
#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return new CheckBox ();
		}
		
#endregion
	}
}
