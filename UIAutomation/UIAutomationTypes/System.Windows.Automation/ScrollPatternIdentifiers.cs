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
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;

namespace System.Windows.Automation
{
	public static class ScrollPatternIdentifiers
	{
#region Constructor
		private const int PatternId = 10004;
		private const int HorizontallyScrollablePropertyId = 30057;
		private const int HorizontalScrollPercentPropertyId = 30053;
		private const int HorizontalViewSizePropertyId = 30054;
		private const int VerticallyScrollablePropertyId = 30058;
		private const int VerticalScrollPercentPropertyId = 30055;
		private const int VerticalViewSizePropertyId = 30056;
		
		static ScrollPatternIdentifiers ()
		{
			Pattern =
				new AutomationPattern (PatternId,
				                       "ScrollPatternIdentifiers.Pattern");
			HorizontallyScrollableProperty = 
				new AutomationProperty (HorizontallyScrollablePropertyId, 
				                        "ScrollPatternIdentifiers.HorizontallyScrollableProperty");
			HorizontalScrollPercentProperty = 
				new AutomationProperty (HorizontalScrollPercentPropertyId, 
				                        "ScrollPatternIdentifiers.HorizontalScrollPercentProperty");
			HorizontalViewSizeProperty = 
				new AutomationProperty (HorizontalViewSizePropertyId,
				                        "ScrollPatternIdentifiers.HorizontalViewSizeProperty");
			VerticallyScrollableProperty =
				new AutomationProperty (VerticallyScrollablePropertyId,
				                        "ScrollPatternIdentifiers.VerticallyScrollableProperty");
			VerticalScrollPercentProperty = 
				new AutomationProperty (VerticalScrollPercentPropertyId,
				                        "ScrollPatternIdentifiers.VerticalScrollPercentProperty");
			VerticalViewSizeProperty =
				new AutomationProperty (VerticalViewSizePropertyId,
				                        "ScrollPatternIdentifiers.VerticalViewSizeProperty");
		}
		
#endregion
		
#region Public Fields
		
		public static readonly AutomationPattern Pattern;
		
		public static readonly AutomationProperty HorizontallyScrollableProperty;
		
		public static readonly AutomationProperty HorizontalScrollPercentProperty;
		
		public static readonly AutomationProperty HorizontalViewSizeProperty;
		
		public static readonly AutomationProperty VerticallyScrollableProperty;
		
		public static readonly AutomationProperty VerticalScrollPercentProperty;
		
		public static readonly AutomationProperty VerticalViewSizeProperty;
		
		public const double NoScroll = -1;
		
#endregion
	}
}

