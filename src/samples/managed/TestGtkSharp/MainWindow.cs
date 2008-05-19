// MainWindow.cs created with MonoDevelop
// User: knocte at 8:12 PM 3/14/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
using System;
using Gtk;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}
	
	public void OnTest()
	{
		//this.button188.Sensitive = false;
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
		
	}

	protected virtual void ButtonActivated (object sender, System.EventArgs e)
	{
		Console.WriteLine ("hola");
		new TestGtkSharp.Test().Show();
	}

	protected virtual void OnOpenActionActivated (object sender, System.EventArgs e)
	{
		Console.WriteLine("hey");
		ButtonActivated (null, null);
	}
}