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
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.BaseColorControl;

namespace Mono.UIAutomation.Winforms.Behaviors.BaseColorControl
{
	internal class SmallColorControlInvokeProviderBehavior
		: ProviderBehavior, IInvokeProvider
	{
		#region Constructor
		
		public SmallColorControlInvokeProviderBehavior (BaseColorControlProvider.SmallColorControlProvider provider)
			: base (provider)
		{
			this.smallColorControl = (ColorDialog.BaseColorControl.SmallColorControl) Provider.Control;
			this.baseColorControl = (ColorDialog.BaseColorControl) smallColorControl.Parent;
		}
		
		#endregion
		
		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern {
			get { return InvokePatternIdentifiers.Pattern; }
		}

		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   new SmallColorControlInvokePatternInvokedEvent (Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   null);
		}
		
		#endregion
		
		#region IInvokeProvider Members
		
		public virtual void Invoke ()
		{
			if (!Provider.Control.Enabled)
				throw new ElementNotEnabledException ();

			PerformClick ();
		}
		
		#endregion	
		
		#region Private Methods
		
		private void PerformClick ()
		{
			if (Provider.Control.InvokeRequired == true) {
				Provider.Control.BeginInvoke (new MethodInvoker (PerformClick));
				return;
			}

			// TODO: Use internal property when backport
//			ColorDialog.BaseColorControl.SmallColorControl selected =
//				((ColorDialog.BaseColorControl) baseColorControl).UIASelectedSmallColorControl;
			try {
				ColorDialog.BaseColorControl.SmallColorControl selected =
					Helper.GetPrivateProperty<ColorDialog.BaseColorControl, ColorDialog.BaseColorControl.SmallColorControl>
						((ColorDialog.BaseColorControl) baseColorControl, "UIASelectedSmallColorControl");
				if (selected != null)
					selected.IsSelected = false;
			} catch (NotSupportedException) { }
			smallColorControl.IsSelected = true;
		}
		
		#endregion

		#region Private Field

		ColorDialog.BaseColorControl.SmallColorControl smallColorControl;
		ColorDialog.BaseColorControl baseColorControl;

		#endregion
	}
}
