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
using System.Windows;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	// TODO: Include ITextProvider, IScrollProvider tests.
	[TestFixture]
	public class TextBoxProviderTest : BaseProviderTest
	{
		
#region Tests
		
		[Test]
		public void EditPropertiesTest ()
		{
			TextBox textbox = new TextBox ();
			TextBoxProvider provider = new TextBoxProvider (textbox);
			provider.InitializeEvents ();
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Edit.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "edit");
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsContentElementProperty,
			              true);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsControlElementProperty,
			              true);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsPasswordProperty,
			              false);
		}
		
		public void DocumentPropertiesTest ()
		{
			TextBox textbox = new TextBox ();
			TextBoxProvider provider = new TextBoxProvider (textbox);
			provider.InitializeEvents ();
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
			TextBox textbox = new TextBox ();
			TextBoxProvider provider = new TextBoxProvider (textbox);
			provider.InitializeEvents ();
			
			object valueProvider = provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider, "Not returning ValuePatternIdentifiers.");
			Assert.IsTrue (valueProvider is IValueProvider, "Not returning ValuePatternIdentifiers");
		}
		
		[Test]
		public void TextPatternTest () 
		{
			TextBox textbox = new TextBox ();
			TextBoxProvider provider = new TextBoxProvider (textbox);
			provider.InitializeEvents ();
			
			object textProvider = provider.GetPatternProvider (TextPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (textProvider, "Not returning TextPatternIdentifiers.");
			Assert.IsTrue (textProvider is ITextProvider, "Not returning TextPatternIdentifiers.");
		}

		[Test]
		public void RangeValuePatternTest () 
		{
			TextBox textbox = new TextBox ();
			TextBoxProvider provider = new TextBoxProvider (textbox);
			provider.InitializeEvents ();
			
			object rangeProvider = provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNull (rangeProvider, "Returned RangeValuePatternIdentifiers.");
		}

#endregion
		
#region IValueProvider Tests
		
		[Test]
		public void IsNotIValueProviderTest ()
		{
			TextBox textbox = new TextBox ();
			TextBoxProvider provider = new TextBoxProvider (textbox);
			provider.InitializeEvents ();
			textbox.Multiline = true;
			
			object valueProvider = 
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNull (valueProvider, "Is returning ValuePatternIdentifiers.");
			
			textbox.Multiline = false;
			valueProvider = provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider, "Is not returning ValuePatternIdentifiers.");
		}
		
		[Test]
		public void IValueProviderIsReadOnlyTest ()
		{
			TextBox textbox = new TextBox ();
			TextBoxProvider provider = new TextBoxProvider (textbox);
			provider.InitializeEvents ();
			
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
			TextBox textbox = new TextBox ();
			TextBoxProvider provider = new TextBoxProvider (textbox);
			provider.InitializeEvents ();
			
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
			TextBox textbox = new TextBox ();
			TextBoxProvider provider = new TextBoxProvider (textbox);
			provider.InitializeEvents ();
			
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
		
#region Events tests
		
		[Test]
		public void TextChangedEventTest ()
		{
			TextBox textBox = new TextBox ();
			TextBoxProvider provider = new TextBoxProvider (textBox);
			provider.InitializeEvents ();

			bridge.ResetEventLists ();			
			textBox.Text = "Changed!";

			Assert.AreEqual (1,
			                 bridge.AutomationEvents.Count,
			                 "Event count");
		}
		
#endregion
		
#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return new TextBox ();
		}
		
#endregion

	}
}
