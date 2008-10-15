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
//	Sandy Armstrong <sanfordarmstrong@gmail.com>
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.CheckBox;

namespace Mono.UIAutomation.Winforms.Behaviors.CheckBox
{
	
	internal class ToggleProviderBehavior  
		: ProviderBehavior, IToggleProvider, IEmbeddedImage
	{
		
		#region Constructor
		
		public ToggleProviderBehavior (FragmentControlProvider provider)
			: base (provider)
		{
		}
		
		#endregion
		
		#region IEmbeddedImage Interface
		
		public System.Windows.Rect BoundingRectangle {
			get {
				return Helper.GetBoundingRectangleFromButtonBase (Provider, 
				                                        (SWF.ButtonBase) Provider.Control);
			}
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
	
		public void Toggle ()
		{
			SWF.CheckBox checkbox = (SWF.CheckBox) Provider.Control;

			switch (checkbox.CheckState) {
			case SWF.CheckState.Checked:
				PerformToggle (checkbox, SWF.CheckState.Unchecked);
				break;
			case SWF.CheckState.Unchecked:
				if (checkbox.ThreeState)
					PerformToggle (checkbox, SWF.CheckState.Indeterminate);
				else
					PerformToggle (checkbox, SWF.CheckState.Checked);
				break;
			// Control could still have been set to intermediate
			// programatically, regardless of ThreeState value.
			case SWF.CheckState.Indeterminate:
			default:
				PerformToggle (checkbox, SWF.CheckState.Checked);
				break;
			}
		}

		public ToggleState ToggleState {
			get {
				SWF.CheckBox checkbox = (SWF.CheckBox) Provider.Control;
				
				switch (checkbox.CheckState) {
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

		#endregion
		
		#region Private Methods
		
		private void PerformToggle (SWF.CheckBox checkbox, SWF.CheckState state)
		{
	        if (checkbox.InvokeRequired == true) {
	            checkbox.BeginInvoke (new PerformToggleDelegate (PerformToggle),
				                      new object [] { checkbox, state });
	            return;
	        }
			
			checkbox.CheckState = state;
		}
		#endregion
	}
	
	delegate void PerformToggleDelegate (SWF.CheckBox checkbox, SWF.CheckState state);
}
