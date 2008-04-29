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
// 

using System;
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{
	public class ButtonProvider : IInvokeProvider, IRawElementProviderSimple
	{
#region Private Members
		
		private Button button;
		
#endregion
		
#region Constructors
		
		public ButtonProvider (Button button)
		{
			this.button = button;
			button.Click += OnClick;
		}
		
#endregion
		
#region IInvokeProvider Members
		
		public void Invoke ()
		{
			if (!button.Enabled)
				throw new ElementNotEnabledException ();
			
			// TODO: Make sure this runs on the right thread
			button.PerformClick ();
		}
		
#endregion
		
#region IRawElementProviderSimple Members
	
		public object GetPatternProvider (int patternId)
		{
			if (patternId == InvokePatternIdentifiers.Pattern.Id)
				return this;
			
			return null;
		}
		
		public object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
				return button.Enabled;
			else if (propertyId == AutomationElementIdentifiers.ClassNameProperty.Id)
				return "WindowsForms10.BUTTON.app.0.bf7d44"; // TODO: Verify
			else if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Button.Id;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return button.Text;
			else
				return null;
		}


		public IRawElementProviderSimple HostRawElementProvider {
			get {
				return AutomationInteropProvider.HostProviderFromHandle (button.TopLevelControl.Handle);
			}
		}
		
		public ProviderOptions ProviderOptions {
			get {
				return ProviderOptions.ServerSideProvider;
			}
		}

#endregion
		
#region Event Handlers
		
		private void OnClick (object sender, EventArgs e)
		{
			if (AutomationInteropProvider.ClientsAreListening) {
				AutomationEventArgs args = new AutomationEventArgs (InvokePatternIdentifiers.InvokedEvent);
				AutomationInteropProvider.RaiseAutomationEvent (InvokePatternIdentifiers.InvokedEvent, this, args);
			}
		}
		
#endregion
	}
}