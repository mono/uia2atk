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
	public class DataItemElement : Element
	{
		internal int row;
		internal TableElement containingGrid;

		public DataItemElement (TableElement containingGrid, int row) : base (containingGrid.accessible)
		{
			this.containingGrid = containingGrid;
			this.row = row;
		}

		public override IElement Parent {
			get {
				return containingGrid;
			}
		}

		public override IElement FirstChild {
			get {
				int nColumns = containingGrid.table.NColumns;
				for (int i = 0; i < nColumns; i++) {
					Accessible cell = containingGrid.table.GetAccessibleAt (row, i);
					if (cell != null)
						return Element.GetElement (cell, this, i);
				}
				return null;
			}
		}

		public override IElement PreviousSibling {
			get {
				if (row == 0)
					return containingGrid.Header;
				return containingGrid.rows [row - 1];
			}
		}

		public override IElement NextSibling {
			get {
				if (row >= containingGrid.rows.Count - 1)
					return null;
				return containingGrid.rows [row + 1];
			}
		}

		public override ControlType ControlType {
			get {
				return ControlType.DataItem;
			}
		}

		public override Rect BoundingRectangle {
			get {
				return Rect.Union (
					FirstChild.BoundingRectangle,
					LastChild.BoundingRectangle);
			}
		}

		internal override object GetCurrentPatternInternal (AutomationPattern pattern)
		{
			if (pattern == GridItemPatternIdentifiers.Pattern)
				return new DataItemGridItemSource (this);
			if (pattern == SelectionItemPatternIdentifiers.Pattern)
				return new SelectionItemSource ((Element) FirstChild);
			return base.GetCurrentPatternInternal (pattern);
		}

		internal override bool SupportsGrid ()
		{
			return false;
		}

		internal override bool SupportsGridItem ()
		{
			return true;
		}

		internal override bool SupportsSelection ()
		{
			return false;
		}

		internal override bool SupportsSelectionItem ()
		{
			return true;
		}
	}
}
