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
using System.Collections.Generic;

using SWF = System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.UIAutomation.Winforms.Behaviors.TreeView;

namespace Mono.UIAutomation.Winforms
{
	internal class TreeViewProvider : FragmentRootControlProvider
	{
		#region Private Members
		
		private SWF.TreeView treeView;
		private Dictionary<SWF.TreeNode, TreeNodeProvider> nodeProviders;

		#endregion

		#region Constructors
		
		public TreeViewProvider (SWF.TreeView treeView) :
			base (treeView)
		{
			this.treeView = treeView;
			nodeProviders = new Dictionary<SWF.TreeNode, TreeNodeProvider> ();
		}

		#endregion

		#region FragmentRootControlProvider: Specializations

		public override void Initialize ()
		{
			base.Initialize ();

			SetBehavior (SelectionPatternIdentifiers.Pattern,
			             new SelectionProviderBehavior (this));
		}


		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Tree.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return "tree";
			return base.GetProviderPropertyValue (propertyId);
		}
		
		public override void InitializeChildControlStructure ()
		{
			// TODO: Any events?
		
			foreach (SWF.TreeNode node in treeView.Nodes) {
				TreeNodeProvider nodeProvider = GetOrCreateNodeProvider (node);
				if (nodeProvider != null)
					OnNavigationChildAdded (false, nodeProvider);
			}
		}
		
		public override void FinalizeChildControlStructure ()
		{
			// TODO: Any events?
			
			foreach (TreeNodeProvider nodeProvider in nodeProviders.Values)
				OnNavigationChildRemoved (false, nodeProvider);
			OnNavigationChildrenCleared (false);
		}

		#endregion

		#region Internal Methods

		internal TreeNodeProvider GetTreeNodeProvider (SWF.TreeNode node)
		{
			TreeNodeProvider nodeProvider = null;
			nodeProviders.TryGetValue (node, out nodeProvider);
			return nodeProvider;
		}

		internal void RefreshChildControlStructure ()
		{
			List<SWF.TreeNode> nodesToRemove = new List<SWF.TreeNode> (nodeProviders.Keys);
			
			foreach (SWF.TreeNode node in treeView.Nodes) {
				if (nodesToRemove.Contains (node)) {
					nodesToRemove.Remove (node);
					continue;
				}
				
				TreeNodeProvider nodeProvider = GetOrCreateNodeProvider (node);
				if (nodeProvider != null)
					OnNavigationChildAdded (true, nodeProvider);
			}

			// Anything left in nodesToRemove should be removed
			foreach (SWF.TreeNode node in nodesToRemove) {
				TreeNodeProvider nodeProvider = nodeProviders [node];
				nodeProviders.Remove (node);
				nodeProvider.Terminate ();
				OnNavigationChildRemoved (true, nodeProvider);
			}
		}

		#endregion

		#region Private Methods

		private TreeNodeProvider GetOrCreateNodeProvider (SWF.TreeNode node)
		{
			TreeNodeProvider nodeProvider;
			
			if (!nodeProviders.TryGetValue (node, out nodeProvider)) {
				nodeProvider = new TreeNodeProvider (node);
				nodeProvider.Initialize ();
				nodeProviders [node]  = nodeProvider;
			}

			return nodeProvider;
		}

		#endregion
	}

	internal class TreeNodeProvider : FragmentRootControlProvider
	{
		#region Private Members

		private SWF.TreeNode node;

		#endregion

		#region Constructors
		
		public TreeNodeProvider (SWF.TreeNode node) :
			base (null)
		{
			this.node = node;
		}

		#endregion

		#region Public Properties

		public SWF.TreeNode TreeNode {
			get {
				return node;
			}
		}
		
		#endregion

		#region Provider Overrides

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.TreeItem.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return "tree item";
			else if (propertyId == AEIds.NameProperty.Id)
				return node.Text;
			return base.GetProviderPropertyValue (propertyId);
		}

		public override void Initialize ()
		{
			base.Initialize();

			SetBehavior (SelectionItemPatternIdentifiers.Pattern,
			             new SelectionItemProviderBehavior (this));
		}

		#endregion

		
	}
}
