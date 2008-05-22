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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;

using NUnit.Framework;

namespace UiaAtkBridgeTest
{
	
	public abstract class AtkTester {
		
		public abstract object GetAtkObjectThatImplementsInterface <I> ();
		
	
		private static AtkTester instance;
		public static AtkTester Instance {
			get { return instance; }
		}
		
		public AtkTester () {
			instance = this;
		}
		
		public string Text {
			get { return "This is a test sentence.\r\nSecond line. Other phrase.\nThird line?"; }
		}
		
		[Test]
		public void AtkTextImplementor ()
		{
			Atk.Text atkText = (Atk.Text)
				GetAtkObjectThatImplementsInterface <Atk.Text> ();
			Assert.AreEqual (0, atkText.CaretOffset, "CaretOffset");
			Assert.AreEqual (Text.Length, atkText.CharacterCount, "CharacterCount");
			Assert.AreEqual (Text [0], atkText.GetCharacterAtOffset (0), "GetCharacterAtOffset");
			Assert.AreEqual (Text, atkText.GetText (0, Text.Length), "GetText");
			
			//any value
			Assert.AreEqual (false, atkText.SetCaretOffset (-1));
			Assert.AreEqual (false, atkText.SetCaretOffset (0));
			Assert.AreEqual (false, atkText.SetCaretOffset (1));
			Assert.AreEqual (false, atkText.SetCaretOffset (15));
			
			// don't do this until bug#393565 is fixed:
			//Assert.AreEqual (typeof(Atk.TextAttribute), atkText.DefaultAttributes[0].GetType());

			Assert.AreEqual (0, atkText.NSelections);
			
			// you cannot select a label AFAIK so, all zeroes returned!
			int startOffset, endOffset;
			atkText.GetSelection (0, out startOffset, out endOffset);
			Assert.AreEqual (0, startOffset);
			Assert.AreEqual (0, endOffset);
			atkText.GetSelection (1, out startOffset, out endOffset);
			Assert.AreEqual (0, startOffset);
			Assert.AreEqual (0, endOffset);
			atkText.GetSelection (-1, out startOffset, out endOffset);
			Assert.AreEqual (0, startOffset);
			Assert.AreEqual (0, endOffset);
			
			// you cannot select a label AFAIK so, false always returned!
			Assert.AreEqual (false, atkText.SetSelection (0, 1, 2));
			// test GetSelection *after* SetSelection
			atkText.GetSelection (0, out startOffset, out endOffset);
			Assert.AreEqual (0, startOffset);
			Assert.AreEqual (0, endOffset);
			//test crazy numbers for SetSelection
			Assert.AreEqual (false, atkText.SetSelection (-3, 10, -2));
			atkText.GetSelection (0, out startOffset, out endOffset);
			Assert.AreEqual (0, startOffset);
			Assert.AreEqual (0, endOffset);
			
			//did NSelections changed?
			Assert.AreEqual (false, atkText.SetSelection (1, 2, 3));
			Assert.AreEqual (0, atkText.NSelections);
			Assert.AreEqual (false, atkText.RemoveSelection (0));
			Assert.AreEqual (0, atkText.NSelections);
			Assert.AreEqual (false, atkText.RemoveSelection (1));
			Assert.AreEqual (0, atkText.NSelections);
			Assert.AreEqual (false, atkText.RemoveSelection (-1));
			Assert.AreEqual (0, atkText.NSelections);
			
			//GetTextAfterOffset: trickyness in itself
			string expected = " sentence";
			Assert.AreEqual (expected, 
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.WordEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,WordEnd");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextAfterOffset,WordEnd");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,WordEnd");
			
			expected = "sentence.\r\n";
			Assert.AreEqual (expected, 
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.WordStart, out startOffset, out endOffset),
				"GetTextAfterOffset,WordStart");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextAfterOffset,WordStart");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,WordStart");
			
