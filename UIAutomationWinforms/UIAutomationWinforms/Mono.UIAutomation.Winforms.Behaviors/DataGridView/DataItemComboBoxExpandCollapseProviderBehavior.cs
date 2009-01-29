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
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Behaviors.DataGridView
{

	internal class DataItemComboBoxExpandCollapseProviderBehavior 
		: ProviderBehavior, IExpandCollapseProvider
	{
		
		#region Constructor
		
		public DataItemComboBoxExpandCollapseProviderBehavior (DataGridViewProvider.DataGridViewDataItemComboBoxProvider provider)
			: base (provider)
		{
			this.provider = provider;
		}
		
		#endregion

		#region IProviderBehavior Interface
		
		public override AutomationPattern ProviderPattern { 
			get { return ExpandCollapsePatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			// FIXME: Implement, How?
//			Provider.SetEvent (ProviderEventType.ExpandCollapsePatternExpandCollapseStateProperty, 
//			                   new ExpandCollapsePatternStateEvent (Provider));
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
				// FIXME: Implement, How?
				return ExpandCollapseState.Collapsed;
			}
		}

		public void Collapse ()
		{
			if (provider.ComboBoxCell.ReadOnly)
				throw new ElementNotEnabledException ();

			// FIXME: Implement, How?
		}

		public void Expand ()
		{
			if (provider.ComboBoxCell.ReadOnly)
				throw new ElementNotEnabledException ();
			
			// FIXME: Implement, How?
		}

		#endregion

		#region Private Fields

		private DataGridViewProvider.DataGridViewDataItemComboBoxProvider provider;

		#endregion
		
	}
}
