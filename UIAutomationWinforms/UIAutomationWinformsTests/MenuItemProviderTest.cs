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
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class MenuItemProviderTest : BaseProviderTest
	{
		#region Tests
		
		[Test]
		public void BasicPropertiesTest ()
		{
			MenuItem menuItem = new MenuItem ("testing");
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (menuItem);
			
			TestProperty (provider,
			              AEIds.ControlTypeProperty,
			              ControlType.MenuItem.Id);
			
			TestProperty (provider,
			              AEIds.LocalizedControlTypeProperty,
			              "menu item");

			TestProperty (provider,
			              AEIds.LabeledByProperty,
			              null);

			TestProperty (provider,
			              AEIds.NameProperty,
			              menuItem.Text);

			TestProperty (provider,
			              AEIds.IsKeyboardFocusableProperty,
			              true);
		}
		
		[Test]
		public void ProviderPatternTest ()
		{
			MenuItem menuItem = new MenuItem ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (menuItem);

			// Test without children
			menuItem.Checked = false;
			menuItem.RadioCheck = false;
			VerifyMenuItemPatterns (provider, menuItem);
			
			menuItem.Checked = true;
			menuItem.RadioCheck = false;
			VerifyMenuItemPatterns (provider, menuItem);
			
			menuItem.Checked = false;
			menuItem.RadioCheck = true;
			VerifyMenuItemPatterns (provider, menuItem);
			
			menuItem.Checked = true;
			menuItem.RadioCheck = true;
			VerifyMenuItemPatterns (provider, menuItem);

			// TODO: Test with C=F, RC=F, sibling item with RC=T

			// Test with children
			menuItem.MenuItems.Add (new MenuItem ("child item"));

			// Checked and RadioCheck are still true
			VerifyMenuItemPatterns (provider, menuItem);
			
			menuItem.Checked = true;
			menuItem.RadioCheck = false;
			VerifyMenuItemPatterns (provider, menuItem);
			
			menuItem.Checked = false;
			menuItem.RadioCheck = false;
			VerifyMenuItemPatterns (provider, menuItem);
		}
		
		[Test]
		public void InvokeTest ()
		{
			MenuItem menuItem = new MenuItem ();
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
		public new void IsEnabledPropertyTest ()
		{
			MenuItem menuItem = new MenuItem ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (menuItem);
			
			bridge.ResetEventLists ();
			
			object initialVal =
				provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			Assert.IsNotNull (initialVal, "Property returns null");
			Assert.IsTrue ((bool)initialVal, "Should initialize to true");
			
			menuItem.Enabled = false;

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
			
			menuItem.Enabled = true;
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
		}
		
		[Test]
		public void InvokedEventTest ()
		{
			MenuItem menuItem = new MenuItem ();
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
		[Ignore ("Need to patch MWF to be able to know ExpandCollapseState after Popup event")]
		public void ExpandCollapseStateChangedEventTest ()
		{
			MainMenu mainMenu = new MainMenu ();
			MenuItem item1 = new MenuItem ("item1");
			MenuItem item2 = new MenuItem ("item2");
			MenuItem item1sub1 = new MenuItem ("item1 sub1");
			MenuItem item1sub2 = new MenuItem ("item1 sub2");
			MenuItem item1sub1sub1 = new MenuItem ("item1 sub1 sub1");

			item1sub1.MenuItems.Add (item1sub1sub1);
			item1.MenuItems.Add (item1sub1);
			item1.MenuItems.Add (item1sub2);
			mainMenu.MenuItems.Add (item1);
			mainMenu.MenuItems.Add (item2);

			Form.Menu = mainMenu;

			IRawElementProviderSimple item1Provider =
				ProviderFactory.GetProvider (item1);

			bridge.ResetEventLists ();
			System.Threading.Thread.Sleep (1000);
			//Expand
			MenuItemHelper.SimulateClick (item1);
			
			var propertyEventTuple = bridge.GetAutomationPropertyEventFrom (item1Provider,
			                                                                ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id);
			Assert.IsNotNull (propertyEventTuple,
			                  "ExpandCollapseState property change event should be raised");
			Assert.AreEqual (ExpandCollapseState.Expanded,
			                 propertyEventTuple.e.NewValue,
			                 "New ExpandCollapseState value should be Expanded");

			bridge.ResetEventLists ();

			// Collapse
			MenuItemHelper.SimulateClick (item1);
			
			propertyEventTuple = bridge.GetAutomationPropertyEventFrom (item1Provider,
			                                                                ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id);
			Assert.IsNotNull (propertyEventTuple,
			                  "ExpandCollapseState property change event should be raised");
			Assert.AreEqual (ExpandCollapseState.Collapsed,
			                 propertyEventTuple.e.NewValue,
			                 "New ExpandCollapseState value should be Expanded");
		}

		[Test]
		public void ExpandCollapseTest ()
		{
			MainMenu mainMenu = new MainMenu ();
			MenuItem item1 = new MenuItem ("item1");
			SetupItemExpandCollapseEvents (item1);
			MenuItem item2 = new MenuItem ("item2");
			MenuItem item1sub1 = new MenuItem ("item1 sub1");
			MenuItem item1sub2 = new MenuItem ("item1 sub2");
			MenuItem item1sub1sub1 = new MenuItem ("item1 sub1 sub1");
			SetupItemExpandCollapseEvents (item1sub1);

			bool mainMenuCollapseEventRaised = false;
			mainMenu.Collapse += delegate(object sender, EventArgs e) {
				mainMenuCollapseEventRaised = true;
			};

			item1sub1.MenuItems.Add (item1sub1sub1);
			item1.MenuItems.Add (item1sub1);
			item1.MenuItems.Add (item1sub2);
			mainMenu.MenuItems.Add (item1);
			mainMenu.MenuItems.Add (item2);

			Form.Menu = mainMenu;

			IRawElementProviderSimple item1Provider =
				ProviderFactory.GetProvider (item1);
			IExpandCollapseProvider item1ExpandCollapse = (IExpandCollapseProvider)
				item1Provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);

			item1ExpandCollapse.Expand ();

			Assert.IsTrue (poppedupItems [item1]);
			Assert.IsTrue (selectedItems [item1]);
			Assert.IsFalse (clickedItems [item1]);
			Assert.IsFalse (mainMenuCollapseEventRaised);

			ClearExpandCollapseEvents ();
			mainMenuCollapseEventRaised = false;

			IRawElementProviderSimple item1sub1Provider =
				ProviderFactory.GetProvider (item1sub1);
			IExpandCollapseProvider item1sub1ExpandCollapse = (IExpandCollapseProvider)
				item1sub1Provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);

			item1sub1ExpandCollapse.Expand ();

			Assert.IsTrue (poppedupItems [item1sub1]);
			Assert.IsTrue (selectedItems [item1sub1]);
			Assert.IsFalse (clickedItems [item1sub1]);
			Assert.IsFalse (mainMenuCollapseEventRaised);

			ClearExpandCollapseEvents ();
			mainMenuCollapseEventRaised = false;

			item1sub1ExpandCollapse.Collapse ();

			Assert.IsFalse (poppedupItems [item1sub1]);
			Assert.IsFalse (selectedItems [item1sub1]);
			Assert.IsFalse (clickedItems [item1sub1]);
			Assert.IsTrue (mainMenuCollapseEventRaised);

			ClearExpandCollapseEvents ();
			mainMenuCollapseEventRaised = false;

			item1ExpandCollapse.Collapse ();

			Assert.IsFalse (poppedupItems [item1]);
			Assert.IsFalse (selectedItems [item1]);
			Assert.IsFalse (clickedItems [item1]);
			Assert.IsTrue (mainMenuCollapseEventRaised);

			ClearExpandCollapseEvents ();
			mainMenuCollapseEventRaised = false;
		}

		void SetupItemExpandCollapseEvents (MenuItem item)
		{
			clickedItems [item] = false;
			poppedupItems [item] = false;
			selectedItems [item] = false;
			
			item.Click += HandleClick;
			item.Popup += HandlePopup;
			item.Select += HandleSelect;
		}

		Dictionary<MenuItem, bool> clickedItems =
			new Dictionary<MenuItem, bool> ();
		Dictionary<MenuItem, bool> poppedupItems =
			new Dictionary<MenuItem, bool> ();
		Dictionary<MenuItem, bool> selectedItems =
			new Dictionary<MenuItem, bool> ();

		void HandleSelect (object sender, EventArgs e)
		{
			selectedItems [(MenuItem) sender] = true;
		}

		void HandlePopup (object sender, EventArgs e)
		{
			poppedupItems [(MenuItem) sender] = true;
		}

		void HandleClick (object sender, EventArgs e)
		{
			clickedItems [(MenuItem) sender] = true;
		}

		void ClearExpandCollapseEvents ()
		{
			foreach (MenuItem item in new List<MenuItem> (clickedItems.Keys))
				clickedItems [item] = false;
			foreach (MenuItem item in new List<MenuItem> (poppedupItems.Keys))
				poppedupItems [item] = false;
			foreach (MenuItem item in new List<MenuItem> (selectedItems.Keys))
				selectedItems [item] = false;
		}

		#endregion

		private void VerifyMenuItemPatterns (IRawElementProviderSimple provider,
		                                     MenuItem menuItem)
		{
			IRawElementProviderSimple parentProvider = ProviderFactory.GetProvider (menuItem.Parent);
			IExpandCollapseProvider parentExpandCollapse = null;
			if (parentProvider != null)
				parentExpandCollapse = (IExpandCollapseProvider)
					parentProvider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);
			
			bool expectInvoke = (menuItem.MenuItems.Count == 0);
			bool expectExpandCollapse = !expectInvoke && (parentExpandCollapse == null ||
			                                              parentExpandCollapse.ExpandCollapseState == ExpandCollapseState.Expanded);
			bool expectSelectionItem = expectInvoke && menuItem.RadioCheck;
			bool expectToggle = expectInvoke && !expectSelectionItem && menuItem.Checked;

			object invokeProvider =
				provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			if (expectInvoke) {
				Assert.IsNotNull (invokeProvider,
				                  "Should support Invoke pattern.");
				Assert.IsTrue (invokeProvider is IInvokeProvider,
				               "Should support Invoke pattern.");
			} else
				Assert.IsNull (invokeProvider,
				               "Should not support Invoke Pattern.");

			object selectionItemProvider =
				provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			if (expectSelectionItem) {
				Assert.IsNotNull (selectionItemProvider,
				                  "Should support SelectionItem pattern.");
				Assert.IsTrue (selectionItemProvider is ISelectionItemProvider,
				               "Should support SelectionItem pattern.");
			} else
				Assert.IsNull (selectionItemProvider,
				               "Should not support SelectionItem Pattern.");

			object toggleProvider =
				provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			if (expectToggle) {
				Assert.IsNotNull (toggleProvider,
				                  "Should support Toggle pattern.");
				Assert.IsTrue (toggleProvider is IToggleProvider,
				               "Should support Toggle pattern.");
			} else
				Assert.IsNull (toggleProvider,
				               "Should not support Toggle Pattern.");

			object expandCollapseProvider =
				provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);
			if (expectExpandCollapse) {
				Assert.IsNotNull (expandCollapseProvider,
				                  "Should support ExpandCollapse pattern.");
				Assert.IsTrue (expandCollapseProvider is IExpandCollapseProvider,
				               "Should support ExpandCollapse pattern.");
			} else
				Assert.IsNull (expandCollapseProvider,
				               "Should not support ExpandCollapse Pattern.");
		}

		protected override Control GetControlInstance ()
		{
			return null;
		}

		protected override IRawElementProviderSimple GetProvider ()
		{
			MainMenu menu = new MainMenu ();
			MenuItem menuItem = menu.MenuItems.Add ("test");
			Form.Menu = menu;
			return ProviderFactory.GetProvider (menuItem);
		}


	}
}
