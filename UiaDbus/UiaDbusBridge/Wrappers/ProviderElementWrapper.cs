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
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SW = System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.UIAutomation.Bridge;
using DC = Mono.UIAutomation.UiaDbus;
using Mono.UIAutomation.UiaDbus.Interfaces;

using DBus;

namespace Mono.UIAutomation.UiaDbusBridge.Wrappers
{
	internal class ProviderElementWrapper : IAutomationElement
	{
#region Private Static Fields

		private static readonly PathIdCounter pathIdCounter = new PathIdCounter ();

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

#region Private Fields and Types

		private IRawElementProviderSimple provider;
		private IRawElementProviderFragment fragment;
		private string pathId;
		private Bus bus;
		private Dictionary<int, PatternInfo> patternMapping = new Dictionary<int, PatternInfo> ();

		private struct PatternInfo
		{
			public string Path;
			public object Provider;
			public object ProviderWrapper;
		}

		private delegate object CreateWrapperFromProvider (object provider);

#endregion

#region Private Methods

		private PatternInfo GetOrCreatePatternInfo (int id, object provider,
		                                     string path,
		                                     CreateWrapperFromProvider wrapperCreator)
		{
			PatternInfo oldInfo;
			if (patternMapping.TryGetValue (id, out oldInfo)) {
				if (oldInfo.Provider == provider)
					return oldInfo;
			}

			object wrapper = wrapperCreator (provider);
			PatternInfo newInfo = new PatternInfo {
				Path = path,
				Provider = provider,
				ProviderWrapper = wrapper
			};
			patternMapping [id] = newInfo;
			bus.Register (new ObjectPath (path), wrapper);

			return newInfo;
		}

		private void PerformUnregisterPattern (PatternInfo info)
		{
			bus.Unregister (new ObjectPath (info.Path));
			var disposable = info.ProviderWrapper as IDisposable;
			if (disposable != null)
				disposable.Dispose ();
		}

		private ProviderElementWrapper GetWrapperFromPoint (
			ProviderElementWrapper root, double px, double py, bool checkRoot)
		{
			if (checkRoot &&
			    (root.IsOffscreen || !root.BoundingRectangle.Contains (px, py)))
				return null;
			var child = root.FirstChild;
			while (child != null) {
				var ret = GetWrapperFromPoint (child, px, py, true);
				if (ret != null)
					return ret;
				child = child.NextSibling;
			}
			return root;
		}
#endregion

#region Constructor

		public ProviderElementWrapper (IRawElementProviderSimple provider)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");

			var uiTaskScheduler = UiTaskSchedulerHolder.UiTaskScheduler;
			if (uiTaskScheduler != null) {
				this.provider = new UiThreadProxyProviderSimple (provider, uiTaskScheduler);
				this.fragment = (provider is IRawElementProviderFragment fragment)
					? new UiThreadProxyProviderFragment (fragment, uiTaskScheduler)
					: null;
			} else {
				this.provider = provider;
				this.fragment = provider as IRawElementProviderFragment;
			}

			pathId = pathIdCounter.GetNewId ();
		}

#endregion

#region IAutomationElement Members

		public bool SupportsProperty (int propertyId)
		{
			object val = provider.GetPropertyValue (propertyId);
			return val != null && val != AEIds.NotSupported;
		}

