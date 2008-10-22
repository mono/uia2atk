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
// 
using System;
using System.Collections.Generic;

using Gtk;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		
		radioButtons.Add (this.radTest1);
		radioButtons.Add (this.radTest2);
		radioButtons.Add (this.radiobutton1);
		radioButtons.Add (this.radiobutton2);
		
		string uiaQaPath = LookForImagesDir ();
		Gtk.Image img1 = new Gtk.Image ();
		img1.FromFile = System.IO.Path.Combine (uiaQaPath, "opensuse60x38.gif");
		img1.Show ();
		this.btnWithImg.Image = img1;
		
		Gtk.Image img2 = new Gtk.Image ();
		img2.FromFile = System.IO.Path.Combine (uiaQaPath, "soccerball.gif");
		img2.Show ();
		this.checkbutton1.Image = img2;

		Gtk.Image img3 = new Gtk.Image ();
		img3.FromFile = System.IO.Path.Combine (uiaQaPath, "apple-red.png");
		img3.Show ();
		this.radTest2.Image = img3;

		this.imgTest1.FromFile = System.IO.Path.Combine (uiaQaPath, "goalie.gif");

		this.imgTest2.FromFile = System.IO.Path.Combine (uiaQaPath, "apple-red.png");
	}

	private static string LookForImagesDir () {
		string imgDir = System.IO.Directory.GetCurrentDirectory ();
		
		while (imgDir != "/"){
			if (System.IO.Directory.GetFiles (imgDir, "*.gif").Length == 0)
				imgDir = System.IO.Path.GetFullPath (System.IO.Path.Combine (imgDir, ".."));

			else
				break;
			
			string samples = System.IO.Path.Combine (System.IO.Path.Combine (imgDir, "test"), "samples");
			if (System.IO.Directory.Exists (samples)) { 
				if (System.IO.Directory.GetFiles (samples, "*.gif").Length > 0) {
					imgDir = System.IO.Path.GetFullPath (samples);
					break;
				}
			}
		}

		if (imgDir != "/")
			return imgDir;

		return null;
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	
	internal Gtk.Label GiveMeARealLabel () {
		return this.lblTest1;
	}
	
	internal Gtk.TreeView GiveMeARealTreeView () {
		return treeview2;
	}
	
	internal Gtk.Image GiveMeARealImage (bool embeddedImage) {
		return (embeddedImage ? this.imgTest1 : this.imgTest2);
	}
	
	internal Gtk.Button GiveMeARealButton (bool embeddedImage) {
		return (embeddedImage ? this.btnWithImg : this.btnTest1);
	}
	
	internal Gtk.CheckButton GiveMeARealCheckBox (bool embeddedImage) {
		return (embeddedImage ? this.checkbutton1 : this.chkTest);
	}
	
	internal Gtk.ComboBox GiveMeARealComboBox () {
		return this.cbxTest;
	}

	internal Gtk.ComboBox GiveMeARealComboBoxEntry () {
		return this.cbeTest;
	}
	
	internal Gtk.Entry GiveMeARealEntry () {
		return this.txtEntry;
	}

	internal Gtk.TextView GiveMeARealTextView () {
		return this.txtViewTest;
	}
	
	internal Gtk.ImageMenuItem GiveMeARealParentMenu (string name) {
		Gtk.ImageMenuItem ret = (Gtk.ImageMenuItem)this.menubar1.Children[0];
		
		if (!String.IsNullOrEmpty (name)) {
			Gtk.Application.Invoke (delegate {
				((Gtk.AccelLabel)ret.Child).Text = name;
			});
			System.Threading.Thread.Sleep (1000);
		}

//DELETEME: testing
//		Console.WriteLine ("NUUUUUUM children imenuil:");
//		Console.WriteLine (((Gtk.Menu)ret.Submenu).Children.Length + "he");
//		Gtk.MenuItem mi = (Gtk.MenuItem) ((Gtk.Menu)ret.Submenu).Children[1];
//		Gtk.MenuItem empty = (Gtk.MenuItem) ((Gtk.Menu)ret.Submenu).Children[((Gtk.Menu)ret.Submenu).Children.Length - 1];
//		((Gtk.Menu)ret.Submenu).Remove (empty);
//		((Gtk.Menu)ret.Submenu).Add (new Gtk.MenuItem ("testingg"));
//		((Gtk.Menu)ret.Submenu).Add (empty);
//		System.Threading.Thread.Sleep (10000);
//		//((Gtk.Menu)ret.Submenu).Children = ((Gtk.Menu)ret.Submenu).Children;
//		Console.WriteLine ("hey:" + ((Gtk.AccelLabel)mi.Child).Text);
		return ret;
	}
	
	internal Gtk.Notebook GiveMeARealNotebook () {
		return this.notebook1;
	}
	
	List <Gtk.RadioButton> radioButtons = new List <Gtk.RadioButton> ();
	
	int radioButtonToReturn = -1;
	
	internal Gtk.RadioButton GiveMeARealRadioButton (bool embeddedImage)
	{
		if (embeddedImage)
			return radTest2;
		
		if (radioButtonToReturn == 3)
			radioButtonToReturn = -1;
		
		radioButtonToReturn++;
		return radioButtons [radioButtonToReturn];
	}

	internal Gtk.HScrollbar GiveMeARealHScrollbar () {
		return this.hscrollbar1;
	}

	internal Gtk.VScrollbar GiveMeARealVScrollbar () {
		return this.vscrollbar1;
	}

	internal Gtk.Statusbar GiveMeARealStatusbar () {
		return this.statusbar1;
	}

	internal Gtk.ProgressBar GiveMeARealProgressBar () {
		return this.progressbar1;
	}
	
}
