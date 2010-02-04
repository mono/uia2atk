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
using System.Windows.Automation;
using NDesk.DBus;

namespace Mono.UIAutomation.UiaDbus.Interfaces
{
	[Interface (Constants.AutomationElementInterfaceName)]
	public interface IAutomationElement
	{
		bool SupportsProperty (int propertyId);

		string AcceleratorKey { get; }

		string AccessKey { get; }

		string AutomationId { get; }

		Rect BoundingRectangle { get; }

		string ClassName { get; }

		Point ClickablePoint { get; }

		int ControlTypeId { get; }

		string FrameworkId { get; }

		bool HasKeyboardFocus { get; }

		string HelpText { get; }

		bool IsContentElement { get; }

		bool IsControlElement { get; }

		bool IsEnabled { get; }

		bool IsKeyboardFocusable { get; }

		bool IsOffscreen { get; }

		bool IsPassword { get; }

		bool IsRequiredForForm { get; }

		string ItemStatus { get; }

		string ItemType { get; }

		string LabeledByElementPath { get; }

		string LocalizedControlType { get; }

		string Name { get; }

		int NativeWindowHandle { get; }

		OrientationType Orientation { get; }

		int ProcessId { get; }

		int [] RuntimeId { get; }

		string ParentElementPath { get; }

		string FirstChildElementPath { get; }

		string LastChildElementPath { get; }

		string NextSiblingElementPath { get; }

		string PreviousSiblingElementPath { get; }

		string GetCurrentPatternPath (int patternId);

		int [] SupportedPatternIds { get; }

		int [] SupportedPropertyIds { get; }

		void SetFocus ();

		string GetDescendantPathFromPoint (double x, double y);
	}
}
