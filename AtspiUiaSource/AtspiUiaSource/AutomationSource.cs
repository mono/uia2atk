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
//      Mike Gorse <mgorse@novell.com>
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Automation;
using Mono.UIAutomation.Source;
using Atspi;

namespace AtspiUiaSource
{
	public class AutomationSource : IAutomationSource
	{
		private static List<AutomationEventHandlerData> automationEventHandlers;
		private static List<PropertyChangedEventHandlerData> propertyEventHandlers;
		private static List<StructureChangedEventHandlerData> structureEventHandlers;
		private static AutomationSource instance = null;

		public void Initialize ()
		{
			if (automationEventHandlers != null)
				return;
			instance = this;
			Registry.Initialize (true);
			automationEventHandlers = new List<AutomationEventHandlerData> ();
			propertyEventHandlers = new List<PropertyChangedEventHandlerData> ();
			structureEventHandlers = new List<StructureChangedEventHandlerData> ();
			Desktop.DescriptionChanged += OnDescriptionChanged;
			Desktop.NameChanged += OnNameChanged;
			Desktop.StateChanged += OnStateChanged;
			Desktop.ChildAdded += OnChildAdded;
			Desktop.ChildRemoved += OnChildRemoved;
		}

		public IElement [] GetRootElements ()
		{
			List<Accessible> elements = new List<Accessible> ();
			foreach (Accessible element in Desktop.Instance.Children)
				foreach (Accessible child in element.Children)
					elements.Add (child);
			IElement [] ret = new IElement [elements.Count];
			int i = 0;
			foreach (Accessible accessible in elements)
				ret [i++] = Element.GetElement (accessible);
			return ret;
		}

		public event EventHandler RootElementsChanged;

		public IElement GetElementFromHandle (IntPtr handle)
		{
			// TODO: Implement (how best?)
			return null;
		}

		public IElement GetFocusedElement ()
		{
			//TODO Added by Matt Guo, 2009/11/24, due to the change of IAutomationSource
			throw new NotImplementedException ();
		}

		// The below code stolen from UiaAtkBridge
 		public bool IsAccessibilityEnabled {
 			get {
				// FIXME: This is a temporary hack, we will replace it, proposed solutions:
				// - Use GConf API (we will need to fix threading issues).
				// - <Insert your fantastic idea here>
				string output = bool.FalseString;
				bool enabled = false;
				
				ProcessStartInfo
					processInfo = new ProcessStartInfo ("gconftool-2", "-g /desktop/gnome/interface/accessibility");
				processInfo.UseShellExecute = false;
				processInfo.ErrorDialog = false;
				processInfo.CreateNoWindow = true;
				processInfo.RedirectStandardOutput = true;
				
				try {
					Process gconftool2 = Process.Start (processInfo);
					output = gconftool2.StandardOutput.ReadToEnd () ?? bool.FalseString;
					gconftool2.WaitForExit ();
					gconftool2.Close ();
				} catch (System.IO.FileNotFoundException) {}

				try {
					enabled = bool.Parse (output);
				} catch (FormatException) {}
				
				return enabled;
 			}
 		}

		public void AddAutomationEventHandler (AutomationEvent eventId,
		                                       IElement element,
		                                       TreeScope scope,
		                                       AutomationEventHandler eventHandler)
		{
			AutomationEventHandlerData data = new AutomationEventHandlerData (
				eventId, element, scope, eventHandler);
			automationEventHandlers.Add (data);
		}

		public void AddAutomationPropertyChangedEventHandler (IElement element,
		                                                      TreeScope scope,
		                                                      AutomationPropertyChangedEventHandler eventHandler,
		                                                      AutomationProperty [] properties)
		{
			PropertyChangedEventHandlerData data = new PropertyChangedEventHandlerData (
				element, scope, eventHandler, properties);
			propertyEventHandlers.Add (data);
		}

