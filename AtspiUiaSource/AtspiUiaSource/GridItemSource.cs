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
using System.Windows.Automation.Provider;

namespace AtspiUiaSource
{
	public class GridItemSource : IGridItemPattern
	{
		protected Accessible accessible;
		protected Atspi.Table table;

		public GridItemSource (Element element)
		{
			accessible = element.Accessible;
			table = accessible.Parent.QueryTable ();
		}

		public int Row {
			get {
				return table.GetRowAtIndex (accessible.IndexInParent);
			}
		}

		public int Column {
			get {
				return table.GetColumnAtIndex (accessible.IndexInParent);
			}
		}

		public int RowSpan {
			get {
				int row, col, rowExtents, colExtents;
				bool isSelected;
				if (!table.GetRowColumnExtentsAtIndex (accessible.IndexInParent, out row, out col, out rowExtents, out colExtents, out isSelected))
					return 1;
				return rowExtents > 0 ? rowExtents : 1;
			}
		}

		public int ColumnSpan {
			get {
				int row, col, rowExtents, colExtents;
				bool isSelected;
				if (!table.GetRowColumnExtentsAtIndex (accessible.IndexInParent, out row, out col, out rowExtents, out colExtents, out isSelected))
					return 1;
				// colExtents can be 0.  Gail bug?
				return colExtents > 0 ? colExtents : 1;
			}
		}

		public IElement ContainingGrid {
			get {
				return Element.GetElement (accessible.Parent);
			}
		}

		public GridItemProperties Properties {
			get {
				GridItemProperties p = new GridItemProperties ();
				int row, col, rowExtents, colExtents;
				bool isSelected;
				if (table.GetRowColumnExtentsAtIndex (accessible.IndexInParent, out row, out col, out rowExtents, out colExtents, out isSelected)) {
					p.Row = row;
					p.Column = col;
					p.RowSpan = rowExtents > 0 ? rowExtents : 1;
					p.ColumnSpan = colExtents > 0 ? colExtents : 1;
				} else {
					p.Row = -1;
					p.Column = -1;
					p.RowSpan = 1;
					p.ColumnSpan = 1;
				}
				p.ContainingGrid = ContainingGrid;
				return p;
			}
		}
	}
}
