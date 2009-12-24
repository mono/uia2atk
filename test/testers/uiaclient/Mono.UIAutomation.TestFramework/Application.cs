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
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
//
// Authors:
//	Ray Wang  (rawang@novell.com)
//	Felicia Mu  (fxmu@novell.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Mono.UIAutomation.TestFramework
{
	public class Application
	{
		protected String sample;
		protected ProcedureLogger procedureLogger = new ProcedureLogger ();

		public Application (String sample)
		{
			if (sample == null)
				throw new ArgumentException ("Sample cannot be null.");
			this.sample = sample;
		}

		public void Launch ()
		{
			procedureLogger.Action ("Launch " + this.sample);
			try {
				Process.Start (sample);
				String ExpectResult = null;
				ExpectResult = string.Format ("{0} launched.", sample);
				procedureLogger.ExpectedResult (ExpectResult);
			} catch (Exception e) {
				Console.WriteLine (e.Message);
				Process.GetCurrentProcess ().Kill ();
			}
		}
	}
}
