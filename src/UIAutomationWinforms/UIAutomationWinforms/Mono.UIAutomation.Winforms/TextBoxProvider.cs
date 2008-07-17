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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms.Behaviors;

namespace Mono.UIAutomation.Winforms
{

	// TODO: Implement ITextProvider, IScrollProvider
	internal class TextBoxProvider : FragmentControlProvider, /*IValueProvider, ITextProvider, */IScrollProvider
	{
#region Protected section
		
		protected TextBoxBase textboxbase;

#endregion
		
#region Private section
		
		private ITextRangeProvider text_range_provider;
		
#endregion
	
#region Constructors

		public TextBoxProvider (TextBoxBase textBoxBase) : base (textBoxBase)
		{
			this.textboxbase = textBoxBase;
			
			SetBehavior (TextPatternIdentifiers.Pattern,
			             new TextBoxTextProviderBehavior (this));
		}

#endregion

#region Public Methods

		public override void InitializeEvents ()
		{
			base.InitializeEvents ();
			
			// Edit

			// NameProperty. uses control.Name to emit changes, so right now
			// we're "cleaning" the previous value.
			/*SetEvent (ProviderEventType.AutomationElementNameProperty, null);
			SetEvent (ProviderEventType.TextChangedEvent, 
			          new TextPatternTextChangedEvent (this));
			SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty, 
			          new TextBoxHasKeyBoardFocusPropertyEvent (this));*/
			
			// TODO: InvalidatedEvent
			// TODO: TextSelectionChangedEvent: using textbox.SelectionLength != 0?	
			// TODO: NameProperty property-changed event.
			// TODO: ValuePatternIdentifiers.ValueProperty property-changed event.
			
			// Document
			
			// TODO: AutomationFocusChangedEvent
			// TODO: HorizontallyScrollableProperty property-changed event.
			// TODO: HorizontalScrollPercentProperty property-changed event.
			// TODO: HorizontalViewSizeProperty property-changed event.
			// TODO: VerticalScrollPercentProperty property-changed event.
			// TODO: VerticallyScrollableProperty property-changed event.
			// TODO: VerticalViewSizeProperty property-changed event.
			// TODO: InvalidatedEvent (DEPENDS)
			// TODO: TextSelectionChangedEvent
			// TODO: ValueProperty property-changed event. (NEVER)
		}

#endregion
	
#region IRawElementProviderSimple Members
	
		public override object GetPatternProvider (int patternId)
		{			
			if (textboxbase.Multiline) {
				if (patternId == ScrollPatternIdentifiers.Pattern.Id
				    || patternId == TextPatternIdentifiers.Pattern.Id)
					return this;
			} else {
				if (patternId == ValuePatternIdentifiers.Pattern.Id
					|| patternId == TextPatternIdentifiers.Pattern.Id)
					return this;
			}
			return null;
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return textboxbase.Multiline ? ControlType.Document.Id : ControlType.Edit.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return textboxbase.Multiline ? "document" : "edit";
			else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id) {
				// TODO: We are using TabIndex to evaluate whether the previous control
				// is Label (that way we know if this label is associated to the TextBox.
				// Right?)
				if (textboxbase.Parent != null) {
					Label associatedLabel = textboxbase.Parent.GetNextControl (textboxbase, true) as Label;
					if (associatedLabel != null)
						return associatedLabel;
				}
				return null;
			} /*else if (propertyId == AutomationElementIdentifiers.IsPasswordProperty.Id) {
				TextBox textBox = textboxbase as TextBox;
				if (textBox != null)
					return (textBox.UseSystemPasswordChar || (int) textBox.PasswordChar != 0);
				else
					return null;
			} */else
				return base.GetPropertyValue (propertyId);
		}
		
#endregion
		
//#region IValueProvider Members
//		
//		public bool IsReadOnly {
//			get { return textboxbase.ReadOnly; }
//		}
//
//		public string Value {
//			get { return textboxbase.Text; }
//		}
//
//		public void SetValue (string value)
//		{
//			if (IsReadOnly)
//				throw new ElementNotEnabledException ();
//
//			textboxbase.Text = value;
//		}
//		
//#endregion
		
//#region ITextProvider Members
//		
//		//TODO: We should connect the events to update this.text_range_provider?
//		public ITextRangeProvider DocumentRange {
//			get { 
//				if (text_range_provider == null)
//					text_range_provider = new TextRangeProvider (this, textboxbase); 
//				return text_range_provider;
//			}
//		}
//		
//		public SupportedTextSelection SupportedTextSelection {
//			get { return SupportedTextSelection.Single; }
//		}
//
//		public ITextRangeProvider[] GetSelection ()
//		{
//			if (SupportedTextSelection == SupportedTextSelection.None)
//				throw new InvalidOperationException ();
//				
//			//TODO: Return null when system cursor is not present, how to?
//
//			return new ITextRangeProvider [] { 
//				new TextRangeProvider (this, textboxbase) };
//		}
//		
//		public ITextRangeProvider[] GetVisibleRanges ()
//		{
//			throw new NotImplementedException ();
//		}
//		
//		public ITextRangeProvider RangeFromChild (IRawElementProviderSimple childElement) 
//		{
//			throw new NotImplementedException ();
//		}
//		
//		public ITextRangeProvider RangeFromPoint (Point screenLocation)
//		{
//			throw new NotImplementedException ();
//		}
//		
//#endregion		
		
#region IScrollProvider Members
		
		public bool HorizontallyScrollable { 
			get { throw new NotImplementedException (); }
		}
		
		public double HorizontalScrollPercent { 
			get { throw new NotImplementedException (); }
		}
		
		public double HorizontalViewSize { 
			get { throw new NotImplementedException (); }
		}
		
		public bool VerticallyScrollable { 
			get { throw new NotImplementedException (); }
		}
		
		public double VerticalScrollPercent { 
			get { throw new NotImplementedException (); }
		}
		
		public double VerticalViewSize {
			get { throw new NotImplementedException (); }
		}

		public void Scroll (ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
		{
			throw new NotImplementedException ();
		}
		
		public void SetScrollPercent (double horizontalPercent, double verticalPercent) 
		{
			throw new NotImplementedException ();
		}
		
#endregion


	}

}
