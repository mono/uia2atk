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

namespace Mono.UIAutomation.Winforms.Navigation
{
	
	public class NavigationChain
	{

#region Constructors
		
		public NavigationChain ()
		{
			list = new List<INavigation> ();
		}
		
#endregion 
		
#region Public Methods
		
		public void AddLink (INavigation link) 
		{
			if (!list.Contains (link)) {
				list.Add (link);
				if (list.Count > 1) {
					list [list.Count - 2].NextSibling = link;
					link.PreviousSibling = list [list.Count - 2];
				}
			}
		}
		
		public INavigation GetFirstLink () 
		{
			for (int index = 0; index < list.Count; index++) {
				if (list [index].SupportsNavigation)
					return list [index];
			}
			return null;
		}
		
		public INavigation GetLastLink () 
		{
			for (int index = list.Count - 1; index >= 0; index--) {
				if (list [index].SupportsNavigation)
					return list [index];
			}
			return null;
		}
		
#endregion 
		
#region Private Fields
		
		private List<INavigation> list;
		
#endregion
	}
}
