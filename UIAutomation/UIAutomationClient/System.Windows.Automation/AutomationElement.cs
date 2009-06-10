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
//      Brad Taylor <brad@getcoded.net>
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Collections.Generic;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Services;
using Mono.UIAutomation.Source;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace System.Windows.Automation
{
	public sealed class AutomationElement
	{
#region Private Members
		private AutomationElementInformation current;
		private IElement sourceElement;
#endregion

#region Private Static Members
		private static AutomationElement rootElement;
#endregion

#region Internal Properties
		internal IElement SourceElement {
			get { return sourceElement; }
		}
#endregion

#region Public Properties
		public AutomationElementInformation Cached {
			get { throw new NotImplementedException (); }
		}

		public AutomationElementCollection CachedChildren {
			get { throw new NotImplementedException (); }
		}

		public AutomationElement CachedParent {
			get { throw new NotImplementedException (); }
		}

		public AutomationElementInformation Current {
			get { return current; }
		}
#endregion

#region Public Static Properties
		public static AutomationElement FocusedElement {
			get { throw new NotImplementedException (); }
		}

		public static AutomationElement RootElement {
			get { return rootElement; }
		}
#endregion

#region Static Constructor
		static AutomationElement ()
		{
			rootElement = new AutomationElement (new DesktopElement ());
		}
#endregion

#region Constructor
		internal AutomationElement (IElement sourceElement)
		{
			this.sourceElement = sourceElement;
			current = new AutomationElementInformation (sourceElement);
		}
#endregion

#region Public Methods
		public override bool Equals (Object obj)
		{
			AutomationElement other = obj as AutomationElement;
			if (null == obj)
				return false;

			// MSDN says to compare runtime IDs
			int [] thisId = GetRuntimeId ();
			int [] otherId = other.GetRuntimeId ();
			if (thisId.Length != otherId.Length)
				return false;
			for (int i = 0; i < thisId.Length; i++)
				if (thisId [i] != otherId [i])
					return false;
			return true;
		}

		public AutomationElementCollection FindAll (TreeScope scope, Condition condition)
		{
			List<AutomationElement> found = Find (scope, condition, false);
			return new AutomationElementCollection (found);
		}

		public AutomationElement FindFirst (TreeScope scope, Condition condition)
		{
			List<AutomationElement> found = Find (scope, condition, true);
			if (found.Count > 0)
				return found [0];
			return null;
		}

		public Object GetCachedPattern (AutomationPattern pattern)
		{
			throw new NotImplementedException ();
		}

		public Object GetCachedPropertyValue (AutomationProperty property)
		{
			throw new NotImplementedException ();
		}

		public Object GetCachedPropertyValue (AutomationProperty property,
		                                      bool ignoreDefaultValue)
		{
			throw new NotImplementedException ();
		}

		public override int GetHashCode ()
		{
			int code = 0;
			foreach (int i in GetRuntimeId ())
				code ^= i;
			return code;
		}

		public Point GetClickablePoint ()
		{
			return sourceElement.ClickablePoint;
		}

		public Object GetCurrentPattern (AutomationPattern pattern)
		{
			throw new NotImplementedException ();
		}

		public Object GetCurrentPropertyValue (AutomationProperty property)
		{
			// TODO: Throw ElementNotAvailableException if element no longer exists
			return GetCurrentPropertyValue (property, false);
		}

		public Object GetCurrentPropertyValue (AutomationProperty property,
		                                       bool ignoreDefaultValue)
		{
			// TODO: Throw ElementNotAvailableException if element no longer exists

			// Default values already handled in IElement implementation
			if (ignoreDefaultValue && !sourceElement.SupportsProperty (property))
				return NotSupported;

			if (property == AEIds.AcceleratorKeyProperty)
				return current.AcceleratorKey;
			else if (property == AEIds.AccessKeyProperty)
				return current.AccessKey;
			else if (property == AEIds.AutomationIdProperty)
				return current.AutomationId;
			else if (property == AEIds.BoundingRectangleProperty)
				return current.BoundingRectangle;
			else if (property == AEIds.ClassNameProperty)
				return current.ClassName;
			else if (property == AEIds.ClickablePointProperty) {
				Point clickablePoint;
				if (TryGetClickablePoint (out clickablePoint))
					return clickablePoint;
				return null;
			}
			else if (property == AEIds.ControlTypeProperty)
				return current.ControlType;
			else if (property == AEIds.CultureProperty)
				return null;	// TODO: Implement (new IElement member? not used in UIAutomationWinforms)
			else if (property == AEIds.FrameworkIdProperty)
				return current.FrameworkId;
			else if (property == AEIds.HasKeyboardFocusProperty)
				return current.HasKeyboardFocus;
			else if (property == AEIds.HelpTextProperty)
				return current.HelpText;
			else if (property == AEIds.IsContentElementProperty)
				return current.IsContentElement;
			else if (property == AEIds.IsControlElementProperty)
				return current.IsControlElement;
			// TODO: Impelement for AEIds.Is*PatternAvailableProperty
//			else if (property == AEIds.IsDockPatternAvailableProperty)
//				return "IsDockPatternAvailable";
			else if (property == AEIds.IsEnabledProperty)
				return current.IsEnabled;
//			else if (property == AEIds.IsExpandCollapsePatternAvailableProperty)
//				return "IsExpandCollapsePatternAvailable";
//			else if (property == AEIds.IsGridItemPatternAvailableProperty)
//				return "IsGridItemPatternAvailable";
//			else if (property == AEIds.IsGridPatternAvailableProperty)
//				return "IsGridPatternAvailable";
//			else if (property == AEIds.IsInvokePatternAvailableProperty)
//				return "IsInvokePatternAvailable";
			else if (property == AEIds.IsKeyboardFocusableProperty)
				return current.IsKeyboardFocusable;
//			else if (property == AEIds.IsMultipleViewPatternAvailableProperty)
//				return "IsMultipleViewPatternAvailable";
			else if (property == AEIds.IsOffscreenProperty)
				return current.IsOffscreen;
			else if (property == AEIds.IsPasswordProperty)
				return current.IsPassword;
//			else if (property == AEIds.IsRangeValuePatternAvailableProperty)
//				return "IsRangeValuePatternAvailable";
			else if (property == AEIds.IsRequiredForFormProperty)
				return current.IsRequiredForForm;
//			else if (property == AEIds.IsScrollItemPatternAvailableProperty)
//				return "IsScrollItemPatternAvailable";
//			else if (property == AEIds.IsScrollPatternAvailableProperty)
//				return "IsScrollPatternAvailable";
//			else if (property == AEIds.IsSelectionItemPatternAvailableProperty)
//				return "IsSelectionItemPatternAvailable";
//			else if (property == AEIds.IsSelectionPatternAvailableProperty)
//				return "IsSelectionPatternAvailable";
//			else if (property == AEIds.IsTableItemPatternAvailableProperty)
//				return "IsTableItemPatternAvailable";
//			else if (property == AEIds.IsTablePatternAvailableProperty)
//				return "IsTablePatternAvailable";
//			else if (property == AEIds.IsTextPatternAvailableProperty)
//				return "IsTextPatternAvailable";
//			else if (property == AEIds.IsTogglePatternAvailableProperty)
//				return "IsTogglePatternAvailable";
//			else if (property == AEIds.IsTransformPatternAvailableProperty)
//				return "IsTransformPatternAvailable";
//			else if (property == AEIds.IsValuePatternAvailableProperty)
//				return "IsValuePatternAvailable";
//			else if (property == AEIds.IsWindowPatternAvailableProperty)
//				return "IsWindowPatternAvailable";
			else if (property == AEIds.ItemStatusProperty)
				return current.ItemStatus;
			else if (property == AEIds.ItemTypeProperty)
				return current.ItemType;
			else if (property == AEIds.LabeledByProperty)
				return current.LabeledBy;
			else if (property == AEIds.LocalizedControlTypeProperty)
				return current.LocalizedControlType;
			else if (property == AEIds.NameProperty)
				return current.Name;
			else if (property == AEIds.NativeWindowHandleProperty)
				return current.NativeWindowHandle;
			else if (property == AEIds.OrientationProperty)
				return current.Orientation;
			else if (property == AEIds.ProcessIdProperty)
				return current.ProcessId;
			else if (property == AEIds.RuntimeIdProperty)
				return GetRuntimeId ();
			// TODO: Impelement for pattern properties
//			else if (property == DockPatternIdentifiers.DockPositionProperty)
//				return "DockPosition";
//			else if (property == ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty)
//				return "ExpandCollapseState";
//			else if (property == GridItemPatternIdentifiers.ColumnProperty)
//				return "Column";
//			else if (property == GridItemPatternIdentifiers.ColumnSpanProperty)
//				return "ColumnSpan";
//			else if (property == GridItemPatternIdentifiers.ContainingGridProperty)
//				return "ContainingGrid";
//			else if (property == GridItemPatternIdentifiers.RowProperty)
//				return "Row";
//			else if (property == GridItemPatternIdentifiers.RowSpanProperty)
//				return "RowSpan";
//			else if (property == GridPatternIdentifiers.ColumnCountProperty)
//				return "ColumnCount";
//			else if (property == GridPatternIdentifiers.RowCountProperty)
//				return "RowCount";
//			else if (property == MultipleViewPatternIdentifiers.CurrentViewProperty)
//				return "CurrentView";
//			else if (property == MultipleViewPatternIdentifiers.SupportedViewsProperty)
//				return "SupportedViews";
//			else if (property == RangeValuePatternIdentifiers.IsReadOnlyProperty)
//				return "IsReadOnly";
//			else if (property == RangeValuePatternIdentifiers.LargeChangeProperty)
//				return "LargeChange";
//			else if (property == RangeValuePatternIdentifiers.MaximumProperty)
//				return "Maximum";
//			else if (property == RangeValuePatternIdentifiers.MinimumProperty)
//				return "Minimum";
//			else if (property == RangeValuePatternIdentifiers.SmallChangeProperty)
//				return "SmallChange";
//			else if (property == RangeValuePatternIdentifiers.ValueProperty)
//				return "Value";
//			else if (property == ScrollPatternIdentifiers.HorizontallyScrollableProperty)
//				return "HorizontallyScrollable";
//			else if (property == ScrollPatternIdentifiers.HorizontalScrollPercentProperty)
//				return "HorizontalScrollPercent";
//			else if (property == ScrollPatternIdentifiers.HorizontalViewSizeProperty)
//				return "HorizontalViewSize";
//			else if (property == ScrollPatternIdentifiers.VerticallyScrollableProperty)
//				return "VerticallyScrollable";
//			else if (property == ScrollPatternIdentifiers.VerticalScrollPercentProperty)
//				return "VerticalScrollPercent";
//			else if (property == ScrollPatternIdentifiers.VerticalViewSizeProperty)
//				return "VerticalViewSize";
//			else if (property == SelectionItemPatternIdentifiers.IsSelectedProperty)
//				return "IsSelected";
//			else if (property == SelectionItemPatternIdentifiers.SelectionContainerProperty)
//				return "SelectionContainer";
//			else if (property == SelectionPatternIdentifiers.CanSelectMultipleProperty)
//				return "CanSelectMultiple";
//			else if (property == SelectionPatternIdentifiers.IsSelectionRequiredProperty)
//				return "IsSelectionRequired";
//			else if (property == SelectionPatternIdentifiers.SelectionProperty)
//				return "Selection";
//			else if (property == TableItemPatternIdentifiers.ColumnHeaderItemsProperty)
//				return "ColumnHeaderItems";
//			else if (property == TableItemPatternIdentifiers.RowHeaderItemsProperty)
//				return "RowHeaderItems";
//			else if (property == TablePatternIdentifiers.ColumnHeadersProperty)
//				return "ColumnHeaders";
//			else if (property == TablePatternIdentifiers.RowHeadersProperty)
//				return "RowHeaders";
//			else if (property == TablePatternIdentifiers.RowOrColumnMajorProperty)
//				return "RowOrColumnMajor";
//			else if (property == TogglePatternIdentifiers.ToggleStateProperty)
//				return "ToggleState";
//			else if (property == TransformPatternIdentifiers.CanMoveProperty)
//				return "CanMove";
//			else if (property == TransformPatternIdentifiers.CanResizeProperty)
//				return "CanResize";
//			else if (property == TransformPatternIdentifiers.CanRotateProperty)
//				return "CanRotate";
//			else if (property == ValuePatternIdentifiers.IsReadOnlyProperty)
//				return "IsReadOnly";
//			else if (property == ValuePatternIdentifiers.ValueProperty)
//				return "Value";
//			else if (property == WindowPatternIdentifiers.CanMaximizeProperty)
//				return "CanMaximize";
//			else if (property == WindowPatternIdentifiers.CanMinimizeProperty)
//				return "CanMinimize";
//			else if (property == WindowPatternIdentifiers.IsModalProperty)
//				return "IsModal";
//			else if (property == WindowPatternIdentifiers.IsTopmostProperty)
//				return "IsTopmost";
//			else if (property == WindowPatternIdentifiers.WindowInteractionStateProperty)
//				return "WindowInteractionState";
//			else if (property == WindowPatternIdentifiers.WindowVisualStateProperty)
//				return "WindowVisualState";
			Log.Debug ("GetCurrentPropertyValue not implemented for: " + property.ProgrammaticName);
			return NotSupported;
		}

		public int [] GetRuntimeId ()
		{
			return sourceElement.RuntimeId;
		}

		public AutomationPattern [] GetSupportedPatterns ()
		{
			return new AutomationPattern [] {};
		}

		public AutomationProperty [] GetSupportedProperties ()
		{
			throw new NotImplementedException ();
		}

		public AutomationElement GetUpdatedCache (CacheRequest request)
		{
			throw new NotImplementedException ();
		}

		public void SetFocus ()
		{
			throw new NotImplementedException ();
		}

		public bool TryGetCachedPattern (AutomationPattern pattern,
		                                 out Object patternObject)
		{
			// TODO: Implement
			patternObject = null;
			return false;
		}

		public bool TryGetClickablePoint (out Point pt)
		{
			// TODO: Use this as sentinel value on dbus?
			pt = new Point (double.NegativeInfinity, double.NegativeInfinity);
			return false;
		}

		public bool TryGetCurrentPattern (AutomationPattern pattern,
		                                  out Object patternObject)
		{
			// TODO: Implement
			patternObject = null;
			return false;
		}
#endregion

#region Public Static Methods
		public static AutomationElement FromHandle (IntPtr hwnd)
		{
			throw new NotImplementedException ();
		}

		public static AutomationElement FromLocalProvider (IRawElementProviderSimple localImpl)
		{
			throw new NotImplementedException ();
		}

		public static AutomationElement FromPoint (Point pt)
		{
			throw new NotImplementedException ();
		}

		public static bool operator == (AutomationElement left,
		                                AutomationElement right)
		{
			return object.Equals (left, right);
		}

		public static bool operator != (AutomationElement left,
		                                AutomationElement right)
		{
			return !(left == right);
		}
#endregion

#region Private Methods
		private List<AutomationElement> Find (TreeScope scope, Condition condition, bool findFirst)
		{
			// Parent and Ancestors scopes are not supported on
			// Windows (this is specified in MSDN, too).
			if ((scope & TreeScope.Parent) == TreeScope.Parent ||
			    (scope & TreeScope.Ancestors) == TreeScope.Ancestors)
				throw new ArgumentException ();

			List<AutomationElement> found = new List<AutomationElement> ();

			if ((!findFirst || found.Count == 0) &&
			    (scope & TreeScope.Element) == TreeScope.Element &&
			    ElementMeetsCondition (this, condition))
				found.Add (this);

			if ((!findFirst || found.Count == 0) &&
			    ((scope & TreeScope.Children) == TreeScope.Children ||
			    (scope & TreeScope.Descendants) == TreeScope.Descendants)) {
				TreeScope childScope = TreeScope.Element;
				if ((scope & TreeScope.Descendants) == TreeScope.Descendants)
					childScope = TreeScope.Subtree;
				AutomationElement current =
					TreeWalker.RawViewWalker.GetFirstChild (this);
				while (current != null) {
					//Log.Debug ("Inspecting child: " + current.Current.Name);
					found.AddRange (current.Find (childScope, condition, findFirst));
					if (findFirst && found.Count > 0)
						break;
					current = TreeWalker.RawViewWalker.GetNextSibling (current);
				}
			}

			return found;
		}

		private static bool ElementMeetsCondition (AutomationElement element, Condition condition)
		{
			PropertyCondition propertyCond = condition as PropertyCondition;
			if (propertyCond != null) {
				// TODO: Test caching behavior
				object currentVal = element.GetCurrentPropertyValue (propertyCond.Property);
				return ArePropertyValuesEqual (propertyCond.Value, currentVal);
			}

			NotCondition notCond = condition as NotCondition;
			if (notCond != null) {
				return !ElementMeetsCondition (element, notCond.Condition);
			}

			AndCondition andCond = condition as AndCondition;
			if (andCond != null) {
				foreach (Condition cond in andCond.GetConditions ())
					if (!ElementMeetsCondition (element, cond))
						return false;
				return true;
			}

			OrCondition orCond = condition as OrCondition;
			if (orCond != null) {
				foreach (Condition cond in orCond.GetConditions ())
					if (ElementMeetsCondition (element, cond))
						return true;
				return false;
			}

			return false;
		}

		private static bool ArePropertyValuesEqual (object val1, object val2)
		{
			if (val1 == null || val2 == null)
				return val1 == val2;

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

		// Useful for debugging statements, currently unused
//		private static string PropertyValueToString (AutomationProperty prop, object val)
//		{
//			if (val == null)
//				return "(null)";
//			if (prop == AEIds.RuntimeIdProperty && val is int [])
//				return PrintRuntimeId ((int []) val);
//			return val.ToString ();
//		}
//
//		private static string PrintRuntimeId (int [] runtimeId)
//		{
//			string output = "{";
//			for (int i = 0; i < runtimeId.Length; i++) {
//				output += runtimeId [i].ToString ();
//				if ((i+1) < runtimeId.Length)
//					output += ",";
//			}
//			return output + "}";
//		}
#endregion

#region public structures
		public struct AutomationElementInformation
		{
			internal AutomationElementInformation (IElement sourceElement)
			{
				this.AcceleratorKey = sourceElement.AcceleratorKey;
				this.AccessKey = sourceElement.AccessKey;
				this.AutomationId = sourceElement.AutomationId;
				this.BoundingRectangle = sourceElement.BoundingRectangle;
				this.ClassName = sourceElement.ClassName;
				this.ControlType = sourceElement.ControlType;
				this.FrameworkId = sourceElement.FrameworkId;
				this.HasKeyboardFocus = sourceElement.HasKeyboardFocus;
				this.HelpText = sourceElement.HelpText;
				this.IsContentElement = sourceElement.IsContentElement;
				this.IsControlElement = sourceElement.IsControlElement;
				this.IsEnabled = sourceElement.IsEnabled;
				this.IsKeyboardFocusable = sourceElement.IsKeyboardFocusable;
				this.IsOffscreen = sourceElement.IsOffscreen;
				this.IsPassword = sourceElement.IsPassword;
				this.IsRequiredForForm = sourceElement.IsRequiredForForm;
				this.ItemStatus = sourceElement.ItemStatus;
				this.ItemType = sourceElement.ItemType;
				this.LabeledBy = SourceManager.GetOrCreateAutomationElement (sourceElement.LabeledBy);
				this.LocalizedControlType = sourceElement.LocalizedControlType;
				this.Name = sourceElement.Name;
				this.NativeWindowHandle = sourceElement.NativeWindowHandle;
				this.Orientation = sourceElement.Orientation;
				this.ProcessId = sourceElement.ProcessId;
			}

			public string AcceleratorKey {
				get; private set;
			}

			public string AccessKey {
				get; private set;
			}

			public string AutomationId {
				get; private set;
			}

			public Rect BoundingRectangle {
				get; private set;
			}

			public string ClassName {
				get; private set;
			}

			public ControlType ControlType {
				get; private set;
			}

			public string FrameworkId {
				get; private set;
			}

			public bool HasKeyboardFocus {
				get; private set;
			}

			public string HelpText {
				get; private set;
			}

			public bool IsContentElement {
				get; private set;
			}

			public bool IsControlElement {
				get; private set;
			}

			public bool IsEnabled {
				get; private set;
			}

			public bool IsKeyboardFocusable {
				get; private set;
			}

			public bool IsOffscreen {
				get; private set;
			}

			public bool IsPassword {
				get; private set;
			}

			public bool IsRequiredForForm {
				get; private set;
			}

			public string ItemStatus {
				get; private set;
			}

			public string ItemType {
				get; private set;
			}

			public AutomationElement LabeledBy {
				get; private set;
			}

			public string LocalizedControlType {
				get; private set;
			}

			public string Name {
				get; private set;
			}

			public int NativeWindowHandle {
				get; private set;
			}

			public OrientationType Orientation {
				get; private set;
			}

			public int ProcessId {
				get; private set;
			}
		}
#endregion

#region Public Static ReadOnly Fields

		public static readonly Object NotSupported = AEIds.NotSupported;

		public static readonly AutomationProperty AcceleratorKeyProperty = AEIds.AcceleratorKeyProperty;

		public static readonly AutomationProperty AccessKeyProperty = AEIds.AccessKeyProperty;

		public static readonly AutomationProperty AutomationIdProperty = AEIds.AutomationIdProperty;

		public static readonly AutomationProperty BoundingRectangleProperty = AEIds.BoundingRectangleProperty;

		public static readonly AutomationProperty ClassNameProperty = AEIds.ClassNameProperty;

		public static readonly AutomationProperty ClickablePointProperty = AEIds.ClickablePointProperty;

		public static readonly AutomationProperty ControlTypeProperty = AEIds.ControlTypeProperty;

		public static readonly AutomationProperty CultureProperty = AEIds.CultureProperty;

		public static readonly AutomationProperty FrameworkIdProperty = AEIds.FrameworkIdProperty;

		public static readonly AutomationProperty HasKeyboardFocusProperty = AEIds.HasKeyboardFocusProperty;

		public static readonly AutomationProperty HelpTextProperty = AEIds.HelpTextProperty;

		public static readonly AutomationProperty IsContentElementProperty = AEIds.IsContentElementProperty;

		public static readonly AutomationProperty IsControlElementProperty = AEIds.IsControlElementProperty;

		public static readonly AutomationProperty IsDockPatternAvailableProperty = AEIds.IsDockPatternAvailableProperty;

		public static readonly AutomationProperty IsEnabledProperty = AEIds.IsEnabledProperty;

		public static readonly AutomationProperty IsExpandCollapsePatternAvailableProperty = AEIds.IsExpandCollapsePatternAvailableProperty;

		public static readonly AutomationProperty IsGridItemPatternAvailableProperty = AEIds.IsGridItemPatternAvailableProperty;

		public static readonly AutomationProperty IsGridPatternAvailableProperty = AEIds.IsGridPatternAvailableProperty;

		public static readonly AutomationProperty IsInvokePatternAvailableProperty = AEIds.IsInvokePatternAvailableProperty;

		public static readonly AutomationProperty IsKeyboardFocusableProperty = AEIds.IsKeyboardFocusableProperty;

		public static readonly AutomationProperty IsMultipleViewPatternAvailableProperty = AEIds.IsMultipleViewPatternAvailableProperty;

		public static readonly AutomationProperty IsOffscreenProperty = AEIds.IsOffscreenProperty;

		public static readonly AutomationProperty IsPasswordProperty = AEIds.IsPasswordProperty;

		public static readonly AutomationProperty IsRangeValuePatternAvailableProperty = AEIds.IsRangeValuePatternAvailableProperty;

		public static readonly AutomationProperty IsRequiredForFormProperty = AEIds.IsRequiredForFormProperty;

		public static readonly AutomationProperty IsScrollItemPatternAvailableProperty = AEIds.IsScrollItemPatternAvailableProperty;

		public static readonly AutomationProperty IsScrollPatternAvailableProperty = AEIds.IsScrollPatternAvailableProperty;

		public static readonly AutomationProperty IsSelectionItemPatternAvailableProperty = AEIds.IsSelectionItemPatternAvailableProperty;

		public static readonly AutomationProperty IsSelectionPatternAvailableProperty = AEIds.IsSelectionPatternAvailableProperty;

		public static readonly AutomationProperty IsTableItemPatternAvailableProperty = AEIds.IsTableItemPatternAvailableProperty;

		public static readonly AutomationProperty IsTablePatternAvailableProperty = AEIds.IsTablePatternAvailableProperty;

		public static readonly AutomationProperty IsTextPatternAvailableProperty = AEIds.IsTextPatternAvailableProperty;

		public static readonly AutomationProperty IsTogglePatternAvailableProperty = AEIds.IsTogglePatternAvailableProperty;

		public static readonly AutomationProperty IsTransformPatternAvailableProperty = AEIds.IsTransformPatternAvailableProperty;

		public static readonly AutomationProperty IsValuePatternAvailableProperty = AEIds.IsValuePatternAvailableProperty;

		public static readonly AutomationProperty IsWindowPatternAvailableProperty = AEIds.IsWindowPatternAvailableProperty;

		public static readonly AutomationProperty ItemStatusProperty = AEIds.ItemStatusProperty;

		public static readonly AutomationProperty ItemTypeProperty = AEIds.ItemTypeProperty;

		public static readonly AutomationProperty LabeledByProperty = AEIds.LabeledByProperty;

		public static readonly AutomationProperty LocalizedControlTypeProperty = AEIds.LocalizedControlTypeProperty;

		public static readonly AutomationProperty NameProperty = AEIds.NameProperty;

		public static readonly AutomationProperty NativeWindowHandleProperty = AEIds.NativeWindowHandleProperty;

		public static readonly AutomationProperty OrientationProperty = AEIds.OrientationProperty;

		public static readonly AutomationProperty ProcessIdProperty = AEIds.ProcessIdProperty;

		public static readonly AutomationProperty RuntimeIdProperty = AEIds.RuntimeIdProperty;

		public static readonly AutomationEvent AsyncContentLoadedEvent = AEIds.AsyncContentLoadedEvent;

		public static readonly AutomationEvent AutomationFocusChangedEvent = AEIds.AutomationFocusChangedEvent;

		public static readonly AutomationEvent AutomationPropertyChangedEvent = AEIds.AutomationPropertyChangedEvent;

		public static readonly AutomationEvent LayoutInvalidatedEvent = AEIds.LayoutInvalidatedEvent;

		public static readonly AutomationEvent MenuClosedEvent = AEIds.MenuClosedEvent;

		public static readonly AutomationEvent MenuOpenedEvent = AEIds.MenuOpenedEvent;

		public static readonly AutomationEvent StructureChangedEvent = AEIds.StructureChangedEvent;

		public static readonly AutomationEvent ToolTipClosedEvent = AEIds.ToolTipClosedEvent;

		public static readonly AutomationEvent ToolTipOpenedEvent = AEIds.ToolTipOpenedEvent;

#endregion
	}
}
