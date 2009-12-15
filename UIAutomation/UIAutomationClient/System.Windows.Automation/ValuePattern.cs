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
	public class ValuePattern : BasePattern
	{
		public struct ValuePatternInformation
		{
			private bool cache;
			private ValuePattern pattern;

			internal ValuePatternInformation (ValuePattern pattern, bool cache)
			{
				this.pattern = pattern;
				this.cache = cache;
			}

			public string Value {
				get { return (string) pattern.element.GetPropertyValue (ValueProperty, cache); }
			}

			public bool IsReadOnly {
				get { return (bool) pattern.element.GetPropertyValue (IsReadOnlyProperty, cache); }
			}
		}

		private AutomationElement element;
		private bool cached;
		private ValuePatternInformation currentInfo;
		private ValuePatternInformation cachedInfo;

		internal ValuePattern ()
		{
		}

		internal ValuePattern (IValuePattern source, AutomationElement element, bool cached)
		{
			this.element = element;
			this.cached = cached;
			this.Source = source;
			currentInfo = new ValuePatternInformation (this, false);
			if (cached)
				cachedInfo = new ValuePatternInformation (this, true);
		}

		internal IValuePattern Source { get; private set; }

		public ValuePatternInformation Cached {
			get {
				if (!cached)
					throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
				return cachedInfo;
			}
		}

		public ValuePatternInformation Current {
			get {
				return currentInfo;
			}
		}

		public void SetValue (string value)
		{
			Source.SetValue (value);
		}

		public static readonly AutomationPattern Pattern =
			ValuePatternIdentifiers.Pattern;

		public static readonly AutomationProperty ValueProperty =
			ValuePatternIdentifiers.ValueProperty;

		public static readonly AutomationProperty IsReadOnlyProperty =
			ValuePatternIdentifiers.IsReadOnlyProperty;
	}
}
