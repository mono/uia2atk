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
			Assert.AreEqual (Atk.Role.ListItem, listItemChild.Role, "ListBox child#0 should be a list item");
			
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
		[Ignore ("Not ready yet. For 1.0.")]
		public void ComboBoxSimple ()
		{
			ComboBoxSimple (null);
		}
		
		private void ComboBoxSimple (System.ComponentModel.Component comboBox)
		{
			BasicWidgetType type = BasicWidgetType.ComboBoxSimple;

			if (comboBox == null)
				comboBox = cbSim;

			string [] names = new string [] { "First Item", "Second Item", "Last item" };
			Atk.Object accessible = GetAccessible (type, names, comboBox);
			
			PropertyRole (type, accessible);

			StatesComboBox (accessible);

			Assert.AreEqual (2, accessible.NAccessibleChildren, "numChildren; children roles:" + childrenRoles (accessible));

			Atk.Object menuChild = accessible.RefAccessibleChild (0);
			CheckComboBoxMenuChild (menuChild, names, true, false);
			//FIXME: maybe we need to test here like we would test a treeview
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
			BasicWidgetType type = BasicWidgetType.ListView;
			lv1.Items.Clear ();
			lv1.Groups.Clear ();
			lv1.View = SWF.View.SmallIcon;
			lv1.Groups.Add (new SWF.ListViewGroup ("group1"));
			lv1.Groups.Add (new SWF.ListViewGroup ("group2"));
			lv1.Items.Add ("item1");
			lv1.Items.Add ("item2");
			lv1.Items.Add ("item3");
			lv1.Items.Add ("item4");
			lv1.Items[0].Group = lv1.Groups[0];
			lv1.Items[1].Group = lv1.Groups[0];
			lv1.Items[2].Group = lv1.Groups[1];
			lv1.Items[3].Group = lv1.Groups[1];
			lv1.MultiSelect = false;
			lv1.CheckBoxes = false;
			Atk.Object accessible = GetAdapterForWidget (lv1);
			Assert.IsNotNull (accessible, "Adapter should not be null");
			// 2 groups + default group and ScrollBar
			// Exposing empty default group probably not ideal
			Assert.AreEqual (4, accessible.NAccessibleChildren, "NAccessibleChildren #1");
			Atk.Object group1 = FindObjectByName (accessible, "group1");
			Assert.IsNotNull (group1, "FindObjectByName (group1)");
			Assert.AreEqual (Atk.Role.LayeredPane, group1.Role, "Group1 role");
			Atk.Object item1 = FindObjectByName (group1, "item1");
			Assert.IsNotNull (item1, "FindObjectByName (item1)");
			Assert.AreEqual (Atk.Role.ListItem, item1.Role, "Item1 role");
			Atk.Selection atkSelection = CastToAtkInterface<Atk.Selection> (item1.Parent);
			string [] names = { "item1", "item2" };
			InterfaceSelection (atkSelection, names, item1.Parent, type);
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

			lv1.Items[2].Group = lv1.Groups[0];
			lv1.Items[3].Group = lv1.Groups[0];
			Atk.Table atkTable = CastToAtkInterface<Atk.Table> (group1);
			Assert.AreEqual (2, atkTable.NRows, "Table NRows");
			Assert.AreEqual (2, atkTable.NColumns, "Table NColumns");
			Assert.AreEqual ("item1", atkTable.RefAt (0, 0).Name, "Cell (0, 0)");
			Assert.AreEqual ("item2", atkTable.RefAt (0, 1).Name, "Cell (0, 1)");
			Assert.AreEqual ("item3", atkTable.RefAt (1, 0).Name, "Cell (1, 0)");
			Assert.AreEqual ("item4", atkTable.RefAt (1, 1).Name, "Cell (1, 1)");
			Assert.AreEqual (0, atkTable.GetIndexAt (0, 0), "GetIndexAt (0, 0)");
			Assert.AreEqual (1, atkTable.GetIndexAt (0, 1), "GetIndexAt (0, 1)");
			Assert.AreEqual (2, atkTable.GetIndexAt (1, 0), "GetIndexAt (1, 0)");
			Assert.AreEqual (3, atkTable.GetIndexAt (1, 1), "GetIndexAt (1, 1)");

			accessible = group1 = item1 = null;
			atkSelection = null;
			atkTable = null;
			lv1.View = SWF.View.List;
			CheckedListBox (true);
			lv1.CheckBoxes = false;
			ListBox (true);
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

			// The below line would make the test fail, and it
			// isn't clear to me what we can do about it.
			// See bug 463299.
			//dud1.Items.Clear ();
			// so test this instead:
			for (int i = names.Length - 1; i >= 0; i--)
				dud1.Items.RemoveAt (i);
			Assert.AreEqual (0, accessible.NAccessibleChildren, "NAccessibleChildren after clear [need SWF fix]");
		}

		[Test]
		public void PaneChildren ()
		{
			SWF.Button button = new SWF.Button ();
			Atk.Object accessible = GetAdapterForWidget (panel1);
			Assert.AreEqual (0, accessible.NAccessibleChildren, "NAccessibleChildren #1");
			panel1.Controls.Add (button);
			Assert.AreEqual (1, accessible.NAccessibleChildren, "NAccessibleChildren #2");
			panel1.Controls.Remove (button);
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
			
			ss1.Items.Clear ();
			Assert.AreEqual (0, accessible.NAccessibleChildren, "NAccessibleChildren after clear [need SWF fix]");
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
				f.Close ();
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
				Atk.StateType.Focusable,
				Atk.StateType.Focused,
				Atk.StateType.Selectable,
				Atk.StateType.Selected,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible);
			
			Atk.Object firstItem = accessible.RefAccessibleChild (0);
			States (firstItem,
				Atk.StateType.Enabled,
				Atk.StateType.Focusable,
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
			BasicWidgetType type = BasicWidgetType.ToolStrip;

			Atk.Object accessible = GetAdapterForWidget (toolStrip);
			PropertyRole (type, accessible);
			Atk.Component atkComponent = CastToAtkInterface<Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			if (accessible.NAccessibleChildren < 2)
				Assert.Fail ("ToolStrip should have children");

			States (accessible,
				Atk.StateType.Enabled,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible);
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
		public void ToolStripTextBoxSingleLine () {
			TextBoxEntry (toolStripTextBox1, false);
		}


		[Test]
		public void ToolStripTextBoxMultiLine () {
			TextBoxView (toolStripTextBox2, false);
		}

		[Test]
		[Ignore ("It causes a deadlock for now")]
		public void OpenFileDialog ()
		{
			OpenFileDialogStatic ();
		}

		internal static void OpenFileDialogStatic ()
		{
			new DialogTest (new SWF.OpenFileDialog ()).Test ();
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

		internal class DialogTest {
			SWF.CommonDialog dialog;
	
			internal DialogTest (SWF.CommonDialog dialog)
			{
				this.dialog = dialog;
			}

			internal void Test ()
			{
				var th = new Thread (new ThreadStart (Show));
				th.SetApartmentState(ApartmentState.STA);
				try {
					th.Start ();
					Thread.Sleep (4000);
//uncomment this when we resolve the deadlock problem:
//					Atk.Object dialogAdapter = BridgeTester.GetAdapterForWidget (dialog);
//					Assert.AreEqual (dialogAdapter.Role, Atk.Role.Dialog, "dialog should have dialog role");
//					Assert.IsTrue (dialogAdapter.NAccessibleChildren > 0, "dialog should have children");
				}
				finally {
					th.Abort ();
				}
			}

			private void Show ()
			{
				dialog.ShowDialog ();
			}
		}
	}


}
