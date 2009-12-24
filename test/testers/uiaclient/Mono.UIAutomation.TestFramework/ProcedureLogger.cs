// ProcedureLogger.cs: ouput the info to screen and write it into a xml file
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
//	Felicia Mu  (fxmu@novell.com)
//	Ray Wang  (rawang@novell.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace Mono.UIAutomation.TestFramework
{
	// Logger class which provides Actions logger and ExpectedResults logger.
	public class ProcedureLogger
	{
		static StringBuilder actionBuffer;
		static StringBuilder expectedResultBuffer;
		private static List<List<string>> procedures;
		private static DateTime startTime;

		public static void Init()
		{
			actionBuffer = new StringBuilder ();
			expectedResultBuffer = new StringBuilder ();
			procedures = new List<List<string>> ();
			startTime = DateTime.Now;
		}

		/**
		 * Log an action, e.g., Click Cancel
		 *
		 * Multiple calls to Action() (without a call to ExpectedResult() in between)
		 * will cause the message from each call to be concatenated to the message from
		 * previous calls.
		 */
		public void Action (string action)
		{
			FlushBuffer ();
			actionBuffer.Append(action);
			actionBuffer.Append (" ");
			Console.WriteLine ("Action: {0}", action);
		}

		/**
		 * Log an expected result, e.g., The dialog closes
		 *
		 * Multiple calls to ExpectedResult() (without a call to action() in between)
		 * will cause the message from each call to be concatenated to the message from
		 * previous calls.
		 */
		public void ExpectedResult (string expectedResult)
		{
			expectedResultBuffer.Append (expectedResult);
			expectedResultBuffer.Append (" ");
			Console.WriteLine ("Expected result: {0}", expectedResult);
		}

		// Save logged actions and expected results to an XML file
		public void Save ()
		{
			// get the total time the test run
			TimeSpan elapsed_time = DateTime.Now - startTime;

			FlushBuffer ();

			XmlDocument xmlDoc = new XmlDocument ();

			//add XML declaration
			XmlNode xmlDecl = xmlDoc.CreateXmlDeclaration ("1.0", "UTF-8", "");
			xmlDoc.AppendChild (xmlDecl);
			XmlNode xmlStyleSheet = xmlDoc.CreateProcessingInstruction ("xml-stylesheet", "type=\"text/xsl\" href=\"Resources/procedures.xsl\"");
			xmlDoc.AppendChild (xmlStyleSheet);

			//add a root element
			XmlElement rootElm = xmlDoc.CreateElement ("test");
			xmlDoc.AppendChild (rootElm);

			//add <name> element
			XmlElement nameElm = xmlDoc.CreateElement ("name");
			XmlText nameElmText = xmlDoc.CreateTextNode ("KeePass");
			nameElm.AppendChild (nameElmText);
			rootElm.AppendChild (nameElm);

			//add <description> element
			XmlElement descElm = xmlDoc.CreateElement ("description");
			XmlText descElmText = xmlDoc.CreateTextNode ("Test cases for KeePass");
			descElm.AppendChild (descElmText);
			rootElm.AppendChild (descElm);

			//add <parameters> element
			XmlElement paraElm = xmlDoc.CreateElement ("parameters");
			//TODO: add if clause to determine whether add <environments> element or not.
			XmlElement envElm = xmlDoc.CreateElement ("environments");
			paraElm.AppendChild (envElm);
			rootElm.AppendChild (paraElm);

			//add <procedures> element
			XmlElement procElm = xmlDoc.CreateElement ("procedures");

			foreach (List<string> p in procedures) {
				//add <action> element in <step> element
				XmlElement stepElm = xmlDoc.CreateElement ("step");

				//add <action> element in <step> element
				XmlElement actionElm = xmlDoc.CreateElement ("action");
				XmlText actionElmText = xmlDoc.CreateTextNode (p [0]);
				actionElm.AppendChild (actionElmText);
				stepElm.AppendChild (actionElm);

				//add <expectedResult> element in <step> element
				XmlElement resultElm = xmlDoc.CreateElement ("expectedResult");
				XmlText resultElmText = xmlDoc.CreateTextNode (p [1]);
				resultElm.AppendChild (resultElmText);
				stepElm.AppendChild (resultElm);

				//add <screenshot> element in <step> element
				if (Config.Instance.TakeScreenShots) {
					XmlElement screenshotElm = xmlDoc.CreateElement ("screenshot");
					XmlText screenshotElmText = xmlDoc.CreateTextNode (p [2]);
					//add if clause to determine whether has a screenshot or not.
					screenshotElm.AppendChild (screenshotElmText);
					stepElm.AppendChild (screenshotElm);
				}

				procElm.AppendChild (stepElm);
			}


			rootElm.AppendChild (procElm);

			//add <time> element
			XmlElement timeElm = xmlDoc.CreateElement ("time");
			XmlText timeEleText = xmlDoc.CreateTextNode (Convert.ToString (elapsed_time.TotalSeconds));
			timeElm.AppendChild (timeEleText);
			rootElm.AppendChild (timeElm);

			//write the Xml content to a xml file
			try {
				xmlDoc.Save ("procedures.xml");
			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}
		}

		/**
		 * FlushBuffer, Add (actionBuffer, expectedResultBuffer) to the _procedures list, then reset actionBuffer and expectedResultBuffer
		 *
		 * After a call to ExpectedResult() and before the next call to action(),
		 * (after an Action/ExpectedResult pair), we want to append the pair to the
		 * procedures list and possibly take a screenshot.
		 */
		private void FlushBuffer ()
		{
			if (actionBuffer.Length != 0  && expectedResultBuffer.Length != 0) {
				if (Config.Instance.TakeScreenShots) {
					string filename = string.Format ("screen{0:00}.png", procedures.Count + 1);
					// take screenshot
					Utils.TakeScreenshot (Path.Combine (Config.Instance.OutputDir, filename));
					Console.WriteLine ("Screenshot: " + filename);
					procedures.Add (new List<string> { actionBuffer.ToString().TrimEnd (), expectedResultBuffer.ToString().TrimEnd (), filename });
				} else {
					procedures.Add (new List<string> { actionBuffer.ToString().TrimEnd (), expectedResultBuffer.ToString().TrimEnd ()});
				}

				actionBuffer.Remove (0, actionBuffer.Length);
				expectedResultBuffer.Remove (0, expectedResultBuffer.Length);
				Console.WriteLine ();
			}
		}
	}
}