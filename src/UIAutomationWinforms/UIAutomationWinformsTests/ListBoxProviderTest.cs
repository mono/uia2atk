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
	public class ListBoxProviderTest : BaseProviderTest
	{
		
#region Tests
		
		[Test]
		public void BasicPropertiesTest ()
		{
			ListBox listbox = new ListBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (listbox);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.List.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "list");
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsContentElementProperty,
			              true);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsControlElementProperty,
			              true);

			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              true);
			
			//TODO: AutomationElementIdentifiers.BoundingRectangleProperty
			//TODO: AutomationElementIdentifiers.ClickablePointProperty
			//TODO: AutomationElementIdentifiers.IsKeyboardFocusableProperty
			//TODO: AutomationElementIdentifiers.NameProperty
			//TODO: AutomationElementIdentifiers.LabeledByProperty
			//TODO: AutomationElementIdentifiers.HelpTextProperty
		}
		
		[Test]
		public void BasicItemPropertiesTest ()
		{
			ListBox listbox = new ListBox ();
			listbox.Items.Add ("test");
			
			IRawElementProviderFragmentRoot rootProvider;
			ISelectionProvider selectionProvider;
			IRawElementProviderFragment child;
			
			rootProvider = (IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (listbox);
			selectionProvider = (ISelectionProvider) rootProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);
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
		}
		
#endregion
		
#region Navigate Tests
		
		[Test]
		public void NavigateMultipleTest ()
		{
			ListBox listbox = (ListBox) GetControlInstance ();
			IRawElementProviderFragmentRoot rootProvider;
			ISelectionProvider selectionProvider;
			IRawElementProviderFragment firstChild;
			IRawElementProviderFragment child;
			IRawElementProviderFragment childParent;

			listbox.SelectionMode = SelectionMode.MultiSimple;
			
			int elements = 10;
			int index = 0;
			string name = string.Empty;

			for (index = 0; index < elements; index++)
				listbox.Items.Add (string.Format ("Element: {0}", index));
			index = 0;
			
			rootProvider 
				= (IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (listbox);
			selectionProvider 
				= (ISelectionProvider) rootProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);

			//Loop all elements
			child = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (child, "We must have a child");
			
			do {
				childParent = child.Navigate (NavigateDirection.Parent);
				Assert.AreEqual (rootProvider, childParent, 
				                 "Each child must have same parent");
				name = (string) child.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
				Assert.AreEqual (string.Format ("Element: {0}", index++), 
				                 name, "Different names");
				child = child.Navigate (NavigateDirection.NextSibling);
				
			} while (child != null);
			Assert.AreEqual (elements, index, "Elements added = elements navigated");

			//Clear all elements and try again.
			listbox.Items.Clear ();

			child = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (child, "We shouldn't have a child");
		}
		
		[Test]
		public void NavigateSingleTest ()
		{
			ListBox listbox = (ListBox) GetControlInstance ();
			IRawElementProviderFragmentRoot rootProvider;
			IRawElementProviderFragment parent;
			IRawElementProviderFragment firstChild;
			IRawElementProviderFragment secondChild;
			IRawElementProviderFragment thirdChild;
			IRawElementProviderSimple []selection;
			ISelectionProvider selectionProvider;
			
			bridge.ResetEventLists ();
			
			rootProvider = (IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (listbox);
			
			//By default the ListBox doesn't have any selected item, so
			//selection isn't required 
			selectionProvider = (ISelectionProvider) rootProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);
			Assert.AreEqual (false, selectionProvider.IsSelectionRequired, 
			                 "Is false by default");

			selection = selectionProvider.GetSelection ();
			Assert.IsNull (selection, "selection is null");

			firstChild = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (firstChild, "firstChild is null");

			//Once we have a selected item, selection is required.
			listbox.Items.Add (0);
			listbox.SetSelected (0, true);
			Assert.AreEqual (true, selectionProvider.IsSelectionRequired, 
			                 "Is true once an item is selected");			
			
			selection = selectionProvider.GetSelection ();
			Assert.IsNotNull (selection, "selection is not null");		

			//All children have same parent
			firstChild = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (firstChild, "firstChild is not null");
			
			parent = firstChild.Navigate (NavigateDirection.Parent);
			Assert.AreEqual (rootProvider, parent, "Parent - firstChild");

			listbox.Items.Add (1);

			secondChild = firstChild.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (secondChild, "secondChild is not null");
			
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
			
			//ListBox is not enabled to support multiselection
			ISelectionItemProvider selectionItemProvider;
			try {
				selectionItemProvider
					= (ISelectionItemProvider) secondChild.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
				selectionItemProvider.AddToSelection ();
				Assert.Fail ("Should throw InvalidOperationException.");
			} catch (InvalidOperationException) { 
			}
			
			//However we can change selection
			selectionItemProvider
				= (ISelectionItemProvider) secondChild.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			selectionItemProvider.Select ();
			
			//Now should be selected.
			bool firstSelected 
				= (bool) firstChild.GetPropertyValue (SelectionItemPatternIdentifiers.IsSelectedProperty.Id);
			bool secondSelected 
				= (bool) secondChild.GetPropertyValue (SelectionItemPatternIdentifiers.IsSelectedProperty.Id);
			
			Assert.IsFalse (firstSelected, "First should be false");
			Assert.IsTrue (secondSelected, "Second should be true");
			
			//We can't remove from selection once an element is selected
			try {
				selectionItemProvider.RemoveFromSelection ();
				Assert.Fail ("Should throw InvalidOperationException.");
			} catch (InvalidOperationException) {
			}
			
		}
		
