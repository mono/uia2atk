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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using Mono.Unix;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.Source;
using Atspi;
using System.Windows.Automation.Provider;

namespace AtspiUiaSource
{
	public class TextSource : ITextPattern
	{
		private Accessible accessible;
		private Atspi.Text text;

		public TextSource (Element element)
		{
			accessible = element.Accessible;
			text = accessible.QueryText ();
		}

		public ITextPatternRange DocumentRange {
			get {
				return new TextRangePattern (accessible);
			}
		}

		public SupportedTextSelection SupportedTextSelection {
			get {
				return (accessible.StateSet.Contains (StateType.Multiselectable)
					? SupportedTextSelection.Multiple
					: SupportedTextSelection.Single);
			}
		}

		public ITextPatternRange [] GetSelection ()
		{
			int nSelections = text.NSelections;
			TextRangePattern [] range = new TextRangePattern [nSelections];
			for (int i = 0; i < nSelections; i++) {
				int startOffset, endOffset;
				text.GetSelection (i, out startOffset, out endOffset);
				range [i] = new TextRangePattern (accessible, startOffset, endOffset);
			}
			return range;
		}

		public ITextPatternRange [] GetVisibleRanges ()
		{
			// TODO (BNC#551111)
			ITextPatternRange [] range = new ITextPatternRange [1];
			range [0] = DocumentRange;
			return range;
		}

		public ITextPatternRange RangeFromChild (IElement childElement)
		{
			Element child = childElement as Element;
			if (child == null || child.Parent != this)
				throw new InvalidOperationException ();
			return new TextRangePattern (accessible);
		}

		public ITextPatternRange RangeFromPoint (Point screenLocation)
		{
			int offset = text.GetOffsetAtPoint ((int)screenLocation.X, (int)screenLocation.Y, CoordType.Screen);
			// TODO: Check this behavior
			if (offset < 0)
				return null;
			return new TextRangePattern (accessible, offset, offset);
		}
	}

	public class TextRangePattern : ITextPatternRange
	{
		private Accessible accessible;
		private Atspi.Text text;
		private int startOffset;
		private int endOffset;

		public TextRangePattern (Accessible accessible)
		{
			this.accessible = accessible;
			this.text = accessible.QueryText ();
			this.startOffset = 0;
			this.endOffset = text.CharacterCount;
		}

		public TextRangePattern (Accessible accessible, int startOffset, int endOffset)
		{
			this.accessible = accessible;
			this.text = accessible.QueryText ();
			this.startOffset = startOffset;
			this.endOffset = endOffset;
		}

		public void AddToSelection ()
		{
			if (!accessible.StateSet.Contains (StateType.Enabled))
				throw new ElementNotEnabledException ();

			// TODO: Throw exception on failure
			text.AddSelection (startOffset, endOffset);
		}

		public ITextPatternRange Clone ()
		{
			return new TextRangePattern (accessible, startOffset, endOffset);
		}

		public bool Compare (ITextPatternRange range)
		{
			TextRangePattern other = range as TextRangePattern;
			if (other == null)
				return false;
			if (other.accessible != accessible)
				throw new ArgumentException ("Comparing ranges from different elements");
			return (other.startOffset == startOffset &&
				other.endOffset == endOffset);
		}

		public int CompareEndpoints (TextPatternRangeEndpoint endpoint, ITextPatternRange targetRange, TextPatternRangeEndpoint targetEndpoint)
		{
			TextRangePattern targetRangePattern = targetRange as TextRangePattern;
			if (targetRangePattern == null)
				throw new ArgumentNullException ();

			if (targetRangePattern.accessible != accessible)
				throw new ArgumentException ();

			int point = (endpoint == TextPatternRangeEndpoint.End
				? endOffset : startOffset);
			int targetPoint = (targetEndpoint ==
				TextPatternRangeEndpoint.End)
				? targetRangePattern.endOffset
				: targetRangePattern.startOffset;

			if (point < targetPoint)
				return -1;
			if (point > targetPoint)
				return 1;
			return 0;
		}

