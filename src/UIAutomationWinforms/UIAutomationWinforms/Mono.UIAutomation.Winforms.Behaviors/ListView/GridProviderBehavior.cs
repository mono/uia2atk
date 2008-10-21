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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ListView;

namespace Mono.UIAutomation.Winforms.Behaviors.ListView
{

	internal class GridProviderBehavior 
		: ProviderBehavior, IGridProvider
	{
		
		#region Constructors

		public GridProviderBehavior (ListViewProvider provider)
			: base (provider)
		{
			listView = (SWF.ListView) Provider.Control;
		}

		#endregion
		
		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern { 
			get { return GridPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.GridPatternColumnCountProperty,
			                   new GridPatternColumnEvent ((ListViewProvider) Provider));
			Provider.SetEvent (ProviderEventType.GridPatternRowCountProperty,
			                   new GridPatternRowEvent ((ListViewProvider) Provider));
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
			//Works for View.List
			get {
				bool module = listView.Items.Count % RowCount > 0;
				return (listView.Items.Count / RowCount) + (module ? 1 : 0);
			}
		}

		public int RowCount {
			//Works for View.List
			get { return MaxRows; }
		}

		//Remarks: 
		//- Grid coordinates are zero-based with the upper left (or upper
		//  right cell depending on locale) having coordinates (0,0).
		//- If a cell is empty a UI Automation provider must still be returned
		//  in order to support the ContainingGrid property for that cell. This
		//  is possible when the layout of child elements in the grid is 
		//  similar to a ragged array.
		//- Hidden rows and columns, depending on the provider implementation, 
		//  can be loaded in the UI Automation tree and will therefore be 
		//  reflected in the RowCount and ColumnCount properties. If the hidden
		//  rows and columns have not yet been loaded they should not be counted.
		public IRawElementProviderSimple GetItem (int row, int column)
		{		
			int rowCount = RowCount;
			int columnCount = ColumnCount;

			//According to http://msdn.microsoft.com/en-us/library/ms743401.aspx
			if (row < 0 || column < 0 || row >= rowCount || column >= columnCount)
			    throw new ArgumentOutOfRangeException ();

			ListViewProvider provider = (ListViewProvider) Provider;

			//Works for View.List
			return provider.GetProviderAt ((column * rowCount) + row);
		}
		
		#endregion

		#region Private Properties

		private int MaxRows {
			get {
				return Helper.GetPrivateProperty<SWF.ListView, int> (typeof (SWF.ListView),
				                                                     (SWF.ListView) Provider.Control,
				                                                     "UIARows");
			}
		}

		#endregion

		#region

		private SWF.ListView listView;

		#endregion

	}
				                      
}
