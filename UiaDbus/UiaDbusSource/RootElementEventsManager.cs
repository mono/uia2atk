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
// Copyright (c) 2010 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//  Matt Guo <matt@mattguo.com>
// 

using System;
using System.Collections.Generic;
using System.Windows.Automation;

namespace Mono.UIAutomation.UiaDbusSource
{
	internal static class RootElementEventsManager
	{
		private static List<AutomationEventRequest> activeAutomationEvents =
			new List<AutomationEventRequest> ();
		private static List<PropertyEventRequest> activePropertyEvents =
			new List<PropertyEventRequest> ();
		private static List<StructureEventRequest> activeStructureEvents =
			new List<StructureEventRequest> ();

		private static void AddEventRequest<T> (T request, List<T> requests)
			where T : class
		{
			lock (requests)
				requests.Add (request);
		}

		private static void RemoveEventRequest<T> (List<T> requests, Predicate<T> pred)
			where T : class
		{
			lock (requests) {
				List<T> itemsToDelete = new List<T> ();
				foreach (T r in requests) {
					if (pred (r))
						itemsToDelete.Add (r);
				}
				foreach (T r in itemsToDelete)
					requests.Remove (r);
			}
		}

		internal static void AddAutomationEventRequest (int eventId, TreeScope scope, int handlerId)
		{
			var req = new AutomationEventRequest (
				eventId, scope, handlerId);
			AddEventRequest (req, activeAutomationEvents);
		}

		internal static void AddPropertyEventRequest (TreeScope scope, int handlerId, int [] propIds)
		{
			var req = new PropertyEventRequest (
				scope, handlerId, propIds);
			AddEventRequest (req, activePropertyEvents);
		}

		internal static void AddStructureEventRequest (TreeScope scope, int handlerId)
		{
			var req = new StructureEventRequest (scope, handlerId);
			AddEventRequest (req, activeStructureEvents);
		}

		internal static AutomationEventRequest [] ActiveAutomationEventRequests {
			get {
				lock (activeAutomationEvents)
					return activeAutomationEvents.ToArray ();
			}
		}

		internal static PropertyEventRequest [] ActivePropertyEventRequests {
			get {
				lock (activePropertyEvents)
					return activePropertyEvents.ToArray ();
			}
		}

		internal static StructureEventRequest [] ActiveStructureEventRequests {
			get {
				lock (activeStructureEvents)
					return activeStructureEvents.ToArray ();
			}
		}

		internal static void RemoveAutomationEventRequest (int eventId, int handlerId)
		{
			RemoveEventRequest (activeAutomationEvents,
			                    r => r.EventId == eventId &&
			                    r.HandlerId == handlerId);
		}

		internal static void RemovePropertyEventRequest (int handlerId)
		{
			RemoveEventRequest (activePropertyEvents,
			                    r => r.HandlerId == handlerId);
		}

		internal static void RemoveStructureEventRequest (int handlerId)
		{
			RemoveEventRequest (activeStructureEvents,
			                    r => r.HandlerId == handlerId);
		}

		internal static void RemoveAll ()
		{
			lock (activeAutomationEvents)
				activeAutomationEvents.Clear ();
			lock (activePropertyEvents)
				activePropertyEvents.Clear ();
			lock (activeStructureEvents)
				activeStructureEvents.Clear ();
		}

		internal class AutomationEventRequest
		{
			public AutomationEventRequest (int eventId, TreeScope scope, int handlerId)
			{
				EventId = eventId;
				Scope = scope;
				HandlerId = handlerId;
			}

			public int EventId { get; private set; }
			public TreeScope Scope { get; private set; }
			public int HandlerId { get; private set; }
		}

		internal class PropertyEventRequest
		{
			public PropertyEventRequest (TreeScope scope, int handlerId, int [] propIds)
			{
				Scope = scope;
				HandlerId = handlerId;
				PropertyIds = (int []) propIds.Clone ();
			}

			public TreeScope Scope { get; private set; }
			public int HandlerId { get; private set; }
			public int [] PropertyIds { get; private set; }
		}

		internal class StructureEventRequest
		{
			public StructureEventRequest (TreeScope scope, int handlerId)
			{
				Scope = scope;
				HandlerId = handlerId;
			}

			public TreeScope Scope { get; private set; }
			public int HandlerId { get; private set; }
		}
	}
}
