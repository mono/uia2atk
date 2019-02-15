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
// Copyright (c) 2010 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//     Matt Guo <matt@mattguo.com>
// 

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Source;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;
using System.Collections.Generic;
using Mono.Unix;
using Mono.UIAutomation.Services;
using System.Linq;

namespace Mono.UIAutomation.ClientSource
{
	// TODO the ClientElement class shares many common code with UiaDbusBridge.ProviderElementWrapper,
	// Any change to refactor and merge these code?
	internal class ClientElement : IElement
	{
		#region All patterns and properties
		private static int [] allPatternIds = {
			ExpandCollapsePatternIdentifiers.Pattern.Id,
			GridItemPatternIdentifiers.Pattern.Id,
			GridPatternIdentifiers.Pattern.Id,
			InvokePatternIdentifiers.Pattern.Id,
			LegacyIAccessiblePatternIdentifiers.Pattern.Id,
			MultipleViewPatternIdentifiers.Pattern.Id,
			RangeValuePatternIdentifiers.Pattern.Id,
			ScrollPatternIdentifiers.Pattern.Id,
			SelectionItemPatternIdentifiers.Pattern.Id,
			SelectionPatternIdentifiers.Pattern.Id,
			TablePatternIdentifiers.Pattern.Id,
			TextPatternIdentifiers.Pattern.Id,
			TogglePatternIdentifiers.Pattern.Id,
			TransformPatternIdentifiers.Pattern.Id,
			ValuePatternIdentifiers.Pattern.Id,
			WindowPatternIdentifiers.Pattern.Id,
			ScrollItemPatternIdentifiers.Pattern.Id,
			DockPatternIdentifiers.Pattern.Id,
			TableItemPatternIdentifiers.Pattern.Id
		};

