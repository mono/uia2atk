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
			try {
				range.AddToSelection ();
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public ITextPatternRange Clone ()
		{
			string path = null;
			try {
				path = range.Clone ();
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
			return parent.GetTextPatternRange (path);
		}

		public bool Compare (ITextPatternRange range)
		{
			UiaDbusTextPatternRange r = range as UiaDbusTextPatternRange;
			if (r == null)
				throw new Exception ("Require a UiaDbusTextPatternRange object");
			try {
				return this.range.Compare (r.path);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public int CompareEndpoints (TextPatternRangeEndpoint endpoint, ITextPatternRange targetRange, TextPatternRangeEndpoint targetEndpoint)
		{
			UiaDbusTextPatternRange r = targetRange as UiaDbusTextPatternRange;
			if (r == null)
				throw new Exception ("Require a UiaDbusTextPatternRange object");
			try {
				return this.range.CompareEndpoints (endpoint, r.path, targetEndpoint);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public void ExpandToEnclosingUnit (TextUnit unit)
		{
			try {
				range.ExpandToEnclosingUnit (unit);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public ITextPatternRange FindAttribute (AutomationTextAttribute attribute, object value, bool backward)
		{
			//todo Need object transformation
			string path = null;
			try {
				path = range.FindAttribute (attribute.Id, value, backward);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
			return parent.GetTextPatternRange (path);
		}

		public ITextPatternRange FindText (string text, bool backward, bool ignoreCase)
		{
			string path = null;
			try {
				path = range.FindText (text, backward, ignoreCase);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
			return parent.GetTextPatternRange (path);
		}

		public object GetAttributeValue (AutomationTextAttribute attribute)
		{
			try {
				//todo Need object transformation
				return range.GetAttributeValue (attribute.Id);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public Rect[] GetBoundingRectangles ()
		{
			DC.Rect [] rects = null;
			try {
				rects = range.GetBoundingRectangles ();
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
			List<Rect> ret = new List<Rect> (rects.Length);
			foreach (var rect in rects)
				ret.Add (rect.ToSWRect ());
			return ret.ToArray ();
		}

		public IElement[] GetChildren ()
		{
			string [] childrenPaths;
			try {
				childrenPaths = range.GetChildrenPaths ();
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
			List<IElement> elements = new List<IElement> (childrenPaths.Length);
			foreach (string path in childrenPaths)
				elements.Add (parent.GetElement (path));
			return elements.ToArray ();
		}

		public IElement GetEnclosingElement ()
		{
			string elementPath = null;
			try {
				elementPath = range.GetEnclosingElementPath ();
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
			if (string.IsNullOrEmpty (elementPath))
				return null;
			else
				return parent.GetElement (elementPath);
		}

		public string GetText (int maxLength)
		{
			try {
				return range.GetText (maxLength);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public int Move (TextUnit unit, int count)
		{
			try {
				return range.Move (unit, count);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public void MoveEndpointByRange (TextPatternRangeEndpoint endpoint, ITextPatternRange targetRange, TextPatternRangeEndpoint targetEndpoint)
		{
			UiaDbusTextPatternRange r = targetRange as UiaDbusTextPatternRange;
			if (r == null)
				throw new Exception ("Require a UiaDbusTextPatternRange object");
			try {
				range.MoveEndpointByRange (endpoint, r.path, targetEndpoint);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public int MoveEndpointByUnit (TextPatternRangeEndpoint endpoint, TextUnit unit, int count)
		{
			try {
				return range.MoveEndpointByUnit (endpoint, unit, count);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public void RemoveFromSelection ()
		{
			try {
				range.RemoveFromSelection ();
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public void ScrollIntoView (bool alignToTop)
		{
			try {
				range.ScrollIntoView (alignToTop);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public void Select ()
		{
			try {
				range.Select ();
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

#endregion
	}
}
