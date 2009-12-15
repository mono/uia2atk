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
	public class RangeValuePattern : BasePattern
	{
		public struct RangeValuePatternInformation
		{
			private bool cache;
			private RangeValuePattern pattern;

			internal RangeValuePatternInformation (RangeValuePattern pattern, bool cache)
			{
				this.pattern = pattern;
				this.cache = cache;
			}

			public double Value {
				get { return (double) pattern.element.GetPropertyValue (ValueProperty, cache); }
			}

			public bool IsReadOnly {
				get { return (bool) pattern.element.GetPropertyValue (IsReadOnlyProperty, cache); }
			}

			public double Maximum {
				get { return (double) pattern.element.GetPropertyValue (MaximumProperty, cache); }
			}

			public double Minimum {
				get { return (double) pattern.element.GetPropertyValue (MinimumProperty, cache); }
			}

			public double LargeChange {
				get { return (double) pattern.element.GetPropertyValue (LargeChangeProperty, cache); }
			}

			public double SmallChange {
				get { return (double) pattern.element.GetPropertyValue (SmallChangeProperty, cache); }
			}
		}

		private AutomationElement element;
		private bool cached;
		private RangeValuePatternInformation currentInfo;
		private RangeValuePatternInformation cachedInfo;

		internal RangeValuePattern ()
		{
		}

		internal RangeValuePattern (IRangeValuePattern source, AutomationElement element, bool cached)
		{
			this.element = element;
			this.cached = cached;
			this.Source = source;
			currentInfo = new RangeValuePatternInformation (this, false);
			if (cached)
				cachedInfo = new RangeValuePatternInformation (this, true);
		}

		internal IRangeValuePattern Source { get; private set; }

		public RangeValuePatternInformation Cached {
			get {
				if (!cached)
					throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
				return cachedInfo;
			}
		}

		public RangeValuePatternInformation Current {
			get {
				return currentInfo;
			}
		}

		public void SetValue (double value)
		{
			Source.SetValue (value);
		}

		public static readonly AutomationPattern Pattern =
			RangeValuePatternIdentifiers.Pattern;

		public static readonly AutomationProperty ValueProperty =
			RangeValuePatternIdentifiers.ValueProperty;

		public static readonly AutomationProperty IsReadOnlyProperty =
			RangeValuePatternIdentifiers.IsReadOnlyProperty;

		public static readonly AutomationProperty MinimumProperty =
			RangeValuePatternIdentifiers.MinimumProperty;

		public static readonly AutomationProperty MaximumProperty =
			RangeValuePatternIdentifiers.MaximumProperty;

		public static readonly AutomationProperty LargeChangeProperty =
			RangeValuePatternIdentifiers.LargeChangeProperty;

		public static readonly AutomationProperty SmallChangeProperty =
			RangeValuePatternIdentifiers.SmallChangeProperty;
	}
}