		private static int [] allPropertyIds = {
			AutomationElementIdentifiers.IsControlElementProperty.Id,
			AutomationElementIdentifiers.ControlTypeProperty.Id,
			AutomationElementIdentifiers.IsContentElementProperty.Id,
			AutomationElementIdentifiers.LabeledByProperty.Id,
			AutomationElementIdentifiers.NativeWindowHandleProperty.Id,
			AutomationElementIdentifiers.AutomationIdProperty.Id,
			AutomationElementIdentifiers.ItemTypeProperty.Id,
			AutomationElementIdentifiers.IsPasswordProperty.Id,
			AutomationElementIdentifiers.LocalizedControlTypeProperty.Id,
			AutomationElementIdentifiers.NameProperty.Id,
			AutomationElementIdentifiers.AcceleratorKeyProperty.Id,
			AutomationElementIdentifiers.AccessKeyProperty.Id,
			AutomationElementIdentifiers.HasKeyboardFocusProperty.Id,
			AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id,
			AutomationElementIdentifiers.IsEnabledProperty.Id,
			AutomationElementIdentifiers.BoundingRectangleProperty.Id,
			AutomationElementIdentifiers.ProcessIdProperty.Id,
			AutomationElementIdentifiers.RuntimeIdProperty.Id,
			AutomationElementIdentifiers.ClassNameProperty.Id,
			AutomationElementIdentifiers.HelpTextProperty.Id,
			AutomationElementIdentifiers.ClickablePointProperty.Id,
			AutomationElementIdentifiers.CultureProperty.Id,
			AutomationElementIdentifiers.IsOffscreenProperty.Id,
			AutomationElementIdentifiers.OrientationProperty.Id,
			AutomationElementIdentifiers.FrameworkIdProperty.Id,
			AutomationElementIdentifiers.IsRequiredForFormProperty.Id,
			AutomationElementIdentifiers.ItemStatusProperty.Id,
			// Comment Is*PatternAvailableProperty since MS.Net never include those
			// properties in the return value of AutomationElement.GetSupportedProperties ()
			//AutomationElementIdentifiers.IsDockPatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsGridItemPatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsGridPatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsInvokePatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsMultipleViewPatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsSelectionPatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsScrollPatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsTableItemPatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsTextPatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsTogglePatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsTransformPatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsValuePatternAvailableProperty.Id,
			//AutomationElementIdentifiers.IsWindowPatternAvailableProperty.Id,
			ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id,
			GridItemPatternIdentifiers.RowProperty.Id,
			GridItemPatternIdentifiers.ColumnProperty.Id,
			GridItemPatternIdentifiers.RowSpanProperty.Id,
			GridItemPatternIdentifiers.ColumnSpanProperty.Id,
			GridItemPatternIdentifiers.ContainingGridProperty.Id,
			GridPatternIdentifiers.RowCountProperty.Id,
			GridPatternIdentifiers.ColumnCountProperty.Id,
			LegacyIAccessiblePatternIdentifiers.ChildIdProperty.Id,
			LegacyIAccessiblePatternIdentifiers.DefaultActionProperty.Id,
			LegacyIAccessiblePatternIdentifiers.DescriptionProperty.Id,
			LegacyIAccessiblePatternIdentifiers.HelpProperty.Id,
			LegacyIAccessiblePatternIdentifiers.KeyboardShortcutProperty.Id,
			LegacyIAccessiblePatternIdentifiers.NameProperty.Id,
			LegacyIAccessiblePatternIdentifiers.RoleProperty.Id,
			LegacyIAccessiblePatternIdentifiers.StateProperty.Id,
			LegacyIAccessiblePatternIdentifiers.ValueProperty.Id,
			MultipleViewPatternIdentifiers.CurrentViewProperty.Id,
			MultipleViewPatternIdentifiers.SupportedViewsProperty.Id,
			RangeValuePatternIdentifiers.ValueProperty.Id,
			RangeValuePatternIdentifiers.IsReadOnlyProperty.Id,
			RangeValuePatternIdentifiers.MinimumProperty.Id,
			RangeValuePatternIdentifiers.MaximumProperty.Id,
			RangeValuePatternIdentifiers.LargeChangeProperty.Id,
			RangeValuePatternIdentifiers.SmallChangeProperty.Id,
			ScrollPatternIdentifiers.HorizontalScrollPercentProperty.Id,
			ScrollPatternIdentifiers.HorizontalViewSizeProperty.Id,
			ScrollPatternIdentifiers.VerticalScrollPercentProperty.Id,
			ScrollPatternIdentifiers.VerticalViewSizeProperty.Id,
			ScrollPatternIdentifiers.HorizontallyScrollableProperty.Id,
			ScrollPatternIdentifiers.VerticallyScrollableProperty.Id,
			SelectionItemPatternIdentifiers.IsSelectedProperty.Id,
			SelectionItemPatternIdentifiers.SelectionContainerProperty.Id,
			SelectionPatternIdentifiers.SelectionProperty.Id,
			SelectionPatternIdentifiers.CanSelectMultipleProperty.Id,
			SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id,
			TablePatternIdentifiers.RowHeadersProperty.Id,
			TablePatternIdentifiers.ColumnHeadersProperty.Id,
			TablePatternIdentifiers.RowOrColumnMajorProperty.Id,
			TogglePatternIdentifiers.ToggleStateProperty.Id,
			TransformPatternIdentifiers.CanMoveProperty.Id,
			TransformPatternIdentifiers.CanResizeProperty.Id,
			TransformPatternIdentifiers.CanRotateProperty.Id,
			ValuePatternIdentifiers.ValueProperty.Id,
			ValuePatternIdentifiers.IsReadOnlyProperty.Id,
			WindowPatternIdentifiers.CanMaximizeProperty.Id,
			WindowPatternIdentifiers.CanMinimizeProperty.Id,
			WindowPatternIdentifiers.IsModalProperty.Id,
			WindowPatternIdentifiers.WindowVisualStateProperty.Id,
			WindowPatternIdentifiers.WindowInteractionStateProperty.Id,
			WindowPatternIdentifiers.IsTopmostProperty.Id,
			DockPatternIdentifiers.DockPositionProperty.Id,
			TableItemPatternIdentifiers.RowHeaderItemsProperty.Id,
			TableItemPatternIdentifiers.ColumnHeaderItemsProperty.Id
		};
		#endregion

		#region Private FIelds

		private ClientAutomationSource source = null;
		private IRawElementProviderSimple provider = null;
		private IRawElementProviderFragment providerFragment = null;
		private IRawElementProviderFragmentRoot providerFragmentRoot = null;

		#endregion

		#region Constructor

		public ClientElement (ClientAutomationSource source, IRawElementProviderSimple provider)
		{
			ArgumentCheck.NotNull (source, "source");
			ArgumentCheck.NotNull (provider, "provider");

			this.source = source;
			this.provider = provider;
			providerFragment = provider as IRawElementProviderFragment;
			providerFragmentRoot = provider as IRawElementProviderFragmentRoot;
		}

		#endregion

		internal IRawElementProviderSimple Provider {
			get { return provider; }
		}

		#region IElement implementation

