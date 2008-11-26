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
using SCG = System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public abstract class ComponentParentAdapter : ParentAdapter, Atk.ComponentImplementor
	{
		//FIXME: should we receive a IRawElementProviderFragment instead? this way we can drop ArgumentExceptions in derived classes' ctors
		public ComponentParentAdapter (IRawElementProviderSimple provider) : base (provider)
		{
			componentExpert = new ComponentImplementorHelper (this);
		}
		
		private ComponentImplementorHelper componentExpert;

		internal Atk.Relation RadioButsRelation { get; private set; }
		
		internal override void AddOneChild (Atk.Object child)
		{
			base.AddOneChild (child);

			RadioButton rad = child as RadioButton;
			if (rad == null)
				return;

			if (RadioButsRelation == null)
				RadioButsRelation = new Atk.Relation (new Atk.Object [] { rad }, Atk.RelationType.MemberOf);
			else
				RadioButsRelation.AddTarget (rad);
		}

		internal override void RemoveChild (Atk.Object childToRemove)
		{
			base.RemoveChild (childToRemove);

			RadioButton rad = childToRemove as RadioButton;
			if (rad == null)
				return;

			SCG.List <Atk.Object> restRads = new SCG.List<Atk.Object> (RadioButsRelation.Target);
			restRads.Remove ((Atk.Object)rad);
			RadioButsRelation = new Atk.Relation (restRads.ToArray (), Atk.RelationType.MemberOf);
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
		
#endregion

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			if (componentExpert.CanResize)
				states.AddState (Atk.StateType.Resizable);
			else
				states.RemoveState (Atk.StateType.Resizable);

			return states;
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == AutomationElementIdentifiers.BoundingRectangleProperty) {
				// TODO: Handle BoundingRectangleProperty change
			} else
				base.RaiseAutomationPropertyChangedEvent (e);
		}

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			base.RaiseAutomationEvent (eventId, e);
		}
	}
}
