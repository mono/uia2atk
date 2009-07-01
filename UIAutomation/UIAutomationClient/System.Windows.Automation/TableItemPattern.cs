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
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
//  Mike Gorse <mgorse@novell.com>
// 

using System;
using Mono.UIAutomation.Source;

namespace System.Windows.Automation
{
	public class TableItemPattern : GridItemPattern
	{
		public struct TableItemPatternInformation
		{
		private AutomationElement [] rowHeaderItems;
		private AutomationElement [] columnHeaderItems;

			internal TableItemPatternInformation (TableItemProperties properties)
			{
				Row = properties.Row;
				Column = properties.Column;
				RowSpan = properties.RowSpan;
				ColumnSpan = properties.ColumnSpan;
				ContainingGrid = SourceManager.GetOrCreateAutomationElement (properties.ContainingGrid);
				rowHeaderItems = SourceManager.GetOrCreateAutomationElements (properties.RowHeaderItems);
				columnHeaderItems = SourceManager.GetOrCreateAutomationElements (properties.ColumnHeaderItems);
			}

			public int Row {
				get; private set;
			}

			public int Column {
				get; private set;
			}

			public int RowSpan {
				get; private set;
			}

			public int ColumnSpan {
				get; private set;
			}

			public AutomationElement ContainingGrid {
				get; private set;
			}

			public AutomationElement [] GetRowHeaderItems ()
			{
				return rowHeaderItems;
			}

			public AutomationElement [] GetColumnHeaderItems ()
			{
				return columnHeaderItems;
			}
		}

		private ITableItemPattern source;

		internal TableItemPattern (ITableItemPattern source) : base (null)
		{
			this.source = source;
		}

		public new TableItemPatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public new TableItemPatternInformation Current {
			get {
				return new TableItemPatternInformation (source.Properties);
			}
		}

		public new static readonly AutomationPattern Pattern;

		public static readonly AutomationProperty RowHeaderItemsProperty;

		public static readonly AutomationProperty ColumnHeaderItemsProperty;
	}
}
