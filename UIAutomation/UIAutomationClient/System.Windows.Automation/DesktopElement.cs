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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using Mono.UIAutomation.Source;

namespace System.Windows.Automation
{
	internal class DesktopElement : IElement
	{
		#region IElement implementation
		public bool SupportsProperty (AutomationProperty property)
		{
			// TODO: Implement
			return true;
		}

		public string AcceleratorKey {
			get {
				return string.Empty;
			}
		}

		public string AccessKey {
			get {
				return string.Empty;
			}
		}

		public string AutomationId {
			get {
				return string.Empty;
			}
		}

		public Rect BoundingRectangle {
			get {
				int width, height;
				NativeMethods.GetScreenBound (out width, out height);
				return new Rect (0, 0, width, height);
			}
		}

		public string ClassName {
			get {
				return string.Empty;
			}
		}

		public Point ClickablePoint {
			get {
				return new Point (double.NegativeInfinity, double.NegativeInfinity);
			}
		}

		public ControlType ControlType {
			get {
				return ControlType.Pane;
			}
		}

		public string FrameworkId {
			get {
				return string.Empty;
			}
		}

		public bool HasKeyboardFocus {
			get {
				return false;
			}
		}

		public string HelpText {
			get {
				return string.Empty;
			}
		}

		public bool IsContentElement {
			get {
				return true;
			}
		}

		public bool IsControlElement {
			get {
				return true;
			}
		}

		public bool IsEnabled {
			get {
				return true;
			}
		}

		public bool IsKeyboardFocusable {
			get {
				return false;
			}
		}

		public bool IsOffscreen {
			get {
				return false;
			}
		}

		public bool IsPassword {
			get {
				return false;
			}
		}

		public bool IsRequiredForForm {
			get {
				return false;
			}
		}

		public string ItemStatus {
			get {
				return string.Empty;
			}
		}

		public string ItemType {
			get {
				return string.Empty;
			}
		}

		public IElement LabeledBy {
			get {
				return null;
			}
		}

		public string LocalizedControlType {
			get {
				return ControlType.LocalizedControlType;
			}
		}

		public string Name {
			get {
				return "Desktop";
			}
		}

		public int NativeWindowHandle {
			get {
				return -1;
			}
		}

		public OrientationType Orientation {
			get {
				return OrientationType.None;
			}
		}

		public int ProcessId {
			get {
				return -1;
			}
		}

		public int[] RuntimeId {
			get {
				return new int[0];
			}
		}

		public IElement Parent {
			get {
				return null;
			}
		}

		public IElement FirstChild {
			get {
				throw new NotImplementedException ();
			}
		}

		public IElement LastChild {
			get {
				throw new NotImplementedException ();
			}
		}

		public IElement NextSibling {
			get {
				return null;
			}
		}

		public IElement PreviousSibling {
			get {
				return null;
			}
		}

		public IAutomationSource AutomationSource {
			get { return null; }
		}

		public object GetCurrentPattern (AutomationPattern pattern)
		{
			throw new InvalidOperationException ();
		}

		public AutomationPattern [] GetSupportedPatterns ()
		{
			return new AutomationPattern [0];
		}

		public AutomationProperty [] GetSupportedProperties ()
		{
			//TODO need to test whether other properties' values are 'NotSupported'
			return new AutomationProperty [] {
				AutomationElementIdentifiers.AcceleratorKeyProperty,
				AutomationElementIdentifiers.AccessKeyProperty,
				AutomationElementIdentifiers.AutomationIdProperty,
				AutomationElementIdentifiers.BoundingRectangleProperty,
				AutomationElementIdentifiers.ClassNameProperty,
				AutomationElementIdentifiers.ControlTypeProperty,
				AutomationElementIdentifiers.FrameworkIdProperty,
				AutomationElementIdentifiers.HasKeyboardFocusProperty,
				AutomationElementIdentifiers.HelpTextProperty,
				AutomationElementIdentifiers.ItemStatusProperty,
				AutomationElementIdentifiers.ItemTypeProperty,
				AutomationElementIdentifiers.IsControlElementProperty,
				AutomationElementIdentifiers.IsContentElementProperty,
				AutomationElementIdentifiers.IsEnabledProperty,
				AutomationElementIdentifiers.IsKeyboardFocusableProperty,
				AutomationElementIdentifiers.IsOffscreenProperty,
				AutomationElementIdentifiers.IsPasswordProperty,
				AutomationElementIdentifiers.IsRequiredForFormProperty,
				AutomationElementIdentifiers.LabeledByProperty,
				AutomationElementIdentifiers.LocalizedControlTypeProperty,
				AutomationElementIdentifiers.NameProperty,
				AutomationElementIdentifiers.OrientationProperty,
				AutomationElementIdentifiers.ProcessIdProperty,
				AutomationElementIdentifiers.RuntimeIdProperty
			};
		}

		public void SetFocus ()
		{
			// TODO: Need to test
		}

		public IElement GetDescendantFromPoint (double x, double y)
		{
			//Root element show have no child element which is not
			//a native window
			return this;
		}
		#endregion
	}
}
