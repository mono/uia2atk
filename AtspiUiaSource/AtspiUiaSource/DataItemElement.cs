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
			internal TableElement parent;

		public DataItemElement (TableElement parent, int row) : base (parent.accessible)
		{
			this.parent = parent;
			this.row = row;
		}

		public override IElement FirstChild {
			get {
				int nColumns = parent.table.NColumns;
				for (int i = 0; i < nColumns; i++) {
					Accessible cell = parent.table.GetAccessibleAt (row, i);
					if (cell != null)
						return Element.GetElement (cell, this, i);
				}
				return null;
			}
		}

		public override IElement PreviousSibling {
			get {
				if (row == 0)
					return parent.Header;
				return parent.rows [row - 1];
			}
		}

		public override IElement NextSibling {
			get {
				if (row >= parent.rows.Count - 1)
					return null;
				return parent.rows [row + 1];
			}
		}

		public override ControlType ControlType {
			get {
				return ControlType.DataItem;
			}
		}
	}
}
