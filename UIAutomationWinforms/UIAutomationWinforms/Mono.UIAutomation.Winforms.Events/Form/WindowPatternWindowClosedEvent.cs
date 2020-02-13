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
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Events.Form
{

	internal class WindowPatternWindowClosedEvent : BaseAutomationEvent
	{
		#region Constructors
		
		public WindowPatternWindowClosedEvent (SimpleControlProvider provider)
			: base (provider, WindowPatternIdentifiers.WindowClosedEvent)
		{
			formProvider = (FormProvider) provider;
		}
		
		#endregion
		
		#region IConnectable Overrides

		public override void Connect ()
		{
			// Dialog Windows block user input so in order to continue you have to 
			// close them, Regular Windows don't block but you can "close them"
			// by calling Dispose, so we are listening both events but raising
			// event only once.
			((SWF.Form) Provider.Control).Closed += OnClosed;
			((SWF.Form) Provider.Control).Disposed += OnClosed;
		}

		public override void Disconnect ()
		{
			((SWF.Form) Provider.Control).Closed -= OnClosed;
			((SWF.Form) Provider.Control).Disposed -= OnClosed;
		}
		
		#endregion
		
		#region Private Methods

		private void OnClosed (object sender, EventArgs args)
		{
			if (!formProvider.AlreadyClosed) {
				RaiseAutomationEvent ();
				formProvider.Close ();
			}
		}
		
		#endregion

		#region Private Fields

		private FormProvider formProvider;

		#endregion
	}
}
