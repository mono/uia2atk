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
	internal class TreeViewProvider : FragmentRootControlProvider, IScrollBehaviorSubject
	{
		#region Private Members
		
		private SWF.TreeView treeView;
		private Dictionary<SWF.TreeNode, TreeNodeProvider> nodeProviders;
		private ScrollBehaviorObserver observer;

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

			SWF.ScrollBar vscrollbar 
				= Helper.GetPrivateProperty<SWF.TreeView, SWF.ScrollBar> (
				typeof (SWF.TreeView), treeView, "UIAVScrollBar");
			SWF.ScrollBar hscrollbar 
				= Helper.GetPrivateProperty<SWF.TreeView, SWF.ScrollBar> (
				typeof (SWF.TreeView), treeView, "UIAHScrollBar");

			observer = new ScrollBehaviorObserver (this, hscrollbar, vscrollbar);
			observer.ScrollPatternSupportChanged += OnScrollPatternSupportChanged;
			
			UpdateScrollBehavior ();
			UpdateBehaviors ();

			// TODO: Use a custom event, or someting else?
			treeView.Invalidated += HandleInvalidated;

			// TODO: Move these to event handler classes, and
			//       connect/disconnect based on CheckBoxes value
			treeView.AfterCheck += HandleAfterCheck;
			treeView.AfterSelect += HandleAfterSelect;
		}

		void HandleAfterSelect(object sender, SWF.TreeViewEventArgs e)
		{
			TreeNodeProvider nodeProvider;
			if (!nodeProviders.TryGetValue (e.Node, out nodeProvider))
				return;
			nodeProvider.OnAfterSelect ();
		}

		void HandleAfterCheck(object sender, SWF.TreeViewEventArgs e)
		{
			TreeNodeProvider nodeProvider;
			if (!nodeProviders.TryGetValue (e.Node, out nodeProvider))
				return;
			nodeProvider.OnAfterCheck ();
		}

		void HandleInvalidated(object sender, SWF.InvalidateEventArgs e)
		{
			UpdateBehaviors ();
			RefreshChildControlStructure ();
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

		#region IScrollBehaviorSubject implementation 
		
//		public void AddChildProvider (bool raiseEvent, FragmentControlProvider provider)
//		{
//			throw new System.NotImplementedException();
//		}
//		
//		public void RemoveChildProvider (bool raiseEvent, FragmentControlProvider provider)
//		{
//			throw new System.NotImplementedException();
//		}
		
		public FragmentControlProvider GetScrollbarProvider (SWF.ScrollBar scrollbar)
		{
			return new ScrollBarProvider (scrollbar);
		}
		
		public IScrollBehaviorObserver ScrollBehaviorObserver {
			get {
				return observer;
			}
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
					nodeProviders [node].UpdateBehaviors ();
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

		private void OnScrollPatternSupportChanged (object o, EventArgs args)
		{
			UpdateScrollBehavior ();
		}

		private void UpdateBehaviors ()
		{
			if (GetBehavior (SelectionPatternIdentifiers.Pattern) == null)
				SetBehavior (SelectionPatternIdentifiers.Pattern,
				             new SelectionProviderBehavior (this));
		}

		private void UpdateScrollBehavior ()
		{
			if (observer.SupportsScrollPattern &&
			    GetBehavior (ScrollPatternIdentifiers.Pattern) == null)
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             new ScrollProviderBehavior (this));
			else if (!observer.SupportsScrollPattern)
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             null);
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

			UpdateBehaviors ();
		}

		#endregion

		#region Internal Methods

		internal void UpdateBehaviors ()
		{
			if (node.TreeView.CheckBoxes &&
			    GetBehavior (TogglePatternIdentifiers.Pattern) == null)
				SetBehavior (TogglePatternIdentifiers.Pattern,
				             new ToggleProviderBehavior (this));
			else if (!node.TreeView.CheckBoxes)
				SetBehavior (TogglePatternIdentifiers.Pattern,
				             null);
				
//			if (node.TreeView.Scrollable)
//				; // TODO: ScrollItem
//			if (node.TreeView.LabelEdit)
//				; // TODO: Value

			if (GetBehavior (SelectionItemPatternIdentifiers.Pattern) == null)
				SetBehavior (SelectionItemPatternIdentifiers.Pattern,
			        	     new SelectionItemProviderBehavior (this));
		}

		internal void OnAfterCheck ()
		{
			if (AfterCheck != null)
				AfterCheck (this, EventArgs.Empty);
		}

		internal void OnAfterSelect ()
		{
			if (AfterSelect != null)
				AfterSelect (this, EventArgs.Empty);
		}

		#endregion

		#region Events

		public event EventHandler AfterCheck;

		public event EventHandler AfterSelect;
		
		#endregion
	}
}
