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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Automation.Text;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.Source;
using DC = Mono.UIAutomation.UiaDbus;
using DCI = Mono.UIAutomation.UiaDbus.Interfaces;

namespace Mono.UIAutomation.UiaDbusSource
{
	public class UiaDbusTextPatternRange : ITextPatternRange
	{
		private DCI.ITextPatternRange range;
		private UiaDbusTextPattern parent;
		private string path;

		public UiaDbusTextPatternRange (DCI.ITextPatternRange range,
		                                UiaDbusTextPattern parent,
		                                string path)
		{
			this.range = range;
			this.parent = parent;
			this.path = path;
		}

#region ITextPatternRange members

		public void AddToSelection ()
		{
			range.AddToSelection ();
		}

		public ITextPatternRange Clone ()
		{
			string path = range.Clone ();
			return parent.GetTextPatternRange (path);
		}

		public bool Compare (ITextPatternRange range)
		{
			UiaDbusTextPatternRange r = range as UiaDbusTextPatternRange;
			if (r == null)
				throw new Exception ("Require a UiaDbusTextPatternRange object");
			return this.range.Compare (r.path);
		}

		public int CompareEndpoints (TextPatternRangeEndpoint endpoint, ITextPatternRange targetRange, TextPatternRangeEndpoint targetEndpoint)
		{
			UiaDbusTextPatternRange r = targetRange as UiaDbusTextPatternRange;
			if (r == null)
				throw new Exception ("Require a UiaDbusTextPatternRange object");
			return this.range.CompareEndpoints (endpoint, r.path, targetEndpoint);
		}

		public void ExpandToEnclosingUnit (TextUnit unit)
		{
			range.ExpandToEnclosingUnit (unit);
		}

		public ITextPatternRange FindAttribute (AutomationTextAttribute attribute, object value, bool backward)
		{
			//todo Need object transformation
			string path = range.FindAttribute (attribute.Id, value, backward);
			return parent.GetTextPatternRange (path);
		}

		public ITextPatternRange FindText (string text, bool backward, bool ignoreCase)
		{
			string path = range.FindText (text, backward, ignoreCase);
			return parent.GetTextPatternRange (path);
		}

		public object GetAttributeValue (AutomationTextAttribute attribute)
		{
			//todo Need object transformation
			return range.GetAttributeValue (attribute.Id);
		}

		public Rect[] GetBoundingRectangles ()
		{
			var rects = range.GetBoundingRectangles ();
			List<Rect> ret = new List<Rect> (rects.Length);
			foreach (var rect in rects)
				ret.Add (rect.ToSWRect ());
			return ret.ToArray ();
		}

		public IElement[] GetChildren ()
		{
			var childrenPaths = range.GetChildrenPaths ();
			List<IElement> elements = new List<IElement> (childrenPaths.Length);
			foreach (string path in childrenPaths)
				elements.Add (parent.GetElement (path));
			return elements.ToArray ();
		}

		public IElement GetEnclosingElement ()
		{
			string elementPath = range.GetEnclosingElementPath ();
			if (string.IsNullOrEmpty (elementPath))
				return null;
			else
				return parent.GetElement (elementPath);
		}

		public string GetText (int maxLength)
		{
			return range.GetText (maxLength);
		}

		public int Move (TextUnit unit, int count)
		{
			return range.Move (unit, count);
		}

		public void MoveEndpointByRange (TextPatternRangeEndpoint endpoint, ITextPatternRange targetRange, TextPatternRangeEndpoint targetEndpoint)
		{
			UiaDbusTextPatternRange r = targetRange as UiaDbusTextPatternRange;
			if (r == null)
				throw new Exception ("Require a UiaDbusTextPatternRange object");
			range.MoveEndpointByRange (endpoint, r.path, targetEndpoint);
		}

		public int MoveEndpointByUnit (TextPatternRangeEndpoint endpoint, TextUnit unit, int count)
		{
			return range.MoveEndpointByUnit (endpoint, unit, count);
		}

		public void RemoveFromSelection ()
		{
			range.RemoveFromSelection ();
		}

		public void ScrollIntoView (bool alignToTop)
		{
			range.ScrollIntoView (alignToTop);
		}

		public void Select ()
		{
			range.Select ();
		}

#endregion
	}
}
