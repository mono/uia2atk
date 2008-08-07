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
			ComboBox combobox = new ComboBox ();
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
			              AutomationElementIdentifiers.IsContentElementProperty,
			              true);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsControlElementProperty,
			              true);
			              
			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              true);			              
		}

		#endregion
		
		#region Navigation Test
		
		[Test]
		public void NavigateSingleTest ()
		{
			ComboBox combobox = (ComboBox) GetControlInstance ();
			
			IRawElementProviderFragmentRoot rootProvider;
			IRawElementProviderFragment parent;
			IRawElementProviderFragment firstChild;
			IRawElementProviderFragment secondChild;
			IRawElementProviderFragment thirdChild;
			IRawElementProviderFragment firstItem;
			IRawElementProviderFragment secondItem;
			IRawElementProviderFragment thirdItem;
			IRawElementProviderFragment prevItem;
			IRawElementProviderSimple []selection;
			ISelectionProvider selectionProvider;

			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (combobox);
			
			//By default the ComboBox doesn't have any selected item, so
			//selection isn't required
			selectionProvider = (ISelectionProvider) rootProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);
			Assert.AreEqual (false, selectionProvider.IsSelectionRequired, 
			                 "Is false by default");

			selection = selectionProvider.GetSelection ();
			Assert.IsNull (selection, "selection is null");
			
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

			//TODO: Add more tests
		}
		
		#endregion
		
		#region BaseProviderTest Overrides

		protected override Control GetControlInstance ()
		{
			return new ComboBox ();
		}
		
		#endregion
	}
}
