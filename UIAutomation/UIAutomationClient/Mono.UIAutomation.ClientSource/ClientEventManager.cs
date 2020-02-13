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
//     Matt Guo <matt@mattguo.com>
// 

using System;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Source;

namespace Mono.UIAutomation.ClientSource
{
	internal static class ClientEventManager
	{
		private static List<PropertyChangedEventEntry> propertyChangedEventEntries
			= new List<PropertyChangedEventEntry> ();
		private static List<AutomationEventEntry> automationEventEntries
			= new List<AutomationEventEntry> ();
		private static List<StructureChangedEventEntry> structureChangedEventEntries
			= new List<StructureChangedEventEntry> ();

		public static void AddAutomationEventHandler (AutomationEvent eventId,
			IRawElementProviderSimple provider, TreeScope scope,
			AutomationEventHandler eventHandler)
		{
			var entry = new AutomationEventEntry (eventId, provider, scope, eventHandler);
			lock (automationEventEntries)
				automationEventEntries.Add (entry);
		}

		public static void AddAutomationPropertyChangedEventHandler (
			IRawElementProviderSimple provider, TreeScope scope,
			AutomationPropertyChangedEventHandler eventHandler,
			int [] properties)
		{
			var entry = new PropertyChangedEventEntry (provider, scope, properties, eventHandler);
			lock (propertyChangedEventEntries)
				propertyChangedEventEntries.Add (entry);
		}

		public static void AddStructureChangedEventHandler (
			IRawElementProviderSimple provider, TreeScope scope,
			StructureChangedEventHandler eventHandler)
		{
			var entry = new StructureChangedEventEntry (provider, scope, eventHandler);
			lock (structureChangedEventEntries)
				structureChangedEventEntries.Add (entry);
		}

		public static void RemoveAutomationEventHandler (AutomationEvent eventId, IRawElementProviderSimple provider, AutomationEventHandler eventHandler)
		{
			lock (automationEventEntries)
				automationEventEntries.RemoveAll (e =>
					e.EventId == eventId &&
					e.Provider == provider &&
					e.Handler == eventHandler);
		}

		public static void RemoveAutomationPropertyChangedEventHandler (IRawElementProviderSimple provider, AutomationPropertyChangedEventHandler eventHandler)
		{
			lock (propertyChangedEventEntries)
				propertyChangedEventEntries.RemoveAll (e =>
					e.Provider == provider &&
					e.Handler == eventHandler);
		}

		public static void RemoveStructureChangedEventHandler (IRawElementProviderSimple provider, StructureChangedEventHandler eventHandler)
		{
			lock (structureChangedEventEntries)
				structureChangedEventEntries.RemoveAll (e =>
					e.Provider == provider &&
					e.Handler == eventHandler);
		}

		public static void RemoveAllEventHandlers ()
		{
			lock (automationEventEntries)
				automationEventEntries.Clear ();
			lock (structureChangedEventEntries)
				structureChangedEventEntries.Clear ();
			lock (propertyChangedEventEntries)
				propertyChangedEventEntries.Clear ();
		}

		public static void RaiseAutomationEvent (IRawElementProviderSimple provider, AutomationEventArgs e)
		{
			lock (automationEventEntries)
				foreach (var entry in automationEventEntries)
					if (entry.EventId == e.EventId &&
						IsProviderInScope (provider, entry.Provider, entry.Scope)) {
						var clientElement =
							ClientAutomationSource.Instance.GetOrCreateElement (provider);
						var element = SourceManager.GetOrCreateAutomationElement (clientElement);
						entry.Handler (element, e);
					}
		}

		public static void RaiseAutomationPropertyChangedEvent (IRawElementProviderSimple provider,
			AutomationPropertyChangedEventArgs e)
		{
			lock (propertyChangedEventEntries)
				foreach (var entry in propertyChangedEventEntries)
					if (IsProviderInScope (provider, entry.Provider, entry.Scope) &&
						Array.Exists (entry.Properties, i => i == e.Property.Id)) {
						var clientElement =
							ClientAutomationSource.Instance.GetOrCreateElement (provider);
						var element = SourceManager.GetOrCreateAutomationElement (clientElement);
						// TODO implement
						// Translate e.NewValue && e.OldValue
						entry.Handler (element, e);
					}
		}

		public static void RaiseStructureChangedEvent (IRawElementProviderSimple provider,
			StructureChangedEventArgs e)
		{
			lock (structureChangedEventEntries)
				foreach (var entry in structureChangedEventEntries)
					if (IsProviderInScope (provider, entry.Provider, entry.Scope)) {
						var clientElement =
							ClientAutomationSource.Instance.GetOrCreateElement (provider);
						var element = SourceManager.GetOrCreateAutomationElement (clientElement);
						entry.Handler (element, e);
					}
		}

		private static bool IsProviderInScope (IRawElementProviderSimple target,
			IRawElementProviderSimple element, TreeScope scope)
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
#region Internal Classes

		private class AutomationEventEntryBase
		{
			public AutomationEventEntryBase (IRawElementProviderSimple provider,
			                                 TreeScope scope)
			{
				this.Provider = provider;
				this.Scope = scope;
			}

			public IRawElementProviderSimple Provider { get; private set; }
			public TreeScope Scope { get; private set; }
		}

		private class StructureChangedEventEntry : AutomationEventEntryBase
		{
			public StructureChangedEventEntry (IRawElementProviderSimple provider,
			                                   TreeScope scope,
			                                   StructureChangedEventHandler handler)
				: base (provider, scope)
			{
				this.Handler = handler;
			}
			public StructureChangedEventHandler Handler { get; private set; }
		}

		private class AutomationEventEntry : AutomationEventEntryBase
		{
			public AutomationEventEntry (AutomationEvent eventId,
			                             IRawElementProviderSimple provider,
			                             TreeScope scope,
			                             AutomationEventHandler handler)
				: base (provider, scope)
			{
				this.EventId = eventId;
				this.Handler = handler;
			}

			public AutomationEvent EventId { get; private set; }
			public AutomationEventHandler Handler { get; private set; }
		}

		private class PropertyChangedEventEntry : AutomationEventEntryBase
		{
			public PropertyChangedEventEntry (IRawElementProviderSimple provider,
			                                  TreeScope scope,
			                                  int [] properties,
			                                  AutomationPropertyChangedEventHandler handler)
				: base (provider, scope)
			{
				this.Properties = properties;
				this.Handler = handler;
			}

			public int [] Properties { get; private set; }
			public AutomationPropertyChangedEventHandler Handler { get; private set; }
		}
#endregion
	}
}
