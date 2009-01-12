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
// Copyright (c) 2008-2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using System.Threading;

// NOTE: In our implementation we are setting HelpProvider's 
// Provider ControlType as ControlType.ToolTip instead of 
// ControlType.Pane
namespace MonoTests.Mono.UIAutomation.Winforms.Client
{
    [TestFixture]
    [Description ("Tests SWF.DataGrid as ControlType.Pane")]
    public class HelpProviderTest
    {
        [TestFixtureSetUp]
        public void TestSetup ()
        {
			form = new Form ();
			button = new Button ();
			button.Text = "Show Help";
			button.Location = new Point (5, 5);
			button.Size = new Size (100, 30);
			button.Click += delegate (object sender, EventArgs args) {
				Rectangle rectangle = form.TopLevelControl.RectangleToScreen (form.Bounds);
				Help.ShowPopup (form,
					Message,
					new Point (rectangle.X + 10, rectangle.Y + 10));
			};
			button1 = new Button ();
			button1.Text = "Another button";
			button1.Location = new Point (5, 40);
			button1.Size = new Size (100, 30);

			form.Controls.Add (button);
			form.Controls.Add (button1);
			form.Size = new Size (200, 200);
        }

		[Test]
		[RequiresSTA]
		public void PatternsTest ()
		{
			form.Show ();
			AutomationElement element = AutomationElement.FromHandle (form.Handle);

			Automation.AddStructureChangedEventHandler (element,
				TreeScope.Element | TreeScope.Children,
				OnStructureChangedEventHandler);

			Monitor.Enter (lockObject);
			button.PerformClick ();
			Monitor.Wait (lockObject);
			button1.PerformClick ();

			// Testing Patterns helpProviderElement
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsDockPatternAvailableProperty),
				"DockPatternIdentifiers should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty),
				"ExpandCollapsePattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsGridPatternAvailableProperty),
				"GridPattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsGridItemPatternAvailableProperty),
				"GridItemPattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsInvokePatternAvailableProperty),
				"InvokePattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsMultipleViewPatternAvailableProperty),
				"MultipleViewPattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty),
				"RangeValue should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsScrollPatternAvailableProperty),
				"ScrollPattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty),
				"ScrollItemPattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsSelectionPatternAvailableProperty),
				"SelectionPattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty),
				"SelectionItemPattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsTablePatternAvailableProperty),
				"TablePattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsTableItemPatternAvailableProperty),
				"TableItemPattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsTogglePatternAvailableProperty),
				"TogglePattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsTransformPatternAvailableProperty),
				"TransformPattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsValuePatternAvailableProperty),
				"ValuePattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsWindowPatternAvailableProperty),
				"WindowPattern should not be supported");
			Assert.IsFalse ((bool) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.IsTextPatternAvailableProperty),
				"TextPattern should not be supported");

			form.Close ();
			Monitor.Exit (lockObject);		
		}

		private void OnStructureChangedEventHandler (object sender, StructureChangedEventArgs args)
		{
			Monitor.Enter (lockObject);
			if (sender != null) {
				AutomationElement element = sender as AutomationElement;
				ControlType ctype
					= (ControlType) element.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty);
				if (ctype == ControlType.Pane) { // NOTE: In our implementation we are using ToolTip instead of Pane
					helpProviderElement = element;
					// NameProperty must be equal to this.Message
					Assert.AreEqual (Message,
						(string) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.NameProperty),
						string.Format ("Message: {0} different to {1}",
							(string) helpProviderElement.GetCurrentPropertyValue (AutomationElementIdentifiers.NameProperty),
							Message));

					Monitor.Pulse (lockObject);
					// We are done listening for the HelpProvider ToolTip
					AutomationElement formElement = AutomationElement.FromHandle (form.Handle);
					Automation.RemoveStructureChangedEventHandler (formElement, 
						OnStructureChangedEventHandler);
				}
			}
			Monitor.Exit (lockObject);
		}
		
		[TestFixtureTearDown]
		public void TestTeardDown ()
		{
			form.Close ();
		}

		private AutomationElement helpProviderElement;
		private const string Message = "HelpProvider showing ToolTip";
		private object lockObject = new object ();
		private Button button;
		private Button button1;
		private Form form;
	}
}
