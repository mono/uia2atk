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
	public class TablePattern : GridPattern
	{
		public struct TablePatternInformation
		{
			private AutomationElement [] rowHeaders;
			private AutomationElement [] columnHeaders;

			internal TablePatternInformation (TableProperties properties)
			{
				RowCount = properties.RowCount;
				ColumnCount = properties.ColumnCount;
				RowOrColumnMajor = properties.RowOrColumnMajor;
				rowHeaders = SourceManager.GetOrCreateAutomationElements (properties.RowHeaders);
				columnHeaders = SourceManager.GetOrCreateAutomationElements (properties.ColumnHeaders);
			}

			public int RowCount {
				get; private set;
				}

			public int ColumnCount {
				get; private set;
			}

			public RowOrColumnMajor RowOrColumnMajor {
				get; private set;
			}

			public AutomationElement [] GetRowHeaders ()
			{
				return rowHeaders;
			}

			public AutomationElement [] GetColumnHeaders ()
			{
				return columnHeaders;
			}
		}

		private ITablePattern source;

		internal TablePattern (ITablePattern source) : base (null)
		{
			this.source = source;
		}

		public new TablePatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public new TablePatternInformation Current {
			get {
				return new TablePatternInformation (source.Properties);
			}
		}

		public new static readonly AutomationPattern Pattern;

		public static readonly AutomationProperty RowHeadersProperty;

		public static readonly AutomationProperty ColumnHeadersProperty;

		public static readonly AutomationProperty RowOrColumnMajorProperty;
	}
}
