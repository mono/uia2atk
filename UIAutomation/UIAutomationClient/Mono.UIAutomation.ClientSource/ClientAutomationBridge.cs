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
//     Matt Guo <matt@mattguo.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Bridge;

namespace Mono.UIAutomation.ClientSource
{
	internal class ClientAutomationBridge : IAutomationBridge
	{
		#region IAutomationBridge implementation
		public object HostProviderFromHandle (IntPtr hwnd)
		{
			return null;
		}

		public void RaiseAutomationEvent (object provider, AutomationEventArgs e)
		{
			var providerSimple = provider as IRawElementProviderSimple;
			if (providerSimple == null)
				return;
			ClientEventManager.RaiseAutomationEvent (providerSimple, e);
		}

		public void RaiseAutomationPropertyChangedEvent (object provider, AutomationPropertyChangedEventArgs e)
		{
			var providerSimple = provider as IRawElementProviderSimple;
			if (providerSimple == null)
				return;
			ClientEventManager.RaiseAutomationPropertyChangedEvent (providerSimple, e);
		}

		public void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			var providerSimple = provider as IRawElementProviderSimple;
			if (providerSimple == null)
				return;
			ClientEventManager.RaiseStructureChangedEvent (providerSimple, e);
		}

		public void Initialize ()
		{
		}

		public void Terminate ()
		{
		}

		public bool IsAccessibilityEnabled {
			get {
				return true;
			}
		}

		public bool ClientsAreListening {
			get {
				return true;
			}
		}

		#endregion
	}
}
