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
// Copyright (c) 2010 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Matt Guo <matt@mattguo.com>
// 

using System;
using System.Reflection;
using System.IO;

namespace System.Windows.Automation
{
	public static class ClientSettings
	{
		public static void RegisterClientSideProviderAssembly (AssemblyName assemblyName)
		{
			if (assemblyName == null)
				throw new ArgumentNullException ("assemblyName");

			Assembly assembly = null;
			// TODO, wrap exception messages into Catalog.GetString
			try {
				assembly = Assembly.Load (assemblyName);
			} catch (FileNotFoundException) {
				throw new ProxyAssemblyNotLoadedException (
					string.Format ("'{0}' assembly not found.", assemblyName));
			}

			string typeName = assemblyName.Name + ".UIAutomationClientSideProviders";
			Type type = assembly.GetType(typeName);
			if (type == null)
				throw new ProxyAssemblyNotLoadedException (
					string.Format ("Cannot find type {0} in assembly {1}",
						typeName, assemblyName));

			FieldInfo field = type.GetField("ClientSideProviderDescriptionTable", BindingFlags.Public | BindingFlags.Static);
			if ((field == null) || (field.FieldType != typeof(ClientSideProviderDescription[])))
				throw new ProxyAssemblyNotLoadedException (
					string.Format ("Cannot find Register method on type {0} in assembly {1}",
						typeName, assemblyName));

			var description = field.GetValue(null) as ClientSideProviderDescription [];
			if (description != null)
				ClientSettings.RegisterClientSideProviders (description);
		}

		public static void RegisterClientSideProviders (
			ClientSideProviderDescription [] clientSideProviderDescription)
		{
			throw new NotImplementedException ();
		}
	}
}
