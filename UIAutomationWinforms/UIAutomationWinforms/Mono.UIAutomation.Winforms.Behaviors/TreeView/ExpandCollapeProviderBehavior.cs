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
//	Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using SWF = System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.TreeView;

namespace Mono.UIAutomation.Winforms.Behaviors.TreeView
{
	internal class ExpandCollapeProviderBehavior : ProviderBehavior, IExpandCollapseProvider
	{
		#region Private Members

		private TreeNodeProvider nodeProvider;

		#endregion
		
		#region Constructors
		
		public ExpandCollapeProviderBehavior (TreeNodeProvider nodeProvider) :
			base (nodeProvider)
		{
			this.nodeProvider = nodeProvider;
		}

		#endregion

		#region ProviderBehavior Overrides

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id)
			    return ExpandCollapseState;
			return base.GetPropertyValue(propertyId);
		}

		public override void Connect ()
		{
			nodeProvider.SetEvent (ProviderEventType.ExpandCollapsePatternExpandCollapseStateProperty,
			                       new ExpandCollapsePatternExpandCollapseStateEvent (nodeProvider));
		}

		public override void Disconnect ()
		{
			nodeProvider.SetEvent (ProviderEventType.ExpandCollapsePatternExpandCollapseStateProperty,
			                       null);
		}

		public override AutomationPattern ProviderPattern {
			get {
				return ExpandCollapsePatternIdentifiers.Pattern;
			}
		}

		#endregion
		
		#region IExpandCollapseProvider implementation
		
		public void Collapse ()
		{
			if (ExpandCollapseState == ExpandCollapseState.LeafNode)
				throw new InvalidOperationException ("LeafNode");

			SWF.TreeView treeView = nodeProvider.TreeNode.TreeView;

			if (!treeView.Enabled)
				throw new ElementNotEnabledException ();
			
			if (treeView != null && treeView.InvokeRequired) {
				treeView.BeginInvoke (new SWF.MethodInvoker (PerformCollapse));
				return;
			}

			PerformCollapse ();
		}
		
		public void Expand ()
		{
			if (ExpandCollapseState == ExpandCollapseState.LeafNode)
				throw new InvalidOperationException ("LeafNode");

			SWF.TreeView treeView = nodeProvider.TreeNode.TreeView;

			if (!treeView.Enabled)
				throw new ElementNotEnabledException ();
			
			if (treeView != null && treeView.InvokeRequired) {
				treeView.BeginInvoke (new SWF.MethodInvoker (PerformExpand));
				return;
			}

			PerformExpand ();
		}
		
		public ExpandCollapseState ExpandCollapseState {
			get {
				if (nodeProvider.TreeNode.Nodes.Count == 0)
					return ExpandCollapseState.LeafNode;
				else if (nodeProvider.TreeNode.IsExpanded)
					return ExpandCollapseState.Expanded;
				else
					return ExpandCollapseState.Collapsed;
			}
		}
		
		#endregion

		#region Private Methods

		private void PerformCollapse ()
		{
			nodeProvider.TreeNode.Collapse ();
		}

		private void PerformExpand ()
		{
			nodeProvider.TreeNode.Expand ();
		}

		#endregion
	}
}
