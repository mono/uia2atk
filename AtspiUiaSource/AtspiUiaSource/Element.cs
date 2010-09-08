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
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using Mono.Unix;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.Source;
using Atspi;

namespace AtspiUiaSource
{
	public class Element : IElement
	{
		internal Accessible accessible;
		private List<ISourceEventHandler> patterns;
		private int runtimeId;
		private Element parent;
		internal List<Element> extraChildren;

		private static int id;
		private static Dictionary<Accessible, Element> elements = new Dictionary<Accessible, Element> ();

		private static AutomationProperty [] allProperties = {
			AutomationElementIdentifiers.IsControlElementProperty,
			AutomationElementIdentifiers.ControlTypeProperty,
			AutomationElementIdentifiers.IsContentElementProperty,
			AutomationElementIdentifiers.LabeledByProperty,
			AutomationElementIdentifiers.NativeWindowHandleProperty,
			AutomationElementIdentifiers.AutomationIdProperty,
			AutomationElementIdentifiers.ItemTypeProperty,
			AutomationElementIdentifiers.IsPasswordProperty,
			AutomationElementIdentifiers.LocalizedControlTypeProperty,
			AutomationElementIdentifiers.NameProperty,
			AutomationElementIdentifiers.AcceleratorKeyProperty,
			AutomationElementIdentifiers.AccessKeyProperty,
			AutomationElementIdentifiers.HasKeyboardFocusProperty,
			AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			AutomationElementIdentifiers.IsEnabledProperty,
			AutomationElementIdentifiers.BoundingRectangleProperty,
			AutomationElementIdentifiers.ProcessIdProperty,
			AutomationElementIdentifiers.RuntimeIdProperty,
			AutomationElementIdentifiers.ClassNameProperty,
			AutomationElementIdentifiers.HelpTextProperty,
			AutomationElementIdentifiers.ClickablePointProperty,
			AutomationElementIdentifiers.CultureProperty,
			AutomationElementIdentifiers.IsOffscreenProperty,
			AutomationElementIdentifiers.OrientationProperty,
			AutomationElementIdentifiers.FrameworkIdProperty,
			AutomationElementIdentifiers.IsRequiredForFormProperty,
			AutomationElementIdentifiers.ItemStatusProperty,
			// Comment Is*PatternAvailableProperty since MS.Net never include those 
			// properties in the return value of AutomationElement.GetSupportedProperties ()
			//AutomationElementIdentifiers.IsDockPatternAvailableProperty,
			//AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty,
			//AutomationElementIdentifiers.IsGridItemPatternAvailableProperty,
			//AutomationElementIdentifiers.IsGridPatternAvailableProperty,
			//AutomationElementIdentifiers.IsInvokePatternAvailableProperty,
			//AutomationElementIdentifiers.IsMultipleViewPatternAvailableProperty,
			//AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty,
			//AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty,
			//AutomationElementIdentifiers.IsSelectionPatternAvailableProperty,
			//AutomationElementIdentifiers.IsScrollPatternAvailableProperty,
			//AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty,
			//AutomationElementIdentifiers.IsTablePatternAvailableProperty,
			//AutomationElementIdentifiers.IsTableItemPatternAvailableProperty,
			//AutomationElementIdentifiers.IsTextPatternAvailableProperty,
			//AutomationElementIdentifiers.IsTogglePatternAvailableProperty,
			//AutomationElementIdentifiers.IsTransformPatternAvailableProperty,
			//AutomationElementIdentifiers.IsValuePatternAvailableProperty,
			//AutomationElementIdentifiers.IsWindowPatternAvailableProperty,
			ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			GridItemPatternIdentifiers.RowProperty,
			GridItemPatternIdentifiers.ColumnProperty,
			GridItemPatternIdentifiers.RowSpanProperty,
			GridItemPatternIdentifiers.ColumnSpanProperty,
			GridItemPatternIdentifiers.ContainingGridProperty,
			GridPatternIdentifiers.RowCountProperty,
			GridPatternIdentifiers.ColumnCountProperty,
			MultipleViewPatternIdentifiers.CurrentViewProperty,
			MultipleViewPatternIdentifiers.SupportedViewsProperty,
			RangeValuePatternIdentifiers.ValueProperty,
			RangeValuePatternIdentifiers.IsReadOnlyProperty,
			RangeValuePatternIdentifiers.MinimumProperty,
			RangeValuePatternIdentifiers.MaximumProperty,
			RangeValuePatternIdentifiers.LargeChangeProperty,
			RangeValuePatternIdentifiers.SmallChangeProperty,
			ScrollPatternIdentifiers.HorizontalScrollPercentProperty,
			ScrollPatternIdentifiers.HorizontalViewSizeProperty,
			ScrollPatternIdentifiers.VerticalScrollPercentProperty,
			ScrollPatternIdentifiers.VerticalViewSizeProperty,
			ScrollPatternIdentifiers.HorizontallyScrollableProperty,
			ScrollPatternIdentifiers.VerticallyScrollableProperty,
			SelectionItemPatternIdentifiers.IsSelectedProperty,
			SelectionItemPatternIdentifiers.SelectionContainerProperty,
			SelectionPatternIdentifiers.SelectionProperty,
			SelectionPatternIdentifiers.CanSelectMultipleProperty,
			SelectionPatternIdentifiers.IsSelectionRequiredProperty,
			TablePatternIdentifiers.RowHeadersProperty,
			TablePatternIdentifiers.ColumnHeadersProperty,
			TablePatternIdentifiers.RowOrColumnMajorProperty,
			TogglePatternIdentifiers.ToggleStateProperty,
			TransformPatternIdentifiers.CanMoveProperty,
			TransformPatternIdentifiers.CanResizeProperty,
			TransformPatternIdentifiers.CanRotateProperty,
			ValuePatternIdentifiers.ValueProperty,
			ValuePatternIdentifiers.IsReadOnlyProperty,
			WindowPatternIdentifiers.CanMaximizeProperty,
			WindowPatternIdentifiers.CanMinimizeProperty,
			WindowPatternIdentifiers.IsModalProperty,
			WindowPatternIdentifiers.WindowVisualStateProperty,
			WindowPatternIdentifiers.WindowInteractionStateProperty,
			WindowPatternIdentifiers.IsTopmostProperty,
			DockPatternIdentifiers.DockPositionProperty,
			TableItemPatternIdentifiers.RowHeaderItemsProperty,
			TableItemPatternIdentifiers.ColumnHeaderItemsProperty
		};

