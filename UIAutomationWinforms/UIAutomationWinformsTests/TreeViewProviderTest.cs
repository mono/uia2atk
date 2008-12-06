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
using System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class TreeViewProviderTest : BaseProviderTest
	{
		#region Tests

        	[Test]
        	public void BasicPropertiesTest ()
        	{
            		TreeView treeView = new TreeView ();
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Tree.Id);

			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "tree");
		}

		[Test]
		public void ProviderPatternTest ()
		{
			TreeView treeView = new TreeView ();
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			object selectionProvider =
				provider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (selectionProvider,
			                  "Not returning SelectionPatternIdentifiers.");
			Assert.IsTrue (selectionProvider is ISelectionProvider,
			               "Not returning SelectionPatternIdentifiers.");

			// TODO: This should depend on treeView.Scrollable
			object scrollProvider =
				provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (scrollProvider,
			                  "Not returning ScrollPatternIdentifiers.");
			Assert.IsTrue (scrollProvider is IScrollProvider,
			               "Not returning ScrollPatternIdentifiers.");
		}

		#region ISelectionProvider Tests

		[Test]
		public void CanSelectMultipleTest ()
		{
			TreeView treeView = new TreeView ();
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			ISelectionProvider selectionProvider = (ISelectionProvider)
				provider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);

			Assert.IsFalse (selectionProvider.CanSelectMultiple,
			                "False when empty");

			TestProperty (provider,
			              SelectionPatternIdentifiers.CanSelectMultipleProperty,
			              false);

			treeView.Nodes.AddRange (new TreeNode [] {
				new TreeNode ("one"),
				new TreeNode ("two"),
				new TreeNode ("three")});

			Assert.IsFalse (selectionProvider.CanSelectMultiple,
			                "False when containing multiple nodes");

			TestProperty (provider,
			              SelectionPatternIdentifiers.CanSelectMultipleProperty,
			              false);
		}

		[Test]
		public void IsSelectionRequiredTest ()
		{
			TreeView treeView = new TreeView ();
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			ISelectionProvider selectionProvider = (ISelectionProvider)
				provider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);

			Assert.IsFalse (selectionProvider.IsSelectionRequired,
			                "False when empty");

			TestProperty (provider,
			              SelectionPatternIdentifiers.IsSelectionRequiredProperty,
			              false);

			bridge.ResetEventLists ();

			treeView.Nodes.AddRange (new TreeNode [] {
				new TreeNode ("one"),
				new TreeNode ("two"),
				new TreeNode ("three")});

			Assert.IsFalse (selectionProvider.IsSelectionRequired,
			                "False when non-empty, until selected");

			TestProperty (provider,
			              SelectionPatternIdentifiers.IsSelectionRequiredProperty,
			              false);

			treeView.SelectedNode = treeView.Nodes [0];

			Assert.IsTrue (selectionProvider.IsSelectionRequired,
			                "True when non-empty, after selected");

			TestProperty (provider,
			              SelectionPatternIdentifiers.IsSelectionRequiredProperty,
			              true);

			treeView.SelectedNode = null;

			Assert.IsFalse (selectionProvider.IsSelectionRequired,
			                "False when non-empty, selected node set to null");

			TestProperty (provider,
			              SelectionPatternIdentifiers.IsSelectionRequiredProperty,
			              false);

//			Assert.AreEqual (1, // TODO: When finished, will probably be more than 1! Search instead!
//			                 bridge.AutomationPropertyChangedEvents.Count,
//			                 "property change event expected");
//			AutomationPropertyChangedEventTuple eventTuple =
//				bridge.AutomationPropertyChangedEvents [0];
//			Assert.AreEqual (provider,
//			                 eventTuple.element,
//			                 "event sender");
//			Assert.AreEqual (SelectionPatternIdentifiers.IsSelectionRequiredProperty,
//			                 eventTuple.e.Property,
//			                 "event property");
//			Assert.AreEqual (true,
//			                 eventTuple.e.NewValue,
//			                 "event new value");
//			Assert.AreEqual (false,
//			                 eventTuple.e.OldValue,
//			                 "event old value");
		}

		[Test]
		public void GetSelectionTest ()
		{
			TreeView treeView = new TreeView ();
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			ISelectionProvider selectionProvider = (ISelectionProvider)
				provider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);

			IRawElementProviderSimple [] selection =
				selectionProvider.GetSelection ();
			Assert.IsNotNull (selection);
			Assert.AreEqual (0,
			                 selection.Length,
			                "Selection list empty when tree view empty");

			TestProperty (provider,
			              SelectionPatternIdentifiers.SelectionProperty,
			              new IRawElementProviderSimple [] {});

			bridge.ResetEventLists ();

			TreeNode node1 = new TreeNode ("node1");
			TreeNode node2 = new TreeNode ("node2");
			TreeNode node3 = new TreeNode ("node3");

			treeView.Nodes.AddRange (new TreeNode [] {
				node1,
				node2,
				node3});

			selection =
				selectionProvider.GetSelection ();

			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragmentRoot node2Provider = (IRawElementProviderFragmentRoot)
				node1Provider.Navigate (NavigateDirection.NextSibling);
			IRawElementProviderFragmentRoot node3Provider = (IRawElementProviderFragmentRoot)
				node2Provider.Navigate (NavigateDirection.NextSibling);

			// Make sure we have the right provider for the right node
			TestProperty (node1Provider,
			              AEIds.NameProperty,
			              node1.Text);
			TestProperty (node2Provider,
			              AEIds.NameProperty,
			              node2.Text);
			TestProperty (node3Provider,
			              AEIds.NameProperty,
			              node3.Text);
			
			Assert.IsNotNull (selection);
			Assert.AreEqual (0,
			                 selection.Length,
			                "Selection list empty when tree view not empty, but not yet selected");

			TestProperty (provider,
			              SelectionPatternIdentifiers.SelectionProperty,
			              new IRawElementProviderSimple [] {});

			VerifySelection (new IRawElementProviderSimple [] {node1Provider, node2Provider, node3Provider},
			                 new IRawElementProviderSimple [] {});

			treeView.SelectedNode = node1;

			selection =
				selectionProvider.GetSelection ();
			Assert.IsNotNull (selection);
			Assert.AreEqual (1,
			                 selection.Length,
			                "Selection list not empty when tree view not empty");
			Assert.AreEqual (node1Provider,
			                 selection [0],
			                 "Expected TODO to be selected");

			TestProperty (provider,
			              SelectionPatternIdentifiers.SelectionProperty,
			              new IRawElementProviderSimple [] {node1Provider});

			VerifySelection (new IRawElementProviderSimple [] {node2Provider, node3Provider},
			                 new IRawElementProviderSimple [] {node1Provider});

			// TODO: Figure out what will be supported when it comes to events
