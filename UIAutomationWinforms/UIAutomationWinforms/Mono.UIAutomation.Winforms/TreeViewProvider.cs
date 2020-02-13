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
using System.ComponentModel;
using System.Collections.Generic;

using SWF = System.Windows.Forms;

using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.Unix;

using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Behaviors.TreeView;
using ETVTN = Mono.UIAutomation.Winforms.Events.TreeView.TreeNode;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (SWF.TreeView))]
	internal class TreeViewProvider : TreeNodeRootProvider, IScrollBehaviorSubject
	{
		#region Private Members
		
		private SWF.TreeView treeView;
		private ScrollBehaviorObserver observer;

		#endregion

		#region Constructors
		
		public TreeViewProvider (SWF.TreeView treeView) :
			base (treeView)
		{
			this.treeView = treeView;
		}

		#endregion

		#region FragmentRootControlProvider: Specializations

		public override void Initialize ()
		{
			base.Initialize ();

			observer = new ScrollBehaviorObserver (this, treeView.UIAHScrollBar,
			                                       treeView.UIAVScrollBar);
			observer.ScrollPatternSupportChanged += OnScrollPatternSupportChanged;
			
			UpdateBehaviors ();

			treeView.UIALabelEditChanged += HandleInvalidated;
			treeView.UIACheckBoxesChanged += HandleInvalidated;
			treeView.AfterCheck += HandleAfterCheck;
			treeView.AfterSelect += HandleAfterSelect;
			treeView.AfterExpand += HandleAfterExpand;
			treeView.AfterCollapse += HandleAfterCollapse;
			treeView.UIANodeTextChanged += HandleUIANodeTextChanged;
			treeView.UIACollectionChanged += HandleUIACollectionChanged;
		}

		public override void Terminate ()
		{
			base.Terminate ();

			treeView.UIALabelEditChanged -= HandleInvalidated;
			treeView.UIACheckBoxesChanged -= HandleInvalidated;
			treeView.AfterCheck -= HandleAfterCheck;
			treeView.AfterSelect -= HandleAfterSelect;
			treeView.AfterExpand -= HandleAfterExpand;
			treeView.AfterCollapse -= HandleAfterCollapse;
			treeView.UIANodeTextChanged -= HandleUIANodeTextChanged;
			treeView.UIACollectionChanged -= HandleUIACollectionChanged;
		}

		void HandleUIACollectionChanged (object sender, CollectionChangeEventArgs e)
		{
			TreeNodeRootProvider rootProvider = null;
			SWF.TreeNode senderNode = sender as SWF.TreeNode;

			if (senderNode != null)
				rootProvider = GetTreeNodeProvider (senderNode);
			else if (sender == treeView)
				rootProvider = this;

			if (rootProvider != null)
				rootProvider.OnUIACollectionChanged (e);
		}

		void HandleUIANodeTextChanged (object sender, SWF.TreeViewEventArgs e)
		{
			TreeNodeProvider nodeProvider = GetTreeNodeProvider (e.Node);
			if (nodeProvider != null)
				nodeProvider.OnTextChanged ();
		}

		void HandleAfterCollapse (object sender, SWF.TreeViewEventArgs e)
		{
			TreeNodeProvider nodeProvider = GetTreeNodeProvider (e.Node);
			if (nodeProvider != null)
				nodeProvider.OnAfterCollapse ();
		}

		void HandleAfterExpand (object sender, SWF.TreeViewEventArgs e)
		{
			TreeNodeProvider nodeProvider = GetTreeNodeProvider (e.Node);
			if (nodeProvider != null)
				nodeProvider.OnAfterExpand ();
		}

		void HandleAfterSelect (object sender, SWF.TreeViewEventArgs e)
		{
			TreeNodeProvider nodeProvider = GetTreeNodeProvider (e.Node);
			if (nodeProvider != null)
				nodeProvider.OnAfterSelect ();
		}

		void HandleAfterCheck (object sender, SWF.TreeViewEventArgs e)
		{
			TreeNodeProvider nodeProvider = GetTreeNodeProvider (e.Node);
			if (nodeProvider != null)
				nodeProvider.OnAfterCheck ();
		}

		void HandleInvalidated (object sender, EventArgs e)
		{
			UpdateBehaviors ();
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Tree.Id;
			return base.GetProviderPropertyValue (propertyId);
		}

		#endregion

		#region TreeNodeRootProvider Overrides

		protected override bool SupportsScroll {
			get {
				return observer.SupportsScrollPattern;
			}
		}

		protected override SWF.TreeNodeCollection Nodes {
			get {
				return treeView.Nodes;
			}
		}

		#endregion

		#region IScrollBehaviorSubject implementation
		
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

		#region Private Methods

		private void OnScrollPatternSupportChanged (object o, EventArgs args)
		{
			UpdateBehaviors ();
		}

		private void UpdateBehaviors ()
		{
			UpdateBehaviors (SupportsScroll);
		}

		internal override void UpdateBehaviors (bool supportsScroll)
		{
			if (observer.SupportsScrollPattern &&
			    GetBehavior (ScrollPatternIdentifiers.Pattern) == null)
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             new ScrollProviderBehavior (this));
			else if (!observer.SupportsScrollPattern)
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             null);

			if (GetBehavior (SelectionPatternIdentifiers.Pattern) == null)
				SetBehavior (SelectionPatternIdentifiers.Pattern,
				             new SelectionProviderBehavior (this));
			
			base.UpdateBehaviors (SupportsScroll);
		}

		#endregion
	}

	internal class TreeNodeProvider : TreeNodeRootProvider
	{
		#region Private Members

		private SWF.TreeNode node;
		private SWF.TreeView treeView;
		private bool parentTreeKnownToSupportScroll;

		#endregion

		#region Constructors
		
		public TreeNodeProvider (SWF.TreeNode node, bool treeSupportsScroll) :
			base (null)
		{
			this.node = node;
			treeView = node.TreeView;
			parentTreeKnownToSupportScroll = treeSupportsScroll;
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

		public override SWF.Control AssociatedControl {
			get { return treeView; }
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.TreeItem.Id;
			else if (propertyId == AEIds.NameProperty.Id)
				return node.Text;
			else if (propertyId == AEIds.LabeledByProperty.Id)
				return null;
			else if (propertyId == AEIds.IsOffscreenProperty.Id) {
				return !node.IsVisible;
			} else if (propertyId == AEIds.IsEnabledProperty.Id)
				return treeView.Enabled;
			else if (propertyId == AEIds.HasKeyboardFocusProperty.Id)
				return node.IsSelected && treeView.Focused;
			else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
				return treeView.CanFocus && AreAllParentsExpanded ();
			return base.GetProviderPropertyValue (propertyId);
		}

		protected override Rect BoundingRectangleProperty {
			get {
				var nodeBounds = node.Bounds;
				nodeBounds.Offset (node.TreeView.PointToScreen (System.Drawing.Point.Empty));
				return Helper.RectangleToRect (nodeBounds);
			}
		}

		public override void Initialize ()
		{
			base.Initialize();

			UpdateBehaviors (parentTreeKnownToSupportScroll);

			treeView.EnabledChanged += HandleEnabledChanged;
			
			SetEvent (ProviderEventType.AutomationElementIsOffscreenProperty,
			          new ETVTN.AutomationIsOffscreenPropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementIsEnabledProperty, 
			          new ETVTN.AutomationIsEnabledPropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
			          new ETVTN.AutomationHasKeyboardFocusPropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementBoundingRectangleProperty,
			          new ETVTN.AutomationBoundingRectanglePropertyEvent (this));
			SetEvent (ProviderEventType.AutomationFocusChangedEvent,
			          new ETVTN.AutomationFocusChangedEvent (this));
			SetEvent (ProviderEventType.AutomationElementIsKeyboardFocusableProperty,
			          new ETVTN.AutomationIsKeyboardFocusablePropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementNameProperty,
			          new ETVTN.AutomationNamePropertyEvent (this));
		}

		public override void Terminate ()
		{
			base.Terminate ();
			
			treeView.EnabledChanged -= HandleEnabledChanged;
		}


		private void HandleEnabledChanged(object sender, EventArgs e)
		{
			UpdateBehaviors (parentTreeKnownToSupportScroll);
		}

		private bool AreAllParentsExpanded ()
		{
			SWF.TreeNode currentParent = node.Parent;
			while (currentParent != null) {
				if (!currentParent.IsExpanded)
					return false;
				currentParent = currentParent.Parent;
			}
			return true;
		}

		#endregion

		#region TreeNodeRootProvider Overrides

		protected override bool SupportsScroll {
			get {
				return parentTreeKnownToSupportScroll;
			}
		}

		protected override SWF.TreeNodeCollection Nodes {
			get {
				return node.Nodes;
			}
		}

		#endregion

		#region Internal Methods

		internal override void UpdateBehaviors (bool treeSupportsScroll)
		{
			parentTreeKnownToSupportScroll = treeSupportsScroll;
			
			if (treeView.CheckBoxes &&
			    GetBehavior (TogglePatternIdentifiers.Pattern) == null)
				SetBehavior (TogglePatternIdentifiers.Pattern,
				             new ToggleProviderBehavior (this));
			else if (!treeView.CheckBoxes)
				SetBehavior (TogglePatternIdentifiers.Pattern,
				             null);

			if (treeSupportsScroll && treeView.Enabled &&
			    GetBehavior (ScrollItemPatternIdentifiers.Pattern) == null)
				SetBehavior (ScrollItemPatternIdentifiers.Pattern,
				             new ScrollItemProviderBehavior (this));
			else if (!treeSupportsScroll || !treeView.Enabled)
				SetBehavior (ScrollItemPatternIdentifiers.Pattern,
				             null);

			if (GetBehavior (ExpandCollapsePatternIdentifiers.Pattern) == null)
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
				             new ExpandCollapeProviderBehavior (this));
			
			if (treeView.LabelEdit &&
			    GetBehavior (ValuePatternIdentifiers.Pattern) == null)
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new ValueProviderBehavior (this));
			else if (!treeView.LabelEdit)
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             null);

			if (GetBehavior (SelectionItemPatternIdentifiers.Pattern) == null)
				SetBehavior (SelectionItemPatternIdentifiers.Pattern,
			        	     new SelectionItemProviderBehavior (this));

			base.UpdateBehaviors (treeSupportsScroll);
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

		internal void OnAfterExpand ()
		{
			if (AfterExpand != null)
				AfterExpand (this, EventArgs.Empty);
		}

		internal void OnAfterCollapse ()
		{
			if (AfterCollapse != null)
				AfterCollapse (this, EventArgs.Empty);
		}

		internal void OnTextChanged ()
		{
			if (TextChanged != null)
				TextChanged (this, EventArgs.Empty);
		}

		#endregion

		#region Events

		public event EventHandler AfterCheck;

		public event EventHandler AfterSelect;

		public event EventHandler AfterExpand;

		public event EventHandler AfterCollapse;

		public event EventHandler TextChanged;
		
		#endregion
	}

	internal abstract class TreeNodeRootProvider : FragmentRootControlProvider
	{
		#region Private Members
		
		private Dictionary<SWF.TreeNode, TreeNodeProvider> nodeProviders;

		#endregion

		#region Constructors

		public TreeNodeRootProvider (System.ComponentModel.Component component) :
			base (component)
		{
			nodeProviders =
				new Dictionary<SWF.TreeNode, TreeNodeProvider> ();
		}

		#endregion

		#region FragmentRootControlProvider Overrides
		
		protected override void InitializeChildControlStructure ()
		{
			foreach (SWF.TreeNode node in Nodes) {
				TreeNodeProvider nodeProvider = GetOrCreateNodeProvider (node);
				if (nodeProvider != null)
					AddChildProvider (nodeProvider);
			}
		}
		
		protected override void FinalizeChildControlStructure ()
		{
			foreach (TreeNodeProvider nodeProvider in nodeProviders.Values)
				RemoveChildProvider (nodeProvider);
			OnNavigationChildrenCleared ();
		}

		#endregion

		#region Internal Methods

		/// <summary>
		/// Search recursively for the TreeNodeProvider for the given
		/// TreeNode.
		/// </summary>
		internal TreeNodeProvider GetTreeNodeProvider (SWF.TreeNode node)
		{
			TreeNodeProvider nodeProvider = null;
			if (!nodeProviders.TryGetValue (node, out nodeProvider)) {
				foreach (TreeNodeProvider childNodeProvider in nodeProviders.Values) {
					nodeProvider = childNodeProvider.GetTreeNodeProvider (node);
					if (nodeProvider != null)
						return nodeProvider;
				}
			}
			
			return nodeProvider;
		}

		internal virtual void UpdateBehaviors (bool supportsScroll)
		{
			foreach (TreeNodeProvider nodeProvider in nodeProviders.Values)
				nodeProvider.UpdateBehaviors (supportsScroll);
		}

		#endregion

		#region Private Methods

		private TreeNodeProvider GetOrCreateNodeProvider (SWF.TreeNode node)
		{
			TreeNodeProvider nodeProvider;
			
			if (!nodeProviders.TryGetValue (node, out nodeProvider)) {
				nodeProvider = new TreeNodeProvider (node, SupportsScroll);
				nodeProvider.Initialize ();
				nodeProviders [node]  = nodeProvider;
			}

			return nodeProvider;
		}

		internal void OnUIACollectionChanged (CollectionChangeEventArgs e)
		{
			var changedNode = e.Element as SWF.TreeNode;
			if (changedNode == null)
				return;

			if (e.Action == CollectionChangeAction.Add) {
				var changedNodeProvider = GetOrCreateNodeProvider (changedNode);
				if (changedNodeProvider == null)
					return;
				
				if (changedNode.NextNode != null) {
					if (!nodeProviders.TryGetValue (changedNode.NextNode, out TreeNodeProvider nextNodeProvider))
						return;
					InsertChildProviderBefore (changedNodeProvider, nextNodeProvider);
				} else {
					AddChildProvider (changedNodeProvider);
				}
			} else if (e.Action == CollectionChangeAction.Remove) {
				if (nodeProviders.TryGetValue (changedNode, out TreeNodeProvider changedNodeProvider)) {
					nodeProviders.Remove (changedNode);
					changedNodeProvider.Terminate ();
					RemoveChildProvider (changedNodeProvider);
				}
			}
		}

		#endregion

		#region Protected Abstract Properties

		protected abstract SWF.TreeNodeCollection Nodes { get; }

		protected abstract bool SupportsScroll { get; }

		#endregion
	}
}
