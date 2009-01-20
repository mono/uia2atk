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
//	Neville Gao <nevillegao@gmail.com>
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
	public class SplitContainerProviderTest : BaseProviderTest
	{
		#region Test
		
		[Test]
		public void BasicPropertiesTest ()
		{
			SplitContainer splitContainer = new SplitContainer ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (splitContainer);
			
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
			SplitContainer splitContainer = new SplitContainer ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (splitContainer);
			
			object dockProvider =
				provider.GetPatternProvider (DockPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (dockProvider,
			                  "Not returning DockPatternIdentifiers.");
			Assert.IsTrue (dockProvider is IDockProvider,
			               "Not returning DockPatternIdentifiers.");
		}
		
		#endregion
		
		#region SplitterPanel ITransformProvider Test
		
		[Test]
		public void SplitterPanelITransformProviderCanMoveTest ()
		{
			SplitContainer splitContainer = new SplitContainer ();
			IRawElementProviderFragmentRoot rootProvider = 
				(IRawElementProviderFragmentRoot) GetProviderFromControl (splitContainer);
			IRawElementProviderFragment childProvider =
				rootProvider.Navigate (NavigateDirection.FirstChild);
			
			ITransformProvider transformProvider = (ITransformProvider)
				childProvider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (transformProvider,
			                  "Not returning TransformPatternIdentifiers.");
			
			// Default DockStyle is None
			Assert.IsFalse (transformProvider.CanMove,
			                "SplitterPanel can't be moved by default.");
		}
		
		[Test]
		public void SplitterPanelITransformProviderCanResizeTest ()
		{
			SplitContainer splitContainer = new SplitContainer ();
			IRawElementProviderFragmentRoot rootProvider = 
				(IRawElementProviderFragmentRoot) GetProviderFromControl (splitContainer);
			IRawElementProviderFragment childProvider =
				rootProvider.Navigate (NavigateDirection.FirstChild);
			
			ITransformProvider transformProvider = (ITransformProvider)
				childProvider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (transformProvider,
			                  "Not returning TransformPatternIdentifiers.");

			Assert.IsTrue (transformProvider.CanResize,
			               "SplitterPanel can be resized by default.");

			splitContainer.Panel1Collapsed = true;
			splitContainer.Panel2Collapsed = false;
			Assert.IsFalse (transformProvider.CanResize,
			               "SplitterPanel1 can't be resized.");

			splitContainer.Panel1Collapsed = false;
			splitContainer.Panel2Collapsed = true;
			Assert.IsFalse (transformProvider.CanResize,
			               "SplitterPanel2 can't be resized.");
		}
		
		[Test]
		public void SplitterPanelITransformProviderCanRotateTest ()
		{
			SplitContainer splitContainer = new SplitContainer ();
			IRawElementProviderFragmentRoot rootProvider = 
				(IRawElementProviderFragmentRoot) GetProviderFromControl (splitContainer);
			IRawElementProviderFragment childProvider =
				rootProvider.Navigate (NavigateDirection.FirstChild);
			
			ITransformProvider transformProvider = (ITransformProvider)
				childProvider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (transformProvider,
			                  "Not returning TransformPatternIdentifiers.");
			
			Assert.IsFalse (transformProvider.CanRotate,
			                "SplitterPanel can't be rotated.");
		}
		
		[Test]
		public void SplitterPanelITransformProviderMoveTest ()
		{
			SplitContainer splitContainer = new SplitContainer ();
			IRawElementProviderFragmentRoot rootProvider = 
				(IRawElementProviderFragmentRoot) GetProviderFromControl (splitContainer);
			IRawElementProviderFragment childProvider =
				rootProvider.Navigate (NavigateDirection.FirstChild);
			
			ITransformProvider transformProvider = (ITransformProvider)
				childProvider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (transformProvider,
			                  "Not returning TransformPatternIdentifiers.");
			
			try {
				double x = 10, y = 10;
				transformProvider.Move (x, y);
				Assert.Fail ("InvalidOperationException not thrown");
			} catch (InvalidOperationException) { }
		}
		
		[Test]
		public void SplitterPanelITransformProviderResizeTest ()
		{
			SplitContainer splitContainer = new SplitContainer ();
			IRawElementProviderFragmentRoot rootProvider = 
				(IRawElementProviderFragmentRoot) GetProviderFromControl (splitContainer);
			IRawElementProviderFragment childProvider =
				rootProvider.Navigate (NavigateDirection.FirstChild);
			
			ITransformProvider transformProvider = (ITransformProvider)
				childProvider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (transformProvider,
			                  "Not returning TransformPatternIdentifiers.");

			try {
				double width1 = 50, heitht1 = 50;
				splitContainer.Panel1Collapsed = true;
				splitContainer.Panel2Collapsed = false;
				transformProvider.Resize (width1, heitht1);
				Assert.Fail ("InvalidOperationException not thrown");
			} catch (InvalidOperationException) { }
			
			try {
				double width2 = 50, heitht2 = 50;
				splitContainer.Panel1Collapsed = false;
				splitContainer.Panel2Collapsed = true;
				transformProvider.Resize (width2, heitht2);
				Assert.Fail ("InvalidOperationException not thrown");
			} catch (InvalidOperationException) { }
		}
		
		[Test]
		public void SplitterPanelITransformProviderRotateTest ()
		{
			SplitContainer splitContainer = new SplitContainer ();
			IRawElementProviderFragmentRoot rootProvider = 
				(IRawElementProviderFragmentRoot) GetProviderFromControl (splitContainer);
			IRawElementProviderFragment childProvider =
				rootProvider.Navigate (NavigateDirection.FirstChild);
			
			ITransformProvider transformProvider = (ITransformProvider)
				childProvider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (transformProvider,
			                  "Not returning TransformPatternIdentifiers.");
			
			try {
				double degrees = 50;
				transformProvider.Rotate (degrees);
				Assert.Fail ("InvalidOperationException not thrown");
			} catch (InvalidOperationException) { }
		}
		
		#endregion
		
		#region IDockProvider Test
		
		[Test]
		public void IDockProviderDockPositionTest ()
		{
			SplitContainer splitContainer = new SplitContainer ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (splitContainer);
			
			TestHelper.IDockProviderDockPositionTest (provider, splitContainer);
		}
		
		[Test]
		public void IDockProviderSetDockPositionTest ()
		{
			SplitContainer splitContainer = new SplitContainer ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (splitContainer);
			
			TestHelper.IDockProviderSetDockPositionTest (provider, splitContainer);
		}
		
		#endregion
		
		#region SplitterPanel Test
		
		[Test]
        	public void SplitterPanelBasicPropertiesTest ()
        	{
            		SplitContainer splitContainer = new SplitContainer ();
            		IRawElementProviderFragmentRoot rootProvider =
				(IRawElementProviderFragmentRoot) GetProviderFromControl (splitContainer);
			
			IRawElementProviderFragment childProvider =
				rootProvider.Navigate (NavigateDirection.FirstChild);

			TestProperty (childProvider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Pane.Id);

			TestProperty (childProvider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "pane");
		}
		
		#endregion
		
		#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return new SplitContainer ();
		}
		
		#endregion
	}
}
