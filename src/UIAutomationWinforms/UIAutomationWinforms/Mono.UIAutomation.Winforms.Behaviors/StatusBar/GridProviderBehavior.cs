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
// 	Neville Gao <nevillegao@gmail.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.StatusBar;

namespace Mono.UIAutomation.Winforms.Behaviors.StatusBar
{
	internal class GridProviderBehavior : ProviderBehavior, IGridProvider
	{
		#region Constructor

		public GridProviderBehavior (StatusBarProvider provider)
			: base (provider)
		{
			this.provider = provider;
			this.statusBar = (SWF.StatusBar) provider.Control;
		}
		
		#endregion

		#region IProviderBehavior Interface		
		
		public override AutomationPattern ProviderPattern { 
			get { return GridPatternIdentifiers.Pattern; }
		}

		public override void Connect (SWF.Control control)
		{
			// NOTE: RowColumn Property NEVER changes.
			Provider.SetEvent (ProviderEventType.GridPatternColumnCountProperty,
			                   new GridPatternColumnCountEvent (Provider));
		}
		
		public override void Disconnect (SWF.Control control)
		{
			Provider.SetEvent (ProviderEventType.GridPatternRowCountProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.GridPatternColumnCountProperty,
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == GridPatternIdentifiers.RowCountProperty.Id)
				return RowCount;
			else if (propertyId == GridPatternIdentifiers.ColumnCountProperty.Id)
				return ColumnCount;
			else
				return base.GetPropertyValue (propertyId);
		}

		#endregion
		
		#region IGridProvider Members

		public int RowCount {
			get { return 1; }
		}
		
		public int ColumnCount {
			get { return statusBar.Panels.Count; }
		}
		
		public IRawElementProviderSimple GetItem (int row, int column)
		{
			if (column >= RowCount || row >= ColumnCount)
				throw new ArgumentOutOfRangeException ();
			if (row < 0 || column < 0)
				throw new ArgumentOutOfRangeException ();
			
			return (IRawElementProviderSimple) provider.GetPanelProvider (column);
		}

		#endregion

		#region Private Fields

		private StatusBarProvider provider;
		private SWF.StatusBar statusBar;
		
		#endregion
	}
}
