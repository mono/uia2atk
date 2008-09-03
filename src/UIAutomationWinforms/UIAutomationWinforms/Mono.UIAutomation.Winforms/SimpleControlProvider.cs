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
using System.Windows.Forms;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Navigation;
using SWFErrorProvider = System.Windows.Forms.ErrorProvider;
using SWFHelpProvider = System.Windows.Forms.HelpProvider;

namespace Mono.UIAutomation.Winforms
{
	internal abstract class SimpleControlProvider : IRawElementProviderSimple
	{
		
		#region Private Fields

		private Control control;
		private Dictionary<ProviderEventType, IConnectable> events;
		private Dictionary<AutomationPattern, IProviderBehavior> providerBehaviors;
		private int runtimeId;
		private ToolTipProvider toolTipProvider;
		private INavigation navigation;
		private SWFErrorProvider errorProvider;
		private HelpProvider helpProvider;

		#endregion
		
		#region Constructors
		
		protected SimpleControlProvider (Component component)
		{
			control = component as Control;
			
			events = new Dictionary<ProviderEventType,IConnectable> ();
			providerBehaviors = 
				new Dictionary<AutomationPattern,IProviderBehavior> ();
			
			runtimeId = -1;
			
			if (Control != null)
				InitializeInternalControlEvents ();
		}
		
		#endregion
		
		#region Public Properties
		
		public virtual Component Container {
			get { return control.Parent; }
		}
		
