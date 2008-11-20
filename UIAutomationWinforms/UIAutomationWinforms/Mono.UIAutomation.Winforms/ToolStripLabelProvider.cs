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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Behaviors.ToolStripItem;

namespace Mono.UIAutomation.Winforms
{
	internal class ToolStripLabelProvider : ToolStripItemProvider
	{
		private ToolStripLabel label;
		
		public ToolStripLabelProvider (ToolStripLabel label) : base (label)
		{
			this.label = label;
		}

		public override void Initialize()
		{
			base.Initialize ();

			if (label.IsLink)
				SetBehavior (InvokePatternIdentifiers.Pattern,
				             new InvokeProviderBehavior (this));

			try {
				Helper.AddPrivateEvent (typeof (ToolStripLabel),
				                        label,
				                        "UIAIsLinkChanged",
				                        this,
				                        "OnIsLinkChanged");
			} catch (NotSupportedException) { }
		}

		public override void Terminate()
		{
			base.Terminate ();

			try {
				Helper.RemovePrivateEvent (typeof (ToolStripLabel),
				                           label,
				                           "UIAIsLinkChanged",
				                           this,
				                           "OnIsLinkChanged");
			} catch (NotSupportedException) { }
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return label.IsLink ? ControlType.Hyperlink.Id : ControlType.Text.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return label.IsLink ? "hyperlink" : "text";
			else
				return base.GetProviderPropertyValue (propertyId);
		}

		#pragma warning disable 169
		
		private void OnIsLinkChanged (object sender, EventArgs args)
		{
			if (label.IsLink)
				SetBehavior (InvokePatternIdentifiers.Pattern,
				             new InvokeProviderBehavior (this));
			else
				SetBehavior (InvokePatternIdentifiers.Pattern,
				             null);
		}

		#pragma warning restore 169
	}
}
