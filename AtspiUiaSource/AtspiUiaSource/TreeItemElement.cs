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
//      Mike Gorse <mgorse@novell.com>
//

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using Mono.Unix;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.Source;
using Atspi;

namespace AtspiUiaSource
{
	public class TreeItemElement : Element
	{
		internal int row;
			private TableElement tableElement;
		internal TreeItemElement parentCell;
			private List<TreeItemElement> ancestors;

		public TreeItemElement (Accessible accessible, TableElement tableElement, int row) : base (accessible)
		{
			this.row = row;
			this.tableElement = tableElement;
			SetParentCell ();
		}

		public TreeItemElement (Accessible accessible) : base (accessible)
		{
			tableElement = Element.GetElement (accessible.Parent) as TableElement;
			this.row = tableElement.table.GetRowAtIndex (accessible.IndexInParent);;
			SetParentCell ();
		}

		private void SetParentCell ()
		{
			Relation [] relations = accessible.RelationSet;
			foreach (Relation relation in relations) {
				if (relation.Type == RelationType.NodeChildOf) {
					parentCell = Element.GetElement (relation.Targets [0]) as TreeItemElement;
					break;
				}
			}
			ancestors = new List<TreeItemElement> ();
			for (TreeItemElement ancestor = parentCell; ancestor != null; ancestor = ancestor.parentCell)
				ancestors.Add (ancestor);
		}

		public override IElement Parent {
			get {
				if (parentCell != null)
					return parentCell;
				return Element.GetElement (accessible.Parent);
			}
		}

		public override IElement FirstChild {
			get {
				if (row >= tableElement.table.NRows - 1)
					return null;
				TreeItemElement e = tableElement.GetTreeItemElement (row + 1);
				if (e.parentCell == this)
					return e;
				return null;
			}
		}

		public override IElement NextSibling {
			get {
				int count = tableElement.table.NRows;
				for (int i = row + 1; i < count; i++) {
					TreeItemElement e = tableElement.GetTreeItemElement (i);
					if (e.parentCell == this.parentCell)
						return e;
					if (ancestors.IndexOf (e.parentCell) > -1)
						return null;
				}
				return null;
			}
		}

		public override IElement PreviousSibling {
			get {
				for (int i = row - 1; i >= 0; i--) {
					TreeItemElement e = tableElement.GetTreeItemElement (i);
					if (e.parentCell == this.parentCell)
						return e;
					if (ancestors.IndexOf (e.parentCell) > -1)
						return null;
				}
				return null;
			}
		}

		public override ControlType ControlType {
			get {
				return ControlType.TreeItem;
			}
		}
	}
}
