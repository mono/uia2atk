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
//	Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class MainMenuProviderTest : BaseProviderTest
	{
		#region Tests
		
		[Test]
		public void BasicPropertiesTest ()
		{
			MainMenu menu = new MainMenu ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (menu);
			
			TestProperty (provider,
			              AEIds.ControlTypeProperty,
			              ControlType.MenuBar.Id);
			
			TestProperty (provider,
			              AEIds.LocalizedControlTypeProperty,
			              "menu bar");

			TestProperty (provider,
			              AEIds.AcceleratorKeyProperty,
			              null);

			TestProperty (provider,
			              AEIds.LabeledByProperty,
			              null);

			TestProperty (provider,
			              AEIds.OrientationProperty,
			              OrientationType.Horizontal);

			TestProperty (provider,
			              AEIds.AccessKeyProperty,
			              "ALT");

			TestProperty (provider,
			              AEIds.NameProperty,
			              "Application");

			TestProperty (provider,
			              AEIds.IsKeyboardFocusableProperty,
			              true);
		}
		
		[Test]
		public void ProviderPatternTest ()
		{
			MainMenu menu = new MainMenu ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (menu);

			// Should never support Transform
			object transformProvider = provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			Assert.IsNull (transformProvider, "Transform pattern should not be supported");

			// Should never support ExpandCollapse
			object expandCollapseProvider = provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);
			Assert.IsNull (expandCollapseProvider, "ExpandCollapse pattern should not be supported");

			// Should never support Dock
			object dockProvider = provider.GetPatternProvider (DockPatternIdentifiers.Pattern.Id);
			Assert.IsNull (dockProvider, "Dock pattern should not be supported");
		}

		// NOTE: This tests navigation for MainMenu and MenuItem
		[Test]
		public void NavigationTest ()
		{
			MainMenu menu = new MainMenu ();
			MenuItem item1 = menu.MenuItems.Add ("item1");
			MenuItem item2 = menu.MenuItems.Add ("item2");
			MenuItem item1sub1 = item1.MenuItems.Add ("item1 sub1");

			Form.Menu = menu;

			var menuProvider = (IRawElementProviderFragmentRoot)
				ProviderFactory.FindProvider (menu);
			Assert.IsNotNull (menuProvider, "Provider should be added as soon as Form.Menu is set");

			Assert.AreEqual (FormProvider,
			                 menuProvider.Navigate (NavigateDirection.Parent),
			                 "MainMenu parent is Form");

			var item1Provider = GetAndAssertNonNullMenuItem (item1);
			var item2Provider = GetAndAssertNonNullMenuItem (item2);
			var item1sub1Provider = GetAndAssertNonNullMenuItem (item1sub1);

			// Test basic initial navigation
			var item1MenuProvider = item1Provider.Navigate (NavigateDirection.FirstChild);
			Assert.AreEqual (ControlType.Menu.Id,
			                 item1MenuProvider.GetPropertyValue (AEIds.ControlTypeProperty.Id),
			                 "Menu child of item 1 ControlType");

			Assert.AreEqual (item1Provider,
			                 menuProvider.Navigate (NavigateDirection.FirstChild),
			                 "First child should be item1");
			Assert.AreEqual (item2Provider,
			                 menuProvider.Navigate (NavigateDirection.LastChild),
			                 "Last child should be item2");
			Assert.AreEqual (item2Provider,
			                 item1Provider.Navigate (NavigateDirection.NextSibling),
			                 "item2 should be item1's next sibling");
			Assert.AreEqual (menuProvider,
			                 item1Provider.Navigate (NavigateDirection.Parent),
			                 "menu should be item1's parent");
			
			Assert.AreEqual (item1MenuProvider,
			                 item1Provider.Navigate (NavigateDirection.FirstChild),
			                 "item1's hidden menu should be item1's first child");
			Assert.AreEqual (item1sub1Provider,
			                 item1MenuProvider.Navigate (NavigateDirection.FirstChild),
			                 "item1sub1 should be item1's hidden menu's first child");
			Assert.AreEqual (item1MenuProvider,
			                 item1sub1Provider.Navigate (NavigateDirection.Parent),
			                 "item1's hidden menu should be item1sub1's parent");
			Assert.AreEqual (item1Provider,
			                 item1MenuProvider.Navigate (NavigateDirection.Parent),
			                 "item1 should be item1's hidden menu's parent");

			// Test adding items dynamically
			MenuItem item3 = menu.MenuItems.Add ("item3");
			GetAndAssertNonNullMenuItem (item3);

			MenuItem item2sub1 = item2.MenuItems.Add ("item2 sub1");
			var item2sub1Provider = GetAndAssertNonNullMenuItem (item2sub1);

			MenuItem item1sub2 = item1.MenuItems.Add ("item1 sub2");
			GetAndAssertNonNullMenuItem (item1sub2);

			MenuItem item1sub1sub1 = item1sub1.MenuItems.Add ("item1 sub1 sub1");
			GetAndAssertNonNullMenuItem (item1sub1sub1);

			// Test some stuff with hidden menu
			var item2MenuProvider = item2Provider.Navigate (NavigateDirection.FirstChild);
			Assert.AreEqual (item2sub1Provider,
			                 item2MenuProvider.Navigate (NavigateDirection.FirstChild),
			                 "Hidden menu setup when item added dynamically");
			item2.MenuItems.Remove (item2sub1);
			item2MenuProvider = item2Provider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (item2MenuProvider,
			               "Hidden menu gone when item removed dynamically");
			Assert.IsNull (ProviderFactory.FindProvider (item2sub1),
			               "Sub-item provider gone after being removed dynamically");
			item2.MenuItems.Add (item2sub1);
			item2sub1Provider = GetAndAssertNonNullMenuItem (item2sub1);
			item2MenuProvider = item2Provider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (item2MenuProvider,
			                  "Hidden menu returns when item re-added");
			Assert.AreEqual (item2sub1Provider,
			                 item2MenuProvider.Navigate (NavigateDirection.FirstChild),
			                 "Hidden menu hierarchy correct when item re-added");

			// Test recursive removal
			menu.MenuItems.Remove (item1);
			Assert.IsNull (ProviderFactory.FindProvider (item1),
			               "Item 1 and all sub-items removed (item1)");
			Assert.IsNull (ProviderFactory.FindProvider (item1sub1),
			               "Item 1 and all sub-items removed (item1sub1)");
			Assert.IsNull (ProviderFactory.FindProvider (item1sub2),
			               "Item 1 and all sub-items removed (item1sub2)");
			Assert.IsNull (ProviderFactory.FindProvider (item1sub1sub1),
			               "Item 1 and all sub-items removed (item1sub1sub1)");

			Assert.AreEqual (item2Provider,
			                 menuProvider.Navigate (NavigateDirection.FirstChild),
			                 "After removing item1, item2 is menu's first child");
			
		}

		private IRawElementProviderFragment GetAndAssertNonNullMenuItem (MenuItem item)
		{
			var itemProvider = (IRawElementProviderFragment)
				ProviderFactory.FindProvider (item);
			Assert.IsNotNull (itemProvider,
			                  "Provider should be generated as soon as item is added");
			return itemProvider;
		}

		#endregion

		protected override Control GetControlInstance ()
		{
			return null;
		}

		protected override IRawElementProviderSimple GetProvider ()
		{
			Form.Menu = new MainMenu ();
			return ProviderFactory.GetProvider (Form.Menu);
		}


	}
}
