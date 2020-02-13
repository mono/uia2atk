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
// 	Sandy Armstrong <sanfordarmstrong@gmail.com>
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
	[MapsComponent (typeof (SWF.ContextMenu))]
	internal class ContextMenuProvider : MenuProvider, IRawElementProviderFragmentRoot
	{
		private SWF.ContextMenu contextMenu;
		
		public ContextMenuProvider (SWF.ContextMenu contextMenu) :
			base (contextMenu)
		{
			this.contextMenu = contextMenu;
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Menu.Id;
			else if (propertyId == AEIds.IsContentElementProperty.Id)
				return false;
			else if (propertyId == AEIds.LabeledByProperty.Id)
				return null;
			return base.GetProviderPropertyValue (propertyId);
		}

		protected override Rect BoundingRectangleProperty {
			get {
				if (contextMenu.Wnd == null)
					return System.Windows.Rect.Empty;
				System.Drawing.Rectangle rect = contextMenu.Rect;
				rect.Y -= rect.Height;
				return Helper.RectangleToRect (contextMenu.Wnd.RectangleToScreen (rect));
			}
		}

		public override void Initialize ()
		{
			base.Initialize ();

			AutomationEventArgs args = new AutomationEventArgs (AEIds.MenuOpenedEvent);
			AutomationInteropProvider.RaiseAutomationEvent (this, args);
		}

		public override void Terminate ()
		{
			AutomationEventArgs args = new AutomationEventArgs (AEIds.MenuClosedEvent);
			AutomationInteropProvider.RaiseAutomationEvent (this, args);
			base.Terminate ();
		}
		
		#region MenuProvider Overrides

		protected override SWF.Menu Menu {
			get {
				return contextMenu;
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
				IRawElementProviderFragmentRoot root = null;
				
				if (Container == null)
					root = Navigate (NavigateDirection.Parent) as IRawElementProviderFragmentRoot; 
				else
					root = ProviderFactory.GetProvider (Container) as IRawElementProviderFragmentRoot;

				if (root == null)
					return this;
				else
					return root;
			}
		}
		
		#endregion
	}
}
