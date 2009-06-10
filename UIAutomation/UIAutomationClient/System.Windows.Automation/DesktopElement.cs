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
				// TODO: Should be rect of entire desktop
				return Rect.Empty;
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
				return string.Empty;
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
		#endregion
	}
}
