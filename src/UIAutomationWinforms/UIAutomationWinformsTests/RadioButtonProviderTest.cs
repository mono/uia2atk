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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 
//

using System;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	public class RadioButtonProviderTest : BaseProviderTest
	{
#region Test Methods
		
		[Test]
		public void BasicPropertiesTest ()
		{
			RadioButton radioButton = new RadioButton ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (radioButton);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.RadioButton.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "radio button");
			
			// TODO: Test properties implemented by SimpleControlProvider
			//       (should those be in BaseProviderTest perhaps?)
		}
		
		[Test]
		public void ProviderPatternTest ()
		{
			RadioButton radioButton = new RadioButton ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (radioButton);
			
			object toggleProvider = provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (toggleProvider);
			Assert.IsTrue (toggleProvider is ISelectionItemProvider, "ISelectionItemProvider");
		}
		
#endregion
		
#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return new RadioButton ();
		}
		
#endregion
	}
}
