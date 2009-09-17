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
	public class TableSource : GridSource, ITablePattern
	{
		public TableSource (Element element) : base (element)
		{
		}

		public RowOrColumnMajor RowOrColumnMajor {
			get {
				// TODO
				return RowOrColumnMajor.Indeterminate;
			}
		}

		public IElement [] GetRowHeaders ()
		{
			// TODO: would be nice if at-spi made this easier
			Accessible accessible = table.GetRowHeader (0);
			// If that returned null, then maybe we have no
			// headers, so not going to query every single row
			if (accessible == null)
				return new Element [0];
			int count = RowCount;
			Element [] elements = new Element [count];
			elements [0] = Element.GetElement (accessible);
			for (int i = 1; i < count; i++) {
				accessible = table.GetRowHeader (i);
				if (accessible != null)
					elements [i] = Element.GetElement (accessible);
			}
			return elements;
		}

		public IElement [] GetColumnHeaders ()
		{
			// TODO: would be nice if at-spi made this easier
			Accessible accessible = table.GetColumnHeader (0);
			// If that returned null, then maybe we have no
			// headers, so not going to query every single row
			if (accessible == null)
				return new Element [0];
			int count = ColumnCount;
			Element [] elements = new Element [count];
			elements [0] = Element.GetElement (accessible);
			for (int i = 1; i < count; i++) {
				accessible = table.GetColumnHeader (i);
				if (accessible != null)
					elements [i] = Element.GetElement (accessible);
			}
			return elements;
		}

		TableProperties ITablePattern.Properties {
			get {
				TableProperties p = new TableProperties ();
				p.RowCount = RowCount;
				p.ColumnCount = ColumnCount;
				p.RowOrColumnMajor = RowOrColumnMajor;
				p.RowHeaders = GetRowHeaders ();
				p.ColumnHeaders = GetColumnHeaders ();
				return p;
			}
		}
	}
}
