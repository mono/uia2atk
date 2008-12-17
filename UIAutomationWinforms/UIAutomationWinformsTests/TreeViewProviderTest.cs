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

		#region Basic Tests

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
			
			object scrollProvider =
				provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			Assert.IsNull (scrollProvider,
			               "No ScrollPattern support for empty tree");

			TreeNode node = treeView.Nodes.Add ("test");
			while (node.IsVisible) {
				scrollProvider =
					provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
				Assert.IsNull (scrollProvider,
				               "No ScrollPattern support for barely-filled tree; count:" + treeView.Nodes.Count.ToString ());
				node = treeView.Nodes.Add ("test");
			}

			scrollProvider =
				provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			
			Assert.IsNotNull (scrollProvider,
			                  "Not returning ScrollPatternIdentifiers.");
			Assert.IsTrue (scrollProvider is IScrollProvider,
			               "Not returning ScrollPatternIdentifiers.");
		}

		#endregion

		#region TreeNode Basic Tests

		[Test]
		public void TreeNodeBasicPropertiesTest ()
		{
            		TreeView treeView = new TreeView ();
			TreeNode node1 = treeView.Nodes.Add ("node1");
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);
			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild);

			TestProperty (node1Provider,
			              AEIds.ControlTypeProperty,
			              ControlType.TreeItem.Id);

			TestProperty (node1Provider,
			              AEIds.LocalizedControlTypeProperty,
			              "tree item");

			TestProperty (node1Provider,
			              AEIds.NameProperty,
			              node1.Text);

			TestProperty (node1Provider,
			              AEIds.LabeledByProperty,
			              null);
		}

		#endregion

		#region ISelectionProvider/ISelectionItemProvider Tests

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

		/// <summary>
		/// Test Selection.GetSelection and GetSelection.IsSelected,
		/// and associated change events.
		/// </summary>
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

			TreeNode node1 = new TreeNode ("node1");
			TreeNode node2 = new TreeNode ("node2");
			TreeNode node3 = new TreeNode ("node3");
			
			TreeNode node1sub1 = new TreeNode ("node1 - sub1");
			node1.Nodes.Add (node1sub1);

			treeView.Nodes.AddRange (new TreeNode [] {
				node1,
				node2,
				node3});
			node1.Expand ();

			selection =
				selectionProvider.GetSelection ();

			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragmentRoot node2Provider = (IRawElementProviderFragmentRoot)
				node1Provider.Navigate (NavigateDirection.NextSibling);
			IRawElementProviderFragmentRoot node3Provider = (IRawElementProviderFragmentRoot)
				node2Provider.Navigate (NavigateDirection.NextSibling);
			IRawElementProviderFragmentRoot node1sub1Provider = (IRawElementProviderFragmentRoot)
				node1Provider.Navigate (NavigateDirection.FirstChild);

			VerifyTreeNodePatterns (provider, node1Provider, node1);
			VerifyTreeNodePatterns (provider, node2Provider, node2);
			VerifyTreeNodePatterns (provider, node3Provider, node3);
			VerifyTreeNodePatterns (provider, node1sub1Provider, node1sub1);

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
			TestProperty (node1sub1Provider,
			              AEIds.NameProperty,
			              node1sub1.Text);
			
			Assert.IsNotNull (selection);
			Assert.AreEqual (0,
			                 selection.Length,
			                "Selection list empty when tree view not empty, but not yet selected");

			TestProperty (provider,
			              SelectionPatternIdentifiers.SelectionProperty,
			              new IRawElementProviderSimple [] {});

			VerifySelection (new IRawElementProviderSimple [] {node1Provider, node2Provider, node3Provider, node1sub1Provider},
			                 new IRawElementProviderSimple [] {});

			bridge.ResetEventLists ();

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

			VerifySelection (new IRawElementProviderSimple [] {node2Provider, node3Provider, node1sub1Provider},
			                 new IRawElementProviderSimple [] {node1Provider});

			Assert.AreEqual (2,
			                 bridge.AutomationPropertyChangedEvents.Count,
			                 "property change event expected");
			AutomationPropertyChangedEventTuple propertyEventTuple =
				bridge.GetAutomationPropertyEventFrom (provider, SelectionPatternIdentifiers.SelectionProperty.Id);
			Assert.AreEqual (new IRawElementProviderSimple [] {node1Provider},
			                 propertyEventTuple.e.NewValue,
			                 "event new value");
			Assert.AreEqual (new IRawElementProviderSimple [] {},
			                 propertyEventTuple.e.OldValue,
			                 "event old value");
			
			propertyEventTuple =
				bridge.GetAutomationPropertyEventFrom (node1Provider, SelectionItemPatternIdentifiers.IsSelectedProperty.Id);
			Assert.AreEqual (true,
			                 propertyEventTuple.e.NewValue,
			                 "event new value");
			Assert.AreEqual (false,
			                 propertyEventTuple.e.OldValue,
			                 "event old value");

			Assert.AreEqual (1,
			                 bridge.GetAutomationEventCount (SelectionItemPatternIdentifiers.ElementSelectedEvent),
			                 "selection change event expected");
			AutomationEventTuple eventTuple =
				bridge.GetAutomationEventFrom (node1Provider, SelectionItemPatternIdentifiers.ElementSelectedEvent.Id);
			Assert.AreEqual (SelectionItemPatternIdentifiers.ElementSelectedEvent,
			                 eventTuple.e.EventId,
			                 "event id");

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

			VerifySelection (new IRawElementProviderSimple [] {node1Provider, node3Provider, node1sub1Provider},
			                 new IRawElementProviderSimple [] {node2Provider});

			Assert.AreEqual (2,
			                 bridge.AutomationPropertyChangedEvents.Count,
			                 "property change event expected");
			propertyEventTuple =
				bridge.GetAutomationPropertyEventFrom (provider, SelectionPatternIdentifiers.SelectionProperty.Id);
			Assert.AreEqual (new IRawElementProviderSimple [] {node2Provider},
			                 propertyEventTuple.e.NewValue,
			                 "event new value");
			Assert.AreEqual (new IRawElementProviderSimple [] {node1Provider},
			                 propertyEventTuple.e.OldValue,
			                 "event old value");
			
			propertyEventTuple =
				bridge.GetAutomationPropertyEventFrom (node2Provider, SelectionItemPatternIdentifiers.IsSelectedProperty.Id);
			Assert.AreEqual (true,
			                 propertyEventTuple.e.NewValue,
			                 "event new value");
			Assert.AreEqual (false,
			                 propertyEventTuple.e.OldValue,
			                 "event old value");

			Assert.AreEqual (1,
			                 bridge.GetAutomationEventCount (SelectionItemPatternIdentifiers.ElementSelectedEvent),
			                 "selection change event expected");
			eventTuple =
				bridge.GetAutomationEventFrom (node2Provider, SelectionItemPatternIdentifiers.ElementSelectedEvent.Id);
			Assert.AreEqual (SelectionItemPatternIdentifiers.ElementSelectedEvent,
			                 eventTuple.e.EventId,
			                 "event id");

			// Test subnode
			treeView.SelectedNode = node1sub1;

			selection =
				selectionProvider.GetSelection ();
			Assert.IsNotNull (selection);
			Assert.AreEqual (1,
			                 selection.Length,
			                "Selection list not empty when tree view not empty");
			Assert.AreEqual (node1sub1Provider,
			                 selection [0],
			                 "Expected TODO to be selected");

			TestProperty (provider,
			              SelectionPatternIdentifiers.SelectionProperty,
			              new IRawElementProviderSimple [] {node1sub1Provider});

			VerifySelection (new IRawElementProviderSimple [] {node1Provider, node3Provider, node2Provider},
			                 new IRawElementProviderSimple [] {node1sub1Provider});

			// Quickie test of node collection change events
			// TODO: Break out into a more thorough test
			bridge.ResetEventLists ();
			node1sub1.Nodes.Add ("node1 - sub1 - sub1");
			Assert.AreNotEqual (0, bridge.StructureChangedEvents.Count);

			bridge.ResetEventLists ();
			node1sub1.Nodes.Clear ();
			Assert.AreNotEqual (0, bridge.StructureChangedEvents.Count);
		}

		[Test]
		public void SelectTest ()
		{
			TreeView treeView = new TreeView ();
			TreeNode node1 = new TreeNode ("node1");
			TreeNode node2 = new TreeNode ("node2");
			TreeNode node3 = new TreeNode ("node3");

			treeView.Nodes.AddRange (new TreeNode [] {
				node1,
				node2,
				node3});
			
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragmentRoot node2Provider = (IRawElementProviderFragmentRoot)
				node1Provider.Navigate (NavigateDirection.NextSibling);
			IRawElementProviderFragmentRoot node3Provider = (IRawElementProviderFragmentRoot)
				node2Provider.Navigate (NavigateDirection.NextSibling);

			ISelectionItemProvider node1Selection = (ISelectionItemProvider)
				node1Provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			ISelectionItemProvider node2Selection = (ISelectionItemProvider)
				node2Provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			ISelectionItemProvider node3Selection = (ISelectionItemProvider)
				node3Provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);

			node1Selection.Select ();
			Assert.AreEqual (node1,
			                 treeView.SelectedNode);
			
			node2Selection.Select ();
			Assert.AreEqual (node2,
			                 treeView.SelectedNode);
			
			node3Selection.Select ();
			Assert.AreEqual (node3,
			                 treeView.SelectedNode);

			treeView.Enabled = false;

			try {
				node1Selection.Select ();
				Assert.Fail ("Expected ElementNotEnabledException");
			} catch (ElementNotEnabledException) {
				// Expected !
			}
		}

		[Test]
		public void AddToSelectionTest ()
		{
			TreeView treeView = new TreeView ();
			TreeNode node1 = new TreeNode ("node1");
			TreeNode node2 = new TreeNode ("node2");
			TreeNode node3 = new TreeNode ("node3");

			treeView.Nodes.AddRange (new TreeNode [] {
				node1,
				node2,
				node3});
			
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragmentRoot node2Provider = (IRawElementProviderFragmentRoot)
				node1Provider.Navigate (NavigateDirection.NextSibling);

			ISelectionItemProvider node1Selection = (ISelectionItemProvider)
				node1Provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			ISelectionItemProvider node2Selection = (ISelectionItemProvider)
				node2Provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);

			node1Selection.AddToSelection ();
			Assert.AreEqual (node1,
			                 treeView.SelectedNode,
			                 "AddToSelection when no node has yet been selected should work");

			try {
				node2Selection.AddToSelection ();
				Assert.Fail ("Expected InvalidOperationException when AddToSelection is called and there is already a selected node");
			} catch (InvalidOperationException) {
				// Expected!
			}

			treeView.SelectedNode = null;
			
			node2Selection.AddToSelection ();
			Assert.AreEqual (node2,
			                 treeView.SelectedNode,
			                 "AddToSelection with no SelectedNode should work");

			treeView.Enabled = false;

			try {
				node1Selection.AddToSelection ();
				Assert.Fail ("Expected ElementNotEnabledException");
			} catch (ElementNotEnabledException) {
				// Expected !
			}

			treeView.SelectedNode = null;

			try {
				node1Selection.AddToSelection ();
				Assert.Fail ("Expected ElementNotEnabledException");
			} catch (ElementNotEnabledException) {
				// Expected !
			}
		}

		[Test]
		public void RemoveFromSelectionTest ()
		{
			TreeView treeView = new TreeView ();
			TreeNode node1 = new TreeNode ("node1");
			TreeNode node2 = new TreeNode ("node2");
			TreeNode node3 = new TreeNode ("node3");

			treeView.Nodes.AddRange (new TreeNode [] {
				node1,
				node2,
				node3});
			
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragmentRoot node2Provider = (IRawElementProviderFragmentRoot)
				node1Provider.Navigate (NavigateDirection.NextSibling);

			ISelectionItemProvider node1Selection = (ISelectionItemProvider)
				node1Provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			ISelectionItemProvider node2Selection = (ISelectionItemProvider)
				node2Provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);

			// Should have no effect
			node1Selection.RemoveFromSelection ();

			node1Selection.Select ();
			try {
				node1Selection.RemoveFromSelection ();
				Assert.Fail ("Expected InvalidOperationException when RemoveFromSelection is called on a selected node");
			} catch (InvalidOperationException) {
				// Expected!
			}

			Assert.IsTrue (node1Selection.IsSelected,
			               "RemoveToSelection should not have changed selection");

			// Should have no effect
			node2Selection.RemoveFromSelection ();

			treeView.Enabled = false;

			try {
				node1Selection.RemoveFromSelection ();
				Assert.Fail ("Expected ElementNotEnabledException");
			} catch (ElementNotEnabledException) {
				// Expected!
			}

			try {
				node2Selection.RemoveFromSelection ();
				Assert.Fail ("Expected ElementNotEnabledException");
			} catch (ElementNotEnabledException) {
				// Expected!
			}
		}

		[Test]
		public void SelectionContainerTest ()
		{
			TreeView treeView = new TreeView ();
			TreeNode node1 = new TreeNode ("node1");

			treeView.Nodes.Add (node1);
			
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild);

			ISelectionItemProvider node1Selection = (ISelectionItemProvider)
				node1Provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);

			Assert.AreEqual (provider,
			                 node1Selection.SelectionContainer);
			Assert.AreEqual (provider,
			                 node1Provider.GetPropertyValue (SelectionItemPatternIdentifiers.SelectionContainerProperty.Id));
		}

		#endregion

		#region IScrollProvider/IScrollItemProvider Tests

		[Test]
		public void ScrollIntoViewTest ()
		{
			TreeView treeView = new TreeView ();
			
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			TreeNode tempNode = treeView.Nodes.Add ("test");
			while (tempNode.IsVisible) {
				tempNode = treeView.Nodes.Add ("test");
			}
			
			TreeNode node1 = new TreeNode ("node1");
			TreeNode node2 = new TreeNode ("node2");
			TreeNode node3 = new TreeNode ("node3");

			treeView.Nodes.AddRange (new TreeNode [] {
				node1,
				node2,
				node3});

			TreeNode subNode1 = treeView.Nodes [0].Nodes.Add ("test - sub1");

			IRawElementProviderFragmentRoot node3Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.LastChild);
			IRawElementProviderFragmentRoot node2Provider = (IRawElementProviderFragmentRoot)
				node3Provider.Navigate (NavigateDirection.PreviousSibling);
			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				node2Provider.Navigate (NavigateDirection.PreviousSibling);
			IRawElementProviderFragmentRoot subNode1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild).Navigate (NavigateDirection.FirstChild);

			VerifyTreeNodePatterns (provider, node1Provider, node1);
			VerifyTreeNodePatterns (provider, node2Provider, node2);
			VerifyTreeNodePatterns (provider, node3Provider, node3);
			VerifyTreeNodePatterns (provider, subNode1Provider, subNode1);

			IScrollItemProvider node1ScrollItem = (IScrollItemProvider)
				node1Provider.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id);
			IScrollItemProvider node3ScrollItem = (IScrollItemProvider)
				node3Provider.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id);
			IScrollItemProvider subNode1ScrollItem = (IScrollItemProvider)
				subNode1Provider.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id);
			
			Assert.IsFalse (node1.IsVisible);
			Assert.IsFalse (node2.IsVisible);
			Assert.IsFalse (node3.IsVisible);
			Assert.IsFalse (subNode1.IsVisible);

			node1ScrollItem.ScrollIntoView ();

			Assert.IsTrue (node1.IsVisible);
			Assert.IsFalse (node2.IsVisible);
			Assert.IsFalse (node3.IsVisible);
			Assert.IsFalse (subNode1.IsVisible);

			node3ScrollItem.ScrollIntoView ();

			Assert.IsTrue (node1.IsVisible);
			Assert.IsTrue (node2.IsVisible);
			Assert.IsTrue (node3.IsVisible);
			Assert.IsFalse (subNode1.IsVisible);

			subNode1ScrollItem.ScrollIntoView ();

			Assert.IsFalse (node1.IsVisible);
			Assert.IsFalse (node2.IsVisible);
			Assert.IsFalse (node3.IsVisible);
			Assert.IsTrue (subNode1.IsVisible);

			treeView.Enabled = false;

			VerifyTreeNodePatterns (provider, node1Provider, node1);
			VerifyTreeNodePatterns (provider, node2Provider, node2);
			VerifyTreeNodePatterns (provider, node3Provider, node3);
			VerifyTreeNodePatterns (provider, subNode1Provider, subNode1);

			treeView.Enabled = true;

			VerifyTreeNodePatterns (provider, node1Provider, node1);
			VerifyTreeNodePatterns (provider, node2Provider, node2);
			VerifyTreeNodePatterns (provider, node3Provider, node3);
			VerifyTreeNodePatterns (provider, subNode1Provider, subNode1);
		}
		
		#endregion

		#region IToggleProvider Tests

		[Test]
		public void ToggleStateTest ()
		{
			TreeView treeView = new TreeView ();
			TreeNode node1 = new TreeNode ("node1");
			TreeNode node2 = new TreeNode ("node2");
			TreeNode node3 = new TreeNode ("node3");

			treeView.Nodes.AddRange (new TreeNode [] {
				node1,
				node2,
				node3});
			
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragmentRoot node2Provider = (IRawElementProviderFragmentRoot)
				node1Provider.Navigate (NavigateDirection.NextSibling);
			IRawElementProviderFragmentRoot node3Provider = (IRawElementProviderFragmentRoot)
				node2Provider.Navigate (NavigateDirection.NextSibling);

			bridge.ResetEventLists ();
			
			node1.Checked = true;

			Assert.AreEqual (0,
			                 bridge.AutomationPropertyChangedEvents.Count,
			                 "no property change event expected with CheckBoxes disabled");

			VerifyTreeNodePatterns (provider, node1Provider, node1);
			VerifyTreeNodePatterns (provider, node2Provider, node2);
			VerifyTreeNodePatterns (provider, node3Provider, node3);

			treeView.CheckBoxes = true;

			VerifyTreeNodePatterns (provider, node1Provider, node1);
			VerifyTreeNodePatterns (provider, node2Provider, node2);
			VerifyTreeNodePatterns (provider, node3Provider, node3);

			IToggleProvider node1Toggle = (IToggleProvider)
				node1Provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			IToggleProvider node2Toggle = (IToggleProvider)
				node2Provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			IToggleProvider node3Toggle = (IToggleProvider)
				node3Provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);

			Assert.AreEqual (ToggleState.On, node1Toggle.ToggleState);
			Assert.AreEqual (ToggleState.Off, node2Toggle.ToggleState);
			Assert.AreEqual (ToggleState.Off, node3Toggle.ToggleState);

			bridge.ResetEventLists ();

			node1.Checked = false;
			node3.Checked = true;

			Assert.AreEqual (2,
			                 bridge.AutomationPropertyChangedEvents.Count,
			                 "property change events expected");
			AutomationPropertyChangedEventTuple propertyEventTuple =
				bridge.GetAutomationPropertyEventFrom (node1Provider,
				                                       TogglePatternIdentifiers.ToggleStateProperty.Id);
			Assert.IsNotNull (propertyEventTuple,
			                  string.Format ("Expected property change event with provider {0} and property {1}",
			                                 node1Provider, TogglePatternIdentifiers.ToggleStateProperty.ProgrammaticName));
			Assert.AreEqual (ToggleState.Off,
			                 propertyEventTuple.e.NewValue,
			                 "event new value");
			Assert.AreEqual (ToggleState.On,
			                 propertyEventTuple.e.OldValue,
			                 "event old value");
			propertyEventTuple =
				bridge.GetAutomationPropertyEventFrom (node3Provider,
				                                       TogglePatternIdentifiers.ToggleStateProperty.Id);
			Assert.IsNotNull (propertyEventTuple,
			                  string.Format ("Expected property change event with provider {0} and property {1}",
			                                 node1Provider, TogglePatternIdentifiers.ToggleStateProperty.ProgrammaticName));
			Assert.AreEqual (ToggleState.On,
			                 propertyEventTuple.e.NewValue,
			                 "event new value");
			Assert.AreEqual (ToggleState.Off,
			                 propertyEventTuple.e.OldValue,
			                 "event old value");

			Assert.AreEqual (ToggleState.Off, node1Toggle.ToggleState);
			Assert.AreEqual (ToggleState.Off, node2Toggle.ToggleState);
			Assert.AreEqual (ToggleState.On, node3Toggle.ToggleState);
		}

		[Test]
		public void ToggleTest ()
		{
			TreeView treeView = new TreeView ();
			TreeNode node1 = new TreeNode ("node1");
			TreeNode node2 = new TreeNode ("node2");
			TreeNode node3 = new TreeNode ("node3");
			TreeNode subNode1 = node1.Nodes.Add ("node1 - sub1");

			treeView.Nodes.AddRange (new TreeNode [] {
				node1,
				node2,
				node3});
			
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragmentRoot node2Provider = (IRawElementProviderFragmentRoot)
				node1Provider.Navigate (NavigateDirection.NextSibling);
			IRawElementProviderFragmentRoot node3Provider = (IRawElementProviderFragmentRoot)
				node2Provider.Navigate (NavigateDirection.NextSibling);
			IRawElementProviderFragmentRoot subNode1Provider = (IRawElementProviderFragmentRoot)
				node1Provider.Navigate (NavigateDirection.FirstChild);

			node1.Checked = true;

			VerifyTreeNodePatterns (provider, node1Provider, node1);
			VerifyTreeNodePatterns (provider, node2Provider, node2);
			VerifyTreeNodePatterns (provider, node3Provider, node3);
			VerifyTreeNodePatterns (provider, subNode1Provider, subNode1);

			treeView.CheckBoxes = true;

			VerifyTreeNodePatterns (provider, node1Provider, node1);
			VerifyTreeNodePatterns (provider, node2Provider, node2);
			VerifyTreeNodePatterns (provider, node3Provider, node3);
			VerifyTreeNodePatterns (provider, subNode1Provider, subNode1);

			IToggleProvider node1Toggle = (IToggleProvider)
				node1Provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			IToggleProvider node2Toggle = (IToggleProvider)
				node2Provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			IToggleProvider node3Toggle = (IToggleProvider)
				node3Provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			IToggleProvider subNode1Toggle = (IToggleProvider)
				subNode1Provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);

			Assert.AreEqual (true, node1.Checked);
			Assert.AreEqual (false, node2.Checked);
			Assert.AreEqual (false, node3.Checked);
			Assert.AreEqual (false, subNode1.Checked);

			node1Toggle.Toggle ();
			node3Toggle.Toggle ();

			Assert.AreEqual (false, node1.Checked);
			Assert.AreEqual (false, node2.Checked);
			Assert.AreEqual (true, node3.Checked);
			Assert.AreEqual (false, subNode1.Checked);

			node2Toggle.Toggle ();
			subNode1Toggle.Toggle ();

			Assert.AreEqual (false, node1.Checked);
			Assert.AreEqual (true, node2.Checked);
			Assert.AreEqual (true, node3.Checked);
			Assert.AreEqual (true, subNode1.Checked);

			treeView.Enabled = false;

			try {
				node2Toggle.Toggle ();
				Assert.Fail ("Expected ElementNotEnabledException");
			} catch (ElementNotEnabledException) {
				// Expected!
			}

			// Nothing should have changed...
			Assert.AreEqual (false, node1.Checked);
			Assert.AreEqual (true, node2.Checked);
			Assert.AreEqual (true, node3.Checked);
			Assert.AreEqual (true, subNode1.Checked);
		}
		
		#endregion

		#region IExpandCollapseProvider Tests

		[Test]
		public void ExpandCollapseStateTest ()
		{
			TreeView treeView = new TreeView ();

			TreeNode node1 = new TreeNode ("node1");
			TreeNode node2 = new TreeNode ("node2");
			
			TreeNode node1sub1 = new TreeNode ("node1 - sub1");
			TreeNode node1sub2 = new TreeNode ("node1 - sub2");
			node1.Nodes.Add (node1sub1);
			node1.Nodes.Add (node1sub2);

			TreeNode node1sub1sub1 = new TreeNode ("node1 - sub1 - sub1");
			node1sub1.Nodes.Add (node1sub1sub1);

			treeView.Nodes.AddRange (new TreeNode [] {
				node1,
				node2});

			treeView.CollapseAll ();
			
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragmentRoot node2Provider = (IRawElementProviderFragmentRoot)
				node1Provider.Navigate (NavigateDirection.NextSibling);
			
			IRawElementProviderFragmentRoot node1sub1Provider = (IRawElementProviderFragmentRoot)
				node1Provider.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragmentRoot node1sub2Provider = (IRawElementProviderFragmentRoot)
				node1sub1Provider.Navigate (NavigateDirection.NextSibling);
			
			IRawElementProviderFragmentRoot node1sub1sub1Provider = (IRawElementProviderFragmentRoot)
				node1sub1Provider.Navigate (NavigateDirection.FirstChild);

			VerifyTreeNodePatterns (provider, node1Provider, node1);
			VerifyTreeNodePatterns (provider, node2Provider, node2);
			VerifyTreeNodePatterns (provider, node1sub1Provider, node1sub1);
			VerifyTreeNodePatterns (provider, node1sub2Provider, node1sub2);
			VerifyTreeNodePatterns (provider, node1sub1sub1Provider, node1sub1sub1);

			IExpandCollapseProvider node1ExpandCollapse = (IExpandCollapseProvider)
				node1Provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);
			IExpandCollapseProvider node2ExpandCollapse = (IExpandCollapseProvider)
				node2Provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);

			IExpandCollapseProvider node1sub1ExpandCollapse = (IExpandCollapseProvider)
				node1sub1Provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);
			IExpandCollapseProvider node1sub2ExpandCollapse = (IExpandCollapseProvider)
				node1sub2Provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);

			IExpandCollapseProvider node1sub1sub1ExpandCollapse = (IExpandCollapseProvider)
				node1sub1sub1Provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);

			Assert.AreEqual (ExpandCollapseState.Collapsed,
			                 node1ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.LeafNode,
			                 node2ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.Collapsed,
			                 node1sub1ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.LeafNode,
			                 node1sub2ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.LeafNode,
			                 node1sub1sub1ExpandCollapse.ExpandCollapseState);

			TestProperty (node1Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.Collapsed);
			TestProperty (node2Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.LeafNode);
			TestProperty (node1sub1Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.Collapsed);
			TestProperty (node1sub2Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.LeafNode);
			TestProperty (node1sub1sub1Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.LeafNode);

			bridge.ResetEventLists ();
			
			node1.Expand ();
			
			AutomationPropertyChangedEventTuple propertyEventTuple =
				bridge.GetAutomationPropertyEventFrom (node1Provider,
				                                       ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id);
			Assert.IsNotNull (propertyEventTuple,
			                  "Expected property change event on node 1");
			Assert.AreEqual (ExpandCollapseState.Collapsed,
			                 propertyEventTuple.e.OldValue,
			                 "Old value");
			Assert.AreEqual (ExpandCollapseState.Expanded,
			                 propertyEventTuple.e.NewValue,
			                 "New value");

			Assert.AreEqual (ExpandCollapseState.Expanded,
			                 node1ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.LeafNode,
			                 node2ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.Collapsed,
			                 node1sub1ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.LeafNode,
			                 node1sub2ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.LeafNode,
			                 node1sub1sub1ExpandCollapse.ExpandCollapseState);

			TestProperty (node1Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.Expanded);
			TestProperty (node2Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.LeafNode);
			TestProperty (node1sub1Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.Collapsed);
			TestProperty (node1sub2Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.LeafNode);
			TestProperty (node1sub1sub1Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.LeafNode);

			bridge.ResetEventLists ();
			
			node1sub1.Expand ();
			
			propertyEventTuple =
				bridge.GetAutomationPropertyEventFrom (node1sub1Provider,
				                                       ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id);
			Assert.IsNotNull (propertyEventTuple,
			                  "Expected property change event on node 1 sub 1");
			Assert.AreEqual (ExpandCollapseState.Collapsed,
			                 propertyEventTuple.e.OldValue,
			                 "Old value");
			Assert.AreEqual (ExpandCollapseState.Expanded,
			                 propertyEventTuple.e.NewValue,
			                 "New value");

			Assert.AreEqual (ExpandCollapseState.Expanded,
			                 node1ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.LeafNode,
			                 node2ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.Expanded,
			                 node1sub1ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.LeafNode,
			                 node1sub2ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.LeafNode,
			                 node1sub1sub1ExpandCollapse.ExpandCollapseState);

			TestProperty (node1Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.Expanded);
			TestProperty (node2Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.LeafNode);
			TestProperty (node1sub1Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.Expanded);
			TestProperty (node1sub2Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.LeafNode);
			TestProperty (node1sub1sub1Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.LeafNode);

			bridge.ResetEventLists ();
			
			node1.Collapse ();
			
			propertyEventTuple =
				bridge.GetAutomationPropertyEventFrom (node1Provider,
				                                       ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id);
			Assert.IsNotNull (propertyEventTuple,
			                  "Expected property change event on node 1");
			Assert.AreEqual (ExpandCollapseState.Expanded,
			                 propertyEventTuple.e.OldValue,
			                 "Old value");
			Assert.AreEqual (ExpandCollapseState.Collapsed,
			                 propertyEventTuple.e.NewValue,
			                 "New value");

			Assert.AreEqual (ExpandCollapseState.Collapsed,
			                 node1ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.LeafNode,
			                 node2ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.Expanded,
			                 node1sub1ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.LeafNode,
			                 node1sub2ExpandCollapse.ExpandCollapseState);
			Assert.AreEqual (ExpandCollapseState.LeafNode,
			                 node1sub1sub1ExpandCollapse.ExpandCollapseState);

			TestProperty (node1Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.Collapsed);
			TestProperty (node2Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.LeafNode);
			TestProperty (node1sub1Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.Expanded);
			TestProperty (node1sub2Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.LeafNode);
			TestProperty (node1sub1sub1Provider,
			              ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			              ExpandCollapseState.LeafNode);
		}

		[Test]
		public void ExpandCollapseTest ()
		{
			TreeView treeView = new TreeView ();

			TreeNode node1 = new TreeNode ("node1");
			TreeNode node2 = new TreeNode ("node2");
			
			TreeNode node1sub1 = new TreeNode ("node1 - sub1");
			TreeNode node1sub2 = new TreeNode ("node1 - sub2");
			node1.Nodes.Add (node1sub1);
			node1.Nodes.Add (node1sub2);

			TreeNode node1sub1sub1 = new TreeNode ("node1 - sub1 - sub1");
			node1sub1.Nodes.Add (node1sub1sub1);

			treeView.Nodes.AddRange (new TreeNode [] {
				node1,
				node2});

			treeView.CollapseAll ();
			
			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragmentRoot node2Provider = (IRawElementProviderFragmentRoot)
				node1Provider.Navigate (NavigateDirection.NextSibling);
			
//			IRawElementProviderFragmentRoot node1sub1Provider = (IRawElementProviderFragmentRoot)
//				node1Provider.Navigate (NavigateDirection.FirstChild);
//			IRawElementProviderFragmentRoot node1sub2Provider = (IRawElementProviderFragmentRoot)
//				node1sub1Provider.Navigate (NavigateDirection.NextSibling);
			
//			IRawElementProviderFragmentRoot node1sub1sub1Provider = (IRawElementProviderFragmentRoot)
//				node1sub1Provider.Navigate (NavigateDirection.FirstChild);

			IExpandCollapseProvider node1ExpandCollapse = (IExpandCollapseProvider)
				node1Provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);
			IExpandCollapseProvider node2ExpandCollapse = (IExpandCollapseProvider)
				node2Provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);

