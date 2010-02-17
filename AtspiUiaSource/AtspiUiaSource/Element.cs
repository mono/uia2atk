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

		private static int id;
		private static Dictionary<Accessible, Element> elements;

		public Element (Accessible accessible)
		{
			patterns = new List<ISourceEventHandler> ();
			this.accessible = accessible;
			runtimeId = -1;
			AddEvents ();
		}

		~Element ()
		{
			RemoveEvents ();
		}

		private void AddEvents ()
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
			// TODO
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
						return ControlType.Menu;
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
				return 0;	// not supported
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
				// can't do this with the current at-spi implementation.
				return 0;
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
				return GetElement (accessible.Parent);
			}
		}

		public virtual IElement FirstChild {
			get {
				if (accessible.Children.Count == 0)
					return null;
				return GetElement (accessible.Children [0]);
			}
		}

		public virtual IElement LastChild {
			get {
				if (accessible.Children.Count == 0)
					return null;
				return GetElement (accessible.Children [accessible.Children.Count - 1]);
			}
		}

		public virtual IElement NextSibling {
			get {
				int index = accessible.IndexInParent;
				if (index < 0 || accessible.Parent.Children.Count <= index + 1)
					return null;
				return GetElement (accessible.Parent.Children [index + 1]);
			}
		}

		public virtual IElement PreviousSibling {
			get {
				int index = accessible.IndexInParent;
				if (index <= 0)
					return null;
				return GetElement (accessible.Parent.Children [index - 1]);
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
			// TODO: to implement, return the descendant element at point (x,y);
			return this;
		}

		private object GetCurrentPatternInternal (AutomationPattern pattern)
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
			throw new NotImplementedException ();
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
			if (accessible == null)
				return null;
			// We expose the children of Applications as top-level,
			// to be more like UIA
			if (accessible.Role == Role.Application)
				return null;
			if (elements == null)
				elements = new Dictionary<Accessible, Element> ();
			if (elements.ContainsKey (accessible))
			return elements [accessible];
			Element element;
			if (IsTable (accessible))
				element = new TableElement (accessible);
			else if (IsTableHeaderItem (accessible))
				element = new TableHeaderItemElement (accessible);
			else
				element = new Element (accessible);
			elements [accessible] = element;
			return element;
		}

		internal static Element GetElement (Accessible accessible, TableElement parent, int row)
		{
			if (accessible == null)
				return null;
			if (elements == null)
				elements = new Dictionary<Accessible, Element> ();
			if (elements.ContainsKey (accessible))
				return elements [accessible];
			elements [accessible] = new TreeItemElement (accessible, parent, row);
			return elements [accessible];
		}

		internal static Element GetElement (Accessible accessible, DataItemElement parent, int column)
		{
			if (accessible == null)
				return null;
			if (elements == null)
				elements = new Dictionary<Accessible, Element> ();
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

		internal Accessible Accessible {
			get {
				return accessible;
			}
		}

		internal bool SupportsExpandCollapse ()
		{
			Atspi.Action action = accessible.QueryAction ();
			if (action == null)
				return false;
			ActionDescription [] actions = action.Actions;
			for (int i = 0; i < actions.Length; i++)
				if (actions [i].Name == "expand or contract")
					return accessible.StateSet.Contains (StateType.Expandable);
			return false;
		}

		internal bool SupportsGridItem ()
		{
			return (accessible.Parent.QueryTable () != null);
		}

		internal bool SupportsGrid ()
		{
			return (accessible.QueryTable () != null);
		}

		internal bool SupportsInvoke ()
		{
			Atspi.Action action = accessible.QueryAction ();
			if (action == null)
				return false;
			ActionDescription [] actions = action.Actions;
			for (int i = 0; i < actions.Length; i++)
				if (actions [i].Name == "activate" || actions [i].Name == "invoke" || actions [i].Name == "click")
					return true;
			return false;
		}

		internal bool SupportsRangeValue ()
		{
			return (accessible.QueryValue () != null);
		}

		internal bool SupportsSelectionItem ()
		{
			return (accessible.Parent.QuerySelection () != null);
		}

		internal bool SupportsSelection ()
		{
			return (accessible.QuerySelection () != null);
		}

		internal bool SupportsTableItem ()
		{
			// TODO: Only enable if no headers?
			return SupportsGridItem ();
		}

		internal bool SupportsTable ()
		{
			// TODO: Only enable if no headers?
			return SupportsGrid ();
		}

		internal bool SupportsToggle ()
		{
			return (accessible.Role == Role.CheckBox || accessible.Role == Role.ToggleButton);
		}

		internal bool SupportsText ()
		{
			return (accessible.QueryText () != null);
		}

		internal bool SupportsValue ()
		{
			// Perhaps we should only support this if MultiLine
			// is not set, but then the TextPattern test will
			// fail; might as well always enable it.
			return (accessible.QueryEditableText () != null);
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
