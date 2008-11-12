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
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class TextBoxProviderTest : BaseProviderTest
	{
#region Tests
		
		[Test]
		public void EditPropertiesTest ()
		{
			TextBox textbox = CreateTextBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (textbox);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Edit.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "edit");
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsPasswordProperty,
			              false);
		}
		
		public void DocumentPropertiesTest ()
		{
			TextBox textbox = CreateTextBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (textbox);
			textbox.Multiline = true;
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Document.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "document");
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsContentElementProperty,
			              true);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsControlElementProperty,
			              true);
		}
		
		[Test]
		public void ValuePatternTest ()
		{
			TextBox textbox = CreateTextBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (textbox);
			
			object valueProvider = provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider, "Not returning ValuePatternIdentifiers.");
			Assert.IsTrue (valueProvider is IValueProvider, "Not returning ValuePatternIdentifiers");
		}
		
		[Test]
		public void TextPatternTest () 
		{
			TextBox textbox = CreateTextBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (textbox);
			
			ITextProvider textProvider
				= provider.GetPatternProvider (TextPatternIdentifiers.Pattern.Id)
					as ITextProvider;
			Assert.IsNotNull (textProvider, "Not returning TextPatternIdentifiers.");
			Assert.IsTrue (textProvider is ITextProvider, "Not returning TextPatternIdentifiers.");

			textbox.Text = "abc123";
			textbox.Multiline = true;
			
			ITextRangeProvider range = textProvider.RangeFromPoint (new Point (0, 0));
			Assert.IsNotNull (range);

			// should always return a degenerate range
			Assert.AreEqual (String.Empty, range.GetText (-1));

			// TODO: Actually check the value, instead of just
			// making sure it doesn't crash
			range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Character, 1);

			// TODO: Actually check the value, instead of just
			// making sure it doesn't crash
			ITextRangeProvider[] ranges = textProvider.GetVisibleRanges ();
			Assert.AreEqual (1, ranges.Length);
		}

		[Test]
		public void RangeValuePatternTest () 
		{
			TextBox textbox = CreateTextBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (textbox);
			
			object rangeProvider = provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNull (rangeProvider, "Returned RangeValuePatternIdentifiers.");
		}

#endregion
		
#region IValueProvider Tests
		
		[Test]
		public void IsNotIValueProviderTest ()
		{
			TextBox textbox = CreateTextBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (textbox);
			
			object valueProvider;
			/* NOTDOTNET: We diverge from the spec here, as we
			 * provide ValueProvider in Document mode so folks can
			 * subscribe to an event to see if text has changed. 
			textbox.Multiline = true;
			
			object valueProvider = 
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNull (valueProvider, "Is returning ValuePatternIdentifiers.");
			*/ 
			
			textbox.Multiline = false;
			valueProvider = provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider, "Is not returning ValuePatternIdentifiers.");
		}
		
		[Test]
		public void IValueProviderIsReadOnlyTest ()
		{
			TextBox textbox = CreateTextBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (textbox);
			
			IValueProvider valueProvider = (IValueProvider)
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider, "Not returning ValuePatternIdentifiers.");

			textbox.ReadOnly = true;
			Assert.AreEqual (valueProvider.IsReadOnly, true, "Is read only");
			
			textbox.ReadOnly = false;
			Assert.AreEqual (valueProvider.IsReadOnly, false, "Is not only");
		}
		
		[Test]
		public void IValueProviderValueTest ()
		{
			TextBox textbox = CreateTextBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (textbox);
			
			IValueProvider valueProvider = (IValueProvider)
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider, "Not returning ValuePatternIdentifiers.");
			
			Assert.AreEqual (valueProvider.Value, string.Empty, "Empty value");

			string value = "Hello world";
			
			textbox.Text = value;
			Assert.AreEqual (valueProvider.Value, value, "HelloWorld value");
		}
		
		[Test]
		public void IValueProviderSetValueTest ()
		{
			TextBox textbox = CreateTextBox ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (textbox);
			
			IValueProvider valueProvider = (IValueProvider)
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider, "Not returning ValuePatternIdentifiers.");
			
			string value = "Hello world";

			valueProvider.SetValue (value);
			Assert.AreEqual (valueProvider.Value, value, "Different value");

			try {
				textbox.ReadOnly = true;
				valueProvider.SetValue (value);
				Assert.Fail ("ElementNotEnabledException not thrown.");
			} catch (ElementNotEnabledException) { }
		}
		
#endregion

