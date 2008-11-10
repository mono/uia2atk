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
	
	public abstract class BridgeTests : AtkTests
	{

		protected SWF.GroupBox gb1 = new SWF.GroupBox ();
		protected SWF.GroupBox gb2 = new SWF.GroupBox ();
		protected SWF.RadioButton rad1 = new SWF.RadioButton ();
		protected SWF.RadioButton rad2 = new SWF.RadioButton ();
		protected SWF.RadioButton rad3 = new SWF.RadioButton ();
		protected SWF.RadioButton rad4 = new SWF.RadioButton ();
		protected SWF.RadioButton radWithImage = new SWF.RadioButton ();
		protected List<SWF.RadioButton> radios = new List<SWF.RadioButton> ();
		protected int currentRadio = -1;
		protected SWF.ListBox lb1 = new SWF.ListBox ();
		protected SWF.CheckedListBox clb1 = new SWF.CheckedListBox ();
		protected SWF.ComboBox cbDD = new SWF.ComboBox ();
		protected SWF.ComboBox cbSim = new SWF.ComboBox ();
		protected SWF.ComboBox cbDDL = new SWF.ComboBox ();
		protected SWF.Label lab1 = new SWF.Label ();
		protected SWF.LinkLabel linklab1 = new SWF.LinkLabel ();
		protected SWF.Button butWithoutImage = new SWF.Button ();
		protected SWF.Button butWithImage = new SWF.Button ();
		protected SWF.CheckBox chkWithoutImage = new SWF.CheckBox ();
		protected SWF.CheckBox chkWithImage = new SWF.CheckBox ();
		protected SWF.StatusBar sb1 = new SWF.StatusBar ();
		protected SWF.ProgressBar pb1 = new SWF.ProgressBar ();
		protected SWF.NumericUpDown nud1 = new SWF.NumericUpDown ();
		protected SWF.Form form = new SWF.Form ();
		protected SWF.MenuStrip menuStrip1 = new SWF.MenuStrip ();
		protected SWF.PictureBox pboxWithoutImage = new SWF.PictureBox ();
		protected SWF.PictureBox pboxWithImage = new SWF.PictureBox ();
		protected SWF.TextBox tbx1 = new SWF.TextBox ();
		protected SWF.TextBox tbx2 = new SWF.TextBox ();
		protected SWF.ToolStrip toolStrip = new SWF.ToolStrip ();
		protected SWF.ToolStripComboBox toolStripComboBoxSim = new SWF.ToolStripComboBox ();

		protected int lastClickedLink = -1;
		
		[Test]
		public void ListBox ()
		{
			Console.WriteLine ("<Test id=\"ListBox\">");
			
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
			
			Atk.StateSet stateSet = listItemChild.RefStateSet ();
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Enabled), "RefStateSet().Contains(Enabled)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Focusable), "RefStateSet().Contains(Focusable)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Selectable), "RefStateSet().Contains(Selectable)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Sensitive), "RefStateSet().Contains(Sensitive)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Showing), "RefStateSet().Contains(Showing)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Visible), "RefStateSet().Contains(Visible)");

			Assert.AreEqual (0, listItemChild.NAccessibleChildren, "ListBox ListItem numChildren");

			Atk.SelectionImplementor selection = accessible as Atk.SelectionImplementor;
			Assert.IsNotNull (selection, "ListBox Atk.Selection should not be null");
			InterfaceSelection (new Atk.SelectionAdapter(selection), names, accessible, type);

			// Below line needed because InterfaceAction tests that first item is not selected, so that it can test the action
			selection.AddSelection(1);
			InterfaceAction (BasicWidgetType.ListItem, CastToAtkInterface <Atk.Action> (listItemChild), listItemChild);

			InterfaceText (BasicWidgetType.ListItem, true, listItemChild);

			Parent (type, accessible);

			Console.WriteLine ("</Test>");
		}

		[Test]
		public void CheckedListBox ()
		{
			Console.WriteLine ("<Test id=\"CheckedListBox\">");
			
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
			
			Atk.StateSet stateSet = listItemChild.RefStateSet ();
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Enabled), "RefStateSet().Contains(Enabled)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Focusable), "RefStateSet().Contains(Focusable)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Selectable), "RefStateSet().Contains(Selectable)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Sensitive), "RefStateSet().Contains(Sensitive)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Showing), "RefStateSet().Contains(Showing)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Visible), "RefStateSet().Contains(Visible)");
			Assert.AreEqual (0, listItemChild.NAccessibleChildren, "ListBox ListItem numChildren");

			Atk.SelectionImplementor selection = accessible as Atk.SelectionImplementor;
			Assert.IsNotNull (selection, "ListBox Atk.Selection should not be null");
			InterfaceSelection (new Atk.SelectionAdapter(selection), names, accessible, type);

			// Below line needed because InterfaceAction tests that first item is not selected, so that it can test the action
			selection.AddSelection(1);
			InterfaceAction (BasicWidgetType.CheckedListItem, CastToAtkInterface <Atk.Action> (listItemChild), listItemChild);

			InterfaceText (BasicWidgetType.CheckedListItem, true, listItemChild);

			Parent (type, accessible);

			Console.WriteLine ("</Test>");
		}

		[Test]
		public void LinkLabel ()
		{
			Console.WriteLine ("<Test id=\"LinkLabel\">");
			
			UiaAtkBridge.Hyperlink hyperlink;
			hyperlink = (UiaAtkBridge.Hyperlink) 
			  UiaAtkBridge.AutomationBridge.GetAdapterForProviderLazy (
			    (IRawElementProviderSimple) ProviderFactory.GetProvider (linklab1, true, true));
			Atk.Text atkText = CastToAtkInterface<Atk.Text> (hyperlink);

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

			Console.WriteLine ("</Test>");
		}

		[Test]
		public void GroupBox ()
		{
			Console.WriteLine ("<Test id=\"GroupBox\">");
			
			BasicWidgetType type = BasicWidgetType.GroupBox;
			Atk.Object accessible = 
			  UiaAtkBridge.AutomationBridge.GetAdapterForProviderLazy (
			    (IRawElementProviderSimple) ProviderFactory.GetProvider (gb1, true, true));

			PropertyRole (type, accessible);

			States (accessible,
				Atk.StateType.Enabled,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible);
			
			Console.WriteLine ("</Test>");
		}

		[Test]
		public void ComboBoxSimple ()
		{
			Console.WriteLine ("<Test id=\"ComboBoxSimple\">");
			
			ComboBoxSimple (null);

			Console.WriteLine ("</Test>");
		}
		
		private void ComboBoxSimple (System.ComponentModel.Component comboBox)
		{
			BasicWidgetType type = BasicWidgetType.ComboBoxSimple;

			if (comboBox == null)
				comboBox = cbSim;

			Atk.Object accessible = 
			  UiaAtkBridge.AutomationBridge.GetAdapterForProviderLazy (
			    (IRawElementProviderSimple) ProviderFactory.GetProvider (comboBox, true, true));
			
			PropertyRole (type, accessible);

			StatesComboBox (accessible);

			string [] names = new string [] { "First item", "Second Item", "Last Item" };
			((SWF.ComboBox)comboBox).Items.Clear ();
			foreach(string item in names)
				((SWF.ComboBox)comboBox).Items.Add (item);
			Assert.AreEqual (2, accessible.NAccessibleChildren, "numChildren; children roles:" + childrenRoles (accessible));

			Atk.Object menuChild = accessible.RefAccessibleChild (0);
			CheckComboBoxMenuChild (menuChild, names);
			//FIXME: maybe we need to test here like we would test a treeview
			
			Console.WriteLine ("</Test>");
		}

		[Test]
		public void ToolStripComboBoxSimple ()
		{
			Console.WriteLine ("<Test id=\"ToolStripComboBoxSimple\">");
			
			ComboBoxSimple (toolStripComboBoxSim);
			
			Console.WriteLine ("</Test>");
		}

		[Test]
		public void Bug416602 ()
		{
			Console.WriteLine ("<Test regression=\"{0}\">", 416602);
			
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
			
			Console.WriteLine ("</Test>");
		}

	}
}
