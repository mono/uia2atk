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
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms.Client
{
	//According to http://msdn.microsoft.com/en-us/library/ms743712.aspx
	[TestFixture]
	[Description ("Tests SWF.ScrollBar (SWF.HScrollBar/SWF.VScrollBar) as ControlType.ScrollBar")]
	public class ScrollBarButtonTest : ButtonTest
	{
		#region Properties

		[Test]
		[Description ("Notes: See notes. | Values: If the control can receive keyboard focus, it must "
			+"support this property. ")]
		public override void MsdnIsKeyboardFocusablePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (false,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty, true),
				"IsKeyboardFocusable");
		}

		[Test]
		[Ignore ("This is really vague, the values changues depending on the button in the scrollbar")]
		public override void MsdnNamePropertyTest ()
		{
		}

		[Test]
		[LameSpec]
		[Description ("Value: True | Notes: The Button control must always be content.")]
		public override void MsdnIsContentElementPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (true,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.IsContentElementProperty, true),
				"IsContentElementProperty");
		}

		[Test]
		[Description ("Value: See Notes | Notes: The Help Text should indicate what the end result of activating "
			+ "the button will be. This information must be exposed through a ToolTip. ")]
		public override void MsdnHelpTextPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (null,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.HelpTextProperty, true),
				"HelpTextProperty");
		}

		#endregion Properties

		#region Patterns

		[Test]
		[Description ("Support/Value: See notes. | Notes: All buttons must support the "
			+ "Invoke control pattern or the Toggle control pattern. Invoke is supported when the button performs "
			+ "a command at the request of the user. This command maps to a single operation such as Cut, Copy, "
			+ "Paste, or Delete. ")]
		public override void MsdnInvokePatternTest ()
		{
			AutomationElement element = GetAutomationElement ();

			if (SupportsPattern (element, TogglePatternIdentifiers.Pattern) == true
				&& SupportsPattern (element, InvokePatternIdentifiers.Pattern) == true)
				Assert.Fail ("InvokePattern SHOULD ONLY be supported, not TogglePattern");
		}

		[Test]
		[Description ("Support/Value: See notes. | Notes: All buttons must support the Invoke control "
			+ "pattern or the Toggle control pattern. Toggle is supported if the button can be cycled through a series "
			+ "of up to three states. Typically this is seen as an on/off switch for specific features. ")]
		public override void MsdnTogglePatternTest ()
		{
			AutomationElement element = GetAutomationElement ();

			if (SupportsPattern (element, TogglePatternIdentifiers.Pattern) == true
				&& SupportsPattern (element, InvokePatternIdentifiers.Pattern) == true)
				Assert.Fail ("TogglePattern SHOULD ONLY be supported, not InvokePattern ");
		}

		#endregion Patterns

		#region Protected Methods

		protected override AutomationElement GetAutomationElement ()
		{
			ListBox listbox = new ListBox ();
			listbox.Items.AddRange (new object [] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
			listbox.Size = new Size (100, 100);
			listbox.Location = new Point (3, 3);

			AutomationElement listboxElement = GetAutomationElementFromControl (listbox);
			AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild (listboxElement);
			AutomationElement scrollBarChild = null;

			Assert.IsNotNull (child, "ListBox should have children");
			while (child != null) {
				if (child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty, true)
					== ControlType.ScrollBar) {
					scrollBarChild = child;
					break;
				}
				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}

			AutomationElement buttonElement = null;

			child = TreeWalker.RawViewWalker.GetFirstChild (scrollBarChild);
			while (child != null) {
				if (child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty, true)
					== ControlType.Button) {
					buttonElement = child;
					break;
				}
				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}

			return buttonElement;
		}

		#endregion
	}
}
