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
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms.Client
{
	public abstract class BaseTest
	{

		#region Setup/Teardown

		[SetUp]
		public virtual void SetUp ()
		{
			form = new Form ();
			formElement = AutomationElement.FromHandle (form.Handle);
		}

		[TearDown]
		public virtual void TearDown ()
		{
			form.Dispose ();
			formElement = null;
		}

		#endregion

		#region Automation Properties Tests

		[Test]
		[Ignore ("No idea how to test")]
		public virtual void MsdnAutomationIdPropertyTest ()
		{
		}

		[Test]
		[Ignore ("No idea how to test")]
		public virtual void MsdnBoundingRectanglePropertyTest ()
		{
		}

		[Test]
		[Description ("Implementation in BaseTest. Using true as default value for IsKeyboardFocusable")]
		public virtual void MsdnIsKeyboardFocusablePropertyTest ()
		{
			AutomationElement child = GetAutomationElement ();
			Assert.AreEqual (true,
				child.GetCurrentPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty, true),
				"IsKeyboardFocusable");
		}

		[Test]
		public abstract void MsdnHelpTextPropertyTest ();

		[Test]
		public abstract void MsdnNamePropertyTest ();

		[Test]
		[Ignore ("No idea how to test")]
		public virtual void MsdnClickablePointPropertyTest ()
		{
		}

		[Test]
		public abstract void MsdnControlTypePropertyTest ();

		[Test]
		public abstract void MsdnLabeledByPropertyTest ();

		[Test]
		public abstract void MsdnLocalizedControlTypePropertyTest ();

		[Test]
		public abstract void MsdnIsContentElementPropertyTest ();

		[Test]
		public abstract void MsdnIsControlElementPropertyTest ();

		[Test]
		public abstract void MsdnOrientationPropertyTest ();

		[Test]
		public virtual void MsdnIsOffscreenPropertyTest()
		{
		}

		[Test]
		public virtual void MsdnAcceleratorKeyPropertyTest()
		{
		}

		[Test]
		public virtual void MsdnAccessKeyPropertyTest()
		{
		}

		[Test]
		public virtual void MsdnIsPasswordPropertyTest()
		{
                }

		#endregion Automation Properties Tests

		#region Automation Patterns Tests

		[Test]
		public virtual void MsdnDockPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, DockPatternIdentifiers.Pattern),
				string.Format ("DockPattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnExpandCollapsePatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, ExpandCollapsePattern.Pattern),
				string.Format ("ExpandCollapse SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnGridPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, GridPatternIdentifiers.Pattern),
				string.Format ("GridPattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnGridItemPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, GridItemPatternIdentifiers.Pattern),
				string.Format ("GridItemPattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnInvokePatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, InvokePatternIdentifiers.Pattern),
				string.Format ("InvokePattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnMultipleViewPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, MultipleViewPatternIdentifiers.Pattern),
				string.Format ("MultipleViewPattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnRangeValuePatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, RangeValuePatternIdentifiers.Pattern),
				string.Format ("RangeValuePattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnScrollPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, ScrollPatternIdentifiers.Pattern),
				string.Format ("ScrollPattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnScrollItemPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, ScrollItemPatternIdentifiers.Pattern),
				string.Format ("ScrollItemPattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnSelectionPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, SelectionPatternIdentifiers.Pattern),
				string.Format ("SelectionPattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnSelectionItemPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, SelectionItemPatternIdentifiers.Pattern),
				string.Format ("SelectionItemPattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnTablePatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, TablePatternIdentifiers.Pattern),
				string.Format ("TablePattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnTableItemPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, TableItemPatternIdentifiers.Pattern),
				string.Format ("TableItemPattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnTogglePatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, TogglePatternIdentifiers.Pattern),
				string.Format ("TogglePattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnTransformPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, TransformPatternIdentifiers.Pattern),
				string.Format ("TransformPattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnValuePatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, ValuePatternIdentifiers.Pattern),
				string.Format ("ValuePattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnWindowPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, WindowPatternIdentifiers.Pattern),
				string.Format ("WindowPattern SHOULD NOT be supported: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		[Test]
		public virtual void MsdnTextPatternTest ()
		{
			AutomationElement element = GetAutomationElement ();
			Assert.IsFalse (SupportsPattern (element, TextPatternIdentifiers.Pattern),
				string.Format ("TextPattern SHOULD NOT be supported in: {0} -> {1}",
				GetControl ().GetType (), GetControlTypeFromElement (element).ProgrammaticName));
		}

		#endregion 

		#region Protected Properties

		protected Form Form {
			get { return form; }
		}

		protected AutomationElement FormElement {
			get { return formElement; }
		}

		#endregion Protected Properties

		#region Protected Methods

		protected ControlType GetControlTypeFromElement (AutomationElement element)
		{
			ControlType controlType 
				= element.GetCurrentPropertyValue (AutomationElementIdentifiers.ControlTypeProperty) as ControlType;
			return controlType;
		}

		protected abstract Control GetControl ();

		protected virtual AutomationElement GetAutomationElement ()
		{
			return GetAutomationElementFromControl (GetControl ());
		}

		protected AutomationElement GetAutomationElementFromControl (Control control)
		{
			if (Form != control) {
				if (Form.Controls.Contains (control) == false)
					Form.Controls.Remove (control);
				Form.Controls.Add (control);
				Form.Show ();
			}

			return AutomationElement.FromHandle (control.Handle);
		}

		protected void TestProperty (AutomationElement element, 
			AutomationProperty property, 
			object expectedValue)
		{
			Assert.AreEqual (expectedValue, 
				element.GetCurrentPropertyValue (property),
				property.ProgrammaticName);
		}

		protected bool SupportsPattern (AutomationElement element, AutomationPattern pattern)
		{
			object rtnPattern;
			return element.TryGetCurrentPattern (pattern, out rtnPattern);
		}

		#endregion Protected Methods

		#region Private Fields

		private Form form;
		private AutomationElement formElement;

		#endregion Private Fields
	}
}
