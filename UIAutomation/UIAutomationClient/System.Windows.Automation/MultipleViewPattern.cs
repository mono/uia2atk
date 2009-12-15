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
	public class MultipleViewPattern : BasePattern
	{
		public struct MultipleViewPatternInformation
		{
			private bool cache;
			private MultipleViewPattern pattern;

			internal MultipleViewPatternInformation (MultipleViewPattern pattern, bool cache)
			{
				this.pattern = pattern;
				this.cache = cache;
			}

			public int CurrentView {
				get { return (int) pattern.element.GetPropertyValue (CurrentViewProperty, cache); }
			}

			public int [] GetSupportedViews ()
			{
				return (int []) pattern.element.GetPropertyValue (SupportedViewsProperty, cache);
			}
		}

		private AutomationElement element;
		private bool cached;
		private MultipleViewPatternInformation currentInfo;
		private MultipleViewPatternInformation cachedInfo;

		internal MultipleViewPattern ()
		{
		}

		internal MultipleViewPattern (IMultipleViewPattern source, AutomationElement element, bool cached)
		{
			this.element = element;
			this.cached = cached;
			this.Source = source;
			currentInfo = new MultipleViewPatternInformation (this, false);
			if (cached)
				cachedInfo = new MultipleViewPatternInformation (this, true);
		}

		internal IMultipleViewPattern Source { get; private set; }

		public MultipleViewPatternInformation Cached {
			get {
				if (!cached)
					throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
				return cachedInfo;
			}
		}

		public MultipleViewPatternInformation Current {
			get {
				return currentInfo;
			}
		}

		public string GetViewName (int viewId)
		{
			return Source.GetViewName (viewId);
		}

		public void SetCurrentView (int viewId)
		{
			Source.SetCurrentView (viewId);
		}

		public static readonly AutomationPattern Pattern =
			MultipleViewPatternIdentifiers.Pattern;

		public static readonly AutomationProperty CurrentViewProperty =
			MultipleViewPatternIdentifiers.CurrentViewProperty;

		public static readonly AutomationProperty SupportedViewsProperty =
			MultipleViewPatternIdentifiers.SupportedViewsProperty;
	}
}
