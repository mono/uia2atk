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
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{

	public class Slider : ComponentAdapter , Atk.ValueImplementor, Atk.TextImplementor
	{
		private IRangeValueProvider rangeValueProvider;
		internal ITextImplementor textExpert = null;
		private string oldText;
		
		public Slider (IRawElementProviderSimple provider) : base (provider)
		{
			Role = Atk.Role.Slider;
			rangeValueProvider = (IRangeValueProvider)provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			textExpert = TextImplementorFactory.GetImplementor (this, provider);
			oldText = textExpert.Text;
		}

		protected override void UpdateNameProperty (string newName, bool fromCtor)
		{
			// ControlType.Slider returns Name from one static label, Atk returns NULL
		}

		public void GetMinimumValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider != null? rangeValueProvider.Minimum: 0);
		}

		public void GetMaximumValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider != null? rangeValueProvider.Maximum: 100);
		}

		public void GetMinimumIncrement (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider != null? rangeValueProvider.SmallChange: 0);
		}

		public void GetCurrentValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider != null? rangeValueProvider.Value: (double)0);
		}

		public bool SetCurrentValue (GLib.Value value)
		{
			double v = (double)value.Val;
			if (rangeValueProvider != null) {
				if (v > rangeValueProvider.Maximum)
					return false;
				if (v < rangeValueProvider.Minimum)
					v = rangeValueProvider.Minimum;
				rangeValueProvider.SetValue (v);
				return true;
			}
			
			return false;
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			states.AddState (Orientation == OrientationType.Vertical? Atk.StateType.Vertical: Atk.StateType.Horizontal);
			return states;
		}

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			// TODO
			base.RaiseAutomationEvent (eventId, e);
		}
		
		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == RangeValuePatternIdentifiers.ValueProperty) {
				Notify ("accessible-value");
				NewText (e.NewValue.ToString ());
			}
			else
				base.RaiseAutomationPropertyChangedEvent (e);
		}

		#region TextImplementor Members
		public int CaretOffset {
			get {
				return 0;
			}
		}

		public int CharacterCount {
			get {
				return textExpert.Text.Length;
			}
		}

		public int NSelections {
			get {
				return -1;
			}
		}
		
		public string GetText (int startOffset, int endOffset)
		{
			return textExpert.GetText (startOffset, endOffset);
		}

		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = 
				textExpert.GetTextAfterOffset (offset, boundaryType, out startOffset, out endOffset);
			return ret;
		}
		
		public string GetTextAtOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = 
				textExpert.GetTextAtOffset (offset, boundaryType, out startOffset, out endOffset);
			return ret;
		}
		
		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = 
				textExpert.GetTextBeforeOffset (offset, boundaryType, out startOffset, out endOffset);
			return ret;
		}
		
		public Atk.Attribute [] GetRunAttributes (int offset, out int startOffset, out int endOffset)
		{
			return textExpert.GetRunAttributes (offset, out startOffset, out endOffset);
		}

		public Atk.Attribute [] DefaultAttributes {
			get { return textExpert.DefaultAttributes; }
		}

		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			textExpert.GetCharacterExtents (offset, out x, out y, out width, out height, coords);
		}

		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			return textExpert.GetOffsetAtPoint (x, y, coords);
		}

		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			startOffset = 0;
			endOffset = 0;
			return null;
		}

		public bool AddSelection (int startOffset, int endOffset)
		{
			return false;
		}

		public bool RemoveSelection (int selectionNum)
		{
			return false;
		}

		public bool SetSelection (int selectionNum, int startOffset, int endOffset)
		{
			return false;
		}
		
		public char GetCharacterAtOffset (int offset)
		{
			return textExpert.GetCharacterAtOffset (offset);
		}

		public bool SetCaretOffset (int offset)
		{
			return false;
		}

		public void GetRangeExtents (int startOffset, int endOffset, Atk.CoordType coordType, out Atk.TextRectangle rect)
		{
			textExpert.GetRangeExtents (startOffset, endOffset, coordType, out rect);
		}

		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coordType, Atk.TextClipType xClipType, Atk.TextClipType yClipType)
		{
			return textExpert.GetBoundedRanges (rect, coordType, xClipType, yClipType);
		}
		#endregion

		#region Private Methods
		private void NewText (string newText)
		{
			int caretOffset = textExpert.Length;
			if (textExpert.HandleSimpleChange (ref oldText, ref caretOffset))
				return;

			Atk.TextAdapter adapter = new Atk.TextAdapter (this);

			// First delete all text, then insert the new text
			adapter.EmitTextChanged (Atk.TextChangedDetail.Delete, 0, oldText.Length);

			adapter.EmitTextChanged (Atk.TextChangedDetail.Insert, 0,
				                 newText == null ? 0 : newText.Length);
			oldText = newText;
		}

		private OrientationType Orientation {
			get {
				return (OrientationType)Provider.GetPropertyValue (AutomationElementIdentifiers.OrientationProperty.Id);
			}
		}
		#endregion
	}
}
