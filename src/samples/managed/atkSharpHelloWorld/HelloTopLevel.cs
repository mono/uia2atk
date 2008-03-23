// HelloTopLevel.cs created with MonoDevelop
// User: knocte at 6:20 PM 3/21/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections;


namespace atkSharpHelloWorld
{
	
	
	class HelloTopLevel : Atk.Object
	{
		private static HelloTopLevel instance;
		
		public static HelloTopLevel Instance {
			get
			{
				lock(typeof(HelloTopLevel))
				{
					if (instance == null)
					{
						instance = new HelloTopLevel();
					}
					return instance;
				}
			}
		}
		
		protected HelloTopLevel()
		{
			this.Name = GLib.ProgramName;
		}
		
		public Atk.Object[] Children
		{
			get {
				List<AtkObject> accessibles = new List<Atk.Object>();
				foreach(Widget window in Mytk.MytkGlobal.TopLevelWindows)
				{
					accessibles.Add(window.GetAccessible());
				}
				return accessibles.ToArray();
			}
		}
		
		public new Atk.Role Role
		{
			get { return Atk.Role.Application; }
		}
	}
}
