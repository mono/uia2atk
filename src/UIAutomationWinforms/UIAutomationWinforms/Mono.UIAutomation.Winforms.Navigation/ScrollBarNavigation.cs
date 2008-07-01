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
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;

namespace Mono.UIAutomation.Winforms.Navigation
{
		
	//Navigation tree has the following leafs:
	// 1. ButtonSmallBackwards - ScrollBarButtonProvider
	// 2. ButtonLargeBackwards - ScrollBarButtonProvider
	// 3. Thumb	- ThumbProvider
	// 4. ButtonSmallForward - ScrollBarButtonProvider
	// 5. ButtonLargeForward - ScrollBarButtonProvider
	internal class ScrollBarNavigation : SimpleNavigation
	{
		
#region	 Constructor
		
		public ScrollBarNavigation (ScrollBarProvider provider)
			: base (provider)
		{
			chain = new NavigationChain ();
			
			chain.AddLink (new ScrollBarButtonNavigation (provider, 
			                                              ScrollBarButtonOrientation.SmallBack));
			chain.AddLink (new ScrollBarButtonNavigation (provider, 
			                                              ScrollBarButtonOrientation.LargeBack));
			chain.AddLink (new ScrollBarThumbNavigation (provider));
			chain.AddLink (new ScrollBarButtonNavigation (provider, 
			                                              ScrollBarButtonOrientation.LargeForward));
			chain.AddLink (new ScrollBarButtonNavigation (provider, 
			                                              ScrollBarButtonOrientation.SmallForward));
		}
		
#endregion
		
#region INavigable Interface
		
		public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
		{
			if (direction == NavigateDirection.FirstChild) {
				INavigation first = chain.GetFirstLink ();
				return first != null ? (IRawElementProviderFragment) first.Provider : null;
			} else if (direction == NavigateDirection.LastChild) {
				INavigation last = chain.GetLastLink ();
				return last != null ? (IRawElementProviderFragment) last.Provider : null;
			} else
				return base.Navigate (direction);
		}

#endregion
		
#region Private Fields
		
		private NavigationChain chain;
		
#endregion
		
#region Button Navigation Class
		
		class ScrollBarButtonNavigation : SimpleNavigation
		{
			public ScrollBarButtonNavigation (ScrollBarProvider provider,
			                                  ScrollBarButtonOrientation orientation)
				: base (provider)
			{
				this.provider = provider;
				this.orientation = orientation;
			}

			public override IRawElementProviderSimple Provider {
				get { 
					if (button_provider == null) {
						button_provider = new ScrollBarButtonProvider ((ScrollBar) provider.Control,
						                                               orientation);
						button_provider.Navigation = this;
					}

					return button_provider;
				}
			}
			
			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				if (direction == NavigateDirection.Parent)
					return provider;
				else if (direction == NavigateDirection.NextSibling)
					return GetNextSiblingProvider ();
				else if (direction == NavigateDirection.PreviousSibling)
					return GetPreviousSiblingProvider ();
				else
					return null; 
			}
			
			private ScrollBarButtonProvider button_provider; 
			private ScrollBarButtonOrientation orientation;
			private ScrollBarProvider provider;
		}
		
#endregion
		
#region Thumb Navigation Class

		class ScrollBarThumbNavigation : SimpleNavigation
		{
			public ScrollBarThumbNavigation (ScrollBarProvider provider)
				: base (provider)
			{
				this.provider = provider;
			}

			public override IRawElementProviderSimple Provider {
				get { 
					if (thumb_provider == null) {
						thumb_provider = new ThumbProvider ();
						thumb_provider.Navigation = this;
					}

					return thumb_provider;
				}
			}
			
			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				if (direction == NavigateDirection.Parent)
					return provider;
				else if (direction == NavigateDirection.NextSibling)
					return GetNextSiblingProvider ();
				else if (direction == NavigateDirection.PreviousSibling)
					return GetPreviousSiblingProvider ();
				else
					return null; 
			}
			
			private ScrollBarProvider provider;
			private ThumbProvider thumb_provider;
		}
		
#endregion
		
	}
}
