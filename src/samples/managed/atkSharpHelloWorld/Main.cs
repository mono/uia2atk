// Main.cs created with MonoDevelop
// User: knocte at 5:59 PM 3/21/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
using System;
using System.Runtime.InteropServices;

namespace atkSharpHelloWorld
{
	class MainClass
	{
		private static int defaultNumberOfTopLevels = 2;
		
		private static System.Timers.Timer timer = new System.Timers.Timer();
		
		public static void Main(string[] args)
		{
			GLib.Program.Name = System.IO.Path.GetFileNameWithoutExtension (Environment.GetCommandLineArgs () [0]);
			
			GLib.GType.Init();
			
			StartProgramGui();
			
			Atk.Util.GetToolkitNameHandler = HelloUtil.GetToolkitName;
			Atk.Util.GetToolkitVersionHandler = HelloUtil.GetToolkitVersion;
			Atk.Util.GetRootHandler = HelloUtil.GetRoot;
			
			LaunchAtkBridge();
			
			timer.Interval = 10000;
			timer.Elapsed += FireTimer;
			timer.Enabled = true;
			
			new GLib.MainLoop().Run();
		}
		
		static void FireTimer(object o, System.Timers.ElapsedEventArgs args)
		{
			Console.WriteLine("I'm here!:" + HelloTopLevel.Instance.Role.ToString());
			
			Console.WriteLine("refchildname: is null?" + ((HelloTopLevel.Instance.RefChild(0)==null)?"yes":"no") +
			                  " Name:'" + HelloTopLevel.Instance.RefChild(0).Name + "'");
		}
		
		static void StartProgramGui()
		{
			if (Mytk.MytkGlobal.TopLevelWindows.Length > 0)
			{
				return;
			}
			
			for(int i = 0; i < defaultNumberOfTopLevels; i++)
			{
				Mytk.MytkGlobal.AddOneTopLevelWindow("TopLevel " + (i + 1));
			}
		}

		// we need to load this to communicate to AT-SPI
		// (this is done in gtk_init and in our UiaAtkBridge)
		[DllImport("libatk-bridge")]
		static extern void gnome_accessibility_module_init ();
		
		private static void LaunchAtkBridge()
		{
			gnome_accessibility_module_init();
		}
	}
}