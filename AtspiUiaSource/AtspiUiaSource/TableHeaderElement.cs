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
				return tableElement.rows [0];
			}
		}

		public override ControlType ControlType {
			get {
				return ControlType.Header;
			}
		}

		// TODO: We should probably override BoundingRectangle;
			// otherwise we'll return the size of the table
		}

		public class TableHeaderItemElement : Element
		{
		public TableHeaderItemElement (Accessible accessible) : base (accessible)
		{
		}

		public override IElement Parent {
			get {
				TableElement table = Element.GetElement (accessible.Parent) as TableElement;
				if (table == null) {
					Console.WriteLine ("AtspiUiaSource: TableHeaderElement in a bad state!");
					return null;
				}
				return table.Header;
			}
		}

		public override IElement PreviousSibling {
			get {
				Accessible parent = accessible.Parent;
				// Can someone suggest a better way to do this?
				// Seems like a horrible way to iterate through a List.
			// Can't remember whether I or a reviewer decided that
			// Children should return an IList and not an array.
				for (int i = accessible.IndexInParent - 1; i >= 0; i--) {
					Accessible sibling = parent.Children [i];
					if (sibling.Role == Role.TableColumnHeader || sibling.Role == Role.TableRowHeader)
						return Element.GetElement (parent.Children [i]);
				}
				return null;
			}
		}

		public override IElement NextSibling {
			get {
				Accessible parent = accessible.Parent;
			int count = parent.Children.Count;
				for (int i = accessible.IndexInParent + 1; i < count; i++) {
					Accessible sibling = parent.Children [i];
					if (sibling.Role == Role.TableColumnHeader || sibling.Role == Role.TableRowHeader)
						return Element.GetElement (parent.Children [i]);
				}
				return null;
			}
		}
	}
}
