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
	public class RangeValuePattern : BasePattern
	{
		public struct RangeValuePatternInformation
		{
			public double Value {
				get {
					throw new NotImplementedException ();
				}
			}

			public bool IsReadOnly {
				get {
					throw new NotImplementedException ();
				}
			}

			public double Maximum {
				get {
					throw new NotImplementedException ();
				}
			}

			public double Minimum {
				get {
					throw new NotImplementedException ();
				}
			}

			public double LargeChange {
				get {
					throw new NotImplementedException ();
				}
			}

			public double SmallChange {
				get {
					throw new NotImplementedException ();
				}
			}
		}

		internal RangeValuePattern ()
		{
		}

		public RangeValuePatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public RangeValuePatternInformation Current {
			get {
				throw new NotImplementedException ();
			}
		}

		public void SetValue (double value)
		{
			throw new NotImplementedException ();
		}

		public static readonly AutomationPattern Pattern;

		public static readonly AutomationProperty ValueProperty;

		public static readonly AutomationProperty IsReadOnlyProperty;

		public static readonly AutomationProperty MinimumProperty;

		public static readonly AutomationProperty MaximumProperty;

		public static readonly AutomationProperty LargeChangeProperty;

		public static readonly AutomationProperty SmallChangeProperty;
	}
}