		public void AddStructureChangedEventHandler (IElement element,
		                                             TreeScope scope,
		                                             StructureChangedEventHandler eventHandler)
		{
			StructureChangedEventHandlerData data = new StructureChangedEventHandlerData (
				element, scope, eventHandler);
			structureEventHandlers.Add (data);
		}

		public void AddAutomationFocusChangedEventHandler (FocusChangedEventHandler eventHandler)
		{
			//TODO Added by Matt Guo, 2009/11/24, due to the change of IAutomationSource
			throw new NotImplementedException ();
		}

		public void RemoveAutomationEventHandler (AutomationEvent eventId,
		                                          IElement element,
		                                          AutomationEventHandler eventHandler)
		{
			List<AutomationEventHandlerData> handlersToDelete = new List<AutomationEventHandlerData> ();
			foreach (var handlerData in automationEventHandlers) {
				if (handlerData.Element == element && handlerData.EventHandler == eventHandler && handlerData.EventId == eventId) {
					handlersToDelete.Add (handlerData);
				}
			}
			foreach (var h in handlersToDelete)
				automationEventHandlers.Remove (h);
		}

		public void RemoveAutomationPropertyChangedEventHandler (IElement element,
		                                                         AutomationPropertyChangedEventHandler eventHandler)
		{
			List<PropertyChangedEventHandlerData> handlersToDelete = new List<PropertyChangedEventHandlerData> ();
			foreach (var handlerData in propertyEventHandlers) {
				if (handlerData.Element == element && handlerData.EventHandler == eventHandler) {
					handlersToDelete.Add (handlerData);
				}
			}
			foreach (var h in handlersToDelete)
				propertyEventHandlers.Remove (h);
		}

		public void RemoveStructureChangedEventHandler (IElement element,
		                                                StructureChangedEventHandler eventHandler)
		{
			List<StructureChangedEventHandlerData> handlersToDelete = new List<StructureChangedEventHandlerData> ();
			foreach (var handlerData in structureEventHandlers) {
				if (handlerData.Element == element && handlerData.EventHandler == eventHandler) {
					handlersToDelete.Add (handlerData);
				}
			}
			foreach (var h in handlersToDelete)
				structureEventHandlers.Remove (h);
		}

		public void RemoveAutomationFocusChangedEventHandler (FocusChangedEventHandler eventHandler)
		{
			//TODO Added by Matt Guo, 2009/11/24, due to the change of IAutomationSource
			throw new NotImplementedException ();
		}

		public void RemoveAllEventHandlers ()
		{
			automationEventHandlers.Clear ();
			propertyEventHandlers.Clear ();
			structureEventHandlers.Clear ();
		}

		internal static AutomationSource Instance {
			get { return instance; }
		}

		internal static void RaiseAutomationEvent (Accessible accessible, AutomationEvent eventId)
		{
			IElement element = Element.GetElement (accessible);
			RaiseAutomationEvent (element, eventId);
		}

		internal static void RaiseAutomationEvent (IElement element, AutomationEvent eventId)
		{
			AutomationEventArgs e = new AutomationEventArgs (eventId);
			RaiseAutomationEvent (element, e);
		}

		internal static void RaiseAutomationEvent (IElement element,
				AutomationEventArgs e)
		{
			foreach (AutomationEventHandlerData handler in automationEventHandlers)
				if (IsElementInScope (element, handler.Element, handler.Scope) &&
					handler.EventId == e.EventId)
							handler.EventHandler (SourceManager.GetOrCreateAutomationElement (element), e);
		}

		internal static void RaisePropertyChangedEvent (Accessible accessible, AutomationProperty property, object oldValue, object newValue)
		{
			IElement element = Element.GetElement (accessible);
			RaisePropertyChangedEvent (element, property, oldValue, newValue);
		}

		internal static void RaisePropertyChangedEvent (IElement element, AutomationProperty property, object oldValue, object newValue)
		{
			AutomationPropertyChangedEventArgs e;
			e = new AutomationPropertyChangedEventArgs (property,
				oldValue,
				newValue);
			RaisePropertyChangedEvent (element, e);
		}