		public Element (Accessible accessible)
		{
			extraChildren = new List<Element> ();
			if (!elements.ContainsKey (accessible))
				elements [accessible] = this;
			patterns = new List<ISourceEventHandler> ();
			this.accessible = accessible;
			runtimeId = -1;

			if (accessible.Role == Role.Dialog &&
				accessible.Parent.Role == Role.Application &&
				accessible.QueryComponent () != null) {
				// Try to figure out if the dialog is painted
				// on top of another window
				BoundingBox curExtents = accessible.QueryComponent ().GetExtents (CoordType.Screen);
				int count = accessible.Parent.Children.Count;
				for (int i = 0; i < count; i++) {
					Accessible child = accessible.Parent.Children [i];
					if (child == null ||
						child.Role != Role.Frame)
						continue;
					Component childComponent = child.QueryComponent ();
					if (childComponent == null)
						continue;
					BoundingBox windowExtents = childComponent.GetExtents (CoordType.Screen);
					if (windowExtents.X <= curExtents.X &&
						windowExtents.Y <= curExtents.Y &&
						(windowExtents.X + windowExtents.Width) > curExtents.X &&
						(windowExtents.Y + windowExtents.Height) > curExtents.Y) {
						parent = GetElement (child);
						parent.extraChildren.Add (this);
						break;
					}
				}
			}
			if (parent == null)
				parent = GetElement (accessible.Parent);
			AddEvents (true);
		}

		~Element ()
		{
			RemoveEvents ();
		}

		protected virtual void AddEvents (bool fromElementConstructor)
		{
			// TODO: Only listen for events where we have
			// PropertyChange listeners for them
			if (accessible.ObjectEvents != null)
				accessible.ObjectEvents.BoundsChanged += OnBoundsChanged;
			if (SupportsGrid ())
				patterns.Add (new GridEventHandler (this));
			if (SupportsRangeValue ())
				patterns.Add (new RangeValueEventHandler (this));
			if (SupportsSelection ())
				patterns.Add (new SelectionEventHandler (this, new SelectionSource (this)));
			if (SupportsText ())
				patterns.Add (new TextEventHandler (this));
			if (SupportsValue ())
				patterns.Add (new ValueEventHandler (this));
		}