		public string AcceleratorKey {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.AcceleratorKeyProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public string AccessKey {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.AccessKeyProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public string AutomationId {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.AutomationIdProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public DC.Rect BoundingRectangle {
			get {
				// TODO it seems that BoundingRectangle is decided by
				// IRawElementProviderFragment.BoundingRectangle, but not by
				// GetPropertyValue.
				// We need more test to verify this.
				//
				// See ClientElement for more details.
				SW.Rect? val = (SW.Rect?)
					provider.GetPropertyValue (AEIds.BoundingRectangleProperty.Id);
				if (!val.HasValue)
					return new DC.Rect (SW.Rect.Empty);
				return new DC.Rect (val.Value);
			}
		}

		public string ClassName {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.ClassNameProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public DC.Point ClickablePoint {
			get {
				SW.Point? val = (SW.Point?)
					provider.GetPropertyValue (AEIds.ClickablePointProperty.Id);
				if (!val.HasValue)
					return new DC.Point (new SW.Point (double.NegativeInfinity, double.NegativeInfinity));
				return new DC.Point (val.Value);
			}
		}

		public int ControlTypeId {
			get {
				int? val = (int?)
					provider.GetPropertyValue (AEIds.ControlTypeProperty.Id);
				if (!val.HasValue)
					return -1;
				return val.Value;
			}
		}

		public string FrameworkId {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.FrameworkIdProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public bool HasKeyboardFocus {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.HasKeyboardFocusProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public string HelpText {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.HelpTextProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public bool IsContentElement {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.IsContentElementProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public bool IsControlElement {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.IsControlElementProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public bool IsEnabled {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.IsEnabledProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public bool IsKeyboardFocusable {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.IsKeyboardFocusableProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public bool IsOffscreen {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.IsOffscreenProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public bool IsPassword {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.IsPasswordProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public bool IsRequiredForForm {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.IsRequiredForFormProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public string ItemStatus {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.ItemStatusProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public string ItemType {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.ItemTypeProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public string LabeledByElementPath {
			get {
				IRawElementProviderSimple labeledBy = (IRawElementProviderSimple)
					provider.GetPropertyValue (AEIds.LabeledByProperty.Id);
				if (labeledBy == null)
					return string.Empty;
				ProviderElementWrapper labeledByWrapper =
					AutomationBridge.Instance.FindWrapperByProvider (labeledBy);
				if (labeledByWrapper == null)
					return string.Empty;
				return labeledByWrapper.Path;
			}
		}

		public string LocalizedControlType {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.LocalizedControlTypeProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public string Name {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.NameProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public int NativeWindowHandle {
			get {
				IntPtr? val = (IntPtr?)
					provider.GetPropertyValue (AEIds.NativeWindowHandleProperty.Id);
				if (!val.HasValue)
					return 0;
				return val.Value.ToInt32 ();
			}
		}

		public OrientationType Orientation {
			get {
				OrientationType? val = (OrientationType?)
					provider.GetPropertyValue (AEIds.OrientationProperty.Id);
				if (!val.HasValue)
					return OrientationType.None;
				return val.Value;
			}
		}

		public int ProcessId {
			get {
				int? val = (int?)
					provider.GetPropertyValue (AEIds.ProcessIdProperty.Id);
				if (!val.HasValue)
					return 0;
				return val.Value;
			}
		}

		public int [] RuntimeId {
			get {
				// TODO it seems that BoundingRectangle is decided by
				// IRawElementProviderFragment.GetRuntimeId, but not by
				// GetPropertyValue.
				// We need more test to verify this.
				//
				// See ClientElement for more details.
				int [] val = (int [])
					provider.GetPropertyValue (AEIds.RuntimeIdProperty.Id);
				if (val == null)
					return new int [0];
				return val;
			}
		}

		public string ParentElementPath {
			get {
				var wrapper = Parent;
				return (wrapper != null) ? wrapper.Path : string.Empty;
			}
		}

		public string FirstChildElementPath {
			get {
				var wrapper = FirstChild;
				return (wrapper != null) ? wrapper.Path : string.Empty;
			}
		}

		public string LastChildElementPath {
			get {
				var wrapper = LastChild;
				return (wrapper != null) ? wrapper.Path : string.Empty;
			}
		}

		public string NextSiblingElementPath {
			get {
				var wrapper = NextSibling;
				return (wrapper != null) ? wrapper.Path : string.Empty;
			}
		}

		public string PreviousSiblingElementPath {
			get {
				var wrapper = PreviousSibling;
				return (wrapper != null) ? wrapper.Path : string.Empty;
			}
		}

		public string GetCurrentPatternPath (int patternId)
		{
			if (bus == null)
				throw new ElementNotAvailableException ();

			object patternProvider = provider.GetPatternProvider (patternId);
			if (patternProvider == null)
				return string.Empty;

			string patternPath = this.Path + "/";

			if (patternId == DockPatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.DockPatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new DockPatternWrapper ((IDockProvider) p));
			} else if (patternId == ExpandCollapsePatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.ExpandCollapsePatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new ExpandCollapsePatternWrapper ((IExpandCollapseProvider) p));
			} else if (patternId == GridPatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.GridPatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new GridPatternWrapper ((IGridProvider) p));
			} else if (patternId == GridItemPatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.GridItemPatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new GridItemPatternWrapper ((IGridItemProvider) p));
			} else if (patternId == InvokePatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.InvokePatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new InvokePatternWrapper ((IInvokeProvider) p));
			} else if (patternId == LegacyIAccessiblePatternIdentifiers.Pattern.Id) {
			    patternPath += DC.Constants.LegacyIAccessiblePatternSubPath;
			    GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
			        p => new LegacyIAccessiblePatternWrapper ((ILegacyIAccessibleProvider) p));
			} else if (patternId == MultipleViewPatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.MultipleViewPatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new MultipleViewPatternWrapper ((IMultipleViewProvider) p));
			} else if (patternId == RangeValuePatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.RangeValuePatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new RangeValuePatternWrapper ((IRangeValueProvider) p));
			} else if (patternId == ScrollItemPatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.ScrollItemPatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new ScrollItemPatternWrapper ((IScrollItemProvider) p));
			} else if (patternId == ScrollPatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.ScrollPatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new ScrollPatternWrapper ((IScrollProvider) p));
			} else if (patternId == SelectionItemPatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.SelectionItemPatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new SelectionItemPatternWrapper ((ISelectionItemProvider) p));
			} else if (patternId == SelectionPatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.SelectionPatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
			                        p => new SelectionPatternWrapper ((ISelectionProvider) p));
			} else if (patternId == TablePatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.TablePatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new TablePatternWrapper ((ITableProvider) p));
			} else if (patternId == TableItemPatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.TableItemPatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new TableItemPatternWrapper ((ITableItemProvider) p));
			} else if (patternId == TextPatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.TextPatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new TextPatternWrapper ((ITextProvider) p, bus, patternPath));
			} else if (patternId == TogglePatternIdentifiers.Pattern.Id) {
   				patternPath += DC.Constants.TogglePatternSubPath;
   				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
  				                        p => new TogglePatternWrapper ((IToggleProvider) p));
			} else if (patternId == TransformPatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.TransformPatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new TransformPatternWrapper ((ITransformProvider) p));
			} else if (patternId == ValuePatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.ValuePatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new ValuePatternWrapper ((IValueProvider) p));
			} else if (patternId == WindowPatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.WindowPatternSubPath;
				GetOrCreatePatternInfo (patternId, patternProvider, patternPath,
				                        p => new WindowPatternWrapper ((IWindowProvider) p));
			} else
				throw new InvalidOperationException ();

			return patternPath;
		}

