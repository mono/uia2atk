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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;

namespace System.Windows.Automation
{
	public static class AutomationElementIdentifiers
	{
#region Internal Constants

		// IDs for internal use, should not conflict with MS
		private const int KeyEventId = 60000;
		private const int WindowDeactivatedEventId = 60004;
		private const int HasNativeAccessibilityObjectPropertyId = 60001;
		private const int NativeAccessibilityObjectPropertyId = 60002;
		private const int IsNotifyIconPropertyId = 60005;
		
		// Event IDs
		private const int AsyncContentLoadedEventId = 20006;
		private const int AutomationFocusChangedEventId = 20005;
		private const int AutomationPropertyChangedEventId = 20004;
		private const int LayoutInvalidatedEventId = 20008;
		private const int MenuClosedEventId = 20007;
		private const int MenuOpenedEventId = 20003;
		private const int StructureChangedEventId = 20002;
		private const int ToolTipClosedEventId = 20001;
		private const int ToolTipOpenedEventId = 20000;
		
		// Property IDs
		private const int AcceleratorKeyPropertyId = 30006;
		private const int AccessKeyPropertyId = 30007;
		private const int AutomationIdPropertyId = 30011;
		private const int BoundingRectanglePropertyId = 30001;
		private const int ClassNamePropertyId = 30012;
		private const int ClickablePointPropertyId = 30014;
		private const int ControlTypePropertyId = 30003;
		private const int CulturePropertyId = 30015;
		private const int FrameworkIdPropertyId = 30024;
		private const int HasKeyboardFocusPropertyId = 30008;
		private const int HelpTextPropertyId = 30013;
		private const int IsContentElementPropertyId = 30017;
		private const int IsControlElementPropertyId = 30016;
		private const int IsDockPatternAvailablePropertyId = 30027;
		private const int IsEnabledPropertyId = 30010;
		private const int IsExpandCollapsePatternAvailablePropertyId = 30028;
		private const int IsGridItemPatternAvailablePropertyId = 30029;
		private const int IsGridPatternAvailablePropertyId = 30030;
		private const int IsInvokePatternAvailablePropertyId = 30031;
		private const int IsKeyboardFocusablePropertyId = 30009;
		private const int IsMultipleViewPatternAvailablePropertyId = 30032;
		private const int IsOffscreenPropertyId = 30022;
		private const int IsPasswordPropertyId = 30019;
		private const int IsRangeValuePatternAvailablePropertyId = 30033;
		private const int IsRequiredForFormPropertyId = 30025;
		private const int IsScrollItemPatternAvailablePropertyId = 30035;
		private const int IsScrollPatternAvailablePropertyId = 30034;
		private const int IsSelectionItemPatternAvailablePropertyId = 30036;
		private const int IsSelectionPatternAvailablePropertyId = 30037;
		private const int IsTableItemPatternAvailablePropertyId = 30039;
		private const int IsTablePatternAvailablePropertyId = 30038;
		private const int IsTextPatternAvailablePropertyId = 30040;
		private const int IsTogglePatternAvailablePropertyId = 30041;
		private const int IsTransformPatternAvailablePropertyId = 30042;
		private const int IsValuePatternAvailablePropertyId = 30043;
		private const int IsWindowPatternAvailablePropertyId = 30044;
		private const int ItemStatusPropertyId = 30026;
		private const int ItemTypePropertyId = 30021;
		private const int LabeledByPropertyId = 30018;
		private const int LocalizedControlTypePropertyId = 30004;
		private const int NamePropertyId = 30005;
		private const int NativeWindowHandlePropertyId = 30020;
		private const int OrientationPropertyId = 30023;
		private const int ProcessIdPropertyId = 30002;
		private const int RuntimeIdPropertyId = 30000;
		
#endregion
		
#region Static Constructor
		
