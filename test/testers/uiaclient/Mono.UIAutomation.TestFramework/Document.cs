// Document.cs: Document control class wrapper.
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
	public class Document : Element
	{
		public static readonly ControlType UIAType = ControlType.Document;
		
		//List the patterns that the control must support
		string[] SupportedPatterns = new string[] {"Text"};

		public Document (AutomationElement elm)
			: base (elm)
		{
		}

		#region Scroll Pattern
		public void Scroll (ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
		{
			Scroll (horizontalAmount, verticalAmount, true);
		}

		public void Scroll (ScrollAmount horizontalAmount, ScrollAmount verticalAmount, bool log)
		{
			if (log)
				procedureLogger.Action (string.Format ("Scroll {0} horizontally and {1} vertically.",
				                                       horizontalAmount.ToString (), verticalAmount.ToString ()));

			ScrollPattern sp = (ScrollPattern) element.GetCurrentPattern (ScrollPattern.Pattern);
			sp.Scroll (horizontalAmount, verticalAmount);
		}

		public void ScrollHorizontal (ScrollAmount amount)
		{
			ScrollHorizontal (amount, true);
		}

		public void ScrollHorizontal (ScrollAmount amount, bool log)
		{
			if (log)
				procedureLogger.Action (string.Format ("Scroll {0} horizontally.", amount.ToString ()));

			ScrollPattern sp = (ScrollPattern) element.GetCurrentPattern (ScrollPattern.Pattern);
			sp.ScrollHorizontal (amount);
		}

		public void ScrollVertical (ScrollAmount amount)
		{
			ScrollVertical (amount, true);
		}

		public void ScrollVertical (ScrollAmount amount, bool log)
		{
			if (log)
				procedureLogger.Action (string.Format ("Scroll {0} vertically..", amount.ToString ()));

			ScrollPattern sp = (ScrollPattern) element.GetCurrentPattern (ScrollPattern.Pattern);
			sp.ScrollVertical (amount);
		}

		public void SetScrollPercent (double horizontalPercent, double verticalPercent)
		{
			SetScrollPercent (horizontalPercent, verticalPercent, true);
		}

		public void SetScrollPercent (double horizontalPercent, double verticalPercent, bool log)
		{
			if (log)
				procedureLogger.Action (string.Format ("Set scroll {0} percent horizontally and {1} percent vertically.",
				                                       horizontalPercent, verticalPercent));

			ScrollPattern sp = (ScrollPattern) element.GetCurrentPattern (ScrollPattern.Pattern);
			sp.SetScrollPercent (horizontalPercent, verticalPercent);
		}

		public bool HorizontallyScrollable {
			get { return (bool) element.GetCurrentPropertyValue (ScrollPattern.HorizontallyScrollableProperty, true); }
		}

		public double HorizontalScrollPercent {
			get { return (double) element.GetCurrentPropertyValue (ScrollPattern.HorizontalScrollPercentProperty, true); }
		}

		public double HorizontalViewSize {
			get { return (double) element.GetCurrentPropertyValue (ScrollPattern.HorizontalViewSizeProperty, true); }
		}

		public bool VerticallyScrollable {
			get { return (bool) element.GetCurrentPropertyValue (ScrollPattern.VerticallyScrollableProperty, true); }
		}

		public double VerticalScrollPercent {
			get { return (double) element.GetCurrentPropertyValue (ScrollPattern.VerticalScrollPercentProperty, true); }
		}

		public double VerticalViewSize {
			get { return (double) element.GetCurrentPropertyValue (ScrollPattern.VerticalViewSizeProperty, true); }
		}
		#endregion
	}
}