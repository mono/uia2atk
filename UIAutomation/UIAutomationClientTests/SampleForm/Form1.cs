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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SampleForm {
	public partial class Form1 : Form {

		private DataTable table = new DataTable ();
		private Form childForm;

		public Form1 ()
		{
			InitializeComponent ();
			txtCommand.AccessibleName = "txtCommand";
			textBox3.AccessibleName = "textBox3";

			this.button4.Click += new System.EventHandler (this.button4_Click);

			TreeNode node = new TreeNode ("item 1");
			node.Nodes.Add (new TreeNode ("item 1a"));
			treeView1.Nodes.Add (node);
			node = new TreeNode ("item 2");
			node.Nodes.Add (new TreeNode ("item 2a"));
			node.Nodes.Add (new TreeNode ("item 2b"));
			treeView1.Nodes.Add (node);

			table.Columns.Add ("Gender", typeof (bool));
			table.Columns.Add ("Name", typeof (string));
			table.Columns.Add ("Age", typeof (uint));
			DataRow tableRow;
			tableRow = table.NewRow ();
			tableRow [0] = false;
			tableRow [1] = "Alice";
			tableRow [2] = 24;
			table.Rows.Add (tableRow);
			tableRow = table.NewRow ();
			tableRow [0] = true;
			tableRow [1] = "Bob";
			tableRow [2] = 28;
			table.Rows.Add (tableRow);

			dataGridView1.DataSource = table;
			dataGridView1.AccessibleName = "dataGridView1";

			listView1.AccessibleName = "listView1";
			var view = listView1;
			view.CheckBoxes = true;
			view.View = View.Details;
			view.Columns.Add ("Subcolumn 1", 100, HorizontalAlignment.Left);
			view.Columns.Add ("Subcolumn 2", 100, HorizontalAlignment.Center);
			for (int i = 0; i < 10; i++) {
				ListViewItem item = new ListViewItem ();
				item.Text = "Item " + i;
				item.SubItems.Add ("subitem1");
				item.SubItems.Add ("subitem2");
				view.Items.Add (item);
			}
		}

		private void button1_Click (object sender, EventArgs e)
		{
			textBox1.Text = "button1_click";
			label1.Text = "button1_click";
			Console.WriteLine ("textbox1 & label1's texts are modified.");
		}

		private void button4_Click (object sender, EventArgs e)
		{
			numericUpDown1.Enabled = !numericUpDown1.Enabled;
			treeView1.Enabled = !treeView1.Enabled;
		}

		private void btnAddTextbox_Click (object sender, EventArgs e)
		{
			TextBox box = new TextBox();
			box.Width = 30;
			box.Left = 10;
			box.Top = panel1.Controls.Count * 25;
			panel1.Controls.Add(box);
		}

		private void btnRemoveTextbox_Click (object sender, EventArgs e)
		{
			if (panel1.Controls.Count <= 2)
				throw new Exception ("No more child control to delete");
			Control controlToDelete = null;
			foreach (Control c in panel1.Controls)
			{
				if (controlToDelete == null || controlToDelete.Top < c.Top)
					controlToDelete = c;
			}
			if (controlToDelete != null)
				panel1.Controls.Remove (controlToDelete);
		}

		private void btnRun_Click (object sender, EventArgs e)
		{
			const string sampleText = "Lorem ipsum dolor sit amet";

			string cmd = txtCommand.Text;
			if (cmd == "click button1")
				button1.PerformClick ();
			else if (cmd == "set textbox3 text")
				textBox3.Text = sampleText;
			else if (cmd == "select textbox3") {
				if (textBox3.Text.Length < 4)
					textBox3.Text = sampleText;
				if (textBox3.SelectionLength == 3)
					textBox3.Select (0, 4);
				else
					textBox3.Select (0, 3);
			} else if (cmd == "MoveTo.Origin") {
				Location = new Point (0, 0);
			} else if (cmd == "Toggle.Transform.CanMove") {
				if (WindowState == FormWindowState.Normal)
					WindowState = FormWindowState.Maximized;
				else
					WindowState = FormWindowState.Normal;
			} else if (cmd == "Toggle.Transform.CanResize") {
				if (FormBorderStyle == FormBorderStyle.Sizable)
					FormBorderStyle = FormBorderStyle.FixedSingle;
				else
					FormBorderStyle = FormBorderStyle.Sizable;
			} else if (cmd == "add table row")
				table.Rows.Add (true, "Mallory", 40);
			else if (cmd == "add table column")
				table.Columns.Add("More");
			else if (cmd == "set textBox3 long text")
				textBox3.Text = "very very very very very very very very long text to enable the horizontal scroll bar";
			else if (cmd == "disable textBox3")
				textBox3.Enabled = false;
			else if (cmd == "disable checkBox1")
				checkBox1.Enabled = false;
			else if (cmd == "enable checkBox1")
				checkBox1.Enabled = true;
			else if (cmd == "change list view mode list")
				listView1.View = View.List;
			else if (cmd == "change list view mode details")
				listView1.View = View.Details;
			else if (cmd == "disable list view")
				listView1.Enabled = false;
			else if (cmd == "enable list view")
				listView1.Enabled = true;
			else if (cmd == "make listView1 higher")
				listView1.Height = 500;
			else if (cmd == "add listView1 item") {
				ListViewItem item = new ListViewItem ();
				item.Text = "Item Extra";
				item.SubItems.Add ("subitem1");
				item.SubItems.Add ("subitem2");
				listView1.Items.Add (item);
			} else if (cmd == "Open.ChildWindow") {
				if (childForm == null) {
					childForm = new Form ();
					childForm.Text = "TestForm1.ChildForm1";
				}
				childForm.Show ();
			} else if (cmd == "Close.ChildWindow") {
				if (childForm != null) {
					childForm.Close ();
					childForm = null;
				}
			} else if (cmd == "Toggle.Window.CanMaximize") {
				MaximizeBox = !MaximizeBox;
			} else if (cmd == "Toggle.Window.CanMinimize") {
				MinimizeBox = !MinimizeBox;
			} else if (cmd == "Toggle.Window.IsTopmost") {
				TopMost = !TopMost;
			} else if (cmd == "Open.ModalChildWindow") {
				new Form () {Text = "TestForm1.ModalForm1"}.ShowDialog (this);
			} else if (cmd == "Sleep.2000") {
				System.Threading.Thread.Sleep (2000);
			} else if (cmd == "enable multiselect")
				listView1.MultiSelect = true;
			else if (cmd == "disable multiselect")
				listView1.MultiSelect = false;
			else if (cmd == "change button3 name") {
				button3.AccessibleName = "xyzzy";
			} else if (cmd == "change button3 helptext") {
				button3.AccessibleDescription = "plugh";
			} else if (cmd == "enable button3") {
				button3.Enabled = true;
			} else if (cmd == "disable button3") {
				button3.Enabled = false;
			} else if (cmd == "focus textBox3")
 				textBox3.Focus ();
			else if (cmd == "focus button2")
				button2.Focus ();
		}
	}
}
