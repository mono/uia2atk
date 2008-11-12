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
// 

using System;
using System.Windows.Automation.Text;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	
	[TestFixture]	
	public class TextNormalizerTest
	{
		private const string character_message = "hello my baby, hello my darling.";

		private const string word_message = "hello   my   baby,   hello my   darling.";
		
		private string message = string.Format ("One morning, when Gregor Samsa   woke from troubled dreams, "
            +"he found himself transformed in his bed into a horrible vermin. {0}{0}He lay on his armour-like back, "
            +"and if he lifted his head a little he could see his brown belly, slightly domed and divided by arches "
            +"into stiff sections. The bedding was hardly able to cover it and seemed ready to slide off any moment. "
            +"His many legs, pitifully thin compared with the size of the rest of him, waved about helplessly as he "
            +"looked. \"What's happened to me? \" he thought. It wasn't a dream. His room, a proper human room although "
            +"a little too small, lay peacefully between its four familiar walls. A collection of textile samples lay "
            +"spread out on the table - Samsa was a travelling salesman - and above it there hung a picture that he had "
            +"recently cut out of an illustrated magazine and housed in a nice, gilded frame. {0}{0}It showed a lady "
            +"fitted out with a fur hat and fur boa who sat upright, raising a heavy fur muff that covered the whole "
            +"of her lower arm towards the viewer. Gregor then turned to look out the window at the dull weather. Drops ",
            Environment.NewLine);
		
#region Line-like-methods Tests
		
		[Test]
		public void LineNormalizeTest ()
		{
			TextBox textbox = new TextBox ();
			textbox.Text = string.Format ("abc{0}de{0}fghi{0}jk", Environment.NewLine);
			TextNormalizer normalizer = new TextNormalizer (textbox, 5, 8);
			
			//Starts: "abc_d{e_f}ghi_jk"
			normalizer.LineNormalize ();
			
			//Isn't multiline
			Assert.AreEqual (0, normalizer.StartPoint, "NotMultiLine StartPoint");
			Assert.AreEqual (textbox.Text.Length, normalizer.EndPoint, "NotMultiLine EndPoint");
			
			normalizer = new TextNormalizer (textbox, 5, 8);
			textbox.Multiline = true;
			//"abc_{de_fghi}_jk"
			normalizer.LineNormalize ();

			Assert.AreEqual (4, normalizer.StartPoint, "MultiLine StartPoint");
			Assert.AreEqual (11, normalizer.EndPoint, "MultiLine EndPoint");
			
			//"{abc_de_f}ghi_jk"
			normalizer = new TextNormalizer (textbox, 0, 8);
			//"{abc_de_fghi}_jk"
			normalizer.LineNormalize ();
			Assert.AreEqual (0, normalizer.StartPoint, "MultiLine StartPoint");
			Assert.AreEqual (11, normalizer.EndPoint, "MultiLine EndPoint");
			
			//"abc_d{e_fghi_jk}"
			normalizer = new TextNormalizer (textbox, 5, 14);
			//"abc_{de_fghi_jk}"
			normalizer.LineNormalize ();
			Assert.AreEqual (4, normalizer.StartPoint, "MultiLine StartPoint");
			Assert.AreEqual (14, normalizer.EndPoint, "MultiLine EndPoint");			
		}
		
#endregion
		
#region Mixing-like Tests
		
		[Test]
		public void MixingTest ()
		{
			TextBox textbox = new TextBox ();
			textbox.Text = "hola mundo!";
			TextNormalizer normalizer = new TextNormalizer (textbox);
			
			//Starts: "{hola mundo!}"
			Assert.AreEqual ("hola mundo!", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "MixingTest. Substring before");
			
			//"hola m{undo!}"
			normalizer.CharacterMoveStartPoint (6);

			Assert.AreEqual (6,  normalizer.StartPoint, "CharacterMoveStartPoint+6. StartPoint");
			Assert.AreEqual (11, normalizer.EndPoint,   "CharacterMoveStartPoint+6. EndPoint");
			Assert.AreEqual ("undo!", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "CharacterMoveStartPoint+6. Substring after");
			//"hola {mundo!}"
			normalizer.WordNormalize ();

			Assert.AreEqual (5,  normalizer.StartPoint, "WordNormalize. StartPoint");
			Assert.AreEqual (11, normalizer.EndPoint,   "WordNormalize. EndPoint");
			Assert.AreEqual ("mundo!", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize. Substring after");
			
			//"hola mundo!{}"
			TextNormalizerPoints points = normalizer.Move (TextUnit.Character, 3);
			Assert.AreEqual (11, points.End,   "Move(Character, 3). End");
			Assert.AreEqual (11, points.Start, "Move(Character, 3). Start");
			Assert.AreEqual (0,  points.Moved, "Move(Character, 3). Moved");
			
			//"h}ola mundo!{"
			Assert.AreEqual (10, normalizer.CharacterMoveEndPoint (-10), "CharacterMoveEndPoint(-10)");
			Assert.AreEqual (1,  normalizer.EndPoint, "CharacterMoveEndPoint-10. End");
			Assert.AreEqual (11, normalizer.StartPoint, "CharacterMoveEndPoint-10. Start");
			
			//"{h}ola mundo!"
			Assert.AreEqual (11, normalizer.CharacterMoveStartPoint (-11), "CharacterMoveStartPoint(-11)");
			Assert.AreEqual (1,  normalizer.EndPoint, "CharacterMoveStartPoint-11. End");
			Assert.AreEqual (0,  normalizer.StartPoint, "CharacterMoveStartPoint-11. Start");
			
			//"{hola} mundo!"
			normalizer.WordNormalize ();
			Assert.AreEqual (4, normalizer.EndPoint,   "WordNormalize. End");
			Assert.AreEqual (0, normalizer.StartPoint, "WordNormalize. Start");			
			Assert.AreEqual ("hola", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize. Substring");			
			
			//"{hola }mundo!"
			Assert.AreEqual (1, normalizer.WordMoveEndPoint (1), "WordMoveEndPoint+1.");
			Assert.AreEqual (5, normalizer.EndPoint, "WordMoveEndPoint+1. End");
			Assert.AreEqual (0, normalizer.StartPoint, "WordMoveEndPoint+1. Start");			
			Assert.AreEqual ("hola ", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordMoveEndPoint+1. Substring");
			
			//"{}hola mundo!"
			Assert.AreEqual (5, normalizer.CharacterMoveEndPoint (-10), "CharacterMoveEndPoint-10");
			Assert.AreEqual ("", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "CharacterMoveEndPoint. Substring");
			
			//"{hola mundo!}"
			Assert.AreEqual (3, normalizer.WordMoveEndPoint (3), "WordMoveEndPoint+3.");
			Assert.AreEqual ("hola mundo!", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordMoveEndPoint+3. Substring");
			
			//"{hola }mundo!"
			Assert.AreEqual (6, normalizer.CharacterMoveEndPoint (-6), "CharacterMoveEndPoint-6.");
			Assert.AreEqual ("hola ", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "CharacterMoveEndPoint-6. Substring");
			
			//"{hola mundo!}"
			Assert.AreEqual (1, normalizer.WordMoveEndPoint (1), "WordMoveEndPoint+1.");
			Assert.AreEqual ("hola mundo!", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordMoveEndPoint+1. Substring");
			
			textbox.Text = "hola      mundo! hello world     !";
			normalizer = new TextNormalizer (textbox);
			
			//"{hola}      mundo! hello world     !"
			Assert.AreEqual (30, normalizer.CharacterMoveEndPoint (-30), "CharacterMoveEndPoint-30.");
			Assert.AreEqual ("hola", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "CharacterMoveEndPoint-30. Substring");
			
			//"{hola      mundo! }hello world     !"
			Assert.AreEqual (3, normalizer.WordMoveEndPoint (3), "WordMoveEndPoint+3.");
			Assert.AreEqual ("hola      mundo! ", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordMoveEndPoint+3. Substring");
			
			//"{hola      mundo! hello world     !}"
			Assert.AreEqual (5, normalizer.WordMoveEndPoint (10), "WordMoveEndPoint+10.");
			Assert.AreEqual ("hola      mundo! hello world     !", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordMoveEndPoint+10. Substring");
		}
		
#endregion 
		
#region Word-like-methods Tests
		
		[Test]
		public void WordMoveEndPointTest ()
		{
			string test_message = "One    morning,   when Gregor Samsa   woke from troubled dreams,";
			TextBox textbox = new TextBox ();
			textbox.Text = test_message;
			TextNormalizer normalizer = new TextNormalizer (textbox, 0, 3);
			
			//Starts: "{One}    morning,   when Gregor Samsa   woke from troubled dreams,"
			Assert.AreEqual ("One", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordMoveEndPointTest. Substring");
			
			//"{One    }morning,   when Gregor Samsa   woke from troubled dreams,"
			Assert.AreEqual (1, normalizer.WordMoveEndPoint (1), "WordMoveEndPoint+1");
			Assert.AreEqual ("One    ", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordMoveEndPointTest+1. Substring");

			//"{One    morning,   when Gregor Samsa   }woke from troubled dreams,"
			Assert.AreEqual (8, normalizer.WordMoveEndPoint (8), "WordMoveEndPoint+8");
			Assert.AreEqual ("One    morning,   when Gregor Samsa   ", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordMoveEndPointTest+8. Substring");
			
			Console.WriteLine ("message '{0}'",
			                   textbox.Text.Substring (normalizer.StartPoint, 
			                                           normalizer.EndPoint - normalizer.StartPoint));
			
			//"{One    morning,   when Gregor Samsa}   woke from troubled dreams,"
			Assert.AreEqual (1, normalizer.WordMoveEndPoint (-1), "WordMoveEndPoint-1");
			Assert.AreEqual ("One    morning,   when Gregor Samsa", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordMoveEndPointTest-1. Substring");

			//"{One    morning,   }when Gregor Samsa   woke from troubled dreams,"
			Assert.AreEqual (5, normalizer.WordMoveEndPoint (-5), "WordMoveEndPoint-5");
			Assert.AreEqual ("One    morning,   ", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordMoveEndPointTest-5. Substring");
		}
		
		[Test]
		public void WordNormalizeTest ()
		{
			TextBox textbox = new TextBox ();
			textbox.Text = message;
			
			//Starts: "On{e morning, wh}en Gregor Samsa   woke from troubled dreams,"
			TextNormalizer normalizer = new TextNormalizer (textbox, 2, 15);
			Assert.AreEqual ("e morning, wh", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize1. Substring before");
			
			//"{One morning, when} Gregor Samsa   woke from troubled dreams,"
			normalizer.WordNormalize ();
			Assert.AreEqual (0,  normalizer.StartPoint, "WordNormalize1. StartPoint");
			Assert.AreEqual (17, normalizer.EndPoint,   "WordNormalize1. EndPoint");
			Assert.AreEqual ("One morning, when", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize1. Substring after");
			
			//Starts: "{One} morning, when Gregor Samsa   woke from troubled dreams,"
			normalizer = new TextNormalizer (textbox, 0, 3);
			Assert.AreEqual ("One", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize2. Substring before");

			//"{One} morning, when Gregor Samsa   woke from troubled dreams,"
			normalizer.WordNormalize ();
			Assert.AreEqual (0, normalizer.StartPoint, "WordNormalize2. StartPoint");
			Assert.AreEqual (3, normalizer.EndPoint,   "WordNormalize2. EndPoint");
			Assert.AreEqual ("One", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize2. Substring after");
			
			//Starts: "One {morning,} when Gregor Samsa   woke from troubled dreams,"
			normalizer = new TextNormalizer (textbox, 4, 12);
			Assert.AreEqual ("morning,", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize3. Substring before");
			//"One {morning,} when Gregor Samsa   woke from troubled dreams,"
			normalizer.WordNormalize ();
			Assert.AreEqual (4,  normalizer.StartPoint, "WordNormalize3. StartPoint");
			Assert.AreEqual (12, normalizer.EndPoint,   "WordNormalize3. EndPoint");
			Assert.AreEqual ("morning,", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize3. Substring after");
			
			//Starts: "One {morning}, when Gregor Samsa   woke from troubled dreams,"
			normalizer = new TextNormalizer (textbox, 4, 11);
			Assert.AreEqual ("morning", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize4. Substring before");
			//"One {morning,} when Gregor Samsa   woke from troubled dreams,"
			normalizer.WordNormalize ();
			Assert.AreEqual ("morning,", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize4. Substring after");
			Assert.AreEqual (4,  normalizer.StartPoint, "WordNormalize4. StartPoint");
			Assert.AreEqual (12, normalizer.EndPoint,   "WordNormalize4. EndPoint");

			//Starts: "One m{orning, when Gregor Samsa }  woke from troubled dreams,"
			normalizer = new TextNormalizer (textbox, 5, 31);
			Assert.AreEqual ("orning, when Gregor Samsa ", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize5. Substring before");
			//"One {morning, when Gregor Samsa   }woke from troubled dreams,"
			normalizer.WordNormalize ();
			Assert.AreEqual (4,  normalizer.StartPoint, "WordNormalize5. StartPoint");
			Assert.AreEqual (33, normalizer.EndPoint,   "WordNormalize5. EndPoint");
			Assert.AreEqual ("morning, when Gregor Samsa   ", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize5. Substring after");

			//Starts: "One morning, when Gregor Samsa {  wo}ke from troubled dreams,"
			normalizer = new TextNormalizer (textbox, 31, 35);
			Assert.AreEqual ("  wo", textbox.Text.Substring (normalizer.StartPoint, 
			                                                 normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormaliz6. Substring before");
			//"One morning, when Gregor Samsa{   woke} from troubled dreams,"
			normalizer.WordNormalize ();
			Assert.AreEqual (30,  normalizer.StartPoint, "WordNormalize6. StartPoint");
			Assert.AreEqual (37, normalizer.EndPoint,    "WordNormalize6. EndPoint");
			Assert.AreEqual ("   woke", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize6. Substring after");
			
			//Starts: "One morning, when Gregor Samsa {  woke }from troubled dreams,"
			normalizer = new TextNormalizer (textbox, 31, 38);
			Assert.AreEqual ("  woke ", textbox.Text.Substring (normalizer.StartPoint, 
			                                                 normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize7. Substring before");
			//"One morning, when Gregor Samsa{   woke }from troubled dreams,"
			normalizer.WordNormalize ();
			Assert.AreEqual (30,  normalizer.StartPoint, "WordNormalize7. StartPoint");
			Assert.AreEqual (38, normalizer.EndPoint,    "WordNormalize7. EndPoint");
			Assert.AreEqual ("   woke ", 
			                 textbox.Text.Substring (normalizer.StartPoint, 
			                                         normalizer.EndPoint - normalizer.StartPoint), 
			                 "WordNormalize7. Substring after");
		}
		
#endregion

#region Character-like-methods Tests

		[Test]
		public void CharacterMoveEndStartPointTest ()
		{
			TextBox textbox = new TextBox ();
			textbox.Text = "hello my baby, hello my darling.";
			
			//Starts: "{}hello my baby, hello my darling."
			TextNormalizer normalizer = new TextNormalizer (textbox, 0, 0);
			
			//"}he{llo my baby, hello my darling."
			Assert.AreEqual (2, normalizer.CharacterMoveStartPoint (2), "CharacterMoveStartPoint + 2");
			Assert.AreEqual (2, normalizer.StartPoint, "CharacterMoveStartPoint + 2. StartPoint");
			Assert.AreEqual (0, normalizer.EndPoint, "CharacterMoveStartPoint + 2. EndPoint");
			
			//"{}hello my baby, hello my darling."
			Assert.AreEqual (2, normalizer.CharacterMoveStartPoint (-3), "CharacterMoveStartPoint - 3");
			Assert.AreEqual (0, normalizer.StartPoint, "CharacterMoveStartPoint - 3. StartPoint");
			Assert.AreEqual (0, normalizer.EndPoint, "CharacterMoveStartPoint - 3. EndPoint");
			
			//"{hello} my baby, hello my darling."
			Assert.AreEqual (5, normalizer.CharacterMoveEndPoint (5), "CharacterMoveEndPoint + 5");
			Assert.AreEqual (0, normalizer.StartPoint, "CharacterMoveStartPoint + 5. StartPoint");
			Assert.AreEqual (5, normalizer.EndPoint, "CharacterMoveStartPoint + 5. EndPoint");

			//"{}hello my baby, hello my darling."
			Assert.AreEqual (5, normalizer.CharacterMoveEndPoint (-6), "CharacterMoveEndPoint - 6");
			Assert.AreEqual (0, normalizer.StartPoint, "CharacterMoveStartPoint - 6. StartPoint");
			Assert.AreEqual (0, normalizer.EndPoint, "CharacterMoveStartPoint - 6. EndPoint");

			//"}hello my baby, hello {my darling."
			Assert.AreEqual (21, normalizer.CharacterMoveStartPoint (21), "CharacterMoveStartPoint + 21");
			Assert.AreEqual (21, normalizer.StartPoint, "CharacterMoveStartPoint + 21. StartPoint");
			Assert.AreEqual (0,  normalizer.EndPoint, "CharacterMoveStartPoint + 21. EndPoint");
			
			//"}hello my baby, hello my darling.{"
			Assert.AreEqual (11, normalizer.CharacterMoveStartPoint (12), "CharacterMoveStartPoint + 12");
			Assert.AreEqual (32, normalizer.StartPoint, "CharacterMoveStartPoint + 12. StartPoint");
			Assert.AreEqual (0,  normalizer.EndPoint, "CharacterMoveStartPoint + 12. EndPoint");
			
			//"}hello my baby, hello my darling.{"
			Assert.AreEqual (0,  normalizer.CharacterMoveEndPoint (0), "CharacterMoveEndPoint + 0");
			Assert.AreEqual (32, normalizer.StartPoint, "CharacterMoveStartPoint + 0. StartPoint");
			Assert.AreEqual (0,  normalizer.EndPoint, "CharacterMoveStartPoint + 0. EndPoint");
			
			//"}hello my baby, hello my darling.{"
			Assert.AreEqual (0,  normalizer.CharacterMoveStartPoint (0), "CharacterMoveEndPoint + 0");
			Assert.AreEqual (32, normalizer.StartPoint, "CharacterMoveStartPoint + 0. StartPoint");
			Assert.AreEqual (0,  normalizer.EndPoint, "CharacterMoveStartPoint + 0. EndPoint");			
			
			//"hello my} baby, hello my darling.{"
			Assert.AreEqual (8,  normalizer.CharacterMoveEndPoint (8), "CharacterMoveEndPoint + 8");
			Assert.AreEqual (32, normalizer.StartPoint, "CharacterMoveEndPoint + 8. StartPoint");
			Assert.AreEqual (8,  normalizer.EndPoint, "CharacterMoveEndPoint + 8. EndPoint");
			
			//"hello {my} baby, hello my darling."
			Assert.AreEqual (26, normalizer.CharacterMoveStartPoint (-26), "CharacterMoveStartPoint - 26");
			Assert.AreEqual (6,  normalizer.StartPoint, "CharacterMoveStartPoint - 26. StartPoint");
			Assert.AreEqual (8,  normalizer.EndPoint, "CharacterMoveStartPoint - 26. EndPoint");
		}
		
		[Test]
		public void CharacterMovePositiveTest ()
		{
			TextBox textbox = new TextBox ();
			textbox.Text = "abcdefghijk";
			TextNormalizer normalizer = new TextNormalizer (textbox, 3, 6);
			TextNormalizerPoints points;
			
			//Starts: "abc{def}ghijk"
			
			//Should change to: "abcdefghij{}k"
			points = normalizer.Move (TextUnit.Character, 4);

			Assert.AreEqual (10, points.Start, "Start");
			Assert.AreEqual (10, points.End,   "End");
			Assert.AreEqual (4,  points.Moved, "Moved");
			
			//Should change to: "abcdefghijk{}"
			points = normalizer.Move (TextUnit.Character, 7);
			Assert.AreEqual (11, points.Start, "Start");
			Assert.AreEqual (11, points.End,   "End");
			Assert.AreEqual (1,  points.Moved, "Moved");
			
			//Should not change"
			points = normalizer.Move (TextUnit.Character, 0);
			Assert.AreEqual (11, points.Start, "Start");
			Assert.AreEqual (11, points.End,   "End");
			Assert.AreEqual (0,  points.Moved, "Moved");
		}

		[Test]
		public void CharacterMoveNegativeTest ()
		{
			TextBox textbox = new TextBox ();
			textbox.Text = "abcdefghijk";
			TextNormalizer normalizer = new TextNormalizer (textbox, 3, 6);
			TextNormalizerPoints points;
			
			//Starts: "abc{def}ghijk"
			
			//Should change to: "a{}bcdefghijk"
			points = normalizer.Move (TextUnit.Character, -2);
			Assert.AreEqual (1, points.Start, "Start");
			Assert.AreEqual (1, points.End,   "End");
			Assert.AreEqual (2, points.Moved, "Moved");
			
			//Should change to: "{}abcdefghijk"
			points = normalizer.Move (TextUnit.Character, -4);
			Assert.AreEqual (0, points.Start, "Start");
			Assert.AreEqual (0, points.End,   "End");
			Assert.AreEqual (1, points.Moved, "Moved");
			
			//Should not change: "{}abcdefghijk"
			points = normalizer.Move (TextUnit.Character, 0);
			Assert.AreEqual (0, points.Start, "Start");
			Assert.AreEqual (0, points.End,   "End");
			Assert.AreEqual (0, points.Moved, "Moved");
		}

#endregion

	}
}
