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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

using Mono.UIAutomation.Source;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.ClientSource;

namespace System.Windows.Automation
{
	internal static class SourceManager
	{
		private const string UIA_ENABLED_ENV_VAR = "MONO_UIA_ENABLED";
		private const string SOURCE_ASSEMBLY_NAMES_ENV_VAR = "MONO_UIA_SOURCE";
		private const char SOURCE_ASSEMBLY_NAMES_DELIM = ';';

		private const string AtspiUiaSourceAssembly =
			"AtspiUiaSource, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4ceacb585d99812";
		private const string UiaDbusSourceAssembly =
			"UiaDbusSource, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4ceacb585d99812";

		private static IAutomationSource [] sources = null;
		private static object sourcesLock = new object ();

		internal static IAutomationSource [] GetAutomationSources ()
		{
			bool isUiaEnabled = true;
			
			var isUiaEnabledEnvVar = Environment.GetEnvironmentVariable (UIA_ENABLED_ENV_VAR);
			if (!string.IsNullOrEmpty(isUiaEnabledEnvVar)) {
				if (isUiaEnabledEnvVar == "0")
					isUiaEnabled = false;
				else if (isUiaEnabledEnvVar != "1")
					throw new Exception($"The environment varianble '{UIA_ENABLED_ENV_VAR}' may be equal to '0' and '1' only, or may be empty or undefined.");
			}

			if (!isUiaEnabled) {
				Console.WriteLine ($"No one UIA source is loaded: {UIA_ENABLED_ENV_VAR}='{isUiaEnabledEnvVar}'");
				return new IAutomationSource [0];
			}

			if (sources == null) {
				lock (sourcesLock) {
					if (sources == null) {
						// Let MONO_UIA_SOURCE env var override default source
						List<string> sourceAssemblyNames =
							(Environment.GetEnvironmentVariable (SOURCE_ASSEMBLY_NAMES_ENV_VAR) ?? string.Empty)
							.Split (SOURCE_ASSEMBLY_NAMES_DELIM)
							.Where (name => !string.IsNullOrEmpty(name))
							.ToList ();

						if (sourceAssemblyNames.Count == 0)
							sourceAssemblyNames.AddRange(
								new [] { UiaDbusSourceAssembly, AtspiUiaSourceAssembly });

						var sourcesList =
							sourceAssemblyNames.Select (name => (name: name, src: GetAutomationSource (name)))
							.Where (x => x.src != null)
							.ToList ();

						sourcesList.Add ((name: "ClientAutomationSource", src: ClientAutomationSource.Instance));

						var loadedsourcesMsg = new List<string> { "Loaded UIA sources:" };
						loadedsourcesMsg.AddRange (sourcesList.Select (x => x.name));
						loadedsourcesMsg.Add ($"* To disable UIA sources loading set environment variable {UIA_ENABLED_ENV_VAR} to '0'. *");
						Console.WriteLine (string.Join (Environment.NewLine + "  ", loadedsourcesMsg));

						sources = sourcesList.Select (x => x.src).ToArray ();
					}
				}
			}

			return sources;
		}

		private static IAutomationSource GetAutomationSource (string sourceAssemblyName)
		{
			Assembly sourceAssembly = null;
			try {
				sourceAssembly = Assembly.Load (sourceAssemblyName);
			} catch (Exception e){
				Log.Warn (string.Format ("Error loading UIA source ({0}): {1}", sourceAssemblyName, e));
				return null;
			}
			Type sourceType = null;

			// Quickie inefficent implementation
			Type sourceInterfaceType = typeof (IAutomationSource);
			foreach (Type type in sourceAssembly.GetTypes ()) {
				if (sourceInterfaceType.IsAssignableFrom (type)) {
					sourceType = type;
					break;
				}
			}

			if (sourceType == null) {
				Log.Error ("No UIA source found in assembly: " + sourceAssemblyName);
				return null;
			}

		    try {
				var source = (IAutomationSource) Activator.CreateInstance (sourceType);
				if (!source.IsAccessibilityEnabled)
					return null;
				source.Initialize ();
				return source;
			} catch (Exception e) {
				Log.Error (string.Format("Failed to load UIA source from {0}: {1}", sourceAssemblyName, e));
				return null;
			}
		}

		// TODO: Rename to GetAutomationElement
		// Note: this method could be invoked by multiple threads simultaneously
		internal static AutomationElement GetOrCreateAutomationElement (IElement sourceElement)
		{
			if (sourceElement == null)
				return null;
			return new AutomationElement (sourceElement);
		}

		// TODO: Rename to GetAutomationElements
		internal static AutomationElement [] GetOrCreateAutomationElements (IElement [] sourceElements)
		{
			AutomationElement [] ret = new AutomationElement [sourceElements.Length];
			for (int i = 0; i < sourceElements.Length; i++)
				ret [i] = GetOrCreateAutomationElement (sourceElements [i]);
			return ret;
		}
	}
}
