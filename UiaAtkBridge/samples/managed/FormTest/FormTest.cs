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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace FormTest
{
	public class FormTest
	{
		static void Main (string[] args)
		{
			if (args.Length > 0 && args [0] == "dialog")
				new OpenFileDialog ().ShowDialog ();
			else
				Application.Run (BuildGui ());
		}
		
		static void OnButtonClick (object sender, EventArgs args)
		{
			Form f2 = new Form ();
			f2.Text = "Secondary Form";
			
			f2.Show ();
			new OpenFileDialog ().ShowDialog ();
		}

		static private void Color_Clicked(object sender, System.EventArgs e)
		{
		}

		static public void InitializeMyToolBar(Form form)
		{
			// Create and initialize the ToolBar and ToolBarButton controls.
			ToolBar toolBar1 = new ToolBar();
			ToolBarButton toolBarButton1 = new ToolBarButton();
			ToolBarButton toolBarButton2 = new ToolBarButton();
			ToolBarButton toolBarButton3 = new ToolBarButton();
	
			// Set the Text properties of the ToolBarButton controls.
			toolBarButton1.Text = "Open";
			toolBarButton2.Text = "Save";
			toolBarButton3.Text = "Print";
	
			// Add the ToolBarButton controls to the ToolBar.
			toolBar1.Buttons.Add(toolBarButton1);
			toolBar1.Buttons.Add(toolBarButton2);
			toolBar1.Buttons.Add(toolBarButton3);
	
			//Add the event-handler delegate.
			toolBar1.ButtonClick += new ToolBarButtonClickEventHandler (
			  toolBar1_ButtonClick);
	
			// Add the ToolBar to the Form.
			form.Controls.Add(toolBar1);
 		}

		static private void toolBar1_ButtonClick (Object sender, 
		                                          ToolBarButtonClickEventArgs e)
		{
			// Evaluate the Button property to determine which button was clicked.
			new OpenFileDialog ().ShowDialog();
		}

		
		static Form BuildGui ()
		{
			Form f1 = new Form ();
			f1.Text = "Main Form";
			f1.Height += 90;
			f1.Width += 70;

//			//InitializeMyToolBar (f1);
//			TextBox t;
//			t.PasswordChar = "*";
			Button b = new Button ();
			b.Text = "Click me to open second form!";
			b.Width = f1.Width;
			b.Click += OnButtonClick;
			f1.Controls.Add (b);
			
			Button b2 = new Button ();
			b2.Text = "I'm inactive";
			b2.Width = f1.Width;
			b2.Enabled = false;
			//b2.Location.Y = f1.Height / 2;
			b2.Top = b.Height;
			f1.Controls.Add (b2);

			Label lab = new Label ();
			lab.Text = "This is a test label";
			lab.Width = f1.Width;
			lab.Height = b.Height;
			lab.Enabled = true;
			lab.Top = b.Height + 30;
			f1.Controls.Add (lab);

			ContextMenuStrip contextMenu = new ContextMenuStrip();
			contextMenu.Items.Add("&Color");
			contextMenu.Items.Add("&Font");
			lab.ContextMenuStrip = contextMenu;
			
			CheckBox chk = new CheckBox ();
			chk.Text = "This is a test checkbox";
			chk.Width = f1.Width;
			chk.Height = b.Height;
			chk.Enabled = true;
			chk.Top = lab.Top + 30;
			f1.Controls.Add (chk);
			
			RadioButton rad1 = new RadioButton ();
			RadioButton rad2 = new RadioButton ();
			rad1.Text = "Test radiobutton 1";
			rad2.Text = "Test radiobutton 2";
			rad1.Width = f1.Width;
			rad2.Width = f1.Width;
			rad1.Height = b.Height;
			rad1.Top = chk.Top + 30;
			rad2.Top = rad1.Top + 30;
			f1.Controls.Add (rad1);
			f1.Controls.Add (rad2);
			
			ComboBox cbx = new ComboBox ();
			cbx.DropDownStyle = ComboBoxStyle.DropDownList;
			cbx.FormattingEnabled = true;
			cbx.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			cbx.Items.AddRange(new object[] {
			  "Foo",
			  "Bar"});
			cbx.Width = f1.Width - 30;
			cbx.Height = b.Height;
			cbx.Top = rad2.Top + 30;
			
			f1.Controls.Add (cbx);
			
			GroupBox gpb = new GroupBox ();
			gpb.Width = f1.Width - 30;
			gpb.Height = b.Height * 4;
			gpb.Top = cbx.Top + 30;
			gpb.Text = "this is a groupbox";
			
			RadioButton rad3 = new RadioButton ();
			RadioButton rad4 = new RadioButton ();

			gpb.Controls.Add (rad3);
			gpb.Controls.Add (rad4);
			
			rad3.Text = "Test radiobutton 3";
			rad4.Text = "Test radiobutton 4";
			rad3.Width = f1.Width;
			rad4.Width = f1.Width;
			rad3.Top += 20;
			rad3.Height = b.Height;
			rad4.Height = b.Height;
			rad4.Top = rad3.Top + 30;

			f1.Controls.Add (gpb);

			TextBox tbx = new TextBox ();
			tbx.Top = gpb.Top + gpb.Height + 30;
			tbx.Width = f1.Width - 10;
			f1.Controls.Add (tbx);
			
			Form bgf = new Form ();
			bgf.Text = "Background Form";
			CreateMenu (bgf);
			bgf.Show ();
			
			return f1;
		}
		
		static void CreateMenu (Form f)
		{
			MenuStrip menuStrip1 = new MenuStrip();
			
			ToolStripMenuItem fileToolStripMenuItem = new ToolStripMenuItem();
			ToolStripMenuItem newToolStripMenuItem = new ToolStripMenuItem();
			ToolStripMenuItem quitToolStripMenuItem = new ToolStripMenuItem();
			ToolStripMenuItem helpToolStripMenuItem = new ToolStripMenuItem();
			ToolStripMenuItem aboutToolStripMenuItem = new ToolStripMenuItem();
			
			// 
			// menuStrip1
			// 
			menuStrip1.Items.AddRange (new ToolStripItem[] {
				fileToolStripMenuItem,
				helpToolStripMenuItem
			});
			menuStrip1.Location = new System.Drawing.Point (0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Size = new System.Drawing.Size (284, 24);
			menuStrip1.Text = "menuStrip1";
			
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange (new ToolStripItem[] {
				newToolStripMenuItem,
				quitToolStripMenuItem
			});
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			fileToolStripMenuItem.Text = "File";
			
			// 
			// newToolStripMenuItem
			// 
			newToolStripMenuItem.Name = "newToolStripMenuItem";
			newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			newToolStripMenuItem.Text = "New";

			// 
			// quitToolStripMenuItem
			// 
			quitToolStripMenuItem.Name = "quitToolStripMenuItem";
			quitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			quitToolStripMenuItem.Text = "Quit";

			// 
			// helpToolStripMenuItem
			// 
			helpToolStripMenuItem.DropDownItems.AddRange (
			  new System.Windows.Forms.ToolStripItem[] { aboutToolStripMenuItem });
			helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			helpToolStripMenuItem.Text = "Help";

			// 
			// aboutToolStripMenuItem
			// 
			aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			aboutToolStripMenuItem.Text = "About";
			
			
			
			f.Controls.Add(menuStrip1);
			f.MainMenuStrip = menuStrip1;
		}
	}
}
