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
#if __MonoCS__
#else
		private AutomationElement child = null;
		private AutomationElement root = null;
		private CustomProviderFragment childProvider =null;
		private CustomProviderRoot rootProvider =null;

		[TestFixtureSetUp]
		public virtual void FixtureSetUp ()
		{
			childProvider = new CustomProviderFragment (null);
			rootProvider = new CustomProviderRoot(childProvider);
			childProvider.Root = rootProvider;
			child = AutomationElement.FromLocalProvider(childProvider);
			root = AutomationElement.FromLocalProvider(rootProvider);
		}

		#region Test Methods
		[Test]
		public void PropertyTest ()
		{
			Assert.AreEqual ("Custom Simple", child.Current.Name);
			Assert.AreEqual (ControlType.TabItem, child.Current.ControlType);
			Assert.AreEqual ("Custom Root", root.Current.Name);
			Assert.AreEqual (ControlType.Tab, root.Current.ControlType);
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

	internal class CustomProviderBase : IRawElementProviderFragment
	{
		private int[] runtimeId = null;
		private SW.Rect rect = new SW.Rect (100.0, 100.0, 200.0, 200.0);

		public CustomProviderBase (IRawElementProviderFragmentRoot root)
		{
			this.Root = root;
		}

		public IRawElementProviderFragmentRoot Root { get; set; }

		#region IRawElementProviderFragment Members

		public SW.Rect BoundingRectangle {
			get { return rect; }
		}

		public IRawElementProviderFragmentRoot FragmentRoot {
			get { return Root; }
		}

		public IRawElementProviderSimple[] GetEmbeddedFragmentRoots ()
		{
			return new IRawElementProviderSimple[0];
		}

		public int [] GetRuntimeId ()
		{
			const int CustomPrefix = 8888;
			if (runtimeId == null) {
				byte [] bytes = new Guid ().ToByteArray ();
				runtimeId = new int [bytes.Length + 1];
				runtimeId [0] = CustomPrefix;
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

		public void PerformInvoke ()
		{
			ClickCount++;
			AutomationInteropProvider.RaiseAutomationEvent(InvokePattern.InvokedEvent, this,
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
				return "Custom Simple";
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
			else
				return base.GetPropertyValue (propertyId);
		}
#endif
	}
}
