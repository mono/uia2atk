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

using Mono.UIAutomation.Winforms.Behaviors.ToolStripItem;

namespace Mono.UIAutomation.Winforms
{
	internal class ToolStripLabelProvider : ToolStripItemProvider
	{
		private ToolStripLabel label;
		
		public ToolStripLabelProvider (ToolStripLabel label) : base (label)
		{
			this.label = label;


			// TODO: No built-in event for IsLink property changing,
			//	 may need to patch MWF.
		}

		public override void Initialize()
		{
			base.Initialize ();

			if (label.IsLink)
				SetBehavior (InvokePatternIdentifiers.Pattern,
				             new InvokeProviderBehavior (this));
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return label.IsLink ? ControlType.Hyperlink.Id : ControlType.Text.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return label.IsLink ? "hyperlink" : "text";
			else
				return base.GetPropertyValue (propertyId);
		}
	}
}
