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
			
			//Atk.Role.
			Gtk.Application.Run();
			
		}
		
		public static void Main(string[] args)
		{
			Initialize();
			
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
			Atk.ObjectFactory factory = Atk.Global.DefaultRegistry.GetFactory(new GLib.GType());
			//factory.CreateAccessible(
			//factory.
		}
	}
	
	class MwfWindow : Atk.Object, Atk.Component
	{
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