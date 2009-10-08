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
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using DC = Mono.UIAutomation.UiaDbus;
using Mono.UIAutomation.UiaDbus.Interfaces;

using NDesk.DBus;

namespace Mono.UIAutomation.UiaDbusBridge.Wrappers
{
	public class TextPatternWrapper : ITextPattern
	{
#region Private Fields

		private ITextProvider provider;
		Bus bus;
		string patternPath;
		private Dictionary<string, TextPatternRangeInfo> textRangeMapping =
			new Dictionary<string, TextPatternRangeInfo> ();
		private int textRangeIdCount = 0;

#endregion

#region internal methods and types

		internal struct TextPatternRangeInfo
		{
			public ITextRangeProvider Provider;
			public TextPatternRangeWrapper ProviderWrapper;
		}

		internal string GetOrCreateTextRange (ITextRangeProvider rangeProvider)
		{
			if (rangeProvider == null)
				return string.Empty;
			foreach (var entry in textRangeMapping) {
				if (entry.Value.Provider == rangeProvider)
					return entry.Key;
			}
			TextPatternRangeWrapper tpw = new TextPatternRangeWrapper (rangeProvider, this);
			string path = string.Format ("{0}/Range{1}", patternPath, textRangeIdCount);
			bus.Register (new ObjectPath (path), tpw);
			textRangeIdCount++;
			TextPatternRangeInfo newInfo = new TextPatternRangeInfo {
				Provider = rangeProvider,
				ProviderWrapper = tpw
			};
			textRangeMapping.Add (path, newInfo);
			return path;
		}

		internal TextPatternRangeInfo GetTextRangeFromPath (string path)
		{
			TextPatternRangeInfo ret;
			if (!textRangeMapping.TryGetValue (path, out ret))
				throw new Exception (
					string.Format("\"{0}\" is an invalid TextRange dbus path", path));
			return ret;
		}
#endregion

#region Constructor

		internal TextPatternWrapper (ITextProvider provider, Bus bus, string patternPath)
		{
			this.provider = provider;
			this.bus = bus;
			this.patternPath = patternPath;
		}

#endregion

#region ITextPattern Members

		public string DocumentRangePath {
			get {
				var textRangeProvider = provider.DocumentRange;
				return GetOrCreateTextRange (textRangeProvider);
			}
		}

		public SupportedTextSelection SupportedTextSelection {
			get {
				return provider.SupportedTextSelection;
			}
		}

		public string [] GetSelectionPaths ()
		{
			var selection = provider.GetSelection ();
			List<string> selectionPaths = new List<string> (selection.Length);
			foreach (var selectionRange in selection)
				selectionPaths.Add (GetOrCreateTextRange (selectionRange));
			return selectionPaths.ToArray ();
		}

		public string [] GetVisibleRangePaths ()
		{
			var visibleRanges = provider.GetVisibleRanges ();
			List<string> visibleRangePaths = new List<string> (visibleRanges.Length);
			foreach (var visibleRange in visibleRanges)
				visibleRangePaths.Add (GetOrCreateTextRange (visibleRange));
			return visibleRangePaths.ToArray ();
		}

		public string RangePathFromChild (string childPath)
		{
			var childProvider = AutomationBridge.Instance.FindProviderByPath (childPath);
			if (childProvider == null)
				throw new Exception (
					string.Format("\"{0}\" is an invalid element dbus path", childPath));
			var textRangeProvider = provider.RangeFromChild (childProvider);
			return GetOrCreateTextRange (textRangeProvider);
		}

		public string RangePathFromPoint (DC.Point screenLocation)
		{
			var textRangeProvider = provider.RangeFromPoint (new SW.Point (screenLocation.x, screenLocation.y));
			return GetOrCreateTextRange (textRangeProvider);
		}

#endregion
	}
}
