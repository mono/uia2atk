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

using System;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	public class ToolStripProviderTest : BaseProviderTest
	{
		[Test]
		public void BasicPropertiesTest ()
		{
			ToolStrip strip = new ToolStrip ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (strip);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.ToolBar.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "tool bar");

			// TODO: OrientationProperty, IsKeyboardFocusableProperty, AccessKeyProperty
		}

		// TODO: Test add/removal of items, navigation, etc
		
		[Test]
		[Ignore ("Not implemented")]
		public void ProviderPatternTest ()
		{
			//ToolStrip strip = new ToolStrip ();
			//IRawElementProviderSimple provider = ProviderFactory.GetProvider (strip);

			// Should never support Transform // TODO: Really? Maybe possible. Test in Vista.
			//object transformProvider = provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			//Assert.IsNull (transformProvider);

			// Should never support ExpandCollapse // TODO: Really? I think this is possible. Test in Vista.
			//object expandCollapseProvider = provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);
			//Assert.IsNull (expandCollapseProvider);

			// TODO: When to support dock?
			//object dockProvider = provider.GetPatternProvider (DockPatternIdentifiers.Pattern.Id);
			//Assert.IsNotNull (dockProvider);
			//Assert.IsTrue (dockProvider is IDockProvider, "IDockProvider");
		}

		[Test]
		public override void LabeledByAndNamePropertyTest()
		{
			TestLabeledByAndName (false, false);
		}

		protected override Control GetControlInstance()
		{
			return new ToolStrip ();
		}
	}
}
