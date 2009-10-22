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
//  Matt Guo <matt@mattguo.com>
// 

using System;

namespace Mono.UIAutomation.UiaDbus.Interfaces
{
	public static class InterfaceConverter
	{
		public static IGridPattern Table2Grid (ITablePattern pattern)
		{
			return new GridProxy (pattern);
		}

		public static IGridItemPattern TableItem2GridItem (ITableItemPattern pattern)
		{
			return new GridItemProxy (pattern);
		}
	}

	internal class GridProxy : IGridPattern
	{
		private ITablePattern pattern;

		public GridProxy (ITablePattern pattern)
		{
			this.pattern = pattern;
		}

		public string GetItemPath (int row, int column)
		{
			return pattern.GetItemPath (row, column);
		}

		public int ColumnCount {
			get {
				return pattern.ColumnCount;
			}
		}

		public int RowCount {
			get {
				return pattern.RowCount;
			}
		}
	}

	internal class GridItemProxy : IGridItemPattern
	{
		private ITableItemPattern pattern;

		public GridItemProxy (ITableItemPattern pattern)
		{
			this.pattern = pattern;
		}

		public int Column {
			get {
				return pattern.Column;
			}
		}

		public int ColumnSpan {
			get {
				return pattern.ColumnSpan;
			}
		}

		public int Row {
			get {
				return pattern.Row;
			}
		}

		public int RowSpan {
			get {
				return pattern.RowSpan;
			}
		}

		public string ContainingGridPath {
			get {
				return pattern.ContainingGridPath;
			}
		}
	}
}
