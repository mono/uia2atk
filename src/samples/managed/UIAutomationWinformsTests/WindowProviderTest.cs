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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using System.Reflection;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	public class MockBridge : IAutomationBridge
	{
		public bool ClientsAreListening {
			get { return false; }
		}
		
		public void RaiseAutomationEvent (AutomationEvent eventId, object provider, AutomationEventArgs e)
		{
			return;
		}

		public void RaiseAutomationPropertyChangedEvent (object element, AutomationPropertyChangedEventArgs e)
		{
			return;
		}

		public void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			return;
		}

	}
	
	[TestFixture]
	public class WindowProviderTest
	{
		private MockBridge bridge;
		
		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			// Inject a mock automation bridge into the
			// AutomationInteropProvider, so that we don't try
			// to load the UiaAtkBridge.
			bridge = new MockBridge ();
			Type interopProviderType = typeof (AutomationInteropProvider);
			FieldInfo bridgeField =
				interopProviderType.GetField ("bridge", BindingFlags.NonPublic | BindingFlags.Static);
			bridgeField.SetValue (null, bridge);
		}
		
		[Test]
		public void IsTopmostTest ()
		{
			Form f = new Form ();
			IWindowProvider provider = new WindowProvider (f);
			
			Assert.IsFalse (provider.IsTopmost, "IsTopmost");
			f.TopMost = true;
			Assert.IsTrue (provider.IsTopmost, "IsTopmost");
		}
		
		[Test]
		public void IsModalTest ()
		{
			Form f = new Form ();
			IWindowProvider provider = new WindowProvider (f);
			
			Assert.IsFalse (provider.IsModal);
			//f.Modal = true;
			//Assert.IsTrue (provider.IsModal);
		}
		
		[Test]
		public void CloseTest ()
		{
			Form f = new Form ();
			IWindowProvider provider = new WindowProvider (f);
			
			f.Show ();
			
			bool formClosed = false;
			f.Closed += delegate (Object sender, EventArgs e) {
				formClosed = true;
			};
			
			provider.Close ();
			
			Assert.IsTrue (formClosed, "Form closed event didn't fire.");
		}
	}

}
