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
	//According to http://msdn.microsoft.com/en-us/library/ms742153.aspx
	[TestFixture]
	[Description ("Tests SWF.Button as ControlType.Button")]
	public class ButtonTest : BaseTest
	{
		#region Properties Test 
		
		[Test]
		[Description ("Value: See notes. | Notes: The name of the button control is the text "
			+"that is used to label it. Whenever an image is used to label a button, alternate text "
			+"must be supplied for the button's Name property.")] 
		public override void MsdnNamePropertyTest ()
		{
			Button button = GetButton ();
			AutomationElement child = GetAutomationElementFromControl (button);

			Assert.AreEqual (button.Text,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.NameProperty, true),
				"NameProperty");

			button.Text = "Unhappy button";
			Assert.AreEqual (button.Text,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.NameProperty, true),
				"NameProperty");
		}

		[Test]
		[LameSpec]
		[Description ("Value: Null | Notes: Button controls are self-labeled by their contents. ")]
		public override void MsdnLabeledByPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (null,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.LabeledByProperty, true),
				"LabeledByProperty");
		}

		[Test]
		[Description (@"Value: ""button"" | Notes: Localized string corresponding to the Button control type.")]
		public override void MsdnLocalizedControlTypePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual ("button",
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
		[NotListedAttribute]
		[Description ("Is not listed. We are using null as default value.")]
		public override void MsdnOrientationPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (null,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.OrientationProperty, true),
				"OrientationProperty");
		}

		#endregion

		#region Pattern Tests

		[Test]
		[Description ("Support/Value: See notes. | Notes: All buttons must support the "
			+"Invoke control pattern or the Toggle control pattern. Invoke is supported when the button performs "
			+"a command at the request of the user. This command maps to a single operation such as Cut, Copy, "
			+"Paste, or Delete. ")]
		public override void MsdnInvokePatternTest ()
		{
			AutomationElement element = GetAutomationElement ();

			if (SupportsPattern (element, TogglePatternIdentifiers.Pattern) == true
				&& SupportsPattern (element, InvokePatternIdentifiers.Pattern) == true)
				Assert.Fail ("InvokePattern SHOULD ONLY be supported, not TogglePattern");
		}

		[Test]
		[Description ("Support/Value: See notes. | Notes: All buttons must support the Invoke control "
			+"pattern or the Toggle control pattern. Toggle is supported if the button can be cycled through a series "
			+"of up to three states. Typically this is seen as an on/off switch for specific features. ")]
		public override void MsdnTogglePatternTest ()
		{
			AutomationElement element = GetAutomationElement ();

			if (SupportsPattern (element, TogglePatternIdentifiers.Pattern) == true
				&& SupportsPattern (element, InvokePatternIdentifiers.Pattern) == true)
				Assert.Fail ("TogglePattern SHOULD ONLY be supported, not InvokePattern ");
		}

		#endregion

		#region Protected metods

		protected override AutomationElement GetAutomationElement ()
		{
			return GetAutomationElementFromControl (GetButton ());
		}

		#endregion

		#region Private Methods

		private Button GetButton ()
		{
			Button button = new Button ();
			button.Text = "I'm a happy SWF button :)";
			button.Size = new System.Drawing.Size (100, 30);
			button.Location = new System.Drawing.Point (5, 5);
			return button;
		}

		#endregion
	}
}