#endregion
		
		
#region ScrollBar internal widgets Test

//		[Test]
//		public void ScrollBarTest ()
//		{
//			using (Form f = new Form ()) {
//				f.Size = new System.Drawing.Size (500, 500);
//				
//				ListBox listbox = new ListBox ();
//				listbox.Location = new System.Drawing.Point (3, 3);
//				listbox.Size = new System.Drawing.Size (100, 50);
//
//				f.Controls.Add (listbox);
//
//				//We should show the ScrollBar
//				for (int index = 20; index > 0; index--) {
//					listbox.Items.Add (string.Format ("Element {0}", index));
//				}
//				
//				f.Load += delegate (object sender, EventArgs args) {
//					System.Threading.Thread.Sleep (500);
//					IRawElementProviderFragmentRoot provider;
//					//ISelectionProvider selectionProvider;
//					IRawElementProviderFragment scrollbarChild;
//					IRawElementProviderFragment buttonChild;
//	
//					provider 
//						= (IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (listbox);
//					
//					//Should return a ScrollBar, because we have added many items
//					scrollbarChild = provider.Navigate (NavigateDirection.FirstChild);
//					Assert.IsNotNull (scrollbarChild, "FirstChild must not be null");
//					
//					Assert.AreEqual (ControlType.ScrollBar.Id,
//					                 scrollbarChild.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id),
//					                 "scrollbarChild Control Type must be Scroll.");
//
//					buttonChild = scrollbarChild.Navigate (NavigateDirection.FirstChild);
//					Assert.IsNotNull (buttonChild , "buttonChild must not be null");
//					
//					Assert.AreEqual (ControlType.Button.Id,
//					                 buttonChild.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id),
//					                 "buttonChild Control Type must be Buttoon.");
//					
//					//TODO: Add more tests
//					
//					f.Close ();
//				};
//
//				Application.Run (f);
//			}
//		}
		
#endregion

#region BaseProviderTest Overrides

		protected override Control GetControlInstance ()
		{
			return new ListBox ();
		}
		
#endregion
		
//#region Private Methods
//		
//		private Form GetForm
//		
//#endregion
		
	}
}
