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
	public class AutomationTextAttribute : AutomationIdentifier
	{
#region Internal Constructor
		
		internal AutomationTextAttribute (int id, string programmaticName) :
			base (id, programmaticName)
		{
		}
		
#endregion
		
#region Public Static Methods
		
		public static AutomationTextAttribute LookupById (int id)
		{
			if (id == TextPatternIdentifiers.AnimationStyleAttribute.Id)
				return TextPatternIdentifiers.AnimationStyleAttribute;
			else if (id == TextPatternIdentifiers.BackgroundColorAttribute.Id)
				return TextPatternIdentifiers.BackgroundColorAttribute;
			else if (id == TextPatternIdentifiers.BulletStyleAttribute.Id)
				return TextPatternIdentifiers.BulletStyleAttribute;
			else if (id == TextPatternIdentifiers.CapStyleAttribute.Id)
				return TextPatternIdentifiers.CapStyleAttribute;
			else if (id == TextPatternIdentifiers.CultureAttribute.Id)
				return TextPatternIdentifiers.CultureAttribute;
			else if (id == TextPatternIdentifiers.FontNameAttribute.Id)
				return TextPatternIdentifiers.FontNameAttribute;
			else if (id == TextPatternIdentifiers.FontSizeAttribute.Id)
				return TextPatternIdentifiers.FontSizeAttribute;
			else if (id == TextPatternIdentifiers.FontWeightAttribute.Id)
				return TextPatternIdentifiers.FontWeightAttribute;
			else if (id == TextPatternIdentifiers.ForegroundColorAttribute.Id)
				return TextPatternIdentifiers.ForegroundColorAttribute;
			else if (id == TextPatternIdentifiers.HorizontalTextAlignmentAttribute.Id)
				return TextPatternIdentifiers.HorizontalTextAlignmentAttribute;
			else if (id == TextPatternIdentifiers.IndentationFirstLineAttribute.Id)
				return TextPatternIdentifiers.IndentationFirstLineAttribute;
			else if (id == TextPatternIdentifiers.IndentationLeadingAttribute.Id)
				return TextPatternIdentifiers.IndentationLeadingAttribute;
			else if (id == TextPatternIdentifiers.IndentationTrailingAttribute.Id)
				return TextPatternIdentifiers.IndentationTrailingAttribute;
			else if (id == TextPatternIdentifiers.IsHiddenAttribute.Id)
				return TextPatternIdentifiers.IsHiddenAttribute;
			else if (id == TextPatternIdentifiers.IsItalicAttribute.Id)
				return TextPatternIdentifiers.IsItalicAttribute;
			else if (id == TextPatternIdentifiers.IsReadOnlyAttribute.Id)
				return TextPatternIdentifiers.IsReadOnlyAttribute;
			else if (id == TextPatternIdentifiers.IsSubscriptAttribute.Id)
				return TextPatternIdentifiers.IsSubscriptAttribute;
			else if (id == TextPatternIdentifiers.IsSuperscriptAttribute.Id)
				return TextPatternIdentifiers.IsSuperscriptAttribute;
			else if (id == TextPatternIdentifiers.MarginBottomAttribute.Id)
				return TextPatternIdentifiers.MarginBottomAttribute;
			else if (id == TextPatternIdentifiers.MarginLeadingAttribute.Id)
				return TextPatternIdentifiers.MarginLeadingAttribute;
			else if (id == TextPatternIdentifiers.MarginTopAttribute.Id)
				return TextPatternIdentifiers.MarginTopAttribute;
			else if (id == TextPatternIdentifiers.MarginTrailingAttribute.Id)
				return TextPatternIdentifiers.MarginTrailingAttribute;
			else if (id == TextPatternIdentifiers.OutlineStylesAttribute.Id)
				return TextPatternIdentifiers.OutlineStylesAttribute;
			else if (id == TextPatternIdentifiers.OverlineColorAttribute.Id)
				return TextPatternIdentifiers.OverlineColorAttribute;
			else if (id == TextPatternIdentifiers.OverlineStyleAttribute.Id)
				return TextPatternIdentifiers.OverlineStyleAttribute;
			else if (id == TextPatternIdentifiers.StrikethroughColorAttribute.Id)
				return TextPatternIdentifiers.StrikethroughColorAttribute;
			else if (id == TextPatternIdentifiers.StrikethroughStyleAttribute.Id)
				return TextPatternIdentifiers.StrikethroughStyleAttribute;
			else if (id == TextPatternIdentifiers.TabsAttribute.Id)
				return TextPatternIdentifiers.TabsAttribute;
			else if (id == TextPatternIdentifiers.TextFlowDirectionsAttribute.Id)
				return TextPatternIdentifiers.TextFlowDirectionsAttribute;
			else if (id == TextPatternIdentifiers.UnderlineColorAttribute.Id)
				return TextPatternIdentifiers.UnderlineColorAttribute;
			else if (id == TextPatternIdentifiers.UnderlineStyleAttribute.Id)
				return TextPatternIdentifiers.UnderlineStyleAttribute;
			else
				return null;
		}
		
#endregion
	}
}
