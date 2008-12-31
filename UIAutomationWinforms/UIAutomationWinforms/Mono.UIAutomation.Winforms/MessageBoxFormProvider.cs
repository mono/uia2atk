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
using System.Windows;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms
{

	internal class MessageBoxFormProvider : FormProvider
	{

		#region Constructors
		
		public MessageBoxFormProvider (SWF.MessageBox.MessageBoxForm form) : base (form)
		{
		}

		#endregion

		#region Public Methods

		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			// FIXME: Add icon
			if (labelProvider == null) {
				labelProvider = new MessageBoxFormLabelProvider (this);
				labelProvider.Initialize ();
				OnNavigationChildAdded (false, labelProvider);
			}
		}

		public override void FinalizeChildControlStructure ()
		{
			base. FinalizeChildControlStructure ();

			if (labelProvider != null) {
				labelProvider.Terminate ();
				OnNavigationChildRemoved (false, labelProvider);
				labelProvider = null;
			}
		}

		#endregion

		#region Private Fields

		private MessageBoxFormLabelProvider labelProvider;

		#endregion

		#region Internal Class: Label Provider

		internal class MessageBoxFormLabelProvider : LabelProvider
		{
			public MessageBoxFormLabelProvider (MessageBoxFormProvider provider) 
				: base (null)
			{
				this.provider = provider;
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get {  return provider; }
			}

			public override void Initialize ()
			{
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id) {
					string msg = string.Empty;
					try {
						msg = Helper.GetPrivateProperty<SWF.MessageBox.MessageBoxForm, string> (typeof (SWF.MessageBox.MessageBoxForm),
							                                                                    (SWF.MessageBox.MessageBoxForm) provider.Control,
							                                                                    "UIAMessage");
					} catch (NotSupportedException) { }
					return msg;
				} else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					Rect bounds 
						= (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					return Helper.IsOffScreen (bounds, provider.Control, true);
				} else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
					SD.Rectangle rect1 = SD.Rectangle.Empty;
					try {
						SD.RectangleF rect = Helper.GetPrivateProperty<SWF.MessageBox.MessageBoxForm, SD.RectangleF> (typeof (SWF.MessageBox.MessageBoxForm),
						                                                                                              (SWF.MessageBox.MessageBoxForm) provider.Control,
					        	                                                                                      "UIAMessageRectangle");
						rect1 = new SD.Rectangle ((int) rect.X, (int) rect.Y,
						                          (int) rect.Width, (int) rect.Height);
					} catch (NotSupportedException) { }
					return Helper.RectangleToRect (provider.Control.TopLevelControl.RectangleToScreen (rect1));
				} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private MessageBoxFormProvider provider;
		}

		#endregion
	}
}

