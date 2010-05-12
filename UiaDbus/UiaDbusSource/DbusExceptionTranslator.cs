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
//  Matt Guo <matt@mattguo.com>
// 

using System;
using System.Windows.Automation;
using System.Reflection;

namespace Mono.UIAutomation.UiaDbusSource
{
	// This class recovers the exception raised at provider end.
	// 
	// Note:
	// 1. A NDesk.DBus bug: an Exeception without namespace cannot be passed over dbus.
	// 2. We can only handle those Execeptions which have the constructor of "ctor (string msg)",
	// 		where msg is the Message property of the exception.
	//
	public class DbusExceptionTranslator
	{
		static DbusExceptionTranslator ()
		{
			dbusErrorList = new string [] {
				"org.freedesktop.DBus.Error.Failed",
				"org.freedesktop.DBus.Error.NoMemory",
				"org.freedesktop.DBus.Error.ServiceUnknown",
				"org.freedesktop.DBus.Error.NameHasNoOwner",
				"org.freedesktop.DBus.Error.NoReply",
				"org.freedesktop.DBus.Error.IOError",
				"org.freedesktop.DBus.Error.BadAddress",
				"org.freedesktop.DBus.Error.NotSupported",
				"org.freedesktop.DBus.Error.LimitsExceeded",
				"org.freedesktop.DBus.Error.AccessDenied",
				"org.freedesktop.DBus.Error.AuthFailed",
				"org.freedesktop.DBus.Error.NoServer",
				"org.freedesktop.DBus.Error.Timeout",
				"org.freedesktop.DBus.Error.NoNetwork",
				"org.freedesktop.DBus.Error.AddressInUse",
				"org.freedesktop.DBus.Error.Disconnected",
				"org.freedesktop.DBus.Error.FileNotFound",
				"org.freedesktop.DBus.Error.UnknownMethod",
				"org.freedesktop.DBus.Error.TimedOut",
				"org.freedesktop.DBus.Error.MatchRuleNotFound",
				"org.freedesktop.DBus.Error.MatchRuleInvalid",
				"org.freedesktop.DBus.Error.Spawn.ExecFailed",
				"org.freedesktop.DBus.Error.Spawn.ForkFailed",
				"org.freedesktop.DBus.Error.Spawn.ChildExited",
				"org.freedesktop.DBus.Error.Spawn.ChildSignaled",
				"org.freedesktop.DBus.Error.Spawn.Failed",
				"org.freedesktop.DBus.Error.UnixProcessIdUnknown",
				"org.freedesktop.DBus.Error.InvalidSignature",
				"org.freedesktop.DBus.Error.SELinuxSecurityContextUnknown"
			};
			Array.Sort (dbusErrorList);
		}

		public static Exception Translate (Exception ex)
		{
			if (ex == null || ex.GetType () != typeof (Exception))
				return ex;
			Exception ret = BuildException (ex.Message);
			return (ret != null) ? ret : ex;
		}

		private static Exception BuildException (string exceptionInfo)
		{
			int colonIndex = exceptionInfo.IndexOf (": ");
			if (colonIndex == -1)
				return null;
			string errorName = exceptionInfo.Substring (0, colonIndex);
			string errorMessage = exceptionInfo.Substring (colonIndex + 2);

			Exception ret = null;
			if (errorName == dbusInvalidArgsError)
				ret = new ArgumentException (exceptionInfo);
			if (Array.BinarySearch (dbusErrorList, errorName) >= 0) {
				ret = new ElementNotAvailableException (exceptionInfo);
			} else {
				try {
					Type type = GetType (errorName);
					if (type != null)
						ret = Activator.CreateInstance (type, errorMessage) as Exception;
				} catch {
					ret = null;
				}
			}
			return ret;
		}

		private static Type GetType (string typeName)
		{
			Type type = Type.GetType (typeName);
			if (type != null)
				return type;
			Assembly [] assemblies = AppDomain.CurrentDomain.GetAssemblies ();
			for (int i = 0; i != assemblies.Length; i++) {
				type = assemblies [i].GetType (typeName);
				if (type != null)
					return type;
			}
			return null;
		}

		private static string [] dbusErrorList;
		private const string dbusInvalidArgsError = "org.freedesktop.DBus.Error.InvalidArgs";
	}
}