		public bool SupportsProperty (AutomationProperty property)
		{
			ArgumentCheck.NotNull (property, "property");

			object val = provider.GetPropertyValue (property.Id);
			return val != null && val != AEIds.NotSupported;
		}

		public object GetCurrentPattern (AutomationPattern pattern)
		{
			ArgumentCheck.NotNull (pattern, "pattern");

			var patternProvider = provider.GetPatternProvider (pattern.Id);
			if (patternProvider == null)
				throw new InvalidOperationException ();
			object ret = null;
			if (pattern == InvokePattern.Pattern)
				ret = new ClientInvokePattern ((IInvokeProvider) patternProvider);
			// TODO implement
			// we still have more pattern to implement
			else
				throw new System.InvalidOperationException ();
			return ret;
		}

		public AutomationPattern[] GetSupportedPatterns ()
		{
			return (from patternId in allPatternIds
				where provider.GetPatternProvider (patternId) != null
				select AutomationPattern.LookupById (patternId)).ToArray ();
		}

		public AutomationProperty[] GetSupportedProperties ()
		{
			return (from propertyId in allPropertyIds
			        let val = provider.GetPropertyValue (propertyId)
				where val != null && val != AEIds.NotSupported
				select AutomationProperty.LookupById (propertyId)).ToArray ();
		}

		public void SetFocus ()
		{
			if (!IsKeyboardFocusable)
				// as tested on Windows, the InvalidOperationException hasn't any error message 
				throw new InvalidOperationException ();
			if (providerFragment != null)
				providerFragment.SetFocus ();
		}

		public IElement GetDescendantFromPoint (double x, double y)
		{
			if (providerFragmentRoot != null) {
				var childProvider =
					providerFragmentRoot.ElementProviderFromPoint (x, y);
				return source.GetOrCreateElement (childProvider);
			}
			return null;
		}

		public string AcceleratorKey {
			get {
				return provider.GetPropertyValue (AEIds.AcceleratorKeyProperty.Id)
					as string ?? string.Empty;
			}
		}

		public string AccessKey {
			get {
				return provider.GetPropertyValue (AEIds.AccessKeyProperty.Id)
					as string ?? string.Empty;
			}
		}

		public string AutomationId {
			get {
				return provider.GetPropertyValue (AEIds.AutomationIdProperty.Id)
					as string ?? string.Empty;
			}
		}

		public Rect BoundingRectangle {
			get {
				// As tested on Windows, runtime id is solely decided
				// by providerFragment.BoundingRectangle, and is irrelevant with
				// IRawElementProviderSimple.GetPropertyValue (AEIds.BoundingRectangleProperty)

				if (providerFragment != null)
					return providerFragment.BoundingRectangle;
				else
					return Rect.Empty;
			}
		}

		public string ClassName {
			get {
				return provider.GetPropertyValue (AEIds.ClassNameProperty.Id)
					as string ?? string.Empty;
			}
		}

		public Point ClickablePoint {
			get {
				object obj = provider.GetPropertyValue (AEIds.ClickablePointProperty.Id);
				return (obj != null) ? (Point) obj :
					new Point (double.NegativeInfinity, double.NegativeInfinity);
			}
		}

		public ControlType ControlType {
			get {
				object obj = provider.GetPropertyValue (AEIds.ControlTypeProperty.Id);
				return (obj != null) ? ControlType.LookupById ((int) obj) : ControlType.Custom;
			}
		}

		public string FrameworkId {
			get {
				return provider.GetPropertyValue (AEIds.FrameworkIdProperty.Id)
					as string ?? string.Empty;
			}
		}

		public bool HasKeyboardFocus {
			get {
				object obj = provider.GetPropertyValue (AEIds.HasKeyboardFocusProperty.Id);
				return (obj != null) ? (bool) obj : false;
			}
		}

		public string HelpText {
			get {
				return provider.GetPropertyValue (AEIds.HelpTextProperty.Id)
					as string ?? string.Empty;
			}
		}

		public bool IsContentElement {
			get {
				object obj = provider.GetPropertyValue (AEIds.IsContentElementProperty.Id);
				return (obj != null) ? (bool) obj : false;
			}
		}

		public bool IsControlElement {
			get {
				object obj = provider.GetPropertyValue (AEIds.IsControlElementProperty.Id);
				return (obj != null) ? (bool) obj : false;
			}
		}

		public bool IsEnabled {
			get {
				object obj = provider.GetPropertyValue (AEIds.IsEnabledProperty.Id);
				return (obj != null) ? (bool) obj : false;
			}
		}

