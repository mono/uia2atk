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
	public static class WindowPatternIdentifiers
	{
#region Constructor
		private const int PatternId = 10009;
		private const int CanMaximizePropertyId = 30073;
		private const int CanMinimizePropertyId = 30074;
		private const int IsModalPropertyId = 30077;
		private const int IsTopmostPropertyId = 30078;
		private const int WindowInteractionStatePropertyId = 30076;
		private const int WindowVisualStatePropertyId = 30075;
		private const int WindowClosedEventId = 20017;
		private const int WindowOpenedEventId = 20016;
		
		static WindowPatternIdentifiers ()
		{
			Pattern =
				new AutomationPattern (PatternId,
				                       "WindowPatternIdentifiers.Pattern");
			CanMaximizeProperty =
				new AutomationProperty (CanMaximizePropertyId,
				                        "WindowPatternIdentifiers.CanMaximizeProperty");
			CanMinimizeProperty =
				new AutomationProperty (CanMinimizePropertyId,
				                        "WindowPatternIdentifiers.CanMinimizeProperty");
			IsModalProperty =
				new AutomationProperty (IsModalPropertyId,
				                        "WindowPatternIdentifiers.IsModalProperty");
			IsTopmostProperty =
				new AutomationProperty (IsTopmostPropertyId,
				                        "WindowPatternIdentifiers.IsTopmostProperty");
			WindowInteractionStateProperty =
				new AutomationProperty (WindowInteractionStatePropertyId,
				                        "WindowPatternIdentifiers.WindowInteractionStateProperty");
			WindowVisualStateProperty =
				new AutomationProperty (WindowVisualStatePropertyId,
				                        "WindowPatternIdentifiers.WindowVisualStateProperty");
			WindowClosedEvent =
				new AutomationEvent (WindowClosedEventId,
				                     "WindowPatternIdentifiers.WindowClosedProperty");
			WindowOpenedEvent =
				new AutomationEvent (WindowOpenedEventId,
				                     "WindowPatternIdentifiers.WindowOpenedProperty");
		}
		
#endregion
		
#region Public Fields
		
		public static readonly AutomationProperty CanMaximizeProperty;
		
		public static readonly AutomationProperty CanMinimizeProperty;
		
		public static readonly AutomationProperty IsModalProperty;
		
		public static readonly AutomationProperty IsTopmostProperty;
		
		public static readonly AutomationPattern Pattern;
		
		public static readonly AutomationEvent WindowClosedEvent;
		
		public static readonly AutomationProperty WindowInteractionStateProperty;
		
		public static readonly AutomationEvent WindowOpenedEvent;
		
		public static readonly AutomationProperty WindowVisualStateProperty;
		
#endregion
	}
}
