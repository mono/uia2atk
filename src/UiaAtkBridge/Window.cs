// Window.cs created with MonoDevelop
// User: knocte at 12:32 AM 3/31/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;

namespace UiaAtkBridge
{
	public class Window : Atk.Object
	{
		public Window(string name)
		{
			this.Name = name;
		}
	}
}
