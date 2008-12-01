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
//      Brad Taylor <brad@getcoded.net>
// 


using System;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class TabControlProviderTest : BaseProviderTest
	{
		[Test]
		public void BasicPropertiesTest ()
		{
			TabControl tc = new TabControl ();
			IRawElementProviderSimple provider
				= ProviderFactory.GetProvider (tc);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Tab.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "tab");
		}

		[Test]
		public void ISelectionProviderEventTest ()
		{
			TabControl tc = new TabControl ();
			TabPage tp1 = new TabPage ();
			TabPage tp2 = new TabPage ();
			tc.Controls.Add (tp1);
			tc.Controls.Add (tp2);
			Form.Controls.Add (tc);
			
			IRawElementProviderSimple provider
				= ProviderFactory.GetProvider (tc);

			object selectionProvider
				= provider.GetPatternProvider (
					SelectionPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (selectionProvider, "Not returning SelectionPatternIdentifiers.");
			Assert.IsTrue (selectionProvider is ISelectionProvider, "Not returning SelectionPatternIdentifiers");

			tc.SelectTab (0);

			// Case 1: Select currently selected tab
			bridge.ResetEventLists ();
			tc.SelectTab (0);
			Assert.AreEqual (0, bridge.AutomationEvents.Count,
			                 "EventCount after selecting selected tab");

			// Case 2: Select different tab
			bridge.ResetEventLists ();
			tc.SelectTab (1);
			Assert.AreEqual (1, bridge.AutomationEvents.Count,
			                 "EventCount after selecting new tab");
		}
		
		[Test]
		public override void LabeledByAndNamePropertyTest ()
		{
			TestLabeledByAndName (true, false);
		}

		protected override Control GetControlInstance ()
		{
			return new TabControl ();
		}
	}
}
