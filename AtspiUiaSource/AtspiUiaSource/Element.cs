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
using Mono.UIAutomation.Source;
using Atspi;

namespace AtspiUiaSource
{
	public class Element : IElement
	{
		private Accessible accessible;
		private int runtimeId;

		private static int id;
		private static Dictionary<Accessible, Element> elements;

		public Element (Accessible accessible)
		{
			this.accessible = accessible;
			runtimeId = -1;
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
						return ControlType.Table;
					case Role.TableCell:
						return (accessible.Parent != null && accessible.Parent.Role == Role.Table? ControlType.DataItem: ControlType.TreeItem);
					case Role.Text:
						return ControlType.Edit;
					case Role.ToolBar:
						return ControlType.ToolBar;
					case Role.ToolTip:
						return ControlType.ToolTip;
					case Role.Tree:
					case Role.TreeTable:
						// TODO: perhaps return List sometimes
						return ControlType.Tree;
					default:
						Console.WriteLine ("AtSpiUIASource: Unknown role: " + accessible.Role);
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
				return accessible.StateSet.ContainsState (StateType.Focused);
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
				return accessible.StateSet.ContainsState (StateType.Enabled);
			}
		}

		public virtual bool IsKeyboardFocusable {
			get {
				return accessible.StateSet.ContainsState (StateType.Focusable);
			}
		}

		public virtual bool IsOffscreen {
			get {
				// TODO: Figure out if it's scrolled into view
				return !(accessible.StateSet.ContainsState (StateType.Showing));
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
				if (accessible.StateSet.ContainsState (StateType.Vertical))
					return (accessible.Role == Role.SplitPane? OrientationType.Horizontal: OrientationType.Vertical);
				if (accessible.StateSet.ContainsState (StateType.Horizontal))
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
			if (elements == null)
				elements = new Dictionary<Accessible, Element> ();
			if (!elements.ContainsKey (accessible))
				elements [accessible] = new Element (accessible);
			return elements [accessible];
		}
	}
}
