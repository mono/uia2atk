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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Mike Gorse <mgorse@novell.com>
// 

using System;
using NUnit.Framework;
using Mono.UIAutomation.Source;
using System.Windows.Automation;

namespace AtspiUiaSourceTest
{
	[TestFixture]
	public class IElementTest : Base
	{
		IElement frame = null;

		public IElementTest ()
		{
			frame = GetApplication ("gtkbutton.py", "AtspiUiaSource Test");
			Assert.IsNotNull (frame, "frame should not be null");
		}

		#region Test
		[Test]
		public void NavigationTest ()
		{
			Assert.AreEqual ("window", frame.LocalizedControlType, "Frame.LocalizedControlType");
			Assert.AreEqual (ControlType.Window, frame.ControlType, "Frame.ControlType");
			Assert.IsNull (frame.NextSibling, "frame.NextSibling");
			Assert.IsNull (frame.PreviousSibling, "frame.NextSibling");

			IElement pane = frame.FirstChild;
			Assert.AreEqual ("group", pane.LocalizedControlType, "Pane.LocalizedControlType");
			Assert.AreEqual (ControlType.Group, pane.ControlType, "Pane ControlType");

			IElement button1 = pane.FirstChild;
			Assert.AreEqual ("button", button1.LocalizedControlType, "Button.LocalizedControlType");
			Assert.AreEqual (ControlType.Button, button1.ControlType, "Button ControlType");

			Assert.IsNull (button1.PreviousSibling, "Button1.PreviousSibling");
			IElement button2 = button1.NextSibling;
			Assert.IsNotNull (button2, "Second button");
			Assert.AreNotEqual (button1, button2, "Button and button2 are distinct buttons");
			Assert.AreEqual ("button", button2.LocalizedControlType, "Button.LocalizedControlType");
			Assert.AreEqual (ControlType.Button, button2.ControlType, "Button ControlType");
			Assert.AreEqual (button1, button2.PreviousSibling, "PreviousSibling");
			Assert.AreEqual (button2, button2.PreviousSibling.NextSibling, "PreviousSibling.NextSibling");

			Assert.AreEqual ("openSUSE", pane.LastChild.Name, "Pane.LastChild");
			Assert.AreEqual (button2.NextSibling, pane.LastChild, "button2.NextChild == pane.LastChild");
			Assert.AreNotEqual (button1.NextSibling, pane.LastChild, "button1.NextSibling != pane.LastChild");

			Assert.IsNull (button1.FirstChild, "button1 FirstChild");
			Assert.IsNull (button1.LastChild, "button1 LastChild");
		}

		[Test]
		public void IsEnabledTest ()
		{
			IElement button1 = frame.FirstChild.FirstChild;
			IElement button2 = button1.NextSibling;

			Assert.IsTrue (button1.IsEnabled, "IsEnabled #1");
			Assert.IsFalse (button2.IsEnabled, "IsEnabled #2");
		}

		[Test]
		public void IsOffscreenTest ()
		{
			IElement button1 = frame.FirstChild.FirstChild;
			IElement button3 = button1.NextSibling.NextSibling;

			Assert.IsTrue (button3.IsOffscreen, "IsOffscreen #1");
			Assert.IsFalse (button1.IsOffscreen, "IsOffscreen #2");
		}

		[Test]
		public void IsKeyboardFocusableTest ()
		{
			IElement pane = frame.FirstChild;
			IElement button = pane.FirstChild;

			Assert.IsTrue (button.IsKeyboardFocusable, "IsKeyboardFocusable #1");
			Assert.IsFalse (pane.IsKeyboardFocusable, "IsKeyboardFocusable #2");
		}

		[Test]
		public void HasKeyboardFocusTest ()
		{
			IElement button1 = frame.FirstChild.FirstChild;
			IElement button2 = button1.NextSibling;

			Assert.IsTrue (button1.HasKeyboardFocus, "HasKeyboardFocus #1");
			Assert.IsFalse (button2.HasKeyboardFocus, "HasKeyboardFocus #2");
		}
#endregion
	}
}
