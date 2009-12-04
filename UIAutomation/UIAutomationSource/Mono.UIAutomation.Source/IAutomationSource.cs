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
using System.Windows;
using System.Windows.Automation;

namespace Mono.UIAutomation.Source
{
	//We can't use the AutomationFocusChangedEventHandler since it's defined in UIAutomationClient.dll
	//For most source implementation on Linux, childId and objectId can be ignored, since they're
	//traditional Microsoft Active Accessibility identifiers.
	public delegate void FocusChangedEventHandler (IElement element, int objectId, int childId);

	public interface IAutomationSource
	{
		void Initialize ();

		IElement [] GetRootElements ();

		event EventHandler RootElementsChanged;

		IElement GetFocusedElement ();

		IElement GetElementFromHandle (IntPtr handle);

		bool IsAccessibilityEnabled { get; }

		void AddAutomationEventHandler (AutomationEvent eventId,
		                           IElement element,
		                           TreeScope scope,
		                           AutomationEventHandler eventHandler);

		void AddAutomationPropertyChangedEventHandler (IElement element,
		                                               TreeScope scope,
		                                               AutomationPropertyChangedEventHandler eventHandler,
		                                               AutomationProperty [] properties);

		void AddStructureChangedEventHandler (IElement element,
		                                      TreeScope scope,
		                                      StructureChangedEventHandler eventHandler);

		void AddAutomationFocusChangedEventHandler (FocusChangedEventHandler eventHandler);

		void RemoveAutomationEventHandler (AutomationEvent eventId,
		                                   IElement element,
		                                   AutomationEventHandler eventHandler);

		void RemoveAutomationPropertyChangedEventHandler (IElement element,
		                                                  AutomationPropertyChangedEventHandler eventHandler);

		void RemoveStructureChangedEventHandler (IElement element,
		                                         StructureChangedEventHandler eventHandler);

		void RemoveAutomationFocusChangedEventHandler (FocusChangedEventHandler eventHandler);

		void RemoveAllEventHandlers ();
	}
}
