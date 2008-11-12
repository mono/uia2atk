// HelloUtil.cs created with MonoDevelop
// User: knocte at 9:33 PM 3/30/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

namespace atkSharpHelloWorld
{
	
	public class HelloUtil 
		// Atk# doesn't work this way yet:
		//: Atk.Util
	{
		
		private HelloUtil ()
		{
		}
		
		internal static Atk.Object GetRoot()
		{
			return HelloTopLevel.Instance;
		}
		
		internal static string GetToolkitName()
		{
			return "MANAGED-HELLO";
		}
		
		internal static string GetToolkitVersion()
		{
			return "1.1";
		}
		
		internal static uint AddGlobalEventListener(
		    GLib.Signal.EmissionHook listener, 
		    string eventType)
		{
			uint rc = 0;
			
			Console.WriteLine ("add global event listener, event_type: " + eventType);
			
			//split_string[0]: toolkit
			//            [1]: class/interface
			//            [2]: event type
			// example: Gtk:AtkObject:children-changed
			string[] splitString = eventType.Split (':');
			if ( (splitString != null) &&
			     (splitString.Length > 2) &&
			     (!String.IsNullOrEmpty (splitString [1])) &&
			     (!String.IsNullOrEmpty (splitString [2])))
			{
				rc = KeepListener (listener, splitString [1], splitString [2], eventType);
			}
			
			return rc;
		}
		
		private static uint KeepListener (
		    GLib.Signal.EmissionHook listener, 
		    string objType, string signalName, string hookData)
		{
			GLib.GType type = GLib.GType.FromName (objType);
			if (type != GLib.GType.Invalid) {
				lock (listenerListMutex)
				{
					ListenerInfo info = new ListenerInfo();
					info.Id = (uint) ListenerList.Count + 1;
					info.SignalName = signalName;
					info.Type = type;
					info.HookId = GLib.Signal.AddEmissionHook (signalName, type, listener);
					ListenerList.Add (info.Id, info);
					return info.Id;
				}
			}
			else
			{
				throw new NotSupportedException ("Invalid object type " + objType);
			}
		}
		
		private static object listenerListMutex = new object ();
		
		private static Dictionary <uint, ListenerInfo> ListenerList
			= new Dictionary<uint, ListenerInfo> ();
		
		struct ListenerInfo
		{
			internal uint Id;
			internal string SignalName;
			internal GLib.GType Type;
			internal ulong HookId;
		}
	}
}
