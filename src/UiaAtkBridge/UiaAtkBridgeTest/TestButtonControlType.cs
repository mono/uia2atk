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
//      Calvin Gaisford <calvinrg@gmail.com>
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridgeTest
{
	public class TestButtonControlType : TestProviderSimple, IInvokeProvider, IToggleProvider
	{
		private		ToggleState		toggleState;
		private		bool			isToggle;
		
		public TestButtonControlType(string name, bool isToggle) : base()
		{
			if(properties == null)
				throw new Exception("Dude!");
			// set values for all of the required properties
			properties[AutomationElementIdentifiers.AcceleratorKeyProperty.Id] = "Ctrl+T";
			//properties[AutomationElementIdentifiers.BoundingRectangleProperty.Id] = true;
			//properties[AutomationElementIdentifiers.ClickablePointProperty.Id] = true;
			properties[AutomationElementIdentifiers.ControlTypeProperty.Id] = ControlType.Button.Id;
			properties[AutomationElementIdentifiers.HelpTextProperty.Id] = "Need a little help from my friends";
			properties[AutomationElementIdentifiers.IsContentElementProperty.Id] = true;
			properties[AutomationElementIdentifiers.IsControlElementProperty.Id] = true;
			properties[AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id] = true;
			properties[AutomationElementIdentifiers.IsEnabledProperty.Id] = true;
			//properties[AutomationElementIdentifiers.LabeledByProperty.Id] = true;
			//properties[AutomationElementIdentifiers.LocalizedControlTypeProperty.Id] = true;
			properties[AutomationElementIdentifiers.NameProperty.Id] = name;

			this.isToggle = isToggle;
			
			toggleState = ToggleState.Off;
		}
		
#region IInvokeProvider Members
		
		public void Invoke ()
		{
			// poke!  this test button does nothing when invoked!
		}

#endregion

#region IToggleProvider Members
		
		public ToggleState ToggleState
		{ 
			get {
				return toggleState;
			}
		}
		
		public void Toggle ()
		{
			if(toggleState == ToggleState.Off)
				toggleState = ToggleState.On;
			else if(toggleState == ToggleState.On)
				toggleState = ToggleState.Indeterminate;
			else
				toggleState = ToggleState.Off;
		}

#endregion

#region IRawElementProviderSimple Members
	
		public override object GetPatternProvider (int patternId)
		{
			if ( (!isToggle) && (patternId == InvokePatternIdentifiers.Pattern.Id) )
				return this;
			
			if ( (isToggle) && (patternId == TogglePatternIdentifiers.Pattern.Id) )
				return this;
			
			return null;
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			return base.GetPropertyValue (propertyId);
		}

#endregion
		
		
	}
}
