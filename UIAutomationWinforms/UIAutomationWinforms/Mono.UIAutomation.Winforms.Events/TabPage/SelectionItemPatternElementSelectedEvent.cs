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

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Events.TabPage
{
	internal class SelectionItemPatternElementSelectedEvent
		: BaseAutomationEvent
	{
#region Constructors

		public SelectionItemPatternElementSelectedEvent (TabPageProvider provider)
			: base (provider, 
			        SelectionItemPatternIdentifiers.ElementSelectedEvent)
		{
		}
		
#endregion
		
#region ProviderEvent Methods

		public override void Connect ()
		{
			if (Provider.Control.Parent == null) {
				// This is kind of painting over a potentially bad
				// situation, but I'm not sure what the correct
				// behavior should be.  I have a feeling that
				// this will never be called in a normal
				// situation (as without a parent, the Provider
				// would never be created), but it appears in
				// our unit tests, so it can't hurt to not
				// crash horribly.
				return;
			}

			((SWF.TabControl) Provider.Control.Parent)
				.SelectedIndexChanged += OnElementSelectedEvent;
		}

		public override void Disconnect ()
		{
			if (Provider.Control.Parent == null) {
				return;
			}

			((SWF.TabControl) Provider.Control.Parent)
				.SelectedIndexChanged -= OnElementSelectedEvent;
		}
		
#endregion 

#region Protected methods
		
		private void OnElementSelectedEvent (object sender, EventArgs args)
		{
			if (((TabPageProvider) Provider).IsSelected) {
				RaiseAutomationEvent ();
			}
		}

#endregion
	}
}
