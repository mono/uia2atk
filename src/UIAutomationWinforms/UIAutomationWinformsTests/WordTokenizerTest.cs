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
using System.Collections.Generic;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{

	[TestFixture]
	public class WordTokenizerTest
	{
		private string message = "One    morning,    when Gregor Samsa woke "
			+"from troubled dreams, he found himself transformed in his bed "
			+"into a horrible vermin.   ";
		
		[Test]
		public void BackwardsMissingTest ()
		{
			string localMessage = "one, two,  three,   four";
			
			WordTokenizer tokenizer = new WordTokenizer (localMessage);
			WordTokenCollection tokens = tokenizer.Backwards (localMessage.Length - 1, 10);
			
			Assert.AreEqual (7, tokens.Count, "Count");
			
			Assert.AreEqual (tokens [0].Message, 
			                 localMessage.Substring (tokens [0].Index, tokens [0].Message.Length), 
			                 "four");
			Assert.AreEqual ("four", tokens [0].Message, "four");
			
			Assert.AreEqual (tokens [1].Message, 
			                 localMessage.Substring (tokens [1].Index, tokens [1].Message.Length), 
			                 "3 spaces");
			Assert.AreEqual ("   ", tokens [1].Message, "3 spaces");
			
			Assert.AreEqual (tokens [2].Message, 
			                 localMessage.Substring (tokens [2].Index, tokens [2].Message.Length), 
			                 "three,");
			Assert.AreEqual ("three,", tokens [2].Message, "three,");
			
			Assert.AreEqual (tokens [3].Message, 
			                 localMessage.Substring (tokens [3].Index, tokens [3].Message.Length), 
			                 "2 spaces");
			Assert.AreEqual ("  ", tokens [3].Message, "2 spaces,");
			
			Assert.AreEqual (tokens [4].Message, 
			                 localMessage.Substring (tokens [4].Index, tokens [4].Message.Length), 
			                 "two,");
			Assert.AreEqual ("two,", tokens [4].Message, "two,");
			
			Assert.AreEqual (tokens [5].Message, 
			                 localMessage.Substring (tokens [5].Index, tokens [5].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ", tokens [5].Message, "1 space");
			
			Assert.AreEqual (tokens [6].Message, 
			                 localMessage.Substring (tokens [6].Index, tokens [6].Message.Length), 
			                 "one,");
			Assert.AreEqual ("one,", tokens [6].Message, "one,");
			
			tokens = tokenizer.Backwards (0);
			
			Assert.AreEqual (0, tokens.Count, "Count");
		}
		
		[Test]
		public void BackwardsTest10MaxLength2 ()
		{
			WordTokenizer tokenizer = new WordTokenizer (message);
			WordTokenCollection tokens = tokenizer.Backwards (10, 2);

			Assert.AreEqual ("morn", tokens [0].Message, "morn");
			Assert.AreEqual ("    ", tokens [1].Message, "3 spaces.");
			Assert.AreEqual (2,      tokens.Count,       "Count");
		}
			
		[Test]
		public void BackwardsTestEmptyWord ()
		{
			string localMessage = "One    morning,   when Gregor Samsa   ";
			WordTokenizer tokenizer = new WordTokenizer (localMessage);
			
			WordTokenCollection tokens = tokenizer.Backwards (localMessage.Length - 1, 2);
			
			Assert.AreEqual (tokens [0].Message, 
			                 localMessage.Substring (tokens [0].Index, tokens [0].Message.Length), 
			                 "3 spaces");
			Assert.AreEqual ("   ",   tokens [0].Message, "3 spaces");

			Assert.AreEqual (tokens [1].Message, 
			                 localMessage.Substring (tokens [1].Index, tokens [1].Message.Length), 
			                 "Samsa");
			Assert.AreEqual ("Samsa", tokens [1].Message, "Samsa");

			Assert.AreEqual (2, tokens.Count, "Count");
		}
			
		[Test]
		public void BackwardsTest10 ()
		{
			WordTokenizer tokenizer = new WordTokenizer (message);
			WordTokenCollection tokens = tokenizer.Backwards (10);

			Assert.AreEqual (tokens [0].Message, 
			                 message.Substring (tokens [0].Index, tokens [0].Message.Length), 
			                 "morn");
			Assert.AreEqual ("morn", tokens [0].Message, "morn");
			
			Assert.AreEqual ("    ", tokens [1].Message, "3 spaces.");
			Assert.AreEqual (tokens [1].Message, 
			                 message.Substring (tokens [1].Index, tokens [1].Message.Length), 
			                 "4 spaces");
			
			Assert.AreEqual ("One",   tokens [2].Message, "One");
			Assert.AreEqual (tokens [2].Message, 
			                 message.Substring (tokens [2].Index, tokens [2].Message.Length), 
			                 "One");			
		}

		[Test]
		public void BackwardsTest ()
		{
			WordTokenizer tokenizer = new WordTokenizer (message);
			WordTokenCollection tokens = tokenizer.Backwards (message.Length - 1);

			Assert.AreEqual (tokens [0].Message, 
			                 message.Substring (tokens [0].Index, tokens [0].Message.Length), 
			                 "3 spaces");
			Assert.AreEqual ("   ", tokens [0].Message, "3 spaces");
			
			Assert.AreEqual (tokens [1].Message, 
			                 message.Substring (tokens [1].Index, tokens [1].Message.Length), 
			                 "vermin.");
			Assert.AreEqual ("vermin.", tokens [1].Message, "vermin.");
						
			Assert.AreEqual (tokens [2].Message, 
			                 message.Substring (tokens [2].Index, tokens [2].Message.Length), 
			                 "1 space");			
			Assert.AreEqual (" ", tokens [2].Message, "1 space");

			Assert.AreEqual (tokens [3].Message, 
			                 message.Substring (tokens [3].Index, tokens [3].Message.Length), 
			                 "horrible");
			Assert.AreEqual ("horrible", tokens [3].Message, "horrible");

			Assert.AreEqual (tokens [4].Message, 
			                 message.Substring (tokens [4].Index, tokens [4].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ", tokens [4].Message, "1 space");

			Assert.AreEqual (tokens [5].Message, 
			                 message.Substring (tokens [5].Index, tokens [5].Message.Length), 
			                 "a");
			Assert.AreEqual ("a", tokens [5].Message, "a");

			Assert.AreEqual (tokens [6].Message, 
			                 message.Substring (tokens [6].Index, tokens [6].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ",  tokens [6].Message, "1 space");

			Assert.AreEqual (tokens [7].Message, 
			                 message.Substring (tokens [7].Index, tokens [7].Message.Length), 
			                 "into");			
			Assert.AreEqual ("into", tokens [7].Message,  "into");

			Assert.AreEqual (tokens [8].Message, 
			                 message.Substring (tokens [8].Index, tokens [8].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ", tokens [8].Message,  "1 space");

			Assert.AreEqual (tokens [9].Message, 
			                 message.Substring (tokens [9].Index, tokens [9].Message.Length), 
			                 "bed");
			Assert.AreEqual ("bed", tokens [9].Message,  "bed");

			Assert.AreEqual (tokens [10].Message, 
			                 message.Substring (tokens [10].Index, tokens [10].Message.Length), 
			                 "bed");
			Assert.AreEqual (" ", tokens [10].Message, "1 space");

			Assert.AreEqual (tokens [11].Message, 
			                 message.Substring (tokens [11].Index, tokens [11].Message.Length), 
			                 "his");
			Assert.AreEqual ("his", tokens [11].Message, "his");

			Assert.AreEqual (tokens [12].Message, 
			                 message.Substring (tokens [12].Index, tokens [12].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ", tokens [12].Message, "1 space");

			Assert.AreEqual (tokens [13].Message, 
			                 message.Substring (tokens [13].Index, tokens [13].Message.Length), 
			                 "in");
			Assert.AreEqual ( "in", tokens [13].Message, "in");

			Assert.AreEqual (tokens [14].Message, 
			                 message.Substring (tokens [14].Index, tokens [14].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ", tokens [14].Message, "1 space");

			Assert.AreEqual (tokens [15].Message, 
			                 message.Substring (tokens [15].Index, tokens [15].Message.Length), 
			                 "transformed");
			Assert.AreEqual ("transformed", tokens [15].Message, "transformed");

			Assert.AreEqual (tokens [16].Message, 
			                 message.Substring (tokens [16].Index, tokens [16].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ", tokens [16].Message, "1 space");

			Assert.AreEqual (tokens [17].Message, 
			                 message.Substring (tokens [17].Index, tokens [17].Message.Length), 
			                 "himself");
			Assert.AreEqual ("himself", tokens [17].Message, "himself");

			Assert.AreEqual (tokens [18].Message, 
			                 message.Substring (tokens [18].Index, tokens [18].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ", tokens [18].Message, "1 space");

			Assert.AreEqual (tokens [19].Message, 
			                 message.Substring (tokens [19].Index, tokens [19].Message.Length), 
			                 "found");
			Assert.AreEqual ("found", tokens [19].Message, "found");

			Assert.AreEqual (tokens [20].Message, 
			                 message.Substring (tokens [20].Index, tokens [20].Message.Length), 
			                 "found");
			Assert.AreEqual (" ", tokens [20].Message, "1 space");

			Assert.AreEqual (tokens [21].Message, 
			                 message.Substring (tokens [21].Index, tokens [21].Message.Length), 
			                 "he");
			Assert.AreEqual ("he", tokens [21].Message, "he");

			Assert.AreEqual (tokens [22].Message, 
			                 message.Substring (tokens [22].Index, tokens [22].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ", tokens [22].Message, "1 space");

			Assert.AreEqual (tokens [23].Message, 
			                 message.Substring (tokens [23].Index, tokens [23].Message.Length), 
			                 "dreams,");
			Assert.AreEqual ("dreams,", tokens [23].Message, "dreams,");

			Assert.AreEqual (tokens [24].Message, 
			                 message.Substring (tokens [24].Index, tokens [24].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ",           tokens [24].Message, "1 space");

			Assert.AreEqual (tokens [25].Message, 
			                 message.Substring (tokens [25].Index, tokens [25].Message.Length), 
			                 "troubled");
			Assert.AreEqual ("troubled", tokens [25].Message, "troubled");
			
			Assert.AreEqual (tokens [26].Message, 
			                 message.Substring (tokens [26].Index, tokens [26].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ", tokens [26].Message, "1 space");
			
			Assert.AreEqual (tokens [27].Message, 
			                 message.Substring (tokens [27].Index, tokens [27].Message.Length), 
			                 "from");
			Assert.AreEqual ("from", tokens [27].Message, "from");

			Assert.AreEqual (tokens [28].Message, 
			                 message.Substring (tokens [28].Index, tokens [28].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ", tokens [28].Message, "1 space");

			Assert.AreEqual (tokens [29].Message, 
			                 message.Substring (tokens [29].Index, tokens [29].Message.Length), 
			                 "woke");
			Assert.AreEqual ("woke", tokens [29].Message, "woke");

			Assert.AreEqual (tokens [30].Message, 
			                 message.Substring (tokens [30].Index, tokens [30].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ", tokens [30].Message, "1 space");

			Assert.AreEqual (tokens [31].Message, 
			                 message.Substring (tokens [31].Index, tokens [31].Message.Length), 
			                 "Samsa");
			Assert.AreEqual ("Samsa", tokens [31].Message, "Samsa");

			Assert.AreEqual (tokens [32].Message, 
			                 message.Substring (tokens [32].Index, tokens [32].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ", tokens [32].Message, "1 space");

			Assert.AreEqual (tokens [33].Message, 
			                 message.Substring (tokens [33].Index, tokens [33].Message.Length), 
			                 "Gregor");
			Assert.AreEqual ("Gregor", tokens [33].Message, "Gregor");

			Assert.AreEqual (tokens [34].Message, 
			                 message.Substring (tokens [34].Index, tokens [34].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ", tokens [34].Message, "1 space");

			Assert.AreEqual (tokens [35].Message, 
			                 message.Substring (tokens [35].Index, tokens [35].Message.Length), 
			                 "when");			
			Assert.AreEqual ("when", tokens [35].Message, "when");

			Assert.AreEqual (tokens [36].Message, 
			                 message.Substring (tokens [36].Index, tokens [36].Message.Length), 
			                 "4 spaces");
			Assert.AreEqual ("    ", tokens [36].Message, "4 spaces");

			Assert.AreEqual (tokens [37].Message, 
			                 message.Substring (tokens [37].Index, tokens [37].Message.Length), 
			                 "morning");
			Assert.AreEqual ("morning,", tokens [37].Message, "morning,");

			Assert.AreEqual (tokens [38].Message, 
			                 message.Substring (tokens [38].Index, tokens [38].Message.Length), 
			                 "4 spaces");
			Assert.AreEqual ("    ",        tokens [38].Message, "4 spaces");
			
			Assert.AreEqual (tokens [39].Message, 
			                 message.Substring (tokens [39].Index, tokens [39].Message.Length), 
			                 "One");
			Assert.AreEqual ("One", tokens [39].Message, "One");
		}
		
		[Test]
		public void ForwardTest10MaxLength3 ()
		{
			WordTokenizer tokenizer = new WordTokenizer (message);
			WordTokenCollection tokens = tokenizer.Forward (10, 3);
			
			Assert.AreEqual (tokens [0].Message, 
			                 message.Substring (tokens [0].Index, tokens [0].Message.Length), 
			                 "ing,");
			Assert.AreEqual ("ning,", tokens [0].Message, "ning,");
			
			Assert.AreEqual (tokens [1].Message, 
			                 message.Substring (tokens [1].Index, tokens [1].Message.Length), 
			                 "4 spaces");
			Assert.AreEqual ("    ", tokens [1].Message, "4 spaces");
			
			Assert.AreEqual (tokens [2].Message, 
			                 message.Substring (tokens [2].Index, tokens [2].Message.Length), 
			                 "when");
			Assert.AreEqual ("when", tokens [2].Message, "when");

			Assert.AreEqual (3, tokens.Count, "MaxLength");
		}
		
		[Test]
		public void ForwardTest10 ()
		{
			WordTokenizer tokenizer = new WordTokenizer (message);
			WordTokenCollection tokens = tokenizer.Forward (10);
			
			Assert.AreEqual (tokens [0].Message, 
			                 message.Substring (tokens [0].Index, tokens [0].Message.Length), 
			                 "ning,");
			Assert.AreEqual ("ning,", tokens [0].Message, "ning,");

			Assert.AreEqual (tokens [1].Message, 
			                 message.Substring (tokens [1].Index, tokens [1].Message.Length), 
			                 "4 spaces");
			Assert.AreEqual ("    ", tokens [1].Message, "4 spaces");

			Assert.AreEqual (tokens [2].Message, 
			                 message.Substring (tokens [2].Index, tokens [2].Message.Length), 
			                 "when");
			Assert.AreEqual ("when", tokens [2].Message, "when");
		}
		
		[Test]
		public void ForwardTest ()
		{
			WordTokenizer tokenizer = new WordTokenizer (message);
			WordTokenCollection tokens = tokenizer.Forward (0);
			
			Assert.AreEqual (tokens [0].Message, 
			                 message.Substring (tokens [0].Index, tokens [0].Message.Length), 
			                 "One");			
			Assert.AreEqual (tokens [1].Message, 
			                 message.Substring (tokens [1].Index, tokens [1].Message.Length), 
			                 "3 spaces");
			Assert.AreEqual (tokens [2].Message, 
			                 message.Substring (tokens [2].Index, tokens [2].Message.Length), 
			                 "morning");
			Assert.AreEqual (tokens [3].Message, 
			                 message.Substring (tokens [3].Index, tokens [3].Message.Length), 
			                 "4 spaces");
			Assert.AreEqual (tokens [4].Message, 
			                 message.Substring (tokens [4].Index, tokens [4].Message.Length), 
			                 "when");
			Assert.AreEqual (tokens [5].Message, 
			                 message.Substring (tokens [5].Index, tokens [5].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [6].Message, 
			                 message.Substring (tokens [6].Index, tokens [6].Message.Length), 
			                 "Gregor");
			Assert.AreEqual (tokens [7].Message, 
			                 message.Substring (tokens [7].Index, tokens [7].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [8].Message, 
			                 message.Substring (tokens [8].Index, tokens [8].Message.Length), 
			                 "Samsa");
			Assert.AreEqual (tokens [9].Message, 
			                 message.Substring (tokens [9].Index, tokens [9].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [10].Message, 
			                 message.Substring (tokens [10].Index, tokens [10].Message.Length), 
			                 "woke");
			Assert.AreEqual (tokens [11].Message, 
			                 message.Substring (tokens [11].Index, tokens [11].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [12].Message, 
			                 message.Substring (tokens [12].Index, tokens [12].Message.Length), 
			                 "from");
			Assert.AreEqual (tokens [13].Message, 
			                 message.Substring (tokens [13].Index, tokens [13].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [14].Message, 
			                 message.Substring (tokens [14].Index, tokens [14].Message.Length), 
			                 "troubled");
			Assert.AreEqual (tokens [15].Message, 
			                 message.Substring (tokens [15].Index, tokens [15].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [16].Message, 
			                 message.Substring (tokens [16].Index, tokens [16].Message.Length), 
			                 "dreams,");
			Assert.AreEqual (tokens [17].Message, 
			                 message.Substring (tokens [17].Index, tokens [17].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [18].Message, 
			                 message.Substring (tokens [18].Index, tokens [18].Message.Length), 
			                 "he");
			Assert.AreEqual (tokens [19].Message, 
			                 message.Substring (tokens [19].Index, tokens [19].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [20].Message, 
			                 message.Substring (tokens [20].Index, tokens [20].Message.Length), 
			                 "found");
			Assert.AreEqual (tokens [21].Message, 
			                 message.Substring (tokens [21].Index, tokens [21].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [22].Message, 
			                 message.Substring (tokens [22].Index, tokens [22].Message.Length), 
			                 "himself");
			Assert.AreEqual (tokens [23].Message, 
			                 message.Substring (tokens [23].Index, tokens [23].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [24].Message, 
			                 message.Substring (tokens [24].Index, tokens [24].Message.Length), 
			                 "transformed");
			Assert.AreEqual (tokens [25].Message, 
			                 message.Substring (tokens [25].Index, tokens [25].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [26].Message, 
			                 message.Substring (tokens [26].Index, tokens [26].Message.Length), 
			                 "in");
			Assert.AreEqual (tokens [27].Message, 
			                 message.Substring (tokens [27].Index, tokens [27].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [28].Message, 
			                 message.Substring (tokens [28].Index, tokens [28].Message.Length), 
			                 "his");
			Assert.AreEqual (tokens [29].Message, 
			                 message.Substring (tokens [29].Index, tokens [29].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [30].Message, 
			                 message.Substring (tokens [30].Index, tokens [30].Message.Length), 
			                 "bed");
			Assert.AreEqual (tokens [31].Message, 
			                 message.Substring (tokens [31].Index, tokens [31].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [32].Message, 
			                 message.Substring (tokens [32].Index, tokens [32].Message.Length), 
			                 "into");
			Assert.AreEqual (tokens [33].Message, 
			                 message.Substring (tokens [33].Index, tokens [33].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [34].Message, 
			                 message.Substring (tokens [34].Index, tokens [34].Message.Length), 
			                 "a");
			Assert.AreEqual (tokens [35].Message, 
			                 message.Substring (tokens [35].Index, tokens [35].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [36].Message, 
			                 message.Substring (tokens [36].Index, tokens [36].Message.Length), 
			                 "horrible");
			Assert.AreEqual (tokens [37].Message, 
			                 message.Substring (tokens [37].Index, tokens [37].Message.Length), 
			                 "1 space");
			Assert.AreEqual (tokens [38].Message, 
			                 message.Substring (tokens [38].Index, tokens [38].Message.Length), 
			                 "vermin.");
			Assert.AreEqual (tokens [39].Message, 
			                 message.Substring (tokens [39].Index, tokens [39].Message.Length), 
			                 "3 spaces");
		}
		
		[Test]
		public void ForwardMissingTest ()
		{
			string localMessage = "one, two,  three,   four";
			
			WordTokenizer tokenizer = new WordTokenizer (localMessage);
			WordTokenCollection tokens = tokenizer.Forward (0, 10);
			
			Assert.AreEqual (7, tokens.Count, "Count");
			
			Assert.AreEqual (tokens [0].Message, 
			                 localMessage.Substring (tokens [0].Index, tokens [0].Message.Length), 
			                 "one,");
			Assert.AreEqual ("one,", tokens [0].Message, "one,");
			
			Assert.AreEqual (tokens [1].Message, 
			                 localMessage.Substring (tokens [1].Index, tokens [1].Message.Length), 
			                 "1 space");
			Assert.AreEqual (" ", tokens [1].Message, "1 space");

			Assert.AreEqual (tokens [2].Message, 
			                 localMessage.Substring (tokens [2].Index, tokens [2].Message.Length), 
			                 "two,");
			Assert.AreEqual ("two,", tokens [2].Message, "two,");
			
			Assert.AreEqual (tokens [3].Message, 
			                 localMessage.Substring (tokens [3].Index, tokens [3].Message.Length), 
			                 "2 spaces");
			Assert.AreEqual ("  ", tokens [3].Message, "2 spaces,");

			Assert.AreEqual (tokens [4].Message,
			                 localMessage.Substring (tokens [4].Index, tokens [4].Message.Length), 
			                 "three,");
			Assert.AreEqual ("three,", tokens [4].Message, "three,");

			Assert.AreEqual (tokens [5].Message, 
			                 localMessage.Substring (tokens [5].Index, tokens [5].Message.Length), 
			                 "3 spaces");
			Assert.AreEqual ("   ", tokens [5].Message, "3 spaces");
			
			Assert.AreEqual (tokens [6].Message, 
			                 localMessage.Substring (tokens [6].Index, tokens [6].Message.Length), 
			                 "four");
			Assert.AreEqual ("four", tokens [6].Message, "four");
			
			tokens = tokenizer.Forward (localMessage.Length);
			
			Assert.AreEqual (0, tokens.Count, "Count");
		}
		
		[Test]
		public void ForwardIndexTest ()
		{
			string localMessage = "one, two,  three,   four   ";
			
			WordTokenizer tokenizer = new WordTokenizer (localMessage);
			WordTokenCollection tokens = tokenizer.Forward (4);
			
			Assert.AreEqual (7, tokens.Count, "Count");
			
			tokens = tokenizer.Forward (4, 2);

			Assert.AreEqual (2, tokens.Count, "Count");
			
			tokens = tokenizer.Forward (0);

			Assert.AreEqual (tokens [tokens .Count - 1].Message, 
			                 localMessage.Substring (tokens [tokens.Count - 1].Index,
			                                         tokens [tokens.Count - 1].Message.Length),
			                 "Empty spaces");
		}
		
	}
}
