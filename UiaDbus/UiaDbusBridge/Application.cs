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
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.UiaDbus.Interfaces;
using Mono.UIAutomation.UiaDbusBridge.Wrappers;

namespace Mono.UIAutomation.UiaDbusBridge
{
	internal class Application : IApplication
	{
		private List<ProviderElementWrapper> rootElements;
		private List<AutomationEventHandlerData> automationEventHandlers;
		private List<AutomationPropertyChangedHandlerData> propertyEventHandlers;
		private List<AutomationEventHandlerData> structureEventHandlers;

		public Application ()
		{
			rootElements = new List<ProviderElementWrapper> ();
			automationEventHandlers = new List<AutomationEventHandlerData> ();
			propertyEventHandlers = new List<AutomationPropertyChangedHandlerData> ();
			structureEventHandlers = new List<AutomationEventHandlerData> ();
		}

		public void AddRootElement (ProviderElementWrapper element)
		{
			if (element != null && !rootElements.Contains (element))
				rootElements.Add (element);
		}

		public void RemoveRootElement (ProviderElementWrapper element)
		{
			if (element != null && rootElements.Contains (element))
				rootElements.Remove (element);
		}

		internal void RemoveProvider (IRawElementProviderSimple provider)
		{
			RemoveProviderFromHandlers (automationEventHandlers, provider);
			RemoveProviderFromHandlers (propertyEventHandlers, provider);
			RemoveProviderFromHandlers (structureEventHandlers, provider);
		}

		internal void RaiseAutomationEvent (AutomationEvent eventId, object provider, AutomationEventArgs e)
		{
			IRawElementProviderSimple simpleProvider = provider as IRawElementProviderSimple;
			if (simpleProvider == null)
				return;
			foreach (AutomationEventHandlerData handler in automationEventHandlers) {
				if (handler.EventId == eventId.Id &&
				    IsProviderInScope (simpleProvider, handler.Provider, handler.Scope)) {
					var wrapper = AutomationBridge.Instance.FindWrapperByPovider (simpleProvider);
					if (wrapper == null)
						throw new Exception ("Inconsistent provider -> wrapper mapping state");
					AutomationEvent (handler.HandlerId, handler.EventId, wrapper.Path);
				}
			}
		}

		internal void RaiseAutomationPropertyChangedEvent (object provider, AutomationPropertyChangedEventArgs e)
		{
			IRawElementProviderSimple simpleProvider = provider as IRawElementProviderSimple;
			if (simpleProvider == null)
				return;
			foreach (AutomationPropertyChangedHandlerData handler in propertyEventHandlers) {
				if (IsProviderInScope (simpleProvider, handler.Provider, handler.Scope)) {
					int eventPropId = e.Property.Id;
					foreach (int propId in handler.Properties) {
						if (eventPropId == propId) {
							var wrapper = AutomationBridge.Instance.FindWrapperByPovider (simpleProvider);
							if (wrapper == null)
								throw new Exception ("Inconsistent provider -> wrapper mapping state");
							//todo Need to add a general class to serialize/deserialize OldValue/Newvalue
							//over dbus, and so as to GetCurrentPropertyValue etc.
							AutomationPropertyChanged (handler.HandlerId,
							                           e.EventId.Id, wrapper.Path,
							                           eventPropId, e.OldValue, e.NewValue);
							break;
						}
					}
				}
			}
		}

		internal void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			IRawElementProviderSimple simpleProvider = provider as IRawElementProviderSimple;
			if (simpleProvider == null)
				return;
			foreach (AutomationEventHandlerData handler in structureEventHandlers) {
				//Accroding to MSDN (StructureChangedEventHandler Delegate),
				//as for ChildRemoved and ChildrenBulkRemoved, the sender of the event handler is
				//"the parent of the child that was removed".
				if (e.StructureChangeType == StructureChangeType.ChildRemoved ||
				    e.StructureChangeType == StructureChangeType.ChildrenBulkRemoved) {
					IRawElementProviderFragment fragmentProvider = simpleProvider
						as IRawElementProviderFragment;
					if (fragmentProvider != null)
						fragmentProvider = fragmentProvider.Navigate (NavigateDirection.Parent);
					if (fragmentProvider == null)
						throw new Exception ("provider cannot be null");
					simpleProvider = fragmentProvider;
				}
				if (IsProviderInScope (simpleProvider, handler.Provider, handler.Scope)) {
					var wrapper = AutomationBridge.Instance.FindWrapperByPovider (simpleProvider);
					if (wrapper == null)
						throw new Exception ("Inconsistent provider -> wrapper mapping state");
					StructureChanged (handler.HandlerId, handler.EventId, wrapper.Path,
					                  e.StructureChangeType);
				}
			}
		}