		private void RemoveEvents ()
		{
			foreach (ISourceEventHandler pattern in patterns)
				pattern.Terminate ();
			if (accessible.ObjectEvents != null)
				accessible.ObjectEvents.BoundsChanged -= OnBoundsChanged;
		}

		public virtual bool SupportsProperty (AutomationProperty property)
		{
			if ((property.Id == AutomationElementIdentifiers.IsControlElementProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.ControlTypeProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.IsContentElementProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.LabeledByProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.AutomationIdProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.IsPasswordProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.NameProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.AcceleratorKeyProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.AccessKeyProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.IsEnabledProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.BoundingRectangleProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.ProcessIdProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.RuntimeIdProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.HelpTextProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.ClickablePointProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.IsOffscreenProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.OrientationProperty.Id) ||
				(property.Id == AutomationElementIdentifiers.FrameworkIdProperty.Id))
				return true;
			if (property.Id == ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id)
				return SupportsExpandCollapse ();
			if ((property.Id == GridItemPatternIdentifiers.RowProperty.Id) ||
				(property.Id == GridItemPatternIdentifiers.ColumnProperty.Id) ||
				(property.Id == GridItemPatternIdentifiers.RowSpanProperty.Id) ||
				(property.Id == GridItemPatternIdentifiers.ColumnSpanProperty.Id) ||
				(property.Id == GridItemPatternIdentifiers.ContainingGridProperty.Id))
				return SupportsGridItem ();
			if ((property.Id == GridPatternIdentifiers.RowCountProperty.Id) ||
				(property.Id == GridPatternIdentifiers.ColumnCountProperty.Id))
				return SupportsGrid ();
			if ((property.Id == RangeValuePatternIdentifiers.ValueProperty.Id) ||
				(property.Id == RangeValuePatternIdentifiers.IsReadOnlyProperty.Id) ||
				(property.Id == RangeValuePatternIdentifiers.MinimumProperty.Id) ||
				(property.Id == RangeValuePatternIdentifiers.MaximumProperty.Id) ||
				(property.Id == RangeValuePatternIdentifiers.SmallChangeProperty.Id))
				return SupportsRangeValue ();
			if ((property.Id == SelectionItemPatternIdentifiers.IsSelectedProperty.Id) ||
				(property.Id == SelectionItemPatternIdentifiers.SelectionContainerProperty.Id))
				return SupportsSelectionItem ();
			if ((property.Id == SelectionPatternIdentifiers.SelectionProperty.Id) ||
				(property.Id == SelectionPatternIdentifiers.CanSelectMultipleProperty.Id) ||
				(property.Id == SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id))
				return SupportsSelection ();
			if ((property.Id == TablePatternIdentifiers.RowHeadersProperty.Id) ||
				(property.Id == TablePatternIdentifiers.ColumnHeadersProperty.Id))
				return SupportsTable ();
			if (property.Id == TogglePatternIdentifiers.ToggleStateProperty.Id)
				return SupportsToggle ();
			if ((property.Id == ValuePatternIdentifiers.ValueProperty.Id) ||
				(property.Id == ValuePatternIdentifiers.IsReadOnlyProperty.Id))
				return SupportsValue ();
			if ((property.Id == TableItemPatternIdentifiers.RowHeaderItemsProperty.Id) ||
				(property.Id == TableItemPatternIdentifiers.ColumnHeaderItemsProperty.Id))
				return SupportsTableItem ();
			return false;
		}

		public virtual string AcceleratorKey {
			get {
				Atspi.Action action = accessible.QueryAction ();
				if (action != null)
					return action.GetKeyBinding (0);
				return null;
			}
		}

		public virtual string AccessKey {
			get {
				return AcceleratorKey;
			}
		}

		public virtual string AutomationId {
			get {
				if (runtimeId == -1)
					runtimeId = GetUniqueRuntimeId ();
				return runtimeId.ToString ();
			}
		}

		public virtual Rect BoundingRectangle {
			get {
				Component component = accessible.QueryComponent ();
				if (component != null)
					return BoundingBoxToRect (component.GetExtents (CoordType.Screen));
				return Rect.Empty;
			}
		}

		public virtual string ClassName {
			get {
				// TODO: Is this right?
				return "Mono.UIAutomation.Atspi.Element";
			}
		}

		public virtual Point ClickablePoint {
			get {
				Rect rect = BoundingRectangle;
				return new Point (rect.X, rect.Y);
			}
		}

