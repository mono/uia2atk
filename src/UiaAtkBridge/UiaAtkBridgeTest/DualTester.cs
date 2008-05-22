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
	
	public interface IAtkTester
	{
		// how to do this interace method? invalid example:
		object GetAtkObjectThatImplementsInterface <I> ();
	}
	
	[TestFixture]
	public class DualTester {
		
		IAtkTester[] typesToTest =
			new IAtkTester[] { //new BridgeTester (), 
								new GailTester () };
		
		public static string Text {
			get { return "This is a test sentence.\r\nSecond line. Other sentence.\nThird line?"; }
		}
		
		[Test]
		public void AtkTextImplementor ()
		{
			foreach (IAtkTester tester in typesToTest)
			{
				Atk.Text atkText = (Atk.Text)
					tester.GetAtkObjectThatImplementsInterface <Atk.Text> ();
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
				
				//GetTextAfterOffset: trickyness in itself
				Assert.AreEqual (" sentence", 
					atkText.GetTextAfterOffset (12, Atk.TextBoundary.WordEnd, out startOffset, out endOffset));
				Assert.AreEqual ("sentence.\r\n", 
					atkText.GetTextAfterOffset (12, Atk.TextBoundary.WordStart, out startOffset, out endOffset));
				Assert.AreEqual ("\r\nSecond line. Other sentence.",
					atkText.GetTextAfterOffset (12, Atk.TextBoundary.LineEnd, out startOffset, out endOffset));
				Assert.AreEqual ("Second line. Other sentence.\n",
					atkText.GetTextAfterOffset (12, Atk.TextBoundary.LineStart, out startOffset, out endOffset));
				Assert.AreEqual ("\r\nSecond line.",
					atkText.GetTextAfterOffset (18, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset));
				Assert.AreEqual ("Second line. ",
					atkText.GetTextAfterOffset (18, Atk.TextBoundary.SentenceStart, out startOffset, out endOffset));
				Assert.AreEqual ("e",
					atkText.GetTextAfterOffset (18, Atk.TextBoundary.Char, out startOffset, out endOffset));
				Assert.AreEqual ("\r",
					atkText.GetTextAfterOffset (23, Atk.TextBoundary.Char, out startOffset, out endOffset));
				Assert.AreEqual ("?",
					atkText.GetTextAfterOffset (Text.Length - 2, Atk.TextBoundary.Char, out startOffset, out endOffset));
				Assert.AreEqual (String.Empty,
					atkText.GetTextAfterOffset (Text.Length - 1, Atk.TextBoundary.Char, out startOffset, out endOffset));
				Assert.AreEqual (String.Empty,
					atkText.GetTextAfterOffset (Text.Length, Atk.TextBoundary.Char, out startOffset, out endOffset));


			}
		}
	}
}
