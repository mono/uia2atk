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
//	Brad Taylor <brad@getcoded.net>
//

using System;
using Mono.Unix;
using System.Threading;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Automation;
using System.Collections.Generic;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors.MonthCalendar;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace Mono.UIAutomation.Winforms
{
	internal class MonthCalendarListItemProvider : FragmentRootControlProvider
	{
		public MonthCalendarListItemProvider (MonthCalendarDataGridProvider dataGridProvider,
		                                      MonthCalendarProvider calendarProvider,
		                                      Control control, DateTime date,
		                                      int row, int col)
			: base (control)
		{
			this.dataGridProvider = dataGridProvider;
			this.calendarProvider = calendarProvider;
			this.date = date;
			this.row = row;
			this.col = col;
		}

		public MonthCalendarDataGridProvider DataGridProvider {
			get { return dataGridProvider; }
		}

		public MonthCalendarProvider MonthCalendarProvider {
			get { return calendarProvider; }
		}

		public int Column {
			get { return col; }
		}

		public int Row {
			get { return row; }
		}

		public string Text {
			get { return date.Day.ToString (); }
		}

		public DateTime Date {
			get { return date; }
		}

		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { return dataGridProvider; }
		}

		public override void Initialize ()
		{
			base.Initialize ();

			SetBehavior (GridItemPatternIdentifiers.Pattern,
			             new ListItemGridItemProviderBehavior (this));
			SetBehavior (TableItemPatternIdentifiers.Pattern,
			             new ListItemTableItemProviderBehavior (this));
			SetBehavior (ValuePatternIdentifiers.Pattern,
			             new ListItemValueProviderBehavior (this, this));
			SetBehavior (SelectionItemPatternIdentifiers.Pattern,
			             new ListItemSelectionItemProviderBehavior (this));
		}

		protected override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			editChild = new MonthCalendarListItemEditProvider (
				this, Control);
			editChild.Initialize ();
			AddChildProvider (editChild);
		}
		
		protected override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();

			RemoveChildProvider (editChild);
			editChild.Terminate ();
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.ListItem.Id;
			else if (propertyId == AEIds.NameProperty.Id)
				return Text;
			else if (propertyId == AEIds.NativeWindowHandleProperty.Id)
				return null;

			return base.GetProviderPropertyValue (propertyId);
		}

		private int row, col;
		private DateTime date;
		private MonthCalendarProvider calendarProvider;
		private MonthCalendarDataGridProvider dataGridProvider;
		private MonthCalendarListItemEditProvider editChild;
	}

	internal class MonthCalendarListItemEditProvider
		: FragmentControlProvider
	{
		public MonthCalendarListItemEditProvider (MonthCalendarListItemProvider listItemProvider,
		                                          Control control)
			: base (control)
		{
			this.listItemProvider = listItemProvider;
		}

		public override void Initialize ()
		{
			base.Initialize ();

			SetBehavior (ValuePatternIdentifiers.Pattern,
			             new ListItemValueProviderBehavior (this, listItemProvider));
		}

		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { return listItemProvider; }
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Edit.Id;
			else if (propertyId == AEIds.NameProperty.Id)
				return listItemProvider.Text;
			else if (propertyId == AEIds.NativeWindowHandleProperty.Id)
				return null;

			return base.GetProviderPropertyValue (propertyId);
		}

		private MonthCalendarListItemProvider listItemProvider;
	}
}
