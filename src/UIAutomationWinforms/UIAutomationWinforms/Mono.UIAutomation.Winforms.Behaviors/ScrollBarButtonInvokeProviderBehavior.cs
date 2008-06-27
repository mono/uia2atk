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

	public class ScrollBarButtonInvokeProviderBehavior 
		: ButtonInvokeProviderBehavior
	{

#region Constructor
		
		public ScrollBarButtonInvokeProviderBehavior (ScrollBarButtonProvider provider)
			: base (provider)
		{
			this.provider = provider;
		}
		
#endregion
		
#region IProviderBehavior Interface

		public override void Connect (Control control)
		{
			Provider.SetEvent (ProviderEventType.InvokeInvokedEvent, 
			          new ScrollBarButtonInvokePatternInvokeEvent (provider));
		}
		
		public override void Disconnect (Control control)
		{
			Provider.SetEvent (ProviderEventType.InvokeInvokedEvent, null);
		}

#endregion
		
#region IInvokeProvider Members
		
		public override void Invoke ()
		{
			if (!Provider.Control.Enabled)
				throw new ElementNotEnabledException ();
			
			string methodName = string.Empty;
			
			//TODO: Should we use generalization?			
			if (provider.Orientation == ScrollBarButtonOrientation.LargeBack)
				methodName = "LargeDecrement";
			else if (provider.Orientation == ScrollBarButtonOrientation.LargeForward)
				methodName = "LargeIncrement";
			else if (provider.Orientation == ScrollBarButtonOrientation.SmallBack)
				methodName = "SmallDecrement";
			else //Should be ScrollBarButtonOrientation.SmallForward
				methodName = "SmallIncrement";

			Type type = provider.ScrollBarContainer.GetType ();
			MethodInfo methodInfo = type.GetMethod (methodName,
			                                        BindingFlags.InvokeMethod
			                                        | BindingFlags.NonPublic
			                                        | BindingFlags.Instance);
			methodInfo.Invoke (provider.ScrollBarContainer, null);
		}
		
#endregion	
		
#region Private Fields
		
		private ScrollBarButtonProvider provider;

#endregion

	}
}
