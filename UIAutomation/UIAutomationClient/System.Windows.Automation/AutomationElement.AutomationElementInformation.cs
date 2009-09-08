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
			private IElement sourceElement;

			internal AutomationElementInformation (IElement sourceElement)
			{
				this.sourceElement = sourceElement;
			}

			public string AcceleratorKey {
				get { return sourceElement.AcceleratorKey; }
			}

			public string AccessKey {
				get { return sourceElement.AccessKey; }
			}

			public string AutomationId {
				get { return sourceElement.AutomationId; }
			}

			public Rect BoundingRectangle {
				get { return sourceElement.BoundingRectangle; }
			}

			public string ClassName {
				get { return sourceElement.ClassName; }
			}

			public ControlType ControlType {
				get { return sourceElement.ControlType; }
			}

			public string FrameworkId {
				get { return sourceElement.FrameworkId; }
			}

			public bool HasKeyboardFocus {
				get { return sourceElement.HasKeyboardFocus; }
			}

			public string HelpText {
				get { return sourceElement.HelpText; }
			}

			public bool IsContentElement {
				get { return sourceElement.IsContentElement; }
			}

			public bool IsControlElement {
				get { return sourceElement.IsControlElement; }
			}

			public bool IsEnabled {
				get { return sourceElement.IsEnabled; }
			}

			public bool IsKeyboardFocusable {
				get { return sourceElement.IsKeyboardFocusable; }
			}

			public bool IsOffscreen {
				get { return sourceElement.IsOffscreen; }
			}

			public bool IsPassword {
				get { return sourceElement.IsPassword; }
			}

			public bool IsRequiredForForm {
				get { return sourceElement.IsRequiredForForm; }
			}

			public string ItemStatus {
				get { return sourceElement.ItemStatus; }
			}

			public string ItemType {
				get { return sourceElement.ItemType; }
			}

			public AutomationElement LabeledBy {
				get { return SourceManager.GetOrCreateAutomationElement (sourceElement.LabeledBy); }
			}

			public string LocalizedControlType {
				get { return sourceElement.LocalizedControlType; }
			}

			public string Name {
				get { return sourceElement.Name; }
			}

			public int NativeWindowHandle {
				get { return sourceElement.NativeWindowHandle; }
			}

			public OrientationType Orientation {
				get { return sourceElement.Orientation; }
			}

			public int ProcessId {
				get { return sourceElement.ProcessId; }
			}
		}
	}
}
