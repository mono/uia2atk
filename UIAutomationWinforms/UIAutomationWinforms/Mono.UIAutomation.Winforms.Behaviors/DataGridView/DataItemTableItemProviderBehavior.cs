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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.DataGridView;

namespace Mono.UIAutomation.Winforms.Behaviors.DataGridView
{
	internal class DataItemTableItemProviderBehavior
		: DataItemGridItemProviderBehavior, ITableItemProvider
	{
		#region Constructors
		
		public DataItemTableItemProviderBehavior (ListItemProvider provider)
			: base (provider)
		{
		}
		
		#endregion
		
		#region ProviderBehavior Specialization
		
		public override AutomationPattern ProviderPattern {
			get { return TableItemPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			// NOTE: RowHeaderItemsProperty Property NEVER changes.
			Provider.SetEvent (ProviderEventType.TableItemPatternColumnHeaderItemsProperty,
			                   new DataItemTableItemColumnHeaderItemsEvent (DataItemProvider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.TableItemPatternColumnHeaderItemsProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.TableItemPatternRowHeaderItemsProperty,
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == TableItemPatternIdentifiers.RowHeaderItemsProperty.Id)
				return GetRowHeaderItems ();
			else if (propertyId == TableItemPatternIdentifiers.ColumnHeaderItemsProperty.Id)
				return GetColumnHeaderItems ();
			else
				return base.GetPropertyValue (propertyId);
		}
		
		#endregion
		
		#region ITableItemProvider Specialization

		public IRawElementProviderSimple[] GetColumnHeaderItems ()
		{
			if (DataItemProvider == null 
			    || DataItemProvider.DataGridViewProvider.Header == null)
				return new IRawElementProviderSimple [0];
			else
				return DataItemProvider.DataGridViewProvider.Header.GetHeaderItems ();
		}

		public IRawElementProviderSimple[] GetRowHeaderItems ()
		{
			return new IRawElementProviderSimple [0];
		}
		
		#endregion
	}
}
