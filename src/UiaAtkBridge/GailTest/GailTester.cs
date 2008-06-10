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

using NUnit.Framework;

namespace UiaAtkBridgeTest
{
	
	[TestFixture]
	public class GailTester : AtkTester {

		static GailTester ()
		{
			Gtk.Application.Init ();
		}
		
		public override object GetAtkObjectThatImplementsInterface <I> (
		  BasicWidgetType type, string[] name, out Atk.Object accessible, bool real)
		{
			accessible = null;
			if (type != BasicWidgetType.ComboBox) {
				throw new NotSupportedException ("This AtkTester overload doesn't handle this type of widget: " +
					type.ToString ());
			}
			//this is because of this:
//Gtk-CRITICAL **: gtk_combo_box_append_text: assertion `GTK_IS_LIST_STORE (combo_box->priv->model)' failed
//Gtk-CRITICAL **: gtk_combo_box_append_text: assertion `GTK_IS_LIST_STORE (combo_box->priv->model)' failed
//Gtk-CRITICAL **: gtk_combo_box_append_text: assertion `GTK_IS_LIST_STORE (combo_box->priv->model)' failed
			if (!real)
				throw new NotSupportedException ("We cannot add items to a non-real widget because of some GtkCritical");
			
			Gtk.Widget widget = new Gtk.ComboBox ();
			if (real)
				widget = GailTestApp.MainClass.GiveMeARealComboBox ();

			//FIXME: update this line when this bug is fixed: http://bugzilla.gnome.org/show_bug.cgi?id=324899
			((Gtk.ListStore)((Gtk.ComboBox) widget).Model).Clear ();
			
			foreach (string text in name) {
				((Gtk.ComboBox)widget).AppendText (text);
			}
			
			accessible = widget.Accessible;
			
			if (typeof (I) == typeof (Atk.Component)) {
				return Atk.ComponentAdapter.GetObject (widget.Accessible.Handle, false);
			}
			else if (typeof (I) == typeof (Atk.Action)) {
				return Atk.ActionAdapter.GetObject (widget.Accessible.Handle, false);
			}
			else if (typeof (I) == typeof (Atk.Selection)) {
				return Atk.SelectionAdapter.GetObject (widget.Accessible.Handle, false);
			}
			throw new NotImplementedException ("The interface finder backend still hasn't got support for " +
				typeof(I).Name);
		}
		
		public override object GetAtkObjectThatImplementsInterface <I> (
		  BasicWidgetType type, string text, out Atk.Object accessible, bool real)
		{
			accessible = null;
			Gtk.Widget widget = null;
			switch (type) {
			case BasicWidgetType.Label:
				widget = new Gtk.Label ();
				((Gtk.Label)widget).Text = text;
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealLabel ();
				break;
			case BasicWidgetType.Button:
				widget = new Gtk.Button ();
				((Gtk.Button)widget).Label = text;
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealButton ();
				break;
			case BasicWidgetType.Window:
				widget = new Gtk.Window (text);
//				if (real)
//					widget = GailTestApp.MainClass.GiveMeARealWindow ();
				break;
			case BasicWidgetType.CheckBox:
				widget = new Gtk.CheckButton ();
				((Gtk.CheckButton)widget).Label = text;
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealCheckBox ();
				break;
			case BasicWidgetType.ComboBox:
				throw new NotSupportedException ("You have to use the GetObject overload that receives a name array");
			default:
				throw new NotImplementedException ("The widget finder backend still hasn't got support for " +
					type.ToString ());
			}
			
			accessible = widget.Accessible;
			
			if (typeof (I) == typeof (Atk.Text)) {
				return Atk.TextAdapter.GetObject (widget.Accessible.Handle, false);
			}
			else if (typeof (I) == typeof (Atk.Component)) {
				return Atk.ComponentAdapter.GetObject (widget.Accessible.Handle, false);
			}
			else if (typeof (I) == typeof (Atk.Action)) {
				return Atk.ActionAdapter.GetObject (widget.Accessible.Handle, false);
			}
			throw new NotImplementedException ("The interface finder backend still hasn't got support for " +
				typeof(I).Name);
		}
		
		protected override int ValidNumberOfActionsForAButton { get { return 3; } }
		
		[TestFixtureTearDown]
		public void End () 
		{
			GailTestApp.MainClass.Kill ();
		}
		
	}
}
