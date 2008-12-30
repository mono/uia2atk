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
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Bridge;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class WindowProviderTest : BaseProviderTest
	{
		
#region IWindowProvider Tests
		
		[Test]
		public void MinimizableTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment) ProviderFactory.GetProvider (f);
				IWindowProvider pattern = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				
				Assert.IsTrue (pattern.Minimizable, "Initialize to false");
				f.MinimizeBox = false;
				Assert.IsFalse (pattern.Minimizable, "Set to true");
				f.MinimizeBox = true;
				Assert.IsTrue (pattern.Minimizable, "Set to false");
			}
		}
		
		[Test]
		public void MaximizableTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment) ProviderFactory.GetProvider (f);
				IWindowProvider pattern = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				
				Assert.IsTrue (pattern.Maximizable, "Initialize to false");
				f.MaximizeBox = false;
				Assert.IsFalse (pattern.Maximizable, "Set to true");
				f.MaximizeBox = true;
				Assert.IsTrue (pattern.Maximizable, "Set to false");
			}
		}
		
		[Test]
		public void IsTopmostTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment) ProviderFactory.GetProvider (f);
				IWindowProvider pattern = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				
				Assert.IsFalse (pattern.IsTopmost, "Initialize to false");
				f.TopMost = true;
				Assert.IsTrue (pattern.IsTopmost, "Set to true");
				f.TopMost = false;
				Assert.IsFalse (pattern.IsTopmost, "Set to false");
			}
		}
		
		[Test]
		public void IsModalTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment) ProviderFactory.GetProvider (f);
				IWindowProvider pattern = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				
				Assert.IsFalse (pattern.IsModal, "Form should initialize to not modal");
				
				// Run modal dialog in separate thread
				Thread t = new Thread (new ParameterizedThreadStart (delegate {
					f.ShowDialog ();
				}));
				t.Start ();
				
				// Wait for dialog to appear
				Thread.Sleep (500); // TODO: Fragile
				
				Assert.IsTrue (pattern.IsModal, "ShowDialog should be modal");
				
				f.Close ();
				t.Join ();
				
				f.Show ();
				// Wait for form to appear
				Thread.Sleep (500); // TODO: Fragile
				
				Assert.IsFalse (pattern.IsModal, "Show should not be modal");
				f.Close ();
			}
		}
		
		[Test]
		public void CloseTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment) ProviderFactory.GetProvider (f);
				IWindowProvider pattern = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				
				f.Show ();
				
				Assert.AreEqual (WindowInteractionState.Running,
				                 pattern.InteractionState,
				                 "Interaction state while running normally");
				
				bool formClosed = false;
				bool formClosingChecked = false;
				f.Closed += delegate (Object sender, EventArgs e) {
					formClosed = true;
				};
				f.Closing += delegate (Object sender, CancelEventArgs e) {
					Assert.AreEqual (WindowInteractionState.Closing,
					                 pattern.InteractionState,
					                 "Interaction state while closing");
					formClosingChecked = true;
				};
				
				bridge.ResetEventLists ();
				pattern.Close ();
				
				Assert.IsTrue (formClosed, "Form closed event didn't fire.");
				Assert.IsTrue (formClosingChecked, "Interaction state while closing never confirmed.");
				
				Assert.AreEqual (1, bridge.StructureChangedEvents.Count, "event count");
				Assert.AreSame (provider, bridge.StructureChangedEvents [0].provider, "event provider");
				Assert.AreEqual (StructureChangeType.ChildRemoved, bridge.StructureChangedEvents [0].e.StructureChangeType, "event change type");
				
				Application.DoEvents ();
			}
		}
		
		[Test]
		public void SetVisualStateTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment) ProviderFactory.GetProvider (f);
				IWindowProvider pattern = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				
				//f.Show ();
				//Application.DoEvents ();
					
				Assert.AreEqual (FormWindowState.Normal, f.WindowState, "Form should initially be 'normal'");
				
				pattern.SetVisualState (WindowVisualState.Maximized);
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (FormWindowState.Maximized, f.WindowState, "Form should maximize");
				
				pattern.SetVisualState (WindowVisualState.Minimized);
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (FormWindowState.Minimized, f.WindowState, "Form should minimize");
				
				pattern.SetVisualState (WindowVisualState.Normal);
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (FormWindowState.Normal, f.WindowState, "Form should return to 'normal'");
			}
		}
		
		[Test]
		public void VisualStateTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment) ProviderFactory.GetProvider (f);
				IWindowProvider pattern = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				
				//f.Show ();
				//Application.DoEvents ();
				
				Assert.AreEqual (WindowVisualState.Normal, pattern.VisualState, "Provider should initially be 'normal'");
				
				f.WindowState = FormWindowState.Maximized;
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (WindowVisualState.Maximized, pattern.VisualState, "Provider should maximize");
				
				f.WindowState = FormWindowState.Minimized;
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (WindowVisualState.Minimized, pattern.VisualState, "Provider should minimize");
				
				f.WindowState = FormWindowState.Normal;
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (WindowVisualState.Normal, pattern.VisualState, "Provider should return to 'normal'");
			}
		}
		
