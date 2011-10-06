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
using Mono.UIAutomation.Services;
using Atspi;

namespace AtspiUiaSource
{
	public class AutomationSource : IAutomationSource
	{
		private static List<AutomationEventHandlerData> automationEventHandlers;
		private static List<PropertyChangedEventHandlerData> propertyEventHandlers;
		private static List<StructureChangedEventHandlerData> structureEventHandlers;
		private static List<FocusChangedEventHandler> focusChangedHandlers;
		private static Accessible focusedAccessible = null;
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
			focusChangedHandlers = new List<FocusChangedEventHandler> ();
			Desktop.DescriptionChanged += OnDescriptionChanged;
			Desktop.NameChanged += OnNameChanged;
			Desktop.StateChanged += OnStateChanged;
			Desktop.ChildAdded += OnChildAdded;
			Desktop.ChildRemoved += OnChildRemoved;
			string [] args = new string [0];
			Gdk.Global.InitCheck (ref args);
		}

		public IElement [] GetRootElements ()
		{
			List<Accessible> elements = new List<Accessible> ();
			foreach (Accessible element in Desktop.Instance.Children)
				foreach (Accessible child in element.Children)
					if (Element.GetElement (child).Parent == null)
						elements.Add (child);
			IElement [] ret = new IElement [elements.Count];
			int i = 0;
			foreach (Accessible accessible in elements)
				ret [i++] = Element.GetElement (accessible);
			Log.Debug ("AtspiUiaSource: GetRootElements count will be: " + ret.Length);
			return ret;
		}

		public event EventHandler RootElementsChanged;

		// This is only here to implement AutomationElementFromPoint;
		// it doesn't really work.
		public IElement GetElementFromHandle (IntPtr handle)
		{
			var win = Gdk.Window.ForeignNew ((uint) handle);
			if (win == null)
				return null;
			int x, y, width, height, depth;
			win.GetGeometry (out x, out y, out width, out height, out depth);
			win.GetOrigin (out x, out y);
			foreach (IElement element in GetRootElements ()) {
				Component component = ((Element)element).Accessible.QueryComponent ();
				if (component != null) {
					BoundingBox extents = component.GetExtents (CoordType.Screen);
					if (SizeFits (extents, x, y, width, height)) {
						Accessible ret = component.GetAccessibleAtPoint (x, y, CoordType.Screen);
						if (ret == null)
							return element;
						return Element.GetElement (ret);
					}
				}
			}
			return null;
		}

		private bool SizeFits (BoundingBox extents, int x, int y, int width, int height)
		{
			// This hack is to distinguish the desktop from other
			// windows. We should find a better way to do this and
			// undo this fudging, especially if Nautilus starts to
			// return something other than Window for the layer.
			return ((extents.X <= x && (x - extents.X) < 50) &&
				(extents.Y <= y && (y - extents.Y) < 50) &&
				(extents.Width >= width && (extents.Width - width) < 50) &&
				(extents.Height >= height && (extents.Height - height) < 50));
		}

		public IElement GetFocusedElement ()
		{
			if (focusedAccessible == null)
				focusedAccessible = FindFocusedAccessible (Desktop.Instance);
			return Element.GetElement (focusedAccessible);
		}

		private Accessible FindFocusedAccessible (Accessible root)
		{
			if (root.StateSet.Contains (StateType.Focused))
				return root;
			if (root.StateSet.Contains (StateType.ManagesDescendants))
				return null;
			int count = root.Children.Count;
			for (int i = 0; i < count; i++) {
				Accessible focus = FindFocusedAccessible (root.Children [i]);
				if (focus != null)
					return focus;
			}
			return null;
		}

