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
	public sealed partial class AutomationElement
	{
#region Private Members
		private AutomationElementInformation current;
		private AutomationElementInformation cached;
		private IElement sourceElement;
		private Dictionary<int, CachedValue> propertyCache;
		private AutomationElementMode mode;
		private List<AutomationElement> cachedChildren;
		private AutomationElement cachedParent;
#endregion

#region Private Static Members
		private static AutomationElement rootElement;
		private static AutomationElement focusedElement;
		private static bool isFocusedElementInitialized = false;

		internal static void OnFocusChanged (object sender, AutomationFocusChangedEventArgs e)
		{
			AutomationElement ae = sender as AutomationElement;
			if (ae == null)
				return;
			bool hasFocus = false;
			try {
				hasFocus = ae.Current.HasKeyboardFocus;
				if ((!hasFocus) && ae == focusedElement)
					focusedElement = null;
				else
					focusedElement = ae;
			} catch (ElementNotAvailableException) {
				focusedElement = null;
			} catch (Exception ex) {
				Log.Error ("[OnFocusChanged] Unknown Error: {0}", ex);
				focusedElement = null;
			}
		}
#endregion

#region Internal Properties
		internal IElement SourceElement {
			get { return sourceElement; }
		}

		internal CacheRequest CacheRequest {
			get; private set;
		}
#endregion

#region Public Properties
		public AutomationElementInformation Cached {
			get { return cached; }
		}

		public AutomationElementCollection CachedChildren {
			get {
				if (cachedChildren == null)
					throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
				return new AutomationElementCollection (cachedChildren);
			}
		}

		public AutomationElement CachedParent {
			get {
				if (cachedParent == null)
					throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
				return cachedParent;
			}
		}

		public AutomationElementInformation Current {
			get { return current; }
		}
#endregion

#region Public Static Properties
		public static AutomationElement FocusedElement {
			get {
				if (!isFocusedElementInitialized) {
					foreach (var source in SourceManager.GetAutomationSources ()) {
						var element = source.GetFocusedElement ();
						if (element != null) {
							focusedElement = SourceManager.GetOrCreateAutomationElement (element);
							break;
						}
					}
					Automation.AddAutomationFocusChangedEventHandler (OnFocusChanged);
					isFocusedElementInitialized = true;
				}
				return focusedElement;
			}
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
			current = new AutomationElementInformation (this, false);
			cached = new AutomationElementInformation (this, true);
			propertyCache = new Dictionary<int, CachedValue> ();
			mode = AutomationElementMode.Full;
		}
#endregion

#region Public Methods
		public override bool Equals (Object obj)
		{
			AutomationElement other = obj as AutomationElement;
			if (null == other)
				return false;

			return Automation.Compare (this, other);
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
			return GetPattern (pattern, true);
		}

		public Object GetCachedPropertyValue (AutomationProperty property)
		{
			return GetCachedPropertyValue (property, false);
		}

		public Object GetCachedPropertyValue (AutomationProperty property,
		                                      bool ignoreDefaultValue)
		{
			CachedValue val = null;
			bool found = propertyCache.TryGetValue (property.Id,
			                                        out val);
			if (!found)
				throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
			if (ignoreDefaultValue && !val.IsSupported)
				return NotSupported;
			return val.Value;
		}

		public override int GetHashCode ()
		{
			int code = 1;
			foreach (int i in GetRuntimeId ())
				code = 31 * code + i;
			return code;
		}

		public Point GetClickablePoint ()
		{
			return sourceElement.ClickablePoint;
		}

		public Object GetCurrentPattern (AutomationPattern pattern)
		{
			return GetPattern (pattern, false);
		}

		public Object GetCurrentPropertyValue (AutomationProperty property)
		{
			// TODO: Throw ElementNotAvailableException if element no longer exists
			return GetCurrentPropertyValue (property, false);
		}

		public Object GetCurrentPropertyValue (AutomationProperty property,
		                                       bool ignoreDefaultValue)
		{
			// TODO: Is this tested? Also, add message
			if (mode == AutomationElementMode.None)
				throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");

			object pattern;

			// TODO: Throw ElementNotAvailableException if element no longer exists

			// Default values already handled in IElement implementation
			if (ignoreDefaultValue && !sourceElement.SupportsProperty (property))
				return NotSupported;

			if (property == AEIds.AcceleratorKeyProperty)
				return sourceElement.AcceleratorKey;
			else if (property == AEIds.AccessKeyProperty)
				return sourceElement.AccessKey;
			else if (property == AEIds.AutomationIdProperty)
				return sourceElement.AutomationId;
			else if (property == AEIds.BoundingRectangleProperty)
				return sourceElement.BoundingRectangle;
			else if (property == AEIds.ClassNameProperty)
				return sourceElement.ClassName;
			else if (property == AEIds.ClickablePointProperty) {
				Point clickablePoint;
				if (TryGetClickablePoint (out clickablePoint))
					return clickablePoint;
				return null;
			}
			else if (property == AEIds.ControlTypeProperty)
				return sourceElement.ControlType;
			else if (property == AEIds.CultureProperty)
				return null;	// TODO: Implement (new IElement member? not used in UIAutomationWinforms)
			else if (property == AEIds.FrameworkIdProperty)
				return sourceElement.FrameworkId;
			else if (property == AEIds.HasKeyboardFocusProperty)
				return sourceElement.HasKeyboardFocus;
			else if (property == AEIds.HelpTextProperty)
				return sourceElement.HelpText;
			else if (property == AEIds.IsContentElementProperty)
				return sourceElement.IsContentElement;
			else if (property == AEIds.IsControlElementProperty)
				return sourceElement.IsControlElement;
			else if (property == AEIds.IsDockPatternAvailableProperty)
				return TryGetCurrentPattern (DockPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsEnabledProperty)
				return sourceElement.IsEnabled;
			else if (property == AEIds.IsExpandCollapsePatternAvailableProperty)
				return TryGetCurrentPattern (ExpandCollapsePatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsGridItemPatternAvailableProperty)
				return TryGetCurrentPattern (GridItemPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsGridPatternAvailableProperty)
				return TryGetCurrentPattern (GridPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsInvokePatternAvailableProperty)
				return TryGetCurrentPattern (InvokePatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsKeyboardFocusableProperty)
				return sourceElement.IsKeyboardFocusable;
			else if (property == AEIds.IsMultipleViewPatternAvailableProperty)
				return TryGetCurrentPattern (MultipleViewPatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsOffscreenProperty)
				return sourceElement.IsOffscreen;
			else if (property == AEIds.IsPasswordProperty)
				return sourceElement.IsPassword;
			else if (property == AEIds.IsRangeValuePatternAvailableProperty)
				return TryGetCurrentPattern (RangeValuePatternIdentifiers.Pattern, out pattern);
			else if (property == AEIds.IsRequiredForFormProperty)
				return sourceElement.IsRequiredForForm;
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
				return sourceElement.ItemStatus;
			else if (property == AEIds.ItemTypeProperty)
				return sourceElement.ItemType;
			else if (property == AEIds.LabeledByProperty)
				// TODO: Caching behavior here should be tested more
				return SourceManager.GetOrCreateAutomationElement (sourceElement.LabeledBy);
			else if (property == AEIds.LocalizedControlTypeProperty)
				return sourceElement.LocalizedControlType;
			else if (property == AEIds.NameProperty)
				return sourceElement.Name;
			else if (property == AEIds.NativeWindowHandleProperty)
				return sourceElement.NativeWindowHandle;
			else if (property == AEIds.OrientationProperty)
				return sourceElement.Orientation;
			else if (property == AEIds.ProcessIdProperty)
				return sourceElement.ProcessId;
			else if (property == AEIds.RuntimeIdProperty)
				return GetRuntimeId ();
			else if (property == DockPatternIdentifiers.DockPositionProperty)
				return (TryGetCurrentPattern (DockPatternIdentifiers.Pattern, out pattern)? ((DockPattern)pattern).Source.DockPosition: DockPosition.None);
			else if (property == ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty)
				return (TryGetCurrentPattern (ExpandCollapsePatternIdentifiers.Pattern, out pattern)? ((ExpandCollapsePattern)pattern).Source.ExpandCollapseState: ExpandCollapseState.LeafNode);
			else if (property == GridItemPatternIdentifiers.ColumnProperty)
				return (TryGetCurrentPattern (GridItemPatternIdentifiers.Pattern, out pattern)? ((GridItemPattern)pattern).Source.Column: 0);
			else if (property == GridItemPatternIdentifiers.ColumnSpanProperty)
				return (TryGetCurrentPattern (GridItemPatternIdentifiers.Pattern, out pattern)? ((GridItemPattern)pattern).Source.ColumnSpan: 1);
			else if (property == GridItemPatternIdentifiers.ContainingGridProperty)
				return (TryGetCurrentPattern (GridItemPatternIdentifiers.Pattern, out pattern)? SourceManager.GetOrCreateAutomationElement (((GridItemPattern)pattern).Source.ContainingGrid): null);
			else if (property == GridItemPatternIdentifiers.RowProperty)
				return (TryGetCurrentPattern (GridItemPatternIdentifiers.Pattern, out pattern)? ((GridItemPattern)pattern).Source.Row: 0);
			else if (property == GridItemPatternIdentifiers.RowSpanProperty)
				return (TryGetCurrentPattern (GridItemPatternIdentifiers.Pattern, out pattern)? ((GridItemPattern)pattern).Source.RowSpan: 1);
			else if (property == GridPatternIdentifiers.ColumnCountProperty)
				return (TryGetCurrentPattern (GridPatternIdentifiers.Pattern, out pattern)? ((GridPattern)pattern).Source.ColumnCount: 0);
			else if (property == GridPatternIdentifiers.RowCountProperty)
				return (TryGetCurrentPattern (GridPatternIdentifiers.Pattern, out pattern)? ((GridPattern)pattern).Source.RowCount: 0);
			else if (property == MultipleViewPatternIdentifiers.CurrentViewProperty)
				return (TryGetCurrentPattern (MultipleViewPatternIdentifiers.Pattern, out pattern)? ((MultipleViewPattern)pattern).Source.CurrentView: 0);
			else if (property == MultipleViewPatternIdentifiers.SupportedViewsProperty)
				return (TryGetCurrentPattern (MultipleViewPatternIdentifiers.Pattern, out pattern)? ((MultipleViewPattern)pattern).Source.GetSupportedViews (): new int [0]);
			else if (property == RangeValuePatternIdentifiers.IsReadOnlyProperty)
				return (TryGetCurrentPattern (RangeValuePatternIdentifiers.Pattern, out pattern)? ((RangeValuePattern)pattern).Source.IsReadOnly: true);
			else if (property == RangeValuePatternIdentifiers.LargeChangeProperty)
				return (TryGetCurrentPattern (RangeValuePatternIdentifiers.Pattern, out pattern)? ((RangeValuePattern)pattern).Source.LargeChange: 0);
			else if (property == RangeValuePatternIdentifiers.MaximumProperty)
				return (TryGetCurrentPattern (RangeValuePatternIdentifiers.Pattern, out pattern)? ((RangeValuePattern)pattern).Source.Maximum: 0);
			else if (property == RangeValuePatternIdentifiers.MinimumProperty)
				return (TryGetCurrentPattern (RangeValuePatternIdentifiers.Pattern, out pattern)? ((RangeValuePattern)pattern).Source.Minimum: 0);
			else if (property == RangeValuePatternIdentifiers.SmallChangeProperty)
				return (TryGetCurrentPattern (RangeValuePatternIdentifiers.Pattern, out pattern)? ((RangeValuePattern)pattern).Source.SmallChange: 0);
			else if (property == RangeValuePatternIdentifiers.ValueProperty)
				return (TryGetCurrentPattern (RangeValuePatternIdentifiers.Pattern, out pattern)? ((RangeValuePattern)pattern).Source.Value: 0);
			else if (property == ScrollPatternIdentifiers.HorizontallyScrollableProperty)
				return (TryGetCurrentPattern (ScrollPatternIdentifiers.Pattern, out pattern)? ((ScrollPattern)pattern).Source.HorizontallyScrollable: false);
			else if (property == ScrollPatternIdentifiers.HorizontalScrollPercentProperty)
				return (TryGetCurrentPattern (ScrollPatternIdentifiers.Pattern, out pattern)? ((ScrollPattern)pattern).Source.HorizontalScrollPercent: 0);
			else if (property == ScrollPatternIdentifiers.HorizontalViewSizeProperty)
				return (TryGetCurrentPattern (ScrollPatternIdentifiers.Pattern, out pattern)? ((ScrollPattern)pattern).Source.HorizontalViewSize: 100);
			else if (property == ScrollPatternIdentifiers.VerticallyScrollableProperty)
				return (TryGetCurrentPattern (ScrollPatternIdentifiers.Pattern, out pattern)? ((ScrollPattern)pattern).Source.VerticallyScrollable: false);
			else if (property == ScrollPatternIdentifiers.VerticalScrollPercentProperty)
				return (TryGetCurrentPattern (ScrollPatternIdentifiers.Pattern, out pattern)? ((ScrollPattern)pattern).Source.VerticalScrollPercent: 0);
			else if (property == ScrollPatternIdentifiers.VerticalViewSizeProperty)
				return (TryGetCurrentPattern (ScrollPatternIdentifiers.Pattern, out pattern)? ((ScrollPattern)pattern).Source.VerticalViewSize: 100);
			else if (property == SelectionItemPatternIdentifiers.IsSelectedProperty)
				return (TryGetCurrentPattern (SelectionItemPatternIdentifiers.Pattern, out pattern)? ((SelectionItemPattern)pattern).Source.IsSelected: false);
			else if (property == SelectionItemPatternIdentifiers.SelectionContainerProperty)
				return (TryGetCurrentPattern (SelectionItemPatternIdentifiers.Pattern, out pattern)? SourceManager.GetOrCreateAutomationElement (((SelectionItemPattern)pattern).Source.SelectionContainer): null);
			else if (property == SelectionPatternIdentifiers.CanSelectMultipleProperty)
				return (TryGetCurrentPattern (SelectionPatternIdentifiers.Pattern, out pattern)? ((SelectionPattern)pattern).Source.CanSelectMultiple: false);
			else if (property == SelectionPatternIdentifiers.IsSelectionRequiredProperty)
				return (TryGetCurrentPattern (SelectionPatternIdentifiers.Pattern, out pattern)? ((SelectionPattern)pattern).Source.IsSelectionRequired: false);
			else if (property == SelectionPatternIdentifiers.SelectionProperty)
				return (TryGetCurrentPattern (SelectionPatternIdentifiers.Pattern, out pattern)? SourceManager.GetOrCreateAutomationElements (((SelectionPattern)pattern).Source.GetSelection ()): new AutomationElement [0]);
			else if (property == TableItemPatternIdentifiers.ColumnHeaderItemsProperty)
				return (TryGetCurrentPattern (TableItemPatternIdentifiers.Pattern, out pattern)? SourceManager.GetOrCreateAutomationElements (((TableItemPattern)pattern).Source.GetColumnHeaderItems ()): new AutomationElement [0]);
			else if (property == TableItemPatternIdentifiers.RowHeaderItemsProperty)
				return (TryGetCurrentPattern (TableItemPatternIdentifiers.Pattern, out pattern)? SourceManager.GetOrCreateAutomationElements (((TableItemPattern)pattern).Source.GetRowHeaderItems ()): new AutomationElement [0]);
			else if (property == TablePatternIdentifiers.ColumnHeadersProperty)
				return (TryGetCurrentPattern (TablePatternIdentifiers.Pattern, out pattern)? SourceManager.GetOrCreateAutomationElements (((TablePattern)pattern).Source.GetColumnHeaders ()): new AutomationElement [0]);
			else if (property == TablePatternIdentifiers.RowHeadersProperty)
				return (TryGetCurrentPattern (TablePatternIdentifiers.Pattern, out pattern)? SourceManager.GetOrCreateAutomationElements (((TablePattern)pattern).Source.GetRowHeaders ()): new AutomationElement [0]);
			else if (property == TablePatternIdentifiers.RowOrColumnMajorProperty)
				return (TryGetCurrentPattern (TablePatternIdentifiers.Pattern, out pattern)? ((TablePattern)pattern).Source.RowOrColumnMajor: RowOrColumnMajor.Indeterminate);
			else if (property == TogglePatternIdentifiers.ToggleStateProperty)
				return (TryGetCurrentPattern (TogglePatternIdentifiers.Pattern, out pattern)? ((TogglePattern)pattern).Source.ToggleState: ToggleState.Indeterminate);
			else if (property == TransformPatternIdentifiers.CanMoveProperty)
				return (TryGetCurrentPattern (TransformPatternIdentifiers.Pattern, out pattern)? (bool)((TransformPattern)pattern).Source.CanMove: false);
			else if (property == TransformPatternIdentifiers.CanResizeProperty)
				return (TryGetCurrentPattern (TransformPatternIdentifiers.Pattern, out pattern)? ((TransformPattern)pattern).Source.CanResize: false);
			else if (property == TransformPatternIdentifiers.CanRotateProperty)
				return (TryGetCurrentPattern (TransformPatternIdentifiers.Pattern, out pattern)? ((TransformPattern)pattern).Source.CanRotate: false);
			else if (property == ValuePatternIdentifiers.IsReadOnlyProperty)
				return (TryGetCurrentPattern (ValuePatternIdentifiers.Pattern, out pattern)? ((ValuePattern)pattern).Source.IsReadOnly: true);
			else if (property == ValuePatternIdentifiers.ValueProperty)
				return (TryGetCurrentPattern (ValuePatternIdentifiers.Pattern, out pattern)? ((ValuePattern)pattern).Source.Value: String.Empty);
			else if (property == WindowPatternIdentifiers.CanMaximizeProperty)
				return (TryGetCurrentPattern (WindowPatternIdentifiers.Pattern, out pattern)? (bool)((WindowPattern)pattern).Source.CanMaximize: false);
			else if (property == WindowPatternIdentifiers.CanMinimizeProperty)
				return (TryGetCurrentPattern (WindowPatternIdentifiers.Pattern, out pattern)? (bool)((WindowPattern)pattern).Source.CanMinimize: false);
			else if (property == WindowPatternIdentifiers.IsModalProperty)
				return (TryGetCurrentPattern (WindowPatternIdentifiers.Pattern, out pattern)? ((WindowPattern)pattern).Source.IsModal: false);
			else if (property == WindowPatternIdentifiers.IsTopmostProperty)
				return (TryGetCurrentPattern (WindowPatternIdentifiers.Pattern, out pattern)? ((WindowPattern)pattern).Source.IsTopmost: false);
			else if (property == WindowPatternIdentifiers.WindowInteractionStateProperty)
				return (TryGetCurrentPattern (WindowPatternIdentifiers.Pattern, out pattern)? ((WindowPattern)pattern).Source.WindowInteractionState: WindowInteractionState.Running);
			else if (property == WindowPatternIdentifiers.WindowVisualStateProperty)
				return (TryGetCurrentPattern (WindowPatternIdentifiers.Pattern, out pattern)? ((WindowPattern)pattern).Source.WindowVisualState: WindowVisualState.Normal);
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
			return sourceElement.GetSupportedProperties ();
		}

		public AutomationElement GetUpdatedCache (CacheRequest request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");

			var updated = SourceManager.GetOrCreateAutomationElement (sourceElement);
			if (request == CacheRequest.DefaultRequest)
				return updated;

			if ((request.TreeScope & TreeScope.Element) == TreeScope.Element) {
				foreach (var property in request.CachedProperties) {
					updated.propertyCache [property.Id ] =
						new CachedValue (updated.GetCurrentPropertyValue (property),
						                 updated.sourceElement.SupportsProperty (property));
				}
				updated.mode = request.AutomationElementMode;
				updated.CacheRequest = request.Clone ();
			}

			if ((request.TreeScope & TreeScope.Children) == TreeScope.Children ||
			    (request.TreeScope & TreeScope.Descendants) == TreeScope.Descendants) {
				// Modify scope to make sure children include
				// themselves, and only include their own
				// children if specified by original scope
				var childRequest = request.Clone ();
				childRequest.TreeScope |= TreeScope.Element;
				if ((request.TreeScope & TreeScope.Descendants) != TreeScope.Descendants)
					childRequest.TreeScope ^= TreeScope.Children;

				updated.cachedChildren = new List<AutomationElement> ();
				var child = TreeWalker.RawViewWalker.GetFirstChild (updated, childRequest);
				while (child != null) {
					if (childRequest.TreeFilter.AppliesTo (child)) {
						child.cachedParent = updated;
						updated.cachedChildren.Add (child);
					}
					child = TreeWalker.RawViewWalker.GetNextSibling (child, childRequest);
				}
			}

			return updated;
		}

		public void SetFocus ()
		{
			sourceElement.SetFocus ();
		}

		public bool TryGetCachedPattern (AutomationPattern pattern,
		                                 out Object patternObject)
		{
			try {
				patternObject = GetCachedPattern (pattern);
				return true;
			} catch (Exception) {
				patternObject = null;
				return false;
			}
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
				patternObject = GetCurrentPattern (pattern);
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
			ArgumentCheck.Assert (hwnd, (h => h != IntPtr.Zero), "hwnd");
			if (hwnd == NativeMethods.RootWindowHandle)
				return RootElement;
			AutomationElement element = null;
			foreach (var source in SourceManager.GetAutomationSources ()) {
				var sourceElement = source.GetElementFromHandle (hwnd);
				if (sourceElement != null) {
					element = SourceManager.GetOrCreateAutomationElement (sourceElement);
					break;
				}
			}
			if (element == null)
				throw new ElementNotAvailableException ();
			return element;
		}

		public static AutomationElement FromLocalProvider (IRawElementProviderSimple localImpl)
		{
			IElement sourceElement = Mono.UIAutomation.ClientSource.ClientAutomationSource.Instance
				.GetOrCreateElement (localImpl);
			return new AutomationElement (sourceElement);
		}

		public static AutomationElement FromPoint (Point pt)
		{
			IntPtr handle = NativeMethods.WindowAtPosition ((int) pt.X, (int) pt.Y);
			if (handle == IntPtr.Zero)
				return RootElement;
			AutomationElement startElement = null;
			try {
				startElement = FromHandle (handle);
			} catch (ElementNotAvailableException) {
				return RootElement;
			}
			if (startElement == RootElement)
				return RootElement;
			//Keep searching the descendant element which are not native window
			var sourceElement = startElement.SourceElement.
				GetDescendantFromPoint (pt.X, pt.Y);
			return SourceManager.GetOrCreateAutomationElement (sourceElement);
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

#region Internal Methods
		internal object GetPropertyValue (AutomationProperty property,
		                                  bool cached)
		{
			if (cached)
				return GetCachedPropertyValue (property);
			else
				return GetCurrentPropertyValue (property);
		}
#endregion

#region Private Methods
		private Object GetPattern (AutomationPattern pattern, bool cached)
		{
			if (pattern == null)
				throw new ArgumentNullException ("pattern");
			if (cached && (CacheRequest == null || !CacheRequest.CachedPatterns.Contains (pattern)))
				throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
			object source = sourceElement.GetCurrentPattern (pattern);
			if (source == null)
				throw new InvalidOperationException ("Cannot request an unsupported pattern");

			if (pattern == DockPatternIdentifiers.Pattern)
				return new DockPattern ((IDockProvider) source, this, cached);
			else if (pattern == ExpandCollapsePatternIdentifiers.Pattern)
				return new ExpandCollapsePattern ((IExpandCollapseProvider) source, this, cached);
			else if (pattern == GridItemPatternIdentifiers.Pattern)
				return new GridItemPattern ((IGridItemPattern) source, this, cached);
			else if (pattern == GridPatternIdentifiers.Pattern)
				return new GridPattern ((IGridPattern) source, this, cached);
			else if (pattern == InvokePatternIdentifiers.Pattern)
				return new InvokePattern ((IInvokePattern) source);
			else if (pattern == MultipleViewPatternIdentifiers.Pattern)
				return new MultipleViewPattern ((IMultipleViewPattern) source, this, cached);
			else if (pattern == RangeValuePatternIdentifiers.Pattern)
				return new RangeValuePattern ((IRangeValuePattern) source, this, cached);
			else if (pattern == ScrollItemPatternIdentifiers.Pattern)
				return new ScrollItemPattern ((IScrollItemProvider) source);
			else if (pattern == ScrollPatternIdentifiers.Pattern)
				return new ScrollPattern ((IScrollPattern) source, this, cached);
			else if (pattern == SelectionItemPatternIdentifiers.Pattern)
				return new SelectionItemPattern ((ISelectionItemPattern) source, this, cached);
			else if (pattern == SelectionPatternIdentifiers.Pattern)
				return new SelectionPattern ((ISelectionPattern) source, this, cached);
			else if (pattern == TableItemPatternIdentifiers.Pattern)
				return new TableItemPattern ((ITableItemPattern) source, this, cached);
			else if (pattern == TablePatternIdentifiers.Pattern)
				return new TablePattern ((ITablePattern) source, this, cached);
			else if (pattern == TextPatternIdentifiers.Pattern)
				return new TextPattern ((ITextPattern) source);
			else if (pattern == TogglePatternIdentifiers.Pattern)
				return new TogglePattern ((IToggleProvider) source, this, cached);
			else if (pattern == TransformPatternIdentifiers.Pattern)
				return new TransformPattern ((ITransformPattern) source, this, cached);
			else if (pattern == ValuePatternIdentifiers.Pattern)
				return new ValuePattern ((IValuePattern) source, this, cached);
			else if (pattern == WindowPatternIdentifiers.Pattern)
				return new WindowPattern ((IWindowPattern) source, this, cached);
			else
				throw new ArgumentException ();
		}

		private List<AutomationElement> Find (TreeScope scope, Condition condition, bool findFirst)
		{
			// Parent and Ancestors scopes are not supported on
			// Windows (this is specified in MSDN, too).
			if (scope.HasFlag (TreeScope.Parent) || scope.HasFlag (TreeScope.Ancestors))
				throw new ArgumentException ("scope");

			List<AutomationElement> found = new List<AutomationElement> ();

			var isElementScope = scope.HasFlag (TreeScope.Element);
			var isThis = condition.AppliesTo (this);
			var isDefaulRequest = CacheRequest.Current == CacheRequest.DefaultRequest;
			var isTreeFilter = CacheRequest.Current.TreeFilter.AppliesTo (this);
			if (isElementScope && isThis && (isDefaulRequest || isTreeFilter))
			{
				// TODO: Need to check request's TreeScope, too?
				found.Add (GetUpdatedCache (CacheRequest.Current));
			}

			if (scope.HasFlag (TreeScope.Children) || scope.HasFlag (TreeScope.Descendants))
			{
				TreeScope childScope = TreeScope.Element;
				if (scope.HasFlag (TreeScope.Descendants))
					childScope = TreeScope.Subtree;
				AutomationElement current = TreeWalker.RawViewWalker.GetFirstChild (this);
				while (current != null)
				{
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

		class CachedValue
		{
			public bool IsSupported { get; private set; }

			public Object Value { get; private set; }

			public CachedValue (object val, bool isSupported)
			{
				Value = val;
				IsSupported = isSupported;
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
