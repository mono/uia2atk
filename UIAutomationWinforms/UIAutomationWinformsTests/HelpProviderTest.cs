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
using System.ComponentModel;
using SD = System.Drawing;
using System.Runtime.InteropServices;
using System.Reflection;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{	
	// This test includes source from:
	// - http://forge.novell.com/mailman/private/mono-a11y/2008-September/000100.html and 
	// - https://bugzilla.novell.com/show_bug.cgi?id=412849

	[TestFixture]
	public class HelpProviderTest : BaseProviderTest
	{
		#region Basic Tests


		[Test]
		public void BasicPropertiesTest ()
		{
			SWF.HelpProvider swfHelpProvider = new SWF.HelpProvider ();
			
			SWF.Button swfButton = new SWF.Button ();
			swfButton.Location = new SD.Point (3, 3);
			swfButton.Size = new SD.Size (272, 72);
			swfButton.Text = "With help";
			
			SWF.Button swfButtonNoHelp = new SWF.Button ();
			swfButtonNoHelp.Location = new SD.Point (3, 30);
			swfButtonNoHelp.Size = new SD.Size (272, 72);
			swfButtonNoHelp.Text = "No help";
			
			//We have to use an event to fake the user click
			swfButton.Click += new System.EventHandler (OnControlClicked);

			swfHelpProvider.SetShowHelp (swfButton, true);
			swfHelpProvider.SetHelpString (swfButton, "I'm showing a button tooltip.");
			
			Form.Controls.Add (swfButton);
			Form.Controls.Add (swfButtonNoHelp);
			
			//Testing ToolTipOpenedEvent
			bridge.ResetEventLists ();
			swfButton.PerformClick (); //Clicking the button will fake the event!

			StructureChangedEventTuple eventTuple 
				= bridge.GetStructureChangedEventAt (0, StructureChangeType.ChildAdded);
			Assert.IsNotNull (eventTuple, "GetAutomationEventAt (0)");
			
			//We have the HelpProvider!
			IRawElementProviderFragment helpProvider = eventTuple.provider as IRawElementProviderFragment;

			Console.WriteLine ("element: {0}", helpProvider.GetType ());
			
			Assert.IsNotNull (helpProvider, "helpProvider is null");
			
			TestProperty (helpProvider,
			              AutomationElementIdentifiers.IsControlElementProperty,
			              true);
			
			TestProperty (helpProvider,
			              AutomationElementIdentifiers.IsContentElementProperty,
			              false);			

			TestProperty (helpProvider,
			              AutomationElementIdentifiers.HelpTextProperty,
			              null);

			TestProperty (helpProvider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "tool tip");
			
			TestProperty (helpProvider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.ToolTip.Id);
			
			TestProperty (helpProvider,
			              AutomationElementIdentifiers.LabeledByProperty,
			              null);
			
			TestProperty (helpProvider,
			              AutomationElementIdentifiers.NameProperty,
			              swfHelpProvider.GetHelpString (swfButton));
			
			TestProperty (helpProvider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              false);
			
			TestProperty (helpProvider,
			              AutomationElementIdentifiers.ClickablePointProperty,
			              null);

			// TODO: How to allow it?
//			bridge.ResetEventLists ();
//
//			swfButtonNoHelp.PerformClick (); //Clicking this button will close the tooltip
//			
//			Assert.AreEqual (1,
//			                 bridge.GetAutomationEventCount (InvokePatternIdentifiers.InvokedEvent),
//			                 "AutomationElementIdentifiers.InvokedEvent");
//	
//			Assert.IsNotNull (bridge.GetStructureChangedEventAt (0, StructureChangeType.ChildRemoved),
//			                 "AutomationElementIdentifiers.ChildRemoved");
		}
		
		#endregion

		#region BaseProviderTest Overrides
		
		protected override SWF.Control GetControlInstance ()
		{
			return null;
		}

		protected override bool IsContentElement {
			get { return false; }
		}
		
		#endregion
		
		#region Structures and enum to fake WndPrc message
		
		//Ripped of from: mcs/class/Managed.Windows.Forms/System.Windows.Forms
		enum Msg {
			WM_HELP = 0x0053
		}

		//Information provided by Ivan Zlatev: https://bugzilla.novell.com/show_bug.cgi?id=412849
		[StructLayout (LayoutKind.Sequential)]
		struct HELPINFO {
				internal uint cbSize;
				internal int iContextType;
				internal int iCtrlId;
				internal IntPtr hItemHandle;
				internal uint dwContextId;
				internal POINT MousePos;
		}

		[StructLayout (LayoutKind.Sequential)]
		struct POINT {
#pragma warning disable 414
				int x;
				int y;
				
				public POINT (int x, int y)
				{
					this.x = x;
					this.y = y;
				}
#pragma warning restore 414
		}
		
		#endregion
		
		#region Private Methods used to Fake HelpProvider
		
		private void OnControlClicked (object sender, EventArgs args)
		{
			ShowHelpInControl (sender as SWF.Control);
		}
		
		private void ShowHelpInControl (SWF.Control ctrl) 
		{		
			MethodInfo methodInfo = ctrl.GetType ().GetMethod ("WndProc",
			                                                   BindingFlags.InvokeMethod
			                                                   | BindingFlags.NonPublic
			                                                   | BindingFlags.Instance);
			HELPINFO info = new HELPINFO ();

			SD.Rectangle rectangle = ctrl.TopLevelControl.RectangleToScreen (ctrl.Bounds);
			
			info.MousePos = new POINT (rectangle.X + 5, 
			                           rectangle.Y + 5);
			
			IntPtr ptr = Marshal.AllocHGlobal (Marshal.SizeOf (typeof (HELPINFO)));
			Marshal.StructureToPtr (info, ptr, false);
			
			SWF.Message message = SWF.Message.Create (ctrl.Handle,
			                                          (int) Msg.WM_HELP, 
			                                          IntPtr.Zero, 
			                                          ptr);
			
			methodInfo.Invoke (ctrl, new object[] { message });
		}
		
		#endregion
	}
}
