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
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	
	[TestFixture]	
	public class TextNormalizerTest
	{
		private const string character_message = "hello my baby, hello my darling.";

		private const string word_message = "hello   my   baby,   hello my   darling.";

#region TextUnit.Character Tests
		
		[Test]
		public void TextUnitCharacterTestPositive ()
		{
			//"hello {my} baby, hello my darling."
			TextNormalizer normalizer = new TextNormalizer (character_message, 6, 8);
			//"hello my b{a}by, hello my darling."
			TextNormalizerPoints result = normalizer.Normalize (TextUnit.Character, 5);
			TextNormalizerPoints points = new TextNormalizerPoints (11, 1, 5);
			
			Assert.AreEqual (points.Start,  result.Start,  "Start");
			Assert.AreEqual (points.Length, result.Length, "Length");
			Assert.AreEqual (points.Moved,  result.Moved,  "Moved");
		}
		
		[Test]
		public void TextUnitCharacterTestNegative ()
		{
			//"hello {my} baby, hello my darling."
			TextNormalizer normalizer = new TextNormalizer (character_message, 6, 8);
			//"{h}ello my baby, hello my darling."
			TextNormalizerPoints result = normalizer.Normalize (TextUnit.Character, -10);
			TextNormalizerPoints points = new TextNormalizerPoints (0, 1, 6);
			
			Assert.AreEqual (points.Start,  result.Start,  "Start");
			Assert.AreEqual (points.Length, result.Length, "Length");
			Assert.AreEqual (points.Moved,  result.Moved,  "Moved");
		}
		
		[Test]
		public void TextUnitCharacterTestZero ()
		{
			//"hello {my} baby, hello my darling."
			TextNormalizer normalizer = new TextNormalizer (character_message, 6, 8);
			//"hello {my} baby, hello my darling."
			TextNormalizerPoints result = normalizer.Normalize (TextUnit.Character, 0);
			TextNormalizerPoints points = new TextNormalizerPoints (6, 8, 0);
			
			Assert.AreEqual (points.Start,  result.Start,  "Start");
			Assert.AreEqual (points.Length, result.Length, "Length");
			Assert.AreEqual (points.Moved,  result.Moved,  "Moved");
		}
		
		[Test]
		public void TextUnitCharacterTestComplex ()
		{
			//"hello {my b}aby, hello my darling."
			TextNormalizer normalizer = new TextNormalizer (character_message, 6, 9);
			//"hello my b{a}by, hello my darling."
			TextNormalizerPoints result = normalizer.Normalize (TextUnit.Character, 5);
			TextNormalizerPoints points = new TextNormalizerPoints (11, 1, 5);
			
			Assert.AreEqual (points.Start,  result.Start,  "Start");
			Assert.AreEqual (points.Length, result.Length, "Length");
			Assert.AreEqual (points.Moved,  result.Moved,  "Moved");
		}

#endregion

#region TextUnit.Word Test

		//[Test]
		public void TextUnitWordTestPositive ()
		{
			//"hello   my   baby,   hello my   darling.";

			//"hel{lo   m}y   baby,   hello my   darling."; //3 = 0
			//"hello   {m}y   baby,   hello my   darling."; //8 = 8
			//"hello   m{y}   baby,   hello my   darling."; //9 = 8
			//"hello   my   ba{b}y,   hello my   darling."; //16 = 13
			
			//"hello   my   baby{,   hel}lo my   darling."
			TextNormalizer normalizer = new TextNormalizer (word_message, 17, 7);
			//"hello   my   baby,   hello {my}   darling."
			TextNormalizerPoints result = normalizer.Normalize (TextUnit.Word, 2);
			TextNormalizerPoints points = new TextNormalizerPoints (27, 2, 2);

			Assert.AreEqual (points.Start,  result.Start,  "Start");
			Assert.AreEqual (points.Length, result.Length, "Length");
			Assert.AreEqual (points.Moved,  result.Moved,  "Moved");
		}
		
		[Test]
		public void TextUnitWordTestNegative ()
		{
			//"hello   my   baby{,   hel}lo my   darling."
			TextNormalizer normalizer = new TextNormalizer (word_message, 17, 7);
			//"{hello}   my   baby,   hello my   darling."
			TextNormalizerPoints result = normalizer.Normalize (TextUnit.Word, -2);
			TextNormalizerPoints points = new TextNormalizerPoints (0, 5, 2);

			Assert.AreEqual (points.Start,  result.Start,  "Start");
			Assert.AreEqual (points.Length, result.Length, "Length");
			Assert.AreEqual (points.Moved,  result.Moved,  "Moved");
		}
		
		[Test]
		public void TextUnitWordTestBeyondNegative ()
		{
			//"hello   my   baby{,   hel}lo my   darling."
			TextNormalizer normalizer = new TextNormalizer (word_message, 17, 7);
			//"{hello}   my   baby,   hello my   darling."
			TextNormalizerPoints result = normalizer.Normalize (TextUnit.Word, -10);
			TextNormalizerPoints points = new TextNormalizerPoints (0, 0, 2);

			Assert.AreEqual (points.Start,  result.Start,  "Start");
			Assert.AreEqual (points.Length, result.Length, "Length");
			Assert.AreEqual (points.Moved,  result.Moved,  "Moved");
		}

		//One morning, when Gregor Samsa woke from troubled dreams, he found 
		//himself transformed in his bed into a horrible vermin. He lay on his
		//armour-like back, and if he lifted his head a  little he could see his 
		//brown belly, slightly domed and divided by arches into stiff sections. 
		//The bedding was hardly able to cover it and seemed ready to slide off 
		//any moment. His many legs, pitifully thin compared with the size of 
		//the rest of him, waved about helplessly as he looked. "What's happened 
		//to me? " he thought. It wasn't a dream. His room, a proper human 
		//room although a little too small, lay peacefully between its four 
		//familiar walls. A collection of textile samples lay spread out on the 
		//table - Samsa was a travelling salesman - and above it there hung 
		//a picture that he had recently cut out of an illustrated magazine 
		//and housed in a nice, gilded frame. It showed a lady fitted out 
		//with a fur hat and fur boa who sat upright, raising a heavy fur muff 
		//that covered the whole of her lower arm towards the viewer. Gregor 
		//then turned to look out the window at the dull weather.

#endregion

	}
}
