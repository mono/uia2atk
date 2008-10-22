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
	public class CheckedListBoxProviderTest : BaseProviderTest
	{

		#region Tests
		
		[Test]
		public void BasicPropertiesTest ()
		{
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (GetControlInstance ());
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.List.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "list");
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              true);
		}
		
		[Test]
		public void BasicItemPropertiesTest ()
		{
			CheckedListBox listbox = (CheckedListBox) GetControlInstance ();
			listbox.Items.Add ("1");
			
			IRawElementProviderFragmentRoot rootProvider;
			ISelectionProvider selectionProvider;
			IRawElementProviderFragment child;
			IToggleProvider toggleProvider1;
			IToggleProvider toggleProvider2;
			IRawElementProviderFragment child2;			
			
			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (listbox);
			child = rootProvider.Navigate (NavigateDirection.FirstChild);
			
			TestProperty (child,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.ListItem.Id);
			
			TestProperty (child,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "list item");
			
			TestProperty (child,
			              AutomationElementIdentifiers.IsContentElementProperty,
			              true);
			
			TestProperty (child,
			              AutomationElementIdentifiers.IsControlElementProperty,
			              true);

			TestProperty (child,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              true);

			// Should support selection
			selectionProvider = rootProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id) as ISelectionProvider;
			Assert.IsNotNull (selectionProvider, "Selection Pattern in SelectionItem");
			
			// TODO: add more selection-related tests
			
			//Should support Toggle
			toggleProvider1 = child.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id) as IToggleProvider;
			Assert.IsNotNull (toggleProvider1, "Toggle Pattern in SelectionItem");
			
			Assert.AreEqual (toggleProvider1.ToggleState, ToggleState.Off);
			toggleProvider1.Toggle ();
			Assert.AreEqual (toggleProvider1.ToggleState, ToggleState.On);
			toggleProvider1.Toggle ();
			Assert.AreEqual (toggleProvider1.ToggleState, ToggleState.Off);
			
			//By default ListItem supports: SelectionItemPattern and ScrollItemPattern
			ISelectionItemProvider selectionItem =
				child.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id) as ISelectionItemProvider;			
			Assert.IsNotNull (selectionItem, "ListItem should ALWAYS SUPPORT SelectionItem");
			
			IScrollProvider scroll 
				= rootProvider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id) as IScrollProvider;
			
			if (scroll != null) {
				IScrollItemProvider scrollItem =
					child.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id) as IScrollItemProvider;
				Assert.IsNotNull (scrollItem, "ListItem should ALWAYS SUPPORT ScrollItem");
			}
			
			IToggleProvider toggleItem =
				child.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id) as IToggleProvider;
			Assert.IsNotNull (toggleItem, "ListItem show ALWAYS SUPPORT Toggle");
			
			//Add new item
			child2 = child.Navigate (NavigateDirection.NextSibling);
			Assert.IsNull (child2, "Child2 should be NULL");
			
			listbox.Items.Add ("2");
			child2 = child.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (child2, "Child2 is NULL");
			
			toggleProvider2 = (IToggleProvider) child2.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			Assert.AreEqual (toggleProvider2.ToggleState, ToggleState.Off);
			toggleProvider2.Toggle ();
			Assert.AreEqual (toggleProvider2.ToggleState, ToggleState.On);
			toggleProvider2.Toggle ();
			Assert.AreEqual (toggleProvider2.ToggleState, ToggleState.Off);
		}

		#endregion

		#region Pattern Tests

		[Test]
		public void PatternsTest ()
		{
			CheckedListBox checkedListbox = (CheckedListBox) GetControlInstance ();
			

			IRawElementProviderFragmentRoot rootProvider
				= (IRawElementProviderFragmentRoot) GetProviderFromControl (checkedListbox);

			Assert.IsNotNull (rootProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id),
			                  "Selection Pattern IS supported");

			//Lets add a lot of items to show scrollbar
			for (int i = 0; i < 30; i++)
				checkedListbox.Items.Add (string.Format ("dummy {0}", i));

			Assert.IsNotNull (rootProvider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id),
			                  "Scroll Pattern IS supported");
			checkedListbox.Items.Clear ();
			Assert.IsNull (rootProvider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id),
			               "Scroll Pattern IS NOT supported");
		}

		#endregion
		
		#region BaseProviderTest Overrides

		protected override Control GetControlInstance ()
		{
			return new CheckedListBox ();
		}
		
		#endregion
	}
}
