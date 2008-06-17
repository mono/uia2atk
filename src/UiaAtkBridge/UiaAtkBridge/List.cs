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
//      Andrés G. Aragoneses <aaragoneses@novell.com>
//      Calvin Gaisford <cgaisford@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class List : ComponentAdapter, Atk.SelectionImplementor
	{
		private IRawElementProviderSimple	provider;
		private ISelectionProvider			selectionProvider;
		
/*
AtkObject,
?AtkAction,
?AtkSelection,
?AtkRelation (to associate a text label with the control),
?AtkRelationSet,
?AtkStateSet
*/


#region UI Automation Properties supported

		// AutomationElementIdentifiers.AutomationIdProperty.Id
		// AutomationElementIdentifiers.BoundingRectangleProperty.Id
		// AutomationElementIdentifiers.ClickablePointProperty.Id
		// AutomationElementIdentifiers.NameProperty.Id
		// AutomationElementIdentifiers.LabeledByProperty.Id
		// AutomationElementIdentifiers.ControlTypeProperty.Id
		// AutomationElementIdentifiers.LocalizedControlTypeProperty.Id
		// AutomationElementIdentifiers.IsContentElementProperty.Id
		// AutomationElementIdentifiers.IsControlElementProperty.Id
		// AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id
		// AutomationElementIdentifiers.HelpTextProperty.Id

#endregion



		public List (IRawElementProviderSimple provider)
		{
			this.provider = provider;
			
			//FIXME: use provider.GetPatternProvider ()
			selectionProvider = (ISelectionProvider)provider.GetPatternProvider(SelectionPatternIdentifiers.Pattern.Id);
			if(selectionProvider != null) {
				//it seems the default description should be null:
				//actionDescription = default_invoke_description;
				// actionName = default_invoke_name;
				Role = Atk.Role.List;
			} 
			
			string componentName = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			Name = componentName;
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			bool canFocus = (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id);
			if (canFocus)
				states.AddState (Atk.StateType.Selectable);
			else
				states.RemoveState (Atk.StateType.Selectable);
			
			bool enabled = (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			if (enabled)
			{
				states.AddState (Atk.StateType.Sensitive);
				states.AddState (Atk.StateType.Enabled);
			}
			else
			{
				states.RemoveState (Atk.StateType.Sensitive);
				states.RemoveState (Atk.StateType.Enabled);
			}			
			return states;
		}

		
		public override IRawElementProviderSimple Provider {
			get { return provider; }
		}

		public GLib.SList DefaultAttributes {
			get {
				throw new NotImplementedException();
			}
		}


#region Atk.SelectionImplementor

		public int SelectionCount
		{
			get {
				return selectionProvider.GetSelection().GetLength(0);
			}
		}

		public bool AddSelection (int i)
		{
			//TODO: Implement
			return false;
		}

		public bool ClearSelection ()
		{
			//TODO: Implement
			return false;
		}

		public bool IsChildSelected (int i)
		{
			//TODO: Implement
			return false;
		}
		
		public Atk.Object RefSelection (int i)		
		{
			//TODO: Implement
			return null;
		}
		
		public bool RemoveSelection (int i)
		{
			//TODO: Implement
			return false;
		}

		public bool SelectAllSelection ()
		{
			//TODO: Implement
			return false;
		}

#endregion


		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == AutomationElementIdentifiers.AsyncContentLoadedEvent) {
				// TODO: Handle AsyncContentLoadedEvent
			} else if (eventId == AutomationElementIdentifiers.AutomationFocusChangedEvent) {
				// TODO: Handle AutomationFocusChangedEvent
			} else if (eventId == AutomationElementIdentifiers.StructureChangedEvent) {
				// TODO: Handle StructureChangedEvent
			}
		}


		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == TogglePatternIdentifiers.ToggleStateProperty) {
				//if it's a toggle, it should not be a basic Button class, but CheckBox or other
				throw new NotSupportedException ("Toggle events should not land here (should not be reached)");
			} else if (e.Property == AutomationElementIdentifiers.BoundingRectangleProperty) {
				// TODO: Handle BoundingRectangleProperty change
			} else if (e.Property == AutomationElementIdentifiers.IsOffscreenProperty) { 
				//if((bool)e.NewValue)
					//TODO: call to NotifyStateChange instead of using RefStateSet (the former will cause the call to OnRefState)
					//RefStateSet ().AddState (Atk.StateType.Visible);
				//else
					//TODO: call to NotifyStateChange instead of using RefStateSet (the former will cause the call to OnRefState)
					//RefStateSet ().RemoveState (Atk.StateType.Visible);
			} else if (e.Property == AutomationElementIdentifiers.IsEnabledProperty) {
				if((bool)e.NewValue)
				{
					//OnEnabled ();
				}
				else
				{
					//OnDisabled ();
				}
			} else if (e.Property == AutomationElementIdentifiers.NameProperty) {
				Name = (string)e.NewValue;
			}
		}
		
	}
}