		internal static void RaisePropertyChangedEvent (IElement element,
				AutomationPropertyChangedEventArgs e)
		{
			// To always use public API
			// - AutomationElement[]
			if (e.Property.Id == TableItemPatternIdentifiers.ColumnHeaderItemsProperty.Id
			    || e.Property.Id == TableItemPatternIdentifiers.RowHeaderItemsProperty.Id
			    || e.Property.Id == TablePatternIdentifiers.ColumnHeadersProperty.Id
			    || e.Property.Id == TablePatternIdentifiers.RowHeadersProperty.Id
			    || e.Property.Id == SelectionPatternIdentifiers.SelectionProperty.Id) {
				IElement[] oldIElements = e.OldValue as IElement[];
				IElement[] newIElements = e.NewValue as IElement[];

				e = new AutomationPropertyChangedEventArgs (e.Property,
				                                            SourceManager.GetOrCreateAutomationElements (oldIElements),
									    SourceManager.GetOrCreateAutomationElements (newIElements));
			// - AutomationElement
			} else if (e.Property.Id == TableItemPatternIdentifiers.ColumnHeaderItemsProperty.Id
			           || e.Property.Id == GridItemPatternIdentifiers.ContainingGridProperty.Id
				   || e.Property.Id == SelectionItemPatternIdentifiers.SelectionContainerProperty.Id) {
				IElement oldElement = e.OldValue as IElement;
				IElement newElement = e.NewValue as IElement;

				e = new AutomationPropertyChangedEventArgs (e.Property,
				                                            SourceManager.GetOrCreateAutomationElement (oldElement),
									    SourceManager.GetOrCreateAutomationElement (newElement));
			}

			foreach (PropertyChangedEventHandlerData handler in propertyEventHandlers) {
				if (IsElementInScope (element, handler.Element, handler.Scope)) {
					foreach (AutomationProperty property in handler.Properties) {
						if (property == e.Property) {
							handler.EventHandler (SourceManager.GetOrCreateAutomationElement (element),
								e);
							break;
						}
					}
				}
			}
		}

		internal static void RaiseStructureChangedEvent (Accessible accessible, StructureChangeType type)
		{
			IElement element = Element.GetElement (accessible);
			RaiseStructureChangedEvent (element, type);
		}

		internal static void RaiseStructureChangedEvent (IElement element, StructureChangeType type)
		{
			StructureChangedEventArgs e;
			int [] runtimeId = (element != null
				? element.RuntimeId
				: new int [0]);
			e = new StructureChangedEventArgs (type, runtimeId);
			foreach (StructureChangedEventHandlerData handler in structureEventHandlers) {
				if (IsElementInScope (element, handler.Element, handler.Scope)) {
					handler.EventHandler (SourceManager.GetOrCreateAutomationElement (element),
						e);
					break;
				}
			}
		}

		//Check whether target is in the scope defined by <element, scope>
		private static bool IsElementInScope (IElement target,
		                                       IElement element,
		                                       TreeScope scope)
		{
			IElement e;

			// TODO: Handle the case of a child being added to the top-level element
			if (element == null || target == null)
				return false;

			if ((scope & TreeScope.Element) == TreeScope.Element && target == element)
				return true;

			if ((scope & TreeScope.Children) == TreeScope.Children &&
			    target.Parent == element)
				return true;

			if ((scope & TreeScope.Descendants) == TreeScope.Descendants) {
				e = target.Parent;
				while (e != null) {
					if (e == element)
						return true;
					e = e.Parent;
				}
			}

			e = element.Parent;
			if ((scope & TreeScope.Parent) == TreeScope.Parent &&
			    e != null &&
			    e == target)
				return true;
			if ((scope & TreeScope.Ancestors) == TreeScope.Ancestors) {
				while (e != null) {
					if (e == target)
						return true;
					e = e.Parent;
				}
			}

			return false;
		}

