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
//	Felicia Mu  (fxmu@novell.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using System.Collections;


namespace Mono.UIAutomation.TestFramework
{
	public class Helper
	{
		protected ProcedureLogger procedureLogger = new ProcedureLogger ();
		//Check the patterns that a control must support according to MSDN.
		protected void PatternChcek (Element e, AutomationElement ae, AutomationPattern[] addStates, AutomationPattern[] invalidState)
		{
			procedureLogger.Action (string.Format ("Check {0}'s supported patterns.", e));
			
			//Remove the extra patterns from supported patterns list
			for (int i = 0; i < invalidState.Length; i++)
				if(((System.Collections.IList)e.SupportedPatterns).Contains(invalidState [i]))
					e.SupportedPatterns.Remove(invalidState[i]);
					
			//Add necessary patterns to the supported patterns list		
			for (int j = 0; j < addStates.Length; j++)
				e.SupportedPatterns.Add(addStates [j]);

			//Get list of actual patterns of this control
			AutomationPattern[] actualPatterns = ae.GetSupportedPatterns();
			procedureLogger.ExpectedResult (string.Format ("supported patterns :"));
			for (int k = 0; k < actualPatterns.Length; k++)
				procedureLogger.ExpectedResult (string.Format ("{0} ", actualPatterns [k]));
			
			//get a list of actual patterns that are missing or extraneous
			List<AutomationPattern> missPattern = new List<AutomationPattern>();
			List<AutomationPattern> extraPattern = new List<AutomationPattern>();
			for (int p = 0; p < e.SupportedPatterns.Count; p++)
					if(!(actualPatterns.Contains (e.SupportedPatterns[p])))
						missPattern.Add(e.SupportedPatterns[p]);
				
			for (int q = 0; q < actualPatterns.Length; q++)
					if(!(e.SupportedPatterns.Contains (actualPatterns[q])))
						extraPattern.Add(actualPatterns[q]);

			//If missingPatterns and extraPatterns are empty, the test case passes
			//otherwise, throw an exception
			if(0 != missPattern.Count)
					Console.WriteLine("Missing miss actions: ");
					for (int n = 0; n < missPattern.Count; n++)
						Console.WriteLine(" {0}", missPattern[n]);	
			Assert.AreEqual(0, missPattern.Count);
				
			if(0 != actualPatterns.Length)
					Console.WriteLine("Missing actual actions: ");
					for (int m = 0; m < actualPatterns.Length; m++)
						Console.WriteLine(" {0}", actualPatterns[m]);
		}

	}
}

