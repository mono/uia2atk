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
using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using System.Reflection;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class WindowProviderTest
	{
#region Private Fields
		
		private MockBridge bridge;
		
#endregion
		
#region Setup/Teardown
		
		[SetUp]
		public void SetUp ()
		{
			// Inject a mock automation bridge into the
			// AutomationInteropProvider, so that we don't try
			// to load the UiaAtkBridge.
			bridge = new MockBridge ();
			Type interopProviderType = typeof (AutomationInteropProvider);
			FieldInfo bridgeField =
				interopProviderType.GetField ("bridge", BindingFlags.NonPublic | BindingFlags.Static);
			bridgeField.SetValue (null, bridge);
			
			bridge.ClientsAreListening = true;
		}
		
		[TearDown]
		public void TearDown ()
		{
			Type interopProviderType = typeof (AutomationInteropProvider);
			FieldInfo bridgeField =
				interopProviderType.GetField ("bridge", BindingFlags.NonPublic | BindingFlags.Static);
			bridgeField.SetValue (null, null);
		}
		
#endregion
		
#region IWindowProvider Tests
		
		[Test]
		public void IsTopmostTest ()
		{
			using (Form f = new Form ()) {
				IWindowProvider provider = new WindowProvider (f);
				
				Assert.IsFalse (provider.IsTopmost, "Initialize to false");
				f.TopMost = true;
				Assert.IsTrue (provider.IsTopmost, "Set to true");
				f.TopMost = false;
				Assert.IsFalse (provider.IsTopmost, "Set to false");
			}
		}
		
		[Test]
		public void IsModalTest ()
		{
			using (Form f = new Form ()) {
				IWindowProvider provider = new WindowProvider (f);
				
				Assert.IsFalse (provider.IsModal, "Form should initialize to not modal");
				
				// Run modal dialog in separate thread
				Thread t = new Thread (new ParameterizedThreadStart (delegate {
					f.ShowDialog ();
				}));
				t.Start ();
				
				// Wait for dialog to appear
				Thread.Sleep (500); // TODO: Fragile
				
				Assert.IsTrue (provider.IsModal, "ShowDialog should be modal");
				
				f.Close ();
				t.Join ();
				
				f.Show ();
				// Wait for form to appear
				Thread.Sleep (500); // TODO: Fragile
				
				Assert.IsFalse (provider.IsModal, "Show should not be modal");
				f.Close ();
			}
		}
		
		[Test]
		public void CloseTest ()
		{
			using (Form f = new Form ()) {
				IWindowProvider provider = new WindowProvider (f);
				
				f.Show ();
				
				bool formClosed = false;
				f.Closed += delegate (Object sender, EventArgs e) {
					formClosed = true;
				};
				
				bridge.ResetEventLists ();
				provider.Close ();
				
				Assert.IsTrue (formClosed, "Form closed event didn't fire.");
				
				Assert.AreEqual (1, bridge.StructureChangedEvents.Count, "event count");
				Assert.AreSame (provider, bridge.StructureChangedEvents [0].provider, "event provider");
				Assert.AreEqual (StructureChangeType.ChildrenBulkRemoved, bridge.StructureChangedEvents [0].e.StructureChangeType, "event change type");
				
				Application.DoEvents ();
			}
		}
		
		[Test]
		public void SetVisualStateTest ()
		{
			using (Form f = new Form ()) {
				IWindowProvider provider = new WindowProvider (f);
				
				//f.Show ();
				//Application.DoEvents ();
					
				Assert.AreEqual (FormWindowState.Normal, f.WindowState, "Form should initially be 'normal'");
				
				provider.SetVisualState (WindowVisualState.Maximized);
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (FormWindowState.Maximized, f.WindowState, "Form should maximize");
				
				provider.SetVisualState (WindowVisualState.Minimized);
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (FormWindowState.Minimized, f.WindowState, "Form should minimize");
				
				provider.SetVisualState (WindowVisualState.Normal);
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
				IWindowProvider provider = new WindowProvider (f);
				
				//f.Show ();
				//Application.DoEvents ();
				
				Assert.AreEqual (WindowVisualState.Normal, provider.VisualState, "Provider should initially be 'normal'");
				
				f.WindowState = FormWindowState.Maximized;
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (WindowVisualState.Maximized, provider.VisualState, "Provider should maximize");
				
				f.WindowState = FormWindowState.Minimized;
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (WindowVisualState.Minimized, provider.VisualState, "Provider should minimize");
				
				f.WindowState = FormWindowState.Normal;
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (WindowVisualState.Normal, provider.VisualState, "Provider should return to 'normal'");
			}
		}
		
#endregion
		
#region IRawElementProviderFragmentRoot Tests
		
		[Test]
		[Ignore ("Not implemented")]
		public void HostRawElementProviderTest ()
		{
			;
		}
		
		[Test]
		public void ProviderOptionsTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					new WindowProvider (f);
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
					new WindowProvider (f);
				Assert.AreEqual (provider,
				                 provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id));
				// TODO: Test null return on other IDs?
				//       Testing every other int takes too long.
			}
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void GetPropertyValueTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					new WindowProvider (f);
			}
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void BoundingRectangleTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					new WindowProvider (f);
			}
		}
		
		[Test]
		public void FragmentRootTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					new WindowProvider (f);
				Assert.AreEqual (provider,
				                 provider.FragmentRoot);
			}
		}
		
		[Test]
		public void GetEmbeddedFragmentRootsTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					new WindowProvider (f);
				Assert.IsNull (provider.GetEmbeddedFragmentRoots ());
			}
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void GetRuntimeIdTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					new WindowProvider (f);
			}
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void NavigateTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					new WindowProvider (f);
			}
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void SetFocusTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					new WindowProvider (f);
			}
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void ElementProviderFromPointTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					new WindowProvider (f);
			}
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void GetFocusTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					new WindowProvider (f);
			}
		}
		
