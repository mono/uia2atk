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
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Forms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms.Client
{
	//According to http://msdn.microsoft.com/en-us/library/ms742462.aspx
	[TestFixture]
	[Description ("Tests SWF.ListView as ControlType.List & ControlType.DataGrid")]
	public class ListViewTest : ListBoxTest
	{

		#region Properties

		[Test]
		[LameSpec]
		[Description ("Value: True | Notes: The list control is always included in the control view of the UI Automation tree.")]
		public override void MsdnIsControlElementPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (true,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.IsControlElementProperty, true),
				"IsControlElementProperty");
		}

		#endregion

		#region Patterns

		[Test]
		[Description ("Support/Value: Depends. | Notes: Implement this control pattern if the control can "
			+ "support multiple views of the items in the container.")]
		public override void MsdnMultipleViewPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsTrue (SupportsPattern (element, MultipleViewPatternIdentifiers.Pattern),
				string.Format ("MultipleViewPattern SHOULD BE supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		[UISpyMissing]
		[Description ("Support/Value: Depends. | Notes: Implement this pattern when grid navigation needs "
			+"to be available on an item by item basis.")]
		public override void MsdnGridPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsTrue (SupportsPattern (element, GridPatternIdentifiers.Pattern),
				string.Format ("GridPattern SHOULD BE supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		#endregion

		#region MutipleView Pattern Tests

		[Test]
		[UISpyMissing]
		public void MultipleView_GetSupportedViewsTest ()
		{
			ListView view = GetControl () as ListView;
			int []supportedViews = null;
			AutomationElement element;
			MultipleViewPattern pattern;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				view.View = viewVal;
				element = GetAutomationElementFromControl (view);
				pattern = element.GetCurrentPattern (MultipleViewPatternIdentifiers.Pattern) as MultipleViewPattern;

				supportedViews = pattern.Current.GetSupportedViews ();
				Assert.AreEqual (1, supportedViews.Length, string.Format ("GetSupportedViews Length: {0}", viewVal));
				Assert.AreEqual (0, supportedViews [0], string.Format ("GetSupportedViews Value ", viewVal));
			}
		}

		[Test]
		[UISpyMissing]
		public void MultipleView_CurrentViewTest ()
		{
			ListView view = GetControl () as ListView;
			AutomationElement element;
			MultipleViewPattern pattern;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				view.View = viewVal;
				element = GetAutomationElementFromControl (view);
				pattern = element.GetCurrentPattern (MultipleViewPatternIdentifiers.Pattern) as MultipleViewPattern;

				Assert.AreEqual (0, pattern.Current.CurrentView, string.Format ("CurrentView Value = 0 -> {0}", view.View));
			}
		}

		[Test]
		public void MultipleView_ViewNameTest ()
		{
		    ListView view = GetControl () as ListView;
		    AutomationElement element;
		    MultipleViewPattern pattern;
			int []supportedViews = null;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				view.View = viewVal;
				element = GetAutomationElementFromControl (view);
				pattern = element.GetCurrentPattern (MultipleViewPatternIdentifiers.Pattern) as MultipleViewPattern;

				supportedViews = pattern.Current.GetSupportedViews ();
				int lastViewId = 0;
				foreach (int viewId in supportedViews) {
					pattern.SetCurrentView (viewId);
					Assert.AreEqual ("Icons", pattern.GetViewName (viewId), 
						string.Format ("GetViewName -> {0}", view.View));
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
			ListView view = GetControl () as ListView;
			AutomationElement element;
			MultipleViewPattern pattern;
			int [] supportedViews = null;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				view.View = viewVal;
				element = GetAutomationElementFromControl (view);
				pattern = element.GetCurrentPattern (MultipleViewPatternIdentifiers.Pattern) as MultipleViewPattern;

				supportedViews = pattern.Current.GetSupportedViews ();
				int lastViewId = 0;
				foreach (int viewId in supportedViews) {
					pattern.SetCurrentView (viewId);
					Assert.AreEqual ("Icons", pattern.GetViewName (viewId),
						string.Format ("GetViewName -> {0}", view.View));
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
			ListView view = GetControl () as ListView;
			AutomationElement element;
			ControlType defaultControlType = ControlType.List;

			foreach (View viewVal in Enum.GetValues (typeof (View))) {
				view.View = viewVal;
				element = GetAutomationElementFromControl (view);

				ControlType controlType
					= element.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty) as ControlType;

				if (view.View == View.Details)
					defaultControlType = ControlType.DataGrid;
				else
					defaultControlType = ControlType.List;

				Assert.AreEqual (defaultControlType, controlType,
					string.Format ("Different Control Type: {0} {1}", controlType.ProgrammaticName, view.View));
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

			AutomationElement element = GetAutomationElementFromControl (view);

			//When SWF.ListView.ShowGroups = true we only have Group or ScrollBar as children 
			AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild (element);
			view.ShowGroups = true;
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType != ControlType.ScrollBar && controlType != ControlType.Group)
					Assert.Fail (string.Format ("When SWF.ListView.ShowGroups = true we only have Group or ScrollBar as children: {0}",
						controlType.ProgrammaticName));

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}

			//Lets disable ShowGroups, we must not have groups!
			view.ShowGroups = false;
			child = TreeWalker.RawViewWalker.GetFirstChild (element);
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType == ControlType.Group)
					Assert.Fail (string.Format ("When SWF.ListView.ShowGroups = false we shouldn't have Group: {0}",
						controlType.ProgrammaticName));

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}

			view = GetListView (2, 17, 3, 3);
			view.View = View.LargeIcon;
			element = GetAutomationElementFromControl (view);

			//Control Type tests
			Assert.AreEqual (ControlType.List,
				element.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty), "View.LargeIcon: ControlType.List");
			Assert.AreEqual ("list",
				element.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty), "View.LargeIcon: list");

			Assert.AreEqual (true,
				SupportsPattern (element, SelectionPatternIdentifiers.Pattern), "View.LargeIcon: MUST support Selection Pattern");
			Assert.AreEqual (true,
				SupportsPattern (element, ScrollPatternIdentifiers.Pattern), "View.LargeIcon: MIGHT support Scroll Pattern");
			Assert.AreEqual (true,
				SupportsPattern (element, MultipleViewPatternIdentifiers.Pattern), "View.LargeIcon: MIGHT support MultipleView Pattern");
			Assert.AreEqual (false,
				SupportsPattern (element, GridPatternIdentifiers.Pattern), "View.LargeIcon: SHOULD NOT support GridPattern Pattern");
			Assert.AreEqual (false,
				SupportsPattern (element, TablePatternIdentifiers.Pattern), "View.LargeIcon: SHOULD NOT support TablePattern Pattern");

			//Lets test the group
			child = TreeWalker.RawViewWalker.GetFirstChild (element);
			AutomationElement group = null;
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType == ControlType.Group) {
					group = child;
					break;
				}

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}
			Assert.IsNotNull (group, "View.LargeIcon: Group should not be NULL");

			Assert.AreEqual (false,
				SupportsPattern (group, DockPatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support Dock Pattern");
			Assert.AreEqual (true,
				SupportsPattern (group, ExpandCollapsePatternIdentifiers.Pattern), "Group in View.LargeIcon: MUST support ExpandCollapse Pattern");
			Assert.AreEqual (true,
				SupportsPattern (group, GridPatternIdentifiers.Pattern), "Group in View.LargeIcon: MUST support Grid Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, GridItemPatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support GridItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, InvokePatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support Invoke Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, MultipleViewPatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support MultipleView Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, RangeValuePatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support RangeValue Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, SelectionPatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support Selection Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, SelectionItemPatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support SelectionItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, ScrollPatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support Scroll Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, ScrollItemPatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support ScrollItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TablePatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support Table Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TableItemPatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support TableItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TogglePatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support Toggle Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TextPatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support Text Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TransformPatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support Transform Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, ValuePatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support Value Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, WindowPatternIdentifiers.Pattern), "Group in View.LargeIcon: SHOULD NOT support Window Pattern");

			//Lets test the ListItem
			child = TreeWalker.RawViewWalker.GetFirstChild (group);
			AutomationElement listItem = null;
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType == ControlType.ListItem) {
					listItem = child;
					break;
				}

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}
			Assert.IsNotNull (listItem, "View.LargeIcon: ListItem should not be NULL");
			Assert.AreEqual (ControlType.ListItem,
				listItem.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty), "View.LargeIcon: ControlType.ListItem");
			Assert.AreEqual ("list item",
				listItem.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty), "View.LargeIcon: list item");

			Assert.AreEqual (false,
				SupportsPattern (listItem, DockPatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support Dock Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, ExpandCollapsePatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support ExpandCollapse Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, GridPatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support Grid Pattern");
			Assert.AreEqual (true,
				SupportsPattern (listItem, GridItemPatternIdentifiers.Pattern), "listItem in View.LargeIcon: MUST support GridItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, InvokePatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support Invoke Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, MultipleViewPatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support MultipleView Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, RangeValuePatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support RangeValue Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, SelectionPatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support Selection Pattern");
			Assert.AreEqual (true,
				SupportsPattern (listItem, SelectionItemPatternIdentifiers.Pattern), "listItem in View.LargeIcon: MUST support SelectionItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, ScrollPatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support Scroll Pattern");
			Assert.AreEqual (true,
				SupportsPattern (listItem, ScrollItemPatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support ScrollItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, TablePatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support Table Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, TableItemPatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support TableItem Pattern");

			view.CheckBoxes = true;
			Assert.AreEqual (true,
				SupportsPattern (listItem, TogglePatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support Toggle Pattern when view.CheckBoxes=true");
			view.CheckBoxes = false;
			Assert.AreEqual (false,
				SupportsPattern (listItem, TogglePatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support Toggle Pattern when view.CheckBoxes=false.");

			Assert.AreEqual (false,
				SupportsPattern (listItem, TextPatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support Text Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, TransformPatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support Transform Pattern");

			view.LabelEdit = true;
			Assert.AreEqual (true,
				SupportsPattern (listItem, ValuePatternIdentifiers.Pattern), "listItem in View.LargeIcon: MUST support Value Pattern when view.LabelEdit=true.");
			view.LabelEdit = false;
			Assert.AreEqual (false,
				SupportsPattern (listItem, ValuePatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support Value Pattern when view.LabelEdit=false.");

			Assert.AreEqual (false,
				SupportsPattern (listItem, WindowPatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support Window Pattern");

			//Children in ListItem
			view.CheckBoxes = false;
			AutomationElement listItemChild = TreeWalker.RawViewWalker.GetFirstChild (listItem);
			Assert.IsNull (listItemChild, "listItem in View.LargeIcon: No children");
			
			view.CheckBoxes = true;
			listItemChild = TreeWalker.RawViewWalker.GetFirstChild (listItem);
			Assert.IsNotNull (listItemChild, "listItem in View.LargeIcon: view.CheckBoxes=true We should have one child:");

			listItemChild = TreeWalker.RawViewWalker.GetNextSibling (listItemChild);
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

			AutomationElement element = GetAutomationElementFromControl (view);

			//When SWF.ListView.ShowGroups = true we only have Group or ScrollBar as children 
			view.ShowGroups = true;
			AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild (element);
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType != ControlType.ScrollBar && controlType != ControlType.Group)
					Assert.Fail (string.Format ("When SWF.ListView.ShowGroups = true we only have Group or ScrollBar as children: {0}",
						controlType.ProgrammaticName));

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}

			//Lets disable ShowGroups, we must not have groups!
			view.ShowGroups = false;
			child = TreeWalker.RawViewWalker.GetFirstChild (element);
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType == ControlType.Group)
					Assert.Fail (string.Format ("When SWF.ListView.ShowGroups = false we shouldn't have Group: {0}",
						controlType.ProgrammaticName));

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}

			view = GetListView (2, 17, 3, 3);
			view.View = View.LargeIcon;
			element = GetAutomationElementFromControl (view);

			//Control Type tests
			Assert.AreEqual (ControlType.List,
				element.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty), "View.SmallIcon: ControlType.List");
			Assert.AreEqual ("list",
				element.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty), "View.SmallIcon: list");

			Assert.AreEqual (true,
				SupportsPattern (element, SelectionPatternIdentifiers.Pattern), "View.SmallIcon: MUST support Selection Pattern");
			Assert.AreEqual (true,
				SupportsPattern (element, ScrollPatternIdentifiers.Pattern), "View.SmallIcon: MIGHT support Scroll Pattern");
			Assert.AreEqual (true,
				SupportsPattern (element, MultipleViewPatternIdentifiers.Pattern), "View.SmallIcon: MIGHT support MultipleView Pattern");
			Assert.AreEqual (false,
				SupportsPattern (element, GridPatternIdentifiers.Pattern), "View.SmallIcon: SHOULD NOT support GridPattern Pattern");
			Assert.AreEqual (false,
				SupportsPattern (element, TablePatternIdentifiers.Pattern), "View.SmallIcon: SHOULD NOT support TablePattern Pattern");

			//Lets test the group
			child = TreeWalker.RawViewWalker.GetFirstChild (element);
			AutomationElement group = null;
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType == ControlType.Group) {
					group = child;
					break;
				}

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}
			Assert.IsNotNull (group, "View.SmallIcon: Group should not be NULL");

			Assert.AreEqual (false,
				SupportsPattern (group, DockPatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support Dock Pattern");
			Assert.AreEqual (true,
				SupportsPattern (group, ExpandCollapsePatternIdentifiers.Pattern), "Group in View.SmallIcon: MUST support ExpandCollapse Pattern");
			Assert.AreEqual (true,
				SupportsPattern (group, GridPatternIdentifiers.Pattern), "Group in View.SmallIcon: MUST support Grid Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, GridItemPatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support GridItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, InvokePatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support Invoke Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, MultipleViewPatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support MultipleView Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, RangeValuePatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support RangeValue Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, SelectionPatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support Selection Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, SelectionItemPatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support SelectionItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, ScrollPatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support Scroll Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, ScrollItemPatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support ScrollItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TablePatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support Table Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TableItemPatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support TableItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TogglePatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support Toggle Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TextPatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support Text Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TransformPatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support Transform Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, ValuePatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support Value Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, WindowPatternIdentifiers.Pattern), "Group in View.SmallIcon: SHOULD NOT support Window Pattern");

			//Lets test the ListItem
			child = TreeWalker.RawViewWalker.GetFirstChild (group);
			AutomationElement listItem = null;
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType == ControlType.ListItem) {
					listItem = child;
					break;
				}

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}
			Assert.IsNotNull (listItem, "View.SmallIcon: ListItem should not be NULL");
			Assert.AreEqual (ControlType.ListItem,
				listItem.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty), "View.SmallIcon: ControlType.ListItem");
			Assert.AreEqual ("list item",
				listItem.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty), "View.SmallIcon: list item");

			Assert.AreEqual (false,
				SupportsPattern (listItem, DockPatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support Dock Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, ExpandCollapsePatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support ExpandCollapse Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, GridPatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support Grid Pattern");
			Assert.AreEqual (true,
				SupportsPattern (listItem, GridItemPatternIdentifiers.Pattern), "listItem in View.SmallIcon: MUST support GridItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, InvokePatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support Invoke Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, MultipleViewPatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support MultipleView Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, RangeValuePatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support RangeValue Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, SelectionPatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support Selection Pattern");
			Assert.AreEqual (true,
				SupportsPattern (listItem, SelectionItemPatternIdentifiers.Pattern), "listItem in View.SmallIcon: MUST support SelectionItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, ScrollPatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support Scroll Pattern");
			Assert.AreEqual (true,
				SupportsPattern (listItem, ScrollItemPatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support ScrollItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, TablePatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support Table Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, TableItemPatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support TableItem Pattern");

			view.CheckBoxes = false;
			Assert.AreEqual (false,
				SupportsPattern (listItem, TogglePatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support Toggle Pattern when view.CheckBoxes=false.");
			view.CheckBoxes = true;
			Assert.AreEqual (true,
				SupportsPattern (listItem, TogglePatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support Toggle Pattern when view.CheckBoxes=true");

			Assert.AreEqual (false,
				SupportsPattern (listItem, TextPatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support Text Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, TransformPatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support Transform Pattern");

			view.LabelEdit = false;
			Assert.AreEqual (false,
				SupportsPattern (listItem, ValuePatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support Value Pattern when view.LabelEdit=false.");
			view.LabelEdit = true;
			Assert.AreEqual (true,
				SupportsPattern (listItem, ValuePatternIdentifiers.Pattern), "listItem in View.SmallIcon: MUST support Value Pattern when view.LabelEdit=true.");

			Assert.AreEqual (false,
				SupportsPattern (listItem, WindowPatternIdentifiers.Pattern), "listItem in View.SmallIcon: SHOULD NOT support Window Pattern");

			//Children in ListItem
			view.CheckBoxes = false;
			AutomationElement listItemChild = TreeWalker.RawViewWalker.GetFirstChild (listItem);
			Assert.IsNull (listItemChild, "listItem in View.SmallIcon: No children");

			view.CheckBoxes = true;
			listItemChild = TreeWalker.RawViewWalker.GetFirstChild (listItem);
			Assert.IsNotNull (listItemChild, "listItem in View.SmallIcon: view.CheckBoxes=true We should have one child:");

			listItemChild = TreeWalker.RawViewWalker.GetNextSibling (listItemChild);
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

			AutomationElement element = GetAutomationElementFromControl (view);

			//SWF.ListView.ShowGroups is not used, so setting it true doesnt care, we only have ListItem and ScrollBars
			AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild (element);
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType != ControlType.ScrollBar && controlType != ControlType.ListItem)
					Assert.Fail (string.Format ("Only ListItem Group or ScrollBar as children are allowed: {0}",
						controlType.ProgrammaticName));

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}

			//Lets disable ShowGroups, we should have the same...
			view.ShowGroups = false;
			child = TreeWalker.RawViewWalker.GetFirstChild (element);
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType == ControlType.Group)
					Assert.Fail (string.Format ("Only ListItem Group or ScrollBar as children are allowed: {0}",
						controlType.ProgrammaticName));

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}

			view = GetListView (2, 17, 3, 3);
			view.View = View.List;
			element = GetAutomationElementFromControl (view);

			//Control Type tests
			Assert.AreEqual (ControlType.List,
				element.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty), "View.List: ControlType.List");
			Assert.AreEqual ("list",
				element.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty), "View.List: list");

			Assert.AreEqual (true,
				SupportsPattern (element, SelectionPatternIdentifiers.Pattern), "View.List: MUST support Selection Pattern");
			Assert.AreEqual (true,
				SupportsPattern (element, ScrollPatternIdentifiers.Pattern), "View.List: MIGHT support Scroll Pattern");
			Assert.AreEqual (true,
				SupportsPattern (element, MultipleViewPatternIdentifiers.Pattern), "View.List: MIGHT support MultipleView Pattern");
			Assert.AreEqual (true,
				SupportsPattern (element, GridPatternIdentifiers.Pattern), "View.List: MUST support GridPattern Pattern");
			Assert.AreEqual (false,
				SupportsPattern (element, TablePatternIdentifiers.Pattern), "View.List: SHOULD NOT support TablePattern Pattern");

			//Lets test the ListItem
			child = TreeWalker.RawViewWalker.GetFirstChild (element);
			AutomationElement listItem = null;
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType == ControlType.ListItem) {
					listItem = child;
					break;
				}

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}
			Assert.IsNotNull (listItem, "View.List: ListItem should not be NULL");
			Assert.AreEqual (ControlType.ListItem,
				listItem.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty), "View.List: ControlType.ListItem");
			Assert.AreEqual ("list item",
				listItem.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty), "View.List: list item");

			Assert.AreEqual (false,
				SupportsPattern (listItem, DockPatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support Dock Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, ExpandCollapsePatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support ExpandCollapse Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, GridPatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support Grid Pattern");
			Assert.AreEqual (true,
				SupportsPattern (listItem, GridItemPatternIdentifiers.Pattern), "listItem in View.List: MUST support GridItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, InvokePatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support Invoke Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, MultipleViewPatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support MultipleView Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, RangeValuePatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support RangeValue Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, SelectionPatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support Selection Pattern");
			Assert.AreEqual (true,
				SupportsPattern (listItem, SelectionItemPatternIdentifiers.Pattern), "listItem in View.List: MUST support SelectionItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, ScrollPatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support Scroll Pattern");
			Assert.AreEqual (true,
				SupportsPattern (listItem, ScrollItemPatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support ScrollItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, TablePatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support Table Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, TableItemPatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support TableItem Pattern");

			view.CheckBoxes = false;
			Assert.AreEqual (false,
				SupportsPattern (listItem, TogglePatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support Toggle Pattern when view.CheckBoxes=false.");
			view.CheckBoxes = true;
			Assert.AreEqual (true,
				SupportsPattern (listItem, TogglePatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support Toggle Pattern when view.CheckBoxes=true");

			Assert.AreEqual (false,
				SupportsPattern (listItem, TextPatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support Text Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, TransformPatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support Transform Pattern");

			view.LabelEdit = false;
			Assert.AreEqual (false,
				SupportsPattern (listItem, ValuePatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support Value Pattern when view.LabelEdit=false.");
			view.LabelEdit = true;
			Assert.AreEqual (true,
				SupportsPattern (listItem, ValuePatternIdentifiers.Pattern), "listItem in View.List: MUST support Value Pattern when view.LabelEdit=true.");

			Assert.AreEqual (false,
				SupportsPattern (listItem, WindowPatternIdentifiers.Pattern), "listItem in View.List: SHOULD NOT support Window Pattern");

			//Children in ListItem
			view.CheckBoxes = false;
			AutomationElement listItemChild = TreeWalker.RawViewWalker.GetFirstChild (listItem);
			Assert.IsNull (listItemChild, "listItem in View.List: No children");

			view.CheckBoxes = true;
			listItemChild = TreeWalker.RawViewWalker.GetFirstChild (listItem);
			Assert.IsNotNull (listItemChild, "listItem in View.List: view.CheckBoxes=true We should have one child:");

			listItemChild = TreeWalker.RawViewWalker.GetNextSibling (listItemChild);
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

			AutomationElement element = GetAutomationElementFromControl (view);

			//SWF.ListView.ShowGroups is not used, so setting it true doesnt care, we only have ListItem and ScrollBars
			AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild (element);
			while (child != null)
			{
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType != ControlType.ScrollBar && controlType != ControlType.Group)
					Assert.Fail (string.Format ("Only Group or ScrollBar as children are allowed: {0}",
						controlType.ProgrammaticName));

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}

			//Lets disable ShowGroups, we should have the same...
			view.ShowGroups = false;
			child = TreeWalker.RawViewWalker.GetFirstChild (element);
			while (child != null)
			{
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType == ControlType.Group)
					Assert.Fail (string.Format ("Only ListItem as children are allowed: {0}",
						controlType.ProgrammaticName));

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}

			view = GetListView (2, 17, 3, 3);
			view.View = View.Tile;
			element = GetAutomationElementFromControl (view);

			//Control Type tests
			Assert.AreEqual (ControlType.List,
				element.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty), "View.Tile: ControlType.List");
			Assert.AreEqual ("list",
				element.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty), "View.Tile: list");

			Assert.AreEqual (true,
				SupportsPattern (element, SelectionPatternIdentifiers.Pattern), "View.Tile: MUST support Selection Pattern");
			Assert.AreEqual (true,
				SupportsPattern (element, ScrollPatternIdentifiers.Pattern), "View.Tile: MIGHT support Scroll Pattern");
			Assert.AreEqual (true,
				SupportsPattern (element, MultipleViewPatternIdentifiers.Pattern), "View.Tile: MIGHT support MultipleView Pattern");
			Assert.AreEqual (false,
				SupportsPattern (element, GridPatternIdentifiers.Pattern), "View.Tile: SHOULD NOT support GridPattern Pattern");
			Assert.AreEqual (false,
				SupportsPattern (element, TablePatternIdentifiers.Pattern), "View.Tile: SHOULD NOT support TablePattern Pattern");

			//Lets test the group
			child = TreeWalker.RawViewWalker.GetFirstChild (element);
			AutomationElement group = null;
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType == ControlType.Group) {
					group = child;
					break;
				}

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}
			Assert.IsNotNull (group, "View.SmallIcon: Group should not be NULL");

			Assert.AreEqual (false,
				SupportsPattern (group, DockPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Dock Pattern");
			Assert.AreEqual (true,
				SupportsPattern (group, ExpandCollapsePatternIdentifiers.Pattern), "Group in View.Tile: MUST support ExpandCollapse Pattern");
			Assert.AreEqual (true,
				SupportsPattern (group, GridPatternIdentifiers.Pattern), "Group in View.Tile: MUST support Grid Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, GridItemPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support GridItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, InvokePatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Invoke Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, MultipleViewPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support MultipleView Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, RangeValuePatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support RangeValue Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, SelectionPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Selection Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, SelectionItemPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support SelectionItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, ScrollPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Scroll Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, ScrollItemPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support ScrollItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TablePatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Table Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TableItemPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support TableItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TogglePatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Toggle Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TextPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Text Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TransformPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Transform Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, ValuePatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Value Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, WindowPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Window Pattern");

			//Lets test the ListItem
			child = TreeWalker.RawViewWalker.GetFirstChild (group);
			AutomationElement listItem = null;
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType == ControlType.ListItem) {
					listItem = child;
					break;
				}

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}
			Assert.IsNotNull (listItem, "View.Tile: ListItem should not be NULL");
			Assert.AreEqual (ControlType.ListItem,
				listItem.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty), "View.Tile: ControlType.ListItem");
			Assert.AreEqual ("list item",
				listItem.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty), "View.Tile: list item");

			Assert.AreEqual (false,
				SupportsPattern (listItem, DockPatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support Dock Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, ExpandCollapsePatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support ExpandCollapse Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, GridPatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support Grid Pattern");
			Assert.AreEqual (true,
				SupportsPattern (listItem, GridItemPatternIdentifiers.Pattern), "listItem in View.Tile: MUST support GridItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, InvokePatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support Invoke Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, MultipleViewPatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support MultipleView Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, RangeValuePatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support RangeValue Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, SelectionPatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support Selection Pattern");
			Assert.AreEqual (true,
				SupportsPattern (listItem, SelectionItemPatternIdentifiers.Pattern), "listItem in View.Tile: MUST support SelectionItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, ScrollPatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support Scroll Pattern");
			Assert.AreEqual (true,
				SupportsPattern (listItem, ScrollItemPatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support ScrollItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, TablePatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support Table Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, TableItemPatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support TableItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, TogglePatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support Toggle Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, TextPatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support Text Pattern");
			Assert.AreEqual (false,
				SupportsPattern (listItem, TransformPatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support Transform Pattern");

			view.LabelEdit = false;
			Assert.AreEqual (false,
				SupportsPattern (listItem, ValuePatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support Value Pattern when view.LabelEdit=false.");
			view.LabelEdit = true;
			Assert.AreEqual (true,
				SupportsPattern (listItem, ValuePatternIdentifiers.Pattern), "listItem in View.Tile: MUST support Value Pattern when view.LabelEdit=true.");

			Assert.AreEqual (false,
				SupportsPattern (listItem, WindowPatternIdentifiers.Pattern), "listItem in View.Tile: SHOULD NOT support Window Pattern");

			//Children in ListItem
			view.CheckBoxes = false;
			AutomationElement listItemChild = TreeWalker.RawViewWalker.GetFirstChild (listItem);
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

			AutomationElement element = GetAutomationElementFromControl (view);

			//SWF.ListView.ShowGroups is not used, so setting it true doesnt care, we only have ListItem and ScrollBars
			AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild (element);
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType != ControlType.ScrollBar && controlType != ControlType.Group && controlType != ControlType.Header)
					Assert.Fail (string.Format ("Only Group, ScrollBar or Header as children are allowed: {0}",
						controlType.ProgrammaticName));

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}

			//Lets disable ShowGroups, we should have the same...
			view.ShowGroups = false;
			child = TreeWalker.RawViewWalker.GetFirstChild (element);
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType == ControlType.Group && controlType != ControlType.Header)
					Assert.Fail (string.Format ("Only ListItem and Header as children are allowed: {0}",
						controlType.ProgrammaticName));

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}

			view = GetListView (3, 17, 3, 10);
			view.View = View.Details;
			element = GetAutomationElementFromControl (view);

			//Control Type tests
			Assert.AreEqual (ControlType.DataGrid,
				element.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty), "View.Tile: ControlType.DataGrid");
			Assert.AreEqual ("data grid",
				element.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty), "View.Tile: data grid");

			Assert.AreEqual (true,
				SupportsPattern (element, SelectionPatternIdentifiers.Pattern), "View.Tile: MUST support Selection Pattern");
			Assert.AreEqual (true,
				SupportsPattern (element, ScrollPatternIdentifiers.Pattern), "View.Tile: MIGHT support Scroll Pattern");
			Assert.AreEqual (true,
				SupportsPattern (element, MultipleViewPatternIdentifiers.Pattern), "View.Tile: MIGHT support MultipleView Pattern");
			Assert.AreEqual (true,
				SupportsPattern (element, GridPatternIdentifiers.Pattern), "View.Tile: MUST support GridPattern Pattern");
			Assert.AreEqual (true,
				SupportsPattern (element, TablePatternIdentifiers.Pattern), "View.Tile: MUST support TablePattern Pattern");

			//Lets test the group
			child = TreeWalker.RawViewWalker.GetFirstChild (element);
			AutomationElement group = null;
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType == ControlType.Group) {
					group = child;
					break;
				}

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}
			Assert.IsNotNull (group, "View.SmallIcon: Group should not be NULL");

			Assert.AreEqual (false,
				SupportsPattern (group, DockPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Dock Pattern");
			Assert.AreEqual (true,
				SupportsPattern (group, ExpandCollapsePatternIdentifiers.Pattern), "Group in View.Tile: MUST support ExpandCollapse Pattern");
			Assert.AreEqual (true,
				SupportsPattern (group, GridPatternIdentifiers.Pattern), "Group in View.Tile: MUST support Grid Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, GridItemPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support GridItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, InvokePatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Invoke Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, MultipleViewPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support MultipleView Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, RangeValuePatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support RangeValue Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, SelectionPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Selection Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, SelectionItemPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support SelectionItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, ScrollPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Scroll Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, ScrollItemPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support ScrollItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TablePatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Table Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TableItemPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support TableItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TogglePatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Toggle Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TextPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Text Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, TransformPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Transform Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, ValuePatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Value Pattern");
			Assert.AreEqual (false,
				SupportsPattern (group, WindowPatternIdentifiers.Pattern), "Group in View.Tile: SHOULD NOT support Window Pattern");

			//Lets test the ListItem
			child = TreeWalker.RawViewWalker.GetFirstChild (group);
			AutomationElement dataItem = null;
			while (child != null) {
				ControlType controlType
					= (ControlType) child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);

				if (controlType == ControlType.DataItem) {
					dataItem = child;
					break;
				}

				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}
			Assert.IsNotNull (dataItem, "View.Tile: ListItem should not be NULL");
			Assert.AreEqual (ControlType.DataItem,
				dataItem.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty), "View.Details: ControlType.DataItem");
			Assert.AreEqual ("item", //LAMESPEC: SHOULD BE "data item" not "item"
				dataItem.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty), "View.Tile: data item");

			Assert.AreEqual (false,
				SupportsPattern (dataItem, DockPatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support Dock Pattern");
			Assert.AreEqual (false,
				SupportsPattern (dataItem, ExpandCollapsePatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support ExpandCollapse Pattern");
			Assert.AreEqual (false,
				SupportsPattern (dataItem, GridPatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support Grid Pattern");
			Assert.AreEqual (false,
				SupportsPattern (dataItem, GridItemPatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support GridItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (dataItem, InvokePatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support Invoke Pattern");
			Assert.AreEqual (false,
				SupportsPattern (dataItem, MultipleViewPatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support MultipleView Pattern");
			Assert.AreEqual (false,
				SupportsPattern (dataItem, RangeValuePatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support RangeValue Pattern");
			Assert.AreEqual (false,
				SupportsPattern (dataItem, SelectionPatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support Selection Pattern");
			Assert.AreEqual (true,
				SupportsPattern (dataItem, SelectionItemPatternIdentifiers.Pattern), "listItem in View.Details: MUST support SelectionItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (dataItem, ScrollPatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support Scroll Pattern");
			Assert.AreEqual (true,
				SupportsPattern (dataItem, ScrollItemPatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support ScrollItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (dataItem, TablePatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support Table Pattern");
			Assert.AreEqual (false,
				SupportsPattern (dataItem, TableItemPatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support TableItem Pattern");

			view.CheckBoxes = true;
			Assert.AreEqual (true,
				SupportsPattern (dataItem, TogglePatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support Toggle Pattern when view.CheckBoxes=true");
			view.CheckBoxes = false;
			Assert.AreEqual (false,
				SupportsPattern (dataItem, TogglePatternIdentifiers.Pattern), "listItem in View.LargeIcon: SHOULD NOT support Toggle Pattern when view.CheckBoxes=false.");

			Assert.AreEqual (false,
				SupportsPattern (dataItem, TextPatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support Text Pattern");
			Assert.AreEqual (false,
				SupportsPattern (dataItem, TransformPatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support Transform Pattern");

			view.LabelEdit = false;
			Assert.AreEqual (false,
				SupportsPattern (dataItem, ValuePatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support Value Pattern when view.LabelEdit=false.");
			view.LabelEdit = true;
			Assert.AreEqual (true,
				SupportsPattern (dataItem, ValuePatternIdentifiers.Pattern), "listItem in View.Details: MUST support Value Pattern when view.LabelEdit=true.");

			Assert.AreEqual (false,
				SupportsPattern (dataItem, WindowPatternIdentifiers.Pattern), "listItem in View.Details: SHOULD NOT support Window Pattern");

			//Header in ListView

			element = this.GetAutomationElementFromControl (view);
			AutomationElement headerElement = TreeWalker.RawViewWalker.GetFirstChild (element);
			while (headerElement != null) {
				ControlType ctype
					= (ControlType) headerElement.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);
				if (ctype == ControlType.Header)
					break;
				headerElement = TreeWalker.RawViewWalker.GetNextSibling (headerElement);
			}

			Assert.IsNotNull (headerElement, "We should have a Header");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, DockPatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support Dock Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, ExpandCollapsePatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support ExpandCollapse Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, GridPatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support Grid Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, GridItemPatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support GridItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, InvokePatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support Invoke Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, MultipleViewPatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support MultipleView Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, RangeValuePatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support RangeValue Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, SelectionPatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support Selection Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, SelectionItemPatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support SelectionItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, ScrollPatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support Scroll Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, ScrollItemPatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support ScrollItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, TablePatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support Table Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, TableItemPatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support TableItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, TogglePatternIdentifiers.Pattern), "headerElement in View.LargeIcon: SHOULD NOT support Toggle Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, TextPatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support Text Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, TransformPatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support Transform Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, ValuePatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support Value Pattern.");
			Assert.AreEqual (false,
				SupportsPattern (headerElement, WindowPatternIdentifiers.Pattern), "headerElement in View.Details: SHOULD NOT support Window Pattern");

			AutomationElement headerItem = TreeWalker.RawViewWalker.GetFirstChild (headerElement);
			Assert.IsNull (headerItem, "We SHOULD NOT HAVE HeaderItem");

			view.Columns.Add ("Column 0");

			headerItem = TreeWalker.RawViewWalker.GetFirstChild (headerElement);
			Assert.IsNotNull (headerItem, "We SHOULD HAVE HeaderItem");

			Assert.AreEqual (false,
				SupportsPattern (headerItem, DockPatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support Dock Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, ExpandCollapsePatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support ExpandCollapse Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, GridPatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support Grid Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, GridItemPatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support GridItem Pattern");
			Assert.AreEqual (true,
				SupportsPattern (headerItem, InvokePatternIdentifiers.Pattern), "headerItem in View.Details: MUST support Invoke Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, MultipleViewPatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support MultipleView Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, RangeValuePatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support RangeValue Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, SelectionPatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support Selection Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, SelectionItemPatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support SelectionItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, ScrollPatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support Scroll Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, ScrollItemPatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support ScrollItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, TablePatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support Table Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, TableItemPatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support TableItem Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, TogglePatternIdentifiers.Pattern), "headerItem in View.LargeIcon: SHOULD NOT support Toggle Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, TextPatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support Text Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, TransformPatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support Transform Pattern");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, ValuePatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support Value Pattern.");
			Assert.AreEqual (false,
				SupportsPattern (headerItem, WindowPatternIdentifiers.Pattern), "headerItem in View.Details: SHOULD NOT support Window Pattern");

			view.Columns.Clear ();

			//Children in ListItem

			view.ShowGroups = false;
			view.CheckBoxes = false;

			dataItem = TreeWalker.RawViewWalker.GetFirstChild (element);
			while (dataItem != null) {
				ControlType ctype 
					= (ControlType) dataItem.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);
				if (ctype == ControlType.DataItem)
					break;
				dataItem = TreeWalker.RawViewWalker.GetNextSibling (dataItem);
			}
			Assert.IsNotNull (dataItem, "We should have a DataItem");

			AutomationElement listItemChild = TreeWalker.RawViewWalker.GetFirstChild (dataItem);

			Assert.IsNull (listItemChild, "We SHOULD NOT have children because we don't have columns.");
			view.Columns.Add ("Column 0");

			listItemChild = TreeWalker.RawViewWalker.GetFirstChild (dataItem);

			if ((ControlType) listItemChild.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty)
					== ControlType.Edit) {
				Assert.IsNotNull (listItemChild, "We SHOULD HAVE children because we have columns.");
				Assert.AreEqual (ControlType.Edit,
					listItemChild.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty),
					string.Format ("View.Details: ControlType.Edit: {0}", ((ControlType) listItemChild.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty)).ProgrammaticName));
				Assert.AreEqual ("edit",
					listItemChild.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty), "View.Tile: edit");

				Assert.AreEqual (false,
					SupportsPattern (listItemChild, DockPatternIdentifiers.Pattern), "editItem in View.Details: SHOULD NOT support Dock Pattern");
				Assert.AreEqual (false,
					SupportsPattern (listItemChild, ExpandCollapsePatternIdentifiers.Pattern), "editItem in View.Details: SHOULD NOT support ExpandCollapse Pattern");
				Assert.AreEqual (false,
					SupportsPattern (listItemChild, GridPatternIdentifiers.Pattern), "editItem in View.Details: SHOULD NOT support Grid Pattern");
				Assert.AreEqual (true,
					SupportsPattern (listItemChild, GridItemPatternIdentifiers.Pattern), "editItem in View.Details: MUST support GridItem Pattern");
				Assert.AreEqual (false,
					SupportsPattern (listItemChild, InvokePatternIdentifiers.Pattern), "editItem in View.Details: SHOULD NOT support Invoke Pattern");
				Assert.AreEqual (false,
					SupportsPattern (listItemChild, MultipleViewPatternIdentifiers.Pattern), "editItem in View.Details: SHOULD NOT support MultipleView Pattern");
				Assert.AreEqual (false,
					SupportsPattern (listItemChild, RangeValuePatternIdentifiers.Pattern), "editItem in View.Details: SHOULD NOT support RangeValue Pattern");
				Assert.AreEqual (false,
					SupportsPattern (listItemChild, SelectionPatternIdentifiers.Pattern), "editItem in View.Details: SHOULD NOT support Selection Pattern");
				Assert.AreEqual (false,
					SupportsPattern (listItemChild, SelectionItemPatternIdentifiers.Pattern), "editItem in View.Details: SHOULD NOT support SelectionItem Pattern");
				Assert.AreEqual (false,
					SupportsPattern (listItemChild, ScrollPatternIdentifiers.Pattern), "editItem in View.Details: SHOULD NOT support Scroll Pattern");
				Assert.AreEqual (false,
					SupportsPattern (listItemChild, ScrollItemPatternIdentifiers.Pattern), "editItem in View.Details: SHOULD NOT support ScrollItem Pattern");
				Assert.AreEqual (false,
					SupportsPattern (listItemChild, TablePatternIdentifiers.Pattern), "editItem in View.Details: SHOULD NOT support Table Pattern");
				Assert.AreEqual (true,
					SupportsPattern (listItemChild, TableItemPatternIdentifiers.Pattern), "editItem in View.Details: MUST support TableItem Pattern");
				Assert.AreEqual (false,
					SupportsPattern (listItemChild, TogglePatternIdentifiers.Pattern), "editItem in View.LargeIcon: SHOULD NOT support Toggle Pattern when view.CheckBoxes=true");
				Assert.AreEqual (false, //LAMESPEC: Should be true
					SupportsPattern (listItemChild, TextPatternIdentifiers.Pattern), "editItem in View.Details: SHOULD NOT support Text Pattern");
				Assert.AreEqual (false,
					SupportsPattern (listItemChild, TransformPatternIdentifiers.Pattern), "editItem in View.Details: SHOULD NOT support Transform Pattern");
				Assert.AreEqual (true,
					SupportsPattern (listItemChild, ValuePatternIdentifiers.Pattern), "editItem in View.Details: MUST support Value Pattern when view.LabelEdit=false.");
				Assert.AreEqual (false,
					SupportsPattern (listItemChild, WindowPatternIdentifiers.Pattern), "editItem in View.Details: SHOULD NOT support Window Pattern");
			}
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

			AutomationElement element = GetAutomationElementFromControl (view);

			Assert.AreEqual (true,
				SupportsPattern (element, TablePatternIdentifiers.Pattern), "element: SHOULD support Tabble Pattern");

			TablePattern pattern = element.GetCurrentPattern (TablePatternIdentifiers.Pattern) as TablePattern;

			Assert.AreEqual (view.Columns.Count,
				pattern.Current.ColumnCount, "TablePattern.ColumnCount");
			for (int i = 0; i < 10; i++)
				view.Items [0].SubItems.Add (string.Format ("new subitem {0}", i));
			Assert.AreEqual (view.Columns.Count,
				pattern.Current.ColumnCount, "TablePattern.ColumnCount");

			Assert.AreEqual (view.Items.Count,
				pattern.Current.RowCount, "TablePattern.RowCount");

			Assert.AreEqual (RowOrColumnMajor.RowMajor,
				pattern.Current.RowOrColumnMajor, "TablePattern.RowOrColumnMajor");

			Assert.AreEqual (true,
				pattern.Current.GetRowHeaders ().Length == 0, "TablePattern.GetRowHeaders");

			AutomationElement []columnHeaders = pattern.Current.GetColumnHeaders ();

			Assert.IsNotNull (columnHeaders, "TablePattern.GetColumnHeaders is not null");
			Assert.AreEqual (view.Columns.Count,
				columnHeaders.Length, "TablePattern.GetColumnHeaders count");

			//Column headers should point to all the HeaderItem found in Header
			AutomationElement header = TreeWalker.RawViewWalker.GetFirstChild (element);
			while (header != null) {
				if (header.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty)
					== ControlType.Header)
					break;
				header = TreeWalker.RawViewWalker.GetNextSibling (header);
			}

			Assert.IsNotNull (header, "TablePattern we need a header");

			List<AutomationElement> headerItems = new List<AutomationElement> ();
			AutomationElement headerItem = TreeWalker.RawViewWalker.GetFirstChild (header);
			while (headerItem != null) {
				headerItems.Add (headerItem);
				headerItem = TreeWalker.RawViewWalker.GetNextSibling (headerItem);
			}

			for (int columnHeader = 0; columnHeader < columnHeaders.Length; columnHeader++)				
				Assert.AreEqual (headerItems [columnHeader], columnHeaders [columnHeader], "TablePattern.GetColumnHeaders items");
		}

		#endregion

		#region Private Methods

		private ListView GetListView (int groups, int items, int defaultGroupItems, int maxSubitems)
		{
			ListView view = new ListView ();
			view.Size = new Size (300, 200);
			view.Location = new Point (3, 3);

			//Groups
			for (int group = 0; group < groups; group++) {
				view.Groups.Add (new ListViewGroup (string.Format ("Group: {0}", group), HorizontalAlignment.Left));
				//Items
				for (int item = 0; item < items; item++) {
					view.Items.Add (new ListViewItem (string.Format ("Item: {0}.{1}", group, item)));
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
			for (int columns = 0; columns < maxSubitems; columns++)
				view.Columns.Add (string.Format ("Column {0}", columns));

			return view;
		}

		#endregion

		#region Protected Methods

		protected override Control GetControl ()
		{
			ListView listview = new ListView ();
			listview.View = View.LargeIcon;
			listview.Items.Add (new ListViewItem (new string [] {"1", "2", "3", "4", "5", "6"}));
			listview.Size = new Size (100, 100);
			listview.Location = new Point (3, 3);
			return listview;
		}

		#endregion
	}
}
