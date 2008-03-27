// HelloChild.cs created with MonoDevelop
// User: knocte at 6:28 PM 3/21/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;

namespace atkSharpHelloWorld
{
	
	
	public class HelloChild : Atk.Object, Atk.Text
	{
		public HelloChild(string name)
		{
			this.Name = name;
		}
		
		public Atk.Object[] Children
		{
			get { return new Atk.Object[0]; } // no grandchildren
		}
		
		public new Atk.Role Role
		{
			get { return Atk.Role.Window; }
		}
	}
}
