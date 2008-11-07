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
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	
	public class MenuItem : ComponentParentAdapter, Atk.SelectionImplementor, 
	                        Atk.ActionImplementor, Atk.TextImplementor
	{
		bool? comboBoxStructure = null;
		TextImplementorHelper textExpert = null;
		IInvokeProvider invokeProvider = null;
		
		public MenuItem (IRawElementProviderSimple provider) : base (provider)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");

			if ((provider as IRawElementProviderFragment) == null)
				throw new ArgumentException ("Provider for ParentMenu should be IRawElementProviderFragment");

			string name = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			if (!String.IsNullOrEmpty (name))
				Name = name;

			textExpert = new TextImplementorHelper (Name);
			
			comboBoxStructure = ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) 
			  == ControlType.List.Id);

			if (!comboBoxStructure.Value) {
				invokeProvider = (IInvokeProvider)provider.GetPatternProvider(InvokePatternIdentifiers.Pattern.Id);
				if (invokeProvider == null)
					throw new ArgumentException ("Provider for ParentMenu should implement IInvokeProvider");
			}
			
			IRawElementProviderFragment child = ((IRawElementProviderFragment)provider).Navigate (NavigateDirection.FirstChild);

			//FIXME: take in account Role changes at runtime
			Role = (child != null || comboBoxStructure.Value) ? Atk.Role.Menu : Atk.Role.MenuItem;
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			//FIXME: figure out why MenuItem elements in Gail don't like this state
			// (maybe because they replace it with Selectable?)
			if (states.ContainsState (Atk.StateType.Focusable)) {
				states.RemoveState (Atk.StateType.Focusable);
				states.AddState (Atk.StateType.Selectable);
			}
			return states;
		}

		public override Atk.Layer Layer {
			get { return Atk.Layer.Popup; }
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			Console.WriteLine ("WARNING: RaiseAutomationEvent({0},...) not handled yet", eventId.ProgrammaticName);
		}
		
		public int SelectionCount {
			get {
				throw new NotImplementedException ();
			}
		}
		
		public bool AddSelection (int i)
		{
			throw new NotImplementedException ();
		}

		public bool ClearSelection ()
		{
			throw new NotImplementedException ();
		}

		public Atk.Object RefSelection (int i)
		{
			throw new NotImplementedException ();
		}

		public bool IsChildSelected (int i)
		{
			throw new NotImplementedException ();
		}

		public bool RemoveSelection (int i)
		{
			return false;
		}

		public bool SelectAllSelection ()
		{
			throw new NotImplementedException ();
		}
		
		public override void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			throw new NotImplementedException ();
		}

		#region Action implementation 

		private string actionDescription = null;
		
		public bool DoAction (int i)
		{
			if (i == 0) {
				if (invokeProvider != null)
				try {
					invokeProvider.Invoke ();
					return true;
				}
				catch (ElementNotEnabledException) { }
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
		
		public GLib.SList GetRunAttributes (int offset, out int startOffset, out int endOffset)
		{
			throw new NotImplementedException ();
		}
		
		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			throw new NotImplementedException ();
		}
		
		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			throw new NotImplementedException ();
		}
		
		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			return textExpert.GetSelection (selectionNum, out startOffset, out endOffset);
		}
		
		public bool AddSelection (int startOffset, int endOffset)
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
			throw new NotImplementedException ();
		}
		
		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coordType, Atk.TextClipType xClipType, Atk.TextClipType yClipType)
		{
			throw new NotImplementedException ();
		}
		
		public int CaretOffset {
			get { return 0; }
		}
		
		public GLib.SList DefaultAttributes {
			get {
				throw new NotImplementedException ();
			}
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
