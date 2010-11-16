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
using System.Threading;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Source;
using Mono.UIAutomation.Services;

namespace Mono.UIAutomation.ClientSource
{
	internal class ClientAutomationSource : IAutomationSource
	{
		private static ClientAutomationSource instance = null;
		private static object staticLock = new object ();
		public static ClientAutomationSource Instance {
			get {
				if (instance == null)
					lock (staticLock)
						if (instance == null)
							instance = new ClientAutomationSource ();
				return instance;
			}
		}

		#region IAutomationSource implementation
		public event EventHandler RootElementsChanged;

		public void Initialize ()
		{
		}

		public IElement [] GetRootElements ()
		{
			return new IElement [0];
		}

		public IElement GetFocusedElement ()
		{
			return null;
		}

		public IElement GetElementFromHandle (IntPtr handle)
		{
			return null;
		}

		public void AddAutomationEventHandler (AutomationEvent eventId, IElement element, TreeScope scope, AutomationEventHandler eventHandler)
		{
			if (element == null)
				// elements from local providers are not descendants of the RootElement.
				return;
			ClientElement clientElement = element as ClientElement;
			if (clientElement == null) {
				Log.Error ("[ClientAutomationSource.AddAutomationEventHandler] Not ClientElement");
				return;
			}
			ClientEventManager.AddAutomationEventHandler (eventId,
				clientElement.Provider, scope, eventHandler);
		}

		public void AddAutomationPropertyChangedEventHandler (IElement element, TreeScope scope, AutomationPropertyChangedEventHandler eventHandler, AutomationProperty[] properties)
		{
			if (element == null)
				return;
			ClientElement clientElement = element as ClientElement;
			if (clientElement == null) {
				Log.Error ("[ClientAutomationSource.AddAutomationPropertyChangedEventHandler] Not ClientElement");
				return;
			}
			int [] propertyIds = Array.ConvertAll (properties, p => p.Id);
			ClientEventManager.AddAutomationPropertyChangedEventHandler (
				clientElement.Provider, scope, eventHandler, propertyIds);
		}

		public void AddStructureChangedEventHandler (IElement element, TreeScope scope, StructureChangedEventHandler eventHandler)
		{
			if (element == null)
				return;
			ClientElement clientElement = element as ClientElement;
			if (clientElement == null) {
				Log.Error ("[ClientAutomationSource.AddStructureChangedEventHandler] Not ClientElement");
				return;
			}
			ClientEventManager.AddStructureChangedEventHandler (clientElement.Provider,
				scope, eventHandler);
		}

		public void AddAutomationFocusChangedEventHandler (FocusChangedEventHandler eventHandler)
		{
			// client provider never fires FocusChangedEvent.
			return;
		}

		public void RemoveAutomationEventHandler (AutomationEvent eventId, IElement element, AutomationEventHandler eventHandler)
		{
			if (element == null)
				return;
			ClientElement clientElement = element as ClientElement;
			if (clientElement == null) {
				Log.Error ("[ClientAutomationSource.RemoveAutomationEventHandler] Not ClientElement");
				return;
			}
			ClientEventManager.RemoveAutomationEventHandler (eventId,
				clientElement.Provider, eventHandler);
		}

		public void RemoveAutomationPropertyChangedEventHandler (IElement element, AutomationPropertyChangedEventHandler eventHandler)
		{
			if (element == null)
				return;
			ClientElement clientElement = element as ClientElement;
			if (clientElement == null) {
				Log.Error ("[ClientAutomationSource.RemoveAutomationPropertyChangedEventHandler] Not ClientElement");
				return;
			}
			ClientEventManager.RemoveAutomationPropertyChangedEventHandler (
				clientElement.Provider, eventHandler);
		}

		public void RemoveStructureChangedEventHandler (IElement element, StructureChangedEventHandler eventHandler)
		{
			if (element == null)
				return;
			ClientElement clientElement = element as ClientElement;
			if (clientElement == null) {
				Log.Error ("[ClientAutomationSource.RemoveStructureChangedEventHandler] Not ClientElement");
				return;
			}
			ClientEventManager.RemoveStructureChangedEventHandler (
				clientElement.Provider, eventHandler);
		}

		public void RemoveAutomationFocusChangedEventHandler (FocusChangedEventHandler eventHandler)
		{
			// client provider never fires FocusChangedEvent.
			return;
		}

		public void RemoveAllEventHandlers ()
		{
			ClientEventManager.RemoveAllEventHandlers ();
		}

		public bool IsAccessibilityEnabled {
			get {
				return true;
			}
		}

		#endregion

		public ClientElement GetOrCreateElement (IRawElementProviderSimple provider)
		{
			if (provider == null)
				return null;
			//?????
			return new ClientElement (this, provider);
		}
	}
}