#region IScrollProvider Tests
		
		[Test]
		public void IScrollProviderTest ()
		{
			TextBox textbox = CreateTextBox ();
			textbox.Size = new System.Drawing.Size (30, 30);
			Form.Controls.Add (textbox);
			textbox.ScrollBars = ScrollBars.Both;

			IRawElementProviderSimple prov
				= ProviderFactory.GetProvider (textbox);

			IRawElementProviderFragmentRoot root_prov
				= (IRawElementProviderFragmentRoot)prov;

			// Case #1: Edit with no text
			textbox.Text = String.Empty;
			textbox.Multiline = false;

			IScrollProvider scroll = prov.GetPatternProvider (
				ScrollPatternIdentifiers.Pattern.Id) as IScrollProvider;
			Assert.IsNull (scroll, "Edit mode with no text incorrectly supports ScrollProvider");

			// Case #2: Edit with large amount of text
			textbox.Text = TEST_MESSAGE;
			textbox.Multiline = false;

			scroll = prov.GetPatternProvider (
				ScrollPatternIdentifiers.Pattern.Id) as IScrollProvider;
			Assert.IsNull (scroll, "Edit mode with text incorrectly supports ScrollProvider");

			// Case #3: Document with no text
			textbox.Text = String.Empty;
			textbox.Multiline = true;

			scroll = prov.GetPatternProvider (
				ScrollPatternIdentifiers.Pattern.Id) as IScrollProvider;
			Assert.IsNull (scroll, "Document mode with no text incorrectly supports ScrollProvider");

			// Case #4: Document with large amount of text
			textbox.Text = TEST_MESSAGE;
			textbox.Multiline = true;

			scroll = prov.GetPatternProvider (
				ScrollPatternIdentifiers.Pattern.Id) as IScrollProvider;
			Assert.IsNotNull (scroll, "Document mode with text doesn't support ScrollProvider");

			// Test that an event is fired when we scroll vertically
			bridge.ResetEventLists ();
			scroll.Scroll (ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
			Assert.AreEqual (1, bridge.AutomationEvents.Count,
			                 "EventCount after scrolling vertically");

			// We can't scroll horizontally, so verify that no event is fired
			bridge.ResetEventLists ();
			scroll.Scroll (ScrollAmount.LargeIncrement, ScrollAmount.NoAmount);
			Assert.AreEqual (0, bridge.AutomationEvents.Count,
			                 "EventCount after scrolling horizontally");

			IRawElementProviderFragment child;
			child = root_prov.Navigate (NavigateDirection.FirstChild);

			Assert.IsNotNull (child, "TextBox has no children");
			TestProperty (child, AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.ScrollBar.Id);

			child = child.Navigate (NavigateDirection.NextSibling);
			Assert.IsNull (child, "TextBox has more than one scrollbar");
		}

#endregion
		
#region Events tests
		
		[Test]
		public void TextChangedEventTest ()
		{
			TextBox textbox = CreateTextBox ();
			Form.Controls.Add (textbox);

			bridge.ResetEventLists ();

			textbox.Text = "lfesalhafew";

			Assert.AreEqual (1,
			                 bridge.AutomationPropertyChangedEvents.Count,
			                 "Event count");
		}

		// NOTDOTNET: We diverge from the spec here, as we provide
		// ValueProvider in Document mode so folks can subscribe to an
		// event to see if text has changed. 
		[Test]
		public void TextChangedEventTestMultiLine ()
		{
			TextBox textbox = CreateTextBox ();
			Form.Controls.Add (textbox);
			textbox.Multiline = true;

			bridge.ResetEventLists ();

			textbox.Text = "lifewauhfewa";

			Assert.AreEqual (1,
			                 bridge.AutomationPropertyChangedEvents.Count,
			                 "Event count");
		}

		[Test]
		public void TextChangedEventTestMultiLineBeforeFormShow ()
		{
			TextBox textbox = CreateTextBox ();
			textbox.Multiline = true;
		
			Form form = new Form ();
			form.Controls.Add (textbox);

			bridge.ResetEventLists ();

			textbox.Text = "lifewauhfewa\nsokksooks";

			Assert.AreEqual (1,
			                 bridge.AutomationPropertyChangedEvents.Count,
			                 "Event count");
		}
		
#endregion
		
#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return CreateTextBox ();
		}
		
#endregion

		#region Protected Methods

		protected virtual TextBox CreateTextBox ()
		{
			return new TextBox ();
		}

		#endregion
		
		// TODO: Move this somewhere else
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
	}
}
