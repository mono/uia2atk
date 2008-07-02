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
            		IRawElementProviderSimple provider = ProviderFactory.GetProvider (statusBar);

			TestProperty (provider,
				AutomationElementIdentifiers.ControlTypeProperty,
				ControlType.StatusBar.Id);

			TestProperty (provider,
				AutomationElementIdentifiers.LocalizedControlTypeProperty,
				"status bar");
		}

		[Test]
		public void ProviderPatternTest ()
		{
			StatusBar statusBar = new StatusBar ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (statusBar);

			object gridProvider = provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (gridProvider, "Not returning GridPatternIdentifiers.");
			Assert.IsTrue (gridProvider is IGridProvider, "Not returning GridPatternIdentifiers.");
		}

#endregion

#region IGridPattern Test

		public void IGridProviderRowCountTest ()
		{
			StatusBar statusBar = new StatusBar ();
			StatusBarPanel panel = new StatusBarPanel ();
			statusBar.Panels.Add (panel);

			IRawElementProviderSimple provider = ProviderFactory.GetProvider (statusBar);

			IGridProvider gridProvider = (IGridProvider)
				provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (gridProvider, "Not returning GridPatternIdentifiers.");

			int value = 1;
			Assert.AreEqual (gridProvider.RowCount, value, "RowCount value");
		}

		public void IGridProviderColumnCountTest ()
		{
			StatusBar statusBar = new StatusBar ();
			StatusBarPanel panel = new StatusBarPanel ();
			statusBar.Panels.Add (panel);

			IRawElementProviderSimple provider = ProviderFactory.GetProvider (statusBar);

			IGridProvider gridProvider = (IGridProvider)
				provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (gridProvider, "Not returning GridPatternIdentifiers.");

			int value = 1;
			Assert.AreEqual (gridProvider.ColumnCount, value, "ColumnCount value");
		}

		public void IGridProviderGetItemTest ()
		{
			StatusBar statusBar = new StatusBar ();
			StatusBarPanel panel = new StatusBarPanel ();
			statusBar.Panels.Add (panel);

			IRawElementProviderSimple provider = ProviderFactory.GetProvider (statusBar);

			IGridProvider gridProvider = (IGridProvider)
				provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (gridProvider, "Not returning GridPatternIdentifiers.");

			int rowValue = 1;
			int columnValue = 1;
			gridProvider.GetItem (rowValue, columnValue);
//			Assert.AreEqual (gridProvider., null, "Different value");

			try {
				rowValue = gridProvider.RowCount + 1;
				columnValue = gridProvider.ColumnCount + 1;
				gridProvider.GetItem (rowValue, columnValue);
				Assert.Fail ("ArgumentOutOfRangeException not thrown.");
			} catch (ArgumentOutOfRangeException) { }

			try {
				rowValue = -1;
				columnValue = -1;
				gridProvider.GetItem (rowValue, columnValue);
				Assert.Fail ("ArgumentOutOfRangeException not thrown.");
			} catch (ArgumentOutOfRangeException) { }
		}

#endregion

#region BaseProviderTest Overrides

        	protected override Control GetControlInstance ()
        	{
            		return new StatusBar ();
        	}

#endregion
    	}
}
