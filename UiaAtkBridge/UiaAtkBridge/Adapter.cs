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
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Services;

namespace UiaAtkBridge
{
	public abstract class Adapter : Atk.Object
	{

#region Constructors

		//FIXME: consider to add a new param here: Adapter parent; in order to have the parent always accessible, and
		//       prevent workarounds such as using provider navigation (like in the UiaAtkBridge.Window or 
		//       UiaAtkBridge.ComboBox cases)
		public Adapter (IRawElementProviderSimple provider)
		{
			Provider = provider;
			// Avoid setting the name.  Not sure why this is needed.
		}
		
#endregion

#region Adapter Methods
	
		protected void NotifyStateChange (Atk.StateType state) {
			NotifyStateChange (state, RefStateSet ().ContainsState (state));
		}
		
		protected void NotifyFocused (bool focused)
		{
			NotifyStateChange (Atk.StateType.Focused, focused);

			Window focusWindow = null;
			// FIXME: Gail sends the events in a slighly different order:
			// window:activate , object:state-changed:focused, object:state-changed:active
			if (focused) {
				Atk.Object container = Parent;
				while (container != null) {
					if (container is Window) {
						focusWindow = (Window)container;
						TopLevelRootItem.Instance.CheckAndHandleNewActiveWindow (focusWindow);
						break;
					}
					container = container.Parent;
				}
			}

			if (focused)
				Atk.Focus.TrackerNotify (this);
			if (focusWindow != null)
				focusWindow.SendActiveStateChange ();
		}

		public IRawElementProviderSimple Provider { get; private set; }

		protected bool manages_removal = false;

		// Returns true if this object will manage it's own removal
		// from the automation tree
		public virtual bool ManagesRemoval {
			get { return manages_removal; }
			set { manages_removal = value; }
		}
		
		public virtual void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId != AutomationElementIdentifiers.AutomationFocusChangedEvent)
				return;

			Atk.Object parent = this; //in case this.GetType () == typeof(UiaAtkBridge.Window) 
			while (!(parent is UiaAtkBridge.Window)) {
				if (parent == null) {
					Log.Error ("Parent of an object should not be null");
					return;
				}
				parent = parent.Parent;
			}
			TopLevelRootItem.Instance.CheckAndHandleNewActiveWindow ((UiaAtkBridge.Window)parent);
		}
		
		public virtual void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == AutomationElementIdentifiers.HasKeyboardFocusProperty) {
				bool focused = (bool)e.NewValue;
				NotifyFocused (focused);
			} else if (e.Property == AutomationElementIdentifiers.IsOffscreenProperty) { 
				bool offscreen = (bool)e.NewValue;
				NotifyStateChange (Atk.StateType.Visible, !offscreen);
			} else if (e.Property == AutomationElementIdentifiers.IsEnabledProperty) {
				bool enabled = (bool) e.NewValue;
				NotifyStateChange (Atk.StateType.Enabled, enabled);
				NotifyStateChange (Atk.StateType.Sensitive, enabled);
			} else if (e.Property == AutomationElementIdentifiers.HelpTextProperty) {
				Description = (string)e.NewValue;
			} else if (e.Property == AutomationElementIdentifiers.BoundingRectangleProperty) {
				EmitBoundsChanged ((System.Windows.Rect)e.NewValue);
			} else if (e.Property == AutomationElementIdentifiers.NameProperty) {
				string newName = (string) e.NewValue;

				// Don't set Name if we don't really want to
				// and don't fire events if we're not changing
				if (!(Name == null && newName == String.Empty)
				    && Name != newName) {
					UpdateNameProperty ((string)e.NewValue, false);
				}
			}
		}

		public void RemoveFromParent (ParentAdapter parent)
		{
			RemoveFromParent (parent, true);
		}

		public void RemoveFromParent (ParentAdapter parent, bool terminate)
		{
			NotifyStateChange (Atk.StateType.Showing, false);
			NotifyStateChange (Atk.StateType.Visible, false);

			//don't remove the parent if this was not the first parent
			if (parent == Parent)
				Parent = null;

			if (terminate)
				defunct = true;
		}
		
		internal virtual void PostInit ()
		{
			if (Provider == null)
				return;

			string desc = (string) Provider.GetPropertyValue (AutomationElementIdentifiers.HelpTextProperty.Id);
			if (!String.IsNullOrEmpty (desc))
				Description = desc;

			UpdateNameProperty (Provider.GetPropertyValue (
				AutomationElementIdentifiers.NameProperty.Id)
					as string,
				true);
		}

		protected virtual void UpdateNameProperty (string newName, bool fromCtor)
		{
			if (fromCtor) {
				if (!String.IsNullOrEmpty (newName))
					Name = newName;
				return;
			}

			if (Name == null && String.IsNullOrEmpty (newName))
				return;

			Name = newName ?? String.Empty;
		}
#endregion
		
