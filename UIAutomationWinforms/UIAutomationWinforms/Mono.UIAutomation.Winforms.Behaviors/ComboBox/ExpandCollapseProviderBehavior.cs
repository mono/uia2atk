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
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ComboBox;

namespace Mono.UIAutomation.Winforms.Behaviors.ComboBox
{

	internal class ExpandCollapseProviderBehavior 
		: ProviderBehavior, IExpandCollapseProvider
	{
		
		#region Constructor
		
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
				return ((SWF.ComboBox) Provider.Control).DroppedDown ? ExpandCollapseState.Expanded 
					: ExpandCollapseState.Collapsed;
			}
		}

		public void Collapse ()
		{
			if (((SWF.ComboBox) Provider.Control).DropDownStyle == SWF.ComboBoxStyle.Simple)
				return;
			
			if (Provider.Control.Enabled == false)
				throw new ElementNotEnabledException ();
			
			PerformExpandOrCollapse ((SWF.ComboBox) Provider.Control, false);
		}

		public void Expand ()
		{
			if (((SWF.ComboBox) Provider.Control).DropDownStyle == SWF.ComboBoxStyle.Simple)
				return;

			if (Provider.Control.Enabled == false)
				throw new ElementNotEnabledException ();
			
			PerformExpandOrCollapse ((SWF.ComboBox) Provider.Control, true);
		}

		#endregion
		
		#region Private Methods
		
		private void PerformExpandOrCollapse (SWF.ComboBox combobox, bool droppedDown)
		{
			if (combobox.InvokeRequired == true) {
				combobox.BeginInvoke (new PerformExpandOrCollapseDelegate (PerformExpandOrCollapse),
				                      new object [] { combobox, droppedDown } );
				return;
			}
			combobox.DroppedDown = droppedDown;
		}
		
		#endregion
		
	}
	
	delegate void PerformExpandOrCollapseDelegate (SWF.ComboBox combobox, bool droppedDown);
}
