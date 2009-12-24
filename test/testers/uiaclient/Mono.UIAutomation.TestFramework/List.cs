// List.cs: List control class wrapper.
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
	public class List : Element
	{
		public static readonly ControlType UIAType = ControlType.List;

		public List (AutomationElement elm)
			: base (elm)
		{
		}

		// The methods of MultipleViewPattern.
		public string GetViewName (int viewId)
		{
			MultipleViewPattern mvp = (MultipleViewPattern) element.GetCurrentPattern (MultipleViewPattern.Pattern);
			return mvp.GetViewName (viewId);
		}

		public int [] GetSupportedViews ()
		{
			MultipleViewPattern mvp = (MultipleViewPattern) element.GetCurrentPattern (MultipleViewPattern.Pattern);
			return mvp.Current.GetSupportedViews ();
		}

		public void SetCurrentView (int viewId)
		{
			SetCurrentView (viewId, true);
		}

		public void SetCurrentView (int viewId, bool log)
		{
			if (log == true)
				procedureLogger.Action (string.Format ("Set current view to {0}.", GetViewName (viewId)));

			MultipleViewPattern mvp = (MultipleViewPattern) element.GetCurrentPattern (MultipleViewPattern.Pattern);
			mvp.SetCurrentView (viewId);
		}

		public int CurrentView {
			get { return (int) element.GetCurrentPropertyValue (MultipleViewPattern.CurrentViewProperty); }
		}

		// The method and properties of SelectionPattern.
		public AutomationElement [] GetSelection ()
		{
			SelectionPattern sp = (SelectionPattern) element.GetCurrentPattern (SelectionPattern.Pattern);
			return (AutomationElement []) sp.Current.GetSelection ();
		}

		public bool CanSelectMultiple {
			get { return (bool) element.GetCurrentPropertyValue (SelectionPattern.CanSelectMultipleProperty); }
		}

		public bool IsSelectionRequired {
			get { return (bool) element.GetCurrentPropertyValue (SelectionPattern.IsSelectionRequiredProperty); }
		}
	}
}