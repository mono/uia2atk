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

namespace Mono.UIAutomation.DbusCore
{
	public struct Rect
	{
		public double x;
		public double y;
		public double width;
		public double height;

		public Rect (System.Windows.Rect rect)
		{
			x = rect.X;
			y = rect.Y;
			width = rect.Width;
			height = rect.Height;
		}

		public override string ToString ()
		{
			if (IsEmpty)
				return "Empty";

			return String.Format ("{0},{1},{2},{3}",
					      x, y, width, height);
		}
		
		public bool IsEmpty { 
			get {
				return (x == Double.PositiveInfinity &&
					y == Double.PositiveInfinity &&
					width == Double.NegativeInfinity &&
					height == Double.NegativeInfinity);
			}
		}
	}
}
