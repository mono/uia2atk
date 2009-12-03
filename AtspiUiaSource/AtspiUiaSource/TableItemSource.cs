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
	public class TableItemSource : GridItemSource, ITableItemPattern
	{
		public TableItemSource (Element element) : base (element)
		{
		}

		public IElement [] GetRowHeaderItems ()
		{
			Accessible header = table.GetRowHeader (Row);
			if (header == null)
				return new Element [0];
			Element [] elements = new Element [1];
			elements [0] = Element.GetElement (header);
			return elements;
		}

		public IElement [] GetColumnHeaderItems ()
		{
			Accessible header = table.GetColumnHeader (Column);
			if (header == null)
				return new Element [0];
			Element [] elements = new Element [1];
			elements [0] = Element.GetElement (header);
			return elements;
		}

		public new TableItemProperties Properties {
			get {
				TableItemProperties p = new TableItemProperties ();
				int row, col, rowExtents, colExtents;
				bool isSelected;
				if (table.GetRowColumnExtentsAtIndex (accessible.IndexInParent, out row, out col, out rowExtents, out colExtents, out isSelected)) {
					p.Row = row;
					p.Column = col;
					p.RowSpan = rowExtents > 0? rowExtents : 1;
					p.ColumnSpan = colExtents > 0? colExtents : 1;
				} else {
					p.Row = Row;
					p.Column = Column;
					p.RowSpan = RowSpan;
					p.ColumnSpan = ColumnSpan;
				}
				p.ContainingGrid = ContainingGrid;
				p.RowHeaderItems = GetRowHeaderItems ();
				p.ColumnHeaderItems = GetColumnHeaderItems ();
				return p;
			}
		}
	}
}
