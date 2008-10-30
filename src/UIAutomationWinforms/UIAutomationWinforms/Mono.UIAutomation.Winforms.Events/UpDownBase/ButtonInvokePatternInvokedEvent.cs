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
//	Neville Gao <nevillegao@gmail.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Events.UpDownBase
{
	internal class ButtonInvokePatternInvokedEvent : BaseAutomationEvent
	{
		#region Constructor

		public ButtonInvokePatternInvokedEvent (SimpleControlProvider provider) 
			: base (provider, InvokePatternIdentifiers.InvokedEvent)
		{
			this.provider = (UpDownBaseProvider.UpDownBaseButtonProvider) provider;
		}
		
		#endregion
		
		#region IConnectable Overrides

		public override void Connect ()
		{
			if (provider.Orientation == UpDownBaseProvider.UpDownBaseButtonOrientation.Forward) {
				try {
					Helper.AddPrivateEvent (typeof (SWF.UpDownBase),
					                        (SWF.UpDownBase) Provider.Control,
					                        "UIAUpButtonClick",
					                        this,
					                        "OnButtonClick");
				} catch (NotSupportedException) { }
			} else if (provider.Orientation == UpDownBaseProvider.UpDownBaseButtonOrientation.Backward) {
				try {
					Helper.AddPrivateEvent (typeof (SWF.UpDownBase),
					                        (SWF.UpDownBase) Provider.Control,
					                        "UIADownButtonClick",
					                        this,
					                        "OnButtonClick");
				} catch (NotSupportedException) { }
			}
		}
		
		public override void Disconnect ()
		{
			if (provider.Orientation == UpDownBaseProvider.UpDownBaseButtonOrientation.Forward) {
				try {
					Helper.RemovePrivateEvent (typeof (SWF.UpDownBase),
					                           (SWF.UpDownBase) Provider.Control,
					                           "UIAUpButtonClick",
					                           this,
					                           "OnButtonClick");
				} catch (NotSupportedException) { }
			} else if (provider.Orientation == UpDownBaseProvider.UpDownBaseButtonOrientation.Backward) {
				try {
					Helper.RemovePrivateEvent (typeof (SWF.UpDownBase),
					                           (SWF.UpDownBase) Provider.Control,
					                           "UIADownButtonClick",
					                           this,
					                           "OnButtonClick");
				} catch (NotSupportedException) { }
			}
		}
		
		#endregion
		
		#region Private Methods
		
		#pragma warning disable 169
		
		private void OnButtonClick (object sender, EventArgs e)
		{
			RaiseAutomationEvent ();
		}
		
		#pragma warning restore 169
		
		#endregion
		
		#region Private Fields
		
		UpDownBaseProvider.UpDownBaseButtonProvider provider;
		
		#endregion
	}
}
