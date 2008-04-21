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
		
		public void FormIsRemoved(IWindowProvider form)
		{
			if (isApplicationStarted)
			{
				Console.WriteLine ("FormIsRemoved");
				TopLevelRootItem.Instance.RemoveChild (associations[form]);
			}
			else
			{
				// TODO: an app can remove a form if it has not been started??
				throw new NotImplementedException();
			}
		}
		
		private Dictionary<IWindowProvider, Atk.Object> associations =
			new Dictionary<IWindowProvider,Atk.Object>();
		
		public void FormIsAdded(IWindowProvider form)
		{
			IRawElementProviderSimple simpleProvider = (IRawElementProviderSimple) form;
			string windowName = (string) simpleProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			
			Window newWindow = new Window(windowName);
			associations.Add(form, newWindow);
			TopLevelRootItem.Instance.AddOneChild(newWindow);
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
		
		private int AddGlobalEventListener(IntPtr listener, string event_type)
		{
			//dummy method for now
			return 0;
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