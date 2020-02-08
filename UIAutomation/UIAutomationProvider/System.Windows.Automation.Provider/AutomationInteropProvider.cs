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
//      Calvin Gaisford <calvinrg@gmail.com>
// 

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Mono.UIAutomation.Bridge;

namespace System.Windows.Automation.Provider
{
	public static class AutomationInteropProvider
	{
		public const int AppendRuntimeId = 3;
		public const int InvalidateLimit = 20;
		public const int RootObjectId = -25;
		
		private static IList<IAutomationBridge> bridges;
		private static Dictionary<IntPtr, WeakReference> providerMapping
			= new Dictionary<IntPtr, WeakReference> ();
		
		static AutomationInteropProvider ()
		{
			bridges = BridgeManager.GetAutomationBridges ();
			if (bridges == null)
				bridges = new List<IAutomationBridge> ();
		}

		public static bool ClientsAreListening {
			get {
				// Important to forward to all bridges,
				// even after one has returned true.
				bool listening = false;
				foreach (var bridge in bridges)
					if (bridge.ClientsAreListening)
						listening = true;
				return listening;
			}
		}

		public static IRawElementProviderSimple HostProviderFromHandle (IntPtr hwnd)
		{
			foreach (var bridge in bridges) {
				var provider =
					bridge.HostProviderFromHandle (hwnd) as IRawElementProviderSimple;
				if (provider != null)
					return provider;
			}
			return null;
		}

		public static void RaiseAutomationEvent (AutomationEvent eventId, IRawElementProviderSimple provider, AutomationEventArgs e)
		{
			foreach (var bridge in bridges)
				bridge.RaiseAutomationEvent (eventId, provider, e);
		}

		public static void RaiseAutomationPropertyChangedEvent (IRawElementProviderSimple element, AutomationPropertyChangedEventArgs e) 
		{
			foreach (var bridge in bridges)
				bridge.RaiseAutomationPropertyChangedEvent (element, e);
		}

		public static void RaiseStructureChangedEvent (IRawElementProviderSimple provider, StructureChangedEventArgs e)
		{
			foreach (var bridge in bridges)
				bridge.RaiseStructureChangedEvent (provider, e);
		}

		public static IntPtr ReturnRawElementProvider (IntPtr hwnd, IntPtr wParam, IntPtr lParam, IRawElementProviderSimple el) 
		{
			// Hopefully using the hwnd as a hashcode is unique
			// enough.  I am concerned about being called multiple
			// times for the same hwnd before the value is
			// retrieved though.
			providerMapping [hwnd] = new WeakReference (el);
			return hwnd;
		}

		internal static IRawElementProviderSimple RetrieveAndDeleteProvider (IntPtr result)
		{
			if (!providerMapping.ContainsKey (result)) {
				return null;
			}

			WeakReference weakRef = providerMapping [result];
			if (!weakRef.IsAlive) {
				return null;
			}
			
			providerMapping.Remove (result);
			return weakRef.Target as IRawElementProviderSimple;
		}
	}
	
	internal static class BridgeManager
	{
		private const string UIA_ENABLED_ENV_VAR = "MONO_UIA_ENABLED";
		private const string BRIDGE_ASSEMBLY_NAMES_ENV_VAR = "MONO_UIA_BRIDGE";
		private const char BRIDGE_ASSEMBLY_NAMES_DELIM = ';';

		private static string UiaAtkBridgeAssembly =
			"UiaAtkBridge, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4ceacb585d99812";
		private static string UiaDbusBridgeAssembly =
			"UiaDbusBridge, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4ceacb585d99812";
		private static string ClientBridgeAssembly =
			"UIAutomationClient, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";

		public static IList<IAutomationBridge> GetAutomationBridges ()
		{
			bool isUiaEnabled = true;
			
			var isUiaEnabledEnvVar = Environment.GetEnvironmentVariable (UIA_ENABLED_ENV_VAR);
			if (!string.IsNullOrEmpty(isUiaEnabledEnvVar))
			{
				if (isUiaEnabledEnvVar == "0")
					isUiaEnabled = false;
				else if (isUiaEnabledEnvVar != "1")
					throw new Exception($"The environment varianble '{UIA_ENABLED_ENV_VAR}' may be equal to '0' and '1' only, or may be empty or undefined.");
			}

			if (!isUiaEnabled) {
				Console.WriteLine ($"No one UIA bridge is loaded: {UIA_ENABLED_ENV_VAR}='{isUiaEnabledEnvVar}'");
				return new List<IAutomationBridge> ();
			}

			// Let MONO_UIA_BRIDGE env var override default bridge
			List<string> bridgeAssemblyNames =
				(Environment.GetEnvironmentVariable (BRIDGE_ASSEMBLY_NAMES_ENV_VAR) ?? string.Empty)
				.Split (BRIDGE_ASSEMBLY_NAMES_DELIM)
				.Where (name => !string.IsNullOrEmpty(name))
				.ToList ();

			if (bridgeAssemblyNames.Count == 0)
				bridgeAssemblyNames.AddRange(new [] { UiaAtkBridgeAssembly, UiaDbusBridgeAssembly });

			bridgeAssemblyNames.Add(ClientBridgeAssembly);

			var namedBridges =
				bridgeAssemblyNames.Select (name => (name: name, bridge: GetAutomationBridge (name)))
				.Where (x => x.bridge != null)
				.ToList ();

			var loadedBridgesMsg = new List<string> { "Loaded UIA bridges:" };
			loadedBridgesMsg.AddRange (namedBridges.Select (x => x.name));
			loadedBridgesMsg.Add ($"* To disable UIA bridges loading set environment variable {UIA_ENABLED_ENV_VAR} to '0'. *");
			Console.WriteLine (string.Join (Environment.NewLine + "  ", loadedBridgesMsg));

			return namedBridges.Select (x => x.bridge).ToList ();
		}
		
		private static IAutomationBridge GetAutomationBridge (string bridgeAssemblyName)
		{
			Assembly bridgeAssembly = null;
			try {
				bridgeAssembly = Assembly.Load (bridgeAssemblyName);
			} catch (Exception e){
				Console.WriteLine (string.Format ("Error loading UIA bridge ({0}): {1}",
				                                  bridgeAssemblyName,
				                                  e));
				return null;
			}
			
			Type bridgeType = null;
			
			// Quickie inefficent implementation
			Type bridgeInterfaceType = typeof (IAutomationBridge);
			foreach (Type type in bridgeAssembly.GetTypes ()) {
				if (bridgeInterfaceType.IsAssignableFrom (type)) {
					bridgeType = type;
					break;
				}
			}
			if (bridgeType == null) {
				Console.WriteLine ("No UIA bridge found in assembly: " +
				                   bridgeAssemblyName);
				return null;
			}

			try {
				IAutomationBridge bridge
					= (IAutomationBridge) Activator.CreateInstance (bridgeType);
				if (!bridge.IsAccessibilityEnabled)
					return null;

				bridge.Initialize ();
				return bridge;
			} catch (Exception e) {
				Console.WriteLine ("Failed to load UIA bridge: " + e);
				return null;
			}
		}
	}
}