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
	[Description ("Tests SWF.CheckedListBox Item as ControlType.ListItem")]
	public class CheckedListBoxListItem : ListBoxListItem
	{

		#region Automation Patterns Tests

		[Test]
		public override void  MsdnTogglePatternTest()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsTrue (SupportsPattern (element, TogglePatternIdentifiers.Pattern),
				"TogglePattern SHOULD be supported");
		}

		#endregion

		#region Protected Methods

		protected override Control GetControl ()
		{
			CheckedListBox listbox = new CheckedListBox ();
			listbox.Items.AddRange (new object [] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
			listbox.Size = new Size (100, 100);
			listbox.Location = new Point (3, 3);
			return listbox;
		}

		protected override AutomationElement GetAutomationElement ()
		{
			CheckedListBox listbox = new CheckedListBox ();
			listbox.Items.AddRange (new object [] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
			listbox.Size = new Size (100, 100);
			listbox.Location = new Point (3, 3);

			AutomationElement listboxElement = GetAutomationElementFromControl (listbox);

			AutomationElement child = TreeWalker.ContentViewWalker.GetFirstChild (listboxElement);
			while (child != null)
			{
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
