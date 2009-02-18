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
//      Andres G. Aragoneses <aaragoneses@novell.com>
//      Michael Gorse <mgorse@novell.com>
// 

using System;
using System.Collections.Generic;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

using System.Threading;

//only used for the regression test:
using SWF = System.Windows.Forms;
using System.Drawing;


namespace UiaAtkBridgeTest
{

	[TestFixture]
	public class BridgeTests : BridgeTester
	{
		
		[Test]
		public void ListBox ()
		{
			ListBox (false);
		}

		private void ListBox (bool listView)
		{
			BasicWidgetType type = BasicWidgetType.ListBox;
			BasicWidgetType accessibleType = (listView? BasicWidgetType.ListView: type);
			Atk.Object accessible;
			
			string[] names = new string [] { simpleTestText, "Second Item", "Last Item" };
			accessible = GetAccessible (accessibleType, names);
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);

			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (3, accessible.NAccessibleChildren, "ListBox#RO numChildren");
			
			Atk.Object listItemChild = accessible.RefAccessibleChild (0);
			Assert.IsNotNull (listItemChild, "ListBox child#0 should not be null");
			Assert.AreEqual (Atk.Role.TableCell, listItemChild.Role, "ListBox child#0 should be a table cell");
			
			States (listItemChild, 
			  Atk.StateType.Enabled,
			  Atk.StateType.Focusable,
			  Atk.StateType.Selectable,
			  Atk.StateType.Sensitive,
			  Atk.StateType.SingleLine,
			  Atk.StateType.Showing,
			  Atk.StateType.Transient,
			  Atk.StateType.Visible);

			Assert.AreEqual (0, listItemChild.NAccessibleChildren, "ListBox ListItem numChildren");

			Atk.SelectionImplementor selection = accessible as Atk.SelectionImplementor;
			Assert.IsNotNull (selection, "ListBox Atk.Selection should not be null");
			InterfaceSelection (new Atk.SelectionAdapter(selection), names, accessible, type);

			// Below line needed because InterfaceAction tests that first item is not selected, so that it can test the action
			selection.AddSelection(1);
			InterfaceAction (BasicWidgetType.ListItem, CastToAtkInterface <Atk.Action> (listItemChild), listItemChild);

			InterfaceText (BasicWidgetType.ListItem, true, listItemChild);

			listItemChild = accessible.RefAccessibleChild (1);
			Focus (listItemChild);

			Parent (type, accessible);
		}

		[Test]
		public void CheckedListBox ()
		{
			CheckedListBox (false);
		}

		private void CheckedListBox (bool listView)
		{
			BasicWidgetType type = BasicWidgetType.CheckedListBox;
			BasicWidgetType accessibleType = (listView? BasicWidgetType.ListView: type);
			Atk.Object accessible;
			
			string[] names = new string[] { simpleTestText, "Second Item", "Last Item" };
			accessible = GetAccessible (accessibleType, names);
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);

			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (3, accessible.NAccessibleChildren, "ListBox#RO numChildren");
			
			Atk.Object listItemChild = accessible.RefAccessibleChild (0);
			Assert.IsNotNull (listItemChild, "ListBox child#0 should not be null");
			Assert.AreEqual (listItemChild.Role, Atk.Role.CheckBox, "ListBox child#0 should be a check box");
			
			States (listItemChild,
				Atk.StateType.Enabled,
				Atk.StateType.Focusable,
				Atk.StateType.Selectable,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.SingleLine,
				Atk.StateType.Transient,
				Atk.StateType.Visible);

			Assert.AreEqual (0, listItemChild.NAccessibleChildren, "ListBox ListItem numChildren");

			Atk.SelectionImplementor selection = accessible as Atk.SelectionImplementor;
			Assert.IsNotNull (selection, "ListBox Atk.Selection should not be null");
			InterfaceSelection (new Atk.SelectionAdapter(selection), names, accessible, type);

			// Below line needed because InterfaceAction tests that first item is not selected, so that it can test the action
			selection.AddSelection(1);
			InterfaceAction (BasicWidgetType.CheckedListItem, CastToAtkInterface <Atk.Action> (listItemChild), listItemChild);

			InterfaceText (BasicWidgetType.CheckedListItem, true, listItemChild);

