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
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{

	public class ListItem : ComponentAdapter, Atk.TextImplementor, Atk.ActionImplementor
	{
		private IRawElementProviderSimple provider;
		private IInvokeProvider				invokeProvider;
		private TextImplementorHelper textExpert = null;
		private string						selectActionDescription = null;
		private string						invokeActionDescription = null;
		protected string					selectActionName = "click";
		protected string					invokeActionName = "invoke";

		public ListItem (IRawElementProviderSimple provider)
		{
			this.provider = provider;
			invokeProvider = (IInvokeProvider)provider.GetPatternProvider(InvokePatternIdentifiers.Pattern.Id);
			string text = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			textExpert = new TextImplementorHelper (text);
			Name = text;
			Role = Atk.Role.ListItem;
		}

		public override IRawElementProviderSimple Provider {
			get { return provider; }
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			states.AddState (Atk.StateType.Selectable);
			bool enabled = (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			if (enabled)
			{
				states.AddState (Atk.StateType.Sensitive);
				states.AddState (Atk.StateType.Enabled);
			}
			else
			{
				states.RemoveState (Atk.StateType.Sensitive);
				states.RemoveState (Atk.StateType.Enabled);
			}
			return states;
		}

		public string GetText (int startOffset, int endOffset)
		{
			return textExpert.GetText (startOffset, endOffset);
		}

		private int selectionStartOffset = 0, selectionEndOffset = 0;
		
		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = 
				textExpert.GetTextAfterOffset (offset, boundaryType, out startOffset, out endOffset);
			selectionStartOffset = startOffset;
			selectionEndOffset = endOffset;
			return ret;
		}
		
		public string GetTextAtOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = 
				textExpert.GetTextAtOffset (offset, boundaryType, out startOffset, out endOffset);
			selectionStartOffset = startOffset;
			selectionEndOffset = endOffset;
			return ret;
		}
		
		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = 
				textExpert.GetTextBeforeOffset (offset, boundaryType, out startOffset, out endOffset);
			selectionStartOffset = startOffset;
			selectionEndOffset = endOffset;
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
			throw new NotImplementedException();
		}

		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			throw new NotImplementedException();
		}

		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			startOffset = selectionStartOffset;
			endOffset = selectionEndOffset;
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

		public void GetRangeExtents (int startOffset, int endOffset, Atk.CoordType coordType, Atk.TextRectangle rect)
		{
			throw new NotImplementedException();
		}

		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coordType, Atk.TextClipType xClipType, Atk.TextClipType yClipType)
		{
			throw new NotImplementedException();
		}
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			// TODO
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
				return Name.Length;
			}
		}

		// Return the number of actions (Read-Only)
		public int NActions
		{
			get {
				return (invokeProvider != null? 2: 1);
			}
		}
		
		// Get a localized name for the specified action
		public string GetLocalizedName (int action)
		{
			// TODO: Localize the name?s
			switch (action) {
			case 0:
			return selectActionName;
			case 1:
				return (invokeProvider != null? invokeActionName: null);
			default:
				return null;
			}
		}
		
		// Sets a description of the specified action
		public bool SetDescription (int action, string description)
		{
			switch (action) {
			case 0:
				selectActionDescription = description;
				return true;
			case 1:
				if (invokeProvider == null)
					return false;
				invokeActionDescription = description;
				return true;
			default:
				return false;
			}
		}
		
		// Get the key bindings for the specified action
		public string GetKeybinding (int action)
		{
			return null;
		}

		// Get the name of the specified action
		public virtual string GetName (int action)
		{
			switch (action) {
			case 0:
				return selectActionName;
			case 1:
				return (invokeProvider != null? invokeActionName: null);
			default:
				return null;
			}
		}
		
		// Get the description of the specified action
		public string GetDescription (int action)
		{
			switch (action) {
			case 0:
				return selectActionDescription;
			case 1:
				return (invokeProvider != null? invokeActionDescription: null);
			default:
				return null;
			}
		}

		// Perform the action specified
		public virtual bool DoAction (int action)
		{
			switch (action) {
			case 0:
				Atk.SelectionImplementor sel = Parent as Atk.SelectionImplementor;
				if (sel == null)
					return false;
				return sel.AddSelection (IndexInParent);
			case 1:
				if (invokeProvider == null)
					return false;
				invokeProvider.Invoke();
				return true;
			default:
				return false;
			}
		}
		public int NSelections {
			get {
				return 0;
			}
		}
		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == ValuePatternIdentifiers.ValueProperty)
			{
				String stringValue = (String)e.NewValue;
				textExpert = new TextImplementorHelper (stringValue);
				Name = stringValue;
			} else if (e.Property == AutomationElementIdentifiers.IsEnabledProperty) {
				if((bool)e.NewValue)
				{
					OnEnabled ();
				}
				else
				{
					OnDisabled ();
				}
			}
		}

		private void OnEnabled ()
		{
			NotifyStateChange ((ulong) Atk.StateType.Sensitive, true);
			NotifyStateChange ((ulong) Atk.StateType.Enabled, true);
		}

		private void OnDisabled ()
		{
			NotifyStateChange ((ulong) Atk.StateType.Sensitive, false);
			NotifyStateChange ((ulong) Atk.StateType.Enabled, false);
		}
	}
}
