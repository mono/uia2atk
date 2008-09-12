//// Permission is hereby granted, free of charge, to any person obtaining 
//// a copy of this software and associated documentation files (the 
//// "Software"), to deal in the Software without restriction, including 
//// without limitation the rights to use, copy, modify, merge, publish, 
//// distribute, sublicense, and/or sell copies of the Software, and to 
//// permit persons to whom the Software is furnished to do so, subject to 
//// the following conditions: 
////  
//// The above copyright notice and this permission notice shall be 
//// included in all copies or substantial portions of the Software. 
////  
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
//// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
//// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
//// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
//// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
//// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//// 
//// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
//// 
//// Authors: 
////      Sandy Armstrong <sanfordarmstrong@gmail.com>
//// 
//

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using NUnit.Framework;
using Mono.UIAutomation.Winforms;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	public abstract class BaseProviderTest
	{
	
		#region Protected Fields
		
		protected MockBridge bridge;
		
		#endregion
		
		#region Private Fields
		
		private Form form;
		private IRawElementProviderFragmentRoot windowProvider;
		
		#endregion
		
		#region Setup/Teardown
		
		[SetUp]
		public virtual void SetUp ()
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
			
			form = new Form ();
		}
		
		[TearDown]
		public virtual void TearDown ()
		{
			Type interopProviderType = typeof (AutomationInteropProvider);
			FieldInfo bridgeField =
				interopProviderType.GetField ("bridge", BindingFlags.NonPublic | BindingFlags.Static);
			bridgeField.SetValue (null, null);
			
			form.Dispose ();
			windowProvider = null;
		}

		#endregion
		
		#region Basic Tests
		
		[Test]
		public virtual void IsEnabledPropertyTest ()
		{
			Control control = GetControlInstance ();
			if (control == null)
				return;
			
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (control);
			
			bridge.ResetEventLists ();
			
			object initialVal =
				provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			Assert.IsNotNull (initialVal, "Property returns null");
			Assert.IsTrue ((bool)initialVal, "Should initialize to true");
			
			control.Enabled = false;

			Assert.IsFalse ((bool)provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id),
			                "Toggle to false");
			
			Assert.AreEqual (1,
			                 bridge.AutomationPropertyChangedEvents.Count,
			                 "event count");
			AutomationPropertyChangedEventTuple eventTuple =
				bridge.AutomationPropertyChangedEvents [0];
			Assert.AreEqual (provider,
			                 eventTuple.element,
			                 "event element");
			Assert.AreEqual (AutomationElementIdentifiers.IsEnabledProperty,
			                 eventTuple.e.Property,
			                 "event property");
			Assert.AreEqual (initialVal,
			                 eventTuple.e.OldValue,
			                 "Old value when disabled");
			Assert.AreEqual (false,
			                 eventTuple.e.NewValue,
			                 "New value when disabled");
			
			control.Enabled = true;
			Assert.IsTrue ((bool)provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id),
			               "Toggle to true");
		}
		
		[Test]
		[Ignore ("This test doesn't work anymore")]
		public virtual void AutomationIdPropertyTest ()
		{
			Control control = GetControlInstance ();
			if (control == null)
				return;

			IRawElementProviderSimple provider = ProviderFactory.GetProvider (control);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.AutomationIdProperty,
			              control.GetHashCode ());
		}
		
		[Test]
		[Ignore ("Not working yet")]
		public virtual void BoundingRectanglePropertyTest ()
		{
			Control control = GetControlInstance ();
			if (control == null)
				return;
			
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (control);
			
			Form f = control as Form;
			int xOffset = 0, yOffset = 0;
			if (f == null) {
				f = new Form ();
				f.Controls.Add (control);
			}
			
			try {
				f.Show ();
				f.Location = new System.Drawing.Point (0, 0);
				xOffset = f.Location.X;
				yOffset = f.Location.Y;
				
				control.SetBounds (5, 6, 7, 8);
				System.Drawing.Rectangle screenRect =
					f.RectangleToScreen (control.Bounds);
				Rect rect = new Rect (screenRect.X,
				                      screenRect.Y,
				                      7,
				                      8);
				
				TestProperty (provider,
				              AutomationElementIdentifiers.BoundingRectangleProperty,
				              rect);
			} finally {
				if (f != null)
					f.Dispose ();
			}
		}
		
		[Test]
		public virtual void LabeledByAndNamePropertyTest ()
		{
			TestLabeledByAndName (true, true);
		}
		
		[Test]
		[Ignore ("Not implemented yet")]
		public virtual void ClickablePointPropertyTest ()
		{
			
		}
		
		[Test]
		[Ignore ("Need to test false case")]
		public virtual void IsKeyboardFocusablePropertyTest ()
		{
			Control control = GetControlInstance ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (control);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              control.CanFocus);
		}
		
		#endregion
		
		#region Abstract Members
	
		protected abstract Control GetControlInstance ();

		#endregion
	
		#region Protected Helper Methods
		
		protected void TestProperty (IRawElementProviderSimple provider,
		                           AutomationProperty property,
		                           object expectedValue)
		{
			Assert.AreEqual (expectedValue,
			                 provider.GetPropertyValue (property.Id),
			                 property.ProgrammaticName);
		}
		
		protected IRawElementProviderFragment WindowProvider {
			get { return windowProvider; }
		}
		
		protected Form Form {
			get { return form; }
		}
		
		protected IRawElementProviderFragment GetProviderFromControl (Control control)
		{
			form.Controls.Add (control);
			form.Size = new System.Drawing.Size (400, 400);
			form.Show ();
			
			windowProvider = (IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (form);
			
			return windowProvider.Navigate (NavigateDirection.FirstChild);
		}

		protected void TestLabeledByAndName (bool expectNonNull, bool expectNameFromLabel)
		{
			Control control = GetControlInstance ();
			if (control == null)
				return;
			
			Form f = control as Form;
			if (f != null)
				return;
			
			using (f = new Form ()) {
				f.Controls.Add (control);
				
				Label l = new Label ();
				l.Text = "my label";
				f.Controls.Add (l);
				
				f.Show ();
			
				Type formListenerType = typeof (FormListener);
				MethodInfo onFormAddedMethod =
					formListenerType.GetMethod ("OnFormAdded", BindingFlags.Static | BindingFlags.NonPublic);
				onFormAddedMethod.Invoke (null, new object [] {f, null});
				
				IRawElementProviderSimple controlProvider =
					ProviderFactory.GetProvider (control);
				IRawElementProviderSimple labelProvider =
					ProviderFactory.GetProvider (l);
				
				object labeledBy = controlProvider.GetPropertyValue (AutomationElementIdentifiers.LabeledByProperty.Id);
				Assert.AreEqual (expectNonNull ? labelProvider : null,
				                 labeledBy);

				if (expectNonNull && expectNameFromLabel)
					Assert.AreEqual (labelProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id) as string,
					                 controlProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id) as string,
					                 "Control name should derive from label name.");
				else if (expectNonNull && !expectNameFromLabel)
					Assert.IsTrue (labelProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id) as string ==
					               controlProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id) as string,
					               "Control naame should not derive from label name.");
				
				f.Close ();
			}
		}
		
		#endregion
		
	}
	
}
