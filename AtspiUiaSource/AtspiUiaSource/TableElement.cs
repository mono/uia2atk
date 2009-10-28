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
	public class TableElement: Element
	{
		private TableHeaderElement header;
		private bool treeMode;
		internal Atspi.Table table;
		internal Element [] rows;

		public TableElement (Accessible accessible) : base (accessible)
		{
			header = null;
			table = accessible.QueryTable ();
			RefreshTreeMode (true);
		}

		public void RefreshTreeMode ()
		{
			RefreshTreeMode (false);
		}

		public void RefreshTreeMode (bool fromCtor)
		{
			bool newTreeMode = (table == null || table.NColumns == 1);
			if (!fromCtor && treeMode == newTreeMode)
				return;
			treeMode = newTreeMode;
			int count = table.NRows;
			rows = new Element [count];
			if (!treeMode) {
				for (int i = 0; i < count; i++)
					rows [i] = new DataItemElement (this, i);
			}
		}

		internal TreeItemElement GetTreeItemElement (int row)
		{
			if (rows [row] != null)
				return rows [row] as TreeItemElement;
			Accessible cell = table.GetAccessibleAt (row, 0);
			TreeItemElement e = Element.GetElement (cell, this, row) as TreeItemElement;
			rows [row] = e;
			return e;
		}

		public override IElement FirstChild {
			get {
				if (!treeMode)
					return (Header != null? Header: rows [0]);
				int count = table.NRows;
				for (int i = 0; i < count; i++) {
					TreeItemElement cell = GetTreeItemElement (i);
					if (cell.parentCell == null)
						return cell;
				}
				return null;
			}
		}

		public Element Header {
			get {
				if (header != null)
					return header;
				foreach (Accessible child in accessible.Children) {
					if (child.Role == Role.TableColumnHeader || child.Role == Role.TableRowHeader) {
						header = new TableHeaderElement (accessible, this);
						return header;
					}
				}
				return null;
			}
		}

		public override ControlType ControlType {
			get {
				return (treeMode ?
					ControlType.Tree :
					ControlType.Table);
			}
		}
	}
}
