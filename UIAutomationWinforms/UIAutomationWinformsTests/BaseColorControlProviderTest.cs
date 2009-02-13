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
// 	Neville Gao <nevillegao@gmail.com>
// 

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
    	[TestFixture]
    	public class BaseColorControlProviderTest : BaseProviderTest
    	{
		#region Test

		[Test]
		public void BasicPropertiesTest ()
		{
			IRawElementProviderSimple provider = GetBaseColorControlProvider ();
			Assert.IsNotNull (provider, "We should have a BaseColorCotrol");
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Pane.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "pane");
		}

		#endregion

		#region SmallColorControl IInvokePattern Test

		[Test]
		public void SmallColorControlInvokeTest ()
		{
			IRawElementProviderFragmentRoot provider =
				(IRawElementProviderFragmentRoot) GetBaseColorControlProvider ();
			IRawElementProviderFragment child =
				provider.Navigate (NavigateDirection.FirstChild);

			IInvokeProvider invokeProvider = (IInvokeProvider)
				child.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (invokeProvider,
			                  "Not returning InvokePatternIdentifiers.");
			
			// TODO: we need to get ColorDialog.BaseColorControl.IsSelected to test this.
		}

		#endregion
		
		#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return null;
		}

		protected override IRawElementProviderSimple GetProvider ()
		{
			ColorDialog colorDialog = new ColorDialog ();
			return ProviderFactory.GetProvider (colorDialog);
		}

		#endregion

		private IRawElementProviderSimple GetBaseColorControlProvider ()
		{
			ColorDialog colorDialog = new ColorDialog ();
			IRawElementProviderFragmentRoot rootProvider =
				(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (colorDialog);

			Thread t = new Thread (new ThreadStart (delegate {
				colorDialog.ShowDialog ();
				}));
			t.Start();
			
			// Wait for dialog appear
			Thread.Sleep (1000);

			// Destroy dialog
			t.Abort ();
			
			IRawElementProviderFragment provider =
				rootProvider.Navigate (NavigateDirection.FirstChild);
			while (provider != null && 
			       provider.Navigate (NavigateDirection.FirstChild) == null)
				provider = provider.Navigate (NavigateDirection.NextSibling);

			return provider;
		}
	}
}
