// DataItem.cs: DataItem control type class wrapper.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;

namespace Mono.UIAutomation.TestFramework
{
	public class DataItem : Element
	{
		public static readonly ControlType UIAType = ControlType.DataItem;

		public DataItem (AutomationElement elm)
			:base(elm)
		{
		}

		#region GridItem Pattern
		public int Column {
			get { return (int) element.GetCurrentPropertyValue (GridItemPattern.ColumnProperty, true); }
		}

		public int ColumnSpan {
			get { return (int) element.GetCurrentPropertyValue (GridItemPattern.ColumnSpanProperty, true); }
		}

		public AutomationElement ContainingGrid {
			get { return (AutomationElement) element.GetCurrentPropertyValue (GridItemPattern.ContainingGridProperty, true); }
		}

		public int Row {
			get { return (int) element.GetCurrentPropertyValue (GridItemPattern.RowProperty, true); }
		}

		public int RowSpan {
			get { return (int) element.GetCurrentPropertyValue (GridItemPattern.RowSpanProperty, true); }
		}
		#endregion
	}
}