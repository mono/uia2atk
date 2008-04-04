// HelloChild.cs created with MonoDevelop
// User: knocte at 6:28 PM 3/21/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;

namespace atkSharpHelloWorld
{
	
	
	public class HelloChild : Atk.Object, Atk.ComponentImplementor
	{
		public HelloChild(string name)
		{
			this.Name = name;
			this.Role = Atk.Role.Frame;
			
			// can't set, it's readonly currently
			//this.Layer = Atk.Layer.Window;
			
			//-1 is the default if there are no brothers in which to compare depth
			// can't set, it's readonly currently
			//this.MdiZorder = -1;
		}
		
		public Atk.Object[] Children
		{
			get { return new Atk.Object[0]; } // no grandchildren
		}

		public double Alpha {
			get {
				//FIXME: 1.0 is the default opacitiy
				//throw new System.NotImplementedException();
				return 1.0;
			}
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

		public Atk.Object RefAccessibleAtPoint (int x, int y, Atk.CoordType coord_type)
		{
			//this is still not implemented, but better to return null to prevent crash
			//throw new System.NotImplementedException();
			return null;
		}

		public bool Contains (int x, int y, Atk.CoordType coord_type)
		{
			throw new System.NotImplementedException();
		}

		public void GetPosition (out int x, out int y, Atk.CoordType coord_type)
		{
			x = 50;
			y = 60;
		}

		public void GetExtents (out int x, out int y, out int width, out int height, Atk.CoordType coord_type)
		{
			//coord_type specifies to which concept the coords are relative too
			x = 30;
			y = 40;
			width = 300;
			height = 400;
		}

		public uint AddFocusHandler (Atk.FocusHandler handler)
		{
			throw new System.NotImplementedException();
		}

		public bool SetSize (int width, int height)
		{
			//this is still not implemented, but better to return true to prevent crash
			//throw new System.NotImplementedException();
			return true;
		}

		public bool SetExtents (int x, int y, int width, int height, Atk.CoordType coord_type)
		{
			throw new System.NotImplementedException();
		}

		public void GetSize (out int width, out int height)
		{
			width = 500;
			height = 600;
		}

		public bool GrabFocus ()
		{
			throw new System.NotImplementedException();
		}

		public bool SetPosition (int x, int y, Atk.CoordType coord_type)
		{
			throw new System.NotImplementedException();
		}

		public void RemoveFocusHandler (uint handler_id)
		{
			throw new System.NotImplementedException();
		}

		
		public event Atk.BoundsChangedHandler BoundsChanged;
	}
}