			expected = "\r\nSecond line. Other phrase.";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.LineEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,LineEnd");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextAfterOffset,LineEnd");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,LineEnd");

			expected = "Second line. Other phrase.\n";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.LineStart, out startOffset, out endOffset),
				"GetTextAfterOffset,LineStart");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextAfterOffset,LineStart");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,LineStart");
			
			expected = "\r\nSecond line.";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (18, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,SentenceEnd");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextAfterOffset,SentenceEnd");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,SentenceEnd");
			
			expected = "Second line. ";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (18, Atk.TextBoundary.SentenceStart, out startOffset, out endOffset),
				"GetTextAfterOffset,SentenceStart");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextAfterOffset,SentenceStart");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,SentenceStart");
			
			Assert.AreEqual ("e",
				atkText.GetTextAfterOffset (18, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (19, startOffset, "GetTextAfterOffset,Char");
			Assert.AreEqual (20, endOffset, "GetTextAfterOffset,Char");
			Assert.AreEqual ("\r",
				atkText.GetTextAfterOffset (23, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (24, startOffset, "GetTextAfterOffset,Char");
			Assert.AreEqual (25, endOffset, "GetTextAfterOffset,Char");
			Assert.AreEqual ("?",
				atkText.GetTextAfterOffset (Text.Length - 2, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (Text.Length - 1, startOffset, "GetTextAfterOffset,Char");
			Assert.AreEqual (Text.Length, endOffset, "GetTextAfterOffset,Char");
			Assert.AreEqual (String.Empty,
				atkText.GetTextAfterOffset (Text.Length - 1, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (Text.Length, startOffset, "GetTextAfterOffset,Char");
			Assert.AreEqual (Text.Length, endOffset, "GetTextAfterOffset,Char");
			Assert.AreEqual (String.Empty,
				atkText.GetTextAfterOffset (Text.Length, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (Text.Length, startOffset, "GetTextAfterOffset,Char");
			Assert.AreEqual (Text.Length, endOffset, "GetTextAfterOffset,Char");
			Assert.AreEqual (null,
				atkText.GetTextAfterOffset (-1, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (Text.Length, startOffset, "GetTextAfterOffset,Char");
			Assert.AreEqual (Text.Length, endOffset, "GetTextAfterOffset,Char");
			
			
			//GetTextAtOffset
			expected = " test";
			Assert.AreEqual (expected, 
				atkText.GetTextAtOffset (12, Atk.TextBoundary.WordEnd, out startOffset, out endOffset),
				"GetTextAtOffset,WordEnd");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextAtOffset,WordEnd");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,WordEnd");
			
			expected = "test ";
			Assert.AreEqual (expected, 
				atkText.GetTextAtOffset (12, Atk.TextBoundary.WordStart, out startOffset, out endOffset),
				"GetTextAtOffset,WordStart");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextAtOffset,WordStart");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,WordStart");
			
			expected = "This is a test sentence.";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (12, Atk.TextBoundary.LineEnd, out startOffset, out endOffset),
				"GetTextAtOffset,LineEnd");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextAtOffset,LineEnd");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,LineEnd");

			expected = "This is a test sentence.\r\n";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (12, Atk.TextBoundary.LineStart, out startOffset, out endOffset),
				"GetTextAtOffset,LineStart");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextAtOffset,LineStart");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,LineStart");
			
			expected = "This is a test sentence.";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (18, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextAtOffset,SentenceEnd");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextAtOffset,SentenceEnd");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,SentenceEnd");
			
			expected = "This is a test sentence.\r\n";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (18, Atk.TextBoundary.SentenceStart, out startOffset, out endOffset),
				"GetTextAtOffset,SentenceStart");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextAtOffset,SentenceStart");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,SentenceStart");
			
			Assert.AreEqual ("t",
				atkText.GetTextAtOffset (18, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (18, startOffset, "GetTextAtOffset,Char");
			Assert.AreEqual (19, endOffset, "GetTextAtOffset,Char");
			Assert.AreEqual (".",
				atkText.GetTextAtOffset (23, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (23, startOffset, "GetTextAtOffset,Char");
			Assert.AreEqual (24, endOffset, "GetTextAtOffset,Char");
			Assert.AreEqual ("e",
				atkText.GetTextAtOffset (Text.Length - 2, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (Text.Length - 2, startOffset, "GetTextAtOffset,Char");
			Assert.AreEqual (Text.Length - 1, endOffset, "GetTextAtOffset,Char");
			Assert.AreEqual ("?",
				atkText.GetTextAtOffset (Text.Length - 1, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (Text.Length - 1, startOffset, "GetTextAtOffset,Char");
			Assert.AreEqual (Text.Length, endOffset, "GetTextAtOffset,Char");
			Assert.AreEqual (String.Empty,
				atkText.GetTextAtOffset (Text.Length, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (Text.Length, startOffset, "GetTextAtOffset,Char");
			Assert.AreEqual (Text.Length, endOffset, "GetTextAtOffset,Char");
			Assert.AreEqual (null,
				atkText.GetTextAtOffset (-1, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (Text.Length, startOffset, "GetTextAtOffset,Char");
			Assert.AreEqual (Text.Length, endOffset, "GetTextAtOffset,Char");
			
			//GetTextBeforeOffset
			expected = " a";
			Assert.AreEqual (expected, 
				atkText.GetTextBeforeOffset (12, Atk.TextBoundary.WordEnd, out startOffset, out endOffset),
				"GetTextBeforeOffset,WordEnd");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextBeforeOffset,WordEnd");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,WordEnd");
			
			expected = "a ";
			Assert.AreEqual (expected, 
				atkText.GetTextBeforeOffset (12, Atk.TextBoundary.WordStart, out startOffset, out endOffset),
				"GetTextBeforeOffset,WordStart");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextBeforeOffset,WordStart");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,WordStart");
			
			expected = String.Empty;
			Assert.AreEqual (expected,
				atkText.GetTextBeforeOffset (12, Atk.TextBoundary.LineEnd, out startOffset, out endOffset),
				"GetTextBeforeOffset,LineEnd");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextBeforeOffset,LineEnd");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,LineEnd");

			Assert.AreEqual (expected,
				atkText.GetTextBeforeOffset (12, Atk.TextBoundary.LineStart, out startOffset, out endOffset),
				"GetTextBeforeOffset,LineStart");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextBeforeOffset,LineStart");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,LineStart");
			
			Assert.AreEqual (expected,
				atkText.GetTextBeforeOffset (18, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextBeforeOffset,SentenceEnd");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextBeforeOffset,SentenceEnd");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,SentenceEnd");
			
			Assert.AreEqual (expected,
				atkText.GetTextBeforeOffset (18, Atk.TextBoundary.SentenceStart, out startOffset, out endOffset),
				"GetTextBeforeOffset,SentenceStart");
			Assert.AreEqual (Text.IndexOf (expected), startOffset, "GetTextBeforeOffset,SentenceStart");
			Assert.AreEqual (Text.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,SentenceStart");
			
			Assert.AreEqual ("n",
				atkText.GetTextBeforeOffset (18, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (17, startOffset, "GetTextBeforeOffset,Char");
			Assert.AreEqual (18, endOffset, "GetTextBeforeOffset,Char");
			Assert.AreEqual ("e",
				atkText.GetTextBeforeOffset (23, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (22, startOffset, "GetTextBeforeOffset,Char");
			Assert.AreEqual (23, endOffset, "GetTextBeforeOffset,Char");
			Assert.AreEqual ("n",
				atkText.GetTextBeforeOffset (Text.Length - 2, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (Text.Length - 3, startOffset, "GetTextBeforeOffset,Char");
			Assert.AreEqual (Text.Length - 2, endOffset, "GetTextBeforeOffset,Char");
			Assert.AreEqual ("e",
				atkText.GetTextBeforeOffset (Text.Length - 1, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (Text.Length - 2, startOffset, "GetTextBeforeOffset,Char");
			Assert.AreEqual (Text.Length - 1, endOffset, "GetTextBeforeOffset,Char");
			Assert.AreEqual ("?",
				atkText.GetTextBeforeOffset (Text.Length, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (Text.Length - 1, startOffset, "GetTextBeforeOffset,Char");
			Assert.AreEqual (Text.Length, endOffset, "GetTextBeforeOffset,Char");
			Assert.AreEqual (null,
				atkText.GetTextBeforeOffset (-1, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (Text.Length - 1, startOffset, "GetTextBeforeOffset,Char");
			Assert.AreEqual (Text.Length, endOffset, "GetTextBeforeOffset,Char");
		}
	}
}