		public void ExpandToEnclosingUnit (TextUnit unit)
		{
			int newStartOffset, newEndOffset;
			int dummy;
			switch (unit) {
			case TextUnit.Format:
				text.GetAttributeRun (startOffset, out newStartOffset, out newEndOffset, false);
				break;
			case TextUnit.Word:
				WordNormalize ();
				return;
			case TextUnit.Paragraph:
				ParagraphNormalize ();
				return;
			case TextUnit.Page:
				// Not currently supported; treat as Document
			case TextUnit.Document:
				StartOffset = 0;
				EndOffset = text.CharacterCount;
				return;
			default:
				text.GetTextAtOffset (startOffset,
					GetAtkBoundaryType (unit, false),
					out newStartOffset,
					out dummy);
				text.GetTextAtOffset (endOffset - 1,
					GetAtkBoundaryType (unit, false),
					out dummy,
					out newEndOffset);
				break;
			}
			if (newStartOffset < startOffset)
				StartOffset = newStartOffset;
			if (newEndOffset > endOffset)
				EndOffset = newEndOffset;
		}

		public ITextPatternRange FindAttribute (AutomationTextAttribute attribute, object value, bool backward)
		{
			// TODO: Support this in at-spi.
			// I don't want to iterate over the wire.
			Log.Debug ("AtspiUiaSource: TODO: FindAttribute");
			return null;
		}

		public ITextPatternRange FindText (string txt, bool backward, bool ignoreCase)
		{
			string contents = text.GetText ();
			StringComparison cmp = ignoreCase ? StringComparison.CurrentCultureIgnoreCase
			                                  : StringComparison.CurrentCulture;
			if (String.IsNullOrEmpty (txt) || String.IsNullOrEmpty (contents)) {
				return null;
			}

			int index = -1;
			if (backward) {
				index = contents.LastIndexOf (txt, endOffset - 1,
				                              endOffset - startOffset, cmp);
			} else {
				index = contents.IndexOf (txt, startOffset,
				                          endOffset - startOffset, cmp);
			}

			return (index >= 0) ? new TextRangePattern (accessible,
				index, index + txt.Length)
				: null;
		}

		private Regex parse_color = new Regex (@"^([0-9+),([0-9]+),([0-9]+)$", RegexOptions.Compiled);

		public object GetAttributeValue (AutomationTextAttribute attribute)
		{
			int start, end;
			IDictionary<string, string> attributes = text.GetAttributeRun (startOffset, out start, out end, true);
			string val;
			if (attributes == null)
				return null;
			if (end < endOffset)
				return TextPattern.MixedAttributeValue;

			if (attribute.Id == TextPattern.FontWeightAttribute.Id &&
				attributes.TryGetValue ("weight", out val)) {
				if (val == "bold")
					return 400;
				return Int32.Parse (val);
			} else if (attribute.Id == TextPattern.BackgroundColorAttribute.Id &&
				attributes.TryGetValue ("bg-color", out val)) {
				Match m = parse_color.Match (val);
				if (m.Success) {
					int v1, v2, v3;
					v1 = Int32.Parse (m.Groups [1].ToString ());
					v2 = Int32.Parse (m.Groups [2].ToString ());
					v3 = Int32.Parse (m.Groups [3].ToString ());
					return (v1 << 16) + (v2 << 8) + v3;
				}
			} else if (attribute.Id == TextPattern.ForegroundColorAttribute.Id &&
				attributes.TryGetValue ("fg-color", out val)) {
				Match m = parse_color.Match (val);
				if (m.Success) {
					int v1, v2, v3;
					v1 = Int32.Parse (m.Groups [1].ToString ());
					v2 = Int32.Parse (m.Groups [2].ToString ());
					v3 = Int32.Parse (m.Groups [3].ToString ());
					return (v1 << 16) + (v2 << 8) + v3;
				}
			} else if (attribute.Id == TextPattern.FontNameAttribute.Id &&
				attributes.TryGetValue ("family-name", out val)) {
				return val;
			} else if (attribute.Id == TextPattern.FontSizeAttribute.Id &&
				attributes.TryGetValue ("size", out val)) {
				return Int32.Parse (val);
			} else if (attribute.Id == TextPattern.IsItalicAttribute.Id) {
				if (attributes.TryGetValue ("font-size", out val))
					return (val == "italic");
				return false;
			}
			Log.Debug ("TODO: GetAttributeValue for " + attribute.ProgrammaticName);
			return null;
		}

