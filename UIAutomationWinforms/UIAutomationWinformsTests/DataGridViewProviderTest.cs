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
//	Mario Carrion <mcarrion@novell.com>
//
using System;
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	
	[TestFixture]
	public class DataGridViewProviderTest : BaseProviderTest
	{
		
		#region Column tests

		[Test]
		public void ImageTest ()
		{
			SWF.DataGridViewImageColumn column = new SWF.DataGridViewImageColumn ();
			column.HeaderText = "Image Column";
			
			SWF.DataGridViewImageCell cell = new SWF.DataGridViewImageCell ();
			
			ColumnCellTest (column, cell, false);
		}

		[Test]
		public void ButtonTest ()
		{
			SWF.DataGridViewButtonColumn column = new SWF.DataGridViewButtonColumn ();
			column.HeaderText = "Button Column";
			
			SWF.DataGridViewButtonCell cell = new SWF.DataGridViewButtonCell ();
			cell.Value = "Button cell";
			
			ColumnCellTest (column, cell, false);
		}

		[Test]
		public void LinkTest ()
		{
			SWF.DataGridViewLinkColumn column = new SWF.DataGridViewLinkColumn ();
			column.HeaderText = "Link Column";
			
			SWF.DataGridViewLinkCell cell = new SWF.DataGridViewLinkCell ();
			cell.Value = "Link cell";
			
			ColumnCellTest (column, cell, false);
		}

		[Test]
		public void CheckBoxTest ()
		{
			SWF.DataGridViewCheckBoxColumn column = new SWF.DataGridViewCheckBoxColumn ();
			column.HeaderText = "CheckBox Column";
			
			SWF.DataGridViewCheckBoxCell cell = new SWF.DataGridViewCheckBoxCell ();
			cell.Value = false;
			
			ColumnCellTest (column, cell, false);
		}

		[Test]
		public void ComboBoxTest ()
		{
			SWF.DataGridViewComboBoxColumn column = new SWF.DataGridViewComboBoxColumn ();
			column.HeaderText = "CheckBox Column";
			column.Items.AddRange ("1", "2", "3", "4", "5");
			
			SWF.DataGridViewComboBoxCell cell = new SWF.DataGridViewComboBoxCell ();
			
			ColumnCellTest (column, cell, true);
		}

		[Test]
		public void TextBoxTest ()
		{
			SWF.DataGridViewTextBoxColumn column = new SWF.DataGridViewTextBoxColumn ();
			column.HeaderText = "Edit Column";
			
			SWF.DataGridViewTextBoxCell cell = new SWF.DataGridViewTextBoxCell ();
			
			ColumnCellTest (column, cell, false);
		}

		public void ColumnCellTest (SWF.DataGridViewColumn column, 
		                            SWF.DataGridViewCell cell, 
		                            bool cellChildrenExpected)
		{
			SWF.DataGridView datagridview = GetControlInstance () as SWF.DataGridView;
			datagridview.Size = new Size (200, 200);
			datagridview.AllowUserToAddRows = false;
			
			datagridview.Columns.Add (column);

			IRawElementProviderFragmentRoot provider 
				= GetProviderFromControl (datagridview) as IRawElementProviderFragmentRoot;
			Assert.IsNotNull (provider, "Missing DataGridView provider");

			IRawElementProviderFragmentRoot header = null;
			IRawElementProviderFragment child = provider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (child, "FirstChild is null");
			int childCount = 0;
			while (child != null) {
				childCount++;
				if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.Header.Id)
					header = child as IRawElementProviderFragmentRoot;
				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (header, "Header is missing");
			Assert.AreEqual (1, childCount, "Children (Header)");

			SWF.DataGridViewRow row = new SWF.DataGridViewRow ();
			row.Cells.Add (cell);
			datagridview.Rows.Add (row);

			IRawElementProviderFragment dataItem = null;
			childCount = 0;
			child = provider.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				childCount++;
				if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    == ControlType.DataItem.Id)
					dataItem = child;
				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Assert.IsNotNull (dataItem, "DataItem is missing");
			Assert.AreEqual (2, childCount, "Children (Header/DataItem)");

			IRawElementProviderFragment buttonChild = dataItem.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (buttonChild, "DataItem.Child missing");
			TestPatterns (buttonChild);

			Assert.AreEqual (dataItem,
			                 buttonChild.Navigate (NavigateDirection.Parent),
			                 "DataItem != ButtonChild.Parent");

			childCount = 0;
			child = buttonChild.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				childCount++;
				child = child.Navigate (NavigateDirection.NextSibling);
			}
			if (!cellChildrenExpected)
				Assert.AreEqual (0, childCount, "No children expected.");
			else
				Assert.Greater (childCount, 0, "Children expected.");

			TestChildPatterns (provider);
		}
		
		#endregion

		#region Pattern Tests : Overridden

		protected override void TestEditPatterns (IRawElementProviderSimple provider) 
		{
			// LAMESPEC: We should implement ITextProvider instead of IValueProvider
			
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsValuePatternAvailableProperty.Id),
			               "Edit ControlType must support ITextProvider");
			Assert.IsFalse ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty.Id),
			                "Edit ControlType MUST NOT support IRangeValueProvider");
			Assert.IsFalse ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsTextPatternAvailableProperty.Id),
			                "Edit ControlType MUST NOT support ITextProvider");

			TestValuePattern_ValuePropertyEvent (provider);
			TestValuePattern_IsReadOnlyPropertyEvent (provider);
		}

		protected override void TestValuePattern_ChangeReadOnly (IRawElementProviderSimple provider, 
		                                                         bool newValue)
		{
			if (provider.GetType () == typeof (DataGridViewProvider.DataGridViewDataItemEditProvider)) {
				DataGridViewProvider.DataGridViewDataItemEditProvider editProvider 
					= (DataGridViewProvider.DataGridViewDataItemEditProvider) provider;
				editProvider.TextBoxCell.ReadOnly = newValue;
			} else
				Assert.Fail ("I have no idea about {0}!", provider.GetType ());
		}

		protected override void TestExpandCollapsePattern_ExpandCollapseStatePropertyEvent (IRawElementProviderSimple provider)
		{
			// Remove this method when fixed: https://bugzilla.novell.com/show_bug.cgi?id=471754
		}
		
		#endregion
		
		#region BaseProviderTest Overrides

		protected override SWF.Control GetControlInstance ()
		{			
			return new SWF.DataGridView ();
		}
		
		#endregion

	}
}
