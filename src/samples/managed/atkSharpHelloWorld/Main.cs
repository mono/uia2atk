// Main.cs created with MonoDevelop
// User: knocte at 5:59 PM 3/21/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
using System;

namespace atkSharpHelloWorld
{
	class MainClass
	{
		private static int defaultNumberOfTopLevels = 2;
		
		public static void Main(string[] args)
		{
			GLib.ProgramName = args[0];
			
			GLib.GType.Init();
			
			StartProgramGui();
			
			Atk.Global.Initialize(HelloTopLevel.Instance, true); //true == bool InitBridge
			
			new GLib.MainLoop().Run();
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
	}
}