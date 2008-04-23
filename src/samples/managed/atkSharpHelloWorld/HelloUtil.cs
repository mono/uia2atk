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
		
		private HelloUtil()
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
		
		internal static int AddGlobalEventListener(IntPtr listener, string eventType)
		{
			uint rc = 0;
			
			//split_string[0]: toolkit
			//            [1]: class/interface
			//            [2]: event type
			// example: Gtk:AtkObject:children-changed
			string[] splitString = eventType.Split (':');
			
			if ( (splitString != null) &&
			     (splitString.Length > 2) &&
			     (String.IsNullOrEmpty (splitString [1])) &&
			     (String.IsNullOrEmpty (splitString [2])))
			{
				rc = KeepListener (listener, splitString [1], splitString [2], eventType);
			}
			
			return (int)rc;
		}
		
		private static uint KeepListener (IntPtr listener, string objType, string signalName, string hookData)
		{
			//GLib.GType type = GType.GetFromName (objType);
			//if (type != null)
			{
				GLib.Signal signal; //= GLib.Signal.Lookup (signalName, type);
				
				//if (signal != null)
				{
					lock (listenerListMutex)
					{
						ListenerInfo info = new ListenerInfo();
						info.Id = (uint) ListenerList.Count + 1;
						//info.SignalId = signal.Id;
						//info.HookId = signal.AddEmissionHook (0, listener, hookData);
						ListenerList.Add (info.Id, info);
						return info.Id;
					}
					
				}
				//else
				{
					throw new NotSupportedException ("Invalid signal type " + signalName);
				}
			}
			//else
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
			internal uint SignalId;
			internal ulong HookId;
		}
	}
}
