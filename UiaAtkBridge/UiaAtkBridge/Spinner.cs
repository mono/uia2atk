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


using System;
using System.Windows.Automation;
using Mono.UIAutomation.Services;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Bridge;

namespace UiaAtkBridge
{
	/// <summary>
	/// Adapter for a ControlType.Spinner that does not implement SelectionPattern.
	/// </summary>
	// FIXME: This class *needs* to use TextImplementorHelper and EditableTextImplementorHelper
	public abstract class Spinner : ComponentAdapter, Atk.ITextImplementor, Atk.IEditableTextImplementor
	{
		#region Fields
		protected IRangeValueProvider rangeValueProvider;
		protected IValueProvider valueProvider;
		internal IEditableRangeProvider editableRange = null;
		internal ITextImplementor textExpert = null;
		private string oldText;
		#endregion

		#region Constructor
		public Spinner (IRawElementProviderSimple provider) : base (provider)
		{
			Role = Atk.Role.SpinButton;
			rangeValueProvider = (IRangeValueProvider)provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			
			editableRange
				= provider.GetPatternProvider (EditableRangePatternIdentifiers.Pattern.Id) as IEditableRangeProvider;

			valueProvider = (IValueProvider)provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			textExpert = TextImplementorFactory.GetImplementor (this, provider);
			oldText = textExpert.Text;
		}
		#endregion

		#region Overridden Members

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == RangeValuePatternIdentifiers.ValueProperty) {
				Notify ("accessible-value");
				if (valueProvider == null)
					NewText (e.NewValue.ToString ());
			} else if (e.Property == RangeValuePatternIdentifiers.IsReadOnlyProperty) {
				NotifyStateChange (Atk.StateType.Editable, !(bool)e.NewValue);
			} else if (rangeValueProvider == null && e.Property == ValuePatternIdentifiers.IsReadOnlyProperty) {
				// If we have both rangeValueProvider and
				// Valueprovider, we'll get two notifications,
				// so we should ignore one
				NotifyStateChange (Atk.StateType.Editable, !(bool)e.NewValue);
			} else if (e.Property == ValuePatternIdentifiers.ValueProperty) {
				NewText ((string)e.NewValue);
			} else
				base.RaiseAutomationPropertyChangedEvent (e);
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			if (states.ContainsState (Atk.StateType.Defunct))
				return states;

			states.AddState (Atk.StateType.SingleLine);

			if (!ReadOnly)
				states.AddState (Atk.StateType.Editable);
			else
				states.RemoveState (Atk.StateType.Editable);

