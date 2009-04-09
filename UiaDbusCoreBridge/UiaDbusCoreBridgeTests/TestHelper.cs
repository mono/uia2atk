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
using System.Reflection;
using System.Threading;
using System.Windows.Automation.Provider;

using NDesk.DBus;
using org.freedesktop.DBus;

using DC = Mono.UIAutomation.DbusCore;
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.UiaDbusCoreBridge;
using Mono.UIAutomation.DbusCore.Interfaces;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.UiaDbusCoreBridge
{
	public class TestHelper
	{
		private static Thread mainLoop;
		private static string ourBus = string.Empty;
		
		public static IAutomationBridge SetUpEnvironment ()
		{
			Type interopProviderType = typeof (AutomationInteropProvider);

			// Set the bridge assembly name to a value that will
			// fail when the static constructor attempts to load it.
			Environment.SetEnvironmentVariable ("MONO_UIA_BRIDGE",
			                                    "Ignore this intentional error");

			// Reset element IDs to 0 for predictable results
			Type providerWrapperType = Type.GetType ("Mono.UIAutomation.UiaDbusCoreBridge.Wrappers.ProviderElementWrapper, UiaDbusCoreBridge, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4ceacb585d99812");
			FieldInfo idCountField =
				providerWrapperType.GetField ("idCount", BindingFlags.NonPublic | BindingFlags.Static);
			idCountField.SetValue (null, 0);
			
			// Inject a UiaDbusCoreBridge that we have a reference to.
			Type bridgeType = Type.GetType ("Mono.UIAutomation.UiaDbusCoreBridge.AutomationBridge, UiaDbusCoreBridge, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4ceacb585d99812");
			IAutomationBridge bridge = (IAutomationBridge) Activator.CreateInstance (bridgeType);
			FieldInfo bridgeField =
				interopProviderType.GetField ("bridges", BindingFlags.NonPublic
				                                        | BindingFlags.Static);
			
			List<IAutomationBridge> bridges = new List<IAutomationBridge> ();
			bridges.Add (bridge);
				
			bridgeField.SetValue (null, bridges);
			bridge.Initialize ();

			return bridge;
		}

		public static void TearDownEnvironment (IAutomationBridge bridge)
		{
			bridge.Terminate ();
			Type interopProviderType = typeof (AutomationInteropProvider);
			FieldInfo bridgeField =
				interopProviderType.GetField ("bridges", BindingFlags.NonPublic | BindingFlags.Static);
			bridgeField.SetValue (null, new List<IAutomationBridge> ());

			
			Environment.SetEnvironmentVariable ("MONO_UIA_BRIDGE",
			                                   string.Empty);
		}

		public static string CurrentBus {
			get {
				if (!string.IsNullOrEmpty (ourBus))
				    return ourBus;
				
				uint currentPid = (uint) System.Diagnostics.Process.GetCurrentProcess ().Id;
				
				List<Thread> threads = new List<Thread> ();
				
				IBus ibus = Bus.Session.GetObject<IBus> ("org.freedesktop.DBus",
				                                         new ObjectPath ("/org/freedesktop/DBus"));
				foreach (string busName in ibus.ListNames ()) {
					ThreadStart start = new ThreadStart (delegate {
						// TODO: Ensure thread-safety on this line
						string currentBus = busName;
						uint pid = 0;
						try {
							pid = ibus.GetConnectionUnixProcessID (currentBus);
						} catch { return; }
						if (pid == currentPid)
							ourBus = currentBus;
					});
					Thread thread = new Thread (start);
					thread.Start ();
					threads.Add (thread);
				}
	
				Thread.Sleep (1000);
				foreach (Thread thread in threads)
					thread.Abort ();
	
				Assert.IsFalse (string.IsNullOrEmpty (ourBus),
				                "Application bus not found");
				return ourBus;
			}
		}

		public static IApplication GetApplication (string ourBus)
		{
			IApplication app =
				Bus.Session.GetObject<IApplication> (ourBus,
				                                             new ObjectPath (DC.Constants.ApplicationPath));
			return app;
		}

		public static IAutomationElement GetElement (string ourBus, string path)
		{
			IAutomationElement element =
				Bus.Session.GetObject<IAutomationElement> (ourBus,
				                                         new ObjectPath (path));
			return element;
		}

		public static void StartDbusMainLoop ()
		{
			Log.Debug ("Starting test dbus main loop");
			mainLoop = new Thread (new ThreadStart (MainLoopThread));
			mainLoop.IsBackground = true;
			mainLoop.Start ();
		}

		public static void StopDbusMainLoop ()
		{
			Log.Debug ("Stopping test dbus main loop");
			if (mainLoop != null)
				mainLoop.Abort ();
			mainLoop = null;
		}

		private static void MainLoopThread ()
		{
			Bus sessionBus = Bus.Session;
			while (true)
				sessionBus.Iterate ();
		}
	}
}
