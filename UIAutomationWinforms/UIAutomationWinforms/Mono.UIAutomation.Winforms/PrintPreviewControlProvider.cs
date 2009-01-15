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
// 	Neville Gao <nevillegao@gmail.com>
// 

using System;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.Unix;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.PrintPreviewControl;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.PrintPreviewControl;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (SWF.PrintPreviewControl))]
	internal class PrintPreviewControlProvider
		: FragmentRootControlProvider, IScrollBehaviorSubject
	{
		#region Constructor

		public PrintPreviewControlProvider (SWF.PrintPreviewControl printPreviewControl)
			: base (printPreviewControl)
		{
			this.printPreviewControl = printPreviewControl;
		}

		#endregion
		
		#region SimpleControlProvider: Specializations
		
		public override void Initialize ()
		{
			base.Initialize ();
			
			observer = new ScrollBehaviorObserver (this, printPreviewControl.UIAVScrollBar,
							       printPreviewControl.UIAHScrollBar);
			observer.ScrollPatternSupportChanged += OnScrollPatternSupportChanged;
			UpdateScrollBehavior ();
		}
		
		public override void Terminate ()
		{
			base.Terminate ();

			observer.ScrollPatternSupportChanged -= OnScrollPatternSupportChanged;
		}
		
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Pane.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return Catalog.GetString ("pane");
			else
				return base.GetProviderPropertyValue (propertyId);
		}
		
		#endregion
		
		#region IScrollBehaviorSubject Specialization

		public IScrollBehaviorObserver ScrollBehaviorObserver { 
			get { return observer; }
		}
		
		public FragmentControlProvider GetScrollbarProvider (SWF.ScrollBar scrollbar)
		{
			return new PrintPreviewControlScrollBarProvider (scrollbar, this);
		}

		#endregion
		
		#region ScrollBehaviorObserver Methods
		
		private void OnScrollPatternSupportChanged (object sender, EventArgs args)
		{
			UpdateScrollBehavior ();
		}
		
		private void UpdateScrollBehavior ()
		{
			if (observer.SupportsScrollPattern == true)
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             new ScrollProviderBehavior (this));
			else
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             null);
		}
		
		#endregion
		
		#region Private Fields
		
		private SWF.PrintPreviewControl printPreviewControl;
		private ScrollBehaviorObserver observer;
		
		#endregion
		
		#region Internal Class: ScrollBar provider

		internal class PrintPreviewControlScrollBarProvider : ScrollBarProvider
		{
			#region Constructor
			
			public PrintPreviewControlScrollBarProvider (SWF.ScrollBar scrollbar,
			                                             PrintPreviewControlProvider provider)
				: base (scrollbar)
			{
				this.provider = provider;
				name = scrollbar is SWF.HScrollBar ? "Horizontal Scroll Bar" : "Vertical Scroll Bar";
			}
			
			#endregion
			
			#region Public Methods
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return provider; }
			}			
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return name;
				else
					return base.GetProviderPropertyValue (propertyId);
			}
			
			#endregion
			
			#region Private Fields
			
			private PrintPreviewControlProvider provider;
			private string name;
			
			#endregion
		}
		
		#endregion
	}
}