		public Rect [] GetBoundingRectangles ()
		{
			Component comp = accessible.QueryComponent ();
			if (comp == null)
				return new Rect [0];	// should never happen
			int x, y, width, height;
			text.GetRangeExtents (startOffset, endOffset,
				out x, out y, out width, out height,
				CoordType.Screen);
			Rect [] ret = new Rect [1];
			ret [0] = new Rect (x, y, width, height);
			return ret;
		}

		public IElement [] GetChildren ()
		{
			return new IElement [0];
		}

		public IElement GetEnclosingElement ()
		{
			return Element.GetElement (accessible);
		}

		public string GetText (int maxLength)
		{
			string str = text.GetText (startOffset, endOffset);
			if (maxLength == -1)
				return str;
			return str.Substring (0, maxLength);
		}

		public int Move (TextUnit unit, int count)
		{
			int result = MoveEndpointByUnit (
				TextPatternRangeEndpoint.Start, unit, count);
			MoveEndpointByUnit (
				TextPatternRangeEndpoint.End, unit, 1);
			return result;
		}

		public void MoveEndpointByRange (TextPatternRangeEndpoint endpoint, ITextPatternRange targetRange, TextPatternRangeEndpoint targetEndpoint)
		{
			TextRangePattern target = targetRange as TextRangePattern;
			if (target == null || target.accessible != accessible)
				throw new ArgumentException ();

			int offset =
				(targetEndpoint == TextPatternRangeEndpoint.Start
					? target.startOffset
					: target.endOffset);

			if (endpoint == TextPatternRangeEndpoint.Start) {
				startOffset = offset;
				if (endOffset < startOffset)
					endOffset = startOffset;
			} else {
				endOffset = offset;
				if (startOffset > endOffset)
					startOffset = endOffset;
			}
		}

		public int MoveEndpointByUnit (TextPatternRangeEndpoint endpoint, TextUnit unit, int count)
		{
			if (count == 0)
				return 0;
			switch (unit) {
			case TextUnit.Format:
				Log.Debug ("AtspiUiaSource: Moving by unit "
					+ unit + " not supported.");
				return 0;
			case TextUnit.Word:
				if (endpoint == TextPatternRangeEndpoint.Start)
					return WordMoveStartPoint (count);
				else
					return WordMoveEndPoint (count);
			case TextUnit.Page:
			case TextUnit.Document:
				if (endpoint == TextPatternRangeEndpoint.Start) {
					if (count < 0) {
						if (startOffset <= 0)
							return 0;
						StartOffset = 0;
						return -1;
					} else {
						int characterCount = text.CharacterCount;
						if (startOffset >= characterCount)
							return 0;
						StartOffset = characterCount;
						return 1;
					}
				} else {
					if (count < 0) {
						if (endOffset <= 0)
							return 0;
						EndOffset = 0;
						return -1;
					} else {
						int characterCount = text.CharacterCount;
						if (endOffset >= characterCount)
							return 0;
						EndOffset = characterCount;
						return 1;
					}
				}
			case TextUnit.Paragraph:
				if (endpoint == TextPatternRangeEndpoint.Start)
					return ParagraphMoveStartPoint (count);
				else
					return ParagraphMoveEndPoint (count);
			default:
				int offset = (endpoint == TextPatternRangeEndpoint.Start
					? startOffset
					: endOffset - 1);
				int newStartOffset = offset;
				int newEndOffset = offset;
				int ret = 0;
				BoundaryType boundary = GetAtkBoundaryType (unit, count > 0);
				while (count != 0) {
					int oldStartOffset = newStartOffset;
					int oldEndOffset = newEndOffset;
					if (count < 0) {
						text.GetTextBeforeOffset (oldStartOffset, boundary, out newStartOffset, out newEndOffset);
						if (newStartOffset == oldStartOffset &&
							newEndOffset == oldEndOffset)
							break;
						count++;
						ret--;
					} else {
						text.GetTextAfterOffset (oldStartOffset, boundary, out newStartOffset, out newEndOffset);
						if (newStartOffset == oldStartOffset &&
							newEndOffset == oldEndOffset)
							break;
						count--;
						ret++;
					}
				}
				if (endpoint == TextPatternRangeEndpoint.Start)
					StartOffset = newStartOffset;
				else
					EndOffset = newEndOffset;
				return ret;
			}
		}

