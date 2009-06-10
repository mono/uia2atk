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
//      Brad Taylor <brad@getcoded.net>
//

using Atk;

using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Automation.Peers;

namespace Moonlight.AtkBridge
{
	public class Adapter : Atk.Object
	{
#region Public Methods
		public Adapter (AutomationPeer peer)
		{
			this.Peer = peer;
		}
#endregion

#region Protected Properties
		protected AutomationPeer Peer {
			get;
			set;
		}
#endregion

#region Protected Methods
		protected override string OnGetName ()
		{
			return (Peer != null) ? Peer.GetName ()
			                      : String.Empty;
		}

		protected override Role OnGetRole ()
		{
			if (Peer == null)
				return Role.Unknown;

			AutomationControlType type = Peer.GetAutomationControlType ();
			switch (type) {
			case AutomationControlType.Button:
				return Role.PushButton;
			case AutomationControlType.Calendar:
				return Role.Calendar;
			case AutomationControlType.CheckBox:
				return Role.CheckBox;
			case AutomationControlType.ComboBox:
				return Role.ComboBox;
			case AutomationControlType.Edit:
				return Role.Text;
			case AutomationControlType.Hyperlink:
				return Role.Text;
			case AutomationControlType.Image:
				return Role.Image;
			case AutomationControlType.ListItem:
				return Role.ListItem;
			case AutomationControlType.List:
				return Role.List;
			case AutomationControlType.Menu:
				return Role.Menu;
			case AutomationControlType.MenuBar:
				return Role.MenuBar;
			case AutomationControlType.MenuItem:
				return Role.MenuItem;
			case AutomationControlType.ProgressBar:
				return Role.ProgressBar;
			case AutomationControlType.RadioButton:
				return Role.RadioButton;
			case AutomationControlType.ScrollBar:
				return Role.ScrollBar;
			case AutomationControlType.Slider:
				return Role.Slider;
			case AutomationControlType.Spinner:
				return Role.SpinButton;
			case AutomationControlType.StatusBar:
				return Role.Statusbar;
			case AutomationControlType.Tab:
				return Role.PageTabList;
			case AutomationControlType.TabItem:
				return Role.PageTab;
			case AutomationControlType.Text:
				return Role.Text;
			case AutomationControlType.ToolBar:
				return Role.ToolBar;
			case AutomationControlType.ToolTip:
				return Role.ToolTip;
			case AutomationControlType.Tree:
				return Role.Table;
			case AutomationControlType.TreeItem:
				return Role.TableCell;
			case AutomationControlType.Custom:
				return Role.Unknown;
			case AutomationControlType.Group:
				return Role.LayeredPane;
			case AutomationControlType.Thumb:
				return Role.PushButton;
			case AutomationControlType.DataGrid:
				return Role.Table;
			case AutomationControlType.DataItem:
				return Role.TableCell;
			case AutomationControlType.Document:
				return Role.Panel;
			case AutomationControlType.SplitButton:
				return Role.PushButton;
			case AutomationControlType.Window:
				return Role.Filler;
			case AutomationControlType.Pane:
				return Role.Panel;
			case AutomationControlType.Header:
				return Role.TableRowHeader;
			case AutomationControlType.HeaderItem:
				return Role.TableCell;
			case AutomationControlType.Table:
				return Role.Table;
			case AutomationControlType.TitleBar:
				return Role.MenuBar;
			case AutomationControlType.Separator:
				return Role.Separator;
			default:
				return Role.Unknown;
			}
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			if (Peer == null)
				return states;

			if (!Peer.IsOffscreen ()) {
				states.AddState (Atk.StateType.Showing);
				states.AddState (Atk.StateType.Visible);
			} else {
				states.RemoveState (Atk.StateType.Showing);
				states.RemoveState (Atk.StateType.Visible);
			}

			if (Peer.IsEnabled ()) {
				states.AddState (Atk.StateType.Sensitive);
				states.AddState (Atk.StateType.Enabled);
			} else {
				states.RemoveState (Atk.StateType.Sensitive);
				states.RemoveState (Atk.StateType.Enabled);
			}

			if (Peer.IsKeyboardFocusable ())
				states.AddState (Atk.StateType.Focusable);
			else
				states.RemoveState (Atk.StateType.Focusable);

			if (Peer.HasKeyboardFocus ())
				states.AddState (Atk.StateType.Focused);
			else
				states.RemoveState (Atk.StateType.Focused);

			return states;
		}

		protected override int OnGetNChildren ()
		{
			CacheChildren ();

			return (Children != null) ? Children.Count
			                          : 0;
		}

		protected override Atk.Object OnRefChild (int i)
		{
			CacheChildren ();

			if (Children == null || i < 0 || i > Children.Count)
				return null;

			AutomationPeer child = Children[i];
			if (child == null)
				return null;

			if (!Adapters.ContainsKey (child))
				Adapters[child] = DynamicAdapterFactory
					.Instance.GetAdapter (child);

			return Adapters[child];
		}

		protected override Atk.RelationSet OnRefRelationSet ()
		{
			return base.OnRefRelationSet ();
		}

		protected virtual void CacheChildren ()
		{
			if (Children != null)
				return;

			Children = Peer.GetChildren ();
		}
#endregion

#region Protected Fields
		protected List<AutomationPeer> Children = null;
		protected Dictionary<AutomationPeer, Atk.Object> Adapters
			= new Dictionary<AutomationPeer, Atk.Object> ();
#endregion
	}
}
