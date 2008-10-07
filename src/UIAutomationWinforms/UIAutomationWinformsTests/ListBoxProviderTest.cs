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

			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (listbox);

			selectionProvider = rootProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id) as ISelectionProvider;
			Assert.IsNotNull (selectionProvider, "Selection Provider for ListBox");

			child = rootProvider.Navigate (NavigateDirection.FirstChild);
			
			TestProperty (child,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.ListItem.Id);
			
			TestProperty (child,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "list item");

			TestProperty (child,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              true);

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
			Assert.IsNull (toggleItem, "ListItem SHOULD NOT SUPPORT Toggletlge");

			bridge.ResetEventLists ();
			selectionItem.AddToSelection ();
			
			//Testing events.
			
			Assert.AreEqual (1,
			                bridge.GetAutomationPropertyEventCount (SelectionItemPatternIdentifiers.IsSelectedProperty),
			                "SelectionItemPatternIdentifiers.IsSelectedProperty");			
			
			Assert.AreEqual (1,
			                bridge.GetAutomationEventCount (SelectionItemPatternIdentifiers.ElementSelectedEvent),
			                "SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent");
			
			bridge.ResetEventLists ();
			selectionItem.RemoveFromSelection ();

			Assert.AreEqual (1,
			                bridge.GetAutomationPropertyEventCount (SelectionItemPatternIdentifiers.IsSelectedProperty),
			                "SelectionItemPatternIdentifiers.IsSelectedProperty");			
			
			Assert.AreEqual (0,
			                bridge.GetAutomationEventCount (SelectionItemPatternIdentifiers.ElementSelectedEvent),
			                "SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent");
			
			bridge.ResetEventLists ();
			//Lets add a new item in listbox
			
			listbox.SelectionMode = SelectionMode.MultiSimple;
			listbox.Items.Add ("test 2");
			
			selectionItem.AddToSelection ();
			
			//Get 2nd child
			IRawElementProviderFragment child2nd = child.Navigate (NavigateDirection.NextSibling);
			
			ISelectionItemProvider selectionItem2
				= child2nd.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id) as ISelectionItemProvider;			
			Assert.IsNotNull (selectionItem2, "ListItem should ALWAYS SUPPORT SelectionItem");
			
			selectionItem2.AddToSelection ();

			Assert.AreEqual (1,
			                bridge.GetAutomationEventCount (SelectionItemPatternIdentifiers.ElementSelectedEvent),
			                 "SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent");
			Assert.AreEqual (2,
			                bridge.GetAutomationPropertyEventCount (SelectionItemPatternIdentifiers.IsSelectedProperty),
			                 "SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent");
			Assert.AreEqual (1,
			                bridge.GetAutomationEventCount (SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent),
			                 "SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent");
		}
		
		#endregion
		
		#region Navigate Tests
		
		[Test]
		public void NavigateMultipleTest ()
		{
			ListBox listbox = (ListBox) GetControlInstance ();
			listbox.Size = new System.Drawing.Size (300, 300);

			IRawElementProviderFragmentRoot rootProvider;
			IRawElementProviderFragment child;
			IRawElementProviderFragment childParent;

			listbox.SelectionMode = SelectionMode.MultiSimple;
			
			int elements = 10;
			int index = 0;
			string name = string.Empty;

			for (index = 0; index < elements; index++)
				listbox.Items.Add (string.Format ("Element: {0}", index));
			index = 0;
			
			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (listbox);

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
			
			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (listbox);
			
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
			Assert.AreEqual (false, selectionProvider.IsSelectionRequired, 
			                 "Shouldn't change");
			
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
			} catch (InvalidOperationException) {
				Assert.Fail ("Shouldn't throw InvalidOperationException.");
			}
			
		}
		
		#endregion

		#region ScrollBar internal widgets Test

		[Test]
		public void ScrollBarTest ()
		{
			IRawElementProviderFragmentRoot provider;
			IRawElementProviderFragment parent = null;
			IRawElementProviderFragment scrollBar = null;
			IRawElementProviderFragment child = null;
			
			ListBox listbox = (ListBox) GetControlInstance ();
			listbox.Location = new System.Drawing.Point (3, 3);
			listbox.Size = new System.Drawing.Size (100, 50);
			provider = (IRawElementProviderFragmentRoot) GetProviderFromControl (listbox);
			
			//We should show the ScrollBar
			for (int index = 20; index > 0; index--) {
				listbox.Items.Add (string.Format ("Element {0}", index));
			}
			
			//WE DON'T KNOW THE ORDER OF NAVIGATION
			//However we know that we have "somewhere" one Vertical Scrollbar
			
			bool scrollbarFound = false;
			
			child = provider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (child, "FirstChild must not be null");
			while (child != null) {
				
				if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) == ControlType.ScrollBar.Id
					&& (OrientationType) child.GetPropertyValue (AutomationElementIdentifiers.OrientationProperty.Id) == OrientationType.Vertical) {
					scrollbarFound = true;
					scrollBar = child;
				}
				
				//ALL elements must have same parent
				parent = child.Navigate (NavigateDirection.Parent);
				Assert.AreEqual (provider, parent, "Parents are different");
				child = child.Navigate (NavigateDirection.NextSibling);
			}
			
			//ScrollBar Tests
			
			Assert.IsTrue (scrollbarFound, "ScrollBar not found");
			
			int children = 0;
			
			child = scrollBar.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				parent = child.Navigate (NavigateDirection.Parent);
				Assert.AreEqual (scrollBar, parent, "ScrollBar. Parents are different");
				child = child.Navigate (NavigateDirection.NextSibling);
				children++;
			}
			
			Assert.AreEqual (5, children, "ScrollBar's children must be 5");
			
			
			//Related to bug: https://bugzilla.novell.com/show_bug.cgi?id=414937
			//Get IScrollProvider pattern
			IScrollProvider scrollProvider 
				= (IScrollProvider) provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (scrollProvider, "We should have a ScrollProvider in ListBox");
			
			//We only have one vscrollbar, so using the provider shouldn't
			//crash because of missing hscrollbar
			Assert.IsFalse (scrollProvider.HorizontallyScrollable, "We shoudln't have hscrollbar");
			Assert.IsTrue (scrollProvider.VerticallyScrollable, "We should have hscrollbar");
			
			//Let's move scrollbar
			scrollProvider.Scroll (ScrollAmount.LargeIncrement,
			                       ScrollAmount.LargeIncrement);
			
			if (scrollProvider.VerticalScrollPercent == 0)
				Assert.Fail ("Vertical scroll should move");
//			if (scrollProvider.HorizontalScrollPercent != 100)
//				Assert.Fail ("Vertical scroll shouldn't move");
			
			//EOF-Bug

			//Clearing elements should hide scroll and delete all items.
			listbox.Items.Clear ();

			child = provider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (child, "Shouldn't be any children");

			//This won't add scrollbar
			listbox.Items.Add (1);
			listbox.Items.Add (2);
			child = provider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (child, "Should be a child");

			//Children MUST BE ListItem
			while (child != null) {
				Assert.AreEqual (ControlType.ListItem.Id,
				                 child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id),
				                 "Item should be ListItem");
				child = child.Navigate (NavigateDirection.NextSibling);
			}
		}
		
		#endregion

		#region BaseProviderTest Overrides

		protected override Control GetControlInstance ()
		{
			return new ListBox ();
		}
		
		#endregion
	
	}
}
