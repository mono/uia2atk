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
using System.Windows.Automation;
using System.Windows.Forms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms.Client
{
	//According to http://msdn.microsoft.com/en-us/library/ms752305.aspx
	[TestFixture]
	[Description ("Tests SWF.ComboBox as ControlType.ComboBox")]
	public class ComboBoxTest : BaseTest
	{
		#region Properties Test 

		[Test]
		[Description ("Value: ComboBox | Notes: This value is the same for all UI frameworks.")]
		public override void MsdnControlTypePropertyTest () 
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (ControlType.ComboBox,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty, true),
				"ControlType");
		}
		
		[Test]
		[Description ("Value: See notes. | Notes: The combo box control typically gets its name from "
			+"a static text control.")] 
		public override void MsdnNamePropertyTest ()
		{
			ComboBox combobox = GetControl () as ComboBox;
			AutomationElement child = GetAutomationElementFromControl (combobox);

			Assert.AreEqual (combobox.Text,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.NameProperty, true),
				"NameProperty");

			combobox.Text = "3";
			Assert.AreEqual (combobox.Text,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.NameProperty, true),
				"NameProperty");
		}

		[Test]
		[Ignore ("No Idea how to test")]
		[Description ("Value: See notes. | Notes: Combo box controls typically have a static text "
			+"label that this property references.  ")]
		public override void MsdnLabeledByPropertyTest ()
		{
		}

		[Test]
		[Description (@"Value: ""button"" | Notes: Localized string corresponding to the Button control type.")]
		public override void MsdnLocalizedControlTypePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual ("combo box",
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty, true),
				"LocalizedControlType");
		}

		[Test]
		[Description ("Value: True | Notes: The Button control must always be content.")]
		public override void MsdnIsContentElementPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (true,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.IsContentElementProperty, true),
				"IsContentElementProperty");
		}

		[Test]
		[LameSpec]
		[Description ("Value: True | Notes: The Button control must always be a control.")]
		public override void MsdnIsControlElementPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (true,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.IsControlElementProperty, true),
				"IsControlElementProperty");
		}

		[Test]
		[NotListed]
		[Description ("Is not listed. We are using null as default value.")]
		public override void MsdnOrientationPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (null,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.OrientationProperty, true),
				"OrientationProperty");
		}

		[Test]
		[Ignore ("No idea how to test.")]
		[Description ("Value: See Notes | Notes: The help text for combo box controls should explain "
			+"why the user is being asked to make a choice from a combo box of options. For example, "
			+@"""Selecting an item from this combo box will cause the display resolution for your monitor to be set.""")]
		public override void MsdnHelpTextPropertyTest ()
		{
		}

		#endregion

		#region Pattern Tests

		[Test]
		[Description ("Support/Value: Yes. | Notes: The combo box control must always contain "
			+"the drop-down button in order to be a combo box.")]
		public override void MsdnExpandCollapsePatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsTrue (SupportsPattern (element, ExpandCollapsePatternIdentifiers.Pattern),
				string.Format ("ExpandCollapsePattern SHOULD BE supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		[Description ("Support/Value: Yes. | Notes: Displays the current selection in the combo "
			+"box. This support is delegated to the list box beneath the combo box.")]
		public override void MsdnSelectionPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsTrue (SupportsPattern (element, SelectionPatternIdentifiers.Pattern),
				string.Format ("SelectionPattern SHOULD BE supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		[Description ("Support/Value: Depends. | Notes: If the combo box has the ability to take arbitrary "
			+"text values, the Value pattern must be supported. This pattern provides the ability to programmatically "
			+"set the string contents of the combo box. If the Value pattern is not supported, this indicates that the "
			+"user must make a selection from the list items within the subtree of the combo box.")]
		public override void MsdnValuePatternTest ()
		{
			ComboBox combobox = GetControl () as ComboBox;
			AutomationElement element;

			combobox.DropDownStyle = ComboBoxStyle.DropDown;
			element = this.GetAutomationElementFromControl (combobox);
			Assert.IsTrue (SupportsPattern (element, ValuePatternIdentifiers.Pattern),
				string.Format ("ValuePattern SHOULD BE supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));

			combobox.DropDownStyle = ComboBoxStyle.DropDown;
			element = this.GetAutomationElementFromControl (combobox);
			Assert.IsTrue (SupportsPattern (element, ValuePatternIdentifiers.Pattern),
				string.Format ("ValuePattern SHOULD BE supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));

			combobox.DropDownStyle = ComboBoxStyle.DropDownList;
			element = this.GetAutomationElementFromControl (combobox);
			Assert.IsFalse (SupportsPattern (element, ValuePatternIdentifiers.Pattern),
				string.Format ("ValuePattern SHOULD NOT supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}


		[Test]
		[Description ("Support/Value: Never. | Notes: The Scroll pattern is never supported on a combo"
			+"box directly. It is supported if a list box contained within a combo box can scroll. It may "
			+"only be supported when the list box is visible on the screen.")]
		public override void MsdnScrollPatternTest ()
		{
			ComboBox combobox = GetControl () as ComboBox;
			AutomationElement element;

			combobox.DropDownStyle = ComboBoxStyle.DropDown;
			element = this.GetAutomationElementFromControl (combobox);
			Assert.IsFalse (SupportsPattern (element, ScrollPatternIdentifiers.Pattern),
				string.Format ("ScrollPattern SHOULD BE supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));

			combobox.DropDownStyle = ComboBoxStyle.DropDown;
			element = this.GetAutomationElementFromControl (combobox);
			Assert.IsFalse (SupportsPattern (element, ScrollPatternIdentifiers.Pattern),
				string.Format ("ScrollPattern SHOULD BE supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));

			combobox.DropDownStyle = ComboBoxStyle.DropDownList;
			element = this.GetAutomationElementFromControl (combobox);
			Assert.IsFalse (SupportsPattern (element, ScrollPatternIdentifiers.Pattern),
				string.Format ("ScrollPattern SHOULD NOT supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		#endregion

		#region Protected metods

		protected override Control GetControl ()
		{
			ComboBox combobox = new ComboBox ();
			combobox.DropDownStyle = ComboBoxStyle.DropDownList;
			combobox.Items.AddRange (new object [] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
			return combobox;
		}

		#endregion
	}
}
