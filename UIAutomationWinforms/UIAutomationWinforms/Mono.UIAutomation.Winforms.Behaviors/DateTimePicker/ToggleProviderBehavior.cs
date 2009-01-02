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
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using SD = System.Drawing;
using System.Windows.Automation;
using SWF = System.Windows.Forms;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.DateTimePicker;

namespace Mono.UIAutomation.Winforms.Behaviors.DateTimePicker
{
	internal class ToggleProviderBehavior
		: ProviderBehavior, IToggleProvider
	{
#region Constructor
		public ToggleProviderBehavior (FragmentControlProvider provider)
			: base (provider)
		{
			this.dateTimePicker = ((SWF.DateTimePicker) provider.Control);
		}
#endregion
		
#region IProviderBehavior Interface
		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.TogglePatternToggleStateProperty, 
			                   new TogglePatternToggleStateEvent (Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.TogglePatternToggleStateProperty, 
			                   null);
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == TogglePatternIdentifiers.ToggleStateProperty.Id)
				return ToggleState;
			else
				return null;
		}
		
		public override AutomationPattern ProviderPattern { 
			get { return TogglePatternIdentifiers.Pattern; }
		}
#endregion
		
#region IToggleProvider Members
		public ToggleState ToggleState {
			get {
				return dateTimePicker.Checked ? ToggleState.On
			                                      : ToggleState.Off;
			}
		}

		public void Toggle ()
		{
			if (!Provider.Control.Enabled)
				throw new ElementNotEnabledException ();

			PerformToggle ();
		}
#endregion	
		
#region Private Methods
		private void PerformToggle ()
		{
			if (Provider.Control.InvokeRequired) {
				Provider.Control.BeginInvoke (
					new SWF.MethodInvoker (PerformToggle));
				return;
			}

			dateTimePicker.Checked = !dateTimePicker.Checked;
		}
#endregion

#region Private Fields
		private SWF.DateTimePicker dateTimePicker;
#endregion
	}

}
