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
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Automation;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using NUnit.Framework;

// NOTE: This test is not subclassing BaseTest because we are implementing
// this provider in a different way, DataGrid in Vista is Table, however
// in our implementation is DataGrid supporting same patterns as in 
// ListViewProvider.View=Details, instead this test is to make sure our 
// IValue implementation is the valid implementation.
namespace MonoTests.Mono.UIAutomation.Winforms.Client
{
    [TestFixture]
    [Description ("Tests SWF.DataGrid as ControlType.Grid")]
    public class DataGridTest
    {
        [TestFixtureSetUp]
        public void TestSetup()
        {
            processes = new List<Process>();
        }

        [TestFixtureTearDown]
        public void TestTeardDown()
        {
            foreach (Process oldProcess in processes) {
                if (!oldProcess.HasExited)
                    oldProcess.CloseMainWindow ();
            }
        }

        [Test]
        public void ReadonlyDataBinding ()
        {
            ValuePatternTest (0);
        }

        [Test]
        public void ReadWriteDataBinding ()
        {
            ValuePatternTest (1);
        }

        private void ValuePatternTest (int option)
        {
            Process process = RunProcess (option);

            AutomationElement formElement = GetAutomationElementFromProcessId (process.Id);
            Assert.IsNotNull (formElement, "Form Automation Element missing.");

            AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild (formElement);
            Assert.AreEqual (child.Current.ControlType, ControlType.Table, "DataGrid should be found");

            while (child != null) {
                if (child.Current.ControlType == ControlType.Table) {
                    foreach (AutomationElement customElement in GetCustomElements (child))
                        VerifyValuePattern(customElement);
                }
                child = TreeWalker.RawViewWalker.GetNextSibling (child);
            }
        }

        private List<AutomationElement> GetCustomElements (AutomationElement element)
        {
            List<AutomationElement> collection = new List<AutomationElement> ();

            AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild (element);
            Assert.IsNotNull (child, "We should have children.");
            while (child != null) {
                if (child.Current.ControlType == ControlType.Custom)
                    collection.Add (child);

                child = TreeWalker.RawViewWalker.GetNextSibling (child);
            }

            return collection;
        }

        private AutomationElement GetAutomationElementFromProcessId (int processId)
        {
            AutomationElement element = TreeWalker.RawViewWalker.GetFirstChild (AutomationElement.RootElement);
            while (element != null) {
                if (element.Current.ProcessId == processId)
                    break;

                element = TreeWalker.RawViewWalker.GetNextSibling(element);
            }
            return element;
        }

        private static void VerifyValuePattern (AutomationElement element)
        {
            object patternObj;
            if (!element.TryGetCurrentPattern (ValuePattern.Pattern, out patternObj))
                return;

            ValuePattern pattern = patternObj as ValuePattern;
            try {
                if (!pattern.Current.IsReadOnly) {
                    string oldValue = pattern.Current.Value;
                    pattern.SetValue ("hello world!");
                    // This test fails and confirms that we should implement IValueProvider
                    // but return IsReadOnly = true
                    Assert.AreEqual ("hello world!", pattern.Current.Value, "Value not set even when IsReadOnly is false");
                }
            // This is weird, it should not throw any excepiont but: 
            // ElementNotEnabledException, ArgumentException or InvalidDataException
            } catch (System.Runtime.InteropServices.COMException) { }
        }

        private Process RunProcess(int option)
        {
            Process process = new Process ();
            processes.Add (process);

            process.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), "datagrid.exe");
            process.StartInfo.Arguments = option.ToString ();
            process.Start ();
            // We have to wait for the application before inspecting.
            System.Threading.Thread.Sleep (5000);
            return process;
        }

        private List<Process> processes;
    }
}
