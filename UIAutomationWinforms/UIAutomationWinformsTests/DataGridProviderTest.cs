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
		}
		
		#endregion

		[Test]
		public void PatternsTest ()
		{
			SWF.DataGrid datagrid = GetControlInstance () as SWF.DataGrid;

			IRawElementProviderFragment datagridProvider 
				= (IRawElementProviderFragment) GetProviderFromControl (datagrid);

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
			datagrid.ColumnHeadersVisible = false;
			pattern = datagridProvider.GetPatternProvider (TablePatternIdentifiers.Pattern.Id);
			Assert.IsNull (pattern, 
			               "Table Pattern should not be available");
			Assert.IsFalse ((bool) datagridProvider.GetPropertyValue (AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id),
			                "Table Pattern should not be available");
		}

		// FIXME: Navigation
		// FIXME: Column changes

		#region BaseProviderTest Overrides

		protected override SWF.Control GetControlInstance ()
		{
			SWF.DataGrid datagrid = new SWF.DataGrid ();
			datagrid.Size = new SD.Size (300, 300);
			
            ArrayList arraylist = new ArrayList ();

            for (int index = 0; index < 10; index++)
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
