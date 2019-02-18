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
//      Calvin Gaisford <calvinrg@gmail.com>
//      Mario Carrion <mcarrion@novell.com>
// 

using System;

namespace System.Windows.Automation
{
	public class AutomationProperty : AutomationIdentifier
	{
#region Internal Constructor
		
		internal AutomationProperty (int id, string programmaticName) :
			base (id, programmaticName)
		{
		}
		
#endregion
		
#region Public Static Methods
		
		public static AutomationProperty LookupById (int id)
		{
			if (id == AutomationElementIdentifiers.AcceleratorKeyProperty.Id)
				return AutomationElementIdentifiers.AcceleratorKeyProperty;
			else if (id == AutomationElementIdentifiers.AccessKeyProperty.Id)
				return AutomationElementIdentifiers.AccessKeyProperty;
			else if (id == AutomationElementIdentifiers.AutomationIdProperty.Id)
				return AutomationElementIdentifiers.AutomationIdProperty;
			else if (id == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
				return AutomationElementIdentifiers.BoundingRectangleProperty;
			else if (id == AutomationElementIdentifiers.ClassNameProperty.Id)
				return AutomationElementIdentifiers.ClassNameProperty;
			else if (id == AutomationElementIdentifiers.ClickablePointProperty.Id)
				return AutomationElementIdentifiers.ClickablePointProperty;
			else if (id == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return AutomationElementIdentifiers.ControlTypeProperty;
			else if (id == AutomationElementIdentifiers.CultureProperty.Id)
				return AutomationElementIdentifiers.CultureProperty;
			else if (id == AutomationElementIdentifiers.FrameworkIdProperty.Id)
				return AutomationElementIdentifiers.FrameworkIdProperty;
			else if (id == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return AutomationElementIdentifiers.HasKeyboardFocusProperty;
			else if (id == AutomationElementIdentifiers.HelpTextProperty.Id)
				return AutomationElementIdentifiers.HelpTextProperty;
			else if (id == AutomationElementIdentifiers.IsContentElementProperty.Id)
				return AutomationElementIdentifiers.IsContentElementProperty;
			else if (id == AutomationElementIdentifiers.IsControlElementProperty.Id)
				return AutomationElementIdentifiers.IsControlElementProperty;
			else if (id == AutomationElementIdentifiers.IsDockPatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsDockPatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsEnabledProperty.Id)
				return AutomationElementIdentifiers.IsEnabledProperty;
			else if (id == AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsGridItemPatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsGridItemPatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsGridPatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsGridPatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsInvokePatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsInvokePatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return AutomationElementIdentifiers.IsKeyboardFocusableProperty;
			else if (id == AutomationElementIdentifiers.IsLegacyIAccessiblePatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsLegacyIAccessiblePatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsMultipleViewPatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsMultipleViewPatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsOffscreenProperty.Id)
				return AutomationElementIdentifiers.IsOffscreenProperty;
			else if (id == AutomationElementIdentifiers.IsPasswordProperty.Id)
				return AutomationElementIdentifiers.IsPasswordProperty;
			else if (id == AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsRequiredForFormProperty.Id)
				return AutomationElementIdentifiers.IsRequiredForFormProperty;
			else if (id == AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsScrollPatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsScrollPatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsSelectionPatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsSelectionPatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsTableItemPatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsTableItemPatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsTablePatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsTextPatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsTextPatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsTogglePatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsTogglePatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsTransformPatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsTransformPatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsValuePatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsValuePatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.IsWindowPatternAvailableProperty.Id)
				return AutomationElementIdentifiers.IsWindowPatternAvailableProperty;
			else if (id == AutomationElementIdentifiers.ItemStatusProperty.Id)
				return AutomationElementIdentifiers.ItemStatusProperty;
			else if (id == AutomationElementIdentifiers.ItemTypeProperty.Id)
				return AutomationElementIdentifiers.ItemTypeProperty;
			else if (id == AutomationElementIdentifiers.LabeledByProperty.Id)
				return AutomationElementIdentifiers.LabeledByProperty;
			else if (id == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return AutomationElementIdentifiers.LocalizedControlTypeProperty;
			else if (id == AutomationElementIdentifiers.NameProperty.Id)
				return AutomationElementIdentifiers.NameProperty;
			else if (id == AutomationElementIdentifiers.NativeWindowHandleProperty.Id)
				return AutomationElementIdentifiers.NativeWindowHandleProperty;
			else if (id == AutomationElementIdentifiers.OrientationProperty.Id)
				return AutomationElementIdentifiers.OrientationProperty;
			else if (id == AutomationElementIdentifiers.ProcessIdProperty.Id)
				return AutomationElementIdentifiers.ProcessIdProperty;
			else if (id == AutomationElementIdentifiers.RuntimeIdProperty.Id)
				return AutomationElementIdentifiers.RuntimeIdProperty;
			else if (id == TogglePatternIdentifiers.ToggleStateProperty.Id)
				return TogglePatternIdentifiers.ToggleStateProperty;
			else if (id == SelectionItemPatternIdentifiers.IsSelectedProperty.Id)
				return SelectionItemPatternIdentifiers.IsSelectedProperty;
			else if (id == SelectionItemPatternIdentifiers.SelectionContainerProperty.Id)
				return SelectionItemPatternIdentifiers.SelectionContainerProperty;
			else if (id == SelectionPatternIdentifiers.CanSelectMultipleProperty.Id)
				return SelectionPatternIdentifiers.CanSelectMultipleProperty;
			else if (id == SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id)
				return SelectionPatternIdentifiers.IsSelectionRequiredProperty;
			else if (id == SelectionPatternIdentifiers.SelectionProperty.Id)
				return SelectionPatternIdentifiers.SelectionProperty;
			else if (id == RangeValuePatternIdentifiers.IsReadOnlyProperty.Id)
				return RangeValuePatternIdentifiers.IsReadOnlyProperty;
			else if (id == RangeValuePatternIdentifiers.LargeChangeProperty.Id)
				return RangeValuePatternIdentifiers.LargeChangeProperty;
			else if (id == RangeValuePatternIdentifiers.MaximumProperty.Id)
				return RangeValuePatternIdentifiers.MaximumProperty;
			else if (id == RangeValuePatternIdentifiers.MinimumProperty.Id)
				return RangeValuePatternIdentifiers.MinimumProperty;
			else if (id == RangeValuePatternIdentifiers.SmallChangeProperty.Id)
				return RangeValuePatternIdentifiers.SmallChangeProperty;
			else if (id == RangeValuePatternIdentifiers.ValueProperty.Id)
				return RangeValuePatternIdentifiers.ValueProperty;
			else if (id == WindowPatternIdentifiers.CanMaximizeProperty.Id)
				return WindowPatternIdentifiers.CanMaximizeProperty;
			else if (id == WindowPatternIdentifiers.CanMinimizeProperty.Id)
				return WindowPatternIdentifiers.CanMinimizeProperty;
			else if (id == WindowPatternIdentifiers.IsModalProperty.Id)
				return WindowPatternIdentifiers.IsModalProperty;
			else if (id == WindowPatternIdentifiers.IsTopmostProperty.Id)
				return WindowPatternIdentifiers.IsTopmostProperty;
			else if (id == WindowPatternIdentifiers.WindowInteractionStateProperty.Id)
				return WindowPatternIdentifiers.WindowInteractionStateProperty;
			else if (id == WindowPatternIdentifiers.WindowVisualStateProperty.Id)
				return WindowPatternIdentifiers.WindowVisualStateProperty;
			else if (id == ScrollPatternIdentifiers.HorizontallyScrollableProperty.Id)
				return ScrollPatternIdentifiers.HorizontallyScrollableProperty;
			else if (id == ScrollPatternIdentifiers.HorizontalScrollPercentProperty.Id)
				return ScrollPatternIdentifiers.HorizontalScrollPercentProperty;
			else if (id == ScrollPatternIdentifiers.HorizontalViewSizeProperty.Id)
				return ScrollPatternIdentifiers.HorizontalViewSizeProperty;
			else if (id == ScrollPatternIdentifiers.VerticallyScrollableProperty.Id)
				return ScrollPatternIdentifiers.VerticallyScrollableProperty;
			else if (id == ScrollPatternIdentifiers.VerticalScrollPercentProperty.Id)
				return ScrollPatternIdentifiers.VerticalScrollPercentProperty;
			else if (id == ScrollPatternIdentifiers.VerticalViewSizeProperty.Id)
				return ScrollPatternIdentifiers.VerticalViewSizeProperty;
			else if (id == ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id)
				return ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty;
			else if (id == ValuePatternIdentifiers.IsReadOnlyProperty.Id)
				return ValuePatternIdentifiers.IsReadOnlyProperty;
			else if (id == ValuePatternIdentifiers.ValueProperty.Id)
				return ValuePatternIdentifiers.ValueProperty;
			else if (id == GridPatternIdentifiers.ColumnCountProperty.Id)
				return GridPatternIdentifiers.ColumnCountProperty;
			else if (id == GridPatternIdentifiers.RowCountProperty.Id)
				return GridPatternIdentifiers.RowCountProperty;			
			else if (id == GridItemPatternIdentifiers.RowProperty.Id)
				return GridItemPatternIdentifiers.RowProperty;
			else if (id == GridItemPatternIdentifiers.ColumnProperty.Id)
				return GridItemPatternIdentifiers.ColumnProperty;
			else if (id == GridItemPatternIdentifiers.RowSpanProperty.Id)
				return GridItemPatternIdentifiers.RowSpanProperty;
			else if (id == GridItemPatternIdentifiers.ColumnSpanProperty.Id)
				return GridItemPatternIdentifiers.ColumnSpanProperty;
			else if (id == GridItemPatternIdentifiers.ContainingGridProperty.Id)
				return GridItemPatternIdentifiers.ContainingGridProperty;
			else if (id == TransformPatternIdentifiers.CanMoveProperty.Id)
				return TransformPatternIdentifiers.CanMoveProperty;
			else if (id == TransformPatternIdentifiers.CanResizeProperty.Id)
				return TransformPatternIdentifiers.CanResizeProperty;
			else if (id == TransformPatternIdentifiers.CanRotateProperty.Id)
				return TransformPatternIdentifiers.CanRotateProperty;
			else if (id == LegacyIAccessiblePatternIdentifiers.ChildIdProperty.Id)
				return LegacyIAccessiblePatternIdentifiers.ChildIdProperty;
			else if (id == LegacyIAccessiblePatternIdentifiers.DefaultActionProperty.Id)
				return LegacyIAccessiblePatternIdentifiers.DefaultActionProperty;
			else if (id == LegacyIAccessiblePatternIdentifiers.DescriptionProperty.Id)
				return LegacyIAccessiblePatternIdentifiers.DescriptionProperty;
			else if (id == LegacyIAccessiblePatternIdentifiers.HelpProperty.Id)
				return LegacyIAccessiblePatternIdentifiers.HelpProperty;
			else if (id == LegacyIAccessiblePatternIdentifiers.KeyboardShortcutProperty.Id)
				return LegacyIAccessiblePatternIdentifiers.KeyboardShortcutProperty;
			else if (id == LegacyIAccessiblePatternIdentifiers.NameProperty.Id)
				return LegacyIAccessiblePatternIdentifiers.NameProperty;
			else if (id == LegacyIAccessiblePatternIdentifiers.RoleProperty.Id)
				return LegacyIAccessiblePatternIdentifiers.RoleProperty;
			else if (id == LegacyIAccessiblePatternIdentifiers.StateProperty.Id)
				return LegacyIAccessiblePatternIdentifiers.StateProperty;
			else if (id == LegacyIAccessiblePatternIdentifiers.ValueProperty.Id)
				return LegacyIAccessiblePatternIdentifiers.ValueProperty;
			else if (id == MultipleViewPatternIdentifiers.CurrentViewProperty.Id)
				return MultipleViewPatternIdentifiers.CurrentViewProperty;
			else if (id == MultipleViewPatternIdentifiers.SupportedViewsProperty.Id)
				return MultipleViewPatternIdentifiers.SupportedViewsProperty;
			else if (id == DockPatternIdentifiers.DockPositionProperty.Id)
				return DockPatternIdentifiers.DockPositionProperty;
			else if (id == TablePatternIdentifiers.ColumnHeadersProperty.Id)
				return TablePatternIdentifiers.ColumnHeadersProperty;
			else if (id == TablePatternIdentifiers.RowHeadersProperty.Id)
				return TablePatternIdentifiers.RowHeadersProperty;
			else if (id == TablePatternIdentifiers.RowOrColumnMajorProperty.Id)
				return TablePatternIdentifiers.RowOrColumnMajorProperty;
			else if (id == TableItemPatternIdentifiers.ColumnHeaderItemsProperty.Id)
				return TableItemPatternIdentifiers.ColumnHeaderItemsProperty;
			else if (id == TableItemPatternIdentifiers.RowHeaderItemsProperty.Id)
				return TableItemPatternIdentifiers.RowHeaderItemsProperty;
			else
				return null;
		}
		
#endregion
	}
}

