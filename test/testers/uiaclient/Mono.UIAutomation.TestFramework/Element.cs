// Element.cs: the base class of each control class wrapper.
//
// This program is free software; you can redistribute it and/or modify it under
// the terms of the GNU General Public License version 2 as published by the
// Free Software Foundation.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public License along with
// this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
//
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
//
// Authors:
//	Ray Wang  (rawang@novell.com)
//	Felicia Mu  (fxmu@novell.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using System.Threading;
using System.Reflection;

namespace Mono.UIAutomation.TestFramework
{
	// Basic instance which provides Find methods (e.g. FindButton, FindEdit..
	public class Element
	{
		protected AutomationElement element;
		protected ProcedureLogger procedureLogger = new ProcedureLogger ();

		public Element (AutomationElement element)
		{
			if (element == null)
				throw new ArgumentException("element cannot be null");
			this.element = element;
		}

#region Properties
		public string Name {
			get { return element.Current.Name; }
		}
		
		public string NameAndType {
			get { return UIAutomationMetadata.GetElementDesc(element); }
		}

		public AutomationElement AutomationElement {
			get { return element; }
		}

		public bool IsOffscreen {
			get { return element.Current.IsOffscreen; }
		}

		public Finder Finder {
			get { return new Finder (this.AutomationElement); }
		}
#endregion

#region Methods
		public T Find<T> () where T : Element
		{
			return Find<T> (string.Empty, string.Empty);
		}

		public T Find<T> (string name) where T : Element
		{
			return Find<T> (name, string.Empty);
		}

		public T Find<T> (string name, string automationId) where T : Element
		{
			var uiaType = typeof (T).GetField ("UIAType", BindingFlags.Static | BindingFlags.Public);
			ControlType type = uiaType.GetValue (null) as ControlType;
			return Find (type, name, automationId) as T;
		}

		protected Element Find (ControlType type, string name, string automationId)
		{
			Condition cond;

			if (automationId == string.Empty && name != string.Empty) {
				cond = new AndCondition (new PropertyCondition (AutomationElementIdentifiers.ControlTypeProperty, type),
					new PropertyCondition (AutomationElementIdentifiers.NameProperty, name));
			} else if (name == string.Empty && automationId != string.Empty) {
				cond = new AndCondition (new PropertyCondition (AutomationElementIdentifiers.ControlTypeProperty, type),
					new PropertyCondition (AutomationElementIdentifiers.AutomationIdProperty, automationId));
			} else if (name == string.Empty && automationId == string.Empty) {
				cond =  new PropertyCondition (AutomationElementIdentifiers.ControlTypeProperty, type);
			} else {
				cond = new AndCondition (new PropertyCondition (AutomationElementIdentifiers.ControlTypeProperty, type),
					new PropertyCondition (AutomationElementIdentifiers.NameProperty, name),
					new PropertyCondition (AutomationElementIdentifiers.AutomationIdProperty, automationId));
			}

			for (int i = 0; i < Config.Instance.RetryTimes; i++) {
				AutomationElement control = element.FindFirst (TreeScope.Descendants, cond);
				if (control != null)
					return Promote (control);

				Thread.Sleep (Config.Instance.RetryInterval);
			}

			return null;
		}

		public T Find<T> (Direction direction, int index) where T : Element
		{
			var elements = FindAll<T> ();

			if (direction == Direction.Horizental) {
				Array.Sort(elements, (e1, e2) => (int) (e1.AutomationElement.Current.BoundingRectangle.X -
				                                        e2.AutomationElement.Current.BoundingRectangle.X));
				return elements[index];
			} else if (direction == Direction.Vertical) {
				Array.Sort(elements, (e1, e2) => (int) (e1.AutomationElement.Current.BoundingRectangle.Y -
				                                        e2.AutomationElement.Current.BoundingRectangle.Y));
				return elements[index];
			}

			return null;
		}

		public T [] FindAll<T> () where T : Element
		{
			var uiaType = typeof (T).GetField ("UIAType", BindingFlags.Static | BindingFlags.Public);
			ControlType type = uiaType.GetValue (null) as ControlType;

			for (int i = 0; i < Config.Instance.RetryTimes; i++) {
				var cond = new PropertyCondition (AutomationElementIdentifiers.ControlTypeProperty, type);
				AutomationElementCollection controls = element.FindAll (TreeScope.Children, cond);
				if (controls != null)
					return Promote<T> (controls);

				Thread.Sleep (Config.Instance.RetryInterval);
			}
			return null;
		}

		// To promote a AutomationElement to a certain instance of a class,
		// in order to get more specific mothods.
		public static Element Promote (AutomationElement elm)
		{
			if (elm.Current.ControlType == ControlType.Window)
				return new Window (elm);
			else if (elm.Current.ControlType == ControlType.Button)
				return new Button (elm);
			else if (elm.Current.ControlType == ControlType.Edit)
				return new Edit (elm);
			else if (elm.Current.ControlType == ControlType.CheckBox)
				return new CheckBox (elm);
			else if (elm.Current.ControlType == ControlType.RadioButton)
				return new RadioButton (elm);
			else if (elm.Current.ControlType == ControlType.TabItem)
				return new TabItem (elm);
			else if (elm.Current.ControlType == ControlType.Spinner)
				return new Spinner (elm);
			else if (elm.Current.ControlType == ControlType.ComboBox)
				return new ComboBox (elm);
			else if (elm.Current.ControlType == ControlType.MenuBar)
				return new MenuBar (elm);
			else if (elm.Current.ControlType == ControlType.MenuItem)
				return new MenuItem (elm);
			else if (elm.Current.ControlType == ControlType.List)
				return new List (elm);
			else if (elm.Current.ControlType == ControlType.ListItem)
				return new ListItem (elm);
			else if (elm.Current.ControlType == ControlType.ToolBar)
				return new ToolBar (elm);
			else if (elm.Current.ControlType == ControlType.DataGrid)
				return new DataGrid (elm);
			else if (elm.Current.ControlType == ControlType.DataItem)
				return new DataItem (elm);
			else if (elm.Current.ControlType == ControlType.Document)
				return new Document (elm);
			else if (elm.Current.ControlType == ControlType.ScrollBar)
				return new ScrollBar (elm);
			else if (elm.Current.ControlType == ControlType.Text)
				return new Text (elm);
			else if (elm.Current.ControlType == ControlType.Pane)
				return new Pane (elm);
			else if (elm.Current.ControlType == ControlType.Menu)
				return new Menu (elm);
			else if (elm.Current.ControlType == ControlType.SplitButton)
				return new SplitButton (elm);

			return new Element (elm);
		}

		protected T [] Promote<T> (AutomationElementCollection elm) where T : Element
		{
			T [] ret = new T [elm.Count];
			for (int i = 0; i < elm.Count; i++)
				ret [i] = Promote (elm [i]) as T;
			return ret;
		}
#endregion
	}
}