//			Assert.AreEqual (1, // TODO: When finished, will probably be more than 1! Search instead!
//			                 bridge.AutomationPropertyChangedEvents.Count,
//			                 "property change event expected");
//			AutomationPropertyChangedEventTuple eventTuple =
//				bridge.AutomationPropertyChangedEvents [0];
//			Assert.AreEqual (provider,
//			                 eventTuple.element,
//			                 "event sender");
//			Assert.AreEqual (SelectionPatternIdentifiers.SelectionProperty,
//			                 eventTuple.e.Property,
//			                 "event property");
//			Assert.AreEqual (new IRawElementProviderSimple [] {/*TODO*/},
//			                 eventTuple.e.NewValue,
//			                 "event new value");
//			Assert.AreEqual (new IRawElementProviderSimple [] {},
//			                 eventTuple.e.OldValue,
//			                 "event old value");

			bridge.ResetEventLists ();

			treeView.SelectedNode = node2;

			selection =
				selectionProvider.GetSelection ();
			Assert.IsNotNull (selection);
			Assert.AreEqual (1,
			                 selection.Length,
			                "Selection list not empty when tree view not empty");
			Assert.AreEqual (node2Provider,
			                 selection [0],
			                 "Expected TODO to be selected");

			TestProperty (provider,
			              SelectionPatternIdentifiers.SelectionProperty,
			              new IRawElementProviderSimple [] {node2Provider});

			VerifySelection (new IRawElementProviderSimple [] {node1Provider, node3Provider},
			                 new IRawElementProviderSimple [] {node2Provider});

//			Assert.AreEqual (1, // TODO: When finished, will probably be more than 1! Search instead!
//			                 bridge.AutomationPropertyChangedEvents.Count,
//			                 "property change event expected");
//			eventTuple =
//				bridge.AutomationPropertyChangedEvents [0];
//			Assert.AreEqual (provider,
//			                 eventTuple.element,
//			                 "event sender");
//			Assert.AreEqual (SelectionPatternIdentifiers.SelectionProperty,
//			                 eventTuple.e.Property,
//			                 "event property");
//			Assert.AreEqual (new IRawElementProviderSimple [] {/*TODO*/},
//			                 eventTuple.e.NewValue,
//			                 "event new value");
//			Assert.AreEqual (new IRawElementProviderSimple [] {/*TODO*/},
//			                 eventTuple.e.OldValue,
//			                 "event old value");
		}

		#endregion
		
		#endregion

		#region Private Helper Methods

		private void VerifySelection (IRawElementProviderSimple [] expectedNotSelected,
		                              IRawElementProviderSimple [] expectedSelection)
		{
			for (int i = 0; i < expectedNotSelected.Length; i++) {
				ISelectionItemProvider item = (ISelectionItemProvider)
					expectedNotSelected [i].GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
				Assert.IsNotNull (item,
				                  "SelectionItem Pattern support expected");
				Assert.IsFalse (item.IsSelected,
				                "Item should not be selected");
			}

			for (int i = 0; i < expectedSelection.Length; i++) {
				ISelectionItemProvider item = (ISelectionItemProvider)
					expectedSelection [i].GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
				Assert.IsNotNull (item,
				                  "SelectionItem Pattern support expected");
				Assert.IsTrue (item.IsSelected,
				                "Item should not selected");
			}
		}
		
		#endregion
		
		#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return new TreeView ();
		}

		#endregion
	}
}
