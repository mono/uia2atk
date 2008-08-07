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
using System.Collections.Generic;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms.Navigation
{

	internal abstract class SimpleNavigation : INavigation
	{

		#region Constructor
		
		protected SimpleNavigation (IRawElementProviderSimple provider)
		{
			this.simpleProvider = (SimpleControlProvider) provider;
		}

		#endregion
		
		#region INavigation Interface
		
		public virtual IRawElementProviderSimple Provider {
			get { return simpleProvider; }
		}

		public virtual void Initialize ()
		{
		}

		public virtual void Terminate ()
		{
			if (simpleProvider != null)
				simpleProvider.Terminate ();
		}		
		
		public abstract IRawElementProviderFragment Navigate (NavigateDirection direction);

		#endregion
		
		#region Private Methods

		private Control GetFirstValidChildControl (Control container)
		{
			if (container == null || container.Controls.Count == 0)
				return null;
			
			Control firstChild = container.Controls [0];
			
			if (ErrorProviderProvider.InstancesTracker.IsControlFromErrorProvider (firstChild)) {
				if (ErrorProviderProvider.InstancesTracker.IsFirstControlFromErrorProvider (firstChild))
					return firstChild;
				else
					firstChild = GetNextValidSiblingControl (container, firstChild);
			}
			
			return firstChild;
		}
		
		private Control GetLastValidChildControl (Control container)
		{
			if (container == null || container.Controls.Count == 0)
				return null;
			
			Control lastChild = container.Controls [container.Controls.Count - 1];
			
			if (ErrorProviderProvider.InstancesTracker.IsControlFromErrorProvider (lastChild)) {
				if (ErrorProviderProvider.InstancesTracker.IsFirstControlFromErrorProvider (lastChild))
					return lastChild;
				else
					lastChild = GetPreviousValidSiblingControl (container, lastChild);
			}
			
			return lastChild;
		}

		private Control GetNextValidSiblingControl (Control container, Control sibling)
		{
			if (sibling == null || container == null) {
				Console.WriteLine ("IS NULL: GetNextValidSiblingControl: {0} - {1}",
				                   container == null, sibling == null);
				return null;
			}
			
			int next = container.Controls.IndexOf (sibling) + 1;
			if (next >= container.Controls.Count)
				return null;
			else {
				Control nextSibling = container.Controls [next];
				if (ErrorProviderProvider.InstancesTracker.IsControlFromErrorProvider (nextSibling)) {
					if (ErrorProviderProvider.InstancesTracker.IsFirstControlFromErrorProvider (nextSibling))
						return nextSibling;
					else
						nextSibling = GetNextValidSiblingControl (container, nextSibling);
				}

				return nextSibling;
			}
		}

		private Control GetPreviousValidSiblingControl (Control container, Control sibling)
		{
			if (sibling == null || container == null)
				return null;
			
			int previous = container.Controls.IndexOf (sibling) - 1;
			if (previous < 0)
				return null;
			else {
				Control prevSibling = container.Controls [previous];
				if (ErrorProviderProvider.InstancesTracker.IsControlFromErrorProvider (prevSibling)) {
					if (ErrorProviderProvider.InstancesTracker.IsFirstControlFromErrorProvider (prevSibling))
						return prevSibling;
					else
						prevSibling = GetPreviousValidSiblingControl (container, prevSibling);
				}

				return prevSibling;
			}
		}
		
		#endregion
		
		#region Private Fields

		private SimpleControlProvider simpleProvider;
		
		#endregion
	}
}
