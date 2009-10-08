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
			internal ValuePatternInformation (ValueProperties properties)
			{
				Value = properties.Value;
				IsReadOnly = properties.IsReadOnly;
			}

			public string Value {
				get; private set;
			}

			public bool IsReadOnly {
				get; private set;
			}
		}

		private IValuePattern source;

		internal ValuePattern (IValuePattern source)
		{
			this.source = source;
		}

		public ValuePatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public ValuePatternInformation Current {
			get {
				return new ValuePatternInformation (source.Properties);
			}
		}

		public void SetValue (string value)
		{
			source.SetValue (value);
		}

		public static readonly AutomationPattern Pattern =
			ValuePatternIdentifiers.Pattern;

		public static readonly AutomationProperty ValueProperty =
			ValuePatternIdentifiers.ValueProperty;

		public static readonly AutomationProperty IsReadOnlyProperty =
			ValuePatternIdentifiers.IsReadOnlyProperty;
	}
}