		// The below code stolen from UiaAtkBridge
		private bool IsAccessibilityEnabledGConf ()
		{
			// FIXME: This is a temporary hack, we will replace it, proposed solutions:
			// - Use GConf API (we will need to fix threading issues).
			// - <Insert your fantastic idea here>
			string output = bool.FalseString;
			bool enabled = false;
				
			ProcessStartInfo
				processInfo = new ProcessStartInfo ("gconftool-2",
					                            "-g /desktop/gnome/interface/accessibility");
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
		
		private bool IsAccessibilityEnabledDBus ()
		{
			// FIXME: This is a temporary hack, we will replace it, proposed solutions:
			// - Use dbus-sharp, once we are using it as a
			// stand-alone package rather than copying it into
			// each module we use.
			// Alternatively, we could use C glue with gdbus,
			// but this would up the glib dependency to 2.26
			// unless we dlopen.
			string output = bool.FalseString;
				
			ProcessStartInfo
				processInfo = new ProcessStartInfo ("dbus-send",
					                            "--print-reply --type=method_call --dest=org.a11y.Bus /org/a11y/bus org.freedesktop.DBus.Properties.Get string:org.a11y.Status string:IsEnabled");
			processInfo.UseShellExecute = false;
			processInfo.ErrorDialog = false;
			processInfo.CreateNoWindow = true;
			processInfo.RedirectStandardOutput = true;
			processInfo.RedirectStandardError = true;

			// If this throws an exception, then just let the
			// caller catch it and try the old API
			Process dbus_send = Process.Start (processInfo);
			output = dbus_send.StandardOutput.ReadToEnd () ?? bool.FalseString;
			dbus_send.WaitForExit ();
			dbus_send.Close ();

			if (output.Contains ("true"))
				return true;
			if (output.Contains ("false"))
				return false;
			throw new NotSupportedException ();
		}
		
		public bool IsAccessibilityEnabled{
			get {
				try {
					return IsAccessibilityEnabledDBus ();
				} catch {
					return IsAccessibilityEnabledGConf ();
				}
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
			focusChangedHandlers.Add (eventHandler);
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
			focusChangedHandlers.Remove (eventHandler);
		}

		public void RemoveAllEventHandlers ()
		{
			automationEventHandlers.Clear ();
			propertyEventHandlers.Clear ();
			structureEventHandlers.Clear ();
			focusChangedHandlers.Clear ();
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

		internal static void RaiseFocusChangedEvent (Accessible accessible)
		{
			IElement element = Element.GetElement (accessible);
			IElement parent = Element.GetElement (accessible.Parent);
			RaiseFocusChangedEvent (parent, element);
		}

		internal static void RaiseFocusChangedEvent (IElement parent, IElement child)
		{
			foreach (FocusChangedEventHandler handler in focusChangedHandlers)
				handler (child,
					Int32.Parse (parent.AutomationId),
					Int32.Parse (child.AutomationId));
		}


		//Check whether target is in the scope defined by <element, scope>
		private static bool IsElementInScope (IElement target,
		                                       IElement element,
		                                       TreeScope scope)
		{
			IElement e;

			if (target == null)
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
				if (e == null && element == null)
					return true;
			}

			if (element == null)
				return false;
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
			case StateType.Armed:
				if (set)
					RaiseAutomationEvent (sender, InvokePattern.InvokedEvent);
				break;
			case StateType.Checked:
				RaisePropertyChangedEvent (sender, TogglePatternIdentifiers.ToggleStateProperty, !set, set);
				break;
			case StateType.Enabled:
				RaisePropertyChangedEvent (sender, AutomationElementIdentifiers.IsEnabledProperty, !set, set);
				break;
			case StateType.Expandable:
				RaisePropertyChangedEvent (sender, AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty, !set, set);
				break;
			case StateType.Expanded:
				RaisePropertyChangedEvent (sender, ExpandCollapsePattern.ExpandCollapseStateProperty,
					(set ? ExpandCollapseState.Collapsed : ExpandCollapseState.Expanded),
					(set ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed));
				break;
			case StateType.Focusable:
				RaisePropertyChangedEvent (sender, AutomationElementIdentifiers.IsKeyboardFocusableProperty, !set, set);
				break;
			case StateType.Focused:
				RaisePropertyChangedEvent (sender, AutomationElementIdentifiers.HasKeyboardFocusProperty, !set, set);
				if (set) {
					RaiseFocusChangedEvent (sender);
					focusedAccessible = sender;
				}
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
			if (sender.Role == Role.DesktopFrame) {
				foreach (Accessible frame in child.Children)
					OnChildAdded (child, frame);
				return;
			}

			IElement childElement = Element.GetElement (child, true);
			if (childElement == null)
				return;
			if (childElement.Parent != null)
				RaiseStructureChangedEvent (childElement.Parent, StructureChangeType.ChildrenInvalidated);
			if (child.Role != Role.Application)
				RaiseStructureChangedEvent (childElement, StructureChangeType.ChildAdded);
			if (child.Role == Role.Frame)
				RaiseAutomationEvent (child, WindowPattern.WindowOpenedEvent);
			if (sender.Role == Role.Application)
				OnRootElementsChanged ();
		}

		private void OnChildRemoved (Accessible sender, Accessible child)
		{
			Element childElement = Element.GetElement (child, false);
			if (childElement == null)
				return;
			Element parentElement;
			parentElement = childElement.Parent as Element;
			if (parentElement != null) {
				if (parentElement.extraChildren.IndexOf (childElement) != -1)
					parentElement.extraChildren.Remove (childElement);
				RaiseStructureChangedEvent (parentElement, StructureChangeType.ChildrenInvalidated);
				RaiseStructureChangedEvent (parentElement, StructureChangeType.ChildRemoved);
			}
			if (sender == Desktop.Instance || sender.Role == Role.Application)
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
