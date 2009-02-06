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
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class ContextMenuStripProviderTest : BaseProviderTest
	{
		#region Tests
		
		[Test]
		public void BasicPropertiesTest ()
		{
			IRawElementProviderSimple provider = GetProvider ();
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Menu.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "menu");
		}

		// TODO: Test add/removal of items, navigation, etc

		[Test]
		public void ShowHideTest ()
		{
			ContextMenuStrip formMenu = new ContextMenuStrip ();
			formMenu.Items.Add ("form item 1");
			Form.ContextMenuStrip = formMenu;

			Label label = new Label ();
			label.Text = "I have my own context menu";
			ContextMenuStrip labelMenu = new ContextMenuStrip ();
			labelMenu.Items.Add ("label item 1");
			label.ContextMenuStrip = labelMenu;
			Form.Controls.Add (label);
			label.Show ();

			var labelProvider = ProviderFactory.FindProvider (label);
			Assert.IsNotNull (labelProvider,
			                  "Form's label should have a provider");

			// No menu providers unless shown
			IRawElementProviderFragment formMenuProvider =
				ProviderFactory.FindProvider (formMenu);
			Assert.IsNull (formMenuProvider,
			               "Form menu provider not expected until shown");
			IRawElementProviderFragment labelMenuProvider =
				ProviderFactory.FindProvider (labelMenu);
			Assert.IsNull (labelMenuProvider,
			               "Label menu provider not expected until shown");

			bridge.ResetEventLists ();

			// Test showing Form menu
			formMenu.Show ();

			formMenuProvider =
				ProviderFactory.FindProvider (formMenu);
			Assert.IsNotNull (formMenuProvider,
			                  "Form menu provider expected once menu is shown");
			labelMenuProvider =
				ProviderFactory.FindProvider (labelMenu);
			Assert.IsNull (labelMenuProvider,
			               "Label menu provider not expected until shown");

			Assert.AreEqual (FormProvider,
			                 formMenuProvider.Navigate (NavigateDirection.Parent),
			                 "Form menu parent should be Form");
			Assert.IsTrue (ProviderContainsChild (FormProvider, formMenuProvider),
			               "Form menu should be Form's child");

			var tuple = bridge.GetAutomationEventFrom (formMenuProvider,
			                                           AEIds.MenuOpenedEvent.Id);
			Assert.IsNotNull (tuple,
			                  "MenuOpenedEvent expected");

			bridge.ResetEventLists ();

			// Test closing Form menu
			formMenu.Close ();

			tuple = bridge.GetAutomationEventFrom (formMenuProvider,
			                                       AEIds.MenuClosedEvent.Id);
			Assert.IsNotNull (tuple,
			                  "MenuClosedEvent expected");

			formMenuProvider =
				ProviderFactory.FindProvider (formMenu);
			Assert.IsNull (formMenuProvider,
			               "Form menu provider should be gone after menu closed");
			Assert.IsFalse (ProviderContainsChildOfType (FormProvider, ControlType.Menu),
			                "Form should have no menu child after menu closed");

			bridge.ResetEventLists ();

			// Test showing Label menu
			labelMenu.Show ();

			formMenuProvider =
				ProviderFactory.FindProvider (formMenu);
			Assert.IsNull (formMenuProvider,
			                  "Form menu provider not expected while closed");
			labelMenuProvider =
				ProviderFactory.FindProvider (labelMenu);
			Assert.IsNotNull (labelMenuProvider,
			               "Label menu provider expected once menu is shown");

			Assert.AreEqual (labelProvider,
			                 labelMenuProvider.Navigate (NavigateDirection.Parent),
			                 "Label menu parent should be Label");
			Assert.IsTrue (ProviderContainsChild (labelProvider, labelMenuProvider),
			               "Label menu should be Label's child");

			tuple = bridge.GetAutomationEventFrom (labelMenuProvider,
			                                           AEIds.MenuOpenedEvent.Id);
			Assert.IsNotNull (tuple,
			                  "MenuOpenedEvent expected");

			bridge.ResetEventLists ();

			// Test closing Label menu
			labelMenu.Close ();

			tuple = bridge.GetAutomationEventFrom (labelMenuProvider,
			                                       AEIds.MenuClosedEvent.Id);
			Assert.IsNotNull (tuple,
			                  "MenuClosedEvent expected");

			labelMenuProvider =
				ProviderFactory.FindProvider (labelMenu);
			Assert.IsNull (labelMenuProvider,
			               "Label menu provider should be gone after menu closed");
			Assert.IsFalse (ProviderContainsChildOfType (labelProvider, ControlType.Menu),
			                "Label should have no menu child after menu closed");
		}

		private bool ProviderContainsChild (IRawElementProviderFragment parent,
		                                    IRawElementProviderFragment potentialChild)
		{
			IRawElementProviderFragment currentChild =
				parent.Navigate (NavigateDirection.FirstChild);
			
			while (currentChild != null) {
				if (currentChild == potentialChild)
					return true;
				currentChild = currentChild.Navigate (NavigateDirection.NextSibling);
			}
				
			return false;
		}

		private bool ProviderContainsChildOfType (IRawElementProviderFragment parent,
		                                          ControlType searchType)
		{
			IRawElementProviderFragment currentChild =
				parent.Navigate (NavigateDirection.FirstChild);
			
			while (currentChild != null) {
				if (currentChild.GetPropertyValue (AEIds.ControlTypeProperty.Id) == searchType)
					return true;
				currentChild = currentChild.Navigate (NavigateDirection.NextSibling);
			}
				
			return false;
		}
		
		#endregion

		#region Overridden Tests
		
		[Test]
		public override void LabeledByAndNamePropertyTest ()
		{
			TestLabeledByAndName (false, false);
		}

		[Test]
		public override void IsKeyboardFocusablePropertyTest ()
		{
			IRawElementProviderSimple provider = GetProvider ();
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              false);
		}

		[Test]
		public override void FragmentRootAsParentTest ()
		{			
			IRawElementProviderFragment fragment 
				= GetProvider () as IRawElementProviderFragment;
			Assert.AreEqual (fragment, fragment.FragmentRoot, "FragmentRoot != Fragment");
		}

		#endregion

		#region Other Overridden Members
		
		protected override Control GetControlInstance ()
		{
			return null;//new ContextMenuStrip ();
		}

		protected override IRawElementProviderSimple GetProvider ()
		{
			ContextMenuStrip menu = new ContextMenuStrip ();
			menu.Items.Add ("item1");
			Form.ContextMenuStrip = menu;
			menu.Show ();
			return ProviderFactory.FindProvider (menu);
		}

		protected override bool IsContentElement {
			get {
				return false;
			}
		}

		#endregion
	}
}
