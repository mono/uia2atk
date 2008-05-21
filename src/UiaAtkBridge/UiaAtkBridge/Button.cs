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
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class Button : Adapter, Atk.ActionImplementor
	{
		private IRawElementProviderSimple provider;
		
		// UI Automation Properties supported
		// AutomationElementIdentifiers.AcceleratorKeyProperty.Id
		// AutomationIdProperty() ?
		// AutomationElementIdentifiers.BoundingRectangleProperty.Id
		// AutomationElementIdentifiers.ClickablePointProperty.Id
		// AutomationElementIdentifiers.ControlTypeProperty.Id
		// AutomationElementIdentifiers.HelpTextProperty.Id
		// AutomationElementIdentifiers.IsContentElementProperty.Id
		// AutomationElementIdentifiers.IsControlElementProperty.Id
		// AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id
		// AutomationElementIdentifiers.LabeledByProperty.Id
		// AutomationElementIdentifiers.LocalizedControlTypeProperty.Id
		// AutomationElementIdentifiers.NameProperty.Id
		public Button (IRawElementProviderSimple provider)
		{
			this.provider = provider;
			Role = Atk.Role.PushButton;
			
			
			
			string buttonText = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			Name = buttonText;
			
			bool canFocus = (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id);
			if (canFocus)
				RefStateSet ().AddState (Atk.StateType.Selectable);
			else
				RefStateSet ().RemoveState (Atk.StateType.Selectable);
		}
		
		// Return the number of actions (Read-Only)
		public int NActions
		{
			get {
				return 0;
			}
		}
		
		// Get a localized name for the specified action
		public string GetLocalizedName (int i)
		{
			return "";
		}
		
		// Sets a description of the specified action
		public bool SetDescription(int action, string description)
		{
			return false;
		}
		
		// Get the key bindings for the specified action
		public string GetKeybinding(int action)
		{
			return null;
		}

		// Get the name of the specified action		
		public string GetName(int action)
		{
			return "";
		}
		
		// Get the description of the specified action
		public string GetDescription(int action)
		{
			return "";
		}

		// Perform the action specified
		public bool DoAction(int action)
		{
			return false;
		}

		
		public override IRawElementProviderSimple Provider {
			get { return provider; }
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == InvokePatternIdentifiers.InvokedEvent) {
				OnPressed ();
			} else if (eventId == AutomationElementIdentifiers.AutomationFocusChangedEvent) {
				// TODO: Handle AutomationFocusChangedEvent
			} else if (eventId == AutomationElementIdentifiers.StructureChangedEvent) {
				// TODO: Handle StructureChangedEvent
			}
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
		    if (e.Property == TogglePatternIdentifiers.ToggleStateProperty) {
		        ToggleState state = (ToggleState)e.NewValue;
		        // ToggleState.On
		        // ToggleState.Off
		        // ToggleState.Intermediate
		    } else if(e.Property == AutomationElementIdentifiers.BoundingRectangleProperty) {
		    	// TODO: Handle BoundingRectangleProperty change
		    } else if(e.Property == AutomationElementIdentifiers.IsOffscreenProperty) { 
		        // TODO: Handle IsOffscreenProperty change
		    } else if(e.Property == AutomationElementIdentifiers.IsEnabledProperty) {
		        // TODO: Handle IsEnabledProperty change		    
		    } else if(e.Property == AutomationElementIdentifiers.NameProperty) {
		        // TODO: Handle NameProperty change			    
		    }
		}
		
		private void OnPressed ()
		{
			NotifyStateChange ((ulong) Atk.StateType.Armed, true);
		}
		
		private void OnReleased ()
		{
			NotifyStateChange ((ulong) Atk.StateType.Armed, false);
		}
		
		private void OnDisabled ()
		{
			NotifyStateChange ((ulong) Atk.StateType.Sensitive, false);
		}
		
		private void OnEnabled ()
		{
			NotifyStateChange ((ulong) Atk.StateType.Sensitive, true);
		}
	}
}