		public virtual ControlType ControlType {
			get {
				switch (accessible.Role) {
					case Role.Calendar:
						return ControlType.Calendar;
					case Role.CheckBox:
						return ControlType.CheckBox;
					case Role.CheckMenuItem:
						return ControlType.MenuItem;
					case Role.ComboBox:
						return ControlType.ComboBox;
					case Role.Dialog:
						return ControlType.Window;
					case Role.DocumentFrame:
						return ControlType.Document;
					case Role.Filler:
						return ControlType.Group;
					case Role.Frame:
						return ControlType.Window;
					case Role.Image:
						return ControlType.Image;
					case Role.Label:
						return ControlType.Text;
					case Role.Link:
						return ControlType.Hyperlink;
					case Role.List:
						return ControlType.List;
					case Role.ListItem:
						return ControlType.ListItem;
					case Role.Menu:
						return ControlType.MenuItem;
					case Role.MenuBar:
						return ControlType.MenuBar;
					case Role.MenuItem:
						return ControlType.MenuItem;
					case Role.PageTab:
						return ControlType.TabItem;
					case Role.PageTabList:
						return ControlType.Tab;
					case Role.Panel:
						return ControlType.Pane;
					case Role.PasswordText:
						return ControlType.Edit;
					case Role.ProgressBar:
						return ControlType.ProgressBar;
					case Role.PushButton:
						return ControlType.Button;
					case Role.RadioButton:
						return ControlType.RadioButton;
					case Role.ScrollBar:
						return ControlType.ScrollBar;
					case Role.Separator:
						return ControlType.Separator;
					case Role.Slider:
						return ControlType.Slider;
					case Role.SpinButton:
						return ControlType.Spinner;
					case Role.SplitPane:
						return ControlType.Pane;
					case Role.Statusbar:
						return ControlType.StatusBar;
					case Role.Table:
						return (FirstChild is TableHeaderElement ?
							ControlType.Table :
							ControlType.DataGrid);
					case Role.TableColumnHeader:
						return ControlType.HeaderItem;
					case Role.TableRowHeader:
						return ControlType.HeaderItem;
					case Role.Text:
						return (accessible.StateSet.Contains (StateType.MultiLine)? ControlType.Document: ControlType.Edit);
					case Role.ToggleButton:
						return ControlType.Button;
					case Role.ToolBar:
						return ControlType.ToolBar;
					case Role.ToolTip:
						return ControlType.ToolTip;
					case Role.Window:
						return ControlType.Window;
					default:
						Log.Debug ("AtSpiUIASource: Unknown role: " + accessible.Role);
						return ControlType.Custom;
				}
			}
		}

		public virtual string FrameworkId {
			get {
				// TODO: Need to bind org.freedesktop.atspi.Application,
				// return GTK+ if we see GAIL
				return "at-spi";
			}
		}

		public virtual bool HasKeyboardFocus {
			get {
				return accessible.StateSet.Contains (StateType.Focused);
			}
		}

		public virtual string HelpText {
			get {
				return accessible.Description;
			}
		}

		public virtual bool IsContentElement {
			get {
				// TODO
				return true;
			}
		}

		public virtual bool IsControlElement {
			get {
				// TODO
				return true;
			}
		}

		public virtual bool IsEnabled {
			get {
				return accessible.StateSet.Contains (StateType.Enabled);
			}
		}

		public virtual bool IsKeyboardFocusable {
			get {
				return accessible.StateSet.Contains (StateType.Focusable);
			}
		}

		public virtual bool IsOffscreen {
			get {
				// TODO: Figure out if it's scrolled into view
				return !(accessible.StateSet.Contains (StateType.Showing));
			}
		}

		public virtual bool IsPassword {
			get {
				return accessible.Role == Role.PasswordText;
			}
		}

		public virtual bool IsRequiredForForm {
			get {
				// TODO: Can we support this?
				return false;
			}
		}

		public virtual string ItemStatus {
			get {
				// TODO
				return string.Empty;
			}
		}

		public virtual string ItemType {
			get {
				// TODO
				return string.Empty;
			}
		}

		public virtual IElement LabeledBy {
			get {
				Relation rel = GetRelation (RelationType.LabelledBy);
				if (rel == null)
					return null;
				return GetElement (rel.Targets [0]);
			}
		}

		public virtual string LocalizedControlType {
			get {
				return ControlType.LocalizedControlType;
			}
		}

		public virtual string Name {
			get {
				return accessible.Name;
			}
		}

