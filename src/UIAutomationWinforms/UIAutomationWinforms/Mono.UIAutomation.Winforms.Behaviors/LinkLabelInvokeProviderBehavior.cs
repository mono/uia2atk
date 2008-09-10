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
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Behaviors
{

	internal class LinkLabelInvokeProviderBehavior : ProviderBehavior, IInvokeProvider
	{
		
		#region Constructor
		
		public LinkLabelInvokeProviderBehavior (FragmentControlProvider provider)
			: base (provider)
		{
		}
		
		#endregion

		#region IProviderBehavior Interface
		
		public override void Disconnect (Control control)
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   null);
		}

		public override void Connect (Control control)
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   new LinkLabelInvokePatternInvokedEvent (Provider));
		}
		
		public override AutomationPattern ProviderPattern { 
			get { return InvokePatternIdentifiers.Pattern; }
		}
		
		#endregion
		
		#region IInvokeProvider Members
		
		public void Invoke ()
		{
			if (Provider.Control.Enabled == false)
				throw new ElementNotEnabledException ();
			
			MethodInfo methodInfo = typeof (LinkLabel).GetMethod ("OnLinkClicked",
			                                                      BindingFlags.InvokeMethod
			                                                      | BindingFlags.NonPublic
			                                                      | BindingFlags.Instance);
			invokeMethod
				= (Action<LinkLabel,LinkLabelLinkClickedEventArgs>) Delegate.CreateDelegate 
					(typeof (Action<LinkLabel,LinkLabelLinkClickedEventArgs>),
					 methodInfo);
			
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
			
			LinkLabelLinkClickedEventArgs args 
				= new LinkLabelLinkClickedEventArgs (((LinkLabel) Provider.Control).Links [0],
				                                     MouseButtons.Left);
			invokeMethod ((LinkLabel) Provider.Control, args);
			invokeMethod = null;
		}
		
		#endregion
		
		#region Private Fields
		
		private Action<LinkLabel, LinkLabelLinkClickedEventArgs> invokeMethod;
		
		#endregion

	}
}
