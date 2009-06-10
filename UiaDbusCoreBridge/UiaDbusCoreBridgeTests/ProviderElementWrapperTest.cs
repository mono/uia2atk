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
using SW = System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.UiaDbusCoreBridge;
using DC = Mono.UIAutomation.DbusCore;
using Mono.UIAutomation.DbusCore.Interfaces;

using NDesk.DBus;
using org.freedesktop.DBus;

using NUnit;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.UiaDbusCoreBridge
{
	internal class MockProvider : IRawElementProviderSimple
	{
		private Dictionary<int, object> properties =
			new Dictionary<int, object> ();
		
		#region IRawElementProviderSimple implementation
		public object GetPatternProvider (int patternId)
		{
			throw new System.NotImplementedException();
		}
		
		public object GetPropertyValue (int propertyId)
		{
			object val = null;
			if (properties.TryGetValue (propertyId, out val))
				return val;
			return null;
		}
		
		public IRawElementProviderSimple HostRawElementProvider {
			get {
				throw new System.NotImplementedException();
			}
		}
		
		public ProviderOptions ProviderOptions {
			get {
				throw new System.NotImplementedException();
			}
		}
		#endregion

		public void SetPropertyValue (int propertyId, object val)
		{
			properties [propertyId] = val;
		}
	}
	
	[TestFixture]
	public class ProviderElementWrapperTest
	{
		private IAutomationBridge bridge;

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			TestHelper.StartDbusMainLoop ();
		}

		[TestFixtureTearDown]
		public void FixtureTearDown ()
		{
			TestHelper.StopDbusMainLoop ();
		}
		
		[SetUp]
		public void SetUp ()
		{
			bridge = TestHelper.SetUpEnvironment ();
		}

		[TearDown]
		public void TearDown ()
		{
			TestHelper.TearDownEnvironment (bridge);
		}

		[Test]
		public void LabeledByTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			mockWindow.SetPropertyValue (AEIds.ControlTypeProperty.Id,
			                             ControlType.Window.Id);
			mockWindow.SetPropertyValue (AEIds.NativeWindowHandleProperty.Id,
			                             new IntPtr (1234));

			string labelText = "label text";
			MockProvider mockLabel = new MockProvider ();
			mockLabel.SetPropertyValue (AEIds.ControlTypeProperty.Id,
			                             ControlType.Text.Id);
			mockLabel.SetPropertyValue (AEIds.NameProperty.Id,
			                             labelText);

			//string buttonText = "click me";
			MockProvider mockButton = new MockProvider ();
			mockButton.SetPropertyValue (AEIds.ControlTypeProperty.Id,
			                             ControlType.Button.Id);

			AddProvider (mockWindow);
			AddProvider (mockLabel);
			AddProvider (mockButton);

			string windowPath = "/org/mono/UIAutomation/Element/1";
			string labelPath = "/org/mono/UIAutomation/Element/2";
			string buttonPath = "/org/mono/UIAutomation/Element/3";

			string ourBus = TestHelper.CurrentBus;

			IAutomationElement windowElement = TestHelper.GetElement (ourBus, windowPath);
			IAutomationElement labelElement = TestHelper.GetElement (ourBus, labelPath);
			IAutomationElement buttonElement = TestHelper.GetElement (ourBus, buttonPath);

			// Verify that we matched up our elements correctly
			Assert.AreEqual (ControlType.Window.Id,
			                 windowElement.ControlTypeId,
			                 "Window ControlType");
			Assert.AreEqual (ControlType.Text.Id,
			                 labelElement.ControlTypeId,
			                 "Label ControlType");
			Assert.AreEqual (ControlType.Button.Id,
			                 buttonElement.ControlTypeId,
			                 "Button ControlType");

			// Test LabeledBy Initial Value
			Assert.AreEqual (string.Empty,
			                 buttonElement.LabeledByElementPath,
			                 "Default LabeledBy value");

			// Test with LabeledBy Property Set
			mockButton.SetPropertyValue (AEIds.LabeledByProperty.Id,
			                             mockLabel);
			Assert.AreEqual (labelPath,
			                 buttonElement.LabeledByElementPath,
			                 "Button should be LabeledBy label element now");
		}

		[Test]
		public void AcceleratorKeyTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (string.Empty,
			                 windowElement.AcceleratorKey,
			                 "Initial value");

			// Test set value
			object testVal = "alt+s";
			mockWindow.SetPropertyValue (AEIds.AcceleratorKeyProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.AcceleratorKey,
			                 "Set value");
		}

		[Test]
		public void AccessKeyTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (string.Empty,
			                 windowElement.AccessKey,
			                 "Initial value");

			// Test set value
			object testVal = "alt+s";
			mockWindow.SetPropertyValue (AEIds.AccessKeyProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.AccessKey,
			                 "Set value");
		}

		[Test]
		public void AutomationIdTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (string.Empty,
			                 windowElement.AutomationId,
			                 "Initial value");

			// Test set value
			int testVal = 3254;
			mockWindow.SetPropertyValue (AEIds.AutomationIdProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal.ToString (),
			                 windowElement.AutomationId,
			                 "Set value");
		}

		[Test]
		public void BoundingRectangleTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (new DC.Rect (SW.Rect.Empty),
			                 windowElement.BoundingRectangle,
			                 "Initial value");

			// Test set value
			SW.Rect testVal = new SW.Rect (5, 6, 7, 8);
			mockWindow.SetPropertyValue (AEIds.BoundingRectangleProperty.Id,
			                             testVal);
			Assert.AreEqual (new DC.Rect (testVal),
			                 windowElement.BoundingRectangle,
			                 "Set value");
		}

		[Test]
		public void ClassNameTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (string.Empty,
			                 windowElement.ClassName,
			                 "Initial value");

			// Test set value
			object testVal = "SuperWindow";
			mockWindow.SetPropertyValue (AEIds.ClassNameProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.ClassName,
			                 "Set value");
		}

		[Test]
		public void ClickablePointTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (new DC.Point (new SW.Point (double.NegativeInfinity,
			                                               double.NegativeInfinity)),
			                 windowElement.ClickablePoint,
			                 "Initial value");

			// Test set value
			SW.Point testVal = new SW.Point (7, 8);
			mockWindow.SetPropertyValue (AEIds.ClickablePointProperty.Id,
			                             testVal);
			Assert.AreEqual (new DC.Point (testVal),
			                 windowElement.ClickablePoint,
			                 "Set value");
		}

		[Test]
		public void ControlTypeTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);
			mockWindow.SetPropertyValue (AEIds.ControlTypeProperty.Id,
			                             null);

			// Test initial value
			Assert.AreEqual (-1,
			                 windowElement.ControlTypeId,
			                 "Initial value");

			// Test set value
			object testVal = ControlType.Window.Id;
			mockWindow.SetPropertyValue (AEIds.ControlTypeProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.ControlTypeId,
			                 "Set value");
		}

		[Test]
		public void FrameworkIdTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (string.Empty,
			                 windowElement.FrameworkId,
			                 "Initial value");

			// Test set value
			object testVal = "9988";
			mockWindow.SetPropertyValue (AEIds.FrameworkIdProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.FrameworkId,
			                 "Set value");
		}

		[Test]
		public void HasKeyboardFocusTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (false,
			                 windowElement.HasKeyboardFocus,
			                 "Initial value");

			// Test set value
			object testVal = true;
			mockWindow.SetPropertyValue (AEIds.HasKeyboardFocusProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.HasKeyboardFocus,
			                 "Set value");
		}

		[Test]
		public void HelpTextTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (string.Empty,
			                 windowElement.HelpText,
			                 "Initial value");

			// Test set value
			object testVal = "Sample Help Text";
			mockWindow.SetPropertyValue (AEIds.HelpTextProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.HelpText,
			                 "Set value");
		}

		[Test]
		public void IsContentElementTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (false,
			                 windowElement.IsContentElement,
			                 "Initial value");

			// Test set value
			object testVal = true;
			mockWindow.SetPropertyValue (AEIds.IsContentElementProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.IsContentElement,
			                 "Set value");
		}

		[Test]
		public void IsControlElementTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (false,
			                 windowElement.IsControlElement,
			                 "Initial value");

			// Test set value
			object testVal = true;
			mockWindow.SetPropertyValue (AEIds.IsControlElementProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.IsControlElement,
			                 "Set value");
		}

		[Test]
		public void IsEnabledTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (false,
			                 windowElement.IsEnabled,
			                 "Initial value");

			// Test set value
			object testVal = true;
			mockWindow.SetPropertyValue (AEIds.IsEnabledProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.IsEnabled,
			                 "Set value");
		}

		[Test]
		public void IsKeyboardFocusableTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (false,
			                 windowElement.IsKeyboardFocusable,
			                 "Initial value");

			// Test set value
			object testVal = true;
			mockWindow.SetPropertyValue (AEIds.IsKeyboardFocusableProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.IsKeyboardFocusable,
			                 "Set value");
		}

		[Test]
		public void IsOffscreenTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (false,
			                 windowElement.IsOffscreen,
			                 "Initial value");

			// Test set value
			object testVal = true;
			mockWindow.SetPropertyValue (AEIds.IsOffscreenProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.IsOffscreen,
			                 "Set value");
		}

		[Test]
		public void IsPasswordTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (false,
			                 windowElement.IsPassword,
			                 "Initial value");

			// Test set value
			object testVal = true;
			mockWindow.SetPropertyValue (AEIds.IsPasswordProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.IsPassword,
			                 "Set value");
		}

		[Test]
		public void IsRequiredForFormTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (false,
			                 windowElement.IsRequiredForForm,
			                 "Initial value");

			// Test set value
			object testVal = true;
			mockWindow.SetPropertyValue (AEIds.IsRequiredForFormProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.IsRequiredForForm,
			                 "Set value");
		}

		[Test]
		public void ItemStatusTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (string.Empty,
			                 windowElement.ItemStatus,
			                 "Initial value");

			// Test set value
			object testVal = "all systems go";
			mockWindow.SetPropertyValue (AEIds.ItemStatusProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.ItemStatus,
			                 "Set value");
		}

		[Test]
		public void ItemTypeTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (string.Empty,
			                 windowElement.ItemType,
			                 "Initial value");

			// Test set value
			object testVal = "extra special window type";
			mockWindow.SetPropertyValue (AEIds.ItemTypeProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.ItemType,
			                 "Set value");
		}

		[Test]
		public void LocalizedControlTypeTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (string.Empty,
			                 windowElement.LocalizedControlType,
			                 "Initial value");

			// Test set value
			object testVal = ControlType.Window.LocalizedControlType;
			mockWindow.SetPropertyValue (AEIds.LocalizedControlTypeProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.LocalizedControlType,
			                 "Set value");
		}

		[Test]
		public void NameTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (string.Empty,
			                 windowElement.Name,
			                 "Initial value");

			// Test set value
			object testVal = "my cool window";
			mockWindow.SetPropertyValue (AEIds.NameProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.Name,
			                 "Set value");
		}

		[Test]
		public void NativeWindowHandleTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);
			IntPtr originalPtr = (IntPtr)
				mockWindow.GetPropertyValue (AEIds.NativeWindowHandleProperty.Id);

			// Test initial value
			mockWindow.SetPropertyValue (AEIds.NativeWindowHandleProperty.Id,
			                             null);
			Assert.AreEqual (-1,
			                 windowElement.NativeWindowHandle,
			                 "Initial value");

			// Test set value
			mockWindow.SetPropertyValue (AEIds.NativeWindowHandleProperty.Id,
			                             originalPtr);
			Assert.AreEqual (originalPtr.ToInt32 (),
			                 windowElement.NativeWindowHandle,
			                 "Set value");
		}

		[Test]
		public void OrientationTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (OrientationType.None,
			                 windowElement.Orientation,
			                 "Initial value");

			// Test set value
			object testVal = OrientationType.Horizontal;
			mockWindow.SetPropertyValue (AEIds.OrientationProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.Orientation,
			                 "Set value");
		}

		[Test]
		public void ProcessIdTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (-1,
			                 windowElement.ProcessId,
			                 "Initial value");

			// Test set value
			object testVal = 64523;
			mockWindow.SetPropertyValue (AEIds.ProcessIdProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.ProcessId,
			                 "Set value");
		}

		[Test]
		public void RuntimeIdTest ()
		{
			MockProvider mockWindow = new MockProvider ();
			IAutomationElement windowElement =
				SetupBasicWindowElement (mockWindow);

			// Test initial value
			Assert.AreEqual (0,
			                 windowElement.RuntimeId.Length,
			                 "Initial value");

			// Test set value
			object testVal = new int [] {5, 6};
			mockWindow.SetPropertyValue (AEIds.RuntimeIdProperty.Id,
			                             testVal);
			Assert.AreEqual (testVal,
			                 windowElement.RuntimeId,
			                 "Set value");
		}

		private IAutomationElement SetupBasicWindowElement (MockProvider mockWindow)
		{
			mockWindow.SetPropertyValue (AEIds.ControlTypeProperty.Id,
			                             ControlType.Window.Id);
			mockWindow.SetPropertyValue (AEIds.NativeWindowHandleProperty.Id,
			                             new IntPtr (1234));

			AddProvider (mockWindow);

			string ourBus = TestHelper.CurrentBus;

			string windowPath = "/org/mono/UIAutomation/Element/1";

			IAutomationElement windowElement = TestHelper.GetElement (ourBus, windowPath);
			return windowElement;
		}

		private void AddProvider (IRawElementProviderSimple provider)
		{
			AutomationInteropProvider.RaiseStructureChangedEvent (provider,
			                                                      new StructureChangedEventArgs (StructureChangeType.ChildAdded, new int [0]));
		}
	}
}
