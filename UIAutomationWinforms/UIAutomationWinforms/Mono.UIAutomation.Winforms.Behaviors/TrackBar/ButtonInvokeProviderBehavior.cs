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
//	Mike Gorse <mgorse@novell.com>
// 

using System;
using Mono.Unix;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Reflection;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.TrackBar;

namespace Mono.UIAutomation.Winforms.Behaviors.TrackBar
{

	internal class ButtonInvokeProviderBehavior 
		: ProviderBehavior, IInvokeProvider
	{

		#region Constructor
		
		public ButtonInvokeProviderBehavior (TrackBarProvider.TrackBarButtonProvider provider)
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

	        if (provider.Control.InvokeRequired == true) {
	            provider.Control.BeginInvoke (new SWF.MethodInvoker (Invoke));
	            return;
	        }

 			if (provider.Orientation == TrackBarProvider.TrackBarButtonOrientation.LargeBack)
				((SWF.TrackBar)Provider.Control).LargeDecrement ();
			else if (provider.Orientation == TrackBarProvider.TrackBarButtonOrientation.LargeForward)
				((SWF.TrackBar)Provider.Control).LargeIncrement ();
		}
		
		#endregion	
		
		#region Private Methods
		#endregion
		
		#region Private Fields
		
		private TrackBarProvider.TrackBarButtonProvider provider;

		#endregion

	}
}
