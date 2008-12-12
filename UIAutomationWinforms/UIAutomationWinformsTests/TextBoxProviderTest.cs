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
	public class TextBoxProviderTest
		: TextBoxBaseProviderTest<TextBox>
	{
		protected override TextBox GetTextBoxBase ()
		{
			return new TextBox ();
		}

		[Test]
		public void MaxLengthTest ()
		{
			TextBox textBox;
			IRawElementProviderSimple provider;
			GetProviderAndControl (out provider, out textBox);

			IValueProvider valueProvider
				= (IValueProvider) provider.GetPatternProvider (
					ValuePatternIdentifiers.Pattern.Id);

			textBox.MaxLength = 0;
			valueProvider.SetValue (TEST_MESSAGE);
			Assert.AreEqual (TEST_MESSAGE, valueProvider.Value);

			textBox.MaxLength = 1;
			valueProvider.SetValue (TEST_MESSAGE);
			Assert.AreEqual ("O", valueProvider.Value);

			textBox.MaxLength = 5;
			valueProvider.SetValue (TEST_MESSAGE);
			Assert.AreEqual ("One m", valueProvider.Value);
		}

#region IScrollProvider Tests
		
		// No good way to share this:
		// Copied to RichTextBoxProviderTest
		[Test]
		public void IScrollProviderTest ()
		{
			TextBox textBox;
			IRawElementProviderSimple provider;
			GetProviderAndControl (out provider, out textBox);

			textBox.Size = new System.Drawing.Size (30, 30);
			Form.Controls.Add (textBox);
			textBox.ScrollBars = ScrollBars.Both;

			IRawElementProviderFragmentRoot root_prov
				= (IRawElementProviderFragmentRoot)provider;

			// Case #1: Edit with no text
			textBox.Text = String.Empty;
			textBox.Multiline = false;

			IScrollProvider scroll = provider.GetPatternProvider (
				ScrollPatternIdentifiers.Pattern.Id) as IScrollProvider;
			Assert.IsNull (scroll, "Edit mode with no text incorrectly supports ScrollProvider");

			// Case #2: Edit with large amount of text
			textBox.Text = TEST_MESSAGE;
			textBox.Multiline = false;

			scroll = provider.GetPatternProvider (
				ScrollPatternIdentifiers.Pattern.Id) as IScrollProvider;
			Assert.IsNull (scroll, "Edit mode with text incorrectly supports ScrollProvider");

			// Case #3: Document with no text
			textBox.Text = String.Empty;
			textBox.Multiline = true;

			scroll = provider.GetPatternProvider (
				ScrollPatternIdentifiers.Pattern.Id) as IScrollProvider;
			Assert.IsNull (scroll, "Document mode with no text incorrectly supports ScrollProvider");

			// Case #4: Document with large amount of text
			textBox.Text = TEST_MESSAGE;
			textBox.Multiline = true;

			scroll = provider.GetPatternProvider (
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

			Assert.IsNotNull (child, "textBox has no children");
			TestProperty (child, AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.ScrollBar.Id);

			child = child.Navigate (NavigateDirection.NextSibling);
			Assert.IsNull (child, "textBox has more than one scrollbar");
		}

#endregion

	}
}
