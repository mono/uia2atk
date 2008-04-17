// HelloUtil.cs created with MonoDevelop
// User: knocte at 9:33 PM 3/30/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;

namespace atkSharpHelloWorld
{
	
	public class HelloUtil 
		// Atk# doesn't work this way yet:
		//: Atk.Util
	{
		
		private HelloUtil()
		{
		}
		
		internal static Atk.Object GetRoot()
		{
			return HelloTopLevel.Instance;
		}
		
		internal static string GetToolkitName()
		{
			return "MANAGED-HELLO";
		}
		
		internal static string GetToolkitVersion()
		{
			return "1.1";
		}
		
		internal static int AddGlobalEventListener(IntPtr listener, string event_type)
		{
			//dummy function for now
			return 0;
		}
	}
}
