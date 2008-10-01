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
			throw new NotImplementedException ();
		}

		[Test]
		public override void MsdnLabeledByPropertyTest ()
		{
			throw new NotImplementedException ();
		}

		[Test]
		[Description (@"Value: ""list"" | Notes: Localized string corresponding to the List control type. ")]
		public override void MsdnLocalizedControlTypePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual ("list",
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty, true),
				"IsControlElementProperty");
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

		#region Protected Methods

		private ListBox CreateListBox ()
		{
			ListBox listbox = new ListBox ();
			listbox.Items.AddRange (new object [] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
			listbox.Size = new Size (100, 100);
			listbox.Location = new Point (3, 3);
			return listbox;
		}

		protected override AutomationElement GetAutomationElement ()
		{
			return GetAutomationElementFromControl (CreateListBox ());
		}

		#endregion
	}
}
