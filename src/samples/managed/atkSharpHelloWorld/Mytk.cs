// Mytk.cs created with MonoDevelop
// User: knocte at 7:04 PM 3/21/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

namespace atkSharpHelloWorld.Mytk
{
	public static class MytkGlobal
	{
		private static List<Widget> topLevelWindows = new List<Widget>();
		
		public static Widget[] TopLevelWindows
		{
			get 
			{
				return topLevelWindows.ToArray();
			}
		}
		
		public static Widget AddOneTopLevelWindow(string name)
		{
			Widget newWindow = new Widget(name);
			topLevelWindows.Add(newWindow);
			return newWindow;
		}
		
		public static void RemoveOneTopLevelWindow()
		{
			Atk.Object child = topLevelWindows[0].RefAccessible();
			topLevelWindows.Remove(topLevelWindows[0]);
			HelloTopLevel.Instance.FireChildrenChanged(0, child);
		}
	}
}
