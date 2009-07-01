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
//      Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Text;

namespace Mono.UIAutomation.Source
{
	public interface ITextPatternRange
	{
		void AddToSelection ();
		ITextPatternRange Clone ();
		bool Compare (ITextPatternRange range);
		int CompareEndpoints (TextPatternRangeEndpoint endpoint, ITextPatternRange targetRange, TextPatternRangeEndpoint targetEndpoint);
		void ExpandToEnclosingUnit (TextUnit unit);
		ITextPatternRange FindAttribute (AutomationTextAttribute attribute, object value, bool backward);
		ITextPatternRange FindText (string text, bool backward, bool ignoreCase);
		object GetAttributeValue (AutomationTextAttribute attribute);
		Rect[] GetBoundingRectangles ();
		IElement[] GetChildren ();
		IElement GetEnclosingElement ();
		string GetText (int maxLength);
		int Move (TextUnit unit, int count);
		void MoveEndpointByRange (TextPatternRangeEndpoint endpoint, ITextPatternRange targetRange, TextPatternRangeEndpoint targetEndpoint);
		int MoveEndpointByUnit (TextPatternRangeEndpoint endpoint, TextUnit unit, int count);
		void RemoveFromSelection ();
		void ScrollIntoView (bool alignToTop);
		void Select ();
	}
}