		//Check whether target is in the scope defined by <element, scope>
		private static bool IsProviderInScope (IRawElementProviderSimple target,
		                                       IRawElementProviderSimple element,
		                                       TreeScope scope)
		{
			if ((scope & TreeScope.Element) == TreeScope.Element && target == element)
				return true;

			IRawElementProviderFragment targetFragment = target as IRawElementProviderFragment;
			IRawElementProviderFragment elementFragment = element as IRawElementProviderFragment;
			if (targetFragment == null || elementFragment == null)
				return false;

			IRawElementProviderFragment targetFragmentRoot =
				targetFragment.Navigate (NavigateDirection.Parent);
			if ((scope & TreeScope.Children) == TreeScope.Children &&
			    targetFragmentRoot != null &&
			    targetFragmentRoot == elementFragment)
				return true;
			if ((scope & TreeScope.Descendants) == TreeScope.Descendants) {
				while (targetFragmentRoot != null) {
					if (targetFragmentRoot == elementFragment)
						return true;
					targetFragmentRoot = targetFragmentRoot.Navigate (NavigateDirection.Parent);
				}
			}

			IRawElementProviderFragment elementFragmentRoot =
				elementFragment.Navigate (NavigateDirection.Parent);
			if ((scope & TreeScope.Parent) == TreeScope.Parent &&
			    elementFragmentRoot != null &&
			    elementFragmentRoot == targetFragment)
				return true;
			if ((scope & TreeScope.Ancestors) == TreeScope.Ancestors) {
				while (elementFragmentRoot != null) {
					if (elementFragmentRoot == targetFragment)
						return true;
					elementFragmentRoot = elementFragmentRoot.Navigate (NavigateDirection.Parent);
				}
			}

			return false;
		}

		private static void RemoveProviderFromHandlers<T> (List<T> handlerList,
		                                                   IRawElementProviderSimple provider)
			where T : AutomationEventHandlerData
		{
			List<T> handlersToDelete = new List<T> ();
			foreach (var handler in handlerList) {
				if (handler.Provider == provider)
					handlersToDelete.Add (handler);
			}
			foreach (var handler in handlersToDelete)
				handlerList.Remove (handler);
		}

		private static void RemoveEventHandlersByMask<T> (int handlerIdMask,
		                                                  IList<T> handlerList)
			where T : AutomationEventHandlerData
		{
			List<T> handlersToDelete = new List<T> ();
			foreach (var h in handlerList) {
				if ((h.HandlerId & handlerIdMask) == handlerIdMask) {
					handlersToDelete.Add (h);
				}
			}
			foreach (var h in handlersToDelete) {
				handlerList.Remove (h);
			}
		}

#region IApplication Members

		public string [] GetRootElementPaths ()
		{
			string [] paths = new string [rootElements.Count];
			for (int i = 0; i < paths.Length; i++)
				paths [i] = rootElements [i].Path;
			return paths;
		}

		public void AddAutomationEventHandler (int eventId, int [] elementRuntimeId,
		                                TreeScope scope, int handlerId)
		{
			var provider = AutomationBridge.Instance.FindProviderByRuntimeId (elementRuntimeId);
			if (provider == null)
				return;
			AutomationEventHandlerData handlerData =
				new AutomationEventHandlerData (eventId, provider, scope, handlerId);
			automationEventHandlers.Add (handlerData);
		}

		public void AddAutomationPropertyChangedEventHandler
			(int [] elementRuntimeId, TreeScope scope, int handlerId, int [] properties)
		{
			var provider = AutomationBridge.Instance.FindProviderByRuntimeId (elementRuntimeId);
			if (provider == null)
				return;
			AutomationPropertyChangedHandlerData handlerData =
				new AutomationPropertyChangedHandlerData (provider, scope,
					handlerId, properties);
			propertyEventHandlers.Add (handlerData);
		}

