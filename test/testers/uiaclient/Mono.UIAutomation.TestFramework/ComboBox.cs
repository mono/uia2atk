// ComboBox.cs: ComboBox control class wrapper.
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
	public class ComboBox : Element
	{
		public static readonly ControlType UIAType = ControlType.ComboBox;
		
		//List the patterns that the control must support
		string[] SupportedPatterns = new string[] {"ExpandCollapse"};

		public ComboBox (AutomationElement elm)
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
		#endregion

		#region ExpandCollapse Pattern
		public void Expand ()
		{
			Expand (true);
		}

		public void Expand (bool log)
		{
			if (log)
				procedureLogger.Action (string.Format ("Expand {0}.", this.NameAndType));

			ExpandCollapsePattern ecp = (ExpandCollapsePattern) element.GetCurrentPattern (ExpandCollapsePattern.Pattern);
			ecp.Expand ();
		}

		// Collapse the combobox
		public void Collapse ()
		{
			Collapse (true);
		}

		public void Collapse (bool log)
		{
			if (log)
				procedureLogger.Action (string.Format ("Collapse {0}.", this.NameAndType));

			ExpandCollapsePattern ecp = (ExpandCollapsePattern) element.GetCurrentPattern (ExpandCollapsePattern.Pattern);
			ecp.Collapse ();
		}

		public ExpandCollapseState ExpandCollapseState {
			get { return (ExpandCollapseState) element.GetCurrentPropertyValue (ExpandCollapsePattern.ExpandCollapseStateProperty, true); }
		}
		#endregion

		#region Selection Pattern
		public AutomationElement [] GetSelection ()
		{
			SelectionPattern sp = (SelectionPattern) element.GetCurrentPattern (SelectionPattern.Pattern);
			return (AutomationElement []) sp.Current.GetSelection ();
		}

		public bool CanSelectMultiple {
			get { return (bool) element.GetCurrentPropertyValue (SelectionPattern.CanSelectMultipleProperty, true); }
		}

		public bool IsSelectionRequired {
			get { return (bool) element.GetCurrentPropertyValue (SelectionPattern.IsSelectionRequiredProperty, true); }
		}
		#endregion
	}
}
