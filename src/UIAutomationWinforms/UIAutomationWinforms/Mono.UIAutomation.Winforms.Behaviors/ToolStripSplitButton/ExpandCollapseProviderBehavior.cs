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
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ToolStripSplitButton;

namespace Mono.UIAutomation.Winforms.Behaviors.ToolStripSplitButton
{
	internal class ExpandCollapseProviderBehavior
		: ProviderBehavior, IExpandCollapseProvider
	{
#region Public Methods
		public ExpandCollapseProviderBehavior (FragmentControlProvider provider)
			: base (provider)
		{
		}
#endregion
		
#region IProviderBehavior Interface
		public override AutomationPattern ProviderPattern { 
			get { return ExpandCollapsePatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{		
			Provider.SetEvent (ProviderEventType.ExpandCollapsePatternExpandCollapseStateProperty, 
			                   new ExpandCollapsePatternStateEvent (Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.ExpandCollapsePatternExpandCollapseStateProperty, 
			                   null);
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id)
				return ExpandCollapseState;
			else
				return base.GetPropertyValue (propertyId);
		}
#endregion

#region IExpandCollapseProvider Interface
		public ExpandCollapseState ExpandCollapseState {
			get {
				SWF.ToolStripDropDown dropdown
					= ((SWF.ToolStripSplitButton) Provider.Component).DropDown;
				if (dropdown == null)
					return ExpandCollapseState.Collapsed;

				return dropdown.Visible ? ExpandCollapseState.Expanded
				                        : ExpandCollapseState.Collapsed;
			}
		}

		public void Collapse ()
		{
			((SWF.ToolStripSplitButton) Provider.Component).HideDropDown ();
		}

		public void Expand ()
		{
			((SWF.ToolStripSplitButton) Provider.Component).ShowDropDown ();
		}
#endregion
	}

}
