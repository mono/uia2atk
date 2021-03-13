// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// Copyright (c) 2020 AxxonSoft (http://axxonsoft.com)
//
// Authors:
//   Nikita Voronchev <nikita.voronchev@ru.axxonsoft.com>
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace Mono.UIAutomation.Helpers
{
    public enum MONO_UIA_NAVIGATION_TREE_ERR
    {
        Log,
        Exception
    }

    public static class EnvironmentVaribles
    {
        /* 
        Usage: 
            export MONO_UIA_UISYNCCONTEXT={0,1}

        Defaule value:
            "1" (enabled)
        
        Description:
            Enables or disables UI SynchronizationContext. If disabled, Provider
            accesses to Control directly from non-UI thread. This is fast but dangerous.
            Don't disable it in production tests.
        */
        public static bool MONO_UIA_UISYNCCONTEXT => GetEnvVarIntValueAsBoolOrDefault ("MONO_UIA_UISYNCCONTEXT", true);

        /* 
        Usage: 
            export MONO_UIA_ENABLED={0,1}

        Defaule value:
            "1" (enabled)
        
        Description:
            Enables or disables UIA API.
        */
        public static bool MONO_UIA_ENABLED => GetEnvVarIntValueAsBoolOrDefault ("MONO_UIA_ENABLED", true);

        /* 
        Usage: 
            export MONO_UIA_SOURCE="<list of assemblies implementing `IAutomationSource`>"

        Defaule value:
            <all available assemblies>
        
        Description:
            To use D-Bus source only one can set:
            
            export MONO_UIA_SOURCE="UiaDbusSource, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4ceacb585d99812"
        */
        public static string MONO_UIA_SOURCE => GetEnvVarValueOrDefault<string> ("MONO_UIA_SOURCE", string.Empty);

        /* 
        Usage: 
            export MONO_UIA_BRIDGE="<list of assemblies implementing `IAutomationBridge`>"

        Defaule value:
            <all available assemblies>
        
        Description:
            To use D-Bus source only one can set:
            
            export MONO_UIA_BRIDGE="UiaDbusBridge, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4ceacb585d99812"
        */
        public static string MONO_UIA_BRIDGE => GetEnvVarValueOrDefault<string> ("MONO_UIA_BRIDGE", string.Empty);

        /* 
        Usage: 
            export MONO_UIA_NAVIGATION_TREE_ERR={log|exception}

        Defaule value:
            "log"
        
        Description:
            Only 'log' or throw 'exception' in case of internal error related with Provider tree navigation.
        */
        public static MONO_UIA_NAVIGATION_TREE_ERR MONO_UIA_NAVIGATION_TREE_ERR =>
            GetEnvVarValueAsEnumOrDefault<MONO_UIA_NAVIGATION_TREE_ERR> ("MONO_UIA_NAVIGATION_TREE_ERR", MONO_UIA_NAVIGATION_TREE_ERR.Log);

        /* 
        Usage: 
            export MONO_UIA_NAVIGATION_TREE_ERR={Information|Debug|Warning|Error}

        Defaule value:
            "Information"
        
        Description:
            Stdout log level.
        */
        public static string MONO_UIA_LOG_LEVEL => Environment.GetEnvironmentVariable ("MONO_UIA_LOG_LEVEL");

        private static bool GetEnvVarIntValueAsBoolOrDefault (string envVarName, bool defaultValue)
        {
            var defaultIntValue = Convert.ToInt32 (defaultValue);

            var intVal = GetEnvVarValueOrDefault<int> (envVarName, defaultIntValue);
            if (intVal >= 0)
                return Convert.ToBoolean (intVal);

            var msg = $"[EnvironmentVaribles]: Environment variable '{envVarName}' (currently set to '{intVal}')"
                + $" may be set to '0' or '1' only. Unset variable is equal to '{defaultIntValue}'.";
            Console.WriteLine (msg);
            throw new Exception (msg);
        }

        private static T GetEnvVarValueOrDefault<T> (string envVarName, T defaultValue)
        {
            var envValue = Environment.GetEnvironmentVariable (envVarName);
            var value = string.IsNullOrWhiteSpace(envValue)
                ? defaultValue
                : (T) Convert.ChangeType (envValue, typeof(T));
            return value;
        }

        private static T GetEnvVarValueAsEnumOrDefault<T> (string envVarName, T defaultValue ) where T : struct
        {
            var envValue = Environment.GetEnvironmentVariable (envVarName);
            if (string.IsNullOrWhiteSpace (envValue))
                return defaultValue;

            if (Enum.TryParse (envValue, true, out T value))
                return value;
            else
                throw new Exception ($"[EnvironmentVaribles]: Cann't convert {envVarName}='{envValue}' to enum {typeof(T)}={{{string.Join(',', Enum.GetNames (typeof(T)))}}}");
        }
    }
}
