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
//      Mike Gorse <mgorse@novell.com>
// 

using System;
using Mono.Unix;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.Form;
using Mono.UIAutomation.Winforms.Behaviors.BalloonWindow;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (NotifyIcon.BalloonWindow))]
	internal class BalloonWindowProvider : FormProvider
	{
		#region Private Data

		private NotifyIcon.BalloonWindow balloon;

		#endregion
		
		#region Constructors

		public BalloonWindowProvider (NotifyIcon.BalloonWindow window) : base (window)
		{
			balloon = window;
		}
		
		#endregion

		public override void Initialize ()
		{
			base.Initialize ();

			SetBehavior (EmbeddedImagePatternIdentifiers.Pattern, 
			             new EmbeddedImageProviderBehavior (this));
		}

		public override void Terminate ()
		{
			// We are trying to Terminate our events, however the instance
			// is already disposed so can't remove the delegates
			if (!AlreadyClosed)
				base.Terminate ();
		}

		#region IRawElementProviderFragmentRoot Members

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.HelpTextProperty.Id)
				return balloon.AccessibleDescription?? balloon.Title;
			else if (propertyId == AutomationElementIdentifiers.IsNotifyIconProperty.Id)
				return true;
			else
				return base.GetProviderPropertyValue (propertyId);
		}
		#endregion

		#region Public Methods
		#endregion
	}
}
