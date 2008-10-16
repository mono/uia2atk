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
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Automation.Text;
using System.Windows.Forms;
using System.Reflection;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class TextRangeProviderTest
	{
		[SetUp]
		public void Setup()
		{
			// Copy and paste from BaseProviderTest, as this is not
			// a widget, and as such we don't need the default
			// tests
			// %-<------------------------------------------------

			// Inject a mock automation bridge into the
			// AutomationInteropProvider, so that we don't try
			// to load the UiaAtkBridge.
			MockBridge bridge = new MockBridge ();
			Type interopProviderType = typeof (AutomationInteropProvider);
			FieldInfo bridgeField =
				interopProviderType.GetField ("bridge", BindingFlags.NonPublic | BindingFlags.Static);
			bridgeField.SetValue (null, bridge);
			
			bridge.ClientsAreListening = true;

			// ------------------------------------------------>-%

			textbox = new TextBox ();
			textbox.Size = new Size (800, 30);
			textbox.SelectionStart = 0;

			IRawElementProviderSimple simple = ProviderFactory.GetProvider (textbox);
			text_provider = (ITextProvider)simple.GetPatternProvider (TextPatternIdentifiers.Pattern.Id);
		}

		[TearDown]
		public void TearDown()
		{
			// %-<------------------------------------------------

			Type interopProviderType = typeof (AutomationInteropProvider);
			FieldInfo bridgeField =
				interopProviderType.GetField ("bridge", BindingFlags.NonPublic | BindingFlags.Static);
			bridgeField.SetValue (null, null);

			// ------------------------------------------------>-%

			if (textbox != null)
				textbox.Dispose ();
			textbox = null;
		}

		[Test]
		public void FindText ()
		{
			ITextRangeProvider range1, range2;
			int moved_units;

			textbox.Text = "gomez thing\r\nmorticia\twednesday ing";

			range = text_provider.DocumentRange.Clone ();

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
			textbox.Text = "The quick\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.";
			textbox.Multiline = true;

			range = text_provider.DocumentRange.Clone ();

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

			textbox.Multiline = true;
			textbox.Text = text;

			range = text_provider.DocumentRange.Clone ();

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
			textbox.Text = "The quick\tbrown (fox] \"jumps\"\rover:\nthe  lazy, dog.";
			textbox.Multiline = true;

			range = text_provider.DocumentRange.Clone ();

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

			textbox.Multiline = true;
			textbox.Text = text;

			range = text_provider.DocumentRange.Clone ();

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

		// .NET only supports Environment.NewLine in TextBox, and \r in
		// RichTextBox, but we'll test all cases
		[Test]
		public void MoveEndpointByLineRich ()
		{
			MoveEndpointByLine ("\n");
		}

		[Test]
		public void MoveEndpointByLineLimp ()
		{
			MoveEndpointByLine ("\r");
		}

		[Test]
		public void MoveEndpointByLineHard ()
		{
			MoveEndpointByLine ("\r\n");
		}
		
		public void MoveEndpointByLine (string newline_chars)
		{
			textbox.Multiline = true;
			textbox.Text = String.Format ("hello{0}world{0}{0}test", newline_chars);

			range = text_provider.DocumentRange.Clone ();

			// NOTE: These all pass successfully on Windows Vista, so
			// think twice before you change anything.
			int moved_units;
			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Line, 0);
			Assert.AreEqual (0, moved_units);
			Assert.AreEqual (String.Format ("hello{0}world{0}{0}test", newline_chars),
					 range.GetText (-1));

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 1);
			Assert.AreEqual (1, moved_units, "Moved units are incorrect in +1 character move");
			Assert.AreEqual (String.Format ("ello{0}world{0}{0}test", newline_chars),
					 range.GetText (-1), "Text is incorrect in +1 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Line, -1);
			Assert.AreEqual (-1, moved_units, "Moved units are incorrect in -1 line move");
			Assert.AreEqual (String.Format ("ello{0}world{0}", newline_chars),
					 range.GetText (-1), "Text is incorrect in -1 line move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Line, 1);
			Assert.AreEqual (1, moved_units, "Moved units are incorrect in +1 line move");
			Assert.AreEqual (String.Format ("world{0}", newline_chars),
					 range.GetText (-1), "Text is incorrect in +1 line move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Line, 10);
			Assert.AreEqual (1, moved_units, "Moved units are incorrect in +10 line move");
			Assert.AreEqual (String.Format ("world{0}{0}", newline_chars),
					 range.GetText (-1), "Text is incorrect in +10 line move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, 10);
			Assert.AreEqual (4, moved_units, "Moved units are incorrect in +10 character move");
			Assert.AreEqual (String.Format ("world{0}{0}test", newline_chars),
					 range.GetText (-1), "Text is incorrect in +10 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Line, 5);
			Assert.AreEqual (2, moved_units, "Moved units are incorrect in +5 line move");
			Assert.AreEqual (String.Format ("test", newline_chars),
					 range.GetText (-1), "Text is incorrect in +5 line move");

			moved_units = range.MoveEndpointByUnit(TextPatternRangeEndpoint.Start, TextUnit.Line, -5);
			Assert.AreEqual (-4, moved_units, "Moved units are incorrect in -5 line move");
			Assert.AreEqual(String.Format ("hello{0}world{0}{0}test", newline_chars),
					range.GetText (-1), "Text is incorrect in -5 line move");
		}

		[Test]
		public void LineNormalize ()
		{
			textbox.Multiline = true;
			textbox.Text = "gomez thing\r\nmorticia\twednesday";

			range = text_provider.DocumentRange.Clone ();

			// Case #1
			int moved_units;
			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -100);
			Assert.AreEqual (-32, moved_units, "Case 1: Moved units are incorrect in -100 character move");
			Assert.AreEqual (String.Empty, range.GetText (-1), "Case 1: Text is incorrect in -100 character move");

			range.ExpandToEnclosingUnit (TextUnit.Line);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 1: Text is incorrect in ExpandToEnclosingUnit");

			// Case #2
			range = text_provider.DocumentRange.Clone ();

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -23);
			Assert.AreEqual (-23, moved_units, "Case 2: Moved units are incorrect in -23 character move");
			Assert.AreEqual ("gomez th", range.GetText (-1), "Case 2: Text is incorrect in -23 character move");

			range.ExpandToEnclosingUnit (TextUnit.Line);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 2: Text is incorrect in ExpandToEnclosingUnit");

			// Case #3
			range = text_provider.DocumentRange.Clone ();
			
			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -18);
			Assert.AreEqual (-18, moved_units, "Case 3: Moved units are incorrect in -18 character move");
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 3: Text is incorrect in -18 character move");

			range.ExpandToEnclosingUnit (TextUnit.Line);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 3: Text is incorrect in ExpandToEnclosingUnit");

			// Case #4
			range = text_provider.DocumentRange.Clone ();

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -14);
			Assert.AreEqual (-14, moved_units, "Case 4: Moved units are incorrect in -14 character move");
			Assert.AreEqual ("gomez thing\r\nmort", range.GetText (-1), "Case 4: Text is incorrect in -14 character move");

			range.ExpandToEnclosingUnit (TextUnit.Line);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 4: Text is incorrect in ExpandToEnclosingUnit");

			// Case #5
			range = text_provider.DocumentRange.Clone ();

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 2);
			Assert.AreEqual (2, moved_units, "Case 5: Moved units are incorrect in +2 character move");
			Assert.AreEqual ("mez thing\r\nmorticia\twednesday", range.GetText (-1), "Case 5: Text is incorrect in +2 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -100);
			Assert.AreEqual (-32, moved_units, "Case 5: Moved units are incorrect in -100 character move");
			Assert.AreEqual (String.Empty, range.GetText (-1), "Case 5: Text is incorrect in -100 character move");

			range.ExpandToEnclosingUnit (TextUnit.Line);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 5: Text is incorrect in ExpandToEnclosingUnit");

			// Case #6
			range = text_provider.DocumentRange.Clone ();

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -18);
			Assert.AreEqual (-18, moved_units, "Case 6: Moved units are incorrect in -18 character move");
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 6: Text is incorrect in -18 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 2);
			Assert.AreEqual (2, moved_units, "Case 6: Moved units are incorrect in +2 character move");
			Assert.AreEqual ("mez thing\r\n", range.GetText (-1), "Case 6: Text is incorrect in 2 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -4);
			Assert.AreEqual (-4, moved_units, "Case 6: Moved units are incorrect in -4 character move");
			Assert.AreEqual ("mez thi", range.GetText (-1), "Case 6: Text is incorrect in -4 character move");

			range.ExpandToEnclosingUnit (TextUnit.Line);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 6: Text is incorrect in ExpandToEnclosingUnit");

			// Case #7
			range = text_provider.DocumentRange.Clone ();

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -18);
			Assert.AreEqual (-18, moved_units, "Case 7: Moved units are incorrect in -18 character move");
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 7: Text is incorrect in -18 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 4);
			Assert.AreEqual (4, moved_units, "Case 7: Moved units are incorrect in +4 character move");
			Assert.AreEqual ("z thing\r\n", range.GetText (-1), "Case 7: Text is incorrect in +4 character move");

			range.ExpandToEnclosingUnit (TextUnit.Line);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 7: Text is incorrect in ExpandToEnclosingUnit");

			// Case #8
			range = text_provider.DocumentRange.Clone ();

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 2);
			Assert.AreEqual (2, moved_units, "Case 8: Moved units are incorrect in +2 character move");
			Assert.AreEqual ("mez thing\r\nmorticia\twednesday", range.GetText (-1), "Case 8: Text is incorrect in +2 character move");

			moved_units = range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, -16);
			Assert.AreEqual (-16, moved_units, "Case 8: Moved units are incorrect in -16 character move");
			Assert.AreEqual ("mez thing\r\nmo", range.GetText (-1), "Case 8: Text is incorrect in -16 character move");

			range.ExpandToEnclosingUnit (TextUnit.Line);
			Assert.AreEqual ("gomez thing\r\n", range.GetText (-1), "Case 8: Text is incorrect in ExpandToEnclosingUnit");
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
			textbox.Multiline = true;

			// In case you were wondering, the topic is: things that are awesome
			textbox.Text = String.Format("bear{0}{0}shark{0}laser{0}{0}volcano", newline);

			range = text_provider.DocumentRange.Clone ();
			
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
			textbox.Multiline = true;
			textbox.Text = String.Format ("apples{0}{0}pears{0}peaches{0}{0}bananas", newline);

			range = text_provider.DocumentRange.Clone ();
			
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

			range = text_provider.DocumentRange.Clone ();

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
		public void MoveEndpointByPage ()
		{
			textbox.Multiline = true;
			textbox.Text = String.Format ("apples\r\n\npears\r\r\npeaches\nbananas");

			range = text_provider.DocumentRange.Clone ();
			
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

		[Test]
		public void ScrollIntoView ()
		{
			textbox.Multiline = true;
			textbox.Text = TEST_MESSAGE;

			range = text_provider.DocumentRange.Clone ();
			
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
			textbox.Multiline = true;
			textbox.Text = String.Format ("apples\r\n\npears\r\r\npeaches\nbananas");

			ITextRangeProvider range1, range2;
			range1 = text_provider.DocumentRange.Clone ();

			int moved_units = range1.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 2);
			Assert.AreEqual (2, moved_units);
			Assert.AreEqual ("ples\r\n\npears\r\r\npeaches\nbananas", range1.GetText (-1));

			range2 = text_provider.DocumentRange.Clone ();
			Assert.AreEqual ("apples\r\n\npears\r\r\npeaches\nbananas", range2.GetText (-1));

			range2.MoveEndpointByRange (TextPatternRangeEndpoint.Start, range2, TextPatternRangeEndpoint.Start);
			Assert.AreEqual ("apples\r\n\npears\r\r\npeaches\nbananas", range2.GetText (-1));

			range2.MoveEndpointByRange (TextPatternRangeEndpoint.Start, range1, TextPatternRangeEndpoint.Start);
			Assert.AreEqual ("ples\r\n\npears\r\r\npeaches\nbananas", range2.GetText (-1),
			                 "Might be this one");

			range2 = text_provider.DocumentRange.Clone ();

			range1.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Word, -1);
			Assert.AreEqual ("ples\r\n\npears\r\r\npeaches\n", range1.GetText (-1), 
			                 "Or this one?"); 

			range2.MoveEndpointByRange (TextPatternRangeEndpoint.End, range1, TextPatternRangeEndpoint.End);
			Assert.AreEqual ("apples\r\n\npears\r\r\npeaches\n", range2.GetText (-1)); 

			range2 = text_provider.DocumentRange.Clone ();

			range2.MoveEndpointByRange (TextPatternRangeEndpoint.Start, range1, TextPatternRangeEndpoint.End);
			Assert.AreEqual ("bananas", range2.GetText (-1)); 
		}

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

		private TextBox textbox;
		private ITextRangeProvider range;
		private ITextProvider text_provider;
	}
}
