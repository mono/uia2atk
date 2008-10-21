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
	public class GailTester : AtkTests {

		static GailTestApp.MovingThread guiThread = null;
		
		static GailTester ()
		{
			try {
				GLib.Global.ProgramName = "nunit-console";
				guiThread = new GailTestApp.MovingThread ();
				guiThread.NotMainLoopDeleg = Gtk.Application.Init;
				guiThread.Start ();
				guiThread.JoinUntilSuspendedByMainLoop ();
	
				GailTestApp.MainClass.StartRemotely (guiThread);
				System.Threading.Thread.Sleep (1000);
				EventMonitor.Start ();
				System.Threading.Thread.Sleep (1000);
			}
			catch (Exception e) {
				Console.WriteLine ("Exception at tests initialization: " + e);
				throw;
			}
		}

		bool alreadyInGuiThread = false;
		
		public override void RunInGuiThread (System.Action d)
		{
			if (alreadyInGuiThread) {
				Console.WriteLine ();
				Console.WriteLine ("WARNING: You called RunInGuiThread nestedly.");
				d ();
				return;
			}
			alreadyInGuiThread = true;
			
			try {
				System.Threading.EventWaitHandle h = guiThread.CallDelegInMainLoop (d);
				h.WaitOne ();
				h.Close ();
				if (guiThread.ExceptionHappened != null)
					throw guiThread.ExceptionHappened;
			}
			finally {
				alreadyInGuiThread = false;
			}
			System.Threading.Thread.Sleep (2000);
		}
		
		public override Atk.Object GetAccessible (BasicWidgetType type, string text, bool real)
		{
			return GetAccessible (type, text, real, false);
		}

		public override I CastToAtkInterface <I> (Atk.Object accessible)
		{
			if (typeof (I) == typeof (Atk.Action)) {
				return Atk.ActionAdapter.GetObject (accessible.Handle, false) as I;
			}
			else if (typeof (I) == typeof (Atk.Text)) {
				return Atk.TextAdapter.GetObject (accessible.Handle, false) as I;
			}
			else if (typeof (I) == typeof (Atk.Component)) {
				return Atk.ComponentAdapter.GetObject (accessible.Handle, false) as I;
			}
			else if (typeof (I) == typeof (Atk.EditableText)) {
				return Atk.EditableTextAdapter.GetObject (accessible.Handle, false) as I;
			}
			else if (typeof (I) == typeof (Atk.Image)) {
				return Atk.ImageAdapter.GetObject (accessible.Handle, false) as I;
			}
			else if (typeof (I) == typeof (Atk.Table)) {
				return Atk.TableAdapter.GetObject (accessible.Handle, false) as I;
			}
			else if (typeof (I) == typeof (Atk.Selection)) {
				return Atk.SelectionAdapter.GetObject (accessible.Handle, false) as I;
			}
			else if (typeof (I) == typeof (Atk.Value)) {
				return Atk.ValueAdapter.GetObject (accessible.Handle, false) as I;
			}
			throw new NotImplementedException ("Couldn't cast to interface " +
			  typeof (I).Name);
		}
		
		public override Atk.Object GetAccessible (BasicWidgetType type, string[] name, bool real)
		{
			Atk.Object accessible = null;
			//this is because of this:
//Gtk-CRITICAL **: gtk_combo_box_append_text: assertion `GTK_IS_LIST_STORE (combo_box->priv->model)' failed
//Gtk-CRITICAL **: gtk_combo_box_append_text: assertion `GTK_IS_LIST_STORE (combo_box->priv->model)' failed
//Gtk-CRITICAL **: gtk_combo_box_append_text: assertion `GTK_IS_LIST_STORE (combo_box->priv->model)' failed
			if (!real)
				throw new NotSupportedException ("We cannot add items to a non-real ComboBox because of some GtkCritical");
			
			Gtk.Widget widget = null;
			
			switch (type) {
			case BasicWidgetType.ComboBoxDropDownList:
				widget = new Gtk.ComboBox ();
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealComboBox ();
	
				//FIXME: update this line when this bug is fixed: http://bugzilla.gnome.org/show_bug.cgi?id=324899
				((Gtk.ListStore)((Gtk.ComboBox) widget).Model).Clear ();
				
				foreach (string text in name) {
					((Gtk.ComboBox)widget).AppendText (text);
				}
				break;
			case BasicWidgetType.TabControl:
				widget = new Gtk.Notebook ();
				// real not implemented yet
				//if (real)
					//widget = GailTestApp.MainClass.GiveMeARealNotebook (guiThread);
				int i = 0;
				foreach (string text in name)
					((Gtk.Notebook)widget).AppendPage (new Gtk.Label (text), new Gtk.Label ("Tab " + i++));
				break;
			case BasicWidgetType.ParentMenu:
				if (!real)
					throw new NotSupportedException ("No unreal widget access for ParentMenu now");
				
				widget = GailTestApp.MainClass.GiveMeARealParentMenu (name [0]);
				
				Gtk.Application.Invoke (delegate {
					//figure out how to remove old menus and add these ones: names[from 1]
				});
				break;
			default:
				throw new NotSupportedException ("This AtkTester overload doesn't handle this type of widget: " +
					type.ToString ());
			}
			
			accessible = widget.Accessible;
			
			return accessible;
		}

		public override Atk.Object GetAccessibleThatEmbedsAnImage (
		  BasicWidgetType type, string text, bool real)
		{
			return GetAccessible (type, text, real, true);
		}
		
		private Atk.Object GetAccessible (BasicWidgetType type, string text, bool real, bool embeddedImage)
		{
			Atk.Object accessible = null;
			Gtk.Widget widget = null;
			Gtk.Adjustment adj = new Gtk.Adjustment (50, 0, 100, 1, 10, 20);
			switch (type) {
			case BasicWidgetType.Label:
				widget = new Gtk.Label ();
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealLabel ();
				((Gtk.Label)widget).Text = text;
				break;
			case BasicWidgetType.NormalButton:
				widget = new Gtk.Button ();
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealButton (embeddedImage);
				((Gtk.Button)widget).Label = text;
				break;
			case BasicWidgetType.Window:
				widget = new Gtk.Window (text);
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealWindow ();
				break;
			case BasicWidgetType.CheckBox:
				widget = new Gtk.CheckButton ();
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealCheckBox (embeddedImage);
				((Gtk.CheckButton)widget).Label = text;
				break;
			case BasicWidgetType.RadioButton:
				if (!real)
					throw new NotSupportedException ("We cannot use non-real radio buttons because of some wierd behaviour");
				
				widget = GailTestApp.MainClass.GiveMeARealRadioButton (embeddedImage);
				RunInGuiThread (delegate {
					((Gtk.CheckButton)widget).Label = text;
				});
				break;
			case BasicWidgetType.StatusBar:
				widget = new Gtk.Statusbar ();
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealStatusbar ();
				((Gtk.Statusbar)widget).Push (0, text);
				break;
			case BasicWidgetType.TextBoxEntry:
				if (!real)
					throw new NotSupportedException ();
				
				widget = GailTestApp.MainClass.GiveMeARealEntry ();
				Gtk.Application.Invoke (delegate {
					((Gtk.Entry)widget).Text = text;
				});
				System.Threading.Thread.Sleep (1000);
				break;
			case BasicWidgetType.TextBoxView:
				if (!real)
					throw new NotSupportedException ();
				
				widget = GailTestApp.MainClass.GiveMeARealTextView ();
				Gtk.Application.Invoke (delegate {
					((Gtk.TextView)widget).Buffer.Text = text;
				});
				System.Threading.Thread.Sleep (1000);
				break;
			case BasicWidgetType.HScrollBar:
				widget = new Gtk.HScrollbar (adj);
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealHScrollbar ();
				break;
			case BasicWidgetType.VScrollBar:
				widget = new Gtk.VScrollbar (adj);
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealVScrollbar ();
				break;
			case BasicWidgetType.ProgressBar:
				widget = new Gtk.ProgressBar (adj);
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealProgressBar ();
				break;
			case BasicWidgetType.Spinner:
				widget = new Gtk.SpinButton (adj, 1, 2);
				// real not implemented yet
				//if (real)
					//widget = GailTestApp.MainClass.GiveMeARealSpinButton ();
				break;
			case BasicWidgetType.ComboBoxDropDownEntry:
			case BasicWidgetType.ComboBoxDropDownList:
			case BasicWidgetType.ComboBoxSimple:
				throw new NotSupportedException ("You have to use the GetObject overload that receives a name array");
			default:
				throw new NotImplementedException ("The widget finder backend still hasn't got support for " +
					type.ToString ());
			}
			
			accessible = widget.Accessible;
			
			return accessible;
		}
		
		protected override int ValidNumberOfActionsForAButton { get { return 3; } }
		protected override int ValidNChildrenForASimpleStatusBar { get { return 1; } }
		protected override int ValidNChildrenForAScrollBar { get { return 0; } }
		
		[TestFixtureTearDown]
		public void End () 
		{
			try {
				EventMonitor.Stop ();
				GailTestApp.MainClass.Kill (guiThread);
			
				//hack: let's wait for the Gtk.Application env to finish gracefully and then we abort the thread
				System.Threading.Thread.Sleep (1000);

				guiThread.Dispose ();
			}
			catch (Exception e) {
				Console.WriteLine ("Exception occurred in TestFixtureDown:" + e);
				throw;
			}
		}

		
		
	}
}
