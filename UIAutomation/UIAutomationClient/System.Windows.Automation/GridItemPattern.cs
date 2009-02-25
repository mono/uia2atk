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
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;

namespace System.Windows.Automation
{
	public class GridItemPattern : BasePattern
	{
		public struct GridItemPatternInformation
		{
			public int Row {
				get {
					throw new NotImplementedException ();
				}
			}
			
			public int Column {
				get {
					throw new NotImplementedException ();
				}
			}
			
			public int RowSpan {
				get {
					throw new NotImplementedException ();
				}
			}
			
			public int ColumnSpan {
				get {
					throw new NotImplementedException ();
				}
			}
			
			public AutomationElement ContainingGrid {
				get {
					throw new NotImplementedException ();
				}
			}
		}
		
		internal GridItemPattern ()
		{
		}

		public GridItemPatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public GridItemPatternInformation Current {
			get {
				throw new NotImplementedException ();
			}
		}

		public static readonly AutomationPattern Pattern;

		public static readonly AutomationProperty RowProperty;

		public static readonly AutomationProperty ColumnProperty;

		public static readonly AutomationProperty RowSpanProperty;

		public static readonly AutomationProperty ColumnSpanProperty;

		public static readonly AutomationProperty ContainingGridProperty;
	}
}