		public void RemoveFromSelection ()
		{
			if (!accessible.StateSet.Contains (StateType.Enabled))
				throw new ElementNotEnabledException ();

			int nSelections = text.NSelections;
			for (int i = 0; i < nSelections; i++) {
				int start, end;
				bool set = false;
				text.GetSelection (i, out start, out end);
				if (startOffset > start && startOffset < end) {
					startOffset = end;
					set = true;
				}
				if (endOffset > start && endOffset < end) {
					endOffset = start;
					set = true;
				}
				if (set) {
					if (endOffset < startOffset) {
						text.RemoveSelection (i);
						i--;
					} else
						text.SetSelection (i, startOffset, endOffset);
					set = false;
				}
			}
		}

		public void ScrollIntoView (bool alignToTop)
		{
			if (!accessible.StateSet.Contains (StateType.Enabled))
				throw new ElementNotEnabledException ();

			Log.Debug ("AtspiUiaSource: ScrollIntoView unimplemented");
		}

		public void Select ()
		{
			if (!accessible.StateSet.Contains (StateType.Enabled))
				throw new ElementNotEnabledException ();

			text.SetSelection (0, startOffset, endOffset);
		}

		private BoundaryType GetAtkBoundaryType (TextUnit unit, bool end)
		{
			switch (unit) {
			case TextUnit.Character:
				return BoundaryType.Char;
			case TextUnit.Word:
				return (end?
					BoundaryType.WordEnd:
					BoundaryType.WordStart);
			case TextUnit.Line:
				return (end?
					BoundaryType.LineEnd:
					BoundaryType.LineStart);
			default:
				throw new Exception ("GetAtkBoundaryType called with unsupported TextUnit " + unit);
			}
		}

		private int StartOffset {
			set {
				startOffset = value;
				if (endOffset < startOffset)
					endOffset = startOffset;
			}
		}

		public int EndOffset {
			set {
				endOffset = value;
				if (startOffset > endOffset)
					startOffset = endOffset;
			}
		}

		private void ParagraphNormalize ()
		{
			LineParagraphNormalize (false);
		}

