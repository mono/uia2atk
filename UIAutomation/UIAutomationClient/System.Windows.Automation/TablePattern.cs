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
			private bool cache;
			private TablePattern pattern;

			internal TablePatternInformation (TablePattern pattern, bool cache)
			{
				this.pattern = pattern;
				this.cache = cache;
			}

			public int RowCount {
				get { return (int) pattern.element.GetPropertyValue (RowCountProperty, cache); }
			}

			public int ColumnCount {
				get { return (int) pattern.element.GetPropertyValue (ColumnCountProperty, cache); }
			}

			public RowOrColumnMajor RowOrColumnMajor {
				get { return (RowOrColumnMajor) pattern.element.GetPropertyValue (RowOrColumnMajorProperty, cache); }
			}

			public AutomationElement [] GetRowHeaders ()
			{
				return (AutomationElement []) pattern.element.GetPropertyValue (RowHeadersProperty, cache);
			}

			public AutomationElement [] GetColumnHeaders ()
			{
				return (AutomationElement []) pattern.element.GetPropertyValue (ColumnHeadersProperty, cache);
			}
		}

		private AutomationElement element;
		private bool cached;
		private TablePatternInformation currentInfo;
		private TablePatternInformation cachedInfo;

		internal TablePattern ()
		{
		}

		internal TablePattern (ITablePattern source, AutomationElement element, bool cached) :
			base (source, element, cached)
		{
			this.element = element;
			this.cached = cached;
			this.Source = source;
			currentInfo = new TablePatternInformation (this, false);
			if (cached)
				cachedInfo = new TablePatternInformation (this, true);
		}

		internal new ITablePattern Source { get; private set; }

		public new TablePatternInformation Cached {
			get {
				if (!cached)
					throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
				return cachedInfo;
			}
		}

		public new TablePatternInformation Current {
			get {
				return currentInfo;
			}
		}

		public new static readonly AutomationPattern Pattern =
			TablePatternIdentifiers.Pattern;

		public static readonly AutomationProperty RowHeadersProperty =
			TablePatternIdentifiers.RowHeadersProperty;

		public static readonly AutomationProperty ColumnHeadersProperty =
			TablePatternIdentifiers.ColumnHeadersProperty;

		public static readonly AutomationProperty RowOrColumnMajorProperty =
			TablePatternIdentifiers.RowOrColumnMajorProperty;
	}
}
