//
// MonoHotDraw. Diagramming library
//
// Authors:
//	Mario Carrión <mario@monouml.org>
//
// Copyright (C) 2006, 2007, 2008 MonoUML Team (http://www.monouml.org)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Windows.Automation.Text;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{

	//Algorithm:
	//http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.itextrangeprovider.move.aspx
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
		}
		
		public int EndPoint {
			get { return end_point; }
		}
		
#endregion
		
#region Character methods

		public int CharacterMoveEndPoint (int count) 
		{
			return CharacterMoveStartEndPoint (count, ref end_point);
		}
		
		public int CharacterMoveStartPoint (int count)
		{
			return CharacterMoveStartEndPoint (count, ref start_point);
		}
		
		//FIXME: ?
		//Be aware of this method because it returns: "the number of units actually moved."
		//http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.itextrangeprovider.move.aspx
		//and MS implementation doesn't
		private int CharacterMoveStartEndPoint (int count, ref int startEndPoint)
		{
			int moved = 0;
			
			if (count == 0)
				return 0;
			
			if (startEndPoint + count <= 0)
				moved = startEndPoint;
			else if (startEndPoint + count >= textboxbase.Text.Length)
				moved = startEndPoint - textboxbase.Text.Length;
			else 
				moved = count;
			
			startEndPoint += count;
			if (startEndPoint < 0)
				startEndPoint = 0;
			else if (startEndPoint > textboxbase.Text.Length)
				startEndPoint = textboxbase.Text.Length;

			return System.Math.Abs (moved);
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
			if (textboxbase.Multiline == false) {
				start_point = 0;
				end_point = textboxbase.Text.Length;
				return;
			} 

			int startIndex = textboxbase.Text.LastIndexOf (Environment.NewLine, 
								       start_point, 
								       start_point);
			int endIndex = textboxbase.Text.IndexOf (Environment.NewLine,
								 end_point);
			
			bool startFound = false;
			bool endFound = false;
			
			if (startIndex == -1) {
				start_point = 0;
				startFound = true;
			} 
			
			if (endIndex == -1) {
				end_point = textboxbase.Text.Length;
				endFound = true;
			}
			
			if (endFound == false || startFound == false) {
				int length = 0;

				startIndex++; //was found, so we need to increase Lines' index.

				//TODO: Optimize?
				for (int index = 0; index < textboxbase.Lines.Length; index++) {
					if (length == startIndex && startFound == false) {
						start_point = length;
						startFound = true;
					}
					length += textboxbase.Lines [index].Length;
					if (length == endIndex && endFound == false) {
						end_point = length;
						endFound = true;
						break;
					}
					length++;
				}
			}
		}

		public int LineMoveEndPoint (int count)
		{
			return LineMoveStartEndPoint (count, ref end_point);
		}
		
		public int LineMoveStartPoint (int count)
		{
			return LineMoveStartEndPoint (count, ref start_point);
		}

		private int LineMoveStartEndPoint (int count, ref int start_end_point)
		{
			if (count == 0) {
				return 0;
			}

			string text = textboxbase.Text;

			int c = 0, index = -1;
			if (count > 0) {
				// walk forward until you see count number of new lines
				for (int i = start_end_point; i < text.Length; i++) {
					if (text[i] == Environment.NewLine[0]) {
						c++;
						index = i + 1;

						if (c == count) {
							break;
						}
					}
				}

				start_end_point = index;
				return c;
			} else {
				// walk backward until you see count number of new lines
				for (int i = start_end_point - 1; i >= 0; i--) {
					if (text[i] == Environment.NewLine[0]) {
						c--;
						index = i;

						if (c == count) {
							break;
						}
					}
				}

				// TODO: Investigate why we don't do this for
				// the count > 0 case

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

		public int ParagraphMoveStartPoint (int count)
		{
			return ParagraphMoveStartEndPoint (count, ref start_point);
		}
		
		public int ParagraphMoveEndPoint (int count)
		{
			return ParagraphMoveStartEndPoint (count, ref end_point);
		}

		public int ParagraphMoveStartEndPoint (int count, ref int point)
		{
			if (count == 0) {
				return 0;
			}

			string text = textboxbase.Text;

			int c = 0, index = 0;
			if (count > 0) {
				// walk forward until you see count number of
				// new lines
				for (int i = point; i < text.Length; i++) {
					index = i;

					if (text[i] == Environment.NewLine[0]) {
						c++;

						if (c == count) {
							index = i + 1;
							break;
						}
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
					if (text[i] == Environment.NewLine[0]) {
						c--;
						
						if (c == (count - 1)) {
							index = i + 1;
							c++;
							break;
						}
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
		
		public void WordNormalize ()
		{
			char separator = ' ';
			int index = 0;
			
			// NOTE: There is a particular condition when either the StartPoint
			// or EndPoint are spaces, for example:
			//      "Hello my baby, hello  {   my d}arling"
			// After normalizing string the result is:
			//      "Hello my baby, hello{     my darling}"
			//
			// However the string:
			//      "Hello my baby, hello     {my d}arling"
			// is:
			//      "Hello my baby, hello     {my darling}"

			string text = textboxbase.Text;
			if (text [start_point] == separator) {
				//TODO: Evaluate perfomance
				
				// Walk backwards until you hit a non separator
				for (index = start_point; index >= 0; index--) {
					if (text [index] != separator)
						break;
				}
				start_point = index + 1;
			} else {
				// Walk from the start point backwards, finding
				// the last separator
				index = text.LastIndexOf (separator, start_point, start_point);
				start_point = (index == -1) ? 0 : index + 1;
			}

			if (end_point > 0 && text [end_point - 1] == separator) {
				//TODO: Evaluate perfomance
				
				// Extend the range to consume all spaces until the next character
				for (index = end_point - 1; index < text.Length; index++) {
					if (text [index] != separator)
						break;
				}
				end_point = index;
			} else {
				// Search for the next separator
				index = text.IndexOf (separator, end_point);
				if (index == -1) {
					end_point = text.Length;
				} else {
					end_point += index - end_point;
				}
			}			
		}

		public int WordMoveEndPoint (int count)
		{
			return WordMoveStartEndPoint (count, ref end_point);
		}
		
		public int WordMoveStartPoint (int count)
		{
			return WordMoveStartEndPoint (count, ref start_point);
		}
		
		private int WordMoveStartEndPoint (int count, ref int startEndPoint)
		{
			//Be aware that both Start and End points must be normalized
			//otherwise there won't be valid results 
			if (count == 0)
				return 0;

			WordTokenizer tokenizer = new WordTokenizer (textboxbase.Text);
			WordTokenCollection collection;
			
			if (count > 0)
				collection = tokenizer.Forward (startEndPoint, count);
			else
				collection = tokenizer.Backwards (startEndPoint - 1, System.Math.Abs (count));
			
			if (collection.Count > 0) {
				if (count > 0)
					startEndPoint = collection [collection.Count - 1].Index 
						+ collection [collection.Count - 1].Message.Length;
				else
					startEndPoint = collection [collection.Count - 1].Index;
			}
			
			return collection.Count;
		}
		
#endregion
		
		public TextNormalizerPoints Move (TextUnit unit, int count) 
		{
			if (count == 0)
				return new TextNormalizerPoints (start_point, end_point, 0);
			
			int moved = 0;
			if (unit == TextUnit.Character) {
				if (count > 0) {
					moved = CharacterMoveEndPoint (count);
					start_point = end_point;
				} else {
					moved = CharacterMoveStartPoint (count);
					end_point = start_point;
				}
			} else if (unit == TextUnit.Word) {
				WordNormalize ();
				if (count > 0) {
					moved = WordMoveEndPoint (count);
					start_point = end_point;
				} else {
					moved = WordMoveEndPoint (count);
					end_point = start_point;
				}
			} else if (unit == TextUnit.Line) {
				LineNormalize ();
				//TODO: Add missing logic
			}
			
			return new TextNormalizerPoints (start_point, end_point, moved);
		}

#region Private fields
		
		private TextBoxBase textboxbase;				
		private int end_point;
		private int start_point;

#endregion
		
	}
}
