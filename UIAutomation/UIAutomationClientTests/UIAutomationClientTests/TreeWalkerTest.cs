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
using SWA = System.Windows.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class TreeWalkerTest : BaseTest
	{
		#region Test Methods

		[Test]
		public void ControlViewWalkerTest ()
		{
			Assert.AreEqual (SWA.Automation.ControlViewCondition,
				TreeWalker.ControlViewWalker.Condition,
				"Condition");
		}

		[Test]
		public void ContentViewWalkerTest ()
		{
			Assert.AreEqual (SWA.Automation.ContentViewCondition,
				TreeWalker.ContentViewWalker.Condition,
				"Condition");
		}

		[Test]
		public void RawViewWalkerTest ()
		{
			Assert.AreEqual (SWA.Automation.RawViewCondition,
				TreeWalker.RawViewWalker.Condition,
				"Condition");
		}

		[Test]
		public void ConditionTest ()
		{
			AssertRaises<ArgumentNullException> (
				() => new TreeWalker (null),
				"passing null to TreeWalker constructor");

			Condition buttonCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button);
			TreeWalker buttonWalker = new TreeWalker (buttonCondition);
			Assert.AreEqual (buttonCondition, buttonWalker.Condition, "Condition");
		}

		[Test]
		public void GetFirstChildTest ()
		{
			Condition buttonCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button);
			TreeWalker buttonWalker = new TreeWalker (buttonCondition);

			AssertRaises<ArgumentNullException> (
				() => buttonWalker.GetFirstChild (null),
				"passing null to TreeWalker.GetFirstChild");

			VerifyGetFirstChild (buttonWalker, groupBox1Element, button7Element);
		}

		[Test]
		public void GetLastChildTest ()
		{
			Condition buttonCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button);
			TreeWalker buttonWalker = new TreeWalker (buttonCondition);

			AssertRaises<ArgumentNullException> (
				() => buttonWalker.GetLastChild (null),
				"passing null to TreeWalker.GetLastChild");

			VerifyGetLastChild (buttonWalker, groupBox1Element, button2Element);
		}

		[Test]
		public void GetNextSiblingTest ()
		{
			Condition buttonCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button);
			TreeWalker buttonWalker = new TreeWalker (buttonCondition);

			AssertRaises<ArgumentNullException> (
				() => buttonWalker.GetNextSibling (null),
				"passing null to TreeWalker.GetNextSibling");

			VerifyGetNextSibling (buttonWalker, button7Element, button6Element);
			VerifyGetNextSibling (buttonWalker, button6Element, button5Element);
			VerifyGetNextSibling (buttonWalker, button5Element, button4Element);
			VerifyGetNextSibling (buttonWalker, button4Element, button3Element);
			VerifyGetNextSibling (buttonWalker, button3Element, button2Element);

			// Check that there are still more siblings (just without groupBox1Element as parent)
			VerifyGetNextSibling (buttonWalker, button2Element, button1Element);

			// TODO: Test how for buttonWalker, GetNextSibling
			//       eventually gets all buttons on the entire
			//       desktop?

			// Elements whose parents (not the desktop) are also in
			// the tree run out of siblings as expected.
			Condition groupCondition = new AndCondition (
				new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Group),
				new PropertyCondition (AEIds.ProcessIdProperty, p.Id));
			TreeWalker groupWalker = new TreeWalker (groupCondition);

			VerifyGetNextSibling (groupWalker, groupBox3Element, groupBox2Element);
			VerifyGetNextSibling (groupWalker, groupBox2Element, null);

			// When only other matching thing in tree is child (TODO: Hangs)
			//VerifyGetNextSibling (groupWalker, groupBox1Element, groupBox2Element);
		}

		[Test]
		public void GetPreviousSiblingTest ()
		{
			Condition buttonCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button);
			TreeWalker buttonWalker = new TreeWalker (buttonCondition);

			AssertRaises<ArgumentNullException> (
				() => buttonWalker.GetPreviousSibling (null),
				"passing null to TreeWalker.GetPreviousSibling");

			//VerifyGetPreviousSibling (buttonWalker, button7Element, null); // TODO: scrollbar button

			VerifyGetPreviousSibling (buttonWalker, button6Element, button7Element);
			VerifyGetPreviousSibling (buttonWalker, button5Element, button6Element);
			VerifyGetPreviousSibling (buttonWalker, button4Element, button5Element);
			VerifyGetPreviousSibling (buttonWalker, button3Element, button4Element);
			VerifyGetPreviousSibling (buttonWalker, button2Element, button3Element);

			// TODO: Test how for buttonWalker, GetPreviousSibling
			//       eventually gets all buttons on the entire
			//       desktop?

			// Elements whose parents (not the desktop) are also in
			// the tree run out of siblings as expected.
			Condition groupCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Group);
			TreeWalker groupWalker = new TreeWalker (groupCondition);

			VerifyGetPreviousSibling (groupWalker, groupBox3Element, null);
			VerifyGetPreviousSibling (groupWalker, groupBox2Element, groupBox3Element);
		}

		[Test]
		public void GetParentTest ()
		{
			Condition buttonCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button);
			TreeWalker buttonWalker = new TreeWalker (buttonCondition);

			AssertRaises<ArgumentNullException> (
				() => buttonWalker.GetParent (null),
				"passing null to TreeWalker.GetParent");

			VerifyGetParent (buttonWalker, button7Element, null);

			Condition groupCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Group);
			TreeWalker groupWalker = new TreeWalker (groupCondition);

			// Test where applicable ancestor is parent
			VerifyGetParent (groupWalker, checkBox1Element, groupBox2Element);
			VerifyGetParent (groupWalker, groupBox3Element, groupBox1Element);
			VerifyGetParent (groupWalker, groupBox1Element, null);

			VerifyGetParent (TreeWalker.RawViewWalker, testFormElement, AutomationElement.RootElement);

			// Test where applicable ancestor is not direct parent
			Condition windowCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Window);
			TreeWalker windowWalker = new TreeWalker (windowCondition);

			VerifyGetParent (windowWalker, groupBox3Element, testFormElement);
		}

		[Test]
		public void NormalizeTest ()
		{
			Condition groupCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Group);
			TreeWalker groupWalker = new TreeWalker (groupCondition);

			AssertRaises<ArgumentNullException> (
				() => groupWalker.Normalize (null),
				"passing null to TreeWalker.Normalize");

			Assert.AreEqual (groupBox1Element,
				groupWalker.Normalize (groupBox1Element),
				"If element matches, return it");

			Assert.AreEqual (groupBox1Element,
				groupWalker.Normalize (button2Element),
				"When element does not match, return first matching ancestor");

			// This is according to MSDN:
			// http://msdn.microsoft.com/en-us/library/system.windows.automation.treewalker.normalize.aspx
			//Assert.AreEqual (AutomationElement.RootElement,
			//        groupWalker.Normalize (button1Element),
			//        "When neither elment nor ancestors match, return RootElement");

			// This is how Microsoft actually implemented it:
			Assert.IsNull (groupWalker.Normalize (button1Element),
				"When neither element nor ancestors match, return null");

			Condition noNameCondition = new PropertyCondition (AEIds.NameProperty, string.Empty);
			TreeWalker noNameWalker = new TreeWalker (noNameCondition);

			Assert.AreEqual (AutomationElement.RootElement,
				noNameWalker.Normalize (button1Element),
				"When RootElement matches, it should be returned");

			Assert.AreEqual (AutomationElement.RootElement,
				TreeWalker.RawViewWalker.Normalize (AutomationElement.RootElement),
				"When RootElement matches, it should be returned");

			Assert.AreEqual (null,
				new TreeWalker (Condition.FalseCondition).Normalize (AutomationElement.RootElement),
				"When RootElement does not match, null should be returned");
		}

		#endregion

		#region Private Helper Methods

		private void VerifyGetFirstChild (TreeWalker tree, AutomationElement rootElement, AutomationElement expectedElement)
		{
			AutomationElement actualChild = tree.GetFirstChild (rootElement);
			Assert.AreEqual (expectedElement, actualChild,
				String.Format ("GetFirstChild with root element named {0}: Expected element named {1}, got element named {2}",
				GetName (rootElement), GetName (expectedElement), GetName (actualChild)));
			if (expectedElement != null)
				Assert.AreNotSame (expectedElement, actualChild, "GetFirstChild returns a new instance");
		}

		private void VerifyGetLastChild (TreeWalker tree, AutomationElement rootElement, AutomationElement expectedElement)
		{
			AutomationElement actualChild = tree.GetLastChild (rootElement);
			Assert.AreEqual (expectedElement, actualChild,
				String.Format ("GetLastChild with root element named {0}: Expected element named {1}, got element named {2}",
				GetName (rootElement), GetName (expectedElement), GetName (actualChild)));
			if (expectedElement != null)
				Assert.AreNotSame (expectedElement, actualChild, "GetLastChild returns a new instance");
		}

		private void VerifyGetNextSibling (TreeWalker tree, AutomationElement rootElement, AutomationElement expectedElement)
		{
			AutomationElement actualChild = tree.GetNextSibling (rootElement);
			Assert.AreEqual (expectedElement, actualChild,
				String.Format ("GetNextSibling with root element named {0}: Expected element named {1}, got element named {2}",
				GetName (rootElement), GetName (expectedElement), GetName (actualChild)));
			if (expectedElement != null)
				Assert.AreNotSame (expectedElement, actualChild, "GetNextSibling returns a new instance");
		}

		private void VerifyGetPreviousSibling (TreeWalker tree, AutomationElement rootElement, AutomationElement expectedElement)
		{
			AutomationElement actualChild = tree.GetPreviousSibling (rootElement);
			Assert.AreEqual (expectedElement, actualChild,
				String.Format ("GetPreviousSibling with root element named {0}: Expected element named {1}, got element named {2}",
				GetName (rootElement), GetName (expectedElement), GetName (actualChild)));
			if (expectedElement != null)
				Assert.AreNotSame (expectedElement, actualChild, "GetPreviousSibling returns a new instance");
		}

		private void VerifyGetParent (TreeWalker tree, AutomationElement rootElement, AutomationElement expectedElement)
		{
			AutomationElement actualChild = tree.GetParent (rootElement);
			Assert.AreEqual (expectedElement, actualChild,
				String.Format ("GetParent with root element named {0}: Expected element named {1}, got element named {2}",
				GetName (rootElement), GetName (expectedElement), GetName (actualChild)));
			if (expectedElement != null)
				Assert.AreNotSame (expectedElement, actualChild, "GetParent returns a new instance");
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