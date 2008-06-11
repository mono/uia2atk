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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 
//

using System;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class RadioButtonProviderTest : BaseProviderTest
	{
#region Test Methods
		
		[Test]
		public void BasicPropertiesTest ()
		{
			RadioButton radioButton = new RadioButton ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (radioButton);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.RadioButton.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "radio button");
		}
		
		[Test]
		public void ProviderPatternTest ()
		{
			RadioButton radioButton = new RadioButton ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (radioButton);
			
			object selectionItem =
				provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (selectionItem);
			Assert.IsTrue (selectionItem is ISelectionItemProvider,
			               "ISelectionItemProvider");
		}
		
		[Test]
		public void AddToSelectionTest ()
		{
			Form f = new Form ();
			
			RadioButton r1 = new RadioButton ();
			r1.Checked = false;
			f.Controls.Add (r1);
			
			IRawElementProviderSimple provider1 = ProviderFactory.GetProvider (r1);
			ISelectionItemProvider selectionItem1 = (ISelectionItemProvider)
				provider1.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			
			selectionItem1.AddToSelection ();
			Assert.IsTrue (r1.Checked, "Selecting 1/1, initially unchecked");
			
			RadioButton r2 = new RadioButton ();
			r2.Checked = false;
			f.Controls.Add (r2);
			
			IRawElementProviderSimple provider2 = ProviderFactory.GetProvider (r2);
			ISelectionItemProvider selectionItem2 = (ISelectionItemProvider)
				provider2.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			
			
			selectionItem1.AddToSelection ();
			Assert.IsTrue (r1.Checked, "Selecting 1/2, 1 intially checked");
			Assert.IsFalse (r2.Checked, "Selecting 1/2, 2 intially unchecked");
			
			try {
				selectionItem2.AddToSelection ();
				Assert.Fail ("Selecting 2/2, 1 initially checked, should throw InvalidOperationException");
			} catch (InvalidOperationException) {
				// Expected
			} catch (Exception e) {
				Assert.Fail ("Selecting 2/2, 1 initially checked, should throw InvalidOperationException, instaed threw: " + e.Message);
			}
			
			r1.Checked = false;			
			selectionItem2.AddToSelection ();
			Assert.IsTrue (r2.Checked, "Selecting 2/2, 2 intially unchecked");
			Assert.IsFalse (r1.Checked, "Selecting 2/2, 1 intially unchecked");
		}
		
		[Test]
		public void IsSelectedTest ()
		{
			RadioButton r1 = new RadioButton ();
			r1.Checked = false;
			
			IRawElementProviderSimple provider1 = ProviderFactory.GetProvider (r1);
			ISelectionItemProvider selectionItem1 = (ISelectionItemProvider)
				provider1.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			
			Assert.IsFalse (selectionItem1.IsSelected, "Unchecked");
			
			r1.Checked = true;
			
			Assert.IsTrue (selectionItem1.IsSelected, "Checked");
		}
		
		[Test]
		public void RemoveFromSelectionTest ()
		{
			Form f = new Form ();
			
			RadioButton r1 = new RadioButton ();
			r1.Checked = false;
			f.Controls.Add (r1);
			
			IRawElementProviderSimple provider1 = ProviderFactory.GetProvider (r1);
			ISelectionItemProvider selectionItem1 = (ISelectionItemProvider)
				provider1.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			
			RadioButton r2 = new RadioButton ();
			r2.Checked = false;
			f.Controls.Add (r2);
			
			IRawElementProviderSimple provider2 = ProviderFactory.GetProvider (r2);
			ISelectionItemProvider selectionItem2 = (ISelectionItemProvider)
				provider2.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			
			// Should *always* throw InvalidOperationException
			
			try {
				selectionItem1.RemoveFromSelection ();
				Assert.Fail ("This method should always throw InvalidOperationException for RadioButtons");
			} catch (InvalidOperationException) {
				// Expected
			} catch (Exception e) {
				Assert.Fail ("This method should always throw InvalidOperationException for RadioButtons, instead threw: " + e.Message);
			}
			
			r1.Checked = true;
			
			try {
				selectionItem1.RemoveFromSelection ();
				Assert.Fail ("This method should always throw InvalidOperationException for RadioButtons");
			} catch (InvalidOperationException) {
				// Expected
			} catch (Exception e) {
				Assert.Fail ("This method should always throw InvalidOperationException for RadioButtons, instead threw: " + e.Message);
			}
		}
		
		[Test]
		public void SelectTest ()
		{
			Form f = new Form ();
			
			RadioButton r1 = new RadioButton ();
			r1.Checked = false;
			f.Controls.Add (r1);
			
			IRawElementProviderSimple provider1 = ProviderFactory.GetProvider (r1);
			ISelectionItemProvider selectionItem1 = (ISelectionItemProvider)
				provider1.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			
			selectionItem1.Select ();
			Assert.IsTrue (r1.Checked, "Selecting 1/1, initially unchecked");
			
			RadioButton r2 = new RadioButton ();
			r2.Checked = false;
			f.Controls.Add (r2);
			
			IRawElementProviderSimple provider2 = ProviderFactory.GetProvider (r2);
			ISelectionItemProvider selectionItem2 = (ISelectionItemProvider)
				provider2.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			
			selectionItem1.Select ();
			Assert.IsTrue (r1.Checked, "Selecting 1/2, 1 intially checked, should check 1");
			Assert.IsFalse (r2.Checked, "Selecting 1/2, 2 intially unchecked, should uncheck 2");
			
			selectionItem2.Select ();
			Assert.IsTrue (r2.Checked, "Selecting 2/2, 1 initially checked, should check 2");
			Assert.IsFalse (r1.Checked, "Selecting 2/2, 1 initially checked, should uncheck 1");
			
			selectionItem1.Select ();
			Assert.IsTrue (r1.Checked, "Selecting 1/2, 1 intially unchecked, should check 1");
			Assert.IsFalse (r2.Checked, "Selecting 1/2, 2 intially checked, should uncheck 2");
		}
		
		[Test]
		public void SelectionContainerTest ()
		{
			Form f = new Form ();
			
			IRawElementProviderSimple formProvider = ProviderFactory.GetProvider (f);
			
			RadioButton r1 = new RadioButton ();
			r1.Checked = false;
			
			RadioButton r2 = new RadioButton ();
			r2.Checked = false;
			
			f.Controls.Add (r1);
			f.Controls.Add (r2);
			
			IRawElementProviderSimple provider1 = ProviderFactory.FindProvider (r1);
			ISelectionItemProvider selectionItem1 = (ISelectionItemProvider)
				provider1.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			
			IRawElementProviderSimple provider2 = ProviderFactory.FindProvider (r2);
			ISelectionItemProvider selectionItem2 = (ISelectionItemProvider)
				provider2.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			
			Assert.AreEqual (formProvider, 
			                 selectionItem1.SelectionContainer,
			                 "SelectionContainer should be parent provider");
			Assert.AreEqual (formProvider,
			                 selectionItem2.SelectionContainer,
			                 "SelectionContainer should be parent provider");
		}
		
		[Test]
		public void ElementSelectedEventTest ()
		{
			Assert.Fail ("Not implemented");
		}
		
#endregion
		
#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return new RadioButton ();
		}
		
#endregion
	}
}
