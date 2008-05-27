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
using System.Windows;
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
			CheckBoxProvider provider = new CheckBoxProvider (checkbox);
			
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
			CheckBoxProvider provider =
				new CheckBoxProvider (checkbox);
			
			object toggleProvider = provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (toggleProvider);
			Assert.IsTrue (toggleProvider is IToggleProvider, "IToggleProvier");
		}
		
		[Test]
		public void ToggleStatePropertyChangedEventTest ()
		{
			CheckBox checkbox = new CheckBox ();
			CheckBoxProvider provider =
				new CheckBoxProvider (checkbox);
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
			CheckBoxProvider provider =
				new CheckBoxProvider (checkbox);
			IToggleProvider toggleProvider = (IToggleProvider)
				provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			
			// Test two-state toggling
			Assert.AreEqual (CheckState.Unchecked, checkbox.CheckState, "Start two-state Unchecked");
			toggleProvider.Toggle ();
			Assert.AreEqual (CheckState.Checked, checkbox.CheckState, "First two-state toggle: Checked");
			toggleProvider.Toggle ();
			Assert.AreEqual (CheckState.Unchecked, checkbox.CheckState, "Second two-state toggle: Unchecked");
			toggleProvider.Toggle ();
			Assert.AreEqual (CheckState.Checked, checkbox.CheckState, "Third two-state toggle: Checked");
			
			checkbox.ThreeState = true;
			
			// Test three-state toggling
			Assert.AreEqual (CheckState.Checked, checkbox.CheckState, "Start three-state Checked");
			toggleProvider.Toggle ();
			Assert.AreEqual (CheckState.Unchecked, checkbox.CheckState, "First three-state toggle: Unchecked");
			toggleProvider.Toggle ();
			Assert.AreEqual (CheckState.Indeterminate, checkbox.CheckState, "Second three-state toggle: Intermediate");
			toggleProvider.Toggle ();
			Assert.AreEqual (CheckState.Checked, checkbox.CheckState, "Third three-state toggle: Checked");
		}
		
#endregion
		
#region Private Methods
			
		private void TestToggleEvent (CheckBox checkbox,
		                              IToggleProvider provider,
		                              CheckState newState,
		                              ToggleState expectedState)
		{
			bridge.ResetEventLists ();
			checkbox.CheckState = newState;
			
			// Test IToggleProvider.ToggleState
			Assert.AreEqual (expectedState, provider.ToggleState, "ToggleState");
			
			// Test event was fired as expected
			Assert.AreEqual (1,
			                 bridge.AutomationPropertyChangedEvents.Count,
			                 "event count");
			
			object element =
				bridge.AutomationPropertyChangedEvents [0].element;
			Assert.AreEqual (provider,
			                 element,
			                 "event element");
			
			AutomationPropertyChangedEventArgs eventArgs =
				bridge.AutomationPropertyChangedEvents [0].e;
			Assert.AreEqual (TogglePatternIdentifiers.ToggleStateProperty,
			                 eventArgs.Property,
			                 "event args property");
			Assert.AreEqual (null, // Mimics MS implementation
			                 eventArgs.OldValue,
			                 "old value");
			Assert.AreEqual (expectedState,
			                 eventArgs.NewValue,
			                 "new value");
		}
		
#endregion
		
#region BaseProviderTest Overrides
		
		protected override IRawElementProviderSimple GetSimpleProvider (Control control)
		{
			return new CheckBoxProvider ((CheckBox)control);
		}
		
		protected override Control GetControlInstance ()
		{
			return new CheckBox ();
		}
		
#endregion
	}
}
