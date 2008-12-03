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
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.Button;

namespace Mono.UIAutomation.Winforms.Behaviors.Button
{
	
	internal class InvokeProviderBehavior 
		: ProviderBehavior, IInvokeProvider, IEmbeddedImage
	{

		#region Private Members

		private SWF.Button button;

		#endregion
		
		#region Constructor
		
		public InvokeProviderBehavior (FragmentControlProvider provider)
			: base (provider)
		{
			button = provider.Control as SWF.Button;
		}
		
		#endregion
		
		#region IEmbeddedImage Interface
		
		public System.Windows.Rect Bounds {
			get {
				return Helper.GetButtonBaseImageBounds (Provider,
				                                        button);
			}
		}

		public string Description {
			get { return string.Empty; }
		}
		
		#endregion
		
		#region IProviderBehavior Interface

		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   new InvokePatternInvokedEvent (Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "button";
			else
				return null;
		}
		
		public override AutomationPattern ProviderPattern { 
			get { return InvokePatternIdentifiers.Pattern; }
		}
		
		#endregion
		
		#region IInvokeProvider Members
		
		public virtual void Invoke ()
		{
			if (Provider.Control.Enabled == false)
				throw new ElementNotEnabledException ();

			PerformClick ();
		}
		
		#endregion	
		
		#region Private Methods
		
		private void PerformClick ()
		{
			if (Provider.Control.InvokeRequired == true) {
				Provider.Control.BeginInvoke (new SWF.MethodInvoker (PerformClick));
				return;
			}
			button.PerformClick ();
		}
		
		#endregion
		
	}

}