			Parent (type, accessible);
		}

		[Test]
		public void LinkLabel ()
		{
			UiaAtkBridge.Hyperlink hyperlink;
			hyperlink = (UiaAtkBridge.Hyperlink) GetAdapterForWidget (linklab1);
			Atk.Text atkText = CastToAtkInterface<Atk.Text> (hyperlink);

			States (hyperlink,
				Atk.StateType.Enabled,
				Atk.StateType.Focusable,
				Atk.StateType.MultiLine,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible);

			Assert.AreEqual (53, atkText.CharacterCount, "LinkLabel character count");
			Assert.AreEqual ("openSUSE", atkText.GetText (0, 8), "LinkLabel GetText");
			Atk.HypertextAdapter hypertext = new Atk.HypertextAdapter (hyperlink);
			Assert.AreEqual (2, hypertext.NLinks, "LinkLabel NLinks");
			Assert.AreEqual (-1, hypertext.GetLinkIndex (8), "GetLinkIndex #1");
			Assert.AreEqual (0, hypertext.GetLinkIndex (9), "GetLinkIndex #2");
			Assert.AreEqual (-1, hypertext.GetLinkIndex (25), "GetLinkIndex #3");
			Assert.AreEqual (1, hypertext.GetLinkIndex (35), "GetLinkIndex #4");
			Assert.AreEqual (1, hypertext.GetLinkIndex (42), "GetLinkIndex #5");
			Assert.AreEqual (-1, hypertext.GetLinkIndex (51), "GetLinkIndex #6");
			Atk.Hyperlink link1 = hypertext.GetLink (0);
			Assert.AreEqual ("www.opensuse.org", link1.GetUri (0), "Link 1 uri");
			Assert.IsNull (link1.GetUri (1), "LinkLabel GetUri OOR #1");
			Assert.IsNull (link1.GetUri (-1), "LinkLabel GetUri OOR #2");
			Assert.AreEqual (9, link1.StartIndex, "Link 1 StartIndex");
			Assert.AreEqual (25, link1.EndIndex, "Link 1 EndIndex");
			Atk.Object obj1 = link1.GetObject (0);
			Assert.IsNotNull (obj1, "LinkLabel GetObject #1");
			Assert.IsFalse (obj1.RefStateSet().ContainsState (Atk.StateType.Enabled), "RefStateSet().Contains(Enabled)");
			Assert.AreEqual ("www.opensuse.org", obj1.Name, "Link 1 obj name");
			Assert.IsNull (link1.GetObject (1), "GetObject OOR #1");
			Assert.IsNull (link1.GetObject (-1), "GetObject OOR #2");
			Atk.Hyperlink link2 = hypertext.GetLink (1);
			Assert.AreEqual ("gmail.novell.com", link2.GetUri (0), "Link 1 uri");
			Assert.AreEqual (35, link2.StartIndex, "Link 1 StartIndex");
			Assert.AreEqual (51, link2.EndIndex, "Link 1 EndIndex");
			Atk.Object obj2 = link2.GetObject (0);
			Assert.IsNotNull (obj2, "LinkLabel GetObject #2");
			Assert.IsTrue (obj2.RefStateSet ().ContainsState (Atk.StateType.Enabled), "RefStateSet().Contains(Enabled)");
			Assert.AreEqual ("gmail.novell.com", obj2.Name, "Link 2 obj name");
			
			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (obj2);
			Assert.AreEqual (1, atkAction.NActions, "LinkLabel link NActions");
			Assert.IsTrue (atkAction.DoAction (0), "LinkLabel DoAction #1");
			Assert.IsFalse (atkAction.DoAction (1), "LinkLabel DoAction OOR #1");
			Assert.IsFalse (atkAction.DoAction (-1), "LinkLabel DoAction OOR #2");
			Assert.AreEqual (1, lastClickedLink, "LastClickedLink");
		}

		[Test]
		public void GroupBox ()
		{
			BasicWidgetType type = BasicWidgetType.GroupBox;
			Atk.Object accessible = GetAdapterForWidget (gb1);

			PropertyRole (type, accessible);

			States (accessible,
			  Atk.StateType.Enabled,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);
		}

		[Test]
		public void ToolStripComboBoxSimple ()
		{
			ComboBoxSimple (toolStripComboBoxSim);
		}

		[Test]
		public void ToolStripComboBoxDropDownList ()
		{
			ComboBoxDropDownList (toolStripComboBoxDDL);
		}

		[Test]
		public void ToolStripComboBoxDropDown ()
		{
			ComboBoxDropDownEntry (toolStripComboBoxDD);
		}

		[Test]
		public void ToolStripLabel ()
		{
			Label (BasicWidgetType.ToolStripLabel);
		}

		[Test]
		public void ListView2 ()
		{
			lv1.Items.Clear ();
			lv1.Scrollable = false;
			lv1.Groups.Clear ();
			lv1.View = SWF.View.SmallIcon;
			lv1.Groups.Add (new SWF.ListViewGroup ("group1"));
			lv1.Groups.Add (new SWF.ListViewGroup ("group2"));
			lv1.Items.Add("item1");
			lv1.Items.Add("item2");
			lv1.Items.Add("item3");
			lv1.Items.Add("item4");
			lv1.Items[0].Group = lv1.Groups[0];
			lv1.Items[1].Group = lv1.Groups[0];
			lv1.Items[2].Group = lv1.Groups[1];
			lv1.Items[3].Group = lv1.Groups[1];
			lv1.MultiSelect = false;
			lv1.CheckBoxes = false;
			Atk.Object accessible = GetAdapterForWidget (lv1);
			Assert.IsNotNull (accessible, "Adapter should not be null");
			// 2 groups
			Assert.AreEqual (2, accessible.NAccessibleChildren, "NAccessibleChildren #1");
			Atk.Object group1 = FindObjectByName (accessible, "group1");
			Assert.IsNotNull (group1, "FindObjectByName (group1)");
			Assert.AreEqual (Atk.Role.LayeredPane, group1.Role, "Group1 role");
			Atk.Object item1 = FindObjectByName (group1, "item1");
			Assert.IsNotNull (item1, "FindObjectByName (item1)");
			Assert.AreEqual (Atk.Role.TableCell, item1.Role, "Item1 role");
			Atk.Selection atkSelection = CastToAtkInterface<Atk.Selection> (item1.Parent);
			string [] names = { "item1", "item2" };
			InterfaceSelection (atkSelection, names, item1.Parent, BasicWidgetType.GroupBox);
			accessible = group1 = item1 = null;
			lv1.CheckBoxes = true;
			accessible = GetAdapterForWidget (lv1);
			group1 = FindObjectByName (accessible, "group1");
			item1 = FindObjectByName (group1, "item1");
			lv1.Items[0].Checked = true;
			// TODO: Is SingleLine appropriate?
			States (item1,
				Atk.StateType.Checked,
				Atk.StateType.Enabled,
				Atk.StateType.Focusable,
				Atk.StateType.Focused,
				Atk.StateType.Selectable,
				Atk.StateType.Selected,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.SingleLine,
				Atk.StateType.Transient,
				Atk.StateType.Visible);
			lv1.Items[0].Checked = false;
			States (item1,
				Atk.StateType.Enabled,
				Atk.StateType.Focusable,
				Atk.StateType.Focused,
				Atk.StateType.Selectable,
				Atk.StateType.Selected,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.SingleLine,
				Atk.StateType.Transient,
				Atk.StateType.Visible);


			Atk.Table atkTable = CastToAtkInterface<Atk.Table> (group1);
			Assert.AreEqual (2, atkTable.NRows * atkTable.NColumns, "Table NRows*NCols");
			// Gail Indicies when using RefAt 0-based
			Assert.AreEqual ("item1", atkTable.RefAt (0, 0).Name, "Cell (0, 0)");
			int row2 = (atkTable.NRows == 2? 1: 0);
			int col2 = (atkTable.NRows == 2? 0: 1);
			Assert.AreEqual ("item2", atkTable.RefAt (row2, col2).Name, "Cell ("+row2+", "+col2 + ")");
			// Gail Indicies when using GetIndexAt are 1-based
			Assert.AreEqual (1, atkTable.GetIndexAt (0, 0), "GetIndexAt (0, 0)");
			if (atkTable.NRows == 2)
				Assert.AreEqual (2, atkTable.GetIndexAt (1, 0), "GetIndexAt (1, 0)");
			else
				Assert.AreEqual (2, atkTable.GetIndexAt (0, 1), "GetIndexAt (1, 0)");
			Atk.Object group2 = FindObjectByName (accessible, "group2");
			atkTable = CastToAtkInterface<Atk.Table> (group2);
			// Gail Indicies when using RefAt 0-based
			Assert.AreEqual ("item3", atkTable.RefAt (0, 0).Name, "Cell (1, 0)");
			Assert.AreEqual ("item4", atkTable.RefAt (row2, col2).Name, "Cell ("+row2+", " +col2 + ")");
			// Gail Indicies when using GetIndexAt are 1-based
			Assert.AreEqual (1, atkTable.GetIndexAt (0, 0), "GetIndexAt (0, 0)");
			Assert.AreEqual (2, atkTable.GetIndexAt (row2, col2), "GetIndexAt (" + row2 + ", " + col2);

			// Changing groups again
			lv1.Items[2].Group = lv1.Groups[0];
			lv1.Items[3].Group = lv1.Groups[0];
			atkTable = CastToAtkInterface<Atk.Table> (group1);

			Assert.AreEqual (1, accessible.NAccessibleChildren, "NAccessibleChildren #2");

			Assert.AreEqual (4, atkTable.NRows * atkTable.NColumns, "Table NRows * NColumns");
			// Gail Indicies when using RefAt 0-based
			Assert.AreEqual ("item1", atkTable.RefAt (0, 0).Name, "Cell (0, 0)");
			Assert.AreEqual ("item2", atkTable.RefAt (row2, col2).Name, "Cell ("+row2+", " +col2 + ")");
			row2 = (atkTable.NRows == 4? 2: 1);
			col2 = 0;
			Assert.AreEqual ("item3", atkTable.RefAt (row2, col2).Name, "Cell ("+row2+", " +col2 + ")");
			row2 = (atkTable.NRows == 4? 3: 1);
			col2 = (atkTable.NRows == 4? 0: 1);
			Assert.AreEqual ("item4", atkTable.RefAt (row2, col2).Name, "Cell ("+row2+", " +col2 + ")");
			// Gail Indicies when using GetIndexAt are 1-based
			Assert.AreEqual (1, atkTable.GetIndexAt (0, 0), "GetIndexAt (0, 0)");
			row2 = (atkTable.NRows == 4? 1: 0);
			col2 = (atkTable.NRows == 4? 0: 1);
			Assert.AreEqual (2, atkTable.GetIndexAt (row2, col2), "GetIndexAt (" + row2 + ", " + col2);
			row2 = (atkTable.NRows == 4? 2: 1);
			col2 = 0;
			Assert.AreEqual (3, atkTable.GetIndexAt (row2, col2), "GetIndexAt (" + row2 + ", " + col2);
			row2 = (atkTable.NRows == 4? 3: 1);
			col2 = (atkTable.NRows == 4? 0: 1);
			Assert.AreEqual (4, atkTable.GetIndexAt (row2, col2), "GetIndexAt (" + row2 + ", " + col2);

			accessible = group1 = item1 = null;
			atkSelection = null;
			atkTable = null;
			lv1.View = SWF.View.List;
			CheckedListBox (true);
			lv1.CheckBoxes = false;
			ListBox (true);
		}

		[Test]
		public void DataGridView ()
		{
			BasicWidgetType accesibleType = BasicWidgetType.DataGridView;
			Atk.Object accessible;

			string name 
				= "<table><th><td>checkbox|Available</td><td>image|Cover</td><td>textbox|Title</td>"
				+"<td>link|Website</td><td>combobox|Year|1998,1999,2000,2001,2002</td><td>button|Request Header|Request</td></th>"
				+"<tr><td>true</td><td>null</td><td>Programming Windows</td><td>http://www.microsoft.com/</td><td>1998</td><td>Request</td></tr>"
				+"<tr><td>false</td><td>Code: The Hidden Language of Computer Hardware and Software</td><td>null</td><td>http://www.computer.com/</td><td>2000</td><td>Request</td></tr>"
				+"<tr><td>true</td><td>Coding Techniques for Microsoft Visual Basic .NET</td><td>null</td><td>http://www.microsoft.com/</td><td>2001</td><td>Request</td></tr>"
				+"<tr><td>true</td><td>Programming Windows with C#</td><td>null</td><td>http://www.microsoft.com/</td><td>2001</td><td>Request</td></tr>"
				+"<tr><td>true</td><td>C# for Java Developers</td><td>null</td><td>http://www.microsoft.com/</td><td>2002</td><td>Request</td></tr></table>";

			accessible = GetAccessible (accesibleType, name);
			Assert.IsNotNull (accessible, "DataGridView Adapter should not be null");

			// 5 rows (6 columns per row) and 1 header (6 header items)
			Assert.AreEqual (36, accessible.NAccessibleChildren, "NAccessibleChildren #1");
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (accesibleType, atkComponent);
			PropertyRole (accesibleType, accessible);

			// CheckBox Tests
			Atk.Object checkBox = FindObjectByName (accessible, "false");
			PropertyRole (BasicWidgetType.CheckBox, checkBox);
			Atk.Component checkBoxComponent = CastToAtkInterface <Atk.Component> (checkBox);
			InterfaceComponent (BasicWidgetType.CheckBox, checkBoxComponent);
			
			// Image tests
			Atk.Object pictureBox = FindObjectByName (accessible, "null");
			PropertyRole (BasicWidgetType.PictureBox, pictureBox);
			Atk.Component pictureBoxComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (BasicWidgetType.PictureBox, pictureBoxComponent);

			// Button tests
			Atk.Object button = FindObjectByName (accessible, "Request");
			PropertyRole (BasicWidgetType.NormalButton, button);
			Assert.AreEqual (0, button.NAccessibleChildren, "Button numChildren");
			Interfaces (button,
			            typeof (Atk.Image),
			            typeof (Atk.Component),
			            typeof (Atk.Action),
			            typeof (Atk.Text));
			InterfaceText (button, "Request");

			// Link tests
			Atk.Object link = FindObjectByName (accessible, "http://www.microsoft.com/");
			States (link,
				Atk.StateType.Enabled,
				Atk.StateType.MultiLine,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible,
			    Atk.StateType.Selectable);

			// Table tests			
			Atk.Object header = FindObjectByRole (accessible, Atk.Role.TableColumnHeader);
			Assert.IsNotNull (header, "Header not null");
			States (header,
			    Atk.StateType.Selectable,
				Atk.StateType.Enabled,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible);
			
			Atk.Table atkTable = CastToAtkInterface<Atk.Table> (accessible);

//			InterfaceTable (atkTable, 5, 6, 0, 0, false);
			
			Assert.AreEqual (5, atkTable.NRows, "Table NRows");
			Assert.AreEqual (6, atkTable.NColumns, "Table NColumns");

			Assert.AreEqual ("Available", atkTable.RefAt (0, 0).Name, "Cell (0, 0)");
			Assert.AreEqual ("Cover", atkTable.RefAt (0, 1).Name, "Cell (0, 1)");
			Assert.AreEqual ("Title", atkTable.RefAt (0, 2).Name, "Cell (0, 2)");
			Assert.AreEqual ("Website", atkTable.RefAt (0, 3).Name, "Cell (0, 3)");
			Assert.AreEqual ("Year", atkTable.RefAt (0, 4).Name, "Cell (0, 4)");
			Assert.AreEqual ("Request Header", atkTable.RefAt (0, 5).Name, "Cell (0, 5)");
		}

		[Test]
		public void DomainUpDown ()
		{
			BasicWidgetType type = BasicWidgetType.DomainUpDown;

			string [] names = { "first item", "second item", "third item" };
			Atk.Object accessible = GetAccessible (type, names);
			PropertyRole (type, accessible);

			Assert.AreEqual (3, accessible.NAccessibleChildren, "NAccessibleChildren");

			Atk.Selection atkSelection = CastToAtkInterface<Atk.Selection> (accessible);
			InterfaceSelection (atkSelection, names, accessible, type);

			dud1.SelectedIndex = 1;
			InterfaceText (accessible, "second item");

			InterfaceEditableText (type, accessible);

			dud1.ReadOnly = false;
			States (accessible,
				Atk.StateType.Editable,
				Atk.StateType.Enabled,
				Atk.StateType.Focusable,
				Atk.StateType.Focused,
				Atk.StateType.ManagesDescendants,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.SingleLine,
				Atk.StateType.Visible);

			dud1.ReadOnly = true;
			States (accessible,
				Atk.StateType.Enabled,
				Atk.StateType.Focusable,
				Atk.StateType.Focused,
				Atk.StateType.ManagesDescendants,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.SingleLine,
				Atk.StateType.Visible);

			EditReadOnly (accessible);

			Atk.Object listItemChild = accessible.RefAccessibleChild (1);
			Focus (listItemChild);

			for (int i = names.Length - 1; i >= 0; i--)
				dud1.Items.RemoveAt (i);
			Assert.AreEqual (0, accessible.NAccessibleChildren, "NAccessibleChildren after clear [need SWF fix]");
		}

		[Test]
		[Ignore ("Dep. on BNC#463299")]
		public void DomainUpDownClear ()
		{
			dud1.Items.Add ("first item");
			Atk.Object accessible = GetAdapterForWidget (dud1);
			Assert.AreEqual (1, accessible.NAccessibleChildren, "NAccessibleChildren");
			dud1.Items.Clear ();
			Assert.AreEqual (0, accessible.NAccessibleChildren, "NAccessibleChildren after clear");
		}

		[Test]
		public void PaneChildren ()
		{
			PaneChildren (panel1);
		}

		public void PaneChildren (SWF.Control control)
		{
			SWF.Button button = new SWF.Button ();
			Atk.Object accessible = GetAdapterForWidget (control);
			Assert.AreEqual (0, accessible.NAccessibleChildren, "NAccessibleChildren #1");
			control.Controls.Add (button);
			Assert.AreEqual (1, accessible.NAccessibleChildren, "NAccessibleChildren #2");
			control.Controls.Remove (button);
			Assert.AreEqual (0, accessible.NAccessibleChildren, "NAccessibleChildren #3");
		}

		[Test]
		public void ToolStripProgressBar ()
		{
			ProgressBar (BasicWidgetType.ToolStripProgressBar);
		}
		
		[Test]
		public void StatusStrip ()
		{
			BasicWidgetType type = BasicWidgetType.StatusStrip;
			
			Atk.Object accessible = GetAdapterForWidget (ss1);
			Assert.IsNotNull (accessible, "Adapter should not be null");
			States (accessible,
			        Atk.StateType.Enabled,
			        Atk.StateType.Sensitive,
			        Atk.StateType.Showing,
			        Atk.StateType.Visible);
			
			PropertyRole (type, accessible);
			
			ss1.Items.Clear ();
			ss1.Items.Add ("first item");
			ss1.Items.Add ("second item");
			ss1.Items.Add ("third item");
			Assert.AreEqual (3, accessible.NAccessibleChildren, "NAccessibleChildren #1");
			
			Atk.Object child1 = accessible.RefAccessibleChild (0);
			Assert.AreEqual (Atk.Role.Label, child1.Role, "Child role #1");
			InterfaceText (child1, "first item");
			
			for (int i = ss1.Items.Count - 1; i >= 0; i--)
				ss1.Items.RemoveAt (i);
			Assert.AreEqual (0, accessible.NAccessibleChildren, "NAccessibleChildren after remove");
		}

		[Test]
		[Ignore ("Dep. on BNC#446783")]
		public void StatusStripClear ()
		{
			ss1.Items.Add ("first item");
			Atk.Object accessible = GetAdapterForWidget (ss1);
			Assert.AreEqual (1, accessible.NAccessibleChildren, "NAccessibleChildren");
			ss1.Items.Clear ();
			Assert.AreEqual (0, accessible.NAccessibleChildren, "NAccessibleChildren after clear");
		}

		[Test]
		public void Splitter ()
		{
			using (SWF.Form f = new SWF.Form()) {
				SWF.Label label1 = new SWF.Label ();
				label1.Dock = SWF.DockStyle.Left;
				label1.Text = "label1";
				SWF.Label label2 = new SWF.Label ();
				label2.Dock = SWF.DockStyle.Fill;
				label2.Text = "label2";
				SWF.Splitter splitter = new SWF.Splitter ();
				splitter.Dock = SWF.DockStyle.Left;
				f.Controls.Add (label2);
				f.Controls.Add (splitter);
				f.Controls.Add (label1);
				f.Show ();
				Atk.Object accessible = GetAdapterForWidget (f);
				Assert.AreEqual (1, accessible.NAccessibleChildren, "Form should have one child");
				Atk.Object child = accessible.RefAccessibleChild (0);
				HSplitter (child);
				f.Controls.Remove (splitter);
				Assert.AreEqual (2, accessible.NAccessibleChildren, "Form should have two children after removing splitter");
				f.Close ();
			}
		}

		[Test]
		public void Bug416602 ()
		{
			using (SWF.Form f = new SWF.Form ()) {
				SWF.ListBox listbox = null;
				SWF.Button addButton = new SWF.Button ();
				addButton.Text = "Add listbox";
				addButton.Click += delegate (object sender, EventArgs args) {
					if (listbox != null)
						return;
					listbox = new SWF.ListBox ();
					listbox.Items.Add ("1");
					listbox.Items.Add ("2");
					listbox.Items.Add ("3");
					listbox.Size = new Size (100, 100);
					listbox.Location = new Point (1, 1);
					f.Controls.Add (listbox);
				};
				addButton.Size = new Size (60, 25);
				addButton.Location = new Point (105, 1);
				
				SWF.Button deleteItem = new SWF.Button ();
				deleteItem.Text = "delete item";
				deleteItem.Size = new Size (60, 25);
				deleteItem.Location = new Point (105, 27);
				deleteItem.Click += delegate (object sender, EventArgs args) {
					if (listbox == null || listbox.SelectedIndex == -1)
						return;
					
					//The bridge crashes after this line.
					listbox.Items.RemoveAt (listbox.SelectedIndex);
				};
				
				f.Size = new Size (250, 250);
				f.Controls.Add (addButton);
				f.Controls.Add (deleteItem);
				f.Show ();
				
				//Click first button.
				addButton.PerformClick ();

				//Select item
				listbox.SelectedIndex = 1;
				
				try {
					//Click delete button
					//shouldn't raise KeyNotFoundException
					deleteItem.PerformClick ();
				} catch (System.Collections.Generic.KeyNotFoundException) {
					Assert.Fail ("Shouldn't crash");
				}

				//this is not needed thanks to the fix in r122955
				//f.Close ();
			}
		}

		[Test]
		public void ToolStripDropDownButton ()
		{
			BasicWidgetType type = BasicWidgetType.ToolStripDropDownButton;

			string[] extreme_fjords = new string[] {
				"Scoresby Sund", "Sognefjord", "Hardangerfjord"
			};
			
			Atk.Object accessible = GetAccessible (type, extreme_fjords);
			PropertyRole (type, accessible);

			Assert.AreEqual (Atk.Role.Panel, accessible.Parent.Role);

			Assert.AreEqual (extreme_fjords.Length, accessible.NAccessibleChildren);

			for (int i = 0; i < extreme_fjords.Length; i++) {
				Atk.Object child = accessible.RefAccessibleChild (i);
				Assert.AreEqual (Atk.Role.MenuItem, child.Role,
				                 String.Format ("Child role #{0}", i));
				InterfaceText (child, extreme_fjords [i]);
			}

			Atk.Object secondItem = accessible.RefAccessibleChild (1);

			Atk.Action atkAction
				= CastToAtkInterface <Atk.Action> (secondItem);
			atkAction.DoAction (0);
			
			States (secondItem,
				Atk.StateType.Enabled,
				Atk.StateType.Selectable,
				Atk.StateType.Selected,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible);
			
			Atk.Object firstItem = accessible.RefAccessibleChild (0);
			States (firstItem,
				Atk.StateType.Enabled,
				Atk.StateType.Selectable,
				Atk.StateType.Sensitive,
				Atk.StateType.Visible);

			atkAction = CastToAtkInterface <Atk.Action> (firstItem);
			atkAction.DoAction (0);

			// #471411 would cause Focused to show up in this list.
			States (secondItem,
				Atk.StateType.Enabled,
				Atk.StateType.Selectable,
				Atk.StateType.Sensitive,
				Atk.StateType.Visible);
		}

		[Test]
		public void ToolStripDropDownButton_Bug457990 ()
		{
			using (SWF.Form f = new SWF.Form ()) {
				SWF.ToolStrip s = new SWF.ToolStrip ();
				SWF.ToolStripDropDownButton b
					= new SWF.ToolStripDropDownButton ();
					
				s.Items.Add (b);
				f.Controls.Add (s);
				f.Show ();

				Atk.Object accessible = GetAdapterForWidget (b);
				Atk.Action atkAction
					= CastToAtkInterface <Atk.Action> (accessible);
				atkAction.DoAction (0);

				// Ensure we don't see Focusable or Focused
				States (accessible,
					Atk.StateType.Enabled,
					Atk.StateType.Selectable,
					Atk.StateType.Selected,
					Atk.StateType.Sensitive,
					Atk.StateType.Showing,
					Atk.StateType.Visible);

				f.Close ();
			}
		}

		[Test]
		public void ToolStripSplitButton ()
		{
			BasicWidgetType type = BasicWidgetType.ToolStripSplitButton;

			string[] les_schtroumpfs = new string[] {
				"Les Schtroumpfs",
				"Papa", "Smurfette", "Hefty", "Brainy", "Jokey",
				"Grouchy", "Dreamy", "Clumsy",
			};
			
			Atk.Object accessible = GetAccessible (type, les_schtroumpfs);
			PropertyRole (type, accessible);

			Assert.AreEqual (2, accessible.NAccessibleChildren);

			Atk.Object button = accessible.RefAccessibleChild (0);
			Atk.Object toggle = accessible.RefAccessibleChild (1);
			
			Assert.AreEqual (Atk.Role.PushButton, button.Role);

			// TODO: Why does this work in the individual test, but not when run with the suite?
			//InterfaceText (button, les_schtroumpfs[0]);

			Assert.AreEqual (Atk.Role.ToggleButton, toggle.Role);
			Assert.AreEqual (les_schtroumpfs.Length - 1, toggle.NAccessibleChildren);

			// Don't use the full InterfaceSelection as the control
			// can't implement everything fully without
			// ISelectionProvider support
			Atk.Selection atkSelection = CastToAtkInterface <Atk.Selection> (toggle);
			Assert.IsNotNull (atkSelection,
			                  "ExpandCollapseButton doesn't implement Atk.Selection");

			for (int i = 1; i < les_schtroumpfs.Length; i++) {
				Atk.Object child = toggle.RefAccessibleChild (i - 1);
				Assert.AreEqual (Atk.Role.MenuItem, child.Role,
				                 String.Format ("Child role #{0}", i));
				InterfaceText (child, les_schtroumpfs [i]);
				Atk.Component atkComponent = CastToAtkInterface<Atk.Component> (child);
				Assert.AreEqual (Atk.Layer.Widget, atkComponent.Layer, "MenuItem layer");
			}
		}

		[Test]
		public void ToolStrip ()
		{
			ToolBar (GetAdapterForWidget (toolStrip));
		}

		[Test]
		public void StatusBarPanel ()
		{
			BasicWidgetType type = BasicWidgetType.StatusBarPanel;

			Atk.Object sb = GetAdapterForWidget (sb1);
			sb1.ShowPanels = true;
			SWF.StatusBarPanel panel1 = new SWF.StatusBarPanel ();
			string panelText = "Panel 1";
			panel1.Text = panelText;
			sb1.Panels.Add (panel1);
			Assert.AreEqual (1, sb.NAccessibleChildren, "StatusBar should have 1 child after panel is added");
			Atk.Object panel = sb.RefAccessibleChild (0);
			PropertyRole (type, panel);
			Atk.Component atkComponent = CastToAtkInterface<Atk.Component> (panel);
			InterfaceComponent (type, atkComponent);

			Assert.AreEqual (panel.NAccessibleChildren, 0, "StatusBar panel should not have children");
			Assert.AreEqual (panelText, panel.Name, "Panel name should match the text");
			InterfaceText (panel, panelText);

			States (panel,
			  Atk.StateType.Enabled,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible,
			  Atk.StateType.MultiLine);

			sb1.Panels.Remove (panel1);
			Assert.AreEqual (0, sb.NAccessibleChildren, "StatusBar should not have children after panel is removed");
		}

		[Test]
		[Ignore ("Not ready yet. For 1.0.")]
		public override void MaskedTextBoxEntry ()
		{
			base.MaskedTextBoxEntry ();
		}

		[Test]
		[Ignore ("Blocking test")]
		public void ContextMenuDeprecated ()
		{
			base.ContextMenu (BasicWidgetType.ContextMenuDeprecated);
		}

		[Test]
		public void ToolStripTextBoxSingleLine ()
		{
			TextBoxEntry (toolStripTextBox1, false);
		}

		[Test]
		public void ToolStripTextBoxMultiLine ()
		{
			TextBoxView (BasicWidgetType.TextBoxView, toolStripTextBox2, false);
		}

		[Test] //TODO: move to AtkTests.cs
		public void ToolBarButton ()
		{
			BasicWidgetType type = BasicWidgetType.ToolbarButton;

			string name = "test-caption";
			Atk.Object accessible = GetAccessible (type, name);
			PropertyRole (type, accessible);

			Assert.AreEqual (Atk.Role.Panel, accessible.Parent.Role);

			Assert.IsNull (accessible.Parent.Name);
			Assert.AreEqual (0, accessible.NAccessibleChildren);

			Interfaces (accessible.Parent,
			            typeof (Atk.Component));
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible.Parent);
			InterfaceComponent (type, atkComponent);
			
			States (accessible.Parent,
				Atk.StateType.Enabled,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible);
			
			States (accessible,
				Atk.StateType.Enabled,
				Atk.StateType.Focusable,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible);

			//from here, like Button test
			atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);

			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible);

			InterfaceText (type);

			//test with an image
			Atk.Image atkWithOutImage, atkWithImage;
			accessible = GetAccessible (type, name, true);
			atkWithOutImage = CastToAtkInterface <Atk.Image> (accessible);
			accessible = GetAccessibleThatEmbedsAnImage (type, name, true);
			atkWithImage = CastToAtkInterface <Atk.Image> (accessible);
			atkComponent = CastToAtkInterface<Atk.Component> (accessible);
			InterfaceImage (type, atkWithImage, atkComponent, atkWithOutImage);
		}
		
		[Test]
		public void Bug457939 ()
		{
			SWF.ToolStripLabel lab = new SWF.ToolStripLabel ();
			lab.IsLink = true;
			toolStrip.Items.Add (lab);
			Atk.Object accessible = GetAdapterForWidget (lab);
			Assert.AreEqual (Atk.Role.Label, accessible.Role, "A ToolStripLabel with IsLink==True should not have an unknown role");
			toolStrip.Items.Remove (lab);
		}

		[Test]
		public void DateTimePicker ()
		{
			BasicWidgetType type = BasicWidgetType.DateTimePicker;

			Atk.Object accessible = GetAccessible (type, new string [0]);

			dateTimePicker.ShowUpDown = false;
			dateTimePicker.ShowCheckBox = false;

			// DayName, Month Day, Year
			dateTimePicker.Format = SWF.DateTimePickerFormat.Long;
			Atk.Role[] expectedRoles = new Atk.Role[] {
				Atk.Role.TreeTable, Atk.Role.Label, Atk.Role.Label, Atk.Role.TreeTable,
				Atk.Role.Label, Atk.Role.SpinButton, Atk.Role.Label, Atk.Role.Label,
				Atk.Role.SpinButton, Atk.Role.PushButton
			};

			PropertyRole (type, accessible);
			Atk.Component atkComponent = CastToAtkInterface<Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			
			Assert.AreEqual (expectedRoles.Length, accessible.NAccessibleChildren,
			                 "Correct number of children not found");
			
			Atk.Object child = null;
			Atk.Role expectedRole = Atk.Role.Label;
			for (int i = 0; i < accessible.NAccessibleChildren; i++) {
				child = accessible.RefAccessibleChild (i);
				expectedRole = expectedRoles[i];

				Assert.AreEqual (expectedRole, child.Role,
				                 String.Format ("Child {0}'s role is not what was expected", i));
				switch (expectedRole) {
				case Atk.Role.Label:
					Label (child, BasicWidgetType.Label);
					break;
				case Atk.Role.List:
				case Atk.Role.TreeTable:
					// TODO: add when general test for List is more self-contained
					Atk.Object item = child.RefAccessibleChild (0);
					States (item, Atk.StateType.Enabled,
					        Atk.StateType.Focusable, Atk.StateType.Selectable,
					        Atk.StateType.Sensitive, Atk.StateType.SingleLine,
					        Atk.StateType.Transient);
					break;
				case Atk.Role.SpinButton:
					// TODO: add when general test for Spinner has values less hard-coded
					break;
				}
			}
		}

		// This can't be tested in AtkTest because the behavior is
		// slightly different than Gtk+s.  Unfortunately it's not
		// performant to map UIA's api to Atk's api in this instance.
		// See TextPatternTextImplementorHelper for more details.
		[Test]
		public void RichTextBoxAttributeTest ()
		{
			richTextBox.Rtf = 
@"{\rtf1\ansi
{\fonttbl\f0\fswiss Arial;\f1\froman Times New Roman;}
{\colortbl;\red255\green0\blue0;}
\pard {\f0 ab {\b cd} {\cf1 ef} {\strike gh}}\par
\pard {\qr\f1\i ij }\par
\pard {\qc\f0\ul kl }\par
\pard {\qj\f0\fs24 mn }\par
}";
			Atk.Object accessible = GetAdapterForWidget (richTextBox);
			Atk.Text text = CastToAtkInterface<Atk.Text> (accessible);
			Dictionary<string, string> attrs;
			
			Dictionary<string, string> defaultAttrs
				= GetDictionary (text.DefaultAttributes);
			
			attrs = new Dictionary<string, string> (defaultAttrs);
			ExpectAttrAndRemove ("bg-color", "0,0,0", attrs);
			ExpectAttrAndRemove ("pixels-below-lines", "0", attrs);
			ExpectAttrAndRemove ("pixels-above-lines", "0", attrs);
			ExpectAttrAndRemove ("editable", "true", attrs);
			ExpectAttrAndRemove ("invisible", "false", attrs);
			ExpectAttrAndRemove ("indent", "0", attrs);
			Assert.AreEqual (0, attrs.Count);

			int s, e;

			// Normal 
			attrs = GetDictionary (text.GetRunAttributes (1, out s, out e));
			RemoveItems (attrs, defaultAttrs);
			ExpectAttrAndRemove ("style", "normal", attrs);
			ExpectAttrAndRemove ("justification", "left", attrs);
			ExpectAttrAndRemove ("fg-color", "0,0,0", attrs);
			ExpectAttrAndRemove ("family-name", "Arial", attrs);
			ExpectAttrAndRemove ("strikethrough", "false", attrs);
			ExpectAttrAndRemove ("weight", "400", attrs);
			ExpectAttrAndRemove ("underline", "none", attrs);
			Assert.AreEqual (0, attrs.Count);

			// Bold
			attrs = GetDictionary (text.GetRunAttributes (3, out s, out e));
			RemoveItems (attrs, defaultAttrs);
			ExpectAttrAndRemove ("style", "normal", attrs);
			ExpectAttrAndRemove ("justification", "left", attrs);
			ExpectAttrAndRemove ("fg-color", "0,0,0", attrs);
			ExpectAttrAndRemove ("family-name", "Arial", attrs);
			ExpectAttrAndRemove ("strikethrough", "false", attrs);
			ExpectAttrAndRemove ("weight", "700", attrs);
			ExpectAttrAndRemove ("underline", "none", attrs);
			Assert.AreEqual (0, attrs.Count);

			// Blue 
			attrs = GetDictionary (text.GetRunAttributes (6, out s, out e));
			RemoveItems (attrs, defaultAttrs);
			ExpectAttrAndRemove ("style", "normal", attrs);
			ExpectAttrAndRemove ("justification", "left", attrs);
			ExpectAttrAndRemove ("fg-color", "255,0,0", attrs);
			ExpectAttrAndRemove ("family-name", "Arial", attrs);
			ExpectAttrAndRemove ("strikethrough", "false", attrs);
			ExpectAttrAndRemove ("weight", "400", attrs);
			ExpectAttrAndRemove ("underline", "none", attrs);
			Assert.AreEqual (0, attrs.Count);
			
			// Strikethrough
			attrs = GetDictionary (text.GetRunAttributes (9, out s, out e));
			RemoveItems (attrs, defaultAttrs);
			ExpectAttrAndRemove ("style", "normal", attrs);
			ExpectAttrAndRemove ("justification", "left", attrs);
			ExpectAttrAndRemove ("fg-color", "0,0,0", attrs);
			ExpectAttrAndRemove ("family-name", "Arial", attrs);
			ExpectAttrAndRemove ("weight", "400", attrs);
			ExpectAttrAndRemove ("strikethrough", "true", attrs);
			ExpectAttrAndRemove ("underline", "none", attrs);
			Assert.AreEqual (0, attrs.Count);
			
			// Right aligned, Italic, Times New Roman
			attrs = GetDictionary (text.GetRunAttributes (12, out s, out e));
			RemoveItems (attrs, defaultAttrs);
			ExpectAttrAndRemove ("style", "italic", attrs);
			ExpectAttrAndRemove ("justification", "right", attrs);
			ExpectAttrAndRemove ("fg-color", "0,0,0", attrs);
			ExpectAttrAndRemove ("family-name", "Times New Roman", attrs);
			ExpectAttrAndRemove ("strikethrough", "false", attrs);
			ExpectAttrAndRemove ("weight", "400", attrs);
			ExpectAttrAndRemove ("underline", "none", attrs);
			Assert.AreEqual (0, attrs.Count);
			
			// Centered, Underlined
			attrs = GetDictionary (text.GetRunAttributes (16, out s, out e));
			RemoveItems (attrs, defaultAttrs);
			ExpectAttrAndRemove ("style", "normal", attrs);
			ExpectAttrAndRemove ("justification", "center", attrs);
			ExpectAttrAndRemove ("fg-color", "0,0,0", attrs);
			ExpectAttrAndRemove ("family-name", "Arial", attrs);
			ExpectAttrAndRemove ("strikethrough", "false", attrs);
			ExpectAttrAndRemove ("weight", "400", attrs);
			ExpectAttrAndRemove ("underline", "single", attrs);
			Assert.AreEqual (0, attrs.Count);
			
			// Justified (Mono's RichTextBox doesn't support this yet)
			attrs = GetDictionary (text.GetRunAttributes (21, out s, out e));
			RemoveItems (attrs, defaultAttrs);
			ExpectAttrAndRemove ("style", "normal", attrs);
			ExpectAttrAndRemove ("justification", "center", attrs);
			ExpectAttrAndRemove ("fg-color", "0,0,0", attrs);
			ExpectAttrAndRemove ("family-name", "Arial", attrs);
			ExpectAttrAndRemove ("strikethrough", "false", attrs);
			ExpectAttrAndRemove ("weight", "400", attrs);
			ExpectAttrAndRemove ("underline", "none", attrs);
			Assert.AreEqual (0, attrs.Count);
		}	

		[Test]
		public void FlowLayoutPanel ()
		{
			Atk.Object accessible = GetAdapterForWidget (flp);
			Pane (accessible);
			PaneChildren (flp);
		}

		[Test]
		public void TableLayoutPanel ()
		{
			Atk.Object accessible = GetAdapterForWidget (tlp);
			Pane (accessible);
			PaneChildren (tlp);
		}

		[Test]
		public void ContainerControl ()
		{
			Atk.Object accessible = GetAdapterForWidget (containerControl);
			Pane (accessible);
			PaneChildren (containerControl);
		}

		private void RemoveItems (Dictionary<string, string> items, Dictionary<string, string> except)
		{
			foreach (KeyValuePair<string, string> e in except)
			{
				if (items.ContainsKey (e.Key)
				    && items[e.Key] == e.Value) {
					items.Remove (e.Key);
				}
			}
		}

		private Dictionary<string, string> GetDictionary (Atk.Attribute[] attrList)
		{
			Dictionary<string, string> attrs = new Dictionary<string, string> ();
			foreach (Atk.Attribute attr in attrList)
				attrs.Add (attr.Name, attr.Value);
			return attrs;
		}

		private void ExpectAttrAndRemove (string name, string val,
		                                  Dictionary<string, string> attrs)
		{
			Assert.IsTrue (attrs.ContainsKey (name),
			               String.Format ("Text Attribute not found: {0}", name));

			Assert.AreEqual (val, attrs[name],
			                 "Expected attribute value differs");
			attrs.Remove (name);
		}

		/*
		 * Our StreamableContent implementation differs from Gail's
		 * because we don't support fancy mimetypes.
		 */
		[Test]
		public void RichTextBoxStreamableContentTest ()
		{
			string test = "How do you do - Welcome to the human race - You're a mess.";
			richTextBox.Text = test;

			Atk.Object accessible = GetAdapterForWidget (richTextBox);
			Atk.StreamableContent streamableContent
				= CastToAtkInterface<Atk.StreamableContent> (accessible);

			Assert.AreEqual (1, streamableContent.NMimeTypes,
			                 "Exporting more than the expected number of mimetypes");
			Assert.AreEqual ("text/plain", streamableContent.GetMimeType (0),
			                 "0th mimetype isn't text/plain");

			GLib.IOChannel gio = GLib.IOChannel.FromHandle (
				streamableContent.GetStream ("text/plain"));
			Assert.IsNotNull (gio, "Null stream returned");

			string ret;
			gio.ReadToEnd (out ret);

			Assert.AreEqual (test, ret,
			                 "text/plain stream differs from original text");
		}

		[Test]
		public void MonthCalendar ()
		{
			BasicWidgetType type = BasicWidgetType.MonthCalendar;
			
			Atk.Object accessible = GetAccessible (type, String.Empty);
			Assert.AreEqual (1, accessible.NAccessibleChildren,
			                 "Too many or too few children under Calendar");
			
			Atk.Object tableChild = accessible.RefAccessibleChild (0);
			Atk.Table table = CastToAtkInterface<Atk.Table> (tableChild);
			Assert.IsNotNull (table);

			InterfaceTable (table, 6, 7, 0, 0, true);
		}
		
		// This test tries to simulate inserting and deleting a single
		// character and test that the appropriate events are
		// delivered for that case.
		[Test]
		public void TextBoxTextChangedEvents ()
		{
			tbx1.Text = "abc";
			StartEventMonitor ();
			tbx1.Text = "abcd";
			ExpectEvents (1, Atk.Role.Text, "object:text-changed:insert");
			ExpectEvents (0, Atk.Role.Text, "object:text-changed:delete");
			StartEventMonitor ();
			tbx1.Text = "abc";
			ExpectEvents (0, Atk.Role.Text, "object:text-changed:insert");
			ExpectEvents (1, Atk.Role.Text, "object:text-changed:delete");
		}
	}
}
