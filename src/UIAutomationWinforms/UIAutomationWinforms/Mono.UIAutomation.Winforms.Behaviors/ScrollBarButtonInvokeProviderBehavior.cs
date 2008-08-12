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
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Reflection;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Behaviors
{

	internal class ScrollBarButtonInvokeProviderBehavior 
		: ButtonInvokeProviderBehavior
	{

		#region Constructor
		
		public ScrollBarButtonInvokeProviderBehavior (ScrollBarProvider.ScrollBarButtonProvider provider)
			: base (provider)
		{
			this.provider = provider;
		}
		
		#endregion
		
		#region IProviderBehavior Interface

		public override void Connect (Control control)
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   new ScrollBarButtonInvokePatternInvokeEvent (provider));
		}
		
		public override void Disconnect (Control control)
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   null);
		}

		#endregion
		
		#region IInvokeProvider Members
		
		public override void Invoke ()
		{
			if (!Provider.Control.Enabled)
				throw new ElementNotEnabledException ();
			
			string methodName = string.Empty;
			
			if (provider.Orientation == ScrollBarProvider.ScrollBarButtonOrientation.LargeBack)
				methodName = "UIALargeDecrement";
			else if (provider.Orientation == ScrollBarProvider.ScrollBarButtonOrientation.LargeForward)
				methodName = "UIALargeIncrement";
			else if (provider.Orientation == ScrollBarProvider.ScrollBarButtonOrientation.SmallBack)
				methodName = "UIASmallDecrement";
			else //Should be ScrollBarButtonOrientation.SmallForward
				methodName = "UIASmallIncrement";
			
			MethodInfo methodInfo = typeof (ScrollBar).GetMethod (methodName,
			                                                      BindingFlags.InvokeMethod
			                                                      | BindingFlags.NonPublic
			                                                      | BindingFlags.Instance);
			methodInfo.Invoke (provider.ScrollBarContainer, null);
		}
		
		#endregion	
		
		#region Private Fields
		
		private ScrollBarProvider.ScrollBarButtonProvider provider;

		#endregion

	}
}
