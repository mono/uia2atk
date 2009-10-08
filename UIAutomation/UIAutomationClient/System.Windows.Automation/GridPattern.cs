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
			internal GridPatternInformation (GridProperties properties)
			{
			RowCount = properties.RowCount;
				ColumnCount = properties.ColumnCount;
			}

			public int RowCount {
				get; private set;
			}

			public int ColumnCount {
				get; private set;
			}
		}

		private IGridPattern source;

		internal GridPattern (IGridPattern source)
		{
			this.source = source;
		}

		public GridPatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public GridPatternInformation Current {
			get {
				return new GridPatternInformation (source.Properties);
			}
		}

		public AutomationElement GetItem (int row, int column)
		{
			return SourceManager.GetOrCreateAutomationElement (source.GetItem (row, column));
		}

		public static readonly AutomationPattern Pattern =
			GridPatternIdentifiers.Pattern;

		public static readonly AutomationProperty RowCountProperty =
			GridPatternIdentifiers.RowCountProperty;

		public static readonly AutomationProperty ColumnCountProperty =
			GridPatternIdentifiers.ColumnCountProperty;
	}
}
