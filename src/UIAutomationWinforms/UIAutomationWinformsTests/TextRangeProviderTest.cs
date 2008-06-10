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
	
		private string message = string.Format ("One morning, when Gregor Samsa    woke from troubled dreams, "+
            "he found himself transformed in his bed into a horrible vermin. {0}{0}He lay on his armour-like back, "+
            "and if he lifted his head a little he could see his brown belly, slightly domed and divided by arches "+
            "into stiff sections. The bedding was hardly able to cover it and seemed ready to slide off any moment. "+
            "His many legs, pitifully thin compared with the size of the rest of him, waved about helplessly as he "+
            "looked. \"What's happened to me? \" he thought. It wasn't a dream. His room, a proper human room although "+
            "a little too small, lay peacefully between its four familiar walls. A collection of textile samples lay "+
            "spread out on the table - Samsa was a travelling salesman - and above it there hung a picture that he had "+
            "recently cut out of an illustrated magazine and housed in a nice, gilded frame. {0}{0}It showed a lady "+
            "fitted out with a fur hat and fur boa who sat upright, raising a heavy fur muff that covered the whole "+
            "of her lower arm towards the viewer. Gregor then turned to look out the window at the dull weather. Drops ",
            Environment.NewLine);

        private TextBox textbox;
        
        private TextRangeProvider textrange;

        [SetUp]
        public void Setup()
        {
            textbox = new TextBox ();
            textbox.Size = new Size (800, 30);
            textbox.Text = message;
            textbox.SelectionStart = 0;
            
            ITextProvider provider = (ITextProvider) ProviderFactory.GetProvider (textbox);
            textrange = new TextRangeProvider (provider, textbox);
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
		[Ignore ("Not implemented")]
		public void CompareTest ()
		{
		}

		[Test]
		[Ignore ("Not implemented")]
		public void CompareEndpointsTest ()
		{
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
		
		[Test]
		[Ignore ("Not implemented")]
		public void MoveEndpointByUnitTest ()
		{
		}
		
		[Test]
		public void RemoveFromSelectionTest ()
		{
			try {
            	textrange.RemoveFromSelection ();
                Assert.Fail ("Throw InvalidOperationException.");
            } catch (InvalidOperationException) { 
            } catch (Exception) { Assert.Fail ("Throw ONLY InvalidOperationException."); }
		}

		[Test]
		public void CloneTest ()
		{
			TextRangeProvider cloned = (TextRangeProvider) textrange.Clone ();
			
			Assert.AreEqual (cloned.EndPoint,     textrange.EndPoint,     "EndPoint");
			Assert.AreEqual (cloned.StartPoint,   textrange.StartPoint,   "StartPoint");
			Assert.AreEqual (cloned.TextProvider, textrange.TextProvider, "TextProvider");
			
			//TODO: modify cloned version and verify that changes doesn't affect
			//original object
		}
		
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
		public void GetTextTest ()
		{
			try {
				textrange.GetText (-10);
				Assert.Fail ("Throw ArgumentOutOfRangeException.");
			} catch (ArgumentOutOfRangeException) {
			} catch (Exception) { Assert.Fail ("Throw ONLY ArgumentOutOfRangeException."); }

			Assert.AreEqual (textrange.GetText (-1), string.Empty, "GetText(-1)");
			
			//TODO: Move unit and text again
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
		[Ignore ("Not implemented")]
		public void SelectTest ()
		{
		}

	}
}
