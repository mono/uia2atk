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

using DBus;
using org.freedesktop.DBus;

using Mono.UIAutomation.Services;
using Mono.UIAutomation.Source;
using Mono.UIAutomation.UiaDbusBridge;
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

		private EventHandlerManager eventHandlerManager = new EventHandlerManager ();
		private List<string> automationEventBusNames = new List<string> ();
		private List<string> propertyEventBusNames = new List<string> ();
		private List<string> structureEventBusNames = new List<string> ();
		private List<FocusChangedEventHandler> focusChangedHandlers = new List<FocusChangedEventHandler> ();
		private bool listenFocusChangeStarted = false;
		private List<string> uiaDbusNames = null;
		private Object uiaDbusNamesLock = new Object ();

		#endregion

		#region IAutomationSource Members

		public void Initialize ()
		{
			CheckMainLoop ();
			foreach (string busName in GetUiaDbusNames ()) {
				BindApplicationEventHandlers (busName);
			}
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

			Log.Debug ("UiaDbusAutomationSource: GetRootElements count will be: " + dbusElements.Count);
			return dbusElements.ToArray ();
		}

		public event EventHandler RootElementsChanged;

		public IElement GetElementFromHandle (IntPtr handle)
		{
			foreach (var appPair in GetUiaApplications ()) {
				string path = appPair.Value.GetElementPathFromHandle (handle.ToInt32 ());
				if (!string.IsNullOrEmpty (path))
					return GetOrCreateElement (appPair.Key, path);
			}
			return null;
		}

		public IElement GetFocusedElement ()
		{
			foreach (string busName in GetUiaDbusNames ()) {
				DCI.IApplication app =
					Bus.Session.GetObject<DCI.IApplication> (busName,
					                                         new ObjectPath (DC.Constants.ApplicationPath));
				if (app == null)
					continue;
				string elementPath = app.GetFocusedElementPath ();
				if (!string.IsNullOrEmpty (elementPath))
					return GetOrCreateElement (busName, elementPath);
			}
			return null;
		}

		public void AddAutomationEventHandler (AutomationEvent eventId,
		                                       IElement element,
		                                       TreeScope scope,
		                                       AutomationEventHandler eventHandler)
		{
			if (element == null) {
				//the element is the RootElement
				// TODO clean up registered handlers when they're removed
				int handlerId = eventHandlerManager.RegisterAutomationEventHandler (eventHandler);
				RootElementEventsManager.AddAutomationEventRequest (eventId.Id, scope, handlerId);
				foreach (var entry in GetUiaApplications ()) {
					string busName = entry.Key;
					var app = entry.Value;
					EnsureAutomationEventsSetUp (app, busName);
					app.AddRootElementAutomationEventHandler (eventId.Id, scope, handlerId);
				}
			} else {
				UiaDbusElement uiaDbusElement = element as UiaDbusElement;
				if (uiaDbusElement == null) {
					Log.Error ("[AddAutomationEventHandler] The element sent to UiaDbusSource is not UiaDbusElement");
					return;
				}
				string busName = uiaDbusElement.BusName;
				DCI.IApplication app = Bus.Session.GetObject<DCI.IApplication> (busName,
					new ObjectPath (DC.Constants.ApplicationPath));
				int [] runtimeId = uiaDbusElement.RuntimeId;
				int handlerId = eventHandlerManager.RegisterAutomationEventHandler (eventHandler);

				EnsureAutomationEventsSetUp (app, busName);
				app.AddAutomationEventHandler (eventId.Id, runtimeId, scope, handlerId);
			}
		}

		public void AddAutomationPropertyChangedEventHandler (IElement element,
		                                                      TreeScope scope,
		                                                      AutomationPropertyChangedEventHandler eventHandler,
		                                                      AutomationProperty [] properties)
		{
			int [] propertyIds = new int [properties.Length];
			for (int  i = 0; i < properties.Length; i++)
				propertyIds [i] = properties [i].Id;
			if (element == null) {
				//the element is the RootElement
				// TODO clean up registered handlers when they're removed
				int handlerId = eventHandlerManager.RegisterPropertyEventHandler (eventHandler);
				RootElementEventsManager.AddPropertyEventRequest (scope, handlerId, propertyIds);
				foreach (var entry in GetUiaApplications ()) {
					string busName = entry.Key;
					var app = entry.Value;
					EnsurePropertyEventsSetUp (app, busName);
					app.AddRootElementAutomationPropertyChangedEventHandler (
						scope, handlerId, propertyIds);
				}
			} else {
				UiaDbusElement uiaDbusElement = element as UiaDbusElement;
				if (uiaDbusElement == null) {
					Log.Error ("[AddAutomationPropertyChangedEventHandler] The element sent to UiaDbusSource is not UiaDbusElement");
					return;
				}
				string busName = uiaDbusElement.BusName;
				DCI.IApplication app = Bus.Session.GetObject<DCI.IApplication> (busName,
					new ObjectPath (DC.Constants.ApplicationPath));
				int [] runtimeId = uiaDbusElement.RuntimeId;
				int handlerId = eventHandlerManager.RegisterPropertyEventHandler (eventHandler);

				EnsurePropertyEventsSetUp (app, busName);
				app.AddAutomationPropertyChangedEventHandler (runtimeId, scope, handlerId, propertyIds);
			}
		}

		public void AddStructureChangedEventHandler (IElement element,
		                                             TreeScope scope,
		                                             StructureChangedEventHandler eventHandler)
		{
			if (element == null) {
				//the element is the RootElement
				// TODO clean up registered handlers when they're removed
				int handlerId = eventHandlerManager.RegisterStructureEventHandler (eventHandler);
				RootElementEventsManager.AddStructureEventRequest (scope, handlerId);
				foreach (var entry in GetUiaApplications ()) {
					string busName = entry.Key;
					var app = entry.Value;
					EnsureStructureEventsSetUp (app, busName);
					app.AddRootElementStructureChangedEventHandler (
						scope, handlerId);
				}
			} else {
				UiaDbusElement uiaDbusElement = element as UiaDbusElement;
				if (uiaDbusElement == null) {
					Log.Error ("[AddStructureChangedEventHandler] The element sent to UiaDbusSource is not UiaDbusElement");
					return;
				}
				string busName = uiaDbusElement.BusName;
				DCI.IApplication app = Bus.Session.GetObject<DCI.IApplication> (busName,
					new ObjectPath (DC.Constants.ApplicationPath));
				int [] runtimeId = uiaDbusElement.RuntimeId;
				int handlerId = eventHandlerManager.RegisterStructureEventHandler (eventHandler);
				EnsureStructureEventsSetUp (app, busName);
				app.AddStructureChangedEventHandler (runtimeId, scope, handlerId);
			}
		}

		public void AddAutomationFocusChangedEventHandler (FocusChangedEventHandler eventHandler)
		{
			if (!listenFocusChangeStarted)
				StartListenFocusChangedEvents ();
			lock (focusChangedHandlers)
				focusChangedHandlers.Add (eventHandler);
		}

		public void RemoveAutomationEventHandler (AutomationEvent eventId,
		                                          IElement element,
		                                          AutomationEventHandler eventHandler)
		{
			int handlerId = eventHandlerManager.GetAutomationEventIdByHandler (eventHandler);
			if (handlerId == -1)
				return;
			if (element == null) {
				//the element is the RootElement
				RootElementEventsManager.RemoveAutomationEventRequest (eventId.Id, handlerId);
				foreach (var entry in GetUiaApplications ())
					entry.Value.RemoveRootElementAutomationEventHandler (
						eventId.Id, handlerId);
			} else {
				UiaDbusElement uiaDbusElement = element as UiaDbusElement;
				if (uiaDbusElement == null) {
					Log.Error ("[RemoveAutomationEventHandler] The element sent to UiaDbusSource is not UiaDbusElement");
					return;
				}
				string busName = uiaDbusElement.BusName;
				DCI.IApplication app = Bus.Session.GetObject<DCI.IApplication> (busName,
					new ObjectPath (DC.Constants.ApplicationPath));
				int [] runtimeId = uiaDbusElement.RuntimeId;
				app.RemoveAutomationEventHandler (eventId.Id, runtimeId, handlerId);
			}
		}

		public void RemoveAutomationPropertyChangedEventHandler (IElement element,
		                                                         AutomationPropertyChangedEventHandler eventHandler)
		{
			int handlerId = eventHandlerManager.GetPropertyEventIdByHandler (eventHandler);
			if (handlerId == -1)
				return;

			if (element == null) {
				//the element is the RootElement
				RootElementEventsManager.RemovePropertyEventRequest (handlerId);
				foreach (var entry in GetUiaApplications ())
					entry.Value.RemoveRootElementAutomationPropertyChangedEventHandler (handlerId);
			} else {
				UiaDbusElement uiaDbusElement = element as UiaDbusElement;
				if (uiaDbusElement == null) {
					Log.Error ("[RemoveAutomationPropertyChangedEventHandler] " +
						"The element sent to UiaDbusSource is not UiaDbusElement");
					return;
				}
				string busName = uiaDbusElement.BusName;
				DCI.IApplication app = Bus.Session.GetObject<DCI.IApplication> (busName,
					new ObjectPath (DC.Constants.ApplicationPath));
				int [] runtimeId = uiaDbusElement.RuntimeId;
				app.RemoveAutomationPropertyChangedEventHandler (runtimeId, handlerId);
			}
		}

		public void RemoveStructureChangedEventHandler (IElement element,
		                                                StructureChangedEventHandler eventHandler)
		{
			int handlerId = eventHandlerManager.GetStructureEventIdByHandler (eventHandler);
			if (handlerId == -1)
				return;

			if (element == null) {
				//the element is the RootElement
				RootElementEventsManager.RemoveStructureEventRequest (handlerId);
				foreach (var entry in GetUiaApplications ())
					entry.Value.RemoveRootElementStructureChangedEventHandler (handlerId);
			} else {
				UiaDbusElement uiaDbusElement = element as UiaDbusElement;
				if (uiaDbusElement == null) {
					Log.Error ("[RemoveStructureChangedEventHandler] " +
						"The element sent to UiaDbusSource is not UiaDbusElement");
					return;
				}
				string busName = uiaDbusElement.BusName;
				DCI.IApplication app = Bus.Session.GetObject<DCI.IApplication> (busName,
					new ObjectPath (DC.Constants.ApplicationPath));
				int [] runtimeId = uiaDbusElement.RuntimeId;
				app.RemoveStructureChangedEventHandler (runtimeId, handlerId);
			}
		}

		public void RemoveAutomationFocusChangedEventHandler (FocusChangedEventHandler eventHandler)
		{
			lock (focusChangedHandlers)
				focusChangedHandlers.Remove (eventHandler);
		}

		public void RemoveAllEventHandlers ()
		{
			lock (focusChangedHandlers)
				focusChangedHandlers.Clear ();
			foreach (DCI.IApplication app in GetUiaApplications ().Values)
				app.RemoveAllEventHandlers (EventHandlerManager.ClientPrefix);
			RootElementEventsManager.RemoveAll ();
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

		private void EnsureAutomationEventsSetUp (DCI.IApplication app, string busName)
		{
			if (!automationEventBusNames.Contains (busName)) {
				automationEventBusNames.Add (busName);
				app.AutomationEvent += delegate (int hId, int evtId, string providerPath) {
					var handler = eventHandlerManager.GetAutomationEventHandlerById (hId);
					if (handler != null) {
						UiaDbusElement elem = GetOrCreateElement (busName, providerPath);
						AutomationElement ae = SourceManager.GetOrCreateAutomationElement (elem);
						var args = new AutomationEventArgs (AutomationEvent.LookupById (evtId));
						handler (ae, args);
					}
				};
			}
		}

		private void EnsurePropertyEventsSetUp (DCI.IApplication app, string busName)
		{
			if (!propertyEventBusNames.Contains (busName)) {
				propertyEventBusNames.Add (busName);
				app.AutomationPropertyChanged += delegate (
					int hId, int evtId, string providerPath,
					int propertyId, object oldValue, object newValue) {
					var handler = eventHandlerManager.GetPropertyEventHandlerById (hId);
					if (handler != null) {
						UiaDbusElement elem = GetOrCreateElement (busName, providerPath);
						AutomationElement ae = SourceManager.GetOrCreateAutomationElement (elem);
						oldValue = DeserializeAutomationPropertyValue (busName, propertyId, oldValue);
						newValue = DeserializeAutomationPropertyValue (busName, propertyId, newValue);
						var args = new AutomationPropertyChangedEventArgs (
							AutomationProperty.LookupById (propertyId),
							oldValue, newValue);
						handler (ae, args);
					}
				};
			}
		}

		private void EnsureStructureEventsSetUp (DCI.IApplication app, string busName)
		{
			if (!structureEventBusNames.Contains (busName)) {
				structureEventBusNames.Add (busName);
				app.StructureChanged += delegate (int hId, int evtId, string providerPath,
				                                  StructureChangeType changeType) {
					var handler = eventHandlerManager.GetStructureEventHandlerById (hId);
					if (handler != null) {
						UiaDbusElement elem = GetOrCreateElement (busName, providerPath);
						AutomationElement ae = SourceManager.GetOrCreateAutomationElement (elem);
						var args = new StructureChangedEventArgs (changeType, elem.RuntimeId);
						handler (ae, args);
					}
				};
			}
		}

		private void OnRootElementsChanged ()
		{
			if (RootElementsChanged != null)
				RootElementsChanged (this, EventArgs.Empty);
		}

		private string [] GetUiaDbusNames ()
		{
			lock (uiaDbusNamesLock) {
				if (uiaDbusNames == null) {
					FetchUiaDbusNames ();
					Log.Debug ("Count of found Uia applications over dbus: {0}", uiaDbusNames.Count);
					IBus bus = Bus.Session.GetObject<IBus> (DBusName, new ObjectPath (DBusPath));
					bus.NameOwnerChanged += BusNameOwnerChanged;
				}
				return uiaDbusNames.ToArray ();
			}
		}

		private static bool IsUiaDbusName (string busName)
		{
			var intr = Bus.Session.GetObject<Introspectable> (busName,
				new ObjectPath (DC.Constants.ApplicationPath));
			string data = string.Empty;
			try {
				data = intr.Introspect ();
			} catch {}
			return data.Contains (DC.Constants.ApplicationInterfaceName);
		}

		private void BusNameOwnerChanged (string name, string oldOwner, string newOwner)
		{
			if (!string.IsNullOrEmpty(newOwner) &&
			    string.IsNullOrEmpty(oldOwner) &&
			    IsUiaDbusName (newOwner)) {
				lock (uiaDbusNamesLock) {
					if (!uiaDbusNames.Contains (newOwner))
						uiaDbusNames.Add (newOwner);
					else
						Log.Error ("Same application bus name is already in \"uiaDbusNames\"");
				}
				OnRootElementsChanged ();
				BindApplicationEventHandlers (newOwner);
			} else if (!string.IsNullOrEmpty(oldOwner) &&
			    string.IsNullOrEmpty(newOwner)){
				lock (uiaDbusNamesLock) {
					if (uiaDbusNames.Contains (oldOwner))
						uiaDbusNames.Remove (oldOwner);
				}
				OnRootElementsChanged ();
			}
		}

		private void BindApplicationEventHandlers (string busName)
		{
			DCI.IApplication app = Bus.Session.GetObject<DCI.IApplication> (busName,
				new ObjectPath (DC.Constants.ApplicationPath));
			app.RootElementsChanged += OnRootElementsChanged;

			if (listenFocusChangeStarted)
				app.FocusChanged += delegate (string providerPath) {
					if (!string.IsNullOrEmpty (providerPath)) {
						lock (focusChangedHandlers) {
							foreach (var handler in focusChangedHandlers)
								handler (GetOrCreateElement (busName, providerPath), -1, -1);
						}
					}
				};

			var automationEventRequests = RootElementEventsManager.ActiveAutomationEventRequests;
			if (automationEventRequests.Length > 0) {
				foreach (var req in automationEventRequests)
					app.AddRootElementAutomationEventHandler (req.EventId, req.Scope, req.HandlerId);
				EnsureAutomationEventsSetUp (app, busName);
			}
			var propertyEventRequests = RootElementEventsManager.ActivePropertyEventRequests;
			if (propertyEventRequests.Length > 0) {
				foreach (var req in propertyEventRequests)
					app.AddRootElementAutomationPropertyChangedEventHandler (
						req.Scope, req.HandlerId, req.PropertyIds);
				EnsurePropertyEventsSetUp (app, busName);
			}
			var structureEventRequests = RootElementEventsManager.ActiveStructureEventRequests;
			if (structureEventRequests.Length > 0) {
				foreach (var req in structureEventRequests)
					app.AddRootElementStructureChangedEventHandler (
						req.Scope, req.HandlerId);
				EnsureStructureEventsSetUp (app, busName);
			}
		}

		private void FetchUiaDbusNames ()
		{
			var busNames = new List<string> ();

			// Look for suitable buses:
            IBus bus = Bus.Session.GetObject<IBus> (DBusName, new ObjectPath(DBusPath));
            foreach (string busName in bus.ListNames ())
            {
                if (!busName.StartsWith (DBusAutomationBridgeConstants.BusNamePrefix))
                    continue;

			    ObjectPath op = new ObjectPath (DC.Constants.ApplicationPath);
                var app = Bus.Session.GetObject<DCI.IApplication> (busName, op);

				try {
					app.GetRootElementPaths();
				}
				catch {
					continue;
				}

				busNames.Add (busName);

                /*
                2018-12-19:
                At least one case of hanging is related to self-session data reading.
                To avoid this and optimize search procedure lets introduce well-known
                bus names started with "DBusAutomationBridgeConstants.BusNamePrefix".
                
                Older comment:
                *
                * // Reading introspection data hangs on some buses
                * // for unknown reasons, so we call Introspect
                * // asynchronously.  Note that we are using
                * // internal dbus-sharp classes.  The old
                * // behavior was to spin off threads, but this
                * // triggers a Mono bug on Fedora
                * // (BNC#628639/632133)
                * ObjectPath op = new ObjectPath (DC.Constants.ApplicationPath);
                * MethodCall methodCall = new MethodCall (op,
                *     "org.freedesktop.DBus.Introspectable",
                *     "Introspect", busName, Signature.Empty);
                * PendingCall pendingCall = Bus.Session.SendWithReply (methodCall.message);
                * 
                * pendingCall.Completed +=  (message) => {
                *     MessageReader reader = new MessageReader (message);
                *     string data = reader.ReadString ();
                *     if (data.Contains (DC.Constants.ApplicationInterfaceName)) {
                *         busNames.Add ((string)message.Header [FieldCode.Sender]);
                *     }
                * };
                */
            }

			lock (uiaDbusNamesLock)
				uiaDbusNames = busNames;
		}

		private Dictionary<string, DCI.IApplication> GetUiaApplications ()
		{
			Dictionary<string, DCI.IApplication> dbusApps =
				new Dictionary<string, DCI.IApplication> ();

			foreach (string busName in GetUiaDbusNames ()) {
				DCI.IApplication app =
					Bus.Session.GetObject<DCI.IApplication> (busName,
					                                         new ObjectPath (DC.Constants.ApplicationPath));
				if (app == null)
					continue;
				dbusApps [busName] = app;
			}

			return dbusApps;
		}

		private void StartListenFocusChangedEvents ()
		{
			if (listenFocusChangeStarted)
				return;
			foreach (string busName in GetUiaDbusNames ()) {
				DCI.IApplication app =
					Bus.Session.GetObject<DCI.IApplication> (busName,
					                                         new ObjectPath (DC.Constants.ApplicationPath));
				if (app == null)
					continue;
				app.FocusChanged += delegate (string providerPath) {
					if (!string.IsNullOrEmpty (providerPath)) {
						lock (focusChangedHandlers) {
							foreach (var handler in focusChangedHandlers)
								handler (GetOrCreateElement (busName, providerPath), -1, -1);
						}
					}
				};
			}
			listenFocusChangeStarted = true;
		}

		private UiaDbusElement CreateElement (DCI.IAutomationElement dbusElement, string busName, string elementPath)
		{
			if (dbusElement == null)
				return null;
			UiaDbusElement element = new UiaDbusElement (dbusElement, busName, elementPath, this);
			lock (elementMapping)
				elementMapping.Add (new DbusElementTuple (busName, elementPath), element);
			return element;
		}

		private object DeserializeAutomationPropertyValue (string busName, int propId, object value)
		{
			object ret = null;
			if (propId == TableItemPatternIdentifiers.ColumnHeaderItemsProperty.Id ||
			    propId == TableItemPatternIdentifiers.RowHeaderItemsProperty.Id ||
			    propId == TablePatternIdentifiers.ColumnHeadersProperty.Id ||
			    propId == TablePatternIdentifiers.RowHeadersProperty.Id ||
			    propId == SelectionPatternIdentifiers.SelectionProperty.Id) {
				string [] paths = (string [])value;
				AutomationElement [] elements = new AutomationElement [paths.Length];
				for (var i = 0; i < paths.Length; i++) {
					UiaDbusElement elem = GetOrCreateElement (busName, paths [i]);
					elements [i] = SourceManager.GetOrCreateAutomationElement (elem);
				}
				ret = elements;
			} else if (propId == AutomationElementIdentifiers.LabeledByProperty.Id ||
			           propId == GridItemPatternIdentifiers.ContainingGridProperty.Id ||
			           propId == SelectionItemPatternIdentifiers.SelectionContainerProperty.Id) {
				string path = (string) value;
				UiaDbusElement elem = GetOrCreateElement (busName, path);
				ret = SourceManager.GetOrCreateAutomationElement (elem);
			} else
				ret = DC.DbusSerializer.DeserializeValue (propId, value);
			return ret;
		}

		#endregion

		#region Private Methods - DBus Main Loop

		private volatile bool mainLoopStarted = false;
		private volatile static bool runMainLoop = false;
		private Thread mainLoop = null;

		private object mainLoopMethodsLock = new object ();

		private void CheckMainLoop ()
		{
			lock (mainLoopMethodsLock) {
				if (mainLoopStarted)
					return;
				runMainLoop = true;

				Log.Info ("UiaDbusAutomationSource: Starting dbus main loop");
				mainLoop = new Thread (new ThreadStart (MainLoop));
				mainLoop.IsBackground = true;
				mainLoop.Start ();
				mainLoopStarted = true;
			}
		}

		private void AbortMainLoop ()
		{
			lock (mainLoopMethodsLock) {
				runMainLoop = false;
				if (mainLoop != null) {
					Log.Info ("UiaDbusAutomationSource: Stopping dbus main loop");
					mainLoop.Abort ();
					mainLoop = null;
				}
				mainLoopStarted = false;
			}
		}

		private void MainLoop ()
		{
			// TODO: Why does it not work if I uncomment this and
			//       do bus.Iterate (); ?
			//Bus bus = Bus.Session;
			while (true) {
				lock (mainLoopMethodsLock)
					if (!runMainLoop)
						break;

				// TODO: Likely crash source (ndesk-dbus bugs?)
				try {
					Bus.Session.Iterate ();
				}
				catch (ThreadAbortException ex) {
				    Log.Info(
				        "The ThreadAbortException has been catched in the Iterate(). Assume normal program exit.{0}{1}",
				        Environment.NewLine, ex.StackTrace);
				} catch (Exception e) {
					Log.Error ("UiaDbusSource iterate error: " + e);
				}
			}
		}

		#endregion
	}
}