		public virtual int NativeWindowHandle {
			get {
				Rect rect = BoundingRectangle;
				// Is this 64-bit compatible?
				return (int) NativeMethods.WindowAtPosition ((int) rect.X, (int) rect.Y);
			}
		}

		public virtual OrientationType Orientation {
			get {
				if (accessible.StateSet.Contains (StateType.Vertical))
					return (accessible.Role == Role.SplitPane? OrientationType.Horizontal: OrientationType.Vertical);
				if (accessible.StateSet.Contains (StateType.Horizontal))
					return (accessible.Role == Role.SplitPane? OrientationType.Vertical: OrientationType.Horizontal);
				return OrientationType.None;
			}
		}

		public virtual int ProcessId {
			get {
				return (int) accessible.Application.Pid;
			}
		}

		public virtual int [] RuntimeId {
			get {
				int [] ret = new int [1];
				if (runtimeId == -1)
					runtimeId = GetUniqueRuntimeId ();
				ret [0] = runtimeId;
				return ret;
			}
		}

		public virtual IElement Parent {
			get {
				return parent;
			}
		}

		public virtual IElement FirstChild {
			get {
				int count = accessible.Children.Count;
				for (int i = 0; i < count; i++) {
					Element e = GetElement (accessible.Children [i]);
					if (e.parent == this)
						return e;
				}
				if (extraChildren.Count > 0)
					return extraChildren [0];
				return null;
			}
		}

		public virtual IElement LastChild {
			get {
				if (extraChildren.Count > 0)
					return extraChildren.Last ();
				if (accessible.Children.Count == 0)
					return null;
				return GetElement (accessible.Children [accessible.Children.Count - 1]);
			}
		}

		public virtual IElement NextSibling {
			get {
				int index;
				index = parent.extraChildren.IndexOf (this);
				if (index >= 0 && index >= parent.extraChildren.Count - 1)
					return null;
				if (index >= 0)
					return parent.extraChildren [index + 1];
				index = accessible.IndexInParent;
				int childCount = accessible.Parent.Children.Count;
				if (index == childCount - 1) {
					if (parent.extraChildren.Count > 0)
						return parent.extraChildren [0];
					return null;
				}
				if (index < 0)
					return null;
				for (int i = index + 1; i < childCount; i++) {
					Element e = GetElement (accessible.Parent.Children [i]);
					if (e.parent == parent)
						return e;
				}
				if (parent.extraChildren.Count > 0)
					return parent.extraChildren [0];
				return null;
			}
		}

		public virtual IElement PreviousSibling {
			get {
				int index = -1;
				if (parent != null) {
					index = parent.extraChildren.IndexOf (this);
					if (index > 0)
						return parent.extraChildren [index - 1];
					else if (index == 0)
						index = parent.accessible.Children.Count;
				}
				if (index == -1)
					index = accessible.IndexInParent;
				while (--index >= 0) {
					Element child = GetElement (parent.accessible.Children [index]);
					if (child.Parent == parent)
						return child;
				}
				return null;
			}
		}

		public IAutomationSource AutomationSource {
			get { return AtspiUiaSource.AutomationSource.Instance; }
		}

		public object GetCurrentPattern (AutomationPattern pattern)
		{
			if (pattern == null)
				throw new InvalidOperationException ();
			object o = GetCurrentPatternInternal (pattern);
			if (o != null)
				return o;
			throw new InvalidOperationException ();
		}

		public IElement GetDescendantFromPoint (double x, double y)
		{
			Component component = accessible.QueryComponent ();
			if (component == null)
				return this;
			Accessible descendant = component.GetAccessibleAtPoint ((int)x, (int)y, CoordType.Screen);
			if (descendant == null)
				return this;
			return GetElement (descendant);
		}

