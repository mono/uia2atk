// Main.cs created with MonoDevelop
// User: knocte at 10:35 AM 3/31/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
using System;
using UiaAtkBridge;

namespace TestUiaAtkBridge
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Monitor appMonitor = new Monitor();
			appMonitor.FormIsAdded("Main Window");
			appMonitor.FormIsAdded("Widgets Window");
			appMonitor.ApplicationStarts();
		}
	}
}