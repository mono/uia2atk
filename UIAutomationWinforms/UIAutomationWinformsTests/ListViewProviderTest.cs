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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	
	[TestFixture]
	public class ListViewProviderTest : BaseProviderTest
	{
		
		#region Tests
		
		[Test]
		public void BasicPropertiesTest ()
		{
			ListView listView = GetControlInstance () as ListView;

			IRawElementProviderFragment listViewProvider 
				= (IRawElementProviderFragment) GetProviderFromControl (listView);
			
			TestProperty (listViewProvider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.List.Id);
			
			TestProperty (listViewProvider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "list");

			TestProperty (listViewProvider,
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
		public void ViewChangesPatternsTest ()
		{
			ListView listview = GetListView (3, 10, 5, 4);
			IRawElementProviderFragment provider 
				= (IRawElementProviderFragment) GetProviderFromControl (listview);

			foreach (View view in Enum.GetValues (typeof (View))) {
				listview.View = view;
				TestPatterns (provider);
				TestChildPatterns (provider);
				
				listview.ShowGroups = true;
				TestPatterns (provider);
				TestChildPatterns (provider);

				listview.ShowGroups = false;
				TestPatterns (provider);
				TestChildPatterns (provider);
			}
		}

		[Test]
		public void HeaderCreationTest ()
		{
			// Tests https://bugzilla.novell.com/show_bug.cgi?id=462302
			
			ListView view = new ListView ();
			view.View = View.Details;
			view.ShowGroups = true;
			
			IRawElementProviderFragmentRoot viewProvider
				= (IRawElementProviderFragmentRoot) GetProviderFromControl (view);
			IRawElementProviderFragment child = viewProvider.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragment header = null;

			while (child != null) {
				if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.Header.Id) {
					header = child;
					break;
				}
				child = child.Navigate (NavigateDirection.NextSibling);
			}

			Assert.IsNotNull (header, "We didn't find a Header");

			// Testing columns
			Assert.IsNull (header.Navigate (NavigateDirection.FirstChild), "We don't have columns yet");

			view.Columns.Add (new ColumnHeader (string.Format ("column {0}", view.Columns.Count)));
			Assert.IsNotNull (header.Navigate (NavigateDirection.FirstChild), "We must have 1 column.");

			view.Items.Add ("Item 0");
			// So now we have an item and one column

			IRawElementProviderFragment item0Provider = viewProvider.Navigate (NavigateDirection.FirstChild);
			while (item0Provider != null) {
				if ((int) item0Provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.DataItem.Id)
					break;
				item0Provider = item0Provider.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (item0Provider, "Item0 Provider must not be null.");

			view.Items [0].SubItems.Add ("subitem 0");

			bridge.ResetEventLists ();
			view.Columns.Add ("Column 0");

			IRawElementProviderFragment item0EditProvider = null;

			// We are doing this because when adding a column two children are 
			// added: HeaderItem and ListItemEdit
			foreach (StructureChangedEventTuple tuple in bridge.StructureChangedEvents) {
				if (tuple.e.StructureChangeType == StructureChangeType.ChildAdded
				    && ((IRawElementProviderFragment) tuple.provider).Navigate (NavigateDirection.Parent) == item0Provider) {
					item0EditProvider = (IRawElementProviderFragment) tuple.provider;
					break; 
				}
			}
			Assert.IsNotNull (item0EditProvider, "Sub Item Added");

			AutomationPropertyChangedEventTuple columnHeaderItemsTuple 
				= bridge.GetAutomationPropertyEventFrom (item0Provider, TableItemPatternIdentifiers.ColumnHeaderItemsProperty.Id);
			Assert.IsNotNull (columnHeaderItemsTuple, "ColumnHeaderItemsProperty event missing");

			// Adding another column to test item0EditProvider
			bridge.ResetEventLists ();
			view.Columns.Add ("Column 1");

			columnHeaderItemsTuple 
				= bridge.GetAutomationPropertyEventFrom (item0EditProvider, TableItemPatternIdentifiers.ColumnHeaderItemsProperty.Id);
			Assert.IsNotNull (columnHeaderItemsTuple, "ColumnHeaderItemsProperty event missing (Edit)");
		}
		
		#endregion


		#region MutipleView Pattern

		[Test]
		public void MutipleViewPattern_SupportedTest ()
		{
			ListView listView = GetControlInstance () as ListView;
			IRawElementProviderFragment listViewProvider = null;
			
			listView.View = View.LargeIcon;
			listViewProvider = (IRawElementProviderFragment) GetProviderFromControl (listView);
			Assert.IsNotNull (listViewProvider.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id),
			                  string.Format ("MultipleView Pattern IS ALWAYS SUPPORTED: View {0}", listView.View));

			listView.View = View.SmallIcon;
			listViewProvider = (IRawElementProviderFragment) GetProviderFromControl (listView);
			Assert.IsNotNull (listViewProvider.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id),
			                  string.Format ("MultipleView Pattern IS ALWAYS SUPPORTED: View {0}", listView.View));

			listView.View = View.Details;
			listViewProvider = (IRawElementProviderFragment) GetProviderFromControl (listView);
			Assert.IsNotNull (listViewProvider.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id),
			                  string.Format ("MultipleView Pattern IS ALWAYS SUPPORTED: View {0}", listView.View));

			listView.View = View.Tile;
			listViewProvider = (IRawElementProviderFragment) GetProviderFromControl (listView);
			Assert.IsNotNull (listViewProvider.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id),
			                  string.Format ("MultipleView Pattern IS ALWAYS SUPPORTED: View {0}", listView.View));

			listView.View = View.List;
			listViewProvider = (IRawElementProviderFragment) GetProviderFromControl (listView);
			Assert.IsNotNull (listViewProvider.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id),
			                  string.Format ("MultipleView Pattern IS ALWAYS SUPPORTED: View {0}", listView.View));
		}

		[Test]
		public void MultipleView_GetSupportedViewsTest ()
		{
			ListView listView = GetControlInstance () as ListView;
			int []supportedViews = null;
			IRawElementProviderFragment listViewProvider = null;
			IMultipleViewProvider pattern;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				listView.View = viewVal;
				listViewProvider = (IRawElementProviderFragment) GetProviderFromControl (listView);
				Assert.IsNotNull (listViewProvider, "Provider not implemented for ListView");
				
				pattern = listViewProvider.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id) as IMultipleViewProvider;
				Assert.IsNotNull (pattern, string.Format ("MultipleView Pattern SHOULD NOT be null {0}", listView.View));
				
				supportedViews = pattern.GetSupportedViews ();
				Assert.AreEqual (1, supportedViews.Length, string.Format ("GetSupportedViews Length: {0}", viewVal));
				Assert.AreEqual (0, supportedViews [0], string.Format ("GetSupportedViews Value ", viewVal));
			}
		}

		[Test]
		public void MultipleView_CurrentViewTest ()
		{
			ListView listView = GetControlInstance () as ListView;
			IRawElementProviderFragment listViewProvider;
			IMultipleViewProvider pattern;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				listView.View = viewVal;
				listViewProvider = (IRawElementProviderFragment) GetProviderFromControl (listView);
				Assert.IsNotNull (listViewProvider, "Provider not implemented for ListView");

				pattern = listViewProvider.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id) as IMultipleViewProvider;
				Assert.IsNotNull (pattern, string.Format ("MultipleView Pattern SHOULD NOT be null {0}", listView.View));

				Assert.AreEqual (0, pattern.CurrentView, string.Format ("CurrentView Value = 0 -> {0}", listView.View));
			}
		}

		[Test]
		public void MultipleView_ViewNameTest ()
		{
		    ListView listView = GetControlInstance () as ListView;
		    IRawElementProviderFragment listViewProvider;
		    IMultipleViewProvider pattern;
			int []supportedViews = null;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				listView.View = viewVal;
				listViewProvider = (IRawElementProviderFragment) GetProviderFromControl (listView);
				Assert.IsNotNull (listViewProvider, "Provider not implemented for ListView");
				
				pattern = listViewProvider.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id) as IMultipleViewProvider;
				Assert.IsNotNull (pattern, string.Format ("MultipleView Pattern SHOULD NOT be null {0}", listView.View));

				supportedViews = pattern.GetSupportedViews ();
				int lastViewId = 0;
				foreach (int viewId in supportedViews) {
					pattern.SetCurrentView (viewId);
					Assert.AreEqual ("Icons", pattern.GetViewName (viewId), 
						string.Format ("GetViewName -> {0}", listView.View));
				}
				//We should throw an exception
				try {
					lastViewId += 12345;
					pattern.SetCurrentView (lastViewId);
					Assert.Fail ("Should throw ArgumentException");
				} catch (ArgumentException) { }
			}
		}

		[Test]
		public void MultipleView_SetCurrentViewTest ()
		{
			ListView listView = GetControlInstance () as ListView;
			IRawElementProviderFragment listViewProvider;
			IMultipleViewProvider pattern;
			int [] supportedViews = null;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				listView.View = viewVal;
				listViewProvider = (IRawElementProviderFragment) GetProviderFromControl (listView);
				Assert.IsNotNull (listViewProvider, "Provider not implemented for ListView");
				
				pattern = listViewProvider.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id) as IMultipleViewProvider;
				Assert.IsNotNull (pattern, string.Format ("MultipleView Pattern SHOULD NOT be null {0}", listView.View));

				supportedViews = pattern.GetSupportedViews ();
				int lastViewId = 0;
				foreach (int viewId in supportedViews) {
					pattern.SetCurrentView (viewId);
					Assert.AreEqual ("Icons", pattern.GetViewName (viewId),
						string.Format ("GetViewName -> {0}", listView.View));
					lastViewId = viewId;
				}
				//We should throw an exception
				try {
					lastViewId += 12345;
					pattern.SetCurrentView (lastViewId);
					Assert.Fail ("Should throw ArgumentException");
				} catch (ArgumentException) { }
			}
		}

		#endregion

		#region Control Type Tests

		[Test]
		public void ControlTypeTest ()
		{
			ListView listView = GetControlInstance () as ListView;
			IRawElementProviderFragment listViewProvider;
			int defaultControlType = ControlType.List.Id;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				listView.View = viewVal;
				listViewProvider = (IRawElementProviderFragment) GetProviderFromControl (listView);
				Assert.IsNotNull (listViewProvider, "Provider not implemented for ListView");

				int controlType
					= (int) listViewProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) ;

				if (listView.View == View.Details)
					defaultControlType = ControlType.DataGrid.Id;
				else
					defaultControlType = ControlType.List.Id;

				Assert.AreEqual (defaultControlType, controlType,
					string.Format ("Different Control Type: {0} {1}",
				                                ControlType.LookupById (controlType).ProgrammaticName, listView.View));
			}
		}

		#endregion

		#region View.LargeIcon Pattern Tests

		[Test]
		public void ViewLargeIcon_PatternTest ()
		{
			//TESTS: ListView.View = View.LargeIcon contains ONLY the following children: ScrollBar and Group

			//LAMESPEC: We need to call this otherwise the Groups aren't shown!!
			Application.EnableVisualStyles ();

			ListView view = GetListView (3, 10, 4, 6);
			view.View = View.LargeIcon;

			IRawElementProviderFragment element = (IRawElementProviderFragment) GetProviderFromControl (view);
			Assert.IsNotNull (element, "Provider not implemented for ListView");

			//When SWF.ListView.ShowGroups = true we only have Group or ScrollBar as children 
			IRawElementProviderFragment child = (IRawElementProviderFragment) element.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (child, "We should have children! view.ShowGroups=true");
			
			view.ShowGroups = true;
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType != ControlType.ScrollBar.Id && controlType != ControlType.Group.Id)
					Assert.Fail (string.Format ("When SWF.ListView.ShowGroups = true we only have Group or ScrollBar as children: {0}",
						ControlType.LookupById (controlType).ProgrammaticName));

				child = child.Navigate (NavigateDirection.NextSibling);
			}

			//Lets disable ShowGroups, we must not have groups!
			view.ShowGroups = false;
			child = (IRawElementProviderFragment) element.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (child, "We should have children! view.ShowGroups=false");
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.Group.Id)
					Assert.Fail (string.Format ("When SWF.ListView.ShowGroups = false we shouldn't have Group: {0}",
						ControlType.LookupById (controlType).ProgrammaticName));

				child = child.Navigate (NavigateDirection.NextSibling);
			}

			view = GetListView (2, 17, 3, 3);
			view.View = View.LargeIcon;
			element = (IRawElementProviderFragment) GetProviderFromControl (view);

			//Control Type tests
			Assert.AreEqual (ControlType.List.Id,
				element.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id), "View.LargeIcon: ControlType.List");
			Assert.AreEqual ("list",
				element.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id), "View.LargeIcon: list");

			Assert.IsNotNull (element.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id), 
			                  "View.LargeIcon: MUST support Selection Pattern");
			Assert.IsNotNull (element.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id), 
			                  "View.LargeIcon: MIGHT support Scroll Pattern");
			Assert.IsNotNull (element.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id), 
			                  "View.LargeIcon: MIGHT support MultipleView Pattern");
			view.ShowGroups = false;
			Assert.IsNotNull (element.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			                  "View.LargeIcon: MUST support GridPattern Pattern. view.ShowGroups=false");
			view.ShowGroups = true;
			Assert.IsNull (element.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			               "View.LargeIcon: SHOULD NOT support GridPattern Pattern view.ShowGroups=true");
			Assert.IsNull (element.GetPatternProvider (TablePatternIdentifiers.Pattern.Id),
			               "View.LargeIcon: SHOULD NOT support TablePattern Pattern");

			//Lets test the group
			child = element.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragment group = null;
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.Group.Id) {
					group = child;
					break;
				}

				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (group, "View.LargeIcon: Group should not be NULL");

			Assert.IsNull (group.GetPatternProvider (DockPatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support Dock Pattern");
			Assert.IsNotNull (group.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id),
			                  "Group in View.LargeIcon: MUST support ExpandCollapse Pattern");
			Assert.IsNotNull (group.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			                  "Group in View.LargeIcon: MUST support Grid Pattern");
			Assert.IsNull (group.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support GridItem Pattern");
			Assert.IsNull (group.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support Invoke Pattern");
			Assert.IsNull (group.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support MultipleView Pattern");
			Assert.IsNull (group.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support RangeValue Pattern");
			Assert.IsNull (group.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support Selection Pattern");
			Assert.IsNull (group.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support SelectionItem Pattern");
			Assert.IsNull (group.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support Scroll Pattern");
			Assert.IsNull (group.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support ScrollItem Pattern");
			Assert.IsNull (group.GetPatternProvider (TablePatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support Table Pattern");
			Assert.IsNull (group.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support TableItem Pattern");
			Assert.IsNull (group.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support Toggle Pattern");
			Assert.IsNull (group.GetPatternProvider (TextPatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support Text Pattern");
			Assert.IsNull (group.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support Transform Pattern");
			Assert.IsNull (group.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support Value Pattern");
			Assert.IsNull (group.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id),
			               "Group in View.LargeIcon: SHOULD NOT support Window Pattern");

			//Lets test the ListItem
			child = group.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragment listItem = null;
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.ListItem.Id) {
					listItem = child;
					break;
				}

				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (listItem, "View.LargeIcon: ListItem should not be NULL");
			Assert.AreEqual (ControlType.ListItem.Id,
				listItem.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id), "View.LargeIcon: ControlType.ListItem");
			Assert.AreEqual ("list item",
				listItem.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id), "View.LargeIcon: list item");

			Assert.IsNull (listItem.GetPatternProvider (DockPatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD NOT support Dock Pattern");
			Assert.IsNull (listItem.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD NOT support ExpandCollapse Pattern");
			Assert.IsNull (listItem.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD NOT support Grid Pattern");
			Assert.IsNotNull (listItem.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id),
			                  "listItem in View.LargeIcon: MUST support GridItem Pattern");
			Assert.IsNull (listItem.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD NOT support Invoke Pattern");
			Assert.IsNull (listItem.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD NOT support MultipleView Pattern");
			Assert.IsNull (listItem.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD NOT support RangeValue Pattern");
			Assert.IsNull (listItem.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD NOT support Selection Pattern");
			Assert.IsNotNull (listItem.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: MUST support SelectionItem Pattern");
			Assert.IsNull (listItem.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD NOT support Scroll Pattern");
			Assert.IsNotNull (listItem.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD NOT support ScrollItem Pattern");
			Assert.IsNull (listItem.GetPatternProvider (TablePatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD NOT support Table Pattern");
			Assert.IsNull (listItem.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id),
			               "listlItem in View.LargeIcon: SHOULD NOT support TableItem Pattern");

			view.CheckBoxes = true;
			Assert.IsNotNull (listItem.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD support Toggle Pattern when view.CheckBoxes=true");
			view.CheckBoxes = false;
			Assert.IsNull (listItem.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD NOT support Toggle Pattern when view.CheckBoxes=false");

			Assert.IsNull (listItem.GetPatternProvider (TextPatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD NOT support Text Pattern");
			Assert.IsNull (listItem.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD NOT support Transform Pattern");

			view.LabelEdit = true;
			Assert.IsNotNull (listItem.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			                  "listItem in View.LargeIcon: MUST support Value Pattern when view.LabelEdit=true.");
			view.LabelEdit = false;
			Assert.IsNull (listItem.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			                  "listItem in View.LargeIcon: SHOULD NOT support Value Pattern when view.LabelEdit=false.");

			Assert.IsNull (listItem.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id),
			               "listItem in View.LargeIcon: SHOULD NOT support Window Pattern");

			//Children in ListItem
			view.CheckBoxes = false;
			IRawElementProviderFragment listItemChild = listItem.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (listItemChild, "listItem in View.LargeIcon: No children");
			
			view.CheckBoxes = true;
			listItemChild = listItem.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (listItemChild, "listItem in View.LargeIcon: view.CheckBoxes=true We should have one child:");

			listItemChild = listItemChild.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (listItemChild, "listItem in View.LargeIcon: view.CheckBoxes=true ONE CHILD ONLY");
		}

		#endregion

		#region View.SmallIcon Pattern Tests

		[Test]
		public void ViewSmallIcon_PatternTest ()
		{
			//TESTS: ListView.View = View.SmallIcon contains ONLY the following children: ScrollBar and Group

			//LAMESPEC: We need to call this otherwise the Groups aren't shown!!
			Application.EnableVisualStyles ();

			ListView view = GetListView (3, 10, 4, 6);
			view.View = View.SmallIcon;

			IRawElementProviderFragment element = (IRawElementProviderFragment) GetProviderFromControl (view);

			//When SWF.ListView.ShowGroups = true we only have Group or ScrollBar as children 
			view.ShowGroups = true;
			IRawElementProviderFragment child = element.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType != ControlType.ScrollBar.Id && controlType != ControlType.Group.Id)
					Assert.Fail (string.Format ("When SWF.ListView.ShowGroups = true we only have Group or ScrollBar as children: {0}",
						ControlType.LookupById (controlType).ProgrammaticName));

				child = child.Navigate (NavigateDirection.NextSibling);
			}

			//Lets disable ShowGroups, we must not have groups!
			view.ShowGroups = false;
			child = element.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.Group.Id)
					Assert.Fail (string.Format ("When SWF.ListView.ShowGroups = false we shouldn't have Group: {0}",
						ControlType.LookupById (controlType).ProgrammaticName));

				child = child.Navigate (NavigateDirection.NextSibling);
			}

			view = GetListView (2, 17, 3, 3);
			view.View = View.LargeIcon;
			element = GetProviderFromControl (view);

			//Control Type tests
			Assert.AreEqual (ControlType.List.Id,
				element.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id), "View.SmallIcon: ControlType.List");
			Assert.AreEqual ("list",
				element.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id), "View.SmallIcon: list");

			Assert.IsNotNull (element.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id),
			                  "View.SmallIcon: MUST support Selection Pattern");
			Assert.IsNotNull (element.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id),
			                  "View.SmallIcon: MIGHT support Scroll Pattern");
			Assert.IsNotNull (element.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id),
			                  "View.SmallIcon: MIGHT support MultipleView Pattern");
			view.ShowGroups = false;
			Assert.IsNotNull (element.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			                  "View.SmallIcon: MUST support GridPattern Pattern. view.ShowGroups=false");
			view.ShowGroups = true;
			Assert.IsNull (element.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			               "View.SmallIcon: SHOULD NOT support GridPattern Pattern view.ShowGroups=true");
			Assert.IsNull (element.GetPatternProvider (TablePatternIdentifiers.Pattern.Id),
			               "View.SmallIcon: SHOULD NOT support TablePattern Pattern");

			//Lets test the group
			child = element.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragment group = null;
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.Group.Id) {
					group = child;
					break;
				}

				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (group, "View.SmallIcon: Group should not be NULL");

			Assert.IsNull (group.GetPatternProvider (DockPatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support Dock Pattern");
			Assert.IsNotNull (group.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id),
			                  "Group in View.SmallIcon: MUST support ExpandCollapse Pattern");
			Assert.IsNotNull (group.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			                  "Group in View.SmallIcon: MUST support Grid Pattern");
			Assert.IsNull (group.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support GridItem Pattern");
			Assert.IsNull (group.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support Invoke Pattern");
			Assert.IsNull (group.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support MultipleView Pattern");
			Assert.IsNull (group.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support RangeValue Pattern");
			Assert.IsNull (group.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support Selection Pattern");
			Assert.IsNull (group.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support SelectionItem Pattern");
			Assert.IsNull (group.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support Scroll Pattern");
			Assert.IsNull (group.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support ScrollItem Pattern");
			Assert.IsNull (group.GetPatternProvider (TablePatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support Table Pattern");
			Assert.IsNull (group.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support TableItem Pattern");
			Assert.IsNull (group.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support Toggle Pattern");
			Assert.IsNull (group.GetPatternProvider (TextPatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support Text Pattern");
			Assert.IsNull (group.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support Transform Pattern");
			Assert.IsNull (group.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support Value Pattern");
			Assert.IsNull (group.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id),
			               "Group in View.SmallIcon: SHOULD NOT support Window Pattern");


			//Lets test the ListItem
			child = group.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragment listItem = null;
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.ListItem.Id) {
					listItem = child;
					break;
				}

				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (listItem, "View.SmallIcon: ListItem should not be NULL");
			Assert.AreEqual (ControlType.ListItem.Id,
				listItem.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id), "View.SmallIcon: ControlType.ListItem");
			Assert.AreEqual ("list item",
				listItem.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id), "View.SmallIcon: list item");

			Assert.IsNotNull (listItem.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id),
			                  "listItem in View.SmallIcon: MUST support GridItem Pattern");
			Assert.IsNotNull (listItem.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id),
			                  "listItem in View.SmallIcon: MUST support SelectionItem Pattern");
			Assert.IsNotNull (listItem.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id),
			                  "listItem in View.SmallIcon: SHOULD NOT support ScrollItem Pattern");
			Assert.IsNull (listItem.GetPatternProvider (DockPatternIdentifiers.Pattern.Id),
			               "listItem in View.SmallIcon: SHOULD NOT support Dock Pattern");
			Assert.IsNull (listItem.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id),
			               "listItem in View.SmallIcon: SHOULD NOT support ExpandCollapse Pattern");
			Assert.IsNull (listItem.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			               "listItem in View.SmallIcon: SHOULD NOT support Grid Pattern");
			Assert.IsNull (listItem.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id),
			               "listItem in View.SmallIcon: SHOULD NOT support Invoke Pattern");
			Assert.IsNull (listItem.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id),
			               "listItem in View.SmallIcon: SHOULD NOT support MultipleView Pattern");
			Assert.IsNull (listItem.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id),
			               "listItem in View.SmallIcon: SHOULD NOT support RangeValue Pattern");
			Assert.IsNull (listItem.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id),
			               "listItem in View.SmallIcon: SHOULD NOT support Selection Pattern");
			Assert.IsNull (listItem.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id),
			               "listItem in View.SmallIcon: SHOULD NOT support Scroll Pattern");
			Assert.IsNull (listItem.GetPatternProvider (TablePatternIdentifiers.Pattern.Id),
			               "listItem in View.SmallIcon: SHOULD NOT support Table Pattern");
			Assert.IsNull (listItem.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id),
			               "listItem in View.SmallIcon: SHOULD NOT support TableItem Pattern");

			view.CheckBoxes = false;
			Assert.IsNull (listItem.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id),
			               "listItem in View.SmallIcon: SHOULD NOT support Toggle Pattern when view.CheckBoxes=false.");
			view.CheckBoxes = true;
			Assert.IsNotNull (listItem.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id),
			                  "listItem in View.SmallIcon: SHOULD support Toggle Pattern when view.CheckBoxes=true");

			Assert.IsNull (listItem.GetPatternProvider (TextPatternIdentifiers.Pattern.Id),
			               "listItem in View.SmallIcon: SHOULD NOT support Text Pattern");
			Assert.IsNull (listItem.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id),
			               "listItem in View.SmallIcon: SHOULD NOT support Transform Pattern");
			
			view.LabelEdit = false;
			Assert.IsNull (listItem.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			               "listItem in View.SmallIcon: SHOULD NOT support Value Pattern when view.LabelEdit=false.");
			view.LabelEdit = true;
			Assert.IsNotNull (listItem.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			                  "listItem in View.SmallIcon: MUST support Value Pattern when view.LabelEdit=true.");

			//Children in ListItem
			view.CheckBoxes = false;
			IRawElementProviderFragment listItemChild = listItem.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (listItemChild, "listItem in View.SmallIcon: No children");

			view.CheckBoxes = true;
			listItemChild = listItem.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (listItemChild, "listItem in View.SmallIcon: view.CheckBoxes=true We should have one child:");

			listItemChild = listItemChild.Navigate (NavigateDirection.NextSibling);
			Assert.IsNull (listItemChild, "listItem in View.SmallIcon: view.CheckBoxes=true ONE CHILD ONLY");
		}

		#endregion

		#region View.List Pattern Tests

		[Test]
		public void ViewList_PatternTest ()
		{
			//TESTS: ListView.View = View.List contains ONLY the following children: ScrollBar and ListItem

			//LAMESPEC: We need to call this otherwise the Groups aren't shown!!
			Application.EnableVisualStyles ();

			ListView view = GetListView (3, 10, 4, 6);
			view.View = View.List;
			view.ShowGroups = true;

			IRawElementProviderFragment element = (IRawElementProviderFragment) GetProviderFromControl  (view);

			//SWF.ListView.ShowGroups is not used, so setting it true doesnt care, we only have ListItem and ScrollBars
			IRawElementProviderFragment child = element.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType != ControlType.ScrollBar.Id && controlType != ControlType.ListItem.Id)
					Assert.Fail (string.Format ("Only ListItem Group or ScrollBar as children are allowed: {0}",
						ControlType.LookupById (controlType).ProgrammaticName));

				child = child.Navigate (NavigateDirection.NextSibling);
			}

			//Lets disable ShowGroups, we should have the same...
			view.ShowGroups = false;
			child = element.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.Group.Id)
					Assert.Fail (string.Format ("Only ListItem Group or ScrollBar as children are allowed: {0}",
						ControlType.LookupById (controlType).ProgrammaticName));

				child = child.Navigate (NavigateDirection.NextSibling);
			}

			view = GetListView (2, 17, 3, 3);
			view.View = View.List;
			element = GetProviderFromControl (view);

			//Control Type tests
			Assert.AreEqual (ControlType.List.Id,
				element.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id), "View.List: ControlType.List");
			Assert.AreEqual ("list",
				element.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id), "View.List: list");

			Assert.IsNotNull (element.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id),
			                  "View.List: MUST support Selection Pattern");
			Assert.IsNotNull (element.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id),
			                  "View.List: MIGHT support Scroll Pattern");
			Assert.IsNotNull (element.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id),
			                  "View.List: MIGHT support MultipleView Pattern");
			view.ShowGroups = false;
			Assert.IsNotNull (element.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			                  "View.List: MUST support GridPattern Pattern. view.ShowGroups=false");
			view.ShowGroups = true;
			Assert.IsNotNull (element.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			                  "View.List: MUST support GridPattern Pattern view.ShowGroups=true");
			Assert.IsNull (element.GetPatternProvider (TablePatternIdentifiers.Pattern.Id),
			               "View.List: SHOULD NOT support TablePattern Pattern");

			//Lets test the ListItem
			child = element.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragment listItem = null;
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.ListItem.Id) {
					listItem = child;
					break;
				}

				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (listItem, "View.List: ListItem should not be NULL");
			Assert.AreEqual (ControlType.ListItem.Id,
				listItem.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id), "View.List: ControlType.ListItem");
			Assert.AreEqual ("list item",
				listItem.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id), "View.List: list item");

			Assert.IsNotNull (listItem.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id),
			                  "listItem in View.List: MUST support GridItem Pattern");
			Assert.IsNotNull (listItem.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id),
			                  "listItem in View.List: SHOULD NOT support ScrollItem Pattern");
			Assert.IsNotNull (listItem.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id),
			                  "listItem in View.List: MUST support SelectionItem Pattern");

			Assert.IsNull (listItem.GetPatternProvider (DockPatternIdentifiers.Pattern.Id),
			               "listItem in View.List: SHOULD NOT support Dock Pattern");
			Assert.IsNull (listItem.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id),
			               "listItem in View.List: SHOULD NOT support ExpandCollapse Pattern");
			Assert.IsNull (listItem.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			               "listItem in View.List: SHOULD NOT support Grid Pattern");
			Assert.IsNull (listItem.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id),
			               "listItem in View.List: SHOULD NOT support Invoke Pattern");
			Assert.IsNull (listItem.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id),
			               "listItem in View.List: SHOULD NOT support MultipleView Pattern");
			Assert.IsNull (listItem.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id),
			               "listItem in View.List: SHOULD NOT support RangeValue Pattern");
			Assert.IsNull (listItem.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id),
			               "listItem in View.List: SHOULD NOT support Selection Pattern");
			Assert.IsNull (listItem.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id),
			               "listItem in View.List: SHOULD NOT support Scroll Pattern");
			Assert.IsNull (listItem.GetPatternProvider (TablePatternIdentifiers.Pattern.Id),
			               "listItem in View.List: SHOULD NOT support Table Pattern");
			Assert.IsNull (listItem.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id),
			               "listItem in View.List: SHOULD NOT support TableItem Pattern");

			view.CheckBoxes = false;
			Assert.IsNull (listItem.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id),
			               "listItem in View.List: SHOULD NOT support Toggle Pattern when view.CheckBoxes=false.");
			view.CheckBoxes = true;
			Assert.IsNotNull (listItem.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id),
			                  "listItem in View.List: SHOULD NOT support Toggle Pattern when view.CheckBoxes=true");

			Assert.IsNull (listItem.GetPatternProvider (TextPatternIdentifiers.Pattern.Id),
			               "listItem in View.List: SHOULD NOT support Text Pattern");
			Assert.IsNull (listItem.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id),
			               "listItem in View.List: SHOULD NOT support Transform Pattern");

			view.LabelEdit = false;
			Assert.IsNull (listItem.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			               "listItem in View.List: SHOULD NOT support Value Pattern when view.LabelEdit=false.");
			view.LabelEdit = true;
			Assert.IsNotNull (listItem.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			                  "listItem in View.List: MUST support Value Pattern when view.LabelEdit=true.");

			Assert.IsNull (listItem.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id), 
			               "listItem in View.List: SHOULD NOT support Window Pattern");

			//Children in ListItem
			view.CheckBoxes = false;
			IRawElementProviderFragment listItemChild = listItem.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (listItemChild, "listItem in View.List: No children");

			view.CheckBoxes = true;
			listItemChild = listItem.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (listItemChild, "listItem in View.List: view.CheckBoxes=true We should have one child:");

			listItemChild = listItemChild.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (listItemChild, "listItem in View.List: view.CheckBoxes=true ONE CHILD ONLY");
		}

		#endregion

		#region View.Tile Pattern Tests

		[Test]
		public void ViewTile_PatternTest ()
		{
			//TESTS: ListView.View = View.Tile contains ONLY the following children: ScrollBar and ListItem

			//LAMESPEC: We need to call this otherwise the Groups aren't shown!!
			Application.EnableVisualStyles ();

			ListView view = GetListView (3, 10, 4, 6);
			view.View = View.Tile;
			view.ShowGroups = true;

			IRawElementProviderFragment element = (IRawElementProviderFragment) GetProviderFromControl (view);

			//SWF.ListView.ShowGroups is not used, so setting it true doesnt care, we only have ListItem and ScrollBars
			IRawElementProviderFragment child = element.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType != ControlType.ScrollBar.Id && controlType != ControlType.Group.Id)
					Assert.Fail (string.Format ("Only Group or ScrollBar as children are allowed: {0}",
						ControlType.LookupById (controlType).ProgrammaticName));

				child = child.Navigate (NavigateDirection.NextSibling);
			}

			//Lets disable ShowGroups, we should have the same...
			view.ShowGroups = false;
			child = element.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.Group.Id)
					Assert.Fail (string.Format ("Only ListItem as children are allowed: {0}",
						ControlType.LookupById (controlType).ProgrammaticName));

				child = child.Navigate (NavigateDirection.NextSibling);
			}

			view = GetListView (2, 17, 3, 3);
			view.View = View.Tile;
			element = GetProviderFromControl (view);

			//Control Type tests
			Assert.AreEqual (ControlType.List.Id,
				element.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id), "View.Tile: ControlType.List");
			Assert.AreEqual ("list",
				element.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id), "View.Tile: list");

			Assert.IsNotNull (element.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id), 
			                  "View.Tile: MUST support Selection Pattern");
			Assert.IsNotNull (element.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id), 
			                  "View.Tile: MIGHT support Scroll Pattern");
			Assert.IsNotNull (element.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id), 
			                  "View.Tile: MIGHT support MultipleView Pattern");
			view.ShowGroups = false;
			Assert.IsNotNull (element.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			                  "View.Tile: MUST support GridPattern Pattern. view.ShowGroups=false");
			view.ShowGroups = true;
			Assert.IsNull (element.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			               "View.Tile: SHOULD NOT support GridPattern Pattern view.ShowGroups=true");
			Assert.IsNull (element.GetPatternProvider (TablePatternIdentifiers.Pattern.Id), 
			               "View.Tile: SHOULD NOT support TablePattern Pattern");

			//Lets test the group
			child = element.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragment group = null;
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.Group.Id) {
					group = child;
					break;
				}

				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (group, "View.Tile: Group should not be NULL");

			Assert.IsNotNull (group.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id), 
			                  "Group in View.Tile: MUST support ExpandCollapse Pattern");
			Assert.IsNotNull (group.GetPatternProvider (GridPatternIdentifiers.Pattern.Id), 
			                  "Group in View.Tile: MUST support Grid Pattern");

			Assert.IsNull (group.GetPatternProvider (DockPatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support Dock Pattern");
			Assert.IsNull (group.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support GridItem Pattern");
			Assert.IsNull (group.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support Invoke Pattern");
			Assert.IsNull (group.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support MultipleView Pattern");
			Assert.IsNull (group.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support RangeValue Pattern");
			Assert.IsNull (group.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support Selection Pattern");

			Assert.IsNull (group.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support SelectionItem Pattern");
			Assert.IsNull (group.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support Scroll Pattern");
			Assert.IsNull (group.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support ScrollItem Pattern");
			Assert.IsNull (group.GetPatternProvider (TablePatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support Table Pattern");
			Assert.IsNull (group.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support TableItem Pattern");
			Assert.IsNull (group.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support Toggle Pattern");
			Assert.IsNull (group.GetPatternProvider (TextPatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support Text Pattern");
			Assert.IsNull (group.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support Transform Pattern");
			Assert.IsNull (group.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support Value Pattern");
			Assert.IsNull (group.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id), 
			               "Group in View.Tile: SHOULD NOT support Window Pattern");

			//Lets test the ListItem
			child = group.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragment listItem = null;
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.ListItem.Id) {
					listItem = child;
					break;
				}

				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (listItem, "View.Tile: ListItem should not be NULL");
			Assert.AreEqual (ControlType.ListItem.Id,
				listItem.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id), "View.Tile: ControlType.ListItem");
			Assert.AreEqual ("list item",
				listItem.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id), "View.Tile: list item");

			Assert.IsNotNull (listItem.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id), 
			                  "listItem in View.Tile: MUST support GridItem Pattern");
			Assert.IsNotNull (listItem.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id), 
			                  "listItem in View.Tile: SHOULD NOT support ScrollItem Pattern");
			Assert.IsNotNull (listItem.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id), 
			                  "listItem in View.Tile: MUST support SelectionItem Pattern");
			Assert.IsNull (listItem.GetPatternProvider (DockPatternIdentifiers.Pattern.Id), 
			               "listItem in View.Tile: SHOULD NOT support Dock Pattern");
			Assert.IsNull (listItem.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id), 
			               "listItem in View.Tile: SHOULD NOT support ExpandCollapse Pattern");
			Assert.IsNull (listItem.GetPatternProvider (GridPatternIdentifiers.Pattern.Id), 
			               "listItem in View.Tile: SHOULD NOT support Grid Pattern");
			Assert.IsNull (listItem.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id), 
			               "listItem in View.Tile: SHOULD NOT support Invoke Pattern");
			Assert.IsNull (listItem.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id), 
			               "listItem in View.Tile: SHOULD NOT support MultipleView Pattern");
			Assert.IsNull (listItem.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id), 
			               "listItem in View.Tile: SHOULD NOT support RangeValue Pattern");
			Assert.IsNull (listItem.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id), 
			               "listItem in View.Tile: SHOULD NOT support Selection Pattern");
			Assert.IsNull (listItem.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id), 
			               "listItem in View.Tile: SHOULD NOT support Scroll Pattern");
			Assert.IsNull (listItem.GetPatternProvider (TablePatternIdentifiers.Pattern.Id), 
			               "listItem in View.Tile: SHOULD NOT support Table Pattern");
			Assert.IsNull (listItem.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id), 
			               "listItem in View.Tile: SHOULD NOT support TableItem Pattern");
			Assert.IsNull (listItem.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id), 
			               "listItem in View.Tile: SHOULD NOT support Toggle Pattern");
			Assert.IsNull (listItem.GetPatternProvider (TextPatternIdentifiers.Pattern.Id),
			               "listItem in View.Tile: SHOULD NOT support Text Pattern");
			Assert.IsNull (listItem.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id),
			               "listItem in View.Tile: SHOULD NOT support Transform Pattern");

			view.LabelEdit = false;
			Assert.IsNull (listItem.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			               "listItem in View.Tile: SHOULD NOT support Value Pattern when view.LabelEdit=false.");			
			view.LabelEdit = true;
			Assert.IsNotNull (listItem.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			               "listItem in View.Tile: MUST support Value Pattern when view.LabelEdit=true.");

			Assert.IsNull (listItem.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id),
			               "listItem in View.Tile: SHOULD NOT support Window Pattern");

			//Children in ListItem
			view.CheckBoxes = false;
			IRawElementProviderFragment listItemChild = listItem.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (listItemChild, "listItem in View.Tile: No children");
		}

		#endregion

		#region View.Details Pattern Tests

		[Test]
		public void ViewDetails_PatternTest ()
		{
			//TESTS: ListView.View = View.Details contains ONLY the following children: ScrollBar, ListItem, Header

			//LAMESPEC: We need to call this otherwise the Groups aren't shown!!
			Application.EnableVisualStyles ();

			ListView view = GetListView (3, 10, 4, 6);
			view.View = View.Details;
			view.ShowGroups = true;

			IRawElementProviderFragment element = GetProviderFromControl (view);

			//SWF.ListView.ShowGroups is not used, so setting it true doesnt care, we only have ListItem and ScrollBars
			IRawElementProviderFragment child = element.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType != ControlType.ScrollBar.Id && controlType != ControlType.Group.Id && controlType != ControlType.Header.Id)
					Assert.Fail (string.Format ("Only Group, ScrollBar or Header as children are allowed: {0}",
						ControlType.LookupById (controlType).ProgrammaticName));

				child = child.Navigate (NavigateDirection.NextSibling);
			}

			//Lets disable ShowGroups, we should have the same...
			view.ShowGroups = false;
			child = element.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.Group.Id && controlType != ControlType.Header.Id)
					Assert.Fail (string.Format ("Only ListItem and Header as children are allowed: {0}",
						ControlType.LookupById (controlType).ProgrammaticName));

				child = child.Navigate (NavigateDirection.NextSibling);
			}

			view = GetListView (3, 17, 3, 10);
			view.View = View.Details;
			element = GetProviderFromControl (view);

			//Control Type tests
			Assert.AreEqual (ControlType.DataGrid.Id,
				element.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id), "View.Details: ControlType.DataGrid");
			Assert.AreEqual ("data grid",
				element.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id), "View.Details: data grid");

			TestDataGridPatterns (element);
			view.ShowGroups = false;
			TestDataGridPatterns (element);
			view.ShowGroups = true;
			TestDataGridPatterns (element);

			//Lets test the group
			child = element.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragment group = null;
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.Group.Id) {
					group = child;
					break;
				}

				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (group, "View.Details: Group should not be NULL");

			Assert.IsNotNull (group.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id),
			                  "Group in View.Details: MUST support ExpandCollapse Pattern");
			Assert.IsNotNull (group.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			                  "Group in View.Details: MUST support Grid Pattern");

			Assert.IsNull (group.GetPatternProvider (DockPatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support Dock Pattern");
			Assert.IsNull (group.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support GridItem Pattern");
			Assert.IsNull (group.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support Invoke Pattern");
			Assert.IsNull (group.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support MultipleView Pattern");
			Assert.IsNull (group.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support RangeValue Pattern");
			Assert.IsNull (group.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support Selection Pattern");
			Assert.IsNull (group.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support SelectionItem Pattern");
			Assert.IsNull (group.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support Scroll Pattern");
			Assert.IsNull (group.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support ScrollItem Pattern");
			Assert.IsNull (group.GetPatternProvider (TablePatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support Table Pattern");
			Assert.IsNull (group.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support TableItem Pattern");
			Assert.IsNull (group.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support Toggle Pattern");
			Assert.IsNull (group.GetPatternProvider (TextPatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support Text Pattern");
			Assert.IsNull (group.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support Transform Pattern");
			Assert.IsNull (group.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support Value Pattern");
			Assert.IsNull (group.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id),
			               "Group in View.Details: SHOULD NOT support Window Pattern");

			//Lets test the ListItem
			child = group.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragment dataItem = null;
			while (child != null) {
				int controlType
					= (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.DataItem.Id) {
					dataItem = child;
					break;
				}

				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (dataItem, "View.Details: ListItem should not be NULL");

			Assert.AreEqual (ControlType.DataItem.Id,
				dataItem.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id), "View.Details: ControlType.DataItem");
			Assert.AreEqual ("data item", //Vista return "item" WE ARE RETURNING THE VALID VALUE
				dataItem.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id), "View.Tile: data item");

			IRawElementProviderFragment dataItemParent = dataItem.Navigate (NavigateDirection.Parent);
			if (dataItemParent.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id) != null) {
				Assert.IsNotNull (dataItem.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id),
				                  "listItem in View.Details: MUST support ScrollItem Pattern");
			}
			Assert.IsNotNull (dataItem.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id),
			                  "listItem in View.Details: MUST support SelectionItem Pattern");

			Assert.IsNull (dataItem.GetPatternProvider (DockPatternIdentifiers.Pattern.Id),
			               "listItem in View.Details: SHOULD NOT support Dock Pattern");
			Assert.IsNull (dataItem.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id),
			               "listItem in View.Details: SHOULD NOT support ExpandCollapse Pattern");
			Assert.IsNull (dataItem.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			               "listItem in View.Details: SHOULD NOT support Grid Pattern");
			// LAMESPEC: Should be IsNull instead of IsNotNull, Vista doesn't implement GridItem
			if ((bool) dataItemParent.GetPropertyValue (AutomationElementIdentifiers.IsGridPatternAvailableProperty.Id))
				Assert.IsNotNull (dataItem.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id),
				                  "listItem in View.Details: SHOULD support GridItem Pattern");
			Assert.IsNull (dataItem.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id),
			               "listItem in View.Details: SHOULD NOT support Invoke Pattern");
			Assert.IsNull (dataItem.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id),
			               "listItem in View.Details: SHOULD NOT support MultipleView Pattern");
			Assert.IsNull (dataItem.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id),
			               "listItem in View.Details: SHOULD NOT support RangeValue Pattern");
			Assert.IsNull (dataItem.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id),
			               "listItem in View.Details: SHOULD NOT support Selection Pattern");
			Assert.IsNull (dataItem.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id),
			               "listItem in View.Details: SHOULD NOT support Scroll Pattern");
			Assert.IsNull (dataItem.GetPatternProvider (TablePatternIdentifiers.Pattern.Id),
			               "listItem in View.Details: SHOULD NOT support Table Pattern");
			// LAMESPEC:  Should be IsNull instead of IsNotNull, Vista doesn't implement TableItem
			if ((int) dataItemParent.Navigate (NavigateDirection.Parent).GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    == ControlType.DataGrid.Id)
				Assert.IsNotNull (dataItem.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id),
				                  "listItem in View.Details: SHOULD support TableItem Pattern when parent is DataGrid");

			view.CheckBoxes = true;
			Assert.IsNotNull (dataItem.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id),
			                  "listItem in View.Details: SHOULD NOT support Toggle Pattern when view.CheckBoxes=true");
			view.CheckBoxes = false;
			Assert.IsNull (dataItem.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id),
			               "listItem in View.Details: SHOULD NOT support Toggle Pattern when view.CheckBoxes=false.");

			Assert.IsNull (dataItem.GetPatternProvider (TextPatternIdentifiers.Pattern.Id),
			               "listItem in View.Details: SHOULD NOT support Text Pattern");
			Assert.IsNull (dataItem.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id),
			               "listItem in View.Details: SHOULD NOT support Transform Pattern");

			view.LabelEdit = false;
			Assert.IsNull (dataItem.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			               "listItem in View.Details: SHOULD NOT support Value Pattern when view.LabelEdit=false.");
			view.LabelEdit = true;
			Assert.IsNotNull (dataItem.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			               "listItem in View.Details: MUST support Value Pattern when view.LabelEdit=true.");
			Assert.IsNull (dataItem.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id), 
			               "listItem in View.Details: SHOULD NOT support Window Pattern");

			//Header in ListView

			element = GetProviderFromControl (view);
			IRawElementProviderFragment headerElement = element.Navigate (NavigateDirection.FirstChild);
			while (headerElement != null) {
				int ctype
					= (int) headerElement.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
				if (ctype == ControlType.Header.Id)
					break;
				headerElement = headerElement.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (headerElement, "We should have a Header");

			Assert.IsNull (headerElement.GetPatternProvider (DockPatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support Dock Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support ExpandCollapse Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (GridPatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support Grid Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support GridItem Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support Invoke Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support MultipleView Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support RangeValue Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support Selection Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support SelectionItem Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support Scroll Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support ScrollItem Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (TablePatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support Table Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support TableItem Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id), 
			               "headerElement in View.LargeIcon: SHOULD NOT support Toggle Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (TextPatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support Text Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support Transform Pattern");
			Assert.IsNull (headerElement.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support Value Pattern.");
			Assert.IsNull (headerElement.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id), 
			               "headerElement in View.Details: SHOULD NOT support Window Pattern");

			view.Columns.Clear ();
			IRawElementProviderFragment headerItem = headerElement.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (headerItem, "We SHOULD NOT HAVE HeaderItem");

			view.Columns.Add ("Column 0");

			headerItem = headerElement.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (headerItem, "We SHOULD HAVE HeaderItem");

			Assert.IsNotNull (headerItem.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id), 
			                  "headerItem in View.Details: MUST support Invoke Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (DockPatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support Dock Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support ExpandCollapse Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (GridPatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support Grid Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support GridItem Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support MultipleView Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support RangeValue Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support Selection Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support SelectionItem Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support Scroll Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support ScrollItem Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (TablePatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support Table Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support TableItem Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id), 
			               "headerItem in View.LargeIcon: SHOULD NOT support Toggle Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (TextPatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support Text Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support Transform Pattern");
			Assert.IsNull (headerItem.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support Value Pattern.");
			Assert.IsNull (headerItem.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id), 
			               "headerItem in View.Details: SHOULD NOT support Window Pattern");
			
			view.Columns.Clear ();
			headerItem = headerElement.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (headerItem, "We SHOULD NOT HAVE HeaderItem");

			//Children in ListItem

			view.ShowGroups = false;
			view.CheckBoxes = false;

			dataItem = element.Navigate (NavigateDirection.FirstChild);
			while (dataItem != null) {
				int ctype 
					= (int) dataItem.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
				if (ctype == ControlType.DataItem.Id)
					break;
				dataItem = dataItem.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (dataItem, "We should have a DataItem");

			IRawElementProviderFragment listItemChild = dataItem.Navigate (NavigateDirection.FirstChild);

			Assert.IsNull (listItemChild, "We SHOULD NOT have children because we don't have columns.");
			view.Columns.Add ("Column 0");

			listItemChild = dataItem.Navigate (NavigateDirection.FirstChild);

			if ((int) listItemChild.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
					== ControlType.Edit.Id) {
				Assert.IsNotNull (listItemChild, "We SHOULD HAVE children because we have columns.");
				Assert.AreEqual (ControlType.Edit.Id,
					listItemChild.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id), "View.Details: ControlType.Edit:");
				Assert.AreEqual ("edit",
					listItemChild.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id), 
				                 "View.Tile: edit");

				Assert.IsNotNull (listItemChild.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id), 
				                  "editItem in View.Details: MUST support Value Pattern when view.LabelEdit=false.");
				Assert.IsNotNull (listItemChild.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id), 
				                  "editItem in View.Details: MUST support TableItem Pattern");
				Assert.IsNotNull (listItemChild.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id), 
				                  "editItem in View.Details: MUST support GridItem Pattern");

				Assert.IsNull (listItemChild.GetPatternProvider (DockPatternIdentifiers.Pattern.Id), 
				               "editItem in View.Details: SHOULD NOT support Dock Pattern");
				Assert.IsNull (listItemChild.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id), 
				               "editItem in View.Details: SHOULD NOT support ExpandCollapse Pattern");
				Assert.IsNull (listItemChild.GetPatternProvider (GridPatternIdentifiers.Pattern.Id), 
				               "editItem in View.Details: SHOULD NOT support Grid Pattern");
				Assert.IsNull (listItemChild.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id), 
				               "editItem in View.Details: SHOULD NOT support Invoke Pattern");
				Assert.IsNull (listItemChild.GetPatternProvider (MultipleViewPatternIdentifiers.Pattern.Id), 
				               "editItem in View.Details: SHOULD NOT support MultipleView Pattern");
				Assert.IsNull (listItemChild.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id), 
				               "editItem in View.Details: SHOULD NOT support RangeValue Pattern");
				Assert.IsNull (listItemChild.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id), 
				               "editItem in View.Details: SHOULD NOT support Selection Pattern");
				Assert.IsNull (listItemChild.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id), 
				               "editItem in View.Details: SHOULD NOT support SelectionItem Pattern");
				Assert.IsNull (listItemChild.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id), 
				               "editItem in View.Details: SHOULD NOT support Scroll Pattern");
				Assert.IsNull (listItemChild.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id), 
				               "editItem in View.Details: SHOULD NOT support ScrollItem Pattern");
				Assert.IsNull (listItemChild.GetPatternProvider (TablePatternIdentifiers.Pattern.Id),
				               "editItem in View.Details: SHOULD NOT support Table Pattern");
				Assert.IsNull (listItemChild.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id),
				               "editItem in View.LargeIcon: SHOULD NOT support Toggle Pattern");  
				Assert.IsNull (listItemChild.GetPatternProvider (TextPatternIdentifiers.Pattern.Id), //LAMESPEC: Should be true
				               "editItem in View.Details: SHOULD NOT support Text Pattern");
				Assert.IsNull (listItemChild.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id),
				               "editItem in View.Details: SHOULD NOT support Transform Pattern");
				Assert.IsNull (listItemChild.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id),
				               "editItem in View.Details: SHOULD NOT support Window Pattern");
			}
		}

		#endregion
		
		#region Basic Item Tests
		
		[Test]
		public void BasicItemPropertiesTest ()
		{
			ListView lisview = GetListView (3, 10, 4, 6);
			lisview.View = View.LargeIcon;
			lisview.ShowGroups = false;
			lisview.Scrollable = false;
			
			IRawElementProviderFragmentRoot rootProvider;
			ISelectionProvider selectionProvider;
			IRawElementProviderFragment child;

			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (lisview);

			selectionProvider = rootProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id) as ISelectionProvider;
			Assert.IsNotNull (selectionProvider, "Selection Provider for ListView");

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
			Application.EnableVisualStyles ();
			
			ListView listview = new ListView ();
			listview.View = View.LargeIcon;
			listview.ShowGroups = false;
			listview.Scrollable = false;

			IRawElementProviderFragmentRoot rootProvider;
			IRawElementProviderFragment child;
			IRawElementProviderFragment childParent;
			
			int elements = 10;
			int index = 0;
			string name = string.Empty;

			for (index = 0; index < elements; index++)
				listview.Items.Add (string.Format ("Element: {0}", index));
			index = 0;
			
			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (listview);

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
			listview.Items.Clear ();

			child = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (child, "We shouldn't have a child");
			
			// Lets use View.Details
			//(int groups, int items, int defaultGroupItems, int maxSubitems)
			int groupsCount = 3;
			int subItemsCount = 5;
			int defaultGroupItemsCount = 2;
			int itemsCount = 4;

			listview = GetListView (groupsCount, 
			                        itemsCount, 
			                        defaultGroupItemsCount, 
			                        subItemsCount);
			listview.ShowGroups = true;
			listview.View = View.Details;
			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (listview);

			IRawElementProviderFragment header = null;
			// +1 because of DetaulGroup
			IRawElementProviderFragment []groups = new IRawElementProviderFragment [groupsCount +1];
			
			index = 0;
			child = rootProvider.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				int ctype = (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
				if (ctype == ControlType.Header.Id) {
					if (header != null)
						Assert.Fail ("We already have an header");
					else {
						header = child;
						IRawElementProviderFragment headerChild = child.Navigate (NavigateDirection.FirstChild);
						Assert.IsNotNull (headerChild, "We should have HeaderItem");
						while (headerChild != null) {
							Assert.AreEqual (header, 
							                 headerChild.Navigate (NavigateDirection.Parent),
							                 "DataItemChild.Parent != DataItem");
							
							headerChild = headerChild.Navigate (NavigateDirection.NextSibling);
						}
					}
				} else if (ctype == ControlType.Group.Id) {
					if (index >= groups.Length)
						Assert.Fail (string.Format ("Index is greater than count: {0}>={1}", index, groups.Length));
					groups [index] = child;
					index++;
				}/*else if (ctype == ControlType.DataItem.Id) {
					// Validate DataItem navigation
					index++;
					IRawElementProviderFragment dataItemChild = child.Navigate (NavigateDirection.FirstChild);
					Assert.IsNotNull (dataItemChild, "We should have DataItem.Child");
					while (dataItemChild != null) {
						Assert.AreEqual (child, 
						                 dataItemChild.Navigate (NavigateDirection.Parent),
						                 "DataItemChild.Parent != DataItem");
						
						dataItemChild = dataItemChild.Navigate (NavigateDirection.NextSibling);
					}
				} */else if (ctype != ControlType.Group.Id
				           && ctype != ControlType.ScrollBar.Id)
					Assert.Fail (string.Format ("ControlTypes valid are ScrollBar, DataItem and Header: {0}",
					                            ControlType.LookupById (ctype).ProgrammaticName));

				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.AreEqual (index, groups.Length, "GroupsCount different");

			// We tests each Group and children
			foreach (IRawElementProviderFragment group in groups) {
				child = group.Navigate (NavigateDirection.FirstChild);
				while (child != null) {
					Assert.AreEqual (group,
					                 child.Navigate (NavigateDirection.Parent),
					                 "Child.Parent != Group");
					Assert.AreEqual (ControlType.DataItem.Id,
					                 (int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id),
					                 "ControlType != DataItem");

					// DataItem children
					IRawElementProviderFragment dataItemChild = child.Navigate (NavigateDirection.FirstChild);
					Assert.IsNotNull (dataItemChild, "DataItem.Child");
					while (dataItemChild != null) {
						Assert.AreEqual (child,
						                 dataItemChild.Navigate (NavigateDirection.Parent),
						                 "DataItem.Child.Parent != DataItem");
						dataItemChild = dataItemChild.Navigate (NavigateDirection.NextSibling);
					}
					
					child = child.Navigate (NavigateDirection.NextSibling);
				}
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
			
			ListView listView = (ListView) GetControlInstance ();
			listView.Location = new System.Drawing.Point (3, 3);
			listView.Size = new System.Drawing.Size (100, 50);
			listView.ShowGroups = false;
			provider = (IRawElementProviderFragmentRoot) GetProviderFromControl (listView);
			
			//We should show the ScrollBar
			for (int index = 30; index > 0; index--) {
				listView.Items.Add (string.Format ("Element {0}", index));
			}
			
			//WE DON'T KNOW THE ORDER OF NAVIGATION
			//However we know that we have "somewhere" one Vertical Scrollbar
			
			bool scrollbarFound = false;
			
			child = provider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (child, "FirstChild must not be null");
			int count = 0;
			while (child != null) {				
				if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) == ControlType.ScrollBar.Id
					&& (OrientationType) child.GetPropertyValue (AutomationElementIdentifiers.OrientationProperty.Id) == OrientationType.Vertical) {
					scrollbarFound = true;
					scrollBar = child;
				}
				
				//ALL elements must have same parent
				parent = child.Navigate (NavigateDirection.Parent);
				Assert.AreEqual (provider, parent, "Parents are different");
				Assert.AreEqual (child.FragmentRoot, parent, "Parents are different (FragmentRoot)");
				child = child.Navigate (NavigateDirection.NextSibling);
				count++;
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
			
			//EOF-Bug

			//Clearing elements should hide scroll and delete all items.
			listView.Items.Clear ();

			child = provider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (child, "Shouldn't be any children");

			//This won't add scrollbar
			listView.Items.Add ("1");
			listView.Items.Add ("2");
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

		#region Collection tests

		[Test]
		public void CollectionTest ()
		{
			ListView listView = (ListView) GetControlInstance ();
			listView.ShowGroups = false;

			IRawElementProviderFragmentRoot list
				= (IRawElementProviderFragmentRoot) GetProviderFromControl (listView);

			Assert.IsNotNull (list, "We must have a List");

			IRawElementProviderFragment item0
				= list.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (item0, "We shouldn't have children in List");

			//Add item 0. (Items.Add)
			
			bridge.ResetEventLists ();
			
			listView.Items.Add ("Item 0");
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
			listView.Items.Add ("Item 1");
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
			listView.Items.RemoveAt (0);

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
			listView.Items.AddRange (new ListViewItem [] { new ListViewItem ("Item 2"), new ListViewItem ("Item 3") });

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

			listView.Items [1] = new ListViewItem ("Item4");

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
			listView.Items.Clear ();

			Assert.AreEqual (1,
			                 bridge.GetStructureChangedEventCount (StructureChangeType.ChildrenBulkRemoved),
			                 "We should have 1 event");
			Assert.IsNull (list.Navigate (NavigateDirection.FirstChild),
			               "We shouldn't have children in List");
		}
		
		#endregion

		#region View.Details TablePattern
	
		[Test]
		public void ViewDetails_TablePatternTest ()
		{
			int maxSubitems = 3;

			ListView view = GetListView (3, 10, 4, maxSubitems);
			view.View = View.Details;
			view.ShowGroups = true;
			
			IRawElementProviderFragmentRoot viewProvider
				= (IRawElementProviderFragmentRoot) GetProviderFromControl (view);

			ITableProvider tableProvider 
				= viewProvider.GetPatternProvider (TablePatternIdentifiers.Pattern.Id) as ITableProvider;
			Assert.IsNotNull (tableProvider, "Table Provider for ListView");

			Assert.AreEqual (view.Columns.Count,
			                 tableProvider.ColumnCount, "TablePattern.ColumnCount");
			for (int i = 0; i < 10; i++)
				view.Items [0].SubItems.Add (string.Format ("new subitem {0}", i));
			Assert.AreEqual (view.Columns.Count,
			                 tableProvider.ColumnCount, "TablePattern.ColumnCount");

			Assert.AreEqual (view.Items.Count,
			                 tableProvider.RowCount, "TablePattern.RowCount");

			Assert.AreEqual (RowOrColumnMajor.RowMajor,
			                 tableProvider.RowOrColumnMajor, "TablePattern.RowOrColumnMajor");

			Assert.IsTrue (tableProvider.GetRowHeaders ().Length == 0, "TablePattern.GetRowHeaders");

			IRawElementProviderSimple []columnHeaders = tableProvider.GetColumnHeaders ();

			Assert.IsTrue (columnHeaders.Length > 0, 
			               string.Format ("TablePattern.GetColumnHeaders is not null: {0}", columnHeaders.Length));
			Assert.AreEqual (view.Columns.Count,
			                 columnHeaders.Length, "TablePattern.GetColumnHeaders count");

			//Column headers should point to all the HeaderItem found in Header
			IRawElementProviderFragment header = viewProvider.Navigate (NavigateDirection.FirstChild);
			while (header != null) {
				if ((int) header.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
					== ControlType.Header.Id)
					break;
				header = header.Navigate (NavigateDirection.NextSibling);
			}

			Assert.IsNotNull (header, "TablePattern we need a header");

			List<IRawElementProviderFragment> headerItems = new List<IRawElementProviderFragment> ();
			IRawElementProviderFragment headerItem = header.Navigate (NavigateDirection.FirstChild);
			while (headerItem != null) {
				headerItems.Add (headerItem);
				headerItem = headerItem.Navigate (NavigateDirection.NextSibling);
			}

			for (int columnHeader = 0; columnHeader < columnHeaders.Length; columnHeader++)				
				Assert.AreEqual (headerItems [columnHeader], columnHeaders [columnHeader], "TablePattern.GetColumnHeaders items");
		}

		#endregion


		[Test]
		public void Bug459306Test ()
		{
			ListView listView = (ListView) GetControlInstance ();
			listView.Location = new System.Drawing.Point (3, 3);
			listView.Size = new System.Drawing.Size (100, 50);
			listView.ShowGroups = false;
			listView.View  = View.Details;

			IRawElementProviderFragmentRoot provider
				= (IRawElementProviderFragmentRoot) GetProviderFromControl (listView);

			listView.Columns.Add (new ColumnHeader ("Element"));
			listView.Columns.Add (new ColumnHeader ("SubItem 0"));
			listView.Columns.Add (new ColumnHeader ("SubItem 1"));
			
			listView.Items.Add ("Element 0");
			listView.Items [0].SubItems.Add ("SubItem 0.0");
			listView.Items [0].SubItems.Add ("SubItem 0.1");

			//Search for the first DataItem
			IRawElementProviderFragment dataItem = null;
			IRawElementProviderFragment child 
				= provider.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.DataItem.Id) {
					dataItem = child;
					break;
				}
				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (dataItem, "ListItem SHOULD NOT be null");

			//Search for Edit subitems
			child = dataItem.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragment []edits = new IRawElementProviderFragment [3];
			int count = 0;
			while (child != null) {
				//We should only have 3 children, because we have 2 subitems and the main element
				edits [count++] = child;
				child = child.Navigate (NavigateDirection.NextSibling);
			}

			IValueProvider valueProvider0 
				= (IValueProvider) edits [0].GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider0, "Edit0 not supporting value Provider");

			IValueProvider valueProvider1 
				= (IValueProvider) edits [1].GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider1, "Edit1 not supporting value Provider");

			listView.LabelEdit = false;
			Assert.IsTrue (valueProvider0.IsReadOnly, "Edit0.IsReadOnly should be true");
			Assert.IsTrue (valueProvider1.IsReadOnly, "Edit1.IsReadOnly should be true");

			listView.LabelEdit = true;
			Assert.IsFalse (valueProvider0.IsReadOnly, "Edit0.IsReadOnly should be true");
			Assert.IsFalse (valueProvider1.IsReadOnly, "Edit1.IsReadOnly should be true");
		}

		#region BaseProviderTest Overrides

		protected override Control GetControlInstance ()
		{
			return new ListView ();
		}
		
		#endregion

		#region Private Methods

		private ListView GetListView (int groups, int items, int defaultGroupItems, int maxSubitems)
		{
			ListView view = new ListView ();
			view.Size = new System.Drawing.Size (300, 200);
			view.Location = new System.Drawing.Point (3, 3);

			//Groups
			for (int group = 0; group < groups; group++) {
				view.Groups.Add (new ListViewGroup (string.Format ("Group: {0}", group), HorizontalAlignment.Left));
				//Items
				for (int item = 0; item < items; item++) {
					ListViewItem viewItem = new ListViewItem (string.Format ("Item: {0}.{1}", group, item));
					viewItem.Group = view.Groups [group];
					view.Items.Add (viewItem);
					//Subitems
					if (item < maxSubitems) {
						for (int subitem = 0; subitem < item + 1; subitem++)
							view.Items [subitem].SubItems.Add (string.Format ("SubItem: {0}.{1}.{2}", group, item, subitem));
					}
				}
			}
			//Default items
			for (int item = 0; item < defaultGroupItems; item++)
				view.Items.Add (new ListViewItem (string.Format ("Default #{0}", item)));

			//Columns
			for (int column = 0; column < maxSubitems; column++) {
				view.Columns.Add (string.Format ("Column:{0}", column));
			}

			return view;
		}

		#endregion
	
	}
}
