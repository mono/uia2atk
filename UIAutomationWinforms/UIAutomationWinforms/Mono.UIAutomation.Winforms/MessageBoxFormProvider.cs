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
using Mono.Unix;
using System.Windows;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Bridge;

namespace Mono.UIAutomation.Winforms
{

	[MapsComponent (typeof (SWF.MessageBox.MessageBoxForm))]
	internal class MessageBoxFormProvider : FormProvider
	{

		#region Constructors
		
		public MessageBoxFormProvider (SWF.MessageBox.MessageBoxForm form) : base (form)
		{
			this.form = form;
		}

		#endregion

		#region Public Methods

		public SWF.MessageBox.MessageBoxForm Form {
				get { return form; }
		}
		
		#endregion

		#region Public Methods

		protected override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			if (labelProvider == null) {
				labelProvider = new MessageBoxFormLabelProvider (this);
				labelProvider.Initialize ();
				AddChildProvider (labelProvider);
			}
			if (imageProvider == null
			    && form.UIAIconRectangle.Width >= 0
			    && form.UIAIconRectangle.Height >= 0) {
				
				imageProvider = new MessageBoxImageProvider (this);
				imageProvider.Initialize ();
				AddChildProvider (imageProvider);
			}
		}

		protected override void FinalizeChildControlStructure ()
		{
			if (labelProvider != null) {
				RemoveChildProvider (labelProvider);
				labelProvider.Terminate ();
				labelProvider = null;
			}
			if (imageProvider != null) {
				RemoveChildProvider (imageProvider);
				imageProvider.Terminate ();
				imageProvider = null;
			}
			base.FinalizeChildControlStructure ();
		}

		#endregion

		#region Private Fields

		private SWF.MessageBox.MessageBoxForm form;
		private MessageBoxImageProvider imageProvider;
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
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id) 
					return provider.Form.UIAMessage;
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					Rect bounds 
						= (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					return Helper.IsOffScreen (bounds, provider.Control, true);
				} else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get {
					return Helper.RectangleToRect (provider.Control.TopLevelControl.RectangleToScreen (provider.Form.UIAMessageRectangle));
				}
			}

			private MessageBoxFormProvider provider;
		}

		#endregion

		#region Internal Class: Image Provider

		internal class MessageBoxImageProvider : FragmentControlProvider, IEmbeddedImageProvider
		{
			public MessageBoxImageProvider (MessageBoxFormProvider provider) 
				: base (null)
			{
				this.provider = provider;
			}

			#region IEmbeddedImageProvider Members
			public Rect Bounds {
				get { return (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id); }
			}
	
			public string Description {
				get { return string.Empty; }
			}
			#endregion

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get {  return provider; }
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Image.Id;
				else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return string.Empty;
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					Rect bounds 
						= (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					return Helper.IsOffScreen (bounds, provider.Control, true);
				} else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			protected override Rect BoundingRectangleProperty {
				get {
					return Helper.RectangleToRect (provider.Control.TopLevelControl.RectangleToScreen (provider.Form.UIAIconRectangle));
				}
			}

			private MessageBoxFormProvider provider;
		}

		#endregion
	}
}
