// Edit.cs: Edit control class wrapper.
//
// This program is free software; you can redistribute it and/or modify it under
// the terms of the GNU General Public License version 2 as published by the
// Free Software Foundation.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public License along with
// this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
//
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
//
// Authors:
//	Ray Wang  (rawang@novell.com)
//	Felicia Mu  (fxmu@novell.com)

using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using System.Windows.Automation.Text;

namespace Mono.UIAutomation.TestFramework
{
	public class Edit : Element
	{
		public static readonly ControlType UIAType = ControlType.Edit;

		public Edit (AutomationElement elm)
			: base (elm)
		{
		}

		#region Value Pattern
		public void SetValue (string value)
		{
			SetValue (value, true);
		}

		public void SetValue (string value, bool log)
		{
			if (log)
				procedureLogger.Action (string.Format ("Set {0} to {1}.", value, this.NameAndType));

			ValuePattern vp = (ValuePattern) element.GetCurrentPattern (ValuePattern.Pattern);
			vp.SetValue (value);
		}

		public string Value {
			get { return (string) element.GetCurrentPropertyValue (ValuePattern.ValueProperty, true); }
		}

		public bool IsReadOnly {
			get { return (bool) element.GetCurrentPropertyValue (ValuePattern.IsReadOnlyProperty, true); }
		}
		#endregion

		#region Text Pattern
		public TextPatternRange [] GetSelection ()
		{
			return GetSelection (true);
		}

		public TextPatternRange [] GetSelection (bool log)
		{
			if (log)
				procedureLogger.Action(string.Format("Get selection from {0}.", this.NameAndType));

			TextPattern tp = (TextPattern) element.GetCurrentPattern (TextPattern.Pattern);
			return (TextPatternRange []) tp.GetSelection ();
		}

		public TextPatternRange [] GetVisibleRanges ()
		{
			return GetVisibleRanges (true);
		}

		public TextPatternRange [] GetVisibleRanges (bool log)
		{
			if (log)
				procedureLogger.Action(string.Format("Get visible ranges from {0}.", this.NameAndType));

			TextPattern tp = (TextPattern) element.GetCurrentPattern (TextPattern.Pattern);
			return (TextPatternRange []) tp.GetVisibleRanges();
		}

		public TextPatternRange RangeFromChild (AutomationElement childElement)
		{
			return RangeFromChild (childElement, true);
		}

		public TextPatternRange RangeFromChild (AutomationElement childElement, bool log)
		{
			if (log)
				procedureLogger.Action(string.Format("Get the range from child of {0}.", this.NameAndType));

			TextPattern tp = (TextPattern) element.GetCurrentPattern (TextPattern.Pattern);
			return (TextPatternRange) tp.RangeFromChild (childElement);
		}

		public TextPatternRange RangeFromPoint (Point screenLocation)
		{
			return RangeFromPoint (screenLocation, true);
		}

		public TextPatternRange RangeFromPoint (Point screenLocation, bool log)
		{
			if (log)
				procedureLogger.Action(string.Format("Get the range from point of {0}.", this.NameAndType));

			TextPattern tp = (TextPattern) element.GetCurrentPattern (TextPattern.Pattern);
			return (TextPatternRange) tp.RangeFromPoint (screenLocation);
		}

		public TextPatternRange DocumentRange {
			get {
				TextPattern tp = (TextPattern) element.GetCurrentPattern (TextPattern.Pattern);
				return (TextPatternRange) tp.DocumentRange;
			}
		}
		#endregion

		#region TableItem Pattern
		public int Column {
			get { return (int) element.GetCurrentPropertyValue (TableItemPattern.ColumnProperty, true); }
		}

		public int ColumnSpan {
			get { return (int) element.GetCurrentPropertyValue (TableItemPattern.ColumnSpanProperty, true); }
		}

		public int Row {
			get { return (int) element.GetCurrentPropertyValue (TableItemPattern.RowProperty, true); }
		}

		public int RowSpan {
			get { return (int) element.GetCurrentPropertyValue (TableItemPattern.RowSpanProperty, true); }
		}

		public AutomationElement ContainingGrid {
			get { return (AutomationElement) element.GetCurrentPropertyValue (TableItemPattern.ContainingGridProperty, true); }
		}

		public AutomationElement[] ColumnHeaderItems {
			get { return (AutomationElement[]) element.GetCurrentPropertyValue (TableItemPattern.ColumnHeaderItemsProperty, true); }
		}

		public AutomationElement[] RowHeaderItems {
			get { return (AutomationElement[]) element.GetCurrentPropertyValue (TableItemPattern.RowHeaderItemsProperty, true); }
		}
		#endregion
	}
}