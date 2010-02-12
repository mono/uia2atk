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
	public delegate void AutomationEventHandler (int handlerId, int eventId, string providerPath);
	public delegate void AutomationPropertyChangedHandler (int handlerId, int eventId, string providerPath, int propertyId, object oldValue, object newValue);
	public delegate void StructureChangedHandler (int handlerId, int eventId, string providerPath, StructureChangeType changeType);
	public delegate void VoidHandler ();
	public delegate void FocusChangedHandler (string providerPath);

	[Interface (Constants.ApplicationInterfaceName)]
	public interface IApplication
	{
		string [] GetRootElementPaths ();
		string GetFocusedElementPath ();

		string GetElementPathFromHandle (int handle);

		void AddAutomationEventHandler (int eventId, int [] elementRuntimeId, TreeScope scope, int handlerId);
		void AddRootElementAutomationEventHandler (int eventId, TreeScope scope, int handlerId);
		void AddAutomationPropertyChangedEventHandler (int [] elementRuntimeId, TreeScope scope, int handlerId, int[] properties);
		void AddRootElementAutomationPropertyChangedEventHandler (TreeScope scope, int handlerId, int[] properties);
		void AddStructureChangedEventHandler (int [] elementRuntimeId, TreeScope scope, int handlerId);
		void AddRootElementStructureChangedEventHandler (TreeScope scope, int handlerId);

		void RemoveAutomationEventHandler (int eventId, int [] elementRuntimeId, int handlerId);
		void RemoveRootElementAutomationEventHandler (int eventId, int handlerId);
		void RemoveAutomationPropertyChangedEventHandler (int [] elementRuntimeId, int handlerId);
		void RemoveRootElementAutomationPropertyChangedEventHandler (int handlerId);
		void RemoveStructureChangedEventHandler (int [] elementRuntimeId, int handlerId);
		void RemoveRootElementStructureChangedEventHandler (int handlerId);
		void RemoveAllEventHandlers (int handlerIdMask);

		event AutomationEventHandler AutomationEvent;
		event AutomationPropertyChangedHandler AutomationPropertyChanged;
		event StructureChangedHandler StructureChanged;
		event VoidHandler RootElementsChanged;
		event FocusChangedHandler FocusChanged;
	}
}
