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
using Mono.UIAutomation.Winforms.Events.TreeView;

namespace Mono.UIAutomation.Winforms.Behaviors.TreeView
{
	internal class ToggleProviderBehavior : ProviderBehavior, IToggleProvider
	{
		#region Private Members

		TreeNodeProvider nodeProvider;

		#endregion
		
		#region Constructors
		
		public ToggleProviderBehavior (TreeNodeProvider provider) :
			base (provider)
		{
			nodeProvider = provider;
		}

		#endregion
		
		#region IToggleProvider implementation
		
		public void Toggle ()
		{
			nodeProvider.TreeNode.Checked = !nodeProvider.TreeNode.Checked;
		}
		
		public ToggleState ToggleState {
			get {
				if (nodeProvider.TreeNode.Checked)
					return ToggleState.On;
				else
					return ToggleState.Off;
			}
		}
		
		#endregion

		#region ProviderBehavior Overrides

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == TogglePatternIdentifiers.ToggleStateProperty.Id)
				return ToggleState;
			return base.GetPropertyValue(propertyId);
		}


		public override AutomationPattern ProviderPattern {
			get {
				return TogglePatternIdentifiers.Pattern;
			}
		}
		
		public override void Connect ()
		{
			nodeProvider.SetEvent (ProviderEventType.TogglePatternToggleStateProperty,
			                       new TogglePatternToggleStateEvent (nodeProvider));
		}

		public override void Disconnect ()
		{
			nodeProvider.SetEvent (ProviderEventType.TogglePatternToggleStateProperty,
			                       null);
		}

		#endregion
	}
}
