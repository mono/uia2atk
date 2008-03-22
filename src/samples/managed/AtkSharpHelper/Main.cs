// Main.cs created with MonoDevelop
// User: knocte at 10:46 AM 3/10/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
using System;
using System.Windows.Automation.Provider;

namespace AtkSharpHelper
{
	class MainClass
	{
		public static void Initialize()
		{
			//FIXME: use this in order not to depend on Gtk#:
			//new GLib.GType();
			//(or better a method that mkestner created for me on r98056):
			//GLib.GType.Init();
			Gtk.Application.Init();
			
			
			//new Atk.Object().
			//Atk.Util AtkUtility = new Atk.Util(null);
			//Atk.Registry.
			Console.WriteLine("Tkname: " + Atk.Global.ToolkitName);
			Console.WriteLine("Tkversion: " + Atk.Global.ToolkitVersion);
			Console.WriteLine("Hello World!");
			if (Atk.Global.Root == null)
			{
				Console.WriteLine("root is null");
			}
			else
			{
				Console.WriteLine("root is not null");
			}
			Console.WriteLine("root role" + Atk.Global.Root.Role.ToString());
			
			if (Atk.Global.FocusObject == null)
			{
				Console.WriteLine("fo is null");
			}
			else
			{
				Console.WriteLine("fo is not null");
			}
			
			//testing factories stuff...
			Atk.ObjectFactory factory = Atk.Global.DefaultRegistry.GetFactory(
			    UiaBridgeToAtk.AtkForMwfWindow.GType);
			Atk.Object aobj = factory.CreateAccessible(new UiaBridgeToAtk.AtkForMwfWindow());
			//aobj.
			
			//according to http://developer.gnome.org/projects/gap/guide/gad/gad-api-examples.html, 
			// we have to set the factory first
			Atk.Global.DefaultRegistry.SetFactoryType(UiaBridgeToAtk.GTypeTest.GType, UiaBridgeToAtk.MwfWindowFactory.GType);
			
			//test1, seems to do nothing
			//Atk.Global.Root.AddRelationship(Atk.RelationType.NodeChildOf, new UiaBridgeToAtk.MwfWindow());
			
			//test2, seems to do nothing
			//new UiaBridgeToAtk.MwfWindow().AddRelationship(Atk.RelationType.NodeChildOf, Atk.Global.Root);
			
			//test3, seems to do nothing
			//Atk.Global.Root.AddRelationship(Atk.RelationType.ParentWindowOf, new UiaBridgeToAtk.MwfWindow());
			
			//test4, seems to do nothing
			//new UiaBridgeToAtk.MwfWindow().AddRelationship(Atk.RelationType.ParentWindowOf, Atk.Global.Root);
			
			//test5: relation to be created twice (both sides), seems to do nothing
			//UiaBridgeToAtk.AtkForMwfWindow bridgeWindow = new UiaBridgeToAtk.AtkForMwfWindow();
			//Atk.Global.Root.AddRelationship(Atk.RelationType.ParentWindowOf, bridgeWindow);
			//bridgeWindow.AddRelationship(Atk.RelationType.NodeChildOf, Atk.Global.Root);
			
			//don't drop this call or otherwise accerciser won't see our app!:
			Gtk.Application.Run();
			//unreachable code from here
		}
		
		public static void Main(string[] args)
		{
			Initialize();
			Console.WriteLine("after initialize");
			
			System.Threading.Thread.Sleep(60000);
		}
	}
	

}

namespace UiaBridgeToAtk 
{
	class InProcessBridge
	{
	
		public InProcessBridge(object provider)
		{
			AtkSharpHelper.MainClass.Initialize();
			
			if (provider is IWindowProvider)
			{
				IWindowProvider winProvider = (IWindowProvider)provider;
			}
			Atk.Component component;
			AtkForMwfWindow f;
			//f.Role
		}
	}
	
	class MwfWindowFactory : Atk.ObjectFactory
	{
		public AtkForMwfWindow CreateAccessible(GTypeTest widget)
		{
			return widget.atkObject;
		}
	}

	class GTypeTest : GLib.Object
	{
		internal AtkForMwfWindow atkObject;
	}
	
	class AtkForMwfWindow : Atk.Object, Atk.Component
	{
		public Atk.Role Role
		{
			get { return Atk.Role.Window; }
		}
		
		public IntPtr Handle {
			get {
				throw new NotImplementedException();
			}
		}

		public Atk.Layer Layer {
			get {
				throw new NotImplementedException();
			}
		}

		public int MdiZorder {
			get {
				throw new NotImplementedException();
			}
		}

		IntPtr GLib.IWrapper.Handle {
			get {
				throw new NotImplementedException();
			}
		}

		Atk.Layer Atk.Component.Layer {
			get {
				throw new NotImplementedException();
			}
		}

		int Atk.Component.MdiZorder {
			get {
				throw new NotImplementedException();
			}
		}

		void Atk.Component.RemoveFocusHandler(uint handlerId)
		{
			throw new NotImplementedException();
		}

		public Atk.Object RefAccessibleAtPoint (int x, int y, Atk.CoordType coord_type)
		{
			throw new NotImplementedException();
		}

		public bool Contains (int x, int y, Atk.CoordType coord_type)
		{
			throw new NotImplementedException();
		}

		public void GetPosition (out int x, out int y, Atk.CoordType coord_type)
		{
			throw new NotImplementedException();
		}

		public void GetExtents (out int x, out int y, out int width, out int height, Atk.CoordType coord_type)
		{
			throw new NotImplementedException();
		}

		public uint AddFocusHandler (Atk.FocusHandler handler)
		{
			throw new NotImplementedException();
		}

		public bool SetSize (int width, int height)
		{
			throw new NotImplementedException();
		}

		public bool SetExtents (int x, int y, int width, int height, Atk.CoordType coord_type)
		{
			throw new NotImplementedException();
		}

		public void GetSize (int width, int height)
		{
			throw new NotImplementedException();
		}

		public bool GrabFocus ()
		{
			throw new NotImplementedException();
		}

		public bool SetPosition (int x, int y, Atk.CoordType coord_type)
		{
			throw new NotImplementedException();
		}

		public void RemoveFocusHandler (uint handler_id)
		{
			throw new NotImplementedException();
		}

		Atk.Object Atk.Component.RefAccessibleAtPoint (int x, int y, Atk.CoordType coord_type)
		{
			throw new NotImplementedException();
		}

		bool Atk.Component.Contains (int x, int y, Atk.CoordType coord_type)
		{
			throw new NotImplementedException();
		}

		uint Atk.Component.AddFocusHandler (Atk.FocusHandler handler)
		{
			throw new NotImplementedException();
		}

		bool Atk.Component.SetSize (int width, int height)
		{
			throw new NotImplementedException();
		}

		bool Atk.Component.SetExtents (int x, int y, int width, int height, Atk.CoordType coord_type)
		{
			throw new NotImplementedException();
		}

		void Atk.Component.GetSize (out int width, out int height)
		{
			throw new NotImplementedException();
		}

		bool Atk.Component.GrabFocus ()
		{
			throw new NotImplementedException();
		}

		bool Atk.Component.SetPosition (int x, int y, Atk.CoordType coord_type)
		{
			throw new NotImplementedException();
		}

		public event Atk.BoundsChangedHandler BoundsChanged;
	}
}