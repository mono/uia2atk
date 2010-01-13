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
using Mono.UIAutomation.Services;

namespace Mono.UIAutomation.UiaDbusBridge
{
	internal class Application : IApplication
	{
		private List<ProviderElementWrapper> rootElements;
		private List<AutomationEventHandlerData> automationEventHandlers;
		private List<AutomationPropertyChangedHandlerData> propertyEventHandlers;
		private List<AutomationEventHandlerData> structureEventHandlers;
		private string focusedElementPath = string.Empty;

		public Application ()
		{
			rootElements = new List<ProviderElementWrapper> ();
			automationEventHandlers = new List<AutomationEventHandlerData> ();
			propertyEventHandlers = new List<AutomationPropertyChangedHandlerData> ();
			structureEventHandlers = new List<AutomationEventHandlerData> ();
		}

		public void AddRootElement (ProviderElementWrapper element)
		{
			bool elementChanged = false;
			lock (rootElements) {
				if (element != null && !rootElements.Contains (element)) {
					rootElements.Add (element);
					elementChanged = true;
				}
			}
			if (elementChanged)
				OnRootElementsChanged ();
		}

		public void RemoveRootElement (ProviderElementWrapper element)
		{
			bool elementChanged = false;
			lock (rootElements) {
				if (element != null && rootElements.Contains (element)) {
					rootElements.Remove (element);
					elementChanged = true;
				}
			}
			if (elementChanged)
				OnRootElementsChanged ();
		}

		internal void RemoveProvider (IRawElementProviderSimple provider)
		{
			lock (automationEventHandlers)
				RemoveHandlers (automationEventHandlers,
				                h => h.Provider == provider);
			lock (propertyEventHandlers)
				RemoveHandlers (propertyEventHandlers,
				                h => h.Provider == provider);
			lock (structureEventHandlers)
				RemoveHandlers (structureEventHandlers,
				                h => h.Provider == provider);
		}

		internal void RaiseAutomationEvent (AutomationEvent eventId, object provider, AutomationEventArgs e)
		{
			IRawElementProviderSimple simpleProvider = provider as IRawElementProviderSimple;
			if (simpleProvider == null)
				return;
			var wrapper = AutomationBridge.Instance.FindWrapperByProvider (simpleProvider);
			if (wrapper == null) {
				Log.Error ("[UiaDbusBridge.RaiseAutomationEvent] Inconsistent provider -> wrapper mapping state");
				return;
			}
			lock (automationEventHandlers) {
				foreach (AutomationEventHandlerData handler in automationEventHandlers) {
					if (handler.EventId == eventId.Id &&
						IsProviderInScope (simpleProvider, handler.Provider, handler.Scope)) {
						OnAutomationEvent (handler.HandlerId, handler.EventId, wrapper.Path);
					}
				}
			}
			if (eventId == AutomationElementIdentifiers.AutomationFocusChangedEvent) {
				var valueObj = simpleProvider.GetPropertyValue (
					AutomationElementIdentifiers.HasKeyboardFocusProperty.Id);
				bool hasFocus = false;
				try { hasFocus = (bool)valueObj; } catch {}
				if (hasFocus && focusedElementPath != wrapper.Path) {
					focusedElementPath = wrapper.Path;
					OnFocusChanged (focusedElementPath);
				} else if (!hasFocus && focusedElementPath == wrapper.Path) {
					focusedElementPath = string.Empty;
					OnFocusChanged (focusedElementPath);
				}
			}
		}

