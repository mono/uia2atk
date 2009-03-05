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
//	Andrés G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.ComponentModel;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{

	[TestFixture]
	public class ToolBarButtonProviderTest : BaseProviderTest
	{
		[SetUp]
		public override void SetUp ()
		{
			base.SetUp ();

			toolBar = new ToolBar ();
			toolBarButton = new ToolBarButton ("Button");
			toolBar.Buttons.Add (toolBarButton);

			//we rather create 2, to expose a bug in the provider
			toolBar.Buttons.Add (new ToolBarButton ("another one"));
			
			Form.Controls.Add (toolBar);
			Form.Show ();
		}

		protected override Control GetControlInstance ()
		{
			return null;
		}

		static ToolBar toolBar = null;
		static ToolBarButton toolBarButton = null;
		
		protected override IRawElementProviderSimple GetProvider ()
		{
			return ProviderFactory.GetProvider (toolBarButton);
		}
		
		#region Test

		[Test]
		public void BasicPropertiesTest ()
		{
			// default is PushButton style
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (toolBarButton);
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Button.Id);
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "button");

			// DropDownButton style
			ToolBarButton dropDownButton = new ToolBarButton ();
			dropDownButton.Style = ToolBarButtonStyle.DropDownButton;
			toolBar.Buttons.Add (dropDownButton);
			IRawElementProviderSimple dropDownProvider =
				ProviderFactory.GetProvider (dropDownButton);
			TestProperty (dropDownProvider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.SplitButton.Id);
			TestProperty (dropDownProvider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "split button");

			// ToggleButton style
			ToolBarButton toggleButton = new ToolBarButton ();
			toggleButton.Style = ToolBarButtonStyle.ToggleButton;
			toolBar.Buttons.Add (toggleButton);
			IRawElementProviderSimple toggleProvider =
				ProviderFactory.GetProvider (toggleButton);
			TestProperty (toggleProvider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Button.Id);
			TestProperty (toggleProvider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "button");

			// Separator style
			ToolBarButton separatorButton = new ToolBarButton ();
			separatorButton.Style = ToolBarButtonStyle.Separator;
			toolBar.Buttons.Add (dropDownButton);
			IRawElementProviderSimple separatorProvider =
				ProviderFactory.GetProvider (separatorButton);
			TestProperty (separatorProvider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Separator.Id);
			TestProperty (separatorProvider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "separator");
			
			string value = "ToolBarButton Name Property";
			toolBarButton.Text = value;
			TestProperty (provider,
			              AutomationElementIdentifiers.NameProperty,
			              value);
			TestProperty (provider,
			              AutomationElementIdentifiers.IsEnabledProperty,
			              toolBarButton.Enabled);

			toolBarButton.Enabled = false;
			TestProperty (provider,
			              AutomationElementIdentifiers.IsEnabledProperty,
			              false);
			toolBarButton.Enabled = true;
		}

		[Test]
		//tested with UIAVerify, the bridge depends on this behaviour
		public override void IsKeyboardFocusablePropertyTest ()
		{
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (toolBarButton);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              false);

			object hasKbFocus = provider.GetPropertyValue (AutomationElementIdentifiers.HasKeyboardFocusProperty.Id);
			Assert.IsNotNull (hasKbFocus);
			Assert.IsTrue (hasKbFocus is bool);
			Assert.IsFalse ((bool)hasKbFocus);
		}

		[Test]
		public void ProviderPatternTest ()
		{
			TestHelper.TestPatterns (GetProvider (), InvokePatternIdentifiers.Pattern);
		}

		[Test]
		public void Visualization ()
		{
			var parentProvider = ProviderFactory.GetProvider (toolBar);
			var provider = ProviderFactory.GetProvider (toolBarButton);
			
			Assert.AreEqual (parentProvider.GetPropertyValue (AutomationElementIdentifiers.IsOffscreenProperty.Id),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.IsOffscreenProperty.Id));
		}

		[Test]
		public void InvokeTest ()
		{
			IRawElementProviderSimple provider = GetProvider ();
			IInvokeProvider invokeProvider = (IInvokeProvider)
				provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			
			bool buttonClicked = false;
			toolBar.ButtonClick += delegate (object sender, ToolBarButtonClickEventArgs e) {
				buttonClicked = true;
			};
			
			invokeProvider.Invoke ();
			Assert.IsTrue (buttonClicked,
			               "Click should fire when button is enabled");
			
			buttonClicked = false;
			toolBarButton.Enabled = false;
			try {
				invokeProvider.Invoke ();
				Assert.Fail ("Expected ElementNotEnabledException");
			} catch (ElementNotEnabledException) {
				// Awesome, this is expected
			} catch (Exception e) {
				Assert.Fail ("Expected ElementNotEnabledException, " +
				             "but got exception with message: " +
				             e.Message);
			} finally {
				toolBarButton.Enabled = true;
			}
			Assert.IsFalse (buttonClicked,
			                "Click should not fire when button is disabled");
		}
		
		[Test]
		public void InvokedEventTest ()
		{
			IRawElementProviderSimple provider = GetProvider ();
			
			bridge.ResetEventLists ();

			//we cannot do this, there's no PerformClick() API in toolbarButton:
			//toolBarButton.PerformClick ();
			//so we use the provider:
			IInvokeProvider invokeProvider = 
				(IInvokeProvider)provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			invokeProvider.Invoke ();
			
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
		public void ToggleTest ()
		{
			ToolBarButton toggleButton = new ToolBarButton ();
			toggleButton.Style = ToolBarButtonStyle.ToggleButton;
			toolBar.Buttons.Add (toggleButton);
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (toggleButton);
			Assert.IsFalse (toggleButton.Pushed, "Button is not pushed by default.");
			
			IToggleProvider toggleProvider = (IToggleProvider) 
				provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			toggleProvider.Toggle ();
			Assert.IsTrue (toggleButton.Pushed, "Button is pushed.");
		}


		[Test]
		[Ignore ("Only works with MWF 2.6")]
		public override void IsEnabledPropertyTest ()
		{
			var provider = GetProvider ();
			
			bridge.ResetEventLists ();
			
			object initialVal =
				provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			Assert.IsNotNull (initialVal, "Property returns null");
			Assert.IsTrue ((bool)initialVal, "Should initialize to true");
			
			toolBarButton.Enabled = false;

			Assert.IsFalse ((bool)provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id),
			                "Toggle to false");

			AutomationPropertyChangedEventTuple tuple 
				= bridge.GetAutomationPropertyEventFrom (provider,
				                                         AutomationElementIdentifiers.IsEnabledProperty.Id);
			
			Assert.IsNotNull (tuple, "Tuple missing");
			Assert.AreEqual (initialVal,
			                 tuple.e.OldValue,
			                 string.Format ("1st. Old value should be true: '{0}'", tuple.e.OldValue));
			Assert.AreEqual (false,
			                 tuple.e.NewValue,
			                 string.Format ("1st. New value should be true: '{0}'", tuple.e.NewValue));
			
			bridge.ResetEventLists ();
			
			toolBarButton.Enabled = true;
			Assert.IsTrue ((bool)provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id),
			               "Toggle to true");
			
			tuple 
				= bridge.GetAutomationPropertyEventFrom (provider,
				                                         AutomationElementIdentifiers.IsEnabledProperty.Id);
			Assert.IsNotNull (tuple, "Tuple missing");
			Assert.AreEqual (false,
			                 tuple.e.OldValue,
			                 string.Format ("2nd. Old value should be false: '{0}'", tuple.e.OldValue));
			Assert.AreEqual (true,
			                 tuple.e.NewValue,
			                 string.Format ("2nd. New value should be true: '{0}'", tuple.e.NewValue));

			//restore state
			toolBarButton.Enabled = true;
		}
		
		#endregion

		#region Navigation Test

		[Test]
		public void NavigationTest ()
		{
			IRawElementProviderSimple childProvider,parentProvider;
			var provider = ProviderFactory.GetProvider (toolBarButton);
			parentProvider = ((IRawElementProviderFragment)provider).Navigate (NavigateDirection.Parent);
			Assert.IsNotNull (parentProvider, "We must have a parent");

			Assert.AreEqual (parentProvider, ProviderFactory.GetProvider (toolBar));

			parentProvider = ProviderFactory.GetProvider (toolBar);
			childProvider = ((IRawElementProviderFragment)parentProvider).Navigate (NavigateDirection.FirstChild);

			Assert.IsNotNull (childProvider, "We must have a child");

			Assert.AreEqual (childProvider, provider);

			// Terminate Provider
			((FragmentControlProvider) parentProvider).Terminate ();
		}

		#endregion
	}
}
