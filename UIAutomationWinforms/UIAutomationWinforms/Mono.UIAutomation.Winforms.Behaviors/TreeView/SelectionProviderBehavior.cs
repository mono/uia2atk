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

namespace Mono.UIAutomation.Winforms.Behaviors.TreeView
{
	internal class SelectionProviderBehavior : ProviderBehavior, ISelectionProvider
	{
		#region Private Members

		private TreeViewProvider treeViewProvider;
		private SWF.TreeView treeView;

		#endregion
		
		#region Constructor
		
		public SelectionProviderBehavior (TreeViewProvider provider) :
			base (provider)
		{
			treeViewProvider = provider;
			treeView = provider.Control as SWF.TreeView;
		}

		#endregion

		#region ProviderBehavior Overrides

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == SelectionPatternIdentifiers.CanSelectMultipleProperty.Id)
				return CanSelectMultiple;
			else if (propertyId == SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id)
				return IsSelectionRequired;
			else if (propertyId == SelectionPatternIdentifiers.SelectionProperty.Id)
				return GetSelection ();
			else
				return base.GetPropertyValue(propertyId);
		}

		public override void Disconnect ()
		{
			//TODO
		}

		public override void Connect ()
		{
			//TODO
		}

		public override AutomationPattern ProviderPattern {
			get {
				return SelectionPatternIdentifiers.Pattern;
			}
		}

		#endregion
		
		#region ISelectionProvider implementation 
		
		public IRawElementProviderSimple [] GetSelection ()
		{
			treeViewProvider.RefreshChildControlStructure ();
			
			SWF.TreeNode selectedNode = treeView.SelectedNode;
			TreeNodeProvider selectedNodeProvider = null;
			
			if (selectedNode != null)
				selectedNodeProvider = treeViewProvider.GetTreeNodeProvider (selectedNode);
			
			if (selectedNodeProvider == null)
				return new IRawElementProviderSimple [] {};

			return new IRawElementProviderSimple [] {selectedNodeProvider};
		}
		
		public bool CanSelectMultiple {
			get {
				return false;
			}
		}
		
		public bool IsSelectionRequired {
			get {
				treeViewProvider.RefreshChildControlStructure ();
				return treeView.SelectedNode != null;
			}
		}
		
		#endregion
	}
}
