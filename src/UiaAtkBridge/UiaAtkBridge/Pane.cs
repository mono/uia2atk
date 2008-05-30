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
//      Calvin Gaisford <cgaisford@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class Pane : Adapter
	{
		private IRawElementProviderSimple provider;
		private ITransformProvider		transformProvider;
		private IDockProvider			dockProvider;
		
		// UI Automation Properties supported
		// AutomationIdProperty() ?
		// AutomationElementIdentifiers.BoundingRectangleProperty.Id
		// AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id
		// AutomationElementIdentifiers.NameProperty.Id
		// AutomationElementIdentifiers.ClickablePointProperty.Id
		// AutomationElementIdentifiers.LabeledByProperty.Id
		// AutomationElementIdentifiers.ControlTypeProperty.Id
		// AutomationElementIdentifiers.LocalizedControlTypeProperty.Id
		// AutomationElementIdentifiers.IsContentElementProperty.Id
		// AutomationElementIdentifiers.IsControlElementProperty.Id
		// AutomationElementIdentifiers.HelpTextProperty.Id
		// AutomationElementIdentifiers.HelpTextProperty.Id
		// AutomationElementIdentifiers.AccessKeyProperty.Id
		
		public Pane (IRawElementProviderSimple provider)
		{
			this.provider = provider;

			Role = Atk.Role.Panel;
			
			// The Pane doesn't have to have either of these
			if(provider is ITransformProvider) {
				transformProvider = (ITransformProvider)provider;
			} else if(provider is IDockProvider)  {
				dockProvider = (IDockProvider)provider;
			}
			
			string nameText = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			Name = nameText;
		}
		
		public override IRawElementProviderSimple Provider {
			get { return provider; }
		}

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == AutomationElementIdentifiers.AutomationFocusChangedEvent) {
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
		
	}
}