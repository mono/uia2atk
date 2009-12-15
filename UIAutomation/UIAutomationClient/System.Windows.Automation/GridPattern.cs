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
// 

using System;
using Mono.UIAutomation.Source;

namespace System.Windows.Automation
{
	public class GridPattern : BasePattern
	{
		public struct GridPatternInformation
		{
			private bool cache;
			private GridPattern pattern;

			internal GridPatternInformation (GridPattern pattern, bool cache)
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
		}

		private AutomationElement element;
		private bool cached;
		private GridPatternInformation currentInfo;
		private GridPatternInformation cachedInfo;

		internal GridPattern ()
		{
		}

		internal GridPattern (IGridPattern source, AutomationElement element, bool cached)
		{
			this.element = element;
			this.cached = cached;
			this.Source = source;
			currentInfo = new GridPatternInformation (this, false);
			if (cached)
				cachedInfo = new GridPatternInformation (this, true);
		}

		internal IGridPattern Source { get; private set; }

		public GridPatternInformation Cached {
			get {
				if (!cached)
					throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
				return cachedInfo;
			}
		}

		public GridPatternInformation Current {
			get {
				return currentInfo;
			}
		}

		public AutomationElement GetItem (int row, int column)
		{
			return SourceManager.GetOrCreateAutomationElement (Source.GetItem (row, column));
		}

		public static readonly AutomationPattern Pattern =
			GridPatternIdentifiers.Pattern;

		public static readonly AutomationProperty RowCountProperty =
			GridPatternIdentifiers.RowCountProperty;

		public static readonly AutomationProperty ColumnCountProperty =
			GridPatternIdentifiers.ColumnCountProperty;
	}
}
