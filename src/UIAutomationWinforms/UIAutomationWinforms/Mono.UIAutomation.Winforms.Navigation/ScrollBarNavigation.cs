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
			
			chain.AddLast (new ScrollBarButtonNavigation (chain, 
			                                              provider, 
			                                              ScrollBarButtonOrientation.SmallBack));
			chain.AddLast (new ScrollBarButtonNavigation (chain,
			                                              provider, 
			                                              ScrollBarButtonOrientation.LargeBack));
			chain.AddLast (new ScrollBarThumbNavigation (chain,
			                                             provider));
			chain.AddLast (new ScrollBarButtonNavigation (chain,
			                                              provider, 
			                                              ScrollBarButtonOrientation.LargeForward));
			chain.AddLast (new ScrollBarButtonNavigation (chain,
			                                              provider, 
			                                              ScrollBarButtonOrientation.SmallForward));
		}
		
#endregion
		
#region INavigable Interface
		
		public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
		{
			if (direction == NavigateDirection.FirstChild)
				return (IRawElementProviderFragment) chain.First.Value.Provider;
			else if (direction == NavigateDirection.LastChild)
				return (IRawElementProviderFragment) chain.Last.Value.Provider;
			else
				return base.Navigate (direction);
		}

#endregion
		
#region Private Fields
		
		private NavigationChain chain;
		
#endregion
		
#region Button Navigation Class
		
		class ScrollBarButtonNavigation : SimpleNavigation
		{
			public ScrollBarButtonNavigation (NavigationChain chain,
			                                  ScrollBarProvider provider,
			                                  ScrollBarButtonOrientation orientation)
				: base (provider)
			{
				this.chain = chain;
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
					return GetNextSiblingProvider (chain);
				else if (direction == NavigateDirection.PreviousSibling)
					return GetPreviousSiblingProvider (chain);
				else
					return null; 
			}
			
			private NavigationChain chain;
			private ScrollBarButtonProvider button_provider; 
			private ScrollBarButtonOrientation orientation;
			private ScrollBarProvider provider;
		}
		
#endregion
		
#region Thumb Navigation Class

		class ScrollBarThumbNavigation : SimpleNavigation
		{
			public ScrollBarThumbNavigation (NavigationChain chain,
			                                 ScrollBarProvider provider)
				: base (provider)
			{
				this.chain = chain;
				this.provider = provider;
			}

			public override IRawElementProviderSimple Provider {
				get { 
					if (thumb_provider == null) {
						thumb_provider = new ThumbProvider ((ScrollBar) provider.Control);
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
					return GetNextSiblingProvider (chain);
				else if (direction == NavigateDirection.PreviousSibling)
					return GetPreviousSiblingProvider (chain);
				else
					return null; 
			}
			
			private NavigationChain chain;
			private ScrollBarProvider provider;
			private ThumbProvider thumb_provider;
		}
		
#endregion
		
	}
}
