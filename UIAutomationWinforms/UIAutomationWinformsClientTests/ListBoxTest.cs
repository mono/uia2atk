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
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Forms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms.Client
{
	//According to http://msdn.microsoft.com/en-us/library/ms742462.aspx
	[TestFixture]
	[Description ("Tests SWF.ListBox as ControlType.List")]
	public class ListBoxTest : BaseTest
	{
		#region Properties

		[Test]
		[Description ("Value: List | Notes: This value is the same for all UI frameworks.")]
		public override void MsdnControlTypePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (ControlType.List,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty, true),
				"ControlType");
		}
	
		[Test]
		public override void MsdnNamePropertyTest ()
		{
			ListBox listbox = GetControl () as ListBox;
			AutomationElement element = GetAutomationElementFromControl (listbox);
			
			listbox.Text = "listbox text";
			Assert.AreEqual ("listbox text",
				element.GetCurrentPropertyValue (AutomationElementIdentifiers.NameProperty, true),
				"NameProperty");

			listbox.Text = "other listbox text";
			Assert.AreEqual ("other listbox text",
				element.GetCurrentPropertyValue (AutomationElementIdentifiers.NameProperty, true),
				"NameProperty");
		}

		[Test]
		public override void MsdnLabeledByPropertyTest ()
		{
			ListBox listbox = GetControl () as ListBox;
			AutomationElement element = GetAutomationElementFromControl (listbox);
			
			listbox.Text = "listbox text";
			Assert.AreEqual (null,
				element.GetCurrentPropertyValue (AutomationElementIdentifiers.LabeledByProperty, true),
				"LabeledByProperty");
		}

		[Test]
		[Description (@"Value: ""list"" | Notes: Localized string corresponding to the List control type. ")]
		public override void MsdnLocalizedControlTypePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual ("list",
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty, true),
				"LocalizedControlTypeProperty");
		}

		[Test]
		[Description ("Value: True | Notes: The list control is always included in the content view of the UI Automation tree. .")]
		public override void MsdnIsContentElementPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (true,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.IsContentElementProperty, true),
				"IsControlElementProperty");
		}

		[Test]
		[Description ("Value: True | Notes: The list control is always included in the control view of the UI Automation tree.")]
		public override void MsdnIsControlElementPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (true,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.IsControlElementProperty, true),
				"IsControlElementProperty");
		}

		[Test]
		[NotListedAttribute]
		[Description ("Is not listed. We are using null as default value.")]
		public override void MsdnOrientationPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (null,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.OrientationProperty, true),
				"OrientationProperty");
		}

		[Test]
		[Description ("Value: See Notes | Notes: The Help text for list controls should explain why the user is being "
			+ @"asked to make a choice from a list of options. For example, ""Selection an item from this list will set the "
			+ @"display resolution for your monitor.""")]
		public override void MsdnHelpTextPropertyTest ()
		{
			ListBox listbox = GetControl () as ListBox;

			ToolTip tooltip = new ToolTip ();
			tooltip.SetToolTip (listbox, "I'm HelpTextProperty in listbox");

			AutomationElement child = GetAutomationElementFromControl (listbox);
			Assert.AreEqual (tooltip.GetToolTip (listbox),
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.HelpTextProperty, true),
				"HelpTextProperty");
		}

		#endregion

		#region Patterns

		[Test]
		[Description ("Support/Value: Required . | Notes: All controls that support the List control type "
			+ "must implement ISelectionProvider when a selection state is maintained between the items contained "
			+ "in the control. If the items within the container are not selectable, the Group control type must be used.")]
		public override void MsdnSelectionPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsTrue (SupportsPattern (element, SelectionPatternIdentifiers.Pattern),
				string.Format ("SelectionPattern SHOULD BE supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		#endregion

		#region Protected Methods

		protected override Control GetControl ()
		{
			ListBox listbox = new ListBox ();
			listbox.Items.AddRange (new object [] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
			listbox.Size = new Size (100, 100);
			listbox.Location = new Point (3, 3);
			return listbox;
		}

		#endregion
	}
}