		public int [] SupportedPatternIds {
			get {
				return allPatternIds
					.Where (id => provider.GetPatternProvider (id) != null)
					.ToArray ();
			}
		}

		public int [] SupportedPropertyIds {
			get {
				return allPropertyIds
					.Where (id => SupportsProperty (id))
					.ToArray ();
			}
		}

		public string GetDescendantPathFromPoint (double x, double y)
		{
			var wrapper = GetWrapperFromPoint (this, x, y, false);
			return wrapper.Path;
		}
#endregion

#region Public Methods and Properties

		public ProviderElementWrapper Parent {
			get {
				if (fragment == null)
					return null;
				var parent = fragment.Navigate (NavigateDirection.Parent);
				if (parent == null || parent == fragment)
					return null;
				return AutomationBridge.Instance.FindWrapperByProvider (parent);
			}
		}

		public ProviderElementWrapper FirstChild {
			get {
				if (fragment == null)
					return null;
				var child = fragment.Navigate (NavigateDirection.FirstChild);
				if (child == null)
					return null;
				return AutomationBridge.Instance.FindWrapperByProvider (child);
			}
		}

		public ProviderElementWrapper LastChild {
			get {
				if (fragment == null)
					return null;
				var child = fragment.Navigate (NavigateDirection.LastChild);
				if (child == null)
					return null;
				return AutomationBridge.Instance.FindWrapperByProvider (child);
			}
		}

		public ProviderElementWrapper NextSibling {
			get {
				if (fragment == null)
					return null;
				var sibling = fragment.Navigate (NavigateDirection.NextSibling);
				if (sibling == null)
					return null;
				return AutomationBridge.Instance.FindWrapperByProvider (sibling);
			}
		}

		public ProviderElementWrapper PreviousSibling {
			get {
				if (fragment == null)
					return null;
				var sibling = fragment.Navigate (NavigateDirection.PreviousSibling);
				if (sibling == null)
					return null;
				return AutomationBridge.Instance.FindWrapperByProvider (sibling);
			}
		}

		internal IRawElementProviderSimple Provider
		{
			get { return provider; }
		}

		public string Path {
			get { return DC.Constants.AutomationElementBasePath + pathId; }
		}

