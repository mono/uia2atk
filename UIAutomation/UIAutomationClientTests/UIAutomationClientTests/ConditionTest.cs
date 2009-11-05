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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class ConditionTest : BaseTest
	{
		[Test]
		public void TrueConditionTest ()
		{
			Condition trueCond = Condition.TrueCondition;
			Assert.IsNotNull (trueCond, "TrueCondition");

			PropertyCondition truePropCond = trueCond as PropertyCondition;
			Assert.IsNull (truePropCond, "TrueCondition is not a PropertyCondition");

			AndCondition trueAndCond = trueCond as AndCondition;
			Assert.IsNull (trueAndCond, "TrueCondition is not a AndCondition");

			OrCondition trueOrCond = trueCond as OrCondition;
			Assert.IsNull (trueOrCond, "TrueCondition is not a OrCondition");

			NotCondition trueNotCond = trueCond as NotCondition;
			Assert.IsNull (trueNotCond, "TrueCondition is not a NotCondition");
		}

		[Test]
		public void FalseConditionTest ()
		{
			Condition falseCond = Condition.FalseCondition;
			Assert.IsNotNull (falseCond, "FalseCondition");

			PropertyCondition falsePropCond = falseCond as PropertyCondition;
			Assert.IsNull (falsePropCond, "FalseCondition is not a PropertyCondition");

			AndCondition falseAndCond = falseCond as AndCondition;
			Assert.IsNull (falseAndCond, "FalseCondition is not a AndCondition");

			OrCondition falseOrCond = falseCond as OrCondition;
			Assert.IsNull (falseOrCond, "FalseCondition is not a OrCondition");

			NotCondition falseNotCond = falseCond as NotCondition;
			Assert.IsNull (falseNotCond, "FalseCondition is not a NotCondition");
		}

		[Test]
		public void PropertyConditionTest ()
		{
			AssertRaises<ArgumentNullException> (
				() => new PropertyCondition (null, null),
				"passing null to both params of PropertyCondition constructor");

			//Load for everything in AEIds
			VerifyPropertyConditionBasics (AEIds.AcceleratorKeyProperty,
				new object [] { string.Empty, null },
				new object [] { 5 });
			VerifyPropertyConditionBasics (AEIds.AccessKeyProperty,
				new object [] { string.Empty, null },
				new object [] { true });
			VerifyPropertyConditionBasics (AEIds.AutomationIdProperty,
				new object [] { string.Empty, null },
				new object [] { true });
			VerifyPropertyConditionBasics (AEIds.BoundingRectangleProperty,
				new object [] { Rect.Empty },
				new object [] { null, true },
				new object [] { new double [] { double.PositiveInfinity, double.PositiveInfinity, double.NegativeInfinity, double.NegativeInfinity } });
			VerifyPropertyConditionBasics (AEIds.ClassNameProperty,
				new object [] { string.Empty, null },
				new object [] { true });
			VerifyPropertyConditionBasics (AEIds.ClickablePointProperty,
			        new object [] { new Point (0, 0) },
			        new object [] { null, true },
				new object [] { new double [] {0, 0} });
			VerifyPropertyConditionBasics (AEIds.ControlTypeProperty,
				new object [] { ControlType.Button, null },
				new object [] { ControlType.Button.Id, string.Empty },
				new object [] { ControlType.Button.Id, null });
			VerifyPropertyConditionBasics (AEIds.CultureProperty,
				new object [] { new CultureInfo ("en-US"), new CultureInfo ("fr-FR"), null },
				new object [] { new CultureInfo ("en-US").LCID, "en-US", string.Empty, 5 },
				new object [] { new CultureInfo ("en-US").LCID, new CultureInfo ("fr-FR").LCID, null });
			VerifyPropertyConditionBasics (AEIds.FrameworkIdProperty,
				new object [] { "WinForm", string.Empty, "hiya", null },
				new object [] { 5 });
			VerifyPropertyConditionBasics (AEIds.HasKeyboardFocusProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.HelpTextProperty,
				new object [] { string.Empty, null },
				new object [] { true });
			VerifyPropertyConditionBasics (AEIds.IsContentElementProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsControlElementProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsDockPatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsEnabledProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsExpandCollapsePatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsGridItemPatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsGridPatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsInvokePatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsKeyboardFocusableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsMultipleViewPatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsOffscreenProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsPasswordProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsRangeValuePatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsRequiredForFormProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsScrollItemPatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsScrollPatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsSelectionItemPatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsSelectionPatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsTableItemPatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsTablePatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsTextPatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsTogglePatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsTransformPatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsValuePatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.IsWindowPatternAvailableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (AEIds.ItemStatusProperty,
				new object [] { string.Empty, null },
				new object [] { true });
			VerifyPropertyConditionBasics (AEIds.ItemTypeProperty,
				new object [] { string.Empty, null },
				new object [] { true });
			VerifyPropertyConditionBasics (AEIds.LabeledByProperty,
				new object [] { button1Element, null },
				new object [] { string.Empty, true },
				new object [] { button1Element.GetRuntimeId (), null });
			VerifyPropertyConditionBasics (AEIds.LocalizedControlTypeProperty,
				new object [] { string.Empty, null },
				new object [] { true });
			VerifyPropertyConditionBasics (AEIds.NameProperty,
				new object [] { string.Empty, null },
				new object [] { true });
			VerifyPropertyConditionBasics (AEIds.NativeWindowHandleProperty,
				new object [] { 5 },
				new object [] { null, string.Empty, true });
			VerifyPropertyConditionBasics (AEIds.OrientationProperty,
				new object [] { OrientationType.Horizontal },
				new object [] { null, true });
			VerifyPropertyConditionBasics (AEIds.ProcessIdProperty,
				new object [] { 5 },
				new object [] { null, true });
			VerifyPropertyConditionBasics (AEIds.RuntimeIdProperty,
				new object [] { new int [] { 5, 6, 7 }, null },
				new object [] { true });

			// Load everything for *PatternIdentifiers
			VerifyPropertyConditionBasics (DockPatternIdentifiers.DockPositionProperty,
				new object [] { DockPosition.Bottom },
				new object [] { null, true });
			VerifyPropertyConditionBasics (ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
				new object [] { ExpandCollapseState.Collapsed },
				new object [] { null, true });
			VerifyPropertyConditionBasics (GridItemPatternIdentifiers.ColumnProperty,
				new object [] { 5, -5 },
				new object [] { null, true });
			VerifyPropertyConditionBasics (GridItemPatternIdentifiers.ColumnSpanProperty,
				new object [] { 5, -5 },
				new object [] { null, true });
			VerifyPropertyConditionBasics (GridItemPatternIdentifiers.ContainingGridProperty,
				new object [] { button1Element, null },
				new object [] { true },
				new object [] { button1Element.GetRuntimeId (), null });
			VerifyPropertyConditionBasics (GridItemPatternIdentifiers.RowProperty,
				new object [] { 5, -5 },
				new object [] { null, true });
			VerifyPropertyConditionBasics (GridItemPatternIdentifiers.RowSpanProperty,
				new object [] { 5, -5 },
				new object [] { null, true });
			VerifyPropertyConditionBasics (GridPatternIdentifiers.ColumnCountProperty,
				new object [] { 5, -5 },
				new object [] { null, true });
			VerifyPropertyConditionBasics (GridPatternIdentifiers.RowCountProperty,
				new object [] { 5, -5 },
				new object [] { null, true });
			VerifyPropertyConditionBasics (MultipleViewPatternIdentifiers.CurrentViewProperty,
				new object [] { 5, -5 },
				new object [] { null, true });
			VerifyPropertyConditionBasics (MultipleViewPatternIdentifiers.SupportedViewsProperty,
				new object [] { new int [] { 5, -5 }, new int [] { }, null },
				new object [] { true });
			VerifyPropertyConditionBasics (RangeValuePatternIdentifiers.IsReadOnlyProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (RangeValuePatternIdentifiers.LargeChangeProperty,
				new object [] { 5.0, -5.0, 1.2 },
				new object [] { null, 5, -5, true });
			VerifyPropertyConditionBasics (RangeValuePatternIdentifiers.MaximumProperty,
				new object [] { 5.0, -5.0, 1.2, 5, -5, true, string.Empty, null },
				new object [] { });
			VerifyPropertyConditionBasics (RangeValuePatternIdentifiers.MinimumProperty,
				new object [] { 5.0, -5.0, 1.2, 5, -5, true, string.Empty, null },
				new object [] { });
			VerifyPropertyConditionBasics (RangeValuePatternIdentifiers.SmallChangeProperty,
				new object [] { 5.0, -5.0, 1.2 },
				new object [] { null, 5, -5, true });
			VerifyPropertyConditionBasics (RangeValuePatternIdentifiers.ValueProperty,
				new object [] { 5.0, -5.0, 1.2 },
				new object [] { null, 5, -5, true });
			VerifyPropertyConditionBasics (ScrollPatternIdentifiers.HorizontallyScrollableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (ScrollPatternIdentifiers.HorizontalScrollPercentProperty,
				new object [] { 5.0, -5.0, 1.2 },
				new object [] { null, 5, -5, true });
			VerifyPropertyConditionBasics (ScrollPatternIdentifiers.HorizontalViewSizeProperty,
				new object [] { 5.0, -5.0, 1.2 },
				new object [] { null, 5, -5, true });
			VerifyPropertyConditionBasics (ScrollPatternIdentifiers.VerticallyScrollableProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (ScrollPatternIdentifiers.VerticalScrollPercentProperty,
				new object [] { 5.0, -5.0, 1.2 },
				new object [] { null, 5, -5, true });
			VerifyPropertyConditionBasics (ScrollPatternIdentifiers.VerticalViewSizeProperty,
				new object [] { 5.0, -5.0, 1.2 },
				new object [] { null, 5, -5, true });
			VerifyPropertyConditionBasics (SelectionItemPatternIdentifiers.IsSelectedProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (SelectionItemPatternIdentifiers.SelectionContainerProperty,
				new object [] { button1Element, null },
				new object [] { true },
				new object [] { button1Element.GetRuntimeId (), null });
			VerifyPropertyConditionBasics (SelectionPatternIdentifiers.CanSelectMultipleProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (SelectionPatternIdentifiers.IsSelectionRequiredProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (SelectionPatternIdentifiers.SelectionProperty,
				new object [] { new AutomationElement [] { button1Element }, null },
				new object [] { button1Element, true });
			VerifyPropertyConditionBasics (TableItemPatternIdentifiers.ColumnHeaderItemsProperty,
				new object [] { new AutomationElement [] { button1Element }, null },
				new object [] { button1Element, true });
			VerifyPropertyConditionBasics (TableItemPatternIdentifiers.RowHeaderItemsProperty,
				new object [] { new AutomationElement [] { button1Element }, null },
				new object [] { button1Element, true });
			VerifyPropertyConditionBasics (TablePatternIdentifiers.ColumnHeadersProperty,
				new object [] { new AutomationElement [] { button1Element }, null },
				new object [] { button1Element, true });
			VerifyPropertyConditionBasics (TablePatternIdentifiers.RowHeadersProperty,
				new object [] { new AutomationElement [] { button1Element }, null },
				new object [] { button1Element, true });
			VerifyPropertyConditionBasics (TablePatternIdentifiers.RowOrColumnMajorProperty,
				new object [] { RowOrColumnMajor.ColumnMajor, AutomationElement.NotSupported },
				new object [] { null, true });
			VerifyPropertyConditionBasics (TogglePatternIdentifiers.ToggleStateProperty,
				new object [] { ToggleState.Indeterminate },
				new object [] { null, true });
			VerifyPropertyConditionBasics (TransformPatternIdentifiers.CanMoveProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (TransformPatternIdentifiers.CanResizeProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (TransformPatternIdentifiers.CanRotateProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (ValuePatternIdentifiers.IsReadOnlyProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (ValuePatternIdentifiers.ValueProperty,
				new object [] { string.Empty, null },
				new object [] { 5, true });
			VerifyPropertyConditionBasics (WindowPatternIdentifiers.CanMaximizeProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (WindowPatternIdentifiers.CanMinimizeProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (WindowPatternIdentifiers.IsModalProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (WindowPatternIdentifiers.IsTopmostProperty,
				new object [] { true },
				new object [] { null, string.Empty });
			VerifyPropertyConditionBasics (WindowPatternIdentifiers.WindowInteractionStateProperty,
				new object [] { WindowInteractionState.BlockedByModalWindow },
				new object [] { null, true });
			VerifyPropertyConditionBasics (WindowPatternIdentifiers.WindowVisualStateProperty,
				new object [] { WindowVisualState.Maximized },
				new object [] { null, true });

			Assert.IsNotNull (button1Element.FindFirst (TreeScope.Element,
				new PropertyCondition (AEIds.NameProperty, "button1")));
			Assert.IsNull (button1Element.FindFirst (TreeScope.Element,
				new PropertyCondition (AEIds.NameProperty, "Button1")));
			Assert.IsNull (button1Element.FindFirst (TreeScope.Element,
				new PropertyCondition (AEIds.NameProperty, "Button1", PropertyConditionFlags.None)));
			Assert.IsNotNull (button1Element.FindFirst (TreeScope.Element,
				new PropertyCondition (AEIds.NameProperty, "Button1", PropertyConditionFlags.IgnoreCase)));

			PropertyCondition cond1 = new PropertyCondition (AEIds.NameProperty,
				string.Empty);
			PropertyCondition cond2 = new PropertyCondition (AEIds.NameProperty,
				string.Empty);
			Assert.AreNotEqual (cond1, cond2);
		}

		private void VerifyPropertyConditionBasics (AutomationProperty property,
			object [] expectedGoodValues,
			object [] expectedBadValues)
		{
			VerifyPropertyConditionBasics (property,
				expectedGoodValues,
				expectedBadValues,
				expectedGoodValues);
		}

		private void VerifyPropertyConditionBasics (AutomationProperty property,
			object [] expectedGoodValues,
			object [] expectedBadValues,
			object [] expectedGoodValuePropValues)
		{
			Assert.AreEqual (expectedGoodValues.Length,
				expectedGoodValuePropValues.Length,
				"Cannot test PropertyConditon Value property if expectedGoodValues.Length != expectedGoodValuePropValues.Length");

			List<object> goodVals = new List<object> (expectedGoodValues);
			goodVals.Add (AutomationElement.NotSupported);

			List<object> goodValPropVals = new List<object> (expectedGoodValuePropValues);
			goodValPropVals.Add (AutomationElement.NotSupported);

			for (int i = 0; i < goodVals.Count; i++) {
				object val = goodVals [i];
				object expectedPropVal = goodValPropVals [i];

				VerifyPropertyConditionConstructor (property,
					val,
					expectedPropVal,
					null);
				VerifyPropertyConditionConstructor (property,
					val,
					expectedPropVal,
					PropertyConditionFlags.None);
				VerifyPropertyConditionConstructor (property,
					val,
					expectedPropVal,
					PropertyConditionFlags.IgnoreCase);
			}

			foreach (object val in expectedBadValues) {
				AssertRaises<ArgumentException> (
					() => new PropertyCondition (property, val),
					string.Format ("using '{0}' as value for {1}",
				                        val ?? "(null)",
				                        property.ProgrammaticName));
			}
		}

		private void VerifyPropertyConditionConstructor (AutomationProperty property,
			object val,
			object expectedPropVal,
			PropertyConditionFlags? flags)
		{
			bool exceptionRaised = false;
			bool ignoreCaseAllowed = val is string;
			bool exceptionExpected = flags.HasValue &&
				flags.Value == PropertyConditionFlags.IgnoreCase &&
				!ignoreCaseAllowed;
			try {
				PropertyCondition cond;
				if (flags.HasValue)
					cond = new PropertyCondition (property, val, flags.Value);
				else
					cond = new PropertyCondition (property, val);
				Assert.AreEqual (property, cond.Property,
					"PropertyCondition.Property");
				Assert.AreEqual (expectedPropVal, cond.Value,
					"PropertyCondition.Value for " +
					property.ProgrammaticName);
				Assert.AreEqual (flags.HasValue ? flags.Value : PropertyConditionFlags.None,
					cond.Flags,
					"PropertyCondition.Flags");
			} catch (ArgumentException) {
				exceptionRaised = true;
			}
			Assert.AreEqual (exceptionExpected,
				exceptionRaised,
				string.Format ("For {0} expected '{1}' with {2} to be a {3}",
					property.ProgrammaticName,
					val ?? "(null)",
					flags.HasValue ? flags.Value.ToString () : "no flag specified",
					exceptionExpected ? "bad value" : "good value, but instead received ArgumentException"));
		}
	}
}
