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
using System.Windows.Automation.Provider;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace Moonlight.AtkBridge
{
	public class Adapter : Atk.Object, Atk.ComponentImplementor
	{
#region Public Properties
		public AutomationPeer Peer {
			get;
			protected set;
		}

		public double Alpha {
			get { return 1.0d; }
		}

		public new Layer Layer {
			get { return Atk.Layer.Widget; }
		}

		public new int MdiZorder {
			get { return 0; }
		}
#endregion

#region Public Methods
		public Adapter (AutomationPeer peer)
		{
			this.Peer = peer;
		}

		public uint AddFocusHandler (FocusHandler handler)
		{
			if (focusHandlers.ContainsValue (handler))
				return 0;

			lastFocusHandlerId++;
			focusHandlers [lastFocusHandlerId] = handler;
			return lastFocusHandlerId;
		}

		public bool Contains (int x, int y, CoordType coordType)
		{
			if (Peer == null)
				return false;

			// Despite MSDN documentation, this is actually in
			// window coordinates
			Rect r = Peer.GetBoundingRectangle ();

			if (coordType == CoordType.Screen)
				ScreenToWindow (ref x, ref y);

			return r.Contains (new System.Windows.Point (x, y));
		}

		public void GetExtents (out int x, out int y,
		                        out int width, out int height,
		                        CoordType coordType)
		{
			x = y = Int32.MinValue;
			width = height = 0;

			if (Peer == null)
				return;

			// Despite MSDN documentation, this is actually in
			// window coordinates
			Rect r = Peer.GetBoundingRectangle ();

			if (!Peer.IsOffscreen ()) {
				x = (int) r.X;
				y = (int) r.Y;

				if (coordType == CoordType.Screen)
					WindowToScreen (ref x, ref y);
			}

			width = 100;
			height = 100;
			width = (int) r.Width;
			height = (int) r.Height;
		}

		public void GetPosition (out int x, out int y,
		                         CoordType coordType)
		{
			x = y = Int32.MinValue;

			if (Peer == null || Peer.IsOffscreen ())
				return;

			// Despite MSDN documentation, this is actually in
			// window coordinates
			Rect r = Peer.GetBoundingRectangle ();

			x = (int) r.X;
			y = (int) r.Y;

			if (coordType == CoordType.Screen)
				WindowToScreen (ref x, ref y);
		}

		public void GetSize (out int width, out int height)
		{
			width = height = 0;

			if (Peer == null)
				return;

			// Despite MSDN documentation, this is actually in
			// window coordinates
			Rect r = Peer.GetBoundingRectangle ();

			width = (int) r.Width;
			height = (int) r.Height;
		}

		public bool GrabFocus ()
		{
			if (Peer == null)
				return false;

			Peer.SetFocus ();

			return Peer.HasKeyboardFocus ();
		}

		public Atk.Object RefAccessibleAtPoint (int x, int y,
		                                        CoordType coordType)
		{
			CacheChildren ();

			lock (ChildrenLock) {
				if (Children != null) {
					foreach (AutomationPeer peer in Children) {
						Adapter adapter = DynamicAdapterFactory
							.Instance.GetAdapter (peer);
						if (adapter == null)
							continue;

						if (adapter.Contains (x, y, coordType))
							return adapter;
					}
				}
			}

			return Contains (x, y, coordType) ? this : null;
		}

		public void RemoveFocusHandler (uint handlerId)
		{
			if (focusHandlers.ContainsKey (handlerId))
				focusHandlers.Remove (handlerId);
		}

		public bool SetExtents (int x, int y, int width, int height,
		                        CoordType coordType)
		{
			return false;
		}

		public bool SetPosition (int x, int y, CoordType coordType)
		{
			return false;
		}

		public bool SetSize (int width, int height)
		{
			return false;
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
				return Role.Label;
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
				return Role.Label;
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

			// Selection PatternImplementor specific states
			ISelectionItemProvider selectionItem
				= Peer.GetPattern (PatternInterface.SelectionItem)
					as ISelectionItemProvider;
			if (selectionItem != null) {
				states.AddState (Atk.StateType.Selectable);

				if (selectionItem.IsSelected)
					states.AddState (Atk.StateType.Selected);
				else
					states.RemoveState (Atk.StateType.Selected);
			} else {
				states.RemoveState (Atk.StateType.Selectable);
				states.RemoveState (Atk.StateType.Selected);
			}

			return states;
		}

		protected override int OnGetNChildren ()
		{
			CacheChildren ();

			lock (ChildrenLock) {
				return (Children != null) ? Children.Count : 0;
			}
		}

		protected override Atk.Object OnRefChild (int i)
		{
			CacheChildren ();

			lock (ChildrenLock) {
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
		}

		protected override Atk.RelationSet OnRefRelationSet ()
		{
			return base.OnRefRelationSet ();
		}

		protected override Atk.Object OnGetParent ()
		{
			AutomationPeer parent = Peer.GetParent ();

			// XXX: This is a huge hack.
			// ScrollViewer is implemented so that its children
			// appear as the children of its parent.  This gives us
			// a bit of a headache as the ScrollViewer is never
			// available in the Atk hierarchy, and an adapter is
			// never created for it.  Thus we pretend it doesn't
			// exist.
			if (parent is ScrollViewerAutomationPeer
			    && Peer is ItemAutomationPeer)
				parent = parent.GetParent ();

			if (parent == null)
				return DynamicAdapterFactory.Instance.RootVisualAdapter;

			return DynamicAdapterFactory.Instance.GetAdapter (parent);
		}

		protected override int OnGetIndexInParent ()
		{
			Adapter parent = Parent as Adapter;
			if (parent == null)
				return -1;

			return parent.GetIndexOfChild (this);
		}

		protected virtual void CacheChildren ()
		{
			lock (ChildrenLock) {
				if (Children != null)
					return;

				Children = Peer.GetChildren ();
			}
		}

		protected void NotifyFocused (bool focused)
		{
			NotifyStateChange (Atk.StateType.Focused, focused);

			if (focused)
				Atk.Focus.TrackerNotify (this);
		}
#endregion

#region Protected Fields
		protected List<AutomationPeer> Children = null;
		protected object ChildrenLock = new object ();
		protected Dictionary<AutomationPeer, Atk.Object> Adapters
			= new Dictionary<AutomationPeer, Atk.Object> ();
#endregion

#region Internal Events
		internal event EventHandler<AutomationPropertyChangedEventArgs> AutomationPropertyChanged;
		internal event EventHandler<AutomationEventEventArgs> AutomationEventRaised;
#endregion

#region Internal Methods
		internal int GetIndexOfChild (Adapter child)
		{
			CacheChildren ();

			if (Children == null || child == null)
				return -1;

			// Intentionally use Children list instead of
			// Peer.GetChildren () as we're concerned with what is
			// currently displayed to the user
			lock (ChildrenLock) {
				return Children.IndexOf (child.Peer);
			}
		}

		internal void HandleAutomationPropertyChanged (AutomationPropertyChangedEventArgs args)
		{
			if (args.Property == AEIds.HasKeyboardFocusProperty) {
				bool focused = (bool) args.NewValue;
				NotifyFocused (focused);

				foreach (FocusHandler handler in focusHandlers.Values)
					handler (this, focused);
			} else if (args.Property == AEIds.IsOffscreenProperty) {
				bool offscreen = (bool) args.NewValue;
				NotifyStateChange (Atk.StateType.Visible, !offscreen);
			} else if (args.Property == AEIds.IsEnabledProperty) {
				bool enabled = (bool) args.NewValue;
				NotifyStateChange (Atk.StateType.Enabled, enabled);
				NotifyStateChange (Atk.StateType.Sensitive, enabled);
			} else if (args.Property == AEIds.HelpTextProperty) {
				Description = (string) args.NewValue;
			} else if (args.Property == AEIds.BoundingRectangleProperty) {
				EmitBoundsChanged ((System.Windows.Rect) args.NewValue);
			} else if (args.Property == AEIds.NameProperty) {
				// TODO: Emit name changed signal
			}

			if (AutomationPropertyChanged != null)
				AutomationPropertyChanged (args.Peer, args);
		}

		internal void HandleAutomationEventRaised (AutomationEventEventArgs args)
		{
			if (AutomationEventRaised != null)
				AutomationEventRaised (args.Peer, args);
		}

		internal void WindowToScreen (ref int x, ref int y)
		{
			ConvertCoords (ref x, ref y, true);
		}

		internal void ScreenToWindow (ref int x, ref int y)
		{
			ConvertCoords (ref x, ref y, false);
		}

		internal void NotifyStateChange (Atk.StateType state)
		{
			NotifyStateChange (state, RefStateSet ().ContainsState (state));
		}

		internal void EmitSignal (string signal)
		{
			GLib.Signal.Emit (this, signal);
		}
#endregion

#region Private Methods
		private void ConvertCoords (ref int x, ref int y, bool addParent)
		{
			Adapter rootVisual
				= DynamicAdapterFactory.Instance.RootVisualAdapter;
			if (rootVisual == null || rootVisual.Parent == null) {
				// TODO: Logging
				return;
			}

			// Since our parent is unmanaged, we can't just cast.
			Atk.Component parent = ComponentAdapter.GetObject (rootVisual.Parent);
			if (parent == null) {
				// TODO: Logging
				return;
			}

			int parentX, parentY;
			parent.GetPosition (out parentX, out parentY, CoordType.Screen);

			x += parentX * (addParent ? 1 : -1);
			y += parentY * (addParent ? 1 : -1);
		}

		private void EmitBoundsChanged (System.Windows.Rect rect)
		{
			Atk.Rectangle atkRect;
			atkRect.X = (int) rect.X;
			atkRect.Y = (int) rect.Y;
			atkRect.Width = (int) rect.Width;
			atkRect.Height = (int) rect.Height;
			GLib.Signal.Emit (this, "bounds_changed", atkRect);
		}
#endregion

#region Private Fields
		private uint lastFocusHandlerId = 0;
		private Dictionary<uint, FocusHandler> focusHandlers
			= new Dictionary<uint, FocusHandler> ();
#endregion
	}
}
