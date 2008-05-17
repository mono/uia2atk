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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{

	// TODO: Supposly this control should support Edit and Document control 
	// types (according to http://www.mono-project.com/Accessibility:_Control_Status)
	// however right now only Edit control type is being implemented.
	public class TextBoxProvider : SimpleControlProvider, ITextProvider, IValueProvider, IRangeValueProvider
	{
#region Private section
		private TextBox textbox;
#endregion
	
#region Constructors
		public TextBoxProvider (TextBox textbox) : base (textbox)
		{
			this.textbox = textbox;
			
			SetEventStrategy (EventStrategyType.TextChangedEvent, 
			                  new TextChangedEventStrategy (this, control));
		}
#endregion
		
#region Protected Methods
		protected override int GetControlTypeProperty () 
		{
			return ControlType.Edit.Id;
		}
#endregion
	
#region IRawElementProviderSimple Members
	
		public override object GetPatternProvider (int patternId)
		{
			return null;
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			// Edit Control Type properties
			if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
				return textbox.Bounds.ToRect ();
			// TODO: ClickablePointProperty
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return control.Name;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "text";
			else if (propertyId == AutomationElementIdentifiers.IsPasswordProperty.Id)
				return (textbox.UseSystemPasswordChar || (int) textbox.PasswordChar != 0);
			else
				return base.GetPropertyValue (propertyId);
		}

#endregion
		
#region ITextProvider members
		public ITextRangeProvider DocumentRange {
			get {
				throw new NotImplementedException();
			}
		}

		public SupportedTextSelection SupportedTextSelection {
			get {
				throw new NotImplementedException();
			}
		}

		public ITextRangeProvider[] GetSelection ()
		{
			throw new NotImplementedException();
		}

		public ITextRangeProvider[] GetVisibleRanges ()
		{
			throw new NotImplementedException();
		}

		public ITextRangeProvider RangeFromChild (IRawElementProviderSimple childElement)
		{
			throw new NotImplementedException();
		}

		public ITextRangeProvider RangeFromPoint (System.Windows.Point screenLocation)
		{
			throw new NotImplementedException();
		}
#endregion
		
#region IValueProvider members
		public bool IsReadOnly { 
			get { return textbox.ReadOnly; }
		}

		public string Value {
			get { 
				if ((bool) GetPropertyValue (AutomationElementIdentifiers.IsPasswordProperty.Id))
					throw new InvalidOperationException ();

				return textbox.Text; 
			}
		}

		public void SetValue (string value) 
		{
			//TODO: Exceptions?
			textbox.Text = value;
		}
#endregion
		
#region IRangeValueProvider members
		public double LargeChange {
			get { return 0; }
		}
		
		// TODO: ???
		public double Maximum  {
			get { return double.MaxValue; }
		}
		
		// TODO: ???
		public double Minimum  {
			get { return double.MinValue; }
		}
		
		// TODO: ???
		public double SmallChange  {
			get { return 0; }
		}
		
		double IRangeValueProvider.Value  {
			get { 
				double value;
				// TODO: What should I do in case of failure?
				double.TryParse (Value, out value);
				return value; 
			}
		}

		public void SetValue (double value) 
		{
			if (value < Minimum || value > Maximum)
				throw new ArgumentOutOfRangeException ();

			textbox.Text = value.ToString ();
		}
#endregion
		
#region Event handlers

		// TODO: InvalidatedEvent
		// TODO: TextSelectionChangedEvent: using textbox.SelectionLength != 0?	
		// TODO: NameProperty property-changed event.
		// TODO: ValuePatternIdentifiers.ValueProperty property-changed event.
		// TODO: RangeValuePatternIdentifiers.ValueProperty property-changed event.
		// TODO: StructureChangedEvent
#endregion
	}

}
