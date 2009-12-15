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
	public class GridItemPattern : BasePattern
	{
		public struct GridItemPatternInformation
		{
			private bool cache;
			private GridItemPattern pattern;

			internal GridItemPatternInformation (GridItemPattern pattern, bool cache)
			{
				this.pattern = pattern;
				this.cache = cache;
			}

			public int Row {
				get { return (int) pattern.element.GetPropertyValue (RowProperty, cache); }
			}

			public int Column {
				get { return (int) pattern.element.GetPropertyValue (ColumnProperty, cache); }
			}

			public int RowSpan {
				get { return (int) pattern.element.GetPropertyValue (RowSpanProperty, cache); }
			}

			public int ColumnSpan {
				get { return (int) pattern.element.GetPropertyValue (ColumnSpanProperty, cache); }
			}

			public AutomationElement ContainingGrid {
				get { return (AutomationElement) pattern.element.GetPropertyValue (ContainingGridProperty, cache); }
			}
		}

		private AutomationElement element;
		private bool cached;
		private GridItemPatternInformation currentInfo;
		private GridItemPatternInformation cachedInfo;

		internal GridItemPattern ()
		{
		}

		internal GridItemPattern (IGridItemPattern source, AutomationElement element, bool cached)
		{
			this.element = element;
			this.cached = cached;
			this.Source = source;
			currentInfo = new GridItemPatternInformation (this, false);
			if (cached)
				cachedInfo = new GridItemPatternInformation (this, true);
		}

		internal IGridItemPattern Source { get; private set; }

		public GridItemPatternInformation Cached {
			get {
				if (!cached)
					throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
				return cachedInfo;
			}
		}

		public GridItemPatternInformation Current {
			get {
				return currentInfo;
			}
		}

		public static readonly AutomationPattern Pattern =
			GridItemPatternIdentifiers.Pattern;

		public static readonly AutomationProperty RowProperty =
			GridItemPatternIdentifiers.RowProperty;

		public static readonly AutomationProperty ColumnProperty =
			GridItemPatternIdentifiers.ColumnProperty;

		public static readonly AutomationProperty RowSpanProperty =
			GridItemPatternIdentifiers.RowSpanProperty;

		public static readonly AutomationProperty ColumnSpanProperty =
			GridItemPatternIdentifiers.ColumnSpanProperty;

		public static readonly AutomationProperty ContainingGridProperty =
			GridItemPatternIdentifiers.ContainingGridProperty;
	}
}
