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
using System.Collections.Generic;
using System.Reflection;

using Mono.UIAutomation.Source;
using Mono.UIAutomation.Services;

namespace System.Windows.Automation
{
	internal static class SourceManager
	{
		private const string AtspiUiaSourceAssembly =
			"AtspiUiaSource, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4ceacb585d99812";
		private const string UiaDbusSourceAssembly =
			"UiaDbusSource, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4ceacb585d99812";
		private static IAutomationSource [] sources = null;
		private static object sourcesLock = new object ();

		internal static IAutomationSource [] GetAutomationSources ()
		{
			if (sources == null) {
				lock (sourcesLock) {
					if (sources == null) {
						var sourcesList = new List<IAutomationSource> ();

						// Let MONO_UIA_SOURCE env var override default source
						string sourceAssemblyNames =
							Environment.GetEnvironmentVariable ("MONO_UIA_SOURCE");

						if (string.IsNullOrEmpty (sourceAssemblyNames))
							sourceAssemblyNames =
								UiaDbusSourceAssembly + ";" + AtspiUiaSourceAssembly;

						foreach (string sourceAssembly in sourceAssemblyNames.Split (';')) {
							if (string.IsNullOrEmpty (sourceAssembly))
								continue;
							IAutomationSource source = GetAutomationSource (sourceAssembly);
							if (source != null)
								sourcesList.Add (source);
						}
						sources = sourcesList.ToArray ();
					}
				}
			}
			return sources;
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

		private static IAutomationSource GetAutomationSource (string sourceAssemblyName)
		{
			Assembly sourceAssembly = null;
			try {
				sourceAssembly = Assembly.Load (sourceAssemblyName);
			} catch (Exception e){
				Log.Warn (string.Format ("Error loading UIA source ({0}): {1}",
				                                  sourceAssemblyName,
				                                  e));
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
				Log.Error ("No UIA source found in assembly: " +
				                   sourceAssemblyName);
				return null;
			}

			try {
				IAutomationSource source
					= (IAutomationSource) Activator.CreateInstance (sourceType);
				if (!source.IsAccessibilityEnabled)
					return null;

				source.Initialize ();
				return source;
			} catch (Exception e) {
				Log.Error ("Failed to load UIA source: " + e);
				return null;
			}
		}
	}
}
