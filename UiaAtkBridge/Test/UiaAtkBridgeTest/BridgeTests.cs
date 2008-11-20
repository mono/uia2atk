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
			BasicWidgetType type = BasicWidgetType.ListBox;
			Atk.Object accessible;
			
			string[] names = new string [] { simpleTestText, "Second Item", "Last Item" };
			accessible = GetAccessible (type, names);
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);

			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (3, accessible.NAccessibleChildren, "ListBox#RO numChildren");
			
			Atk.Object listItemChild = accessible.RefAccessibleChild (0);
			Assert.IsNotNull (listItemChild, "ListBox child#0 should not be null");
			Assert.AreEqual (listItemChild.Role, Atk.Role.ListItem, "ListBox child#0 should be a list item");
			
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

			Parent (type, accessible);
		}

		[Test]
		public void CheckedListBox ()
		{
			BasicWidgetType type = BasicWidgetType.CheckedListBox;
			Atk.Object accessible;
			
			string[] names = new string[] { simpleTestText, "Second Item", "Last Item" };
			accessible = GetAccessible (type, names);
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
			Assert.IsNull (link1.GetObject (1), "GetObject OOR #1");
			Assert.IsNull (link1.GetObject (-1), "GetObject OOR #2");
			Atk.Hyperlink link2 = hypertext.GetLink (1);
			Assert.AreEqual ("gmail.novell.com", link2.GetUri (0), "Link 1 uri");
			Assert.AreEqual (35, link2.StartIndex, "Link 1 StartIndex");
			Assert.AreEqual (51, link2.EndIndex, "Link 1 EndIndex");
			Atk.Object obj2 = link2.GetObject (0);
			Assert.IsNotNull (obj2, "LinkLabel GetObject #2");
			Assert.IsTrue (obj2.RefStateSet ().ContainsState (Atk.StateType.Enabled), "RefStateSet().Contains(Enabled)");
			
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
			CheckComboBoxMenuChild (menuChild, names);
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
			lv1.View = SWF.View.List;
			lv1.View = SWF.View.Details;
			TestListBox ();
			lv1.View = SWF.View.LargeIcon;
			TestListBox ();
			lv1.View = SWF.View.SmallIcon;
			TestListBox ();
		}

		public void TestListBox ()
		{
			lv1.Items.Clear();
			lv1.CheckBoxes = false;
			lv1.Items.Add ("Item1");
			lv1.Items.Add ("Item2");
			Atk.Object accessible = GetAdapterForWidget (lv1);
			Assert.IsNotNull (accessible, "Adapter should not be null");
			Assert.AreEqual (2, accessible.NAccessibleChildren, "NAccessibleChildren #1");
			Atk.Object child1 = accessible.RefAccessibleChild (0);
			Assert.AreEqual (Atk.Role.ListItem, child1.Role, "Child role #1");
			child1 = null;
			lv1.Items.Clear();
			Assert.AreEqual (0, accessible.NAccessibleChildren, "NAccessibleChildren #1");
			lv1.CheckBoxes = true;
			lv1.Items.Add ("Item1");
			lv1.Items[0].Checked = true;
			lv1.Items.Add ("Item2");
			child1 = accessible.RefAccessibleChild (0);
			Assert.AreEqual (Atk.Role.CheckBox, child1.Role, "Child role #2");
			// TODO: Is SingleLine appropriate?
			States (child1,
				Atk.StateType.Checked,
				Atk.StateType.Enabled,
				Atk.StateType.Focusable,
				Atk.StateType.Selectable,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.SingleLine,
				Atk.StateType.Transient,
				Atk.StateType.Visible);
			Atk.Object child2 = accessible.RefAccessibleChild (1);
			States (child2,
				Atk.StateType.Enabled,
				Atk.StateType.Focusable,
				Atk.StateType.Selectable,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.SingleLine,
				Atk.StateType.Transient,
				Atk.StateType.Visible);
		}

		[Test]
		public void DomainUpDown ()
		{
			BasicWidgetType type = BasicWidgetType.DomainUpDown;

			dud1.Items.Clear ();
			dud1.Items.Add ("first item");
			dud1.Items.Add ("second item");
			dud1.Items.Add ("third item");
			Atk.Object accessible = GetAdapterForWidget (dud1);
			PropertyRole (type, accessible);

			Assert.AreEqual (3, accessible.NAccessibleChildren, "NAccessibleChildren");

			dud1.SelectedIndex = 1;
			InterfaceText (accessible, "second item");

			InterfaceEditableText (type, accessible);

			dud1.Items.Clear ();
			Assert.AreEqual (0, accessible.NAccessibleChildren, "NAccessibleChildren after clear [need SWF fix]");
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

			for (int i = 0; i < extreme_fjords.Length; i++) {
				Atk.Object child = accessible.RefAccessibleChild (i);
				Assert.AreEqual (Atk.Role.MenuItem, child.Role,
				                 String.Format ("Child role #{0}", i));
				InterfaceText (child, extreme_fjords[i]);
			}
		}

	}
}
