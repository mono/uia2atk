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
//	Mario Carrion <mcarrion@novell.com>
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Automation.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Mono.UIAutomation.Winforms
{
	// A TextPatternRange can represent an 
	// - insertion point, 
	// - a subset, or 
	// - all of the text in a TextPattern container.
	internal class TextRangeProvider : ITextRangeProvider
	{
#region Constructors
		public TextRangeProvider (ITextProvider provider, TextBoxBase textboxbase)
		{
			this.provider = provider;
			this.textboxbase = textboxbase;
			
			normalizer = new TextNormalizer (textboxbase);
		}

		internal TextRangeProvider (ITextProvider provider, TextBoxBase textboxbase,
		                            int start_point, int end_point)
		{
			this.provider = provider;
			this.textboxbase = textboxbase;
			this.normalizer = new TextNormalizer (textboxbase, start_point, end_point);
		}
#endregion
		
#region Public Properties 
		public ITextProvider TextProvider {
			get { return provider; }
		}		
#endregion

#region ITextRangeProvider Members
		public void AddToSelection ()
		{
			if (provider.SupportedTextSelection != SupportedTextSelection.Multiple) {
				throw new InvalidOperationException ();
			}
			
			//TODO: Verify if RichTextBox supports: SupportedTextSelection.Multiple
		}
	
		public ITextRangeProvider Clone ()
		{		
			return new TextRangeProvider (provider, textboxbase,
			                              StartPoint, EndPoint); 
		}

		public bool Compare (ITextRangeProvider range)
		{
			TextRangeProvider rangeProvider = range as TextRangeProvider;
			if (rangeProvider == null) {
				return false;
			}

			if (rangeProvider.TextProvider != provider) {
				throw new ArgumentException ();
			}
		
			return (rangeProvider.StartPoint == StartPoint) 
				&& (rangeProvider.EndPoint == EndPoint);
		}

		public int CompareEndpoints (TextPatternRangeEndpoint endpoint, 
		                             ITextRangeProvider targetRange,
		                             TextPatternRangeEndpoint targetEndpoint)
		{
			TextRangeProvider targetRangeProvider = targetRange as TextRangeProvider;
			if (targetRangeProvider == null) {
				throw new ArgumentNullException ();
			}

			if (targetRangeProvider.TextProvider != provider) {
				throw new ArgumentException ();
			}
			
			int point = endpoint == TextPatternRangeEndpoint.End ? EndPoint : StartPoint;
			int targePoint = (targetEndpoint == TextPatternRangeEndpoint.End)
				? targetRangeProvider.EndPoint : targetRangeProvider.StartPoint;

			return point.CompareTo (targePoint);
		}

		public void ExpandToEnclosingUnit (TextUnit unit)
		{
			switch (unit) {
			case TextUnit.Character:
				// This does nothing
				break;
			case TextUnit.Format:
				// Textbox doesn't support Format
				break;
			case TextUnit.Word:
				normalizer.WordNormalize ();
				break;
			case TextUnit.Line:
				normalizer.LineNormalize ();
				break;
			case TextUnit.Paragraph:
				normalizer.ParagraphNormalize ();
				break;
			case TextUnit.Page:
				// Textbox doesn't support Page
			case TextUnit.Document:
				normalizer.DocumentNormalize ();
				break;
			}
		}

		public ITextRangeProvider FindAttribute (int attribute, object @value, 
		                                         bool backward)
		{
			// Lazy load mappings
			if (attr_to_val_handler == null) {
				PopulateAttributeDictionary ();
			}

			if (!attr_to_val_handler.ContainsKey (attribute)) {
				return null;
			}

			Line line;
			LineTag tag;
			int pos;

			TagDataValueHandler val_handler = attr_to_val_handler[attribute];
			Document d = textboxbase.document;
			TextRangeProvider range = null;

			if (!backward) {
				for (int i = StartPoint; i < EndPoint; i += tag.Length) {
					d.CharIndexToLineTag (i, out line, out tag, out pos);
					if (val_handler (new TagData (tag, line)).Equals (@value)) {
						if (range == null) {
							range = new TextRangeProvider (provider, textboxbase,
										       i, i + tag.Length);
						} else {
							range.EndPoint += tag.Length;
						}
					} else if (range != null) {
						break;
					}
				}
			} else {
				for (int i = EndPoint - 1; i >= StartPoint; i -= tag.Length) {
					d.CharIndexToLineTag (i, out line, out tag, out pos);
					if (val_handler (new TagData (tag, line)).Equals (@value)) {
						if (range == null) {
							int start = i - tag.Length + 1;
							range = new TextRangeProvider (provider, textboxbase,
										       start, start + tag.Length);
						} else {
							range.StartPoint -= tag.Length;
						}
					} else if (range != null) {
						break;
					}
				}
			}

			return range;
		}

		public ITextRangeProvider FindText (string text, bool backward, 
		                                    bool ignoreCase)
		{
			string contents = textboxbase.Text;	
			StringComparison cmp = ignoreCase ? StringComparison.CurrentCultureIgnoreCase
			                                  : StringComparison.CurrentCulture;
			if (String.IsNullOrEmpty (text) || String.IsNullOrEmpty (contents)) {
				return null;
			}
			
			int index = -1;
			if (backward) {
				index = contents.LastIndexOf (text, EndPoint - 1,
				                              EndPoint - StartPoint, cmp);
			} else {
				index = contents.IndexOf (text, StartPoint,
				                          EndPoint - StartPoint, cmp);
			}

			return (index >= 0) ? new TextRangeProvider (provider, textboxbase,
			                                             index, index + text.Length)
			                    : null;
		}

		public object GetAttributeValue (int attribute)
		{
			// Lazy load mappings
			if (attr_to_val_handler == null) {
				PopulateAttributeDictionary ();
			}

			if (!attr_to_val_handler.ContainsKey (attribute)) {
				throw new ArgumentException ();
			}

			TagDataValueHandler val_handler = attr_to_val_handler[attribute];

			Line line;
			LineTag tag;
			int pos;
			
			List<TagData> tags = new List<TagData> ();
			Document d = textboxbase.document;

			int point = normalizer.StartPoint;
			while (point < normalizer.EndPoint) {
				d.CharIndexToLineTag (point, out line,
				                      out tag, out pos);
				tags.Add (new TagData (tag, line));
				point += tag.Length + 1;
			}

			IEnumerable<TagData> results
				= tags.Distinct (new LineTagComparer (val_handler));

			int count = results.Count ();
			if (count == 1) {
				return val_handler (results.First ());
			} else if (count > 1) {
				return TextPattern.MixedAttributeValue;
			}

			return null;
		}

		public Rect[] GetBoundingRectangles ()
		{
			if (StartPoint == EndPoint
			    || String.IsNullOrEmpty (textboxbase.Text)) {
				return new Rect[0];
			}

			Document document = textboxbase.document;
			List<Rect> rects = new List<Rect> ();

			for (int i = 0; i < document.Lines; i++) {
				Line line = document.GetLine (i);
				rects.Add (new Rect (line.X, line.Y,
				                     line.Width, line.Height));
			}

			return rects.ToArray ();
		}

		public IRawElementProviderSimple[] GetChildren ()
		{
			// TextBoxes don't have children
			if (textboxbase is TextBox) {
				return new IRawElementProviderSimple[0];
			}

			throw new NotImplementedException();
		}

		public IRawElementProviderSimple GetEnclosingElement ()
		{
			// MSDN: The enclosing AutomationElement, typically the
			// text provider that supplies the text range.
			if (textboxbase is TextBox) {
				return (IRawElementProviderSimple)provider;
			}

			// MSDN: However, if the text provider supports child
			// elements such as tables or hyperlinks, then the
			// enclosing element could be a descendant of the text
			// provider.
			throw new NotImplementedException();
		}

		public string GetText (int maxLength)
		{
			if (maxLength < -1)
				throw new ArgumentOutOfRangeException ();
				
			int startPoint = StartPoint;
			int endPoint = EndPoint;
			if (StartPoint > EndPoint) {
				startPoint = EndPoint;
				endPoint = StartPoint;
			}

			int length = endPoint - startPoint;
			if (length > maxLength && maxLength != -1)
				length = maxLength;

			return textboxbase.Text.Substring (startPoint, length);
		}

		public int Move (TextUnit unit, int count)
		{	
			return normalizer.Move (unit, count).Moved;
		}

		public void MoveEndpointByRange (TextPatternRangeEndpoint endpoint, 
		                                 ITextRangeProvider targetRange, 
		                                 TextPatternRangeEndpoint targetEndpoint)
		{
			TextRangeProvider prov = (TextRangeProvider)targetRange;
			
			int val = (targetEndpoint == TextPatternRangeEndpoint.Start)
					? prov.StartPoint : prov.EndPoint;

			if (endpoint == TextPatternRangeEndpoint.Start) {
				normalizer.StartPoint = val;
			} else if (endpoint == TextPatternRangeEndpoint.End) {
				normalizer.EndPoint = val;
			}
		}

		public int MoveEndpointByUnit (TextPatternRangeEndpoint endpoint, 
		                               TextUnit unit, int count)
		{
			// NOTE: The order of these cases is crucial
			switch (unit) {
			case TextUnit.Character:
				if (endpoint == TextPatternRangeEndpoint.Start)
					return normalizer.CharacterMoveStartPoint (count);
				else
					return normalizer.CharacterMoveEndPoint (count);
			case TextUnit.Word:
				if (endpoint == TextPatternRangeEndpoint.Start)
					return normalizer.WordMoveStartPoint (count);
				else
					return normalizer.WordMoveEndPoint (count);
			case TextUnit.Line:
				if (endpoint == TextPatternRangeEndpoint.Start)
					return normalizer.LineMoveStartPoint (count);
				else
					return normalizer.LineMoveEndPoint (count);
			case TextUnit.Paragraph:
				if (endpoint == TextPatternRangeEndpoint.Start)
					return normalizer.ParagraphMoveStartPoint (count);
				else
					return normalizer.ParagraphMoveEndPoint (count);
			// LAMESPEC: this should fall back on TextUnit.Word
			// according to MSDN, but for TextBox, this resembles
			// TextUnit.Page.

			// TextBox doesn't support Page or Format
			case TextUnit.Format:
			case TextUnit.Page:
			case TextUnit.Document:
				if (endpoint == TextPatternRangeEndpoint.Start)
					return normalizer.DocumentMoveStartPoint (count);
				else
					return normalizer.DocumentMoveEndPoint (count);
			}

			return 0;
		}

		public void RemoveFromSelection ()
		{
			if (provider.SupportedTextSelection != SupportedTextSelection.Multiple)
				throw new InvalidOperationException ();
			
			//TODO: Verify if RichTextBox supports: SupportedTextSelection.Multiple
		}

		public void ScrollIntoView (bool alignToTop)
		{
			// TODO: handle alignToTop
			
			// XXX: Not sure if moving the caret is appropriate
			// here, but the limited API doesn't support any
			// alternative
			Document document = textboxbase.document;

			Line line;
			LineTag linetag;
			int char_pos;
			document.CharIndexToLineTag (StartPoint, out line,
			                             out linetag, out char_pos);
			
			document.PositionCaret (line, char_pos);
			textboxbase.ScrollToCaret ();
		}

		public void Select ()
		{
			textboxbase.SelectionStart = normalizer.StartPoint;
			textboxbase.SelectionLength = System.Math.Abs (normalizer.EndPoint - normalizer.StartPoint);
		}
#endregion

		private delegate object TagDataValueHandler (TagData d);
		private class LineTagComparer : IEqualityComparer<TagData>
		{
			public LineTagComparer (TagDataValueHandler h) { this.val_handler = h; }
			public bool Equals (TagData a, TagData b) { return val_handler (a).Equals (val_handler (b)); }
			public int GetHashCode (TagData d) { return d.Tag.BackColor.GetHashCode (); }

			private TagDataValueHandler val_handler;
		}

		private struct TagData
		{
			public LineTag Tag;
			public Line Line;

			public TagData (LineTag tag, Line line)
			{
				Tag = tag;
				Line = line;
			}
		}

		// Font weights
		private const int LOGFONT_NORMAL = 400;
		private const int LOGFONT_BOLD = 700;

		private static Dictionary<int, TagDataValueHandler> attr_to_val_handler = null;

		private void PopulateAttributeDictionary ()
		{
			attr_to_val_handler = new Dictionary<int, TagDataValueHandler> ();

			// This is not as much crack as it seems.  This
			// dictionary maps from the Attributes in TextPattern
			// to handlers that return the internal Document
			// representation of the attribute.  It's used in
			// FindAttribute and GetAttributeValue.  Since this
			// is a big dictionary, it's also lazy-loaded the first
			// time FindAttribute or GetAttributeValue are called.
			attr_to_val_handler.Add (TextPattern.BackgroundColorAttribute.Id,
			                         x => x.Tag.BackColor.ToArgb ());
			attr_to_val_handler.Add (TextPattern.FontNameAttribute.Id,
			                         x => x.Tag.Font.Name);
			attr_to_val_handler.Add (TextPattern.FontSizeAttribute.Id,
			                         x => x.Tag.Font.Size);
			attr_to_val_handler.Add (TextPattern.FontWeightAttribute.Id,
			                         x => x.Tag.Font.Bold ? LOGFONT_BOLD : LOGFONT_NORMAL);
			attr_to_val_handler.Add (TextPattern.ForegroundColorAttribute.Id,
			                         x => x.Tag.Color.ToArgb ());
			attr_to_val_handler.Add (TextPattern.IsItalicAttribute.Id,
			                         x => x.Tag.Font.Italic);
			attr_to_val_handler.Add (TextPattern.StrikethroughStyleAttribute.Id,
			                         x => x.Tag.Font.Strikeout ? TextDecorationLineStyle.Single
			                                                   : TextDecorationLineStyle.None);
			attr_to_val_handler.Add (TextPattern.UnderlineStyleAttribute.Id,
			                         x => x.Tag.Font.Underline ? TextDecorationLineStyle.Single
			                                                   : TextDecorationLineStyle.None);
			attr_to_val_handler.Add (TextPattern.HorizontalTextAlignmentAttribute.Id,
			                         x => MapTextAlignment (x.Line.Alignment));
			attr_to_val_handler.Add (TextPattern.IndentationFirstLineAttribute.Id,
			                         x => x.Line.Indent);
			attr_to_val_handler.Add (TextPattern.IndentationLeadingAttribute.Id,
			                         x => x.Line.HangingIndent);
			attr_to_val_handler.Add (TextPattern.IndentationTrailingAttribute.Id,
			                         x => x.Line.RightIndent);

			// Not currently supported by Document API
			attr_to_val_handler.Add (TextPattern.TabsAttribute.Id,
			                         x => new double[0]);
			attr_to_val_handler.Add (TextPattern.AnimationStyleAttribute.Id,
			                         x => AnimationStyle.None);
			attr_to_val_handler.Add (TextPattern.BulletStyleAttribute.Id,
			                         x => BulletStyle.None);
			attr_to_val_handler.Add (TextPattern.CapStyleAttribute.Id,
			                         x => CapStyle.None);
			attr_to_val_handler.Add (TextPattern.IsHiddenAttribute.Id,
			                         x => false);
			attr_to_val_handler.Add (TextPattern.IsReadOnlyAttribute.Id,
			                         x => false);
			attr_to_val_handler.Add (TextPattern.IsSubscriptAttribute.Id,
			                         x => false);
			attr_to_val_handler.Add (TextPattern.IsSuperscriptAttribute.Id,
			                         x => false);
			attr_to_val_handler.Add (TextPattern.OutlineStylesAttribute.Id,
			                         x => OutlineStyles.None);
		}
	
		private HorizontalTextAlignment MapTextAlignment (HorizontalAlignment align)
		{
			switch (align) {
			case HorizontalAlignment.Left:
				return HorizontalTextAlignment.Left;
			case HorizontalAlignment.Right:
				return HorizontalTextAlignment.Right;
			case HorizontalAlignment.Center:
				return HorizontalTextAlignment.Centered;
			}
			return HorizontalTextAlignment.Left;
		}

#region Private Properties
		private int EndPoint {
			get { return normalizer.EndPoint; }
			set { normalizer.EndPoint = value; }
		}
		
		private int StartPoint {
			get { return normalizer.StartPoint; }
			set { normalizer.StartPoint = value; }
		}
#endregion

#region Private Members
		private TextNormalizer normalizer;
		private ITextProvider provider;
		private TextBoxBase textboxbase;
#endregion
	}
}
