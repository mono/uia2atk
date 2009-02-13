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
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ToolBar;

namespace Mono.UIAutomation.Winforms.Behaviors.ToolBar
{
	internal class ToolBarButtonToggleProviderBehavior : ProviderBehavior, IToggleProvider
	{
		#region Constructor

		public ToolBarButtonToggleProviderBehavior (ToolBarProvider.ToolBarButtonProvider provider)
			: base (provider)
		{
			this.toolBarButton = (SWF.ToolBarButton) Provider.Component;
			this.toolBar = toolBarButton.Parent;
		}
		
		#endregion
		
		#region IProviderBehavior Interface
		
		public override AutomationPattern ProviderPattern { 
			get { return TogglePatternIdentifiers.Pattern; }
		}

		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.TogglePatternToggleStateProperty,
			                   new ToolBarButtonTogglePatternToggleStateEvent (Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.TogglePatternToggleStateProperty,
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == TogglePatternIdentifiers.ToggleStateProperty.Id)
				return ToggleState;
			else
				return null;
		}

		#endregion
		
		#region IToggleProvider Members
		
		public ToggleState ToggleState
		{
			get {
				if (toolBarButton.Pushed)
					return ToggleState.On;
				else
					return ToggleState.Off;
			}
		}

		public void Toggle ()
		{
			if (!toolBarButton.Enabled)
				throw new ElementNotEnabledException ();

			PerformToggle ();
		}
		
		#endregion

		#region Private Methods

		private void PerformToggle ()
		{
			if (toolBar.InvokeRequired == true) {
				toolBar.BeginInvoke (new SWF.MethodInvoker (PerformToggle));
				return;
			}

			toolBarButton.Pushed = !toolBarButton.Pushed;
		}
		#endregion

		#region Private Fields

		private SWF.ToolBarButton toolBarButton;
		private SWF.ToolBar toolBar;

		#endregion
	}
}
