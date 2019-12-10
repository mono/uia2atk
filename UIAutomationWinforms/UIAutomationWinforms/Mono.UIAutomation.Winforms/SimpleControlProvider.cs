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
//      Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using SWF = System.Windows.Forms;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.Unix;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Navigation;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace Mono.UIAutomation.Winforms
{
	class DummyComponent : Component
	{
	}

	internal abstract class SimpleControlProvider : IRawElementProviderSimple
	{
		#region Private Static Fields

		private static int? pid = null;

		#endregion

		#region Private Fields

		private SWF.Control control;
		private Component component;
		private Dictionary<ProviderEventType, IConnectable> events;
		private Dictionary<AutomationPattern, IProviderBehavior> providerBehaviors;
		private SWF.ToolTip tooltip;
		private SWF.ErrorProvider errorProvider;

		protected readonly int runtimeId = Helper.GetUniqueRuntimeId ();

		#endregion
		
		#region Constructors

		protected SimpleControlProvider (Component component)
		{
			this.component = component ?? new DummyComponent ();
			control = component as SWF.Control;
			
			events = new Dictionary<ProviderEventType,IConnectable> ();
			providerBehaviors = 
				new Dictionary<AutomationPattern,IProviderBehavior> ();
			
			if (Control != null) {
				ErrorProvider = ErrorProviderListener.GetErrorProviderFromControl (Control);
				ToolTip = ToolTipListener.GetToolTipFromControl (Control);
			}
		}
		
		#endregion
		
		#region Public Properties
		
		public virtual Component Container {
			get { return control != null ? control.Parent : null; }
		}

		public Component Component {
			get { return component; }
		}
		
		public SWF.Control Control {
			get { return control; }
		}
		
		public SWF.ToolTip ToolTip {
			get { return tooltip; }
			set { tooltip = value; }
		}
		
		public SWF.ErrorProvider ErrorProvider {
			get { return errorProvider; }
			set { errorProvider = value; }
		}

		// Control-based providers return Control but Component-based 
		// providers will return their SWF.Control associated
		public virtual SWF.Control AssociatedControl {
			get { return Control; }
		}

		#endregion

		#region Public Events
		
		public event ProviderBehaviorEventHandler ProviderBehaviorSet;
		
		#endregion
		
		#region Public Methods

		public virtual void Initialize ()
		{
			SetEvent (ProviderEventType.AutomationElementControlTypeProperty,
			          new AutomationControlTypePropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementIsPatternAvailableProperty,
			          new AutomationIsPatternAvailablePropertyEvent (this));

			// These events only apply to Control providers
			if (Control == null)
				return;
			
			SetEvent (ProviderEventType.AutomationElementIsOffscreenProperty,
			          new AutomationIsOffscreenPropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementIsEnabledProperty, 
			          new AutomationIsEnabledPropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementNameProperty,
			          new AutomationNamePropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
			          new AutomationHasKeyboardFocusPropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementBoundingRectangleProperty,
			          new AutomationBoundingRectanglePropertyEvent (this));
			SetEvent (ProviderEventType.AutomationFocusChangedEvent,
			          new AutomationFocusChangedEvent (this));
			SetEvent (ProviderEventType.AutomationElementLabeledByProperty,
			          new AutomationLabeledByPropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementIsKeyboardFocusableProperty,
			          new AutomationIsKeyboardFocusablePropertyEvent (this));
			
			//TODO: We need deeper tests before uncommenting this.
//			SetEvent (ProviderEventType.StructureChangedEvent,
//			          new StructureChangedEvent (this));
		}
		
		public virtual void Terminate ()
		{
			foreach (IConnectable strategy in events.Values)
			    strategy.Disconnect ();
			foreach (IProviderBehavior behavior in providerBehaviors.Values)
				behavior.Disconnect ();

			events.Clear ();
			providerBehaviors.Clear ();
		}

		public void SetEvent (ProviderEventType type, IConnectable strategy)
		{
			IConnectable value;
			
			if (events.TryGetValue (type, out value) == true) {			
				value.Disconnect ();
				events.Remove (type);
			}

			if (strategy != null) {
				events [type] = strategy;
				strategy.Connect ();
			}
		}
		
		#endregion

		#region Protected Methods
		
		protected void SetBehavior (AutomationPattern pattern, IProviderBehavior behavior)
		{
			IProviderBehavior oldBehavior;
			bool exists = false;
			
			if (providerBehaviors.TryGetValue (pattern, out oldBehavior) == true) {
				oldBehavior.Disconnect ();
				providerBehaviors.Remove (pattern);
				exists = true;
			}
			
			if (behavior != null) {
				providerBehaviors [pattern] = behavior;
				behavior.Connect ();
			}

			OnProviderBehaviorSet (new ProviderBehaviorEventArgs (behavior,
			                                                      pattern,
			                                                      exists));

		}
		
		protected IProviderBehavior GetBehavior (AutomationPattern pattern)
		{
			IProviderBehavior behavior;
			if (providerBehaviors.TryGetValue (pattern, out behavior))
				return behavior;
			
			return null;
		}
		
		protected virtual bool IsBehaviorEnabled (AutomationPattern pattern) 
		{
			return providerBehaviors.ContainsKey (pattern);
		}

		protected virtual System.Drawing.Rectangle ScreenBounds
		{
			get {
				if (Control == null || !Control.Visible)
					return System.Drawing.Rectangle.Empty;

				return Helper.RectToRectangle (
					Helper.GetControlScreenBounds (Control.Bounds, Control)
				);
			}
		}

		protected virtual object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty.Id)
				return IsBehaviorEnabled (ExpandCollapsePatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsGridPatternAvailableProperty.Id)
				return IsBehaviorEnabled (GridPatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsLegacyIAccessiblePatternAvailableProperty.Id)
				return IsBehaviorEnabled (LegacyIAccessiblePatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsInvokePatternAvailableProperty.Id)
				return IsBehaviorEnabled (InvokePatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsMultipleViewPatternAvailableProperty.Id)
				return IsBehaviorEnabled (MultipleViewPatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty.Id)
				return IsBehaviorEnabled (RangeValuePatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty.Id)
				return IsBehaviorEnabled (ScrollItemPatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsScrollPatternAvailableProperty.Id)
				return IsBehaviorEnabled (ScrollPatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty.Id)
				return IsBehaviorEnabled (SelectionItemPatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsSelectionPatternAvailableProperty.Id)
				return IsBehaviorEnabled (SelectionPatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id)
				return IsBehaviorEnabled (TablePatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsTextPatternAvailableProperty.Id)
				return IsBehaviorEnabled (TextPatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsTogglePatternAvailableProperty.Id)
				return IsBehaviorEnabled (TogglePatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsTransformPatternAvailableProperty.Id)
				return IsBehaviorEnabled (TransformPatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsValuePatternAvailableProperty.Id)
				return IsBehaviorEnabled (ValuePatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsWindowPatternAvailableProperty.Id)
				return IsBehaviorEnabled (WindowPatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsDockPatternAvailableProperty.Id)
				return IsBehaviorEnabled (DockPatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsGridItemPatternAvailableProperty.Id)
				return IsBehaviorEnabled (GridItemPatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsTableItemPatternAvailableProperty.Id)
				return IsBehaviorEnabled (TableItemPatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.AutomationIdProperty.Id)
			{
				string name = (Component as SWF.Control)?.Name;
				if (String.IsNullOrEmpty (name))
					return runtimeId.ToString ();
				else
					return name;
			} else if (propertyId == AutomationElementIdentifiers.IsControlElementProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.IsPasswordProperty.Id)
				return false;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id) {
				object controlTypeIdObj =
					GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
				if (controlTypeIdObj != null) {
					int controlTypeId = (int) controlTypeIdObj;
					//On Win7, for DataGrid, DataItem and List, the ControlType.LocalizedControlType
					//is inconsistent with the element's LocalizedControlType property.
					if (controlTypeId == ControlType.DataGrid.Id)
						return Catalog.GetString ("data grid");
					else if (controlTypeId == ControlType.DataItem.Id)
						return Catalog.GetString ("data item");
					else if (controlTypeId == ControlType.List.Id)
						return Catalog.GetString ("list");
					else {
						var ct = ControlType.LookupById (controlTypeId);
						return ct != null ? ct.LocalizedControlType : null;
					}
				} else
					return null;
			} else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
				Rect bounds = (Rect)
					GetPropertyValue (AEIds.BoundingRectangleProperty.Id);
				if (Control == null)
					return Helper.IsOffScreen (Helper.RectToRectangle (bounds));
				return Helper.IsOffScreen (bounds, Control);
			}
			else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
				return BoundingRectangleProperty;
			
			//Control-like properties
			if (Control == null)
				return null;
			else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
				return Control.Enabled;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id) {
				if (!string.IsNullOrEmpty (Control.AccessibleName)) 
					return Control.AccessibleName;
				
				IRawElementProviderSimple label =
					GetPropertyValue (AutomationElementIdentifiers.LabeledByProperty.Id)
						as IRawElementProviderSimple;
				if (label == null) {
					int controlType = (int) GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
					// http://msdn.microsoft.com/en-us/library/ms748367.aspx
					// "The Name property should never contain the textual contents of the edit control."
					if (controlType == ControlType.Edit.Id || controlType == ControlType.Document.Id)
						return string.Empty;
					else
						return Helper.StripAmpersands (Control.Text);
				} else
					return label.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			} else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id) {
				IRawElementProviderFragment thisAsFragment = this as IRawElementProviderFragment;
				if (thisAsFragment == null)
					return null;

				IRawElementProviderFragment parent = thisAsFragment.Navigate (NavigateDirection.Parent);
				if (parent == null || parent == thisAsFragment)
					return null;

				IRawElementProviderFragment closestLabel = null;
				double closestLabelDistance = double.MaxValue;

				parent.NavigateEachChildProvider ((IRawElementProviderFragment sibling) => {
					if (sibling == this)
						return;
					if ((int)sibling.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) == ControlType.Text.Id) {
						double siblingDistance;
						if ((siblingDistance = DistanceFrom (sibling)) < closestLabelDistance) {
							closestLabel = sibling;
							closestLabelDistance = siblingDistance;
						}
					}
				});
				
				return closestLabel;
				
			} else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return Control.CanFocus && Control.CanSelect;
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return Control.Focused;
			else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
				return Helper.GetClickablePoint (this);
			else if (propertyId == AEIds.FrameworkIdProperty.Id)
				return "WinForm"; // TODO: Localizable?
			else if (propertyId == AutomationElementIdentifiers.HelpTextProperty.Id) {
				if (ToolTip == null)
					return Control.AccessibleDescription ?? string.Empty;
				else
					return ToolTip.GetToolTip (Control);
			} else if (propertyId == AutomationElementIdentifiers.AccessKeyProperty.Id) {
				if (!Control.Text.Contains ("&"))
					return null;
				else {
					int index = Control.Text.LastIndexOf ('&') + 1;
					return "Alt+" + Control.Text.Substring (index, 1);
				}
			} else if (propertyId == AEIds.ProcessIdProperty.Id) {
				// TODO: Write test for this property
				if (!pid.HasValue)
					pid = System.Diagnostics.Process.GetCurrentProcess ().Id;
				return pid.Value;
			} else if (propertyId == AutomationElementIdentifiers.NativeWindowHandleProperty.Id)
				return Control.Handle; // TODO: Should be int, maybe?
			else
				return null;
		}

		protected void OnProviderBehaviorSet (ProviderBehaviorEventArgs args)
		{
			if (ProviderBehaviorSet != null)
				ProviderBehaviorSet (this, args);
		}
		
		#endregion
		
		#region Protected Properties
		
		protected IEnumerable<IProviderBehavior> ProviderBehaviors {
			get { return providerBehaviors.Values; }
		}

		protected virtual Rect BoundingRectangleProperty {
			get { return Helper.RectangleToRect (ScreenBounds); }
		}

		#endregion
		
		#region IRawElementProviderSimple: Specializations
	
		// TODO: Get this used in all base classes. Consider refactoring
		//       so that *all* pattern provider behaviors are dynamically
		//       attached to make this more uniform.
		public virtual object GetPatternProvider (int patternId)
		{
			foreach (IProviderBehavior behavior in ProviderBehaviors)
				if (behavior.ProviderPattern.Id == patternId)
					return behavior;
			return null;
		}
		
		public virtual object GetPropertyValue (int propertyId)
		{
			object val = null;

			foreach (IProviderBehavior behavior in ProviderBehaviors) {
				val = behavior.GetPropertyValue (propertyId);
				if (val != null)
					break;
			}

			if (val == null)
				val = GetProviderPropertyValue (propertyId) ?? Helper.GetDefaultAutomationPropertyValue (propertyId);

			if (propertyId == AEIds.IsOffscreenProperty.Id
			   || propertyId == AEIds.BoundingRectangleProperty.Id) {
				// The upper "if" is purely to enhance performance.
				if (Helper.IsFormMinimized (this)) {
					if (propertyId == AEIds.IsOffscreenProperty.Id)
						val = true;
					else if (propertyId == AEIds.BoundingRectangleProperty.Id) {
						Rect bound = (Rect) val;
						if (bound != Rect.Empty)
							// -32000 is to copy the Windows/.Net behavior
							bound.Offset (-32000, -32000);
						val = bound;
					}
				}
			}

			return val;
		}

		public virtual IRawElementProviderSimple HostRawElementProvider {
			get {
				// TODO: Address for Components (*Strip*)
				if (Control == null || Control.TopLevelControl == null)
					return null;
				else
					return AutomationInteropProvider.HostProviderFromHandle (Control.TopLevelControl.Handle);
			}
		}
		
		public virtual ProviderOptions ProviderOptions {
			get { return ProviderOptions.ServerSideProvider; }
		}

		#endregion
		
		#region Private Methods
		
		private double DistanceFrom (IRawElementProviderSimple otherProvider)
		{
			Rect bounds = (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
			Rect otherBounds = (Rect) otherProvider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
			
			if (control != null && control.RightToLeft == SWF.RightToLeft.Yes)
				return Distance (bounds.TopRight, otherBounds.TopRight);
			else
				return Distance (bounds.TopLeft, otherBounds.TopLeft);
		}
		
		private double Distance (System.Windows.Point p1, System.Windows.Point p2)
		{
			return System.Math.Sqrt ((p1.X - p2.X) * (p1.X - p2.X) +
			                         (p1.Y - p2.Y) * (p1.Y - p2.Y));
		}

		#endregion

	}
}
