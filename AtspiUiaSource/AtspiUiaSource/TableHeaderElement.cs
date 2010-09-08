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
	public class TableHeaderElement: Element
	{
		internal TableElement tableElement;

		public TableHeaderElement (Accessible accessible, TableElement tableElement) : base (accessible)
		{
			this.tableElement = tableElement;
		}

		public override IElement FirstChild {
			get {
				foreach (Accessible child in accessible.Children)
					if (child.Role == Role.TableColumnHeader || child.Role == Role.TableRowHeader)
						return Element.GetElement (child);
				return null;
			}
		}

		public override IElement PreviousSibling {
			get {
				return null;
			}
		}

		public override IElement NextSibling {
			get {
				if (tableElement.rows.Count == 0)
					return null;
				return tableElement.rows [0];
			}
		}

		public override ControlType ControlType {
			get {
				return ControlType.Header;
			}
		}

		public override bool IsContentElement {
			get {
				return false;
			}
		}

		public override Rect BoundingRectangle {
			get {
				return Rect.Union (
					FirstChild.BoundingRectangle,
					LastChild.BoundingRectangle);
			}
		}

		internal override bool SupportsGrid ()
		{
			return false;
		}

		internal override bool SupportsSelection ()
		{
			return false;
		}
	}
}
