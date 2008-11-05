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
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class ToolTipProviderTest
	{
		[SetUp]
		public void SetUp ()
		{
			TestHelper.SetUpEnvironment ();
			
			form = new Form ();
			form.Show ();

			label = new Label ();
			label.Size = new System.Drawing.Size (30, 30);
			form.Controls.Add (label);

			tooltip = new ToolTip ();
			
			// Causes the tooltip to stay open after you focus
			// something else.  Second arg must not be empty, but
			// value is overwritten by Show().
			tooltip.SetToolTip (label, "ABCDEFGHIJKLMNOP");
			tooltip.ShowAlways = true;

			// Causes the tooltip to open even though the cursor isn't over the widget
			//tooltip.Show ("Hello", label, label.Location, 1000);
		}
		
		[TearDown]
		public void TearDown ()
		{
			TestHelper.TearDownEnvironment ();

			tooltip.Hide (label);
			form.Close ();
		}
		
		[Test]
		public void ControlPropertiesTest ()
		{
			// Causes the tooltip to open even though the cursor isn't over the widget
			tooltip.Show ("ABCDEFGHIJKLMNOP", label, label.Location, 1000);

			IRawElementProviderSimple provider
				= ProviderFactory.GetProvider (tooltip);
			
			TestHelper.TestAutomationProperty (
				provider, AutomationElementIdentifiers.ControlTypeProperty, ControlType.ToolTip.Id);
			TestHelper.TestAutomationProperty (
				provider, AutomationElementIdentifiers.NameProperty, "ABCDEFGHIJKLMNOP");
			TestHelper.TestAutomationProperty (
				provider, AutomationElementIdentifiers.LabeledByProperty, null);
			TestHelper.TestAutomationProperty (
				provider, AutomationElementIdentifiers.LocalizedControlTypeProperty, "tool tip");
		}

#region IWindowProvider tests
		[Test]
		public void IWindowProviderTest ()
		{
			IRawElementProviderSimple provider
				= ProviderFactory.GetProvider (tooltip);

			IWindowProvider win_prov = provider.GetPatternProvider (
				WindowPatternIdentifiers.Pattern.Id) as IWindowProvider;

			// SWF in Vista never implements IWindowProvider
			Assert.IsNull (win_prov);
		}

		[Test]
		public void IWindowProviderBalloonTest ()
		{
			// SWF in Vista never implements IWindowProvider, even in Balloon mode
			tooltip.IsBalloon = true;

			IRawElementProviderSimple provider
				= ProviderFactory.GetProvider (tooltip);

			IWindowProvider win_prov = provider.GetPatternProvider (
				WindowPatternIdentifiers.Pattern.Id) as IWindowProvider;

			Assert.IsNull (win_prov);
		}
#endregion

#region private fields
		private Form form;
		private Label label;
		private ToolTip tooltip;
#endregion
	}
}
