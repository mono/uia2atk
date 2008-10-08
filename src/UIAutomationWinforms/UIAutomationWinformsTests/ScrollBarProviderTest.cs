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
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	
	
	[TestFixture]
	public class ScrollBarProviderTest : BaseProviderTest
	{
		#region Basic Tests
		
		[Test]
		public void BasicPropertiesTest ()
		{
			ScrollBar scrollbar = (ScrollBar) GetControlInstance ();
			IRawElementProviderFragment provider = ProviderFactory.GetProvider (scrollbar);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.NameProperty,
			              null);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ClickablePointProperty,
			              Single.NaN);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LabeledByProperty,
			              null);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.ScrollBar.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "scroll bar");
			
			TestProperty (provider,
			              AutomationElementIdentifiers.OrientationProperty,
			              OrientationType.Horizontal);
			
			// Value returned from Vista tests.
			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              false); 
		}
		
		public override void LabeledByAndNamePropertyTest ()
		{
			//Name is not supported.
			TestLabeledByAndName (false, false);
		}
		
		#endregion
		

		#region Basic Tests

		[Test]
		public void NavigationTest ()
		{
			ScrollBar scrollbar = (ScrollBar) GetControlInstance ();
			IRawElementProviderFragment provider = GetProviderFromControl (scrollbar);
			
			Assert.AreEqual (null,
			                 provider.Navigate (NavigateDirection.NextSibling),
			                 "NextSibling should be null");
			
			Assert.AreEqual (null,
			                 provider.Navigate (NavigateDirection.PreviousSibling),
			                 "PreviousSibling should be null");
			
			IRawElementProviderFragment firstButton 
				= provider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (firstButton, "FirstChild shouldn't be null");
						
			Assert.AreEqual (firstButton.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id),
			                 ControlType.Button.Id);
			
			IRawElementProviderFragment fragment 
				= firstButton.Navigate (NavigateDirection.PreviousSibling);
			Assert.IsNull (fragment, "FirstChild.PreviousSibling must be null");
			
			IRawElementProviderFragment secondButton
				= firstButton.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (secondButton, "firstButton.NextSibling shouldn't be null");
			
			Assert.AreEqual (secondButton.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id),
			                 ControlType.Button.Id);
			
			IRawElementProviderFragment thumbProvider
				= secondButton.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (thumbProvider, "secondButton.NextSibling shouldn't be null");

			Assert.AreEqual (thumbProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id),
			                 ControlType.Thumb.Id);
			
			IRawElementProviderFragment thirdButton
				= thumbProvider.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (thirdButton, "thumbProvider.NextSibling shouldn't be null");
			
			Assert.AreEqual (thirdButton.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id),
			                 ControlType.Button.Id);
			
			IRawElementProviderFragment fourthButton
				= thirdButton.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (fourthButton, "thirdButton.NextSibling shouldn't be null");
			
			Assert.AreEqual (fourthButton.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id),
			                 ControlType.Button.Id);

			Assert.IsNull (fourthButton.Navigate (NavigateDirection.NextSibling),
			               "fourthButton.NextSibling should be null");
			
			//All parents should be the same reference:
			Assert.AreEqual (provider,
			                 firstButton.Navigate (NavigateDirection.Parent),
			               "firstButton with different Parent");
			Assert.AreEqual (provider,
			                 firstButton.FragmentRoot,
			               "firstButton with different FragmentRoot");
			Assert.AreEqual (provider,
			                 secondButton.Navigate (NavigateDirection.Parent),
			               "secondButton with different Parent");
			Assert.AreEqual (provider,
			                 secondButton.FragmentRoot,
			               "secondButton with different FragmentRoot");
			Assert.AreEqual (provider,
			                 thumbProvider.Navigate (NavigateDirection.Parent),
			               "thumbProvider with different Parent");
			Assert.AreEqual (provider,
			                 thumbProvider.FragmentRoot,
			               "thumbProvider with different FragmentRoot");
			Assert.AreEqual (provider,
			                 thirdButton.Navigate (NavigateDirection.Parent),
			               "thirdButton with different Parent");	
			Assert.AreEqual (provider,
			                 thirdButton.FragmentRoot,
			               "thirdButton with different FragmentRoot");
			Assert.AreEqual (provider,
			                 fourthButton.Navigate (NavigateDirection.Parent),
			               "fourthButton with different Parent");
			Assert.AreEqual (provider,
			                 fourthButton.FragmentRoot,
			               "fourthButton with different FragmentRoot");			
			
			//All children MUST not have any children			
			Assert.AreEqual (null,
			                 firstButton.Navigate (NavigateDirection.FirstChild),
			               "firstButton.FirstChild must be null");
			Assert.AreEqual (null,
			                 firstButton.Navigate (NavigateDirection.LastChild),
			               "firstButton.LastChild must be null");
			
			Assert.AreEqual (null,
			                 secondButton.Navigate (NavigateDirection.FirstChild),
			               "secondButton.FirstChild must be null");
			Assert.AreEqual (null,
			                 secondButton.Navigate (NavigateDirection.LastChild),
			               "secondButton.LastChild must be null");
			
			Assert.AreEqual (null,
			                 thumbProvider.Navigate (NavigateDirection.FirstChild),
			               "thumbProvider.FirstChild with different Parent");
			Assert.AreEqual (null,
			                 thumbProvider.Navigate (NavigateDirection.LastChild),
			               "thumbProvider.LastChild with different Parent");
			
			Assert.AreEqual (null,
			                 thirdButton.Navigate (NavigateDirection.FirstChild),
			               "thirdButton.FirstChild with different Parent");
			Assert.AreEqual (null,
			                 thirdButton.Navigate (NavigateDirection.LastChild),
			               "thirdButton.LastChild with different Parent");
			
			Assert.AreEqual (null,
			                 fourthButton.Navigate (NavigateDirection.FirstChild),
			               "fourthButton.FirstChild with different Parent");
			Assert.AreEqual (null,
			                 fourthButton.Navigate (NavigateDirection.LastChild),
			               "fourthButton.LastChild with different Parent");			
		}

		#endregion		

		#region BaseProviderTest Overrides
		
		protected override bool IsContentElement {
			get { return false; }
		}

		protected override Control GetControlInstance ()
		{
			return new HScrollBar ();
		}
		
		#endregion
		
		#region Patterns test
		
		[Test]
		public void PatternsTest ()
		{
			ListBox listbox = new ListBox ();
			
			for (int i = 0; i < 15; i++)
				listbox.Items.Add (i);

			IRawElementProviderFragmentRoot listboxProvider
				= (IRawElementProviderFragmentRoot) GetProviderFromControl (listbox);
			
			IRawElementProviderFragment scrollBarProvider = null;
			IRawElementProviderFragment child = 
				listboxProvider.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.ScrollBar.Id) {
					scrollBarProvider = child;
					break;
				}
				child = child.Navigate (NavigateDirection.NextSibling);
			}
			
			Assert.IsNotNull (scrollBarProvider, "Should be scrollbar");
			
			//LAMESPEC: This SHOULD BE Assert.IsFalse
			Assert.IsTrue ((bool) child.GetPropertyValue (AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty.Id),
			               "Should support RangeValue Pattern");
			
			//Lets test buttons!
			//IRawElementProviderFragment firstButton = null;
		}
		
		#endregion
	}
}
