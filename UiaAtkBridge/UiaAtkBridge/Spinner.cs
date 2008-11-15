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
//      Mike Gorse <mgorse@novell.com>
// 
// Note: This class handles spinners that set a numeric value.
// Spinners that select items instead are treated as lists.

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{

	public class Spinner : ComponentAdapter, Atk.TextImplementor, Atk.EditableTextImplementor
	{
		protected IRangeValueProvider rangeValueProvider;
		protected IValueProvider valueProvider;
		internal TextImplementorHelper textExpert = null;

		public Spinner (IRawElementProviderSimple provider) : base (provider)
		{
			string text = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			Name = text;
			Role = Atk.Role.SpinButton;
			rangeValueProvider = (IRangeValueProvider)provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			valueProvider = (IValueProvider)provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			if (valueProvider != null)
				text = valueProvider.Value;
			else if (rangeValueProvider != null)
				text = rangeValueProvider.Value.ToString ();
			else
				text = String.Empty;
			textExpert = new TextImplementorHelper (text, this);
		}

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			// TODO
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == RangeValuePatternIdentifiers.ValueProperty) {
				Notify ("accessible-value");
				NewText (((double)e.NewValue).ToString ());
			}
			else if (e.Property == ValuePatternIdentifiers.ValueProperty) {
				NewText ((string)e.NewValue);
			}
		}

		public int CaretOffset {
			get {
				return 0;
			}
		}

		public GLib.SList DefaultAttributes {
			get {
				//TODO:
				GLib.SList attribs = new GLib.SList(typeof(Atk.TextAttribute));
				return attribs;
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
		
		public GLib.SList GetRunAttributes (int offset, out int startOffset, out int endOffset)
		{
			// don't ask me why, this is what gail does 
			// (instead of throwing or returning null):
			if (offset > Name.Length)
				offset = Name.Length;
			else if (offset < 0)
				offset = 0;
			
			//just test values for now:
			endOffset = Name.Length;
			startOffset = offset;
			
			//TODO:
			GLib.SList attribs = new GLib.SList(typeof(Atk.TextAttribute));
			return attribs;
		}

		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			textExpert.GetCharacterExtents (offset, out x, out y, out width, out height, coords);
		}

		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		private void NewText (string newText)
		{
				// Don't fire spurious events if the text hasn't changed
				if (textExpert.Text == newText)
					return;

			Atk.TextAdapter adapter = new Atk.TextAdapter (this);

			// First delete all text, then insert the new text
			adapter.EmitTextChanged (Atk.TextChangedDetail.Delete, 0, textExpert.Length);

			textExpert = new TextImplementorHelper (newText, this);
			adapter.EmitTextChanged (Atk.TextChangedDetail.Insert, 0,
				                 newText == null ? 0 : newText.Length);
		}

		public string TextContents {
			set {
				if (valueProvider != null)
					valueProvider.SetValue (value);
				else
					// It's a numeric spinner; do not set
					// the number until DoAction called
					NewText (value);
			}
		}

		public bool SetRunAttributes (GLib.SList attrib_set, int start_offset, int end_offset)
		{
			return false;
		}

		public void CopyText (int start_pos, int end_pos)
		{
			Console.WriteLine ("UiaAtkBridge (spinner): CopyText unimplemented");
		}

		public void CutText (int start_pos, int end_pos)
		{
			Console.WriteLine ("UiaAtkBridge (spinner): CutText unimplemented");
		}

		public void PasteText (int position)
		{
			Console.WriteLine ("UiaAtkBridge (spinner): PasteText unimplemented");
		}

		public void DeleteText (int start_pos, int end_pos)
		{
			int length = textExpert.Length;
			if (start_pos < 0)
				start_pos = 0;
			if (end_pos < 0 || end_pos > length)
				end_pos = length;
			if (start_pos > end_pos)
				start_pos = end_pos;
Console.WriteLine ("start_pos " + start_pos + " end_pos " + end_pos);
			TextContents = textExpert.Text.Substring (0, start_pos)
				+ textExpert.Text.Substring (end_pos);
		}

		public void InsertText (string str1ng, ref int position)
		{
			if (position < 0 || position > textExpert.Length)
				position = textExpert.Length;	// gail
			TextContents = textExpert.Text.Substring (0, position)
				+ str1ng
				+ textExpert.Text.Substring (position);
			position += str1ng.Length;
		}
	}

	public class SpinnerWithValue : Spinner, Atk.ValueImplementor, Atk.ActionImplementor
	{
		private string						actionDescription = null;
		static string					actionName = "activate";

		public SpinnerWithValue (IRawElementProviderSimple provider) : base (provider)
		{
		}

		public void GetMinimumValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider.Minimum);
		}

		public void GetMaximumValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider.Maximum);
		}

		public void GetMinimumIncrement (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider.SmallChange);
		}

		public void GetCurrentValue (ref GLib.Value value)
		{
			value = new GLib.Value(rangeValueProvider.Value);
		}

		public bool SetCurrentValue (GLib.Value value)
		{
			double v = (double)value.Val;
			if (v < rangeValueProvider.Minimum || v > rangeValueProvider.Maximum)
				return false;
			rangeValueProvider.SetValue (v);
			return true;
		}

		// Return the number of actions (Read-Only)
		public int NActions
		{
			get {
				return 1;
			}
		}
		
		// Get a localized name for the specified action
		public string GetLocalizedName (int action)
		{
			if (action != 0)
				return null;

			// TODO: Localize the name?
			return actionName;
		}
		
		// Sets a description of the specified action
		public bool SetDescription (int action, string description)
		{
			if (action != 0)
				return false;
			
			actionDescription = description;
			return true;
		}
		
		// Get the key bindings for the specified action
		public string GetKeybinding (int action)
		{
			return null;
		}

		// Get the name of the specified action
		public string GetName (int action)
		{
			if (action != 0)
				return null;

			return actionName;
		}
		
		// Get the description of the specified action
		public string GetDescription (int action)
		{
			if (action != 0)
				return null;

			return actionDescription;
		}

		// Perform the action specified
		public virtual bool DoAction (int action)
		{
			if (action != 0)
				return false;
			bool enabled = (bool) Provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			if (!enabled)
				return false;
			rangeValueProvider.SetValue (double.Parse (textExpert.Text));
			return true;
		}
	}
}
