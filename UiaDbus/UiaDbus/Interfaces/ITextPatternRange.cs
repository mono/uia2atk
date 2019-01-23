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
//  Matt Guo <matt@mattguo.com>
// 

using System;
using DBus;
using System.Windows.Automation;
using System.Windows.Automation.Text;

namespace Mono.UIAutomation.UiaDbus.Interfaces
{
	[Interface (Constants.TextPatternRangeInterfaceName)]
	public interface ITextPatternRange
	{
		void AddToSelection ();

		//return the dbus path of the cloned ITextPatternRange
		string Clone ();

		bool Compare (string rangePath);
		int CompareEndpoints (TextPatternRangeEndpoint endpoint, string targetRangePath, TextPatternRangeEndpoint targetEndpoint);
		void ExpandToEnclosingUnit (TextUnit unit);

		//return the dbus path of the found ITextPatternRange
		string FindAttribute (int attributeId, object value, bool backward);
		//return the dbus path of the found ITextPatternRange
		string FindText (string text, bool backward, bool ignoreCase);

		object GetAttributeValue (int attributeId);
		Rect[] GetBoundingRectangles ();
		string[] GetChildrenPaths ();
		string GetEnclosingElementPath ();
		string GetText (int maxLength);
		int Move (TextUnit unit, int count);
		void MoveEndpointByRange (TextPatternRangeEndpoint endpoint, string targetRangePath, TextPatternRangeEndpoint targetEndpoint);
		int MoveEndpointByUnit (TextPatternRangeEndpoint endpoint, TextUnit unit, int count);
		void RemoveFromSelection ();
		void ScrollIntoView (bool alignToTop);
		void Select ();
	}
}
