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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ToolStripButton;

namespace Mono.UIAutomation.Winforms.Behaviors.ToolStripButton
{
	internal class ToggleProviderBehavior  
		: ProviderBehavior, IToggleProvider
	{
#region Constructor
		public ToggleProviderBehavior (FragmentControlProvider provider)
			: base (provider)
		{
			button = (SWF.ToolStripButton) provider.Component;
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
		
		public override AutomationPattern ProviderPattern { 
			get { return TogglePatternIdentifiers.Pattern; }
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == TogglePatternIdentifiers.ToggleStateProperty.Id)
				return ToggleState;
			else
				return null;
		}
#endregion
		
#region IToggleProvider Members
		public ToggleState ToggleState {
			get {
				switch (button.CheckState) {
				case SWF.CheckState.Checked:
					return ToggleState.On;
				case SWF.CheckState.Unchecked:
					return ToggleState.Off;
				case SWF.CheckState.Indeterminate:
				default:
					return ToggleState.Indeterminate;
				}
			}
		}

		public void Toggle ()
		{
			if (!button.Enabled)
				throw new ElementNotEnabledException ();

			switch (button.CheckState) {
			case SWF.CheckState.Checked:
				PerformToggle (button, SWF.CheckState.Unchecked);
				break;
			case SWF.CheckState.Unchecked:
				PerformToggle (button, SWF.CheckState.Checked);
				break;
			case SWF.CheckState.Indeterminate:
			default:
				PerformToggle (button, SWF.CheckState.Checked);
				break;
			}
		}
#endregion
		
#region Private Methods
		private void PerformToggle (SWF.ToolStripButton button, SWF.CheckState state)
		{
			SWF.ToolStrip toolstrip = ((SWF.ToolStripButton) Provider.Component).Owner;
			if (toolstrip.InvokeRequired) {
				toolstrip.BeginInvoke (new PerformToggleDelegate (PerformToggle),
				                       new object [] { button, state });
				return;
			}

			button.CheckState = state;
		}
#endregion

#region Private Members
		private SWF.ToolStripButton button;
#endregion
	}

	delegate void PerformToggleDelegate (SWF.ToolStripButton button, SWF.CheckState state);
}
