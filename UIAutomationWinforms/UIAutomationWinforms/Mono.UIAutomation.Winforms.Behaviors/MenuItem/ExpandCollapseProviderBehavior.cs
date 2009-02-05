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
using SWF = System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.MenuItem;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace Mono.UIAutomation.Winforms.Behaviors.MenuItem
{
	internal class ExpandCollapseProviderBehavior : ProviderBehavior, IExpandCollapseProvider
	{
		private MenuItemProvider itemProvider;
		private SWF.MenuItem menuItem;
		
		public ExpandCollapseProviderBehavior (MenuItemProvider itemProvider) :
			base (itemProvider)
		{
			this.itemProvider = itemProvider;
			menuItem = this.itemProvider.Component as SWF.MenuItem;
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id)
				return ExpandCollapseState;
			return base.GetPropertyValue (propertyId);
		}

		public override AutomationPattern ProviderPattern {
			get {
				return ExpandCollapsePatternIdentifiers.Pattern;
			}
		}

		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.ExpandCollapsePatternExpandCollapseStateProperty,
			                   new ExpandCollapsePatternExpandCollapseStateEvent (itemProvider));
		}

		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.ExpandCollapsePatternExpandCollapseStateProperty,
			                   null);
		}

		#region IExpandCollapseProvider implementation
		
		public void Collapse ()
		{
			if (ExpandCollapseState != ExpandCollapseState.Collapsed) {
				if (menuItem.Parent == itemProvider.ParentMenu)
					MenuItemHelper.SimulateClick (menuItem);
				else if (menuItem.Parent is SWF.MenuItem)
					MenuItemHelper.SimulateMotion ((SWF.MenuItem) menuItem.Parent);
			}
		}
		
		public void Expand ()
		{
			if (ExpandCollapseState != ExpandCollapseState.Expanded)
				MenuItemHelper.SimulateClick (menuItem);
		}
		
		public ExpandCollapseState ExpandCollapseState {
			get {
				if (menuItem.Wnd != null)
					return ExpandCollapseState.Expanded;
				else
					return ExpandCollapseState.Collapsed;
			}
		}
		
		#endregion
	}
}
