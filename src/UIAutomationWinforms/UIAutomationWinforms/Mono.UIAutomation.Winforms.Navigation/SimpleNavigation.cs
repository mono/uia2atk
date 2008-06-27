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
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms.Navigation
{

	internal class SimpleNavigation : INavigation
	{

#region Constructor
		
		public SimpleNavigation (IRawElementProviderSimple provider)
		{
			this.simple_provider = (SimpleControlProvider) provider;
		}

#endregion
		
#region INavigable Interface
		
		public virtual IRawElementProviderSimple Provider {
			get { return simple_provider; }
		}
		
		public virtual bool SupportsNavigation  {
			get { return true; }
		}
		
		public INavigation NextSibling { 
			get { return next_sibling; }
			set { next_sibling = value; }
		}
		
		public INavigation NextNavigableSibling { 
			get { 
				INavigation next = NextSibling;
				while (next != null) {
					if (next.SupportsNavigation)
						break;
					next = next.NextNavigableSibling;
				}
				return next; 
			}
		}

		public INavigation PreviousSibling { 
			get { return previous_sibling; } 
			set { previous_sibling = value; }
		}

		public INavigation PreviousNavigableSibling { 
			get { 
				INavigation previous = PreviousSibling;
				while (previous != null) {
					if (previous.SupportsNavigation)
						break;
					previous = previous.PreviousNavigableSibling;
				}
				return previous;
			}
		}

		public virtual IRawElementProviderFragment Navigate (NavigateDirection direction) 
		{
			if (direction == NavigateDirection.Parent) {
				if (simple_provider.Control.Parent == null)
					return null;
				else
					return ProviderFactory.FindProvider (simple_provider.Control.Parent);
			} else if (direction == NavigateDirection.NextSibling) {
				if (simple_provider.Control.Parent == null)
					return null;
				
				int next = simple_provider.Control.Parent.Controls.IndexOf (simple_provider.Control) + 1;
				if (next >= simple_provider.Control.Parent.Controls.Count)
					return null;
				else
					return ProviderFactory.FindProvider (simple_provider.Control.Parent.Controls [next]);
			} else if (direction == NavigateDirection.PreviousSibling) {
				if (simple_provider.Control.Parent == null)
					return null;

				int previous = simple_provider.Control.Parent.Controls.IndexOf (simple_provider.Control) - 1;
				if (previous < 0)
					return null;
				else
					return ProviderFactory.FindProvider (simple_provider.Control.Parent.Controls [previous]);
			} else if (direction == NavigateDirection.FirstChild) {
				if (simple_provider.Control.Controls.Count == 0)
					return null;
				else
					return ProviderFactory.FindProvider (simple_provider.Control.Controls [0]);
			} else if (direction == NavigateDirection.LastChild) {
				if (simple_provider.Control.Controls.Count == 0)
					return null;
				else
					return ProviderFactory.FindProvider (simple_provider.Control.Controls [simple_provider.Control.Controls.Count - 1]);
			} else
				return null;
		}

#endregion
		
#region Protected Methods
		
		protected IRawElementProviderFragment GetNextSiblingProvider ()
		{
			INavigation next = NextNavigableSibling;
			return next != null ? (IRawElementProviderFragment) next.Provider : null;
		}
		
		protected IRawElementProviderFragment GetPreviousSiblingProvider ()
		{
			INavigation previous = PreviousNavigableSibling;
			return previous != null ? (IRawElementProviderFragment) previous.Provider : null;
		}
		
#endregion
		
#region Private Fields

		private SimpleControlProvider simple_provider;
		private INavigation next_sibling;
		private INavigation previous_sibling;
		
#endregion
	}
}