			return states;
		}
		#endregion

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

		#region EditableTextImplementor Members
		public string TextContents {
			set {
				// TODO: This gets better behavior in Accerciser, but fails tests
				/*double parsedVal;
				if (rangeValueProvider != null && double.TryParse (value, out parsedVal) &&
				         parsedVal > rangeValueProvider.Minimum &&
				         parsedVal < rangeValueProvider.Maximum)
					rangeValueProvider.SetValue (parsedVal);*/
				
				// It's a numeric spinner; do not set
				// the number until DoAction called
				if (editableRange != null) {
					try {
						editableRange.BeginEdit (value);
					} catch (ElementNotEnabledException e) {
						Log.Debug (e);
					}
				} else if (valueProvider != null) {
					try {
						valueProvider.SetValue (value);
					} catch (ElementNotEnabledException e) {
						Log.Debug (e);
					}
				} else
					NewText (value);
			}
		}

		public bool SetRunAttributes (GLib.SList attrib_set, int start_offset, int end_offset)
		{
			return false;
		}

		public void CopyText (int start_pos, int end_pos)
		{
			Log.Warn ("Spinner: CopyText not implemented");
		}

		public void CutText (int start_pos, int end_pos)
		{
			Log.Warn ("Spinner: CutText not implemented");
		}

		public void PasteText (int position)
		{
			Log.Warn ("Spinner: PasteText not implemented");
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
			TextContents = textExpert.Text.Substring (0, start_pos)
				+ textExpert.Text.Substring (end_pos);
		}

		public void InsertText (string text, ref int position)
		{
			if (position < 0 || position > textExpert.Length)
				position = textExpert.Length;	// gail
			TextContents = textExpert.Text.Substring (0, position)
				+ text
				+ textExpert.Text.Substring (position);
			position += text.Length;
		}
		#endregion

		#region Private Methods
		private void NewText (string newText)
		{
			int caretOffset = textExpert.Length;
			if (textExpert.HandleSimpleChange (ref oldText, ref caretOffset))
				return;

			// First delete all text, then insert the new text
			textExpert.EmitTextChanged (Atk.TextChangedDetail.Delete, 0, oldText.Length, oldText);

			textExpert.EmitTextChanged (Atk.TextChangedDetail.Insert, 0,
				                 newText == null ? 0 : newText.Length);
			oldText = newText;
		}

		protected virtual bool ReadOnly {
			get {
				if (rangeValueProvider != null)
					return (bool) Provider.GetPropertyValue (RangeValuePatternIdentifiers.IsReadOnlyProperty.Id);
				return false;
			}
		}
		#endregion

	}

	public class SpinnerWithValue : Spinner, Atk.IValueImplementor, Atk.IActionImplementor
	{
		#region Private Members
		private ActionImplementorHelper actionExpert = null;
		#endregion

		#region Constructor
		public SpinnerWithValue (IRawElementProviderSimple provider) : base (provider)
		{
			actionExpert = new ActionImplementorHelper ();
			actionExpert.Add ("activate", "activate", null, DoActivate);
		}
		#endregion

		#region ValueImplementor Members
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
			try {
				rangeValueProvider.SetValue (v);
			} catch (ArgumentOutOfRangeException e) {
				Log.Debug (e);
				return false;
			} catch (ElementNotEnabledException e) {
				Log.Debug (e);
				return false;
			}

			return true;
		}
		#endregion

		#region ActionImplementor Members
		// Return the number of actions (Read-Only)
		public int NActions
		{
			get {
				return actionExpert.NActions;
			}
		}
		
		// Get a localized name for the specified action
		public string GetLocalizedName (int action)
		{
			return actionExpert.GetLocalizedName (action);
		}
		
		// Sets a description of the specified action
		public bool SetDescription (int action, string description)
		{
			return actionExpert.SetDescription (action, description);
		}
		
		// Get the key bindings for the specified action
		public string GetKeybinding (int action)
		{
			return null;
		}

		// Get the name of the specified action
		public string GetName (int action)
		{
			return actionExpert.GetName (action);
		}
		
		// Get the description of the specified action
		public string GetDescription (int action)
		{
			return actionExpert.GetDescription (action);
		}

		// Perform the action specified
		public virtual bool DoAction (int action)
		{
			return actionExpert.DoAction (action);
		}
		#endregion

		#region Private Methods
		private bool DoActivate ()
		{
			bool enabled = (bool) Provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			if (!enabled)
				return false;

			double val;
			if (!double.TryParse (textExpert.Text, out val) ||
			    val > rangeValueProvider.Maximum ||
			    val < rangeValueProvider.Minimum)
				return false;
			
			if (editableRange != null)
				editableRange.CommitEdit ();
			else {
				try {
					rangeValueProvider.SetValue (double.Parse (textExpert.Text));
				} catch (ArgumentOutOfRangeException e) {
					Log.Debug (e);
					return false;
				} catch (ElementNotEnabledException e) {
					Log.Debug (e);
					return false;
				}
			}
			return true;
		}

		protected override bool ReadOnly {
			get {
				return (bool) Provider.GetPropertyValue (RangeValuePatternIdentifiers.IsReadOnlyProperty.Id);
			}
		}
		#endregion
	}
}
