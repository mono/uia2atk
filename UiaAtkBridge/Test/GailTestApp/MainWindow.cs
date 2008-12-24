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
		
		string uiaQaPath = UiaAtkBridgeTest.Misc.LookForParentDir ("*.gif");
		Gtk.Image img1 = new Gtk.Image ();
		img1.File = System.IO.Path.Combine (uiaQaPath, "opensuse60x38.gif");
		img1.Show ();
		this.btnWithImg.Image = img1;
		
		Gtk.Image img2 = new Gtk.Image ();
		img2.File = System.IO.Path.Combine (uiaQaPath, "soccerball.gif");
		img2.Show ();
		this.checkbutton1.Image = img2;

		Gtk.Image img3 = new Gtk.Image ();
		img3.File = System.IO.Path.Combine (uiaQaPath, "apple-red.png");
		img3.Show ();
		this.radTest2.Image = img3;

		this.imgTest1.File = System.IO.Path.Combine (uiaQaPath, "goalie.gif");

		this.imgTest2.File = System.IO.Path.Combine (uiaQaPath, "apple-red.png");

		this.maskedEntry.Visibility = false;

		 
		this.hscrollbar1.Adjustment.Lower = 0; //Value tested in AtkTester.InterfaceValue
		this.hscrollbar1.Adjustment.Upper = 100; //Value tested in AtkTester.InterfaceValue
		this.hscrollbar1.Adjustment.PageSize = 1;
		this.hscrollbar1.Adjustment.StepIncrement = 1;

		this.vscrollbar1.Adjustment.Lower = 0; //Value tested in AtkTester.InterfaceValue
		this.vscrollbar1.Adjustment.Upper = 100; //Value tested in AtkTester.InterfaceValue
		this.vscrollbar1.Adjustment.PageSize = 1;
		this.vscrollbar1.Adjustment.StepIncrement = 1;

		//Used when testing Atk.Action.GetKeybinding
		this.btnWithImg.UseUnderline = true;
		this.btnTest1.UseUnderline = true;

		this.WindowPosition = WindowPosition.CenterAlways;

		Gtk.ToolItem item = new Gtk.ToolItem ();
		item.Add (new Gtk.Entry ("TEST TEXT"));
		this.toolbar1.Insert (item, 3);
		this.toolbar1.ShowAll ();
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
	
	internal Gtk.Entry GiveMeARealEntry (bool visible) {
		return visible ? this.txtEntry : this.maskedEntry;
	}

	internal Gtk.TextView GiveMeARealTextView () {
		return this.txtViewTest;
	}
	
	internal Gtk.MenuBar GiveMeARealMenuBar () {
		return this.menubar1;
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

	internal Gtk.SpinButton GiveMeARealSpinButton () {
		return this.spinbuttonTest1;
	}

	internal Gtk.Frame GiveMeARealFrame () {
		return this.frame1;
	}
}
