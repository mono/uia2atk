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
using Mono.UIAutomation.Services;
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
		
		private Monitor ()
		{
			GLib.GType.Init ();

			PreventGailInitialization ();

			RegisterWindowSignals ();

			Atk.Util.GetRootHandler = ReturnTopLevel;
			
			Atk.Util.GetToolkitNameHandler = GetAssemblyName;
			Atk.Util.GetToolkitVersionHandler = GetAssemblyVersionNumber;
			
			if (!useNativeInitialization)
				Atk.Util.AddGlobalEventListenerHandler = AddGlobalEventListener;
			else
				override_global_event_listener ();
			Atk.Util.AddKeyEventListenerHandler = AddKeyEventListener;
		}

		private static volatile Monitor instance = null;

		private static object syncRoot = new object ();
		
		public static Monitor Instance {
			get {
				if (instance == null)
					lock (syncRoot)
						if (instance == null)
							instance = new Monitor ();
				return instance;
			}
		}

		string gtk_modules_envvar_content = null;
		const string GTK_MODULES_ENVVAR_NAME = "GTK_MODULES";
		const string ATK_BRIDGE_ENVVAR_NAME = "NO_AT_BRIDGE";
		const string GAIL_ENVVAR_NAME = "NO_GAIL";
		
		// we need to set this because MWF happens to depend on gtk+ (this may change on the future)
		void PreventGailInitialization ()
		{
			// Solution for gtk+ < 2.14:
			// (A better solution would probably be to get these
			//  values out-of-process.  See details/discussion here:
			//  https://bugzilla.novell.com/show_bug.cgi?id=375987 )
			gtk_modules_envvar_content = Environment.GetEnvironmentVariable (GTK_MODULES_ENVVAR_NAME);
			Environment.SetEnvironmentVariable (GTK_MODULES_ENVVAR_NAME, String.Empty);

			// Solution for gtk+ >= 2.14
			// see https://bugzilla.novell.com/show_bug.cgi?id=457787
			Environment.SetEnvironmentVariable (ATK_BRIDGE_ENVVAR_NAME, "1");
			Environment.SetEnvironmentVariable (GAIL_ENVVAR_NAME, "1");
		}
		
		public void ApplicationStarts ()
		{
			if (mainLoop != null && mainLoop.IsRunning) {
				Log.Warn ("AutomationBridge: Received init event, but already running;  ignoring.");
				return;
			}

			CheckMainLoop ();
			AutoResetEvent sync = GLibHacks.Invoke (delegate (object sender, EventArgs args) {
				Environment.SetEnvironmentVariable (ATK_BRIDGE_ENVVAR_NAME, null);
				LaunchAtkBridge ();
				Environment.SetEnvironmentVariable (GAIL_ENVVAR_NAME, null);
			});
			sync.WaitOne ();
			sync.Close ();
			sync = null;

			Environment.SetEnvironmentVariable (GTK_MODULES_ENVVAR_NAME, gtk_modules_envvar_content);
		}
		
		public void CheckMainLoop ()
		{
			if (mainLoop != null && mainLoop.IsRunning)
				return;

			Thread glibThread = new Thread (new ThreadStart (GLibMainLoopThread));
			glibThread.IsBackground = true;
			glibThread.Start ();
		}

		private void GLibMainLoopThread ()
		{
			mainLoop = new GLib.MainLoop ();
			mainLoop.Run ();
		}
		
		internal void Dispose ()
		{
			//FIXME: find a better way to see if we have been already disposed
			if (!mainLoop.IsRunning) {
#if DEBUG
				//we prefer to crash, in order to fix ASAP the buggy clown that did this
				throw new ObjectDisposedException ("You shouldn't call dispose more than once");
#else
				return;// probably already disposed
#endif
			}
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
			
			Log.Info ("AutomationBridge: Add global event listener, eventType = {0}", eventType);
			
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
		
		[DllImport("libbridge-glue.so")]
		static extern void gnome_accessibility_module_init ();

		[DllImport("libbridge-glue.so")]
		static extern void gnome_accessibility_module_shutdown ();
		
		private void LaunchAtkBridge ()
		{
			gnome_accessibility_module_init ();
		}

		private static void ShutdownAtkBridge ()
		{
			Environment.SetEnvironmentVariable ("AT_BRIDGE_SHUTDOWN", "1");
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
