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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public abstract class ComponentAdapter : Adapter, Atk.ComponentImplementor
	{
		public ComponentAdapter (IRawElementProviderSimple provider) : base (provider)
		{
			componentExpert = new ComponentImplementorHelper (this);
		}
		
		private ComponentImplementorHelper componentExpert;
		internal ComponentImplementorHelper ComponentExpert {
			get { return this.componentExpert; }
		}
		
#region ComponentImplementor Methods

		public virtual double Alpha
		{
			get {
				return componentExpert.Alpha;
			}
		}
		
		public virtual uint AddFocusHandler (Atk.FocusHandler handler)
		{
			return componentExpert.AddFocusHandler (handler);
		}

		public virtual bool Contains (int x, int y, Atk.CoordType coordType)
		{
			return componentExpert.Contains (x, y, coordType);
		}

		public virtual void GetExtents (out int x, out int y, out int width, out int height, Atk.CoordType coordType)
		{
			componentExpert.GetExtents (out x, out y, out width, out height, coordType);
		}
		
		public virtual void GetPosition (out int x, out int y, Atk.CoordType coordType)
		{
			componentExpert.GetPosition (out x, out y, coordType);
		}

		// we should use "override" instead of "new" when this bug is fixed and it gets
		// propragated to GTK#: http://bugzilla.gnome.org/show_bug.cgi?id=526752
		public virtual new Atk.Layer Layer {
			get { return componentExpert.Layer; }
		}
		
		public virtual new int MdiZorder {
			get { return componentExpert.MdiZorder; }
		}
		
		public virtual void GetSize (out int width, out int height)
		{
			componentExpert.GetSize (out width, out height);
		}
		
		public virtual bool GrabFocus ()
		{
			return componentExpert.GrabFocus ();
		}
		
		public virtual Atk.Object RefAccessibleAtPoint (int x, int y, Atk.CoordType coordType)
		{
			return componentExpert.RefAccessibleAtPoint (x, y, coordType);
		}
		
		public virtual void RemoveFocusHandler (uint handlerId)
		{
			componentExpert.RemoveFocusHandler (handlerId);
		}
		
		public virtual bool SetExtents (int x, int y, int width, int height, Atk.CoordType coordType)
		{
			return componentExpert.SetExtents (x, y, width, height, coordType);
		}
		
		public virtual bool SetPosition (int x, int y, Atk.CoordType coordType)
		{
			return componentExpert.SetPosition (x, y, coordType);
		}
		
		public virtual bool SetSize (int width, int height)
		{
			return componentExpert.SetSize (width, height);
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			if (Provider != null) {
				bool canFocus = (bool) Provider.GetPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id);
				if (canFocus)
					states.AddState (Atk.StateType.Focusable);
				else
					states.RemoveState (Atk.StateType.Focusable);

				bool focused = (bool) Provider.GetPropertyValue (
				  AutomationElementIdentifiers.HasKeyboardFocusProperty.Id);
				if (focused)
					states.AddState (Atk.StateType.Focused);
				else
					states.RemoveState (Atk.StateType.Focused);
				
				bool enabled = 
				  (bool) Provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
				if (enabled) {
					states.AddState (Atk.StateType.Sensitive);
					states.AddState (Atk.StateType.Enabled);
				} else {
					states.RemoveState (Atk.StateType.Sensitive);
					states.RemoveState (Atk.StateType.Enabled);
				}
			}
			
			if (componentExpert.CanResize)
				states.AddState (Atk.StateType.Resizable);
			else
				states.RemoveState (Atk.StateType.Resizable);

			return states;
		}

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs args)
		{
			base.RaiseAutomationEvent(eventId, args);
		}
		
		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == AutomationElementIdentifiers.BoundingRectangleProperty) {
				// TODO: Handle BoundingRectangleProperty change
			} else
				base.RaiseAutomationPropertyChangedEvent (e);
		}
#endregion
	}
}
