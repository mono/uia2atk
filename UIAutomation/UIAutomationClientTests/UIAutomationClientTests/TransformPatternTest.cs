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
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using SWA = System.Windows.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation
{
	[TestFixture]
	public class TransformPatternTest : BaseTest
	{
		#region Test Methods
		[Test]
		public void PropertiesTest ()
		{
			Assert.AreEqual (TransformPatternIdentifiers.Pattern.Id,
				TransformPattern.Pattern.Id,
				"Pattern.Id");
			Assert.AreEqual (TransformPatternIdentifiers.Pattern.ProgrammaticName,
				TransformPattern.Pattern.ProgrammaticName,
				"Pattern.ProgrammaticName");
			Assert.AreEqual (TransformPatternIdentifiers.CanMoveProperty.Id,
				TransformPattern.CanMoveProperty.Id,
				"CanMoveProperty.Id");
			Assert.AreEqual (TransformPatternIdentifiers.CanMoveProperty.ProgrammaticName,
				TransformPattern.CanMoveProperty.ProgrammaticName,
				"CanMoveProperty.ProgrammaticName");
			Assert.AreEqual (TransformPatternIdentifiers.CanResizeProperty.Id,
				TransformPattern.CanResizeProperty.Id,
				"CanResizeProperty.Id");
			Assert.AreEqual (TransformPatternIdentifiers.CanResizeProperty.ProgrammaticName,
				TransformPattern.CanResizeProperty.ProgrammaticName,
				"CanResizeProperty.ProgrammaticName");
			Assert.AreEqual (TransformPatternIdentifiers.CanRotateProperty.Id,
				TransformPattern.CanRotateProperty.Id,
				"CanRotateProperty.Id");
			Assert.AreEqual (TransformPatternIdentifiers.CanRotateProperty.ProgrammaticName,
				TransformPattern.CanRotateProperty.ProgrammaticName,
				"CanRotateProperty.ProgrammaticName");
		}

		[Test]
		public void MoveTest ()
		{
			var automationEventsArray = new [] {
				new {Sender = (object) null, Args = (AutomationPropertyChangedEventArgs) null}};
			var automationEvents = automationEventsArray.ToList ();
			automationEvents.Clear ();

			SWA.Automation.AddAutomationPropertyChangedEventHandler (testFormElement,
				TreeScope.Element,
				(o, e) => automationEvents.Add (new { Sender = o, Args = e }),
				TransformPattern.CanMoveProperty,
				TransformPattern.CanResizeProperty);

			var transform = (TransformPattern)
				testFormElement.GetCurrentPattern (TransformPattern.Pattern);

			Assert.IsTrue (transform.Current.CanMove,
				"Expected CanMove to be true");

			RunCommand ("MoveTo.Origin");
			Thread.Sleep (500);

			var rect = testFormElement.Current.BoundingRectangle;
			double minX = rect.X;
			double minY = rect.Y;

			transform.Move (0, 0);
			Thread.Sleep (500);
			rect = testFormElement.Current.BoundingRectangle;
			Assert.AreEqual (minX, rect.X, "X after moving to 0,0");
			Assert.AreEqual (minY, rect.Y, "Y after moving to 0,0");

			transform.Move (50, 100);
			Thread.Sleep (500);
			rect = testFormElement.Current.BoundingRectangle;
			Assert.AreEqual (50, rect.X, "X after moving to 50,100");
			Assert.AreEqual (100, rect.Y, "Y after moving to 50,100");

			// Set CanMove to false
			RunCommand ("Toggle.Transform.CanMove");
			Thread.Sleep (500);
			Assert.IsFalse (transform.Current.CanMove,
				"CanMove should be false after changing WindowState");
			bool exceptionRaised = false;
			try {
				transform.Move (0, 0);
			} catch (InvalidOperationException) {
				exceptionRaised = true;
			}
			Assert.IsTrue (exceptionRaised,
				"Expected InvalidOperationException when calling Move with CanMove == false");

			// Set CanMove back to true
			RunCommand ("Toggle.Transform.CanMove");
			Thread.Sleep (500);
			rect = testFormElement.Current.BoundingRectangle;
			Assert.AreEqual (50, rect.X, "X still at 50,100");
			Assert.AreEqual (100, rect.Y, "Y still at 50,100");
			Assert.IsTrue (transform.Current.CanMove,
				"CanMove should be true after changing WindowState back");
			transform.Move (0, 0);
			Thread.Sleep (500);
			rect = testFormElement.Current.BoundingRectangle;
			Assert.AreEqual (minX, rect.X, "X after moving to 0,0");
			Assert.AreEqual (minY, rect.Y, "Y after moving to 0,0");

			// No events raised
			Assert.AreEqual (0, automationEvents.Count,
				"No events should have been raised");
		}

		[Test]
		public void MoveToNegativeCoordinatesTest ()
		{
			var transform = (TransformPattern)
				testFormElement.GetCurrentPattern (TransformPattern.Pattern);
			// TODO: Is this case fixable on Linux?
			transform.Move (-5, -1);
			Thread.Sleep (500);
			var rect = testFormElement.Current.BoundingRectangle;
			Assert.AreEqual (-5, rect.X, "X after moving to -5,-1");
			Assert.AreEqual (-1, rect.Y, "Y after moving to -5,-1");
		}

		[Test]
		public void ResizeTest ()
		{
			var transform = (TransformPattern)
				testFormElement.GetCurrentPattern (TransformPattern.Pattern);

			// Determine minimime resize available
			transform.Resize (0, 0);
			Thread.Sleep (200);
			var rect = testFormElement.Current.BoundingRectangle;
			var minRect = rect;

			transform.Resize (500, 1000);
			Thread.Sleep (200);
			rect = testFormElement.Current.BoundingRectangle;
			Assert.AreEqual (500, rect.Width);
			Assert.AreEqual (1000, rect.Height);

			transform.Resize (600, 800);
			Thread.Sleep (200);
			rect = testFormElement.Current.BoundingRectangle;
			Assert.AreEqual (600, rect.Width);
			Assert.AreEqual (800, rect.Height);

			transform.Resize (-5, -1);
			Thread.Sleep (200);
			rect = testFormElement.Current.BoundingRectangle;
			Assert.AreEqual (minRect.Width, rect.Width);
			Assert.AreEqual (minRect.Height, rect.Height);
		}

		[Test]
		// NOTE: The "Z_" prefix on the name forces this test to run last,
		//       as changing FormBorderStyle breaks other tests on Linux
		public void Z_CanResizeTest ()
		{
			var automationEventsArray = new [] {
				new {Sender = (object) null, Args = (AutomationPropertyChangedEventArgs) null}};
			var automationEvents = automationEventsArray.ToList ();
			automationEvents.Clear ();

			SWA.Automation.AddAutomationPropertyChangedEventHandler (testFormElement,
				TreeScope.Element,
				(o, e) => automationEvents.Add (new { Sender = o, Args = e }),
				TransformPattern.CanMoveProperty,
				TransformPattern.CanResizeProperty);

			var transform = (TransformPattern)
				testFormElement.GetCurrentPattern (TransformPattern.Pattern);

			Assert.IsTrue (transform.Current.CanResize,
				"Expected CanResize to be true");

			transform.Resize (600, 800);
			Thread.Sleep (200);
			var rect = testFormElement.Current.BoundingRectangle;
			Assert.AreEqual (600, rect.Width);
			Assert.AreEqual (800, rect.Height);

			// Set CanResize to false
			RunCommand ("Toggle.Transform.CanResize");
			Thread.Sleep (500);
			Assert.IsFalse (transform.Current.CanResize,
				"CanResize should be false after changing FormBorderStyle");
			bool exceptionRaised = false;
			try {
				transform.Resize (500, 1000);
			} catch (InvalidOperationException) {
				exceptionRaised = true;
			}
			Assert.IsTrue (exceptionRaised,
				"Expected InvalidOperationException when calling Resize with CanResize == false");

			// Set CanResize back to true
			RunCommand ("Toggle.Transform.CanResize");
			Thread.Sleep (500);
			rect = testFormElement.Current.BoundingRectangle;
			Assert.AreEqual (600, rect.Width);
			Assert.AreEqual (800, rect.Height);
			Assert.IsTrue (transform.Current.CanResize,
				"CanResize should be true after changing FormBorderStyle back");
			// NOTE: Winforms bug: inability to resize or
			//       move after FormBorderStyle is set back
			transform.Resize (500, 1000);
			Thread.Sleep (500);
			rect = testFormElement.Current.BoundingRectangle;
			Assert.AreEqual (500, rect.Width);
			Assert.AreEqual (1000, rect.Height);

			// No events raised
			Assert.AreEqual (0, automationEvents.Count,
				"No events should have been raised");
		}

		[Test]
		public void RotateTest ()
		{
			var transform = (TransformPattern)
				testFormElement.GetCurrentPattern (TransformPattern.Pattern);

			Assert.IsFalse (transform.Current.CanRotate,
				"CanRotate always false for winforms and gtk+");

			bool exceptionRaised = false;

			try {
				transform.Rotate (5);
			} catch (InvalidOperationException) {
				exceptionRaised = true;
			}
			Assert.IsTrue (exceptionRaised,
				"InvalidOperationException expected when CanRotate is false");

			try {
				transform.Rotate (0);
			} catch (InvalidOperationException) {
				exceptionRaised = true;
			}
			Assert.IsTrue (exceptionRaised,
				"InvalidOperationException expected even when rotating 0 degrees");
		}
		#endregion
	}
}
