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
//	Andrés G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using PGI = System.Windows.Forms.PropertyGridInternal;

namespace Mono.UIAutomation.Winforms.Behaviors.PropertyGrid
{
	
	internal class CategoryInvokeProviderBehavior 
		: ProviderBehavior, IInvokeProvider
	{

		#region Private Members

		private PGI.GridEntry entry;

		#endregion
		
		#region Constructor
		
		public CategoryInvokeProviderBehavior (PropertyGridCategoryProvider provider)
			: base (provider)
		{
			entry = (PGI.GridEntry)provider.entry;
		}
		
		#endregion
		
		#region IProviderBehavior Interface

		public override void Connect ()
		{
			//TODO:
//			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
//			                   new InvokePatternInvokedEvent (Provider));
		}
		
		public override void Disconnect ()
		{
			//TODO:
//			Provider.SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
//			                   null);
		}
		
		public override AutomationPattern ProviderPattern { 
			get { return InvokePatternIdentifiers.Pattern; }
		}
		
		#endregion
		
		#region IInvokeProvider Members
		
		public virtual void Invoke ()
		{
			PerformClick ();
		}
		
		#endregion	
		
		#region Private Methods
		
		private void PerformClick ()
		{
			if (Provider.Control.InvokeRequired == true) {
				Provider.Control.BeginInvoke (new SWF.MethodInvoker (PerformClick));
				return;
			}
			entry.Expanded = !entry.Expanded;
		}
		
		#endregion
		
	}

}