		private void LineParagraphNormalize (bool is_line)
		{
			if (!accessible.StateSet.Contains (StateType.MultiLine)) {
				startOffset = 0;
				endOffset = text.CharacterCount;
				return;
			}

			int n_chars = 0;
			string txt = text.GetText ();

			// First, fix up the start point
			int new_start = 0;
			for (int i = startOffset; i >= 0; i--) {
				if (BackwardPeekNewline (i, txt, out n_chars)) {
					new_start = i + 1;
					break;
				}
			}

			StartOffset = new_start;

			// if our range ends with a newline, don't do anything [Case 3]
			if (endOffset - 1 >= 0
			    && BackwardPeekNewline (endOffset - 1, txt, out n_chars)) {
				return;
			}

			int new_end = -1;
			if (is_line) {
				// walk backward until you hit a newline, or the startOffset
				for (int i = endOffset; i > startOffset; i--) {
					if (BackwardPeekNewline (i, txt, out n_chars)) {
						new_end = i + 1;
						break;
					}
				}
			} else {
				new_end = txt.Length;

				// walk forward until you hit a newline, or the end
				for (int i = endOffset; i < txt.Length; i++) {
					if (ForwardPeekNewline (i, txt, out n_chars)) {
						new_end = i + n_chars;
						break;
					}
				}

				EndOffset = new_end;
				return;
			}

			// if we found a newline, move on
			if (new_end >= 0) {
				EndOffset = new_end;
				return;
			}

			// we hit the start, so look forward for a newline
			new_end = txt.Length;
			for (int i = endOffset; i < txt.Length; i++) {
				if (ForwardPeekNewline (i, txt, out n_chars)) {
					new_end = i + n_chars;
					break;
				}
			}

			EndOffset = new_end;
		}

		private int ParagraphMoveStartPoint (int count)
		{
			int moved = ParagraphMoveStartEndPoint (count, ref startOffset);
			if (startOffset > endOffset) {
				endOffset = startOffset;
			}
			return moved;
		}

		private int ParagraphMoveEndPoint (int count)
		{
			int moved = ParagraphMoveStartEndPoint (count, ref endOffset);
			if (endOffset < startOffset) {
				startOffset = endOffset;
			}
			return moved;
		}

		private int ParagraphMoveStartEndPoint (int count, ref int point)
		{
			if (count == 0) {
				return 0;
			}

			string txt = text.GetText ();

			int c = 0, index = 0, n_chars = 0;
			if (count > 0) {
				// walk forward until you see count number of
				// new lines
				for (int i = point; i < txt.Length; i++) {
					index = i;

					if (ForwardPeekNewline (i, txt, out n_chars)) {
						c++;
						i += n_chars;

						if (c == count) {
							index = i;
							break;
						}

						i -= 1;  // account for iteration
					}
				}

				// if we didn't find the number of lines we
				// were asked for, jump to the end of the
				// string, and count that as a line
				if (c != count && point < (txt.Length - 1)) {
					c++;
					index = txt.Length;
				}

				point = index;
				return c;
			} else {
				// walk backwards until you see count number of
				// new lines
				for (int i = point - 1; i >= 0; i--) {
					index = i;

					// stop when you hit count newlines
					// (plus 1 since we want all the text
					// leading up to the next newline)
					if (BackwardPeekNewline (i, txt, out n_chars)) {
						c--;

						if (c == (count - 1)) {
							c += 1;
							index = i + 1;
							break;
						}

						i -= n_chars;
						i += 1;  // account for iteration
					}
				}

				// if we didn't find the number of lines we
				// were asked for, jump to the front of the
				// string, and count that as a line
				if (c != count && point > 0) {
					c--;
					index = 0;
				}

				point = index;
				return c;
			}
		}

		private bool ForwardPeekNewline (int i, string txt, out int n_chars)
		{
			n_chars = 0;

			if (i + 1 < txt.Length
			    && txt [i] == '\r' && txt [i+1] == '\n') {
				n_chars = 2;
				return true;
			} else if (txt [i] == '\r' || txt [i] == '\n') {
				n_chars = 1;
				return true;
			}
			return false;
		}

		private bool BackwardPeekNewline (int i, string txt, out int n_chars)
		{
			n_chars = 0;

			if (i - 1 >= 0
			    && txt [i] == '\n' && txt [i-1] == '\r') {
				n_chars = 2;
				return true;
			} else if (txt [i] == '\r' || txt[i] == '\n') {
				n_chars = 1;
				return true;
			}
			return false;
		}