		internal virtual object GetCurrentPatternInternal (AutomationPattern pattern)
		{
			if (pattern == ExpandCollapsePatternIdentifiers.Pattern)
				return (SupportsExpandCollapse () ? new ExpandCollapseSource (this) : null);
			else if (pattern == GridItemPatternIdentifiers.Pattern)
				return (SupportsGridItem () ? new GridItemSource (this) : null);
			else if (pattern == GridPatternIdentifiers.Pattern)
				return (SupportsGrid () ? new GridSource (this) : null);
			else if (pattern == InvokePatternIdentifiers.Pattern)
				return (SupportsInvoke () ? new InvokeSource (this) : null);
			else if (pattern == RangeValuePatternIdentifiers.Pattern)
				return (SupportsRangeValue () ? new RangeValueSource (this) : null);
			else if (pattern == SelectionItemPatternIdentifiers.Pattern)
				return (SupportsSelectionItem () ? new SelectionItemSource (this) : null);
			else if (pattern == SelectionPatternIdentifiers.Pattern)
				return (SupportsSelection () ? new SelectionSource (this) : null);
			else if (pattern == TableItemPatternIdentifiers.Pattern)
				return (SupportsTableItem () ? new TableItemSource (this) : null);
			else if (pattern == TablePatternIdentifiers.Pattern)
				return (SupportsTable () ? new TableSource (this) : null);
			else if (pattern == TextPatternIdentifiers.Pattern)
				return (SupportsText () ? new TextSource (this) : null);
			else if (pattern == TogglePatternIdentifiers.Pattern)
				return (SupportsToggle () ? new ToggleSource (this) : null);
			else if (pattern == ValuePatternIdentifiers.Pattern)
				return (SupportsValue () ? new ValueSource (this) : null);
			return null;
		}

		public AutomationPattern [] GetSupportedPatterns ()
		{
			List<AutomationPattern> patterns = new List<AutomationPattern> ();

			if (SupportsExpandCollapse ())
				patterns.Add (ExpandCollapsePatternIdentifiers.Pattern);

			if (SupportsGrid ())
				patterns.Add (GridPatternIdentifiers.Pattern);

			if (SupportsGridItem ())
				patterns.Add (GridItemPatternIdentifiers.Pattern);

			if (SupportsInvoke ())
				patterns.Add (InvokePatternIdentifiers.Pattern);

			if (SupportsRangeValue ())
				patterns.Add (RangeValuePatternIdentifiers.Pattern);

			if (SupportsSelection ())
				patterns.Add (SelectionPatternIdentifiers.Pattern);

			if (SupportsSelectionItem ())
				patterns.Add (SelectionItemPatternIdentifiers.Pattern);

			if (SupportsTable ())
				patterns.Add (TablePatternIdentifiers.Pattern);

			if (SupportsTableItem ())
				patterns.Add (TableItemPatternIdentifiers.Pattern);

			if (SupportsText ())
				patterns.Add (TextPatternIdentifiers.Pattern);

			if (SupportsToggle ())
				patterns.Add (TogglePatternIdentifiers.Pattern);

			if (SupportsValue ())
				patterns.Add (ValuePatternIdentifiers.Pattern);

			return patterns.ToArray ();
		}

		public AutomationProperty [] GetSupportedProperties ()
		{
				return allProperties
					.Where (property => SupportsProperty (property))
					.ToArray ();
		}

		public void SetFocus ()
		{
			Component component = accessible.QueryComponent ();
			if (component != null)
				component.GrabFocus ();
		}

		internal static int GetUniqueRuntimeId ()
		{
			return ++id;
		}

		internal Rect BoundingBoxToRect (BoundingBox box)
		{
			return new Rect (box.X, box.Y, box.Width, box.Height);
		}

		internal Relation GetRelation (RelationType type)
		{
			Relation [] relations = accessible.RelationSet;
			foreach (Relation relation in relations)
				if (relation.Type == type)
					return relation;
			return null;
		}

		internal static Element GetElement (Accessible accessible)
		{
			return GetElement (accessible, true);
		}

		internal static Element GetElement (Accessible accessible, bool create)
		{
			if (accessible == null)
				return null;
			// We expose the children of Applications as top-level,
			// to be more like UIA
			if (accessible.Role == Role.Application)
				return null;
			if (elements.ContainsKey (accessible))
			return elements [accessible];
			if (!create)
				return null;
			Element element;
			if (IsTable (accessible))
				element = new TableElement (accessible);
			else if (IsTableHeaderItem (accessible))
				element = new TableHeaderItemElement (accessible);
			else if (IsTreeItem (accessible))
				element = new TreeItemElement (accessible);
			else
				element = new Element (accessible);
			return element;
		}

		internal static Element GetElement (Accessible accessible, TableElement parent, int row)
		{
			if (accessible == null)
				return null;
			if (elements.ContainsKey (accessible))
				return elements [accessible];
			elements [accessible] = new TreeItemElement (accessible, parent, row);
			return elements [accessible];
		}

