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
//	Neville Gao <nevillegao@gmail.com>
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
    	public class DomainUpDownProviderTest : BaseProviderTest
    	{
		#region Test

		[Test]
		public void BasicPropertiesTest ()
		{
			DomainUpDown domainUpDown = new DomainUpDown ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (domainUpDown);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Spinner.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "spinner");
		}

		[Test]
		public void ProviderPatternTest ()
		{
			DomainUpDown domainUpDown = new DomainUpDown ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (domainUpDown);
			
			object valueProvider =
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider,
			                  "Not returning ValuePatternIdentifiers.");
			Assert.IsTrue (valueProvider is IValueProvider,
			               "Not returning ValuePatternIdentifiers.");

			object selectionProvider =
				provider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (selectionProvider,
			                  "Not returning SelectionPatternIdentifiers.");
			Assert.IsTrue (selectionProvider is ISelectionProvider,
			               "Not returning SelectionPatternIdentifiers.");
		}

		#endregion

		#region ISelectionPattern Test
		
		[Test]
		public void ISelectionPatternTest ()
		{
			DomainUpDown domainUpDown = new DomainUpDown ();
			domainUpDown.Items.Add ("First");
			domainUpDown.Items.Add ("Second");
			domainUpDown.Items.Add ("Third");
			domainUpDown.Items.Add ("Fourth");
			Form.Controls.Add (domainUpDown);

			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (domainUpDown);
			ISelectionProvider prov
				= (ISelectionProvider)provider.GetPatternProvider (
					SelectionPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (prov);
	
			IRawElementProviderSimple[] items;
			
			// Test initial no selection
			Assert.AreEqual (-1, domainUpDown.SelectedIndex);
			items = prov.GetSelection ();
			Assert.IsNotNull (items, "Should never return null");
			Assert.AreEqual (0, items.Length, "Too many items returned");

			// Test first item selection
			domainUpDown.SelectedIndex = 0;
			Assert.AreEqual (0, domainUpDown.SelectedIndex);

			items = prov.GetSelection ();
			Assert.IsNotNull (items, "Should never return null");
			Assert.AreEqual (1, items.Length, "Too many or too few items returned");
			
			IValueProvider item_val_prov
				= (IValueProvider)items[0].GetPatternProvider (
					ValuePatternIdentifiers.Pattern.Id);
			Assert.AreEqual ("First", item_val_prov.Value);
		}

		#endregion

		#region IValuePattern Test
		
		[Test]
		public void IValueProviderIsReadOnlyTest ()
		{
			DomainUpDown domainUpDown = new DomainUpDown ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (domainUpDown);
			
			IValueProvider valueProvider = (IValueProvider)
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider,
			                  "Not returning ValuePatternIdentifiers.");
			
			Assert.AreEqual (domainUpDown.ReadOnly,
			                 valueProvider.IsReadOnly,
			                 "IsReadOnly value");
		}
		
		[Test]
		public void IValueProviderValueTest ()
		{
			DomainUpDown domainUpDown = new DomainUpDown ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (domainUpDown);
			
			IValueProvider valueProvider = (IValueProvider)
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider,
			                  "Not returning ValuePatternIdentifiers.");
			
			string value = "Item";
			domainUpDown.Items.Add (value);
			domainUpDown.DownButton ();
			Assert.AreEqual (value, valueProvider.Value, "Value value");
		}
		
		[Test]
		public void IValueProviderSetValueTest ()
		{
			DomainUpDown domainUpDown = new DomainUpDown ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (domainUpDown);
			
			IValueProvider valueProvider = (IValueProvider)
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (valueProvider,
			                  "Not returning ValuePatternIdentifiers.");
			
			try {
				domainUpDown.Enabled = false;
				valueProvider.SetValue ("NEW Item");
				Assert.Fail ("ElementNotEnabledException not thrown.");
			} catch (ElementNotEnabledException) { }
			
			domainUpDown.Enabled = true;
			string value = "NEW Item";
			valueProvider.SetValue (value);
			domainUpDown.DownButton ();
			Assert.AreEqual(value, valueProvider.Value, "SetValue value");
		}
		
		#endregion
		
		#region Navigation Test

		[Test]
		public void NavigationTest ()
		{
			DomainUpDown domainUpDown = (DomainUpDown) GetControlInstance ();
			IRawElementProviderFragmentRoot rootProvider;
			IRawElementProviderFragment forwardButtonProvider;
			IRawElementProviderFragment backwardButtonProvider;
			
			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (domainUpDown);
			
			forwardButtonProvider = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (forwardButtonProvider,
			                  "ForwardButton shouldn't be null.");
			Assert.IsNull (forwardButtonProvider.Navigate (NavigateDirection.PreviousSibling),
			               "ForwardButton should be the first child.");
			
			backwardButtonProvider = forwardButtonProvider.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (backwardButtonProvider,
			                  "BackwardButton shouldn't be null.");
			Assert.IsNull (backwardButtonProvider.Navigate (NavigateDirection.NextSibling),
			               "BackwardButton should be the last child.");
			
			Assert.AreEqual (rootProvider,
			                 forwardButtonProvider.Navigate (NavigateDirection.Parent),
			                 "ForwardButton with different parent");
			Assert.AreEqual (rootProvider,
			                 forwardButtonProvider.FragmentRoot,
			                 "ForwardButton with different FragmentRoot");
			Assert.AreEqual (rootProvider,
			                 backwardButtonProvider.Navigate (NavigateDirection.Parent),
			                 "BackwardButton with different parent");
			Assert.AreEqual (rootProvider,
			                 backwardButtonProvider.FragmentRoot,
			                 "BackwardButton with different FragmentRoot");
			
			TestProperty (forwardButtonProvider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Button.Id);
			TestProperty (forwardButtonProvider,
			              AutomationElementIdentifiers.IsContentElementProperty,
			              false);
			TestProperty (forwardButtonProvider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              false);
		}

		[Test]
		public void ItemsNavigationTest ()
		{
			DomainUpDown domainUpDown = (DomainUpDown) GetControlInstance ();
			domainUpDown.Items.Add ("First");
			domainUpDown.Items.Add ("Second");
			domainUpDown.Items.Add ("Third");
			domainUpDown.Items.Add ("Fourth");
			Form.Controls.Add (domainUpDown);

			IRawElementProviderFragmentRoot rootProvider;
			IRawElementProviderFragment childProvider;
			
			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (domainUpDown);

			// Skip over the Forward and Backward buttons
			childProvider = rootProvider.Navigate (NavigateDirection.FirstChild);
			childProvider = childProvider.Navigate (NavigateDirection.NextSibling);
			childProvider = childProvider.Navigate (NavigateDirection.NextSibling);
			
			IValueProvider item_val_prov
				= (IValueProvider)childProvider.GetPatternProvider (
					ValuePatternIdentifiers.Pattern.Id);
			Assert.AreEqual ("First", item_val_prov.Value);

			childProvider = childProvider.Navigate (NavigateDirection.NextSibling);
			item_val_prov
				= (IValueProvider)childProvider.GetPatternProvider (
					ValuePatternIdentifiers.Pattern.Id);
			Assert.AreEqual ("Second", item_val_prov.Value);

			// Try to select the Second item
			ISelectionItemProvider sel_item_prov
				= (ISelectionItemProvider)childProvider.GetPatternProvider (
					SelectionItemPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (sel_item_prov);

			sel_item_prov.Select ();
			Assert.AreEqual (1, domainUpDown.SelectedIndex);

			// Verify that the SelectionProvider reflects the new
			// selection
			ISelectionProvider sel_prov
				= (ISelectionProvider)rootProvider.GetPatternProvider (
					SelectionPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (sel_prov);
			
			IRawElementProviderSimple[] children = sel_prov.GetSelection ();
			Assert.IsNotNull (children);
			Assert.AreEqual (1, children.Length);
			
			IValueProvider val_prov
				= (IValueProvider)children[0].GetPatternProvider (
					ValuePatternIdentifiers.Pattern.Id);
			Assert.AreEqual ("Second", val_prov.Value);
		}
		
		#endregion
		
		#region BaseProviderTest Overrides

		protected override Control GetControlInstance ()
		{
			return new DomainUpDown ();
		}

		#endregion
	}
}
