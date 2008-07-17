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

namespace Mono.UIAutomation.Winforms.Navigation
{
	
	internal abstract class ChildNavigation : SimpleNavigation
	{

#region Constructor

		protected ChildNavigation (IRawElementProviderSimple provider,
		                           NavigationChain chain) 
			: base (provider)
		{
			this.chain = chain;
			parent_provider = (IRawElementProviderFragment) provider;
		}
		
#endregion 

#region Public Properties

		public NavigationChain Chain {
			get { return chain; }
		}
		
		public IRawElementProviderFragment ParentProvider {
			get { return parent_provider; }
		}

#endregion		
		
#region SimpleNavigation: Specializations

		public override IRawElementProviderSimple Provider {
			get { return GetChildProvider (); }
		}
		
		public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
		{
			if (direction == NavigateDirection.Parent)
				return ParentProvider;
			else if (direction == NavigateDirection.NextSibling)
				return GetNextSiblingProvider (Chain);
			else if (direction == NavigateDirection.PreviousSibling)
				return GetPreviousSiblingProvider (Chain);
			else
				return null;
		}
		
		public override void Terminate ()
		{
		}		
		
#endregion
	
#region Protected Methods
		
		protected abstract IRawElementProviderSimple GetChildProvider ();
		
#endregion
		
#region Private Properties

		private NavigationChain chain;		
		private IRawElementProviderFragment parent_provider;
		
#endregion
	}
}
