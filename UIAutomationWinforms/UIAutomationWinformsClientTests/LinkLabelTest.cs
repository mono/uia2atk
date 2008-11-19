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
	[Description ("Tests SWF.LinkLabel as ControlType.HyperLink")]
	public class LinkLabelTest : BaseTest
	{
		#region Properties Test 

		[Test]
		[Description ("Value: Button | Notes: This value is the same for all UI frameworks.")]
		public override void MsdnControlTypePropertyTest () 
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (ControlType.Hyperlink,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty, true),
				"ControlType");
		}
		
		[Test]
		[Description ("Value: See notes. | Notes: The hyperlink control’s name is the text that "
			+"is displayed on the screen as underlined. ")] 
		public override void MsdnNamePropertyTest ()
		{
			LinkLabel linkLabel = GetControl () as LinkLabel;
			AutomationElement child = GetAutomationElementFromControl (linkLabel);

			Assert.AreEqual (linkLabel.Text,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.NameProperty, true),
				"NameProperty");

			linkLabel.Text = "Unhappy linklabel";
			Assert.AreEqual (linkLabel.Text,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.NameProperty, true),
				"NameProperty");
		}

		[Test]
		[LameSpec]
		[Description ("Value: Null | Notes: If there is a static text label then this property "
			+"must expose a reference to that control.  ")]
		public override void MsdnLabeledByPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (null,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.LabeledByProperty, true),
				"LabeledByProperty");
		}

		[Test]
		[Description (@"Value: ""hyperlink"" | Notes: Localized string corresponding to the Hyperlink control type. ")]
		public override void MsdnLocalizedControlTypePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual ("hyperlink",
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty, true),
				"LocalizedControlType");
		}

		[Test]
		[Description ("Value: True | Notes: The hyperlink control is always included in the content view of the UI Automation tree. ")]
		public override void MsdnIsContentElementPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (true,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.IsContentElementProperty, true),
				"IsContentElementProperty");
		}

		[Test]
		[LameSpec]
		[Description ("Value: True | Notes: The hyperlink control is always included in the control view of the UI Automation tree. ")]
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
		[NotListed]
		[Description ("Is not listed. We are using the default logic used in other controls.")]
		public override void MsdnHelpTextPropertyTest ()
		{
			LinkLabel linkLabel = GetControl () as LinkLabel;

			ToolTip tooltip = new ToolTip ();
			tooltip.SetToolTip (linkLabel, "I'm HelpTextProperty in button");

			AutomationElement child = GetAutomationElementFromControl (linkLabel);
			Assert.AreEqual (tooltip.GetToolTip (linkLabel),
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.HelpTextProperty, true),
				"HelpTextProperty");
		}

		#endregion

		#region Pattern Tests

		[Test]
		[Description ("Support/Value: See notes. | Notes: All hyperlink controls must support the Invoke pattern. ")]
		public override void MsdnInvokePatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsTrue (SupportsPattern (element, InvokePatternIdentifiers.Pattern), 
				"InvokePattern SHOULD be supported, not TogglePattern");
		}

		#endregion

		#region Protected metods

		protected override Control GetControl ()
		{
			LinkLabel linkLabel = new LinkLabel ();
			linkLabel.Text = "I'm a happy SWF linklabel :)";
			linkLabel.Size = new System.Drawing.Size (100, 30);
			linkLabel.Location = new System.Drawing.Point (5, 5);
			return linkLabel;
		}

		#endregion
	}
}
