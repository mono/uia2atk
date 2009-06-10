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

namespace System.Windows.Automation
{
	public class ScrollPattern : BasePattern
	{
		public struct ScrollPatternInformation
		{
			public double HorizontalScrollPercent {
				get {
					throw new NotImplementedException ();
				}
			}

			public double VerticalScrollPercent {
				get {
					throw new NotImplementedException ();
				}
			}

			public double HorizontalViewSize {
				get {
					throw new NotImplementedException ();
				}
			}

			public double VerticalViewSize {
				get {
					throw new NotImplementedException ();
				}
			}

			public bool HorizontallyScrollable {
				get {
					throw new NotImplementedException ();
				}
			}

			public bool VerticallyScrollable {
				get {
					throw new NotImplementedException ();
				}
			}
		}

		internal ScrollPattern ()
		{
		}

		public ScrollPatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public ScrollPatternInformation Current {
			get {
				throw new NotImplementedException ();
			}
		}

		public void SetScrollPercent (double horizontalPercent, double verticalPercent)
		{
			throw new NotImplementedException ();
		}

		public void Scroll (ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
		{
			throw new NotImplementedException ();
		}

		public void ScrollHorizontal (ScrollAmount amount)
		{
			throw new NotImplementedException ();
		}

		public void ScrollVertical (ScrollAmount amount)
		{
			throw new NotImplementedException ();
		}

		public static readonly AutomationPattern Pattern;

		public static readonly AutomationProperty HorizontalScrollPercentProperty;

		public static readonly AutomationProperty VerticalScrollPercentProperty;

		public static readonly AutomationProperty HorizontalViewSizeProperty;

		public static readonly AutomationProperty VerticalViewSizeProperty;

		public static readonly AutomationProperty HorizontallyScrollableProperty;

		public static readonly AutomationProperty VerticallyScrollableProperty;

		public const double NoScroll = -1;
	}
}
