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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	public abstract class ToolStripDropDownItemProviderTest<T> : BaseProviderTest
		where T : ToolStripDropDownItem
	{
		[Test]
		public void ProviderPatternTest ()
		{
			T menuItem = GetNewToolStripDropDownItem ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (menuItem);

			// Should never support Toggle
			object toggleProvider = provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			Assert.IsNull (toggleProvider);

			// Should never support SelectionItem
			object selectionItemProvider = provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			Assert.IsNull (selectionItemProvider);
			
			object invokeProvider = provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (invokeProvider);
			Assert.IsTrue (invokeProvider is IInvokeProvider, "IInvokeProvider");
		}
		
		[Test]
		public void InvokeTest ()
		{
			T menuItem = GetNewToolStripDropDownItem ();
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
			// Set up initial menu item structure, with some sub-items
			T control = GetNewToolStripDropDownItem ();
			control.Text = "My menu item";
			
			T subMenuItem1 = GetNewToolStripDropDownItem ();
			subMenuItem1.Text = "sub1";
			T subMenuItem2 = GetNewToolStripDropDownItem ();
			subMenuItem2.Text = "sub2";

			control.DropDownItems.Add (subMenuItem1);
			control.DropDownItems.Add (subMenuItem2);

			// Add item to parent strip, parent strip to form,
			// and initialize providers
			MenuStrip strip = new MenuStrip ();
			strip.Items.Add (control);
			GetProviderFromControl (strip);
			
			//T control = GetNewToolStripDropDownItem ();
			if (control == null)
				return;
			
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (control);
			
			bridge.ResetEventLists ();
			
			object initialVal =
				provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			Assert.IsNotNull (initialVal, "Property returns null");
			Assert.IsTrue ((bool)initialVal, "Should initialize to true");
			
			control.Enabled = false;

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
			
			control.Enabled = true;
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
			T menuItem = GetNewToolStripDropDownItem ();
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
			T menuItem = GetNewToolStripDropDownItem ();
			menuItem.Text = "My menu item";
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (menuItem);

			Assert.AreEqual (menuItem.Text,
			                 provider.GetPropertyValue (AEIds.NameProperty.Id) as string,
			                 "Name");
			Assert.IsNull (provider.GetPropertyValue (AEIds.LabeledByProperty.Id),
			               "LabeledBy");
		}

		[Test]
		public void NavigationTest ()
		{
			// Set up initial menu item structure, with some sub-items
			T menuItem = GetNewToolStripDropDownItem ();
			menuItem.Text = "My menu item";
			
			T subMenuItem1 = GetNewToolStripDropDownItem ();
			subMenuItem1.Text = "sub1";
			T subMenuItem2 = GetNewToolStripDropDownItem ();
			subMenuItem2.Text = "sub2";

			menuItem.DropDownItems.Add (subMenuItem1);
			menuItem.DropDownItems.Add (subMenuItem2);

			// Add item to parent strip, parent strip to form,
			// and initialize providers
			MenuStrip strip = new MenuStrip ();
			strip.Items.Add (menuItem);
			GetProviderFromControl (strip);
			
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				ProviderFactory.GetProvider (menuItem);
			IRawElementProviderFragmentRoot subProvider1 = (IRawElementProviderFragmentRoot)
				ProviderFactory.GetProvider (subMenuItem1);
			IRawElementProviderFragmentRoot subProvider2 = (IRawElementProviderFragmentRoot)
				ProviderFactory.GetProvider (subMenuItem2);

			
			// Verify that all children are present during navigation
			List<IRawElementProviderFragment> expectedChildren =
				new List<IRawElementProviderFragment> ();
			expectedChildren.Add (subProvider1);
			expectedChildren.Add (subProvider2);

			VerifyChildren (provider, expectedChildren);

			
			// Add new sub-item, verify that navigation updates
			T subMenuItem3 = GetNewToolStripDropDownItem ();
			subMenuItem3.Text = "sub3";
			menuItem.DropDownItems.Add (subMenuItem3);
			IRawElementProviderFragmentRoot subProvider3 = (IRawElementProviderFragmentRoot)
				ProviderFactory.GetProvider (subMenuItem3);
			expectedChildren.Add (subProvider3);

			VerifyChildren (provider, expectedChildren);

			
			// Remove sub-item, verify that navigation updates
			menuItem.DropDownItems.Remove (subMenuItem1);
			expectedChildren.Remove (subProvider1);

			VerifyChildren (provider, expectedChildren);
		}

		private void VerifyChildren (IRawElementProviderFragmentRoot rootProvider,
		                             List<IRawElementProviderFragment> expectedChildren)
		{
			List<IRawElementProviderFragment> foundChildren =
				new List<IRawElementProviderFragment> ();

			// Find all direct children of rootProvider
			IRawElementProviderFragment child;
			child = rootProvider.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				foundChildren.Add (child);
				IRawElementProviderFragment parent =
					child.Navigate (NavigateDirection.Parent);
				Assert.AreEqual (rootProvider, parent,
				                 "Wrong parent");
				child = child.Navigate (NavigateDirection.NextSibling);
			}

			Assert.AreEqual (expectedChildren.Count, foundChildren.Count,
			                 "Did not find expected number of children");

			// Check that all expectedChildren were found during navigation
			List<IRawElementProviderFragment> expectedButNotFound =
				new List<IRawElementProviderFragment> (expectedChildren);
			
			foreach (IRawElementProviderFragment fragment in expectedChildren) {
				if (foundChildren.Contains (fragment))
					expectedButNotFound.Remove (fragment);
			}
			
			if (expectedButNotFound.Count > 0) {
				string message = "These expected children were not found: ";
				foreach (IRawElementProviderFragment fragment in expectedButNotFound) {
					string name = fragment.GetPropertyValue (AEIds.NameProperty.Id) as String;
					if (string.IsNullOrEmpty (name))
						message += "<no name> , ";
					else
						message += name + " , ";
				}
				Assert.Fail (message);
			}
		}

		protected override Control GetControlInstance ()
		{
			return null; // TODO: Lots of work...
		}

		protected abstract T GetNewToolStripDropDownItem ();

	}

	[TestFixture]
	public class ToolStripMenuItemProviderTest :
		ToolStripDropDownItemProviderTest<ToolStripMenuItem>
	{
		protected override ToolStripMenuItem GetNewToolStripDropDownItem ()
		{
			return new ToolStripMenuItem ();
		}

		[Test]
		public void ExpandCollapseProvider ()
		{
			ToolStripMenuItem menuItem = GetNewToolStripDropDownItem ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (menuItem);

			// Should never support ExpandCollapse
			object expandCollapseProvider = provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);
			Assert.IsNull (expandCollapseProvider);
		}
	}

	[TestFixture]
	public class ToolStripDropDownButtonProviderTest :
		ToolStripDropDownItemProviderTest<ToolStripDropDownButton>
	{
		protected override ToolStripDropDownButton GetNewToolStripDropDownItem ()
		{
			return new ToolStripDropDownButton ();
		}
	}

	[TestFixture]
	public class ToolStripSplitButtonProviderTest :
		ToolStripDropDownItemProviderTest<ToolStripSplitButton>
	{
		protected override ToolStripSplitButton GetNewToolStripDropDownItem ()
		{
			return new ToolStripSplitButton ();
		}

		[Test]
		public void ExpandCollapseProvider ()
		{
			ToolStripSplitButton split_button = GetNewToolStripDropDownItem ();
			ToolStripMenuItem item = new ToolStripMenuItem ();
			ToolStrip tool_strip = new ToolStrip ();

			tool_strip.Items.AddRange (new ToolStripItem[] { split_button });
			split_button.DropDownItems.AddRange (new ToolStripItem[] { item });
			Form.Controls.Add (tool_strip);

			tool_strip.Show ();

			IRawElementProviderSimple provider = ProviderFactory.GetProvider (split_button);

			// Should always support ExpandCollapse
			IExpandCollapseProvider expandCollapseProvider
				= (IExpandCollapseProvider) provider.GetPatternProvider (
					ExpandCollapsePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (expandCollapseProvider);

			Assert.AreEqual (ExpandCollapseState.Collapsed,
			                 expandCollapseProvider.ExpandCollapseState);

			expandCollapseProvider.Expand ();
			Assert.AreEqual (ExpandCollapseState.Expanded,
			                 expandCollapseProvider.ExpandCollapseState);

			expandCollapseProvider.Collapse ();
			Assert.AreEqual (ExpandCollapseState.Collapsed,
			                 expandCollapseProvider.ExpandCollapseState);

			// TODO: Test eventing
		}
	}
}
