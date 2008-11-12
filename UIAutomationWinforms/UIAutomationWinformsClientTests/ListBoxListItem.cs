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
	//According to http://msdn.microsoft.com/en-us/library/ms744765.aspx
	[TestFixture]
	[Description ("Tests SWF.ListBox Item as ControlType.ListItem")]
	public class ListBoxListItem : BaseTest
	{
		[Test]
		[Description ("Value: ListItem | Notes: This value is the same for all UI frameworks.")]
		public override void MsdnControlTypePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (ControlType.ListItem,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty, true),
				"ControlType");
		}

		[Test]
		[Description ("Value See Notes | Notes: The value of a list item control's name property comes from the text "
			+"contents of the item.")]
		public override void MsdnNamePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual ("1",
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.NameProperty, true),
				"NameProperty");
		}

		[Test]
		public override void MsdnLabeledByPropertyTest ()
		{
			throw new NotImplementedException ();
		}

		[Test]
		[Description (@"Value: ""list item"" | Notes: The value of a list item control's name property comes from the "
			+"text contents of the item. ")]
		public override void MsdnLocalizedControlTypePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual ("list item",
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
		[LameSpec]
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
		[LameSpec]
		[Description (@"Value: """" | Notes: The Help text for list controls should explain why the user is "
			+@"being asked to make a choice from a list of options. For example, ""Selecting an item from this list "
			+@"will set the display resolution for your monitor.""")]
		public override void MsdnHelpTextPropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual ("",
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.HelpTextProperty, true),
				"HelpTextProperty");
		}

		#region Automation Patterns Tests

		[Test]
		public override void MsdnScrollItemPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsTrue (SupportsPattern (element, ScrollItemPatternIdentifiers.Pattern),
				"ScrollItemPattern SHOULD be supported");
		}

		[Test]
		public override void MsdnSelectionItemPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsTrue (SupportsPattern (element, SelectionItemPatternIdentifiers.Pattern),
				"SelectionItemPattern SHOULD be supported");
		}

		#endregion 

		#region Protected Methods

		protected override Control GetControl ()
		{
			return null;
		}

		protected override AutomationElement GetAutomationElement ()
		{
			ListBox listbox = new ListBox ();
			listbox.Items.AddRange (new object [] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
			listbox.Size = new Size (100, 100);
			listbox.Location = new Point (3, 3);

			AutomationElement listboxElement = GetAutomationElementFromControl (listbox);

			AutomationElement child = TreeWalker.ContentViewWalker.GetFirstChild (listboxElement);
			while (child != null) {
				if (child.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty)
					== ControlType.ListItem)
					return child;
				else
					child = TreeWalker.ContentViewWalker.GetNextSibling (child);
			}
			return null;
		}

		#endregion
	}
}
