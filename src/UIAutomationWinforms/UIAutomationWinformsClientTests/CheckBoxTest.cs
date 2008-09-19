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
	//According to http://msdn.microsoft.com/en-us/library/ms751693.aspx
	[TestFixture]
	[Description ("Tests SWF.CheckBox as ControlType.Button")]
	public class CheckBoxTest : BaseTest
	{
		#region Properties Test

		[Test]
		[Description ("Vaule: See notes. | Notes: The value of the check box control's Name property is the text that "
			+"is displayed beside the box that maintains the toggle state. ")]
		public override void MsdnNamePropertyTest ()
		{
			CheckBox checkbox = new CheckBox ();
			AutomationElement child = GetAutomationElementFromControl (checkbox);

			Assert.AreEqual (checkbox.Text,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.NameProperty, true),
				"NameProperty");

			checkbox.Text = "Unhappy CheckBox";
			Assert.AreEqual (checkbox.Text,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.NameProperty, true),
				"NameProperty");
		}

		[Test]
		[LameSpec]
		[Description ("Value: Null | Notes: Check boxes are self-labeling controls.")]
		public override void MsdnLabeledByPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (null,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.LabeledByProperty, true),
				"LabeledByProperty");
		}

		[Test]
		[Description (@"Value ""check box"" | Notes: Localized string corresponding to the CheckBox control type.")]
		public override void MsdnLocalizedControlTypePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual ("check box",
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty, true),
				"LocalizedControlType");
		}

		[Test]
		[Description ("Value: True | Notes: The value of this property must always be True. "
			+"This means that the check box control must always be included in the content view of "
			+"the UI Automation tree.")]
		public override void MsdnIsContentElementPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (true,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.IsContentElementProperty, true),
				"IsContentElementProperty");
		}

		[Test]
		[Description ("Value: True | Notes: The value of this property must always be True. This means "
			+"that the check box control must always be included in the control view of the UI Automation tree.")]
		public override void MsdnIsControlElementPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (true,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.IsControlElementProperty, true),
				"IsControlElementProperty");
		}

		[Test]
		[NotListed]
		public override void MsdnOrientationPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (null,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.OrientationProperty, true),
				"OrientationProperty");
		}

		#endregion Properties Test

		#region Pattern Tests

		[Test]
		[Description ("Support/Value: Required . | Notes: Allows the check box to be cycled through its "
			+"internal states programmatically.")]
		public override void MsdnTogglePatternTest ()
		{
			AutomationElement element = GetAutomationElement ();

			Assert.IsTrue (SupportsPattern (element, TogglePatternIdentifiers.Pattern),
				"TogglePattern SHOULD be supported");
		}

		#endregion

		#region Protected Methods

		protected override AutomationElement GetAutomationElement ()
		{
			return GetAutomationElementFromControl (GetCheckBox ());
		}

		#endregion Protected Methods

		#region Private Methods

		private CheckBox GetCheckBox ()
		{
			CheckBox checkbox = new CheckBox ();
			checkbox.Text = "I'm a happy SWF CheckBox :)";
			checkbox.Size = new System.Drawing.Size (100, 30);
			checkbox.Location = new System.Drawing.Point (5, 5);
			return checkbox;
		}

		#endregion
	}
}
