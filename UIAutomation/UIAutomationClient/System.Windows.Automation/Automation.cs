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
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;

namespace System.Windows.Automation
{
	
	
	public static class Automation
	{
		public static bool Compare (int [] runtimeId1, int [] runtimeId2)
		{
			throw new NotImplementedException ();
		}

		public static bool Compare (AutomationElement el1, AutomationElement el2)
		{
			throw new NotImplementedException ();
		}

		public static string PatternName (AutomationPattern pattern)
		{
			throw new NotImplementedException ();
		}

		public static string PropertyName (AutomationProperty property)
		{
			throw new NotImplementedException ();
		}

		public static void AddAutomationEventHandler (AutomationEvent eventId,
		                                       AutomationElement element,
		                                       TreeScope scope,
		                                       AutomationEventHandler eventHandler)
		{
			throw new NotImplementedException ();
		}

		public static void AddAutomationFocusChangedEventHandler (AutomationFocusChangedEventHandler eventHandler)
		{
			throw new NotImplementedException ();
		}

		public static void AddAutomationPropertyChangedEventHandler (AutomationElement element,
		                                                      TreeScope scope,
		                                                      AutomationPropertyChangedEventHandler eventHandler,
		                                                      AutomationProperty [] properties)
		{
			throw new NotImplementedException ();
		}

		public static void AddStructureChangedEventHandler (AutomationElement element,
		                                             TreeScope scope,
		                                             StructureChangedEventHandler eventHandler)
		{
			throw new NotImplementedException ();
		}

		public static void RemoveAllEventHandlers ()
		{
			throw new NotImplementedException ();
		}

		public static void RemoveAutomationEventHandler (AutomationEvent eventId,
		                                          AutomationElement element,
		                                          AutomationEventHandler eventHandler)
		{
			throw new NotImplementedException ();
		}

		public static void RemoveAutomationFocusChangedEventHandler (AutomationFocusChangedEventHandler eventHandler)
		{
			throw new NotImplementedException ();
		}

		public static void RemoveAutomationPropertyChangedEventHandler (AutomationElement element,
		                                                         AutomationPropertyChangedEventHandler eventHandler)
		{
			throw new NotImplementedException ();
		}

		public static void RemoveStructureChangedEventHandler (AutomationElement element,
		                                                StructureChangedEventHandler eventHandler)
		{
			throw new NotImplementedException ();
		}

		public static readonly Condition ContentViewCondition;

		public static readonly Condition ControlViewCondition;

		public static readonly Condition RawViewCondition;
	}
}
