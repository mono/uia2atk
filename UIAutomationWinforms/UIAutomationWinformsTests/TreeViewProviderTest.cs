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

			VerifyTreeNodePatterns (provider, node1Provider, node1);
			VerifyTreeNodePatterns (provider, node2Provider, node2);
			VerifyTreeNodePatterns (provider, node3Provider, node3);

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

			Assert.AreEqual (1, // TODO: When finished, will probably be more than 1! Search instead!
			                 bridge.AutomationPropertyChangedEvents.Count,
			                 "property change event expected");
			AutomationPropertyChangedEventTuple propertyEventTuple =
				bridge.AutomationPropertyChangedEvents [0];
			Assert.AreEqual (node1Provider,
			                 propertyEventTuple.element,
			                 "event sender");
			Assert.AreEqual (SelectionItemPatternIdentifiers.IsSelectedProperty,
			                 propertyEventTuple.e.Property,
			                 "event property");
			Assert.AreEqual (true,
			                 propertyEventTuple.e.NewValue,
			                 "event new value");
			Assert.AreEqual (false,
			                 propertyEventTuple.e.OldValue,
			                 "event old value");

			Assert.AreEqual (1, // TODO: When finished, will probably be more than 1! Search instead!
			                 bridge.AutomationEvents.Count,
			                 "selection change event expected");
			AutomationEventTuple eventTuple =
				bridge.AutomationEvents [0];
			Assert.AreEqual (node1Provider,
			                 eventTuple.provider,
			                 "event sender");
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

			Assert.AreEqual (1, // TODO: When finished, will probably be more than 1! Search instead!
			                 bridge.AutomationPropertyChangedEvents.Count,
			                 "property change event expected");
			propertyEventTuple =
				bridge.AutomationPropertyChangedEvents [0];
			Assert.AreEqual (node2Provider,
			                 propertyEventTuple.element,
			                 "event sender");
			Assert.AreEqual (SelectionItemPatternIdentifiers.IsSelectedProperty,
			                 propertyEventTuple.e.Property,
			                 "event property");
			Assert.AreEqual (true,
			                 propertyEventTuple.e.NewValue,
			                 "event new value");
			Assert.AreEqual (false,
			                 propertyEventTuple.e.OldValue,
			                 "event old value");

			Assert.AreEqual (1, // TODO: When finished, will probably be more than 1! Search instead!
			                 bridge.AutomationEvents.Count,
			                 "selection change event expected");
			eventTuple =
				bridge.AutomationEvents [0];
			Assert.AreEqual (node2Provider,
			                 eventTuple.provider,
			                 "event sender");
			Assert.AreEqual (SelectionItemPatternIdentifiers.ElementSelectedEvent,
			                 eventTuple.e.EventId,
			                 "event id");
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
			} catch {
				Assert.Fail ("Expected InvalidOperationException when AddToSelection is called and there is already a selected node");
			}

			treeView.SelectedNode = null;
			
			node2Selection.AddToSelection ();
			Assert.AreEqual (node2,
			                 treeView.SelectedNode,
			                 "AddToSelection with no SelectedNode should work");
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
			} catch {
				Assert.Fail ("Expected InvalidOperationException when RemoveFromSelection is called on a selected node");
			}

			Assert.IsTrue (node1Selection.IsSelected,
			               "RemoveToSelection should not have changed selection");

			// Should have no effect
			node2Selection.RemoveFromSelection ();
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

		#region IScrollProvider Tests

		
		
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

			node1.Checked = true;

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

			Assert.AreEqual (true, node1.Checked);
			Assert.AreEqual (false, node2.Checked);
			Assert.AreEqual (false, node3.Checked);

			node1Toggle.Toggle ();
			node3Toggle.Toggle ();

			Assert.AreEqual (false, node1.Checked);
			Assert.AreEqual (false, node2.Checked);
			Assert.AreEqual (true, node3.Checked);

			node2Toggle.Toggle ();

			Assert.AreEqual (false, node1.Checked);
			Assert.AreEqual (true, node2.Checked);
			Assert.AreEqual (true, node3.Checked);
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
			bool expectInovke = false;
			bool expectSelectionItem = true;
			bool expectScrollItem =
				(parentProvider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id) as IScrollProvider != null);
			bool expectToggle = treeNode.TreeView.CheckBoxes;

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
