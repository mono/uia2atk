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
			              AutomationElementIdentifiers.IsContentElementProperty,
			              false);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsControlElementProperty,
			              true);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.OrientationProperty,
			              OrientationType.Horizontal);
		}
		
		[Test]
		public override void NamePropertyTest ()
		{
			//Name is not supported.
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
			
			IRawElementProviderFragment fragment 
				= firstButton.Navigate (NavigateDirection.PreviousSibling);
			Assert.IsNull (fragment, "FirstChild.PreviousSibling must be null");
			
			IRawElementProviderFragment secondButton
				= firstButton.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (secondButton, "firstButton.NextSibling shouldn't be null");
			
			IRawElementProviderFragment thumbProvider
				= secondButton.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (thumbProvider, "secondButton.NextSibling shouldn't be null");
			
			IRawElementProviderFragment thirdButton
				= thumbProvider.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (thirdButton, "thumbProvider.NextSibling shouldn't be null");
			
			IRawElementProviderFragment fourthButton
				= thirdButton.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (fourthButton, "thirdButton.NextSibling shouldn't be null");

			Assert.IsNull (fourthButton.Navigate (NavigateDirection.NextSibling),
			               "fourthButton.NextSibling should be null");
			
			//All parents should be the same reference:
			Assert.AreEqual (provider,
			                 firstButton.Navigate (NavigateDirection.Parent),
			               "firstButton with different Parent");
			Assert.AreEqual (provider,
			                 secondButton.Navigate (NavigateDirection.Parent),
			               "secondButton with different Parent");
			Assert.AreEqual (provider,
			                 thumbProvider.Navigate (NavigateDirection.Parent),
			               "thumbProvider with different Parent");
			Assert.AreEqual (provider,
			                 thirdButton.Navigate (NavigateDirection.Parent),
			               "thirdButton with different Parent");
			Assert.AreEqual (provider,
			                 fourthButton.Navigate (NavigateDirection.Parent),
			               "fourthButton with different Parent");
			
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

		protected override Control GetControlInstance ()
		{
			return new HScrollBar ();
		}

		public override void LabeledByPropertyTest ()
		{
			TestLabeledBy (false);
		}
		
		#endregion
	}
}
