// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions: 
//  
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software. 
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// 
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	internal class Monitor
	{
		public Monitor()
		{
			GLib.GType.Init();
			
			Atk.Util.GetRootHandler = ReturnTopLevel;
			
			Atk.Util.GetToolkitNameHandler = GetAssemblyName;
			Atk.Util.GetToolkitVersionHandler = GetAssemblyVersionNumber;
			Atk.Util.AddGlobalEventListenerHandler = AddGlobalEventListener;
		}
		
		private bool isApplicationStarted = false;
		
		public void ApplicationStarts()
		{
			isApplicationStarted = true;
			Thread glibThread = new Thread (new ThreadStart (GLibMainLoopThread));
			glibThread.Start ();
			GLibHacks.Invoke (delegate (object sender, EventArgs args) {
				LaunchAtkBridge ();});
		}
		
		private void GLibMainLoopThread ()
		{
			new GLib.MainLoop().Run();
		}
		
		internal static string GetProgramName()
		{
			return System.IO.Path.GetFileNameWithoutExtension (Environment.GetCommandLineArgs () [0]);
		}
		
		private static Atk.Object ReturnTopLevel()
		{
			return TopLevelRootItem.Instance;
		}
		
		private static string GetAssemblyName()
		{
			return typeof(Monitor).Assembly.GetName().Name;
		}
		
		private static string GetAssemblyVersionNumber()
		{
			return typeof(Monitor).Assembly.GetName().Version.ToString();
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
				rc = KeepListener (listener, splitString [1], splitString [2]);
			}
			
			return rc;
		}
		
		private static uint KeepListener (
		    GLib.Signal.EmissionHook listener, 
		    string objType, string signalName)
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
		
		[DllImport("libatk-bridge")]
		static extern void gnome_accessibility_module_init ();
		
		private void LaunchAtkBridge()
		{
			gnome_accessibility_module_init();
		}
	}
	
	/// <summary>
	/// This code is ripped from gtk-sharp, and just provides a convenient
	/// way to invoke some code on the glib mainloop.
	/// </summary>
	internal static class GLibHacks
	{
		internal class InvokeCB {
			EventHandler d;
			object sender;
			EventArgs args;
			
			internal InvokeCB (EventHandler d)
			{
				this.d = d;
				args = EventArgs.Empty;
				sender = this;
			}
			
			internal InvokeCB (EventHandler d, object sender, EventArgs args)
			{
				this.d = d;
				this.args = args;
				this.sender = sender;
			}
			
			internal bool Invoke ()
			{
				d (sender, args);
				return false;
			}
		}
		
		public static void Invoke (EventHandler d)
		{
			InvokeCB icb = new InvokeCB (d);
			
			GLib.Timeout.Add (0, new GLib.TimeoutHandler (icb.Invoke));
		}

		public static void Invoke (object sender, EventArgs args, EventHandler d)
		{
			InvokeCB icb = new InvokeCB (d, sender, args);
			
			GLib.Timeout.Add (0, new GLib.TimeoutHandler (icb.Invoke));
		}
	}
}