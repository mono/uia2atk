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
//      Brad Taylor <brad@getcoded.net>
// 

using System;

// XXX: Directly copied from Mono.UIAutomation.Services
namespace Moonlight.AtkBridge
{
	public enum LogLevel
	{
		Information,
		Debug,
		Warning,
		Error
	}

	// Store these in a ring buffer?
  	// Color highlighting?
  	// Textual log levels?
	public static class Log
	{
#region Public Properties
		public static LogLevel CurrentLogLevel {
			get { return currentLogLevel; }
		}
#endregion

#region Public Methods
		static Log ()
		{
			string level = Environment.GetEnvironmentVariable ("MONO_UIA_LOG_LEVEL");
			if (level != null) {
				try {
					currentLogLevel = (LogLevel) Convert.ToInt32 (level);
				} catch { }
			}
		}

		public static void Info (string message)
		{
			Info (message, null);
		}

		public static void Info (string message, params object[] args)
		{	
			PrintMessage (LogLevel.Information, message, args);
		}

		public static void Debug (string message)
		{
			Debug (message, null);
		}

		public static void Debug (Exception e)
		{
			Debug ("Exception was caught:\n{0}", e);
		}

		public static void Debug (string message, params object[] args)
		{
			PrintMessage (LogLevel.Debug, message, args);
		}

		public static void Warn (string message)
		{	
			Warn (message, null);
		}	

		public static void Warn (string message, params object[] args)
		{
			PrintMessage (LogLevel.Warning, message, args);
		}

		public static void Error (string message)
		{
			Error (message, null);
		}

		public static void Error (Exception e)
		{
			Error ("Exception was caught:\n{0}", e);
		}

		public static void Error (string message, params object[] args)
		{
			PrintMessage (LogLevel.Error, message, args);
		}
#endregion

#region Private Methods
		private static void PrintMessage (LogLevel level, string message, object[] args)
		{
			if ((int)currentLogLevel > (int)level)
				return;

			// Code from Banshee under the MIT/X11 License
			Console.Error.Write ("[{0} {1:00}:{2:00}:{3:00}.{4:000}] ",
			                     GetLogLevelString (level),
			                     DateTime.Now.Hour, DateTime.Now.Minute,
			                     DateTime.Now.Second, DateTime.Now.Millisecond); 

			if (args == null)
				Console.Error.WriteLine (message);
			else
				Console.Error.WriteLine (message, args);
		}

		private static string GetLogLevelString (LogLevel level)
		{
			switch (level) {
			case LogLevel.Information:
				return "Info ";
			case LogLevel.Debug:
				return "Debug";
			case LogLevel.Warning:
				return "Warn ";
			case LogLevel.Error:
				return "Error";
			}
			return String.Empty;
		}
#endregion

#region Private Fields
		private static LogLevel currentLogLevel = LogLevel.Warning;
#endregion
	}
}
