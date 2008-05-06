// HelloTopLevel.cs created with MonoDevelop
// User: knocte at 6:20 PM 3/21/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections;
using System.Collections.Generic;


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
			this.Name = GLib.Global.ProgramName;
			this.Role = Atk.Role.Application;
		}
		
		internal Atk.Object[] Children
		{
			get {
				List<Atk.Object> accessibles = new List<Atk.Object>();
				foreach(Mytk.Widget window in Mytk.MytkGlobal.TopLevelWindows)
				{
					accessibles.Add(window.RefAccessible());
				}
				return accessibles.ToArray();
			}
		}
		
		protected override int OnGetNChildren()
		{
			return this.Children.Length;
		}
		
		protected override Atk.Object OnRefChild(int i)
		{
			if (i >= Children.Length)
				return null;
			
			return Children[i];
		}
		
		public void FireChildrenChanged(uint changeIndex, Atk.Object child)
		{
			Console.WriteLine ("emitting");
			this.EmitChildrenChanged(Atk.Object.ChildrenChangedDetail.Remove, changeIndex, child);
			Console.WriteLine ("emitted");
		}
		
	}
}
