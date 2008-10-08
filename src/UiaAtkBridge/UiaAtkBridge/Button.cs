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
//      Andrés G. Aragoneses <aaragoneses@novell.com>
//      Calvin Gaisford <cgaisford@novell.com>
// 

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class Button : ComponentAdapter, Atk.ActionImplementor, Atk.TextImplementor, Atk.ImageImplementor
	{
		private static string default_invoke_description = "Sends a request to activate a control and initiate its single, unambiguous action.";
		private static string default_invoke_name = "click";

		private IRawElementProviderSimple	provider;
		private IInvokeProvider				invokeProvider;
		private IToggleProvider				toggleProvider;
		private string						actionDescription = null;
		protected string					actionName = null;
		
		private TextImplementorHelper textExpert = null;
		private int selectionStartOffset = 0, selectionEndOffset = 0;
		
		// UI Automation Properties supported
		// AutomationElementIdentifiers.AcceleratorKeyProperty.Id
		// AutomationIdProperty() ?
		// AutomationElementIdentifiers.BoundingRectangleProperty.Id
		// AutomationElementIdentifiers.ClickablePointProperty.Id
		// AutomationElementIdentifiers.ControlTypeProperty.Id
		// AutomationElementIdentifiers.HelpTextProperty.Id
		// AutomationElementIdentifiers.IsContentElementProperty.Id
		// AutomationElementIdentifiers.IsControlElementProperty.Id
		// AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id
		// AutomationElementIdentifiers.LabeledByProperty.Id
		// AutomationElementIdentifiers.LocalizedControlTypeProperty.Id
		// AutomationElementIdentifiers.NameProperty.Id
		public Button (IRawElementProviderSimple provider)
		{
			this.provider = provider;
			
			invokeProvider = (IInvokeProvider)provider.GetPatternProvider(InvokePatternIdentifiers.Pattern.Id);
			if (invokeProvider != null) {
				//it seems the default description should be null:
				//actionDescription = default_invoke_description;
				actionName = default_invoke_name;
				Role = Atk.Role.PushButton;
			}
			
			string buttonText = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			textExpert = new TextImplementorHelper (buttonText, this);
			Name = buttonText;
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			return states;
		}

		
		// Return the number of actions (Read-Only)
		// Both IInvokeProvider and IToggleProvider have only one action
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
			string keyBinding = null;
			
			if (action != 0)
				return keyBinding;

			keyBinding = (string) 
				provider.GetPropertyValue (AutomationElementIdentifiers.AcceleratorKeyProperty.Id);
			return keyBinding;
		}

		// Get the name of the specified action
		public virtual string GetName (int action)
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
			if (invokeProvider != null) {
				if (action != 0)
					return false;

				OnPressed ();
				invokeProvider.Invoke ();
				OnReleased ();
				
				return true;
			}
			return false;
		}

		
		public override IRawElementProviderSimple Provider {
			get { return provider; }
		}

		public int CaretOffset {
			get {
				return 0;
			}
		}

		public GLib.SList DefaultAttributes {
			get {
				throw new NotImplementedException ();
			}
		}

		public int CharacterCount {
			get {
				return Name.Length;
			}
		}

		public int NSelections {
			get {
				return -1;
			}
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == InvokePatternIdentifiers.InvokedEvent) {
				// TODO: send signal to ATK
			} else if (eventId == AutomationElementIdentifiers.AutomationFocusChangedEvent) {
				// TODO: Handle AutomationFocusChangedEvent
			} else if (eventId == AutomationElementIdentifiers.StructureChangedEvent) {
				// TODO: Handle StructureChangedEvent
			}
			base.RaiseAutomationEvent (eventId, e);
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == TogglePatternIdentifiers.ToggleStateProperty) {
				//if it's a toggle, it should not be a basic Button class, but CheckBox or other
				throw new NotSupportedException ("Toggle events should not land here (should not be reached)");
			} else if (e.Property == AutomationElementIdentifiers.BoundingRectangleProperty) {
				// TODO: Handle BoundingRectangleProperty change
			} else if (e.Property == AutomationElementIdentifiers.NameProperty) {
				string newName = (string)e.NewValue;
				
				// Don't fire spurious events if the text hasn't changed
				if (textExpert.Text == newName)
					return;

				Atk.TextAdapter adapter = new Atk.TextAdapter (this);

				// First delete all text, then insert the new text
				adapter.EmitTextChanged (Atk.TextChangedDetail.Delete, 0, textExpert.Length);

				textExpert = new TextImplementorHelper (newName, this);
				adapter.EmitTextChanged (Atk.TextChangedDetail.Insert, 0,
				                         newName == null ? 0 : newName.Length);

				// Accessible name and label text are one and
				// the same, so update accessible name
				Name = newName;

				EmitVisibleDataChanged ();
			}
			else
				base.RaiseAutomationPropertyChangedEvent (e);
		}
		
		// TODO: although UIA doesn't cover press and release actions, figure out if maybe it's useful to
		// notify the state change, regardless of its actual non-effect
		private void OnPressed ()
		{
			NotifyStateChange (Atk.StateType.Armed, true);
		}
		private void OnReleased ()
		{
			NotifyStateChange (Atk.StateType.Armed, false);
		}
		
		public string GetText (int startOffset, int endOffset)
		{
			return textExpert.GetText (startOffset, endOffset);
		}

		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = textExpert.GetTextAfterOffset (offset, boundaryType, out startOffset, out endOffset);
			selectionStartOffset = startOffset;
			selectionEndOffset = endOffset;
			return ret;
		}

		public string GetTextAtOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = textExpert.GetTextAtOffset (offset, boundaryType, out startOffset, out endOffset);
			selectionStartOffset = startOffset;
			selectionEndOffset = endOffset;
			return ret;
		}

		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = textExpert.GetTextBeforeOffset (offset, boundaryType, out startOffset, out endOffset);
			selectionStartOffset = startOffset;
			selectionEndOffset = endOffset;
			return ret;
		}
		
		public char GetCharacterAtOffset (int offset)
		{
			return textExpert.GetCharacterAtOffset (offset);
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
			GLib.SList attribs = new GLib.SList (typeof (Atk.TextAttribute));
			return attribs;
		}

		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			textExpert.GetCharacterExtents (offset, out x, out y, out width, out height, coords);
		}

		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			throw new NotImplementedException ();
		}

		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			startOffset = selectionStartOffset;
			endOffset = selectionEndOffset;
			return null;
		}

		public bool AddSelection (int startOffset, int endOffset)
		{
			throw new NotImplementedException ();
		}

		public bool RemoveSelection (int selectionNum)
		{
			return false;
		}

		public bool SetSelection (int selectionNum, int startOffset, int endOffset)
		{
			return false;
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
			throw new NotImplementedException ();
		}


