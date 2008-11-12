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
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	internal abstract class SimpleControlProvider : IRawElementProviderSimple
	{
		
		#region Private Fields

		private SWF.Control control;
		private Component component;
		private Dictionary<ProviderEventType, IConnectable> events;
		private Dictionary<AutomationPattern, IProviderBehavior> providerBehaviors;
		private int runtimeId;
		private SWF.ToolTip tooltip;
		private INavigation navigation;
		private SWF.ErrorProvider errorProvider;

		#endregion
		
		#region Constructors
		
		protected SimpleControlProvider (Component component)
		{
			this.component = component;
			control = component as SWF.Control;
			
			events = new Dictionary<ProviderEventType,IConnectable> ();
			providerBehaviors = 
				new Dictionary<AutomationPattern,IProviderBehavior> ();
			
			runtimeId = -1;
			
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
		
		public INavigation Navigation {
			get { return navigation; }
			set { 
				if (value != navigation && navigation != null)
					navigation.Terminate ();

				navigation = value; 
			}
		}
		
		public SWF.ToolTip ToolTip {
			get { return tooltip; }
			set { tooltip = value; }
		}
		
		public SWF.ErrorProvider ErrorProvider {
			get { return errorProvider; }
			set { errorProvider = value; }
		}
		
		#endregion
		
		#region Public Methods

		public virtual void Initialize ()
		{
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
			
			if ((exists == true && behavior == null)
			    || (exists == false && behavior != null))
				GenerateIsPatternEnabledEvent (pattern);
		}
		
		protected IProviderBehavior GetBehavior (AutomationPattern pattern)
		{
			IProviderBehavior behavior;
			if (providerBehaviors.TryGetValue (pattern, out behavior))
				return behavior;
			
			return null;
		}
		
		protected bool IsBehaviorEnabled (AutomationPattern pattern) 
		{
			return providerBehaviors.ContainsKey (pattern);
		}

		protected virtual System.Drawing.Rectangle GetControlScreenBounds ()
		{
			if (control.Parent == null || control.TopLevelControl == null)
				return Control.Bounds;
			else {
				if (Control.FindForm () == Control.Parent)
					return Control.TopLevelControl.RectangleToScreen (Control.Bounds);
				else
					return Control.Parent.RectangleToScreen (Control.Bounds);
			}
		}

		protected virtual object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty.Id)
				return IsBehaviorEnabled (ExpandCollapsePatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsGridPatternAvailableProperty.Id)
				return IsBehaviorEnabled (GridPatternIdentifiers.Pattern);
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
			else if (propertyId == AutomationElementIdentifiers.IsGridPatternAvailableProperty.Id)
				return IsBehaviorEnabled (GridPatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id)
				return IsBehaviorEnabled (TablePatternIdentifiers.Pattern);
			else if (propertyId == AutomationElementIdentifiers.AutomationIdProperty.Id) {
				if (runtimeId == -1)
					runtimeId = Helper.GetUniqueRuntimeId ();

				return runtimeId;
			} else if (propertyId == AutomationElementIdentifiers.IsControlElementProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.IsPasswordProperty.Id)
				return false;
			 
			//Control-like properties
			if (Control == null)
				return null;			
			else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
				return Control.Enabled;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id) {
				IRawElementProviderSimple label =
					GetPropertyValue (AutomationElementIdentifiers.LabeledByProperty.Id)
						as IRawElementProviderSimple;
				if (label == null)
					return string.Empty;
				else
					return label.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			}
			else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id) {
				IRawElementProviderFragment sibling = this as IRawElementProviderFragment;
				if (sibling == null)
					return null;
				IRawElementProviderFragment closestLabel = null;
				double closestLabelDistance = double.MaxValue;
				do {
					sibling = sibling.Navigate (NavigateDirection.NextSibling);
					if (sibling == null)
						break;
					if ((int)sibling.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) == ControlType.Text.Id) {
						double siblingDistance;
						if (closestLabel == null ||
						    ((siblingDistance = DistanceFrom (sibling)) < closestLabelDistance)) {
							closestLabel = sibling;
							closestLabelDistance = siblingDistance;
						}
					}
				} while (sibling != null && sibling != this);
				
				return closestLabel;
				
			} else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return Control.CanFocus && Control.CanSelect;
			else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
				return Helper.IsOffScreen (GetControlScreenBounds (), Control);
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return Control.Focused;
			else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
				return Helper.RectangleToRect (GetControlScreenBounds ());
			else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
				return Helper.GetClickablePoint (this);
			else if (propertyId == AutomationElementIdentifiers.HelpTextProperty.Id) {
				return ToolTip == null ? null : ToolTip.GetToolTip (Control);
			} else if (propertyId == AutomationElementIdentifiers.AccessKeyProperty.Id) {
				if (!Control.Text.Contains ("&"))
					return null;
				else {
					int index = Control.Text.LastIndexOf ('&') + 1;
					return "Alt+" + Control.Text.Substring (index, 1);
				}
			} else
				return null;
		}
		
		#endregion
		
		#region Protected Properties
		
		protected IEnumerable<IProviderBehavior> ProviderBehaviors {
			get { return providerBehaviors.Values; }
		}
		
		#endregion
		
		#region IRawElementProviderSimple: Specializations
	
		// TODO: Get this used in all base classes. Consider refactoring
		//       so that *all* pattern provider behaviors are dynamically
		//       attached to make this more uniform.
		public object GetPatternProvider (int patternId)
		{
			foreach (IProviderBehavior behavior in ProviderBehaviors)
				if (behavior.ProviderPattern.Id == patternId)
					return behavior;
			return null;
		}
		
		public object GetPropertyValue (int propertyId)
		{
			foreach (IProviderBehavior behavior in ProviderBehaviors) {
				object val = behavior.GetPropertyValue (propertyId);
				if (val != null)
					return val;
			}

			return GetProviderPropertyValue (propertyId);
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
			
			double [] pointDistances = new double [] {
				Distance (bounds.BottomLeft, otherBounds.BottomLeft),
				Distance (bounds.BottomLeft, otherBounds.BottomRight),
				Distance (bounds.BottomLeft, otherBounds.TopLeft),
				Distance (bounds.BottomLeft, otherBounds.TopRight),
				Distance (bounds.BottomRight, otherBounds.BottomLeft),
				Distance (bounds.BottomRight, otherBounds.BottomRight),
				Distance (bounds.BottomRight, otherBounds.TopLeft),
				Distance (bounds.BottomRight, otherBounds.TopRight),
				Distance (bounds.TopLeft, otherBounds.BottomLeft),
				Distance (bounds.TopLeft, otherBounds.BottomRight),
				Distance (bounds.TopLeft, otherBounds.TopLeft),
				Distance (bounds.TopLeft, otherBounds.TopRight),
				Distance (bounds.TopRight, otherBounds.BottomLeft),
				Distance (bounds.TopRight, otherBounds.BottomRight),
				Distance (bounds.TopRight, otherBounds.TopLeft),
				Distance (bounds.TopRight, otherBounds.TopRight)
			};
			
			double minDistance = double.MaxValue;
			
			foreach (double distance in pointDistances)
				if (distance < minDistance)
					minDistance = distance;
			
			return minDistance;
		}
		
		private double Distance (System.Windows.Point p1, System.Windows.Point p2)
		{
			return System.Math.Abs (System.Math.Sqrt ( System.Math.Pow (p1.X - p2.X, 2) +
			                                          System.Math.Pow (p1.Y - p2.Y, 2)));
		}
		
		private void GenerateIsPatternEnabledEvent (AutomationPattern pattern)
		{
			if (AutomationInteropProvider.ClientsAreListening == false
			    || navigation == null || navigation.Navigate (NavigateDirection.Parent) == null)
				return;
			
			AutomationProperty property = null;

			if (pattern == DockPatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsDockPatternAvailableProperty;
			else if (pattern == ExpandCollapsePatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty;
			else if (pattern == GridItemPatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsGridItemPatternAvailableProperty;
			else if (pattern == GridPatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsGridPatternAvailableProperty;
			else if (pattern == InvokePatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsInvokePatternAvailableProperty;
			else if (pattern == MultipleViewPatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsMultipleViewPatternAvailableProperty;
			else if (pattern == RangeValuePatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty;
			else if (pattern == ScrollItemPatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty;
			else if (pattern == ScrollPatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsScrollPatternAvailableProperty;
			else if (pattern == SelectionItemPatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty;
			else if (pattern == SelectionPatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsSelectionPatternAvailableProperty;
			else if (pattern == TableItemPatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsTableItemPatternAvailableProperty;
			else if (pattern == TablePatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsTablePatternAvailableProperty;
			else if (pattern == TextPatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsTextPatternAvailableProperty;
			else if (pattern == TogglePatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsTogglePatternAvailableProperty;
			else if (pattern == TransformPatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsTransformPatternAvailableProperty;
			else if (pattern == ValuePatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsValuePatternAvailableProperty;
			else if (pattern == WindowPatternIdentifiers.Pattern)
				property = AutomationElementIdentifiers.IsWindowPatternAvailableProperty;
			
			if (property == null) //This never should happen, theorically
				return;

			bool newValue = (bool) GetPropertyValue (property.Id);
			
			AutomationPropertyChangedEventArgs args 
				= new AutomationPropertyChangedEventArgs (property,
				                                          !newValue,
				                                          newValue);
			AutomationInteropProvider.RaiseAutomationPropertyChangedEvent (this, 
			                                                               args);
		}
		
		#endregion

	}
}
