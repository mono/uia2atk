
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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Windows.Automation;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation {
	
	[TestFixture]
	public class AutomationElementIdentifiersTest {

		[Test]
		public void NotSupportedTest ()
		{
			Object myNotSupported =
				AutomationElementIdentifiers.NotSupported;
			Assert.IsNotNull (
				myNotSupported,
				"Field must not be null.");
		}

		[Test]
		public void IsControlElementPropertyTest ()
		{
			AutomationProperty myIsControlElementProperty =
				AutomationElementIdentifiers.IsControlElementProperty;
			Assert.IsNotNull (
				myIsControlElementProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30016,
				myIsControlElementProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsControlElementProperty",
				myIsControlElementProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsControlElementProperty,
				AutomationProperty.LookupById (myIsControlElementProperty.Id),
				"LookupById");
		}

		[Test]
		public void ControlTypePropertyTest ()
		{
			AutomationProperty myControlTypeProperty =
				AutomationElementIdentifiers.ControlTypeProperty;
			Assert.IsNotNull (
				myControlTypeProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30003,
				myControlTypeProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.ControlTypeProperty",
				myControlTypeProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myControlTypeProperty,
				AutomationProperty.LookupById (myControlTypeProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsContentElementPropertyTest ()
		{
			AutomationProperty myIsContentElementProperty =
				AutomationElementIdentifiers.IsContentElementProperty;
			Assert.IsNotNull (
				myIsContentElementProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30017,
				myIsContentElementProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsContentElementProperty",
				myIsContentElementProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsContentElementProperty,
				AutomationProperty.LookupById (myIsContentElementProperty.Id),
				"LookupById");
		}

		[Test]
		public void LabeledByPropertyTest ()
		{
			AutomationProperty myLabeledByProperty =
				AutomationElementIdentifiers.LabeledByProperty;
			Assert.IsNotNull (
				myLabeledByProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30018,
				myLabeledByProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.LabeledByProperty",
				myLabeledByProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myLabeledByProperty,
				AutomationProperty.LookupById (myLabeledByProperty.Id),
				"LookupById");
		}

		[Test]
		public void NativeWindowHandlePropertyTest ()
		{
			AutomationProperty myNativeWindowHandleProperty =
				AutomationElementIdentifiers.NativeWindowHandleProperty;
			Assert.IsNotNull (
				myNativeWindowHandleProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30020,
				myNativeWindowHandleProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.NativeWindowHandleProperty",
				myNativeWindowHandleProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myNativeWindowHandleProperty,
				AutomationProperty.LookupById (myNativeWindowHandleProperty.Id),
				"LookupById");
		}

		[Test]
		public void AutomationIdPropertyTest ()
		{
			AutomationProperty myAutomationIdProperty =
				AutomationElementIdentifiers.AutomationIdProperty;
			Assert.IsNotNull (
				myAutomationIdProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30011,
				myAutomationIdProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.AutomationIdProperty",
				myAutomationIdProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myAutomationIdProperty,
				AutomationProperty.LookupById (myAutomationIdProperty.Id),
				"LookupById");
		}

		[Test]
		public void ItemTypePropertyTest ()
		{
			AutomationProperty myItemTypeProperty =
				AutomationElementIdentifiers.ItemTypeProperty;
			Assert.IsNotNull (
				myItemTypeProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30021,
				myItemTypeProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.ItemTypeProperty",
				myItemTypeProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myItemTypeProperty,
				AutomationProperty.LookupById (myItemTypeProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsPasswordPropertyTest ()
		{
			AutomationProperty myIsPasswordProperty =
				AutomationElementIdentifiers.IsPasswordProperty;
			Assert.IsNotNull (
				myIsPasswordProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30019,
				myIsPasswordProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsPasswordProperty",
				myIsPasswordProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsPasswordProperty,
				AutomationProperty.LookupById (myIsPasswordProperty.Id),
				"LookupById");
		}

		[Test]
		public void LocalizedControlTypePropertyTest ()
		{
			AutomationProperty myLocalizedControlTypeProperty =
				AutomationElementIdentifiers.LocalizedControlTypeProperty;
			Assert.IsNotNull (
				myLocalizedControlTypeProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30004,
				myLocalizedControlTypeProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.LocalizedControlTypeProperty",
				myLocalizedControlTypeProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myLocalizedControlTypeProperty,
				AutomationProperty.LookupById (myLocalizedControlTypeProperty.Id),
				"LookupById");
		}

		[Test]
		public void NamePropertyTest ()
		{
			AutomationProperty myNameProperty =
				AutomationElementIdentifiers.NameProperty;
			Assert.IsNotNull (
				myNameProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30005,
				myNameProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.NameProperty",
				myNameProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myNameProperty,
				AutomationProperty.LookupById (myNameProperty.Id),
				"LookupById");
		}

		[Test]
		public void AcceleratorKeyPropertyTest ()
		{
			AutomationProperty myAcceleratorKeyProperty =
				AutomationElementIdentifiers.AcceleratorKeyProperty;
			Assert.IsNotNull (
				myAcceleratorKeyProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30006,
				myAcceleratorKeyProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.AcceleratorKeyProperty",
				myAcceleratorKeyProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myAcceleratorKeyProperty,
				AutomationProperty.LookupById (myAcceleratorKeyProperty.Id),
				"LookupById");
		}

		[Test]
		public void AccessKeyPropertyTest ()
		{
			AutomationProperty myAccessKeyProperty =
				AutomationElementIdentifiers.AccessKeyProperty;
			Assert.IsNotNull (
				myAccessKeyProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30007,
				myAccessKeyProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.AccessKeyProperty",
				myAccessKeyProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myAccessKeyProperty,
				AutomationProperty.LookupById (myAccessKeyProperty.Id),
				"LookupById");
		}

		[Test]
		public void HasKeyboardFocusPropertyTest ()
		{
			AutomationProperty myHasKeyboardFocusProperty =
				AutomationElementIdentifiers.HasKeyboardFocusProperty;
			Assert.IsNotNull (
				myHasKeyboardFocusProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30008,
				myHasKeyboardFocusProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.HasKeyboardFocusProperty",
				myHasKeyboardFocusProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myHasKeyboardFocusProperty,
				AutomationProperty.LookupById (myHasKeyboardFocusProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsKeyboardFocusablePropertyTest ()
		{
			AutomationProperty myIsKeyboardFocusableProperty =
				AutomationElementIdentifiers.IsKeyboardFocusableProperty;
			Assert.IsNotNull (
				myIsKeyboardFocusableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30009,
				myIsKeyboardFocusableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsKeyboardFocusableProperty",
				myIsKeyboardFocusableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsKeyboardFocusableProperty,
				AutomationProperty.LookupById (myIsKeyboardFocusableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsEnabledPropertyTest ()
		{
			AutomationProperty myIsEnabledProperty =
				AutomationElementIdentifiers.IsEnabledProperty;
			Assert.IsNotNull (
				myIsEnabledProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30010,
				myIsEnabledProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsEnabledProperty",
				myIsEnabledProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsEnabledProperty,
				AutomationProperty.LookupById (myIsEnabledProperty.Id),
				"LookupById");
		}

		[Test]
		public void BoundingRectanglePropertyTest ()
		{
			AutomationProperty myBoundingRectangleProperty =
				AutomationElementIdentifiers.BoundingRectangleProperty;
			Assert.IsNotNull (
				myBoundingRectangleProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30001,
				myBoundingRectangleProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.BoundingRectangleProperty",
				myBoundingRectangleProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myBoundingRectangleProperty,
				AutomationProperty.LookupById (myBoundingRectangleProperty.Id),
				"LookupById");
		}

		[Test]
		public void ProcessIdPropertyTest ()
		{
			AutomationProperty myProcessIdProperty =
				AutomationElementIdentifiers.ProcessIdProperty;
			Assert.IsNotNull (
				myProcessIdProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30002,
				myProcessIdProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.ProcessIdProperty",
				myProcessIdProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myProcessIdProperty,
				AutomationProperty.LookupById (myProcessIdProperty.Id),
				"LookupById");
		}

		[Test]
		public void RuntimeIdPropertyTest ()
		{
			AutomationProperty myRuntimeIdProperty =
				AutomationElementIdentifiers.RuntimeIdProperty;
			Assert.IsNotNull (
				myRuntimeIdProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30000,
				myRuntimeIdProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.RuntimeIdProperty",
				myRuntimeIdProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myRuntimeIdProperty,
				AutomationProperty.LookupById (myRuntimeIdProperty.Id),
				"LookupById");
		}

		[Test]
		public void ClassNamePropertyTest ()
		{
			AutomationProperty myClassNameProperty =
				AutomationElementIdentifiers.ClassNameProperty;
			Assert.IsNotNull (
				myClassNameProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30012,
				myClassNameProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.ClassNameProperty",
				myClassNameProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myClassNameProperty,
				AutomationProperty.LookupById (myClassNameProperty.Id),
				"LookupById");
		}

		[Test]
		public void HelpTextPropertyTest ()
		{
			AutomationProperty myHelpTextProperty =
				AutomationElementIdentifiers.HelpTextProperty;
			Assert.IsNotNull (
				myHelpTextProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30013,
				myHelpTextProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.HelpTextProperty",
				myHelpTextProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myHelpTextProperty,
				AutomationProperty.LookupById (myHelpTextProperty.Id),
				"LookupById");
		}

		[Test]
		public void ClickablePointPropertyTest ()
		{
			AutomationProperty myClickablePointProperty =
				AutomationElementIdentifiers.ClickablePointProperty;
			Assert.IsNotNull (
				myClickablePointProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30014,
				myClickablePointProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.ClickablePointProperty",
				myClickablePointProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myClickablePointProperty,
				AutomationProperty.LookupById (myClickablePointProperty.Id),
				"LookupById");
		}

		[Test]
		public void CulturePropertyTest ()
		{
			AutomationProperty myCultureProperty =
				AutomationElementIdentifiers.CultureProperty;
			Assert.IsNotNull (
				myCultureProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30015,
				myCultureProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.CultureProperty",
				myCultureProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myCultureProperty,
				AutomationProperty.LookupById (myCultureProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsOffscreenPropertyTest ()
		{
			AutomationProperty myIsOffscreenProperty =
				AutomationElementIdentifiers.IsOffscreenProperty;
			Assert.IsNotNull (
				myIsOffscreenProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30022,
				myIsOffscreenProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsOffscreenProperty",
				myIsOffscreenProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsOffscreenProperty,
				AutomationProperty.LookupById (myIsOffscreenProperty.Id),
				"LookupById");
		}

		[Test]
		public void OrientationPropertyTest ()
		{
			AutomationProperty myOrientationProperty =
				AutomationElementIdentifiers.OrientationProperty;
			Assert.IsNotNull (
				myOrientationProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30023,
				myOrientationProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.OrientationProperty",
				myOrientationProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myOrientationProperty,
				AutomationProperty.LookupById (myOrientationProperty.Id),
				"LookupById");
		}

		[Test]
		public void FrameworkIdPropertyTest ()
		{
			AutomationProperty myFrameworkIdProperty =
				AutomationElementIdentifiers.FrameworkIdProperty;
			Assert.IsNotNull (
				myFrameworkIdProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30024,
				myFrameworkIdProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.FrameworkIdProperty",
				myFrameworkIdProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myFrameworkIdProperty,
				AutomationProperty.LookupById (myFrameworkIdProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsRequiredForFormPropertyTest ()
		{
			AutomationProperty myIsRequiredForFormProperty =
				AutomationElementIdentifiers.IsRequiredForFormProperty;
			Assert.IsNotNull (
				myIsRequiredForFormProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30025,
				myIsRequiredForFormProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsRequiredForFormProperty",
				myIsRequiredForFormProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsRequiredForFormProperty,
				AutomationProperty.LookupById (myIsRequiredForFormProperty.Id),
				"LookupById");
		}

		[Test]
		public void ItemStatusPropertyTest ()
		{
			AutomationProperty myItemStatusProperty =
				AutomationElementIdentifiers.ItemStatusProperty;
			Assert.IsNotNull (
				myItemStatusProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30026,
				myItemStatusProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.ItemStatusProperty",
				myItemStatusProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myItemStatusProperty,
				AutomationProperty.LookupById (myItemStatusProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsDockPatternAvailablePropertyTest ()
		{
			AutomationProperty myIsDockPatternAvailableProperty =
				AutomationElementIdentifiers.IsDockPatternAvailableProperty;
			Assert.IsNotNull (
				myIsDockPatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30027,
				myIsDockPatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsDockPatternAvailableProperty",
				myIsDockPatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsDockPatternAvailableProperty,
				AutomationProperty.LookupById (myIsDockPatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsExpandCollapsePatternAvailablePropertyTest ()
		{
			AutomationProperty myIsExpandCollapsePatternAvailableProperty =
				AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty;
			Assert.IsNotNull (
				myIsExpandCollapsePatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30028,
				myIsExpandCollapsePatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty",
				myIsExpandCollapsePatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsExpandCollapsePatternAvailableProperty,
				AutomationProperty.LookupById (myIsExpandCollapsePatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsGridItemPatternAvailablePropertyTest ()
		{
			AutomationProperty myIsGridItemPatternAvailableProperty =
				AutomationElementIdentifiers.IsGridItemPatternAvailableProperty;
			Assert.IsNotNull (
				myIsGridItemPatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30029,
				myIsGridItemPatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsGridItemPatternAvailableProperty",
				myIsGridItemPatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsGridItemPatternAvailableProperty,
				AutomationProperty.LookupById (myIsGridItemPatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsGridPatternAvailablePropertyTest ()
		{
			AutomationProperty myIsGridPatternAvailableProperty =
				AutomationElementIdentifiers.IsGridPatternAvailableProperty;
			Assert.IsNotNull (
				myIsGridPatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30030,
				myIsGridPatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsGridPatternAvailableProperty",
				myIsGridPatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsGridPatternAvailableProperty,
				AutomationProperty.LookupById (myIsGridPatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsInvokePatternAvailablePropertyTest ()
		{
			AutomationProperty myIsInvokePatternAvailableProperty =
				AutomationElementIdentifiers.IsInvokePatternAvailableProperty;
			Assert.IsNotNull (
				myIsInvokePatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30031,
				myIsInvokePatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsInvokePatternAvailableProperty",
				myIsInvokePatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsInvokePatternAvailableProperty,
				AutomationProperty.LookupById (myIsInvokePatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsMultipleViewPatternAvailablePropertyTest ()
		{
			AutomationProperty myIsMultipleViewPatternAvailableProperty =
				AutomationElementIdentifiers.IsMultipleViewPatternAvailableProperty;
			Assert.IsNotNull (
				myIsMultipleViewPatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30032,
				myIsMultipleViewPatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsMultipleViewPatternAvailableProperty",
				myIsMultipleViewPatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsMultipleViewPatternAvailableProperty,
				AutomationProperty.LookupById (myIsMultipleViewPatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsRangeValuePatternAvailablePropertyTest ()
		{
			AutomationProperty myIsRangeValuePatternAvailableProperty =
				AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty;
			Assert.IsNotNull (
				myIsRangeValuePatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30033,
				myIsRangeValuePatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty",
				myIsRangeValuePatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsRangeValuePatternAvailableProperty,
				AutomationProperty.LookupById (myIsRangeValuePatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsSelectionItemPatternAvailablePropertyTest ()
		{
			AutomationProperty myIsSelectionItemPatternAvailableProperty =
				AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty;
			Assert.IsNotNull (
				myIsSelectionItemPatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30036,
				myIsSelectionItemPatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty",
				myIsSelectionItemPatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsSelectionItemPatternAvailableProperty,
				AutomationProperty.LookupById (myIsSelectionItemPatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsSelectionPatternAvailablePropertyTest ()
		{
			AutomationProperty myIsSelectionPatternAvailableProperty =
				AutomationElementIdentifiers.IsSelectionPatternAvailableProperty;
			Assert.IsNotNull (
				myIsSelectionPatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30037,
				myIsSelectionPatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsSelectionPatternAvailableProperty",
				myIsSelectionPatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsSelectionPatternAvailableProperty,
				AutomationProperty.LookupById (myIsSelectionPatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsScrollPatternAvailablePropertyTest ()
		{
			AutomationProperty myIsScrollPatternAvailableProperty =
				AutomationElementIdentifiers.IsScrollPatternAvailableProperty;
			Assert.IsNotNull (
				myIsScrollPatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30034,
				myIsScrollPatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsScrollPatternAvailableProperty",
				myIsScrollPatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsScrollPatternAvailableProperty,
				AutomationProperty.LookupById (myIsScrollPatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsScrollItemPatternAvailablePropertyTest ()
		{
			AutomationProperty myIsScrollItemPatternAvailableProperty =
				AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty;
			Assert.IsNotNull (
				myIsScrollItemPatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30035,
				myIsScrollItemPatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty",
				myIsScrollItemPatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsScrollItemPatternAvailableProperty,
				AutomationProperty.LookupById (myIsScrollItemPatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsTablePatternAvailablePropertyTest ()
		{
			AutomationProperty myIsTablePatternAvailableProperty =
				AutomationElementIdentifiers.IsTablePatternAvailableProperty;
			Assert.IsNotNull (
				myIsTablePatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30038,
				myIsTablePatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsTablePatternAvailableProperty",
				myIsTablePatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsTablePatternAvailableProperty,
				AutomationProperty.LookupById (myIsTablePatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsTableItemPatternAvailablePropertyTest ()
		{
			AutomationProperty myIsTableItemPatternAvailableProperty =
				AutomationElementIdentifiers.IsTableItemPatternAvailableProperty;
			Assert.IsNotNull (
				myIsTableItemPatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30039,
				myIsTableItemPatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsTableItemPatternAvailableProperty",
				myIsTableItemPatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsTableItemPatternAvailableProperty,
				AutomationProperty.LookupById (myIsTableItemPatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsTextPatternAvailablePropertyTest ()
		{
			AutomationProperty myIsTextPatternAvailableProperty =
				AutomationElementIdentifiers.IsTextPatternAvailableProperty;
			Assert.IsNotNull (
				myIsTextPatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30040,
				myIsTextPatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsTextPatternAvailableProperty",
				myIsTextPatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsTextPatternAvailableProperty,
				AutomationProperty.LookupById (myIsTextPatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsTogglePatternAvailablePropertyTest ()
		{
			AutomationProperty myIsTogglePatternAvailableProperty =
				AutomationElementIdentifiers.IsTogglePatternAvailableProperty;
			Assert.IsNotNull (
				myIsTogglePatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30041,
				myIsTogglePatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsTogglePatternAvailableProperty",
				myIsTogglePatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsTogglePatternAvailableProperty,
				AutomationProperty.LookupById (myIsTogglePatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsTransformPatternAvailablePropertyTest ()
		{
			AutomationProperty myIsTransformPatternAvailableProperty =
				AutomationElementIdentifiers.IsTransformPatternAvailableProperty;
			Assert.IsNotNull (
				myIsTransformPatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30042,
				myIsTransformPatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsTransformPatternAvailableProperty",
				myIsTransformPatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsTransformPatternAvailableProperty,
				AutomationProperty.LookupById (myIsTransformPatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsValuePatternAvailablePropertyTest ()
		{
			AutomationProperty myIsValuePatternAvailableProperty =
				AutomationElementIdentifiers.IsValuePatternAvailableProperty;
			Assert.IsNotNull (
				myIsValuePatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30043,
				myIsValuePatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsValuePatternAvailableProperty",
				myIsValuePatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsValuePatternAvailableProperty,
				AutomationProperty.LookupById (myIsValuePatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void IsWindowPatternAvailablePropertyTest ()
		{
			AutomationProperty myIsWindowPatternAvailableProperty =
				AutomationElementIdentifiers.IsWindowPatternAvailableProperty;
			Assert.IsNotNull (
				myIsWindowPatternAvailableProperty,
				"Field must not be null.");
			Assert.AreEqual (
				30044,
				myIsWindowPatternAvailableProperty.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.IsWindowPatternAvailableProperty",
				myIsWindowPatternAvailableProperty.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsWindowPatternAvailableProperty,
				AutomationProperty.LookupById (myIsWindowPatternAvailableProperty.Id),
				"LookupById");
		}

		[Test]
		public void ToolTipOpenedEventTest ()
		{
			AutomationEvent myToolTipOpenedEvent =
				AutomationElementIdentifiers.ToolTipOpenedEvent;
			Assert.IsNotNull (
				myToolTipOpenedEvent,
				"Field must not be null.");
			Assert.AreEqual (
				20000,
				myToolTipOpenedEvent.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.ToolTipOpenedEvent",
				myToolTipOpenedEvent.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myToolTipOpenedEvent,
				AutomationEvent.LookupById (myToolTipOpenedEvent.Id),
				"LookupById");
		}

		[Test]
		public void ToolTipClosedEventTest ()
		{
			AutomationEvent myToolTipClosedEvent =
				AutomationElementIdentifiers.ToolTipClosedEvent;
			Assert.IsNotNull (
				myToolTipClosedEvent,
				"Field must not be null.");
			Assert.AreEqual (
				20001,
				myToolTipClosedEvent.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.ToolTipClosedEvent",
				myToolTipClosedEvent.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myToolTipClosedEvent,
				AutomationEvent.LookupById (myToolTipClosedEvent.Id),
				"LookupById");
		}

		[Test]
		public void StructureChangedEventTest ()
		{
			AutomationEvent myStructureChangedEvent =
				AutomationElementIdentifiers.StructureChangedEvent;
			Assert.IsNotNull (
				myStructureChangedEvent,
				"Field must not be null.");
			Assert.AreEqual (
				20002,
				myStructureChangedEvent.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.StructureChangedEvent",
				myStructureChangedEvent.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myStructureChangedEvent,
				AutomationEvent.LookupById (myStructureChangedEvent.Id),
				"LookupById");
		}

		[Test]
		public void MenuOpenedEventTest ()
		{
			AutomationEvent myMenuOpenedEvent =
				AutomationElementIdentifiers.MenuOpenedEvent;
			Assert.IsNotNull (
				myMenuOpenedEvent,
				"Field must not be null.");
			Assert.AreEqual (
				20003,
				myMenuOpenedEvent.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.MenuOpenedEvent",
				myMenuOpenedEvent.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myMenuOpenedEvent,
				AutomationEvent.LookupById (myMenuOpenedEvent.Id),
				"LookupById");
		}

		[Test]
		public void AutomationPropertyChangedEventTest ()
		{
			AutomationEvent myAutomationPropertyChangedEvent =
				AutomationElementIdentifiers.AutomationPropertyChangedEvent;
			Assert.IsNotNull (
				myAutomationPropertyChangedEvent,
				"Field must not be null.");
			Assert.AreEqual (
				20004,
				myAutomationPropertyChangedEvent.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.AutomationPropertyChangedEvent",
				myAutomationPropertyChangedEvent.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myAutomationPropertyChangedEvent,
				AutomationEvent.LookupById (myAutomationPropertyChangedEvent.Id),
				"LookupById");
		}

		[Test]
		public void AutomationFocusChangedEventTest ()
		{
			AutomationEvent myAutomationFocusChangedEvent =
				AutomationElementIdentifiers.AutomationFocusChangedEvent;
			Assert.IsNotNull (
				myAutomationFocusChangedEvent,
				"Field must not be null.");
			Assert.AreEqual (
				20005,
				myAutomationFocusChangedEvent.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.AutomationFocusChangedEvent",
				myAutomationFocusChangedEvent.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myAutomationFocusChangedEvent,
				AutomationEvent.LookupById (myAutomationFocusChangedEvent.Id),
				"LookupById");
		}

		[Test]
		public void AsyncContentLoadedEventTest ()
		{
			AutomationEvent myAsyncContentLoadedEvent =
				AutomationElementIdentifiers.AsyncContentLoadedEvent;
			Assert.IsNotNull (
				myAsyncContentLoadedEvent,
				"Field must not be null.");
			Assert.AreEqual (
				20006,
				myAsyncContentLoadedEvent.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.AsyncContentLoadedEvent",
				myAsyncContentLoadedEvent.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myAsyncContentLoadedEvent,
				AutomationEvent.LookupById (myAsyncContentLoadedEvent.Id),
				"LookupById");
		}

		[Test]
		public void MenuClosedEventTest ()
		{
			AutomationEvent myMenuClosedEvent =
				AutomationElementIdentifiers.MenuClosedEvent;
			Assert.IsNotNull (
				myMenuClosedEvent,
				"Field must not be null.");
			Assert.AreEqual (
				20007,
				myMenuClosedEvent.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.MenuClosedEvent",
				myMenuClosedEvent.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myMenuClosedEvent,
				AutomationEvent.LookupById (myMenuClosedEvent.Id),
				"LookupById");
		}

		[Test]
		public void LayoutInvalidatedEventTest ()
		{
			AutomationEvent myLayoutInvalidatedEvent =
				AutomationElementIdentifiers.LayoutInvalidatedEvent;
			Assert.IsNotNull (
				myLayoutInvalidatedEvent,
				"Field must not be null.");
			Assert.AreEqual (
				20008,
				myLayoutInvalidatedEvent.Id,
				"Id");
			Assert.AreEqual (
				"AutomationElementIdentifiers.LayoutInvalidatedEvent",
				myLayoutInvalidatedEvent.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myLayoutInvalidatedEvent,
				AutomationEvent.LookupById (myLayoutInvalidatedEvent.Id),
				"LookupById");
		}


	}
}

