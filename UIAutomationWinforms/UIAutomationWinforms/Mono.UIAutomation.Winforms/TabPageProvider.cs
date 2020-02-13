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
//	Brad Taylor <brad@getcoded.net>
//

using System;
using Mono.Unix;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors.TabPage;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (TabPage))]
	internal class TabPageProvider : PanelProvider
	{
		public TabPageProvider (TabPage control) : base (control)
		{
			this.tabPage = control;

			selectionBehavior = new SelectionItemProviderBehavior (this);
			SetBehavior (SelectionItemPatternIdentifiers.Pattern,
			             selectionBehavior);
		}
		
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.TabItem.Id;
			return base.GetProviderPropertyValue (propertyId);
		}

		protected override Rect BoundingRectangleProperty {
			get {
				// Don't return empty if we're not visible.  We
				// want this control's visiblity managed by the
				// TabControl, as tab pages are on-screen even
				// if they report Visible = false.
				return Helper.RectangleToRect (
					Control.Parent.RectangleToScreen (tabPage.TabBounds));
			}
		}

		protected override bool GuideChildComponentVisible (FragmentControlProvider childProvider, out bool isVisible)
		{
			// Hide the TabPage's children if it's not visible.
			// This is to sweep under the rug the fact that SWF
			// seems to keep a TabPages' children visible even if
			// the TabPage isn't.  This is to model Vista's
			// behavior.
			isVisible = Control.Visible;
			return true;
		}

		internal TabControlProvider TabControlProvider {
			get { return (TabControlProvider) Navigate (NavigateDirection.Parent); }
		}

		internal bool IsSelected {
			get {
				if (TabControlProvider == null)
					return false;

				return TabControlProvider.IsItemSelected (this);
			}
		}

		public override void SetFocus ()
		{
			//because we want to cause a HasKeyboardFocus property changed event on
			//the tab control, not on the tabpage (as this is how this works on UIA+Vista)
			selectionBehavior.Select ();
		}
		
		private TabPage tabPage;
		private SelectionItemProviderBehavior selectionBehavior;
	}
}
