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
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows.Automation;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using System.Windows.Automation.Provider;
//using Mono.UIAutomation.Winforms.Events.MonthCalendar;

namespace Mono.UIAutomation.Winforms.Behaviors.MonthCalendar
{
	internal class ButtonInvokeProviderBehavior 
		: ProviderBehavior, IInvokeProvider
	{
#region Constructor
		
		public ButtonInvokeProviderBehavior (MonthCalendarButtonProvider provider)
			: base (provider)
		{
			this.provider = provider;
		}
	
#endregion
		
#region IProviderBehavior Interface

		public override void Connect ()
		{
			// XXX: Not sure if this is even needed, as we'll be
			// notified when the selection changes
			// Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			//                   new ButtonInvokePatternInvokedEvent (provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   null);
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

			SWF.MonthCalendar calendar
				= (SWF.MonthCalendar) provider.DataGridProvider.Control;

			int offset = 1;
			if (provider.Direction
				== MonthCalendarButtonProvider.ButtonDirection.Back) {
				offset = -1;
			}

			calendar.CurrentMonth = calendar.CurrentMonth.AddMonths (offset);
		}
		
#endregion	
		
#region Private Fields
		
		private MonthCalendarButtonProvider provider;

#endregion
	}
}
