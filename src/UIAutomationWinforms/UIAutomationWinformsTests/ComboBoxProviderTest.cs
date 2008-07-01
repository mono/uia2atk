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
			IRawElementProviderSimple []selection;
			ISelectionProvider selectionProvider;

			rootProvider = (IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (combobox);
//			
//			//By default the ComboBox doesn't have any selected item, so
//			//selection isn't required 
			selectionProvider = (ISelectionProvider) rootProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);
			Assert.AreEqual (false, selectionProvider.IsSelectionRequired, 
			                 "Is false by default");

			selection = selectionProvider.GetSelection ();
			Assert.IsNull (selection, "selection is null");

			firstChild = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (firstChild, "firstChild is null");

			//Once we have a selected item, selection is required.
			combobox.Items.Add (0);
			combobox.SelectedIndex = 0;
			Assert.AreEqual (true, selectionProvider.IsSelectionRequired, 
			                 "Is true once an item is selected");			

			selection = selectionProvider.GetSelection ();
			Assert.IsNotNull (selection, "selection is not null");		

			//All children have same parent
			firstChild = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (firstChild, "firstChild is not null");
			
			parent = firstChild.Navigate (NavigateDirection.Parent);
			Assert.AreEqual (rootProvider, parent, "Parent - firstChild");

			combobox.Items.Add (1);

			secondChild = firstChild.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (secondChild, "secondChild is null");
			
			parent = secondChild .Navigate (NavigateDirection.Parent);
			Assert.AreEqual (rootProvider, parent, "Parent - secondChild");
			
			//We only have 2 items, there's no third child.
			thirdChild = secondChild.Navigate (NavigateDirection.NextSibling);
			Assert.IsNull (thirdChild, "thirdChild is null");
			
			//Lest navigate from second to first
			Assert.AreEqual (firstChild,
			                 secondChild.Navigate (NavigateDirection.PreviousSibling),
			                 "secondChild.Navigate (PreviousSibling)");
			
			//secondChild is the last child
			Assert.AreEqual (secondChild,
			                 rootProvider.Navigate (NavigateDirection.LastChild),
			                 "rootProvider.Navigate (LastChild)");
			
			//ComboBox doesn't support support multiselection
			ISelectionItemProvider selectionItemProvider;
			try {
				selectionItemProvider
					= (ISelectionItemProvider) secondChild.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
				selectionItemProvider.AddToSelection ();
				Assert.Fail ("Should throw InvalidOperationException.");
			} catch (InvalidOperationException) { 
			}
//			
//			//However we can change selection
//			selectionItemProvider
//				= (ISelectionItemProvider) secondChild.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
//			selectionItemProvider.Select ();
//			
//			//Now should be selected.
//			bool firstSelected 
//				= (bool) firstChild.GetPropertyValue (SelectionItemPatternIdentifiers.IsSelectedProperty.Id);
//			bool secondSelected 
//				= (bool) secondChild.GetPropertyValue (SelectionItemPatternIdentifiers.IsSelectedProperty.Id);
//			
//			Assert.IsFalse (firstSelected, "First should be false");
//			Assert.IsTrue (secondSelected, "Second should be true");
//			
//			//We can't remove from selection once an element is selected
//			try {
//				selectionItemProvider.RemoveFromSelection ();
//				Assert.Fail ("Should throw InvalidOperationException.");
//			} catch (InvalidOperationException) {
//			}
//			
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
