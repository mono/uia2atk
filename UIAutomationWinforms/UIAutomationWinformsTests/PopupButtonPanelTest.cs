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
//	Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Reflection;
using System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class PopupButtonPanelTest : BaseProviderTest
	{
        	[Test]
        	public void BasicPropertiesTest ()
        	{
            		Control panel = PopupButtonPanelHelper.CreatePopupButtonPanel ();
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (panel);

			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.ToolBar.Id);

			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "tool bar");
		}
		
		[Test]
		public override void LabeledByAndNamePropertyTest ()
		{
			TestLabeledByAndName (false, false);
		}
		
		protected override Control GetControlInstance ()
		{
			return PopupButtonPanelHelper.CreatePopupButtonPanel ();
		}
	}
	
	[TestFixture]
	public class PopupButtonTest : BaseProviderTest
	{
        	[Test]
        	public void BasicPropertiesTest ()
        	{
            		Control button = PopupButtonPanelHelper.CreatePopupButton ();
			IRawElementProviderFragment provider = (IRawElementProviderFragment)
				GetProviderFromControl (button);

			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.MenuItem.Id);

			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "menu item");
		}

		[Test]
		public void PatternsTest ()
		{
            		Control button = PopupButtonPanelHelper.CreatePopupButton ();
			IRawElementProviderFragment provider = (IRawElementProviderFragment)
				GetProviderFromControl (button);

			object invokeProvider =
				provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (invokeProvider, "Invoke pattern should be supported");
			Assert.IsTrue (invokeProvider is IInvokeProvider,
			               "Invoke pattern should be supported");
		}

		[Test]
		public void InvokeTest ()
		{
			Control button = PopupButtonPanelHelper.CreatePopupButton ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (button);
			IInvokeProvider invokeProvider = (IInvokeProvider)
				provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			
			bool buttonClicked = false;
			button.Click += delegate (object sender, EventArgs e) {
				buttonClicked = true;
			};
			
			invokeProvider.Invoke ();			
			Assert.IsTrue (buttonClicked,
			               "Click should fire when button is enabled");
		}

		[Test]
		public void InvokedEventTest ()
		{
			Control button = PopupButtonPanelHelper.CreatePopupButton ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (button);
			
			bridge.ResetEventLists ();
			
			PopupButtonPanelHelper.PerformButtonClick (button);
			
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
		public override void LabeledByAndNamePropertyTest ()
		{
			TestLabeledByAndName (false, false);
		}
		
		protected override Control GetControlInstance ()
		{
			return PopupButtonPanelHelper.CreatePopupButton ();
		}
	}

	internal class PopupButtonPanelHelper
	{
		private static Type pbpType;
		private static Type pbType;
		private static MethodInfo performClickMethod;
		
		static PopupButtonPanelHelper ()
		{
			Assembly swfAssembly = Assembly.GetAssembly (typeof (Control));
			pbpType = swfAssembly.GetType ("System.Windows.Forms.PopupButtonPanel");
			pbType = pbpType.GetNestedType ("PopupButton", BindingFlags.NonPublic);
			performClickMethod = pbType.GetMethod ("PerformClick",
			                                       BindingFlags.Instance |
			                                       BindingFlags.NonPublic);
		}

		public static Control CreatePopupButtonPanel ()
		{
			return (Control) Activator.CreateInstance (pbpType);
		}

		public static Control CreatePopupButton ()
		{
			return (Control) Activator.CreateInstance (pbType);
		}

		public static void PerformButtonClick (Control popupButton)
		{
			performClickMethod.Invoke (popupButton, new object [] {});
		}
	}
}
