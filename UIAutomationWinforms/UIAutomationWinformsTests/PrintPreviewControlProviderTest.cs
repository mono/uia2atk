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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
    	//[TestFixture]
    	public class PrintPreviewControlroviderTest : BaseProviderTest
    	{
		#region Test

		[Test]
		public void BasicPropertiesTest ()
		{
			PrintPreviewControl printPreviewControl = new PrintPreviewControl ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (printPreviewControl);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Pane.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "pane");
		}

		[Test]
		public void ProviderPatternTest ()
		{
			PrintPreviewControl printPreviewControl = new PrintPreviewControl ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (printPreviewControl);

			object scrollProvider =
				provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (scrollProvider,
			                  "Not returning ScrollPatternIdentifiers.");
			Assert.IsTrue (scrollProvider is IScrollProvider,
			               "Not returning ScrollPatternIdentifiers.");
		}

		#endregion
		
		#region Navigation Test
		
		[Test]
		public void NavigationTest ()
		{
			PrintPreviewControl printPreviewControl = (PrintPreviewControl) GetControlInstance ();
			IRawElementProviderFragmentRoot rootProvider;
			IRawElementProviderFragment childProvider;

			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (printPreviewControl);
			IScrollProvider scrollProvider = (IScrollProvider)
				rootProvider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (scrollProvider,
			                  "Not returning ScrollPatternIdentifiers.");
			
			childProvider = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (childProvider, "We must have a child");
		}
		
		#endregion
		
		#region IScrollPattern Test

		[Test]
		public void IScrollProviderHorizontalScrollPercentTest ()
		{
			PrintPreviewControl printPreviewControl = new PrintPreviewControl ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (printPreviewControl);

			IScrollProvider scrollProvider = (IScrollProvider)
				provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (scrollProvider,
			                  "Not returning ScrollPatternIdentifiers.");
		}
		
		[Test]
		public void IScrollProviderHorizontalViewSizeTest ()
		{
			PrintPreviewControl printPreviewControl = new PrintPreviewControl ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (printPreviewControl);

			IScrollProvider scrollProvider = (IScrollProvider)
				provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (scrollProvider,
			                  "Not returning ScrollPatternIdentifiers.");
		}
		
		[Test]
		public void IScrollProviderVerticalScrollPercentTest ()
		{
			PrintPreviewControl printPreviewControl = new PrintPreviewControl ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (printPreviewControl);

			IScrollProvider scrollProvider = (IScrollProvider)
				provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (scrollProvider,
			                  "Not returning ScrollPatternIdentifiers.");
		}
		
		[Test]
		public void IScrollProviderVerticalViewSizeTest ()
		{
			PrintPreviewControl printPreviewControl = new PrintPreviewControl ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (printPreviewControl);

			IScrollProvider scrollProvider = (IScrollProvider)
				provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (scrollProvider,
			                  "Not returning ScrollPatternIdentifiers.");
		}
		
		[Test]
		public void IScrollProviderHorizontallyScrollableTest ()
		{
			PrintPreviewControl printPreviewControl = new PrintPreviewControl ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (printPreviewControl);

			IScrollProvider scrollProvider = (IScrollProvider)
				provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (scrollProvider,
			                  "Not returning ScrollPatternIdentifiers.");
		}
		
		[Test]
		public void IScrollProviderVerticallyScrollableTest ()
		{
			PrintPreviewControl printPreviewControl = new PrintPreviewControl ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (printPreviewControl);

			IScrollProvider scrollProvider = (IScrollProvider)
				provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (scrollProvider,
			                  "Not returning ScrollPatternIdentifiers.");
		}
		
		[Test]
		public void IScrollProviderScrollTest ()
		{
			PrintPreviewControl printPreviewControl = new PrintPreviewControl ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (printPreviewControl);

			IScrollProvider scrollProvider = (IScrollProvider)
				provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (scrollProvider,
			                  "Not returning ScrollPatternIdentifiers.");
		}
		
		[Test]
		public void IScrollProviderSetScrollPercentTest ()
		{
			PrintPreviewControl printPreviewControl = new PrintPreviewControl ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (printPreviewControl);

			IScrollProvider scrollProvider = (IScrollProvider)
				provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (scrollProvider,
			                  "Not returning ScrollPatternIdentifiers.");
		}
		
		#endregion
		
		#region BaseProviderTest Overrides

		protected override Control GetControlInstance ()
		{
			return new PrintPreviewControl ();
		}

		#endregion
	}
}
