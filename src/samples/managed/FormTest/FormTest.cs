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
			Form f1 = new Form ();
			f1.Text = "Main Form";
			
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
            cbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbx.FormattingEnabled = true;
            cbx.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            cbx.Items.AddRange(new object[] {
            "Foo",
            "Bar"});
			cbx.Width = f1.Width - 30;
			cbx.Height = b.Height;
			cbx.Top = rad2.Top + 30;
			
			f1.Controls.Add (cbx);
			
			Form bgf = new Form ();
			bgf.Text = "Background Form";
			bgf.Show ();
			
			Application.Run (f1);
		}
		
		static void OnButtonClick (object sender, EventArgs args)
		{
			Form f2 = new Form ();
			f2.Text = "Secondary Form";
			
			f2.Show ();
		}
	}
}
