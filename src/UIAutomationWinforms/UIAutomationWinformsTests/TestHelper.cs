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
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using NUnit.Framework;
using Mono.UIAutomation.Bridge;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	public static class TestHelper
	{
		public static MockBridge SetUpEnvironment ()
		{
			Type interopProviderType = typeof (AutomationInteropProvider);

			// HACK: Clear the string referencing the UiaAtkBridge
			//       assembly so that when AutomationInteropProvider's
			//       static constructor attempts to load a bridge,
			//       it will fail.
			Type bridgeManagerType =
				interopProviderType.Assembly.GetType ("System.Windows.Automation.Provider.BridgeManager");
			FieldInfo assemblyField =
				bridgeManagerType.GetField ("UiaAtkBridgeAssembly",
				                            BindingFlags.NonPublic | BindingFlags.Static);
			assemblyField.SetValue (null, string.Empty);
			
			// Inject a mock automation bridge into the
			// AutomationInteropProvider, so that we don't try
			// to load the UiaAtkBridge.
			MockBridge bridge = new MockBridge ();
			FieldInfo bridgeField =
				interopProviderType.GetField ("bridge", BindingFlags.NonPublic
				                                        | BindingFlags.Static);
			bridgeField.SetValue (null, bridge);
			
			bridge.ClientsAreListening = true;

			return bridge;
		}

		public static void TearDownEnvironment ()
		{
			Type interopProviderType = typeof (AutomationInteropProvider);
			FieldInfo bridgeField =
				interopProviderType.GetField ("bridge", BindingFlags.NonPublic | BindingFlags.Static);
			bridgeField.SetValue (null, null);
		}

		public static void TestAutomationProperty (IRawElementProviderSimple provider,
		                                           AutomationProperty property,
		                                           object expectedValue)
		{
			Assert.AreEqual (expectedValue,
			                 provider.GetPropertyValue (property.Id),
			                 property.ProgrammaticName);
		}
	}
}
