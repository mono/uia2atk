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
        public static bool MONO_UIA_UISYNCCONTEXT => GetEnvVarIntValueAsBoolOrDefault ("MONO_UIA_UISYNCCONTEXT", true);

        public static bool MONO_UIA_ENABLED => GetEnvVarIntValueAsBoolOrDefault ("MONO_UIA_ENABLED", true);

        public static string MONO_UIA_SOURCE => GetEnvVarValueOrDefault<string> ("MONO_UIA_SOURCE", string.Empty);

        public static string MONO_UIA_BRIDGE => GetEnvVarValueOrDefault<string> ("MONO_UIA_BRIDGE", string.Empty);

        public static MONO_UIA_NAVIGATION_TREE_ERR MONO_UIA_NAVIGATION_TREE_ERR =>
            GetEnvVarValueAsEnumOrDefault<MONO_UIA_NAVIGATION_TREE_ERR> ("MONO_UIA_NAVIGATION_TREE_ERR", MONO_UIA_NAVIGATION_TREE_ERR.Log);

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