#endregion
		
#region ITransformProvider Tests

		[Test]
		public void CanMoveTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment)
					ProviderFactory.GetProvider (f);
				ITransformProvider transform = (ITransformProvider)
					provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
				
				//f.Show ();
				
				Assert.IsTrue (transform.CanMove,
				               "True by default");
				f.WindowState = FormWindowState.Maximized;
				Assert.IsFalse (transform.CanMove,
				                "Maximized");
				f.WindowState = FormWindowState.Minimized;
				Assert.IsFalse (transform.CanMove,
				                "Minimized");
				f.WindowState = FormWindowState.Normal;
				Assert.IsTrue (transform.CanMove,
				               "Normal");
			}
		}

		[Test]
		public void CanResizeTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment)
					ProviderFactory.GetProvider (f);
				ITransformProvider transform = (ITransformProvider)
					provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
				
				f.Show ();
				
				Assert.IsTrue (transform.CanResize,
				               "True by default");
				
				f.FormBorderStyle = FormBorderStyle.Fixed3D;
				Assert.IsFalse (transform.CanResize,
				                "FormBorderStyle.Fixed3D");
				f.FormBorderStyle = FormBorderStyle.FixedDialog;
				Assert.IsFalse (transform.CanResize,
				                "FormBorderStyle.FixedDialog");
				f.FormBorderStyle = FormBorderStyle.FixedSingle;
				Assert.IsFalse (transform.CanResize,
				                "FormBorderStyle.FixedSingle");
				f.FormBorderStyle = FormBorderStyle.FixedToolWindow;
				Assert.IsFalse (transform.CanResize,
				                "FormBorderStyle.FixedToolWindow");
				
				f.FormBorderStyle = FormBorderStyle.None;
				Assert.IsTrue (transform.CanResize,
				                "FormBorderStyle.None");
				f.FormBorderStyle = FormBorderStyle.Sizable;
				Assert.IsTrue (transform.CanResize,
				                "FormBorderStyle.Sizable");
				f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
				Assert.IsTrue (transform.CanResize,
				                "FormBorderStyle.SizableToolWindow");
			}
		}

		[Test]
		public void CanRotateTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment)
					ProviderFactory.GetProvider (f);
				ITransformProvider transform = (ITransformProvider)
					provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
				
				Assert.IsFalse (transform.CanRotate,
				                "Should always be false");
			}
		}

		[Test]
		public void MoveTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment)
					ProviderFactory.GetProvider (f);
				ITransformProvider transform = (ITransformProvider)
					provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
				
				//f.Show ();
				
				transform.Move (15, 20);
				Assert.AreEqual (15, f.Location.X, "X, default form");
				Assert.AreEqual (20, f.Location.Y, "Y, default form");
				
				f.WindowState = FormWindowState.Maximized;
				VerifyMoveFail (f, transform, FormWindowState.Maximized);
				f.WindowState = FormWindowState.Minimized;
				VerifyMoveFail (f, transform, FormWindowState.Minimized);
				
				f.WindowState = FormWindowState.Normal;
				
				transform.Move (150, 100);
				Assert.AreEqual (150, f.Location.X, "X, normal form");
				Assert.AreEqual (100, f.Location.Y, "Y, normal form");
			}
		}
		
		private void VerifyMoveFail (Form f, ITransformProvider transform, FormWindowState state)
		{
			f.WindowState = state;
			try {
				transform.Move (10, 10);
				Assert.Fail ("Expected InvalidOperationException");
			} catch (InvalidOperationException) {
				// Expected
			} catch (Exception e){
				Assert.Fail ("Expected InvalidOperationException, instead got this exception: " + e.Message);
			}
		}

		[Test]
		public void ResizeTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment)
					ProviderFactory.GetProvider (f);
				ITransformProvider transform = (ITransformProvider)
					provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
				
				f.Show ();
				
				VerifyResizeFail (f, transform, FormBorderStyle.Fixed3D);
				VerifyResizeFail (f, transform, FormBorderStyle.FixedDialog);
				VerifyResizeFail (f, transform, FormBorderStyle.FixedSingle);
				VerifyResizeFail (f, transform, FormBorderStyle.FixedToolWindow);
				
				f.FormBorderStyle = FormBorderStyle.None;
				transform.Resize (100, 200);
				Assert.AreEqual (100, f.Width, "Width");
				Assert.AreEqual (200, f.Height, "Height");
				
				// Problematic...getting 110 for width!
				/*f.FormBorderStyle = FormBorderStyle.Sizable;
				transform.Resize (35.7, 10);
				Assert.AreEqual (36, f.Width, "Width");
				Assert.AreEqual (10, f.Height, "Height");*/
				
				f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
				transform.Resize (1234, 500.2);
				Assert.AreEqual (1234, f.Width, "Width");
				Assert.AreEqual (500, f.Height, "Height");
			}
		}
		
		private void VerifyResizeFail (Form f, ITransformProvider transform, FormBorderStyle style)
		{
			f.FormBorderStyle = style;
			try {
				transform.Resize (10, 10);
				Assert.Fail ("Expected InvalidOperationException");
			} catch (InvalidOperationException) {
				// Expected
			} catch (Exception e){
				Assert.Fail ("Expected InvalidOperationException, instead got this exception: " + e.Message);
			}
		}

		[Test]
		public void RotateTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment)
					ProviderFactory.GetProvider (f);
				ITransformProvider transform = (ITransformProvider)
					provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
				
				try {
					transform.Rotate (1);
					Assert.Fail ("Expected InvalidOperationException");
				} catch (InvalidOperationException) {
					// Expected
				} catch (Exception e){
					Assert.Fail ("Expected InvalidOperationException, instead got this exception: " + e.Message);
				}
			}
		}
