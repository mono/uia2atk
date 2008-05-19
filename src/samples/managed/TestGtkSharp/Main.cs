// Main.cs created with MonoDevelop
// User: knocte at 8:12 PM 3/14/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
using System;
using Gtk;

namespace TestGtkSharp
{
	class MainClass
	{
		private static System.Timers.Timer timer = new System.Timers.Timer();
		
		static MainWindow theWin;
		
		public static void Main (string[] args)
		{
			Application.Init ();
			theWin = new MainWindow ();
			theWin.Show ();
			Application.Run ();
		}

	}
}