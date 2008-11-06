//// Permission is hereby granted, free of charge, to any person obtaining 
//// a copy of this software and associated documentation files (the 
//// "Software"), to deal in the Software without restriction, including 
//// without limitation the rights to use, copy, modify, merge, publish, 
//// distribute, sublicense, and/or sell copies of the Software, and to 
//// permit persons to whom the Software is furnished to do so, subject to 
//// the following conditions: 
////  
//// The above copyright notice and this permission notice shall be 
//// included in all copies or substantial portions of the Software. 
////  
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
//// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
//// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
//// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
//// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
//// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//// 
//// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
//// 
//// Authors: 
////      Stephen Shaw <sshaw@decriptor.com>
////      Mario Carrion <mcarrion@novell.com>
//// 
//

using System;

namespace System.Windows.Automation
{
	public static class TableItemPatternIdentifiers
	{
		private const int PatternId = 10013;
		private const int ColumnHeaderItemsPropertyId = 30085;
		private const int RowHeaderItemsPropertyId = 30084;
		
#region Constructor
		static TableItemPatternIdentifiers()
		{
			Pattern =
				new AutomationPattern (PatternId,
				                       "TableItemPatternIdentifiers.Pattern");

			ColumnHeaderItemsProperty =
				new AutomationProperty (ColumnHeaderItemsPropertyId,
						"TableItemPatternIdentifiers.ColumnHeaderItemsProperty");

			RowHeaderItemsProperty =
				new AutomationProperty (RowHeaderItemsPropertyId,
						"TableItemPatternIdentifiers.RowHeaderItemsProperty");
		}

#endregion

#region Public Fields

		public static readonly AutomationProperty ColumnHeaderItemsProperty;

		public static readonly AutomationPattern Pattern;

		public static readonly AutomationProperty RowHeaderItemsProperty;

#endregion

	}
}