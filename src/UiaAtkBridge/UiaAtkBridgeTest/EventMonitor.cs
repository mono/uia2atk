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
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.IO;
using System.Diagnostics;

namespace UiaAtkBridgeTest
{
	
	
	internal class EventMonitor
	{
		Process p = null;
		string xmlResult = String.Empty;

		
		internal EventMonitor ()
		{
			string appWatcher = "atspimon.py";
			string appsToWatch = "nunit-console GtkSharpValue";
			File.Delete (System.IO.Directory.GetCurrentDirectory () + "/atspimon.py");
			System.IO.File.Copy (System.IO.Directory.GetCurrentDirectory () + "/../../../atspimon.py",
			                     System.IO.Directory.GetCurrentDirectory () + "/atspimon.py");
			p = new System.Diagnostics.Process ();
			p.StartInfo.FileName = "python";
			p.StartInfo.Arguments = String.Format ("-u {0} --xml {1}", appWatcher, appsToWatch);
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.EnableRaisingEvents = true;
			p.OutputDataReceived += new DataReceivedEventHandler (OnDataReceived);
			p.Start ();
			p.BeginOutputReadLine();
		}

		void OnDataReceived (object sender, DataReceivedEventArgs args)
		{
		    if (args.Data != null)
		        xmlResult += args.Data;
		}
		
		internal EventCollection Stop () {
			p.Close ();
			//Console.WriteLine ("XML:{0}", xmlResult);
			return new EventCollection (xmlResult + "</events>");
		}
	}
}
