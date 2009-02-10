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
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows.Automation;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.Behaviors.PropertyGrid
{
	internal class GridProviderBehavior : ProviderBehavior, IGridProvider
	{
#region Constructors
		public GridProviderBehavior (PropertyGridViewProvider provider)
			: base (provider)
		{
			viewProvider = provider;
		}
#endregion
		
#region IProviderBehavior Interface
		public override AutomationPattern ProviderPattern { 
			get { return GridPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			// NOTE: RowCount never changes (we nuke all the elements and start over)
			// NOTE: ColumnCount never changes (we nuke all the elements and start over)
		}
		
		public override void Disconnect ()
		{	
			Provider.SetEvent (ProviderEventType.GridPatternColumnCountProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.GridPatternRowCountProperty,
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == GridPatternIdentifiers.ColumnCountProperty.Id)
				return ColumnCount;
			else if (propertyId == GridPatternIdentifiers.RowCountProperty.Id)
				return RowCount;
			else
				return base.GetPropertyValue (propertyId);
		}
#endregion

#region IGridProvider implementation
		public int ColumnCount {
			get { return 2; }
		}

		public int RowCount {
			get { return viewProvider.ItemsCount; }
		}

		public IRawElementProviderSimple GetItem (int row, int column)
		{
			// According to http://msdn.microsoft.com/en-us/library/ms743401.aspx
			if (row < 0 || column < 0 || row >= RowCount || column >= ColumnCount)
			    throw new ArgumentOutOfRangeException ();

			return viewProvider.GetItem (row, column);
		}
#endregion

#region Private Fields
		private PropertyGridViewProvider viewProvider;
#endregion
	}
}
