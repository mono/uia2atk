
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

			//FIXME: we're getting a TypeLoadException for now: (need to be on the white-list already?)
//System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation. ---> System.TypeLoadException: Could not load type 'GLib.Object' from assembly 'glib-sharp, Version=2.12.0.0, Culture=neutral, PublicKeyToken=35e10195dab3c99f'.
//  at Atk.Object..ctor () [0x00000] 
//  at MoonUiaAtkBridge.PluginRoot..ctor () [0x00000] 
//  at MoonUiaAtkBridge.AutomationBridge.Initialize () [0x00000] 
//  at (wrapper managed-to-native) System.Reflection.MonoMethod:InternalInvoke (object,object[],System.Exception&)
//  at System.Reflection.MonoMethod.Invoke (System.Object obj, BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture) [0x00000] 
//  --- End of inner exception stack trace ---
//  at System.Reflection.MonoMethod.Invoke (System.Object obj, BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture) [0x00000] 
//  at System.MonoType.InvokeMember (System.String name, BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object target, System.Object[] args, System.Reflection.ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, System.String[] namedParameters) [0x00000] 
//  at System.Type.InvokeMember (System.String name, BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object target, System.Object[] args) [0x00000] 
			plugin_root = new PluginRoot ();

			//TODO: hook here to events in System.Windows.dll UIA types
			
			
			return plugin_root.Handle;
		}
	}

	public class PluginRoot : Atk.Object
	{
	}
}
