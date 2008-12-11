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
////      Mario Carrion <mcarrion@novell.com>
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
using Mono.UIAutomation.Winforms.Navigation;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	public abstract class BaseProviderTest
	{
	
		#region Protected Fields
		
		protected MockBridge bridge;
		
		#endregion
		
		#region Private Fields
		
		private Form form;
		private FormProvider formProvider;
		
		#endregion
		
		#region Setup/Teardown

		[SetUp]
		public virtual void SetUp ()
		{			
			bridge = TestHelper.SetUpEnvironment ();
			form = new Form ();
			form.Show ();
			formProvider = (FormProvider) ProviderFactory.GetProvider (form);
		}
		
		[TearDown]
		public virtual void TearDown ()
		{
			TestHelper.TearDownEnvironment ();
			
			form.Close ();
			formProvider = null;
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

			AutomationPropertyChangedEventTuple tuple 
				= bridge.GetAutomationPropertyEventFrom (provider,
				                                         AutomationElementIdentifiers.IsEnabledProperty.Id);
			
			Assert.IsNotNull (tuple, "Tuple missing");
			Assert.AreEqual (initialVal,
			                 tuple.e.OldValue,
			                 string.Format ("1st. Old value should be true: '{0}'", tuple.e.OldValue));
			Assert.AreEqual (false,
			                 tuple.e.NewValue,
			                 string.Format ("1st. New value should be true: '{0}'", tuple.e.NewValue));
			
			bridge.ResetEventLists ();
			
			control.Enabled = true;
			Assert.IsTrue ((bool)provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id),
			               "Toggle to true");
			
			tuple 
				= bridge.GetAutomationPropertyEventFrom (provider,
				                                         AutomationElementIdentifiers.IsEnabledProperty.Id);
			Assert.IsNotNull (tuple, "Tuple missing");
			Assert.AreEqual (false,
			                 tuple.e.OldValue,
			                 string.Format ("2nd. Old value should be false: '{0}'", tuple.e.OldValue));
			Assert.AreEqual (true,
			                 tuple.e.NewValue,
			                 string.Format ("2nd. New value should be true: '{0}'", tuple.e.NewValue));
		}

		protected virtual bool IsContentElement {
			get { return true; }
		}

		[Test]
		public virtual void IsContentElementPropertyTest ()
		{
			Control control = GetControlInstance ();
			if (control == null)
				return;

			IRawElementProviderSimple provider = ProviderFactory.GetProvider (control);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsContentElementProperty,
			              IsContentElement);
		}

		protected virtual bool IsControlElement {
			get { return true; }
		}

		[Test]
		public virtual void IsControlElementPropertyTest ()
		{
			Control control = GetControlInstance ();
			if (control == null)
				return;

			IRawElementProviderSimple provider = ProviderFactory.GetProvider (control);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsControlElementProperty,
			              IsControlElement);
		}
		
		[Test]
		public virtual void AutomationIdPropertyTest ()
		{
			Control control = GetControlInstance ();
			if (control == null)
				return;

			IRawElementProviderSimple provider = ProviderFactory.GetProvider (control);

			Assert.IsNotNull (provider.GetPropertyValue (AutomationElementIdentifiers.AutomationIdProperty.Id),
			                  "AutomationIdProperty should not be null.");
		}
		
		[Test]
		public virtual void BoundingRectanglePropertyTest ()
		{
			Control control = GetControlInstance ();
			if (control == null)
				return;

			IRawElementProviderSimple provider = GetProviderFromControl (control);
			
			try {
				Form.Show ();
				Form.Location = new System.Drawing.Point (0, 0);
				
				control.SetBounds (5, 6, 7, 8);
				System.Drawing.Rectangle screenRect =
					Form.RectangleToScreen (control.Bounds);
				Rect rect = new Rect (screenRect.X,
				                      screenRect.Y,
				                      7,
				                      8);
				
				TestProperty (provider,
				              AutomationElementIdentifiers.BoundingRectangleProperty,
				              rect);
			} catch (Exception) {}
		}
		
		[Test]
		public virtual void LabeledByAndNamePropertyTest ()
		{
			TestLabeledByAndName (true, true);
		}
		
		[Test]
		public virtual void ClickablePointPropertyTest ()
		{
			IRawElementProviderSimple provider = GetProvider ();

			bool offscreen 
				= (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsOffscreenProperty.Id);
			object clickablePointObj 
				= provider.GetPropertyValue (AutomationElementIdentifiers.ClickablePointProperty.Id);

			Point clickablePoint = new Point (0, 0);

			// Clickable point should be either null or Rect.Empty
			if (offscreen) {
				if (clickablePointObj != null) {
					try {
						clickablePoint = (Point) clickablePointObj;
						Assert.IsTrue (clickablePoint.X >= 0,
						               string.Format ("X is negative, your provider should be OffScreen: {0}", clickablePoint.X));
						Assert.IsTrue (clickablePoint.Y >= 0,
						               string.Format ("Y is negative, your provider should be OffScreen: {0}", clickablePoint.Y));

					} catch (InvalidCastException) {
						Assert.Fail (string.Format ("You are not returning System.Windows.Point in ClickablePointProperty: {0}",
						                            clickablePoint,GetType ()));
					}
				}
			// Clickable point should intersect bounding rectangle...
			} else {
				if (clickablePointObj == null) // ...unless you are not clickable at all
					return;

				Assert.IsNotNull (clickablePoint, 
				                  "Your ClickablePointProperty should not be null.");

				try {
					clickablePoint = (Point) clickablePointObj;
					Assert.IsTrue (clickablePoint.X >= 0,
					               string.Format ("X is negative, your provider should be OffScreen: {0}", clickablePoint.X));
					Assert.IsTrue (clickablePoint.Y >= 0,
					               string.Format ("Y is negative, your provider should be OffScreen: {0}", clickablePoint.Y));

				} catch (InvalidCastException) {
					Assert.Fail (string.Format ("You are not returning System.Windows.Point in ClickablePointProperty: {0}",
					                            clickablePoint.GetType ()));
				}

				object boundingRectangle
					= provider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
				Assert.IsNotNull (boundingRectangle, 
				                  "You need to return BoundingRectangle if you return ClickablePointProperty.");

				try {
					Rect boundingRectangleRect = (Rect) boundingRectangle;
					Assert.AreNotEqual (Rect.Empty, 
					                    boundingRectangleRect,
					                    "BoundingRectangle SHOULD NOT be Rect.Empty");

					Rect clickablePointRect = new Rect (clickablePoint.X, clickablePoint.Y, 1, 1);

					Assert.IsTrue (clickablePointRect.IntersectsWith (boundingRectangleRect),
					               string.Format ("ClickablePoint ({0}) SHOULD Intersect with BoundingRectangle: {1}",
					                              clickablePointRect.ToString (), 
					                              boundingRectangleRect.ToString ()));
					
				} catch (InvalidCastException) {
					Assert.Fail (string.Format ("You are not returning System.Windows.Rect in BoundingRectangle: {0}",
					                            boundingRectangle.GetType ()));
				}
			}			
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
		
		[Test]
		public virtual void FragmentRootAsParentTest ()
		{
			Control control = GetControlInstance ();			
			IRawElementProviderFragment fragment 
				= (IRawElementProviderFragment) GetProviderFromControl (control);
			if (fragment != null) {
				IRawElementProviderFragment parent = fragment.Navigate (NavigateDirection.Parent);
				
				//http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.irawelementproviderfragment.fragmentroot.aspx
				if (parent == null)
					Assert.AreEqual (fragment, fragment.FragmentRoot, "FragmentRoot != Parent");
				else
					Assert.AreEqual (parent, fragment.FragmentRoot, "FragmentRoot != Parent");
			}
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
			TestHelper.TestAutomationProperty (provider, property, expectedValue);
		}
		
		internal FormProvider FormProvider {
			get { return formProvider; }
		}
		
		protected Form Form {
			get { return form; }
		}
		
		protected IRawElementProviderFragment GetProviderFromControl (Control control)
		{
			if (form.Contains (control) == false && form != control 
			    && control != null && ((control as Form) == null))
				form.Controls.Add (control);
			form.Size = new System.Drawing.Size (400, 400);

			return (IRawElementProviderFragment) ProviderFactory.GetProvider (control);
		}

		protected void TestLabeledByAndName (bool expectNonNullLabel, bool expectNameFromLabel)
		{
			TestLabeledByAndName (expectNonNullLabel, expectNameFromLabel, true, false, false);
		}
		
		/// <summary>
		/// Simple test of expected relationship between LabeledBy
		/// property and Name property.  Does not handle change events.
		/// </summary>
		/// <param name="expectNonNullLabel">
		/// A <see cref="System.Boolean"/> indicating whether or not a
		/// non-null value should be expected when getting LabeledBy.
		/// </param>
		/// <param name="expectNameFromLabel">
		/// A <see cref="System.Boolean"/> indicating whether or not the
		/// Name property should come from the provider returned by
		/// LabeledBy. Ignored if expectNonNullLabel is false.
		/// </param>
		/// <param name="expectNonNullName">
		/// A <see cref="System.Boolean"/> indicating whether or not a
		/// non-null value should be expected when getting Name. Ignored
		/// if expectNameFromLabel is true.
		/// </param>
		/// <param name="expectNameFromText">
		/// A <see cref="System.Boolean"/> indicating whether or not the
		/// Name property should come from Control.Text (a common case).
		/// Ignored if expectNameFromLabel is true.
		/// </param>
		protected void TestLabeledByAndName (bool expectNonNullLabel,
		                                     bool expectNameFromLabel, 
		                                     bool expectNonNullName, 
		                                     bool expectNameFromText, 
		                                     bool expectNameFromAccessible)
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
				Assert.AreEqual (expectNonNullLabel ? labelProvider : null,
				                 labeledBy);

				if (expectNonNullLabel && expectNameFromLabel)
					Assert.AreEqual (labelProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id) as string,
					                 controlProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id) as string,
					                 "Control provider name should derive from label name.");
				else if (expectNonNullLabel && !expectNameFromLabel)
					Assert.IsTrue (labelProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id) as string ==
					               controlProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id) as string,
					               "Control provider name should not derive from label name.");
				
				if (!expectNameFromLabel && expectNonNullName && expectNameFromText && !expectNameFromAccessible)
					Assert.AreEqual (control.Text,
					                 controlProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id),
					                 "Control provider name should derive from control text.");
				else if (!expectNameFromLabel && expectNonNullName && !expectNameFromText && expectNameFromAccessible)
					Assert.AreEqual (control.AccessibleName,
					                 controlProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id),
					                 "Control provider name should derive from control AccessibleName.");
				else if (!expectNameFromLabel && !expectNonNullName)
					Assert.IsNull (controlProvider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id),
					               "Control provider name should be null.");
				
				f.Close ();
			}
		}

		protected virtual IRawElementProviderSimple GetProvider ()
		{
			Control control = GetControlInstance ();
			if (control == null) {
				Assert.Fail ("You need to override GetProvider because you are testing a Component-based provider.");
				return null;
			} else
				return GetProviderFromControl (control);
		}
		
		#endregion
		
	}
	
}
