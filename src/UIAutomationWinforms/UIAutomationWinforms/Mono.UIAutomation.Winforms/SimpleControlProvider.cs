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
using System.Windows.Forms;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	public abstract class SimpleControlProvider : IRawElementProviderSimple
	{
		
#region Private Fields

		private Control control;
		private Dictionary<ProviderEventType, IConnectable> events;
		private Dictionary<AutomationPattern, IProviderBehavior> providerBehaviors;
		private int runtime_id;

#endregion
		
#region Protected Fields
		
		protected INavigation navigation;
		
#endregion
		
#region Constructors
		
		protected SimpleControlProvider (Control control)
		{
			this.control = control;
			
			events = new Dictionary<ProviderEventType,IConnectable> ();
			providerBehaviors =
				new Dictionary<AutomationPattern,IProviderBehavior> ();
			
			runtime_id = -1;
		}
		
#endregion
		
#region Public Properties
		
		public virtual Control Control {
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
		
		//TODO: I'm still wondering about the right name for this method
		public virtual void Terminate ()
		{
			if (Control != null) {
				foreach (IConnectable strategy in events.Values)
				    strategy.Disconnect (Control);
				foreach (IProviderBehavior behavior in providerBehaviors.Values)
					behavior.Disconnect (Control);
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
		
		protected IEnumerable<IProviderBehavior> ProviderBehaviors
		{
			get {
				return providerBehaviors.Values;
			}
		}
#endregion
		
#region IRawElementProviderSimple Members
	
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
			
			if (Control == null)
				return null;			
			else if (propertyId == AutomationElementIdentifiers.AutomationIdProperty.Id) {
				if (runtime_id == -1)
					runtime_id = Helper.GetUniqueRuntimeId ();

				return runtime_id;
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
				if (Control.Parent == null)
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
			}
			
			return null;
		}

		public virtual IRawElementProviderSimple HostRawElementProvider {
			get {
				if (Control == null)
					return null;
				else
					return AutomationInteropProvider.HostProviderFromHandle (Control.TopLevelControl.Handle);
			}
		}
		
		public virtual ProviderOptions ProviderOptions {
			get {
				return ProviderOptions.ServerSideProvider;
			}
		}

#endregion
	}
}
