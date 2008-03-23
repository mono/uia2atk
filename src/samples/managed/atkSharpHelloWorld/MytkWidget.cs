// MytkWidget.cs created with MonoDevelop
// User: knocte at 7:04 PM 3/21/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;

using atkSharpHelloWorld;

namespace atkSharpHelloWorld.Mytk
{
	
	public class Widget : Atk.IAccessible
	{
		private string name;
		private HelloChild accessible;
		
		public Widget(string name)
		{
			this.name = name;
			this.accessible = new HelloChild(name);
		}
		
		HelloChild Atk.IAccessible.Accessible
		{
			get { return accessible; }
		}
	}
}
