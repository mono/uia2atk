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
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	
	[TestFixture]
	public class TextRangeProviderTest
	{
	
		private string message = "One morning, when Gregor Samsa    woke from troubled dreams, "+
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
        
        private ITextRangeProvider textrange;

        [SetUp]
        public void Setup()
        {
            textbox = new TextBox ();
            textbox.Size = new Size (800, 30);
            textbox.Text = message;
            textbox.SelectionStart = 0;
            
            ITextProvider provider = (ITextProvider) ProviderFactory.GetProvider (textbox);
            textrange = provider.DocumentRange;
        }

        [TearDown]
        public void TeardDown()
        {
            if (textbox != null)
                textbox.Dispose ();
            textbox = null;
        }
		
		[Test]
		public void AddToSelectionTest ()
		{
			try {
            	textrange.AddToSelection ();
                Assert.Fail ("Throw InvalidOperationException.");
            } catch (InvalidOperationException) { 
            } catch (Exception) { Assert.Fail ("Throw ONLY InvalidOperationException."); }
		}
		
		[Test]
		public void CompareTest ()
		{
			ITextRangeProvider cloned = (ITextRangeProvider) textrange.Clone ();
			Assert.AreEqual (true, cloned.Compare (textrange), "Compared");
			
			//Move start and end to change.
			cloned.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, 
			                           TextUnit.Character, 10);
			Assert.AreEqual (false, cloned.Compare (textrange), "After moved.");
			
			try {
//				TextBox textbox = new TextBox ();
//				textbox.Text = message;
//				TextBoxProvider provider = (TextBoxProvider) ProviderFactory.GetProvider (textbox);
//				textrange.Compare (provider.DocumentRange);
//				Assert.Fail ("Should throw ArgumentException");
			} catch (ArgumentException) { 
			} catch (Exception) { Assert.Fail ("Should ONLY throw ArgumentException"); }
		}
			


		[Test]
		[Ignore ("Missing upgrades")]
		public void CompareEndpointsTest ()
		{
			ITextRangeProvider cloned = (ITextRangeProvider) textrange.Clone ();
			Assert.AreEqual (0, 
			                 textrange.CompareEndpoints (TextPatternRangeEndpoint.Start, 
			                                             cloned, 
			                                             TextPatternRangeEndpoint.Start),
			                 "Compared 1");
			
			Assert.AreEqual (0, 
			                 textrange.CompareEndpoints (TextPatternRangeEndpoint.End, 
			                                             cloned, 
			                                             TextPatternRangeEndpoint.End),
			                 "Compared 2");
			
			textrange.MoveEndpointByUnit (TextPatternRangeEndpoint.Start,
			                              TextUnit.Character, 
			                              10);
			Assert.AreEqual (10, 
			                 textrange.CompareEndpoints (TextPatternRangeEndpoint.Start, 
			                                             cloned, 
			                                             TextPatternRangeEndpoint.Start),
			                 "Compared 3");
			
			try {
//				TextBox textbox = new TextBox ();
//				textbox.Text = message;
//				TextBoxProvider provider = (TextBoxProvider) ProviderFactory.GetProvider (textbox);
//				textrange.CompareEndpoints (TextPatternRangeEndpoint.Start, 
//				                            provider.DocumentRange, TextPatternRangeEndpoint.Start);
//				Assert.Fail ("Should throw ArgumentException");
			} catch (ArgumentException) { 
			} catch (Exception) { Assert.Fail ("Should ONLY throw ArgumentException"); }
		}

		[Test]
		[Ignore ("Not implemented")]
		public void MoveTest ()
		{
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void MoveEndpointByRangeTest ()
		{
		}
		
//		[Test]
//		public void MoveEndpointByUnitTest ()
//		{
//			string localMessage = "abc  def  gh  ijk l m n opqrs   tu  vw   xy    z   ";
//			TextBox textbox = new TextBox ();
//			textbox.Text = localMessage;
//			TextBoxProvider provider = (TextBoxProvider) ProviderFactory.GetProvider (textbox);
//			TextRangeProvider range = new TextRangeProvider (provider, textbox);
//			
//			Assert.AreEqual (localMessage, range.GetText (-1), "GetText-1");
//					
//			Assert.AreEqual (4, 
//			                 range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, 
//			                                           TextUnit.Character, 4),
//			                 "MoveEndpointByUnit+4");
//			Assert.AreEqual (" def  gh  ijk l m n opqrs   tu  vw   xy    z   ", 
//			                 range.GetText (-1), 
//			                 "MoveEndpointByUnit+4 & GetText-1");
//			
//			Assert.AreEqual (10,
//			                 range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, 
//			                                           TextUnit.Character, -10),
//			                 "MoveEndpointByUnit-10");
//			Assert.AreEqual (" def  gh  ijk l m n opqrs   tu  vw   ", 
//			                 range.GetText (-1), 
//			                 "MoveEndpointByUnit-10 & GetText-1");
//			
//			Assert.AreEqual (1,
//			                 range.Move (TextUnit.Word, 1),
//			                 "Move=TextUnit.Word+1");
//			Assert.AreEqual (string.Empty, 
//			                 range.GetText (-1), 
//			                 "Move=TextUnit.Word+1 & GetText-1");
//			
//			Assert.AreEqual (5,
//			                 range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, 
//			                                           TextUnit.Character, 
//			                                           5),
//			                 "MoveEndpointByUnit=TextUnit.Character+5");
//			Assert.AreEqual ("    z", 
//			                 range.GetText (-1), 
//			                 "MoveEndpointByUnit=TextUnit.Character+5 & GetText-1");
//		}
		
		[Test]
		public void RemoveFromSelectionTest ()
		{
			try {
            	textrange.RemoveFromSelection ();
                Assert.Fail ("Throw InvalidOperationException.");
            } catch (InvalidOperationException) { 
            } catch (Exception) { Assert.Fail ("Throw ONLY InvalidOperationException."); }
		}

//		[Test]
//		public void CloneTest ()
//		{
//			ITextRangeProvider cloned = (ITextRangeProvider) textrange.Clone ();
//			
//			Assert.AreEqual (cloned.EndPoint,     textrange.EndPoint,     "EndPoint");
//			Assert.AreEqual (cloned.StartPoint,   textrange.StartPoint,   "StartPoint");
//			Assert.AreEqual (cloned.TextProvider, textrange.TextProvider, "TextProvider");
//			
//			//Modify StartPoint
//			textrange.MoveEndpointByUnit (TextPatternRangeEndpoint.Start,
//			                              TextUnit.Character, 
//			                              15);
//			if (cloned.StartPoint == textrange.StartPoint)
//				Assert.Fail ("StartPoint should be different");
//
//			textrange.MoveEndpointByUnit (TextPatternRangeEndpoint.End,
//			                              TextUnit.Character, 
//			                              -10);
//			if (cloned.EndPoint == textrange.EndPoint)
//				Assert.Fail ("EndPoint should be different");
//		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void GetAttributeValueTest ()
		{
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void GetBoundingRectanglesTest ()
		{
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void GetChildrenTest ()
		{
		}

		[Test]
		[Ignore ("Not implemented")]
		public void GetEnclosingElementTest ()
		{
		}

		[Test]
		[Ignore ("Missing upgrades")]
		public void GetTextTest ()
		{
			try {
				textrange.GetText (-10);
				Assert.Fail ("Throw ArgumentOutOfRangeException.");
			} catch (ArgumentOutOfRangeException) {
			} catch (Exception) { Assert.Fail ("Throw ONLY ArgumentOutOfRangeException."); }

			//By default Start and End point are matching all text
			Assert.AreEqual (textrange.GetText (-1), message, "GetText(-1)");
			
			Assert.AreEqual (textrange.GetText (10), message.Substring (0, 10), "GetText(10)");
			
			Assert.AreEqual (textrange.GetText (0), string.Empty, "GetText(0)");
			
			textrange.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Character, 5);
			Assert.AreEqual (textrange.GetText (7), message.Substring (5, 7), "GetText(7)");
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void ExpandToEnclosingUnitTest ()
		{
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void FindAttributeTest ()
		{
		}

		[Test]
		[Ignore ("Not implemented")]
		public void FindTextTest ()
		{
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void ScrollIntoViewTest ()
		{
		}
		
		[Test]
		[Ignore ("Missing upgrades")]
		public void SelectTest ()
		{
			int start = 12;
			int end = 23;
			textrange.MoveEndpointByUnit (TextPatternRangeEndpoint.Start,
			                              TextUnit.Character, 
			                              start);
			textrange.MoveEndpointByUnit (TextPatternRangeEndpoint.End,
			                              TextUnit.Character, 
			                              -end);
			textrange.Select ();

			Assert.AreEqual (textbox.SelectedText, 
			                 textbox.Text.Substring (start, 
			                                         System.Math.Abs (textbox.Text.Length - (end + start))),
			                 "Select");
		}

	}
}
