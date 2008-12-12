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
	public abstract class TextBoxBaseProviderTest<T> : BaseProviderTest
		where T : TextBoxBase
	{

#region Tests

		[Test]
		public void EditPropertiesTest ()
		{
			T textBoxBase;
			IRawElementProviderSimple provider;
			GetProviderAndControl (out provider, out textBoxBase);

			textBoxBase.Multiline = false;
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Edit.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "edit");
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsPasswordProperty,
			              false);

			textBoxBase.Text = string.Empty;
			TestProperty (provider,
			              AutomationElementIdentifiers.NameProperty,
			              string.Empty);
			textBoxBase.Text = "hello world";
			TestProperty (provider,
			              AutomationElementIdentifiers.NameProperty,
			              string.Empty);

			// We need to call the property otherwise the test fails
			AccessibleObject obj = textBoxBase.AccessibilityObject;
			Assert.IsNotNull (obj);
		
			textBoxBase.AccessibleName = "Accessible name";
			Console.WriteLine ("textBoxBase.AccessibleName: {0}", textBoxBase.AccessibleName);
			TestProperty (provider,
			              AutomationElementIdentifiers.NameProperty,
			              textBoxBase.AccessibleName);
			textBoxBase.AccessibleName = "Accessible name (changed)";
			TestProperty (provider,
			              AutomationElementIdentifiers.NameProperty,
			              textBoxBase.AccessibleName);
		}
		
		public void DocumentPropertiesTest ()
		{
			T textBoxBase;
			IRawElementProviderSimple provider;
			GetProviderAndControl (out provider, out textBoxBase);

			textBoxBase.Multiline = true;
			
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
			T textBoxBase;
			IRawElementProviderSimple provider;
			GetProviderAndControl (out provider, out textBoxBase);

			textBoxBase.Multiline = false;
			
			object valueProvider = provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider, "Not returning ValuePatternIdentifiers.");
			Assert.IsTrue (valueProvider is IValueProvider, "Not returning ValuePatternIdentifiers");
		}
		
		[Test]
		public void TextPatternTest () 
		{
			T textBoxBase;
			IRawElementProviderSimple provider;
			GetProviderAndControl (out provider, out textBoxBase);
			
			ITextProvider textProvider
				= provider.GetPatternProvider (TextPatternIdentifiers.Pattern.Id)
					as ITextProvider;
			Assert.IsNotNull (textProvider, "Not returning TextPatternIdentifiers.");
			Assert.IsTrue (textProvider is ITextProvider, "Not returning TextPatternIdentifiers.");

			textBoxBase.Text = "abc123";
			textBoxBase.Multiline = true;
			
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
			T textBoxBase;
			IRawElementProviderSimple provider;
			GetProviderAndControl (out provider, out textBoxBase);
			
			object rangeProvider = provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNull (rangeProvider, "Returned RangeValuePatternIdentifiers.");
		}

#endregion
		
