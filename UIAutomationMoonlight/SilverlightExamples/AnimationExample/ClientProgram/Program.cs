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
//      Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Moonlight.AnimationExample
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
			process.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), "TestPage.html");
            process.Start ();
            // We have to wait for the application before inspecting.
            System.Threading.Thread.Sleep (10000);

            AutomationElement iexplorerElement = GetAutomationElementFromProcessId (process.Id);

			if (iexplorerElement == null)
				throw new NullReferenceException ("Internet Explorer Automation Element is missing.");

			// We have the Internet Explorer Automation Element, we need to find an elmenet
			// with Name equals to "Silverlight Control", that's the container of the Silverlight 2
			// controls
			AutomationElement silverlightElement = GetSilverlightElement (iexplorerElement);

			if (silverlightElement == null)
				throw new NullReferenceException("Silverlight Element is missing.");

            // Buttons used to manipulate and generate events
			ButtonElement animationButton = GetButtonElementWithHelpText (silverlightElement, "AnimatedButton");
			ButtonElement invokeButton = GetButtonElementWithHelpText (silverlightElement, "InitializeButton");
			ButtonElement widthButton = GetButtonElementWithHelpText (silverlightElement, "WidthButton");
			ButtonElement nameButton = GetButtonElementWithHelpText (silverlightElement, "NameButton");
			ButtonElement enabledButton = GetButtonElementWithHelpText (silverlightElement, "EnabledButton");

            if (animationButton == null)
                throw new NullReferenceException ("Missing Animation Button in Silverlight.");
            if (invokeButton == null)
                throw new NullReferenceException ("Missing Invoke Button in Silverlight.");
            if (widthButton == null)
                throw new NullReferenceException ("Missing Width Button in Silverlight.");
			if (nameButton == null)
				throw new NullReferenceException("Missing Name Button in Silverlight.");
			if (enabledButton == null)
				throw new NullReferenceException("Missing Enabled Button in Silverlight.");

			// Suscribing to Property changed event
			Automation.AddAutomationPropertyChangedEventHandler(animationButton.AutomationElement,
				 TreeScope.Element,
				 /*propChangeHandler = new AutomationPropertyChangedEventHandler(*/OnPropertyChange/*)*/,
				 AutomationElement.IsEnabledProperty);

			//// Suscribing to Invoke event
			//Automation.AddAutomationEventHandler (InvokePattern.InvokedEvent,
			//    invokeButton.AutomationElement, TreeScope.Element,
			//    invokeEventHandler = new AutomationEventHandler (OnUIAutomationEvent));

			widthButton.InvokePattern.Invoke ();
			System.Threading.Thread.Sleep(10000);

			nameButton.InvokePattern.Invoke ();
			System.Threading.Thread.Sleep(10000);

			enabledButton.InvokePattern.Invoke ();
			System.Threading.Thread.Sleep(10000);

			invokeButton.InvokePattern.Invoke ();

			System.Threading.Thread.Sleep (10000);

            // FIXME: iexplore.exe doesn't close :(
            while (!process.HasExited) 
                 process.CloseMainWindow();

			Automation.RemoveAllEventHandlers ();
        }

		//static AutomationPropertyChangedEventHandler propChangeHandler;
		//static AutomationEventHandler invokeEventHandler;

		//static void OnUIAutomationEvent (object src, AutomationEventArgs e)
		//{
		//    // Make sure the element still exists. Elements such as tooltips
		//    // can disappear before the event is processed.
		//    AutomationElement sourceElement;
		//    try
		//    {
		//        sourceElement = src as AutomationElement;
		//    } catch (ElementNotAvailableException)
		//    {
		//        return;
		//    }
		//    Console.WriteLine ("event is: {0}", e.EventId.ProgrammaticName);
		//    //if (e.EventId == InvokePattern.InvokedEvent)
		//    //{
		//    //    // TODO Add handling code.
		//    //} else
		//    //{
		//    //    // TODO Handle any other events that have been subscribed to.
		//    //}
		//}

		static void OnPropertyChange(object src, AutomationPropertyChangedEventArgs e)
		{
			AutomationElement sourceElement = src as AutomationElement;
			Console.WriteLine ("source: {0} - {1}", e.EventId.ProgrammaticName, e.Property.ProgrammaticName);
			//if (e.Property == AutomationElement.IsEnabledProperty)
			//{
			//    bool enabled = (bool)e.NewValue;
			//    // TODO: Do something with the new value. 
			//    // The element that raised the event can be identified by its runtime ID property.
			//} else
			//{
			//    // TODO: Handle other property-changed events.
			//}
		}

		// Gets "Silverlight Control"
		static AutomationElement GetSilverlightElement (AutomationElement element)
		{
			AutomationElement found = null;
			NavigateAutomationElements (element, out found);
			return found;
		}

		// Recursive method to search "Silverlight Control"
		static bool NavigateAutomationElements (AutomationElement element, out AutomationElement found)
		{
			if (element.Current.Name == SilverlightControl) {
				found = element;
				return true;
			} else {
				AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild (element);
				while (child != null) {
					if (NavigateAutomationElements (child, out found))
						return true;
					child = TreeWalker.RawViewWalker.GetNextSibling (child);
				}
			}
			found = null;
			return false;
		}

		// Search Internet Explorer Automation Element
        static AutomationElement GetAutomationElementFromProcessId (int processId)
        {
            AutomationElement element = TreeWalker.RawViewWalker.GetFirstChild (AutomationElement.RootElement);
            while (element != null) {
                if (element.Current.Name.Contains ("Windows Internet Explorer"))
                    break;

                element = TreeWalker.RawViewWalker.GetNextSibling(element);
            }
            return element;
        }

		// Search Internet Explorer Automation Element
		static ButtonElement GetButtonElementWithHelpText(AutomationElement parent, string helpText)
		{
			ButtonElement buttonElement = null;
			AutomationElement element = TreeWalker.RawViewWalker.GetFirstChild (parent);
			while (element != null) {
				if (element.Current.HelpText == helpText) {
					buttonElement = new ButtonElement {
						AutomationElement = element, 
						InvokePattern = (InvokePattern) element.GetCurrentPattern (InvokePatternIdentifiers.Pattern) };
					break;
				}

				element = TreeWalker.RawViewWalker.GetNextSibling (element);
			}

			return buttonElement;
		}

		static readonly string SilverlightControl = "Silverlight Control";

    }

	class ButtonElement
	{
		public ButtonElement ()
		{
		}

		public AutomationElement AutomationElement { 
			get;
			set;
		}

		public InvokePattern InvokePattern {
			get;
			set;
		}
	}
}
