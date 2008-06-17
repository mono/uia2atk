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
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	
	internal Gtk.Label GiveMeARealLabel () {
		return this.lblTest1;
	}
	
	internal Gtk.Button GiveMeARealButton () {
		return this.btnTest1;
	}
	
	internal Gtk.CheckButton GiveMeARealCheckBox () {
		return this.chkTest;
	}
	
	internal Gtk.ComboBox GiveMeARealComboBox () {
		return this.cbxTest;
	}
	
	List <Gtk.RadioButton> radioButtons = new List <Gtk.RadioButton> ();
	
	int radioButtonToReturn = -1;
	
	internal Gtk.RadioButton GiveMeARealRadioButton () {

		if (radioButtonToReturn == 3)
			radioButtonToReturn = -1;
		
		radioButtonToReturn++;
		return radioButtons [radioButtonToReturn];
	}
}