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
	//According to http://msdn.microsoft.com/en-us/library/ms742539.aspx
	[TestFixture]
	[Description ("Tests SWF.ScrollBar internal Thumb as ControlType.Thumb")]
	public class ScrollBarThumbTest : ScrollBarTest
	{
		#region Properties

		[Test]
		[Ignore ("Not yet implemented")]
		[Description ("Value: See notes. | Notes: Any point within the visible client area of the Thumb control. ")]
		public override void MsdnClickablePointPropertyTest ()
		{
		}

		[Test]
		[Description ("Value: See notes. | Notes: If the control can receive keyboard focus, it must support "
			+ "this property.")]
		public override void MsdnIsKeyboardFocusablePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (false,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty, true),
				"IsKeyboardFocusable");
		}

		[Test]
		[Description ("Value: Thumb | Notes: This value is the same for all UI frameworks.")]
		public override void MsdnControlTypePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (ControlType.Thumb,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty, true),
				"ControlType");
		}

		[Test]
		[LameSpec]
		[Description ("Value: Null | The Thumb control is not available in the Content View of the UI"
			+"Automation tree so it odes not require a name. .")]
		public override void MsdnNamePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (null,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.NameProperty, true),
				"NameProperty");
		}

		[Test]
		[Description ("Value: null | Notes: Thumb controls never have a label. . ")]
		public override void MsdnLabeledByPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (null,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.LabeledByProperty, true),
				"LabeledByProperty");
		}

		[Test]
		[Description (@"Value: ""thumb"" | Notes: Localized string corresponding to the Thumb control type. ")]
		public override void MsdnLocalizedControlTypePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual ("thumb",
					child.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty, true),
					"LocalizedControlTypeProperty");
		}

		[Test]
		[Description ("Value: False | Notes: The Thumb control is never content.")]
		public override void MsdnIsContentElementPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (false,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.IsContentElementProperty, true),
				"IsContentElementProperty");
		}

		[Test]
		[Description ("Value: True | Notes: The Thumb control must always be a control.")]
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
		[Description ("Is not listed. We are using null as default value.")]
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
		[LameSpec]
		[Description ("Support/Value: Depends | This functionality is required to be supported only if "
			+"the Scroll control pattern is not supported on the container that has the scroll bar.")]
		public override void MsdnRangeValuePatternTest ()
		{
			AutomationElement child = GetAutomationElement ();
			AutomationElement parent = TreeWalker.RawViewWalker.GetParent (child);

			bool parentSupportsScrollPattern = SupportsPattern (parent, ScrollPatternIdentifiers.Pattern);

			if (SupportsPattern (child, RangeValuePatternIdentifiers.Pattern) == true
				&& parentSupportsScrollPattern == true)
				Assert.Fail ("RangeValuePattern NOT SUPPORTED when parent supports ScrollPattern");

			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, RangeValuePatternIdentifiers.Pattern),
				string.Format ("RangeValuePattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		[Description ("Support/Value: Depends | Controls that use a Thumb for moving or resizing should "
			+"implement ITransformProvider.")]
		public override void  MsdnTransformPatternTest()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, TransformPatternIdentifiers.Pattern),
				string.Format ("TransformPattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		#endregion Patterns

		#region Protected Methods

		protected override AutomationElement GetAutomationElement ()
		{
			AutomationElement scrollBarElement = base.GetAutomationElement ();
			AutomationElement thumbElement = null;

			AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild (scrollBarElement);
			while (child != null) {
				if (child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty, true)
					== ControlType.Thumb) {
					thumbElement  = child;
					break;
				}
				child = TreeWalker.RawViewWalker.GetNextSibling (child);
			}

			return thumbElement;
		}

		#endregion
	}
}
