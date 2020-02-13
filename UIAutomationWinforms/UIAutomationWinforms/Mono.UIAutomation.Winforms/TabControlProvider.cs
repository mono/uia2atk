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
using System.Windows.Automation;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors.TabControl;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (TabControl))]
	internal class TabControlProvider
		: FragmentRootControlProvider
	{
#region Public Methods

		public TabControlProvider (TabControl control) : base (control)
		{
			this.control = control;
		}

#endregion

		public override void Initialize ()
		{
			base.Initialize ();

			SetBehavior (SelectionPatternIdentifiers.Pattern,
				     new SelectionProviderBehavior (this));
			SetBehavior (ScrollPatternIdentifiers.Pattern,
			             new ScrollProviderBehavior (this));
		}

#region Protected Methods

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Tab.Id;
			else if (propertyId == AEIds.OrientationProperty.Id)
				return (control.Alignment == TabAlignment.Top || control.Alignment == TabAlignment.Bottom)
				       ? Orientation.Horizontal : Orientation.Vertical;

			return base.GetProviderPropertyValue (propertyId);
		}

		protected override bool GuideChildComponentVisible (FragmentControlProvider childProvider, out bool isVisible)
		{
			// Ensure that even though the TabPages will have
			// Visible = False when they're not selected, they stay
			// in the A11y hierarchy.  This is to model Vista's
			// behavior.
			isVisible = true;
			return true;
		}
#endregion

		internal bool HasSelection {
			get { return (control.SelectedIndex > -1); }
		}

		internal TabPageProvider GetSelectedTab ()
		{
			if (control.SelectedTab == null) {
				return null;
			}

			return (TabPageProvider) ProviderFactory.GetProvider (
				control.SelectedTab);
		}

		internal void SelectItem (TabPageProvider tabPage)
		{
			control.SelectedTab = (TabPage) tabPage.Control;
		}

		internal bool IsItemSelected (TabPageProvider tabPage)
		{
			return (control.SelectedTab == (TabPage) tabPage.Control);
		}
		
#region Private Fields

		private TabControl control;

#endregion
	}
}
