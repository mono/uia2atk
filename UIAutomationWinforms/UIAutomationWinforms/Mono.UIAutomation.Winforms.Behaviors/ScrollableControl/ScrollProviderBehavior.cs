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
// 	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows.Forms;
using System.Windows.Automation;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors.Generic;
using Mono.UIAutomation.Winforms.Events.ScrollableControl;

namespace Mono.UIAutomation.Winforms.Behaviors.ScrollableControl
{
	internal class ScrollProviderBehavior
		: ScrollProviderBehavior<ScrollableControlProvider>
	{
#region Public Methods
		public ScrollProviderBehavior (ScrollableControlProvider provider)
			: base (provider)
		{
		}
#endregion

#region IProviderBehavior Implementation
		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontalScrollPercentProperty,
			                   new ScrollPatternHorizontalScrollPercentEvent ((ScrollableControlProvider) Provider));
			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontalViewSizeProperty,
			                   new ScrollPatternHorizontalViewSizeEvent ((ScrollableControlProvider) Provider));
			Provider.SetEvent (ProviderEventType.ScrollPatternVerticalScrollPercentProperty,
			                   new ScrollPatternVerticalScrollPercentEvent ((ScrollableControlProvider) Provider));
			Provider.SetEvent (ProviderEventType.ScrollPatternVerticalViewSizeProperty,
			                   new ScrollPatternVerticalViewSizeEvent ((ScrollableControlProvider) Provider));
			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontallyScrollableProperty,
			                   new ScrollPatternHorizontallyScrollableEvent ((ScrollableControlProvider) Provider));
			Provider.SetEvent (ProviderEventType.ScrollPatternVerticallyScrollableProperty,
			                   new ScrollPatternVerticallyScrollableEvent ((ScrollableControlProvider) Provider));
		}
#endregion
	}
}
