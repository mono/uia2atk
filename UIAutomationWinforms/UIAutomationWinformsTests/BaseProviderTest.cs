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
//      Mario Carrion <mcarrion@novell.com>
// 


using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.Unix;
using NUnit.Framework;
using Mono.UIAutomation.Bridge;
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
			IRawElementProviderSimple provider = GetProvider ();
			
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
			IRawElementProviderSimple provider = GetProvider ();
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsControlElementProperty,
			              IsControlElement);
		}
		
		[Test]
		public virtual void AutomationIdPropertyTest ()
		{
			IRawElementProviderSimple provider = GetProvider ();

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
		public virtual void IsOffScreenPropertyTest ()
		{
			// This is Control-based test. If your provider is Component-based
			// you will need to override this test
			Control control = GetControlInstance ();

			// Also there are some controls either always-visible or special:
			if (control == null || control is Form || control is StatusBar
			    || control is MenuStrip || control is StatusStrip 
			    || control is ToolBar || control is Splitter)
				return;

			Form.Controls.Clear ();
			Form.Size = new System.Drawing.Size (150, 50);

			// We are adding our control and is visible
			control.Dock = DockStyle.None;
			control.Size = new System.Drawing.Size (15, 15);
			control.Location = new System.Drawing.Point (5, 5);

			Form.Controls.Add (control);

			IRawElementProviderFragment controlProvider = ProviderFactory.FindProvider (control);
			Assert.IsNotNull (controlProvider, 
			                  string.Format ("Control provider missing: {0}", control.GetType ()));

			Assert.IsFalse ((bool) controlProvider.GetPropertyValue (AutomationElementIdentifiers.IsOffscreenProperty.Id),
			                "Provider should be visible (!OffScreen)");

			// Lets move it out of visible bounds
			bridge.ResetEventLists ();
			control.Location = new System.Drawing.Point (160, 100);

			AutomationPropertyChangedEventTuple offscreenTuple 
				= bridge.GetAutomationPropertyEventFrom (controlProvider,
				                                         AutomationElementIdentifiers.IsOffscreenProperty.Id);
			Assert.IsNotNull (offscreenTuple, "IsOffScreen Event not raised");
			Assert.IsTrue ((bool) offscreenTuple.e.NewValue, "Is OffScreen (not visible)");
			Assert.IsFalse ((bool) offscreenTuple.e.OldValue, "Was not OffScreen (visible)");
			Assert.IsTrue ((bool) controlProvider.GetPropertyValue (AutomationElementIdentifiers.IsOffscreenProperty.Id),
			               "Provider should not be visible (OffScreen)");

			// Lets resize our form to make sure our control is visible again
			bridge.ResetEventLists ();
			Form.Size = new System.Drawing.Size (300, 300);
			offscreenTuple = bridge.GetAutomationPropertyEventFrom (controlProvider,
			                                                        AutomationElementIdentifiers.IsOffscreenProperty.Id);
			Assert.IsNotNull (offscreenTuple, "IsOffScreen Event not raised");
			Assert.IsFalse ((bool) offscreenTuple.e.NewValue, "Is Not OffScreen (visible)");
			Assert.IsTrue ((bool) offscreenTuple.e.OldValue, "Was OffScreen (not visible)");
			Assert.IsFalse ((bool) controlProvider.GetPropertyValue (AutomationElementIdentifiers.IsOffscreenProperty.Id),
			                "Provider should be visible (OffScreen)");

			// Lets resize again our form to its previous value to make sure our
			// control is not visible again
			bridge.ResetEventLists ();
			Form.Size = new System.Drawing.Size (150, 50);
			offscreenTuple = bridge.GetAutomationPropertyEventFrom (controlProvider,
			                                                        AutomationElementIdentifiers.IsOffscreenProperty.Id);
			Assert.IsNotNull (offscreenTuple, "IsOffScreen Event not raised");
			Assert.IsTrue ((bool) offscreenTuple.e.NewValue, "Is OffScreen (not visible)");
			Assert.IsFalse ((bool) offscreenTuple.e.OldValue, "Was Not OffScreen (visible)");
			Assert.IsTrue ((bool) controlProvider.GetPropertyValue (AutomationElementIdentifiers.IsOffscreenProperty.Id),
			               "Provider should not be visible (OffScreen)");
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

					Assert.IsTrue (boundingRectangleRect.Contains (clickablePointRect),
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
		public virtual void IsKeyboardFocusablePropertyTest ()
		{
			Control control = GetControlInstance ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (control);
			if (provider == null) {
				// This can be triggered by controls which
				// don't actually register themselves with
				// ProviderFactory (typically when they're
				// "fake" controls, e.g., they don't map to a
				// widget)
				return;
			}
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              control.CanFocus);
		}

		[Test]
		public virtual void AmpersandsAndNameTest ()
		{
			Control control = GetControlInstance ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (control);
			if (provider == null) {
				// This can be triggered by controls which
				// don't actually register themselves with
				// ProviderFactory (typically when they're
				// "fake" controls, e.g., they don't map to a
				// widget)
				return;
			}

			control.Text = "I like &ampersands";
			
			TestProperty (provider,
			              AutomationElementIdentifiers.NameProperty,
			              "I like ampersands");

			control.Text = "I like double &&ampersands";

			TestProperty (provider,
			              AutomationElementIdentifiers.NameProperty,
			              "I like double &ampersands");
		}
		
		[Test]
		public virtual void FragmentRootAsParentTest ()
		{
			IRawElementProviderFragment fragment 
				= GetProvider () as IRawElementProviderFragment;
			if (fragment != null) {
				IRawElementProviderFragment parent = fragment.Navigate (NavigateDirection.Parent);
				
				//http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.irawelementproviderfragment.fragmentroot.aspx
				if (parent == null)
					Assert.AreEqual (fragment, fragment.FragmentRoot, "FragmentRoot != Fragment");
				else
					Assert.AreEqual (parent, fragment.FragmentRoot, "FragmentRoot != Parent");
			}
		}

		#region Navigation Tests

		[Test]
		public void DefaultNavigationTest ()
		{
			IRawElementProviderFragment provider = GetProvider () as IRawElementProviderFragment;
			if (provider != null) {
				IRawElementProviderFragment child = provider.Navigate (NavigateDirection.FirstChild);
				while (child != null) {
					Assert.AreEqual (provider, 
					                 child.Navigate (NavigateDirection.Parent),
					                 "Child.Parent != provider");
					child = child.Navigate (NavigateDirection.NextSibling);
				}
			}
		}

		#endregion

		#region Pattern Tests

		[Test]
		public virtual void ButtonPatternsTest ()
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Button.Id)
				return;

			TestButtonPatterns (provider);
		}

		[Test]
		public virtual void CalendarPatternsTest ()
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Calendar.Id)
				return;

			TestCalendarPatterns (provider);
		}

		[Test]
		public virtual void CheckBoxPatternsTest () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.CheckBox.Id)
				return;

			TestCheckBoxPatterns (provider);
		}

		[Test]
		public virtual void ComboBoxPatternsTest () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.ComboBox.Id)
				return;

			TestComboBoxPatterns (provider);
		}

		[Test]
		public virtual void PatternsDataGridTest () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.DataGrid.Id)
				return;
			
			TestDataGridPatterns (provider);
		}

		[Test]
		public virtual void PatternsDataItemTest () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.DataItem.Id)
				return;

			TestDataItemPatterns (provider);
		}

		[Test]
		public virtual void PatternsDocumentTest () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.DataItem.Id)
				return;

			TestDocumentPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsEdit () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Edit.Id)
				return;

			TestEditPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsGroup () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Group.Id)
				return;

			TestGroupPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsHeader () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Header.Id)
				return;

			TestHeaderPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsHeaderItem () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.HeaderItem.Id)
				return;

			TestHeaderItemPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsHyperlink () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Hyperlink.Id)
				return;

			TestHyperlinkPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsImage () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Hyperlink.Id)
				return;

			TestHyperlinkPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsList () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.List.Id)
				return;

			TestListPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsListItem () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.ListItem.Id)
				return;

			TestListItemPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsMenu () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Menu.Id)
				return;

			TestMenuPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsMenuBar () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.MenuBar.Id)
				return;

			TestMenuBarPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsMenuItem ()
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.MenuItem.Id)
				return;

			TestMenuItemPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsPane () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Pane.Id)
				return;

			TestPanePatterns (provider);
		}

		[Test]
		public virtual void TestPatternsProgressBar () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.ProgressBar.Id)
				return;

			TestProgressBarPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsRadioButton () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.RadioButton.Id)
				return;

			TestRadioButtonPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsScrollBar () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.ScrollBar.Id)
				return;

			TestScrollBarPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsSeparator () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Separator.Id)
				return;

			TestSeparatorPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsSlider () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Slider.Id)
				return;

			TestSliderPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsSpinner () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Spinner.Id)
				return;

			TestSpinnerPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsSplitButton () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.SplitButton.Id)
				return;

			TestSplitButtonPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsStatusBar () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.StatusBar.Id)
				return;

			TestStatusBarPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsTab () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Tab.Id)
				return;

			TestTabPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsTabItem () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.TabItem.Id)
				return;

			TestTabItemPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsTable () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Table.Id)
				return;

			TestTablePatterns (provider);
		}

		[Test]
		public virtual void TestPatternsText () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Text.Id)
				return;

			TestTextPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsThumb () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Thumb.Id)
				return;

			TestThumbPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsTitleBar () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.TitleBar.Id)
				return;

			TestTitleBarPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsToolBar () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.ToolBar.Id)
				return;

			TestToolBarPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsToolTip () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.ToolTip.Id)
				return;

			TestToolTipPatterns (provider);
		}

		[Test]
		public virtual void TestPatternsTree () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Tree.Id)
				return;

			TestTreePatterns (provider);
		}

		[Test]
		public virtual void TestPatternsTreeItem () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.TreeItem.Id)
				return;

			TestTreeItemPatterns (provider);
		}


		[Test]
		public virtual void TestPatternsWindow () 
		{
			IRawElementProviderSimple provider = GetProvider ();

			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)
			    != ControlType.Window.Id)
				return;

			TestWindowPatterns (provider);
		}

		#endregion
		
		#endregion
		
		#region Abstract Members
	
		protected abstract Control GetControlInstance ();

		#endregion
	
		#region Protected Helper Methods
		
		protected void TestProperty (IRawElementProviderSimple provider,
		                             AutomationProperty property,
		                             object expectedValue)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");
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
			TestLabeledByAndName (expectNonNullLabel, expectNameFromLabel, expectNonNullName, expectNameFromText, expectNameFromAccessible, true);
			TestLabeledByAndName (expectNonNullLabel, expectNameFromLabel, expectNonNullName, expectNameFromText, expectNameFromAccessible, false);
		}

		protected void TestLabeledByAndName (bool expectNonNullLabel,
		                                     bool expectNameFromLabel, 
		                                     bool expectNonNullName, 
		                                     bool expectNameFromText, 
		                                     bool expectNameFromAccessible,
		                                     bool controlFirst)
		{
			Control control = GetControlInstance ();
			if (control == null)
				return;
			
			Form f = control as Form;
			if (f != null)
				return;
			
			using (f = new Form ()) {
				
				Label l = new Label ();
				l.Text = "my label";
				f.Controls.Add (controlFirst? control: l);
				f.Controls.Add (controlFirst? l: control);
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

		// Ripped off from AtkTests.Misc.LookForParentDir
		protected string LookForParentDir (string pattern) {
			// FIXME: it seems we should use this when bnc#450433 is fixed:
			//string imgDir =  System.IO.Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly ().CodeBase);
			string imgDir = System.IO.Directory.GetCurrentDirectory ();
			
			while (imgDir != "/"){
				if (System.IO.Directory.GetFiles (imgDir, pattern).Length == 0)
					imgDir = System.IO.Path.GetFullPath (System.IO.Path.Combine (imgDir, ".."));
	
				else
					break;
				
				string samples = System.IO.Path.Combine (System.IO.Path.Combine (imgDir, "test"), "samples");
				if (System.IO.Directory.Exists (samples)) { 
					if (System.IO.Directory.GetFiles (samples, pattern).Length > 0) {
						imgDir = System.IO.Path.GetFullPath (samples);
						break;
					}
				}
			}
	
			if (imgDir != "/")
				return imgDir;
	
			return null;
		}

		protected string LookForUia2AtkDir ()
		{
			var dirInfo = new DirectoryInfo (Directory.GetCurrentDirectory ());
			var rootInfo = dirInfo.Root;
			string [] subDirToTest = {
				"AtspiUiaSource",
				"MoonAtkBridge",
				"test",
				"UiaDbus",
				"UIAutomation",
				"UIAutomationWinforms",
				"UiaAtkBridge"
			};
			while (!dirInfo.Equals(rootInfo)) {
				int hitCount = 0;
				foreach (var childInfo in dirInfo.GetDirectories()) {
					if (Array.Exists (subDirToTest, item => item == childInfo.Name))
						hitCount++;
				}
				if (hitCount == subDirToTest.Length)
					return dirInfo.FullName;
				dirInfo = dirInfo.Parent;
			}
			return null;
		}

		#endregion

		#region Patterns Tests

		protected virtual void TestButtonPatterns (IRawElementProviderSimple provider)
		{
			// http://msdn.microsoft.com/en-us/library/ms742153.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsInvokePatternAvailableProperty.Id)
			               || (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsTogglePatternAvailableProperty.Id),
			               string.Format ("Button ControlType must support IInvokeProvider or IToggleProvider. Provider: {0}",
			                              provider.GetType ()));
			
			Assert.AreEqual (Catalog.GetString ("button"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
			
			TestInvokePattern_InvokedEvent (provider);
		}
		
		protected virtual void TestCalendarPatterns (IRawElementProviderSimple provider)
		{
			// http://msdn.microsoft.com/en-us/library/ms753925.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsGridPatternAvailableProperty.Id),
			               "Calendar ControlType must support IGridProvider");
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsSelectionPatternAvailableProperty.Id),
			               "Calendar ControlType must support ISelectionProvider");
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id),
			               "Calendar ControlType must support ITableProvider");
			
			Assert.IsFalse ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsValuePatternAvailableProperty.Id),
			               "Calendar ControlType must NOT support IValueProvider");

			Assert.AreEqual (Catalog.GetString ("calendar"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");

			// DEPENDS: IScrollProvider

			TestSelectionPattern_GetSelectionMethod (provider);
			TestTablePattern_All (provider);
		}

		protected virtual void TestCheckBoxPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms751693.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsTogglePatternAvailableProperty.Id),
			               "CheckBox ControlType must support IToggleProvider");

			Assert.AreEqual (Catalog.GetString ("check box"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");

			//A control must cycle through its ToggleState in the following order: On, Off and, if supported, Indeterminate.
			TestTogglePattern_ToggleStatePropertyEvent (provider);
		}
		
		protected virtual void TestComboBoxPatterns (IRawElementProviderSimple provider) 
		{
			//http://msdn.microsoft.com/en-us/library/ms752305.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty.Id),
			               "ComboBox ControlType must support IExpandCollapseProvider");
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsSelectionPatternAvailableProperty.Id),
			               "ComboBox ControlType must support ISelectionProvider");
			
			Assert.IsFalse ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsScrollPatternAvailableProperty.Id),
			                "ComboBox ControlType must NOT support IScrollProvider");

			Assert.AreEqual (Catalog.GetString ("combo box"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");

			// DEPENDS: IValueProvider

			TestExpandCollapsePattern_ExpandCollapseStatePropertyEvent (provider);
			TestSelectionPattern_GetSelectionMethod (provider);
		}

		protected virtual void TestDataGridPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms752044.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsGridPatternAvailableProperty.Id),
			               "DataGrid ControlType must support IGridProvider");
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsSelectionPatternAvailableProperty.Id),
			               "DataGrid ControlType must support ISelectionProvider");
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id),
			               "DataGrid ControlType must support ITableProvider");

			Assert.AreEqual (Catalog.GetString ("data grid"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");

			// DEPENDS: IScrollProvider

			TestSelectionPattern_GetSelectionMethod (provider);
			TestTablePattern_All (provider);
		}

		protected virtual void TestDataItemPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms742561.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty.Id),
			               "DataItem ControlType must support ISelectionItemProvider");

			// ITableItemProvider: (DEPENDS)
			// If the data item is contained within a Data Grid control type then it will support this pattern.
			IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
			if (fragment != null) {
				IRawElementProviderFragment parent = fragment.Navigate (NavigateDirection.Parent);
				if (parent != null) {
					if (ControlType.DataGrid.Id
					    == (int) parent.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id))
						Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsTableItemPatternAvailableProperty.Id),
						               "DataItem ControlType must support ITableItemProvider when parent is DataGrid");
				}
			}

			Assert.AreEqual (Catalog.GetString ("data item"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");

			// DEPENDS: IExpandCollapseProvider
			// DEPENDS: IGridItemProvider
			// DEPENDS: IScrollItemProvider
			// DEPENDS: IToggleProvider
			// DEPENDS: IValueProvider

			TestSelectionPatternChild (provider);
			TestGridPatternChild (provider);
			TestTablePatternChild (provider);
		}

		protected virtual void TestDocumentPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms752058.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsTextPatternAvailableProperty.Id),
			               "Document ControlType must support ITextProvider");

			Assert.IsFalse ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsValuePatternAvailableProperty.Id),
			                "Document ControlType must NOT support IValueProvider");

			Assert.AreEqual (Catalog.GetString ("document"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
			
			// DEPENDS: IScrollProvider
		}

		protected virtual void TestEditPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms748367.aspx
			
			// LAMESPEC: Edit must always support ITextProvider, but this *is not true* in Edit Cells
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsTextPatternAvailableProperty.Id),
			               "Edit ControlType must support ITextProvider (this is usually LAMESPEC)");
			
			// DEPENDS: IValueProvider
			// DEPENDS: IRangeValueProvider

			Assert.AreEqual (Catalog.GetString ("edit"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");

			if (provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id) != null)
				TestValuePattern_All (provider);
		}

		protected virtual void TestGroupPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms742689.aspx
			// DEPENDS: IExpandCollapseProvider

			Assert.AreEqual (Catalog.GetString ("group"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestHeaderPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms753110.aspx
			// DEPENDS: ITransformProvider

			Assert.AreEqual (Catalog.GetString ("header"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestHeaderItemPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms742202.aspx
			// DEPENDS: ITransformProvider
			// DEPENDS: IInvokeProvider

			Assert.AreEqual (Catalog.GetString ("header item"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");

			if (provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id) != null)
				TestInvokePattern_InvokedEvent (provider);
		}

		protected virtual void TestHyperlinkPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms742530.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsInvokePatternAvailableProperty.Id),
			               "Hyperlink ControlType must support IInvokeProvider");

			Assert.AreEqual (Catalog.GetString ("hyperlink"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");

			TestInvokePattern_InvokedEvent (provider);
		}

		protected virtual void TestImagePatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms746603.aspx
			Assert.IsFalse ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsInvokePatternAvailableProperty.Id),
			               "Image ControlType must NOT support IInvokeProvider");
			Assert.IsFalse ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty.Id),
			               "Image ControlType must NOT support ISelectionItemProvider");

			// DEPENDS: ITableItemProvider
			// DEPENDS: IGridItemProvider

			Assert.AreEqual (Catalog.GetString ("image"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestListPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms742462.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsSelectionPatternAvailableProperty.Id),
			               "List ControlType must support ISelectionProvider");

			Assert.IsFalse ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id),
			                "List ControlType must NOT support ITableProvider");

			// DEPENDS: IScrollProvider
			// DEPENDS: IGridProvider
			// DEPENDS: IMultipleViewProvider

			Assert.AreEqual (Catalog.GetString ("list"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");

			TestSelectionPattern_GetSelectionMethod (provider);
		}

		protected virtual void TestListItemPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms744765.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty.Id),
			               "ListItem ControlType must support ISelectionItemProvider");

			// DEPENDS: IScrollItemProvider
			// DEPENDS: IToggleProvider
			// DEPENDS: IExpandCollapseProvider
			// DEPENDS: IValueProvider
			// DEPENDS: IGridItemProvider
			// DEPENDS: IInvokeProvider

			Assert.AreEqual (Catalog.GetString ("list item"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
			
			TestSelectionPatternChild (provider);
			TestGridPatternChild (provider);
			TestTablePatternChild (provider);
		}

		protected virtual void TestMenuPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms741841.aspx
			
			// NO PATTERNS REQUIRED
		}

		protected virtual void TestMenuBarPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms752322.aspx
			
			// DEPENDS: IExpandCollapseProvider
			// DEPENDS: IDockProvider
			// DEPENDS: ITransformProvider

			Assert.AreEqual (Catalog.GetString ("menu bar"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestMenuItemPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms746680.aspx
			
			// DEPENDS: IExpandCollapseProvider
			// DEPENDS: IInvokeProvider
			// DEPENDS: IToggleProvider
			// DEPENDS: ISelectionItemProvider

			Assert.AreEqual (Catalog.GetString ("menu item"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestPanePatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms749129.aspx
			Assert.IsFalse ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsWindowPatternAvailableProperty.Id),
			                "Pane ControlType must NOT support IWindowProvider");
			
			// DEPENDS: ITransformProvider
			// DEPENDS: IDockProvider

			Assert.AreEqual (Catalog.GetString ("pane"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestProgressBarPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms743681.aspx

			// DEPENDS: IValueProvider
			// DEPENDS: IRangeValueProvider

			Assert.AreEqual (Catalog.GetString ("progress bar"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestRadioButtonPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms750484.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty.Id),
			                "RadioButton ControlType must support ISelectionItemProvider");
			
			Assert.IsFalse ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsTogglePatternAvailableProperty.Id),
			                "RadioButton ControlType must support IToggleProvider");

			Assert.AreEqual (Catalog.GetString ("radio button"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestScrollBarPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms743712.aspx
			Assert.IsFalse ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsScrollPatternAvailableProperty.Id),
			                "ScrollBar ControlType must support IScrollProvider");

			// DEPENDS: IRangeValueProvider

			Assert.AreEqual (Catalog.GetString ("scroll bar"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestSeparatorPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms750550.aspx

			// NO PATTERNS REQUIRED

			Assert.AreEqual (Catalog.GetString ("separator"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");			
		}

		protected virtual void TestSliderPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms742106.aspx

			// DEPENDS: ISelectionProvider
			// DEPENDS: IRangeValueProvider

			Assert.AreEqual (Catalog.GetString ("slider"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestSpinnerPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms744847.aspx

			// DEPENDS: ISelectionProvider
			// DEPENDS: IRangeValueProvider
			// DEPENDS: IValueProvider

			Assert.AreEqual (Catalog.GetString ("spinner"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestSplitButtonPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms742545.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty.Id),
			                "SplitButton ControlType must support IExpandCollapseProvider");
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsInvokePatternAvailableProperty.Id),
			                "SplitButton ControlType must support IInvokeProvider");

			Assert.AreEqual (Catalog.GetString ("split button"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");

			TestExpandCollapsePattern_ExpandCollapseStatePropertyEvent (provider);
			TestInvokePattern_InvokedEvent (provider);
		}

		protected virtual void TestStatusBarPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms745809.aspx

			// DEPENDS: IGridProvider

			Assert.AreEqual (Catalog.GetString ("status bar"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");			
		}

		protected virtual void TestTabPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms754207.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsSelectionPatternAvailableProperty.Id),
			                "Tab ControlType must support ISelectionProvider");

			// DEPENDS: IScrollProvider

			Assert.AreEqual (Catalog.GetString ("tab"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");

			TestSelectionPattern_GetSelectionMethod (provider);
		}

		protected virtual void TestTabItemPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms751611.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty.Id),
			                "TabItem ControlType must support ISelectionItemProvider");
			Assert.IsFalse ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsInvokePatternAvailableProperty.Id),
			                "TabItem ControlType must support IInvokeProvider");

			Assert.AreEqual (Catalog.GetString ("tab item"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestTablePatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms750608.aspx
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsGridPatternAvailableProperty.Id),
			                "Table ControlType must support IGridProvider");
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id),
			                "Table ControlType must support ITableProvider");

			// DEPENDS: ITableItemProvider

			Assert.AreEqual (Catalog.GetString ("table"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");

			TestTableItemPattern_All (provider);
		}

		protected virtual void TestTextPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms752073.aspx
			Assert.IsFalse ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsValuePatternAvailableProperty.Id),
			                "Text ControlType must NOT support IValueProvider");

			// DEPENDS: ITextProvider
			// DEPENDS: ITableItemProvider
			// DEPENDS: IRangeValueProvider

			Assert.AreEqual (Catalog.GetString ("text"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestThumbPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms742539.aspx

			// DEPENDS: IRangeValueProvider
			// DEPENDS: ITransformProvider

			Assert.AreEqual (Catalog.GetString ("thumb"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}


		protected virtual void TestTitleBarPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms744939.aspx

			// NO REQUIRED PATTERNS

			Assert.AreEqual (Catalog.GetString ("title bar"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestToolBarPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms750425.aspx

			// DEPENDS: IExpandCollapsePattern
			// DEPENDS: IDockPattern
			// DEPENDS: TransformPattern

			Assert.AreEqual (Catalog.GetString ("tool bar"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestToolTipPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms752335.aspx

			// DEPENDS: IWindowProvider
			// DEPENDS: ITextProvider

			Assert.AreEqual (Catalog.GetString ("tool tip"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestTreePatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms743706.aspx

			// DEPENDS: ISelectionProvider
			// DEPENDS: IScrollProvider

			Assert.AreEqual (Catalog.GetString ("tree"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestTreeItemPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms743384.aspx

			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty.Id),
			               "TreeItem ControlType must support IExpandCollapseProvider");

			// DEPENDS: IInvokeProvider
			// DEPENDS: IScrollItemProvider
			// DEPENDS: ISelectionItemProvider
			// DEPENDS: IToggleProvider
			
			Assert.AreEqual (Catalog.GetString ("tree item"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");

			TestExpandCollapsePattern_ExpandCollapseStatePropertyEvent (provider);
		}

		protected virtual void TestWindowPatterns (IRawElementProviderSimple provider) 
		{
			// http://msdn.microsoft.com/en-us/library/ms746673.aspx

			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsTransformPatternAvailableProperty.Id),
			               "Window ControlType must support ITransformProvider");
			Assert.IsTrue ((bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsWindowPatternAvailableProperty.Id),
			               "Window ControlType must support IWindowProvider");

			// DEPENDS: IDockProvider

			Assert.AreEqual (Catalog.GetString ("window"),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id),
			                 "LocalizedControlTypeProperty");
		}

		protected virtual void TestGridPatternChild (IRawElementProviderSimple provider)
		{
			IRawElementProviderFragment child = provider as IRawElementProviderFragment;
			if (child == null)
				return;

			IRawElementProviderFragment parent = child.Navigate (NavigateDirection.Parent);
			if (parent == null)
				return;
			
			if ((bool) parent.GetPropertyValue (AutomationElementIdentifiers.IsGridPatternAvailableProperty.Id)) {
				Assert.IsTrue ((bool) child.GetPropertyValue (AutomationElementIdentifiers.IsGridItemPatternAvailableProperty.Id),
				               string.Format ("{0} must support IGridItemProvider", child.GetType ()));

				TestGridItemPattern_ColumnPropertyEvent (provider);
				TestGridItemPattern_RowPropertyEvent (provider);

				child = child.Navigate (NavigateDirection.NextSibling);
			}
		}

		protected virtual void TestSelectionPatternChild (IRawElementProviderSimple provider)
		{
			IRawElementProviderFragment child = provider as IRawElementProviderFragment;
			if (child == null)
				return;

			IRawElementProviderFragment parent = child.Navigate (NavigateDirection.Parent);
			if (parent == null)
				return;
			
			if ((bool) parent.GetPropertyValue (AutomationElementIdentifiers.IsSelectionPatternAvailableProperty.Id)) {
				Assert.IsTrue ((bool) child.GetPropertyValue (AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty.Id),
				               string.Format ("{0} must support ISelectionItemProvider", child.GetType ()));

				TestSelectionItemPattern_AddToSelectionMethod (provider);
				TestSelectionItemPattern_RemoveFromSelectionMethod (provider);
				TestSelectionItemPattern_SelectMethod (provider);
				TestSelectionItemPattern_IsSelectedPropertyEvent (provider);
				
				child = child.Navigate (NavigateDirection.NextSibling);
			}
		}

		protected virtual void TestTablePatternChild (IRawElementProviderSimple provider)
		{
			IRawElementProviderFragment child = provider as IRawElementProviderFragment;
			if (child == null)
				return;

			IRawElementProviderFragment parent = child.Navigate (NavigateDirection.Parent);
			if (parent == null)
				return;
			
			if ((bool) parent.GetPropertyValue (AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id)) {
				Assert.IsTrue ((bool) child.GetPropertyValue (AutomationElementIdentifiers.IsTableItemPatternAvailableProperty.Id),
				               string.Format ("{0} must support ITableItemProvider. Parent: {1}", child.GetType (), parent.GetType ()));

				TestTableItemPattern_GetColumnHeaderItems (provider);
				TestTableItemPattern_GetRowHeaderItems (provider);
				
				child = child.Navigate (NavigateDirection.NextSibling);
			}
		}

		protected void TestPatterns (IRawElementProviderSimple provider)
		{
			int ctype = (int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (ctype == ControlType.Button.Id)
				TestButtonPatterns (provider);
			else if (ctype == ControlType.Calendar.Id)
				TestCalendarPatterns (provider);
			else if (ctype == ControlType.CheckBox.Id)
				TestCheckBoxPatterns (provider);
			else if (ctype == ControlType.ComboBox.Id)
				TestComboBoxPatterns (provider);
			else if (ctype == ControlType.DataGrid.Id)
				TestDataGridPatterns (provider);
			else if (ctype == ControlType.DataItem.Id)
				TestDataItemPatterns (provider);
			else if (ctype == ControlType.DataItem.Id)
				TestDocumentPatterns (provider);
			else if (ctype == ControlType.Edit.Id)
				TestEditPatterns (provider);
			else if (ctype == ControlType.Group.Id)
				TestGroupPatterns (provider);
			else if (ctype == ControlType.Header.Id)
				TestHeaderPatterns (provider);
			else if (ctype == ControlType.HeaderItem.Id)
				TestHeaderItemPatterns (provider);
			else if (ctype == ControlType.Hyperlink.Id)
				TestHyperlinkPatterns (provider);
			else if (ctype == ControlType.Hyperlink.Id)
				TestHyperlinkPatterns (provider);
			else if (ctype == ControlType.List.Id)
				TestListPatterns (provider);
			else if (ctype == ControlType.ListItem.Id)
				TestListItemPatterns (provider);
			else if (ctype == ControlType.Menu.Id)
				TestMenuPatterns (provider);
			else if (ctype == ControlType.MenuBar.Id)
				TestMenuBarPatterns (provider);
			else if (ctype == ControlType.MenuItem.Id)
				TestMenuItemPatterns (provider);
			else if (ctype == ControlType.Pane.Id)
				TestPanePatterns (provider);
			else if (ctype == ControlType.ProgressBar.Id)
				TestProgressBarPatterns (provider);
			else if (ctype == ControlType.RadioButton.Id)
				TestRadioButtonPatterns (provider);
			else if (ctype == ControlType.ScrollBar.Id)
				TestScrollBarPatterns (provider);
			else if (ctype == ControlType.Separator.Id)
				TestSeparatorPatterns (provider);
			else if (ctype == ControlType.Slider.Id)
				TestSliderPatterns (provider);
			else if (ctype == ControlType.Spinner.Id)
				TestSpinnerPatterns (provider);
			else if (ctype == ControlType.SplitButton.Id)
				TestSplitButtonPatterns (provider);
			else if (ctype == ControlType.StatusBar.Id)
				TestStatusBarPatterns (provider);
			else if (ctype == ControlType.Tab.Id)
				TestTabPatterns (provider);
			else if (ctype == ControlType.TabItem.Id)
				TestTabItemPatterns (provider);
			else if (ctype == ControlType.Table.Id)
				TestTablePatterns (provider);
			else if (ctype == ControlType.Text.Id)
				TestTextPatterns (provider);
			else if (ctype == ControlType.Thumb.Id)
				TestThumbPatterns (provider);
			else if (ctype == ControlType.TitleBar.Id)
				TestTitleBarPatterns (provider);
			else if (ctype == ControlType.ToolBar.Id)
				TestToolBarPatterns (provider);
			else if (ctype == ControlType.ToolTip.Id)
				TestToolTipPatterns (provider);
			else if (ctype == ControlType.Tree.Id)
				TestTreePatterns (provider);
			else if (ctype == ControlType.TreeItem.Id)
				TestTreeItemPatterns (provider);
			else if (ctype == ControlType.Window.Id)
				TestWindowPatterns (provider);
		}

		protected void TestChildPatterns (IRawElementProviderFragment root)
		{
			IRawElementProviderFragment provider = root.Navigate (NavigateDirection.FirstChild);
			while (provider != null) {
				TestPatterns (provider);
//				TestChildPatterns (provider);
				provider = provider.Navigate (NavigateDirection.NextSibling);
			}
		}

		#endregion

		#region Test Events for each patterns

		protected virtual void TestInvokePattern_InvokedEvent (IRawElementProviderSimple provider)
		{
			IInvokeProvider invokeProvider 
				= provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id) as IInvokeProvider;
			if (invokeProvider == null)
				Assert.Fail ("Provider {0} is not implementing IInvokeProvider", provider.GetType ());

			bridge.ResetEventLists ();

			bool enabled 
				= (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			try {
				invokeProvider.Invoke ();
				Assert.IsNotNull (bridge.GetAutomationEventFrom (provider, InvokePatternIdentifiers.InvokedEvent.Id),
				                  "IInvokeProvider.Invoke didn't raise any event.");
			} catch (ElementNotEnabledException) {
				// According to http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.iinvokeprovider.invoke.aspx
				if (!enabled)
					Assert.Fail ("Your provider is disabled but didn't throw ElementNotEnabledException.");
			}
		}

		protected virtual void TestTogglePattern_ToggleStatePropertyEvent (IRawElementProviderSimple provider)
		{
			IToggleProvider toggleProvider
				= provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id) as IToggleProvider;
			if (toggleProvider == null)
				Assert.Fail ("Provider {0} is not implementing IToggleProvider", provider.GetType ());


			// A control must cycle through its toggle states in this order:
			// On, Off, Indeterminate
			bool enabled 
				= (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			ToggleState toggleState = toggleProvider.ToggleState;
			ToggleState firstState = toggleState;

			try {
				bridge.ResetEventLists ();				
				do {
					bridge.ResetEventLists ();
					toggleProvider.Toggle ();
					Assert.IsNotNull (bridge.GetAutomationPropertyEventFrom (provider, TogglePatternIdentifiers.ToggleStateProperty.Id),
					                  string.Format ("toggleProvider.Toggle didn't raise any event. Old {0} Current: {1}", 
					                                 toggleState,
					                                 toggleProvider.ToggleState));
					ValidateToggleState  (toggleState, toggleProvider.ToggleState);
					toggleState = toggleProvider.ToggleState;
				} while (firstState != toggleState);
			} catch (ElementNotEnabledException) {
				if (!enabled)
					Assert.Fail ("Your provider is disabled but didn't throw ElementNotEnabledException.");
			}			
		}

		protected virtual void TestValuePattern_All (IRawElementProviderSimple provider)
		{
			// FIXME: We need to enable this again.			
//			TestValuePattern_ValuePropertyEvent (provider);
//			TestValuePattern_IsReadOnlyPropertyEvent (provider);
		}

		protected virtual void TestValuePattern_ValuePropertyEvent (IRawElementProviderSimple provider)
		{
			IValueProvider valueProvider 
				= provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id) as IValueProvider;
			if (valueProvider == null)
				Assert.Fail ("Provider {0} is not implementing IValueProvider", provider.GetType ());

			bool enabled 
				= (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			Assert.AreEqual (!enabled,
			                 valueProvider.IsReadOnly, 
			                 "!Enabled and IValueProvider.IsReadOnly should be equal");
			try {
				string newValue = "I'm your new value!";
				bridge.ResetEventLists ();
				valueProvider.SetValue (newValue);
				Assert.IsNotNull (bridge.GetAutomationPropertyEventFrom (provider, ValuePatternIdentifiers.ValueProperty.Id),
				                  "IValueProvider.Value didn't raise any event.");
				Assert.AreEqual (newValue,
				                 valueProvider.Value, 
				                 "Different value even when Value event was raised");
			} catch (ElementNotEnabledException) {
				// According to http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.ivalueprovider.setvalue.aspx
				if (!enabled)
					Assert.Fail ("Your provider is disabled but didn't throw ElementNotEnabledException.");
			}
		}

		protected virtual void TestValuePattern_IsReadOnlyPropertyEvent (IRawElementProviderSimple provider)
		{
			IValueProvider valueProvider 
				= provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id) as IValueProvider;
			if (valueProvider == null)
				Assert.Fail ("Provider {0} is not implementing IValueProvider", provider.GetType ());

			bool enabled 
				= (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			Assert.AreEqual (!enabled,
			                 valueProvider.IsReadOnly, 
			                 "!Enabled and IValueProvider.IsReadOnly should be equal");
			
			bool newValue = enabled;

			bridge.ResetEventLists ();
			TestValuePattern_ChangeReadOnly (provider, newValue);
			AutomationPropertyChangedEventTuple tuple
				= bridge.GetAutomationPropertyEventFrom (provider, ValuePatternIdentifiers.IsReadOnlyProperty.Id);
			Assert.IsNotNull (tuple,
			                  "IValueProvider.IsReadOnly didn't raise any event.");
			Assert.AreEqual (newValue, tuple.e.NewValue, "Value is invalid in event.");
			Assert.AreEqual (!newValue, tuple.e.OldValue, "Value is invalid in event.");

			Assert.AreEqual (newValue,
			                 valueProvider.IsReadOnly, 
			                 "IsReadOnly value not changed.");
			Assert.AreEqual (!newValue,
			                 (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id), 
			                 "Enabled value not changed.");

			// Changing again
			newValue = !newValue;
			
			bridge.ResetEventLists ();
			TestValuePattern_ChangeReadOnly (provider, newValue);
			tuple = bridge.GetAutomationPropertyEventFrom (provider, ValuePatternIdentifiers.IsReadOnlyProperty.Id);
			Assert.IsNotNull (tuple,
			                  "IValueProvider.IsReadOnly didn't raise any event.");
			Assert.AreEqual (newValue, tuple.e.NewValue, "Value is invalid in event.");
			Assert.AreEqual (!newValue, tuple.e.OldValue, "Value is invalid in event.");

			Assert.AreEqual (newValue,
			                 valueProvider.IsReadOnly, 
			                 "IsReadOnly value not changed.");
			Assert.AreEqual (!newValue,
			                 (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id), 
			                 "Enabled value not changed.");
		}

		protected virtual void TestValuePattern_ChangeReadOnly (IRawElementProviderSimple provider, 
		                                                        bool newValue)
		{
			// This must be overridden by providers to actually change the value, 
			// if they are using a different approach to change ReadOnly
			
			SimpleControlProvider controlProvider = provider as SimpleControlProvider;
			if (controlProvider == null)
				return;

			controlProvider.Control.Enabled = newValue;
		}

		protected virtual void TestExpandCollapsePattern_ExpandCollapseStatePropertyEvent (IRawElementProviderSimple provider)
		{
			IExpandCollapseProvider expandCollapseProvider
				= provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id) as IExpandCollapseProvider;
			if (expandCollapseProvider == null)
				Assert.Fail ("Provider {0} is not implementing IExpandCollapseProvider", provider.GetType ());

			bool enabled 
				= (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			ExpandCollapseState currentState = expandCollapseProvider.ExpandCollapseState;

			try {
				bridge.ResetEventLists ();

				if (currentState == ExpandCollapseState.Collapsed)
					expandCollapseProvider.Expand ();
				else if (currentState == ExpandCollapseState.Expanded 
				         || currentState == ExpandCollapseState.PartiallyExpanded)
					expandCollapseProvider.Collapse ();
				
				Assert.IsNotNull (bridge.GetAutomationPropertyEventFrom (provider, ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id),
				                  "IExpandCollapseProvider.ExpandCollapseStateProperty didn't raise any event.");
				// Is hard to test NewValue or OldValue because the spec doesn't
				// say anything about those values when calling Collapse/Expand.
			} catch (ElementNotEnabledException) {
				if (!enabled)
					Assert.Fail ("Your provider is disabled but didn't throw ElementNotEnabledException.");
			} catch (InvalidOperationException) {
				// Expand is called when the ExpandCollapseState = LeafNode.
				if (currentState != ExpandCollapseState.LeafNode)
					Assert.Fail (string.Format ("InvalidOperationException is only thrown when state is LeafNode, current state: ", currentState));
			}
		}

		protected virtual void TestSelectionPattern_GetSelectionMethod (IRawElementProviderSimple provider)
		{
			ISelectionProvider selectionProvider
				= provider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id) as ISelectionProvider;
			if (selectionProvider == null)
				Assert.Fail ("Provider {0} is not implementing ISelectionProvider", provider.GetType ());

			// Provider must never return null
			Assert.IsNotNull (selectionProvider.GetSelection (), 
			                  "GetSelection must return an empty array instead of returning null");

			// Testing children SelectionItem: "The children of this control must implement ISelectionItemProvider."
			// http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.iselectionprovider.aspx

			IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
			if (fragment == null)
				return;

			IRawElementProviderFragment child = fragment.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				TestSelectionPatternChild (child);
				child = child.Navigate (NavigateDirection.NextSibling);
			}
		}

		protected virtual void TestSelectionItemPattern_All (IRawElementProviderSimple provider)
		{
			TestSelectionItemPattern_AddToSelectionMethod (provider);
			TestSelectionItemPattern_RemoveFromSelectionMethod (provider);
			TestSelectionItemPattern_SelectMethod (provider);
			TestSelectionItemPattern_IsSelectedPropertyEvent (provider);
		}

		protected virtual void TestSelectionItemPattern_AddToSelectionMethod (IRawElementProviderSimple provider)
		{
			ISelectionItemProvider selectionItemProvider
				= provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id) as ISelectionItemProvider;
			if (selectionItemProvider == null)
				Assert.Fail ("Provider {0} is not implementing ISelectionItemProvider", provider.GetType ());

			// Parent must implement Selection
			IRawElementProviderFragment child = provider as IRawElementProviderFragment;
			if (child == null)
				Assert.Fail ("Unable to test a non-navigable ISelectionItemProvider");

			IRawElementProviderFragment parent = child.Navigate (NavigateDirection.Parent);
			if (parent == null)
				Assert.Fail ("We need parent to test ISelectionItemProvider.");

			ISelectionProvider selectionProvider
				= parent.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id) as ISelectionProvider;
			if (selectionProvider == null)
				Assert.Fail ("ISelectionItemProvider.Parent {0} is not implementing ISelectionProvider", parent.GetType ());

			bool selectionIsRequired = selectionProvider.IsSelectionRequired;
			
			// Item is selected, we try to unselect it
			if (selectionItemProvider.IsSelected) {
				// We are still trying to remove from selection to make sure the
				// exceptions are thrown, however we won't continue adding/removing
				TestSelectionItemPattern_RemoveFromSelection (parent, child);
			}

			int selectedItems = selectionProvider.GetSelection ().Length;

			try {
				bridge.ResetEventLists ();
				selectionItemProvider.AddToSelection ();

				// SelectionItemPatternIdentifiers.ElementSelectedEvent *ONLY*.
				if (selectedItems == 0) {
					Assert.IsNotNull (bridge.GetAutomationEventFrom (provider,
					                                                 SelectionItemPatternIdentifiers.ElementSelectedEvent.Id),
					                 string.Format ("ElementSelectedEvent event.", provider.GetType ()));
					Assert.IsNull (bridge.GetAutomationEventFrom (provider,
					                                              SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent.Id),
					               string.Format ("{0} ElementAddedToSelectionEvent event.", child.GetType ()));
				// SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent *ONLY*.
				} else {
					if (selectionProvider.CanSelectMultiple) {
						Assert.IsNotNull (bridge.GetAutomationEventFrom (provider,
						                                                 SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent.Id),
						                  string.Format ("{0} ElementAddedToSelectionEvent event.", provider.GetType ()));
						Assert.IsNull (bridge.GetAutomationEventFrom (provider,
						                                              SelectionItemPatternIdentifiers.ElementSelectedEvent.Id),
						               string.Format ("{0} ElementSelectedEvent event.", provider.GetType ()));
					} else if (!selectionItemProvider.IsSelected) {
						// We should get some selected event, not sure
						Assert.Greater (bridge.GetAutomationEventCount (SelectionItemPatternIdentifiers.ElementSelectedEvent), 1);
						
						Assert.IsNotNull (bridge.GetAutomationEventFrom (provider,
						                                                 SelectionItemPatternIdentifiers.ElementSelectedEvent.Id),
						               string.Format ("{0} ElementSelectedEvent event.", provider.GetType ()));
					}
				}
				Assert.IsNull (bridge.GetAutomationEventFrom (provider,
				                                              SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent.Id),
				               string.Format ("{0} ElementRemovedFromSelectionEvent event.", provider.GetType ()));


				// Selection should not be required, because when required the 
				// selection doesn't change.
				if (!selectionIsRequired) {
					// IsSelectedProperty
					Assert.IsNotNull (bridge.GetAutomationPropertyEventFrom (provider,
					                                                         SelectionItemPatternIdentifiers.IsSelectedProperty.Id),
					                 string.Format ("{0} IsSelectedProperty event.", provider.GetType ()));
				}
				
			} catch (InvalidOperationException) {
				if (!selectionProvider.CanSelectMultiple && selectionProvider.GetSelection ().Length == 1) {
					// AddToSelection is called on a single-selection container 
					// where CanSelectMultipleProperty = false and another element is already selected.
				} else
					Assert.Fail ("You should not be throwing InvalidOperationException when calling AddToSelection.");
			}
			
			TestSelectionItemPattern_RemoveFromSelectionMethod (provider);
		}

		protected virtual void TestSelectionItemPattern_RemoveFromSelectionMethod (IRawElementProviderSimple provider)
		{
			ISelectionItemProvider selectionItemProvider
				= provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id) as ISelectionItemProvider;
			if (selectionItemProvider == null)
				Assert.Fail ("Provider {0} is not implementing ISelectionItemProvider", provider.GetType ());

			IRawElementProviderFragment child = provider as IRawElementProviderFragment;
			if (child == null)
				Assert.Fail ("Unable to test a non-navigable ISelectionItemProvider");

			// Parent must implement Selection
			IRawElementProviderFragment parent = child.Navigate (NavigateDirection.Parent);
			if (parent == null)
				Assert.Fail ("We need parent to test ISelectionItemProvider.");

			TestSelectionItemPattern_RemoveFromSelection (parent, child);
		}
		
		protected virtual void TestSelectionItemPattern_SelectMethod (IRawElementProviderSimple provider)
		{
			ISelectionItemProvider selectionItemProvider
				= provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id) as ISelectionItemProvider;
			if (selectionItemProvider == null)
				Assert.Fail ("Provider {0} is not implementing ISelectionItemProvider", provider.GetType ());

			IRawElementProviderFragment child = provider as IRawElementProviderFragment;
			if (child == null)
				Assert.Fail ("Unable to test a non-navigable ISelectionItemProvider");

			// Parent must implement Selection
			IRawElementProviderFragment parent = child.Navigate (NavigateDirection.Parent);
			if (parent == null)
				Assert.Fail ("We need parent to test ISelectionItemProvider.");

			ISelectionProvider selectionProvider
				= parent.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id) as ISelectionProvider;
			if (selectionProvider == null)
				Assert.Fail ("ISelectionItemProvider.Parent {0} is not implementing ISelectionProvider", parent.GetType ());

			// Item is selected, we try to unselect it
			if (selectionItemProvider.IsSelected)
				TestSelectionItemPattern_RemoveFromSelection (parent, child);

			if (selectionItemProvider.IsSelected && selectionProvider.IsSelectionRequired)
				return;

			int selectedItems = selectionProvider.GetSelection ().Length;

			bridge.ResetEventLists ();
			selectionItemProvider.Select ();
			
			// IsSelectedProperty
			AutomationPropertyChangedEventTuple tuple
				= bridge.GetAutomationPropertyEventFrom (provider,
				                                         SelectionItemPatternIdentifiers.IsSelectedProperty.Id);
			Assert.IsNotNull (tuple,
			                  string.Format ("{0} IsSelectedProperty event.", provider.GetType ()));
			Assert.IsFalse ((bool) tuple.e.OldValue, "Old value must be false");
			Assert.IsTrue ((bool) tuple.e.NewValue, "New value must be false");


			if (selectionProvider.CanSelectMultiple)
				Assert.Greater (selectionProvider.GetSelection ().Length,
				                selectedItems,
				                "Selection must be greater that old selectedItems");
				
		}
		
		protected virtual void TestSelectionItemPattern_IsSelectedPropertyEvent (IRawElementProviderSimple provider)
		{
			ISelectionItemProvider selectionItemProvider
				= provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id) as ISelectionItemProvider;
			if (selectionItemProvider == null)
				Assert.Fail ("Provider {0} is not implementing ISelectionItemProvider", provider.GetType ());

			IRawElementProviderFragment child = provider as IRawElementProviderFragment;
			if (child == null)
				Assert.Fail ("Unable to test a non-navigable ISelectionItemProvider");

			// Parent must implement Selection
			IRawElementProviderFragment parent = child.Navigate (NavigateDirection.Parent);
			if (parent == null)
				Assert.Fail ("We need parent to test ISelectionItemProvider.");

			ISelectionProvider selectionProvider
				= parent.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id) as ISelectionProvider;
			if (selectionProvider == null)
				Assert.Fail ("ISelectionItemProvider.Parent {0} is not implementing ISelectionProvider", parent.GetType ());

			// Item is selected, we try to unselect it
			if (selectionItemProvider.IsSelected)
				TestSelectionItemPattern_RemoveFromSelection (parent, child);

			if (selectionItemProvider.IsSelected && selectionProvider.IsSelectionRequired)
				return;

			int selectedItems = selectionProvider.GetSelection ().Length;

			bridge.ResetEventLists ();
			selectionItemProvider.Select ();
			
			// IsSelectedProperty
			AutomationPropertyChangedEventTuple tuple
				= bridge.GetAutomationPropertyEventFrom (provider,
				                                         SelectionItemPatternIdentifiers.IsSelectedProperty.Id);
			Assert.IsNotNull (tuple,
			                  string.Format ("{0} IsSelectedProperty event.", provider.GetType ()));
			Assert.IsFalse ((bool) tuple.e.OldValue, "Old value must be false");
			Assert.IsTrue ((bool) tuple.e.NewValue, "New value must be false");

			Assert.IsTrue (selectionItemProvider.IsSelected, "Must be selected");

			Assert.Greater (selectionProvider.GetSelection ().Length,
			                selectedItems,
			                "Selection must be greater that old selectedItems");
		}

		protected virtual void TestTablePattern_All (IRawElementProviderSimple provider)
		{
			TestTablePattern_GetColumnHeaderItemsMethod (provider);
			TestTablePattern_GetRowHeaderItems (provider);
		}

		protected virtual void TestTablePattern_GetColumnHeaderItemsMethod (IRawElementProviderSimple provider)
		{
			ITableProvider tableProvider
				= provider.GetPatternProvider (TablePatternIdentifiers.Pattern.Id) as ITableProvider;
			if (tableProvider == null)
				Assert.Fail ("Provider {0} is not implementing ITableProvider", provider.GetType ());

			// Provider must never return null
			Assert.IsNotNull (tableProvider.GetColumnHeaders (), 
			                  "GetColumnHeaderItems must return an empty array instead of returning null");

			// Testing children TableItemProvider: "The children of this control must implement ITableItemProvider."
			// http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.itableprovider.aspx

			IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
			if (fragment == null)
				return;

			IRawElementProviderFragment child = fragment.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				TestTablePatternChild (child);
				child = child.Navigate (NavigateDirection.NextSibling);
			}
		}

		protected virtual void TestTablePattern_GetRowHeaderItems (IRawElementProviderSimple provider)
		{
			ITableProvider tableProvider
				= provider.GetPatternProvider (TablePatternIdentifiers.Pattern.Id) as ITableProvider;
			if (tableProvider == null)
				Assert.Fail ("Provider {0} is not implementing ITableProvider", provider.GetType ());

			// Provider must never return null
			Assert.IsNotNull (tableProvider.GetRowHeaders (), 
			                  "GetRowHeaderItems must return an empty array instead of returning null");

			// Testing children TableItemProvider: "The children of this control must implement ITableItemProvider."
			// http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.itableprovider.aspx

			IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
			if (fragment == null)
				return;

			IRawElementProviderFragment child = fragment.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				TestTablePatternChild (child);
				child = child.Navigate (NavigateDirection.NextSibling);
			}
		}

		protected virtual void TestTableItemPattern_All (IRawElementProviderSimple provider)
		{
			TestTableItemPattern_GetColumnHeaderItems (provider);
			TestTableItemPattern_GetRowHeaderItems (provider);
		}

		protected virtual void TestTableItemPattern_GetColumnHeaderItems (IRawElementProviderSimple provider)
		{
			ITableItemProvider tableItemProvider
				= provider.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id) as ITableItemProvider;
			if (tableItemProvider == null)
				Assert.Fail ("Provider {0} is not implementing ITableItemProvider", provider.GetType ());

			IRawElementProviderFragment child = provider as IRawElementProviderFragment;
			if (child == null)
				Assert.Fail ("Unable to test a non-navigable ITableItemProvider");

			Assert.IsNotNull (tableItemProvider.ContainingGrid, 
			                  "ContainingGrid should not be null");

			// http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.igriditemprovider.containinggrid.aspx
			Assert.IsNotNull (tableItemProvider.ContainingGrid.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			                  "ContainingGrid must implement GridPatternProvider");

			// Provider must never return null
			Assert.IsNotNull (tableItemProvider.GetColumnHeaderItems (), 
			                  "GetColumnHeaderItems must return an empty array instead of returning null");
		}

		protected virtual void TestTableItemPattern_GetRowHeaderItems (IRawElementProviderSimple provider)
		{
			ITableItemProvider tableItemProvider
				= provider.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id) as ITableItemProvider;
			if (tableItemProvider == null)
				Assert.Fail ("Provider {0} is not implementing ITableItemProvider", provider.GetType ());

			IRawElementProviderFragment child = provider as IRawElementProviderFragment;
			if (child == null)
				Assert.Fail ("Unable to test a non-navigable ITableItemProvider");

			Assert.IsNotNull (tableItemProvider.ContainingGrid, 
			                  "ContainingGrid should not be null");

			// http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.igriditemprovider.containinggrid.aspx
			Assert.IsNotNull (tableItemProvider.ContainingGrid.GetPatternProvider (GridPatternIdentifiers.Pattern.Id),
			                  "ContainingGrid must implement GridPatternProvider");

			// Provider must never return null
			Assert.IsNotNull (tableItemProvider.GetRowHeaderItems (), 
			                  "GetRowHeaderItems must return an empty array instead of returning null");
		}

		protected virtual void TestGridItemPattern_All (IRawElementProviderSimple provider)
		{
			TestGridItemPattern_ColumnPropertyEvent (provider);
			TestGridItemPattern_RowPropertyEvent (provider);
		}

		protected virtual void TestGridItemPattern_ColumnPropertyEvent (IRawElementProviderSimple provider)
		{
			IGridItemProvider gridItemProvider
				= provider.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id) as IGridItemProvider;
			if (gridItemProvider == null)
				Assert.Fail ("Provider {0} is not implementing IGridItemProvider", provider.GetType ());

			IRawElementProviderFragment child = provider as IRawElementProviderFragment;
			if (child == null)
				Assert.Fail ("Unable to test a non-navigable IGridItemProvider");

			Assert.IsNotNull (gridItemProvider.ContainingGrid, 
			                  "ContainingGrid must not be null");
			IGridProvider gridProvider 
				= gridItemProvider.ContainingGrid.GetPatternProvider (GridPatternIdentifiers.Pattern.Id) as IGridProvider;
			if (gridProvider == null)
				Assert.Fail ("Provider {0} is ContainingGrid of {1} and should implement IGridProvider",
				             gridItemProvider.ContainingGrid,
				             provider.GetType ());

			// FIXME: Implement
		}
		
		protected virtual void TestGridItemPattern_RowPropertyEvent (IRawElementProviderSimple provider)
		{
			IGridItemProvider gridItemProvider
				= provider.GetPatternProvider (GridItemPatternIdentifiers.Pattern.Id) as IGridItemProvider;
			if (gridItemProvider == null)
				Assert.Fail ("Provider {0} is not implementing IGridItemProvider", provider.GetType ());

			IRawElementProviderFragment child = provider as IRawElementProviderFragment;
			if (child == null)
				Assert.Fail ("Unable to test a non-navigable IGridItemProvider");

			Assert.IsNotNull (gridItemProvider.ContainingGrid, 
			                  "ContainingGrid must not be null");
			IGridProvider gridProvider 
				= gridItemProvider.ContainingGrid.GetPatternProvider (GridPatternIdentifiers.Pattern.Id) as IGridProvider;
			if (gridProvider == null)
				Assert.Fail ("Provider {0} is ContainingGrid of {1} and should implement IGridProvider",
				             gridItemProvider.ContainingGrid,
				             provider.GetType ());

			int oldValue = gridItemProvider.Row;
			int currentRow = gridItemProvider.Row;

			// Adding row before to increase our current row value
			bridge.ResetEventLists ();
			TestGridItemPattern_AddRowBefore (provider, 1);

			AutomationPropertyChangedEventTuple tuple
				= bridge.GetAutomationPropertyEventFrom (provider,
				                                         GridItemPatternIdentifiers.RowProperty.Id);
			Assert.IsNotNull (tuple,
			                  string.Format ("{0} RowProperty event.", provider.GetType ()));
			Assert.AreEqual (currentRow,
			                 (int) tuple.e.OldValue,
			                 "Old value should match value before adding row");

			int newValue = 0;
			
			if (gridProvider.ColumnCount > 1)
				// We may be moved to next column
				newValue = (currentRow + 1) % gridProvider.RowCount;
			else
				newValue = currentRow + 1;

			Assert.AreEqual (newValue,
			                 (int) tuple.e.NewValue,
			                 "New value should match value after adding row");
			Assert.AreEqual (newValue,
			                 gridItemProvider.Row,
			                 "Current row is different than NewValue");
			
			// Adding row after to just match sure our Row is not changing at all
			currentRow = gridItemProvider.Row;
			bridge.ResetEventLists ();
			TestGridItemPattern_AddRowAfter (provider, 1);

			Assert.IsNull (bridge.GetAutomationPropertyEventFrom (provider,
			                                                      GridItemPatternIdentifiers.RowProperty.Id),
			               string.Format ("{0} RowProperty event should not be raised.", provider.GetType ()));
			Assert.AreEqual (currentRow,
			                 gridItemProvider.Row,
			                 "Current row is different than value before adding row after");

			// Remove previous added row, so our RowProperty will change again!
			bridge.ResetEventLists ();
			TestGridItemPattern_RemoveRowBefore (provider, 1);

			tuple = bridge.GetAutomationPropertyEventFrom (provider,
			                                               GridItemPatternIdentifiers.RowProperty.Id);
			Assert.IsNotNull (tuple,
			                  string.Format ("{0} RowProperty event.", provider.GetType ()));

			Assert.AreEqual (currentRow, 
			                 (int) tuple.e.OldValue,
			                 "Old value should match value before removing row");
			Assert.AreEqual (oldValue,
			                 (int) tuple.e.NewValue,
			                 "New value should match value after removing row");
			Assert.AreEqual (oldValue,
			                 gridItemProvider.Row,
			                 "Current row is different than NewValue");

			// Remove row added after, to make sure our Row is not changing at all
			bridge.ResetEventLists ();
			TestGridItemPattern_RemoveRowAfter (provider, 1);

			Assert.IsNull (bridge.GetAutomationPropertyEventFrom (provider,
			                                                      GridItemPatternIdentifiers.RowProperty.Id),
			               string.Format ("{0} RowProperty event should not be raised.", provider.GetType ()));
			Assert.AreEqual (oldValue,
			                 gridItemProvider.Row,
			                 "Current row is different than value before removing row after");
		}

		protected void TestGridItemPattern_AddRowBefore (IRawElementProviderSimple provider, 
		                                                 int count)
		{
			while (count > 0) {
				TestGridItemPattern_AddRowBefore (provider);
				count--;
			}
		}

		protected virtual void TestGridItemPattern_AddRowBefore (IRawElementProviderSimple provider)
		{
			// This must be overridden by providers to actually add the row!
		}

		protected void TestGridItemPattern_AddRowAfter (IRawElementProviderSimple provider,
		                                                int count)
		{
			while (count > 0) {
				TestGridItemPattern_AddRowAfter (provider);
				count--;
			}
		}

		protected virtual void TestGridItemPattern_AddRowAfter (IRawElementProviderSimple provider)
		{
			// This must be overridden by providers to actually add the row!
		}

		protected void TestGridItemPattern_RemoveRowBefore (IRawElementProviderSimple provider,
		                                                    int count)
		{
			while (count > 0) {
				TestGridItemPattern_RemoveRowBefore (provider);
				count--;
			}
		}

		protected virtual void TestGridItemPattern_RemoveRowBefore (IRawElementProviderSimple provider)
		{
			// This must be overridden by providers to actually add the row!
		}

		protected void TestGridItemPattern_RemoveRowAfter (IRawElementProviderSimple provider,
		                                                   int count)
		{
			while (count > 0) {
				TestGridItemPattern_RemoveRowAfter (provider);
				count--;
			}
		}

		protected virtual void TestGridItemPattern_RemoveRowAfter (IRawElementProviderSimple provider)
		{
			// This must be overridden by providers to actually add the row!
		}

		protected virtual void TestEmbeddedImagePattern_All (IRawElementProviderSimple provider, bool imageExpected)
		{
			IEmbeddedImageProvider embeddedImage 
				= provider.GetPatternProvider (EmbeddedImagePatternIdentifiers.Pattern.Id) as IEmbeddedImageProvider;

			Assert.IsNotNull (embeddedImage, "Provider {0} is not implementing IEmbeddedImageProvider");
			Assert.AreEqual (!imageExpected,
			                 embeddedImage.Bounds.IsEmpty,
			                 string.Format ("Image was {0}expected, but was {1}found", 
			                                imageExpected ? "" : "not ",
			                                embeddedImage.Bounds.IsEmpty ? "not " : ""));

			if (imageExpected) {
				Rect providerRect
					= (Rect) provider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
				Rect imageRect = embeddedImage.Bounds;

				Assert.IsTrue (imageRect.X >= providerRect.X,
				               string.Format ("Image X: {0} must be >= than container X: {1}", imageRect.X, providerRect.X));
				Assert.IsTrue (imageRect.Y >= providerRect.Y,
				               string.Format ("Image Y: {0} must be >= than container Y: {1}", imageRect.Y, providerRect.Y));
				Assert.IsTrue (imageRect.Width <= providerRect.Width,
				               string.Format ("Image Width: {0} must be <= than container Width: {1}", imageRect.Width, providerRect.Width));
				Assert.IsTrue (imageRect.Height <= providerRect.Height,
				               string.Format ("Image Height: {0} must be <= than container Height: {1}", imageRect.Height, providerRect.Height));
			}
		}

		private void TestSelectionItemPattern_RemoveFromSelection (IRawElementProviderFragment selectionParent,
		                                                           IRawElementProviderFragment selectionItemChild)
		{
			ISelectionItemProvider selectionItemProvider
				= selectionItemChild.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id) as ISelectionItemProvider;

			ISelectionProvider selectionProvider
				= selectionParent.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id) as ISelectionProvider;
			if (selectionProvider == null || selectionItemProvider == null)
				return;

			if (!selectionItemProvider.IsSelected)
				return;

			int selectedItems = selectionProvider.GetSelection ().Length;
			Assert.Greater (selectedItems, 0, "At least one item must be selected");

			bool selectionIsRequired = selectionProvider.IsSelectionRequired;
			bool exceptionCatch = false;
			
			try {
				bridge.ResetEventLists ();
				selectionItemProvider.RemoveFromSelection ();
				Assert.AreEqual (selectedItems - 1, 
				                 selectionProvider.GetSelection ().Length, 
				                 "Selection count inconsistent");

				// The only possible way to get the RemovedFromSelection Event 
				// is if there are more than 1 elements selected. 
				// We should/should not get the ElementRemovedFromSelectionEvent NOTHING ELSE from that provider
				if (selectedItems > 2)
					Assert.IsNotNull (bridge.GetAutomationEventFrom (selectionItemChild,
					                                                 SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent.Id),
					                  string.Format ("{0} ElementRemovedFromSelectionEvent event.", selectionItemChild.GetType ()));

				Assert.IsNull (bridge.GetAutomationEventFrom (selectionItemChild,
				                                              SelectionItemPatternIdentifiers.ElementSelectedEvent.Id),
				               string.Format ("{0} ElementSelectedEvent event.", selectionItemChild.GetType ()));

				Assert.IsNull (bridge.GetAutomationEventFrom (selectionItemChild,
				                                              SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent.Id),
				               string.Format ("{0} ElementAddedToSelectionEvent event.", selectionItemChild.GetType ()));
				
				// IsSelectedProperty
				Assert.IsNotNull (bridge.GetAutomationPropertyEventFrom (selectionItemChild,
				                                                         SelectionItemPatternIdentifiers.IsSelectedProperty.Id),
				                  string.Format ("{0} IsSelectedProperty event.", selectionItemChild.GetType ()));
				
			} catch (InvalidOperationException) {				
				if (!selectionProvider.CanSelectMultiple && selectionProvider.IsSelectionRequired) {
					// RemoveFromSelection is called on a single-selection 
					// container where IsSelectionRequiredProperty = true and an element is already selected.
				} else if (selectionProvider.CanSelectMultiple && selectionProvider.GetSelection ().Length == 1) {
					// RemoveFromSelection is called on a multiple-selection container where
					// IsSelectionRequiredProperty = true and only one element is selected.
				} else
					Assert.Fail ("You should not be throwing InvalidOperationException when calling RemoveFromSelection.");

				exceptionCatch = true;
			}
			
			Assert.AreEqual (exceptionCatch, selectionIsRequired, "Exception not thrown");
		}
		
		private void ValidateToggleState (ToggleState old, ToggleState got)
		{
			if (old == ToggleState.On)
				Assert.AreEqual (ToggleState.Off, got, "On -> Off");
			else if (old == ToggleState.Off) {
				if (got != ToggleState.Indeterminate 
				    && got != ToggleState.On)
					Assert.Fail ("Off -> Indeterminate or On");
			} else if (old == ToggleState.Indeterminate)
				Assert.AreEqual (ToggleState.On, got, "Indeterminate -> On");
		}

		#endregion
		
	}
	
}
