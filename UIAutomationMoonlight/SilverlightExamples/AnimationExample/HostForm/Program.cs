using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace HostForm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process process = new Process ();

            process.StartInfo.FileName = Path.Combine (@"C:\Program Files\Internet Explorer", "iexplore.exe");
            process.StartInfo.Arguments = Path.Combine(Directory.GetCurrentDirectory(), "TestPage.html");
            process.Start ();
            // We have to wait for the application before inspecting.
            System.Threading.Thread.Sleep (10000);
            Console.WriteLine("process.Id: {0}", process.Id);
            AutomationElement iexplorerElement = GetAutomationElementFromProcessId (process.Id);

            AutomationElement tabPane = TreeWalker.ContentViewWalker.GetFirstChild (iexplorerElement);
            while (tabPane != null) {
                if (tabPane.Current.Name.Contains ("AnimationExample"))
                    break;
                tabPane = TreeWalker.ContentViewWalker.GetNextSibling (tabPane);
            }

            if (tabPane == null)
                throw new NullReferenceException ("tabPane should not be null");

            // Lets search for the next Pane
            AutomationElement child = TreeWalker.ContentViewWalker.GetFirstChild (tabPane);
            while (child != null) {
                if (child.Current.ControlType == ControlType.Pane)
                    break;
                child = TreeWalker.ContentViewWalker.GetNextSibling (child);
            }
            if (child == null)
                throw new NullReferenceException("Missing pane");
            tabPane = child;

            AutomationElement silverlightWindow = TreeWalker.ContentViewWalker.GetFirstChild (tabPane);
            if (silverlightWindow == null)
                throw new NullReferenceException ("silverlightWindow should not be null");

            //We should have 3 buttons
            AutomationElement animationButton = null;
            AutomationElement invokeButton = null;
            AutomationElement widthButton = null;
            
            while (child != null) {
                Console.WriteLine("helptext:{0}", child.Current.HelpText);
                if (child.Current.HelpText == "AnimatedButton")
                    animationButton = child;
                else if (child.Current.HelpText == "InitializeButton")
                    invokeButton = child;
                else if (child.Current.HelpText == "WidthButton")
                    widthButton = child;
                child = TreeWalker.ContentViewWalker.GetNextSibling (child);
            }

            if (animationButton == null)
                throw new NullReferenceException("Missing children in Silverlight window 1 ");
            if (invokeButton == null)
                throw new NullReferenceException("Missing children in Silverlight window 2 ");
            if (widthButton == null)
                throw new NullReferenceException ("Missing children in Silverlight window 3");

            // iexplore.exe doesn't close :(
            while (!process.HasExited) 
                 process.CloseMainWindow();
        }

        static AutomationElement GetAutomationElementFromProcessId(int processId)
        {
            AutomationElement element = TreeWalker.RawViewWalker.GetFirstChild(AutomationElement.RootElement);
            while (element != null) {
                if (element.Current.Name.Contains ("Windows Internet Explorer"))
                    break;

                element = TreeWalker.RawViewWalker.GetNextSibling(element);
            }
            return element;
        }

    }
}
