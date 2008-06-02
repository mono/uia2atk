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

namespace Mono.UIAutomation.Winforms
{

	//Algorithm:
	//http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.itextrangeprovider.move.aspx
	public class TextNormalizer
	{
		public TextNormalizer (string text, int startPoint, int length)
		{
			this.text = text;
			start_point = startPoint;
			this.length = length;
		}
		
		public TextNormalizerPoints Normalize (TextUnit unit, int count) 
		{
			if (count == 0)
				return new TextNormalizerPoints (start_point, length, 0);

			if (unit == TextUnit.Character)
				return NormalizeCharacter (count);
			//Format
			else if (unit == TextUnit.Word)
				return NormalizeWord (count);
			else if (unit == TextUnit.Line)
				return NormalizeLine (count);
			else if (unit == TextUnit.Paragraph)
				return NormalizeParagraph (count);
			//Page
			//Document
				
			return new TextNormalizerPoints (0, 0, 0);
		}

		// Based on the complete algorithm and using: 
		//	text:	"hello {my b}aby, hello my darling." 
		//		(start = 6, length = 3)
		//	count:	+5
		// The results are:
		//	1. "hello {}my baby, hello my darling."
		//	2. "hello {}my baby, hello my darling."
		//	3. "hello }my b{aby, hello my darling."
		// 	4. "hello my b{a}by, hello my darling."
		//		(start = 10, length = 1)
		// TODO: Test against Microsoft behaviour
		private TextNormalizerPoints NormalizeCharacter (int count)
		{
			int new_start_point = start_point;
			int moved = 0;

			new_start_point += count; // Step 3
			
			if (new_start_point < 0) {
				new_start_point = 0;
				moved = start_point;
			} else
				moved = System.Math.Abs (count);
			
			return new TextNormalizerPoints (new_start_point, 1, moved);
		}
		
		// Based on the complete algorithm and using: 
		//	text:	"hello m{y baby, h}ello my darling." 
		//		(start = 7, length = 9)
		//	count:	+3
		// The results are:
		//	1. "hello m{}y baby, hello my darling."
		//	2. "hello {m}y baby, hello my darling."
		//	3. "hello m}y baby, {hello my darling."
		// 	4. "hello my baby, {hello} my darling."
		//		(start = 16, length = 5)
		// TODO: Test against Microsoft behaviour
		private TextNormalizerPoints NormalizeWord (int count)
		{
			return NormalizeWithSeparator (count, " ");
		}

		private TextNormalizerPoints NormalizeLine (int count) 
		{
			return NormalizeWithSeparator (count, string.Format ("{0}", Environment.NewLine));
		}
		
		private TextNormalizerPoints NormalizeParagraph (int count) 
		{
			return NormalizeWithSeparator (count, string.Format ("{0}{0}", Environment.NewLine));
		}
		
		private TextNormalizerPoints NormalizeWithSeparator (int count, string separator)
		{
			//TODO: Improve performance.
			int new_start_point = 0;
			int new_length_point = 0;
			int new_moved_point = 0;

			//Step 1, 2
			string []split = text.Substring (0, start_point).Split (new string[] { separator }, StringSplitOptions.None);
			
			if (split.Length == 1)
				new_start_point = 0;
			else 
				new_start_point = start_point - split [split.Length - 1].Length;
			
			// Step 3, 4
			string []sub_strings;
			int word = 0;
			int index = 0;
			int length = 0;
			if (count > 0) {
				sub_strings = text.Substring (new_start_point).Split (new string[] { separator }, StringSplitOptions.None);

				foreach (string str in sub_strings) {
					length += (str.Length == 0 ? 1 : str.Length);
					if (word == count)
						break;
					index++;
					if (string.IsNullOrEmpty (str) == false)
						word++;
				}
				
				if (index >= sub_strings.Length) {
					new_start_point = text.Length;
					new_length_point = 0;
				} else {
					new_start_point += length;
					new_length_point = sub_strings [index].Length;
				}
				new_moved_point = word;
			} else {
//				Console.WriteLine ("\n'{0}'\n", text.Substring (0, new_start_point));

				sub_strings = text.Substring (0, new_start_point).Split (new string[] { separator }, StringSplitOptions.None);
				int abs_count = System.Math.Abs (count);
				int i = 0;
				for (i = sub_strings.Length - 1; i >= 0; i--) {
					length += (sub_strings [i].Length == 0 ? 1 : sub_strings [i].Length);
					//This is because when splitting the separator is removed once a 
					//word is found
					if (sub_strings [i].Length > 0 && word > 0)
						length++;
					if (string.IsNullOrEmpty (sub_strings [i]) == false)
						word++;
//					Console.WriteLine ("LENGTH: {0} = '{1}'", length, sub_strings [i]);
					if (word == abs_count) {
//						Console.WriteLine ("Ya!");
						break;
					}
				}
				
//				Console.WriteLine ("LENGTJH: {0} = {1} = {2}", length, new_start_point - length, new_start_point);
				
				if (i < 0) {
//					Console.WriteLine ("here");
					new_start_point = 0;
					new_length_point = 0;
				} else {
					new_start_point -= length;
					new_length_point = sub_strings [i].Length;
				}
				new_moved_point = word;

				//Console.WriteLine ("Word: '{0}' Le = {1}", text.Substring (new_start_point, new_length_point), new_length_point);

//				foreach (string str in sub_strings) {
//					Console.WriteLine ("Str-: '{0}'", str);
//				}
			}
			
			// TODO: REMOVE
//			string []split_n = text.Split (new string [] { separator }, StringSplitOptions.None);			
//			foreach (string str in split_n) {
//				Console.WriteLine ("Str: '{0}'", str);
//			}
			
			return new TextNormalizerPoints (new_start_point, new_length_point, 
			                                 new_moved_point);
		}
		
		private string text;
		private int start_point;
		private int length;
	}
}
