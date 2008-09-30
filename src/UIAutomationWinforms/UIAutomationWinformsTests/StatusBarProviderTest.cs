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
// 	Neville Gao <nevillegao@gmail.com>
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
    	public class StatusBarProviderTest : BaseProviderTest
    	{
		#region Test

        	[Test]
        	public void BasicPropertiesTest ()
        	{
            		StatusBar statusBar = new StatusBar ();
            		IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (statusBar);

			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.StatusBar.Id);

			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "status bar");
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LabeledByProperty,
			              null);
			
			string value = "Name Property";
			statusBar.Text = value;
			TestProperty (provider,
			              AutomationElementIdentifiers.NameProperty,
			              value);
		}

		[Test]
		public void ProviderPatternTest ()
		{
			StatusBar statusBar = new StatusBar ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (statusBar);

			object gridProvider =
				provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (gridProvider,
			                  "Not returning GridPatternIdentifiers.");
			Assert.IsTrue (gridProvider is IGridProvider,
			               "Not returning GridPatternIdentifiers.");
		}

		#endregion

		#region IGridPattern Test

		[Test]
		public void IGridProviderRowCountTest ()
		{
			StatusBar statusBar = new StatusBar ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (statusBar);

			IGridProvider gridProvider = (IGridProvider)
				provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (gridProvider,
			                  "Not returning GridPatternIdentifiers.");
			
			int index = 0, elements = 10;
			for (; index < elements; ++index)
				statusBar.Panels.Add (string.Format ("Panel: {0}", index));
			int value = 1;
			Assert.AreEqual (gridProvider.RowCount, value, "RowCount value");
		}

		[Test]
		public void IGridProviderColumnCountTest ()
		{
			StatusBar statusBar = new StatusBar ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (statusBar);

			IGridProvider gridProvider = (IGridProvider)
				provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (gridProvider,
			                  "Not returning GridPatternIdentifiers.");
			
			int index = 0, elements = 10;
			for (; index < elements; ++index)
				statusBar.Panels.Add (string.Format ("Panel: {0}", index));
			int value = elements;
			Assert.AreEqual (gridProvider.ColumnCount, value, "ColumnCount value");
		}

		[Test]
		public void IGridProviderGetItemTest ()
		{
			StatusBar statusBar = new StatusBar ();
			IRawElementProviderFragmentRoot provider =
				(IRawElementProviderFragmentRoot) GetProviderFromControl (statusBar);

			IGridProvider gridProvider = (IGridProvider)
				provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (gridProvider,
			                  "Not returning GridPatternIdentifiers.");
			
			int rowValue = 0;
			int columnValue = 0;
			statusBar.Panels.Add ("Panel");
			IRawElementProviderSimple panelProvider = (IRawElementProviderSimple)
				provider.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderSimple itemProvider =
				gridProvider.GetItem (rowValue, columnValue);
			Assert.AreSame (itemProvider, panelProvider, "GetItem method");
			
			try {
				rowValue = gridProvider.RowCount;
				columnValue = gridProvider.ColumnCount;
				itemProvider = gridProvider.GetItem (rowValue, columnValue);
				Assert.Fail ("ArgumentOutOfRangeException not thrown.");
			} catch (ArgumentOutOfRangeException) { }

			try {
				rowValue = -1;
				columnValue = -1;
				itemProvider = gridProvider.GetItem (rowValue, columnValue);
				Assert.Fail ("ArgumentOutOfRangeException not thrown.");
			} catch (ArgumentOutOfRangeException) { }
		}

		#endregion

		#region Navigation Test

		[Test]
		public void NavigationTest ()
		{
			StatusBar statusBar = (StatusBar) GetControlInstance ();
			IRawElementProviderFragmentRoot rootProvider;
			IRawElementProviderFragment childProvider;
			IRawElementProviderFragment childParent;

			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (statusBar);
			
			int index = 0, elements = 10;
			string name = string.Empty;
			for (; index < elements; ++index)
				statusBar.Panels.Add (string.Format ("Panel: {0}", index));
			index = 0;
			
			childProvider = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (childProvider, "We must have a child");
			
			do {
				childParent = childProvider.Navigate (NavigateDirection.Parent);
				Assert.AreSame (rootProvider, childParent, 
				                 "Each child must have same parent");
				name = (string) childProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
				Assert.AreEqual (string.Format ("Panel: {0}", index++), 
				                 name, "Different names");
				childProvider = childProvider.Navigate (NavigateDirection.NextSibling);
			} while (childProvider != null);
			Assert.AreEqual (elements, index, "Elements Added = Elements Navigated");

			statusBar.Panels.Clear ();

			childProvider = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (childProvider, "We shouldn't have a child");
		}

		#endregion

		#region BaseProviderTest Overrides

        	protected override Control GetControlInstance ()
        	{
            		return new StatusBar ();
        	}
		
		public override void LabeledByAndNamePropertyTest ()
		{
			TestLabeledByAndName (false, false);
		}

		#endregion
    	}
}
