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

		[Test]
		public override void LabeledByAndNamePropertyTest()
		{
			ToolStripMenuItem menuItem = new ToolStripMenuItem ();
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
			ToolStripMenuItem menuItem = new ToolStripMenuItem ();
			menuItem.Text = "My menu item";
			
			ToolStripMenuItem subMenuItem1 = new ToolStripMenuItem ();
			subMenuItem1.Text = "sub1";
			ToolStripMenuItem subMenuItem2 = new ToolStripMenuItem ();
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
			ToolStripMenuItem subMenuItem3 = new ToolStripMenuItem ();
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

	}
}
