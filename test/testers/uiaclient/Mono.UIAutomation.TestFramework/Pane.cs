// Pane.cs: Pane control class wrapper.
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
	public class Pane : Element
	{
		public static readonly ControlType UIAType = ControlType.Pane;

		public Pane (AutomationElement elm)
			: base (elm)
		{
		}

		// The methods and properties of DockPattern
		public void SetDockPosition (DockPosition position)
		{
			SetDockPosition (position, true);
		}

		public void SetDockPosition (DockPosition position, bool log)
		{
			if (log == true)
				procedureLogger.Action (string.Format ("Dock {0} to {1}.", this.Name, position));

			DockPattern dp = (DockPattern) element.GetCurrentPattern (DockPattern.Pattern);
			dp.SetDockPosition (position);
		}

		public DockPosition DockPosition {
			get { return (DockPosition) element.GetCurrentPropertyValue (DockPattern.DockPositionProperty); }
		}

		// The method and properties of TransformPattern
		public void Rotate (double degree)
		{
			Rotate (degree, true);
		}

		public void Rotate (double degree, bool log)
		{
			if (log == true)
				procedureLogger.Action (string.Format ("Rotate {0} {1} degree(s).", this.Name, degree));

			TransformPattern tp = (TransformPattern) element.GetCurrentPattern (TransformPattern.Pattern);
			tp.Rotate (degree);
		}

		public bool CanRotate {
			get { return (bool) element.GetCurrentPropertyValue (TransformPattern.CanRotateProperty); }
		}
	}
}