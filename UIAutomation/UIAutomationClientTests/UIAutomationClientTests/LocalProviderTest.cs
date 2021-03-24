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
// Copyright (c) 2010 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//  Matt Guo <matt@mattguo.com>
// 

using System;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;
using NUnit.Framework;
using At = System.Windows.Automation.Automation;
using SW = System.Windows;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class LocalProviderTest
	{
		const int FakeHandle = 12345;

		private AutomationElement simple = null;
		private AutomationElement simple2 = null;
		private AutomationElement child = null;
		private AutomationElement root = null;
		private CustomProviderSimple simpleProvider =null;
		private CustomProviderSimple simpleProvider2 =null;
		private CustomProviderFragment childProvider =null;
		private CustomProviderRoot rootProvider =null;

		[TestFixtureSetUp]
		public virtual void FixtureSetUp ()
		{
			simpleProvider = new CustomProviderSimple ();
			simpleProvider2 = new CustomProviderSimple (FakeHandle);
			childProvider = new CustomProviderFragment (null);
			rootProvider = new CustomProviderRoot (childProvider);
			childProvider.Root = rootProvider;
			simple = AutomationElement.FromLocalProvider (simpleProvider);
			simple2 = AutomationElement.FromLocalProvider (simpleProvider2);
			child = AutomationElement.FromLocalProvider(childProvider);
			root = AutomationElement.FromLocalProvider(rootProvider);
		}

		#region Test Methods
		[Test]
		public void PropertyTest ()
		{
			Assert.AreEqual ("Custom Child", child.Current.Name);
			Assert.AreEqual (ControlType.TabItem, child.Current.ControlType);
			Assert.AreEqual ("Custom Root", root.Current.Name);
			Assert.AreEqual (ControlType.Tab, root.Current.ControlType);
		}

		// IRawElementFragment's GetRuntimeId method will override
		// the value returned by GetPropertyValue
		[Test]
		public void RuntimeIdOverrideTest ()
		{
			var rid1 = root.GetRuntimeId ();
			var rid2 = (int []) root.GetCurrentPropertyValue (AEIds.RuntimeIdProperty);
			Assert.AreEqual (rid1, rid2, "rid1 == rid2");
			Assert.AreEqual (CustomProviderBase.CustomRuntimeIdPrefix, rid1 [0], "Check rid1");
			Assert.AreEqual (CustomProviderBase.CustomRuntimeIdPrefix, rid2 [0], "Check rid2");

			// RuntimeId is null even if it is explicitly returned by
			// IRawElementSimple.GetPropertyValue
			Assert.IsNull (simple.GetRuntimeId (), "simple.GetRuntimeId ()" );
			Assert.IsNull (simple.GetCurrentPropertyValue (AEIds.RuntimeIdProperty),
			               "simple.GetGetCurrentPropertyValue (RuntimeId)" );

			// However if the IRawElementSimple has NativeHandleProperty,
			// then UIA will generate a runtime id for the provider,
			// on Windows 7 the runtime id is [42, NativeHandleValue]
			Assert.IsNotNull (simple2.GetRuntimeId (), "simple2.GetRuntimeId ()" );
		}

		// IRawElementFragment's BoundingRectangle property will override
		// the value returned by GetPropertyValue
		[Test]
		public void BoundingRectangleOverrideTest ()
		{
			var bound = root.Current.BoundingRectangle;
			Assert.AreEqual (200.0, bound.Width, "bound.Width");

			// BoundingRectangle is empty even if it is explicitly returned by
			// IRawElementSimple.GetPropertyValue
			Assert.IsTrue (simple.Current.BoundingRectangle.IsEmpty);
		}

		[Test]
		public void DefaultPropertyValueTest ()
		{
			// LocalizedControlType is compatible with the ControlType though
			// LocalizedControlType is not explicitly returned by ControlType
			Assert.AreEqual (ControlType.Tab.LocalizedControlType, root.Current.LocalizedControlType,
				"root.LocalizedControlType");
			// IsInvokePatternAvailableProperty is automatically set to true as long as
			// the pattern can be returned by GetPatternProvider
			Assert.IsTrue ((bool) child.GetCurrentPropertyValue (AEIds.IsInvokePatternAvailableProperty),
				"child.IsInvokePatternAvailable");
			Assert.IsFalse ((bool) root.GetCurrentPropertyValue (AEIds.IsInvokePatternAvailableProperty),
				"root.IsInvokePatternAvailable");
		}

		[Test]
		public void FromPointTest ()
		{
			var element = AutomationElement.FromPoint (new SW.Point (100, 100));
			// though child and root defined their bounds, they won't be returned by
			// AutomationElement.FromPoint, actually on Windows what returned is the
			// "Desktop" element.
			Assert.AreNotEqual (element, child, "child is never returned by FromPoint");
			Assert.AreNotEqual (element, root, "root is never returned by FromPoint");
		}

		[Test]
		public void FromHandleTest ()
		{
			BaseTest.AssertRaises <ElementNotAvailableException> (
				() =>  AutomationElement.FromHandle (new IntPtr (FakeHandle)),
				"simple2 is never returned by FromHandle");
		}

		[Test]
		public void FocusedElementTest ()
		{
			BaseTest.AssertRaises <InvalidOperationException> (
				() => child.SetFocus (), "child.IsKeyboardFocusable is not set");
			// root.IsKeyboardFocusable is set to ture, so no exception
			root.SetFocus ();
			Assert.AreNotEqual (AutomationElement.FocusedElement, root,
				"root is never returned by FocusedElement");
		}

		[Test]
		public void PatternTest ()
		{
			childProvider.ClickCount = 0;
			var ip = (InvokePattern) child.GetCurrentPattern (InvokePattern.Pattern);
			ip.Invoke ();
			Assert.AreEqual (1, childProvider.ClickCount);
		}

		[Test]
		public void EventTest ()
		{
			int eventCount = 0;
			AutomationEventHandler handler = (o, e) => eventCount++;
			At.AddAutomationEventHandler (InvokePattern.InvokedEvent,
				child, TreeScope.Element, handler);
			childProvider.PerformInvoke ();
			Thread.Sleep (500);
			Assert.AreEqual (1, eventCount);
		}

		[Test]
		public void ControlTreeTest ()
		{
			Assert.AreEqual (null, TreeWalker.RawViewWalker.GetParent (root), "root's parent");
			Assert.AreEqual (child, TreeWalker.RawViewWalker.GetFirstChild (root), "root's first child");
			Assert.AreEqual (child, TreeWalker.RawViewWalker.GetLastChild (root), "root's last child");
			Assert.AreEqual (null, TreeWalker.RawViewWalker.GetPreviousSibling (root), "root's prev sibling");
			Assert.AreEqual (null, TreeWalker.RawViewWalker.GetNextSibling (root), "root's next sibling");
			Assert.AreEqual (child, root.FindFirst (TreeScope.Descendants, Condition.TrueCondition));
			Assert.AreEqual (1,
				root.FindAll (TreeScope.Descendants,
					new PropertyCondition (AEIds.ControlTypeProperty, ControlType.TabItem)).Count,
				"root's TabItem descendants");
			Assert.AreEqual (0,
				root.FindAll (TreeScope.Descendants,
					new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button)).Count,
				"root's Button descendants");

			Assert.AreEqual (root, TreeWalker.RawViewWalker.GetParent (child), "child's parent");
			Assert.AreEqual (null, TreeWalker.RawViewWalker.GetFirstChild (child), "child's first child");
			Assert.AreEqual (null, TreeWalker.RawViewWalker.GetLastChild (child), "child's last child");
			Assert.AreEqual (null, TreeWalker.RawViewWalker.GetPreviousSibling (child), "child's prev sibling");
			Assert.AreEqual (null, TreeWalker.RawViewWalker.GetNextSibling (child), "child's next sibling");
			Assert.AreEqual (0, child.FindAll (TreeScope.Descendants, Condition.TrueCondition).Count, "child's children count");
		}
		#endregion
	}

	internal class CustomProviderSimple : IRawElementProviderSimple
	{
		private int handle = -1;

		public CustomProviderSimple ()
		{
		}

		public CustomProviderSimple (int handle)
		{
			this.handle = handle;
		}

		#region IRawElementProviderSimple Members

		public virtual object GetPatternProvider (int patternId)
		{
			return null;
		}

		public virtual object GetPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.NameProperty.Id)
				return "Custom Simple";
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Pane.Id;
			else if (propertyId == AEIds.RuntimeIdProperty.Id)
				return new int[] {1, 2, 3, 4, 5};
			else if (propertyId == AEIds.BoundingRectangleProperty.Id)
				return new SW.Rect (0, 0, 1000, 1000);
			else if (propertyId == AEIds.NativeWindowHandleProperty.Id && handle != -1)
				return handle;
			else
				return null;
		}

		public IRawElementProviderSimple HostRawElementProvider {
			get { return null; }
		}

		public ProviderOptions ProviderOptions {
			get { return ProviderOptions.ClientSideProvider; }
		}

		#endregion
	}

	internal class CustomProviderBase : IRawElementProviderFragment
	{
		public const int CustomRuntimeIdPrefix = 8888;
		private int[] runtimeId = null;
		private SW.Rect bound = new SW.Rect (50.0, 50.0, 200.0, 200.0);

		public CustomProviderBase (IRawElementProviderFragmentRoot root)
		{
			this.Root = root;
		}

		public IRawElementProviderFragmentRoot Root { get; set; }

		#region IRawElementProviderFragment Members

		public SW.Rect BoundingRectangle {
			get { return bound; }
		}

		public IRawElementProviderFragmentRoot FragmentRoot {
			get { return Root; }
		}

		public IRawElementProviderSimple[] GetEmbeddedFragmentRoots ()
		{
			return new IRawElementProviderSimple[0];
		}

		public int[] GetRuntimeId ()
		{
			if (runtimeId == null) {
				byte [] bytes = Guid.NewGuid ().ToByteArray ();
				runtimeId = new int [bytes.Length + 1];
				runtimeId [0] = CustomRuntimeIdPrefix;
				for (int i = 0; i < bytes.Length; i++)
					runtimeId [i + 1] = bytes [i];
			}
			return runtimeId;
		}

		public virtual IRawElementProviderFragment Navigate (NavigateDirection direction)
		{
			if (direction == NavigateDirection.Parent)
				return Root;
			else
				return null;
		}

		public void SetFocus ()
		{
		}

		#endregion

		#region IRawElementProviderSimple Members

		public virtual object GetPatternProvider (int patternId)
		{
			return null;
		}

		public virtual object GetPropertyValue (int propertyId)
		{
			return null;
		}

		public IRawElementProviderSimple HostRawElementProvider {
			get { return null; }
		}

		public ProviderOptions ProviderOptions {
			get { return ProviderOptions.ClientSideProvider; }
		}

		#endregion
	}

	internal class CustomProviderFragment : CustomProviderBase
	{
		public CustomProviderFragment (IRawElementProviderFragmentRoot root)
			: base (root)
		{
			ClickCount = 0;
		}

		public int ClickCount { get; set; }
		public void PerformInvoke()
		{
			ClickCount++;
			AutomationInteropProvider.RaiseAutomationEvent (this,
				new AutomationEventArgs(InvokePattern.InvokedEvent));
		}

		public override object GetPatternProvider (int patternId)
		{
			if (patternId == InvokePattern.Pattern.Id)
				return new CustomInvokeProvider (this);
			return base.GetPatternProvider (patternId);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.NameProperty.Id)
				return "Custom Child";
			else if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.TabItem.Id;
			else
				return base.GetPropertyValue (propertyId);
		}
	}

	internal class CustomInvokeProvider : IInvokeProvider
	{
		private CustomProviderFragment provider = null;

		public CustomInvokeProvider (CustomProviderFragment provider)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");
			this.provider = provider;
		}

		#region IInvokeProvider implementation

		public void Invoke ()
		{
			provider.PerformInvoke ();
		}

		#endregion
	}

	internal class CustomProviderRoot : CustomProviderBase, IRawElementProviderFragmentRoot
	{
		public IRawElementProviderFragment Child { get; set; }

		public CustomProviderRoot (IRawElementProviderFragment child)
			: base (null)
		{
			this.Child = child;
		}

		#region IRawElementProviderFragmentRoot Members

		public IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			return Child;
		}

		public IRawElementProviderFragment GetFocus ()
		{
			return null;
		}

		#endregion

		public override IRawElementProviderFragment Navigate (NavigateDirection direction)
		{
			if (direction == NavigateDirection.FirstChild || direction == NavigateDirection.LastChild)
				return Child;
			else
				return null;
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.NameProperty.Id)
				return "Custom Root";
			else if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Tab.Id;
			else if (propertyId == AEIds.RuntimeIdProperty.Id)
				// this return value won't be effective since the base class defined GetRuntimeId method
				return new int[] {1, 2, 3, 4, 5};
			else if (propertyId == AEIds.BoundingRectangleProperty.Id)
				// this return value won't be effective since the base class defined BoundingRectangle property
				return new SW.Rect (0, 0, 1000, 1000);
			else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
				return true;
			else
				return base.GetPropertyValue (propertyId);
		}
	}
}
