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
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class AutomationElementTest : BaseTest
	{
		#region Test Methods

		[Test]
		public void AcceleratorKeyTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.AcceleratorKeyProperty),
				button1Element.Current.AcceleratorKey,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");
			// TODO: Complete actual test (mostly unimplemented in Linux right now)
		}

		[Test]
		public void AccessKeyTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.AccessKeyProperty),
				button1Element.Current.AccessKey,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.AreEqual ("Alt+u",
				button1Element.Current.AccessKey,
				"button1");
			Assert.AreEqual (String.Empty,
				button2Element.Current.AccessKey,
				"button2");
		}

		[Test]
		public void AutomationIdTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.AutomationIdProperty),
				button1Element.Current.AutomationId,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");
			// TODO: Complete actual test (implemented incorrectly in Linux right now)
		}

		[Test]
		public void BoundingRectangleTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.BoundingRectangleProperty),
				button1Element.Current.BoundingRectangle,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			// TODO: Coordinates, not just size (not matching in Linux yet)
			Rect button2Rect = button2Element.Current.BoundingRectangle;
			Assert.AreEqual (128,
				button2Rect.Width,
				"button2 width");
			Assert.AreEqual (23,
				button2Rect.Height,
				"button2 height");

			Rect button3Rect = button3Element.Current.BoundingRectangle;
			Assert.AreEqual (75,
				button3Rect.Width,
				"button3 width");
			Assert.AreEqual (23,
				button3Rect.Height,
				"button3 height");
		}

		[Test]
		public void ClassNameTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.ClassNameProperty),
				button1Element.Current.ClassName,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");
			// TODO: Complete actual test (totally unimplemented in Linux right now)
		}

		[Test]
		public void ControlTypeTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.ControlTypeProperty),
				button1Element.Current.ControlType,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.AreEqual (ControlType.Window,
				testFormElement.Current.ControlType,
				"window");
			Assert.AreEqual (ControlType.Button,
				button3Element.Current.ControlType,
				"button3");
		}

		[Test]
		[Ignore ("Not implemented, not sure how to test on Windows")]
		public void CultureTest ()
		{
			// TODO: How to test actual values in Windows?
			//Assert.AreEqual (new CultureInfo ("en-US"),
			//        testFormElement.GetCurrentPropertyValue (AEIds.CultureProperty),
			//        "button1 Culture");
		}

		[Test]
		public void FrameworkIdTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.FrameworkIdProperty),
				button1Element.Current.FrameworkId,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.AreEqual ("WinForm",
				testFormElement.Current.FrameworkId,
				"window");
			Assert.AreEqual ("WinForm",
				label1Element.Current.FrameworkId,
				"label1");
		}

		[Test]
		public void GetCurrentPropertyValueTest ()
		{
			// A typical supported property
			VerifyCurrentPropertyValue (testFormElement,
				AEIds.NameProperty,
				"TestForm1",
				"TestForm1",
				"TestForm1");

			// A typical unsupported property
			VerifyCurrentPropertyValue (testFormElement,
				AEIds.OrientationProperty,
				AutomationElement.NotSupported,
				OrientationType.None,
				OrientationType.None);

			// TODO: Additional things to test (when pattern support implemented)
			//	2. Supported pattern props, true/false/default
			//	3. Unsupported pattern proprs, true/false/default
		}

		[Test]
		public void HasKeyboardFocusTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.HasKeyboardFocusProperty),
				button1Element.Current.HasKeyboardFocus,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			button2Element.SetFocus ();
			Thread.Sleep (100);
			Assert.IsFalse (button1Element.Current.HasKeyboardFocus, "button1, no focus");
			button1Element.SetFocus ();
			Thread.Sleep (100);
			Assert.IsTrue (button1Element.Current.HasKeyboardFocus, "button1, w/ focus");
		}

		[Test]
		public void HelpTextTest ()
		{
			Assert.AreEqual (button3Element.GetCurrentPropertyValue (AEIds.HelpTextProperty),
				button3Element.Current.HelpText,
				"button3 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.AreEqual ("help text 3",
				button3Element.Current.HelpText,
				"button3");
			Assert.AreEqual (String.Empty,
				button1Element.Current.HelpText,
				"button1");
		}

		[Test]
		public void IsContentTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.IsContentElementProperty),
				button1Element.Current.IsContentElement,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.IsTrue (testFormElement.Current.IsContentElement,
				"window");
			Assert.IsFalse (label1Element.Current.IsContentElement,
				"label1");
		}

		[Test]
		public void IsControlTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.IsControlElementProperty),
				button1Element.Current.IsControlElement,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.IsTrue (testFormElement.Current.IsControlElement,
				"window");
			Assert.IsTrue (label1Element.Current.IsControlElement,
				"label1");
		}

		[Test]
		public void IsEnabledTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.IsEnabledProperty),
				button1Element.Current.IsEnabled,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.IsTrue (testFormElement.Current.IsEnabled,
				"window");
			Assert.IsTrue (label1Element.Current.IsEnabled,
				"label1");
			Assert.IsTrue (groupBox1Element.Current.IsEnabled,
				"groupBox1");
			Assert.IsTrue (button1Element.Current.IsEnabled,
				"button1");
			Assert.IsTrue (button2Element.Current.IsEnabled,
				"button2");
			Assert.IsFalse (button3Element.Current.IsEnabled,
				"button3");
		}

		[Test]
		public void IsKeyboardFocusableTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.IsKeyboardFocusableProperty),
				button1Element.Current.IsKeyboardFocusable,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.IsTrue (testFormElement.Current.IsKeyboardFocusable,
				"window");
			Assert.IsFalse (label1Element.Current.IsKeyboardFocusable,
				"label1");
			Assert.IsFalse (groupBox1Element.Current.IsKeyboardFocusable,
				"groupBox1");
			Assert.IsTrue (button1Element.Current.IsKeyboardFocusable,
				"button1");
			Assert.IsTrue (button2Element.Current.IsKeyboardFocusable,
				"button2");
			Assert.IsFalse (button3Element.Current.IsKeyboardFocusable,
				"button3");
		}

		[Test]
		public void IsOffscreenTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.IsOffscreenProperty),
				button1Element.Current.IsOffscreen,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");
			// TODO: Complete actual test (requires pattern support to move window)
		}

		[Test]
		public void IsPasswordTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.IsPasswordProperty),
				button1Element.Current.IsPassword,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.IsFalse (textbox1Element.Current.IsPassword,
				"textbox1");
			Assert.IsTrue (textbox2Element.Current.IsPassword,
				"textbox2");
		}

		[Test]
		public void IsRequiredForFormTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.IsRequiredForFormProperty),
				button1Element.Current.IsRequiredForForm,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");
			// TODO: Complete actual test (totally unimplemented in Linux right now)
		}

		[Test]
		public void ItemStatusTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.ItemStatusProperty),
				button1Element.Current.ItemStatus,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");
			// TODO: Complete actual test (totally unimplemented in Linux right now)
		}

		[Test]
		public void ItemTypeTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.ItemTypeProperty),
				button1Element.Current.ItemType,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");
			// TODO: Complete actual test (totally unimplemented in Linux right now)
		}

		[Test]
		public void LabeledByTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.LabeledByProperty),
				button1Element.Current.LabeledBy,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.AreEqual (label1Element,
				button1Element.Current.LabeledBy,
				"button1");
			Assert.AreEqual (null,
				button2Element.Current.LabeledBy,
				"button2");
			Assert.AreEqual (null,
				textbox1Element.Current.LabeledBy,
				"textbox1");
			Assert.AreEqual (null,
				textbox2Element.Current.LabeledBy,
				"textbox2Element");
		}

		[Test]
		public void LocalizedControlTypeTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.LocalizedControlTypeProperty),
				button1Element.Current.LocalizedControlType,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.AreEqual (ControlType.Window.LocalizedControlType,
				testFormElement.Current.LocalizedControlType,
				"window");
			Assert.AreEqual (ControlType.Button.LocalizedControlType,
				button3Element.Current.LocalizedControlType,
				"button3");
		}

		[Test]
		public void NameTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.NameProperty),
				button1Element.Current.Name,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.AreEqual ("TestForm1",
				testFormElement.Current.Name,
				"window");
			Assert.AreEqual ("groupBox1",
				groupBox1Element.Current.Name,
				"groupBox1");
			Assert.AreEqual ("button1",
				button1Element.Current.Name,
				"button1");
			Assert.AreEqual (label1Element.Current.Name,
				textbox1Element.Current.Name,
				"textbox1");
			Assert.AreEqual (String.Empty,
				textbox2Element.Current.Name,
				"textbox2");
		}

		[Test]
		public void NativeWindowHandleTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.NativeWindowHandleProperty),
				button1Element.Current.NativeWindowHandle,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.AreNotEqual (testFormElement.Current.NativeWindowHandle,
				label1Element.Current.NativeWindowHandle,
				"Control handles should differ");
		}

		[Test]
		public void OrientationTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.OrientationProperty),
				button1Element.Current.Orientation,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.AreEqual (OrientationType.None,
				testFormElement.Current.Orientation,
				"Orientation of Window (unsupported)");

			Assert.AreEqual (tb3verticalScrollBarElement.GetCurrentPropertyValue (AEIds.OrientationProperty),
				tb3verticalScrollBarElement.Current.Orientation,
				"tb3verticalScrollBarElement AutomationElementInformation vs GetCurrentPropertyValue");
			Assert.AreEqual (OrientationType.Vertical,
				tb3verticalScrollBarElement.Current.Orientation,
				"Orientation of vertical scroll bar");

			Assert.AreEqual (tb3horizontalScrollBarElement.GetCurrentPropertyValue (AEIds.OrientationProperty),
				tb3horizontalScrollBarElement.Current.Orientation,
				"tb3horizontalScrollBarElement AutomationElementInformation vs GetCurrentPropertyValue");
			Assert.AreEqual (OrientationType.Horizontal,
				tb3horizontalScrollBarElement.Current.Orientation,
				"Orientation of horizontal scroll bar");
		}

		[Test]
		public void ProcessIdTest ()
		{
			Assert.AreEqual (button1Element.GetCurrentPropertyValue (AEIds.ProcessIdProperty),
				button1Element.Current.ProcessId,
				"button1 AutomationElementInformation vs GetCurrentPropertyValue");

			Assert.AreEqual (p.Id,
				testFormElement.Current.ProcessId,
				"Form");
			Assert.AreEqual (p.Id,
				label1Element.Current.ProcessId,
				"label");
		}

		[Test]
		public void DockPositionTest ()
		{
			Assert.AreEqual (DockPosition.None, (DockPosition)AutomationElement.RootElement.GetCurrentPropertyValue (DockPatternIdentifiers.DockPositionProperty), "DockPosition of root element (unsupported)");
		}

		[Test]
		public void ExpandCollapseStateTest ()
		{
			Assert.AreEqual (ExpandCollapseState.LeafNode, (ExpandCollapseState)AutomationElement.RootElement.GetCurrentPropertyValue (ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty), "ExpandCollapseState of root element (unsupported)");
		}

		[Test]
		public void ColumnTest ()
		{
			Assert.AreEqual (0, AutomationElement.RootElement.GetCurrentPropertyValue (GridItemPatternIdentifiers.ColumnProperty), "Column of root element (unsupported)");
		}

		[Test]
		public void ColumnSpanTest ()
		{
			Assert.AreEqual (1, AutomationElement.RootElement.GetCurrentPropertyValue (GridItemPatternIdentifiers.ColumnSpanProperty), "ColumnSpan of root element (unsupported)");
		}

		[Test]
		public void ContainingGridTest ()
		{
			Assert.IsNull (AutomationElement.RootElement.GetCurrentPropertyValue (GridItemPatternIdentifiers.ContainingGridProperty), "ContainingGrid of root element (unsupported)");
		}

		[Test]
		public void RowTest ()
		{
			Assert.AreEqual (0, AutomationElement.RootElement.GetCurrentPropertyValue (GridItemPatternIdentifiers.RowProperty), "Row of root element (unsupported)");
		}

		[Test]
		public void RowSpanTest ()
		{
			Assert.AreEqual (1, AutomationElement.RootElement.GetCurrentPropertyValue (GridItemPatternIdentifiers.RowSpanProperty), "RowSpan of root element (unsupported)");
		}

		[Test]
		public void ColumnCountTest ()
		{
			Assert.AreEqual (0, AutomationElement.RootElement.GetCurrentPropertyValue (GridPatternIdentifiers.ColumnCountProperty), "ColumnCount of root element (unsupported)");
		}

		[Test]
		public void RowCountTest ()
		{
			Assert.AreEqual (0, AutomationElement.RootElement.GetCurrentPropertyValue (GridPatternIdentifiers.RowCountProperty), "RowCount of root element (unsupported)");
		}

		[Test]
		public void CurrentViewTest ()
		{
			Assert.AreEqual (0, AutomationElement.RootElement.GetCurrentPropertyValue (MultipleViewPatternIdentifiers.CurrentViewProperty), "CurrentView of root element (unsupported)");
		}

		[Test]
		public void SupportedViewsTest ()
		{
			Assert.AreEqual (new int [0], AutomationElement.RootElement.GetCurrentPropertyValue (MultipleViewPatternIdentifiers.SupportedViewsProperty), "SupportedViews of root element (unsupported)");
		}

		[Test]
		public void RangeValueIsReadOnlyTest ()
		{
			Assert.IsTrue ((bool)AutomationElement.RootElement.GetCurrentPropertyValue (RangeValuePatternIdentifiers.IsReadOnlyProperty), "IsReadOnly of root element (unsupported)");
		}

		[Test]
		public void LargeChangeTest ()
		{
			Assert.AreEqual (0, AutomationElement.RootElement.GetCurrentPropertyValue (RangeValuePatternIdentifiers.LargeChangeProperty), "LargeChange of root element (unsupported)");
		}

		[Test]
		public void MaximumTest ()
		{
			Assert.AreEqual (0, AutomationElement.RootElement.GetCurrentPropertyValue (RangeValuePatternIdentifiers.MaximumProperty), "Maximum of root element (unsupported)");
		}

		[Test]
		public void MinimumTest ()
		{
			Assert.AreEqual (0, AutomationElement.RootElement.GetCurrentPropertyValue (RangeValuePatternIdentifiers.MinimumProperty), "Minimum of root element (unsupported)");
		}

		[Test]
		public void SmallChangeTest ()
		{
			Assert.AreEqual (0, AutomationElement.RootElement.GetCurrentPropertyValue (RangeValuePatternIdentifiers.SmallChangeProperty), "SmallChange of root element (unsupported)");
		}

		[Test]
		public void RangeValueValueTest ()
		{
			Assert.AreEqual (0, AutomationElement.RootElement.GetCurrentPropertyValue (RangeValuePatternIdentifiers.ValueProperty), "Value of root element (unsupported)");
		}

		[Test]
		public void HorizontallyScrollableTest ()
		{
			Assert.IsFalse ((bool)AutomationElement.RootElement.GetCurrentPropertyValue (ScrollPatternIdentifiers.HorizontallyScrollableProperty), "HorizontallyScrollable of root element (unsupported)");
		}

		[Test]
		public void HorizontalScrollPercentTest ()
		{
			Assert.AreEqual (0, AutomationElement.RootElement.GetCurrentPropertyValue (ScrollPatternIdentifiers.HorizontalScrollPercentProperty), "HorizontalScrollPercent of root element (unsupported)");
		}

		[Test]
		public void HorizontalViewSizeTest ()
		{
			Assert.AreEqual (100, AutomationElement.RootElement.GetCurrentPropertyValue (ScrollPatternIdentifiers.HorizontalViewSizeProperty), "HorizontalViewSize of root element (unsupported)");
		}

		[Test]
		public void VerticallyScrollableTest ()
		{
			Assert.IsFalse ((bool)AutomationElement.RootElement.GetCurrentPropertyValue (ScrollPatternIdentifiers.VerticallyScrollableProperty), "VerticallyScrollable of root element (unsupported)");
		}

		[Test]
		public void VerticalScrollPercentTest ()
		{
			Assert.AreEqual (0, AutomationElement.RootElement.GetCurrentPropertyValue (ScrollPatternIdentifiers.VerticalScrollPercentProperty), "VerticalScrollPercent of root element (unsupported)");
		}

		[Test]
		public void VerticalViewSizeTest ()
		{
			Assert.AreEqual (100, AutomationElement.RootElement.GetCurrentPropertyValue (ScrollPatternIdentifiers.VerticalViewSizeProperty), "VerticalViewSize of root element (unsupported)");
		}

		[Test]
		public void IsSelectedTest ()
		{
			Assert.IsFalse ((bool)AutomationElement.RootElement.GetCurrentPropertyValue (SelectionItemPatternIdentifiers.IsSelectedProperty), "IsSelected of root element (unsupported)");
		}

		[Test]
		public void SelectionContainerTest ()
		{
			Assert.IsNull (AutomationElement.RootElement.GetCurrentPropertyValue (SelectionItemPatternIdentifiers.SelectionContainerProperty), "SelectionContainer of root element (unsupported)");
		}

		[Test]
		public void CanSelectMultipleTest ()
		{
			Assert.IsFalse ((bool)AutomationElement.RootElement.GetCurrentPropertyValue (SelectionPatternIdentifiers.CanSelectMultipleProperty), "CanSelectMultiple of root element (unsupported)");
		}

		[Test]
		public void IsSelectionRequiredTest ()
		{
			Assert.IsFalse ((bool)AutomationElement.RootElement.GetCurrentPropertyValue (SelectionPatternIdentifiers.IsSelectionRequiredProperty), "IsSelectionRequired of root element (unsupported)");
		}

		[Test]
		public void SelectionTest ()
		{
			Assert.AreEqual (new AutomationElement [0], AutomationElement.RootElement.GetCurrentPropertyValue (SelectionPatternIdentifiers.SelectionProperty), "Selection of root element (unsupported)");
		}

		[Test]
		public void ColumnHeaderItemsTest ()
		{
			Assert.AreEqual (new AutomationElement [0], AutomationElement.RootElement.GetCurrentPropertyValue (TableItemPatternIdentifiers.ColumnHeaderItemsProperty), "ColumnHeaderItems of root element (unsupported)");
		}

		[Test]
		public void RowHeaderItemsTest ()
		{
			Assert.AreEqual (new AutomationElement [0], AutomationElement.RootElement.GetCurrentPropertyValue (TableItemPatternIdentifiers.RowHeaderItemsProperty), "ColumnHeaderItems of root element (unsupported)");
		}

		[Test]
		public void ColumnHeadersTest ()
		{
			Assert.AreEqual (new AutomationElement [0], AutomationElement.RootElement.GetCurrentPropertyValue (TablePatternIdentifiers.ColumnHeadersProperty), "ColumnHeaderItems of root element (unsupported)");
		}

		[Test]
		public void RowHeadersTest ()
		{
			Assert.AreEqual (new AutomationElement [0], AutomationElement.RootElement.GetCurrentPropertyValue (TablePatternIdentifiers.RowHeadersProperty), "RowHeaders of root element (unsupported)");
		}

		[Test]
		public void RowOrColumnMajorTest ()
		{
			Assert.AreEqual (RowOrColumnMajor.Indeterminate, AutomationElement.RootElement.GetCurrentPropertyValue (TablePatternIdentifiers.RowOrColumnMajorProperty), "RowOrColumnMajor of root element (unsupported)");
		}

		[Test]
		public void ToggleStateTest ()
		{
			Assert.AreEqual (ToggleState.Indeterminate, AutomationElement.RootElement.GetCurrentPropertyValue (TogglePatternIdentifiers.ToggleStateProperty), "ToggleState of root element (unsupported)");
		}

		[Test]
		public void CanMoveTest ()
		{
			Assert.IsFalse ((bool)AutomationElement.RootElement.GetCurrentPropertyValue (TransformPatternIdentifiers.CanMoveProperty), "CanMove of root element (unsupported)");
		}

		[Test]
		public void CanResizeTest ()
		{
			Assert.IsFalse ((bool)AutomationElement.RootElement.GetCurrentPropertyValue (TransformPatternIdentifiers.CanResizeProperty), "CanResize of root element (unsupported)");
		}

		[Test]
		public void ValueIsReadOnlyTest ()
		{
			Assert.IsTrue ((bool)AutomationElement.RootElement.GetCurrentPropertyValue (ValuePatternIdentifiers.IsReadOnlyProperty), "IsReadOnly of root element (unsupported)");
		}

		[Test]
		public void ValueValueTest ()
		{
			Assert.AreEqual (String.Empty, AutomationElement.RootElement.GetCurrentPropertyValue (ValuePatternIdentifiers.ValueProperty), "Value of root element (unsupported)");
		}

		[Test]
		public void CanMaximizeTest ()
		{
			Assert.IsFalse ((bool)AutomationElement.RootElement.GetCurrentPropertyValue (WindowPatternIdentifiers.CanMaximizeProperty), "CanMaximize of root element (unsupported)");
		}

		[Test]
		public void CanMinimizeTest ()
		{
			Assert.IsFalse ((bool)AutomationElement.RootElement.GetCurrentPropertyValue (WindowPatternIdentifiers.CanMinimizeProperty), "CanMinimize of root element (unsupported)");
		}

		[Test]
		public void IsModalTest ()
		{
			Assert.IsFalse ((bool)AutomationElement.RootElement.GetCurrentPropertyValue (WindowPatternIdentifiers.IsModalProperty), "IsModal of root element (unsupported)");
		}

		[Test]
		public void IsTopmostTest ()
		{
			Assert.IsFalse ((bool)AutomationElement.RootElement.GetCurrentPropertyValue (WindowPatternIdentifiers.IsTopmostProperty), "IsTopmost of root element (unsupported)");
		}

		[Test]
		public void WindowInteractionStateTest ()
		{
			Assert.AreEqual (WindowInteractionState.Running, AutomationElement.RootElement.GetCurrentPropertyValue (WindowPatternIdentifiers.WindowInteractionStateProperty), "WindowInteractionState of root element (unsupported)");
		}

		[Test]
		public void WindowVisualStateTest ()
		{
			Assert.AreEqual (WindowVisualState.Normal, AutomationElement.RootElement.GetCurrentPropertyValue (WindowPatternIdentifiers.WindowVisualStateProperty), "WindowVisualState of root element (unsupported)");
		}

		[Test]
		public void CanRotateTest ()
		{
			Assert.IsFalse ((bool)AutomationElement.RootElement.GetCurrentPropertyValue (TransformPatternIdentifiers.CanRotateProperty), "CanRotate of root element (unsupported)");
		}

		[Test]
		public void EqualsTest ()
		{
			AutomationElement button1ElementRef2 = testFormElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.Button));
			Assert.IsTrue (button1Element.Equals (button1ElementRef2), "Comparing two references to button1 element");
			Assert.IsFalse (button1Element.Equals (button2Element), "Comparing button1 and button2 elements");
			// TODO: Compare after modifying, ie references have different cached info values
		}

		[Test]
		public void FindFirstTest ()
		{
			// TreeScope.Children
			VerifyFindFirst (testFormElement,
				TreeScope.Children,
				new AutomationElement [] { button1Element, label1Element },
				new AutomationElement [] { testFormElement, button3Element });
			VerifyFindFirst (groupBox1Element,
				TreeScope.Children,
				new AutomationElement [] { button3Element },
				new AutomationElement [] { groupBox1Element, button1Element });

			// TreeScope.Descendants
			VerifyFindFirst (testFormElement,
				TreeScope.Descendants,
				new AutomationElement [] { button1Element, label1Element, button3Element },
				new AutomationElement [] { testFormElement });
			VerifyFindFirst (groupBox1Element,
				TreeScope.Descendants,
				new AutomationElement [] { button3Element },
				new AutomationElement [] { groupBox1Element, button1Element });

			// TreeScope.Subtree
			VerifyFindFirst (testFormElement,
				TreeScope.Subtree,
				new AutomationElement [] { button1Element, label1Element, testFormElement, button3Element },
				new AutomationElement [] { });
			VerifyFindFirst (groupBox1Element,
				TreeScope.Subtree,
				new AutomationElement [] { button3Element, groupBox1Element },
				new AutomationElement [] { button1Element });

			// TreeScope.Ancestors
			bool exceptionRaised = false;
			try {
				groupBox1Element.FindFirst (TreeScope.Ancestors, new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Window));
			} catch (ArgumentException) {
				exceptionRaised = true;
			}
			Assert.IsTrue (exceptionRaised, "Expected ArgumentException when using TreeScope.Ancestors");

			// TreeScope.Parent
			exceptionRaised = false;
			try {
				groupBox1Element.FindFirst (TreeScope.Parent, new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Window));
			} catch (ArgumentException) {
				exceptionRaised = true;
			}
			Assert.IsTrue (exceptionRaised, "Expected ArgumentException when using TreeScope.Parent");

			// Test search order, conditions (also tested in FindAllTest and FixtureSetUp)
			AutomationElement firstFound =
				groupBox1Element.FindFirst (TreeScope.Descendants,
				new AndCondition (new PropertyCondition (AEIds.OrientationProperty, OrientationType.None),
					new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button)));
			Assert.AreEqual (button7Element, firstFound, "In Descendants, first element added found first. " +
				String.Format ("Expected element named {0}, got element named {1}", button7Element.Current.Name, firstFound.Current.Name));
			firstFound =
				groupBox1Element.FindFirst (TreeScope.Subtree,
				new PropertyCondition (AEIds.OrientationProperty, OrientationType.None));
			Assert.AreEqual (groupBox1Element, firstFound, "In Subtree, root found before descendants");
		}

		[Test]
		public void FindAllTest ()
		{
			VerifyFindAll (testFormElement,
				TreeScope.Descendants,
				new PropertyCondition (AutomationElement.ControlTypeProperty, ControlType.Button),
				new AutomationElement [] { button1Element, button2Element, button3Element });
			VerifyFindAll (testFormElement,
				TreeScope.Children,
				new PropertyCondition (AutomationElement.ControlTypeProperty, ControlType.Button),
				new AutomationElement [] { button1Element });

			// TreeScope.Ancestors
			bool exceptionRaised = false;
			try {
				groupBox1Element.FindAll (TreeScope.Ancestors, new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Window));
			} catch (ArgumentException) {
				exceptionRaised = true;
			}
			Assert.IsTrue (exceptionRaised, "Expected ArgumentException when using TreeScope.Ancestors");

			// TreeScope.Parent
			exceptionRaised = false;
			try {
				groupBox1Element.FindAll (TreeScope.Parent, new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Window));
			} catch (ArgumentException) {
				exceptionRaised = true;
			}
			Assert.IsTrue (exceptionRaised, "Expected ArgumentException when using TreeScope.Parent");
		}

		[Test]
		public void StaticFieldsTest ()
		{
			Assert.IsTrue (AEIds.AcceleratorKeyProperty == AutomationElement.AcceleratorKeyProperty, "AcceleratorKeyProperty");
			Assert.IsTrue (AEIds.AccessKeyProperty == AutomationElement.AccessKeyProperty, "AccessKeyProperty");
			Assert.IsTrue (AEIds.AutomationIdProperty == AutomationElement.AutomationIdProperty, "AutomationIdProperty");
			Assert.IsTrue (AEIds.BoundingRectangleProperty == AutomationElement.BoundingRectangleProperty, "BoundingRectangleProperty");
			Assert.IsTrue (AEIds.ClassNameProperty == AutomationElement.ClassNameProperty, "ClassNameProperty");
			Assert.IsTrue (AEIds.ClickablePointProperty == AutomationElement.ClickablePointProperty, "ClickablePointProperty");
			Assert.IsTrue (AEIds.ControlTypeProperty == AutomationElement.ControlTypeProperty, "ControlTypeProperty");
			Assert.IsTrue (AEIds.CultureProperty == AutomationElement.CultureProperty, "CultureProperty");
			Assert.IsTrue (AEIds.FrameworkIdProperty == AutomationElement.FrameworkIdProperty, "FrameworkIdProperty");
			Assert.IsTrue (AEIds.HasKeyboardFocusProperty == AutomationElement.HasKeyboardFocusProperty, "HasKeyboardFocusProperty");
			Assert.IsTrue (AEIds.HelpTextProperty == AutomationElement.HelpTextProperty, "HelpTextProperty");
			Assert.IsTrue (AEIds.IsContentElementProperty == AutomationElement.IsContentElementProperty, "IsContentElementProperty");
			Assert.IsTrue (AEIds.IsControlElementProperty == AutomationElement.IsControlElementProperty, "IsControlElementProperty");
			Assert.IsTrue (AEIds.IsDockPatternAvailableProperty == AutomationElement.IsDockPatternAvailableProperty, "IsDockPatternAvailableProperty");
			Assert.IsTrue (AEIds.IsEnabledProperty == AutomationElement.IsEnabledProperty, "IsEnabledProperty");
			Assert.IsTrue (AEIds.IsExpandCollapsePatternAvailableProperty == AutomationElement.IsExpandCollapsePatternAvailableProperty, "IsExpandCollapsePatternAvailableProperty");
			Assert.IsTrue (AEIds.IsGridItemPatternAvailableProperty == AutomationElement.IsGridItemPatternAvailableProperty, "IsGridItemPatternAvailableProperty");
			Assert.IsTrue (AEIds.IsGridPatternAvailableProperty == AutomationElement.IsGridPatternAvailableProperty, "IsGridPatternAvailableProperty");
			Assert.IsTrue (AEIds.IsInvokePatternAvailableProperty == AutomationElement.IsInvokePatternAvailableProperty, "IsInvokePatternAvailableProperty");
			Assert.IsTrue (AEIds.IsKeyboardFocusableProperty == AutomationElement.IsKeyboardFocusableProperty, "IsKeyboardFocusableProperty");
			Assert.IsTrue (AEIds.IsMultipleViewPatternAvailableProperty == AutomationElement.IsMultipleViewPatternAvailableProperty, "IsMultipleViewPatternAvailableProperty");
			Assert.IsTrue (AEIds.IsOffscreenProperty == AutomationElement.IsOffscreenProperty, "IsOffscreenProperty");
			Assert.IsTrue (AEIds.IsPasswordProperty == AutomationElement.IsPasswordProperty, "IsPasswordProperty");
			Assert.IsTrue (AEIds.IsRangeValuePatternAvailableProperty == AutomationElement.IsRangeValuePatternAvailableProperty, "IsRangeValuePatternAvailableProperty");
			Assert.IsTrue (AEIds.IsRequiredForFormProperty == AutomationElement.IsRequiredForFormProperty, "IsRequiredForFormProperty");
			Assert.IsTrue (AEIds.IsScrollItemPatternAvailableProperty == AutomationElement.IsScrollItemPatternAvailableProperty, "IsScrollItemPatternAvailableProperty");
			Assert.IsTrue (AEIds.IsScrollPatternAvailableProperty == AutomationElement.IsScrollPatternAvailableProperty, "IsScrollPatternAvailableProperty");
			Assert.IsTrue (AEIds.IsSelectionItemPatternAvailableProperty == AutomationElement.IsSelectionItemPatternAvailableProperty, "IsSelectionItemPatternAvailableProperty");
			Assert.IsTrue (AEIds.IsSelectionPatternAvailableProperty == AutomationElement.IsSelectionPatternAvailableProperty, "IsSelectionPatternAvailableProperty");
			Assert.IsTrue (AEIds.IsTableItemPatternAvailableProperty == AutomationElement.IsTableItemPatternAvailableProperty, "IsTableItemPatternAvailableProperty");
			Assert.IsTrue (AEIds.IsTablePatternAvailableProperty == AutomationElement.IsTablePatternAvailableProperty, "IsTablePatternAvailableProperty");
			Assert.IsTrue (AEIds.IsTextPatternAvailableProperty == AutomationElement.IsTextPatternAvailableProperty, "IsTextPatternAvailableProperty");
			Assert.IsTrue (AEIds.IsTogglePatternAvailableProperty == AutomationElement.IsTogglePatternAvailableProperty, "IsTogglePatternAvailableProperty");
			Assert.IsTrue (AEIds.IsTransformPatternAvailableProperty == AutomationElement.IsTransformPatternAvailableProperty, "IsTransformPatternAvailableProperty");
			Assert.IsTrue (AEIds.IsValuePatternAvailableProperty == AutomationElement.IsValuePatternAvailableProperty, "IsValuePatternAvailableProperty");
			Assert.IsTrue (AEIds.IsWindowPatternAvailableProperty == AutomationElement.IsWindowPatternAvailableProperty, "IsWindowPatternAvailableProperty");
			Assert.IsTrue (AEIds.ItemStatusProperty == AutomationElement.ItemStatusProperty, "ItemStatusProperty");
			Assert.IsTrue (AEIds.ItemTypeProperty == AutomationElement.ItemTypeProperty, "ItemTypeProperty");
			Assert.IsTrue (AEIds.LabeledByProperty == AutomationElement.LabeledByProperty, "LabeledByProperty");
			Assert.IsTrue (AEIds.LocalizedControlTypeProperty == AutomationElement.LocalizedControlTypeProperty, "LocalizedControlTypeProperty");
			Assert.IsTrue (AEIds.NameProperty == AutomationElement.NameProperty, "NameProperty");
			Assert.IsTrue (AEIds.NativeWindowHandleProperty == AutomationElement.NativeWindowHandleProperty, "NativeWindowHandleProperty");
			Assert.IsTrue (AEIds.OrientationProperty == AutomationElement.OrientationProperty, "OrientationProperty");
			Assert.IsTrue (AEIds.ProcessIdProperty == AutomationElement.ProcessIdProperty, "ProcessIdProperty");
			Assert.IsTrue (AEIds.RuntimeIdProperty == AutomationElement.RuntimeIdProperty, "RuntimeIdProperty");

			Assert.IsTrue (AEIds.AsyncContentLoadedEvent == AutomationElement.AsyncContentLoadedEvent, "AsyncContentLoadedEvent");
			Assert.IsTrue (AEIds.AutomationFocusChangedEvent == AutomationElement.AutomationFocusChangedEvent, "AutomationFocusChangedEvent");
			Assert.IsTrue (AEIds.AutomationPropertyChangedEvent == AutomationElement.AutomationPropertyChangedEvent, "AutomationPropertyChangedEvent");
			Assert.IsTrue (AEIds.LayoutInvalidatedEvent == AutomationElement.LayoutInvalidatedEvent, "LayoutInvalidatedEvent");
			Assert.IsTrue (AEIds.MenuClosedEvent == AutomationElement.MenuClosedEvent, "MenuClosedEvent");
			Assert.IsTrue (AEIds.MenuOpenedEvent == AutomationElement.MenuOpenedEvent, "MenuOpenedEvent");
			Assert.IsTrue (AEIds.StructureChangedEvent == AutomationElement.StructureChangedEvent, "StructureChangedEvent");
			Assert.IsTrue (AEIds.ToolTipClosedEvent == AutomationElement.ToolTipClosedEvent, "ToolTipClosedEvent");
			Assert.IsTrue (AEIds.ToolTipOpenedEvent == AutomationElement.ToolTipOpenedEvent, "ToolTipOpenedEvent");

			Assert.IsTrue (AEIds.NotSupported == AutomationElement.NotSupported, "NotSupported");
		}

		[Test]
		public void RootElementTest ()
		{
			Assert.AreEqual (String.Empty,
			                 AutomationElement.RootElement.Current.AcceleratorKey,
			                 "AcceleratorKey");
			Assert.AreEqual (String.Empty,
			                 AutomationElement.RootElement.Current.AccessKey,
			                 "AccessKey");
			Assert.AreEqual (String.Empty,
			                 AutomationElement.RootElement.Current.AutomationId,
			                 "AutomationId");
			//Assert.AreEqual (String.Empty, AutomationElement.RootElement.Current.BoundingRectangle, "BoundingRectangle"); // TODO
			//Assert.AreEqual ("#32769", AutomationElement.RootElement.Current.ClassName, "ClassName"); // TODO
			Assert.AreEqual (ControlType.Pane,
			                 AutomationElement.RootElement.Current.ControlType,
			                 "ControlType");
			//Assert.AreEqual (String.Empty, AutomationElement.RootElement.Current.FrameworkId, "FrameworkId"); // TODO: "Win32"
			Assert.IsFalse (AutomationElement.RootElement.Current.HasKeyboardFocus,
			                "HasKeyboardFocus");
			Assert.AreEqual (String.Empty,
			                 AutomationElement.RootElement.Current.HelpText,
			                 "HelpText");
			Assert.IsTrue (AutomationElement.RootElement.Current.IsContentElement,
			               "IsContentElement");
			Assert.IsTrue (AutomationElement.RootElement.Current.IsControlElement,
			               "IsControlElement");
			Assert.IsTrue (AutomationElement.RootElement.Current.IsEnabled,
			               "IsEnabled");
			Assert.IsFalse (AutomationElement.RootElement.Current.IsKeyboardFocusable,
			                "IsKeyboardFocusable");
			Assert.IsFalse (AutomationElement.RootElement.Current.IsOffscreen,
			                "IsOffscreen");
			Assert.IsFalse (AutomationElement.RootElement.Current.IsPassword,
			                "IsPassword");
			Assert.IsFalse (AutomationElement.RootElement.Current.IsRequiredForForm,
			                "IsRequiredForForm");
			Assert.AreEqual (String.Empty,
			                 AutomationElement.RootElement.Current.ItemStatus,
			                 "ItemStatus");
			Assert.AreEqual (String.Empty,
			                 AutomationElement.RootElement.Current.ItemType,
			                 "ItemType");
			Assert.IsNull (AutomationElement.RootElement.Current.LabeledBy,
			               "LabeledBy");
			Assert.AreEqual (ControlType.Pane.LocalizedControlType,
			                 AutomationElement.RootElement.Current.LocalizedControlType,
			                 "LocalizedControlType");
			Assert.AreEqual (String.Empty,
			                 AutomationElement.RootElement.Current.Name,
			                 "Name");
			//Assert.AreEqual (65552, AutomationElement.RootElement.Current.NativeWindowHandle, "NativeWindowHandle"); // TODO: Probably changes
			Assert.AreEqual (OrientationType.None,
			                 AutomationElement.RootElement.Current.Orientation,
			                 "Orientation");
			//Assert.AreEqual (-1, AutomationElement.RootElement.Current.ProcessId, "ProcessId"); // TODO: Probably changes
			//Assert.AreEqual (new int [] {42, 65552}, AutomationElement.RootElement.GetRuntimeId (), "GetRuntimeId"); // TODO: Probably changes
			Assert.IsNull (TreeWalker.RawViewWalker.GetParent (AutomationElement.RootElement),
			               "GetParent");
			Assert.IsNull (TreeWalker.RawViewWalker.GetNextSibling (AutomationElement.RootElement),
			               "GetNextSibling");
			Assert.IsNull (TreeWalker.RawViewWalker.GetPreviousSibling (AutomationElement.RootElement),
			               "GetPreviousSibling");
			VerifyPatterns (AutomationElement.RootElement);
		}

		[Test]
		public void PatternsTest ()
		{
			VerifyPatterns (button1Element,
				InvokePatternIdentifiers.Pattern);
		}

		[Test]
		public void CurrentTest ()
		{
			var current = label1Element.Current;
			Assert.AreEqual ("label1",
				current.Name,
				"check label1's original text");
			InvokePattern pattern = (InvokePattern) button1Element.GetCurrentPattern (InvokePattern.Pattern);
			pattern.Invoke ();
			Thread.Sleep (500);
			Assert.AreEqual ("button1_click",
				current.Name,
				"label1's text is modified after button1 is clicked");
		}

		#endregion

		#region Private Methods

		private void VerifyCurrentPropertyValue (AutomationElement element, AutomationProperty property, object expectedTrue, object expectedFalse, object expectedDefault)
		{
			Assert.AreEqual (expectedTrue,
				element.GetCurrentPropertyValue (property, true),
				property.ProgrammaticName + " w/ true");
			Assert.AreEqual (expectedFalse,
				element.GetCurrentPropertyValue (property, false),
				property.ProgrammaticName + " w/ false");
			Assert.AreEqual (expectedDefault,
				element.GetCurrentPropertyValue (property),
				property.ProgrammaticName + " w/ default");
		}

		private void VerifyFindAll (AutomationElement root, TreeScope scope, Condition condition, AutomationElement [] expectedElementsArray)
		{
			AutomationElementCollection result = null;

			List<AutomationElement> expectedElements = new List<AutomationElement> (expectedElementsArray);

			result = root.FindAll (scope, condition);

			// TODO: Uncomment when we support titlebar and its subelements (including min/max/close buttons)
			//Assert.AreEqual (expectedElements.Count, result.Count, "Result count");
			List<AutomationElement> resultList = new List<AutomationElement> ();
			foreach (AutomationElement element in result) {
				resultList.Add (element);
				// TODO: Uncomment when Count assertion is supported (see above)
				//Assert.IsTrue (expectedElements.Contains (element),
				//        String.Format ("Did not expect element named '{0}' with RuntimeId{1}", element.Current.Name, element.GetRuntimeId ()));
			}

			foreach (var element in expectedElements)
				Assert.IsTrue (resultList.Contains (element),
					String.Format ("Did not find expected element named '{0}' with RuntimeId{1}", element.Current.Name, PrintRuntimeId (element.GetRuntimeId ())));
		}

		private void VerifyFindFirst (AutomationElement root, TreeScope scope, AutomationElement [] includedElements, AutomationElement [] excludedElements)
		{
			foreach (var element in includedElements) {
				int [] runtimeId = element.GetRuntimeId ();
				AutomationElement result = root.FindFirst (scope,
					new PropertyCondition (AEIds.RuntimeIdProperty, runtimeId));
				Assert.IsNotNull (result,
					String.Format ("Expected element named '{0}' with RuntimeId {1}", element.Current.Name, PrintRuntimeId (runtimeId)));
				Assert.AreEqual (element, result,
					String.Format ("Expected element named '{0}' with RuntimeId {1}, got '{2}' with Id {3}", element.Current.Name, PrintRuntimeId (runtimeId), result.Current.Name, result.Current.AutomationId));
			}
			foreach (var element in excludedElements) {
				int [] runtimeId = element.GetRuntimeId ();
				AutomationElement result = root.FindFirst (scope,
					new PropertyCondition (AEIds.RuntimeIdProperty, runtimeId));
				Assert.IsNull (result,
					String.Format ("Did not expect element named '{0}' with RuntimeId{1}", element.Current.Name, PrintRuntimeId (runtimeId)));
			}
		}

		private string PrintRuntimeId (int [] runtimeId)
		{
			string output = "{";
			for (int i = 0; i < runtimeId.Length; i++) {
				output += runtimeId [i].ToString ();
				if (( i + 1 ) < runtimeId.Length)
					output += ",";
			}
			return output + "}";
		}

		private void VerifyPatterns (AutomationElement element, params AutomationPattern [] expected)
		{
			List<AutomationPattern> expectedPatterns = new List<AutomationPattern> (expected);
			List<AutomationPattern> supportedPatterns = new List<AutomationPattern> (element.GetSupportedPatterns ());
				object pattern1, pattern2;

			foreach (AutomationPattern pattern in patternProperties.Keys) {
				bool patternProperty = (bool) element.GetCurrentPropertyValue (patternProperties [pattern]);
				if (expectedPatterns.Contains (pattern)) {
					pattern1 = element.GetCurrentPattern (pattern);
					Assert.IsNotNull (pattern1, "GetCurrentPattern should not return null: " + pattern.ProgrammaticName);
					Assert.IsTrue (element.TryGetCurrentPattern (pattern, out pattern2), "TryGetCurrentPattern should return true: " + pattern.ProgrammaticName);
					Assert.IsNotNull (pattern2, "TryGetCurrentPattern should not return null: " + pattern.ProgrammaticName);
					Assert.IsTrue (supportedPatterns.Contains (pattern), "GetSupportedPatterns should return pattern: " + pattern.ProgrammaticName);
					Assert.IsTrue (patternProperty, "Pattern property: " + pattern.ProgrammaticName);
				} else {
					try {
						pattern1 = element.GetCurrentPattern (pattern);
						Assert.Fail ("GetCurrentPattern should return an InvalidOperation exception: " + pattern.ProgrammaticName);
					} catch (InvalidOperationException) { }
					Assert.IsFalse (element.TryGetCurrentPattern (pattern, out pattern2), "TryGetCurrentPattern should return false: " + pattern.ProgrammaticName);
					Assert.IsFalse (supportedPatterns.Contains (pattern), "GetSupportedPatterns should not return pattern: " + pattern.ProgrammaticName);
					Assert.IsFalse (patternProperty, "Pattern property: " + pattern.ProgrammaticName);
				}
			}

		}
		#endregion
	}
}
