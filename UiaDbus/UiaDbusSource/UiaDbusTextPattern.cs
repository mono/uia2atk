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
using Mono.UIAutomation.Services;
using Mono.UIAutomation.Source;
using DC = Mono.UIAutomation.UiaDbus;
using DCI = Mono.UIAutomation.UiaDbus.Interfaces;
using NDesk.DBus;

namespace Mono.UIAutomation.UiaDbusSource
{
	public class UiaDbusTextPattern : ITextPattern
	{
		private DCI.ITextPattern pattern;
		private string busName;
		private UiaDbusAutomationSource source;

		public UiaDbusTextPattern (DCI.ITextPattern pattern, string busName,
		                           UiaDbusAutomationSource source)
		{
			this.pattern = pattern;
			this.busName = busName;
			this.source = source;
		}

		internal UiaDbusTextPatternRange GetTextPatternRange (string path)
		{
			if (string.IsNullOrEmpty (path))
			    return null;
			DCI.ITextPatternRange textPatternRange =
				Bus.Session.GetObject<DCI.ITextPatternRange> (busName, new ObjectPath (path));
			return new UiaDbusTextPatternRange (textPatternRange, this, path);
		}

		internal UiaDbusElement GetElement (string elementPath)
		{
			return source.GetOrCreateElement (busName, elementPath);
		}

#region ITextPattern members

		public ITextPatternRange DocumentRange {
			get {
				string rangePath = pattern.DocumentRangePath;
				return GetTextPatternRange (rangePath);
			}
		}

		public SupportedTextSelection SupportedTextSelection {
			get {
				return pattern.SupportedTextSelection;
			}
		}

		public ITextPatternRange[] GetSelection ()
		{
			var selection = pattern.GetSelectionPaths ();
			List<ITextPatternRange> ranges = new List<ITextPatternRange> (selection.Length);
			foreach (string path in selection) {
				ranges.Add (GetTextPatternRange (path));
			}
			return ranges.ToArray ();
		}

		public ITextPatternRange[] GetVisibleRanges ()
		{
			var visibleRanges = pattern.GetVisibleRangePaths ();
			List<ITextPatternRange> ranges = new List<ITextPatternRange> ();
			foreach (string path in visibleRanges) {
				ranges.Add (GetTextPatternRange (path));
			}
			return ranges.ToArray ();
		}

		public ITextPatternRange RangeFromChild (IElement childElement)
		{
			UiaDbusElement uiaDbusElement = childElement as UiaDbusElement;
			if (uiaDbusElement == null)
				throw new InvalidOperationException ("The childElement parameter " +
					"is not a child of the AutomationElement associated with the " +
					"TextPattern or from the array of children of the TextPatternRange.");
			string rangePath = pattern.RangePathFromChild (uiaDbusElement.DbusPath);
			return GetTextPatternRange (rangePath);
		}

		public ITextPatternRange RangeFromPoint (Point screenLocation)
		{
			DC.Point pt = new DC.Point (screenLocation);
			string rangePath = pattern.RangePathFromPoint (pt);
			return GetTextPatternRange (rangePath);
		}

#endregion
	}
}
