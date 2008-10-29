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
	public class PanelProviderTest : BaseProviderTest
	{
		#region Test
		
		[Test]
		public void BasicPropertiesTest ()
		{
			Panel panel = new Panel ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (panel);
			
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
			Panel panel = new Panel ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (panel);
			
			object transformProvider =
				provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (transformProvider,
			                  "Not returning TransformPatternIdentifiers.");
			Assert.IsTrue (transformProvider is ITransformProvider,
			               "Not returning TransformPatternIdentifiers.");
			
			object dockProvider =
				provider.GetPatternProvider (DockPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (dockProvider,
			                  "Not returning DockPatternIdentifiers.");
			Assert.IsTrue (dockProvider is IDockProvider,
			               "Not returning DockPatternIdentifiers.");
		}
		
		#endregion
		
		#region ITransformProvider Test
		
		[Test]
		public void ITransformProviderCanMoveTest ()
		{
			Panel panel = new Panel ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (panel);
			
			ITransformProvider transformProvider = (ITransformProvider)
				provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (transformProvider,
			                  "Not returning TransformPatternIdentifiers.");
			
			// Default DockStyle is None
			Assert.IsTrue (transformProvider.CanMove,
			               "Panel can be moved by default.");
			panel.Dock = DockStyle.Bottom;
			Assert.IsFalse (transformProvider.CanMove,
			                "Panel can't be moved when Bottom.");
			panel.Dock = DockStyle.Top;
			Assert.IsFalse (transformProvider.CanMove,
			                "Panel can't be moved when Top.");
			panel.Dock = DockStyle.Left;
			Assert.IsFalse (transformProvider.CanMove,
			                "Panel can't be moved when Left.");
			panel.Dock = DockStyle.Right;
			Assert.IsFalse (transformProvider.CanMove,
			                "Panel can't be moved when Right.");
			panel.Dock = DockStyle.Fill;
			Assert.IsFalse (transformProvider.CanMove,
			                "Panel can't be moved when Fill.");
		}
		
		[Test]
		public void ITransformProviderCanResizeTest ()
		{
			Panel panel = new Panel ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (panel);
			
			ITransformProvider transformProvider = (ITransformProvider)
				provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (transformProvider,
			                  "Not returning TransformPatternIdentifiers.");
			
			// Default DockStyle is None
			Assert.IsTrue (transformProvider.CanResize,
			               "Panel can be resized by default.");
			panel.Dock = DockStyle.Bottom;
			Assert.IsTrue (transformProvider.CanResize,
			               "Panel can be resized when Bottom.");
			panel.Dock = DockStyle.Top;
			Assert.IsTrue (transformProvider.CanResize,
			               "Panel can be resized when Top.");
			panel.Dock = DockStyle.Left;
			Assert.IsTrue (transformProvider.CanResize,
			               "Panel can be resized when Left.");
			panel.Dock = DockStyle.Right;
			Assert.IsTrue (transformProvider.CanResize,
			               "Panel can be resized when Right.");
			panel.Dock = DockStyle.Fill;
			Assert.IsFalse (transformProvider.CanResize,
			                "Panel can't be resized when Fill.");
		}
		
		[Test]
		public void ITransformProviderCanRotateTest ()
		{
			Panel panel = new Panel ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (panel);
			
			ITransformProvider transformProvider = (ITransformProvider)
				provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (transformProvider,
			                  "Not returning TransformPatternIdentifiers.");
			
			Assert.IsFalse (transformProvider.CanRotate,
			                "Panel can't be rotated.");
		}
		
		[Test]
		public void ITransformProviderMoveTest ()
		{
			Panel panel = new Panel ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (panel);
			
			ITransformProvider transformProvider = (ITransformProvider)
				provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (transformProvider,
			                  "Not returning TransformPatternIdentifiers.");
			
			double x = 10, y = 10;
			transformProvider.Move (x, y);
			Assert.AreEqual (x, panel.Location.X, "X axis");
			Assert.AreEqual (y, panel.Location.Y, "Y axis");
			
			try {
				panel.Dock = DockStyle.Bottom;
				transformProvider.Move (x, y);
				Assert.Fail ("InvalidOperationException not thrown");
			} catch (InvalidOperationException) { }
			
			try {
				panel.Dock = DockStyle.Top;
				transformProvider.Move (x, y);
				Assert.Fail ("InvalidOperationException not thrown");
			} catch (InvalidOperationException) { }
			
			try {
				panel.Dock = DockStyle.Left;
				transformProvider.Move (x, y);
				Assert.Fail ("InvalidOperationException not thrown");
			} catch (InvalidOperationException) { }
			
			try {
				panel.Dock = DockStyle.Right;
				transformProvider.Move (x, y);
				Assert.Fail ("InvalidOperationException not thrown");
			} catch (InvalidOperationException) { }
			
			try {
				panel.Dock = DockStyle.Fill;
				transformProvider.Move (x, y);
				Assert.Fail ("InvalidOperationException not thrown");
			} catch (InvalidOperationException) { }
		}
		
		[Test]
		public void ITransformProviderResizeTest ()
		{
			Panel panel = new Panel ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (panel);
			
			ITransformProvider transformProvider = (ITransformProvider)
				provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (transformProvider,
			                  "Not returning TransformPatternIdentifiers.");
			
			double width = 50, heitht = 50;
			transformProvider.Resize (width, heitht);
			Assert.AreEqual (width, panel.Size.Width, "Width");
			Assert.AreEqual (heitht, panel.Size.Height, "Height");
			
			panel.Dock = DockStyle.Bottom;
			transformProvider.Resize (width, heitht);
			Assert.AreEqual (width, panel.Size.Width, "Width");
			Assert.AreEqual (heitht, panel.Size.Height, "Height");
			
			panel.Dock = DockStyle.Top;
			transformProvider.Resize (width, heitht);
			Assert.AreEqual (width, panel.Size.Width, "Width");
			Assert.AreEqual (heitht, panel.Size.Height, "Height");
			
			panel.Dock = DockStyle.Left;
			transformProvider.Resize (width, heitht);
			Assert.AreEqual (width, panel.Size.Width, "Width");
			Assert.AreEqual (heitht, panel.Size.Height, "Height");
			
			panel.Dock = DockStyle.Right;
			transformProvider.Resize (width, heitht);
			Assert.AreEqual (width, panel.Size.Width, "Width");
			Assert.AreEqual (heitht, panel.Size.Height, "Height");
			
			try {
				panel.Dock = DockStyle.Fill;
				transformProvider.Resize (width, heitht);
				Assert.Fail ("InvalidOperationException not thrown");
			} catch (InvalidOperationException) { }
		}
		
		[Test]
		public void ITransformProviderRotateTest ()
		{
			Panel panel = new Panel ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (panel);
			
			ITransformProvider transformProvider = (ITransformProvider)
				provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
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
			Panel panel = new Panel ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (panel);
			
			IDockProvider dockProvider = (IDockProvider)
				provider.GetPatternProvider (DockPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (dockProvider,
			                  "Not returning DockPatternIdentifiers.");
			
			Assert.AreEqual (panel.Dock,
			                 dockProvider.DockPosition,
			                 "Panel is None DockStyle by default.");
		}
		
		[Test]
		public void IDockProviderSetDockPositionTest ()
		{
			Panel panel = new Panel ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (panel);
			
			IDockProvider dockProvider = (IDockProvider)
				provider.GetPatternProvider (DockPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (dockProvider,
			                  "Not returning DockPatternIdentifiers.");
			
			dockProvider.SetDockPosition (DockPosition.Fill);
			Assert.AreEqual (panel.Dock,
			                 dockProvider.DockPosition,
			                 "Panel should be Fill DockStyle.");
		}
		
		#endregion
		
		#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return new Panel ();
		}
		
		#endregion
	}
}
