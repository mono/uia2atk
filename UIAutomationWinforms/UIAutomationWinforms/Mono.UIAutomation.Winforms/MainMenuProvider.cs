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

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (SWF.MainMenu))]
	internal class MainMenuProvider : MenuProvider, IRawElementProviderFragmentRoot
	{
		private SWF.MainMenu mainMenu;
		
		public MainMenuProvider (SWF.MainMenu mainMenu) :
			base (mainMenu)
		{
			this.mainMenu = mainMenu;
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.MenuBar.Id;
			else if (propertyId == AEIds.OrientationProperty.Id)
				return OrientationType.Horizontal;
			else if (propertyId == AEIds.AccessKeyProperty.Id)
				return "ALT";
			else if (propertyId == AEIds.NameProperty.Id)
				return Catalog.GetString ("Application");
			else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
				return true;
			return base.GetProviderPropertyValue (propertyId);
		}

		protected override Rect BoundingRectangleProperty {
			get {
				System.Drawing.Rectangle rect = mainMenu.Rect;
				rect.Y -= rect.Height;
				return Helper.RectangleToRect (mainMenu.Wnd.RectangleToScreen (rect));
			}
		}
		
		#region MenuProvider Overrides

		protected override SWF.Menu Menu {
			get {
				return mainMenu;
			}
		}

		#endregion

		#region IRawElementProviderFragmentRoot Interface
		
		public virtual IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			if (!BoundingRectangle.Contains (x, y))
				return null;
			
			return this;
		}
		
		public virtual IRawElementProviderFragment GetFocus ()
		{
			return null;
		}

		//http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.irawelementproviderfragment.fragmentroot.aspx
		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { 
				if (Container == null)
					return (IRawElementProviderFragmentRoot) Navigate (NavigateDirection.Parent); 
				else
					return (IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (Container);
			}
		}
		
		#endregion
	}

	internal abstract class MenuProvider : FragmentControlProvider
	{
		private Dictionary<SWF.MenuItem, MenuItemProvider>
			itemProviders;
		
		public MenuProvider (SWF.Menu menu) :
			base (menu)
		{
			itemProviders = new Dictionary<SWF.MenuItem, MenuItemProvider> ();
		}

		protected abstract SWF.Menu Menu { get; }
		
		#region FragmentRootControlProvider: Specializations
		
		protected override void InitializeChildControlStructure ()
		{
			if (Menu != null)
				Menu.MenuChanged += OnMenuChanged;
		
			foreach (SWF.MenuItem item in Menu.MenuItems) {
				MenuItemProvider itemProvider = GetItemProvider (item);
				if (itemProvider != null)
					AddChildProvider (itemProvider);
			}
		}
		
		protected override void FinalizeChildControlStructure ()
		{
			if (Menu != null)
				Menu.MenuChanged -= OnMenuChanged;
			
			foreach (MenuItemProvider itemProvider in itemProviders.Values) {
				RemoveChildProvider (itemProvider);
				ProviderFactory.ReleaseProvider (itemProvider.Component);
			}
			OnNavigationChildrenCleared ();
		}

		#endregion

		#region Private Navigation Methods

		private void OnMenuChanged (object sender, EventArgs args)
		{
			RefreshItems ();
		}

		private void RefreshItems ()
		{
			List<SWF.MenuItem> itemsToDelete =
				new List<SWF.MenuItem> (itemProviders.Keys);

			foreach (SWF.MenuItem item in Menu.MenuItems) {
				MenuItemProvider itemProvider;
				if (!itemProviders.TryGetValue (item, out itemProvider)) {
					itemProvider = GetItemProvider (item);
					if (itemProvider != null)
						AddChildProvider (itemProvider);
				} else
					itemsToDelete.Remove (item);
			}

			foreach (SWF.MenuItem item in itemsToDelete) {
				MenuItemProvider itemProvider;
				if (itemProviders.TryGetValue (item, out itemProvider)) {
					itemProviders.Remove (item);
					itemProvider.Terminate ();
					RemoveChildProvider (itemProvider);
					ProviderFactory.ReleaseProvider (itemProvider.Component);
				}
			}
		}

		private MenuItemProvider GetItemProvider (SWF.MenuItem item)
		{
			MenuItemProvider itemProvider;
			
			if (!itemProviders.TryGetValue (item, out itemProvider)) {
				itemProvider = (MenuItemProvider) ProviderFactory.GetProvider (item);
				itemProviders [item]  = itemProvider;
			}

			return itemProvider;
		}

		#endregion
	}
}
