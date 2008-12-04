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

		#region ToolBarButton Test

		[Test]
        	public void ToolBarButtonBasicPropertiesTest ()
        	{
            		ToolBar toolBar = new ToolBar ();
            		IRawElementProviderFragmentRoot rootProvider =
				(IRawElementProviderFragmentRoot) GetProviderFromControl (toolBar);
			
			toolBar.Buttons.Add ("Button");
			IRawElementProviderFragment childProvider =
				rootProvider.Navigate (NavigateDirection.FirstChild);

			TestProperty (childProvider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.MenuItem.Id);

			TestProperty (childProvider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "menu item");
			
			string value = "ToolBarButton Name Property";
			toolBar.Buttons [0].Text = value;
			TestProperty (childProvider,
			              AutomationElementIdentifiers.NameProperty,
			              value);
		}

		[Test]
		public void ToolBarButtonProviderPatternTest ()
		{
			ToolBar toolBar = new ToolBar ();
            		IRawElementProviderFragmentRoot rootProvider =
				(IRawElementProviderFragmentRoot) GetProviderFromControl (toolBar);
			
			toolBar.Buttons.Add ("Button");
			IRawElementProviderFragment childProvider =
				rootProvider.Navigate (NavigateDirection.FirstChild);
			
			object invokeProvider =
				childProvider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (invokeProvider,
			                  "Not returning InvokePatternIdentifiers.");
			Assert.IsTrue (invokeProvider is IInvokeProvider,
			               "Not returning InvokePatternIdentifiers.");
		}

		#endregion

		#region BaseProviderTest Overrides

        	protected override Control GetControlInstance ()
        	{
            		return new ToolBar ();
        	}

		public override void LabeledByAndNamePropertyTest ()
		{
			TestLabeledByAndName (false, false);
		}

		#endregion
	}
}
