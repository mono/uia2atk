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
//  Matt Guo <matt@mattguo.com>
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;
using At = System.Windows.Automation.Automation;
using NUnit.Framework;
using MonoTests.System.Windows.Automation;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class TextPatternTest : BaseTest
	{
		private ValuePattern valuePattern = null;

		private const string TEST_MESSAGE = "One morning, when Gregor Samsa    woke from troubled dreams, "+
			"he found himself transformed in his bed into a horrible vermin.He lay on his armour-like back, "+
			"and if he lifted his head a little he could see his brown belly, slightly domed and divided by arches "+
			"into stiff sections. The bedding was hardly able to cover it and seemed ready to slide off any moment. "+
			"His many legs, pitifully thin compared with the size of the rest of him, waved about helplessly as he "+
			"looked. \"What's happened to me? \" he thought. It wasn't a dream. His room, a proper human room although "+
			"a little too small, lay peacefully between its four familiar walls. A collection of textile samples lay "+
			"spread out on the table - Samsa was a travelling salesman - and above it there hung a picture that he had "+
			"recently cut out of an illustrated magazine and housed in a nice, gilded frame. It showed a lady "+
			"fitted out with a fur hat and fur boa who sat upright, raising a heavy fur muff that covered the whole "+
			"of her lower arm towards the viewer. Gregor then turned to look out the window at the dull weather. Drops ";

		public override void FixtureSetUp ()
		{
			base.FixtureSetUp ();
			valuePattern = (ValuePattern) textbox3Element.GetCurrentPattern (ValuePattern.Pattern);
		}

#region Tests copied from TextRangeProviderTest@UIAumationWinformsTests

		[Test]
		public void FindText ()
		{
			valuePattern.SetValue ("gomez thing\r\nmorticia\twednesday ing");
			Thread.Sleep (500);

			TextPatternRange range1, range2;
			TextPattern textPattern = (TextPattern) textbox3Element.GetCurrentPattern (TextPattern.Pattern);
			int moved_units;

			TextPatternRange range = textPattern.DocumentRange.Clone ();

			range1 = range.FindText ("mort", false, false);
			Assert.AreEqual ("mort", range1.GetText (-1));

			range2 = range1.FindText ("mort", false, false);
			Assert.AreEqual ("mort", range2.GetText (-1));

			range2 = range1.FindText ("gomez", false, false);
			Assert.IsNull (range2);

			range2 = range1.FindText ("thing", true, false);
			Assert.IsNull (range2);

			range1 = range.FindText ("\t", false, false);
			Assert.AreEqual ("\t", range1.GetText (-1));

			range1 = range.FindText ("dayz", false, false);
			Assert.IsNull (range1);

			range1 = range.FindText ("HING\r", false, true);
			Assert.AreEqual ("hing\r", range1.GetText (-1));

			range1 = range.FindText ("HING\r", false, false);
			Assert.IsNull (range1);

			range1 = range.FindText ("ing", true, false);
			Assert.AreEqual ("ing", range1.GetText (-1));

			moved_units = range1.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, -2);
			Assert.AreEqual (-2, moved_units);
			Assert.AreEqual ("y ing", range1.GetText (-1));
		}

		[Test]
		public void MoveEndpointByCharacter ()
		{
			valuePattern.SetValue ("The quick\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.");
			Thread.Sleep (500);

			TextPattern textPattern = (TextPattern) textbox3Element.GetCurrentPattern (TextPattern.Pattern);
			TextPatternRange range = textPattern.DocumentRange.Clone ();

			int moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 0);
			Assert.AreEqual (0, moved_units);
			Assert.AreEqual ("The quick\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1));

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 1);
			Assert.AreEqual (1, moved_units);
			Assert.AreEqual ("he quick\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1));

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 8);
			Assert.AreEqual (8, moved_units);
			Assert.AreEqual ("\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1));

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 1000);
			Assert.AreEqual (42, moved_units);
			Assert.AreEqual (String.Empty, range.GetText (-1));

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, -1000);
			Assert.AreEqual (-52, moved_units);
			Assert.AreEqual ("The quick\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1));

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 5);
			Assert.AreEqual (5, moved_units);
			Assert.AreEqual ("uick\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1));

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, -3);
			Assert.AreEqual (-3, moved_units);
			Assert.AreEqual ("e quick\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1));
		}

		[Test]
		public void CharacterNormalize ()
		{
			string text = "gomez\rmorticia\npugsley\r\nwednesday\r\rfester\n\nlurch\r\n\r\nthing";
			valuePattern.SetValue (text);
			Thread.Sleep (500);

			TextPattern textPattern = (TextPattern) textbox3Element.GetCurrentPattern (TextPattern.Pattern);
			TextPatternRange range = textPattern.DocumentRange.Clone ();

			// NOTE: These all pass successfully on Windows Vista, so
			// think twice before you change anything.
			range.ExpandToEnclosingUnit (TextUnit.Character);
			Assert.AreEqual (text, range.GetText (-1));

			int moved_units;
			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 2);
			Assert.AreEqual (2, moved_units);
			Assert.AreEqual ("mez\rmorticia\npugsley\r\nwednesday\r\rfester\n\nlurch\r\n\r\nthing",
			                 range.GetText (-1));

			range.ExpandToEnclosingUnit (TextUnit.Character);
			Assert.AreEqual ("mez\rmorticia\npugsley\r\nwednesday\r\rfester\n\nlurch\r\n\r\nthing",
			                 range.GetText (-1));

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -2);
			Assert.AreEqual (-2, moved_units);
			Assert.AreEqual ("mez\rmorticia\npugsley\r\nwednesday\r\rfester\n\nlurch\r\n\r\nthi",
			                 range.GetText (-1));

			range.ExpandToEnclosingUnit (TextUnit.Character);
			Assert.AreEqual ("mez\rmorticia\npugsley\r\nwednesday\r\rfester\n\nlurch\r\n\r\nthi",
			                 range.GetText (-1));
		}

		[Test]
		public void MoveEndpointByWord ()
		{
			string text = "The quick\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.";
			valuePattern.SetValue (text);
			Thread.Sleep (500);

			TextPattern textPattern = (TextPattern) textbox3Element.GetCurrentPattern (TextPattern.Pattern);
			TextPatternRange range = textPattern.DocumentRange.Clone ();

			// NOTE: These all pass successfully on Windows Vista, so
			// think twice before you change anything.
			int moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Word, 0);
			Assert.AreEqual (0, moved_units, "Moved units are incorrect in 0 word move");
			Assert.AreEqual ("The quick\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1),
			                 "Text is incorrect in 0 word move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Word, 1);
			Assert.AreEqual (1, moved_units, "Moved units are incorrect in 1 word move");
			Assert.AreEqual (" quick\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1),
			                 "Text is incorrect in first +1 word move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Word, -3);
			Assert.AreEqual (-1, moved_units, "Moved units are incorrect in -3 word move");
			Assert.AreEqual ("The quick\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1),
			                 "Text is incorrect in -3 word move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Word, 2);
			Assert.AreEqual (2, moved_units, "Moved units are incorrect in 2 word move");
			Assert.AreEqual ("quick\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1),
			                 "Text is incorrect in +2 word move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Word, 1);
			Assert.AreEqual (1, moved_units, "Moved units are incorrect in 1 word move");
			Assert.AreEqual ("\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1),
			                 "Text is incorrect in second +1 word move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Word, 1);
			Assert.AreEqual (1, moved_units, "Moved units are incorrect in 1 word move");
			Assert.AreEqual ("brown (fox] \"jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1),
			                 "Text is incorrect in third +1 word move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Word, 3);
			Assert.AreEqual (3, moved_units, "Moved units are incorrect in 3 word move");
			Assert.AreEqual ("fox] \"jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1),
			                 "Text is incorrect in +3 word move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Word, 4);
			Assert.AreEqual (4, moved_units, "Moved units are incorrect in 4 word move");
			Assert.AreEqual ("jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1),
			                 "Text is incorrect in +4 word move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Word, -2);
			Assert.AreEqual (-2, moved_units, "Moved units are incorrect in -2 word move");
			Assert.AreEqual (" \"jumps\"\rover:\nthe  lazy, dog.", range.GetText (-1),
			                 "Text is incorrect in -2 word move");
		}

		[Test]
		public void WordNormalize ()
		{
			string text = "gomez\rmorticia\npugsley\r\nwednesday\r\rfester\n\nlurch\r\n\r\nthing";
			valuePattern.SetValue (text);
			Thread.Sleep (500);

			TextPattern textPattern = (TextPattern) textbox3Element.GetCurrentPattern (TextPattern.Pattern);
			TextPatternRange range = textPattern.DocumentRange.Clone ();

			// NOTE: These all pass successfully on Windows Vista, so
			// think twice before you change anything.
			int moved_units;
			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 2);
			Assert.AreEqual (2, moved_units);
			Assert.AreEqual ("mez\rmorticia\npugsley\r\nwednesday\r\rfester\n\nlurch\r\n\r\nthing",
			                 range.GetText (-1));

			range.ExpandToEnclosingUnit (TextUnit.Word);
			Assert.AreEqual ("gomez\rmorticia\npugsley\r\nwednesday\r\rfester\n\nlurch\r\n\r\nthing",
			                 range.GetText (-1), "Text incorrect when expanding after first +2 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -2);
			Assert.AreEqual (-2, moved_units);
			Assert.AreEqual ("gomez\rmorticia\npugsley\r\nwednesday\r\rfester\n\nlurch\r\n\r\nthi",
			                 range.GetText (-1));

			range.ExpandToEnclosingUnit (TextUnit.Word);
			Assert.AreEqual ("gomez\rmorticia\npugsley\r\nwednesday\r\rfester\n\nlurch\r\n\r\nthing",
			                 range.GetText (-1), "Text incorrect when expanding after -2 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 5);
			Assert.AreEqual (5, moved_units);
			Assert.AreEqual ("\rmorticia\npugsley\r\nwednesday\r\rfester\n\nlurch\r\n\r\nthing",
			                 range.GetText (-1));

			range.ExpandToEnclosingUnit (TextUnit.Word);
			Assert.AreEqual ("\rmorticia\npugsley\r\nwednesday\r\rfester\n\nlurch\r\n\r\nthing",
			                 range.GetText (-1), "Text incorrect when expanding after +5 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 2);
			Assert.AreEqual (2, moved_units);
			Assert.AreEqual ("orticia\npugsley\r\nwednesday\r\rfester\n\nlurch\r\n\r\nthing",
			                 range.GetText (-1));

			range.ExpandToEnclosingUnit (TextUnit.Word);
			Assert.AreEqual ("morticia\npugsley\r\nwednesday\r\rfester\n\nlurch\r\n\r\nthing",
			                 range.GetText (-1), "Text incorrect when expanding after second +2 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -10);
			Assert.AreEqual (-10, moved_units);
			Assert.AreEqual ("morticia\npugsley\r\nwednesday\r\rfester\n\nlurc",
			                 range.GetText (-1));

			range.ExpandToEnclosingUnit (TextUnit.Word);
			Assert.AreEqual ("morticia\npugsley\r\nwednesday\r\rfester\n\nlurch",
			                 range.GetText (-1), "Text incorrect when expanding after -10 character move");
		}

		[Test]
		public void MoveEndpointByParagraphSimpleHard ()
		{
			MoveEndpointByParagraphSimple ("\r\n");
		}

		[Test]
		public void MoveEndpointByParagraphSimpleLimp ()
		{
			MoveEndpointByParagraphSimple ("\r");
		}

		[Test]
		public void MoveEndpointByParagraphSimpleRich ()
		{
			MoveEndpointByParagraphSimple ("\n");
		}

		private void MoveEndpointByParagraphSimple (string newline)
		{
			//In case you were wondering, the topic is: things that are awesome
			string text = String.Format("bear{0}{0}shark{0}laser{0}{0}volcano", newline);
			valuePattern.SetValue (text);
			Thread.Sleep (500);

			TextPattern textPattern = (TextPattern) textbox3Element.GetCurrentPattern (TextPattern.Pattern);
			TextPatternRange range = textPattern.DocumentRange.Clone ();

			// NOTE: These all pass successfully on Windows Vista, so
			// think twice before you change anything.
			int moved_units;
			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, 0);
			Assert.AreEqual (0, moved_units, "Moved units are incorrect in 0 paragraph move");
			Assert.AreEqual (String.Format ("bear{0}{0}shark{0}laser{0}{0}volcano", newline),
					 range.GetText (-1), "Text is incorrect in 0 paragraph move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, -1);
			Assert.AreEqual (0, moved_units, "Moved units are incorrect in -1 paragraph move");
			Assert.AreEqual (String.Format ("bear{0}{0}shark{0}laser{0}{0}volcano", newline),
					 range.GetText (-1), "Text is incorrect in -1 paragraph move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, 1);
			Assert.AreEqual (1, moved_units, "Moved units are incorrect in first +1 paragraph move");
			Assert.AreEqual (String.Format ("{0}shark{0}laser{0}{0}volcano", newline),
					 range.GetText (-1), "Text is incorrect in first +1 paragraph move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, 1);
			Assert.AreEqual (1, moved_units, "Moved units are incorrect in second +1 paragraph move");
			Assert.AreEqual (String.Format ("shark{0}laser{0}{0}volcano", newline),
					 range.GetText (-1), "Moved units are incorrect in second +1 paragraph move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, 1);
			Assert.AreEqual (1, moved_units, "Moved units are incorrect in third +1 paragraph move");
			Assert.AreEqual (String.Format ("laser{0}{0}volcano", newline),
					 range.GetText (-1), "Text is incorrect in third +1 paragraph move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, -1);
			Assert.AreEqual (-1, moved_units, "Moved units are incorrect in first -1 paragraph move");
			Assert.AreEqual (String.Format ("shark{0}laser{0}{0}volcano", newline),
					 range.GetText (-1), "Text is incorrect in first -1 paragraph move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, -1);
			Assert.AreEqual (-1, moved_units, "Moved units are incorrect in second -1 paragraph move");
			Assert.AreEqual (String.Format ("{0}shark{0}laser{0}{0}volcano", newline),
					 range.GetText (-1), "Text is incorrect in second -1 paragraph move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, -1);
			Assert.AreEqual (-1, moved_units, "Moved units are incorrect in third -1 paragraph move");
			Assert.AreEqual (String.Format ("bear{0}{0}shark{0}laser{0}{0}volcano", newline),
					 range.GetText (-1), "Text is incorrect in third -1 paragraph move");
		}

		[Test]
		public void MoveEndpointByParagraphIntensiveHard ()
		{
			MoveEndpointByParagraphIntensive ("\r\n");
		}

		[Test]
		public void MoveEndpointByParagraphIntensiveLimp ()
		{
			MoveEndpointByParagraphIntensive ("\r");
		}

		[Test]
		public void MoveEndpointByParagraphIntensiveRich ()
		{
			MoveEndpointByParagraphIntensive ("\n");
		}

		private void MoveEndpointByParagraphIntensive (string newline)
		{
			string text = String.Format ("apples{0}{0}pears{0}peaches{0}{0}bananas", newline);
			valuePattern.SetValue (text);
			Thread.Sleep (500);

			TextPattern textPattern = (TextPattern) textbox3Element.GetCurrentPattern (TextPattern.Pattern);
			TextPatternRange range = textPattern.DocumentRange.Clone ();

			// NOTE: These all pass successfully on Windows Vista, so
			// think twice before you change anything.
			int moved_units;
			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 1);
			Assert.AreEqual (1, moved_units, "Moved units are incorrect in 0 paragraph move");
			Assert.AreEqual (String.Format ("pples{0}{0}pears{0}peaches{0}{0}bananas", newline),
					 range.GetText (-1), "Text is incorrect in 0 paragraph move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, -1);
			Assert.AreEqual (-1, moved_units, "Moved units are incorrect in -1 paragraph move");
			Assert.AreEqual (String.Format ("apples{0}{0}pears{0}peaches{0}{0}bananas", newline),
					 range.GetText (-1), "Text is incorrect in -1 paragraph move");

			range = textPattern.DocumentRange.Clone ();

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 1);
			Assert.AreEqual (1, moved_units, "Moved units are incorrect in +1 character move");
			Assert.AreEqual (String.Format ("pples{0}{0}pears{0}peaches{0}{0}bananas", newline),
					 range.GetText (-1), "Text is incorrect in +1 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, 2);
			Assert.AreEqual (2, moved_units, "Moved units are incorrect in +2 paragraph move");
			Assert.AreEqual (String.Format ("pears{0}peaches{0}{0}bananas", newline),
					 range.GetText (-1), "Text is incorrect in +2 paragraph move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, -1);
			Assert.AreEqual (-1, moved_units, "Moved units are incorrect in first -1 paragraph move");
			Assert.AreEqual (String.Format ("{0}pears{0}peaches{0}{0}bananas", newline),
					 range.GetText (-1), "Text is incorrect in -1 paragraph move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, -1);
			Assert.AreEqual (-1, moved_units, "Moved units are incorrect in first -1 paragraph move");
			Assert.AreEqual (String.Format ("apples{0}{0}pears{0}peaches{0}{0}bananas", newline),
					 range.GetText (-1), "Text is incorrect in -1 paragraph move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, 4);
			Assert.AreEqual (4, moved_units, "Moved units are incorrect in +4 paragraph move");
			Assert.AreEqual (String.Format ("{0}bananas", newline),
					 range.GetText (-1), "Text is incorrect in +4 paragraph move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, 10);
			Assert.AreEqual (2, moved_units, "Moved units are incorrect in +10 paragraph move");
			Assert.AreEqual (String.Empty, range.GetText (-1), "Text is incorrect in +10 paragraph move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Paragraph, -10);
			Assert.AreEqual (-6, moved_units, "Moved units are incorrect in -10 paragraph move");
			Assert.AreEqual (String.Format ("apples{0}{0}pears{0}peaches{0}{0}bananas", newline),
					 range.GetText (-1), "Text is incorrect in -10 paragraph move");

			// Going bananas yet?
		}

		[Test]
		public void ParagraphNormalize ()
		{
			string text = "gomez thing\r\nmorticia\twednesday";
			valuePattern.SetValue (text);
			Thread.Sleep (500);

			TextPattern textPattern = (TextPattern) textbox3Element.GetCurrentPattern (TextPattern.Pattern);
			TextPatternRange range = textPattern.DocumentRange.Clone ();

			// NOTE: These all pass successfully on Windows Vista, so
			// think twice before you change anything.

			// Case #1
			int moved_units;
			// Looks like gtk collapses \r\n into one char
			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -100);
			// Seems that SWF treats \n as two chars or something similar
			int expected = (Atspi? -31: -32);
			Assert.AreEqual (expected, moved_units, "Case 1: Moved units are incorrect in -100 character move");
			Assert.AreEqual (String.Empty, range.GetText (-1), "Case 1: Text is incorrect in -100 character move");

			range.ExpandToEnclosingUnit (TextUnit.Paragraph);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 1: Text is incorrect in ExpandToEnclosingUnit");

			// Case #2
			range = textPattern.DocumentRange.Clone ();

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -23);
			Assert.AreEqual (-23, moved_units, "Case 2: Moved units are incorrect in -23 character move");
			Assert.AreEqual ("gomez th", range.GetText (-1), "Case 2: Text is incorrect in -23 character move");

			range.ExpandToEnclosingUnit (TextUnit.Paragraph);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 2: Text is incorrect in ExpandToEnclosingUnit");

			// Case #3
			range = textPattern.DocumentRange.Clone ();

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -18);
			Assert.AreEqual (-18, moved_units, "Case 3: Moved units are incorrect in -18 character move");
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 3: Text is incorrect in -18 character move");

			range.ExpandToEnclosingUnit (TextUnit.Paragraph);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 3: Text is incorrect in ExpandToEnclosingUnit");

			// Case #4
			range = textPattern.DocumentRange.Clone ();

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -14);
			Assert.AreEqual (-14, moved_units, "Case 4: Moved units are incorrect in -14 character move");
			Assert.AreEqual ("gomez thing\r\nmort", range.GetText (-1), "Case 4: Text is incorrect in -14 character move");

			// XXX: Behaves differently than TextUnit.Line
			range.ExpandToEnclosingUnit (TextUnit.Paragraph);
			Assert.AreEqual ("gomez thing\r\nmorticia\twednesday", range.GetText (-1), "Case 4: Text is incorrect in ExpandToEnclosingUnit");

			// Case #5
			range = textPattern.DocumentRange.Clone ();

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 2);
			Assert.AreEqual (2, moved_units, "Case 5: Moved units are incorrect in +2 character move");
			Assert.AreEqual ("mez thing\r\nmorticia\twednesday", range.GetText (-1), "Case 5: Text is incorrect in +2 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -100);
			expected = (Atspi? -31: -32);
			Assert.AreEqual (expected, moved_units, "Case 1: Moved units are incorrect in -100 character move");
			Assert.AreEqual (String.Empty, range.GetText (-1), "Case 5: Text is incorrect in -100 character move");

			range.ExpandToEnclosingUnit (TextUnit.Paragraph);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 5: Text is incorrect in ExpandToEnclosingUnit");

			// Case #6
			range = textPattern.DocumentRange.Clone ();

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -18);
			Assert.AreEqual (-18, moved_units, "Case 6: Moved units are incorrect in -18 character move");
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 6: Text is incorrect in -18 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 2);
			Assert.AreEqual (2, moved_units, "Case 6: Moved units are incorrect in +2 character move");
			Assert.AreEqual ("mez thing\r\n", range.GetText (-1), "Case 6: Text is incorrect in 2 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -4);
			Assert.AreEqual (-4, moved_units, "Case 6: Moved units are incorrect in -4 character move");
			Assert.AreEqual ("mez thi", range.GetText (-1), "Case 6: Text is incorrect in -4 character move");

			range.ExpandToEnclosingUnit (TextUnit.Paragraph);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 6: Text is incorrect in ExpandToEnclosingUnit");

			// Case #7
			range = textPattern.DocumentRange.Clone ();

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -18);
			Assert.AreEqual (-18, moved_units, "Case 7: Moved units are incorrect in -18 character move");
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 7: Text is incorrect in -18 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 4);
			Assert.AreEqual (4, moved_units, "Case 7: Moved units are incorrect in +4 character move");
			Assert.AreEqual ("z thing\r\n", range.GetText (-1), "Case 7: Text is incorrect in +4 character move");

			range.ExpandToEnclosingUnit (TextUnit.Paragraph);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 7: Text is incorrect in ExpandToEnclosingUnit");

			// Case #8
			range = textPattern.DocumentRange.Clone ();

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 2);
			Assert.AreEqual (2, moved_units, "Case 8: Moved units are incorrect in +2 character move");
			Assert.AreEqual ("mez thing\r\nmorticia\twednesday", range.GetText (-1), "Case 8: Text is incorrect in +2 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -16);
			Assert.AreEqual (-16, moved_units, "Case 8: Moved units are incorrect in -16 character move");
			Assert.AreEqual ("mez thing\r\nmo", range.GetText (-1), "Case 8: Text is incorrect in -16 character move");

			// XXX: Behaves differently than TextUnit.Line
			range.ExpandToEnclosingUnit (TextUnit.Paragraph);
			Assert.AreEqual ("gomez thing\r\nmorticia\twednesday", range.GetText (-1), "Case 8: Text is incorrect in ExpandToEnclosingUnit");
		}

		[Test]
		public void MoveEndpointByPage ()
		{
			string text = String.Format ("apples\r\n\npears\r\r\npeaches\nbananas");
			valuePattern.SetValue (text);
			Thread.Sleep (500);

			TextPattern textPattern = (TextPattern) textbox3Element.GetCurrentPattern (TextPattern.Pattern);
			TextPatternRange range = textPattern.DocumentRange.Clone ();

			// NOTE: These all pass successfully on Windows Vista, so
			// think twice before you change anything.
			int moved_units;
			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Page, 0);
			Assert.AreEqual (0, moved_units, "Moved units are incorrect in 0 page move");
			Assert.AreEqual ("apples\r\n\npears\r\r\npeaches\nbananas", range.GetText (-1),
			                 "Text is incorrect in 0 page move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 1);
			Assert.AreEqual (1, moved_units, "Moved units are incorrect in +1 character move");
			Assert.AreEqual ("pples\r\n\npears\r\r\npeaches\nbananas", range.GetText (-1),
			                 "Text is incorrect in first +1 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Page, -1);
			Assert.AreEqual (-1, moved_units, "Moved units are incorrect in first -1 page move");
			Assert.AreEqual ("apples\r\n\npears\r\r\npeaches\nbananas", range.GetText (-1),
			                 "Text is incorrect in first -1 page move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Page, 1);
			Assert.AreEqual (1, moved_units, "Moved units are incorrect in first +1 page move");
			Assert.AreEqual (String.Empty, range.GetText (-1),
			                 "Text is incorrect in first +1 page move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, -1);
			Assert.AreEqual (-1, moved_units, "Moved units are incorrect in -1 character move");
			Assert.AreEqual ("s", range.GetText (-1),
			                 "Text is incorrect in -1 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Page, -1);
			Assert.AreEqual (-1, moved_units, "Moved units are incorrect in second -1 page move");
			Assert.AreEqual (String.Empty, range.GetText (-1),
			                 "Text is incorrect in second -1 page move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Page, 1);
			Assert.AreEqual (1, moved_units, "Moved units are incorrect in second +1 page move");
			Assert.AreEqual ("apples\r\n\npears\r\r\npeaches\nbananas", range.GetText (-1),
			                 "Text is incorrect in second +1 page move");
		}

		// For Edit controls, this is the same as PageNormalize
		[Test]
		public void DocumentNormalize ()
		{
			string text = "gomez thing\r\nmorticia\twednesday";
			valuePattern.SetValue (text);
			Thread.Sleep (500);

			TextPattern textPattern = (TextPattern) textbox3Element.GetCurrentPattern (TextPattern.Pattern);
			TextPatternRange range = textPattern.DocumentRange.Clone ();

			int moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -18);
			Assert.AreEqual (-18, moved_units);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1));

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 2);
			Assert.AreEqual (2, moved_units);
			Assert.AreEqual ("mez thing\r\n", range.GetText (-1));

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -4);
			Assert.AreEqual (-4, moved_units);
			Assert.AreEqual ("mez thi", range.GetText (-1));

			range.ExpandToEnclosingUnit (TextUnit.Document);
			Assert.AreEqual ("gomez thing\r\nmorticia\twednesday", range.GetText (-1));
		}

		[Test]
		[Ignore]
		//todo WE HAVE A PROBLEM HERE!
		//The last line: "range.ScrollIntoView (false);" will take very long time to execute.
		//So currently to run other tests, I set "Ignore" to this test case - Matt Guo
		public void ScrollIntoView ()
		{
			valuePattern.SetValue (TEST_MESSAGE);
			Thread.Sleep (500);

			TextPattern textPattern = (TextPattern) textbox3Element.GetCurrentPattern (TextPattern.Pattern);
			TextPatternRange range = textPattern.DocumentRange.Clone ();

			int moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Page, 1);
			Assert.AreEqual (1, moved_units);
			Assert.AreEqual (String.Empty, range.GetText (-1));

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Word, -3);
			Assert.AreEqual (-3, moved_units);
			Assert.AreEqual (" Drops ", range.GetText (-1));

			// We can't actually test this visually, but we can
			// verify that it doesn't crash
			range.ScrollIntoView (false);
		}

		[Test]
		public void MoveEndpointByRange()
		{
			string text = "apples\r\n\npears\r\r\npeaches\nbananas";
			valuePattern.SetValue (text);
			Thread.Sleep (500);

			TextPattern textPattern = (TextPattern) textbox3Element.GetCurrentPattern (TextPattern.Pattern);

			TextPatternRange range1, range2;
			range1 = textPattern.DocumentRange.Clone ();

			int moved_units = range1.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 2);
			Assert.AreEqual (2, moved_units);
			Assert.AreEqual ("ples\r\n\npears\r\r\npeaches\nbananas", range1.GetText (-1));

			range2 = textPattern.DocumentRange.Clone ();
			Assert.AreEqual ("apples\r\n\npears\r\r\npeaches\nbananas", range2.GetText (-1));

			range2.MoveEndpointByRange (TextPatternRangeEndpoint.Start, range2, TextPatternRangeEndpoint.Start);
			Assert.AreEqual ("apples\r\n\npears\r\r\npeaches\nbananas", range2.GetText (-1));

			range2.MoveEndpointByRange (TextPatternRangeEndpoint.Start, range1, TextPatternRangeEndpoint.Start);
			Assert.AreEqual ("ples\r\n\npears\r\r\npeaches\nbananas", range2.GetText (-1),
			                 "Might be this one");

			range2 = textPattern.DocumentRange.Clone ();

			range1.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Word, -1);
			Assert.AreEqual ("ples\r\n\npears\r\r\npeaches\n", range1.GetText (-1),
			                 "Or this one?");

			range2.MoveEndpointByRange (TextPatternRangeEndpoint.End, range1, TextPatternRangeEndpoint.End);
			Assert.AreEqual ("apples\r\n\npears\r\r\npeaches\n", range2.GetText (-1));

			range2 = textPattern.DocumentRange.Clone ();

			range2.MoveEndpointByRange (TextPatternRangeEndpoint.Start, range1, TextPatternRangeEndpoint.End);
			Assert.AreEqual ("bananas", range2.GetText (-1));
		}

#endregion

		[Test]
		//todo this test will fail since we won't even fire the TextChangedEvent at
		//provider/bridge side
		public void TextChangedEvent ()
		{
			int eventCount = 0;
			AutomationEventHandler handler = (o, e) => eventCount++;
			At.AddAutomationEventHandler (TextPattern.TextChangedEvent, textbox3Element,
			                              TreeScope.Element, handler);
			RunCommand ("set textbox3 text");
			Thread.Sleep (500);
			// Ideally we should only receive one event, but at-spi
			// generates a text-changed::delete followed by a
			// text-changed::insert.
			int expectedEventCount = (Atspi? 2: 1);
			Assert.AreEqual (expectedEventCount, eventCount, "TextChangedEvent fired");

			At.RemoveAutomationEventHandler (TextPattern.TextChangedEvent, textbox3Element, handler);
		}

		[Test]
		public void TextSelectionChangedEvent ()
		{
			int eventCount = 0;
			AutomationEventHandler handler = (o, e) => eventCount++;
			At.AddAutomationEventHandler (TextPattern.TextSelectionChangedEvent, textbox3Element,
			                              TreeScope.Element, handler);
			RunCommand ("select textbox3");
			Assert.AreEqual (1, eventCount, "TextSelectionChangedEvent fired");

			At.RemoveAutomationEventHandler (TextPattern.TextSelectionChangedEvent, textbox3Element, handler);
		}
	}
}
