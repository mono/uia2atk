// HelloUtil.cs created with MonoDevelop
// User: knocte at 6:18 PM 3/21/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;

namespace atkSharpHelloWorld
{
	public class HelloUtil : Atk.Util
	{
		public HelloUtil()
		{
		}
		
		public Atk.Object Root
		{
			get { return HelloTopLevel.Instance; }
		}
	}
}
