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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class TreeWalkerTest : BaseTest
	{
		#region Test Methods

		[Test]
		public void ConditionTest ()
		{
			bool exceptionRaised = false;
			try {
				TreeWalker nullConditionWalker = new TreeWalker (null);
			} catch (ArgumentNullException) {
				exceptionRaised = true;
			}
			Assert.IsTrue (exceptionRaised, "Expected ArgumentNullException");

			Condition buttonCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button);
			TreeWalker buttonWalker = new TreeWalker (buttonCondition);
			Assert.AreEqual (buttonCondition, buttonWalker.Condition, "Condition");
		}

		[Test]
		public void GetFirstChildTest ()
		{
			Condition buttonCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button);
			TreeWalker buttonWalker = new TreeWalker (buttonCondition);

			bool exceptionRaised = false;
			try {
				buttonWalker.GetFirstChild (null);
			} catch (ArgumentNullException) {
				exceptionRaised = true;
			}
			Assert.IsTrue (exceptionRaised, "Expected ArgumentNullException");

			VerifyGetFirstChild (buttonWalker, groupBox1Element, button7Element);
		}

		[Test]
		public void GetLastChildTest ()
		{
			Condition buttonCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button);
			TreeWalker buttonWalker = new TreeWalker (buttonCondition);

			bool exceptionRaised = false;
			try {
				buttonWalker.GetLastChild (null);
			} catch (ArgumentNullException) {
				exceptionRaised = true;
			}
			Assert.IsTrue (exceptionRaised, "Expected ArgumentNullException");

			VerifyGetLastChild (buttonWalker, groupBox1Element, button2Element);
		}

		[Test]
		public void GetNextSiblingTest ()
		{
			Condition buttonCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button);
			TreeWalker buttonWalker = new TreeWalker (buttonCondition);

			bool exceptionRaised = false;
			try {
				buttonWalker.GetNextSibling (null);
			} catch (ArgumentNullException) {
				exceptionRaised = true;
			}
			Assert.IsTrue (exceptionRaised, "Expected ArgumentNullException");

			VerifyGetNextSibling (buttonWalker, button7Element, button6Element);
			VerifyGetNextSibling (buttonWalker, button6Element, button5Element);
			VerifyGetNextSibling (buttonWalker, button5Element, button4Element);
			VerifyGetNextSibling (buttonWalker, button4Element, button3Element);
			VerifyGetNextSibling (buttonWalker, button3Element, button2Element);

			// Check that there are still more siblings (just without groupBox1Element as parent)
			VerifyGetNextSibling (buttonWalker, button2Element, button1Element);
		}

		[Test]
		public void GetPreviousSiblingTest ()
		{
			Condition buttonCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button);
			TreeWalker buttonWalker = new TreeWalker (buttonCondition);

			bool exceptionRaised = false;
			try {
				buttonWalker.GetPreviousSibling (null);
			} catch (ArgumentNullException) {
				exceptionRaised = true;
			}
			Assert.IsTrue (exceptionRaised, "Expected ArgumentNullException");

			//VerifyGetPreviousSibling (buttonWalker, button7Element, null); // TODO: scrollbar button

			VerifyGetPreviousSibling (buttonWalker, button6Element, button7Element);
			VerifyGetPreviousSibling (buttonWalker, button5Element, button6Element);
			VerifyGetPreviousSibling (buttonWalker, button4Element, button5Element);
			VerifyGetPreviousSibling (buttonWalker, button3Element, button4Element);
			VerifyGetPreviousSibling (buttonWalker, button2Element, button3Element);
		}

		[Test]
		public void GetParentTest ()
		{
			Condition buttonCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button);
			TreeWalker buttonWalker = new TreeWalker (buttonCondition);

			bool exceptionRaised = false;
			try {
				buttonWalker.GetParent (null);
			} catch (ArgumentNullException) {
				exceptionRaised = true;
			}
			Assert.IsTrue (exceptionRaised, "Expected ArgumentNullException");

			VerifyGetParent (buttonWalker, button7Element, null);
		}

		#endregion

		#region Private Helper Methods

		private void VerifyGetFirstChild (TreeWalker tree, AutomationElement rootElement, AutomationElement expectedElement)
		{
			AutomationElement actualChild = tree.GetFirstChild (rootElement);
			Assert.AreEqual (expectedElement, actualChild,
				String.Format ("GetFirstChild with root element named {0}: Expected element named {1}, got element named {2}",
				GetName (rootElement), GetName (expectedElement), GetName (actualChild)));
		}

		private void VerifyGetLastChild (TreeWalker tree, AutomationElement rootElement, AutomationElement expectedElement)
		{
			AutomationElement actualChild = tree.GetLastChild (rootElement);
			Assert.AreEqual (expectedElement, actualChild,
				String.Format ("GetLastChild with root element named {0}: Expected element named {1}, got element named {2}",
				GetName (rootElement), GetName (expectedElement), GetName (actualChild)));
		}

		private void VerifyGetNextSibling (TreeWalker tree, AutomationElement rootElement, AutomationElement expectedElement)
		{
			AutomationElement actualChild = tree.GetNextSibling (rootElement);
			Assert.AreEqual (expectedElement, actualChild,
				String.Format ("GetNextSibling with root element named {0}: Expected element named {1}, got element named {2}",
				GetName (rootElement), GetName (expectedElement), GetName (actualChild)));
		}

		private void VerifyGetPreviousSibling (TreeWalker tree, AutomationElement rootElement, AutomationElement expectedElement)
		{
			AutomationElement actualChild = tree.GetPreviousSibling (rootElement);
			Assert.AreEqual (expectedElement, actualChild,
				String.Format ("GetPreviousSibling with root element named {0}: Expected element named {1}, got element named {2}",
				GetName (rootElement), GetName (expectedElement), GetName (actualChild)));
		}

		private void VerifyGetParent (TreeWalker tree, AutomationElement rootElement, AutomationElement expectedElement)
		{
			AutomationElement actualChild = tree.GetParent (rootElement);
			Assert.AreEqual (expectedElement, actualChild,
				String.Format ("GetParent with root element named {0}: Expected element named {1}, got element named {2}",
				GetName (rootElement), GetName (expectedElement), GetName (actualChild)));
		}

		private string GetName (AutomationElement element)
		{
			if (element == null)
				return "(null)";
			return element.Current.Name;
		}

		#endregion
	}
}