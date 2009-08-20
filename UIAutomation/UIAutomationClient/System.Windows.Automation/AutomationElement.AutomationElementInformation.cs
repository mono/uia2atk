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
			internal AutomationElementInformation (IElement sourceElement)
			{
				this.AcceleratorKey = sourceElement.AcceleratorKey;
				this.AccessKey = sourceElement.AccessKey;
				this.AutomationId = sourceElement.AutomationId;
				this.BoundingRectangle = sourceElement.BoundingRectangle;
				this.ClassName = sourceElement.ClassName;
				this.ControlType = sourceElement.ControlType;
				this.FrameworkId = sourceElement.FrameworkId;
				this.HasKeyboardFocus = sourceElement.HasKeyboardFocus;
				this.HelpText = sourceElement.HelpText;
				this.IsContentElement = sourceElement.IsContentElement;
				this.IsControlElement = sourceElement.IsControlElement;
				this.IsEnabled = sourceElement.IsEnabled;
				this.IsKeyboardFocusable = sourceElement.IsKeyboardFocusable;
				this.IsOffscreen = sourceElement.IsOffscreen;
				this.IsPassword = sourceElement.IsPassword;
				this.IsRequiredForForm = sourceElement.IsRequiredForForm;
				this.ItemStatus = sourceElement.ItemStatus;
				this.ItemType = sourceElement.ItemType;
				this.LabeledBy = SourceManager.GetOrCreateAutomationElement (sourceElement.LabeledBy);
				this.LocalizedControlType = sourceElement.LocalizedControlType;
				this.Name = sourceElement.Name;
				this.NativeWindowHandle = sourceElement.NativeWindowHandle;
				this.Orientation = sourceElement.Orientation;
				this.ProcessId = sourceElement.ProcessId;
			}

			public string AcceleratorKey {
				get; private set;
			}

			public string AccessKey {
				get; private set;
			}

			public string AutomationId {
				get; private set;
			}

			public Rect BoundingRectangle {
				get; private set;
			}

			public string ClassName {
				get; private set;
			}

			public ControlType ControlType {
				get; private set;
			}

			public string FrameworkId {
				get; private set;
			}

			public bool HasKeyboardFocus {
				get; private set;
			}

			public string HelpText {
				get; private set;
			}

			public bool IsContentElement {
				get; private set;
			}

			public bool IsControlElement {
				get; private set;
			}

			public bool IsEnabled {
				get; private set;
			}

			public bool IsKeyboardFocusable {
				get; private set;
			}

			public bool IsOffscreen {
				get; private set;
			}

			public bool IsPassword {
				get; private set;
			}

			public bool IsRequiredForForm {
				get; private set;
			}

			public string ItemStatus {
				get; private set;
			}

			public string ItemType {
				get; private set;
			}

			public AutomationElement LabeledBy {
				get; private set;
			}

			public string LocalizedControlType {
				get; private set;
			}

			public string Name {
				get; private set;
			}

			public int NativeWindowHandle {
				get; private set;
			}

			public OrientationType Orientation {
				get; private set;
			}

			public int ProcessId {
				get; private set;
			}
		}
	}
}