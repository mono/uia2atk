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
using System.Collections;
using System.Windows;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	
	[TestFixture]
	public class DataGridProviderTest : BaseProviderTest
	{
		#region Tests
		
		[Test]
		public void BasicPropertiesTest ()
		{
			SWF.DataGrid datagrid = GetControlInstance () as SWF.DataGrid;

			IRawElementProviderFragment datagridProvider 
				= (IRawElementProviderFragment) GetProviderFromControl (datagrid);
			
			TestProperty (datagridProvider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.DataGrid.Id);
			
			TestProperty (datagridProvider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "data grid");

			TestProperty (datagridProvider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              true);

			TestChildPatterns (datagridProvider);
		}
		
		#endregion

		#region Patterns Tests

		[Test]
		public void PatternsTest ()
		{
			SWF.DataGrid datagrid = GetControlInstance () as SWF.DataGrid;
			IRawElementProviderFragment datagridProvider 
				= (IRawElementProviderFragment) GetProviderFromControl (datagrid);
			datagrid.Visible = true;

			Assert.IsTrue ((bool) datagridProvider.GetPropertyValue (AutomationElementIdentifiers.IsSelectionPatternAvailableProperty.Id),
			               "Selection Pattern should be available");
			Assert.IsTrue ((bool) datagridProvider.GetPropertyValue (AutomationElementIdentifiers.IsGridPatternAvailableProperty.Id),
			               "Grid Pattern should be available");

			datagrid.ColumnHeadersVisible = true;
			object pattern = datagridProvider.GetPatternProvider (TablePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (pattern, 
			                  "Table Pattern should be available");
			Assert.IsTrue ((bool) datagridProvider.GetPropertyValue (AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id),
			               "Table Pattern should be available");
			IRawElementProviderFragment child = datagridProvider.Navigate (NavigateDirection.FirstChild);
			IRawElementProviderFragment headerChild = null;
			IRawElementProviderFragment dataItemChild = null;
			while (child != null) {
				if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.Header.Id)
					headerChild = child;
				else if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.DataItem.Id)
					dataItemChild = child;
				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (headerChild, "We should have a Header");
			Assert.IsNotNull (dataItemChild, "We should have a DataItem");
			// Testing patterns in DataItem
			Assert.IsNotNull (dataItemChild.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id),
			                  "SelectionItem not implement in DataItem");
			Assert.IsTrue ((bool) dataItemChild.GetPropertyValue (AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty.Id),
			               "SelectionItem not implement in DataItem");
			Assert.IsNotNull (dataItemChild.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id),
			                  "GridItem not implement in DataItem");
			Assert.IsTrue ((bool) dataItemChild.GetPropertyValue (AutomationElementIdentifiers.IsGridItemPatternAvailableProperty.Id),
			               "GridItem not implement in DataItem");
			Assert.IsNotNull (dataItemChild.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id),
			                  "ScrollItem not implement in DataItem");
			Assert.IsTrue ((bool) dataItemChild.GetPropertyValue (AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty.Id),
			               "ScrollItem not implement in DataItem");
			Assert.IsNotNull (dataItemChild.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			                  "Value not implement in DataItem");
			Assert.IsTrue ((bool) dataItemChild.GetPropertyValue (AutomationElementIdentifiers.IsValuePatternAvailableProperty.Id),
			               "Value not implement in DataItem");
			// FIXME: Test TableItem

			// Testing children in DataItem
			IRawElementProviderFragment childDataItem = dataItemChild.Navigate (NavigateDirection.FirstChild);
			while (childDataItem != null) {
				Assert.IsNotNull (childDataItem.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
				                  "Value not implement in ChildDataItem");
				Assert.IsTrue ((bool) childDataItem.GetPropertyValue (AutomationElementIdentifiers.IsValuePatternAvailableProperty.Id),
				               "Value not implement in ChildDataItem");
				Assert.IsNotNull (childDataItem.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id),
				                  "GridItem not implement in ChildDataItem");
				Assert.IsTrue ((bool) childDataItem.GetPropertyValue (AutomationElementIdentifiers.IsGridItemPatternAvailableProperty.Id),
				               "GridItem not implement in ChildDataItem");
				Assert.IsNotNull (childDataItem.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id),
				                  "TableItem not be implement in ChildDataItem");
				Assert.IsTrue ((bool) childDataItem.GetPropertyValue (AutomationElementIdentifiers.IsTableItemPatternAvailableProperty.Id),
				               "TableItem not be implement in ChildDataItem");

				childDataItem = childDataItem.Navigate (NavigateDirection.NextSibling);
			}

			datagrid.ColumnHeadersVisible = false;
			pattern = datagridProvider.GetPatternProvider (TablePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (pattern, 
			                  "Table Pattern should be available. ColumnHeadersVisible doesn't affect Pattern");
			Assert.IsTrue ((bool) datagridProvider.GetPropertyValue (AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id),
			               "Table Pattern should be available. ColumnHeadersVisible doesn't affect Pattern");
			child = datagridProvider.Navigate (NavigateDirection.FirstChild);
			headerChild = null;
			dataItemChild = null;
			while (child != null) {
				if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.Header.Id)
					headerChild = child;
				else if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.DataItem.Id)
					dataItemChild = child;
				TestPatterns (child);
				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (headerChild, "We should have a header. ColumnHeadersVisible doesn't affect Pattern nor Child");

			Assert.IsNotNull (dataItemChild.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id),
			                  "SelectionItem not implement in DataItem");
			Assert.IsTrue ((bool) dataItemChild.GetPropertyValue (AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty.Id),
			               "SelectionItem not implement in DataItem");
			Assert.IsNotNull (dataItemChild.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id),
			                  "GridItem not implement in DataItem");
			Assert.IsTrue ((bool) dataItemChild.GetPropertyValue (AutomationElementIdentifiers.IsGridItemPatternAvailableProperty.Id),
			               "GridItem not implement in DataItem");
			Assert.IsNotNull (dataItemChild.GetPatternProvider (ScrollItemPatternIdentifiers.Pattern.Id),
			                  "ScrollItem not implement in DataItem");
			Assert.IsTrue ((bool) dataItemChild.GetPropertyValue (AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty.Id),
			               "ScrollItem not implement in DataItem");
			Assert.IsNotNull (dataItemChild.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
			                  "Value not implement in DataItem");
			Assert.IsTrue ((bool) dataItemChild.GetPropertyValue (AutomationElementIdentifiers.IsValuePatternAvailableProperty.Id),
			               "Value not implement in DataItem");
			// LAMESPEC: Vista doesn't implement this, we do, because when container implements Grid children must implement GridItem
			Assert.IsNotNull (dataItemChild.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id),
			                  "TableItem should be implement in DataItem");
			Assert.IsTrue ((bool) dataItemChild.GetPropertyValue (AutomationElementIdentifiers.IsTableItemPatternAvailableProperty.Id),
			               "TableItem should be implement in DataItem");

			// Testing children in DataItem
			childDataItem = dataItemChild.Navigate (NavigateDirection.FirstChild);
			while (childDataItem != null) {
				Assert.IsNotNull (childDataItem.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id),
				                  "Value not implement in ChildDataItem");
				Assert.IsTrue ((bool) childDataItem.GetPropertyValue (AutomationElementIdentifiers.IsValuePatternAvailableProperty.Id),
				               "Value not implement in ChildDataItem");
				Assert.IsNotNull (childDataItem.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id),
				                  "GridItem not implement in ChildDataItem");
				Assert.IsTrue ((bool) childDataItem.GetPropertyValue (AutomationElementIdentifiers.IsGridItemPatternAvailableProperty.Id),
				               "GridItem not implement in ChildDataItem");
				Assert.IsNotNull (childDataItem.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id),
				                  "TableItem should not be implement in ChildDataItem");
				// LAMESPEC: Vista doesn't implement this, we do, because when container implements Table children must implement TableItem
				Assert.IsTrue ((bool) childDataItem.GetPropertyValue (AutomationElementIdentifiers.IsTableItemPatternAvailableProperty.Id),
				               "TableItem should implement in ChildDataItem");

				// LAMESPEC: We are skipping Edit because in Vista they are implementing IValue, and we are doing the same,
				// however the spec says that we should not do that.
				if ((int) childDataItem.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    != ControlType.Edit.Id)
					TestPatterns (childDataItem);

				childDataItem = childDataItem.Navigate (NavigateDirection.NextSibling);
			}
		}

		#endregion

		#region Navigation Tests

		[Test]
		public void NavigationTest ()
		{
			SWF.DataGrid datagrid = GetControlInstance () as SWF.DataGrid;

			IRawElementProviderFragment datagridProvider 
				= (IRawElementProviderFragment) GetProviderFromControl (datagrid);
			// We should have 1 header and "this.Elements" items

			Assert.IsNotNull (datagridProvider, "DatagridProvider should not be null");
			
			IRawElementProviderFragment child = datagridProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (child, "First child should not be null");
			IRawElementProviderFragment header = null;
			int elements = 0;
			while (child != null) {
				Assert.AreEqual (datagridProvider, 
				                 child.Navigate (NavigateDirection.Parent),
				                 "Child parent != DataGridProvider");
				if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.DataItem.Id)
				    elements++;
				else if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				         == ControlType.Header.Id)
					header = child;
				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.AreEqual (Elements, 
			                 elements,
			                 string.Format ("DataItems found: {0} different to {1}", elements, Elements));
			Assert.IsNotNull (header, "Header should not be null");

			// Header should have 2 children, because we are using 2 public properties
			elements = 0;
			child = header.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				Assert.AreEqual (header, 
				                 child.Navigate (NavigateDirection.Parent),
				                 "Child parent != HeaderProvider");
				elements++;
				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.AreEqual (2, elements, 
			                 string.Format ("Children found: {0} different to 2", elements));
		}

		#endregion

		#region BaseProviderTest Overrides

		private const int Elements = 10;

		protected override SWF.Control GetControlInstance ()
		{
			SWF.DataGrid datagrid = new SWF.DataGrid ();
			datagrid.Size = new SD.Size (300, 300);
			
            ArrayList arraylist = new ArrayList ();

            for (int index = 0; index < Elements; index++)
                arraylist.Add (new BindableReadWriteElement (index,
                    string.Format ("Name{0}", index)));

            datagrid.DataSource = arraylist;
			return datagrid;
		}
		
		#endregion
	}


    public class BindableReadonlyElement
    {
		public BindableReadonlyElement (int integer, string name)
        {
            this.integer = integer;
            this.name = name;
        }

        public int Integer {
            get { return integer; }
        }

        public string Name {
            get { return name;  }
        }

        private int integer;
        private string name;
    }

    public class BindableReadWriteElement
    {
        public BindableReadWriteElement (int integer, string name)
        {
            Integer = integer;
            Name = name;
        }

        public int Integer { get; set; }
        public string Name { get; set; }
    }
}
