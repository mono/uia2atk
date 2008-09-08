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
using System.ComponentModel;
using System.Windows.Forms;
using SWFErrorProvider = System.Windows.Forms.ErrorProvider;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	
	[TestFixture]
	public class ErrorProviderTest : BaseProviderTest
	{
		
		#region Tests
		
		[Test]
		public void BasicPropertiesTest ()
		{
			//Lets use 2 dummy Labels
			Label label1 = new Label ();
			label1.Text = "hola";
			label1.Size = new System.Drawing.Size (30, 30);
			label1.Location = new System.Drawing.Point (1, 1);
			
			Label label2 = new Label ();
			label2.Text = "mundo";
			label2.Size = new System.Drawing.Size (30, 30);
			label2.Location = new System.Drawing.Point (1, 32);
			
			IRawElementProviderSimple label1Provider 
				= GetProviderFromControl (label1);
			
			Form.Controls.Add (label2);
			
			//We already have 2 controls (label1 y label2) in the form,
			//lets navigate to look for ErrorProvider, we *shouldn't* find any
			
			IRawElementProviderFragmentRoot formProvider 
				= (IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (Form);
			Assert.IsNotNull (formProvider, "Form provider shouldn't be null");
			
			//Lets verify we have two labels *only*
			
			IRawElementProviderFragment child =
				(IRawElementProviderFragment) formProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (child, "child provider shouldn't be null");
			int count = 1;
			while (child != null) {
				child = child.Navigate (NavigateDirection.NextSibling);
				if (child != null)
					count++;
			}
			
			Assert.AreEqual (2, count, "We should only have 2 children");
			
			//Lets show the error message. We should have *only* 3 children
			
			SWFErrorProvider errorProvider = new SWFErrorProvider();
			errorProvider.SetError (label1, "Error: hola");
			errorProvider.SetError (label2, "Error: mundo");
			
			child = (IRawElementProviderFragment) formProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (child, "child provider shouldn't be null");
			count = 1;
			
			IRawElementProviderSimple provider = null;
			
			while (child != null) {
				if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
				    != ControlType.Text.Id)
					provider = child;
				child = child.Navigate (NavigateDirection.NextSibling);
				if (child != null)
					count++;
			}
			
			Assert.AreEqual (3, count, "We should only have 3 children");
			Assert.IsNotNull (provider, "We should have a provider different than Text");

			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Pane.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "pane");
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsContentElementProperty,
			              true);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsControlElementProperty,
			              true);
			              
			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              false);			              
			
			//Test patterns:
			TestProperty (provider,
			              AutomationElementIdentifiers.IsTransformPatternAvailableProperty,
			              false);
			TestProperty (provider,
			              AutomationElementIdentifiers.IsWindowPatternAvailableProperty,
			              false);
			TestProperty (provider,
			              AutomationElementIdentifiers.IsDockPatternAvailableProperty,
			              false);
			
			//Test Navigation
			Assert.IsNull (((IRawElementProviderFragment) provider).Navigate (NavigateDirection.FirstChild),
			               "child provider should be null");
		}

		#endregion
		
		#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return null;
		}
		
		#endregion
	}
}
