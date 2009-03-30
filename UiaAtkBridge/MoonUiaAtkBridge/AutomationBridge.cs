
using System;

namespace MoonUiaAtkBridge
{

	public class AutomationBridge
	{
		static Atk.Object plugin_root;
		
		public AutomationBridge ()
		{

		}

		public static IntPtr Initialize ()
		{
			//TODO: generate *.dll.config files appropiately, here or on extension installation?
			Console.WriteLine ("AutomationBridge::Initialize");
			
			//detect here if we have glib & atk in our system
			try {
				GLib.GType.Init ();
			}
			//libgobject-2.0-0.dll, ...
			catch (DllNotFoundException) {
				Console.WriteLine ("ERROR: Native glib or atk libs not found.");
				return IntPtr.Zero;
			}

			plugin_root = new PluginRoot ();

			//TODO: hook here to events in System.Windows.dll UIA types
			
			
			return plugin_root.Handle;
		}
	}

	public class PluginRoot : Atk.Object
	{

	}
}
