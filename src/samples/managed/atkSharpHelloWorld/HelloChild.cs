// HelloChild.cs created with MonoDevelop
// User: knocte at 6:28 PM 3/21/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;

namespace atkSharpHelloWorld
{
	
	
	public class HelloChild : Atk.Object//, Atk.Text
	{
		public HelloChild(string name)
		{
			this.Name = name;
			this.Role = Atk.Role.Window;
		}
		
		public Atk.Object[] Children
		{
			get { return new Atk.Object[0]; } // no grandchildren
		}
		
		//although this returns 0 already on the base, it may become an abstract method
		protected override int OnGetNChildren()
		{
			return 0;
		}
		
		protected override Atk.Object OnRefChild(int i)
		{
			//FIXME: we should assert here about 'should not be reached'
			return null;
		}
	}
}
