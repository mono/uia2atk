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
	public class TableItemPattern : GridItemPattern
	{
		public struct TableItemPatternInformation
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

			public AutomationElement [] GetRowHeaderItems ()
			{
				throw new NotImplementedException ();
			}

			public AutomationElement [] GetColumnHeaderItems ()
			{
				throw new NotImplementedException ();
			}
		}
		
		internal TableItemPattern ()
		{
		}

		public new TableItemPatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public new TableItemPatternInformation Current {
			get {
				throw new NotImplementedException ();
			}
		}

		public new static readonly AutomationPattern Pattern;

		public static readonly AutomationProperty RowHeaderItemsProperty;

		public static readonly AutomationProperty ColumnHeaderItemsProperty;
	}
}
