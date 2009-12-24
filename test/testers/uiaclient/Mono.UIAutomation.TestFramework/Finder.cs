// Finder.cs: Find Helper class to assist finding controls from application.
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
using System.Threading;
using System.Reflection;

namespace Mono.UIAutomation.TestFramework
{
	public class Finder
	{

		List<Condition> conditions = new List<Condition> ();
		ControlType ct = null;
		AutomationElement element = null;

		public Finder (AutomationElement element)
		{
			this.element = element;
		}

		public Finder ByName (string name)
		{
			conditions.Add (new PropertyCondition (AutomationElementIdentifiers.NameProperty, name));
			return this;
		}

		public Finder ByAutomationId (string automationId)
		{
			conditions.Add (new PropertyCondition (AutomationElementIdentifiers.AutomationIdProperty, automationId));
			return this;
		}

		public T Find<T> () where T : Element
		{
			var uiaType = typeof (T).GetField ("UIAType", BindingFlags.Static | BindingFlags.Public);
			ct = uiaType.GetValue (null) as ControlType;
			conditions.Add (new PropertyCondition (AutomationElementIdentifiers.ControlTypeProperty, ct));
			return Find () as T;
		}

		Element Find ()
		{
			Condition condition = null;

			if (conditions.Count == 1)
				condition = conditions [0];
			else
				condition = new AndCondition (conditions.ToArray ());

			for (int i = 0; i < Config.Instance.RetryTimes; i++) {
				AutomationElement control = element.FindFirst (TreeScope.Descendants, condition);
				if (control != null)
					return Element.Promote (control);

				Thread.Sleep (Config.Instance.RetryInterval);
			}

			return null;
		}
	}
}