#endregion
		
	}

	public class MockBridge : IAutomationBridge
	{
#region Tuple Classes
		public class AutomationEventTuple
		{
			public AutomationEvent eventId;
			public object provider;
			public AutomationEventArgs e;
		}
		
		public class AutomationPropertyChangedEventTuple
		{
			public object element;
			public AutomationPropertyChangedEventArgs e;
		}
		
		public class StructureChangedEventTuple
		{
			public object provider;
			public StructureChangedEventArgs e;
		}
#endregion
		
#region Public Members
		public List<AutomationEventTuple> AutomationEvents =
			new List<AutomationEventTuple> ();
		public List<AutomationPropertyChangedEventTuple> AutomationPropertyChangedEvents =
			new List<AutomationPropertyChangedEventTuple> ();
		public List<StructureChangedEventTuple> StructureChangedEvents =
			new List<StructureChangedEventTuple> ();
		
		public void ResetEventLists ()
		{
			AutomationEvents.Clear ();
			AutomationPropertyChangedEvents.Clear ();
			StructureChangedEvents.Clear ();
		}
#endregion
	
#region IAutomationBridge Members
		public bool ClientsAreListening { get; set; }
		
		public object HostProviderFromHandle (IntPtr hwnd)
		{
			throw new NotImplementedException ();
		}

		
		public void RaiseAutomationEvent (AutomationEvent eventId, object provider, AutomationEventArgs e)
		{			
			AutomationEvents.Add (new AutomationEventTuple {
				eventId = eventId, provider = provider, e = e});
		}

		public void RaiseAutomationPropertyChangedEvent (object element, AutomationPropertyChangedEventArgs e)
		{
			AutomationPropertyChangedEvents.Add (new AutomationPropertyChangedEventTuple {
				element = element, e = e});
		}

		public void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			StructureChangedEvents.Add (new StructureChangedEventTuple {
				provider = provider, e = e});
		}
#endregion
	}
}
