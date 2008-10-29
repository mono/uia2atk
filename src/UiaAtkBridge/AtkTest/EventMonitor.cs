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
	
	public class EventMonitor
	{
		static Process p = null;
		static string xmlResult = String.Empty;
		static EventMonitor singleton = null;
		static object locking = new object ();
		
		public static void Start () {
			lock (locking) {
				if (singleton == null)
					singleton = new EventMonitor ();
				xmlResult = String.Empty;
			}
		}
		
		private EventMonitor ()
		{
			string appWatcher = "atspimon.py";
			string appsToWatch = "nunit-console GtkSharpValue";
			
			File.Delete (Path.Combine (Directory.GetCurrentDirectory (), "atspimon.py"));
			File.Copy (Path.Combine (Misc.LookForParentDir ("*.py"), "atspimon.py"),
			           Path.Combine (Directory.GetCurrentDirectory (), "atspimon.py"));
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
			//wait a bit until atspimon.py initializes
			System.Threading.Thread.Sleep (1000);
		}

		void OnDataReceived (object sender, DataReceivedEventArgs args)
		{
			lock (locking)
				if (args.Data != null)
					xmlResult += args.Data;
		}

		internal static EventCollection Pause () {
			lock (locking) {
				if (singleton == null)
					throw new Exception ("EventMonitor has not been started yet");
				try {
					return new EventCollection (xmlResult);
				}
				finally {
					xmlResult = String.Empty;
				}
			}
		}

		public static void Stop () {
			StopWithResult (true);
		}

		internal static EventCollection StopWithResult () {
			return StopWithResult (false);
		}
		
		private static EventCollection StopWithResult (bool ignoreNotStarted) {
			lock (locking) {
				if (singleton == null) {
					if (ignoreNotStarted)
						return null;
					throw new Exception ("EventMonitor has not been started yet"); 
				}
				p.Kill ();
				p.Dispose ();
				p = null;
				string result = xmlResult;
				xmlResult = String.Empty;
				return new EventCollection (result);
			}
		}
	}
}