		internal static Element GetElement (Accessible accessible, DataItemElement parent, int column)
		{
			if (accessible == null)
				return null;
			if (elements.ContainsKey (accessible))
				return elements [accessible];
			elements [accessible] = new TableCellElement (accessible, parent, column);
			return elements [accessible];
		}

		private static bool IsTable (Accessible accessible)
		{
			if (accessible.Role == Role.Table)
				return true;
			if (accessible.Role != Role.TreeTable)
				return false;
			Atspi.Table table = accessible.QueryTable ();
			return (table != null);
		}

		private static bool IsTableHeaderItem (Accessible accessible)
		{
			return (accessible.Role == Role.TableRowHeader
				|| accessible.Role == Role.TableColumnHeader);
		}

		private static bool IsTreeItem (Accessible accessible)
		{
			if (accessible.Role != Role.TableCell)
				return false;
			Accessible parentAccessible = accessible.Parent;
			Element parent = GetElement (parentAccessible);
			return (parent is TreeItemElement ||
				parent.ControlType == ControlType.Tree);
		}

		internal Accessible Accessible {
			get {
				return accessible;
			}
		}

		internal virtual bool SupportsExpandCollapse ()
		{
			Accessible testAccessible;
			Atspi.Action action;
			if (this is DataItemElement) {
				Element child = FirstChild as Element;
				if (child == null)
					return false;
				testAccessible = child.accessible;
			} else
				testAccessible = accessible;
			action = testAccessible.QueryAction ();
			if (action == null)
				return false;
			ActionDescription [] actions = action.Actions;
			for (int i = 0; i < actions.Length; i++)
				if (actions [i].Name == "expand or contract")
					return testAccessible.StateSet.Contains (StateType.Expandable);
			return false;
		}

		internal virtual bool SupportsGridItem ()
		{
			return (accessible.Parent.QueryTable () != null &&
				ControlType != ControlType.TreeItem);
		}

		internal virtual bool SupportsGrid ()
		{
			return (accessible.QueryTable () != null &&
				ControlType != ControlType.Tree);
		}

		internal virtual bool SupportsInvoke ()
		{
			if (accessible.Role == Role.Separator ||
				accessible.Role == Role.CheckBox ||
				accessible.Role == Role.ScrollBar ||
				accessible.Role == Role.SpinButton ||
				accessible.Role == Role.Text ||
				accessible.Role == Role.ToggleButton)
				return false;
			Atspi.Action action = accessible.QueryAction ();
			if (action == null)
				return false;
			ActionDescription [] actions = action.Actions;
			for (int i = 0; i < actions.Length; i++)
				if (actions [i].Name == "activate" || actions [i].Name == "invoke"
				    || actions [i].Name == "click" || actions [i].Name == "jump")
					return true;
			return false;
		}

		internal virtual bool SupportsRangeValue ()
		{
			return (accessible.QueryValue () != null);
		}

		internal virtual bool SupportsSelectionItem ()
		{
			if (accessible.Role == Role.Separator)
				return false;
			return (accessible.Parent.QuerySelection () != null);
		}

		internal virtual bool SupportsSelection ()
		{
			// disabling for tables because of BNC#604660
			return (accessible.QuerySelection () != null &&
				ControlType != ControlType.Table &&
				accessible.Role != Role.MenuBar);
		}

		internal virtual bool SupportsTableItem ()
		{
			// TODO: Only enable if no headers?
			return SupportsGridItem ();
		}

		internal virtual bool SupportsTable ()
		{
			// TODO: Only enable if no headers?
			return SupportsGrid ();
		}

		internal virtual bool SupportsToggle ()
		{
			return (accessible.Role == Role.CheckBox || accessible.Role == Role.CheckMenuItem || accessible.Role == Role.ToggleButton);
		}

		internal virtual bool SupportsText ()
		{
			if (accessible.Role != Role.Text &&
				accessible.Role != Role.PasswordText)
				return false;
			return (accessible.QueryText () != null);
		}

		internal virtual bool SupportsValue ()
		{
			return (accessible.QueryEditableText () != null &&
				!accessible.StateSet.Contains (StateType.MultiLine));
		}

		private void OnBoundsChanged (Accessible sender, BoundingBox bounds)
		{
			Rect rect = BoundingBoxToRect (bounds);
			AtspiUiaSource.AutomationSource.RaisePropertyChangedEvent (this,
				AutomationElement.BoundingRectangleProperty,
				Rect.Empty,
				rect);
		}
	}
}
