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
			internal RangeValuePatternInformation (RangeValueProperties properties)
			{
				Value = properties.Value;
				IsReadOnly = properties.IsReadOnly;
				Maximum = properties.Maximum;
				Minimum = properties.Minimum;
				LargeChange= properties.LargeChange;
				SmallChange = properties.SmallChange;
			}

			public double Value {
				get; private set;
			}

			public bool IsReadOnly {
				get; private set;
			}

			public double Maximum {
				get; private set;
			}

			public double Minimum {
				get; private set;
			}

			public double LargeChange {
				get; private set;
			}

			public double SmallChange {
				get; private set;
			}
		}

		private IRangeValuePattern source;

		internal RangeValuePattern (IRangeValuePattern source)
		{
			this.source = source;
		}

		public RangeValuePatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public RangeValuePatternInformation Current {
			get {
				return new RangeValuePatternInformation (source.Properties);
			}
		}

		public void SetValue (double value)
		{
			source.SetValue (value);
		}

		public static readonly AutomationPattern Pattern
			= RangeValuePatternIdentifiers.Pattern;

		public static readonly AutomationProperty ValueProperty
			= RangeValuePatternIdentifiers.ValueProperty;

		public static readonly AutomationProperty IsReadOnlyProperty
			= RangeValuePatternIdentifiers.IsReadOnlyProperty;

		public static readonly AutomationProperty MinimumProperty
			= RangeValuePatternIdentifiers.MinimumProperty;

		public static readonly AutomationProperty MaximumProperty
			= RangeValuePatternIdentifiers.MaximumProperty;

		public static readonly AutomationProperty LargeChangeProperty
			= RangeValuePatternIdentifiers.LargeChangeProperty;

		public static readonly AutomationProperty SmallChangeProperty
			= RangeValuePatternIdentifiers.SmallChangeProperty;
	}
}