#region IValueProvider Tests
		
		[Test]
		public void IsNotIValueProviderTest ()
		{
			T textBoxBase;
			IRawElementProviderSimple provider;
			GetProviderAndControl (out provider, out textBoxBase);
			
			object valueProvider;
			/* NOTDOTNET: We diverge from the spec here, as we
			 * provide ValueProvider in Document mode so folks can
			 * subscribe to an event to see if text has changed. 
			textBoxBase.Multiline = true;
			
			object valueProvider = 
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNull (valueProvider, "Is returning ValuePatternIdentifiers.");
			*/ 
			
			textBoxBase.Multiline = false;
			valueProvider = provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider, "Is not returning ValuePatternIdentifiers.");
		}
		
		[Test]
		public void IValueProviderIsReadOnlyTest ()
		{
			T textBoxBase;
			IRawElementProviderSimple provider;
			GetProviderAndControl (out provider, out textBoxBase);

			//textBoxBase.Multiline = false;
			
			IValueProvider valueProvider = (IValueProvider)
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider, "Not returning ValuePatternIdentifiers.");

			textBoxBase.ReadOnly = true;
			Assert.AreEqual (valueProvider.IsReadOnly, true, "Is read only");
			
			textBoxBase.ReadOnly = false;
			Assert.AreEqual (valueProvider.IsReadOnly, false, "Is not only");
		}
		
		[Test]
		public void IValueProviderValueTest ()
		{
			T textBoxBase;
			IRawElementProviderSimple provider;
			GetProviderAndControl (out provider, out textBoxBase);

			textBoxBase.Multiline = false;
			
			IValueProvider valueProvider = (IValueProvider)
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider, "Not returning ValuePatternIdentifiers.");
			
			Assert.AreEqual (valueProvider.Value, string.Empty, "Empty value");

			string value = "Hello world";
			
			textBoxBase.Text = value;
			Assert.AreEqual (valueProvider.Value, value, "HelloWorld value");
		}
		
		[Test]
		public void IValueProviderSetValueTest ()
		{
			T textBoxBase;
			IRawElementProviderSimple provider;
			GetProviderAndControl (out provider, out textBoxBase);

			textBoxBase.Multiline = false;
			
			IValueProvider valueProvider = (IValueProvider)
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider, "Not returning ValuePatternIdentifiers.");
			
			string value = "Hello world";

			valueProvider.SetValue (value);
			Assert.AreEqual (valueProvider.Value, value, "Different value");

			try {
				textBoxBase.ReadOnly = true;
				valueProvider.SetValue (value);
				Assert.Fail ("ElementNotEnabledException not thrown.");
			} catch (ElementNotEnabledException) { }
		}
		
#endregion
		
#region Events tests
		
		[Test]
		public void TextChangedEventTest ()
		{
			T textBoxBase;
			IRawElementProviderSimple provider;
			GetProviderAndControl (out provider, out textBoxBase);
			Form.Controls.Add (textBoxBase);

			bridge.ResetEventLists ();

			textBoxBase.Text = "lfesalhafew";

			Assert.AreEqual (1,
			                 bridge.GetAutomationPropertyEventCount (ValuePatternIdentifiers.ValueProperty),
			                 "Event count");
		}

		// NOTDOTNET: We diverge from the spec here, as we provide
		// ValueProvider in Document mode so folks can subscribe to an
		// event to see if text has changed. 
		[Test]
		public void TextChangedEventTestMultiLine ()
		{
			T textBoxBase;
			IRawElementProviderSimple provider;
			GetProviderAndControl (out provider, out textBoxBase);
			Form.Controls.Add (textBoxBase);
			textBoxBase.Multiline = true;

			bridge.ResetEventLists ();

			textBoxBase.Text = "lifewauhfewa";

			// NOTDOTNET: Read TextBoxProvider.UpdateBehaviors
			Assert.AreEqual (1,
			                 bridge.GetAutomationPropertyEventCount (ValuePatternIdentifiers.ValueProperty),
			                 "Event count");
		}

		[Test]
		public void TextChangedEventTestMultiLineBeforeFormShow ()
		{
			T textBoxBase;
			IRawElementProviderSimple provider;
			GetProviderAndControl (out provider, out textBoxBase);
			textBoxBase.Multiline = true;
		
			Form form = new Form ();
			form.Controls.Add (textBoxBase);
			form.Show ();

			bridge.ResetEventLists ();

			textBoxBase.Text = "lifewauhfewa\nsokksooks";

			// NOTDOTNET: Read TextBoxProvider.UpdateBehaviors
			Assert.AreEqual (1,
			                 bridge.GetAutomationPropertyEventCount (ValuePatternIdentifiers.ValueProperty),
			                 "Event count");
		}
		
#endregion
		
#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return GetTextBoxBase ();
		}
		
#endregion

#region Protected Methods

		protected abstract T GetTextBoxBase ();

		protected void GetProviderAndControl (out IRawElementProviderSimple provider,
		                                      out T textBoxBase)
		{
			textBoxBase = GetTextBoxBase ();
			provider = ProviderFactory.GetProvider (textBoxBase);
		} 
#endregion
		
		// TODO: Move this somewhere else
		protected const string TEST_MESSAGE = "One morning, when Gregor Samsa    woke from troubled dreams, "+
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
