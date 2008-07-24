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

		#endregion
		
		#region Protected Fields
		
		protected INavigation navigation;
		
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
		
		public virtual INavigation Navigation {
			get { 
				if (navigation == null) 
					navigation = new SimpleNavigation (this);

				return navigation; 
			}
			set { navigation = value; }
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
//			SetEvent (ProviderEventType.StructureChangedEvent,
//			          new StructureChangedEvent (this));
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
			
			//TODO: Terminate Navigation?
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
			
			//Control-like properties
			if (Control == null)
				return null;			
			else if (propertyId == AutomationElementIdentifiers.AutomationIdProperty.Id) {
				if (runtimeId == -1)
					runtimeId = Helper.GetUniqueRuntimeId ();

				return runtimeId;
			} else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
				return Control.Enabled;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return Control.Text;
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return Control.CanFocus;
			else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
				return !Control.Visible;
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return Control.Focused;
			else if (propertyId == AutomationElementIdentifiers.IsControlElementProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
				if (Control.Container == null)
					return Helper.RectangleToRect (Control.Bounds);
				else
					return Helper.RectangleToRect (Control.Parent.RectangleToScreen (Control.Bounds));
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
		
		#region Private Methods: ToolTip
		
		private void InitializeInternalControlEvents ()
		{
			GetPrivateToolTipField ();
			
			try {
				Helper.AddPrivateEvent (typeof (Control), 
				                        Control, 
				                        "ToolTipHookup",
				                        this, 
				                        "OnToolTipHookup");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: ToolTipShown not defined in {1}",
				                   GetType (),
				                   typeof (Control));
			}
			
			try {
				Helper.AddPrivateEvent (typeof (Control), 
				                        Control, 
				                        "ToolTipUnhookup",
				                        this, 
				                        "OnToolTipUnhookup");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: ToolTipHidden not defined in {1}",
				                   GetType (),
				                   typeof (Control));
			}

		}
		
		private void TerminateInternalControlEvents ()
		{
			try {
				Helper.RemovePrivateEvent (typeof (Control), 
				                           Control, 
				                           "ToolTipHookup",
				                           this, 
				                           "OnToolTipHookup");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: ToolTipShown not defined in {1}",
				                   GetType (),
				                   typeof (Control));
			}
			
			try {
				Helper.RemovePrivateEvent (typeof (Control), 
				                           Control, 
				                           "ToolTipUnhookup",
				                           this, 
				                           "OnToolTipUnhookup");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: ToolTipUnhookup not defined in {1}",
				                   GetType (),
				                   typeof (Control));
			}			
			
			if (toolTipProvider != null) {
				ProviderFactory.ReleaseProvider (toolTipProvider.ToolTip);
				toolTipProvider = null;
			}

		}
		
		private void GetPrivateToolTipField ()
		{
			ToolTip tooltip = null;

			try {
				tooltip = (ToolTip) Helper.GetPrivateField (typeof (Control),
				                                            Control,
				                                            "tool_tip");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: tool_tip field not defined in {1}",
				                   GetType (),
				                   typeof (Control));
			}
			
			if (toolTipProvider != null && tooltip != toolTipProvider.ToolTip) {
				ProviderFactory.ReleaseProvider (toolTipProvider.ToolTip);
				toolTipProvider = null;					
			}

			if (toolTipProvider == null
			    || (toolTipProvider != null && toolTipProvider.ToolTip != tooltip))
				toolTipProvider = (ToolTipProvider) ProviderFactory.GetProvider (tooltip);
		}
		
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
		
		#endregion
	}
}
