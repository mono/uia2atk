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
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Behaviors.Generic;
using Mono.UIAutomation.Winforms.Events;
//using Mono.UIAutomation.Winforms.Events.ListView;

namespace Mono.UIAutomation.Winforms.Behaviors.ListView
{

	internal class ScrollProviderBehavior : ScrollProviderBehavior<ListViewProvider>
	{
		
		#region Constructors

		public ScrollProviderBehavior (ListViewProvider provider)
			: base (provider)
		{
		}

		#endregion
		
		#region IProviderBehavior Interface

		public override void Connect (System.Windows.Forms.Control control)
		{
//			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontallyScrollableProperty,
//			                   new ScrollPatternHorizontallyScrollableEvent ((ListViewProvider) Provider));
//			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontalScrollPercentProperty,
//			                   new ScrollPatternHorizontalScrollPercentEvent ((ListViewProvider) Provider));
//			Provider.SetEvent (ProviderEventType.ScrollPatternHorizontalViewSizeProperty,
//			                   new ScrollPatternHorizontalViewSizeEvent ((ListViewProvider) Provider));
//			Provider.SetEvent (ProviderEventType.ScrollPatternVerticallyScrollableProperty,
//			                   new ScrollPatternVerticallyScrollableEvent ((ListViewProvider) Provider));
//			Provider.SetEvent (ProviderEventType.ScrollPatternVerticalScrollPercentProperty,
//			                   new ScrollPatternVerticalScrollPercent ((ListViewProvider) Provider));
//			Provider.SetEvent (ProviderEventType.ScrollPatternVerticalViewSizeProperty,
//			                   new ScrollPatternVerticalViewSizeEvent ((ListViewProvider) Provider));
		}
	
		#endregion

	}
					                      
}