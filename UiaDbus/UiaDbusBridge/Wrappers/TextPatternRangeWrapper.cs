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
using SW = System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Automation.Text;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using DC = Mono.UIAutomation.UiaDbus;
using Mono.UIAutomation.UiaDbus.Interfaces;

using NDesk.DBus;

namespace Mono.UIAutomation.UiaDbusBridge.Wrappers
{
	public class TextPatternRangeWrapper : ITextPatternRange
	{
#region Private Fields

		private ITextRangeProvider provider;
		private TextPatternWrapper parent;

#endregion

#region Constructor

		public TextPatternRangeWrapper (ITextRangeProvider provider,
		                                TextPatternWrapper parent)
		{
			this.provider = provider;
			this.parent = parent;
		}

#endregion

#region ITextPattern Members

		public void AddToSelection ()
		{
			provider.AddToSelection ();
		}

		public string Clone ()
		{
			ITextRangeProvider ret = provider.Clone ();
			return parent.GetOrCreateTextRange (ret);
		}

		public bool Compare (string rangePath)
		{
			ITextRangeProvider p = parent.GetTextRangeFromPath (rangePath).Provider;
			return provider.Compare (p);
		}

		public int CompareEndpoints (TextPatternRangeEndpoint endpoint, string targetRangePath, TextPatternRangeEndpoint targetEndpoint)
		{
			ITextRangeProvider target = parent.GetTextRangeFromPath (targetRangePath).Provider;
			return provider.CompareEndpoints (endpoint, target, targetEndpoint);
		}

		public void ExpandToEnclosingUnit (TextUnit unit)
		{
			provider.ExpandToEnclosingUnit (unit);
		}

		public string FindAttribute (int attributeId, object value, bool backward)
		{
			//todo need object transformation here.
			ITextRangeProvider ret = provider.FindAttribute (attributeId, value, backward);
			return parent.GetOrCreateTextRange (ret);
		}

		public string FindText (string text, bool backward, bool ignoreCase)
		{
			ITextRangeProvider ret = provider.FindText (text, backward, ignoreCase);
			return parent.GetOrCreateTextRange (ret);
		}

		public object GetAttributeValue (int attributeId)
		{
			//todo need object transformation here.
			return provider.GetAttributeValue (attributeId);
		}

		public DC.Rect[] GetBoundingRectangles ()
		{
			var rects = provider.GetBoundingRectangles ();
			List<DC.Rect> ret = new List<DC.Rect> (rects.Length);
			foreach (var rect in rects)
				ret.Add (new DC.Rect (rect));
			return ret.ToArray ();
		}

		public string[] GetChildrenPaths ()
		{
			var children = provider.GetChildren ();
			List<string> childrenPaths = new List<string> (children.Length);
			foreach (var child in children)
			{
				string elementPath =
					AutomationBridge.Instance.FindWrapperByProvider (child).Path;
				childrenPaths.Add (elementPath);
			}
			return childrenPaths.ToArray ();
		}

		public string GetEnclosingElementPath ()
		{
			var element = provider.GetEnclosingElement ();
			if (element == null)
				return string.Empty;
			else
				return AutomationBridge.Instance.FindWrapperByProvider (element).Path;
		}

		public string GetText (int maxLength)
		{
			return provider.GetText (maxLength);
		}

		public int Move (TextUnit unit, int count)
		{
			return provider.Move (unit, count);
		}

		public void MoveEndpointByRange (TextPatternRangeEndpoint endpoint, string targetRangePath, TextPatternRangeEndpoint targetEndpoint)
		{
			ITextRangeProvider target = parent.GetTextRangeFromPath (targetRangePath).Provider;
			provider.MoveEndpointByRange (endpoint, target, targetEndpoint);
		}

		public int MoveEndpointByUnit (TextPatternRangeEndpoint endpoint, TextUnit unit, int count)
		{
			return provider.MoveEndpointByUnit (endpoint, unit, count);
		}

		public void RemoveFromSelection ()
		{
			provider.RemoveFromSelection ();
		}

		public void ScrollIntoView (bool alignToTop)
		{
			provider.ScrollIntoView (alignToTop);
		}

		public void Select ()
		{
			provider.Select ();
		}

#endregion
	}
}
