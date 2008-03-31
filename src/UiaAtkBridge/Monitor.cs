// Main.cs created with MonoDevelop
// User: knocte at 9:11 PM 3/30/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
using System;

namespace UiaAtkBridge
{
	public class Monitor
	{
		public Monitor()
		{
			GLib.GType.Init();
			
			Atk.Util.GetRootHandler = ReturnTopLevel;
			
			Atk.Util.GetToolkitNameHandler = GetAssemblyName;
			Atk.Util.GetToolkitVersionHandler = GetAssemblyVersionNumber;
			
			Atk.Global.LaunchAtkBridge();
		}
		
		public void FormIsAdded(string name)
		{
			TopLevelRootItem.Instance.AddOneChild(name);
		}
		
		public void ApplicationStarts()
		{
			new GLib.MainLoop().Run();
		}
		
		internal static string GetProgramName()
		{
			return System.IO.Path.GetFileNameWithoutExtension (Environment.GetCommandLineArgs () [0]);
		}
		
		private static Atk.Object ReturnTopLevel()
		{
			return TopLevelRootItem.Instance;
		}
		
		private static string GetAssemblyName()
		{
			return typeof(Monitor).Assembly.GetName().Name;
		}
		
		private static string GetAssemblyVersionNumber()
		{
			return typeof(Monitor).Assembly.GetName().Version.ToString();
		}
	}
}