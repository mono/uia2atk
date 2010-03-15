// Application.cs: launch the appointed application.
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
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;

namespace Mono.UIAutomation.TestFramework
{
	public class Application
	{
		protected String sample;
		protected ProcedureLogger procedureLogger;

		public Application (String sample)
		{
			if (sample == null)
				throw new ArgumentException ("Sample cannot be null.");

			this.sample = sample;
			procedureLogger = new ProcedureLogger(sample);
		}

		public void Launch (string program, params string[] args)
		{
			try {
				procedureLogger.Action ("Launch " + this.sample);
				Process.Start (program, String.Join (" ", args));
				Thread.Sleep (Config.Instance.ShortDelay);
				procedureLogger.ExpectedResult (string.Format("{0} has been started.", this.sample));
			} catch (Exception e) {
				Console.WriteLine (e.Message);
				Process.GetCurrentProcess ().Kill ();
			}
		}

		public Window GetWindow (String title)
		{
			var ae = AutomationElement.RootElement.FindFirst (TreeScope.Children, 
			                                                  new PropertyCondition (
			                                                  AutomationElementIdentifiers.NameProperty, 
			                                                  title));
			return new Window (ae);
		}
	}
}
