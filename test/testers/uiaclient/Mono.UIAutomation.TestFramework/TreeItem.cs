// TreeItem.cs: TreeItem control class wrapper.
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
//	Felicia Mu  (fxmu@novell.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;

namespace Mono.UIAutomation.TestFramework
{
	public class TreeItem : Element
	{
		public static readonly ControlType UIAType = ControlType.TreeItem;
		
		//List the patterns that the control must support
		string[] SupportedPatterns = new string[] {"ExpandCollapse"};

		public TreeItem (AutomationElement elm)
			: base (elm)
		{
		}

		#region SelectionItem Pattern
		public void Select ()
		{
			Select (true);
		}

		public void Select (bool log)
		{
			if (log)
				procedureLogger.Action (string.Format ("Select {0}.", this.NameAndType));

			SelectionItemPattern sip = (SelectionItemPattern) element.GetCurrentPattern (SelectionItemPattern.Pattern);
			sip.Select ();
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
	}
}