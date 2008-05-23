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
		private static string default_invoke_description = "Sends a request to activate a control and initiate its single, unambiguous action.";
		private static string default_toggle_description = "Cycles through the toggle states of a control.";
		private static string default_invoke_name = "click";
		private static string default_toggle_name = "toggle";
		
		private IRawElementProviderSimple provider;
		private IInvokeProvider			invokeProvider;
		private IToggleProvider			toggleProvider;
		private string					actionDescription;
		private string					actionName;
		
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
			
			if(provider is IInvokeProvider) {
				invokeProvider = (IInvokeProvider)provider;
				actionDescription = default_invoke_description;
				actionName = default_invoke_name;
				Role = Atk.Role.PushButton;
			} else {
				toggleProvider = (IToggleProvider)provider;
				actionDescription = default_toggle_description;
				actionName = default_toggle_name;
				Role = Atk.Role.ToggleButton;
			}
			
			string buttonText = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			Name = buttonText;
			
			bool canFocus = (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id);
			if (canFocus)
				RefStateSet ().AddState (Atk.StateType.Selectable);
			else
				RefStateSet ().RemoveState (Atk.StateType.Selectable);

			bool enabled = (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			if (enabled)
				RefStateSet ().AddState (Atk.StateType.Sensitive);
			else
				RefStateSet ().RemoveState (Atk.StateType.Sensitive);
				
			
		}
		
		// Return the number of actions (Read-Only)
		// Both IInvokeProvider and IToggleProvider have only one action
		public int NActions
		{
			get {
				return 1;
			}
		}
		
		// Get a localized name for the specified action
		public string GetLocalizedName (int action)
		{
			if(action != 0)
				return "";

			// TODO: Localize the name?
			return actionName;
		}
		
		// Sets a description of the specified action
		public bool SetDescription(int action, string description)
		{
			if(action != 0)
				return false;
			
			actionDescription = description;
			return true;
		}
		
		// Get the key bindings for the specified action
		public string GetKeybinding(int action)
		{
			string keyBinding = "";
			
			if(action != 0)
				return keyBinding;

			keyBinding = (string) provider.GetPropertyValue (AutomationElementIdentifiers.AcceleratorKeyProperty.Id);
			if(keyBinding == null)
				keyBinding = "";
				
			return keyBinding;
		}

		// Get the name of the specified action		
		public string GetName(int action)
		{
			if(action != 0)
				return "";

			return actionName;
		}
		
		// Get the description of the specified action
		public string GetDescription(int action)
		{
			if(action != 0)
				return "";

			return actionDescription;
		}

		// Perform the action specified
		public bool DoAction(int action)
		{
			try {
				if(invokeProvider != null) {
					try {
						if(action != 0)
							return false;
							
						invokeProvider.Invoke();
						return true;
					} catch (ElementNotEnabledException e) {
						// TODO: handle this exception? maybe returning false is good enough
					}
				} else if(toggleProvider != null) {
					try {
						if(action != 0)
							return false;
						toggleProvider.Toggle();
						return true;
					} catch (ElementNotEnabledException e) {
						// TODO: handle this exception? maybe returning false is good enough
					}
				}
			} catch (Exception e) {
				// TODO: handle a greater exception?
			}
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
