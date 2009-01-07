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
		private bool useNativeInitialization = true;
		private GLib.MainLoop mainLoop;
		
		private void RegisterSignal (System.Type type, string name)
		{
			IntPtr native_name = GLib.Marshaller.StringToPtrGStrdup (name);
			g_signal_new (native_name, ((GLib.GType)type).Val, 2, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, GLib.GType.None.Val, 0);
			GLib.Marshaller.Free (native_name);
		}

		private void RegisterWindowSignals ()
		{
			// These really should be registered on Window;
			// registering here instead because it's a well-known
			// type in the unmanaged world, so glue.c can find it
			RegisterSignal (typeof (Atk.Object), "activate");
			RegisterSignal (typeof (Atk.Object), "create");
			RegisterSignal (typeof (Atk.Object), "deactivate");
			RegisterSignal (typeof (Atk.Object), "destroy");
			RegisterSignal (typeof (Atk.Object), "maximize");
			RegisterSignal (typeof (Atk.Object), "minimize");
			RegisterSignal (typeof (Atk.Object), "move");
			RegisterSignal (typeof (Atk.Object), "resize");
			RegisterSignal (typeof (Atk.Object), "restore");
		}

		internal Monitor ()
		{
			GLib.GType.Init();
			
			PreventGailInitialization ();
			
			Atk.Util.GetRootHandler = ReturnTopLevel;
			
			Atk.Util.GetToolkitNameHandler = GetAssemblyName;
			Atk.Util.GetToolkitVersionHandler = GetAssemblyVersionNumber;
			
			if (!useNativeInitialization)
				Atk.Util.AddGlobalEventListenerHandler = AddGlobalEventListener;
			else
				override_global_event_listener ();
			Atk.Util.AddKeyEventListenerHandler = AddKeyEventListener;
		}

		const string ATK_BRIDGE_ENVVAR_NAME = "NO_AT_BRIDGE";
		const string GAIL_ENVVAR_NAME = "NO_GAIL";
		
		// we need to set this because MWF happens to depend on gtk+ (this may change on the future)
		void PreventGailInitialization ()
		{
			// Solution for gtk+ >= 2.14
			// see https://bugzilla.novell.com/show_bug.cgi?id=457787
			Environment.SetEnvironmentVariable (ATK_BRIDGE_ENVVAR_NAME, "1");
			Environment.SetEnvironmentVariable (GAIL_ENVVAR_NAME, "1");
		}
		
		private bool isApplicationStarted = false;
		
		public void ApplicationStarts ()
		{
			isApplicationStarted = true;
			Thread glibThread = new Thread (new ThreadStart (GLibMainLoopThread));
			glibThread.IsBackground = true;
			glibThread.Start ();
			AutoResetEvent sync = GLibHacks.Invoke (delegate (object sender, EventArgs args) {
				RegisterWindowSignals ();

				Environment.SetEnvironmentVariable (ATK_BRIDGE_ENVVAR_NAME, "0");
				LaunchAtkBridge ();
			});
			sync.WaitOne ();
			sync.Close ();
			sync = null;
		}
		
		private void GLibMainLoopThread ()
		{
			mainLoop = new GLib.MainLoop ();
			mainLoop.Run ();
		}
		
		public void Quit()
		{
			AutoResetEvent sync = GLibHacks.Invoke (delegate (object sender, EventArgs args) {
				ShutdownAtkBridge ();
				Atk.Util.GetRootHandler = null;
				mainLoop.Quit ();
			});
			sync.WaitOne ();
			sync.Close ();
			sync = null;
		}

		internal static string GetProgramName ()
		{
			return System.IO.Path.GetFileNameWithoutExtension (Environment.GetCommandLineArgs () [0]);
		}
		
		private static Atk.Object ReturnTopLevel ()
		{
			return TopLevelRootItem.Instance;
		}
		
		private static string GetAssemblyName()
		{
			return typeof (Monitor).Assembly.GetName ().Name;
		}
		
		private static string GetAssemblyVersionNumber ()
		{
			return typeof (Monitor).Assembly.GetName ().Version.ToString ();
		}
		
		internal static uint AddGlobalEventListener (
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
		
		internal static uint AddKeyEventListener (
		    Atk.KeySnoopFunc listener)
		{
			KeyListenerInfo info = new KeyListenerInfo ();
			lock (listenerListSync)
			{
				info.Id = 0;
				while (KeyListenerList.ContainsKey (info.Id))
					info.Id++;
				info.listener = listener;
				KeyListenerList.Add (info.Id, info);
			}
			return info.Id;
		}
		
		public bool HandleKey (Atk.KeyEventStruct evnt)
		{
			if (KeyListenerList.Count == 0)
				return false;
			foreach (KeyValuePair<uint, KeyListenerInfo> kvp in KeyListenerList) {
				int result = kvp.Value.listener (evnt);
				if (result != 0)
					return true;
			}
			return false;
		}

		private static uint KeepListener (
		    GLib.Signal.EmissionHook listener, 
		    string objType, string signalName)
		{
			GLib.GType type = GLib.GType.FromName (objType);
			if (type != GLib.GType.Invalid) {
				
				//FIXME: drop this workaround for bug#386950
				if (signalName.Contains ("property")) return 0;
				
				lock (listenerListSync)
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
		
		private static object listenerListSync = new object ();
		
		private static Dictionary <uint, ListenerInfo> ListenerList
			= new Dictionary<uint, ListenerInfo> ();
		
		private static Dictionary <uint, KeyListenerInfo> KeyListenerList
			= new Dictionary<uint, KeyListenerInfo> ();
		
		struct ListenerInfo
		{
			internal uint Id;
			internal string SignalName;
			internal GLib.GType Type;
			internal ulong HookId;
		}
		
		struct KeyListenerInfo
		{
			internal uint Id;
			internal Atk.KeySnoopFunc listener;
		}
		
		[DllImport("libatk-bridge")]
		static extern void gnome_accessibility_module_init ();

		[DllImport("libatk-bridge")]
		static extern void gnome_accessibility_module_shutdown ();
		
		private void LaunchAtkBridge ()
		{
			gnome_accessibility_module_init ();
		}

		private void ShutdownAtkBridge ()
		{
			gnome_accessibility_module_shutdown ();
		}
		
		[DllImport("libbridge-glue.so")]
		static extern void override_global_event_listener ();
		[DllImport("libgobject-2.0-0.dll")]
		static extern void g_signal_new (IntPtr signal_name, IntPtr itype, int signal_flags, uint class_offset, IntPtr accumulator, IntPtr accu_data, IntPtr c_marshaller, IntPtr return_type, uint n_params);
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
			internal System.Threading.AutoResetEvent sync = new AutoResetEvent (false);
			
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
				sync.Set ();
				return false;
			}
		}
		
		public static System.Threading.AutoResetEvent Invoke (EventHandler d)
		{
			InvokeCB icb = new InvokeCB (d);
			
			GLib.Timeout.Add (0, new GLib.TimeoutHandler (icb.Invoke));
			
			return icb.sync;
		}

		public static System.Threading.AutoResetEvent Invoke (object sender, EventArgs args, EventHandler d)
		{
			InvokeCB icb = new InvokeCB (d, sender, args);
			
			GLib.Timeout.Add (0, new GLib.TimeoutHandler (icb.Invoke));
			
			return icb.sync;
		}
	}
}
