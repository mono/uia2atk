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
	public static class SelectionPatternIdentifiers
	{
#region Constructor
		private const int PatternId = 10001;
		private const int CanSelectMultiplePropertyId = 30060;
		private const int IsSelectionRequiredPropertyId = 30061;
		private const int SelectionPropertyId = 30059;
		private const int InvalidatedEventId = 20013;
		
		static SelectionPatternIdentifiers ()
		{
			Pattern =
				new AutomationPattern (PatternId,
				                       "SelectionPatternIdentifiers.Pattern");
			CanSelectMultipleProperty =
				new AutomationProperty (CanSelectMultiplePropertyId,
				                        "SelectionPatternIdentifiers.CanSelectMultipleProperty");
			IsSelectionRequiredProperty =
				new AutomationProperty (IsSelectionRequiredPropertyId,
				                        "SelectionPatternIdentifiers.IsSelectionRequiredProperty");
			SelectionProperty =
				new AutomationProperty (SelectionPropertyId,
				                        "SelectionPatternIdentifiers.SelectionProperty");
			InvalidatedEvent =
				new AutomationEvent (InvalidatedEventId,
				                     "SelectionPatternIdentifiers.InvalidatedEvent");
		}
		
#endregion
		
#region Public Fields
		
		public static readonly AutomationPattern Pattern;
		public static readonly AutomationProperty CanSelectMultipleProperty;
		public static readonly AutomationProperty IsSelectionRequiredProperty;
		public static readonly AutomationProperty SelectionProperty;
		public static readonly AutomationEvent InvalidatedEvent;
		
#endregion
	}
}
