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
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{

	public class ToggleStatePropertyEventStrategy : EventStrategy
	{
		public ToggleStatePropertyEventStrategy (IRawElementProviderSimple provider, 
		                                         IToggleProvider toggleProvider,
		                                         CheckBox control) :
			base (provider, control)
		{
			this.toggleProvider = toggleProvider;
			checkbox = control;
		}
	
		public override void Connect ()
		{
			checkbox.CheckedChanged += new EventHandler (OnCheckChanged);
		}

		public override void Disconnect ()
		{
			checkbox.CheckedChanged -= new EventHandler (OnCheckChanged);
		}
		
		private void OnCheckChanged (object sender, EventArgs e)
		{
			if (AutomationInteropProvider.ClientsAreListening) {
				AutomationPropertyChangedEventArgs args =
					new AutomationPropertyChangedEventArgs (TogglePatternIdentifiers.ToggleStateProperty,
					                                        null, // Mimics MS behavior
					                                        toggleProvider.ToggleState);
				AutomationInteropProvider.RaiseAutomationPropertyChangedEvent (Provider, args);
			}
		}
		
		private IToggleProvider toggleProvider;
		private CheckBox checkbox;
	}
}
