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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class AutomationBridge : IAutomationBridge
	{
#region Private Fields
		
		private bool applicationStarted = false;
		private Monitor appMonitor = null;
		
#endregion

#region Public Constructor
		
		public AutomationBridge()
		{
			bool newMonitor = false;
			if (appMonitor == null) {
				Console.WriteLine ("about to create monitor");
				appMonitor = new Monitor();
				Console.WriteLine ("just made monitor");
			}
			
		}
		
#endregion
		
#region IAutomationBridge Members
		
		public bool ClientsAreListening {
			get {
				return true;
			}
		}
		
		public void RaiseAutomationEvent (AutomationEvent eventId, object provider, AutomationEventArgs e)
		{
			// TODO: Find better way to pass PreRun event on to bridge
			//        (nullx3 is a magic value)
			if (eventId == null && provider == null && e == null) {
				if (!applicationStarted && appMonitor != null)
					appMonitor.ApplicationStarts ();
			}
			
			// TODO: Handle other events
		}
		
		public void RaiseAutomationPropertyChangedEvent (object element, AutomationPropertyChangedEventArgs e)
		{
			throw new NotImplementedException ();
		}
		
		public void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			IWindowProvider windowProvider = provider as IWindowProvider;

			if (e.StructureChangeType == StructureChangeType.ChildrenBulkAdded)
			{
				// TODO: Probably more efficient to check some properties
				//       on IRawElementProviderSimple (two casts total)
				if (windowProvider != null)
					HandleNewWindowProvider (windowProvider);
				
				// TODO: Other providers
			}
			else if (e.StructureChangeType == StructureChangeType.ChildrenBulkRemoved) 
			{
				if (windowProvider != null)
					HandleWindowProviderRemoval (windowProvider);
			}
			
			// TODO: Other structure changes
		}
		
#endregion
		
#region Private Methods
		
		private void HandleNewWindowProvider (IWindowProvider provider)
		{
			appMonitor.FormIsAdded (provider);
		}
		
		private void HandleWindowProviderRemoval (IWindowProvider provider)
		{
			appMonitor.FormIsRemoved (provider);
		}
		
#endregion
	}
}
