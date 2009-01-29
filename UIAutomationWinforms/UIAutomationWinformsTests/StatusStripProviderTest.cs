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
    	public class StatusStripProviderTest : BaseProviderTest
    	{
		#region Test

        	[Test]
        	public void BasicPropertiesTest ()
        	{
            		StatusStrip statusStrip = new StatusStrip ();
            		IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (statusStrip);

			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.StatusBar.Id);

			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "status bar");
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LabeledByProperty,
			              null);
		}

		[Test]
		public void ProviderPatternTest ()
		{
			StatusStrip statusStrip = new StatusStrip ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (statusStrip);

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
			StatusStrip statusStrip = new StatusStrip ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (statusStrip);

			IGridProvider gridProvider = (IGridProvider)
				provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (gridProvider,
			                  "Not returning GridPatternIdentifiers.");
			
			int index = 0, elements = 10;
			for (; index < elements; ++index)
				statusStrip.Items.Add (string.Format ("Item: {0}", index));
			int value = 1;
			Assert.AreEqual (value, gridProvider.RowCount, "RowCount value");
		}

		[Test]
		public void IGridProviderColumnCountTest ()
		{
			StatusStrip statusStrip = new StatusStrip ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (statusStrip);

			IGridProvider gridProvider = (IGridProvider)
				provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (gridProvider,
			                  "Not returning GridPatternIdentifiers.");
			
			int index = 0, elements = 10;
			for (; index < elements; ++index)
				statusStrip.Items.Add (string.Format ("Item: {0}", index));
			int value = elements;
			Assert.AreEqual (value, gridProvider.ColumnCount, "ColumnCount value");
		}

		[Test]
		public void IGridProviderGetItemTest ()
		{
			StatusStrip statusStrip = new StatusStrip ();
			IRawElementProviderFragmentRoot provider =
				(IRawElementProviderFragmentRoot) GetProviderFromControl (statusStrip);

			IGridProvider gridProvider = (IGridProvider)
				provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (gridProvider,
			                  "Not returning GridPatternIdentifiers.");
			
			int rowValue = 0;
			int columnValue = 0;
			statusStrip.Items.Add ("Item");
			IRawElementProviderSimple panelProvider = (IRawElementProviderSimple)
				provider.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderSimple itemProvider =
				gridProvider.GetItem (rowValue, columnValue);
			Assert.AreSame (panelProvider, itemProvider, "GetItem method");
			
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
		
		[Test]
		public void IGridProviderEventTest ()
		{
			StatusStrip statusStrip = new StatusStrip ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (statusStrip);

			IGridProvider gridProvider = (IGridProvider)
				provider.GetPatternProvider (GridPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (gridProvider,
			                  "Not returning GridPatternIdentifiers.");
			
			bool eventInvoked = false;
			statusStrip.ItemAdded += delegate (object sender, ToolStripItemEventArgs e) {
				eventInvoked = true;
			};
			statusStrip.Items.Add ("Item");
			Assert.IsTrue (eventInvoked,
			               "Event should be invoked when items added.");
			
			eventInvoked = false;
			statusStrip.ItemRemoved += delegate (object sender, ToolStripItemEventArgs e) {
				eventInvoked = true;
			};
			statusStrip.Items.RemoveAt (0);
			Assert.IsTrue (eventInvoked,
			               "Event should be invoked when items removed.");
		}

		#endregion
		
		#region Navigation Test

		[Test]
		public void NavigationTest ()
		{
			StatusStrip statusStrip = (StatusStrip) GetControlInstance ();
			IRawElementProviderFragmentRoot rootProvider;
			IRawElementProviderFragment childProvider;
			IRawElementProviderFragment childParent;

			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (statusStrip);
			
			int index = 0, elements = 10;
			string name = string.Empty;
			for (; index < elements; ++index)
				statusStrip.Items.Add (string.Format ("Item: {0}", index));
			index = 0;
			
			childProvider = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (childProvider, "We must have a child");
			
			do {
				childParent = childProvider.Navigate (NavigateDirection.Parent);
				Assert.AreEqual (rootProvider, childParent,
				                 "Each child must have same parent");
				name = (string) childProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
				Assert.AreEqual (string.Format ("Item: {0}", index++), 
				                 name, "Different names");
				childProvider = childProvider.Navigate (NavigateDirection.NextSibling);
			} while (childProvider != null);
			Assert.AreEqual (elements, index, "Elements Added = Elements Navigated");

			// TODO: Uncomment this line and remove the for loop
			//statusStrip.Items.Clear ();
			for (; statusStrip.Items.Count > 0; )
				statusStrip.Items.RemoveAt (0);

			// FIXME: We need to patch SWF.
			childProvider = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNull (childProvider, "We shouldn't have a child");
		}

		#endregion
		
		#region BaseProviderTest Overrides

		protected override Control GetControlInstance ()
		{
			return new StatusStrip ();
		}
		
		public override void LabeledByAndNamePropertyTest ()
		{
			TestLabeledByAndName (false, false, true, true, false);
		}

		public override void IsKeyboardFocusablePropertyTest ()
		{
			Control control = GetControlInstance ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (control);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              false);
		}

		#endregion
    	}
}
