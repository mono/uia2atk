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
using System.Windows;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	
	[TestFixture]
	public class LinkLabelProviderTest : BaseProviderTest
	{
		
#region Tests
		
		[Test]
		public void BasicPropertiesTest ()
		{
			LinkLabel linkLabel = new LinkLabel ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (linkLabel);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LabeledByProperty,
			              null);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Hyperlink.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "hyperlink");
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsContentElementProperty,
			              true);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsControlElementProperty,
			              true);
		}
		
#endregion
		
#region IInvokeEvent
		
		[Test]
		public void InvokeEventTest ()
		{
			LinkLabel linkLabel = new LinkLabel ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (linkLabel);
			
			bridge.ResetEventLists ();
			
			IInvokeProvider invokeProvider = (IInvokeProvider) 
				provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			
			invokeProvider.Invoke ();
			
			Assert.AreEqual (1,
			                 bridge.AutomationEvents.Count,
			                 "Event count");
		}
		
		[Test]
		public void InvokeEventDisabledTest ()
		{
			LinkLabel linkLabel = new LinkLabel ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (linkLabel);
			
			linkLabel.Enabled = false;
			
			IInvokeProvider invokeProvider = (IInvokeProvider) 
				provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			try {
				invokeProvider.Invoke ();
				Assert.Fail ("Exception not thrown");
			} catch (ElementNotEnabledException) { } 
		}
		
#endregion
		
#region BaseProviderTest Overrides

		protected override Control GetControlInstance ()
		{
			return new LinkLabel ();
		}
		
#endregion

	}
}
