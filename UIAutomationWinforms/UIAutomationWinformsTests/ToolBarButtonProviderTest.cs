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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Andrés G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.ComponentModel;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{

	[TestFixture]
	public class ToolBarButtonProviderTest : BaseProviderTest
	{
		[SetUp]
		public override void SetUp ()
		{
			base.SetUp ();

			toolBar = new ToolBar ();
			toolBarButton = new ToolBarButton ("Button");
			toolBar.Buttons.Add (toolBarButton);
			Form.Controls.Add (toolBar);
			Form.Show ();
		}

		protected override Control GetControlInstance ()
		{
			return null;
		}

		static ToolBar toolBar = null;
		static ToolBarButton toolBarButton = null;
		
		protected override IRawElementProviderSimple GetProvider ()
		{
			return ProviderFactory.GetProvider (toolBarButton);
		}
		
		#region Test

		[Test]
		public void BasicPropertiesTest ()
		{
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (toolBarButton);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.MenuItem.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "menu item");
			
			string value = "ToolBarButton Name Property";
			toolBarButton.Text = value;
			TestProperty (provider,
			              AutomationElementIdentifiers.NameProperty,
			              value);
		}

		[Test]
		//tested with UIAVerify, the bridge depends on this behaviour
		public override void IsKeyboardFocusablePropertyTest ()
		{
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (toolBarButton);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              true);
		}

		[Test]
		public void ProviderPatternTest ()
		{
			TestHelper.TestPatterns (GetProvider (), InvokePatternIdentifiers.Pattern);
		}

		#endregion

		#region Navigation Test

		[Test]
		public void NavigationTest ()
		{
			IRawElementProviderSimple childProvider,parentProvider;
			var provider = ProviderFactory.GetProvider (toolBarButton);
			parentProvider = ((IRawElementProviderFragment)provider).Navigate (NavigateDirection.Parent);
			Assert.IsNotNull (parentProvider, "We must have a parent");

			Assert.AreEqual (parentProvider, ProviderFactory.GetProvider (toolBar));

			parentProvider = ProviderFactory.GetProvider (toolBar);
			childProvider = ((IRawElementProviderFragment)parentProvider).Navigate (NavigateDirection.FirstChild);

			Assert.IsNotNull (childProvider, "We must have a child");

			Assert.AreEqual (childProvider, provider);
		}

		[Test]
		public void Visualization ()
		{
			var parentProvider = ProviderFactory.GetProvider (toolBar);
			var provider = ProviderFactory.GetProvider (toolBarButton);
			
			Assert.AreEqual (parentProvider.GetPropertyValue (AutomationElementIdentifiers.IsOffscreenProperty.Id),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.IsOffscreenProperty.Id));
		}

		#endregion
	}
}
