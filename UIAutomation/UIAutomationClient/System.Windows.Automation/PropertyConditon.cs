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
using System.Globalization;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace System.Windows.Automation
{
	[Flags]
	public enum PropertyConditionFlags
	{
		None,
		IgnoreCase
	}

	public class PropertyCondition : Condition
	{
		private AutomationProperty property;
		private object val;
		private PropertyConditionFlags flags;

		public PropertyCondition (AutomationProperty property,
		                          object value,
		                          PropertyConditionFlags flags) : base ()
		{
			if (property == null)
				throw new ArgumentNullException ("property");
			this.property = property;

			// NotSupported is handled the same way for all properties
			if (value == AutomationElement.NotSupported) {
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				this.val = value;
				this.flags = flags;
				return;
			}

			if (property == AEIds.AcceleratorKeyProperty ||
			    property == AEIds.AccessKeyProperty ||
			    property == AEIds.AutomationIdProperty ||
			    property == AEIds.ClassNameProperty ||
			    property == AEIds.HelpTextProperty ||
			    property == AEIds.ItemStatusProperty ||
			    property == AEIds.ItemTypeProperty ||
			    property == AEIds.LocalizedControlTypeProperty ||
			    property == AEIds.NameProperty ||
			    property == AEIds.FrameworkIdProperty ||
			    property == ValuePatternIdentifiers.ValueProperty) {
				if (value != null && !(value is string))
					throw new ArgumentException ("value");
				if (value == null && (flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				val = value;
			} else if (property == AEIds.BoundingRectangleProperty) {
				Rect? rect = null;
				if (value == null || !(rect = value as Rect?).HasValue)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				val = new double [] {
					rect.Value.X,
					rect.Value.Y,
					rect.Value.Width,
					rect.Value.Height };
			} else if (property == AEIds.ClickablePointProperty) {
				Point? point = null;
				if (value == null || !(point = value as Point?).HasValue)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				val = new double [] {
					point.Value.X,
					point.Value.Y };
			} else if (property == AEIds.ControlTypeProperty) {
				ControlType controlType = null;
				if (value != null && (controlType = value as ControlType) == null)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				if (controlType != null)
					val = controlType.Id;
			} else if (property == AEIds.CultureProperty) {
				CultureInfo culture = null;
				if (value != null && (culture = value as CultureInfo) == null)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				if (culture != null)
					val = culture.LCID;
			} else if (property == AEIds.HasKeyboardFocusProperty ||
			           property == AEIds.IsContentElementProperty ||
			           property == AEIds.IsControlElementProperty ||
			           property == AEIds.IsDockPatternAvailableProperty ||
			           property == AEIds.IsEnabledProperty ||
			           property == AEIds.IsExpandCollapsePatternAvailableProperty ||
			           property == AEIds.IsGridItemPatternAvailableProperty ||
			           property == AEIds.IsGridPatternAvailableProperty ||
			           property == AEIds.IsInvokePatternAvailableProperty ||
			           property == AEIds.IsKeyboardFocusableProperty ||
			           property == AEIds.IsMultipleViewPatternAvailableProperty ||
			           property == AEIds.IsOffscreenProperty ||
			           property == AEIds.IsPasswordProperty ||
			           property == AEIds.IsRangeValuePatternAvailableProperty ||
			           property == AEIds.IsRequiredForFormProperty ||
			           property == AEIds.IsScrollItemPatternAvailableProperty ||
			           property == AEIds.IsScrollPatternAvailableProperty ||
			           property == AEIds.IsSelectionItemPatternAvailableProperty ||
			           property == AEIds.IsSelectionPatternAvailableProperty ||
			           property == AEIds.IsTableItemPatternAvailableProperty ||
			           property == AEIds.IsTablePatternAvailableProperty ||
			           property == AEIds.IsTextPatternAvailableProperty ||
			           property == AEIds.IsTogglePatternAvailableProperty ||
			           property == AEIds.IsTransformPatternAvailableProperty ||
			           property == AEIds.IsValuePatternAvailableProperty ||
			           property == AEIds.IsWindowPatternAvailableProperty ||
			           property == RangeValuePatternIdentifiers.IsReadOnlyProperty ||
			           property == ScrollPatternIdentifiers.HorizontallyScrollableProperty ||
			           property == ScrollPatternIdentifiers.VerticallyScrollableProperty ||
			           property == SelectionItemPatternIdentifiers.IsSelectedProperty ||
			           property == SelectionPatternIdentifiers.CanSelectMultipleProperty ||
			           property == SelectionPatternIdentifiers.IsSelectionRequiredProperty ||
			           property == TransformPatternIdentifiers.CanMoveProperty ||
			           property == TransformPatternIdentifiers.CanResizeProperty ||
			           property == TransformPatternIdentifiers.CanRotateProperty ||
			           property == ValuePatternIdentifiers.IsReadOnlyProperty ||
			           property == WindowPatternIdentifiers.CanMaximizeProperty ||
			           property == WindowPatternIdentifiers.CanMinimizeProperty ||
			           property == WindowPatternIdentifiers.IsModalProperty ||
			           property == WindowPatternIdentifiers.IsTopmostProperty) {
				bool? boolVal = null;
				if (value == null || !(boolVal = value as bool?).HasValue)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				val = boolVal.Value;
			} else if (property == AEIds.LabeledByProperty ||
			           property == GridItemPatternIdentifiers.ContainingGridProperty ||
			           property == SelectionItemPatternIdentifiers.SelectionContainerProperty) {
				AutomationElement element = null;
				if (value != null && (element = value as AutomationElement) == null)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				if (element != null)
					val = element.GetRuntimeId ();
			} else if (property == AEIds.NativeWindowHandleProperty ||
			           property == AEIds.ProcessIdProperty ||
			           property == GridItemPatternIdentifiers.ColumnProperty ||
			           property == GridItemPatternIdentifiers.ColumnSpanProperty ||
			           property == GridItemPatternIdentifiers.RowProperty ||
			           property == GridItemPatternIdentifiers.RowSpanProperty ||
			           property == GridPatternIdentifiers.ColumnCountProperty ||
			           property == GridPatternIdentifiers.RowCountProperty ||
			           property == MultipleViewPatternIdentifiers.CurrentViewProperty) {
				int? intVal = null;
				if (value == null || !(intVal = value as int?).HasValue)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				val = intVal.Value;
			} else if (property == AEIds.OrientationProperty) {
				OrientationType? orientation = null;
				if (value == null || !(orientation = value as OrientationType?).HasValue)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				val = orientation.Value;
			} else if (property == AEIds.RuntimeIdProperty ||
			           property == MultipleViewPatternIdentifiers.SupportedViewsProperty) {
				int [] runtimeId = null;
				if (value != null && (runtimeId = value as int []) == null)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				if (runtimeId != null)
					val = runtimeId;
			} else if (property == DockPatternIdentifiers.DockPositionProperty) {
				DockPosition? position = null;
				if (value == null || !(position = value as DockPosition?).HasValue)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				val = position.Value;
			} else if (property == ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty) {
				ExpandCollapseState? state = null;
				if (value == null || !(state = value as ExpandCollapseState?).HasValue)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				val = state.Value;
			} else if (property == RangeValuePatternIdentifiers.LargeChangeProperty ||
			           property == RangeValuePatternIdentifiers.SmallChangeProperty ||
			           property == RangeValuePatternIdentifiers.ValueProperty ||
			           property == ScrollPatternIdentifiers.HorizontalScrollPercentProperty ||
			           property == ScrollPatternIdentifiers.HorizontalViewSizeProperty ||
			           property == ScrollPatternIdentifiers.VerticalScrollPercentProperty ||
			           property == ScrollPatternIdentifiers.VerticalViewSizeProperty) {
				double? doubleVal = null;
				if (value == null || !(doubleVal = value as double?).HasValue)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				val = doubleVal.Value;
			} else if (property == RangeValuePatternIdentifiers.MaximumProperty ||
			           property == RangeValuePatternIdentifiers.MinimumProperty) {
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase &&
				    !(value is string))
					throw new ArgumentException ("flags");
				val = value;
			} else if (property == SelectionPatternIdentifiers.SelectionProperty ||
			           property == TableItemPatternIdentifiers.ColumnHeaderItemsProperty ||
			           property == TableItemPatternIdentifiers.RowHeaderItemsProperty ||
			           property == TablePatternIdentifiers.ColumnHeadersProperty ||
			           property == TablePatternIdentifiers.RowHeadersProperty) {
				AutomationElement [] elements = null;
				if (value != null && (elements = value as AutomationElement []) == null)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				if (elements != null)
					val = elements;
			} else if (property == TablePatternIdentifiers.RowOrColumnMajorProperty) {
				RowOrColumnMajor? state = null;
				if (value == null || !(state = value as RowOrColumnMajor?).HasValue)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				val = state.Value;
			} else if (property == TogglePatternIdentifiers.ToggleStateProperty) {
				ToggleState? state = null;
				if (value == null || !(state = value as ToggleState?).HasValue)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				val = state.Value;
			} else if (property == WindowPatternIdentifiers.WindowInteractionStateProperty) {
				WindowInteractionState? state = null;
				if (value == null || !(state = value as WindowInteractionState?).HasValue)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				val = state.Value;
			} else if (property == WindowPatternIdentifiers.WindowVisualStateProperty) {
				WindowVisualState? state = null;
				if (value == null || !(state = value as WindowVisualState?).HasValue)
					throw new ArgumentException ("value");
				if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase)
					throw new ArgumentException ("flags");
				val = state.Value;
			}

			this.flags = flags;
		}

		public PropertyCondition (AutomationProperty property, object value) :
			this (property, value, PropertyConditionFlags.None)
		{
		}

		public AutomationProperty Property {
			get {
				return property;
			}
		}

		public object Value {
			get {
				return val;
			}
		}

		public PropertyConditionFlags Flags {
			get {
				return flags;
			}
		}

		internal override bool AppliesTo (AutomationElement element)
		{
			// TODO: Test caching behavior
			object currentVal = element.GetCurrentPropertyValue (property);
			object conditionVal = val;

			if (currentVal == null || conditionVal == null)
				return currentVal == conditionVal;

			// Compare AutomationElements against Condition's
			// stored runtime ID array
			if (property == AEIds.LabeledByProperty ||
			    property == GridItemPatternIdentifiers.ContainingGridProperty ||
			    property == SelectionItemPatternIdentifiers.SelectionContainerProperty) {
				AutomationElement elementVal =
					currentVal as AutomationElement;
				int [] conditionId = conditionVal as int [];
				return currentVal != null &&
					conditionId != null &&
					Automation.Compare (conditionId,
					                    elementVal.GetRuntimeId ());
			}

			// For some other properties, need to reconstruct proper
			// object for comparison
			if (property == AEIds.BoundingRectangleProperty) {
				double [] rectArray = (double []) val;
				conditionVal = new Rect (rectArray [0],
				                         rectArray [1],
				                         rectArray [2],
				                         rectArray [3]);
			} else if (property == AEIds.ClickablePointProperty) {
				double [] pointArray = (double []) val;
				conditionVal = new Point (pointArray [0],
				                          pointArray [1]);
			} else if (property == AEIds.ControlTypeProperty)
				conditionVal = ControlType.LookupById ((int) val);
			else if (property == AEIds.CultureProperty)
				conditionVal = new CultureInfo ((int) val);

			return ArePropertyValuesEqual (conditionVal, currentVal);
		}

		private bool ArePropertyValuesEqual (object val1, object val2)
		{
			if (val1 == null || val2 == null)
				return val1 == val2;

			if ((flags & PropertyConditionFlags.IgnoreCase) == PropertyConditionFlags.IgnoreCase) {
				string string1 = val1 as string;
				string string2 = val2 as string;
				if (string1 != null && string2 != null)
					return string1.Equals (string2, StringComparison.OrdinalIgnoreCase);
			}

			// Custom equality checking for certain types
			Array array1 = val1 as Array;
			Array array2 = val2 as Array;
			if (array1 != null && array2 != null) {
				if (array1.Length != array2.Length)
					return false;
				// NOTE: Assuming ordering requirement
				for (int i = 0; i < array1.Length; i++)
					if (!ArePropertyValuesEqual (array1.GetValue (i), array2.GetValue (i)))
						return false;
				return true;
			}

			return val1.Equals (val2);
		}
	}
}
