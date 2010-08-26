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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
//
// Authors:
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using SWA = System.Windows.Automation;
using System.Windows.Automation.Text;

using At = System.Windows.Automation.Automation;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;
using MonoTests.System.Windows.Automation;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class CacheRequestTest : BaseTest
	{
		// TODO: Test behavior of AutomationElements passed to StructureChanged event handlers which were signed up while there was an active CacheRequest:
		// TODO: Out-of-order calls to Dispose
		// TODO: Test GetUpdatedCache with AutomationElementMode.None
		// TODO: Test GetCurrentPropertyValue with AutomationElementMode.None
		// TODO: CachedChildren/CachedParent with GetUpdatedCache (activated and not)

		private readonly CacheRequest originalCurrent = CacheRequest.Current;
		private int eventCount;
		private AutomationElement eventElement;
		private AutomationElement hScrollBarElement;

		[SetUp]
		public void SetUp ()
		{
			while (CacheRequest.Current != originalCurrent)
				CacheRequest.Current.Pop ();
			Assert.AreEqual (originalCurrent, CacheRequest.Current,
				"At SetUp time, CacheRequest.Current should be set back to the original request");

			// to enable textBox3's horizontal scroll bar.
			RunCommand ("set textBox3 long text");

			AndCondition horizontalScrollCondition =
				new AndCondition (
					new PropertyCondition (AutomationElementIdentifiers.ControlTypeProperty,
						ControlType.ScrollBar),
					new PropertyCondition (AutomationElementIdentifiers.OrientationProperty,
						OrientationType.Horizontal));
			hScrollBarElement = textbox3Element.FindFirst (TreeScope.Children, horizontalScrollCondition);
			Assert.IsNotNull (hScrollBarElement);

		}

		[TearDown]
		public void TearDown ()
		{
			Assert.AreEqual (originalCurrent, CacheRequest.Current,
				"At TearDown time, CacheRequest.Current should be set back to the original request");
		}

		[Test]
		public void ActiveRequestModificationTest ()
		{
			// Test modifying originalCurrent
			AssertRaises<InvalidOperationException> (
				() => originalCurrent.Add (AutomationElement.ControlTypeProperty),
				"calling Add on original CacheRequest");
			AssertRaises<InvalidOperationException> (
				() => originalCurrent.AutomationElementMode = AutomationElementMode.None,
				"setting AutomationElementMode on original CacheRequest");
			AssertRaises<InvalidOperationException> (
				() => originalCurrent.TreeScope = TreeScope.Children,
				"setting TreeScope on original CacheRequest");
			AssertRaises<InvalidOperationException> (
				() => originalCurrent.TreeFilter = SWA.Automation.ContentViewCondition,
				"setting TreeFilter on original CacheRequest");

			var request = new CacheRequest ();
			request.Add (AutomationElement.NameProperty);
			request.Push ();

			AssertRaises<InvalidOperationException> (
				() => request.Add (AutomationElement.ControlTypeProperty),
				"calling Add on active CacheRequest");
			AssertRaises<InvalidOperationException> (
				() => request.AutomationElementMode = AutomationElementMode.None,
				"setting AutomationElementMode on active CacheRequest");
			AssertRaises<InvalidOperationException> (
				() => request.TreeScope = TreeScope.Children,
				"setting TreeScope on active CacheRequest");
			AssertRaises<InvalidOperationException> (
				() => request.TreeFilter = SWA.Automation.ContentViewCondition,
				"setting TreeFilter on active CacheRequest");

			var request2 = new CacheRequest ();
			request2.Add (AutomationElement.ControlTypeProperty);
			request2.Push ();

			AssertRaises<InvalidOperationException> (
				() => request.Add (AutomationElement.ControlTypeProperty),
				"calling Add on inactive CacheRequest that's still on the stack");
			AssertRaises<InvalidOperationException> (
				() => request.AutomationElementMode = AutomationElementMode.None,
				"setting AutomationElementMode on inactive CacheRequest that's still on the stack");
			AssertRaises<InvalidOperationException> (
				() => request.TreeScope = TreeScope.Children,
				"setting TreeScope on inactive CacheRequest that's still on the stack");
			AssertRaises<InvalidOperationException> (
				() => request.TreeFilter = SWA.Automation.ContentViewCondition,
				"setting TreeFilter on inactive CacheRequest that's still on the stack");

			request2.Pop ();

			var request3 = new CacheRequest ();
			var disposable3 = request3.Activate ();

			AssertRaises<InvalidOperationException> (
				() => request.Add (AutomationElement.ControlTypeProperty),
				"calling Add on inactive CacheRequest that's still on the stack");
			AssertRaises<InvalidOperationException> (
				() => request.AutomationElementMode = AutomationElementMode.None,
				"setting AutomationElementMode on inactive CacheRequest that's still on the stack");
			AssertRaises<InvalidOperationException> (
				() => request.TreeScope = TreeScope.Children,
				"setting TreeScope on inactive CacheRequest that's still on the stack");
			AssertRaises<InvalidOperationException> (
				() => request.TreeFilter = SWA.Automation.ContentViewCondition,
				"setting TreeFilter on inactive CacheRequest that's still on the stack");

			request2.Push ();

			AssertRaises<InvalidOperationException> (
				() => request3.Add (AutomationElement.ControlTypeProperty),
				"calling Add on activated CacheRequest that's not on top of stack");
			AssertRaises<InvalidOperationException> (
				() => request3.AutomationElementMode = AutomationElementMode.None,
				"setting AutomationElementMode on activated CacheRequest that's not on top of stack");
			AssertRaises<InvalidOperationException> (
				() => request3.TreeScope = TreeScope.Children,
				"setting TreeScope on activated CacheRequest that's not on top of stack");
			AssertRaises<InvalidOperationException> (
				() => request3.TreeFilter = SWA.Automation.ContentViewCondition,
				"setting TreeFilter on activated CacheRequest that's not on top of stack");

			request2.Pop ();

			disposable3.Dispose ();

			request.Pop ();

			// Test modifying a request once it's no longer on the stack
			request.Add (AutomationElement.ControlTypeProperty);
			request.AutomationElementMode = AutomationElementMode.None;
			request.TreeScope = TreeScope.Children;
			request.TreeFilter = SWA.Automation.ContentViewCondition;
		}

		[Test]
		public void TreeWalkerTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.NameProperty);
			request.Push ();

			Condition buttonCondition = new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Button);
			TreeWalker buttonWalker = new TreeWalker (buttonCondition);

			var button7ElementRef2 = buttonWalker.GetFirstChild (groupBox1Element);

			AssertRaises<InvalidOperationException> (
				() => button7ElementRef2.Cached.Name.ToString (),
				"calling Cached.Name on element gotten from TreeWalker without specifying a CacheRequest");

			AssertRaises<ArgumentNullException> (
				() => button7ElementRef2 = buttonWalker.GetFirstChild (groupBox1Element, null),
				"passing null CacheRequest into TreeWalker.GetFirstChild");

			// Even inactive CacheRequests work just fine
			var inactiveRequest = new CacheRequest ();
			inactiveRequest.Add (AutomationElement.ControlTypeProperty);
			button7ElementRef2 = buttonWalker.GetFirstChild (groupBox1Element, inactiveRequest);
			Assert.AreEqual (ControlType.Button, button7ElementRef2.Cached.ControlType, "Cached.ControlType");
			AssertRaises<InvalidOperationException> (
				() => button7ElementRef2.Cached.Name.ToString (),
				"calling Cached.Name on element fetched from TreeWalker with (invalid) CacheRequest that doesn't specify NameProperty");

			button7ElementRef2 = buttonWalker.GetFirstChild (groupBox1Element, request);
			Assert.AreEqual (button7Element.Current.Name, button7ElementRef2.Cached.Name, "Cached.Name");
			AssertRaises<InvalidOperationException> (
				() => button7ElementRef2.Cached.ControlType.ToString (),
				"calling Cached.Name on element fetched from TreeWalker with (invalid) CacheRequest that doesn't specify ControlTypeProperty");

			AssertRaises<ArgumentNullException> (
				() => buttonWalker.GetFirstChild (null, request),
				"passing null element to GetFirstChild");
			AssertRaises<ArgumentNullException> (
				() => buttonWalker.GetFirstChild (groupBox1Element, null),
				"passing null request to GetFirstChild");

			request.Pop ();

			request = new CacheRequest ();
			request.Add (AutomationElement.NameProperty);
			request.TreeFilter = new PropertyCondition (AutomationElement.ControlTypeProperty, ControlType.Button);
			using (request.Activate ()) {
				button7ElementRef2 = button7Element.GetUpdatedCache (request);
				Assert.AreEqual (button7Element.Current.Name, button7ElementRef2.Cached.Name, "Cached.Name");
				// GetUpdatedCache works just fine, even for elements not in scope
				AutomationElement treeView1ElementRef2 = treeView1Element.GetUpdatedCache (request);
				Assert.AreEqual (treeView1Element.Current.Name, treeView1ElementRef2.Cached.Name, "Cached.Name");

				AutomationElement element = TreeWalker.RawViewWalker.GetFirstChild (testFormElement);
				AutomationElement elementRef2 = TreeWalker.RawViewWalker.GetFirstChild (testFormElement, request);
				Assert.AreEqual (element.Current.Name, elementRef2.Cached.Name, "Cached.Name");
				element = TreeWalker.RawViewWalker.GetFirstChild (treeView1Element);
				elementRef2 = TreeWalker.RawViewWalker.GetFirstChild (treeView1Element, request);
				// This works, too
				Assert.AreEqual (element.Current.Name, elementRef2.Cached.Name, "Cached.Name");
			}

			// TODO: More tests with restrictive TreeFilter/TreeScope
		}

		[Test]
		public void GetCachedPropertyValueTest ()
		{
			AssertRaises<InvalidOperationException> (
				() => groupBox1Element.GetCachedPropertyValue (AutomationElement.NameProperty),
				"calling GetCachedPropertyValue on an element with no cache");
			AssertRaises<InvalidOperationException> (
				() => groupBox1Element.GetCachedPropertyValue (AutomationElement.NameProperty, true),
				"calling GetCachedPropertyValue on an element with no cache");
			AssertRaises<InvalidOperationException> (
				() => groupBox1Element.GetCachedPropertyValue (AutomationElement.NameProperty, false),
				"calling GetCachedPropertyValue on an element with no cache");

			var request = new CacheRequest ();
			request.Add (AutomationElement.NameProperty);
			request.Push ();

			var groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);

			AssertRaises<InvalidOperationException> (
				() => groupBox1Element.GetCachedPropertyValue (AutomationElement.NameProperty),
				"calling GetCachedPropertyValue on an element instance fetched without an active cache request");

			Assert.AreEqual (groupBox1Element.Current.Name,
				groupBox1ElementRef2.GetCachedPropertyValue (AutomationElement.NameProperty));

			VerifyCachedPropertyValue (groupBox1ElementRef2,
				AEIds.NameProperty,
				groupBox1Element.Current.Name,
				groupBox1Element.Current.Name,
				groupBox1Element.Current.Name);

			AssertRaises<InvalidOperationException> (
				() => groupBox1ElementRef2.GetCachedPropertyValue (AutomationElement.OrientationProperty),
				"calling GetCachedPropertyValue for an uncached property");

			var request2 = new CacheRequest ();
			request2.Add (AutomationElement.OrientationProperty);
			request2.Push ();

			groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request2);

			VerifyCachedPropertyValue (groupBox1ElementRef2,
				AEIds.OrientationProperty,
				AutomationElement.NotSupported,
				OrientationType.None,
				OrientationType.None);

			request2.Pop ();
			request.Pop ();
		}

		[Test]
		public void CachedChildrenTest ()
		{
			// NOTE: This property is generally tested in other tests
			AssertRaises<InvalidOperationException> (
				() => testFormElement.CachedChildren.ToString (),
				"accessing CachedChildren on element retrieved with no active CacheRequest");
		}

		[Test]
		public void CachedParentTest ()
		{
			// NOTE: This property is generally tested in other tests
			AssertRaises<InvalidOperationException> (
				() => button1Element.CachedParent.ToString (),
				"accessing CachedParent on element retrieved with no active CacheRequest");
		}

		[Test]
		public void CachedTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.NameProperty);

			AutomationElement label1ElementRef2 = null;

			using (request.Activate ()) {
				label1ElementRef2 = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.ControlTypeProperty,
						ControlType.Text));

				var cache = label1ElementRef2.Cached;
				Assert.AreEqual ("label1", cache.Name);
				Assert.AreEqual ("label1", label1ElementRef2.Cached.Name);

				RunCommand ("click button1");
				Thread.Sleep (500);

				Assert.AreEqual ("button1_click", label1ElementRef2.Current.Name);

				Assert.AreEqual ("label1", cache.Name);
				Assert.AreEqual ("label1", label1ElementRef2.Cached.Name);
			}

			Assert.AreEqual ("label1", label1ElementRef2.Cached.Name);

			request = new CacheRequest ();
			request.Add (ValuePattern.ValueProperty);
			request.Add (ValuePattern.Pattern);

			ValuePattern txtCommandCachedValuePattern = null;

			using (request.Activate ()) {
				AutomationElement txtCommandElementRef2 = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.NameProperty, "txtCommand"));
				txtCommandCachedValuePattern = (ValuePattern)
					txtCommandElementRef2.GetCachedPattern (ValuePattern.Pattern);

				var cache = txtCommandCachedValuePattern.Cached;
				Assert.AreEqual ("click button1", cache.Value);
				Assert.AreEqual ("click button1", txtCommandCachedValuePattern.Cached.Value);

				RunCommand ("set textbox3 text");
				Thread.Sleep (500);

				Assert.AreEqual ("set textbox3 text", txtCommandCachedValuePattern.Current.Value);

				Assert.AreEqual ("click button1", cache.Value);
				Assert.AreEqual ("click button1", txtCommandCachedValuePattern.Cached.Value);
			}

			Assert.AreEqual ("click button1", txtCommandCachedValuePattern.Cached.Value);

			// TODO: Fix this test case on Linux (fails because textbox name is coming from label)
			//var request = new CacheRequest ();
			//request.Add (AutomationElement.NameProperty);

			//AutomationElement textbox3ElementRef2 = null;

			//using (request.Activate ()) {
			//        textbox3ElementRef2 = testFormElement.FindFirst (TreeScope.Children,
			//                new PropertyCondition (AEIds.ControlTypeProperty,
			//                        ControlType.Document));

			//        var cache = textbox3ElementRef2.Cached;
			//        Assert.AreEqual ("abcdefgabcdefgabcdef\n\n\n\n\n\n\n\ngabcdefgabcdefg", cache.Name);
			//        Assert.AreEqual ("abcdefgabcdefgabcdef\n\n\n\n\n\n\n\ngabcdefgabcdefg", textbox3ElementRef2.Cached.Name);

			//        RunCommand ("set textbox3 text");
			//        Thread.Sleep (500);

			//        Assert.AreEqual ("abcdefgabcdefgabcdef\n\n\n\n\n\n\n\ngabcdefgabcdefg", cache.Name);
			//        Assert.AreEqual ("abcdefgabcdefgabcdef\n\n\n\n\n\n\n\ngabcdefgabcdefg", textbox3ElementRef2.Cached.Name);
			//}

			//Assert.AreEqual ("abcdefgabcdefgabcdef\n\n\n\n\n\n\n\ngabcdefgabcdefg", textbox3ElementRef2.Cached.Name);
		}

		[Test]
		public void BasicPatternTest ()
		{
			var request = new CacheRequest ();
			request.Add (WindowPattern.CanMaximizeProperty);

			var testFormElementRef2 = testFormElement.GetUpdatedCache (request);

			Assert.AreEqual (testFormElement.GetCurrentPropertyValue (WindowPattern.CanMaximizeProperty),
				testFormElementRef2.GetCachedPropertyValue (WindowPattern.CanMaximizeProperty),
				"GetCachedPropertyValue");

			var windowPattern = (WindowPattern) testFormElementRef2.GetCurrentPattern (WindowPattern.Pattern);
			AssertRaises<InvalidOperationException> (
				() => windowPattern.Cached.ToString (),
				"accessing WindowPattern.Cached on pattern returned from GetCurrentPattern");

			AssertRaises<InvalidOperationException> (
				() => testFormElementRef2.GetCachedPattern (WindowPattern.Pattern),
				"calling GetCachedPattern when WindowPattern is not cached (even though one of its properties is)");
			object patternObj;
			Assert.IsFalse (testFormElementRef2.TryGetCachedPattern (WindowPattern.Pattern, out patternObj),
				"TryGetCachedPattern with uncached pattern");
			Assert.IsNull (patternObj, "output of TryGetCachedPattern when pattern not found");

			request.Add (WindowPattern.Pattern);
			testFormElementRef2 = testFormElement.GetUpdatedCache (request);

			AssertRaises<InvalidOperationException> (
				() => testFormElementRef2.GetCachedPattern (InvokePattern.Pattern),
				"calling GetCachedPattern with unsupported and uncached pattern");

			request.Add (InvokePattern.Pattern);
			testFormElementRef2 = testFormElement.GetUpdatedCache (request);

			AssertRaises<InvalidOperationException> (
				() => testFormElementRef2.GetCachedPattern (InvokePattern.Pattern),
				"calling GetCachedPattern with unsupported (but cached) pattern for the element");

			windowPattern = (WindowPattern) testFormElementRef2.GetCachedPattern (WindowPattern.Pattern);
			Assert.AreEqual (testFormElement.GetCurrentPropertyValue (WindowPattern.CanMaximizeProperty),
				windowPattern.Cached.CanMaximize,
				"WindowPattern.Cached.CanMaximize");
			AssertRaises<InvalidOperationException> (
				() => windowPattern.Cached.CanMinimize.ToString (),
				"accessing WindowPattern.Cached.CanMinimize when only WindowPattern and CanMaximizeProperty are cached");

			Assert.IsTrue (testFormElementRef2.TryGetCachedPattern (WindowPattern.Pattern, out patternObj),
				"TryGetCachedPattern with cached pattern");
			Assert.IsNotNull (patternObj as WindowPattern, "output of TryGetCachedPattern when pattern found");
		}

		[Test]
		public void AddTest ()
		{
			var request = new CacheRequest ();

			AutomationProperty nullProp = null;
			AssertRaises<ArgumentNullException> (
				() => request.Add (nullProp),
				"passing null property");

			AutomationPattern nullPattern = null;
			AssertRaises<ArgumentNullException> (
				() => request.Add (nullPattern),
				"passing null pattern");

			request.Add (AutomationElement.NameProperty);

			// Multiple identical calls to Add should be okay
			request.Add (AutomationElement.NameProperty);

			AutomationElement button1ElementRef2;
			using (request.Activate ()) {
				button1ElementRef2 = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.NameProperty,
						"button1"));
			}

			Assert.AreEqual (button1Element.Current.Name,
				button1ElementRef2.Current.Name,
				"Both references to button1Element should have the same Current.Name");

			Assert.AreEqual (button1Element.Current.Name,
				button1ElementRef2.Cached.Name,
				"button1Element Cached.Name should be the same as Current.Name");

			Assert.IsNotNull (button1Element.Cached,
				"Original button1Element reference can be accessed, even if its properties cannot");

			AssertRaises<InvalidOperationException> (
				() => button1Element.Cached.Name.ToString (),
				"checking for cached NameProperty on original button1Element reference");

			AssertRaises<InvalidOperationException> (
				() => button1ElementRef2.Cached.ControlType.ToString (),
				"checking for cached ControlTypeProperty, since it wasn't part of the CacheRequest");

			request.Add (AutomationElement.ControlTypeProperty);

			AssertRaises<InvalidOperationException> (
				() => button1ElementRef2.Cached.ControlType.ToString (),
				"checking for cached ControlTypeProperty, after adding to inactive CacheRequest");

			request.Activate ().Dispose ();

			AssertRaises<InvalidOperationException> (
				() => button1ElementRef2.Cached.ControlType.ToString (),
				"checking for cached ControlTypeProperty, after adding to inactive CacheRequest");

			using (request.Activate ()) {
				button1ElementRef2 = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.ControlTypeProperty,
						ControlType.Button));
			}

			Assert.AreEqual (button1Element.Current.ControlType,
				button1ElementRef2.Cached.ControlType,
				"ControlType caching should work after accessing element while request was active");
		}

		[Test]
		public void ActivateTest ()
		{
			var request1 = new CacheRequest ();
			var request2 = new CacheRequest ();
			request1.Add (AutomationElement.NameProperty);
			request2.Add (AutomationElement.ControlTypeProperty);

			using (request1.Activate ()) {
				Assert.AreEqual (request1, CacheRequest.Current, "request1 should be set to Current");
				var button1ElementRef2 = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.ControlTypeProperty,
						ControlType.Button));

				button1ElementRef2.Cached.Name.ToString ();

				AssertRaises<InvalidOperationException> (
					() => button1ElementRef2.Cached.ControlType.ToString (),
					"checking for cached ControlTypeProperty, though it is not added to an active CacheRequest");

				using (request2.Activate ()) {
					Assert.AreEqual (request2, CacheRequest.Current, "request2 should be set to Current");
					button1ElementRef2 = testFormElement.FindFirst (TreeScope.Children,
						new PropertyCondition (AEIds.ControlTypeProperty,
							ControlType.Button));

					AssertRaises<InvalidOperationException> (
						() => button1ElementRef2.Cached.Name.ToString (),
						"checking for cached NameProperty, when another CacheRequest has been activated");

					button1ElementRef2.Cached.ControlType.ToString ();
				}
				Assert.AreEqual (request1, CacheRequest.Current, "request1 should be set to Current after request2 is disposed");

				AssertRaises<InvalidOperationException> (
					() => button1ElementRef2.Cached.Name.ToString (),
					"checking for cached NameProperty, when another CacheRequest has been activated");

				button1ElementRef2.Cached.ControlType.ToString ();

				button1ElementRef2 = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.ControlTypeProperty,
						ControlType.Button));

				button1ElementRef2.Cached.Name.ToString ();

				AssertRaises<InvalidOperationException> (
					() => button1ElementRef2.Cached.ControlType.ToString (),
					"checking for cached ControlTypeProperty, it is not added to an active CacheRequest");
			}
			Assert.AreEqual (originalCurrent, CacheRequest.Current, "originalCurrent should be set to Current after request1 is disposed");

			// No error with multiple calls to Dispose
			var request3 = new CacheRequest ();
			var disposable3 = request3.Activate ();
			Assert.AreEqual (request3, CacheRequest.Current, "request3 should be set to Current after being activated");
			disposable3.Dispose ();
			Assert.AreEqual (originalCurrent, CacheRequest.Current, "originalCurrent should be set to Current after request3 is disposed");
			disposable3.Dispose ();
			Assert.AreEqual (originalCurrent, CacheRequest.Current, "originalCurrent should be set to Current after request3 is double-disposed");


			// Test double-activation
			// TODO: Disposal is out-of-order here, but not fully spec'd
			IDisposable disposable2a = request2.Activate ();
			Assert.AreEqual (request2, CacheRequest.Current, "request2 should be set to Current after being activated");
			IDisposable disposable2b = request2.Activate ();
			Assert.AreNotEqual (disposable2a, disposable2b, "Two activations of same request gives two different IDisposables");
			Assert.AreEqual (request2, CacheRequest.Current, "request2 should be set to Current after being activated again");
			disposable2a.Dispose ();
			Assert.AreEqual (request2, CacheRequest.Current, "request2 should be set to Current after its first activation is disposed");

			var button1ElementRef3 = testFormElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.Button));

			Assert.AreEqual (button1Element.Current.ControlType,
				button1ElementRef3.Cached.ControlType,
				"When a request is activated twice, and the first activation is disposed, the second activation should still be active");
			disposable2b.Dispose ();
			Assert.AreEqual (originalCurrent, CacheRequest.Current, "originalCurrent should be set to Current after second request2 activation is disposed");
		}

		[Test]
		public void ThreadingTest ()
		{
			CacheRequest originalRequest2 = null;
			CacheRequest request2 = null;
			CacheRequest current2a = null;
			CacheRequest current2b = null;
			CacheRequest current2c = null;
			AutomationElement testFormElementRef2 = null;
			EventWaitHandle ewh1 = new EventWaitHandle (false, EventResetMode.ManualReset);
			EventWaitHandle ewh2 = new EventWaitHandle (false, EventResetMode.ManualReset);
			bool threadComplete = false;

			var t = new Thread (() => {
				originalRequest2 = CacheRequest.Current;
				request2 = new CacheRequest ();
				request2.Add (AutomationElement.NameProperty);
				request2.Push ();
				current2a = CacheRequest.Current;
				testFormElementRef2 = AutomationElement.RootElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.ProcessIdProperty,
						p.Id));
				ewh1.Set ();
				ewh2.WaitOne ();

				current2b = CacheRequest.Current;
				request2.Pop ();
				current2c = CacheRequest.Current;
				ewh2.Reset ();
				ewh1.Set ();
				ewh2.WaitOne ();

				threadComplete = true;
				ewh1.Set ();
			});
			t.Start ();
			Assert.IsTrue (ewh1.WaitOne (500),
				"Timed out waiting on thread");

			Assert.AreSame (originalCurrent, originalRequest2,
				"No matter the thread, all stacks have the same original request on bottom");
			Assert.AreEqual (originalCurrent, CacheRequest.Current,
				"Even though there is an active request on another thread, Current returns the original request on this thread");
			Assert.AreEqual (request2, current2a,
				"Current on the other thread returns an active request");
			AssertRaises<InvalidOperationException> (
				() => request2.Pop (),
				"popping the active request from another thread");
			AssertRaises<InvalidOperationException> (
				() => request2.Add (AutomationElement.NameProperty),
				"calling Add on active request from another thread");
			request2.Push ();
			request2.Pop ();

			Assert.AreEqual (testFormElement.Current.Name,
				testFormElementRef2.Cached.Name,
				"Okay to use cached element from one thread in another thread");

			var testFormElementRef3 = testFormElement.GetUpdatedCache (request2);
			Assert.AreEqual (testFormElement.Current.Name,
				testFormElementRef3.Cached.Name,
				"Okay to call GetUpdatedCache using request from another thread");
			testFormElementRef3 = AutomationElement.RootElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ProcessIdProperty,
					p.Id));
			AssertRaises<InvalidOperationException> (
				() => testFormElementRef3.Cached.Name.ToString (),
				"accessing cached property from element retrieved when only active CacheRequest was in another thread");

			ewh1.Reset ();
			ewh2.Set ();
			Assert.IsTrue (ewh1.WaitOne (500),
				"Timed out waiting on thread");

			Assert.AreEqual (request2, current2b,
				"Current on the thread unaffected by changes in this thread's request stack");
			Assert.AreEqual (originalRequest2, current2c,
				"Current on the thread goes back to the original request after active request popped");

			Assert.IsFalse (threadComplete,
				"Thread should not have completed at this point");
			ewh1.Reset ();
			ewh2.Set ();
			Assert.IsTrue (ewh1.WaitOne (500),
				"Timed out waiting on thread");
			Assert.IsTrue (threadComplete,
				"Thread should have completed at this point");
		}

		[Test]
		public void CloneTest ()
		{
			var request = new CacheRequest ();
			request.AutomationElementMode = AutomationElementMode.None;
			request.TreeScope = TreeScope.Subtree;
			request.TreeFilter = new PropertyCondition (AutomationElement.NameProperty, "test1");

			var requestClone = request.Clone ();
			Assert.AreEqual (request.AutomationElementMode, requestClone.AutomationElementMode, "AutomationElementMode");
			Assert.AreEqual (request.TreeScope, requestClone.TreeScope, "TreeScope");
			Assert.AreSame (request.TreeFilter, requestClone.TreeFilter, "TreeFilter instance should be shared between original and clone");

			// TODO: Test that same things have been Add'ed
		}

		[Test]
		public void AutomationElementModeTest ()
		{
			var request = new CacheRequest ();
			Assert.AreEqual (AutomationElementMode.Full, request.AutomationElementMode, "Default AutomationElementMode value");

			request.AutomationElementMode = AutomationElementMode.None;
			request.Add (AutomationElement.NameProperty);

			using (request.Activate ()) {
				var groupBox1ElementRef2 = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.ControlTypeProperty,
						ControlType.Group));

				Assert.AreEqual (groupBox1Element.Current.Name,
					groupBox1ElementRef2.Cached.Name,
					"Cached.Name should be available on Element");

				// NOTE: Current is accessible, just not its properties
				AssertRaises<InvalidOperationException> (
					() => groupBox1ElementRef2.Current.Name.ToString (),
					"accessing property from Current with AutomationElementMode.None");
			}

			request.Add (InvokePattern.Pattern);
			request.Add (SelectionPattern.Pattern);
			request.Add (SelectionPattern.SelectionProperty);
			using (request.Activate ()) {
				AutomationElement selectionElement = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.ControlTypeProperty,
						ControlType.Tree));
			SelectionPattern selectionPattern = (SelectionPattern)
				selectionElement.GetCachedPattern (
					SelectionPattern.Pattern);
				selectionPattern.Cached.GetSelection ();
				AssertRaises<InvalidOperationException> (
					() => selectionPattern.Cached.CanSelectMultiple.ToString (),
					"Fetching a property that is not cached should throw InvalidOperationException");

				AutomationElement invokeElement = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.ControlTypeProperty,
						ControlType.Button));
			InvokePattern invokePattern = (InvokePattern)
				invokeElement.GetCachedPattern (
					InvokePattern.Pattern);
				// LAMESPEC: Calling Invoke should throw exception
				invokePattern.Invoke ();
			}
		}

		[Test]
		public void TreeFilterTest ()
		{
			var request = new CacheRequest ();
			Assert.AreEqual (SWA.Automation.ControlViewCondition, request.TreeFilter,
				"TreeFilter should default to ControlViewCondition");

			request.Add (AutomationElement.NameProperty);
			request.TreeScope = TreeScope.Subtree;

			request.TreeFilter = new PropertyCondition (AutomationElement.ControlTypeProperty,
				ControlType.Group);
			using (request.Activate ()) {
				var button1ElementRef2 = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.ControlTypeProperty,
						ControlType.Button));
				Assert.IsNull (button1ElementRef2,
					"AutomationElement.FindFirst should return null if element does not meet CacheRequest.TreeFilter");

				var groupBox1ElementRef2 = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.ControlTypeProperty,
						ControlType.Group));
				Assert.AreEqual (groupBox1ElementRef2.Current.Name,
					groupBox1ElementRef2.Cached.Name,
					"Cached.Name for element that does meet filter reqs");

				// NOTE: This behavior seems very strange considering TreeScope
				Assert.AreEqual (0, groupBox1ElementRef2.CachedChildren.Count,
					"groupBox1 should have 0 CachedChildren, even though it has two children that meet CacheRequest.Filter");
			}
		}

		[Test]
		public void TreeScopeTest ()
		{
			var request = new CacheRequest ();
			Assert.AreEqual (TreeScope.Element, request.TreeScope, "Default TreeScope value");

			AssertRaises<ArgumentException> (
				() => request.TreeScope = TreeScope.Ancestors,
				"setting to TreeScope.Ancestors");

			AssertRaises<ArgumentException> (
				() => request.TreeScope = TreeScope.Parent,
				"setting to TreeScope.Parent");

			AssertRaises<ArgumentException> (
				() => request.TreeScope = TreeScope.Element | TreeScope.Parent,
				"setting to TreeScope.Element | TreeScope.Parent");

			// TODO: Test Combined scopes that include parent/ancestors

			// TODO: TreeScope.Descendants

			request.Add (AutomationElement.NameProperty);

			//TreeScope.Element
			request.TreeScope = TreeScope.Element;
			using (request.Activate ()) {
				var groupBox1ElementRef2 = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.ControlTypeProperty,
						ControlType.Group));

				Assert.AreEqual (groupBox1Element.Current.Name,
					groupBox1ElementRef2.Cached.Name,
					"Cached.Name should be available on Element");

				AssertRaises<InvalidOperationException> (
					() => groupBox1ElementRef2.CachedChildren.ToString (),
					"accessing CachedChildren with TreeScope.Element");

				AssertRaises<InvalidOperationException> (
					() => groupBox1ElementRef2.CachedParent.ToString (),
					"accessing CachedParent with TreeScope.Element");
			}

			//TreeScope.Children
			request.TreeScope = TreeScope.Children;
			using (request.Activate ()) {
				var groupBox1ElementRef2 = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.ControlTypeProperty,
						ControlType.Group));

				AssertRaises<InvalidOperationException> (
					() => groupBox1ElementRef2.Cached.Name.ToString (),
					"accessing cached property on fetched element with TreeScope.Children");

				Assert.AreEqual (4, groupBox1ElementRef2.CachedChildren.Count, "CachedChildren.Count");
				string [] expectedChildNames = { "groupBox3", "groupBox2", "button3", "button2" };
				for (int i = 0; i < groupBox1ElementRef2.CachedChildren.Count; i++) {
					AutomationElement childElement = groupBox1ElementRef2.CachedChildren [i];
					Assert.AreEqual (expectedChildNames [i], childElement.Cached.Name,
						"Cached.Name for child " + i.ToString ());
					Assert.AreSame (groupBox1ElementRef2, childElement.CachedParent,
						"groupbox1ElementRef2 and childElement.CachedParent should point to same instance");
					// This fails because I guess CachedParent doesn't work too well when the parent wasn't included in the cache request
					//Assert.AreEqual (groupBox1ElementRef2.Current.Name, childElement.CachedParent.Cached.Name,
					//        "CachedParent should be available on CachedChildren");
					// This fails because AutomationElement.Equals relies on on RuntimeId comparison that fails...or maybe it's the RuntimeId fetching that fails
					//Assert.AreEqual (groupBox1ElementRef2, childElement.CachedParent,
					//        "CachedParent should be available on CachedChildren");

					AssertRaises<InvalidOperationException> (
						() => childElement.CachedChildren.ToString (),
						"accessing childElement.CachedChildren with TreeScope.Children");
				}

				AssertRaises<InvalidOperationException> (
					() => groupBox1ElementRef2.CachedParent.ToString (),
					"accessing CachedParent with TreeScope.Children");
			}

			//TreeScope.Subtree
			request.TreeScope = TreeScope.Subtree;
			using (request.Activate ()) {
				var groupBox1ElementRef2 = testFormElement.FindFirst (TreeScope.Children,
					new PropertyCondition (AEIds.ControlTypeProperty,
						ControlType.Group));

				Assert.AreEqual (groupBox1Element.Current.Name,
					groupBox1ElementRef2.Cached.Name,
					"Cached.Name should be available on Element with TreeScope.Subtree");

				Assert.AreEqual (4, groupBox1ElementRef2.CachedChildren.Count, "CachedChildren.Count");
				string [] expectedChildNames = { "groupBox3", "groupBox2", "button3", "button2" };
				Dictionary<string, string []> expectedGrandchildNames = new Dictionary<string, string []> ();
				expectedGrandchildNames ["groupBox3"] = new string [] { "button7", "button6" };
				expectedGrandchildNames ["groupBox2"] = new string [] { "button5", "checkBox1", "button4" };
				for (int i = 0; i < groupBox1ElementRef2.CachedChildren.Count; i++) {
					AutomationElement childElement = groupBox1ElementRef2.CachedChildren [i];
					Assert.AreEqual (expectedChildNames [i], childElement.Cached.Name,
						"Cached.Name for child " + i.ToString ());
					Assert.AreSame (groupBox1ElementRef2, childElement.CachedParent,
						"groupbox1ElementRef2 and childElement.CachedParent should point to same instance");

					if (expectedGrandchildNames.ContainsKey (expectedChildNames [i])) {
						Assert.AreEqual (expectedGrandchildNames [expectedChildNames [i]].Length,
							childElement.CachedChildren.Count,
							"childElement.CachedChildren.Count for " + expectedChildNames [i]);
						for (int j = 0; j < childElement.CachedChildren.Count; j++) {
							AutomationElement gcElement = childElement.CachedChildren [j];
							Assert.AreEqual (expectedGrandchildNames [expectedChildNames [i]] [j],
								gcElement.Cached.Name,
								"Cached.Name for grandchild " + i.ToString ());
						}
					} else {
						Assert.AreEqual (0, childElement.CachedChildren.Count, "Childless elements should have empty CachedChildren");
					}
				}

				AssertRaises<InvalidOperationException> (
					() => groupBox1ElementRef2.CachedParent.ToString (),
					"accessing CachedParent with TreeScope.Subtree");
			}
		}

		[Test]
		public void GetUpdatedCacheTest ()
		{
			AssertRaises<ArgumentNullException> (
				() => button1Element.GetUpdatedCache (null),
				"passing null to GetUpdatedCache");

			var request = new CacheRequest ();

			var updatedElement = button1Element.GetUpdatedCache (request);
			AssertRaises<InvalidOperationException> (
				() => updatedElement.Cached.Name.ToString (),
				"trying to access cached property on element returned by GetUpdatedCache, when no properties were specified in CacheRequest");

			request.Add (AutomationElement.NameProperty);

			updatedElement = button1Element.GetUpdatedCache (request);
			Assert.AreEqual (button1Element.Current.Name,
				updatedElement.Cached.Name,
				"updatedElement Cached.Name should be the same as Current.Name if request has NameProperty set, even if it has not been activated yet");

			AutomationElement button1ElementRef2;
			using (request.Activate ()) {
				button1ElementRef2 = button1Element.GetUpdatedCache (request);
			}

			Assert.AreEqual (button1Element.Current.Name,
				button1ElementRef2.Current.Name,
				"Both references to button1Element should have the same Current.Name");

			Assert.AreEqual (button1Element.Current.Name,
				button1ElementRef2.Cached.Name,
				"button1Element Cached.Name should be the same as Current.Name");

			AssertRaises<InvalidOperationException> (
				() => button1Element.Cached.Name.ToString (),
				"trying to access cache on original element instance after GetUpdatedCache is called");

			updatedElement = button1Element.GetUpdatedCache (request);
			Assert.AreEqual (button1Element.Current.Name,
				updatedElement.Cached.Name,
				"updatedElement Cached.Name should be the same as Current.Name if request has NameProperty set, even if it is no longer active");
		}

		[Test]
		public void PopCurrentAndExceptionTest ()
		{
			var request1 = new CacheRequest ();
			var request2 = new CacheRequest ();

			// NOTE: It appears that at the bottom of the CacheRequest
			//       stack is an inactive request with bizarre behavior:
			// TODO: Test if any properties or patterns have been added to originalCurrent

			// Verify that originalCurrent has all the default CacheRequest settings
			Assert.AreEqual (AutomationElementMode.Full, originalCurrent.AutomationElementMode,
				"Original Current.AutomationElementMode");
			Assert.AreEqual (TreeScope.Element, originalCurrent.TreeScope,
				"Original Current.TreeScope");
			Assert.AreEqual (SWA.Automation.ControlViewCondition, originalCurrent.TreeFilter,
				"Original Current.TreeFilter");

			// Cannot pop originalCurrent, even though it is not really active
			AssertRaises<InvalidOperationException> (
				() => originalCurrent.Pop (),
				"popping original Current");

			AssertRaises<InvalidOperationException> (
				() => request1.Pop (),
				"popping inactive CacheRequest");

			request1.Push ();
			Assert.AreEqual (request1, CacheRequest.Current, "CacheRequest.Current should be equal to request at top of stack");
			request2.Push ();
			Assert.AreEqual (request2, CacheRequest.Current, "CacheRequest.Current should be equal to request at top of stack");

			// A request can occupy multiple slots in the request stack
			request2.Push ();
			Assert.AreEqual (request2, CacheRequest.Current, "CacheRequest.Current should be equal to request at top of stack");
			request1.Push ();
			Assert.AreEqual (request1, CacheRequest.Current, "CacheRequest.Current should be equal to request at top of stack");
			request1.Pop ();
			Assert.AreEqual (request2, CacheRequest.Current, "CacheRequest.Current should be equal to request at top of stack");
			request2.Pop ();
			Assert.AreEqual (request2, CacheRequest.Current, "CacheRequest.Current should be equal to request at top of stack");

			AssertRaises<InvalidOperationException> (
				() => request1.Pop (),
				"popping active CacheRequest not at top of stack");

			request2.Pop ();
			Assert.AreEqual (request1, CacheRequest.Current, "CacheRequest.Current should be equal to request at top of stack");
			request1.Pop ();
			Assert.AreEqual (originalCurrent, CacheRequest.Current, "After deactivating all of our requests, original Current should be back");

			// Neat
			request1.Activate ();
			Assert.AreEqual (request1, CacheRequest.Current, "CacheRequest.Current should be equal to request at top of stack");
			request1.Pop ();
			Assert.AreEqual (originalCurrent, CacheRequest.Current, "After deactivating all of our requests, original Current should be back");
		}

		// This behavior seems strange:
		//       "Caching also occurs when you subscribe to an event while a CacheRequest is active.
		//        The AutomationElement passed to your event handler as the source of an event contains
		//        the cached properties and patterns specified by the original CacheRequest. Any changes
		//        made to the CacheRequest after you subscribe to the event have no effect."
		[Test]
		public void PropertyEventTest ()
		{
			string oldName = label1Element.Current.Name;
			var request = new CacheRequest ();
			request.Add (AutomationElement.NameProperty);
			eventCount = 0;

			// Test adding handler before activating cache
			At.AddAutomationPropertyChangedEventHandler (
				testFormElement,
				TreeScope.Children,
				PropertyChangedHandler,
				AEIds.NameProperty);
			using (request.Activate ()) {
				RunCommand ("click button1");
				Assert.AreNotEqual (0, eventCount, "Event not fired");
				Assert.AreNotEqual (eventElement.Current.Name,
					oldName,
					"Name has changed");
				AssertRaises<InvalidOperationException> (
					() => eventElement.Cached.Name.ToString (),
					"Cached name when handler added before the cache was activated");
			}
			At.RemoveAutomationPropertyChangedEventHandler (label1Element,
				PropertyChangedHandler);

			oldName = label1Element.Current.Name;
			eventCount = 0;

			// Test adding handler after activating cache
			using (request.Activate ()) {
				At.AddAutomationPropertyChangedEventHandler (
					label1Element,
					TreeScope.Element,
					PropertyChangedHandler,
					AEIds.NameProperty);
				RunCommand ("click button1");
				Assert.AreNotEqual (0, eventCount, "Event not fired");
				Assert.AreNotEqual (eventElement.Current.Name,
					oldName,
				"Name has changed");
				AssertRaises<InvalidOperationException> (
					() => eventElement.Cached.Name.ToString (),
					"Cached name when handler added after the cache was activated");
				At.RemoveAutomationPropertyChangedEventHandler (label1Element,
					PropertyChangedHandler);
			}

			// Test using Push() rather than Activate()
			request.Push ();
			eventCount = 0;
			At.AddAutomationPropertyChangedEventHandler (
				label1Element,
				TreeScope.Element,
				PropertyChangedHandler,
				AEIds.NameProperty);
			RunCommand ("click button1");
			Assert.AreNotEqual (0, eventCount, "Event not fired");
			Assert.AreNotEqual (eventElement.Current.Name,
				oldName,
			"Name has changed");
			AssertRaises<InvalidOperationException> (
				() => eventElement.Cached.Name.ToString (),
				"Cached name when handler added after the cache was activated");
			At.RemoveAutomationPropertyChangedEventHandler (label1Element,
				PropertyChangedHandler);
			request.Pop ();
		}

		[Test]
		public void SelectionTest ()
		{
			AutomationElement child1Element, child2Element;
			child1Element = treeView1Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.TreeItem));
			Assert.IsNotNull (child1Element, "Child element should not be null");
			child2Element = TreeWalker.RawViewWalker.GetNextSibling (child1Element);
			Assert.IsNotNull (child2Element, "Child element should not be null");
			CacheRequest request = new CacheRequest ();
			request.Add (SelectionItemPattern.Pattern);
			request.Add (SelectionItemPattern.IsSelectedProperty);
			request.Add (SelectionItemPattern.SelectionContainerProperty);
			request.Add (SelectionPattern.Pattern);
			request.Add (SelectionPattern.CanSelectMultipleProperty);
			request.Add (SelectionPattern.IsSelectionRequiredProperty);
			request.Add (SelectionPattern.SelectionProperty);

			using (request.Activate ()) {
			SelectionItemPattern CachedItemPattern;
				AssertRaises<InvalidOperationException> (
					() => CachedItemPattern = (SelectionItemPattern)
						child2Element.GetCachedPattern (
							SelectionItemPattern.Pattern),
						"GetCachedPattern when fetched from an element retrieved while there was no active cache");

				child2Element = child2Element.GetUpdatedCache (request);
				CachedItemPattern = (SelectionItemPattern)
					child2Element.GetCachedPattern (
						SelectionItemPattern.Pattern);
				SelectionItemPattern currentItemPattern = (SelectionItemPattern)
					child2Element.GetCurrentPattern (
						SelectionItemPattern.Pattern);
				AutomationElement treeView1ElementRef2 = treeView1Element.GetUpdatedCache (request);
				SelectionPattern currentSelectionPattern = (SelectionPattern)
					treeView1ElementRef2.GetCurrentPattern (
						SelectionPattern.Pattern);

				Assert.IsFalse (CachedItemPattern.Current.IsSelected);
				Assert.IsFalse (CachedItemPattern.Cached.IsSelected);
				Assert.IsFalse (currentItemPattern.Current.IsSelected);
				AssertRaises<InvalidOperationException> (
					() => currentItemPattern.Cached.IsSelected.ToString (),
					"Cached property on an uncached pattern");

				Assert.AreEqual (treeView1Element,
					CachedItemPattern.Cached.SelectionContainer,
					"Cached.SelectionContainer");
				VerifyCachedPropertyValue (child2Element,
					SelectionItemPattern.SelectionContainerProperty,
					currentItemPattern.Current.SelectionContainer,
					currentItemPattern.Current.SelectionContainer,
					currentItemPattern.Current.SelectionContainer);
				Assert.IsFalse (currentSelectionPattern.Current.IsSelectionRequired,
					"IsSelectionRequired before select");

				CachedItemPattern.Select ();
				Thread.Sleep (500);

				Assert.IsTrue (currentSelectionPattern.Current.IsSelectionRequired,
					"IsSelectionRequired before select");
				Assert.IsTrue (currentItemPattern.Current.IsSelected);
				Assert.IsTrue (CachedItemPattern.Current.IsSelected);
				Assert.IsFalse (CachedItemPattern.Cached.IsSelected);

			SelectionPattern cachedSelectionPattern
				= (SelectionPattern) treeView1ElementRef2.GetCachedPattern (SelectionPattern.Pattern);

				Assert.AreEqual (cachedSelectionPattern.Cached.CanSelectMultiple,
					currentSelectionPattern.Current.CanSelectMultiple,
					"Cached.CanSelectMultiple == Current.CanSelectMultiple");
				VerifyCachedPropertyValue (treeView1ElementRef2,
					SelectionPatternIdentifiers.CanSelectMultipleProperty,
					currentSelectionPattern.Current.CanSelectMultiple,
					currentSelectionPattern.Current.CanSelectMultiple,
					currentSelectionPattern.Current.CanSelectMultiple);

				Assert.IsFalse (cachedSelectionPattern.Cached.IsSelectionRequired,
					"Cached.IsSelectionRequired == Current.IsSelectionRequired");
				VerifyCachedPropertyValue (treeView1ElementRef2,
					SelectionPatternIdentifiers.IsSelectionRequiredProperty,
					false, false, false);

				AutomationElement [] selection = cachedSelectionPattern.Current.GetSelection ();
				Assert.AreEqual (1, selection.Length, "Selection length");

				selection = cachedSelectionPattern.Cached.GetSelection ();
				Assert.AreEqual (0, selection.Length, "Selection length");
			}
		}

		[Test]
		public void TextPatternTest ()
		{
			RunCommand ("set textbox3 to first line\\nsecond line\\nthird line");
			var request = new CacheRequest ();
			request.Add (TextPattern.Pattern);
			using (request.Activate ()) {
				TextPattern textPattern;
				AutomationElement textbox3ElementRef2 = textbox3Element.GetUpdatedCache (request);
				AssertRaises<InvalidOperationException> (
					() => textPattern = (TextPattern) textbox3Element.GetCachedPattern (TextPattern.Pattern),
					"Getting TextPattern from an uncached ref");
				textPattern = (TextPattern) textbox3ElementRef2.GetCachedPattern (TextPattern.Pattern);
				TextPatternRange range = textPattern.DocumentRange.Clone ();
				TextPatternRange range1;

				range1 = range.FindText ("second", false, false);
				Assert.AreEqual ("second", range1.GetText (-1));

				RunCommand ("set textbox3 to gomez thing:morticia\twednesday ing");
				Thread.Sleep (500);

				range1 = range.FindText ("mort", false, false);
				Assert.AreEqual ("mort", range1.GetText (-1));
			}
		}

		[Test]
		public void AcceleratorKeyTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.AcceleratorKeyProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.AcceleratorKey,
					groupBox1Element.Current.AcceleratorKey,
					"Cached.AcceleratorKey == Current.AcceleratorKey");
			}
		}

		[Test]
		public void AccessKeyTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.AccessKeyProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.AccessKey,
					groupBox1Element.Current.AccessKey,
					"Cached.AccessKey == Current.AccessKey");
			}
		}

		[Test]
		public void AutomationIdTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.AutomationIdProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.AutomationId,
					groupBox1Element.Current.AutomationId,
					"Cached.AutomationId == Current.AutomationId");
				VerifyCachedPropertyValue (groupBox1ElementRef2,
					AEIds.AutomationIdProperty,
					groupBox1Element.Current.AutomationId,
					groupBox1Element.Current.AutomationId,
					groupBox1Element.Current.AutomationId);
			}
		}

		[Test]
		public void BoundingRectangleTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.BoundingRectangleProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.BoundingRectangle,
					groupBox1Element.Current.BoundingRectangle,
					"Cached.BoundingRectangle == Current.BoundingRectangle");
				VerifyCachedPropertyValue (groupBox1ElementRef2,
					AEIds.BoundingRectangleProperty,
					groupBox1Element.Current.BoundingRectangle,
					groupBox1Element.Current.BoundingRectangle,
					groupBox1Element.Current.BoundingRectangle);
			}
		}

		[Test]
		public void ClassNameTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.ClassNameProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.ClassName,
					groupBox1Element.Current.ClassName,
					"Cached.ClassName == Current.ClassName");
				Assert.AreEqual (groupBox1ElementRef2.Cached.ClassName,
					groupBox1ElementRef2.GetCachedPropertyValue (AutomationElement.ClassNameProperty),
					"GetCachedPropertyValue");
			}
		}

		[Test]
		public void ClickablePointTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.ClickablePointProperty);
		}

		[Test]
		public void ControlTypeTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.ControlTypeProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.ControlType,
					groupBox1Element.Current.ControlType,
					"Cached.ControlType == Current.ControlType");
				VerifyCachedPropertyValue (groupBox1ElementRef2,
					AEIds.ControlTypeProperty,
					groupBox1Element.Current.ControlType,
					groupBox1Element.Current.ControlType,
					groupBox1Element.Current.ControlType);
			}
		}

		[Test]
		public void CultureTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.CultureProperty);
		}

		[Test]
		public void FrameworkIdTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.FrameworkIdProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.FrameworkId,
					groupBox1Element.Current.FrameworkId,
					"Cached.FrameworkId == Current.FrameworkId");
				VerifyCachedPropertyValue (groupBox1ElementRef2,
					AEIds.FrameworkIdProperty,
					groupBox1Element.Current.FrameworkId,
					groupBox1Element.Current.FrameworkId,
					groupBox1Element.Current.FrameworkId);
			}
		}

		[Test]
		public void HasKeyboardFocusTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.HasKeyboardFocusProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.HasKeyboardFocus,
					groupBox1Element.Current.HasKeyboardFocus,
					"Cached.HasKeyboardFocus == Current.HasKeyboardFocus");
				VerifyCachedPropertyValue (groupBox1ElementRef2,
					AEIds.HasKeyboardFocusProperty,
					groupBox1Element.Current.HasKeyboardFocus,
					groupBox1Element.Current.HasKeyboardFocus,
					groupBox1Element.Current.HasKeyboardFocus);
			}
		}

		[Test]
		public void HelpTextTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.HelpTextProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.HelpText,
					groupBox1Element.Current.HelpText,
					"Cached.HelpText == Current.HelpText");
				Assert.AreEqual (groupBox1ElementRef2.Cached.HelpText,
					groupBox1ElementRef2.GetCachedPropertyValue (AutomationElement.HelpTextProperty),
					"GetCachedPropertyValue");
			}
		}

		[Test]
		public void IsContentElementTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.IsContentElementProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.IsContentElement,
					groupBox1Element.Current.IsContentElement,
					"Cached.IsContentElement == Current.IsContentElement");
				VerifyCachedPropertyValue (groupBox1ElementRef2,
					AEIds.IsContentElementProperty,
					groupBox1Element.Current.IsContentElement,
					groupBox1Element.Current.IsContentElement,
					groupBox1Element.Current.IsContentElement);
			}
		}

		[Test]
		public void IsControlElementTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.IsControlElementProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.IsControlElement,
					groupBox1Element.Current.IsControlElement,
					"Cached.IsControlElement == Current.IsControlElement");
				Assert.AreEqual (groupBox1ElementRef2.Cached.IsControlElement,
					groupBox1ElementRef2.GetCachedPropertyValue (AutomationElement.IsControlElementProperty),
					"GetCachedPropertyValue");
			}
		}

		[Test]
		public void IsDockPatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsDockPatternAvailableProperty);
		}

		public void IsEnableTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.IsEnabledProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.IsEnabled,
					groupBox1Element.Current.IsEnabled,
					"Cached.IsEnabled == Current.IsEnabled");
				VerifyCachedPropertyValue (groupBox1ElementRef2,
					AEIds.IsEnabledProperty,
					groupBox1Element.Current.IsEnabled,
					groupBox1Element.Current.IsEnabled,
					groupBox1Element.Current.IsEnabled);
			}
		}

		[Test]
		public void IsExpandCollapsePatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsExpandCollapsePatternAvailableProperty);
		}

		[Test]
		public void IsGridItemPatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsGridItemPatternAvailableProperty);
		}

		[Test]
		public void IsGridPatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsGridPatternAvailableProperty);
		}

		[Test]
		public void IsInvokePatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsInvokePatternAvailableProperty);
		}

		[Test]
		public void IsKeyboardFocusableTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.IsKeyboardFocusableProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.IsKeyboardFocusable,
					groupBox1Element.Current.IsKeyboardFocusable,
					"Cached.IsKeyboardFocusable == Current.IsKeyboardFocusable");
				VerifyCachedPropertyValue (groupBox1ElementRef2,
					AEIds.IsKeyboardFocusableProperty,
					groupBox1Element.Current.IsKeyboardFocusable,
					groupBox1Element.Current.IsKeyboardFocusable,
					groupBox1Element.Current.IsKeyboardFocusable);
			}
		}

		[Test]
		public void IsMultipleViewPatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsMultipleViewPatternAvailableProperty);
		}

		[Test]
		public void IsOffscreenTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.IsOffscreenProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.IsOffscreen,
					groupBox1Element.Current.IsOffscreen,
					"Cached.IsOffscreen == Current.IsOffscreen");
				VerifyCachedPropertyValue (groupBox1ElementRef2,
					AEIds.IsOffscreenProperty,
					groupBox1Element.Current.IsOffscreen,
					groupBox1Element.Current.IsOffscreen,
					groupBox1Element.Current.IsOffscreen);
			}
		}

		[Test]
		public void IsPasswordTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.IsPasswordProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.IsPassword,
					groupBox1Element.Current.IsPassword,
					"Cached.IsPassword == Current.IsPassword");
				Assert.AreEqual (groupBox1ElementRef2.Cached.IsPassword,
					groupBox1ElementRef2.GetCachedPropertyValue (AutomationElement.IsPasswordProperty),
					"GetCachedPropertyValue");
			}
		}

		[Test]
		public void IsRangeValuePatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsRangeValuePatternAvailableProperty);
		}

		[Test]
		public void IsRequiredForFormTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.IsRequiredForFormProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.IsRequiredForForm,
					groupBox1Element.Current.IsRequiredForForm,
					"Cached.IsRequiredForForm == Current.IsRequiredForForm");
				Assert.AreEqual (groupBox1ElementRef2.Cached.IsRequiredForForm,
					groupBox1ElementRef2.GetCachedPropertyValue (AutomationElement.IsRequiredForFormProperty),
					"GetCachedPropertyValue");
			}
		}

		[Test]
		public void IsScrollItemPatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsScrollItemPatternAvailableProperty);
		}

		[Test]
		public void IsScrollPatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsScrollPatternAvailableProperty);
		}

		[Test]
		public void IsSelectionItemPatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsSelectionItemPatternAvailableProperty);
		}

		[Test]
		public void IsSelectionPatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsSelectionPatternAvailableProperty);
		}

		[Test]
		public void IsTableItemPatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsTableItemPatternAvailableProperty);
		}

		[Test]
		public void IsTablePatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsTablePatternAvailableProperty);
		}

		[Test]
		public void IsTextPatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsTextPatternAvailableProperty);
		}

		[Test]
		public void IsTogglePatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsTogglePatternAvailableProperty);
		}

		[Test]
		public void IsTransformPatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsTransformPatternAvailableProperty);
		}

		[Test]
		public void IsValuePatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsValuePatternAvailableProperty);
		}

		[Test]
		public void IsWindowPatternAvailableTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.IsWindowPatternAvailableProperty);
		}

		[Test]
		public void ItemStatusTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.ItemStatusProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.ItemStatus,
					groupBox1Element.Current.ItemStatus,
					"Cached.ItemStatus == Current.ItemStatus");
				Assert.AreEqual (groupBox1ElementRef2.Cached.ItemStatus,
					groupBox1ElementRef2.GetCachedPropertyValue (AutomationElement.ItemStatusProperty),
					"GetCachedPropertyValue");
			}
		}

		[Test]
		public void ItemTypeTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.ItemTypeProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.ItemType,
					groupBox1Element.Current.ItemType,
					"Cached.ItemType == Current.ItemType");
				Assert.AreEqual (groupBox1ElementRef2.Cached.ItemType,
					groupBox1ElementRef2.GetCachedPropertyValue (AutomationElement.ItemTypeProperty),
					"GetCachedPropertyValue");
			}
		}

		[Test]
		public void LabeledByTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.LabeledByProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.LabeledBy,
					groupBox1Element.Current.LabeledBy,
					"Cached.LabeledBy == Current.LabeledBy");
				Assert.AreEqual (groupBox1ElementRef2.Cached.LabeledBy,
					groupBox1ElementRef2.GetCachedPropertyValue (AutomationElement.LabeledByProperty),
					"GetCachedPropertyValue");
			}
		}

		[Test]
		public void LocalizedControlTypeTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.LocalizedControlTypeProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.LocalizedControlType,
					groupBox1Element.Current.LocalizedControlType,
					"Cached.LocalizedControlType == Current.LocalizedControlType");
				VerifyCachedPropertyValue (groupBox1ElementRef2,
					AEIds.LocalizedControlTypeProperty,
					groupBox1Element.Current.LocalizedControlType,
					groupBox1Element.Current.LocalizedControlType,
					groupBox1Element.Current.LocalizedControlType);
			}
		}

		// name is implicitly tested elsewhere

		[Test]
		public void NativeWindowHandleTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.NativeWindowHandleProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.NativeWindowHandle,
					groupBox1Element.Current.NativeWindowHandle,
					"Cached.NativeWindowHandle == Current.NativeWindowHandle");
				VerifyCachedPropertyValue (groupBox1ElementRef2,
					AEIds.NativeWindowHandleProperty,
					groupBox1Element.Current.NativeWindowHandle,
					groupBox1Element.Current.NativeWindowHandle,
					groupBox1Element.Current.NativeWindowHandle);
			}
		}

		[Test]
		public void OrientationTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.OrientationProperty);
			using (request.Activate ()) {
				AutomationElement hScrollBarElementRef2 = hScrollBarElement.GetUpdatedCache (request);
				Assert.AreEqual (hScrollBarElementRef2.Cached.Orientation,
					hScrollBarElement.Current.Orientation,
					"Cached.Orientation == Current.Orientation");
				VerifyCachedPropertyValue (hScrollBarElementRef2,
					AEIds.OrientationProperty,
					hScrollBarElement.Current.Orientation,
					hScrollBarElement.Current.Orientation,
					hScrollBarElement.Current.Orientation);
			}
		}

		[Test]
		public void ProcessIdTest ()
		{
			var request = new CacheRequest ();
			request.Add (AutomationElement.ProcessIdProperty);
			using (request.Activate ()) {
				AutomationElement groupBox1ElementRef2 = groupBox1Element.GetUpdatedCache (request);
				Assert.AreEqual (groupBox1ElementRef2.Cached.ProcessId,
					groupBox1Element.Current.ProcessId,
					"Cached.ProcessId == Current.ProcessId");
				VerifyCachedPropertyValue (groupBox1ElementRef2,
					AEIds.ProcessIdProperty,
					groupBox1Element.Current.ProcessId,
					groupBox1Element.Current.ProcessId,
					groupBox1Element.Current.ProcessId);
			}
		}

		[Test]
		public void RuntimeIdTest ()
		{
			TestSimpleProperty (groupBox1Element,
				AutomationElement.RuntimeIdProperty);
		}

		[Test]
		public void ExpandCollapsePatternTest ()
		{
			AutomationElement parentElement
				= treeView1Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.TreeItem));
			var request = new CacheRequest ();
			request.Add (ExpandCollapsePattern.Pattern);
			request.Add (ExpandCollapsePattern.ExpandCollapseStateProperty);

			using (request.Activate ()) {
				AutomationElement parentElementRef2 = parentElement.GetUpdatedCache (request);
				ExpandCollapsePattern cachedPattern = (ExpandCollapsePattern) parentElementRef2.GetCachedPattern (ExpandCollapsePattern.Pattern);
				ExpandCollapsePattern currentPattern = (ExpandCollapsePattern) parentElement.GetCurrentPattern (ExpandCollapsePattern.Pattern);
				Assert.AreEqual (cachedPattern.Cached.ExpandCollapseState,
					currentPattern.Current.ExpandCollapseState,
					"Cached.ExpandCollapseState == Current.ExpandCollapseState");
				VerifyCachedPropertyValue (parentElementRef2,
					ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
					currentPattern.Current.ExpandCollapseState,
					currentPattern.Current.ExpandCollapseState,
					currentPattern.Current.ExpandCollapseState);
			}
		}
				
		[Test]
		public void MultipleViewPatternTest ()
		{
			var request = new CacheRequest ();
			request.Add (MultipleViewPattern.Pattern);
			request.Add (MultipleViewPattern.CurrentViewProperty);

			using (request.Activate ()) {
				AutomationElement listView1ElementRef2 = listView1Element.GetUpdatedCache (request);
				MultipleViewPattern cachedPattern = (MultipleViewPattern) listView1ElementRef2.GetCachedPattern (MultipleViewPattern.Pattern);
				MultipleViewPattern currentPattern = (MultipleViewPattern) listView1Element.GetCurrentPattern (MultipleViewPattern.Pattern);
				Assert.AreEqual (cachedPattern.Cached.CurrentView,
					currentPattern.Current.CurrentView,
					"Cached.CurrentView == Current.CurrentView");
				VerifyCachedPropertyValue (listView1ElementRef2,
					MultipleViewPatternIdentifiers.CurrentViewProperty,
					currentPattern.Current.CurrentView,
					currentPattern.Current.CurrentView,
					currentPattern.Current.CurrentView);
			}
		}
				
		[Test]
		public void TogglePatternTest ()
		{
			var request = new CacheRequest ();
			request.Add (TogglePattern.Pattern);
			request.Add (TogglePattern.ToggleStateProperty);

			using (request.Activate ()) {
				AutomationElement checkBox1ElementRef2 = checkBox1Element.GetUpdatedCache (request);
				TogglePattern cachedPattern = (TogglePattern) checkBox1ElementRef2.GetCachedPattern (TogglePattern.Pattern);
				Assert.AreEqual (ToggleState.Off,
					checkBox1Element.GetCurrentPropertyValue (TogglePatternIdentifiers.ToggleStateProperty),
					"CheckBox is initially not checked");
				Assert.AreEqual (ToggleState.Off,
					checkBox1ElementRef2.GetCachedPropertyValue (TogglePatternIdentifiers.ToggleStateProperty),
					"CheckBox initial cached state");
				Assert.AreEqual (ToggleState.Off,
					cachedPattern.Cached.ToggleState,
					"cached.ToggleState");

				cachedPattern.Toggle ();
				Assert.AreEqual (ToggleState.On,
					checkBox1Element.GetCurrentPropertyValue (TogglePatternIdentifiers.ToggleStateProperty),
					"CheckBox is now checked");
				Assert.AreEqual (ToggleState.Off,
					checkBox1ElementRef2.GetCachedPropertyValue (TogglePatternIdentifiers.ToggleStateProperty),
					"CheckBox cached state after toggle");
				Assert.AreEqual (ToggleState.Off,
					cachedPattern.Cached.ToggleState,
					"cached.ToggleState after toggle");
			}
		}
				
		[Test]
		public void RangeValuePatternTest ()
		{
			var request = new CacheRequest ();
			request.Add (RangeValuePattern.Pattern);
			request.Add (RangeValuePattern.IsReadOnlyProperty);
			request.Add (RangeValuePattern.LargeChangeProperty);
			request.Add (RangeValuePattern.MaximumProperty);
			request.Add (RangeValuePattern.MinimumProperty);
			request.Add (RangeValuePattern.SmallChangeProperty);
			request.Add (RangeValuePattern.ValueProperty);

			using (request.Activate ()) {
				AutomationElement hScrollBarElementRef2 = hScrollBarElement.GetUpdatedCache (request);
				RangeValuePattern currentPattern = (RangeValuePattern) hScrollBarElement.GetCurrentPattern (RangeValuePattern.Pattern);
				RangeValuePattern cachedPattern = (RangeValuePattern) hScrollBarElementRef2.GetCachedPattern (RangeValuePattern.Pattern);

				Assert.AreEqual (cachedPattern.Cached.IsReadOnly,
					currentPattern.Current.IsReadOnly,
					"Cached.IsReadOnly == Current.IsReadOnly");
				VerifyCachedPropertyValue (hScrollBarElementRef2,
					RangeValuePatternIdentifiers.IsReadOnlyProperty,
					currentPattern.Current.IsReadOnly,
					currentPattern.Current.IsReadOnly,
					currentPattern.Current.IsReadOnly);

				Assert.AreEqual (cachedPattern.Cached.LargeChange,
					currentPattern.Current.LargeChange,
					"Cached.LargeChange == Current.LargeChange");
				VerifyCachedPropertyValue (hScrollBarElementRef2,
					RangeValuePatternIdentifiers.LargeChangeProperty,
					currentPattern.Current.LargeChange,
					currentPattern.Current.LargeChange,
					currentPattern.Current.LargeChange);

				Assert.AreEqual (cachedPattern.Cached.Maximum,
					currentPattern.Current.Maximum,
					"Cached.Maximum == Current.Maximum");
				VerifyCachedPropertyValue (hScrollBarElementRef2,
					RangeValuePatternIdentifiers.MaximumProperty,
					currentPattern.Current.Maximum,
					currentPattern.Current.Maximum,
					currentPattern.Current.Maximum);

				Assert.AreEqual (cachedPattern.Cached.Minimum,
					currentPattern.Current.Minimum,
					"Cached.Minimum == Current.Minimum");
				VerifyCachedPropertyValue (hScrollBarElementRef2,
					RangeValuePatternIdentifiers.MinimumProperty,
					currentPattern.Current.Minimum,
					currentPattern.Current.Minimum,
					currentPattern.Current.Minimum);

				Assert.AreEqual (cachedPattern.Cached.SmallChange,
					currentPattern.Current.SmallChange,
					"Cached.SmallChange == Current.SmallChange");
				VerifyCachedPropertyValue (hScrollBarElementRef2,
					RangeValuePatternIdentifiers.SmallChangeProperty,
					currentPattern.Current.SmallChange,
					currentPattern.Current.SmallChange,
					currentPattern.Current.SmallChange);

				Assert.AreEqual (cachedPattern.Cached.Value,
					currentPattern.Current.Value,
					"Cached.Value == Current.Value");
				VerifyCachedPropertyValue (hScrollBarElementRef2,
					RangeValuePatternIdentifiers.ValueProperty,
					currentPattern.Current.Value,
					currentPattern.Current.Value,
					currentPattern.Current.Value);
			}
		}
				
		[Test]
		public void ScrollPatternTest ()
		{
			var request = new CacheRequest ();
			request.Add (ScrollPattern.Pattern);
			request.Add (ScrollPattern.HorizontallyScrollableProperty);
			request.Add (ScrollPattern.HorizontalScrollPercentProperty);
			request.Add (ScrollPattern.HorizontalViewSizeProperty);
			request.Add (ScrollPattern.VerticallyScrollableProperty);
			request.Add (ScrollPattern.VerticalViewSizeProperty);

			using (request.Activate ()) {
				AutomationElement listView1ElementRef2 = listView1Element.GetUpdatedCache (request);
				ScrollPattern currentPattern = (ScrollPattern) listView1Element.GetCurrentPattern (ScrollPattern.Pattern);
				ScrollPattern cachedPattern = (ScrollPattern) listView1ElementRef2.GetCachedPattern (ScrollPattern.Pattern);

				Assert.AreEqual (cachedPattern.Cached.HorizontallyScrollable,
					currentPattern.Current.HorizontallyScrollable,
					"Cached.HorizontallyScrollable == Current.HorizontallyScrollable");
				VerifyCachedPropertyValue (listView1ElementRef2,
					ScrollPatternIdentifiers.HorizontallyScrollableProperty,
					currentPattern.Current.HorizontallyScrollable,
					currentPattern.Current.HorizontallyScrollable,
					currentPattern.Current.HorizontallyScrollable);

				Assert.AreEqual (cachedPattern.Cached.HorizontalScrollPercent,
					currentPattern.Current.HorizontalScrollPercent,
					"Cached.HorizontalScrollPercent == Current.HorizontalScrollPercent");
				VerifyCachedPropertyValue (listView1ElementRef2,
					ScrollPatternIdentifiers.HorizontalScrollPercentProperty,
					currentPattern.Current.HorizontalScrollPercent,
					currentPattern.Current.HorizontalScrollPercent,
					currentPattern.Current.HorizontalScrollPercent);

				Assert.AreEqual (cachedPattern.Cached.HorizontalViewSize,
					currentPattern.Current.HorizontalViewSize,
					"Cached.HorizontalViewSize == Current.HorizontalViewSize");
				VerifyCachedPropertyValue (listView1ElementRef2,
					ScrollPatternIdentifiers.HorizontalViewSizeProperty,
					currentPattern.Current.HorizontalViewSize,
					currentPattern.Current.HorizontalViewSize,
					currentPattern.Current.HorizontalViewSize);

				Assert.AreEqual (cachedPattern.Cached.VerticallyScrollable,
					currentPattern.Current.VerticallyScrollable,
					"Cached.VerticallyScrollable == Current.VerticallyScrollable");
				VerifyCachedPropertyValue (listView1ElementRef2,
					ScrollPatternIdentifiers.VerticallyScrollableProperty,
					currentPattern.Current.VerticallyScrollable,
					currentPattern.Current.VerticallyScrollable,
					currentPattern.Current.VerticallyScrollable);

				Assert.AreEqual (cachedPattern.Cached.VerticalViewSize,
					currentPattern.Current.VerticalViewSize,
					"Cached.VerticalViewSize == Current.VerticalViewSize");
				VerifyCachedPropertyValue (listView1ElementRef2,
					ScrollPatternIdentifiers.VerticalViewSizeProperty,
					currentPattern.Current.VerticalViewSize,
					currentPattern.Current.VerticalViewSize,
					currentPattern.Current.VerticalViewSize);
			}
		}
				
		[Test]
		public void TransformPatternTest ()
		{
			var request = new CacheRequest ();
			request.Add (TransformPattern.Pattern);
			request.Add (TransformPattern.CanMoveProperty);
			request.Add (TransformPattern.CanResizeProperty);
			request.Add (TransformPattern.CanRotateProperty);

			using (request.Activate ()) {
				AutomationElement testFormElementRef2 = testFormElement.GetUpdatedCache (request);
				TransformPattern currentPattern = (TransformPattern) testFormElement.GetCurrentPattern (TransformPattern.Pattern);
				TransformPattern cachedPattern = (TransformPattern) testFormElementRef2.GetCachedPattern (TransformPattern.Pattern);

				Assert.AreEqual (cachedPattern.Cached.CanMove,
					currentPattern.Current.CanMove,
					"Cached.CanMove == Current.CanMove");
				VerifyCachedPropertyValue (testFormElementRef2,
					TransformPatternIdentifiers.CanMoveProperty,
					currentPattern.Current.CanMove,
					currentPattern.Current.CanMove,
					currentPattern.Current.CanMove);

				Assert.AreEqual (cachedPattern.Cached.CanResize,
					currentPattern.Current.CanResize,
					"Cached.CanResize == Current.CanResize");
				VerifyCachedPropertyValue (testFormElementRef2,
					TransformPatternIdentifiers.CanResizeProperty,
					currentPattern.Current.CanResize,
					currentPattern.Current.CanResize,
					currentPattern.Current.CanResize);

				Assert.AreEqual (cachedPattern.Cached.CanRotate,
					currentPattern.Current.CanRotate,
					"Cached.CanRotate == Current.CanRotate");
				VerifyCachedPropertyValue (testFormElementRef2,
					TransformPatternIdentifiers.CanRotateProperty,
					currentPattern.Current.CanRotate,
					currentPattern.Current.CanRotate,
					currentPattern.Current.CanRotate);
			}
		}
				
		[Test]
		public void ValuePatternTest ()
		{
			var request = new CacheRequest ();
			request.Add (ValuePattern.Pattern);
			request.Add (ValuePattern.IsReadOnlyProperty);
			request.Add (ValuePattern.ValueProperty);

			using (request.Activate ()) {
				AutomationElement textbox1ElementRef2 = textbox1Element.GetUpdatedCache (request);
				ValuePattern currentPattern = (ValuePattern) textbox1Element.GetCurrentPattern (ValuePattern.Pattern);
				ValuePattern cachedPattern = (ValuePattern) textbox1ElementRef2.GetCachedPattern (ValuePattern.Pattern);

				Assert.AreEqual (cachedPattern.Cached.IsReadOnly,
					currentPattern.Current.IsReadOnly,
					"Cached.IsReadOnly == Current.IsReadOnly");
				VerifyCachedPropertyValue (textbox1ElementRef2,
					ValuePatternIdentifiers.IsReadOnlyProperty,
					currentPattern.Current.IsReadOnly,
					currentPattern.Current.IsReadOnly,
					currentPattern.Current.IsReadOnly);

				Assert.AreEqual (cachedPattern.Cached.Value,
					currentPattern.Current.Value,
					"Cached.Value == Current.Value");
				VerifyCachedPropertyValue (textbox1ElementRef2,
					ValuePatternIdentifiers.ValueProperty,
					currentPattern.Current.Value,
					currentPattern.Current.Value,
					currentPattern.Current.Value);
			}
		}
				
		[Test]
		public void WindowPatternTest ()
		{
			var request = new CacheRequest ();
			request.Add (WindowPattern.Pattern);
			request.Add (WindowPattern.CanMaximizeProperty);
			request.Add (WindowPattern.CanMinimizeProperty);
			request.Add (WindowPattern.IsModalProperty);
			request.Add (WindowPattern.IsTopmostProperty);
			request.Add (WindowPattern.WindowInteractionStateProperty);
			request.Add (WindowPattern.WindowVisualStateProperty);

			using (request.Activate ()) {
				AutomationElement testFormElementRef2 = testFormElement.GetUpdatedCache (request);
				WindowPattern currentPattern = (WindowPattern) testFormElement.GetCurrentPattern (WindowPattern.Pattern);
				WindowPattern cachedPattern = (WindowPattern) testFormElementRef2.GetCachedPattern (WindowPattern.Pattern);

				Assert.AreEqual (cachedPattern.Cached.CanMaximize,
					currentPattern.Current.CanMaximize,
					"Cached.CanMaximize == Current.CanMaximize");
				VerifyCachedPropertyValue (testFormElementRef2,
					WindowPatternIdentifiers.CanMaximizeProperty,
					currentPattern.Current.CanMaximize,
					currentPattern.Current.CanMaximize,
					currentPattern.Current.CanMaximize);

				Assert.AreEqual (cachedPattern.Cached.CanMinimize,
					currentPattern.Current.CanMinimize,
					"Cached.CanMinimize == Current.CanMinimize");
				VerifyCachedPropertyValue (testFormElementRef2,
					WindowPatternIdentifiers.CanMinimizeProperty,
					currentPattern.Current.CanMinimize,
					currentPattern.Current.CanMinimize,
					currentPattern.Current.CanMinimize);

				Assert.AreEqual (cachedPattern.Cached.IsModal,
					currentPattern.Current.IsModal,
					"Cached.IsModal == Current.IsModal");
				VerifyCachedPropertyValue (testFormElementRef2,
					WindowPatternIdentifiers.IsModalProperty,
					currentPattern.Current.IsModal,
					currentPattern.Current.IsModal,
					currentPattern.Current.IsModal);

				Assert.AreEqual (cachedPattern.Cached.IsTopmost,
					currentPattern.Current.IsTopmost,
					"Cached.IsTopmost == Current.IsTopmost");
				VerifyCachedPropertyValue (testFormElementRef2,
					WindowPatternIdentifiers.IsTopmostProperty,
					currentPattern.Current.IsTopmost,
					currentPattern.Current.IsTopmost,
					currentPattern.Current.IsTopmost);

				Assert.AreEqual (cachedPattern.Cached.WindowInteractionState,
					currentPattern.Current.WindowInteractionState,
					"Cached.WindowInteractionState == Current.WindowInteractionState");
				VerifyCachedPropertyValue (testFormElementRef2,
					WindowPatternIdentifiers.WindowInteractionStateProperty,
					currentPattern.Current.WindowInteractionState,
					currentPattern.Current.WindowInteractionState,
					currentPattern.Current.WindowInteractionState);

				Assert.AreEqual (cachedPattern.Cached.WindowVisualState,
					currentPattern.Current.WindowVisualState,
					"Cached.WindowVisualState == Current.WindowVisualState");
				VerifyCachedPropertyValue (testFormElementRef2,
					WindowPatternIdentifiers.WindowVisualStateProperty,
					currentPattern.Current.WindowVisualState,
					currentPattern.Current.WindowVisualState,
					currentPattern.Current.WindowVisualState);
			}
		}
				
		private void TestSimpleProperty (AutomationElement element, AutomationProperty property)
		{
			var request = new CacheRequest ();
			request.Add (property);
			using (request.Activate ()) {
				AutomationElement cachedElement = element.GetUpdatedCache (request);
				object currentValue = element.GetCurrentPropertyValue (property);
				Assert.AreEqual (currentValue,
					cachedElement.GetCachedPropertyValue (property),
					"GetcachedPropertyValue " + property.ProgrammaticName);
			}
		}

		private void VerifyCachedPropertyValue (AutomationElement element, AutomationProperty property, object expectedTrue, object expectedFalse, object expectedDefault)
		{
			Assert.AreEqual (expectedTrue,
				element.GetCachedPropertyValue (property, true),
				property.ProgrammaticName + " w/ true");
			Assert.AreEqual (expectedFalse,
				element.GetCachedPropertyValue (property, false),
				property.ProgrammaticName + " w/ false");
			Assert.AreEqual (expectedDefault,
				element.GetCachedPropertyValue (property),
				property.ProgrammaticName + " w/ default");
		}

		private void VerifyCachedPropertyValue<T> (AutomationElement element, AutomationProperty property, object expected)
		{
			//T retval = (T)
				//element.GetCachedPropertyValue (property, true);
			//Assert.AreEqual (expected, retval,
				//property.ProgrammaticName + " w/ true");
			//retval = (T)
				//element.GetCachedPropertyValue (property, false);
			//Assert.AreEqual (expected, retval,
				//property.ProgrammaticName + " w/ false");
			object retval =
				element.GetCachedPropertyValue (property);
			Assert.AreEqual (expected, retval,
				property.ProgrammaticName + " w/ default");
		}

		private void PropertyChangedHandler (object sender, AutomationPropertyChangedEventArgs e)
		{
			AutomationElement element = sender as AutomationElement;
			if (element.Current.ControlType != ControlType.Text)
				return;
			eventElement = element;
			eventCount++;
		}
	}
}