		static AutomationElementIdentifiers ()
		{
			NotSupported = new object ();
			
			// Automation Properties
			HasNativeAccessibilityObjectProperty =
				new AutomationProperty (HasNativeAccessibilityObjectPropertyId,
					"AutomationElementIdentifiers.HasNativeAccessibilityObjectProperty");
			NativeAccessibilityObjectProperty =
				new AutomationProperty (NativeAccessibilityObjectPropertyId,
					"AutomationElementIdentifiers.NativeAccessibilityObjectProperty");
			IsNotifyIconProperty =
				new AutomationProperty (IsNotifyIconPropertyId,
					"AutomationElementIdentifiers.IsNotifyIconProperty");
			AcceleratorKeyProperty =
				new AutomationProperty (AcceleratorKeyPropertyId,
					"AutomationElementIdentifiers.AcceleratorKeyProperty");
			AccessKeyProperty =
				new AutomationProperty (AccessKeyPropertyId,
					"AutomationElementIdentifiers.AccessKeyProperty");
			AutomationIdProperty =
				new AutomationProperty (AutomationIdPropertyId,
					"AutomationElementIdentifiers.AutomationIdProperty");
			BoundingRectangleProperty =
				new AutomationProperty (BoundingRectanglePropertyId,
					"AutomationElementIdentifiers.BoundingRectangleProperty");
			ClassNameProperty =
				new AutomationProperty (ClassNamePropertyId,
					"AutomationElementIdentifiers.ClassNameProperty");
			ClickablePointProperty =
				new AutomationProperty (ClickablePointPropertyId,
					"AutomationElementIdentifiers.ClickablePointProperty");
			ControlTypeProperty =
				new AutomationProperty (ControlTypePropertyId,
					"AutomationElementIdentifiers.ControlTypeProperty");
			CultureProperty =
				new AutomationProperty (CulturePropertyId,
					"AutomationElementIdentifiers.CultureProperty");
			FrameworkIdProperty =
				new AutomationProperty (FrameworkIdPropertyId,
					"AutomationElementIdentifiers.FrameworkIdProperty");
			HasKeyboardFocusProperty =
				new AutomationProperty (HasKeyboardFocusPropertyId,
					"AutomationElementIdentifiers.HasKeyboardFocusProperty");
			HelpTextProperty =
				new AutomationProperty (HelpTextPropertyId,
					"AutomationElementIdentifiers.HelpTextProperty");
			IsContentElementProperty =
				new AutomationProperty (IsContentElementPropertyId,
					"AutomationElementIdentifiers.IsContentElementProperty");
			IsControlElementProperty =
				new AutomationProperty (IsControlElementPropertyId,
					"AutomationElementIdentifiers.IsControlElementProperty");
			IsDockPatternAvailableProperty =
				new AutomationProperty (IsDockPatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsDockPatternAvailableProperty");
			IsEnabledProperty =
				new AutomationProperty (IsEnabledPropertyId,
					"AutomationElementIdentifiers.IsEnabledProperty");
			IsExpandCollapsePatternAvailableProperty =
				new AutomationProperty (IsExpandCollapsePatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty");
			IsGridItemPatternAvailableProperty =
				new AutomationProperty (IsGridItemPatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsGridItemPatternAvailableProperty");
			IsGridPatternAvailableProperty =
				new AutomationProperty (IsGridPatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsGridPatternAvailableProperty");
			IsInvokePatternAvailableProperty =
				new AutomationProperty (IsInvokePatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsInvokePatternAvailableProperty");
			IsKeyboardFocusableProperty =
				new AutomationProperty (IsKeyboardFocusablePropertyId,
					"AutomationElementIdentifiers.IsKeyboardFocusableProperty");
			IsMultipleViewPatternAvailableProperty =
				new AutomationProperty (IsMultipleViewPatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsMultipleViewPatternAvailableProperty");
			IsOffscreenProperty =
				new AutomationProperty (IsOffscreenPropertyId,
					"AutomationElementIdentifiers.IsOffscreenProperty");
			IsPasswordProperty =
				new AutomationProperty (IsPasswordPropertyId,
					"AutomationElementIdentifiers.IsPasswordProperty");
			IsRangeValuePatternAvailableProperty =
				new AutomationProperty (IsRangeValuePatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty");
			IsRequiredForFormProperty =
				new AutomationProperty (IsRequiredForFormPropertyId,
					"AutomationElementIdentifiers.IsRequiredForFormProperty");
			IsScrollItemPatternAvailableProperty =
				new AutomationProperty (IsScrollItemPatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty");
			IsScrollPatternAvailableProperty =
				new AutomationProperty (IsScrollPatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsScrollPatternAvailableProperty");
			IsSelectionItemPatternAvailableProperty =
				new AutomationProperty (IsSelectionItemPatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty");
			IsSelectionPatternAvailableProperty =
				new AutomationProperty (IsSelectionPatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsSelectionPatternAvailableProperty");
			IsTableItemPatternAvailableProperty =
				new AutomationProperty (IsTableItemPatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsTableItemPatternAvailableProperty");
			IsTablePatternAvailableProperty =
				new AutomationProperty (IsTablePatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsTablePatternAvailableProperty");
			IsTextPatternAvailableProperty =
				new AutomationProperty (IsTextPatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsTextPatternAvailableProperty");
			IsTogglePatternAvailableProperty =
				new AutomationProperty (IsTogglePatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsTogglePatternAvailableProperty");
			IsTransformPatternAvailableProperty =
				new AutomationProperty (IsTransformPatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsTransformPatternAvailableProperty");
			IsValuePatternAvailableProperty =
				new AutomationProperty (IsValuePatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsValuePatternAvailableProperty");
			IsWindowPatternAvailableProperty =
				new AutomationProperty (IsWindowPatternAvailablePropertyId,
					"AutomationElementIdentifiers.IsWindowPatternAvailableProperty");
			ItemStatusProperty =
				new AutomationProperty (ItemStatusPropertyId,
					"AutomationElementIdentifiers.ItemStatusProperty");
			ItemTypeProperty =
				new AutomationProperty (ItemTypePropertyId,
					"AutomationElementIdentifiers.ItemTypeProperty");
			LabeledByProperty =
				new AutomationProperty (LabeledByPropertyId,
					"AutomationElementIdentifiers.LabeledByProperty");
			LocalizedControlTypeProperty =
				new AutomationProperty (LocalizedControlTypePropertyId,
					"AutomationElementIdentifiers.LocalizedControlTypeProperty");
			NameProperty =
				new AutomationProperty (NamePropertyId,
					"AutomationElementIdentifiers.NameProperty");
			NativeWindowHandleProperty =
				new AutomationProperty (NativeWindowHandlePropertyId,
					"AutomationElementIdentifiers.NativeWindowHandleProperty");
			OrientationProperty =
				new AutomationProperty (OrientationPropertyId,
					"AutomationElementIdentifiers.OrientationProperty");
			ProcessIdProperty =
				new AutomationProperty (ProcessIdPropertyId,
					"AutomationElementIdentifiers.ProcessIdProperty");
			RuntimeIdProperty =
				new AutomationProperty (RuntimeIdPropertyId,
					"AutomationElementIdentifiers.RuntimeIdProperty");
			
			// Automation Events			
			WindowDeactivatedEvent =
				new AutomationEvent (WindowDeactivatedEventId,
				                     "AutomationElementIdentifiers.WindowDeactivatedEvent");
			KeyEvent =
				new AutomationEvent (KeyEventId,
				                     "AutomationElementIdentifiers.KeyEvent");
			AsyncContentLoadedEvent =
				new AutomationEvent (AsyncContentLoadedEventId,
				                     "AutomationElementIdentifiers.AsyncContentLoadedEvent");
			AutomationFocusChangedEvent =
				new AutomationEvent (AutomationFocusChangedEventId,
				                     "AutomationElementIdentifiers.AutomationFocusChangedEvent");
			AutomationPropertyChangedEvent =
				new AutomationEvent (AutomationPropertyChangedEventId,
				                     "AutomationElementIdentifiers.AutomationPropertyChangedEvent");
			LayoutInvalidatedEvent =
				new AutomationEvent (LayoutInvalidatedEventId,
				                     "AutomationElementIdentifiers.LayoutInvalidatedEvent");
			MenuClosedEvent =
				new AutomationEvent (MenuClosedEventId,
				                     "AutomationElementIdentifiers.MenuClosedEvent");
			MenuOpenedEvent =
				new AutomationEvent (MenuOpenedEventId,
				                     "AutomationElementIdentifiers.MenuOpenedEvent");
			StructureChangedEvent =
				new AutomationEvent (StructureChangedEventId,
				                     "AutomationElementIdentifiers.StructureChangedEvent");
			ToolTipClosedEvent =
				new AutomationEvent (ToolTipClosedEventId,
				                     "AutomationElementIdentifiers.ToolTipClosedEvent");
			ToolTipOpenedEvent =
				new AutomationEvent (ToolTipOpenedEventId,
				                     "AutomationElementIdentifiers.ToolTipOpenedEvent");
		}
		
#endregion

#region Internal Fields
		
		internal static readonly AutomationEvent WindowDeactivatedEvent;

		internal static readonly AutomationEvent KeyEvent;

		internal static readonly AutomationProperty HasNativeAccessibilityObjectProperty;

		internal static readonly AutomationProperty NativeAccessibilityObjectProperty;

		internal static readonly AutomationProperty IsNotifyIconProperty;

#endregion
		
#region Public Fields
		
		public static readonly AutomationProperty AcceleratorKeyProperty;
		
		public static readonly AutomationProperty AccessKeyProperty;

		public static readonly AutomationEvent AsyncContentLoadedEvent;
		
		public static readonly AutomationEvent AutomationFocusChangedEvent;
		
		public static readonly AutomationProperty AutomationIdProperty;
		
		public static readonly AutomationEvent AutomationPropertyChangedEvent;
		
		public static readonly AutomationProperty BoundingRectangleProperty;
		
		public static readonly AutomationProperty ClassNameProperty;
		
		public static readonly AutomationProperty ClickablePointProperty;
		
		public static readonly AutomationProperty ControlTypeProperty;
		
		public static readonly AutomationProperty CultureProperty;
		
		public static readonly AutomationProperty FrameworkIdProperty;
		
		public static readonly AutomationProperty HasKeyboardFocusProperty;
		
		public static readonly AutomationProperty HelpTextProperty;
		
		public static readonly AutomationProperty IsContentElementProperty;
		
		public static readonly AutomationProperty IsControlElementProperty;
		
		public static readonly AutomationProperty IsDockPatternAvailableProperty;
		
		public static readonly AutomationProperty IsEnabledProperty;
		
		public static readonly AutomationProperty IsExpandCollapsePatternAvailableProperty;
		
		public static readonly AutomationProperty IsGridItemPatternAvailableProperty;
		
		public static readonly AutomationProperty IsGridPatternAvailableProperty;
		
		public static readonly AutomationProperty IsInvokePatternAvailableProperty;
		
		public static readonly AutomationProperty IsKeyboardFocusableProperty;
		
		public static readonly AutomationProperty IsMultipleViewPatternAvailableProperty;
		
		public static readonly AutomationProperty IsOffscreenProperty;
		
		public static readonly AutomationProperty IsPasswordProperty;
		
		public static readonly AutomationProperty IsRangeValuePatternAvailableProperty;
		
		public static readonly AutomationProperty IsRequiredForFormProperty;
		
		public static readonly AutomationProperty IsScrollItemPatternAvailableProperty;
		
		public static readonly AutomationProperty IsScrollPatternAvailableProperty;
		
		public static readonly AutomationProperty IsSelectionItemPatternAvailableProperty;
		
		public static readonly AutomationProperty IsSelectionPatternAvailableProperty;
		
		public static readonly AutomationProperty IsTableItemPatternAvailableProperty;
		
		public static readonly AutomationProperty IsTablePatternAvailableProperty;
		
		public static readonly AutomationProperty IsTextPatternAvailableProperty;
		
		public static readonly AutomationProperty IsTogglePatternAvailableProperty;
		
		public static readonly AutomationProperty IsTransformPatternAvailableProperty;
		
		public static readonly AutomationProperty IsValuePatternAvailableProperty;
		
		public static readonly AutomationProperty IsWindowPatternAvailableProperty;
		
		public static readonly AutomationProperty ItemStatusProperty;
		
		public static readonly AutomationProperty ItemTypeProperty;
		
		public static readonly AutomationProperty LabeledByProperty;
		
		public static readonly AutomationEvent LayoutInvalidatedEvent;
		
		public static readonly AutomationProperty LocalizedControlTypeProperty;
		
		public static readonly AutomationEvent MenuClosedEvent;
		
		public static readonly AutomationEvent MenuOpenedEvent;
		
		public static readonly AutomationProperty NameProperty;
		
		public static readonly AutomationProperty NativeWindowHandleProperty;
		
		public static readonly Object NotSupported;
		
		public static readonly AutomationProperty OrientationProperty;
		
		public static readonly AutomationProperty ProcessIdProperty;
		
		public static readonly AutomationProperty RuntimeIdProperty;
		
		public static readonly AutomationEvent StructureChangedEvent;
		
		public static readonly AutomationEvent ToolTipClosedEvent;
		
		public static readonly AutomationEvent ToolTipOpenedEvent;
		
#endregion
	}
}
