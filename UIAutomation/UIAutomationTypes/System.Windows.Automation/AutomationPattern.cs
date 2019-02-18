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
//      Mario Carrion <mcarrion@novell.com>
// 

using System;

namespace System.Windows.Automation
{
	public class AutomationPattern : AutomationIdentifier
	{
#region Internal Constructor
		
		internal AutomationPattern (int id, string programmaticName) :
			base (id, programmaticName)
		{
		}
		
#endregion
		
#region Public Static Methods
		
		public static AutomationPattern LookupById (int id)
		{
			if (id == ExpandCollapsePatternIdentifiers.Pattern.Id)
				return ExpandCollapsePatternIdentifiers.Pattern;
			else if (id == GridItemPatternIdentifiers.Pattern.Id)
				return GridItemPatternIdentifiers.Pattern;
			else if (id == GridPatternIdentifiers.Pattern.Id)
				return GridPatternIdentifiers.Pattern;
			else if (id == InvokePatternIdentifiers.Pattern.Id)
				return InvokePatternIdentifiers.Pattern;
			else if (id == LegacyIAccessiblePatternIdentifiers.Pattern.Id)
				return LegacyIAccessiblePatternIdentifiers.Pattern;
			else if (id == MultipleViewPatternIdentifiers.Pattern.Id)
				return MultipleViewPatternIdentifiers.Pattern;
			else if (id == RangeValuePatternIdentifiers.Pattern.Id)
				return RangeValuePatternIdentifiers.Pattern;
			else if (id == ScrollPatternIdentifiers.Pattern.Id)
				return ScrollPatternIdentifiers.Pattern;
			else if (id == SelectionItemPatternIdentifiers.Pattern.Id)
				return SelectionItemPatternIdentifiers.Pattern;
			else if (id == SelectionPatternIdentifiers.Pattern.Id)
				return SelectionPatternIdentifiers.Pattern;
			else if (id == TablePatternIdentifiers.Pattern.Id)
				return TablePatternIdentifiers.Pattern;
			else if (id == TextPatternIdentifiers.Pattern.Id)
				return TextPatternIdentifiers.Pattern;
			else if (id == TogglePatternIdentifiers.Pattern.Id)
				return TogglePatternIdentifiers.Pattern;
			else if (id == TransformPatternIdentifiers.Pattern.Id)
				return TransformPatternIdentifiers.Pattern;
			else if (id == ValuePatternIdentifiers.Pattern.Id)
				return ValuePatternIdentifiers.Pattern;
			else if (id == WindowPatternIdentifiers.Pattern.Id)
				return WindowPatternIdentifiers.Pattern;
			else if (id == ScrollItemPatternIdentifiers.Pattern.Id)
				return ScrollItemPatternIdentifiers.Pattern;
			else if (id == DockPatternIdentifiers.Pattern.Id)
				return DockPatternIdentifiers.Pattern;
			else if (id == TableItemPatternIdentifiers.Pattern.Id)
				return TableItemPatternIdentifiers.Pattern;
			else
				return null;
		}
		
#endregion
	}
}