		public bool IsKeyboardFocusable {
			get {
				object obj = provider.GetPropertyValue (AEIds.IsKeyboardFocusableProperty.Id);
				return (obj != null) ? (bool) obj : false;
			}
		}

		public bool IsOffscreen {
			get {
				object obj = provider.GetPropertyValue (AEIds.IsOffscreenProperty.Id);
				return (obj != null) ? (bool) obj : false;
			}
		}

		public bool IsPassword {
			get {
				object obj = provider.GetPropertyValue (AEIds.IsPasswordProperty.Id);
				return (obj != null) ? (bool) obj : false;
			}
		}

		public bool IsRequiredForForm {
			get {
				object obj = provider.GetPropertyValue (AEIds.IsRequiredForFormProperty.Id);
				return (obj != null) ? (bool) obj : false;
			}
		}

		public string ItemStatus {
			get {
				return provider.GetPropertyValue (AEIds.ItemStatusProperty.Id)
					as string ?? string.Empty;
			}
		}

		public string ItemType {
			get {
				return provider.GetPropertyValue (AEIds.ItemTypeProperty.Id)
					as string ?? string.Empty;
			}
		}

		public IElement LabeledBy {
			get {
				var labeledBy = provider.GetPropertyValue (AEIds.LabeledByProperty.Id)
					as IRawElementProviderSimple;
				return (labeledBy != null) ? source.GetOrCreateElement (labeledBy) : null;
			}
		}

		public string LocalizedControlType {
			get {
				var controlType = this.ControlType;
				if (controlType == ControlType.DataGrid)
					return Catalog.GetString ("data grid");
				else if (controlType == ControlType.DataItem)
					return Catalog.GetString ("data item");
				else if (controlType == ControlType.List)
					return Catalog.GetString ("list");
				else
					return controlType.LocalizedControlType;
			}
		}

		public string Name {
			get {
				return provider.GetPropertyValue (AEIds.NameProperty.Id)
					as string ?? string.Empty;
			}
		}

		public int NativeWindowHandle {
			get {
				object obj = provider.GetPropertyValue (AEIds.NativeWindowHandleProperty.Id);
				return (obj != null) ? (int) obj : 0;
			}
		}

		public OrientationType Orientation {
			get {
				object obj = provider.GetPropertyValue (AEIds.OrientationProperty.Id);
				return (obj != null) ? (OrientationType) obj : OrientationType.None;
			}
		}

		public int ProcessId {
			get {
				object obj = provider.GetPropertyValue (AEIds.ProcessIdProperty.Id);
				return (obj != null) ? (int) obj : 0;
			}
		}

		public int[] RuntimeId {
			get {
				// As tested on Windows, runtime id is solely decided
				// by providerFragment.GetRuntimeId, and is irrelevant with
				// IRawElementProviderSimple.GetPropertyValue (AEIds.RuntimeIdProperty)
				//
				// An expcetion: if the runtime id is not explicitly provided, while
				// native handle is provided, then UIA will generate a runtime id for the provider,
				// on Windows 7 the runtime id is [42, NativeHandleValue]
				int [] runtimeId = null;
				if (providerFragment != null)
					runtimeId = providerFragment.GetRuntimeId ();
				else {
					int hwnd = this.NativeWindowHandle;
					if (hwnd != 0)
						runtimeId = new int [] {42, hwnd};
				}
				return runtimeId;
			}
		}

		public IElement Parent {
			get {
				if (providerFragment == null)
					return null;
				return source.GetOrCreateElement (
					providerFragment.Navigate (NavigateDirection.Parent));
			}
		}

		public IElement FirstChild {
			get {
				if (providerFragment == null)
					return null;
				return source.GetOrCreateElement (
					providerFragment.Navigate (NavigateDirection.FirstChild));
			}
		}

		public IElement LastChild {
			get {
				if (providerFragment == null)
					return null;
				return source.GetOrCreateElement (
					providerFragment.Navigate (NavigateDirection.LastChild));
			}
		}

		public IElement NextSibling {
			get {
				if (providerFragment == null)
					return null;
				return source.GetOrCreateElement (
					providerFragment.Navigate (NavigateDirection.NextSibling));
			}
		}

		public IElement PreviousSibling {
			get {
				if (providerFragment == null)
					return null;
				return source.GetOrCreateElement (
					providerFragment.Navigate (NavigateDirection.PreviousSibling));
			}
		}

		public IAutomationSource AutomationSource {
			get {
				return source;
			}
		}

		#endregion
	}
}
