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
// Copyright (c) 2008,2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Neville Gao <nevillegao@gmail.com>
//	Andrés G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class ToolBarProviderTest : BaseProviderTest
	{
		#region Test

		[Test]
		public void BasicPropertiesTest ()
		{
			ToolBar toolBar = new ToolBar ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (toolBar);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.ToolBar.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "tool bar");
			
			string value = "ToolBar Name Property";
			toolBar.Text = value;
			TestProperty (provider,
			              AutomationElementIdentifiers.NameProperty,
			              value);
		}

		[Test]
		public void ProviderPatternTest ()
		{
			TestHelper.TestPatterns (GetProvider ()); //no patterns
		}

		[Test]
		//tested with UIAVerify, the bridge depends on this behaviour
		public override void IsKeyboardFocusablePropertyTest ()
		{
			var toolbar = (ToolBar)GetControlInstanceWithButton ();
			TestFocus ();

			IInvokeProvider invokeProvider = (IInvokeProvider)
				ProviderFactory.GetProvider (toolbar.Buttons [0])
					.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			invokeProvider.Invoke ();

			TestFocus ();
		}

		private void TestFocus ()
		{
			var toolbar = (ToolBar)GetControlInstanceWithButton ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (toolbar);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              false);

			object hasKbFocus = provider.GetPropertyValue (AutomationElementIdentifiers.HasKeyboardFocusProperty.Id);
			Assert.IsNotNull (hasKbFocus);
			Assert.IsTrue (hasKbFocus is bool);
			Assert.IsFalse ((bool)hasKbFocus);
		}

		#endregion

		#region Navigation Test

		[Test]
		public void NavigationTest ()
		{
			ToolBar toolBar = (ToolBar) GetControlInstance ();
			IRawElementProviderFragmentRoot rootProvider;
			IRawElementProviderFragment childProvider;
			IRawElementProviderFragment childParent;

			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (toolBar);
			
			int index = 0, elements = 10;
			string name = string.Empty;
			for (; index < elements; ++index)
				toolBar.Buttons.Add (string.Format ("Button: {0}", index));
			index = 0;
			
			childProvider = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (childProvider, "We must have a child");
			
			do {
				childParent = childProvider.Navigate (NavigateDirection.Parent);
				Assert.AreSame (rootProvider, childParent,
				                "Each child must have same parent");
				name = (string) childProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
				Assert.AreEqual (string.Format ("Button: {0}", index++), 
				                 name, "Different names");
				childProvider = childProvider.Navigate (NavigateDirection.NextSibling);
			} while (childProvider != null);
			Assert.AreEqual (elements, index, "Elements Added = Elements Navigated");

			toolBar.Buttons.Clear ();

			childProvider = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (childProvider, "We shouldn't have a child");
		}

		#endregion

		#region BaseProviderTest Overrides

		static ToolBar toolbar = new ToolBar ();
		
		protected override Control GetControlInstance ()
		{
			return new ToolBar ();
		}

		protected Control GetControlInstanceWithButton ()
		{
			if (toolbar.Buttons.Count == 0)
				toolbar.Buttons.Add ("one butt");
			return toolbar;
		}

		public override void LabeledByAndNamePropertyTest ()
		{
			TestLabeledByAndName (false, false);
		}

		#endregion
	}
}
