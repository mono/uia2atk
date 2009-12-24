// ScrollBar.cs: ScrollBar control class wrapper.
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
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
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
	public class ScrollBar : Element
	{
		public static readonly ControlType UIAType = ControlType.ScrollBar;

		public ScrollBar (AutomationElement elm)
			: base (elm)
		{
		}

		// The methods and properties of RangeValuePattern
		public void SetValue (double value)
		{
			SetValue (value, true);
		}

		public void SetValue (double value, bool log)
		{
			if (log == true)
				procedureLogger.Action (string.Format ("Set {0} to {1}.", value, this.Name));

			RangeValuePattern rvp = (RangeValuePattern) element.GetCurrentPattern (RangeValuePattern.Pattern);
			rvp.SetValue (value);
		}

		public bool IsReadOnly {
			get { return (bool) element.GetCurrentPropertyValue (RangeValuePattern.IsReadOnlyProperty); }
		}

		public double LargeChange {
			get { return (double) element.GetCurrentPropertyValue (RangeValuePattern.LargeChangeProperty); }
		}

		public double SmallChange {
			get { return (double) element.GetCurrentPropertyValue (RangeValuePattern.SmallChangeProperty); }
		}

		public double Maximum {
			get { return (double) element.GetCurrentPropertyValue (RangeValuePattern.MaximumProperty); }
		}

		public double Minimum {
			get { return (double) element.GetCurrentPropertyValue (RangeValuePattern.MinimumProperty); }
		}

		public double Value {
			get { return (double) element.GetCurrentPropertyValue (RangeValuePattern.ValueProperty); }
		}
	}
}