		public void Register (Bus bus)
		{
			this.bus = bus;
			bus.Register (new ObjectPath (DC.Constants.AutomationElementBasePath + pathId), this);
		}

		public void Unregister ()
		{
			bus.Unregister (new ObjectPath (DC.Constants.AutomationElementBasePath + pathId));
			foreach (PatternInfo info in patternMapping.Values)
				PerformUnregisterPattern (info);
			patternMapping.Clear ();
		}

		public void UnregisterPattern (int patternId)
		{
			PatternInfo info;
			if (patternMapping.TryGetValue (patternId, out info)) {
				PerformUnregisterPattern (info);
				patternMapping.Remove (patternId);
			}
		}

		public void SetFocus ()
		{
			if (!IsKeyboardFocusable)
				throw new InvalidOperationException ();
			fragment.SetFocus ();
		}
#endregion

		
		class UiThreadProxyProviderSimple : IRawElementProviderSimple
		{
			protected readonly IRawElementProviderSimple proxiedProviderSimple;
			private readonly TaskScheduler uiTaskScheduler;

			public UiThreadProxyProviderSimple (IRawElementProviderSimple proxiedProviderSimple, TaskScheduler uiTaskScheduler)
			{
				this.proxiedProviderSimple = proxiedProviderSimple;
				this.uiTaskScheduler = uiTaskScheduler;
			}

			protected TResult Execute<TResult> (Func<TResult> func)
			{
				var task = Task.Factory.StartNew<TResult> (func, CancellationToken.None, TaskCreationOptions.None, uiTaskScheduler);
				task.Wait();
				return task.Result;
			}

			protected void Execute (Action func)
			{
				var task = Task.Factory.StartNew (func, CancellationToken.None, TaskCreationOptions.None, uiTaskScheduler);
				task.Wait();
			}

			#region IRawElementProviderSimple

			public IRawElementProviderSimple HostRawElementProvider => Execute<IRawElementProviderSimple> (() => proxiedProviderSimple.HostRawElementProvider);
			public ProviderOptions ProviderOptions => Execute<ProviderOptions> (() => proxiedProviderSimple.ProviderOptions);

			public object GetPatternProvider (int patternId) => Execute<object> (() => proxiedProviderSimple.GetPatternProvider (patternId));
			public object GetPropertyValue (int propertyId) => Execute<object> (() => proxiedProviderSimple.GetPropertyValue (propertyId));

			#endregion  // IRawElementProviderSimple
		}

		class UiThreadProxyProviderFragment : UiThreadProxyProviderSimple, IRawElementProviderFragment
		{
			public UiThreadProxyProviderFragment (IRawElementProviderFragment proxiedProviderFragment, TaskScheduler uiTaskScheduler)
				: base (proxiedProviderFragment, uiTaskScheduler)
			{
			}

			protected IRawElementProviderFragment ProxiedProviderFragment => (IRawElementProviderFragment) proxiedProviderSimple;

			#region IRawElementProviderFragment

			public SW.Rect BoundingRectangle => Execute<SW.Rect> (() => ProxiedProviderFragment.BoundingRectangle);
			public IRawElementProviderFragmentRoot FragmentRoot => Execute<IRawElementProviderFragmentRoot> (() => ProxiedProviderFragment.FragmentRoot);

			public IRawElementProviderSimple[] GetEmbeddedFragmentRoots() => Execute<IRawElementProviderSimple[]> (() => ProxiedProviderFragment.GetEmbeddedFragmentRoots ());
			public int[] GetRuntimeId () => Execute<int[]> (() => ProxiedProviderFragment.GetRuntimeId ());
			public IRawElementProviderFragment Navigate (NavigateDirection direction) => Execute<IRawElementProviderFragment> (() => ProxiedProviderFragment.Navigate (direction));
			public void SetFocus () => Execute (() => ProxiedProviderFragment.SetFocus ());
			
			#endregion  // IRawElementProviderFragment
		}

		class PathIdCounter
		{
			private Int64 id = 0;
			private object locker = new object ();

			public string GetNewId ()
			{
				lock (locker)
				{
					if (id == Int64.MaxValue)
						throw new InvalidOperationException ($"PathIdCounter reached MaxValue={Int64.MaxValue}. Cann't create new D-Bus UIA Element ID.");
					var newId = ++id;
					return newId.ToString ();
				}
			}
		}
	}
}
