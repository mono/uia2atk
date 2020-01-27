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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 


using System;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.Unix;
using Mono.UIAutomation.Services;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (ContextMenuStrip))]
	internal class ContextMenuStripProvider : ToolStripProvider
	{
		public static void HandleContextMenuStripOpened (object sender, EventArgs e)
		{
			var contextMenuStrip = (ContextMenuStrip) sender;
			var contextMenuStripProvider = (FragmentControlProvider) ProviderFactory.GetProvider (contextMenuStrip);

			GetParentProvider (contextMenuStrip).AddChildProvider (contextMenuStripProvider);
		}

		public static void HandleContextMenuStripClosed (object sender, EventArgs e)
		{
			var contextMenuStrip = (ContextMenuStrip) sender;
			var contextMenuStripProvider = (FragmentControlProvider) ProviderFactory.FindProvider (contextMenuStrip);
			if (contextMenuStripProvider == null)
				return;

			GetParentProvider (contextMenuStrip).RemoveChildProvider (contextMenuStripProvider);
			contextMenuStripProvider.Terminate ();
			ProviderFactory.ReleaseProvider (contextMenuStrip);
			
			// TODO: Need to handle disposal of some parent without close happening?
		}

		private static FragmentControlProvider GetParentProvider (ContextMenuStrip contextMenuStrip)
		{
			var control = contextMenuStrip.SourceControl ?? contextMenuStrip.AssociatedControl;
			
			if (control == null) {
				var ownerItem = contextMenuStrip.OwnerItem;
				if (ownerItem != null)
					return (FragmentControlProvider) ProviderFactory.GetProvider (ownerItem);
				else
					Log.Error($"A base Control and OwnerItem for ContextMenuStrip <{contextMenuStrip}> is <null>. Try to find a base Form.");
			}

			var containerForm = control?.FindForm () ?? Form.ActiveForm;
			if (containerForm == null) {
				Log.Error($"Cann't find parent provider for ContextMenuStrip <{contextMenuStrip}>. Use the first opened Form.");
				containerForm = Application.OpenForms [0];
			}
			
			return (FormProvider) ProviderFactory.GetProvider (containerForm);
		}

		public ContextMenuStripProvider (ContextMenuStrip menu) : base (menu)
		{
		}

		public override void Initialize ()
		{
			base.Initialize ();

			AutomationEventArgs args = new AutomationEventArgs (AEIds.MenuOpenedEvent);
			AutomationInteropProvider.RaiseAutomationEvent (AEIds.MenuOpenedEvent,
			                                                this,
			                                                args);
		}

		public override void Terminate ()
		{
			AutomationEventArgs args = new AutomationEventArgs (AEIds.MenuClosedEvent);
			AutomationInteropProvider.RaiseAutomationEvent (AEIds.MenuClosedEvent,
			                                                this,
			                                                args);
			base.Terminate ();
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Menu.Id;
			else if (propertyId == AEIds.IsContentElementProperty.Id)
				return false;
			else if (propertyId == AEIds.LabeledByProperty.Id)
				return null;
			else
				return base.GetProviderPropertyValue (propertyId);
		}
	}
}