		public Control Control {
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
		
		#endregion
		
		#region Public Methods

		public virtual void InitializeEvents ()
		{
			// TODO: Add: EventStrategyType.IsOffscreenProperty, DefaultIsOffscreenPropertyEvent
			SetEvent (ProviderEventType.AutomationElementIsEnabledProperty, 
			          new AutomationIsEnabledPropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementNameProperty,
			          new AutomationNamePropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
			          new AutomationHasKeyboardFocusPropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementBoundingRectangleProperty,
			          new AutomationBoundingRectanglePropertyEvent (this));
			SetEvent (ProviderEventType.StructureChangedEvent,
			          new StructureChangedEvent (this));
		}
		
		public virtual void Terminate ()
		{
			if (Control != null) {
				foreach (IConnectable strategy in events.Values)
				    strategy.Disconnect (Control);
				foreach (IProviderBehavior behavior in providerBehaviors.Values)
					behavior.Disconnect (Control);
				
				TerminateInternalControlEvents ();
			}

			events.Clear ();
			providerBehaviors.Clear ();
		}

		public void SetEvent (ProviderEventType type, IConnectable strategy)
		{
			IConnectable value;
			
			if (events.TryGetValue (type, out value) == true) {			
				if (Control != null)
					value.Disconnect (Control);
				events.Remove (type);
			}

			if (strategy != null) {
				events [type] = strategy;
				if (Control != null)
					strategy.Connect (Control);
			}
		}
		
		#endregion

		#region Protected Methods
		
		protected void SetBehavior (AutomationPattern pattern, IProviderBehavior behavior)
		{
			IProviderBehavior oldBehavior;
			if (providerBehaviors.TryGetValue (pattern, out oldBehavior) == true) {
				if (Control != null)
					oldBehavior.Disconnect (Control);
				providerBehaviors.Remove (pattern);
			}
			
			if (behavior != null) {
				providerBehaviors [pattern] = behavior;
				if (Control != null)
					behavior.Connect (Control);
			}
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
		public virtual object GetPatternProvider (int patternId)
		{
			foreach (IProviderBehavior behavior in ProviderBehaviors)
				if (behavior.ProviderPattern.Id == patternId)
					return behavior;
			return null;
		}
		
		public virtual object GetPropertyValue (int propertyId)
		{
			foreach (IProviderBehavior behavior in ProviderBehaviors) {
				object val = behavior.GetPropertyValue (propertyId);
				if (val != null)
					return val;
			}

			//TODO: Add IsDockPatternAvailableProperty, IsGridItemPatternAvailableProperty, IsTableItemPatternAvailableProperty
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
			else if (propertyId == AutomationElementIdentifiers.AutomationIdProperty.Id) {
				if (runtimeId == -1)
					runtimeId = Helper.GetUniqueRuntimeId ();

				return runtimeId;
			}
			
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
				return Control.CanFocus;
			else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
				System.Drawing.Rectangle bounds =
					GetControlScreenBounds ();				
				System.Drawing.Rectangle screen =
					Screen.GetWorkingArea (Control);
				// True iff the *entire* control is off-screen
				return !screen.Contains (bounds.Left, bounds.Bottom) &&
					!screen.Contains (bounds.Left, bounds.Top) &&
					!screen.Contains (bounds.Right, bounds.Bottom) &&
					!screen.Contains (bounds.Right, bounds.Top);
			}
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return Control.Focused;
			else if (propertyId == AutomationElementIdentifiers.IsControlElementProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
				return Helper.RectangleToRect (GetControlScreenBounds ());
			} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id) {
				if (Control.Visible == false)
					return null;
				else {
					// TODO: Test. MS behavior is different.
					Rect rectangle = (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					return new Point (rectangle.X, rectangle.Y);
				}
			} else if (propertyId == AutomationElementIdentifiers.HelpTextProperty.Id) {
				if (toolTipProvider == null)
					return null;
				else
					return toolTipProvider.ToolTip.GetToolTip (Control);
			} else
				return null;
		}

		public virtual IRawElementProviderSimple HostRawElementProvider {
			get {
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
		
		private System.Drawing.Rectangle GetControlScreenBounds ()
		{
			if (control.Parent == null || control.TopLevelControl == null)
				return Control.Bounds;
			else
				return Control.TopLevelControl.RectangleToScreen (Control.Bounds);
		}

		private void InitializeInternalControlEvents ()
		{
			InitializeToolTipInternalEvents ();
			InitializeErrorProviderInternalEvents ();
			InitializeHelpProviderInternalEvents ();
		}
		
		private void TerminateInternalControlEvents ()
		{
			TerminateToolTipInternalEvents ();
			TerminateErrorProviderInternalEvents ();
			TerminateHelpProviderInternalEvents ();
		}
		
		#endregion
		
		#region Private Methods: ToolTip
		
		private void InitializeToolTipInternalEvents ()
		{
			GetPrivateToolTipField ();
			
			Control.UIAToolTipHookedUp += new EventHandler (OnToolTipHookup);
			Control.UIAToolTipUnhookedUp += new EventHandler (OnToolTipUnhookup);
		}
		
		private void TerminateToolTipInternalEvents ()
		{
			Control.UIAToolTipHookedUp -= new EventHandler (OnToolTipHookup);
			Control.UIAToolTipUnhookedUp -= new EventHandler (OnToolTipUnhookup);
			
			if (toolTipProvider != null) {
				ProviderFactory.ReleaseProvider (toolTipProvider.ToolTip);
				toolTipProvider = null;
			}
		}

		private void GetPrivateToolTipField ()
		{
			ToolTip tooltip = Control.UIAToolTip;

			if (toolTipProvider != null && tooltip != toolTipProvider.ToolTip) {
				ProviderFactory.ReleaseProvider (toolTipProvider.ToolTip);
				toolTipProvider = null;					
			}

			if (toolTipProvider == null
			    || (toolTipProvider != null && toolTipProvider.ToolTip != tooltip))
				toolTipProvider = (ToolTipProvider) ProviderFactory.GetProvider (tooltip);
		}
		
#pragma warning disable 169
		
		private void OnToolTipHookup (object sender, EventArgs args)
		{
			GetPrivateToolTipField ();
		}

		private void OnToolTipUnhookup (object sender, EventArgs args)
		{		
			if (toolTipProvider != null) {
				ProviderFactory.ReleaseProvider (toolTipProvider.ToolTip);
				toolTipProvider = null;
			}
		}
		
#pragma warning restore 169
		
		#endregion
		
		#region Private Methods: ErrorProvider
		
		private void InitializeErrorProviderInternalEvents ()
		{
			GetPrivateErrorProviderField ();
			
			Control.UIAErrorProviderHookedUp += new ControlEventHandler (OnErrorProviderHookup);
			Control.UIAErrorProviderUnhookedUp += new ControlEventHandler (OnErrorProviderUnhookup);
		}
		
		private void TerminateErrorProviderInternalEvents ()
		{
			Control.UIAErrorProviderHookedUp -= new ControlEventHandler (OnErrorProviderHookup);
			Control.UIAErrorProviderUnhookedUp -= new ControlEventHandler (OnErrorProviderUnhookup);
			
			if (errorProvider != null) {
				ProviderFactory.ReleaseProvider (errorProvider);
				errorProvider = null;
			}
		}

		private void GetPrivateErrorProviderField ()
		{
			errorProvider = Control.UIAErrorProvider;
		}
		
#pragma warning disable 169		
		
		private void OnErrorProviderHookup (object sender, ControlEventArgs args)
		{
			GetPrivateErrorProviderField ();
			
			//TODO: The following call may fail when errorProvider is null
			ErrorProvider.InstancesTracker.AddControl (args.Control, 
			                                           Control.Parent,
			                                           errorProvider);			
		}

		private void OnErrorProviderUnhookup (object sender, ControlEventArgs args)
		{
			ErrorProvider.InstancesTracker.RemoveControl (args.Control,
			                                              args.Control.Parent,
			                                              errorProvider);
			errorProvider = null;
		}
		
#pragma warning restore 169
		
		#endregion
		
		#region Private Methods: HelpProvider
		
		private void GetPrivateHelpProviderField ()
		{
			SWFHelpProvider swfHelpProvider = Control.UIAHelpProvider;

			if (helpProvider != null && swfHelpProvider != helpProvider.SWFHelpProvider) {
				ProviderFactory.ReleaseProvider (helpProvider.SWFHelpProvider);
				helpProvider  = null;					
			}

			if (helpProvider == null
			    || (helpProvider != null && helpProvider.SWFHelpProvider != swfHelpProvider))
				helpProvider = (HelpProvider) ProviderFactory.GetProvider (swfHelpProvider);
		}
		
		private void InitializeHelpProviderInternalEvents ()
		{
			GetPrivateHelpProviderField ();
			
			Control.UIAHelpProviderHookedUp += new EventHandler (OnHelpProviderHookup);
			Control.UIAHelpProviderUnhookedUp += new EventHandler (OnHelpProviderUnhookedUp);
		}
		
		private void TerminateHelpProviderInternalEvents ()
		{
			Control.UIAHelpProviderHookedUp -= new EventHandler (OnHelpProviderHookup);
			Control.UIAHelpProviderUnhookedUp -= new EventHandler (OnHelpProviderUnhookedUp);
			
			if (helpProvider != null) {
				ProviderFactory.ReleaseProvider (helpProvider.SWFHelpProvider);
				helpProvider = null;
			}
		}
		
#pragma warning disable 169
		
		private void OnHelpProviderHookup (object sender, EventArgs args)
		{
			GetPrivateHelpProviderField ();
		}

		private void OnHelpProviderUnhookedUp (object sender, EventArgs args)
		{		
			if (helpProvider != null) {
				ProviderFactory.ReleaseProvider (helpProvider.SWFHelpProvider);
				helpProvider = null;
			}
		}
		
#pragma warning restore 169
		
		#endregion
	}
}