		private Regex is_separator = new Regex (@"^[^\w]$", RegexOptions.Compiled);

		private bool IsWordSeparator (char c)
		{
			return is_separator.IsMatch (c.ToString ());
		}

		private void WordNormalize ()
		{
			int index = 0;

			// NOTE: There is a particular condition when either
			// the StartPoint or EndPoint are spaces, for example:
			//      "Hello my baby, hello  {   my d}arling"
			// After normalizing string the result is:
			//      "Hello my baby, hello{     my darling}"
			//
			// However the string:
			//      "Hello my baby, hello     {my d}arling"
			// is:
			//      "Hello my baby, hello     {my darling}"

			string txt = text.GetText ();
			if (IsWordSeparator (txt [startOffset])) {
				// Walk backwards until you hit a non separator
				for (index = startOffset; index >= 0; index--) {
					if (!IsWordSeparator (txt [index])) {
						break;
					}
				}
				startOffset = index + 1;
			} else {
				// Walk from the start point backwards, finding
				// the last separator

				index = -1;
				for (int i = startOffset; i >= 0; i--) {
					if (IsWordSeparator (txt [i])) {
						index = i;
						break;
					}
				}
				startOffset = (index == -1) ? 0 : index + 1;
			}

			index = -1;
			if (endOffset > 0 && IsWordSeparator (txt [endOffset - 1])) {
				// Extend the range to consume all spaces until the next character
				for (index = endOffset - 1; index < txt.Length; index++) {
					if (!IsWordSeparator (txt [index])) {
						break;
					}
				}
				endOffset = index;
			} else {
				// Search for the next separator
				index = -1;
				for (int i = endOffset; i < txt.Length ; i++) {
					if (IsWordSeparator (txt [i])) {
						index = i;
						break;
					}
				}

				if (index == -1) {
					endOffset = txt.Length;
				} else {
					endOffset += index - endOffset;
				}
			}
		}

		private int WordMoveStartPoint (int count)
		{
			int moved = WordMoveStartEndPoint (count, ref startOffset);
			if (startOffset > endOffset)
				endOffset = startOffset;
			return moved;
		}

		private int WordMoveEndPoint (int count)
		{
			int moved = WordMoveStartEndPoint (count, ref endOffset);
			if (endOffset < startOffset)
				startOffset = endOffset;
			return moved;
		}

		private int WordMoveStartEndPoint (int count, ref int point)
		{
			if (count == 0)
				return 0;

			int index = 0, c = 0;
			string txt = text.GetText ();
			if (count > 0) {
				bool last_was_sep = true;
				for (int i = point; i < txt.Length; i++) {
					index = i + 1;

					if (IsWordSeparator (txt [i])) {
						c++;

						last_was_sep = true;
					} else {
						if (last_was_sep) {
							c++;
						}

						last_was_sep = false;
					}

					if (c == (count + 1)) {
						index = i;
						c--;
						break;
					}
				}

				// if we didn't find the number of lines we
				// were asked for, jump to the end of the
				// string, and count that as a line
				if (c < count && point < (txt.Length - 1)) {
					c++;
					index = txt.Length;
				}

				point = index;
				return c;
			} else {
				bool last_was_chr = false;
				for (int i = point - 1; i >= 0; i--) {
					index = i;

					if (IsWordSeparator (txt [i])) {
						c--;

						if (last_was_chr) {
							c--;
						}

						last_was_chr = false;
					} else {
						last_was_chr = true;
					}

					if (c == count) {
						index = i;
						break;
					} else if (c < count) {
						index = i + 1;
						break;
					}
				}

				// if we didn't find the number of lines we
				// were asked for, jump to the front of the
				// string, and count that as a line
				if (c > count && point > 0) {
					c--;
					index = 0;
				}

				point = index;
				return c;
			}
		}
	}
}
