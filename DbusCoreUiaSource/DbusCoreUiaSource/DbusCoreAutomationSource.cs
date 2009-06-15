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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Automation;

using NDesk.DBus;
using org.freedesktop.DBus;

using Mono.UIAutomation.Services;
using Mono.UIAutomation.Source;
using DC = Mono.UIAutomation.DbusCore;
using DCI = Mono.UIAutomation.DbusCore.Interfaces;

namespace Mono.UIAutomation.DbusCoreUiaSource
{
	public class DbusCoreAutomationSource : IAutomationSource
	{
		#region Constants

		private const string DBusName = "org.freedesktop.DBus";
		private const string DBusPath = "/org/freedesktop/DBus";

		#endregion

		#region Utility Classes

		private class DbusElementTuple
		{
			public string Bus;
			public string Path;

			public DbusElementTuple (string bus, string path)
			{
				if (bus == null)
					throw new ArgumentNullException ("bus");
				if (path == null)
					throw new ArgumentNullException ("path");
				Bus = bus;
				Path = path;
			}

			public override bool Equals (object obj)
			{
				DbusElementTuple other = obj as DbusElementTuple;
				return other != null && Bus == other.Bus && Path == other.Path;
			}

			public override int GetHashCode ()
			{
				return Bus.GetHashCode () ^ Path.GetHashCode ();
			}
		}

		#endregion

		#region Private Fields

		private Dictionary<DbusElementTuple, DbusCoreElement> elementMapping =
			new Dictionary<DbusElementTuple, DbusCoreElement> ();

		#endregion

		#region IAutomationSource Members

		public void Initialize ()
		{
			CheckMainLoop ();
		}

		public void Terminate ()
		{
			AbortMainLoop ();
		}

		public bool IsAccessibilityEnabled {
			get {
				return true;
			}
		}

		public IElement [] GetRootElements ()
		{
			List<DCI.IApplication> dbusApps =
				new List<DCI.IApplication> ();
			List<IElement> dbusElements = new List<IElement> ();

			List<Thread> threads = new List<Thread> ();
			List<string> candidateBusNames = new List<string> ();
			Object listLock  = new Object ();

			IBus ibus = Bus.Session.GetObject<IBus> (DBusName,
			                                         new ObjectPath (DBusPath));
			// Look for buses that contain DCI.IApplication
			foreach (string busName in ibus.ListNames ()) {
				// Reading introspection data hangs on some buses
				// for unknown reasons, so we spin each introspection
				// action off into its own thread, sleep for 1 second
				// to give the threads time to execute, and then
				// abort all of them.
				ParameterizedThreadStart start = (busNameObj) => {
					string currentBus = (string) busNameObj;
					// TODO: Likely crash source (ndesk-dbus bugs?)
					Introspectable intr =
						Bus.Session.GetObject<Introspectable> (currentBus,
						                                       new ObjectPath (DC.Constants.ApplicationPath));
					string data = string.Empty;
					try {
						data = intr.Introspect ();
					} catch {}
					if (data.Contains (DC.Constants.ApplicationInterfaceName))
						lock (listLock)
							candidateBusNames.Add (currentBus);
				};
				Thread thread = new Thread (start);
				thread.Start (busName);
				threads.Add (thread);
			}

			Thread.Sleep (1000);
			foreach (Thread thread in threads)
				thread.Abort ();

			foreach (string busName in candidateBusNames) {
				DCI.IApplication app =
					Bus.Session.GetObject<DCI.IApplication> (busName,
					                                         new ObjectPath (DC.Constants.ApplicationPath));
				if (app == null)
					continue;
				string [] rootElementPaths;
				try {
					rootElementPaths = app.GetRootElementPaths ();
				} catch {
					continue;
				}

				dbusApps.Add (app);
				foreach (string elementPath in rootElementPaths) {
					DCI.IAutomationElement dbusElement =
						Bus.Session.GetObject<DCI.IAutomationElement> (busName,
						                                               new ObjectPath (elementPath));
					if (dbusElement != null) {
						dbusElements.Add (CreateElement (dbusElement, busName, elementPath));
					}
				}
			}

			Log.Info ("DbusCoreAutomationSource: GetRootElements count will be: " + dbusElements.Count);
			return dbusElements.ToArray ();
		}

		#endregion

		#region Public Methods

		public DbusCoreElement GetOrCreateElement (string busName, string elementPath)
		{
			if (string.IsNullOrEmpty (elementPath) ||
			    string.IsNullOrEmpty (busName))
				return null;

			DbusCoreElement element;
			if (elementMapping.TryGetValue (new DbusElementTuple (busName, elementPath),
			                                out element))
				return element;

			DCI.IAutomationElement dbusElement =
				Bus.Session.GetObject<DCI.IAutomationElement> (busName,
				                                               new ObjectPath (elementPath));
			element = CreateElement (dbusElement, busName, elementPath);
			return element;
		}

		#endregion

		#region Private Methods

		private DbusCoreElement CreateElement (DCI.IAutomationElement dbusElement, string busName, string elementPath)
		{
			DbusCoreElement element = new DbusCoreElement (dbusElement, busName, elementPath, this);
			elementMapping.Add (new DbusElementTuple (busName, elementPath),
			                    element);
			return element;
		}

		#endregion

		#region Private Methods - DBus Main Loop

		private bool mainLoopStarted = false;
		private static bool runMainLoop = false;
		private Thread mainLoop = null;

		private void CheckMainLoop ()
		{
			if (mainLoopStarted)
				return;
			runMainLoop = true;

			Log.Info ("DbusCoreAutomationSource: Starting dbus main loop");
			mainLoop = new Thread (new ThreadStart (MainLoop));
			mainLoop.IsBackground = true;
			mainLoop.Start ();
			mainLoopStarted = true;
		}

		private void AbortMainLoop ()
		{
			runMainLoop = false;
			if (mainLoop != null) {
				Log.Info ("DbusCoreAutomationSource: Stopping dbus main loop");
				mainLoop.Abort ();
			}
			mainLoopStarted = false;
			mainLoop = null;
		}

		private void MainLoop ()
		{
			// TODO: Why does it not work if I uncomment this and
			//       do bus.Iterate (); ?
			//Bus bus = Bus.Session;
			while (runMainLoop)
				// TODO: Likely crash source (ndesk-dbus bugs?)
				Bus.Session.Iterate ();
		}

		#endregion
	}
}