#region Overrides
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			if (defunct) {
				states.AddState (Atk.StateType.Defunct);
				return states;
			}

			if (Provider != null) {
				bool enabled = 
				  (bool) Provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
				if (enabled) {
					states.AddState (Atk.StateType.Sensitive);
					states.AddState (Atk.StateType.Enabled);
				} else {
					states.RemoveState (Atk.StateType.Sensitive);
					states.RemoveState (Atk.StateType.Enabled);
				}
				
				if (CanFocus)
					states.AddState (Atk.StateType.Focusable);
				else
					states.RemoveState (Atk.StateType.Focusable);

				if (CanFocus && IsFocused)
					states.AddState (Atk.StateType.Focused);
				else
					states.RemoveState (Atk.StateType.Focused);

				bool is_offscreen = (Parent == null || (bool) Provider.GetPropertyValue (AutomationElementIdentifiers.IsOffscreenProperty.Id));
				if (!is_offscreen) {
					states.AddState (Atk.StateType.Showing);
					states.AddState (Atk.StateType.Visible);
				} else {
					states.RemoveState (Atk.StateType.Showing);
					states.RemoveState (Atk.StateType.Visible);
				}
			}

			return states;
		}

		protected override Atk.RelationSet OnRefRelationSet ()
		{
			Atk.RelationSet relationSet = base.OnRefRelationSet ();

			if (defunct)
				return relationSet;

			if (Role == Atk.Role.ScrollBar 
			    || Role == Atk.Role.RadioButton)
				return relationSet;
			
			Adapter parentAdapter = VirtualParent;
			if (parentAdapter != null) {
				// To support NodeChildOf parent must be either
				// - DataGrid, Table or Group, or
				// - ListItem or DataItem, in this case the parent of parent is used,
				//   because we are ignoring this parent (either ListItem or DataItem).
				int controlType 
					= (int) parentAdapter.Provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

				if (controlType == ControlType.ListItem.Id
				    || controlType == ControlType.DataItem.Id)
					parentAdapter = parentAdapter.VirtualParent;
				else if (controlType != ControlType.DataGrid.Id
				         && controlType != ControlType.Table.Id
				         && controlType != ControlType.Group.Id)
					return relationSet;
				
				if (parentAdapter != null)
					relationSet.AddRelationByType (Atk.RelationType.NodeChildOf, 
					                               parentAdapter);
			}

			return relationSet;
		}

		protected override int OnGetIndexInParent()
		{
			if (Parent == null)
				return -1;
			ParentAdapter parent = Parent as ParentAdapter;
			if (parent != null)
				return parent.GetIndexOfChild (this);
			for (int i = Parent.NAccessibleChildren - 1; i >= 0; i--) {
				if (Parent.RefAccessibleChild(i) == this)
					return i;
			}
			return -1;
		}

		protected bool CanFocus {
			get {
				return Provider != null &&
				       true.Equals (Provider.GetPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id));
			}
		}

		internal bool IsFocused {
			get {
				return Provider != null &&
				       true.Equals (Provider.GetPropertyValue (AutomationElementIdentifiers.HasKeyboardFocusProperty.Id));
			}
		}
#endregion

#region Private Fields
		private bool defunct = false;
#endregion

		internal System.Windows.Rect BoundingRectangle
		{
			get {
				if (Provider == null)
					return System.Windows.Rect.Empty;
				return (System.Windows.Rect) 
					Provider.GetPropertyValue (
					  AutomationElementIdentifiers.BoundingRectangleProperty.Id);
			}
		}

		internal void ConvertCoords (ref int x, ref int y, bool toScreen)
		{
			Adapter adapter = this;
			int mult = (toScreen? 1: -1);
			for (;;) {
				Adapter parent = adapter.Parent as Adapter;
				if (this is ContextMenu)
					parent = VirtualParent;
				if (parent == null || parent is TopLevelRootItem) {
					if (adapter is Window) {
						System.Windows.Rect rect = adapter.BoundingRectangle;
						x += (int)rect.X * mult;
						y += (int)rect.Y * mult;
					}
					return;
				}
				adapter = parent;
			}
		}

		internal Adapter VirtualParent {
			get { return (Provider == null) ? null : AutomationBridge.GetParentAdapter (Provider); }
		}
		
		internal bool VirtualChildren {
			get { return (((IRawElementProviderFragment)Provider).Navigate (NavigateDirection.FirstChild) != null); }
		}
		
		internal IRawElementProviderFragment ParentProvider {
			get { return (Provider == null) ? null : ((IRawElementProviderFragment)Provider).Navigate (NavigateDirection.Parent); }
		}

		private void EmitBoundsChanged (System.Windows.Rect rect)
		{
			Atk.Rectangle atkRect;
			atkRect.X = (int)rect.X;
			atkRect.Y = (int)rect.Y;
			atkRect.Width = (int)rect.Width;
			atkRect.Height = (int)rect.Height;
			GLib.Signal.Emit (this, "bounds_changed", atkRect);
		}
	}
}
