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
	internal class ScrollBarNavigation : ParentNavigation
	{
		
#region	Constructors
		
		public ScrollBarNavigation (ScrollBarProvider provider)
			: base (provider)
		{	
			Chain.AddLast (new ScrollBarButtonNavigation (provider, 
			                                              Chain,
			                                              ScrollBarProvider.ScrollBarButtonOrientation.SmallBack));
			Chain.AddLast (new ScrollBarButtonNavigation (provider, 
			                                              Chain,
			                                              ScrollBarProvider.ScrollBarButtonOrientation.LargeBack));
			Chain.AddLast (new ScrollBarThumbNavigation (provider,
			                                             Chain));
			Chain.AddLast (new ScrollBarButtonNavigation (provider, 
			                                              Chain,
			                                              ScrollBarProvider.ScrollBarButtonOrientation.LargeForward));
			Chain.AddLast (new ScrollBarButtonNavigation (provider, 
			                                              Chain,
			                                              ScrollBarProvider.ScrollBarButtonOrientation.SmallForward));
		}
		
#endregion

#region Internal Class: Button Navigation
		
		class ScrollBarButtonNavigation : ChildNavigation
		{
			public ScrollBarButtonNavigation (ScrollBarProvider provider,
			                                  NavigationChain chain,
			                                  ScrollBarProvider.ScrollBarButtonOrientation orientation)
				: base (provider, chain)
			{
				this.orientation = orientation;
			}

			protected override IRawElementProviderSimple GetChildProvider ()
			{
				if (button_provider == null) {
					button_provider = ((ScrollBarProvider) ParentProvider).GetChildButtonProvider (orientation);
					button_provider.Navigation = this;
				}

				return button_provider;
			}
			
			private FragmentControlProvider button_provider; 
			private ScrollBarProvider.ScrollBarButtonOrientation orientation;
		}
		
#endregion
		
#region Internal Class: Thumb Navigation

		class ScrollBarThumbNavigation : ChildNavigation
		{
			public ScrollBarThumbNavigation (ScrollBarProvider provider,
			                                 NavigationChain chain)
				: base (provider, chain)
			{
			}
			
			protected override IRawElementProviderSimple GetChildProvider ()
			{
				if (thumb_provider == null) {
					thumb_provider = ((ScrollBarProvider) ParentProvider).GetChildThumbProvider ();
					thumb_provider.Navigation = this;
				}
				
				return thumb_provider;
			}

			private FragmentControlProvider thumb_provider;
		}
		
#endregion
		
	}
}
