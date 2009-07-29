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
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.UiaDbusBridge;
using Mono.UIAutomation.UiaDbus.Interfaces;

using NDesk.DBus;
using org.freedesktop.DBus;

using NUnit;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.UiaDbusBridge
{
	[TestFixture]
	public class AutomationBridgeTest
	{
		private IAutomationBridge bridge;
		
		[SetUp]
		public void SetUp ()
		{
			TestHelper.StartDbusMainLoop ();
			bridge = TestHelper.SetUpEnvironment ();
		}

		[TearDown]
		public void TearDown ()
		{
			TestHelper.TearDownEnvironment (bridge);
			TestHelper.StopDbusMainLoop ();
		}

		[Test]
		public void SimpleFormTest ()
		{
			Form f = new Form ();
			f.Text = "Hello World";
			f.Show ();

			string ourBus = TestHelper.CurrentBus;

			IApplication app = TestHelper.GetApplication (ourBus);

			Assert.IsNotNull (app, "Unable to get UiaDbus.IApplication");

			string [] rootElementPaths = app.GetRootElementPaths ();
			Assert.AreEqual (1, rootElementPaths.Length);
			Assert.AreEqual ("/org/mono/UIAutomation/Element/1", rootElementPaths [0]);

			IAutomationElement element1 = TestHelper.GetElement (ourBus, rootElementPaths [0]);
			Assert.IsNotNull (element1, "Unable to get root element");
			Assert.AreEqual (f.Text,
			                 element1.Name,
			                 "Root element name");
			Assert.AreEqual (ControlType.Window.Id,
			                 element1.ControlTypeId,
			                 "Root element control type");

			f.Close ();

			rootElementPaths = app.GetRootElementPaths ();
			Assert.AreEqual (0, rootElementPaths.Length,
			                 "No root elements when window closed");

			bool exceptionOccurred = false;
			try {
				element1.Name.ToString ();
			} catch (Exception e) {
				exceptionOccurred = true;
				string expectedMessage = "org.freedesktop.DBus.Error.UnknownMethod: Method \"GetName\" with signature \"\" on interface \"org.mono.UIAutomation.AutomationElement\" doesn't exist";
				Assert.AreEqual (expectedMessage,
				                 e.Message,
				                 "Unexpected exception when trying to use invalid dbus proxy object");
			}
			Assert.IsTrue (exceptionOccurred,
			               "Expected an exception when trying to use invalid dbus proxy object");

			f = new Form ();
			f.Show ();
			
			app = TestHelper.GetApplication (ourBus);

			Assert.IsNotNull (app, "Unable to get UiaDbus.IApplication");

			rootElementPaths = app.GetRootElementPaths ();
			Assert.AreEqual (1, rootElementPaths.Length);
			Assert.AreEqual ("/org/mono/UIAutomation/Element/2", rootElementPaths [0]);

			element1 = TestHelper.GetElement (ourBus, rootElementPaths [0]);
			Assert.IsNotNull (element1, "Unable to get root element");
			Assert.AreEqual (f.Text,
			                 element1.Name,
			                 "Root element name");
			Assert.AreEqual (ControlType.Window.Id,
			                 element1.ControlTypeId,
			                 "Root element control type");
		}
	}
}