		internal void RaiseAutomationPropertyChangedEvent (object provider, AutomationPropertyChangedEventArgs e)
		{
			IRawElementProviderSimple simpleProvider = provider as IRawElementProviderSimple;
			if (simpleProvider == null)
				return;
			var wrapper = AutomationBridge.Instance.FindWrapperByProvider (simpleProvider);
			if (wrapper == null) {
				Log.Error ("[UiaDbusBridge.RaiseAutomationPropertyChangedEvent] Inconsistent provider -> wrapper mapping state");
				return;
			}
			lock (propertyEventHandlers) {
				foreach (AutomationPropertyChangedHandlerData handler in propertyEventHandlers) {
					if (IsProviderInScope (simpleProvider, handler.Provider, handler.Scope)) {
						int eventPropId = e.Property.Id;
						foreach (int propId in handler.Properties) {
							if (eventPropId == propId) {
								//todo Need to add a general class to serialize/deserialize OldValue/Newvalue
								//over dbus, and so as to GetCurrentPropertyValue etc.
								OnAutomationPropertyChanged (handler.HandlerId,
								                             e.EventId.Id, wrapper.Path,
								                             eventPropId, e.OldValue, e.NewValue);
								break;
							}
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
			var wrapper = AutomationBridge.Instance.FindWrapperByProvider (simpleProvider);
			if (wrapper == null) {
				Log.Error ("[UiaDbusBridge.RaiseStructureChangedEvent] Inconsistent provider -> wrapper mapping state");
				return;
			}
			lock (structureEventHandlers) {
				foreach (AutomationEventHandlerData handler in structureEventHandlers) {
					if (IsProviderInScope (simpleProvider, handler.Provider, handler.Scope)) {
						OnStructureChanged (handler.HandlerId, handler.EventId, wrapper.Path,
						                    e.StructureChangeType);
					}
				}
			}
		}

		private void OnAutomationEvent (int handlerId, int eventId, string providerPath)
		{
			if (AutomationEvent != null)
				AutomationEvent (handlerId, eventId, providerPath);
		}

		private void OnAutomationPropertyChanged (int handlerId, int eventId,
		                                          string providerPath, int propertyId,
		                                          object oldValue, object newValue)
		{
			if (AutomationPropertyChanged != null)
				AutomationPropertyChanged (handlerId, eventId, providerPath,
				                           propertyId, oldValue, newValue);
		}

		private void OnStructureChanged (int handlerId, int eventId, string providerPath, StructureChangeType changeType)
		{
			if (StructureChanged != null)
				StructureChanged (handlerId, eventId, providerPath, changeType);
		}

		private void OnFocusChanged (string providerPath)
		{
			if (FocusChanged != null)
				FocusChanged (providerPath);
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

		private static void RemoveHandlers<T> (List<T> handlerList, Predicate<T> pred)
			where T : AutomationEventHandlerData
		{
			List<T> handlersToDelete = new List<T> ();
			foreach (T handler in handlerList) {
				if (pred (handler))
					handlersToDelete.Add (handler);
			}
			foreach (T handler in handlersToDelete)
				handlerList.Remove (handler);
		}

		private void OnRootElementsChanged ()
		{
			if (RootElementsChanged != null)
				RootElementsChanged ();
		}

#region IApplication Members

		public string [] GetRootElementPaths ()
		{
			string [] paths = null;
			lock (rootElements) {
				paths = new string [rootElements.Count];
				for (int i = 0; i < paths.Length; i++)
					paths [i] = rootElements [i].Path;
			}
			return paths;
		}

		public string GetElementPathFromHandle (int handle)
		{
			var element = AutomationBridge.Instance.FindWrapperByHandle (handle);
			if (element != null)
				return element.Path;
			return string.Empty;
		}

		public string GetFocusedElementPath ()
		{
			return AutomationBridge.Instance.GetFocusedElementPath ();
		}

		public void AddAutomationEventHandler (int eventId, int [] elementRuntimeId,
		                                TreeScope scope, int handlerId)
		{
			var provider = AutomationBridge.Instance.FindProviderByRuntimeId (elementRuntimeId);
			if (provider == null)
				return;
			AutomationEventHandlerData handlerData =
				new AutomationEventHandlerData (eventId, provider, scope, handlerId);
			lock (automationEventHandlers)
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
			lock (propertyEventHandlers)
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
			lock (structureEventHandlers)
				structureEventHandlers.Add (handlerData);
		}

		public void RemoveAutomationEventHandler (int eventId, int [] elementRuntimeId, int handlerId)
		{
			var provider = AutomationBridge.Instance.FindProviderByRuntimeId (elementRuntimeId);
			if (provider == null)
				return;
			lock (automationEventHandlers)
				RemoveHandlers (automationEventHandlers,
					h => h.EventId == eventId &&
					h.Provider == provider &&
					h.HandlerId == handlerId);
		}

		public void RemoveAutomationPropertyChangedEventHandler (int [] elementRuntimeId, int handlerId)
		{
			var provider = AutomationBridge.Instance.FindProviderByRuntimeId (elementRuntimeId);
			if (provider == null)
				return;
			lock (propertyEventHandlers)
				RemoveHandlers (propertyEventHandlers,
					h => h.Provider == provider && h.HandlerId == handlerId);
		}

		public void RemoveStructureChangedEventHandler (int [] elementRuntimeId, int handlerId)
		{
			var provider = AutomationBridge.Instance.FindProviderByRuntimeId (elementRuntimeId);
			if (provider == null)
				return;
			lock (structureEventHandlers)
				RemoveHandlers (structureEventHandlers,
					h => h.Provider == provider && h.HandlerId == handlerId);
		}

		public void RemoveAllEventHandlers (int handlerIdMask)
		{
			lock (automationEventHandlers)
				RemoveHandlers (automationEventHandlers,
				               h => (h.HandlerId & handlerIdMask) == handlerIdMask);
			lock (propertyEventHandlers)
				RemoveHandlers (propertyEventHandlers,
				               h => (h.HandlerId & handlerIdMask) == handlerIdMask);
			lock (structureEventHandlers)
				RemoveHandlers (structureEventHandlers,
				               h => (h.HandlerId & handlerIdMask) == handlerIdMask);
		}

		public event Mono.UIAutomation.UiaDbus.Interfaces.AutomationEventHandler
			AutomationEvent;
		public event Mono.UIAutomation.UiaDbus.Interfaces.AutomationPropertyChangedHandler
			AutomationPropertyChanged;
		public event Mono.UIAutomation.UiaDbus.Interfaces.StructureChangedHandler
			StructureChanged;
		public event VoidHandler RootElementsChanged;
		public event Mono.UIAutomation.UiaDbus.Interfaces.FocusChangedHandler FocusChanged;
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
