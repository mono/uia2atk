// TopLevelItem.cs created with MonoDevelop
// User: knocte at 9:40 PM 3/30/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

namespace UiaAtkBridge
{
	
	
	public class TopLevelRootItem : Atk.Object 
	{
		private TopLevelRootItem()
		{
			this.Name = Monitor.GetProgramName();
			this.Children = new List<Atk.Object>();
		}
		
		private static TopLevelRootItem instance = null;
		
		public static TopLevelRootItem Instance {
			get {
				if (instance == null)
					instance = new TopLevelRootItem();
				
				return instance;
			}
		}
		
		private List<Atk.Object> Children;
		
		internal void AddOneChild(string name)
		{
			Window newWindow = new Window(name);
			this.Children.Add(newWindow);
		}
		
		protected override int OnGetNChildren() 
		{
			return this.Children.Count;
		}
	}
}
