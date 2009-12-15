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
	public class SelectionPattern : BasePattern
	{
		public struct SelectionPatternInformation
		{
			private bool cache;
			private SelectionPattern pattern;

			internal SelectionPatternInformation (SelectionPattern pattern, bool cache)
			{
				this.pattern = pattern;
				this.cache = cache;
			}

			public bool CanSelectMultiple {
				get { return (bool) pattern.element.GetPropertyValue (CanSelectMultipleProperty, cache); }
			}

			public bool IsSelectionRequired {
				get { return (bool) pattern.element.GetPropertyValue (IsSelectionRequiredProperty, cache); }
			}

			public AutomationElement [] GetSelection ()
			{
				return (AutomationElement []) pattern.element.GetPropertyValue (SelectionProperty, cache);
			}
		}

		private AutomationElement element;
		private bool cached;
		private SelectionPatternInformation currentInfo;
		private SelectionPatternInformation cachedInfo;

		internal SelectionPattern ()
		{
		}

		internal SelectionPattern (ISelectionPattern source, AutomationElement element, bool cached)
		{
			this.element = element;
			this.cached = cached;
			this.Source = source;
			currentInfo = new SelectionPatternInformation (this, false);
			if (cached)
				cachedInfo = new SelectionPatternInformation (this, true);
		}

		internal ISelectionPattern Source { get; private set; }

		public SelectionPatternInformation Cached {
			get {
				if (!cached)
					throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
				return cachedInfo;
			}
		}

		public SelectionPatternInformation Current {
			get {
				return currentInfo;
			}
		}

		public static readonly AutomationPattern Pattern =
			SelectionPatternIdentifiers.Pattern;

		public static readonly AutomationProperty SelectionProperty =
			SelectionPatternIdentifiers.SelectionProperty;

		public static readonly AutomationProperty CanSelectMultipleProperty =
			SelectionPatternIdentifiers.CanSelectMultipleProperty;

		public static readonly AutomationProperty IsSelectionRequiredProperty =
			SelectionPatternIdentifiers.IsSelectionRequiredProperty;

		public static readonly AutomationEvent InvalidatedEvent =
			SelectionPatternIdentifiers.InvalidatedEvent;
	}
}
