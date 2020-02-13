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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 


using System;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Bridge;

namespace MonoTests.Mono.UIAutomation.Winforms
{
#region Tuple Classes
		public class AutomationEventTuple
		{
			public object provider;
			public AutomationEventArgs e;
		}
		
		public class AutomationPropertyChangedEventTuple
		{
			public object element;
			public AutomationPropertyChangedEventArgs e;
		}
		
		public class StructureChangedEventTuple
		{
			public object provider;
			public StructureChangedEventArgs e;
		}
#endregion
	
	public class MockBridge : IAutomationBridge
	{
		
#region Public Members
		public List<AutomationEventTuple> AutomationEvents =
			new List<AutomationEventTuple> ();
		public List<AutomationPropertyChangedEventTuple> AutomationPropertyChangedEvents =
			new List<AutomationPropertyChangedEventTuple> ();
		public List<StructureChangedEventTuple> StructureChangedEvents =
			new List<StructureChangedEventTuple> ();
		
		public void ResetEventLists ()
		{
			AutomationEvents.Clear ();
			AutomationPropertyChangedEvents.Clear ();
			StructureChangedEvents.Clear ();
		}
#endregion
	
#region IAutomationBridge Members
		public bool IsAccessibilityEnabled { 
			get { return true; }
		}
		
		public bool ClientsAreListening { get; set; }
		
		public object HostProviderFromHandle (IntPtr hwnd)
		{
			throw new NotImplementedException ();
		}

		
		public void RaiseAutomationEvent (object provider, AutomationEventArgs e)
		{
			AutomationEvents.Add (new AutomationEventTuple {provider = provider, e = e});
		}

		public void RaiseAutomationPropertyChangedEvent (object element, AutomationPropertyChangedEventArgs e)
		{
			AutomationPropertyChangedEvents.Add (new AutomationPropertyChangedEventTuple {
				element = element, e = e});
		}

		public void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			StructureChangedEvents.Add (new StructureChangedEventTuple {
				provider = provider, e = e});
		}

		public void Initialize () {}

		public void Terminate () {}
#endregion
		
		public int GetAutomationEventCount (AutomationEvent eventId)
		{
			int count = 0;
			
			foreach (AutomationEventTuple evnt in AutomationEvents) {
				if (evnt.e.EventId.Id == eventId.Id)
					count++;
			}
			return count;
		}
		
		public int GetAutomationPropertyEventCount (AutomationProperty propertyId)
		{
			int count = 0;
			
			foreach (AutomationPropertyChangedEventTuple evnt in AutomationPropertyChangedEvents) {
				if (evnt.e.Property.Id == propertyId.Id)
					count++;
			}
			return count;
		}

		public int GetStructureChangedEventCount (StructureChangeType changeType)
		{
			int count = 0;
			
			foreach (StructureChangedEventTuple tuple in StructureChangedEvents) {
				if (tuple.e.StructureChangeType == changeType)
					count++;
			}
			return count;
		}
		
		public AutomationPropertyChangedEventTuple GetAutomationPropertyEventAt (int index)
		{
			if (AutomationPropertyChangedEvents.Count >= index || index < 0)
				return null;
			
			return AutomationPropertyChangedEvents [index];
		}
		
		public AutomationEventTuple GetAutomationEventAt (int index)
		{
			if (index >= AutomationEvents.Count || index < 0)
				return null;
			
			return AutomationEvents [index];
		}

		public StructureChangedEventTuple GetStructureChangedEventAt (int index,
		                                                              StructureChangeType changeType)
		{
			if (index >= StructureChangedEvents.Count || index < 0)
				return null;

			int counter = 0;
			foreach (StructureChangedEventTuple tuple in StructureChangedEvents) {
				if (tuple.e.StructureChangeType == changeType) {
					if (counter++ == index) 
					    return tuple;
				}
			}			
			
			return null;
		}
		
		public AutomationPropertyChangedEventTuple GetAutomationPropertyEventFrom (object element, int id)
		{
			foreach (AutomationPropertyChangedEventTuple tuple in AutomationPropertyChangedEvents) {
				if (tuple.element == element && tuple.e.Property.Id == id)
					return tuple;
			}
			
			
			return null;
		}

		public AutomationEventTuple GetAutomationEventFrom (object provider, int id)
		{
			foreach (AutomationEventTuple tuple in AutomationEvents)
				if (tuple.provider == provider && tuple.e.EventId.Id == id)
					return tuple;
			return null;
		}

		public StructureChangedEventTuple GetStructureChangedEventFrom (StructureChangeType changeType)
		{
			foreach (StructureChangedEventTuple tuple in StructureChangedEvents) {
				if (tuple.e.StructureChangeType == changeType)
					return tuple;
			}

			return null;
		}

		public StructureChangedEventTuple GetStructureChangedEventFrom (object element, StructureChangeType changeType)
		{
			foreach (StructureChangedEventTuple tuple in StructureChangedEvents) {
				if (tuple.e.StructureChangeType == changeType && tuple.provider == element)
					return tuple;
			}

			return null;
		}
	}
}
