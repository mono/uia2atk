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
using System.Text.RegularExpressions;
using System.Windows.Automation.Text;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{
	// Algorithm:
	// http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.itextrangeprovider.move.aspx
	public class TextNormalizer
	{
#region Constructor
		public TextNormalizer (TextBoxBase textboxbase) 
			: this (textboxbase, 0, textboxbase.Text.Length)
		{
		}
		
		public TextNormalizer (TextBoxBase textboxbase, int startPoint, int endPoint)
		{
			this.textboxbase = textboxbase;
			
			start_point = startPoint;
			end_point = endPoint;
		}
#endregion
		
#region Public properties
		/**
		 * NOTE: These points are actually lie between characters,
		 * specifically to the left of the character who's index they
		 * refer to.
		 */
		public int StartPoint {
			get { return start_point; }
			set { start_point = value; }
		}
		
		public int EndPoint {
			get { return end_point; }
			set { end_point = value; }
		}
#endregion
		
#region Character methods
		public int CharacterMoveStartPoint (int count)
		{
			int moved = CharacterMoveStartEndPoint (count, ref start_point);
			if (start_point > end_point) {
				end_point = start_point;
			}
			return moved;
		}

		public int CharacterMoveEndPoint (int count) 
		{
			int moved = CharacterMoveStartEndPoint (count, ref end_point);
			if (end_point < start_point) {
				start_point = end_point;
			}
			return moved;
		}
		
		private int CharacterMoveStartEndPoint (int count, ref int point)
		{
			if (count == 0) {
				return 0;
			}

			int moved = 0;
			int length = textboxbase.Text.Length;
			int proposed_point = point + count;

			if (proposed_point < 0) {
				proposed_point = 0;
				moved = point + 1;
			} else if (proposed_point > length) {
				proposed_point = length;
				moved = length - point;
			} else {
				point = proposed_point;
				moved = count;
				count = 0;
			}

			point = proposed_point;
			return (count < 0 ? -1 : 1) * moved;
		}
#endregion

#region Format methods
		public int FormatMoveEndPoint (int count)
		{
			//TODO: Doesn't work with non-RichTextBox controls
			throw new NotImplementedException ();
		}
		
		public int FormatMoveStartPoint (int count)
		{
			//TODO: Doesn't work with non-RichTextBox controls
			throw new NotImplementedException ();
		}
#endregion
		
		
#region Line methods
		public void LineNormalize ()
		{
			LineParagraphNormalize (true);
		}
		
		private void LineParagraphNormalize (bool is_line)
		{
			if (textboxbase.Multiline == false) {
				start_point = 0;
				end_point = textboxbase.Text.Length;
				return;
			} 
			
			int n_chars = 0;
			string text = textboxbase.Text;

			// First, fix up the start point
			int new_start = 0;
			for (int i = start_point; i >= 0; i--) {
				if (BackwardPeekNewline (i, text, out n_chars)) {
					new_start = i + 1;
					break;
				}
			}

			start_point = new_start;
			
			// if our range ends with a newline, don't do anything [Case 3]
			if (end_point - 1 >= 0
			    && BackwardPeekNewline (end_point - 1, text, out n_chars)) {
				return;
			}
	
			int new_end = -1;
			if (is_line) {
				// walk backward until you hit a newline, or the start_point
				for (int i = end_point; i > start_point; i--) {
					if (BackwardPeekNewline (i, text, out n_chars)) {
						new_end = i + 1;
						break;
					}
				}
			} else {
				new_end = text.Length;

				// walk forward until you hit a newline, or the end
				for (int i = end_point; i < text.Length; i++) {
					if (ForwardPeekNewline (i, text, out n_chars)) {
						new_end = i + n_chars;
						break;
					}
				}
				
				end_point = new_end;
				return;
			}
			
			// if we found a newline, move on
			if (new_end >= 0) {
				end_point = new_end;
				return;
			}

			// we hit the start, so look forward for a newline
			new_end = text.Length;
			for (int i = end_point; i < text.Length; i++) {
				if (ForwardPeekNewline (i, text, out n_chars)) {
					new_end = i + n_chars;
					break;
				}
			}

			end_point = new_end;
		}
		
		public int LineMoveStartPoint (int count)
		{
			int moved = LineMoveStartEndPoint (count, ref start_point);
			if (start_point > end_point) {
				end_point = start_point;
			}
			return moved;
		}

		public int LineMoveEndPoint (int count)
		{
			int moved = LineMoveStartEndPoint (count, ref end_point);
			if (end_point < start_point) {
				start_point = end_point;
			}
			return moved;
		}

		private enum ParserState {
			Normal,
			LineFeed,
			CarriageReturn
		}
	
		private ParserState GetState (char c)
		{
			if (c == '\r') {
				return ParserState.CarriageReturn;
			} else if (c == '\n') {
				return ParserState.LineFeed;
			}
			return ParserState.Normal;
		}

		private int LineMoveStartEndPoint (int count, ref int start_end_point)
		{
			if (count == 0) {
				return 0;
			}

			string text = textboxbase.Text;

			ParserState[] state = new ParserState[2];
			state[0] = state[1] = ParserState.Normal;

			ParserState new_state;
			int c = 0, index = -1;

			if (count > 0) {
				// walk forward until you see count number of new lines
				for (int i = start_end_point; i < text.Length; i++) {
					new_state = GetState (text[i]);
					
					// Examples:
					//  * a\r\nab     * a\r\rab
					//        ^             ^
					//  * a\n\nab     * a\nab
					//        ^           ^
					if ((state[1] != ParserState.Normal
					     && new_state == ParserState.Normal)
					    || (state[1] == ParserState.CarriageReturn
					        && new_state == ParserState.CarriageReturn)
					    || (state[1] == ParserState.LineFeed
					        && new_state != ParserState.Normal)) {
						c++;
						index = i;

						if (c == count) {
							index += (new_state != ParserState.Normal) ? 1 : 0;
							break;
						}
					}

					state[0] = state[1];
					state[1] = new_state;
				}
				
				if (index < 0) {
					index = text.Length;
					c++;
				}

				start_end_point = index;
				return c;
			} else {
				// walk backward until you see count number of new lines
				for (int i = start_end_point - 1; i >= 0; i--) {
					new_state = GetState (text[i]);
					
					// Examples:
					//  * a\r\nab     * a\r\rab
					//     ^             ^
					//  * a\n\nab     * a\nab
					//     ^           ^
					if ((state[1] != ParserState.Normal
					     && new_state == ParserState.Normal)
					    || (state[1] == ParserState.CarriageReturn
					        && new_state == ParserState.LineFeed)
					    || (state[1] == ParserState.CarriageReturn
					        && new_state == ParserState.CarriageReturn
						&& state[0] != ParserState.LineFeed)
					    || (state[1] == ParserState.LineFeed
					        && new_state == ParserState.LineFeed)) {
						c--;
						index = i + 1;

						if (c == count) {
							index -= (state[1] == ParserState.Normal && new_state != ParserState.Normal) ? 1 : 0;
							break;
						}
					}

					state[0] = state[1];
					state[1] = new_state;
				}

				// Count the remaining characters as a line,
				// and move the cursor to the front

				// We'll only hit this when we've hit close to
				// the beginning of the string
				if (c > count) {
					c--;
					index = 0;
				}

				start_end_point = index;
				return c;
			}

			return 0;
		}
#endregion

#region Paragraph methods
		public void ParagraphNormalize ()
		{
			LineParagraphNormalize (false);
		}

		public int ParagraphMoveStartPoint (int count)
		{
			int moved = ParagraphMoveStartEndPoint (count, ref start_point);
			if (start_point > end_point) {
				end_point = start_point;
			}
			return moved;
		}
		
		public int ParagraphMoveEndPoint (int count)
		{
			int moved = ParagraphMoveStartEndPoint (count, ref end_point);
			if (end_point < start_point) {
				start_point = end_point;
			}
			return moved;
		}

		private int ParagraphMoveStartEndPoint (int count, ref int point)
		{
			if (count == 0) {
				return 0;
			}

			string text = textboxbase.Text;

			int c = 0, index = 0, n_chars = 0;
			if (count > 0) {
				// walk forward until you see count number of
				// new lines
				for (int i = point; i < text.Length; i++) {
					index = i;

					if (ForwardPeekNewline (i, text, out n_chars)) {
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
				if (c != count && point < (text.Length - 1)) {
					c++;
					index = text.Length;
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
					if (BackwardPeekNewline (i, text, out n_chars)) {
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

			return 0;
		}
#endregion
		
#region Word methods
		private Regex is_separator = new Regex (@"^[^\w]$", RegexOptions.Compiled);

		private bool IsWordSeparator (char c)
		{
			return is_separator.IsMatch (c.ToString ());
		}
		
		public void WordNormalize ()
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

			string text = textboxbase.Text;
			if (IsWordSeparator (text[start_point])) {
				// Walk backwards until you hit a non separator
				for (index = start_point; index >= 0; index--) {
					if (!IsWordSeparator (text[index])) {
						break;
					}
				}
				start_point = index + 1;
			} else {
				// Walk from the start point backwards, finding
				// the last separator
				
				index = -1;
				for (int i = start_point; i >= 0; i--) {
					if (IsWordSeparator (text[i])) {
						index = i;
						break;
					}
				}
				start_point = (index == -1) ? 0 : index + 1;
			}

			index = -1;
			if (end_point > 0 && IsWordSeparator (text[end_point - 1])) {
				// Extend the range to consume all spaces until the next character
				for (index = end_point - 1; index < text.Length; index++) {
					if (!IsWordSeparator (text[index])) {
						break;
					}
				}
				end_point = index;
			} else {
				// Search for the next separator
				index = -1;
				for (int i = end_point; i < text.Length ; i++) {
					if (IsWordSeparator (text[i])) {
						index = i;
						break;
					}
				}

				if (index == -1) {
					end_point = text.Length;
				} else {
					end_point += index - end_point;
				}
			}			
		}

		public int WordMoveStartPoint (int count)
		{
			int moved = WordMoveStartEndPoint (count, ref start_point);
			if (start_point > end_point) {
				end_point = start_point;
			}
			return moved;
		}
		
		public int WordMoveEndPoint (int count)
		{
			int moved = WordMoveStartEndPoint (count, ref end_point);
			if (end_point < start_point) {
				start_point = end_point;
			}
			return moved;
		}
		
		private int WordMoveStartEndPoint (int count, ref int point)
		{
			if (count == 0) {
				return 0;
			}

			int index = 0, c = 0;
			string text = textboxbase.Text;
			if (count > 0) {
				bool last_was_sep = true, last_was_chr = false;
				for (int i = point; i < text.Length; i++) {
					index = i + 1;

					if (IsWordSeparator (text[i])) {
						c++;

						last_was_sep = true;
						last_was_chr = false;
					} else {
						if (last_was_sep) {
							c++;
						}

						last_was_sep = false;
						last_was_chr = true;
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
				if (c < count && point < (text.Length - 1)) {
					c++;
					index = text.Length;
				}

				point = index;
				return c;
			} else {
				bool last_was_sep = true, last_was_chr = false;
				for (int i = point - 1; i >= 0; i--) {
					index = i;

					if (IsWordSeparator (text[i])) {
						c--;

						if (last_was_chr) {
							c--;
						}

						last_was_chr = false;
						last_was_sep = true;
					} else {
						last_was_chr = true;
						last_was_sep = false;
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
			
			return 0;
		}
#endregion

#region Page methods
		public int DocumentMoveStartPoint (int count)
		{
			int length = textboxbase.Text.Length;
			if (count > 0 && start_point < length) {
				start_point = length;
				return 1;
			} else if (count < 0 && start_point > 0) {
				start_point = 0;
				return -1;
			}
			return 0;
		}

		public int DocumentMoveEndPoint (int count)
		{
			int length = textboxbase.Text.Length;
			if (count > 0 && end_point < length) {
				end_point = length;
				return 1;
			} else if (count < 0 && end_point > 0) {
				start_point = end_point = 0;
				return -1;
			}
			return 0;
		}

		public void DocumentNormalize ()
		{
			// Expand range to the document range
			start_point = 0;
			end_point = textboxbase.Text.Length;
		}
#endregion
		
		public TextNormalizerPoints Move (TextUnit unit, int count) 
		{
			// 0 has no effect
			if (count == 0) {
				return new TextNormalizerPoints (start_point, end_point, 0);
			}
			
			int moved = 0;
			switch (unit) {
			case TextUnit.Character:
				// Collapse to a degenerate range
				end_point = start_point;
				
				// Move backward to beginning of boundary

				// Move forward or backward by count
				moved = CharacterMoveStartPoint (count);
				
				// Move endpoint 1 unit
				CharacterMoveEndPoint (1);
				break;
			case TextUnit.Word:
				// Move backward to beginning of boundary
				WordNormalize ();

				// Collapse to a degenerate range
				end_point = start_point;
				
				// Move forward or backward by count
				moved = WordMoveStartPoint (count);
				
				// Move endpoint 1 unit
				WordMoveEndPoint (1);
				break;
			case TextUnit.Line:
				// Move backward to beginning of boundary
				LineNormalize ();

				// Collapse to a degenerate range
				end_point = start_point;
				
				// Move forward or backward by count
				moved = LineMoveStartPoint (count);
				
				// Move endpoint 1 unit
				LineMoveEndPoint (1);
				break;
			case TextUnit.Paragraph:
				// Move backward to beginning of boundary
				ParagraphNormalize ();

				// Collapse to a degenerate range
				end_point = start_point;
				
				// Move forward or backward by count
				moved = ParagraphMoveStartPoint (count);
				
				// Move endpoint 1 unit
				ParagraphMoveEndPoint (1);
				break;
			case TextUnit.Page:
			case TextUnit.Document:
				// Move backward to beginning of boundary
				DocumentNormalize ();

				// Collapse to a degenerate range
				end_point = start_point;
				
				// Move forward or backward by count
				moved = DocumentMoveStartPoint (count);
				
				// Move endpoint 1 unit
				DocumentMoveEndPoint (1);
				break;
			}
			
			return new TextNormalizerPoints (start_point, end_point, moved);
		}


#region Helper methods
		private bool ForwardPeekNewline (int i, string text, out int n_chars)
		{
			n_chars = 0;

			if (i + 1 < text.Length
			    && text[i] == '\r' && text[i+1] == '\n') {
				n_chars = 2;
				return true;
			} else if (text[i] == '\r' || text[i] == '\n') {
				n_chars = 1;
				return true;
			}
			return false;
		}

		private bool BackwardPeekNewline (int i, string text, out int n_chars)
		{
			n_chars = 0;

			if (i - 1 >= 0
			    && text[i] == '\n' && text[i-1] == '\r') {
				n_chars = 2;
				return true;
			} else if (text[i] == '\r' || text[i] == '\n') {
				n_chars = 1;
				return true;
			}
			return false;
		}
#endregion

#region private fields
		private TextBoxBase textboxbase;				
		private int end_point;
		private int start_point;
#endregion
	}
}
