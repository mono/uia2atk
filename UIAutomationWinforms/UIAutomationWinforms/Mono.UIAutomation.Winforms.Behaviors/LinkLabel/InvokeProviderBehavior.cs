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
using System.Reflection;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.LinkLabel;

namespace Mono.UIAutomation.Winforms.Behaviors.LinkLabel
{

	internal class InvokeProviderBehavior 
		: ProviderBehavior, IInvokeProvider, IHypertext
	{
		#region Private Members

		private SWF.LinkLabel linkLabel;

		#endregion
		
		#region Constructor
		
		public InvokeProviderBehavior (FragmentControlProvider provider)
			: base (provider)
		{
			this.linkLabel = (SWF.LinkLabel) provider.Control;
		}
		
		#endregion

		#region IProviderBehavior Interface
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   null);
		}

		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
			                   new InvokePatternInvokedEvent (Provider));
		}
		
		public override AutomationPattern ProviderPattern { 
			get { return InvokePatternIdentifiers.Pattern; }
		}
		
		#endregion
		
		#region IInvokeProvider Members
		
		public void Invoke ()
		{
			Invoke (0);
		}
		
		#endregion
		
		#region IHypertext Specialization
		
		public int NumberOfLinks { 
			get { return linkLabel.Links.Count; }
		}
		
		public int Start (int index) 
		{
			if (index >= linkLabel.Links.Count || index < 0)
				return -1;
			else
				return linkLabel.Links [index].Start;
		}
		
		public int Length (int index)
		{
			if (index >= linkLabel.Links.Count || index < 0)
				return -1;
			else
				return linkLabel.Links [index].Length;
		}
		
		public string Uri (int index)
		{
			if (index >= linkLabel.Links.Count || index < 0)
				return null;
			else
				return linkLabel.Links [index].LinkData as string;
		}
		
		public bool Enabled (int index)
		{
			if (!linkLabel.Enabled || index >= linkLabel.Links.Count || index < 0)
				return false;
			else
				return linkLabel.Links [index].Enabled;
		}
		
		public void Invoke (int index)
		{
			if (!Enabled (index))
				throw new ElementNotEnabledException ();
			
			PerformClick (index);
		}
		
		#endregion
		
		#region Private Methods
		
		private void PerformClick (int index)
		{
			if (index >= linkLabel.Links.Count || index < 0)
				return;

			if (linkLabel.InvokeRequired) {
				linkLabel.BeginInvoke (new PerformClickDelegate (PerformClick),
				                       new object [] { index });
				return;
			}
			
			MethodInfo methodInfo = typeof (SWF.LinkLabel).GetMethod ("OnLinkClicked",
			                                                          BindingFlags.InvokeMethod
			                                                          | BindingFlags.NonPublic
			                                                          | BindingFlags.Instance);
			Action<SWF.LinkLabel, SWF.LinkLabelLinkClickedEventArgs> invokeMethod
				= (Action<SWF.LinkLabel, SWF.LinkLabelLinkClickedEventArgs>) Delegate.CreateDelegate 
					(typeof (Action<SWF.LinkLabel, SWF.LinkLabelLinkClickedEventArgs>),
					 methodInfo);

			SWF.LinkLabelLinkClickedEventArgs args 
				= new SWF.LinkLabelLinkClickedEventArgs (linkLabel.Links [index],
				                                         SWF.MouseButtons.Left);
			invokeMethod (linkLabel, args);
		}
		
		#endregion
	}
	
	delegate void PerformClickDelegate (int index);
}
