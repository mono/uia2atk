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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//  Matt Guo <matt@mattguo.com>
// 

using System;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.UiaDbus.Interfaces;

namespace Mono.UIAutomation.UiaDbusBridge.Wrappers
{
	public class SelectionPatternWrapper : ISelectionPattern
	{
#region Private Fields

		private ISelectionProvider provider;

#endregion

#region Constructor

		public SelectionPatternWrapper (ISelectionProvider provider)
		{
			this.provider = provider;
		}

#endregion

#region ISelectionPattern Members

		public string [] GetSelectionPaths ()
		{
			var selection = provider.GetSelection ();
			List<string> selectionPaths = new List<string> (selection.Length);
			foreach (var section in selection)
			{
				string elementPath =
					AutomationBridge.Instance.FindWrapperByProvider (section).Path;
				selectionPaths.Add (elementPath);
			}
			return selectionPaths.ToArray ();
		}

		public bool CanSelectMultiple {
			get {
				return provider.CanSelectMultiple;
			}
		}

		public bool IsSelectionRequired {
			get {
				return provider.IsSelectionRequired;
			}
		}

#endregion
	}
}
