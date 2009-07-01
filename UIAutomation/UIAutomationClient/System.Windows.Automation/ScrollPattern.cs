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
	public class ScrollPattern : BasePattern
	{
		public struct ScrollPatternInformation
		{
			internal ScrollPatternInformation (ScrollProperties properties)
			{
				HorizontalScrollPercent = properties.HorizontalScrollPercent;
				VerticalScrollPercent = properties.VerticalScrollPercent;
				HorizontalViewSize = properties.HorizontalViewSize;
				VerticalViewSize = properties.VerticalViewSize;
				HorizontallyScrollable = properties.HorizontallyScrollable;
				VerticallyScrollable = properties.VerticallyScrollable;
			}

			public double HorizontalScrollPercent {
				get; private set;
			}

			public double VerticalScrollPercent {
				get; private set;
			}

			public double HorizontalViewSize {
				get; private set;
			}

			public double VerticalViewSize {
				get; private set;
			}

			public bool HorizontallyScrollable {
				get; private set;
			}

			public bool VerticallyScrollable {
				get; private set;
			}
		}

		private IScrollPattern source;

		internal ScrollPattern (IScrollPattern source)
		{
			this.source = source;
		}

		public ScrollPatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public ScrollPatternInformation Current {
			get {
				return new ScrollPatternInformation (source.Properties);
			}
		}

		public void SetScrollPercent (double horizontalPercent, double verticalPercent)
		{
			source.SetScrollPercent (horizontalPercent, verticalPercent);
		}

		public void Scroll (ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
		{
			source.Scroll (horizontalAmount, verticalAmount);
		}

		public void ScrollHorizontal (ScrollAmount amount)
		{
			source.Scroll (amount, ScrollAmount.NoAmount);
		}

		public void ScrollVertical (ScrollAmount amount)
		{
			source.Scroll (ScrollAmount.NoAmount, amount);
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
