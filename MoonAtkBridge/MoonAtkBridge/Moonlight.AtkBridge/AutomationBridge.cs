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
//      Andres Aragoneses <aaragoneses@novell.com>
//      Brad Taylor <brad@getcoded.net>
//

using Atk;

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace Moonlight.AtkBridge
{
	public class AutomationBridge
	{
#region Public Methods
		public static AutomationBridge CreateAutomationBridge ()
		{
			return new AutomationBridge ();
		}

 		private AutomationBridge ()
 		{
			Log.Debug ("Moonlight Automation Bridge starting up...");

			AutomationSingleton.Instance.AutomationPropertyChanged
				+= new EventHandler<AutomationPropertyChangedEventArgs> (
					OnAutomationPropertyChanged);

			AutomationSingleton.Instance.AutomationEventRaised
				+= new EventHandler<AutomationEventEventArgs> (
					OnAutomationEventRaised);
		}

		public static bool IsAccessibilityEnabled ()
		{
			// TODO: Detect whether accessibility is turned on at a
			// platform level
			return IsExtensionEnabled ();
		}

		public IntPtr GetAccessibleHandle ()
		{
			Adapter root
				= DynamicAdapterFactory.Instance.RootVisualAdapter;
			return (root != null) ? root.Handle : IntPtr.Zero;
		}

		public void Shutdown ()
		{
			AutomationSingleton.Instance.AutomationPropertyChanged
				-= new EventHandler<AutomationPropertyChangedEventArgs> (
					OnAutomationPropertyChanged);

			AutomationSingleton.Instance.AutomationEventRaised
				-= new EventHandler<AutomationEventEventArgs> (
					OnAutomationEventRaised);

			DynamicAdapterFactory.Instance.UnloadAdapters ();
		}
#endregion

#region Private Methods
		private static bool IsExtensionEnabled ()
		{
			string filePath = null;
			try {
				filePath = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
				filePath = Path.Combine (filePath, "..");
			} catch (ArgumentException) {
				Log.Error ("Unable to construct path to extension directory");
				return false;
			}

			filePath = Path.Combine (filePath, "extension_disabled");
			if (File.Exists (filePath)) {
				Log.Debug ("Extension disabled because sentinel file was found.");
				return false;
			}

			Log.Debug ("Sentinel file not found, assuming extension is enabled...");
			return true;
		}

		private void OnAutomationPropertyChanged (
			object o, AutomationPropertyChangedEventArgs args)
		{
			if (args.Peer == null)
				return;

			if (Log.CurrentLogLevel == LogLevel.Information) {
				string propertyName = GetProgrammaticName (args.Property);
				Log.Info ("OnAutomationPropertyChanged: Peer = {0}, Property = {1}, Old = {2}, New = {3}",
				          args.Peer, propertyName, args.OldValue, args.NewValue);
			}

			Adapter adapter
				= DynamicAdapterFactory.Instance.GetAdapter (args.Peer, false);
			if (adapter == null)
				return;

			adapter.HandleAutomationPropertyChanged (args);
		}

		private void OnAutomationEventRaised (
			object o, AutomationEventEventArgs args)
		{
			if (args.Peer == null)
				return;

			Log.Info ("OnAutomationEventRaised: Peer = {0}, Event = {1}",
			          args.Peer, args.Event);

			Adapter adapter
				= DynamicAdapterFactory.Instance.GetAdapter (args.Peer, false);
			if (adapter == null)
				return;

			adapter.HandleAutomationEventRaised (args);
		}

		private static string GetProgrammaticName (AutomationProperty prop)
		{
			if (prop == AEIds.AcceleratorKeyProperty) {
				return "AcceleratorKeyProperty";
			} else if (prop == AEIds.AccessKeyProperty) {
				return "AccessKeyProperty";
			} else if (prop == AEIds.AutomationIdProperty) {
				return "AutomationIdProperty";
			} else if (prop == AEIds.BoundingRectangleProperty) {
				return "BoundingRectangleProperty";
			} else if (prop == AEIds.ClassNameProperty) {
				return "ClassNameProperty";
			} else if (prop == AEIds.ClickablePointProperty) {
				return "ClickablePointProperty";
			} else if (prop == AEIds.ControlTypeProperty) {
				return "ControlTypeProperty";
			} else if (prop == AEIds.HasKeyboardFocusProperty) {
				return "HasKeyboardFocusProperty";
			} else if (prop == AEIds.HelpTextProperty) {
				return "HelpTextProperty";
			} else if (prop == AEIds.IsContentElementProperty) {
				return "IsContentElementProperty";
			} else if (prop == AEIds.IsControlElementProperty) {
				return "IsControlElementProperty";
			} else if (prop == AEIds.IsEnabledProperty) {
				return "IsEnabledProperty";
			} else if (prop == AEIds.IsKeyboardFocusableProperty) {
				return "IsKeyboardFocusableProperty";
			} else if (prop == AEIds.IsOffscreenProperty) {
				return "IsOffscreenProperty";
			} else if (prop == AEIds.IsPasswordProperty) {
				return "IsPasswordProperty";
			} else if (prop == AEIds.IsRequiredForFormProperty) {
				return "IsRequiredForFormProperty";
			} else if (prop == AEIds.ItemStatusProperty) {
				return "ItemStatusProperty";
			} else if (prop == AEIds.ItemTypeProperty) {
				return "ItemTypeProperty";
			} else if (prop == AEIds.LabeledByProperty) {
				return "LabeledByProperty";
			} else if (prop == AEIds.LocalizedControlTypeProperty) {
				return "LocalizedControlTypeProperty";
			} else if (prop == AEIds.NameProperty) {
				return "NameProperty";
			} else if (prop == AEIds.OrientationProperty) {
				return "OrientationProperty";
			} else if (prop == DockPatternIdentifiers.DockPositionProperty) {
				return "DockPatternIdentifiers.DockPositionProperty";
			} else if (prop == ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty) {
				return "ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty";
			} else if (prop == GridItemPatternIdentifiers.ColumnProperty) {
				return "GridItemPatternIdentifiers.ColumnProperty";
			} else if (prop == GridItemPatternIdentifiers.ColumnSpanProperty) {
				return "GridItemPatternIdentifiers.ColumnSpanProperty";
			} else if (prop == GridItemPatternIdentifiers.ContainingGridProperty) {
				return "GridItemPatternIdentifiers.ContainingGridProperty";
			} else if (prop == GridItemPatternIdentifiers.RowProperty) {
				return "GridItemPatternIdentifiers.RowProperty";
			} else if (prop == GridItemPatternIdentifiers.RowSpanProperty) {
				return "GridItemPatternIdentifiers.RowSpanProperty";
			} else if (prop == GridPatternIdentifiers.ColumnCountProperty) {
				return "GridPatternIdentifiers.ColumnCountProperty";
			} else if (prop == GridPatternIdentifiers.RowCountProperty) {
				return "GridPatternIdentifiers.RowCountProperty";
			} else if (prop == MultipleViewPatternIdentifiers.CurrentViewProperty) {
				return "MultipleViewPatternIdentifiers.CurrentViewProperty";
			} else if (prop == MultipleViewPatternIdentifiers.SupportedViewsProperty) {
				return "MultipleViewPatternIdentifiers.SupportedViewsProperty";
			} else if (prop == RangeValuePatternIdentifiers.IsReadOnlyProperty) {
				return "RangeValuePatternIdentifiers.IsReadOnlyProperty";
			} else if (prop == RangeValuePatternIdentifiers.LargeChangeProperty) {
				return "RangeValuePatternIdentifiers.LargeChangeProperty";
			} else if (prop == RangeValuePatternIdentifiers.MaximumProperty) {
				return "RangeValuePatternIdentifiers.MaximumProperty";
			} else if (prop == RangeValuePatternIdentifiers.MinimumProperty) {
				return "RangeValuePatternIdentifiers.MinimumProperty";
			} else if (prop == RangeValuePatternIdentifiers.SmallChangeProperty) {
				return "RangeValuePatternIdentifiers.SmallChangeProperty";
			} else if (prop == RangeValuePatternIdentifiers.ValueProperty) {
				return "RangeValuePatternIdentifiers.ValueProperty";
			} else if (prop == ScrollPatternIdentifiers.HorizontallyScrollableProperty) {
				return "ScrollPatternIdentifiers.HorizontallyScrollableProperty";
			} else if (prop == ScrollPatternIdentifiers.HorizontalScrollPercentProperty) {
				return "ScrollPatternIdentifiers.HorizontalScrollPercentProperty";
			} else if (prop == ScrollPatternIdentifiers.HorizontalViewSizeProperty) {
				return "ScrollPatternIdentifiers.HorizontalViewSizeProperty";
			} else if (prop == ScrollPatternIdentifiers.VerticallyScrollableProperty) {
				return "ScrollPatternIdentifiers.VerticallyScrollableProperty";
			} else if (prop == ScrollPatternIdentifiers.VerticalScrollPercentProperty) {
				return "ScrollPatternIdentifiers.VerticalScrollPercentProperty";
			} else if (prop == ScrollPatternIdentifiers.VerticalViewSizeProperty) {
				return "ScrollPatternIdentifiers.VerticalViewSizeProperty";
			} else if (prop == SelectionItemPatternIdentifiers.IsSelectedProperty) {
				return "SelectionItemPatternIdentifiers.IsSelectedProperty";
			} else if (prop == SelectionItemPatternIdentifiers.SelectionContainerProperty) {
				return "SelectionItemPatternIdentifiers.SelectionContainerProperty";
			} else if (prop == SelectionPatternIdentifiers.CanSelectMultipleProperty) {
				return "SelectionPatternIdentifiers.CanSelectMultipleProperty";
			} else if (prop == SelectionPatternIdentifiers.IsSelectionRequiredProperty) {
				return "SelectionPatternIdentifiers.IsSelectionRequiredProperty";
			} else if (prop == SelectionPatternIdentifiers.SelectionProperty) {
				return "SelectionPatternIdentifiers.SelectionProperty";
			} else if (prop == TableItemPatternIdentifiers.ColumnHeaderItemsProperty) {
				return "TableItemPatternIdentifiers.ColumnHeaderItemsProperty";
			} else if (prop == TableItemPatternIdentifiers.RowHeaderItemsProperty) {
				return "TableItemPatternIdentifiers.RowHeaderItemsProperty";
			} else if (prop == TablePatternIdentifiers.ColumnHeadersProperty) {
				return "TablePatternIdentifiers.ColumnHeadersProperty";
			} else if (prop == TablePatternIdentifiers.RowHeadersProperty) {
				return "TablePatternIdentifiers.RowHeadersProperty";
			} else if (prop == TablePatternIdentifiers.RowOrColumnMajorProperty) {
				return "TablePatternIdentifiers.RowOrColumnMajorProperty";
			} else if (prop == TogglePatternIdentifiers.ToggleStateProperty) {
				return "TogglePatternIdentifiers.ToggleStateProperty";
			} else if (prop == TransformPatternIdentifiers.CanMoveProperty) {
				return "TransformPatternIdentifiers.CanMoveProperty";
			} else if (prop == TransformPatternIdentifiers.CanResizeProperty) {
				return "TransformPatternIdentifiers.CanResizeProperty";
			} else if (prop == TransformPatternIdentifiers.CanRotateProperty) {
				return "TransformPatternIdentifiers.CanRotateProperty";
			} else if (prop == ValuePatternIdentifiers.IsReadOnlyProperty) {
				return "ValuePatternIdentifiers.IsReadOnlyProperty";
			} else if (prop == ValuePatternIdentifiers.ValueProperty) {
				return "ValuePatternIdentifiers.ValueProperty";
			} else if (prop == WindowPatternIdentifiers.CanMaximizeProperty) {
				return "WindowPatternIdentifiers.CanMaximizeProperty";
			} else if (prop == WindowPatternIdentifiers.CanMinimizeProperty) {
				return "WindowPatternIdentifiers.CanMinimizeProperty";
			} else if (prop == WindowPatternIdentifiers.IsModalProperty) {
				return "WindowPatternIdentifiers.IsModalProperty";
			} else if (prop == WindowPatternIdentifiers.IsTopmostProperty) {
				return "WindowPatternIdentifiers.IsTopmostProperty";
			} else if (prop == WindowPatternIdentifiers.WindowInteractionStateProperty) {
				return "WindowPatternIdentifiers.WindowInteractionStateProperty";
			} else if (prop == WindowPatternIdentifiers.WindowVisualStateProperty) {
				return "WindowPatternIdentifiers.WindowVisualStateProperty";
			} else {
				return prop.ToString ();
			}
		}
	}
#endregion
}
