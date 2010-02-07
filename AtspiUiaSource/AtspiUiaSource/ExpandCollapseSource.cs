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
//      Mike Gorse <mgorse@novell.com>
//

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using Mono.Unix;
using Mono.UIAutomation.Source;
using Atspi;
using System.Windows.Automation.Provider;

namespace AtspiUiaSource
{
	public class ExpandCollapseSource : IExpandCollapseProvider
	{
		private Accessible accessible;
		private Atspi.Action action;
		private int expandOrContractAction;

		public ExpandCollapseSource (Element element)
		{
			accessible = element.Accessible;
			action = accessible.QueryAction ();

			ActionDescription [] actions = action.Actions;
			for (int i = 0; i < actions.Length; i++) {
				if (actions [i].Name == "expand or contract") {
					expandOrContractAction = i;
					break;
				}
			}
		}

		public ExpandCollapseState ExpandCollapseState {
			get {
				return (accessible.StateSet.Contains (StateType.Expanded)
					? ExpandCollapseState.Expanded
					: ExpandCollapseState.Collapsed);
			}
		}

		private bool AccessibleIsEnabled (Accessible accessible)
		{
			if (accessible.Parent == null || accessible.Role == Role.Application)
				return true;
			// Work around gail notifying for Sensitive but not
			// Enabled
			if (!accessible.StateSet.Contains (StateType.Enabled) || !accessible.StateSet.Contains (StateType.Sensitive))
				return false;
			return AccessibleIsEnabled (accessible.Parent);
		}

		public void Collapse ()
		{
			if (!AccessibleIsEnabled (accessible))
				throw new ElementNotEnabledException ();

			if (accessible.StateSet.Contains (StateType.Expanded))
						action.DoAction (expandOrContractAction);
		}

		public void Expand ()
		{
			if (!AccessibleIsEnabled (accessible))
				throw new ElementNotEnabledException ();

			if (!accessible.StateSet.Contains (StateType.Expanded))
						action.DoAction (expandOrContractAction);
		}
	}
}
