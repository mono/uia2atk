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
//  Neville Gao <nevillegao@gmail.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Drawing;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ToolBar;

namespace Mono.UIAutomation.Winforms.Behaviors.ToolBar
{
	internal class ToolBarButtonExpandCollapseProviderBehavior : ProviderBehavior, IExpandCollapseProvider
	{
		#region Constructor

		public ToolBarButtonExpandCollapseProviderBehavior (ToolBarProvider.ToolBarButtonProvider provider)
			: base (provider)
		{
			this.toolBarButton = (SWF.ToolBarButton) Provider.Component;
			this.toolBar = toolBarButton.Parent;
		}
		
		#endregion
		
		#region IProviderBehavior Interface
		
		public override AutomationPattern ProviderPattern { 
			get { return ExpandCollapsePatternIdentifiers.Pattern; }
		}

		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.ExpandCollapsePatternExpandCollapseStateProperty,
			                   new ToolBarButtonExpandCollapsePatternExpandCollapseStateEvent (Provider));
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
				return null;
		}

		#endregion
		
		#region IExpandCollapseProvider Members
		
		public ExpandCollapseState ExpandCollapseState
		{
			get {
				SWF.Menu menu = toolBarButton.DropDownMenu;
				if (menu == null)
					return ExpandCollapseState.Collapsed;

				return toolBarButton.Pushed ? ExpandCollapseState.Expanded
					: ExpandCollapseState.Collapsed;
			}
		}

		public void Expand ()
		{
			if (ExpandCollapseState == ExpandCollapseState.LeafNode)
				throw new InvalidOperationException ();

			PerformExpandCollapse ();
		}

		public void Collapse ()
		{
			if (ExpandCollapseState == ExpandCollapseState.LeafNode)
				throw new InvalidOperationException ();

			PerformExpandCollapse ();
		}
		
		#endregion

		#region Private Methods

		private void PerformExpandCollapse ()
		{
			if (toolBar.InvokeRequired == true) {
				toolBar.BeginInvoke (new SWF.MethodInvoker (PerformExpandCollapse));
				return;
			}

			((SWF.ContextMenu) toolBarButton.DropDownMenu).Show (toolBar,
			                                                     new Point (toolBarButton.Rectangle.X, toolBar.Height));
		}
		
		#endregion

		#region Private Fields

		private SWF.ToolBarButton toolBarButton;
		private SWF.ToolBar toolBar;

		#endregion
	}
}