		private void OnDescriptionChanged (object sender, string oldDescription, string newDescription)
		{
			Accessible accessible = sender as Accessible;
			RaisePropertyChangedEvent (accessible, AutomationElementIdentifiers.HelpTextProperty, oldDescription, newDescription);
		}

		private void OnNameChanged (object sender, string oldName, string newName)
		{
			Accessible accessible = sender as Accessible;
			RaisePropertyChangedEvent (accessible, AutomationElementIdentifiers.NameProperty, oldName, newName);
		}

		private void OnStateChanged (Accessible sender, StateType state, bool set)
		{
			switch (state) {
			case StateType.Checked:
				RaisePropertyChangedEvent (sender, TogglePatternIdentifiers.ToggleStateProperty, !set, set);
				break;
			case StateType.Enabled:
				RaisePropertyChangedEvent (sender, AutomationElementIdentifiers.IsEnabledProperty, !set, set);
				break;
			case StateType.Focusable:
				RaisePropertyChangedEvent (sender, AutomationElementIdentifiers.IsKeyboardFocusableProperty, !set, set);
				break;
			case StateType.Focused:
				RaisePropertyChangedEvent (sender, AutomationElementIdentifiers.HasKeyboardFocusProperty, !set, set);
				break;
			case StateType.Selected:
				RaisePropertyChangedEvent (sender, SelectionItemPatternIdentifiers.IsSelectedProperty, !set, set);
				if (set)
					RaiseAutomationEvent (sender, SelectionItemPatternIdentifiers.ElementSelectedEvent);
				break;
			default:
				break;
			}
		}

		private void OnChildAdded (Accessible sender, Accessible child)
		{
			RaiseStructureChangedEvent (sender, StructureChangeType.ChildrenInvalidated);
			RaiseStructureChangedEvent (child, StructureChangeType.ChildAdded);
			if (sender == Desktop.Instance)
				OnRootElementsChanged ();
		}

		private void OnChildRemoved (Accessible sender, Accessible child)
		{
			RaiseStructureChangedEvent (sender, StructureChangeType.ChildrenInvalidated);
			RaiseStructureChangedEvent (sender, StructureChangeType.ChildRemoved);
			if (sender == Desktop.Instance)
				OnRootElementsChanged ();
		}

		private void OnRootElementsChanged ()
		{
			if (RootElementsChanged != null)
				RootElementsChanged (this, EventArgs.Empty);
		}
	}


	internal class AutomationEventHandlerData
	{
		internal IElement Element { get; private set; }
		internal TreeScope Scope { get; private set; }
		internal AutomationEventHandler EventHandler { get; private set; }
		internal AutomationEvent EventId { get; private set; }

		internal AutomationEventHandlerData (AutomationEvent eventId,
			IElement element,
			TreeScope scope,
			AutomationEventHandler eventHandler)
		{
			this.EventId = eventId;
			this.Element = element;
			this.Scope = scope;
			this.EventHandler = eventHandler;
		}
	}

	internal class PropertyChangedEventHandlerData
	{
		internal IElement Element { get; private set; }
		internal TreeScope Scope { get; private set; }
		internal AutomationPropertyChangedEventHandler EventHandler { get; private set; }
		internal AutomationProperty [] Properties { get; private set; }

		internal PropertyChangedEventHandlerData (IElement element,
			TreeScope scope,
			AutomationPropertyChangedEventHandler eventHandler,
			AutomationProperty [] properties)
		{
			this.Element = element;
			this.Scope = scope;
			this.EventHandler = eventHandler;
			this.Properties = properties;
		}
	}

	internal class StructureChangedEventHandlerData
	{
		internal IElement Element { get; private set; }
		internal TreeScope Scope { get; private set; }
		internal StructureChangedEventHandler EventHandler { get; private set; }

		internal StructureChangedEventHandlerData (IElement element,
			TreeScope scope,
			StructureChangedEventHandler eventHandler)
		{
			this.Element = element;
			this.Scope = scope;
			this.EventHandler = eventHandler;
		}
	}
}
