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
using SWA = System.Windows.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class AutomationTest
	{
		// TODO: Test event handling methods
		[Test]
		public void ComparerRuntimeIdsTest ()
		{
			int [] idSet1a = new int [] { 1, 2 };
			int [] idSet1b = new int [] { 1, 2 };
			int [] idSet2 = new int [] { 1 };
			int [] idSet3 = new int [] { };
			int [] idSet4 = new int [] { 2, 1 };

			Assert.IsTrue (SWA.Automation.Compare (idSet1a, idSet1a),
				"Identity");
			Assert.IsTrue (SWA.Automation.Compare (idSet1a, idSet1b),
				"Identity");
			Assert.IsFalse (SWA.Automation.Compare (idSet1a, idSet4),
				"Order matters");
			Assert.IsFalse (SWA.Automation.Compare (idSet1a, idSet2),
				"Different lengths");
			Assert.IsFalse (SWA.Automation.Compare (idSet1a, idSet3),
				"Compare non-empty with empty");
			Assert.IsTrue (SWA.Automation.Compare (idSet3, idSet3),
				"Compare empty with empty");

			bool argumentNullRaised = false;
			try {
				SWA.Automation.Compare (idSet1a, null);
			} catch (ArgumentNullException) {
				argumentNullRaised = true;
			}
			Assert.IsTrue (argumentNullRaised,
				"Expected ArgumentNullException");
		}

		[Test]
		public void CompareAutomationElementsTest ()
		{
			Process p = BaseTest.StartApplication (@"SampleForm.exe",
				String.Empty);
			try {
				Thread.Sleep (1000);

				SWA.AutomationElement testFormElement = SWA.AutomationElement.RootElement.FindFirst (SWA.TreeScope.Children,
					new SWA.PropertyCondition (AEIds.ProcessIdProperty,
						p.Id));
				Assert.IsNotNull (testFormElement,
					"window");

				SWA.AutomationElement groupBox1Element = testFormElement.FindFirst (SWA.TreeScope.Children,
					new SWA.PropertyCondition (AEIds.ControlTypeProperty,
						SWA.ControlType.Group));
				Assert.IsNotNull (groupBox1Element,
					"groupBox1");

				Assert.IsTrue (SWA.Automation.Compare (testFormElement, testFormElement),
					"Identity");

				SWA.AutomationElement testFormElement2 = SWA.AutomationElement.RootElement.FindFirst (SWA.TreeScope.Children,
					new SWA.PropertyCondition (AEIds.ProcessIdProperty,
						p.Id));

				Assert.IsTrue (SWA.Automation.Compare (testFormElement, testFormElement2),
					"Comparing different instances representing the same element");

				Assert.IsFalse (SWA.Automation.Compare (testFormElement, groupBox1Element),
					"Comparing different elements");

				bool argumentNullRaised = false;
				try {
					SWA.Automation.Compare (testFormElement, null);
				} catch (ArgumentNullException) {
					argumentNullRaised = true;
				}
				Assert.IsTrue (argumentNullRaised,
					"Expected ArgumentNullException");
			} finally {
				p.Kill ();
			}
		}

		[Test]
		public void PatternNameTest ()
		{
			Dictionary<SWA.AutomationPattern, string> expectedPatternNames =
				new Dictionary<SWA.AutomationPattern, string> ();

			// Load for all patterns
			expectedPatternNames [SWA.DockPatternIdentifiers.Pattern] =
				"Dock";
			expectedPatternNames [SWA.ExpandCollapsePatternIdentifiers.Pattern] =
				"ExpandCollapse";
			expectedPatternNames [SWA.GridItemPatternIdentifiers.Pattern] =
				"GridItem";
			expectedPatternNames [SWA.GridPatternIdentifiers.Pattern] =
				"Grid";
			expectedPatternNames [SWA.InvokePatternIdentifiers.Pattern] =
				"Invoke";
			expectedPatternNames [SWA.MultipleViewPatternIdentifiers.Pattern] =
				"MultipleView";
			expectedPatternNames [SWA.RangeValuePatternIdentifiers.Pattern] =
				"RangeValue";
			expectedPatternNames [SWA.ScrollItemPatternIdentifiers.Pattern] =
				"ScrollItem";
			expectedPatternNames [SWA.ScrollPatternIdentifiers.Pattern] =
				"Scroll";
			expectedPatternNames [SWA.SelectionItemPatternIdentifiers.Pattern] =
				"SelectionItem";
			expectedPatternNames [SWA.SelectionPatternIdentifiers.Pattern] =
				"Selection";
			expectedPatternNames [SWA.TableItemPatternIdentifiers.Pattern] =
				"TableItem";
			expectedPatternNames [SWA.TablePatternIdentifiers.Pattern] =
				"Table";
			expectedPatternNames [SWA.TextPatternIdentifiers.Pattern] =
				"Text";
			expectedPatternNames [SWA.TogglePatternIdentifiers.Pattern] =
				"Toggle";
			expectedPatternNames [SWA.TransformPatternIdentifiers.Pattern] =
				"Transform";
			expectedPatternNames [SWA.ValuePatternIdentifiers.Pattern] =
				"Value";
			expectedPatternNames [SWA.WindowPatternIdentifiers.Pattern] =
				"Window";

			foreach (var pair in expectedPatternNames)
				Assert.AreEqual (pair.Value,
					SWA.Automation.PatternName (pair.Key),
					pair.Key.ProgrammaticName);

			bool argumentNullRaised = false;
			try {
				Assert.IsNull (SWA.Automation.PatternName (null));
			} catch (ArgumentNullException) {
				argumentNullRaised = true;
			}
			Assert.IsTrue (argumentNullRaised,
				"Expected ArgumentNullException");
		}

		[Test]
		public void PropertyNameTest ()
		{
			Dictionary<SWA.AutomationProperty, string> expectedPropertyNames =
				new Dictionary<SWA.AutomationProperty, string> ();

			//Load for everything in AEIds
			expectedPropertyNames [AEIds.AcceleratorKeyProperty] =
				"AcceleratorKey";
			expectedPropertyNames [AEIds.AccessKeyProperty] =
				"AccessKey";
			expectedPropertyNames [AEIds.AutomationIdProperty] =
				"AutomationId";
			expectedPropertyNames [AEIds.BoundingRectangleProperty] =
				"BoundingRectangle";
			expectedPropertyNames [AEIds.ClassNameProperty] =
				"ClassName";
			expectedPropertyNames [AEIds.ClickablePointProperty] =
				"ClickablePoint";
			expectedPropertyNames [AEIds.ControlTypeProperty] =
				"ControlType";
			expectedPropertyNames [AEIds.CultureProperty] =
				"Culture";
			expectedPropertyNames [AEIds.FrameworkIdProperty] =
				"FrameworkId";
			expectedPropertyNames [AEIds.HasKeyboardFocusProperty] =
				"HasKeyboardFocus";
			expectedPropertyNames [AEIds.HelpTextProperty] =
				"HelpText";
			expectedPropertyNames [AEIds.IsContentElementProperty] =
				"IsContentElement";
			expectedPropertyNames [AEIds.IsControlElementProperty] =
				"IsControlElement";
			expectedPropertyNames [AEIds.IsDockPatternAvailableProperty] =
				"IsDockPatternAvailable";
			expectedPropertyNames [AEIds.IsEnabledProperty] =
				"IsEnabled";
			expectedPropertyNames [AEIds.IsExpandCollapsePatternAvailableProperty] =
				"IsExpandCollapsePatternAvailable";
			expectedPropertyNames [AEIds.IsGridItemPatternAvailableProperty] =
				"IsGridItemPatternAvailable";
			expectedPropertyNames [AEIds.IsGridPatternAvailableProperty] =
				"IsGridPatternAvailable";
			expectedPropertyNames [AEIds.IsInvokePatternAvailableProperty] =
				"IsInvokePatternAvailable";
			expectedPropertyNames [AEIds.IsKeyboardFocusableProperty] =
				"IsKeyboardFocusable";
			expectedPropertyNames [AEIds.IsMultipleViewPatternAvailableProperty] =
				"IsMultipleViewPatternAvailable";
			expectedPropertyNames [AEIds.IsOffscreenProperty] =
				"IsOffscreen";
			expectedPropertyNames [AEIds.IsPasswordProperty] =
				"IsPassword";
			expectedPropertyNames [AEIds.IsRangeValuePatternAvailableProperty] =
				"IsRangeValuePatternAvailable";
			expectedPropertyNames [AEIds.IsRequiredForFormProperty] =
				"IsRequiredForForm";
			expectedPropertyNames [AEIds.IsScrollItemPatternAvailableProperty] =
				"IsScrollItemPatternAvailable";
			expectedPropertyNames [AEIds.IsScrollPatternAvailableProperty] =
				"IsScrollPatternAvailable";
			expectedPropertyNames [AEIds.IsSelectionItemPatternAvailableProperty] =
				"IsSelectionItemPatternAvailable";
			expectedPropertyNames [AEIds.IsSelectionPatternAvailableProperty] =
				"IsSelectionPatternAvailable";
			expectedPropertyNames [AEIds.IsTableItemPatternAvailableProperty] =
				"IsTableItemPatternAvailable";
			expectedPropertyNames [AEIds.IsTablePatternAvailableProperty] =
				"IsTablePatternAvailable";
			expectedPropertyNames [AEIds.IsTextPatternAvailableProperty] =
				"IsTextPatternAvailable";
			expectedPropertyNames [AEIds.IsTogglePatternAvailableProperty] =
				"IsTogglePatternAvailable";
			expectedPropertyNames [AEIds.IsTransformPatternAvailableProperty] =
				"IsTransformPatternAvailable";
			expectedPropertyNames [AEIds.IsValuePatternAvailableProperty] =
				"IsValuePatternAvailable";
			expectedPropertyNames [AEIds.IsWindowPatternAvailableProperty] =
				"IsWindowPatternAvailable";
			expectedPropertyNames [AEIds.ItemStatusProperty] =
				"ItemStatus";
			expectedPropertyNames [AEIds.ItemTypeProperty] =
				"ItemType";
			expectedPropertyNames [AEIds.LabeledByProperty] =
				"LabeledBy";
			expectedPropertyNames [AEIds.LocalizedControlTypeProperty] =
				"LocalizedControlType";
			expectedPropertyNames [AEIds.NameProperty] =
				"Name";
			expectedPropertyNames [AEIds.NativeWindowHandleProperty] =
				"NativeWindowHandle";
			expectedPropertyNames [AEIds.OrientationProperty] =
				"Orientation";
			expectedPropertyNames [AEIds.ProcessIdProperty] =
				"ProcessId";
			expectedPropertyNames [AEIds.RuntimeIdProperty] =
				"RuntimeId";

			// Load everything for *PatternIdentifiers
			expectedPropertyNames [SWA.DockPatternIdentifiers.DockPositionProperty] =
				"DockPosition";
			expectedPropertyNames [SWA.ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty] =
				"ExpandCollapseState";
			expectedPropertyNames [SWA.GridItemPatternIdentifiers.ColumnProperty] =
				"Column";
			expectedPropertyNames [SWA.GridItemPatternIdentifiers.ColumnSpanProperty] =
				"ColumnSpan";
			expectedPropertyNames [SWA.GridItemPatternIdentifiers.ContainingGridProperty] =
				"ContainingGrid";
			expectedPropertyNames [SWA.GridItemPatternIdentifiers.RowProperty] =
				"Row";
			expectedPropertyNames [SWA.GridItemPatternIdentifiers.RowSpanProperty] =
				"RowSpan";
			expectedPropertyNames [SWA.GridPatternIdentifiers.ColumnCountProperty] =
				"ColumnCount";
			expectedPropertyNames [SWA.GridPatternIdentifiers.RowCountProperty] =
				"RowCount";
			expectedPropertyNames [SWA.MultipleViewPatternIdentifiers.CurrentViewProperty] =
				"CurrentView";
			expectedPropertyNames [SWA.MultipleViewPatternIdentifiers.SupportedViewsProperty] =
				"SupportedViews";
			expectedPropertyNames [SWA.RangeValuePatternIdentifiers.IsReadOnlyProperty] =
				"IsReadOnly";
			expectedPropertyNames [SWA.RangeValuePatternIdentifiers.LargeChangeProperty] =
				"LargeChange";
			expectedPropertyNames [SWA.RangeValuePatternIdentifiers.MaximumProperty] =
				"Maximum";
			expectedPropertyNames [SWA.RangeValuePatternIdentifiers.MinimumProperty] =
				"Minimum";
			expectedPropertyNames [SWA.RangeValuePatternIdentifiers.SmallChangeProperty] =
				"SmallChange";
			expectedPropertyNames [SWA.RangeValuePatternIdentifiers.ValueProperty] =
				"Value";
			expectedPropertyNames [SWA.ScrollPatternIdentifiers.HorizontallyScrollableProperty] =
				"HorizontallyScrollable";
			expectedPropertyNames [SWA.ScrollPatternIdentifiers.HorizontalScrollPercentProperty] =
				"HorizontalScrollPercent";
			expectedPropertyNames [SWA.ScrollPatternIdentifiers.HorizontalViewSizeProperty] =
				"HorizontalViewSize";
			expectedPropertyNames [SWA.ScrollPatternIdentifiers.VerticallyScrollableProperty] =
				"VerticallyScrollable";
			expectedPropertyNames [SWA.ScrollPatternIdentifiers.VerticalScrollPercentProperty] =
				"VerticalScrollPercent";
			expectedPropertyNames [SWA.ScrollPatternIdentifiers.VerticalViewSizeProperty] =
				"VerticalViewSize";
			expectedPropertyNames [SWA.SelectionItemPatternIdentifiers.IsSelectedProperty] =
				"IsSelected";
			expectedPropertyNames [SWA.SelectionItemPatternIdentifiers.SelectionContainerProperty] =
				"SelectionContainer";
			expectedPropertyNames [SWA.SelectionPatternIdentifiers.CanSelectMultipleProperty] =
				"CanSelectMultiple";
			expectedPropertyNames [SWA.SelectionPatternIdentifiers.IsSelectionRequiredProperty] =
				"IsSelectionRequired";
			expectedPropertyNames [SWA.SelectionPatternIdentifiers.SelectionProperty] =
				"Selection";
			expectedPropertyNames [SWA.TableItemPatternIdentifiers.ColumnHeaderItemsProperty] =
				"ColumnHeaderItems";
			expectedPropertyNames [SWA.TableItemPatternIdentifiers.RowHeaderItemsProperty] =
				"RowHeaderItems";
			expectedPropertyNames [SWA.TablePatternIdentifiers.ColumnHeadersProperty] =
				"ColumnHeaders";
			expectedPropertyNames [SWA.TablePatternIdentifiers.RowHeadersProperty] =
				"RowHeaders";
			expectedPropertyNames [SWA.TablePatternIdentifiers.RowOrColumnMajorProperty] =
				"RowOrColumnMajor";
			expectedPropertyNames [SWA.TogglePatternIdentifiers.ToggleStateProperty] =
				"ToggleState";
			expectedPropertyNames [SWA.TransformPatternIdentifiers.CanMoveProperty] =
				"CanMove";
			expectedPropertyNames [SWA.TransformPatternIdentifiers.CanResizeProperty] =
				"CanResize";
			expectedPropertyNames [SWA.TransformPatternIdentifiers.CanRotateProperty] =
				"CanRotate";
			expectedPropertyNames [SWA.ValuePatternIdentifiers.IsReadOnlyProperty] =
				"IsReadOnly";
			expectedPropertyNames [SWA.ValuePatternIdentifiers.ValueProperty] =
				"Value";
			expectedPropertyNames [SWA.WindowPatternIdentifiers.CanMaximizeProperty] =
				"CanMaximize";
			expectedPropertyNames [SWA.WindowPatternIdentifiers.CanMinimizeProperty] =
				"CanMinimize";
			expectedPropertyNames [SWA.WindowPatternIdentifiers.IsModalProperty] =
				"IsModal";
			expectedPropertyNames [SWA.WindowPatternIdentifiers.IsTopmostProperty] =
				"IsTopmost";
			expectedPropertyNames [SWA.WindowPatternIdentifiers.WindowInteractionStateProperty] =
				"WindowInteractionState";
			expectedPropertyNames [SWA.WindowPatternIdentifiers.WindowVisualStateProperty] =
				"WindowVisualState";

			foreach (var pair in expectedPropertyNames)
				Assert.AreEqual (pair.Value,
					SWA.Automation.PropertyName (pair.Key),
					pair.Key.ProgrammaticName);

			bool argumentNullRaised = false;
			try {
				Assert.IsNull (SWA.Automation.PropertyName (null));
			} catch (ArgumentNullException) {
				argumentNullRaised = true;
			}
			Assert.IsTrue (argumentNullRaised,
				"Expected ArgumentNullException");
		}

		[Test]
		public void ContentViewConditionTest ()
		{
			SWA.Condition contentViewCond = SWA.Automation.ContentViewCondition;
			Assert.IsNotNull (contentViewCond, "ContentViewCondition");

			SWA.PropertyCondition contentViewPropCond = contentViewCond as SWA.PropertyCondition;
			Assert.IsNull (contentViewPropCond, "ContentViewCondition is not a PropertyCondition");

			SWA.AndCondition contentViewAndCond = contentViewCond as SWA.AndCondition; Assert.IsNull (contentViewPropCond, "ContentViewCondition is not a PropertyCondition");
			Assert.IsNull (contentViewAndCond, "ContentViewCondition is not a AndCondition");

			SWA.OrCondition contentViewOrCond = contentViewCond as SWA.OrCondition;
			Assert.IsNull (contentViewOrCond, "ContentViewCondition is not a OrCondition");

			SWA.NotCondition contentViewNotCond = contentViewCond as SWA.NotCondition;
			Assert.IsNotNull (contentViewNotCond, "ContentViewCondition is a NotCondition");

			SWA.Condition subCond = contentViewNotCond.Condition;
			Assert.IsNotNull (subCond, "ContentViewCondition.Condition");

			SWA.OrCondition subOrCond = subCond as SWA.OrCondition;
			Assert.IsNotNull (subOrCond, "ContentViewCondition.Condition is a OrCondition");

			SWA.Condition [] subSubConditions = subOrCond.GetConditions ();
			Assert.AreEqual (2, subSubConditions.Length, "ContentViewCondition.Condition.GetConditions length");

			SWA.PropertyCondition subSubPropertyCond1 = subSubConditions [0] as SWA.PropertyCondition;
			Assert.IsNotNull (subSubPropertyCond1);
			SWA.PropertyCondition subSubPropertyCond2 = subSubConditions [1] as SWA.PropertyCondition;
			Assert.IsNotNull (subSubPropertyCond2);

			Assert.AreEqual (AEIds.IsControlElementProperty,
				subSubPropertyCond1.Property,
				"subcondition1 Property");
			Assert.AreEqual (false,
				subSubPropertyCond1.Value,
				"subcondition1 Value");
			Assert.AreEqual (SWA.PropertyConditionFlags.None,
				subSubPropertyCond1.Flags,
				"subcondition1 Flags");

			Assert.AreEqual (AEIds.IsContentElementProperty.ProgrammaticName,
				subSubPropertyCond2.Property.ProgrammaticName,
				"subcondition2 Property");
			Assert.AreEqual (false,
				subSubPropertyCond2.Value,
				"subcondition2 Value");
			Assert.AreEqual (SWA.PropertyConditionFlags.None,
				subSubPropertyCond2.Flags,
				"subcondition2 Flags");
		}

		[Test]
		public void ControlViewConditionTest ()
		{
			SWA.Condition controlViewCond = SWA.Automation.ControlViewCondition;
			Assert.IsNotNull (controlViewCond, "ControlViewCondition");

			SWA.PropertyCondition controlViewPropCond = controlViewCond as SWA.PropertyCondition;
			Assert.IsNull (controlViewPropCond, "ControlViewCondition is not a PropertyCondition");

			SWA.AndCondition controlViewAndCond = controlViewCond as SWA.AndCondition;
			Assert.IsNull (controlViewAndCond, "ControlViewCondition is not a AndCondition");

			SWA.OrCondition controlViewOrCond = controlViewCond as SWA.OrCondition;
			Assert.IsNull (controlViewOrCond, "ControlViewCondition is not a OrCondition");

			SWA.NotCondition controlViewNotCond = controlViewCond as SWA.NotCondition;
			Assert.IsNotNull (controlViewNotCond, "ControlViewCondition is a NotCondition");

			SWA.Condition subCond = controlViewNotCond.Condition;
			Assert.IsNotNull (subCond, "ControlViewCondition.Condition");

			SWA.PropertyCondition subPropertyCond = subCond as SWA.PropertyCondition;
			Assert.IsNotNull (subPropertyCond, "ControlViewCondition.Condition is a PropertyCondition");
			Assert.AreEqual (AEIds.IsControlElementProperty,
				subPropertyCond.Property,
				"ControlViewCondition.Condition.Property");
			Assert.AreEqual (false,
				subPropertyCond.Value,
				"ControlViewCondition.Condition.Value");
			Assert.AreEqual (SWA.PropertyConditionFlags.None,
				subPropertyCond.Flags,
				"ControlViewCondition.Condition.Flags");
		}

		[Test]
		public void RawViewConditionTest ()
		{
			SWA.Condition rawViewCond = SWA.Automation.RawViewCondition;
			Assert.IsNotNull (rawViewCond, "RawViewCondition");

			SWA.PropertyCondition rawViewPropCond = rawViewCond as SWA.PropertyCondition;
			Assert.IsNull (rawViewPropCond, "RawViewCondition is not a PropertyCondition");

			SWA.AndCondition rawViewAndCond = rawViewCond as SWA.AndCondition;
			Assert.IsNull (rawViewAndCond, "RawViewCondition is not a AndCondition");

			SWA.OrCondition rawViewOrCond = rawViewCond as SWA.OrCondition;
			Assert.IsNull (rawViewOrCond, "RawViewCondition is not a OrCondition");

			SWA.NotCondition rawViewNotCond = rawViewCond as SWA.NotCondition;
			Assert.IsNull (rawViewNotCond, "RawViewCondition is not a NotCondition");
		}

		[Test]
		public void PatternMemberTest ()
		{
			Assert.AreEqual (SWA.DockPattern.Pattern, SWA.DockPatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.DockPattern.Pattern);

			Assert.AreEqual (SWA.DockPattern.DockPositionProperty, SWA.DockPatternIdentifiers.DockPositionProperty);
			Assert.IsNotNull (SWA.DockPattern.DockPositionProperty);

			Assert.AreEqual (SWA.ExpandCollapsePattern.Pattern, SWA.ExpandCollapsePatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.ExpandCollapsePattern.Pattern);

			Assert.AreEqual (SWA.ExpandCollapsePattern.ExpandCollapseStateProperty, SWA.ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty);
			Assert.IsNotNull (SWA.ExpandCollapsePattern.ExpandCollapseStateProperty);

			Assert.AreEqual (SWA.GridItemPattern.Pattern, SWA.GridItemPatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.GridItemPattern.Pattern);

			Assert.AreEqual (SWA.GridItemPattern.RowProperty, SWA.GridItemPatternIdentifiers.RowProperty);
			Assert.IsNotNull (SWA.GridItemPattern.RowProperty);

			Assert.AreEqual (SWA.GridItemPattern.ColumnProperty, SWA.GridItemPatternIdentifiers.ColumnProperty);
			Assert.IsNotNull (SWA.GridItemPattern.ColumnProperty);

			Assert.AreEqual (SWA.GridItemPattern.RowSpanProperty, SWA.GridItemPatternIdentifiers.RowSpanProperty);
			Assert.IsNotNull (SWA.GridItemPattern.RowSpanProperty);

			Assert.AreEqual (SWA.GridItemPattern.ColumnSpanProperty, SWA.GridItemPatternIdentifiers.ColumnSpanProperty);
			Assert.IsNotNull (SWA.GridItemPattern.ColumnSpanProperty);

			Assert.AreEqual (SWA.GridItemPattern.ContainingGridProperty, SWA.GridItemPatternIdentifiers.ContainingGridProperty);
			Assert.IsNotNull (SWA.GridItemPattern.ContainingGridProperty);

			Assert.AreEqual (SWA.GridPattern.Pattern, SWA.GridPatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.GridPattern.Pattern);

			Assert.AreEqual (SWA.GridPattern.RowCountProperty, SWA.GridPatternIdentifiers.RowCountProperty);
			Assert.IsNotNull (SWA.GridPattern.RowCountProperty);

			Assert.AreEqual (SWA.GridPattern.ColumnCountProperty, SWA.GridPatternIdentifiers.ColumnCountProperty);
			Assert.IsNotNull (SWA.GridPattern.ColumnCountProperty);

			Assert.AreEqual (SWA.InvokePattern.Pattern, SWA.InvokePatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.InvokePattern.Pattern);

			Assert.AreEqual (SWA.InvokePattern.InvokedEvent, SWA.InvokePatternIdentifiers.InvokedEvent);
			Assert.IsNotNull (SWA.InvokePattern.InvokedEvent);

			Assert.AreEqual (SWA.MultipleViewPattern.Pattern, SWA.MultipleViewPatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.MultipleViewPattern.Pattern);

			Assert.AreEqual (SWA.MultipleViewPattern.CurrentViewProperty, SWA.MultipleViewPatternIdentifiers.CurrentViewProperty);
			Assert.IsNotNull (SWA.MultipleViewPattern.CurrentViewProperty);

			Assert.AreEqual (SWA.MultipleViewPattern.SupportedViewsProperty, SWA.MultipleViewPatternIdentifiers.SupportedViewsProperty);
			Assert.IsNotNull (SWA.MultipleViewPattern.SupportedViewsProperty);

			Assert.AreEqual (SWA.RangeValuePattern.Pattern, SWA.RangeValuePatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.RangeValuePattern.Pattern);

			Assert.AreEqual (SWA.RangeValuePattern.ValueProperty, SWA.RangeValuePatternIdentifiers.ValueProperty);
			Assert.IsNotNull (SWA.RangeValuePattern.ValueProperty);

			Assert.AreEqual (SWA.RangeValuePattern.IsReadOnlyProperty, SWA.RangeValuePatternIdentifiers.IsReadOnlyProperty);
			Assert.IsNotNull (SWA.RangeValuePattern.IsReadOnlyProperty);

			Assert.AreEqual (SWA.RangeValuePattern.MinimumProperty, SWA.RangeValuePatternIdentifiers.MinimumProperty);
			Assert.IsNotNull (SWA.RangeValuePattern.MinimumProperty);

			Assert.AreEqual (SWA.RangeValuePattern.MaximumProperty, SWA.RangeValuePatternIdentifiers.MaximumProperty);
			Assert.IsNotNull (SWA.RangeValuePattern.MaximumProperty);

			Assert.AreEqual (SWA.RangeValuePattern.LargeChangeProperty, SWA.RangeValuePatternIdentifiers.LargeChangeProperty);
			Assert.IsNotNull (SWA.RangeValuePattern.LargeChangeProperty);

			Assert.AreEqual (SWA.RangeValuePattern.SmallChangeProperty, SWA.RangeValuePatternIdentifiers.SmallChangeProperty);
			Assert.IsNotNull (SWA.RangeValuePattern.SmallChangeProperty);

			Assert.AreEqual (SWA.ScrollItemPattern.Pattern, SWA.ScrollItemPatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.ScrollItemPattern.Pattern);

			Assert.AreEqual (SWA.ScrollPattern.Pattern, SWA.ScrollPatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.ScrollPattern.Pattern);

			Assert.AreEqual (SWA.ScrollPattern.HorizontalScrollPercentProperty, SWA.ScrollPatternIdentifiers.HorizontalScrollPercentProperty);
			Assert.IsNotNull (SWA.ScrollPattern.HorizontalScrollPercentProperty);

			Assert.AreEqual (SWA.ScrollPattern.VerticalScrollPercentProperty, SWA.ScrollPatternIdentifiers.VerticalScrollPercentProperty);
			Assert.IsNotNull (SWA.ScrollPattern.VerticalScrollPercentProperty);

			Assert.AreEqual (SWA.ScrollPattern.HorizontalViewSizeProperty, SWA.ScrollPatternIdentifiers.HorizontalViewSizeProperty);
			Assert.IsNotNull (SWA.ScrollPattern.HorizontalViewSizeProperty);

			Assert.AreEqual (SWA.ScrollPattern.VerticalViewSizeProperty, SWA.ScrollPatternIdentifiers.VerticalViewSizeProperty);
			Assert.IsNotNull (SWA.ScrollPattern.VerticalViewSizeProperty);

			Assert.AreEqual (SWA.ScrollPattern.HorizontallyScrollableProperty, SWA.ScrollPatternIdentifiers.HorizontallyScrollableProperty);
			Assert.IsNotNull (SWA.ScrollPattern.HorizontallyScrollableProperty);

			Assert.AreEqual (SWA.ScrollPattern.VerticallyScrollableProperty, SWA.ScrollPatternIdentifiers.VerticallyScrollableProperty);
			Assert.IsNotNull (SWA.ScrollPattern.VerticallyScrollableProperty);

			Assert.AreEqual (SWA.SelectionItemPattern.Pattern, SWA.SelectionItemPatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.SelectionItemPattern.Pattern);

			Assert.AreEqual (SWA.SelectionItemPattern.IsSelectedProperty, SWA.SelectionItemPatternIdentifiers.IsSelectedProperty);
			Assert.IsNotNull (SWA.SelectionItemPattern.IsSelectedProperty);

			Assert.AreEqual (SWA.SelectionItemPattern.SelectionContainerProperty, SWA.SelectionItemPatternIdentifiers.SelectionContainerProperty);
			Assert.IsNotNull (SWA.SelectionItemPattern.SelectionContainerProperty);

			Assert.AreEqual (SWA.SelectionItemPattern.ElementAddedToSelectionEvent, SWA.SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent);
			Assert.IsNotNull (SWA.SelectionItemPattern.ElementAddedToSelectionEvent);

			Assert.AreEqual (SWA.SelectionItemPattern.ElementRemovedFromSelectionEvent, SWA.SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent);
			Assert.IsNotNull (SWA.SelectionItemPattern.ElementRemovedFromSelectionEvent);

			Assert.AreEqual (SWA.SelectionItemPattern.ElementSelectedEvent, SWA.SelectionItemPatternIdentifiers.ElementSelectedEvent);
			Assert.IsNotNull (SWA.SelectionItemPattern.ElementSelectedEvent);

			Assert.AreEqual (SWA.SelectionPattern.Pattern, SWA.SelectionPatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.SelectionPattern.Pattern);

			Assert.AreEqual (SWA.SelectionPattern.SelectionProperty, SWA.SelectionPatternIdentifiers.SelectionProperty);
			Assert.IsNotNull (SWA.SelectionPattern.SelectionProperty);

			Assert.AreEqual (SWA.SelectionPattern.CanSelectMultipleProperty, SWA.SelectionPatternIdentifiers.CanSelectMultipleProperty);
			Assert.IsNotNull (SWA.SelectionPattern.CanSelectMultipleProperty);

			Assert.AreEqual (SWA.SelectionPattern.IsSelectionRequiredProperty, SWA.SelectionPatternIdentifiers.IsSelectionRequiredProperty);
			Assert.IsNotNull (SWA.SelectionPattern.IsSelectionRequiredProperty);

			Assert.AreEqual (SWA.SelectionPattern.InvalidatedEvent, SWA.SelectionPatternIdentifiers.InvalidatedEvent);
			Assert.IsNotNull (SWA.SelectionPattern.InvalidatedEvent);

			Assert.AreEqual (SWA.TableItemPattern.Pattern, SWA.TableItemPatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.TableItemPattern.Pattern);

			Assert.AreEqual (SWA.TableItemPattern.RowHeaderItemsProperty, SWA.TableItemPatternIdentifiers.RowHeaderItemsProperty);
			Assert.IsNotNull (SWA.TableItemPattern.RowHeaderItemsProperty);

			Assert.AreEqual (SWA.TableItemPattern.ColumnHeaderItemsProperty, SWA.TableItemPatternIdentifiers.ColumnHeaderItemsProperty);
			Assert.IsNotNull (SWA.TableItemPattern.ColumnHeaderItemsProperty);

			Assert.AreEqual (SWA.TablePattern.Pattern, SWA.TablePatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.TablePattern.Pattern);

			Assert.AreEqual (SWA.TablePattern.RowHeadersProperty, SWA.TablePatternIdentifiers.RowHeadersProperty);
			Assert.IsNotNull (SWA.TablePattern.RowHeadersProperty);

			Assert.AreEqual (SWA.TablePattern.ColumnHeadersProperty, SWA.TablePatternIdentifiers.ColumnHeadersProperty);
			Assert.IsNotNull (SWA.TablePattern.ColumnHeadersProperty);

			Assert.AreEqual (SWA.TablePattern.RowOrColumnMajorProperty, SWA.TablePatternIdentifiers.RowOrColumnMajorProperty);
			Assert.IsNotNull (SWA.TablePattern.RowOrColumnMajorProperty);

			Assert.AreEqual (SWA.TextPattern.AnimationStyleAttribute, SWA.TextPatternIdentifiers.AnimationStyleAttribute);
			Assert.IsNotNull (SWA.TextPattern.AnimationStyleAttribute);

			Assert.AreEqual (SWA.TextPattern.BackgroundColorAttribute, SWA.TextPatternIdentifiers.BackgroundColorAttribute);
			Assert.IsNotNull (SWA.TextPattern.BackgroundColorAttribute);

			Assert.AreEqual (SWA.TextPattern.BulletStyleAttribute, SWA.TextPatternIdentifiers.BulletStyleAttribute);
			Assert.IsNotNull (SWA.TextPattern.BulletStyleAttribute);

			Assert.AreEqual (SWA.TextPattern.CapStyleAttribute, SWA.TextPatternIdentifiers.CapStyleAttribute);
			Assert.IsNotNull (SWA.TextPattern.CapStyleAttribute);

			Assert.AreEqual (SWA.TextPattern.CultureAttribute, SWA.TextPatternIdentifiers.CultureAttribute);
			Assert.IsNotNull (SWA.TextPattern.CultureAttribute);

			Assert.AreEqual (SWA.TextPattern.FontNameAttribute, SWA.TextPatternIdentifiers.FontNameAttribute);
			Assert.IsNotNull (SWA.TextPattern.FontNameAttribute);

			Assert.AreEqual (SWA.TextPattern.FontSizeAttribute, SWA.TextPatternIdentifiers.FontSizeAttribute);
			Assert.IsNotNull (SWA.TextPattern.FontSizeAttribute);

			Assert.AreEqual (SWA.TextPattern.FontWeightAttribute, SWA.TextPatternIdentifiers.FontWeightAttribute);
			Assert.IsNotNull (SWA.TextPattern.FontWeightAttribute);

			Assert.AreEqual (SWA.TextPattern.ForegroundColorAttribute, SWA.TextPatternIdentifiers.ForegroundColorAttribute);
			Assert.IsNotNull (SWA.TextPattern.ForegroundColorAttribute);

			Assert.AreEqual (SWA.TextPattern.HorizontalTextAlignmentAttribute, SWA.TextPatternIdentifiers.HorizontalTextAlignmentAttribute);
			Assert.IsNotNull (SWA.TextPattern.HorizontalTextAlignmentAttribute);

			Assert.AreEqual (SWA.TextPattern.IndentationFirstLineAttribute, SWA.TextPatternIdentifiers.IndentationFirstLineAttribute);
			Assert.IsNotNull (SWA.TextPattern.IndentationFirstLineAttribute);

			Assert.AreEqual (SWA.TextPattern.IndentationLeadingAttribute, SWA.TextPatternIdentifiers.IndentationLeadingAttribute);
			Assert.IsNotNull (SWA.TextPattern.IndentationLeadingAttribute);

			Assert.AreEqual (SWA.TextPattern.IndentationTrailingAttribute, SWA.TextPatternIdentifiers.IndentationTrailingAttribute);
			Assert.IsNotNull (SWA.TextPattern.IndentationTrailingAttribute);

			Assert.AreEqual (SWA.TextPattern.IsHiddenAttribute, SWA.TextPatternIdentifiers.IsHiddenAttribute);
			Assert.IsNotNull (SWA.TextPattern.IsHiddenAttribute);

			Assert.AreEqual (SWA.TextPattern.IsItalicAttribute, SWA.TextPatternIdentifiers.IsItalicAttribute);
			Assert.IsNotNull (SWA.TextPattern.IsItalicAttribute);

			Assert.AreEqual (SWA.TextPattern.IsReadOnlyAttribute, SWA.TextPatternIdentifiers.IsReadOnlyAttribute);
			Assert.IsNotNull (SWA.TextPattern.IsReadOnlyAttribute);

			Assert.AreEqual (SWA.TextPattern.IsSubscriptAttribute, SWA.TextPatternIdentifiers.IsSubscriptAttribute);
			Assert.IsNotNull (SWA.TextPattern.IsSubscriptAttribute);

			Assert.AreEqual (SWA.TextPattern.IsSuperscriptAttribute, SWA.TextPatternIdentifiers.IsSuperscriptAttribute);
			Assert.IsNotNull (SWA.TextPattern.IsSuperscriptAttribute);

			Assert.AreEqual (SWA.TextPattern.MarginBottomAttribute, SWA.TextPatternIdentifiers.MarginBottomAttribute);
			Assert.IsNotNull (SWA.TextPattern.MarginBottomAttribute);

			Assert.AreEqual (SWA.TextPattern.MarginLeadingAttribute, SWA.TextPatternIdentifiers.MarginLeadingAttribute);
			Assert.IsNotNull (SWA.TextPattern.MarginLeadingAttribute);

			Assert.AreEqual (SWA.TextPattern.MarginTopAttribute, SWA.TextPatternIdentifiers.MarginTopAttribute);
			Assert.IsNotNull (SWA.TextPattern.MarginTopAttribute);

			Assert.AreEqual (SWA.TextPattern.MarginTrailingAttribute, SWA.TextPatternIdentifiers.MarginTrailingAttribute);
			Assert.IsNotNull (SWA.TextPattern.MarginTrailingAttribute);

			Assert.AreEqual (SWA.TextPattern.MixedAttributeValue, SWA.TextPatternIdentifiers.MixedAttributeValue);
			Assert.IsNotNull (SWA.TextPattern.MixedAttributeValue);

			Assert.AreEqual (SWA.TextPattern.OutlineStylesAttribute, SWA.TextPatternIdentifiers.OutlineStylesAttribute);
			Assert.IsNotNull (SWA.TextPattern.OutlineStylesAttribute);

			Assert.AreEqual (SWA.TextPattern.OverlineColorAttribute, SWA.TextPatternIdentifiers.OverlineColorAttribute);
			Assert.IsNotNull (SWA.TextPattern.OverlineColorAttribute);

			Assert.AreEqual (SWA.TextPattern.OverlineStyleAttribute, SWA.TextPatternIdentifiers.OverlineStyleAttribute);
			Assert.IsNotNull (SWA.TextPattern.OverlineStyleAttribute);

			Assert.AreEqual (SWA.TextPattern.Pattern, SWA.TextPatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.TextPattern.Pattern);

			Assert.AreEqual (SWA.TextPattern.StrikethroughColorAttribute, SWA.TextPatternIdentifiers.StrikethroughColorAttribute);
			Assert.IsNotNull (SWA.TextPattern.StrikethroughColorAttribute);

			Assert.AreEqual (SWA.TextPattern.StrikethroughStyleAttribute, SWA.TextPatternIdentifiers.StrikethroughStyleAttribute);
			Assert.IsNotNull (SWA.TextPattern.StrikethroughStyleAttribute);

			Assert.AreEqual (SWA.TextPattern.TabsAttribute, SWA.TextPatternIdentifiers.TabsAttribute);
			Assert.IsNotNull (SWA.TextPattern.TabsAttribute);

			Assert.AreEqual (SWA.TextPattern.TextFlowDirectionsAttribute, SWA.TextPatternIdentifiers.TextFlowDirectionsAttribute);
			Assert.IsNotNull (SWA.TextPattern.TextFlowDirectionsAttribute);

			Assert.AreEqual (SWA.TextPattern.UnderlineColorAttribute, SWA.TextPatternIdentifiers.UnderlineColorAttribute);
			Assert.IsNotNull (SWA.TextPattern.UnderlineColorAttribute);

			Assert.AreEqual (SWA.TextPattern.UnderlineStyleAttribute, SWA.TextPatternIdentifiers.UnderlineStyleAttribute);
			Assert.IsNotNull (SWA.TextPattern.UnderlineStyleAttribute);

			Assert.AreEqual (SWA.TextPattern.TextChangedEvent, SWA.TextPatternIdentifiers.TextChangedEvent);
			Assert.IsNotNull (SWA.TextPattern.TextChangedEvent);

			Assert.AreEqual (SWA.TextPattern.TextSelectionChangedEvent, SWA.TextPatternIdentifiers.TextSelectionChangedEvent);
			Assert.IsNotNull (SWA.TextPattern.TextSelectionChangedEvent);

			Assert.AreEqual (SWA.TogglePattern.Pattern, SWA.TogglePatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.TogglePattern.Pattern);

			Assert.AreEqual (SWA.TogglePattern.ToggleStateProperty, SWA.TogglePatternIdentifiers.ToggleStateProperty);
			Assert.IsNotNull (SWA.TogglePattern.ToggleStateProperty);

			Assert.AreEqual (SWA.ValuePattern.Pattern, SWA.ValuePatternIdentifiers.Pattern);
			Assert.IsNotNull (SWA.ValuePattern.Pattern);

			Assert.AreEqual (SWA.ValuePattern.ValueProperty, SWA.ValuePatternIdentifiers.ValueProperty);
			Assert.IsNotNull (SWA.ValuePattern.ValueProperty);

			Assert.AreEqual (SWA.ValuePattern.IsReadOnlyProperty, SWA.ValuePatternIdentifiers.IsReadOnlyProperty);
			Assert.IsNotNull (SWA.ValuePattern.IsReadOnlyProperty);
		}

		[Test]
		public void RemoveAutomationEventHandlerTest ()
		{
			int eventCount = 0;
			Process p = BaseTest.StartApplication (@"SampleForm.exe", string.Empty);
			SWA.AutomationEventHandler handler = (o, e) => eventCount++;
			Thread.Sleep (2000); // Waiting a little bit for the application to show up

			SWA.AutomationElement testFormElement
				= SWA.AutomationElement.RootElement.FindFirst (SWA.TreeScope.Children,
					new SWA.PropertyCondition (AEIds.ProcessIdProperty, p.Id));
			Assert.IsNotNull (testFormElement, "window");

			SWA.Automation.RemoveAutomationEventHandler (SWA.AutomationElementIdentifiers.AsyncContentLoadedEvent,
			                                             testFormElement,
								     handler);

			BaseTest.AssertRaises<ArgumentException> (
			() => SWA.Automation.RemoveAutomationEventHandler (SWA.AutomationElementIdentifiers.AutomationFocusChangedEvent,
				                                           testFormElement,
									   handler),
			      "SWA.AutomationElementIdentifiers.AutomationFocusChangedEvent");

			BaseTest.AssertRaises<ArgumentException> (
			() => SWA.Automation.RemoveAutomationEventHandler (SWA.AutomationElementIdentifiers.AutomationPropertyChangedEvent,
			                                                   testFormElement,
									   handler),
			      "SWA.AutomationElementIdentifiers.AutomationPropertyChangedEvent");

			SWA.Automation.RemoveAutomationEventHandler (SWA.AutomationElementIdentifiers.LayoutInvalidatedEvent,
			                                             testFormElement,
								     handler);
			SWA.Automation.RemoveAutomationEventHandler (SWA.AutomationElementIdentifiers.MenuClosedEvent,
			                                             testFormElement,
								     handler);
			SWA.Automation.RemoveAutomationEventHandler (SWA.AutomationElementIdentifiers.MenuOpenedEvent,
			                                             testFormElement,
								     handler);

			BaseTest.AssertRaises<ArgumentException> (
			() => SWA.Automation.RemoveAutomationEventHandler (SWA.AutomationElementIdentifiers.StructureChangedEvent,
									   testFormElement,
									   handler),
			      "SWA.AutomationElementIdentifiers.StructureChangedEvent");

			SWA.Automation.RemoveAutomationEventHandler (SWA.AutomationElementIdentifiers.ToolTipClosedEvent,
			                                             testFormElement,
								     handler);
			SWA.Automation.RemoveAutomationEventHandler (SWA.AutomationElementIdentifiers.ToolTipOpenedEvent,
			                                             testFormElement,
								     handler);
			SWA.Automation.RemoveAutomationEventHandler (SWA.InvokePatternIdentifiers.InvokedEvent,
			                                             testFormElement,
								     handler);
			SWA.Automation.RemoveAutomationEventHandler (SWA.SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent,
			                                             testFormElement,
								     handler);
			SWA.Automation.RemoveAutomationEventHandler (SWA.SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent,
			                                             testFormElement,
								     handler);
			SWA.Automation.RemoveAutomationEventHandler (SWA.SelectionItemPatternIdentifiers.ElementSelectedEvent,
			                                             testFormElement,
								     handler);
			SWA.Automation.RemoveAutomationEventHandler (SWA.SelectionPatternIdentifiers.InvalidatedEvent,
			                                             testFormElement,
								     handler);
			SWA.Automation.RemoveAutomationEventHandler (SWA.TextPatternIdentifiers.TextChangedEvent,
			                                             testFormElement,
								     handler);
			SWA.Automation.RemoveAutomationEventHandler (SWA.TextPatternIdentifiers.TextSelectionChangedEvent,
			                                             testFormElement,
								     handler);
			SWA.Automation.RemoveAutomationEventHandler (SWA.WindowPatternIdentifiers.WindowOpenedEvent,
			                                             testFormElement,
								     handler);

			p.Kill ();
		}
	}
}
