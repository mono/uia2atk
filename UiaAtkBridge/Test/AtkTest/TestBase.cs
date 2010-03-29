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
// Copyright (c) 2008,2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Xml;

using System.Collections.Generic;

using NUnit.Framework;

namespace UiaAtkBridgeTest
{
	public abstract class TestBase {

		private EventCollection events = null;

		// Sync up with the GLib thread.  Should be called after the
		// name, role, or parent are changed in UiaAtkBridge when
			// checking for events, since we defer to an idle
			// handler to call atk to avoid deadlock when atk
			// emits signals. Called by RunInGuiThread in
			// UiaAtkBridge.
		public static void GlibSync ()
		{
			System.Threading.AutoResetEvent sync = new System.Threading.AutoResetEvent (false);
			GLib.Timeout.Add (0, new GLib.TimeoutHandler (delegate {
				sync.Set ();
				return false;
			}));
			sync.WaitOne ();
			sync.Close ();
		}

		protected void StartEventMonitor ()
		{
			events = null;
			EventMonitor.Start ();
		}

		protected void ExpectEvents (int count, Atk.Role role, string evType)
		{
			if (events == null)
				events = EventMonitor.Pause ();
			EventCollection evs = events.FindByRole (role).FindByType (evType);
			string eventsInXml = String.Format (" events in XML: {0}", Environment.NewLine + events.OriginalGrossXml);
			Assert.AreEqual (count, evs.Count, "bad number of " + evType + " events: " + eventsInXml);
		}

		protected void ExpectEvents (int count, Atk.Role role, string evType, int detail1)
		{
			GlibSync ();
			if (events == null)
				events = EventMonitor.Pause ();
			EventCollection evs = events.FindByRole (role).FindByType (evType).FindWithDetail1 (detail1.ToString ());
			string eventsInXml = String.Format (" events in XML: {0}", Environment.NewLine + events.OriginalGrossXml);
			Assert.AreEqual (count, evs.Count, "bad number of " + evType + " events: " + eventsInXml);
		}

		protected void ExpectEvents (int count, Atk.Role role, string evType, string name)
		{
			GlibSync ();
			if (events == null)
				events = EventMonitor.Pause ();
			EventCollection evs = events.FindByRole (role).FindByType (evType).FindByName (name);
			string eventsInXml = String.Format (" events in XML: {0}", Environment.NewLine + events.OriginalGrossXml);
			Assert.AreEqual (count, evs.Count, "bad number of " + evType + " events: " + eventsInXml);
		}

		protected void ExpectEvents (int min, int max, Atk.Role role, string evType)
		{
			GlibSync ();
			if (events == null)
				events = EventMonitor.Pause ();
			EventCollection evs = events.FindByRole (role).FindByType (evType);
			string eventsInXml = String.Format (" events in XML: {0}", Environment.NewLine + events.OriginalGrossXml);
			Assert.IsTrue (evs.Count >= min && evs.Count <= max, "Expected " + min +"-" + max +" " + evType + " events but got " + evs.Count +": " + eventsInXml);
		}

		public static void States (Atk.Object accessible, params Atk.StateType [] expected)
		{
			List <Atk.StateType> expectedStates = new List <Atk.StateType> (expected);
			List <Atk.StateType> missingStates = new List <Atk.StateType> ();
			List <Atk.StateType> superfluousStates = new List <Atk.StateType> ();

			Atk.StateSet stateSet = accessible.RefStateSet ();
			foreach (Atk.StateType state in Enum.GetValues (typeof (Atk.StateType))) {
				if (expectedStates.Contains (state) &&
				    (!(stateSet.ContainsState (state))))
					missingStates.Add (state);
				else if ((!expectedStates.Contains (state)) &&
					     (stateSet.ContainsState (state)))
					superfluousStates.Add (state);
			}

			string missingStatesMsg = string.Empty;
			string superfluousStatesMsg = string.Empty;

			if (missingStates.Count != 0) {
				missingStatesMsg = "Missing states: ";
				foreach (Atk.StateType state in missingStates)
					missingStatesMsg += state.ToString () + ",";
			}
			if (superfluousStates.Count != 0) {
				superfluousStatesMsg = "Superfluous states: ";
				foreach (Atk.StateType state in superfluousStates)
					superfluousStatesMsg += state.ToString () + ",";
			}
			Assert.IsTrue ((missingStates.Count == 0) && (superfluousStates.Count == 0),
				missingStatesMsg + " .. " + superfluousStatesMsg);
		}

	}
}