#region ImageImplementor implementation 

		string imageDescription = null;

		bool? hasImage = null;
		Mono.UIAutomation.Bridge.IEmbeddedImage embeddedImage = null;
			
		private bool HasImage {
			get {
				if (hasImage == null) {
					//type only available in our Provider implementation
					embeddedImage = invokeProvider as Mono.UIAutomation.Bridge.IEmbeddedImage;
					
					if (embeddedImage == null) {
						Console.WriteLine ("WARNING: your provider implementation doesn't have unofficial IEmbeddedImage support");
						hasImage = false;
					}
					else
						hasImage = embeddedImage.BoundingRectangle.IsEmpty;
				}
				
				return hasImage.Value;
			}
		}
		
		public string ImageDescription
		{
			get { return imageDescription; }
		}
		
		public void GetImageSize (out int width, out int height)
		{
			width = -1;
			height = -1;
			if (HasImage) {
				width = (int)embeddedImage.BoundingRectangle.Width;
				height = (int)embeddedImage.BoundingRectangle.Height;
			}
		}
		
		public void GetImagePosition (out int x, out int y, Atk.CoordType coordType)
		{
			x = int.MinValue;
			y = int.MinValue;
			if (HasImage) {
				x = (int)embeddedImage.BoundingRectangle.X;
				y = (int)embeddedImage.BoundingRectangle.Y;
			}
		}
		
		public bool SetImageDescription (string description)
		{
			imageDescription = description;
			return true;
		}
		
		public string ImageLocale 
		{
			get { throw new System.NotImplementedException (); }
		}
		
#endregion


	}
}
