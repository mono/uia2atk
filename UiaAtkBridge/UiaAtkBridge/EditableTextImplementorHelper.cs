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
//	Mario Carrion <mcarrion@novell.com>
// 

using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Services;
using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{

	internal class EditableTextImplementorHelper
	{
		
		public EditableTextImplementorHelper (Adapter adapter, Atk.TextImplementor textImplementor)
		{
			this.adapter = adapter;
			this.textImplementor = textImplementor;

			valueProvider 
				= adapter.Provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id)
					as IValueProvider;

			textExpert = TextImplementorFactory.GetImplementor (adapter, adapter.Provider);

			if (valueProvider != null)
				editable = !valueProvider.IsReadOnly;

			insertDeleteProvider
				= adapter.Provider.GetPatternProvider (InsertDeleteTextPatternIdentifiers.Pattern.Id)
					as IInsertDeleteTextProvider;

			oldText = textExpert.Text;

			ClipboardProvider
				= adapter.Provider.GetPatternProvider (ClipboardPatternIdentifiers.Pattern.Id)
					as IClipboardProvider;

			// We are keeping a private caret reference to validate the change
			// of value
			caretProvider
				= adapter.Provider.GetPatternProvider (CaretPatternIdentifiers.Pattern.Id)
					as ICaretProvider;
			caretOffset = (caretProvider != null ? caretProvider.CaretOffset : textExpert.Length);

			RefreshEditable ();
		}

		#region Public Properties

		public IClipboardProvider ClipboardProvider {
			get;
			private set;
		}

		public bool Editable {
			get { return editable; }
			private set {
				if (editable == value)
					return;

				editable = value;
				adapter.NotifyStateChange (Atk.StateType.Editable, editable);
			}
		}

		#endregion

		#region Atk.EditableTextImplementor implementation 
		
		public bool SetRunAttributes (GLib.SList attrib_set, int start_offset, int end_offset)
		{
			return false;
		}
		
		public void InsertText (string str, ref int position)
		{
			if (!Editable)
				return;

			if (position < 0 || position > textExpert.Length)
				position = textExpert.Length;	// gail

			// This provider allows us to avoid string manip when
			// the control itself supports manipulation directly.
			if (insertDeleteProvider != null) {
				try {
					insertDeleteProvider.InsertText (str, ref position);
				} catch (ElementNotEnabledException e) {
					Log.Debug (e);
				}

				return;
			}

			TextContents = textExpert.Text.Substring (0, position)
				+ str + textExpert.Text.Substring (position);
		}
		
		public void CopyText (int startPos, int endPos)
		{
			if (ClipboardProvider == null)
				return;

			ClipboardProvider.Copy (startPos, endPos);
		}
		
		public void CutText (int startPos, int endPos)
		{
			if (ClipboardProvider == null || !Editable)
				return;

			ClipboardProvider.Copy (startPos, endPos);
			DeleteText (startPos, endPos);
		}
		
		public void DeleteText (int startPos, int endPos)
		{
			if (!Editable)
				return;

			if (startPos < 0)
				startPos = 0;
			if (endPos < 0 || endPos > textExpert.Length)
				endPos = textExpert.Length;
			if (startPos > endPos)
				startPos = endPos;

			if (TextContents == null)
				return;

			// This provider allows us to avoid string manip when
			// the control itself supports manipulation directly.
			if (insertDeleteProvider != null) {
				try {
					insertDeleteProvider.DeleteText (startPos, endPos);
				} catch (ElementNotEnabledException e) {
					Log.Debug (e);
				}
				return;
			}

			TextContents = TextContents.Remove (startPos, endPos - startPos);
		}
		
		public void PasteText (int position)
		{
			if (ClipboardProvider == null || !Editable)
				return;

			try {
				ClipboardProvider.Paste (position);
			} catch (ElementNotEnabledException e) {
				Log.Debug (e);
			}
		}
		
		public string TextContents {
			get {
				if (valueProvider == null)
					return null;

				return valueProvider.Value; 
			}
			set {
				if (!Editable) {
					Log.Warn (string.Format ("Cannot set text on an '{0}' Provider {1} is not Editable.",
					                         adapter.GetType (),
					                         adapter.Provider.GetType ()));
					return;
				}

				if (valueProvider == null) {
					Log.Warn (string.Format ("Cannot set text on an '{0}' Provider {1} Does not implement IValueProvider.",
					                         adapter.GetType (),
					                         adapter.Provider.GetType ()));
					return;
				}

				if (!valueProvider.IsReadOnly) {
					try {
						valueProvider.SetValue (value);
					} catch (ElementNotEnabledException e) {
						Log.Debug (e);
					}
				}
			}
		}

		public void UpdateStates (Atk.StateSet states)
		{
			if (Editable)
				states.AddState (Atk.StateType.Editable);
			else
				states.RemoveState (Atk.StateType.Editable);
		}
		
		#endregion

		#region Events Methods

		public bool RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{			
			if (e.Property.Id == AutomationElementIdentifiers.IsValuePatternAvailableProperty.Id) {
				bool isAvailable = (bool) e.NewValue;
				if (!isAvailable) {
					valueProvider = null;
					Editable = false;
					ClipboardProvider = null;
				} else {
					valueProvider = adapter.Provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id)
						as IValueProvider;
					if (valueProvider != null)
						Editable = !valueProvider.IsReadOnly;

					if (ClipboardProvider == null)
						ClipboardProvider = adapter.Provider.GetPatternProvider (ClipboardPatternIdentifiers.Pattern.Id) 
							as IClipboardProvider;
				}

				RefreshEditable ();
				
				return true;
			} else if (e.Property.Id == ValuePatternIdentifiers.ValueProperty.Id) {
				// Don't fire spurious events if the text hasn't changed
				if (textExpert.HandleSimpleChange (ref oldText, 
				                                   ref caretOffset,
				                                   false))
					return true;

				Atk.TextAdapter textAdapter = new Atk.TextAdapter (textImplementor);
				string newText = textExpert.Text;

				// First delete all text, then insert the new text
				textAdapter.EmitTextChanged (Atk.TextChangedDetail.Delete, 
				                             0, 
				                             oldText.Length);

				textAdapter.EmitTextChanged (Atk.TextChangedDetail.Insert, 
				                             0,
				                             newText == null ? 0 : newText.Length);

				if (caretProvider == null)
					caretOffset = textExpert.Length;

				oldText = newText;

				GLib.Signal.Emit (adapter, "visible-data-changed");

				return true;
			} else if (e.Property.Id == ValuePatternIdentifiers.IsReadOnlyProperty.Id) {
				RefreshEditable ();
				return true;
			} else if (e.Property == AutomationElementIdentifiers.IsEnabledProperty) {
				RefreshEditable ();
				return false; // We are explicitly doing this 
			} 

			return false;
		}
		
		public bool RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == TextPatternIdentifiers.CaretMovedEvent) {
				// We are keeping a private caretOffset copy to validate if
				// text changed
				int newCaretOffset = caretProvider.CaretOffset;
				if (newCaretOffset != caretOffset)
					caretOffset = newCaretOffset;

				return true;
			} else if (eventId == TextPatternIdentifiers.TextSelectionChangedEvent) {
				GLib.Signal.Emit (adapter, "text_selection_changed");
				return true;
			} 
			
			return false;
		}

		#endregion

		#region Private Fields

		private void RefreshEditable ()
		{
			if (valueProvider == null)
				Editable = false;
			else
				Editable 
					= (!(bool) adapter.Provider.GetPropertyValue (ValuePatternIdentifiers.IsReadOnlyProperty.Id)) 
					&& (bool) adapter.Provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
		}

		private Adapter adapter;
		private ICaretProvider caretProvider;
		private int caretOffset = -1;
		private bool editable;
		private string oldText;
		private ITextImplementor textExpert;		
		private Atk.TextImplementor textImplementor;
		private IValueProvider valueProvider;
		private IInsertDeleteTextProvider insertDeleteProvider;

		#endregion
	}
}

