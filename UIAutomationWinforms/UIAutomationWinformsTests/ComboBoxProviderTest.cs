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
//	Mario Carrion <mcarrion@novell.com>
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
	public class ComboBoxProviderTest : BaseProviderTest
	{
	
		#region Tests
		
		[Test]
		public void BasicPropertiesTest ()
		{
			ComboBox combobox = GetComboBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (combobox);
			
			//TODO: Test. AutomationElementIdentifiers.LabeledByProperty
			//TODO: Test. AutomationElementIdentifiers.NameProperty
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.ComboBox.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "combo box");
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              true);			              
		}

		#endregion
		
		#region Navigation Test
		
		[Test]
		public void NavigateSingleTest ()
		{
			ComboBox combobox = GetComboBox ();
			
			IRawElementProviderFragmentRoot rootProvider;
			IRawElementProviderFragment parent;
			IRawElementProviderFragment firstItem;
			IRawElementProviderFragment secondItem;
			IRawElementProviderFragment thirdItem;
			IRawElementProviderSimple []selection;
			ISelectionProvider selectionProvider;

			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (combobox);
			
			//By default the ComboBox doesn't have any selected item, so
			//selection isn't required
			selectionProvider = (ISelectionProvider) rootProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);
			Assert.AreEqual (false, selectionProvider.IsSelectionRequired, 
			                 "Is false by default");

			selection = selectionProvider.GetSelection ();
			Assert.AreEqual (0, selection.Length, "no selected items");
			
			//Testin children
			
			bool withList = false;
			bool withEdit = false;
			bool withButton = false;
			IRawElementProviderFragment child;
			IRawElementProviderFragment editChild = null;
			IRawElementProviderFragment listChild = null;
			IRawElementProviderFragment buttonChild = null;
			
			child = rootProvider.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.Edit.Id) {
					editChild = child;
					withEdit = true;
				} else if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.List.Id) {
					listChild = child;
					withList = true;
				} else if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.Button.Id) {
					buttonChild = child;
					withButton = true;
				}
				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsTrue (withEdit, "We should have a Edit Control Type.");
			Assert.IsTrue (withList, "We should have a List Control Type.");
			Assert.IsTrue (withButton, "We should have a Button Control Type.");
					
			//No items in internal collection
			firstItem = listChild.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (firstItem, "FirstChild must be null");
			
			//Once we have a selected item, selection is required.
			combobox.Items.Add ("0");
			combobox.SelectedIndex = 0;
			Assert.AreEqual (true, selectionProvider.IsSelectionRequired, 
			                 "Is true once an item is selected");

			selection = selectionProvider.GetSelection ();
			Assert.IsNotNull (selection, "selection is not null");

			firstItem = listChild.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (firstItem, "FirstChild must not be null");
			Assert.AreEqual (
			  firstItem.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id), ControlType.ListItem.Id,
			  "a combobox item is listitem");

			TestHelper.TestPatterns ("ListItem of ComboBox",
			  firstItem, SelectionItemPatternIdentifiers.Pattern);

			secondItem = firstItem.Navigate (NavigateDirection.NextSibling);
			Assert.IsNull (secondItem, "There isn't a second child");

			combobox.Items.Add ("1");
			secondItem = firstItem.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (secondItem, "There is a second child");
			
			parent = firstItem.Navigate (NavigateDirection.Parent);
			Assert.AreEqual (listChild, parent, "Parent - FirstItem");

			parent = secondItem.Navigate (NavigateDirection.Parent);
			Assert.AreEqual (listChild, parent, "Parent - SecondItem");

			thirdItem = secondItem.Navigate (NavigateDirection.NextSibling);
			Assert.IsNull (thirdItem, "There is no third item");

			//Lest navigate from second to first
			Assert.AreEqual (firstItem,
			                 secondItem.Navigate (NavigateDirection.PreviousSibling),
			                 "secondChild.Navigate (PreviousSibling)");
			
			//Remove second
			combobox.Items.RemoveAt (1);
			secondItem = firstItem.Navigate (NavigateDirection.NextSibling);
			Assert.IsNull (secondItem, "There isn't a second child");			
			
			firstItem = listChild.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (firstItem, "FirstChild must not be null");
			
			//Validate parents
			
			Assert.AreEqual (rootProvider,
			                 listChild.Navigate (NavigateDirection.Parent),
			                 "Different parent. LIST");
			
			Assert.AreEqual (rootProvider,
			                 editChild.Navigate (NavigateDirection.Parent),
			                 "Different parent. EDIT");
			
			Assert.AreEqual (rootProvider,
			                 buttonChild.Navigate (NavigateDirection.Parent),
			                 "Different parent. BUTTON");
			

			//TODO: Add more tests
		}
		
		#endregion

		#region Collection tests

		[Test]
		public void CollectionTest ()
		{
			ComboBox combobox = GetComboBox ();

			IRawElementProviderFragmentRoot rootProvider
				= (IRawElementProviderFragmentRoot) GetProviderFromControl (combobox);

			IRawElementProviderFragment list 
				= rootProvider.Navigate (NavigateDirection.FirstChild);
			while (list != null) {
				if ((int) list.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.List.Id)
					break;
				    
				list = list.Navigate (NavigateDirection.NextSibling);
			}

			Assert.IsNotNull (list, "We must have a List");

			IRawElementProviderFragment item0
				= list.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (item0, "We shouldn't have children in List");

			//Add item 0. (Items.Add)
			
			bridge.ResetEventLists ();
			
			combobox.Items.Add ("Item 0");
			StructureChangedEventTuple tuple
				= bridge.GetStructureChangedEventFrom (StructureChangeType.ChildAdded);
			Assert.IsNotNull (tuple, "We should have tuple");
			Assert.AreEqual (((IRawElementProviderFragment) tuple.provider).FragmentRoot,
			                 list, 
			                 "Added item.FragmentRoot != list");
			item0 = list.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (item0, "We should have children in List");

			//Add item 1. (Items.Add)

			bridge.ResetEventLists ();
			combobox.Items.Add ("Item 1");
			tuple = bridge.GetStructureChangedEventFrom (StructureChangeType.ChildAdded);
			Assert.IsNotNull (tuple, "We should have tuple");
			Assert.AreEqual (((IRawElementProviderFragment) tuple.provider).FragmentRoot,
			                 list,
			                 "Added item.FragmentRoot != list");
			IRawElementProviderFragment item1
				= item0.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (item1, "We should have children in List");

			Assert.AreEqual (item1.Navigate (NavigateDirection.PreviousSibling), 
			                 item0, 
			                 "Navigation invalid: item1 -> item0");
			Assert.AreEqual (item0.Navigate (NavigateDirection.NextSibling), 
			                 item1, 
			                 "Navigation invalid: item1 <- item0");

			//Remove item 0 (Items.RemoveAt)

			bridge.ResetEventLists ();
			combobox.Items.RemoveAt (0);

			tuple = bridge.GetStructureChangedEventFrom (StructureChangeType.ChildRemoved);
			Assert.IsNotNull (tuple, "We should have tuple.");
			Assert.AreEqual (((IRawElementProviderFragment) tuple.provider).FragmentRoot,
			                 list, 
			                 "Removed item.FragmentRoot != list");
			Assert.IsNotNull (list.Navigate (NavigateDirection.FirstChild),
			                  "We should STILL have children in List");
			Assert.AreEqual (list.Navigate (NavigateDirection.FirstChild), 
			                 item1, 
			                 "Navigation invalid: parent.FirstChild != item1");

			//Add item 2 y 3 (Items.AddRange)

			bridge.ResetEventLists ();
			combobox.Items.AddRange (new object [] { "Item 2", "Item 3"});

			Assert.AreEqual (2,
			                 bridge.GetStructureChangedEventCount (StructureChangeType.ChildAdded),
			                 "We should have two events");
			StructureChangedEventTuple tuple0 = bridge.GetStructureChangedEventAt (0, 
			                                                                       StructureChangeType.ChildAdded);
			Assert.IsNotNull (tuple0, "We should have tuple0.");
			StructureChangedEventTuple tuple1 = bridge.GetStructureChangedEventAt (1, 
			                                                                       StructureChangeType.ChildAdded);
			Assert.IsNotNull (tuple1, "We should have tuple1.");
			Assert.AreEqual (((IRawElementProviderFragment) tuple0.provider).FragmentRoot,
			                 list, 
			                 "Added item2.FragmentRoot != list");
			Assert.AreEqual (((IRawElementProviderFragment) tuple1.provider).FragmentRoot,
			                 list, 
			                 "Added item3.FragmentRoot != list");
			
			IRawElementProviderFragment item2
				= item1.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (item2, "We should have children in List");
			IRawElementProviderFragment item3
				= item2.Navigate (NavigateDirection.NextSibling);

			Assert.AreEqual (item3.Navigate (NavigateDirection.PreviousSibling), 
			                 item2, 
			                 "Navigation invalid: item2 -> item3");
			Assert.AreEqual (item2.Navigate (NavigateDirection.NextSibling), 
			                 item3, 
			                 "Navigation invalid: item2 <- item3");

			// We have: "Item1", "Item2" "Item3". Lets replace "Item2" with "Item4"
			bridge.ResetEventLists ();

			combobox.Items [1] = "Item4";

			Assert.AreEqual (1,
			                 bridge.GetStructureChangedEventCount (StructureChangeType.ChildRemoved),
			                 "We should have 1 event");
			Assert.AreEqual (1,
			                 bridge.GetStructureChangedEventCount (StructureChangeType.ChildAdded),
			                 "We should have 1 event");
			StructureChangedEventTuple tupleReplacedRemoved
				= bridge.GetStructureChangedEventAt (0,
				                                     StructureChangeType.ChildRemoved);
			Assert.IsNotNull (tupleReplacedRemoved, "Replaced: Removed tupple shouldn't be null");
			Assert.AreEqual (tupleReplacedRemoved.provider,
			                 item2,
			                 "Removed item should be Item2");
			StructureChangedEventTuple tupleReplacedAdded
				= bridge.GetStructureChangedEventAt (0,
				                                     StructureChangeType.ChildAdded);
			Assert.IsNotNull (tupleReplacedAdded, "Replaced: Added tupple shouldn't be null");
			Assert.AreEqual (tupleReplacedAdded.provider, //Notice that elements ARE NOT SORTED
			                 item3.Navigate (NavigateDirection.NextSibling),
			                 "Navigation failed: item3 -> item4");
			Assert.AreEqual (((IRawElementProviderFragment) tupleReplacedAdded.provider).Navigate (NavigateDirection.PreviousSibling),
			                 item3,
			                 "Navigation failed: item3 <- item4");

			// Lets clear all!

			bridge.ResetEventLists ();
			combobox.Items.Clear ();

			Assert.AreEqual (1,
			                 bridge.GetStructureChangedEventCount (StructureChangeType.ChildrenBulkRemoved),
			                 "We should have 1 event");
			Assert.IsNull (list.Navigate (NavigateDirection.FirstChild),
			               "We shouldn't have children in List");
		}
		
		#endregion

		#region Pattern Tests

		[Test]
		public void PatternsTest ()
		{
			ComboBox combobox = GetComboBox ();
			combobox.Items.Add ("dummy 0");

			IRawElementProviderFragmentRoot rootProvider
				= (IRawElementProviderFragmentRoot) GetProviderFromControl (combobox);

			if (combobox.DropDownStyle == ComboBoxStyle.DropDownList)
				throw new Exception ("combobox default style should not be dropdownlist");

			bridge.ResetEventLists ();
			combobox.DropDownStyle = ComboBoxStyle.DropDownList;
			Assert.IsTrue (bridge.StructureChangedEvents.Count > 0,
			               "Should generate some event after changing to ComboBoxStyle.DropDownList");
			Assert.IsNotNull (rootProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id),
			                  "DropDownList: Selection Pattern IS supported");
			Assert.IsNotNull (rootProvider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id),
			                  "DropDownList: ExpandCollapse Pattern IS supported");
			Assert.IsNull (rootProvider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			               "DropDownList: ValuePattern Pattern IS NOT supported");

			bridge.ResetEventLists ();
			combobox.DropDownStyle = ComboBoxStyle.Simple;
			Assert.IsTrue (bridge.StructureChangedEvents.Count > 0,
			               "Should generate some event after changing to ComboBoxStyle.Simple");
			Assert.IsNotNull (rootProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id),
			                  "Simple: Selection Pattern IS supported");
			Assert.IsNull (rootProvider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id),
			               "Simple: ExpandCollapse Pattern IS NOT supported");
			Assert.IsNotNull (rootProvider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			                  "Simple: ValuePattern Pattern IS supported");

			bridge.ResetEventLists ();
			combobox.DropDownStyle = ComboBoxStyle.DropDown;
			Assert.IsTrue (bridge.StructureChangedEvents.Count > 0,
			               "Should generate some event after changing to ComboBoxStyle.DropDown");
			Assert.IsNotNull (rootProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id),
			                  "DropDown: Selection Pattern IS supported");
			Assert.IsNotNull (rootProvider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id),
			                  "DropDown: ExpandCollapse Pattern IS supported");
			Assert.IsNotNull (rootProvider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			                  "DropDown: ValuePattern Pattern IS supported");
		}


		#endregion

		#region Protected Methods

		protected virtual ComboBox GetComboBox ()
		{
			return new ComboBox ();
		}

		#endregion
		
		#region BaseProviderTest Overrides

		protected override Control GetControlInstance ()
		{
			return GetComboBox ();
		}
		
		#endregion
	}
}
