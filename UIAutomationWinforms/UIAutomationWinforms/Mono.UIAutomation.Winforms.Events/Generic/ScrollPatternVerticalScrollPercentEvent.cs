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
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.Windows.Automation;
using SWF = System.Windows.Forms;

namespace Mono.UIAutomation.Winforms.Events.Generic
{
	
	internal class ScrollPatternVerticalScrollPercentEvent<T>
		: BaseAutomationPropertyEvent
			where T : FragmentControlProvider, IScrollBehaviorSubject
	{
		
		#region Constructors

		public ScrollPatternVerticalScrollPercentEvent (T provider)
			: base (provider, 
			        ScrollPatternIdentifiers.VerticalScrollPercentProperty)
		{
			this.provider = provider;
		}
		
		#endregion
		
		#region ProviderEvent Methods

		public override void Connect ()
		{	
			var bar = ScrollBehaviorSubject?.ScrollBehaviorObserver?.VerticalScrollBar;
			if (bar != null)
				bar.ValueChanged += OnScrollPercentChanged;
		}

		public override void Disconnect ()
		{
			var bar = ScrollBehaviorSubject?.ScrollBehaviorObserver?.VerticalScrollBar;
			if (bar != null)
				bar.ValueChanged -= OnScrollPercentChanged;
		}
		
		#endregion

		#region Protected Properties

		protected T ScrollBehaviorSubject {
			get { return provider; }
		}

		#endregion		
		
		#region Protected methods
		
		private void OnScrollPercentChanged (object sender, EventArgs e)
		{
			RaiseAutomationPropertyChangedEvent ();
		}

		#endregion

		#region Private Fields

		private T provider;

		#endregion
	}
}