#endregion
		
#region IRawElementProviderFragmentRoot Tests
		
		[Test]
		public void HostRawElementProviderTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);

				Assert.IsNotNull (provider.HostRawElementProvider,
				                  "HostRawElementProvider is null");

				Button b = new Button ();
				f.Controls.Add (b);
				f.Show ();
				
				IRawElementProviderSimple buttonProvider = ProviderFactory.GetProvider (b);
				Assert.AreEqual (provider, buttonProvider.HostRawElementProvider,
				                 "Button's HostRawElementProvider isn't the form");
			}
		}
		
		[Test]
		public void ProviderOptionsTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
				Assert.AreEqual (ProviderOptions.ServerSideProvider,
				                 provider.ProviderOptions,
				                 "ProviderOptions");
			}
		}
		
		[Test]
		public void GetPatternProviderTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
			
				object window =
					provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				Assert.IsNotNull (window);
				Assert.IsTrue (window is IWindowProvider,
				               "IWindowProvider");
				
				object transform =
					provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
				Assert.IsNotNull (transform);
				Assert.IsTrue (transform is ITransformProvider,
				               "ITransformProvider");
			}
		}
		
		[Test]
		public void GetPropertyValueTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);

				TestProperty (provider,
					      AutomationElementIdentifiers.ControlTypeProperty,
					      ControlType.Window.Id);
				
				TestProperty (provider,
					      AutomationElementIdentifiers.LocalizedControlTypeProperty,
					      "window");
			}
		}
		
		[Test]
		public void BoundingRectangleTest ()
		{
			using (Form f = new Form ()) {
				// XXX: Weird behaviors happen when the window
				// is resized less than 110x110.  Investigate
				// this.

				f.Location = new System.Drawing.Point (0, 0);
				f.Size = new System.Drawing.Size (200, 200);
				
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);

				Assert.AreEqual (0, provider.BoundingRectangle.X);
				Assert.AreEqual (0, provider.BoundingRectangle.Y);
				Assert.AreEqual (200, provider.BoundingRectangle.Width);
				Assert.AreEqual (200, provider.BoundingRectangle.Height);

				f.Location = new System.Drawing.Point (15, 20);
				f.Size = new System.Drawing.Size (200, 300);

				Assert.AreEqual (15, provider.BoundingRectangle.X);
				Assert.AreEqual (20, provider.BoundingRectangle.Y);
				Assert.AreEqual (200, provider.BoundingRectangle.Width);
				Assert.AreEqual (300, provider.BoundingRectangle.Height);
			}
		}
		
		[Test]
		public void FragmentRootTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
				Assert.AreEqual (provider,
				                 provider.FragmentRoot);
			}
		}
		
		[Test]
		public void GetEmbeddedFragmentRootsTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
				Assert.IsNull (provider.GetEmbeddedFragmentRoots ());
			}
		}
		
		[Test]
		public void GetRuntimeIdTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);

				int[] runtimeId = provider.GetRuntimeId ();
				Assert.IsNotNull (runtimeId, "Runtime ID is null");
				Assert.AreEqual (2, runtimeId.Length, "Runtime ID list is not the right size");

				Assert.AreEqual (AutomationInteropProvider.AppendRuntimeId,
				                 runtimeId[0], "First item in runtime ID is not the AppendRuntimeId");

				int automation_id = (int) provider.GetPropertyValue (
					AutomationElementIdentifiers.AutomationIdProperty.Id);
				Assert.AreEqual (automation_id, runtimeId[1],
				                 "Second item in runtime ID is not the same as the control's AutomationId property");
			}
		}
		
		[Test]
		public void NavigateTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);

				Button b = new Button ();
				b.Location = new System.Drawing.Point (50, 50);

				Label l = new Label ();
				l.Location = new System.Drawing.Point (0, 50);

				f.Controls.Add (b);
				f.Controls.Add (l);
				f.Show ();
				
				IRawElementProviderSimple buttonProvider = ProviderFactory.GetProvider (b);
				Assert.AreEqual (buttonProvider, provider.Navigate (NavigateDirection.FirstChild),
				                 "FirstChild navigation is incorrect");
				
				IRawElementProviderSimple labelProvider = ProviderFactory.GetProvider (l);
				Assert.AreEqual (labelProvider, provider.Navigate (NavigateDirection.LastChild),
				                 "LastChild navigation is incorrect");

				using (Form f2 = new Form ()) {
					f2.Show ();

					// SPEC: Fragment roots do not enable
					// navigation to a parent or siblings;
					// navigation among fragment roots is
					// handled by the default window
					// providers.

					/* TODO: I'm not sure how to implement
					 * the spec here.  Should "do not enable" = return null?

					Assert.IsNull (provider.Navigate (NavigateDirection.Parent),
						       "Form is not returning null for parent");
					*/

					Assert.IsNull (provider.Navigate (NavigateDirection.NextSibling),
						       "Form is not returning null for next sibling");

					Assert.IsNull (provider.Navigate (NavigateDirection.PreviousSibling),
						       "Form is not returning null for previous sibling");
				}
			}
		}
		
		[Test]
		public void SetFocusTest ()
		{
			using (Form f1 = new Form ()) {
				using (Form f2 = new Form ()) {
					IRawElementProviderFragmentRoot p1 =
						(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f1);
					IRawElementProviderFragmentRoot p2 =
						(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f2);

					f1.Show ();
					f2.Show ();

					p1.SetFocus ();
					Assert.IsTrue (f1.Focused, "Form #1 is not focused after SetFocus ()");
					Assert.IsFalse(f2.Focused, "Form #2 is focused after Form #2 SetFocus ()");

					p2.SetFocus ();
					Assert.IsFalse (f1.Focused, "Form #1 is focused after Form #2 SetFocus ()");
					Assert.IsTrue (f2.Focused, "Form #2 is not focused after SetFocus ()");
				}
			}
		}
		
		[Test]
		public void ElementProviderFromPointTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);

				Button b = new Button ();
				b.Location = new System.Drawing.Point (50, 50);

				Label l = new Label ();
				l.Location = new System.Drawing.Point (0, 50);

				f.Controls.Add (b);
				f.Controls.Add (l);
				f.Show ();

				Assert.AreEqual (provider, provider.ElementProviderFromPoint (5, 5),
				                 "ElementProviderFromPoint not returning form control for (5, 5)");
				
				IRawElementProviderSimple buttonProvider = ProviderFactory.GetProvider (b);
				Assert.AreEqual (buttonProvider, provider.ElementProviderFromPoint (55, 55),
				                 "ElementProviderFromPoint not returning button control for (55, 55)");

				IRawElementProviderSimple labelProvider = ProviderFactory.GetProvider (l);
				Assert.AreEqual (labelProvider, provider.ElementProviderFromPoint (7, 60),
				                 "ElementProviderFromPoint not returning label control for (7, 60)");
			}
		}
		
		[Test]
		public void GetFocusTest ()
		{
			using (Form f = new Form ()) {
				f.Show ();

				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);

				Assert.IsNull (provider.GetFocus (),
				               "Nothing is selected and GetFocus is returning something");

				Button b1 = new Button ();
				b1.Location = new System.Drawing.Point (0, 0);
				f.Controls.Add (b1);

				Button b2 = new Button ();
				b2.Location = new System.Drawing.Point (50, 50);
				f.Controls.Add (b2);

				IRawElementProviderSimple button1Provider =
					ProviderFactory.GetProvider (b1);

				b1.Focus ();
				Assert.AreEqual (button1Provider, provider.GetFocus (),
				                 "GetFocus is not returning the first button");

				IRawElementProviderSimple button2Provider =
					ProviderFactory.GetProvider (b2);

				b2.Focus ();
				Assert.AreEqual (button2Provider, provider.GetFocus (),
				                 "GetFocus is not returning the second button");
			}
		}

#endregion

#region IDockProvider Tests
		
		[Test]
		public void DockProviderTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderSimple provider =
					ProviderFactory.GetProvider (f);

				IDockProvider dockProvider
					= provider.GetPatternProvider (DockPatternIdentifiers.Pattern.Id)
					as IDockProvider;
				
				// Conditional -> No
				Assert.IsNull (dockProvider, "Implements IDockProvider");
			}
		}

#endregion
		
#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return new Form ();
		}
		
#endregion
		
	}
}
