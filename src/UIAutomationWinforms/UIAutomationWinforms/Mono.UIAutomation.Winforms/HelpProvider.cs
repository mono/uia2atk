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
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using SD = System.Drawing;
using System.Windows;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms
{

	//NOTE:
	//     SWF.HelpProvider's ToolTip looks like a tooltip, however in UIA is
	//     ControlTypePane not a ControlTypeToolTip.
	internal class HelpProvider : ToolTipBaseProvider
	{
		
		#region Constructors
		
		public HelpProvider (SWF.HelpProvider helpProvider) : base (helpProvider)
		{
			this.helpProvider = helpProvider;
		}
		
		#endregion

		#region SimpleControlProvider: Specializations
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Pane.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "pane";
			else
				return base.GetPropertyValue (propertyId);
		}

		#endregion

		#region Public Methods
		
		public override void Show (SWF.Control control) 
		{
			if (AutomationInteropProvider.ClientsAreListening == true) {					
				Message = GetTextFromControl (control);

				FragmentControlProvider controlProvider 
					= ProviderFactory.FindProvider (control) as FragmentControlProvider;

				FragmentRootControlProvider rootProvider 
					= controlProvider as FragmentRootControlProvider;

				//FIXME: We may need to update this because seems that 
				//FragmentControlProvider *includes* children.
				
				if (rootProvider != null)
					rootProvider.AddChildProvider (true, this);
				else {
					rootProvider 
						= ProviderFactory.FindProvider (controlProvider.Container) as FragmentRootControlProvider;
					if (rootProvider != null)
						rootProvider.AddChildProvider (true, this);
				}
			}
		}
		
		public override void Hide (SWF.Control control)
		{
			if (AutomationInteropProvider.ClientsAreListening == true) {
				Message = GetTextFromControl (control);

				//FIXME: We may need to update this because seems that 
				//FragmentControlProvider *includes* children.

				FragmentRootControlProvider rootProvider 
					= Navigate (NavigateDirection.Parent) as FragmentRootControlProvider;

				if (rootProvider != null)
					rootProvider.RemoveChildProvider (true, this);
			}
		}

		#endregion
		
		#region Protected Methods

		protected override Rect GetBoundingRectangle ()
		{
			return Helper.RectangleToRect (helpProvider.UIAToolTipRectangle);
		}		
		
		protected override string GetTextFromControl (SWF.Control control)
		{
			return helpProvider.GetHelpString (control);
		}
		
		#endregion
		
		#region Private Fields
		
		private SWF.HelpProvider helpProvider;
		
		#endregion
	}
}
