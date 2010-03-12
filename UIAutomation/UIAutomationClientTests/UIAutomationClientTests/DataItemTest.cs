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
// Copyright (c) 2010 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//  Matt Guo <matt@mattguo.com>
// 

using System;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;
using NUnit.Framework;
using At = System.Windows.Automation.Automation;
using SW = System.Windows;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class DataItemTest : BaseTest
	{
		[Test]
		public void Bug584340 ()
		{
			// Bug summary: In ListView, Calling ValuePattern.SetValue on one cell can take effect on another sibling cell Winforms.
			var aliceElement = table1Element.FindFirst (TreeScope.Descendants,
				new PropertyCondition (AEIds.NameProperty, "Alice"));
			var _24Element = TreeWalker.RawViewWalker.GetNextSibling (aliceElement);
			Assert.AreEqual ("24", _24Element.Current.Name);
		}

		[Test]
		public void Bug586635 ()
		{
			// Bug summary: The NameProperty of DataGridView's Cell is not correctly returned.
			var item0Element = listView1Element.FindFirst (TreeScope.Descendants,
				new AndCondition (new PropertyCondition (AEIds.NameProperty, "Item 0"),
					new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Edit)));
			var subItem1Element = listView1Element.FindFirst (TreeScope.Descendants,
				new AndCondition (new PropertyCondition (AEIds.NameProperty, "subitem1"),
					new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Edit)));
			var vp = (ValuePattern) subItem1Element.GetCurrentPattern (ValuePattern.Pattern);
			vp.SetValue ("New Sub");
			Assert.AreEqual ("Item 0", item0Element.Current.Name);
			Assert.AreEqual ("New Sub", subItem1Element.Current.Name);
			vp = (ValuePattern) item0Element.GetCurrentPattern (ValuePattern.Pattern);
			vp.SetValue ("New Item Val");
			Assert.AreEqual ("New Item Val", item0Element.Current.Name);
			Assert.AreEqual ("New Sub", subItem1Element.Current.Name);
		}
	}
}
