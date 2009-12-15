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
			private bool cache;
			private ScrollPattern pattern;

			internal ScrollPatternInformation (ScrollPattern pattern, bool cache)
			{
				this.pattern = pattern;
				this.cache = cache;
			}

			public double HorizontalScrollPercent {
				get { return (double) pattern.element.GetPropertyValue (HorizontalScrollPercentProperty, cache); }
			}

			public double VerticalScrollPercent {
				get { return (double) pattern.element.GetPropertyValue (VerticalScrollPercentProperty, cache); }
			}

			public double HorizontalViewSize {
				get { return (double) pattern.element.GetPropertyValue (HorizontalViewSizeProperty, cache); }
			}

			public double VerticalViewSize {
				get { return (double) pattern.element.GetPropertyValue (VerticalViewSizeProperty, cache); }
			}

			public bool HorizontallyScrollable {
				get { return (bool) pattern.element.GetPropertyValue (HorizontallyScrollableProperty, cache); }
			}

			public bool VerticallyScrollable {
				get { return (bool) pattern.element.GetPropertyValue (VerticallyScrollableProperty, cache); }
			}
		}

		private AutomationElement element;
		private bool cached;
		private ScrollPatternInformation currentInfo;
		private ScrollPatternInformation cachedInfo;

		internal ScrollPattern ()
		{
		}

		internal ScrollPattern (IScrollPattern source, AutomationElement element, bool cached)
		{
			this.element = element;
			this.cached = cached;
			this.Source = source;
			currentInfo = new ScrollPatternInformation (this, false);
			if (cached)
				cachedInfo = new ScrollPatternInformation (this, true);
		}

		internal IScrollPattern Source { get; private set; }

		public ScrollPatternInformation Cached {
			get {
				if (!cached)
					throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
				return cachedInfo;
			}
		}

		public ScrollPatternInformation Current {
			get {
				return currentInfo;
			}
		}

		public void SetScrollPercent (double horizontalPercent, double verticalPercent)
		{
			Source.SetScrollPercent (horizontalPercent, verticalPercent);
		}

		public void Scroll (ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
		{
			Source.Scroll (horizontalAmount, verticalAmount);
		}

		public void ScrollHorizontal (ScrollAmount amount)
		{
			Source.Scroll (amount, ScrollAmount.NoAmount);
		}

		public void ScrollVertical (ScrollAmount amount)
		{
			Source.Scroll (ScrollAmount.NoAmount, amount);
		}

		public static readonly AutomationPattern Pattern =
			ScrollPatternIdentifiers.Pattern;

		public static readonly AutomationProperty HorizontalScrollPercentProperty =
			ScrollPatternIdentifiers.HorizontalScrollPercentProperty;

		public static readonly AutomationProperty VerticalScrollPercentProperty =
			ScrollPatternIdentifiers.VerticalScrollPercentProperty;

		public static readonly AutomationProperty HorizontalViewSizeProperty =
			ScrollPatternIdentifiers.HorizontalViewSizeProperty;

		public static readonly AutomationProperty VerticalViewSizeProperty =
			ScrollPatternIdentifiers.VerticalViewSizeProperty;

		public static readonly AutomationProperty HorizontallyScrollableProperty =
			ScrollPatternIdentifiers.HorizontallyScrollableProperty;

		public static readonly AutomationProperty VerticallyScrollableProperty =
			ScrollPatternIdentifiers.VerticallyScrollableProperty;

		public const double NoScroll = -1;
	}
}