		public void AddStructureChangedEventHandler (int [] elementRuntimeId,
		                                             TreeScope scope, int handlerId)
		{
			var provider = AutomationBridge.Instance.FindProviderByRuntimeId (elementRuntimeId);
			if (provider == null)
				return;
			AutomationEventHandlerData handlerData =
				new AutomationEventHandlerData (
					AutomationElementIdentifiers.StructureChangedEvent.Id,
					provider, scope, handlerId);
			structureEventHandlers.Add (handlerData);
		}

		public void RemoveAutomationEventHandler (int eventId, int [] elementRuntimeId, int handlerId)
		{
			var provider = AutomationBridge.Instance.FindProviderByRuntimeId (elementRuntimeId);
			if (provider == null)
				return;
			List<AutomationEventHandlerData> handlersToDelete = new List<AutomationEventHandlerData> ();
			foreach (var handlerData in automationEventHandlers) {
				if (handlerData.EventId == eventId && handlerData.Provider == provider &&
				    handlerData.HandlerId == handlerId) {
					//As tested against Windows UIA, RemoveAutomationEventHandler will
					//remove all the (but not one) matched event handlers.
					handlersToDelete.Add (handlerData);
				}
			}
			foreach (var h in handlersToDelete)
				//todo Is RemoveAutomationEventHandler required to be thread-safe?
				//We shall lock automationEventHandlers if so.
				automationEventHandlers.Remove (h);
		}

		public void RemoveAutomationPropertyChangedEventHandler (int [] elementRuntimeId, int handlerId)
		{
			var provider = AutomationBridge.Instance.FindProviderByRuntimeId (elementRuntimeId);
			if (provider == null)
				return;
			List<AutomationPropertyChangedHandlerData> handlersToDelete =
				new List<AutomationPropertyChangedHandlerData> ();
			foreach (var handlerData in propertyEventHandlers) {
				if (handlerData.Provider == provider && handlerData.HandlerId == handlerId) {
					handlersToDelete.Add (handlerData);
				}
			}
			foreach (var h in handlersToDelete)
				propertyEventHandlers.Remove (h);
		}

		public void RemoveStructureChangedEventHandler (int [] elementRuntimeId, int handlerId)
		{
			var provider = AutomationBridge.Instance.FindProviderByRuntimeId (elementRuntimeId);
			if (provider == null)
				return;
			List<AutomationEventHandlerData> handlersToDelete = new List<AutomationEventHandlerData> ();
			foreach (var handlerData in structureEventHandlers) {
				if (handlerData.Provider == provider && handlerData.HandlerId == handlerId) {
					handlersToDelete.Add (handlerData);
				}
			}
			foreach (var h in handlersToDelete)
				structureEventHandlers.Remove (h);
		}

		public void RemoveAllEventHandlers (int handlerIdMask)
		{
			RemoveEventHandlersByMask (handlerIdMask, automationEventHandlers);
			RemoveEventHandlersByMask (handlerIdMask, propertyEventHandlers);
			RemoveEventHandlersByMask (handlerIdMask, structureEventHandlers);
		}

		public event Mono.UIAutomation.UiaDbus.Interfaces.AutomationEventHandler
			AutomationEvent;
		public event Mono.UIAutomation.UiaDbus.Interfaces.AutomationPropertyChangedHandler
			AutomationPropertyChanged;
		public event Mono.UIAutomation.UiaDbus.Interfaces.StructureChangedHandler
			StructureChanged;

#endregion
	}

	internal class AutomationEventHandlerData
	{
		public AutomationEventHandlerData (int eventId, IRawElementProviderSimple provider,
		                                   TreeScope scope, int handlerId)
		{
			this.EventId = eventId;
			this.Provider = provider;
			this.Scope = scope;
			this.HandlerId = handlerId;
		}

		public int EventId { get; private set; }
		public IRawElementProviderSimple Provider { get; private set; }
		public TreeScope Scope { get; private set; }
		public int HandlerId { get; private set; }
	}

	internal class AutomationPropertyChangedHandlerData : AutomationEventHandlerData
	{
		public AutomationPropertyChangedHandlerData (IRawElementProviderSimple provider,
		                                            TreeScope scope, int handlerId,
		                                            int [] properties)
			: base (AutomationElementIdentifiers.AutomationPropertyChangedEvent.Id,
			        provider, scope, handlerId)
		{
			this.Properties = properties;
		}

		public int [] Properties { get; private set; }
	}
}
