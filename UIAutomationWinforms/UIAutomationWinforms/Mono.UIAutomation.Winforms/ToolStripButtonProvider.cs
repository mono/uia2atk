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
// Copyright (c) 2008,2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Brad Taylor <brad@getcoded.net>
// 

using System;
using Mono.Unix;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors.ToolStripButton;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (ToolStripButton))]
	internal class ToolStripButtonProvider : ToolStripItemProvider
	{
#region Constructors
		public ToolStripButtonProvider (ToolStripButton button) : base (button)
		{
			this.button = button;
		}
#endregion

#region IRawElementProviderSimple Overrides
		public override void Initialize ()
		{
			base.Initialize ();

			button.UIACheckOnClickChanged += new EventHandler (OnCheckOnClickChanged);

			UpdateBehaviors ();
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return button.CheckOnClick ? ControlType.CheckBox.Id : ControlType.Button.Id;
			else if (propertyId == AEIds.LabeledByProperty.Id)
				return null;
			else
				return base.GetProviderPropertyValue (propertyId);
		}
#endregion

#region Private Methods
		private void UpdateBehaviors ()
		{
			SetBehavior (InvokePatternIdentifiers.Pattern, 
			             new InvokeProviderBehavior (this));

			if (button.CheckOnClick)
				SetBehavior (TogglePatternIdentifiers.Pattern, 
					     new ToggleProviderBehavior (this));
			else
				SetBehavior (TogglePatternIdentifiers.Pattern, 
					     null);
		}

		private void OnCheckOnClickChanged (object o, EventArgs args)
		{
			UpdateBehaviors ();
		}
#endregion

#region Private Fields
		private ToolStripButton button;
#endregion
	}

}
