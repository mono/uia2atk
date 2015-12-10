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

using System.Windows.Automation;
using Mono.UIAutomation.Services;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	
	public class ComboBoxItem : ComponentAdapter, Atk.IActionImplementor, Atk.ITextImplementor
	{
		ITextImplementor textExpert = null;
		ISelectionItemProvider selectionItemProvider = null;
		
		public ComboBoxItem (IRawElementProviderSimple provider) : base (provider)
		{
			int controlType = (int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			selectionItemProvider = (ISelectionItemProvider)provider.GetPatternProvider (
			  SelectionItemPatternIdentifiers.Pattern.Id);
			if (selectionItemProvider == null)
				throw new ArgumentException (
				  String.Format ("Provider for ComboBoxItem (control type {0}) should implement ISelectionItemProvider", controlType));

			textExpert = TextImplementorFactory.GetImplementor (this, provider);
			
			//FIXME: take in account ComboBox style changes at runtime
			if (ParentIsSimple ())
				Role = Atk.Role.TableCell;
			else
				Role = Atk.Role.MenuItem;
		}

		private bool Selected {
			get {
				object isSelectedVal = Provider.GetPropertyValue (
				  SelectionItemPatternIdentifiers.IsSelectedProperty.Id);
				return isSelectedVal is bool && (bool) isSelectedVal;
			}
		}

		public bool ParentIsSimple ()
		{
			//FIXME: change this not to use Provider API when we fix the FIXME in Adapter ctor. (just use Parent.IsSimple())
			IRawElementProviderSimple parentProvider =
			  ((IRawElementProviderFragment) Provider).Navigate (NavigateDirection.Parent);
			parentProvider = ((IRawElementProviderFragment) parentProvider).Navigate (NavigateDirection.Parent);
			return ComboBox.IsSimple (parentProvider);
		}

		private bool showing = false;
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			if (states.ContainsState (Atk.StateType.Defunct))
				return states;

			states.RemoveState (Atk.StateType.Focusable);
			
			showing = states.ContainsState (Atk.StateType.Showing);
			
			states.AddState (Atk.StateType.Selectable);
			if (showing || Selected) {
				states.AddState (Atk.StateType.Showing);
			} else {
				states.RemoveState (Atk.StateType.Showing);
			}

			if (Parent != null && (Parent.Parent is ComboBoxDropDown) &&
			    (Parent.Parent.RefStateSet ().ContainsState (Atk.StateType.Visible)))
				states.AddState (Atk.StateType.Visible);
			
			if (Selected) {
				states.AddState (Atk.StateType.Selected);
				states.AddState (Atk.StateType.Focused);
			} else {
				states.RemoveState (Atk.StateType.Selected);
			}

			return states;
		}

		public override Atk.Layer Layer {
			get { return Atk.Layer.Popup; }
		}

		internal void Deselect ()
		{
			NotifyStateChange (Atk.StateType.Selected, false);
			//FIXME: shouldn't we call selectionItemProvider.RemoveFromSelection (); ?
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == InvokePatternIdentifiers.InvokedEvent) {
				NotifyStateChange (Atk.StateType.Selected, Selected);
				NotifyFocused (Selected);
			} else if (eventId == SelectionItemPatternIdentifiers.ElementSelectedEvent) {
				NotifyStateChange (Atk.StateType.Selected, Selected);
				NotifyFocused (Selected);
				((ComboBoxOptions)Parent).RecursiveDeselect (this);
			} else {
				Log.Warn ("ComboBoxItem: RaiseAutomationEvent({0},...) not implemented", eventId.ProgrammaticName);
				base.RaiseAutomationEvent (eventId, e);
			}
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property.Id == SelectionItemPatternIdentifiers.IsSelectedProperty.Id) {
				NotifyStateChange (Atk.StateType.Selected, Selected);
			} else if (e.Property.Id == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
				showing = !((bool)e.NewValue);
				NotifyStateChange (Atk.StateType.Showing, showing);
			} else {
				base.RaiseAutomationPropertyChangedEvent (e);
			}
		}

		#region Action implementation 

		private string actionDescription = null;
		
		public bool DoAction (int i)
		{
			if (i == 0) {
				try {
					selectionItemProvider.Select ();
					return true;
				} catch (ElementNotEnabledException e) {
					Log.Debug (e);
				}
			}
			return false;
		}
		
		public string GetName (int i)
		{
			if (i == 0)
				return "click";
			return null;
		}
		
		public string GetKeybinding (int i)
		{
			return null;
		}
		
		public string GetLocalizedName (int i)
		{
			return null;
		}
		
		public bool SetDescription (int i, string desc)
		{
			if (i == 0) {
				actionDescription = desc;
				return true;
			}
			return false;
		}
		
		public string GetDescription (int i)
		{
			if (i == 0)
				return actionDescription;
			return null;
		}

		
		public int NActions {
			get { return 1; }
		}
		
		#endregion 


		#region TextImplementor implementation 
		
		public string GetText (int startOffset, int endOffset)
		{
			return textExpert.GetText (startOffset, endOffset);
		}
		
		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			return textExpert.GetTextAfterOffset (offset, boundaryType, out startOffset, out endOffset);
		}
		
		public string GetTextAtOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			return textExpert.GetTextAtOffset (offset, boundaryType, out startOffset, out endOffset);
		}
		
		public char GetCharacterAtOffset (int offset)
		{
			return textExpert.GetCharacterAtOffset (offset);
		}
		
		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			return textExpert.GetTextBeforeOffset (offset, boundaryType, out startOffset, out endOffset);
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
			return textExpert.GetOffsetAtPoint (x, y, coords);
		}
		
		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			return textExpert.GetSelection (selectionNum, out startOffset, out endOffset);
		}
		
		public bool AddSelection (int startOffset, int endOffset)
		{
			return false;
		}

		public bool RemoveSelection (int i)
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
			return textExpert.GetBoundedRanges (rect, coordType, xClipType, yClipType);
		}
		
		public int CaretOffset {
			get { return 0; }
		}
		
		public Atk.Attribute [] DefaultAttributes {
			get { return textExpert.DefaultAttributes; }
		}
		
		public int CharacterCount {
			get { return textExpert.Length; }
		}
		
		public int NSelections {
			get { return -1; }
		}
		
		#endregion
		
	}
}