//			IExpandCollapseProvider node1sub1ExpandCollapse = (IExpandCollapseProvider)
//				node1sub1Provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);
//			IExpandCollapseProvider node1sub2ExpandCollapse = (IExpandCollapseProvider)
//				node1sub2Provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);

//			IExpandCollapseProvider node1sub1sub1ExpandCollapse = (IExpandCollapseProvider)
//				node1sub1sub1Provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);

			Assert.IsFalse (node1.IsExpanded);
			Assert.IsFalse (node2.IsExpanded);
			Assert.IsFalse (node1sub1.IsExpanded);
			Assert.IsFalse (node1sub2.IsExpanded);
			Assert.IsFalse (node1sub1sub1.IsExpanded);

			node1ExpandCollapse.Expand ();

			// TODO: test winforms events?

			Assert.IsTrue (node1.IsExpanded);
			Assert.IsFalse (node2.IsExpanded);
			Assert.IsFalse (node1sub1.IsExpanded);
			Assert.IsFalse (node1sub2.IsExpanded);
			Assert.IsFalse (node1sub1sub1.IsExpanded);

			node1ExpandCollapse.Expand ();

			Assert.IsTrue (node1.IsExpanded);
			Assert.IsFalse (node2.IsExpanded);
			Assert.IsFalse (node1sub1.IsExpanded);
			Assert.IsFalse (node1sub2.IsExpanded);
			Assert.IsFalse (node1sub1sub1.IsExpanded);

			node1ExpandCollapse.Collapse ();

			Assert.IsFalse (node1.IsExpanded);
			Assert.IsFalse (node2.IsExpanded);
			Assert.IsFalse (node1sub1.IsExpanded);
			Assert.IsFalse (node1sub2.IsExpanded);
			Assert.IsFalse (node1sub1sub1.IsExpanded);

			node1ExpandCollapse.Collapse ();

			Assert.IsFalse (node1.IsExpanded);
			Assert.IsFalse (node2.IsExpanded);
			Assert.IsFalse (node1sub1.IsExpanded);
			Assert.IsFalse (node1sub2.IsExpanded);
			Assert.IsFalse (node1sub1sub1.IsExpanded);

			try {
				node2ExpandCollapse.Expand ();
				Assert.Fail ("Expected InvalidOperationException when Expanding a LeafNode");
			} catch (InvalidOperationException) {
				// Expected
			}

			try {
				node2ExpandCollapse.Collapse ();
				Assert.Fail ("Expected InvalidOperationException when Collapsing a LeafNode");
			} catch (InvalidOperationException) {
				// Expected
			}

			treeView.Enabled = false;
			

			try {
				node1ExpandCollapse.Expand ();
				Assert.Fail ("Expected ElementNotEnabledException when Expanding a node in a disabled TreeView");
			} catch (ElementNotEnabledException) {
				// Expected
			}

			try {
				node1ExpandCollapse.Collapse ();
				Assert.Fail ("Expected ElementNotEnabledException when Collapsing a node in a disabled TreeView");
			} catch (ElementNotEnabledException) {
				// Expected
			}
		}
		
		#endregion

		#region IValueProvider Tests

		[Test]
		public void ValueTest ()
		{
			TreeView treeView = new TreeView ();
			treeView.LabelEdit = true;
			TreeNode node1 = treeView.Nodes.Add ("node1");

			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild);

			VerifyTreeNodePatterns (provider, node1Provider, node1);

			IValueProvider node1Value = (IValueProvider)
				node1Provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);

			Assert.AreEqual (node1.Text,
			                 node1Value.Value,
			                 "Value");

			TestProperty (node1Provider,
			              ValuePatternIdentifiers.ValueProperty,
			              node1.Text);

			bridge.ResetEventLists ();

			node1.Text = "modified node1";
			
			AutomationPropertyChangedEventTuple propertyEventTuple =
				bridge.GetAutomationPropertyEventFrom (node1Provider,
				                                       ValuePatternIdentifiers.ValueProperty.Id);
			Assert.IsNotNull (propertyEventTuple,
			                  "Expected property change event on node 1");
			Assert.AreEqual ("node1",
			                 propertyEventTuple.e.OldValue,
			                 "Old value");
			Assert.AreEqual ("modified node1",
			                 propertyEventTuple.e.NewValue,
			                 "New value");

			Assert.AreEqual (node1.Text,
			                 node1Value.Value,
			                 "Value");

			TestProperty (node1Provider,
			              ValuePatternIdentifiers.ValueProperty,
			              node1.Text);
		}

		[Test]
		public void SetValueTest ()
		{
			TreeView treeView = new TreeView ();
			treeView.LabelEdit = true;
			TreeNode node1 = treeView.Nodes.Add ("node1");

			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild);

			VerifyTreeNodePatterns (provider, node1Provider, node1);

			IValueProvider node1Value = (IValueProvider)
				node1Provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);

			string newNodeText = "modified node1";
			node1Value.SetValue (newNodeText);

			Assert.AreEqual (newNodeText,
			                 node1.Text,
			                 "Text should be modified");

			treeView.Enabled = false;

			try {
				node1Value.SetValue ("another new value");
				Assert.Fail ("Expected ElementNotEnabledExcpetion when TreeView disabled");
			} catch (ElementNotEnabledException) {
				// Expected
			}
		}

		[Test]
		public void IsReadOnlyTest ()
		{
			TreeView treeView = new TreeView ();
			treeView.LabelEdit = true;
			TreeNode node1 = treeView.Nodes.Add ("node1");

			IRawElementProviderFragmentRoot provider = (IRawElementProviderFragmentRoot)
				GetProviderFromControl (treeView);

			IRawElementProviderFragmentRoot node1Provider = (IRawElementProviderFragmentRoot)
				provider.Navigate (NavigateDirection.FirstChild);

			VerifyTreeNodePatterns (provider, node1Provider, node1);

			IValueProvider node1Value = (IValueProvider)
				node1Provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);

			Assert.IsFalse (node1Value.IsReadOnly,
			                "Never read-only");

			treeView.Enabled = false;

			Assert.IsFalse (node1Value.IsReadOnly,
			                "Never read-only");
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
				Assert.IsFalse ((bool) expectedNotSelected [i].GetPropertyValue (SelectionItemPatternIdentifiers.IsSelectedProperty.Id),
				                "Item should not be selected");
			}

			for (int i = 0; i < expectedSelection.Length; i++) {
				ISelectionItemProvider item = (ISelectionItemProvider)
					expectedSelection [i].GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
				Assert.IsNotNull (item,
				                  "SelectionItem Pattern support expected");
				Assert.IsTrue (item.IsSelected,
				                "Item should be selected");
				Assert.IsTrue ((bool) expectedSelection [i].GetPropertyValue (SelectionItemPatternIdentifiers.IsSelectedProperty.Id),
				                "Item should be selected");
			}
		}

		private void VerifyTreeNodePatterns (IRawElementProviderSimple parentProvider,
		                                     IRawElementProviderSimple provider,
		                                     TreeNode treeNode)
		{
			bool expectValue = treeNode.TreeView.LabelEdit;
			bool expectInovke = false;
			bool expectSelectionItem = true;
			bool expectScrollItem = treeNode.TreeView.Enabled &&
				(parentProvider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id) as IScrollProvider != null);
			bool expectToggle = treeNode.TreeView.CheckBoxes;
			bool expectExpandCollapse = true;

			object valueProvider =
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			if (expectValue) {
				Assert.IsNotNull (valueProvider,
				                  "Should support Value pattern.");
				Assert.IsTrue (valueProvider is IValueProvider,
				               "Should support Value pattern.");
			} else
				Assert.IsNull (valueProvider,
				               "Should not support Value Pattern.");

			object invokeProvider =
				provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			if (expectInovke) {
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

			object scrollItemProvider =
				provider.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id);
			if (expectScrollItem) {
				Assert.IsNotNull (scrollItemProvider,
				                  "Should support ScrollItem pattern.");
				Assert.IsTrue (scrollItemProvider is IScrollItemProvider,
				               "Should support ScrollItem pattern.");
			} else
				Assert.IsNull (scrollItemProvider,
				               "Should not support ScrollItem Pattern.");

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
		
		#endregion
		
		#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return new TreeView ();
		}

		#endregion
	}
}
