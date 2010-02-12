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
//  Matt Guo <matt@mattguo.com>
// 

using System;
using System.Collections.Generic;
using System.Windows.Automation;

namespace Mono.UIAutomation.UiaDbusSource
{

	//HandlerId format:
	// 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 
	//+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
	//|          Process Id           |        Handler Serial No.     |
	//+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
	internal class EventHandlerManager
	{
		private static object staticLock = new object ();
		private static readonly int clientPrefix;
		private static int handlerSerialNo;

		static EventHandlerManager ()
		{
			handlerSerialNo = 0;
			clientPrefix = System.Diagnostics.Process.GetCurrentProcess().Id << 16;
		}

		private static int NewHandlerId ()
		{
			lock (staticLock) {
				handlerSerialNo++;
				return clientPrefix ^ handlerSerialNo;
			}
		}

		private static int RegisterEventHandler<T> (T handler, Dictionary<int, T> handlerList)
			where T : class
		{
			lock (handlerList) {
				foreach (int id in handlerList.Keys) {
					if (handlerList[id] == handler)
						return id;
				}
				int newId = NewHandlerId ();
				handlerList.Add (newId, handler);
				return newId;
			}
		}

		private static T GetHandlerById<T> (int handlerId, Dictionary<int, T> handlerList)
			where T : class
		{
			T handler;
			lock (handlerList) {
				if (!handlerList.TryGetValue (handlerId, out handler))
					handler = null;
			}
			return handler;
		}

		private static int GetIdByHandler<T> (T handler, Dictionary<int, T> handlerList)
			where T : class
		{
			int ret = -1;
			lock (handlerList) {
				foreach (var id in handlerList.Keys) {
					if (handlerList[id] == handler) {
						ret = id;
						break;
					}
				}
			}
			return ret;
		}

		private Dictionary <int, AutomationEventHandler> automationEventHandlers
			= new Dictionary<int, AutomationEventHandler> ();
		private Dictionary <int, AutomationPropertyChangedEventHandler> propEventHandlers
			= new Dictionary<int, AutomationPropertyChangedEventHandler> ();
		private Dictionary <int, StructureChangedEventHandler> structureEventHandlers
			= new Dictionary<int, StructureChangedEventHandler> ();

		public int RegisterAutomationEventHandler (AutomationEventHandler handler)
		{
			return RegisterEventHandler (handler, automationEventHandlers);
		}

		public AutomationEventHandler GetAutomationEventHandlerById (int handlerId)
		{
			return GetHandlerById (handlerId, automationEventHandlers);
		}

		public int GetAutomationEventIdByHandler (AutomationEventHandler handler)
		{
			return GetIdByHandler (handler, automationEventHandlers);
		}

		public int RegisterPropertyEventHandler (AutomationPropertyChangedEventHandler handler)
		{
			return RegisterEventHandler (handler, propEventHandlers);
		}

		public AutomationPropertyChangedEventHandler GetPropertyEventHandlerById (int handlerId)
		{
			return GetHandlerById (handlerId, propEventHandlers);
		}

		public int GetPropertyEventIdByHandler (AutomationPropertyChangedEventHandler handler)
		{
			return GetIdByHandler (handler, propEventHandlers);
		}

		public int RegisterStructureEventHandler (StructureChangedEventHandler handler)
		{
			return RegisterEventHandler (handler, structureEventHandlers);
		}

		public StructureChangedEventHandler GetStructureEventHandlerById (int handlerId)
		{
			return GetHandlerById (handlerId, structureEventHandlers);
		}

		public int GetStructureEventIdByHandler (StructureChangedEventHandler handler)
		{
			return GetIdByHandler (handler, structureEventHandlers);
		}

		public static int ClientPrefix {
			get { return clientPrefix; }
		}
	}
}

