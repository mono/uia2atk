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
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Reflection;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ScrollBar;

namespace Mono.UIAutomation.Winforms.Behaviors.ScrollBar
{

	internal class ButtonInvokeProviderBehavior 
		: ProviderBehavior, IInvokeProvider
	{

		#region Constructor
		
		public ButtonInvokeProviderBehavior (ScrollBarProvider.ScrollBarButtonProvider provider)
			: base (provider)
		{
			this.provider = provider;
		}
		
		#endregion
		
		#region IProviderBehavior Interface

		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   new ButtonInvokePatternInvokedEvent (provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   null);
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.AcceleratorKeyProperty.Id)
				return null; // TODO
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "button";
			else
				return null;
		}
		
		public override AutomationPattern ProviderPattern { 
			get { return InvokePatternIdentifiers.Pattern; }
		}

		#endregion
		
		#region IInvokeProvider Members
		
		public void Invoke ()
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
			
			MethodInfo methodInfo = typeof (SWF.ScrollBar).GetMethod (methodName,
			                                                          BindingFlags.InvokeMethod
			                                                          | BindingFlags.NonPublic
			                                                          | BindingFlags.Instance);
			if (methodInfo != null) {
				invokeMethod
					= (Action<SWF.ScrollBar>) Delegate.CreateDelegate (typeof (Action<SWF.ScrollBar>), 
					                                                   methodInfo);
				PerformScrollBarButtonClick ();
			}
		}
		
		#endregion	
		
		#region Private Methods
		
		private void PerformScrollBarButtonClick ()
		{
	        if (provider.ScrollBarContainer.InvokeRequired == true) {
	            provider.ScrollBarContainer.BeginInvoke (new SWF.MethodInvoker (PerformScrollBarButtonClick));
	            return;
	        }
			
			invokeMethod (provider.ScrollBarContainer);
			invokeMethod = null;
		}
		
		#endregion
		
		#region Private Fields
		
		private Action<SWF.ScrollBar> invokeMethod;
		private ScrollBarProvider.ScrollBarButtonProvider provider;

		#endregion

	}
}
