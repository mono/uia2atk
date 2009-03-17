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
//	Sandy Armstrong  <sanfordarmstrong@gmail.com>
// 

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;

using Mono.UIAutomation.Services;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Behaviors.TreeView
{

	internal class ScrollItemProviderBehavior 
		: ProviderBehavior, IScrollItemProvider
	{
		#region Private Members

		TreeNodeProvider nodeProvider;
		
		#endregion
			
		#region Constructors

		public ScrollItemProviderBehavior (TreeNodeProvider provider)
			: base (provider)
		{
			nodeProvider = provider;
		}

		#endregion
		
		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern { 
			get { return ScrollItemPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			//Doesn't generate any UIA event
		}
		
		public override void Disconnect ()
		{
			//Doesn't generate any UIA event
		}
		
		#endregion

		#region IScrollItemProvider Interface
		
		public void ScrollIntoView ()
		{
			SWF.TreeView treeView = nodeProvider.TreeNode.TreeView;
			if (treeView == null) {
				Log.Error ("TreeView.ScrollItem.ScrollIntoView: Parent TreeView is not set");
				return;
			}
			if (treeView.InvokeRequired)
				treeView.BeginInvoke (new SWF.MethodInvoker (ScrollIntoView));

			nodeProvider.TreeNode.EnsureVisible ();
		}

		#endregion
	}
}
