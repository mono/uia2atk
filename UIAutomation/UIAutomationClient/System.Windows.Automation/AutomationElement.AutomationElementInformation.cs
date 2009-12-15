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
	public sealed partial class AutomationElement
	{
		public struct AutomationElementInformation
		{
			private AutomationElement element;
			bool cache;

			internal AutomationElementInformation (AutomationElement element, bool cache)
			{
				this.element = element;
				this.cache = cache;
			}

			public string AcceleratorKey {
				get {
					return (string) element.GetPropertyValue (AutomationElement.AcceleratorKeyProperty, cache);
				}
			}

			public string AccessKey {
				get {
					return (string) element.GetPropertyValue (AutomationElement.AccessKeyProperty, cache);
				}
			}

			public string AutomationId {
				get {
					return (string) element.GetPropertyValue (AutomationElement.AutomationIdProperty, cache);
				}
			}

			public Rect BoundingRectangle {
				get {
					return (Rect) element.GetPropertyValue (AutomationElement.BoundingRectangleProperty, cache);
				}
			}

			public string ClassName {
				get {
					return (string) element.GetPropertyValue (AutomationElement.ClassNameProperty, cache);
				}
			}

			public ControlType ControlType {
				get {
					return (ControlType) element.GetPropertyValue (AutomationElement.ControlTypeProperty, cache);
				}
			}

			public string FrameworkId {
				get {
					return (string) element.GetPropertyValue (AutomationElement.FrameworkIdProperty, cache);
				}
			}

			public bool HasKeyboardFocus {
				get {
					return (bool) element.GetPropertyValue (AutomationElement.HasKeyboardFocusProperty, cache);
				}
			}

			public string HelpText {
				get {
					return (string) element.GetPropertyValue (AutomationElement.HelpTextProperty, cache);
				}
			}

			public bool IsContentElement {
				get {
					return (bool) element.GetPropertyValue (AutomationElement.IsContentElementProperty, cache);
				}
			}

			public bool IsControlElement {
				get {
					return (bool) element.GetPropertyValue (AutomationElement.IsControlElementProperty, cache);
				}
			}

			public bool IsEnabled {
				get {
					return (bool) element.GetPropertyValue (AutomationElement.IsEnabledProperty, cache);
				}
			}

			public bool IsKeyboardFocusable {
				get {
					return (bool) element.GetPropertyValue (AutomationElement.IsKeyboardFocusableProperty, cache);
				}
			}

			public bool IsOffscreen {
				get {
					return (bool) element.GetPropertyValue (AutomationElement.IsOffscreenProperty, cache);
				}
			}

			public bool IsPassword {
				get {
					return (bool) element.GetPropertyValue (AutomationElement.IsPasswordProperty, cache);
				}
			}

			public bool IsRequiredForForm {
				get {
					return (bool) element.GetPropertyValue (AutomationElement.IsRequiredForFormProperty, cache);
				}
			}

			public string ItemStatus {
				get {
					return (string) element.GetPropertyValue (AutomationElement.ItemStatusProperty, cache);
				}
			}

			public string ItemType {
				get {
					return (string) element.GetPropertyValue (AutomationElement.ItemTypeProperty, cache);
				}
			}

			public AutomationElement LabeledBy {
				get {
					return (AutomationElement) element.GetPropertyValue (AutomationElement.LabeledByProperty, cache);
				}
			}

			public string LocalizedControlType {
				get {
					return (string) element.GetPropertyValue (AutomationElement.LocalizedControlTypeProperty, cache);
				}
			}

			public string Name {
				get {
					return (string) element.GetPropertyValue (AutomationElement.NameProperty, cache);
				}
			}

			public int NativeWindowHandle {
				get {
					return (int) element.GetPropertyValue (AutomationElement.NativeWindowHandleProperty, cache);
				}
			}

			public OrientationType Orientation {
				get {
					return (OrientationType) element.GetPropertyValue (AutomationElement.OrientationProperty, cache);
				}
			}

			public int ProcessId {
				get {
					return (int) element.GetPropertyValue (AutomationElement.ProcessIdProperty, cache);
				}
			}
		}
	}
}
