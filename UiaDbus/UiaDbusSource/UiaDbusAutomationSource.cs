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
using DC = Mono.UIAutomation.UiaDbus;
using DCI = Mono.UIAutomation.UiaDbus.Interfaces;

namespace Mono.UIAutomation.UiaDbusSource
{
	public class UiaDbusAutomationSource : IAutomationSource
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

		private Dictionary<DbusElementTuple, UiaDbusElement> elementMapping =
			new Dictionary<DbusElementTuple, UiaDbusElement> ();

		private EventHandlerManager eventHandlerManager = new EventHandlerManager();
		private List<string> automationEventBusNames = new List<string> ();
		private List<string> propertyEventBusNames = new List<string> ();
		private List<string> structureEventBusNames = new List<string> ();

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
			List<IElement> dbusElements = new List<IElement> ();

			foreach (string busName in GetUiaDbusNames ()) {
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

				foreach (string elementPath in rootElementPaths) {
					var rootElement = GetOrCreateElement (busName, elementPath);
					if (rootElement != null)
						dbusElements.Add (rootElement);
				}
			}

			Log.Info ("UiaDbusAutomationSource: GetRootElements count will be: " + dbusElements.Count);
			return dbusElements.ToArray ();
		}

		public void AddAutomationEventHandler (AutomationEvent eventId,
		                                       IElement element,
		                                       TreeScope scope,
		                                       AutomationEventHandler eventHandler)
		{
			UiaDbusElement uiaDbusElement = element as UiaDbusElement;
			if (uiaDbusElement == null)
				//throw new Exception ("All elements handled by UiaDbusAutomationSource shall be UiaDbusElement.");
				return;

			string busName = uiaDbusElement.BusName;
			DCI.IApplication app = Bus.Session.GetObject<DCI.IApplication> (busName,
				new ObjectPath (DC.Constants.ApplicationPath));
			int [] runtimeId = uiaDbusElement.RuntimeId;
			int handlerId = eventHandlerManager.RegisterAutomationEventHandler (eventHandler);

			if (!automationEventBusNames.Contains (busName)) {
				automationEventBusNames.Add (busName);
				app.AutomationEvent += delegate(int hId, int evtId, string providerPath) {
					var handler = eventHandlerManager.GetAutomationEventHandlerById (hId);
					if (handler != null) {
						UiaDbusElement elem = GetOrCreateElement (busName, providerPath);
						AutomationElement ae = SourceManager.GetOrCreateAutomationElement (elem);
						var args = new AutomationEventArgs (AutomationEvent.LookupById (evtId));
						handler (ae, args);
					}
				};
			}
			app.AddAutomationEventHandler (eventId.Id, runtimeId, scope, handlerId);
		}

		public void AddAutomationPropertyChangedEventHandler (IElement element,
		                                                      TreeScope scope,
		                                                      AutomationPropertyChangedEventHandler eventHandler,
		                                                      AutomationProperty [] properties)
		{
			UiaDbusElement uiaDbusElement = element as UiaDbusElement;
			if (uiaDbusElement == null)
				return;

			string busName = uiaDbusElement.BusName;
			DCI.IApplication app = Bus.Session.GetObject<DCI.IApplication> (busName,
				new ObjectPath (DC.Constants.ApplicationPath));
			int [] runtimeId = uiaDbusElement.RuntimeId;
			int handlerId = eventHandlerManager.RegisterPropertyEventHandler (eventHandler);

			if (!propertyEventBusNames.Contains (busName)) {
				propertyEventBusNames.Add (busName);
				app.AutomationPropertyChanged += delegate (int hId, int evtId, string providerPath,
				                                          int propertyId, object oldValue,
				                                          object newValue) {
					var handler = eventHandlerManager.GetPropertyEventHandlerById (hId);
					if (handler != null) {
						UiaDbusElement elem = GetOrCreateElement (busName, providerPath);
						AutomationElement ae = SourceManager.GetOrCreateAutomationElement (elem);
						var args =
						         new AutomationPropertyChangedEventArgs (AutomationProperty.LookupById (propertyId),
						                                                 oldValue, newValue);
						handler (ae, args);
					}
				};
			}
			int [] propertyIds = new int [properties.Length];
			for (int  i = 0; i < properties.Length; i++)
				propertyIds [i] = properties [i].Id;
			app.AddAutomationPropertyChangedEventHandler (runtimeId, scope, handlerId, propertyIds);
		}

		public void AddStructureChangedEventHandler (IElement element,
		                                             TreeScope scope,
		                                             StructureChangedEventHandler eventHandler)
		{
			UiaDbusElement uiaDbusElement = element as UiaDbusElement;
			if (uiaDbusElement == null)
				return;

			string busName = uiaDbusElement.BusName;
			DCI.IApplication app = Bus.Session.GetObject<DCI.IApplication> (busName,
				new ObjectPath (DC.Constants.ApplicationPath));
			int [] runtimeId = uiaDbusElement.RuntimeId;
			int handlerId = eventHandlerManager.RegisterStructureEventHandler (eventHandler);

			if (!structureEventBusNames.Contains (busName)) {
				structureEventBusNames.Add (busName);
				app.StructureChanged += delegate (int hId, int evtId, string providerPath,
				                                 StructureChangeType changeType) {
					var handler = eventHandlerManager.GetStructureEventHandlerById (hId);
					if (handler != null) {
						UiaDbusElement elem = GetOrCreateElement (busName, providerPath);
						AutomationElement ae = SourceManager.GetOrCreateAutomationElement (elem);
						//Todo  Shall we make the 2nd arg of StructureChangedEventArgs.ctor be
						// the same as "elem", i.e. the sender object
						var args = new StructureChangedEventArgs (changeType, elem.RuntimeId);
						handler (ae, args);
					}
				};
			}
			app.AddStructureChangedEventHandler (runtimeId, scope, handlerId);
		}

		public void RemoveAutomationEventHandler (AutomationEvent eventId,
		                                          IElement element,
		                                          AutomationEventHandler eventHandler)
		{
			UiaDbusElement uiaDbusElement = element as UiaDbusElement;
			if (uiaDbusElement == null)
				return;

			int handlerId = eventHandlerManager.GetAutomationEventIdByHandler (eventHandler);
			if (handlerId == -1)
				return;

			string busName = uiaDbusElement.BusName;
			DCI.IApplication app = Bus.Session.GetObject<DCI.IApplication> (busName,
				new ObjectPath (DC.Constants.ApplicationPath));
			int [] runtimeId = uiaDbusElement.RuntimeId;
			app.RemoveAutomationEventHandler (eventId.Id, runtimeId, handlerId);
		}

		public void RemoveAutomationPropertyChangedEventHandler (IElement element,
		                                                         AutomationPropertyChangedEventHandler eventHandler)
		{
			UiaDbusElement uiaDbusElement = element as UiaDbusElement;
			if (uiaDbusElement == null)
				return;

			int handlerId = eventHandlerManager.GetPropertyEventIdByHandler (eventHandler);
			if (handlerId == -1)
				return;

			string busName = uiaDbusElement.BusName;
			DCI.IApplication app = Bus.Session.GetObject<DCI.IApplication> (busName,
				new ObjectPath (DC.Constants.ApplicationPath));
			int [] runtimeId = uiaDbusElement.RuntimeId;
			app.RemoveAutomationPropertyChangedEventHandler (runtimeId, handlerId);
		}

		public void RemoveStructureChangedEventHandler (IElement element,
		                                                StructureChangedEventHandler eventHandler)
		{
			UiaDbusElement uiaDbusElement = element as UiaDbusElement;
			if (uiaDbusElement == null)
				return;

			int handlerId = eventHandlerManager.GetStructureEventIdByHandler (eventHandler);
			if (handlerId == -1)
				return;

			string busName = uiaDbusElement.BusName;
			DCI.IApplication app = Bus.Session.GetObject<DCI.IApplication> (busName,
				new ObjectPath (DC.Constants.ApplicationPath));
			int [] runtimeId = uiaDbusElement.RuntimeId;
			app.RemoveStructureChangedEventHandler (runtimeId, handlerId);
		}

		public void RemoveAllEventHandlers ()
		{
			foreach (DCI.IApplication app in GetUiaApplications ())
				app.RemoveAllEventHandlers (EventHandlerManager.ClientPrefix);
		}

		#endregion

		#region Public Methods

		public UiaDbusElement GetOrCreateElement (string busName, string elementPath)
		{
			if (string.IsNullOrEmpty (elementPath) ||
			    string.IsNullOrEmpty (busName))
				return null;

			UiaDbusElement element;
			lock (elementMapping) {
				if (elementMapping.TryGetValue (new DbusElementTuple (busName, elementPath),
				                                out element))
					return element;

				DCI.IAutomationElement dbusElement =
					Bus.Session.GetObject<DCI.IAutomationElement> (busName,
					                                               new ObjectPath (elementPath));
				element = CreateElement (dbusElement, busName, elementPath);
			}
			return element;
		}

		internal UiaDbusElement [] GetOrCreateElements (string busName, string [] paths)
		{
			var elements = new UiaDbusElement [paths.Length];
			for (int i = 0; i < paths.Length; i++) {
				elements [i] = this.GetOrCreateElement (busName, paths [i]);
			}
			return elements;
		}

		#endregion

		#region Private Methods

		private IList<string> GetUiaDbusNames ()
		{
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

			return candidateBusNames;
		}

		private DCI.IApplication [] GetUiaApplications ()
		{
			List<DCI.IApplication> dbusApps =
				new List<DCI.IApplication> ();

			foreach (string busName in GetUiaDbusNames ()) {
				DCI.IApplication app =
					Bus.Session.GetObject<DCI.IApplication> (busName,
					                                         new ObjectPath (DC.Constants.ApplicationPath));
				if (app == null)
					continue;
				dbusApps.Add (app);
			}

			return dbusApps.ToArray ();
		}

		private UiaDbusElement CreateElement (DCI.IAutomationElement dbusElement, string busName, string elementPath)
		{
			if (dbusElement == null)
				return null;
			UiaDbusElement element = new UiaDbusElement (dbusElement, busName, elementPath, this);
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

			Log.Info ("UiaDbusAutomationSource: Starting dbus main loop");
			mainLoop = new Thread (new ThreadStart (MainLoop));
			mainLoop.IsBackground = true;
			mainLoop.Start ();
			mainLoopStarted = true;
		}

		private void AbortMainLoop ()
		{
			runMainLoop = false;
			if (mainLoop != null) {
				Log.Info ("UiaDbusAutomationSource: Stopping dbus main loop");
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
