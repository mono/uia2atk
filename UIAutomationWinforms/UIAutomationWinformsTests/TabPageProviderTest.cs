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
	public class TabPageProviderTest : BaseProviderTest
	{
		[SetUp]
		public override void SetUp ()
		{
			base.SetUp ();

			tabControl = new TabControl ();
			tabPage1 = new TabPage ();
			tabPage2 = new TabPage ();
			tabControl.Controls.Add (tabPage1);
			tabControl.Controls.Add (tabPage2);
			Form.Controls.Add (tabControl);
			Form.Show ();
		}

		[TearDown]
		public override void TearDown ()
		{
			base.TearDown ();

			Form.Controls.Remove (tabControl);
			tabControl = null;
			tabPage1 = tabPage2 = null;
		}

		[Test]
		public void BasicPropertiesTest ()
		{
			IRawElementProviderSimple provider
				= ProviderFactory.GetProvider (tabPage1);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.TabItem.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "tab item");
		}
		
		[Test]
		public void ISelectionItemProviderTest ()
		{
			IRawElementProviderSimple childProvider
				= ProviderFactory.GetProvider (tabPage1);

			ISelectionItemProvider sel_item_prov
				= (ISelectionItemProvider)childProvider.GetPatternProvider (
					SelectionItemPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (sel_item_prov, "Not returning SelectionItemPatternIdentifiers");

			// Test IsSelected property
			tabControl.SelectTab (0);
			Assert.IsTrue (sel_item_prov.IsSelected, "IsSelected should return true when tab is selected");

			tabControl.SelectTab (1);
			Assert.IsFalse (sel_item_prov.IsSelected, "IsSelected should return false when tab is not selected");

			// Test .Select method and eventing
			bridge.ResetEventLists ();

			sel_item_prov.Select ();
			Assert.IsTrue (sel_item_prov.IsSelected,
			               "IsSelected should return true when tab is selected through ISelectionItem interface");

			Assert.AreEqual (1, bridge.AutomationPropertyChangedEvents.Count,
				         "No events fired for selection changed");

			Assert.AreEqual (1, bridge.GetAutomationEventCount (SelectionItemPatternIdentifiers.ElementSelectedEvent),
			                 "IsSelected property change not fired");
		}

		[Test]
		public void ISelectionItemProvider_AddTest ()
		{
			IRawElementProviderSimple childProvider
				= ProviderFactory.GetProvider (tabPage1);

			ISelectionItemProvider sel_item_prov
				= (ISelectionItemProvider)childProvider.GetPatternProvider (
					SelectionItemPatternIdentifiers.Pattern.Id);

			tabControl.SelectTab (0);
			Assert.IsTrue (sel_item_prov.IsSelected, "Tab 0 did not get selected");

			// This should do nothing
			sel_item_prov.AddToSelection ();

			Assert.IsTrue (sel_item_prov.IsSelected, "Tab 0 was unselected");
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void ISelectionItemProvider_InvalidAddTest ()
		{
			IRawElementProviderSimple childProvider
				= ProviderFactory.GetProvider (tabPage1);

			ISelectionItemProvider sel_item_prov
				= (ISelectionItemProvider)childProvider.GetPatternProvider (
					SelectionItemPatternIdentifiers.Pattern.Id);

			tabControl.SelectTab (1);
			Assert.IsFalse (sel_item_prov.IsSelected, "Tab 0 is still selected!");

			sel_item_prov.AddToSelection ();
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void ISelectionItemProvider_InvalidRemoveTest ()
		{
			IRawElementProviderSimple childProvider
				= ProviderFactory.GetProvider (tabPage1);

			ISelectionItemProvider sel_item_prov
				= (ISelectionItemProvider)childProvider.GetPatternProvider (
					SelectionItemPatternIdentifiers.Pattern.Id);

			tabControl.SelectTab (0);
			Assert.IsTrue (sel_item_prov.IsSelected, "Tab 0 did not get selected!");

			sel_item_prov.RemoveFromSelection ();
		}

		[Test]
		public void IInvokeProviderNoTest ()
		{
			IRawElementProviderSimple provider
				= ProviderFactory.GetProvider (tabPage1);

			object invokeProvider
				= provider.GetPatternProvider (
					InvokePatternIdentifiers.Pattern.Id);
			Assert.IsNull (invokeProvider, "Implements IInvokeProvider when Support: No");
		}
		
		[Test]
		public override void LabeledByAndNamePropertyTest ()
		{
			TestLabeledByAndName (true, false);
		}

		protected override Control GetControlInstance ()
		{
			return null;
		}

		protected override IRawElementProviderSimple GetProvider ()
		{
			return ProviderFactory.GetProvider (tabPage1);
		}

		private TabControl tabControl;
		private TabPage tabPage1, tabPage2;
	}
}
