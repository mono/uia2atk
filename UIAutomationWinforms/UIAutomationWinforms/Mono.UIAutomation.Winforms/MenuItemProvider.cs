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
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;

using Mono.Unix;
using Mono.UIAutomation.Winforms.Events;
using EMI = Mono.UIAutomation.Winforms.Events.MenuItem;
using Mono.UIAutomation.Winforms.Behaviors.MenuItem;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (SWF.MenuItem))]
	internal class MenuItemProvider : FragmentControlProvider
	{
		private SWF.MenuItem menuItem;
		private SWF.Menu parentMenu;
		private SWF.MainMenu mainMenu;
		private MenuItemMenuProvider menuProvider;
		private System.Windows.Rect bounds;
		
		public MenuItemProvider (SWF.MenuItem menuItem) :
			base (menuItem)
		{
			this.menuItem = menuItem;
			parentMenu = mainMenu =	menuItem.GetMainMenu ();
			if (parentMenu == null)
				parentMenu = menuItem.GetContextMenu ();
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.MenuItem.Id;
			else if (propertyId == AEIds.NameProperty.Id)
				return Helper.StripAmpersands (menuItem.Text);
			else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
				return true;
			else if (propertyId == AEIds.IsEnabledProperty.Id)
				return menuItem.Enabled;
			return base.GetProviderPropertyValue (propertyId);
		}

		protected override Rect BoundingRectangleProperty {
			get { return bounds; }
		}

		public override SWF.Control AssociatedControl {
			get { 
				if (mainMenu != null)
					return mainMenu.GetForm (); 
				// What about ContextMenu?
				else
					return null;
			}
		}
		
		#region FragmentRootControlProvider: Specializations

		public override void Initialize()
		{
			base.Initialize ();

			SetEvent (ProviderEventType.AutomationElementIsEnabledProperty,
			          new EMI.AutomationIsEnabledPropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementNameProperty,
			          new EMI.AutomationNamePropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementBoundingRectangleProperty,
			          new EMI.AutomationBoundingRectanglePropertyEvent (this));
			//SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
			          //new EMI.AutomationHasKeyboardFocusPropertyEvent (this));

			menuItem.UIACheckedChanged += OnBehaviorChanged;
			menuItem.UIARadioCheckChanged += OnBehaviorChanged;
			menuItem.MenuChanged += OnMenuChanged;
			
			bounds = MenuItemHelper.GetBounds (menuItem);

			UpdateBehaviors ();
		}

		public override void Terminate ()
		{
			base.Terminate ();

			menuItem.UIACheckedChanged -= OnBehaviorChanged;
			menuItem.UIARadioCheckChanged -= OnBehaviorChanged;
			menuItem.MenuChanged -= OnMenuChanged;
		}

		
		protected override void InitializeChildControlStructure ()
		{
			if (menuItem.MenuItems.Count > 0) {
				menuProvider = new MenuItemMenuProvider (menuItem);
				menuProvider.Initialize ();
				AddChildProvider (menuProvider);
			}
		}
		
		protected override void FinalizeChildControlStructure ()
		{
			if (menuProvider != null) {
				menuProvider.Terminate ();
				RemoveChildProvider (menuProvider);
				OnNavigationChildrenCleared ();
			}
		}

		public override IRawElementProviderFragmentRoot FragmentRoot {
			get {
				return (IRawElementProviderFragmentRoot)
					ProviderFactory.GetProvider (parentMenu);
			}
		}

		#endregion

		#region Public Members

		public SWF.Menu ParentMenu
		{
			get { return parentMenu; }
		}

		public void SetBounds (System.Windows.Rect bounds)
		{
			this.bounds = bounds;
		}

		internal SWF.MenuItem MenuItem {
			get {
				return menuItem;
			}
		}
		#endregion

		#region Private Methods

		private void OnMenuChanged (object sender, EventArgs args)
		{
			if (menuProvider == null && menuItem.MenuItems.Count > 0) {
				menuProvider = new MenuItemMenuProvider (menuItem);
				menuProvider.Initialize ();
				AddChildProvider (menuProvider);
			} else if (menuProvider != null && menuItem.MenuItems.Count == 0) {
				menuProvider.Terminate ();
				RemoveChildProvider (menuProvider);
				OnNavigationChildrenCleared ();
				menuProvider = null;
			}
			
			UpdateBehaviors ();
		}

		private void OnBehaviorChanged (object sender, EventArgs args)
		{
			UpdateBehaviors ();
		}

		private void UpdateBehaviors ()
		{
			bool supportInvoke = (menuItem.MenuItems.Count == 0);
			bool supportExpandCollapse = !supportInvoke;
			bool supportSelectionItem = supportInvoke && menuItem.RadioCheck;
			bool supportToggle = supportInvoke && !supportSelectionItem && menuItem.Checked;
			
			if (supportInvoke &&
			    GetBehavior (InvokePatternIdentifiers.Pattern) == null)
				SetBehavior (InvokePatternIdentifiers.Pattern,
				             new InvokeProviderBehavior (this));
			else if (!supportInvoke)
				SetBehavior (InvokePatternIdentifiers.Pattern,
				             null);
			
			if (supportExpandCollapse &&
			    GetBehavior (ExpandCollapsePatternIdentifiers.Pattern) == null)
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
				             new ExpandCollapseProviderBehavior (this));
			else if (!supportExpandCollapse)
				SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
				             null);
			
			if (supportSelectionItem &&
			    GetBehavior (SelectionItemPatternIdentifiers.Pattern) == null)
				SetBehavior (SelectionItemPatternIdentifiers.Pattern,
				             new SelectionItemProviderBehavior (this));
			else if (!supportSelectionItem)
				SetBehavior (SelectionItemPatternIdentifiers.Pattern,
				             null);
			
			if (supportToggle &&
			    GetBehavior (TogglePatternIdentifiers.Pattern) == null)
				SetBehavior (TogglePatternIdentifiers.Pattern,
				             new ToggleProviderBehavior (this));
			else if (!supportToggle)
				SetBehavior (TogglePatternIdentifiers.Pattern,
				             null);
		}

		#endregion
	}

	internal class MenuItemMenuProvider : MenuProvider
	{
		private SWF.MenuItem menuItem;
		
		public MenuItemMenuProvider (SWF.MenuItem menuItem) :
			base (null)
		{
			this.menuItem = menuItem;
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Menu.Id;
			else if (propertyId == AEIds.NameProperty.Id)
				return menuItem.Text;
			return base.GetProviderPropertyValue (propertyId);
		}
		
		#region MenuProvider Overrides

		protected override SWF.Menu Menu {
			get {
				return menuItem;
			}
		}

		#endregion
	}
	
	public class MenuItemHelper
	{
		public delegate void MenuItemOperator (SWF.MenuItem item);
		
		public static void SimulateClick (SWF.MenuItem item)
		{
			SimulateMotion (item);
			
			SWF.Menu parentMenu;
			SWF.Control wnd = GetWnd (item, out parentMenu);
			if (wnd == null)
				return;
			
			if (wnd.InvokeRequired) {
				wnd.BeginInvoke (new MenuItemOperator (SimulateClick),
				                 item);
				return;
			}
			
			SWF.MouseEventArgs args = GetMouseArgs (item);
			parentMenu.tracker.OnMouseDown (args);
			parentMenu.tracker.OnMouseUp (args);
		}

		public static void SimulateMotion (SWF.MenuItem item)
		{
			SWF.Menu parentMenu;
			SWF.Control wnd = GetWnd (item, out parentMenu);
			if (wnd == null)
				return;
			
			if (wnd.InvokeRequired) {
				wnd.BeginInvoke (new MenuItemOperator (SimulateMotion),
				                 item);
				return;
			}

			SWF.MouseEventArgs args = GetMouseArgs (item);
			parentMenu.tracker.OnMotion (args);
		}

		public static System.Windows.Rect GetBounds (SWF.MenuItem item)
		{
			SWF.Menu parentMenu;
			SWF.Control wnd = GetWnd (item, out parentMenu);

			if (wnd == null)
				return System.Windows.Rect.Empty;
			
			System.Drawing.Rectangle rect = item.bounds;
			System.Windows.Rect returnRect =
				Helper.RectangleToRect (wnd.RectangleToScreen (rect));
			if (item.Parent == parentMenu)
				returnRect.Y -= returnRect.Height;
			return returnRect;
		}

		private static SWF.MouseEventArgs GetMouseArgs (SWF.MenuItem item)
		{
			System.Windows.Rect rect = GetBounds (item);
			return new SWF.MouseEventArgs (SWF.MouseButtons.Left,
			                               1,
			                               (int) rect.Left + (int)(rect.Width/2),
			                               (int) rect.Top + (int)(rect.Height/2),
			                               0);
		}

		private static SWF.Control GetWnd (SWF.MenuItem item, out SWF.Menu parentMenu)
		{
			parentMenu = GetRootMenu (item);
			
			if (item.Parent != null && item.Parent.Wnd != null) {
				return item.Parent.Wnd;
			} else if (parentMenu != null && parentMenu.Wnd != null) {
				return parentMenu.Wnd;
			} else
				return null;
		}

		private static SWF.Menu GetRootMenu (SWF.MenuItem item)
		{
			SWF.Menu parentMenu = item.GetMainMenu ();
			if (parentMenu == null)
				parentMenu = item.GetContextMenu ();
			return parentMenu;
		}
	}
}
