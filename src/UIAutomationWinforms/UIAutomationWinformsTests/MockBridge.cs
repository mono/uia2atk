//// Permission is hereby granted, free of charge, to any person obtaining 
//// a copy of this software and associated documentation files (the 
//// "Software"), to deal in the Software without restriction, including 
//// without limitation the rights to use, copy, modify, merge, publish, 
//// distribute, sublicense, and/or sell copies of the Software, and to 
//// permit persons to whom the Software is furnished to do so, subject to 
//// the following conditions: 
////  
//// The above copyright notice and this permission notice shall be 
//// included in all copies or substantial portions of the Software. 
////  
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
//// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
//// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
//// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
//// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
//// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//// 
//// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
//// 
//// Authors: 
////      Sandy Armstrong <sanfordarmstrong@gmail.com>
//// 
//

using System;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Bridge;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	public class MockBridge : IAutomationBridge
	{
#region Tuple Classes
		public class AutomationEventTuple
		{
			public AutomationEvent eventId;
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
		public bool ClientsAreListening { get; set; }
		
		public object HostProviderFromHandle (IntPtr hwnd)
		{
			throw new NotImplementedException ();
		}

		
		public void RaiseAutomationEvent (AutomationEvent eventId, object provider, AutomationEventArgs e)
		{			
			AutomationEvents.Add (new AutomationEventTuple {
				eventId = eventId, provider = provider, e = e});
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
#endregion
	}
}
