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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Collections.Generic;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class TopLevelRootItem : ParentAdapter
	{
		private TopLevelRootItem ()
			//exception: TopLevelRootItem doesn't have an UIA counterpart
			: base (null)
		{
			this.Name = Monitor.GetProgramName ();
			this.Role = Atk.Role.Application;
		}
		
		private static volatile TopLevelRootItem instance = null;
		
		public static TopLevelRootItem Instance {
			get {
				if (instance == null)
					lock (syncRoot)
						if (instance == null)
							instance = new TopLevelRootItem ();
				return instance;
			}
		}

		public void SendWindowActivate ()
		{
			if (currentActiveWindow != null)
				GLib.Signal.Emit (currentActiveWindow, "activate");
		}

		private UiaAtkBridge.Window currentActiveWindow = null;
		
		internal override void AddOneChild (Atk.Object child)
		{
			//FIXME: figure out if we need the Embeds relationship in TopLevelRootItem (if yes, we should not call base)
			base.AddOneChild (child);
		}

		internal void CheckAndHandleNewActiveWindow (UiaAtkBridge.Window newWin)
		{
			if (object.ReferenceEquals (currentActiveWindow, newWin))
				return;
			
			if (currentActiveWindow != null)
				currentActiveWindow.LooseActiveState ();
			currentActiveWindow = newWin;
			currentActiveWindow.GainActiveState ();
		}
		
		public override void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			// TODO
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			// TODO
		}


	}
}
