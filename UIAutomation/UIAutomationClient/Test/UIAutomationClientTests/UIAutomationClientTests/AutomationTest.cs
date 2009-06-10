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

		public virtual Process StartApplication (string name, string arguments)
		{
			Process p = new Process ();
			p.StartInfo.FileName = name;
			p.StartInfo.Arguments = arguments;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.CreateNoWindow = true;
			p.Start ();
			return p;
		}

		[Test]
		public void CompareAutomationElementsTest ()
		{
			Process p = StartApplication (@"SampleForm.exe",
				string.Empty);
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

			SWA.AndCondition controlViewAndCond = controlViewCond as SWA.AndCondition; Assert.IsNull (controlViewPropCond, "ControliewCondition is not a PropertyCondition");
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

			SWA.AndCondition rawViewAndCond = rawViewCond as SWA.AndCondition; Assert.IsNull (rawViewPropCond, "RawViewCondition is not a PropertyCondition");
			Assert.IsNull (rawViewAndCond, "RawViewCondition is not a AndCondition");

			SWA.OrCondition rawViewOrCond = rawViewCond as SWA.OrCondition;
			Assert.IsNull (rawViewOrCond, "RawViewCondition is not a OrCondition");

			SWA.NotCondition rawViewNotCond = rawViewCond as SWA.NotCondition;
			Assert.IsNull (rawViewNotCond, "RawViewCondition is not a NotCondition");
		}
	}
}
