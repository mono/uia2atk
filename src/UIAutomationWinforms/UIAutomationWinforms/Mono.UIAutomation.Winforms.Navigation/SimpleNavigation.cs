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
using System.Collections.Generic;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms.Navigation
{

	internal class SimpleNavigation : INavigation
	{

		#region Constructor
		
		public SimpleNavigation (IRawElementProviderSimple provider)
		{
			this.simpleProvider = (SimpleControlProvider) provider;
		}

		#endregion
		
		#region INavigation Interface
		
		public virtual IRawElementProviderSimple Provider {
			get { return simpleProvider; }
		}

		public virtual IRawElementProviderFragment GetNextSiblingProvider (NavigationChain chain)
		{
			if (chain.Contains (this) == true) {
				LinkedListNode<INavigation> nextNode = chain.Find (this).Next;
				if (nextNode == null)
					return null;
				else
					return (IRawElementProviderFragment) nextNode.Value.Provider;
			} else
				return null;
		}
		
		public virtual IRawElementProviderFragment GetPreviousSiblingProvider (NavigationChain chain)
		{
			if (chain.Contains (this) == true) {
				LinkedListNode<INavigation> previousNode = chain.Find (this).Previous;
				if (previousNode == null)
					return null;
				else
					return (IRawElementProviderFragment) previousNode.Value.Provider;
			} else
				return null;
		}

		public virtual void Terminate ()
		{
			if (simpleProvider != null)
				simpleProvider.Terminate ();
		}		

		public virtual IRawElementProviderFragment Navigate (NavigateDirection direction) 
		{
			Control containerControl;
			
			//TODO: Update ignore ErrorProvider's Custom Controls
			
			if (direction == NavigateDirection.Parent) {
				if (simpleProvider.Container == null)
					return null;
				else
					return ProviderFactory.GetProvider (simpleProvider.Container);
			} else if (direction == NavigateDirection.NextSibling) {				
				if ((containerControl = simpleProvider.Container as Control) == null)
					return null;
				
				int next = containerControl.Controls.IndexOf (simpleProvider.Control) + 1;
				if (next >= containerControl.Controls.Count)
					return null;
				else
					return ProviderFactory.GetProvider (containerControl.Controls [next]);
			} else if (direction == NavigateDirection.PreviousSibling) {
				if ((containerControl = simpleProvider.Container as Control) == null)
					return null;

				int previous = containerControl.Controls.IndexOf (simpleProvider.Control) - 1;
				if (previous < 0)
					return null;
				else
					return ProviderFactory.GetProvider (containerControl.Controls [previous]);
			} else if (direction == NavigateDirection.FirstChild) {
				if (simpleProvider.Control == null || simpleProvider.Control.Controls.Count == 0)
					return null;
				else
					return ProviderFactory.GetProvider (simpleProvider.Control.Controls [0]);
			} else if (direction == NavigateDirection.LastChild) {
				if (simpleProvider.Control == null || simpleProvider.Control.Controls.Count == 0)
					return null;
				else
					return ProviderFactory.GetProvider (simpleProvider.Control.Controls [simpleProvider.Control.Controls.Count - 1]);
			} else
				return null;
		}

		#endregion
		
		#region Private Fields

		private SimpleControlProvider simpleProvider;
		
		#endregion
	}
}
