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
			object source = sourceElement.GetCurrentPattern (pattern);
			if (source == null)
				throw new NotSupportedException ();

			if (pattern == DockPatternIdentifiers.Pattern)
				return new DockPattern ((IDockProvider)source);
			else if (pattern == ExpandCollapsePatternIdentifiers.Pattern)
				return new ExpandCollapsePattern ((IExpandCollapseProvider)source);
			else if (pattern == GridItemPatternIdentifiers.Pattern)
				return new GridItemPattern ((IGridItemPattern)source);
			else if (pattern == GridPatternIdentifiers.Pattern)
				return new GridPattern ((IGridPattern)source);
			else if (pattern == InvokePatternIdentifiers.Pattern)
				return new InvokePattern ((IInvokeProvider)source);
			else if (pattern == MultipleViewPatternIdentifiers.Pattern)
				return new MultipleViewPattern ((IMultipleViewPattern)source);
			else if (pattern == RangeValuePatternIdentifiers.Pattern)
				return new RangeValuePattern ((IRangeValuePattern)source);
			else if (pattern == ScrollItemPatternIdentifiers.Pattern)
				return new ScrollItemPattern ((IScrollItemProvider)source);
			else if (pattern == ScrollPatternIdentifiers.Pattern)
				return new ScrollPattern ((IScrollPattern)source);
			else if (pattern == SelectionItemPatternIdentifiers.Pattern)
				return new SelectionItemPattern ((ISelectionItemPattern)source);
			else if (pattern == SelectionPatternIdentifiers.Pattern)
				return new SelectionPattern ((ISelectionPattern)source);
			else if (pattern == TableItemPatternIdentifiers.Pattern)
				return new TableItemPattern ((ITableItemPattern)source);
			else if (pattern == TablePatternIdentifiers.Pattern)
				return new TablePattern ((ITablePattern)source);
			else if (pattern == TextPatternIdentifiers.Pattern)
				return new TextPattern ((ITextPattern)source);
			else if (pattern == TogglePatternIdentifiers.Pattern)
				return new TogglePattern ((IToggleProvider)source);
			else if (pattern == TransformPatternIdentifiers.Pattern)
				return new TransformPattern ((ITransformPattern)source);
			else if (pattern == ValuePatternIdentifiers.Pattern)
				return new ValuePattern ((IValuePattern)source);
			else if (pattern == WindowPatternIdentifiers.Pattern)
				return new WindowPattern ((IWindowPattern)source);
			else
				throw new ArgumentException ();
		}

		public Object GetCurrentPropertyValue (AutomationProperty property)
		{
			// TODO: Throw ElementNotAvailableException if element no longer exists
			return GetCurrentPropertyValue (property, false);
		}

		public Object GetCurrentPropertyValue (AutomationProperty property,
		                                       bool ignoreDefaultValue)
		{
			object pattern;

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
			else if (property == AEIds.IsDockPatternAvailableProperty)
				return TryGetCurrentPattern (DockPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsEnabledProperty)
				return current.IsEnabled;
			else if (property == AEIds.IsExpandCollapsePatternAvailableProperty)
				return TryGetCurrentPattern (ExpandCollapsePatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsGridItemPatternAvailableProperty)
				return TryGetCurrentPattern (GridItemPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsGridPatternAvailableProperty)
				return TryGetCurrentPattern (GridPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsInvokePatternAvailableProperty)
				return TryGetCurrentPattern (InvokePatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsKeyboardFocusableProperty)
				return current.IsKeyboardFocusable;
			else if (property == AEIds.IsMultipleViewPatternAvailableProperty)
				return TryGetCurrentPattern (MultipleViewPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsOffscreenProperty)
				return current.IsOffscreen;
			else if (property == AEIds.IsPasswordProperty)
				return current.IsPassword;
			else if (property == AEIds.IsRangeValuePatternAvailableProperty)
				return TryGetCurrentPattern (RangeValuePatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsRequiredForFormProperty)
				return current.IsRequiredForForm;
			else if (property == AEIds.IsScrollItemPatternAvailableProperty)
				return TryGetCurrentPattern (ScrollItemPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsScrollPatternAvailableProperty)
				return TryGetCurrentPattern (ScrollPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsSelectionItemPatternAvailableProperty)
				return TryGetCurrentPattern (SelectionItemPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsSelectionPatternAvailableProperty)
				return TryGetCurrentPattern (SelectionPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsTableItemPatternAvailableProperty)
				return TryGetCurrentPattern (TableItemPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsTablePatternAvailableProperty)
				return TryGetCurrentPattern (TablePatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsTextPatternAvailableProperty)
				return TryGetCurrentPattern (TextPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsTogglePatternAvailableProperty)
				return TryGetCurrentPattern (TogglePatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsTransformPatternAvailableProperty)
				return TryGetCurrentPattern (TransformPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsValuePatternAvailableProperty)
				return TryGetCurrentPattern (ValuePatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsWindowPatternAvailableProperty)
				return TryGetCurrentPattern (WindowPatternIdentifiers.Pattern, out pattern);
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
			else if (property == DockPatternIdentifiers.DockPositionProperty)
				return (TryGetCurrentPattern (DockPatternIdentifiers.Pattern, out pattern)? ((DockPattern)pattern).Current.DockPosition: DockPosition.None);
			else if (property == ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty)
				return (TryGetCurrentPattern (ExpandCollapsePatternIdentifiers.Pattern, out pattern)? ((ExpandCollapsePattern)pattern).Current.ExpandCollapseState: ExpandCollapseState.LeafNode);
			else if (property == GridItemPatternIdentifiers.ColumnProperty)
				return (TryGetCurrentPattern (GridItemPatternIdentifiers.Pattern, out pattern)? ((GridItemPattern)pattern).Current.Column: 0);
			else if (property == GridItemPatternIdentifiers.ColumnSpanProperty)
				return (TryGetCurrentPattern (GridItemPatternIdentifiers.Pattern, out pattern)? ((GridItemPattern)pattern).Current.ColumnSpan: 1);
			else if (property == GridItemPatternIdentifiers.ContainingGridProperty)
				return (TryGetCurrentPattern (GridItemPatternIdentifiers.Pattern, out pattern)? (object)((GridItemPattern)pattern).Current.ContainingGrid: (object)null);
			else if (property == GridItemPatternIdentifiers.RowProperty)
				return (TryGetCurrentPattern (GridItemPatternIdentifiers.Pattern, out pattern)? ((GridItemPattern)pattern).Current.Row: 0);
			else if (property == GridItemPatternIdentifiers.RowSpanProperty)
				return (TryGetCurrentPattern (GridItemPatternIdentifiers.Pattern, out pattern)? ((GridItemPattern)pattern).Current.RowSpan: 1);
			else if (property == GridPatternIdentifiers.ColumnCountProperty)
				return (TryGetCurrentPattern (GridPatternIdentifiers.Pattern, out pattern)? ((GridPattern)pattern).Current.ColumnCount: 0);
			else if (property == GridPatternIdentifiers.RowCountProperty)
				return (TryGetCurrentPattern (GridPatternIdentifiers.Pattern, out pattern)? ((GridPattern)pattern).Current.RowCount: 0);
			else if (property == MultipleViewPatternIdentifiers.CurrentViewProperty)
				return (TryGetCurrentPattern (MultipleViewPatternIdentifiers.Pattern, out pattern)? ((MultipleViewPattern)pattern).Current.CurrentView: 0);
			else if (property == MultipleViewPatternIdentifiers.SupportedViewsProperty)
				return (TryGetCurrentPattern (MultipleViewPatternIdentifiers.Pattern, out pattern)? ((MultipleViewPattern)pattern).Current.GetSupportedViews (): new int [0]);
			else if (property == RangeValuePatternIdentifiers.IsReadOnlyProperty)
				return (TryGetCurrentPattern (RangeValuePatternIdentifiers.Pattern, out pattern)? ((RangeValuePattern)pattern).Current.IsReadOnly: true);
			else if (property == RangeValuePatternIdentifiers.LargeChangeProperty)
				return (TryGetCurrentPattern (RangeValuePatternIdentifiers.Pattern, out pattern)? ((RangeValuePattern)pattern).Current.LargeChange: 0);
			else if (property == RangeValuePatternIdentifiers.MaximumProperty)
				return (TryGetCurrentPattern (RangeValuePatternIdentifiers.Pattern, out pattern)? ((RangeValuePattern)pattern).Current.Maximum: 0);
			else if (property == RangeValuePatternIdentifiers.MinimumProperty)
				return (TryGetCurrentPattern (RangeValuePatternIdentifiers.Pattern, out pattern)? ((RangeValuePattern)pattern).Current.Minimum: 0);
			else if (property == RangeValuePatternIdentifiers.SmallChangeProperty)
				return (TryGetCurrentPattern (RangeValuePatternIdentifiers.Pattern, out pattern)? ((RangeValuePattern)pattern).Current.SmallChange: 0);
			else if (property == RangeValuePatternIdentifiers.ValueProperty)
				return (TryGetCurrentPattern (RangeValuePatternIdentifiers.Pattern, out pattern)? ((RangeValuePattern)pattern).Current.Value: 0);
			else if (property == ScrollPatternIdentifiers.HorizontallyScrollableProperty)
				return (TryGetCurrentPattern (ScrollPatternIdentifiers.Pattern, out pattern)? ((ScrollPattern)pattern).Current.HorizontallyScrollable: false);
			else if (property == ScrollPatternIdentifiers.HorizontalScrollPercentProperty)
				return (TryGetCurrentPattern (ScrollPatternIdentifiers.Pattern, out pattern)? ((ScrollPattern)pattern).Current.HorizontalScrollPercent: 0);
			else if (property == ScrollPatternIdentifiers.HorizontalViewSizeProperty)
				return (TryGetCurrentPattern (ScrollPatternIdentifiers.Pattern, out pattern)? ((ScrollPattern)pattern).Current.HorizontalViewSize: 100);
			else if (property == ScrollPatternIdentifiers.VerticallyScrollableProperty)
				return (TryGetCurrentPattern (ScrollPatternIdentifiers.Pattern, out pattern)? ((ScrollPattern)pattern).Current.VerticallyScrollable: false);
			else if (property == ScrollPatternIdentifiers.VerticalScrollPercentProperty)
				return (TryGetCurrentPattern (ScrollPatternIdentifiers.Pattern, out pattern)? ((ScrollPattern)pattern).Current.VerticalScrollPercent: 0);
			else if (property == ScrollPatternIdentifiers.VerticalViewSizeProperty)
				return (TryGetCurrentPattern (ScrollPatternIdentifiers.Pattern, out pattern)? ((ScrollPattern)pattern).Current.VerticalViewSize: 100);
			else if (property == SelectionItemPatternIdentifiers.IsSelectedProperty)
				return (TryGetCurrentPattern (SelectionItemPatternIdentifiers.Pattern, out pattern)? ((SelectionItemPattern)pattern).Current.IsSelected: false);
			else if (property == SelectionItemPatternIdentifiers.SelectionContainerProperty)
				return (TryGetCurrentPattern (SelectionItemPatternIdentifiers.Pattern, out pattern)? (object)((SelectionItemPattern)pattern).Current.SelectionContainer: (object)null);
			else if (property == SelectionPatternIdentifiers.CanSelectMultipleProperty)
				return (TryGetCurrentPattern (SelectionPatternIdentifiers.Pattern, out pattern)? ((SelectionPattern)pattern).Current.CanSelectMultiple: false);
			else if (property == SelectionPatternIdentifiers.IsSelectionRequiredProperty)
				return (TryGetCurrentPattern (SelectionPatternIdentifiers.Pattern, out pattern)? ((SelectionPattern)pattern).Current.IsSelectionRequired: false);
			else if (property == SelectionPatternIdentifiers.SelectionProperty)
				return (TryGetCurrentPattern (SelectionPatternIdentifiers.Pattern, out pattern)? ((SelectionPattern)pattern).Current.GetSelection (): new AutomationElement [0]);
			else if (property == TableItemPatternIdentifiers.ColumnHeaderItemsProperty)
				return (TryGetCurrentPattern (TableItemPatternIdentifiers.Pattern, out pattern)? ((TableItemPattern)pattern).Current.GetColumnHeaderItems (): new AutomationElement [0]);
			else if (property == TableItemPatternIdentifiers.RowHeaderItemsProperty)
				return (TryGetCurrentPattern (TableItemPatternIdentifiers.Pattern, out pattern)? ((TableItemPattern)pattern).Current.GetRowHeaderItems (): new AutomationElement [0]);
			else if (property == TablePatternIdentifiers.ColumnHeadersProperty)
				return (TryGetCurrentPattern (TablePatternIdentifiers.Pattern, out pattern)? ((TablePattern)pattern).Current.GetColumnHeaders (): new AutomationElement [0]);
			else if (property == TablePatternIdentifiers.RowHeadersProperty)
				return (TryGetCurrentPattern (TablePatternIdentifiers.Pattern, out pattern)? ((TablePattern)pattern).Current.GetRowHeaders (): new AutomationElement [0]);
			else if (property == TablePatternIdentifiers.RowOrColumnMajorProperty)
				return (TryGetCurrentPattern (TablePatternIdentifiers.Pattern, out pattern)? ((TablePattern)pattern).Current.RowOrColumnMajor: RowOrColumnMajor.Indeterminate);
			else if (property == TogglePatternIdentifiers.ToggleStateProperty)
				return (TryGetCurrentPattern (TogglePatternIdentifiers.Pattern, out pattern)? ((TogglePattern)pattern).Current.ToggleState: ToggleState.Indeterminate);
			else if (property == TransformPatternIdentifiers.CanMoveProperty)
				return (TryGetCurrentPattern (TransformPatternIdentifiers.Pattern, out pattern)? (bool)((TransformPattern)pattern).Current.CanMove: false);
			else if (property == TransformPatternIdentifiers.CanResizeProperty)
				return (TryGetCurrentPattern (TransformPatternIdentifiers.Pattern, out pattern)? ((TransformPattern)pattern).Current.CanResize: false);
			else if (property == TransformPatternIdentifiers.CanRotateProperty)
				return (TryGetCurrentPattern (TransformPatternIdentifiers.Pattern, out pattern)? (object)((TransformPattern)pattern).Current.CanRotate: (object)null);
			else if (property == ValuePatternIdentifiers.IsReadOnlyProperty)
				return (TryGetCurrentPattern (ValuePatternIdentifiers.Pattern, out pattern)? ((ValuePattern)pattern).Current.IsReadOnly: true);
			else if (property == ValuePatternIdentifiers.ValueProperty)
				return (TryGetCurrentPattern (ValuePatternIdentifiers.Pattern, out pattern)? ((ValuePattern)pattern).Current.Value: String.Empty);
			else if (property == WindowPatternIdentifiers.CanMaximizeProperty)
				return (TryGetCurrentPattern (WindowPatternIdentifiers.Pattern, out pattern)? (bool)((WindowPattern)pattern).Current.CanMaximize: false);
			else if (property == WindowPatternIdentifiers.CanMinimizeProperty)
				return (TryGetCurrentPattern (WindowPatternIdentifiers.Pattern, out pattern)? (bool)((WindowPattern)pattern).Current.CanMinimize: false);
			else if (property == WindowPatternIdentifiers.IsModalProperty)
				return (TryGetCurrentPattern (WindowPatternIdentifiers.Pattern, out pattern)? ((WindowPattern)pattern).Current.IsModal: false);
			else if (property == WindowPatternIdentifiers.IsTopmostProperty)
				return (TryGetCurrentPattern (WindowPatternIdentifiers.Pattern, out pattern)? ((WindowPattern)pattern).Current.IsTopmost: false);
			else if (property == WindowPatternIdentifiers.WindowInteractionStateProperty)
				return (TryGetCurrentPattern (WindowPatternIdentifiers.Pattern, out pattern)? ((WindowPattern)pattern).Current.WindowInteractionState: WindowInteractionState.Running);
			else if (property == WindowPatternIdentifiers.WindowVisualStateProperty)
				return (TryGetCurrentPattern (WindowPatternIdentifiers.Pattern, out pattern)? ((WindowPattern)pattern).Current.WindowVisualState: WindowVisualState.Normal);
			Log.Debug ("GetCurrentPropertyValue not implemented for: " + property.ProgrammaticName);
			return NotSupported;
		}

		public int [] GetRuntimeId ()
		{
			return sourceElement.RuntimeId;
		}

		public AutomationPattern [] GetSupportedPatterns ()
		{
			return sourceElement.GetSupportedPatterns ();
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
			try {
				patternObject = sourceElement.GetCurrentPattern (pattern);
				return true;
			} catch (Exception) {
				patternObject = null;
				return false;
			}
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
			    condition.AppliesTo (this))
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
