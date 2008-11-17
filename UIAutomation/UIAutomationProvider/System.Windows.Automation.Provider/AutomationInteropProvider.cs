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
using System.Windows.Automation;

using System.Reflection;

using Mono.UIAutomation.Bridge;

namespace System.Windows.Automation.Provider
{
	public static class AutomationInteropProvider
	{
		public const int AppendRuntimeId = 3;
		public const int InvalidateLimit = 20;
		public const int RootObjectId = -25;
		
		private static IAutomationBridge bridge = null;
		
		static AutomationInteropProvider ()
		{
			bridge = BridgeManager.GetAutomationBridge ();
		}

		public static bool ClientsAreListening {
			get {
				if (bridge == null)
					return false;
				return bridge.ClientsAreListening;
			}
		}

		public static IRawElementProviderSimple HostProviderFromHandle (IntPtr hwnd)
		{
			if (bridge == null)
				return null;
			return (IRawElementProviderSimple) bridge.HostProviderFromHandle (hwnd);
		}

		public static void RaiseAutomationEvent (AutomationEvent eventId, IRawElementProviderSimple provider, AutomationEventArgs e)
		{
			if (bridge == null)
				return;
			bridge.RaiseAutomationEvent (eventId, provider, e);
		}

		public static void RaiseAutomationPropertyChangedEvent (IRawElementProviderSimple element, AutomationPropertyChangedEventArgs e) 
		{
			if (bridge == null)
				return;
			bridge.RaiseAutomationPropertyChangedEvent (element, e);
		}

		public static void RaiseStructureChangedEvent (IRawElementProviderSimple provider, StructureChangedEventArgs e)
		{
			if (bridge == null)
				return;
			bridge.RaiseStructureChangedEvent (provider, e);
		}

		public static IntPtr ReturnRawElementProvider (IntPtr hwnd, IntPtr wParam, IntPtr lParam, IRawElementProviderSimple el) 
		{
			throw new NotImplementedException();
		}
	}
	
	internal static class BridgeManager
	{
		private static string UiaAtkBridgeAssembly =
			"UiaAtkBridge, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4ceacb585d99812";
		
		public static IAutomationBridge GetAutomationBridge ()
		{
			// TODO: Get bridge assembly details from env var or
			//       some other run-time value?
			Assembly bridgeAssembly = null;
			try {
				bridgeAssembly = Assembly.Load (UiaAtkBridgeAssembly);
			} catch {
				// TODO: This is just here for testing
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
			if (bridgeType == null)
				throw new Exception ("No bridge, bummer!");

			IAutomationBridge bridge
				= (IAutomationBridge) Activator.CreateInstance (bridgeType);
			if (!bridge.IsAccessibilityEnabled)
				return null;

			bridge.Initialize ();
			return bridge;
		}
	}
}


