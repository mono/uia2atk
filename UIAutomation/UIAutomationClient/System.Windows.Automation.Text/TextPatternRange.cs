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
//      Brad Taylor <brad@getcoded.net>
//      Mike Gorse <mgorse@novell.com>
//

using System;
using System.Runtime.InteropServices;
using Mono.UIAutomation.Source;

namespace System.Windows.Automation.Text
{
	public class TextPatternRange
	{
#region Private Fields
		private ITextPatternRange source;
#endregion

#region Public Properties
		public TextPattern TextPattern {
			get; private set;
		}

		internal ITextPatternRange Source {
			get {
				return source;
			}
		}
#endregion

#region Constructor
		private TextPatternRange ()
		{
		}

		internal TextPatternRange (TextPattern textPattern, ITextPatternRange source)
		{
			TextPattern = textPattern;
			this.source = source;
		}
#endregion

#region Public Methods
		public void AddToSelection ()
		{
			source.AddToSelection ();
		}

		public TextPatternRange Clone ()
		{
			return new TextPatternRange (TextPattern, source.Clone ());
		}

		public bool Compare (TextPatternRange range)
		{
			return source.Compare (range.Source);
		}

		public int CompareEndpoints (TextPatternRangeEndpoint endpoint,
		                             TextPatternRange targetRange,
		                             TextPatternRangeEndpoint targetEndpoint)
		{
			return source.CompareEndpoints (endpoint, targetRange.Source, targetEndpoint);
		}

		public void ExpandToEnclosingUnit (TextUnit unit)
		{
			source.ExpandToEnclosingUnit (unit);
		}

		public TextPatternRange FindAttribute (AutomationTextAttribute attribute,
		                                       Object @value, bool backward)
		{
			ITextPatternRange range = source.FindAttribute (attribute, value, backward);
			if (range == null)
				return null;
			return new TextPatternRange (TextPattern, range);
		}

		public TextPatternRange FindText (string text, bool backward,
		                                  bool ignoreCase)
		{
			ITextPatternRange range = source.FindText (text, backward, ignoreCase);
			if (range == null)
				return null;
			return new TextPatternRange (TextPattern, range);
		}

		public Object GetAttributeValue (AutomationTextAttribute attribute)
		{
			return source.GetAttributeValue (attribute);
		}

		public Rect[] GetBoundingRectangles ()
		{
			return source.GetBoundingRectangles ();
		}

		public AutomationElement[] GetChildren ()
		{
			return SourceManager.GetOrCreateAutomationElements (source.GetChildren ());
		}

		public AutomationElement GetEnclosingElement ()
		{
			return SourceManager.GetOrCreateAutomationElement (source.GetEnclosingElement ());
		}

		public string GetText (int maxLength)
		{
			return source.GetText (maxLength);
		}

		public int Move (TextUnit unit, int count)
		{
			return source.Move (unit, count);
		}

		public void MoveEndpointByRange (TextPatternRangeEndpoint endpoint,
		                                 TextPatternRange targetRange,
		                                 TextPatternRangeEndpoint targetEndpoint)
		{
			source.MoveEndpointByRange (endpoint, targetRange.Source, targetEndpoint);
		}

		public int MoveEndpointByUnit (TextPatternRangeEndpoint endpoint,
		                               TextUnit unit, int count)
		{
			return source.MoveEndpointByUnit (endpoint, unit, count);
		}

		public void RemoveFromSelection ()
		{
			source.RemoveFromSelection ();
		}

		public void ScrollIntoView (bool alignToTop)
		{
			source.ScrollIntoView (alignToTop);
		}

		public void Select ()
		{
			source.Select ();
		}
#endregion
	}
}
