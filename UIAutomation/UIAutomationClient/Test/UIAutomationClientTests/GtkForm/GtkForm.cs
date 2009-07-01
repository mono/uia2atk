using System;

using Gtk;

namespace GtkForm
{
	public class DemoMain
	{
		private Gtk.Window window;

		public static void Main (string[] args)
		{
			Application.Init ();
			new DemoMain ();
			Application.Run ();
		}

		public DemoMain ()
		{
			window = new Gtk.Window ("Sample GTK form");
			Gtk.HBox hbox = new Gtk.HBox (false, 0);
			Gtk.HBox hbox1 = new Gtk.HBox (false, 0);
			Gtk.HBox hbox2 = new Gtk.HBox (false, 0);
			Gtk.HBox hbox3 = new Gtk.HBox (false, 0);
			hbox.Add (hbox1);
			window.SetDefaultSize (600, 400);
			window.DeleteEvent += new DeleteEventHandler (WindowDelete);

			Gtk.Button button1 = new Gtk.Button ("button1");
			Gtk.Button button2 = new Gtk.Button ("button2");
			Gtk.Button button3 = new Gtk.Button ("button3");
			Gtk.Button button4 = new Gtk.Button ("button4");
			Gtk.Button button5 = new Gtk.Button ("button5");
			Gtk.Button button6 = new Gtk.Button ("button6");
			Gtk.Button button7 = new Gtk.Button ("button7");
			button7.Sensitive = false;

			hbox1.Add (hbox3);
			hbox1.Add (hbox2);
			hbox1.Add (button3);
			hbox1.Add (button2);

			button3.Accessible.Description = "help text 3";
			button3.Sensitive = false;

			Gtk.Label label1 = new Gtk.Label ("label1");

			Gtk.Entry textBox1 = new Gtk.Entry ();
			Gtk.Entry textBox2 = new Gtk.Entry ();
			textBox2.Visibility = false;
			Gtk.TextView textBox3 = new Gtk.TextView ();
			// TODO: scrollbars
			Gtk.CheckButton checkbox1 = new Gtk.CheckButton ("checkbox1");
			Gtk.CheckButton checkbox2 = new Gtk.CheckButton ("checkbox2");
			checkbox2.Sensitive = false;

			hbox2.Add (button5);
			hbox2.Add (checkbox1);
			hbox2.Add (checkbox2);
			hbox2.Add (button4);

			hbox3.Add (button7);
			hbox3.Add (button6);
			hbox3.Sensitive = false;

			hbox.Add (textBox3);
			hbox.Add (textBox2);
			hbox.Add (textBox1);
			hbox.Add (label1);
			hbox.Add (button1);

			window.Add (hbox);
			window.ShowAll ();
		}

		private void WindowDelete (object o, DeleteEventArgs args)
		{
			Application.Quit ();
			args.RetVal = true;
		}
	}
}
