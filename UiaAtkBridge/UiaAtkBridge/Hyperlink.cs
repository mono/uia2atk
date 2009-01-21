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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Collections.Generic;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Bridge;

namespace UiaAtkBridge
{

	public class Hyperlink : ComponentParentAdapter , Atk.TextImplementor, Atk.HypertextImplementor
	{
		private IRawElementProviderSimple provider;
		private IInvokeProvider invokeProvider;
		internal IHypertext hypertext;
		
		private ITextImplementor textExpert = null;
		
		private List<HyperlinkObject> links;

		public Hyperlink (IRawElementProviderSimple provider) : base (provider)
		{
			this.provider = provider;
			invokeProvider = (IInvokeProvider)provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			hypertext = (IHypertext)invokeProvider;
			links = new List<HyperlinkObject> ();
			Role = Atk.Role.Label;
			
			textExpert = TextImplementorFactory.GetImplementor (this, provider);
			Name = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			states.AddState (Atk.StateType.MultiLine);

			bool canFocus = (bool) Provider.GetPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id);
			if (canFocus)
				states.AddState (Atk.StateType.Focusable);
			else
				states.RemoveState (Atk.StateType.Focusable);

			return states;
		}
		
		public int CaretOffset {
			get {
				return 0;
			}
		}

		public Atk.Attribute [] DefaultAttributes {
			get {
				return textExpert.DefaultAttributes;
			}
		}

		public int CharacterCount {
			get {
				return textExpert.Length;
			}
		}

		public int NSelections {
			get {
				return 0;
			}
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == TextPatternIdentifiers.TextChangedEvent) {
				string newText = provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id) as string;
				
				// Don't fire spurious events if the text hasn't changed
				if (textExpert.Text == newText) {
					return;
				}

				Atk.TextAdapter adapter = new Atk.TextAdapter (this);

				// First delete all text, then insert the new text
				adapter.EmitTextChanged (Atk.TextChangedDetail.Delete, 0, textExpert.Length);

				adapter.EmitTextChanged (Atk.TextChangedDetail.Insert, 0,
				                         newText == null ? 0 : newText.Length);

				// Accessible name and label text are one and
				// the same, so update accessible name
				Name = newText;

				EmitVisibleDataChanged ();
			}
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
		
		public Atk.Attribute [] GetRunAttributes (int offset, out int startOffset, out int endOffset)
		{
			return textExpert.GetRunAttributes (offset, out startOffset, out endOffset);
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

		public void GetRangeExtents (int startOffset, int endOffset, Atk.CoordType coordType, out Atk.TextRectangle rect)
		{
			textExpert.GetRangeExtents (startOffset, endOffset, coordType, out rect);
		}

		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coordType, Atk.TextClipType xClipType, Atk.TextClipType yClipType)
		{
			throw new NotImplementedException();
		}
		
		public Atk.Hyperlink GetLink (int link_index)
		{
			if (link_index < 0 || link_index >= links.Count)
				return null;
			return links [link_index];
		}

		public int NLinks {
			get {
				AdjustLinkObjects ();
				return links.Count;
			}
		}

		protected override int OnGetNChildren ()
		{
			AdjustLinkObjects ();
			return base.OnGetNChildren ();
		}

		// TODO: We really should have an event instead of doing this;
		// would reduce the likelihood of a race condition
		private void AdjustLinkObjects ()
		{
			while (links.Count > hypertext.NumberOfLinks)
				links.RemoveAt (links.Count - 1);
			while (links.Count < hypertext.NumberOfLinks)
				links.Add (new HyperlinkObject (this, links.Count));
		}

		public int GetLinkIndex (int char_index)
		{
			for (int i = 0; i < links.Count; i++)
				if (hypertext.Start (i) <= char_index && (hypertext.Start (i) + hypertext.Length (i)) > char_index)
					return i;
			return -1;
		}
				
		public override void RaiseStructureChangedEvent (object childProvider, StructureChangedEventArgs e)
		{
		}
		
		public override Atk.Layer Layer {
			get { return Atk.Layer.Window; }
		}
		
		public override int MdiZorder {
			get { return -1; }
		}
	}

	public class HyperlinkObject : Atk.Hyperlink
	{
		Hyperlink resource;
		private int index;
		private Atk.Object obj;

		public HyperlinkObject (Hyperlink resource, int index)
		{
			this.resource = resource;
			this.index = index;
			this.obj = new HyperlinkActor (resource, index);
		}

		protected override string OnGetUri (int i)
		{
			return (i == 0? resource.hypertext.Uri (index): null);
		}

		protected override Atk.Object OnGetObject (int i)
		{
			return (i == 0? obj: null);
		}

		protected override int OnGetEndIndex ()
		{
			return resource.hypertext.Start (index) + resource.hypertext.Length (index);
		}

		protected override int OnGetStartIndex ()
		{
			return resource.hypertext.Start (index);
		}

		protected override int OnGetNAnchors ()
		{
			return 1;
		}

	}

	class HyperlinkActor: Adapter, Atk.ActionImplementor
	{
		private Hyperlink hyperlink;
		private int index;
		private string					actionDescription = null;
		static string					actionName = "jump";

		public HyperlinkActor (Hyperlink parent, int index) : base (parent.Provider)
		{
			Role = Atk.Role.PushButton;
			this.Parent = parent;
			this.hyperlink = parent;
			this.index = index;
			parent.AddOneChild (this);
			Name = hyperlink.hypertext.Uri (index);
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			bool enabled = hyperlink.hypertext.Enabled (index);
			if (enabled) {
				states.AddState (Atk.StateType.Sensitive);
				states.AddState (Atk.StateType.Enabled);
			} else {
				states.RemoveState (Atk.StateType.Sensitive);
				states.RemoveState (Atk.StateType.Enabled);
			}

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
			return null;
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
			if (action != 0)
				return false;
			try {
				hyperlink.hypertext.Invoke (index);
			} catch (ElementNotEnabledException) {
				return false;
			}
			return true;
		}

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
		}
	}
}
