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
	
	public class ComboBoxOptions : ComponentParentAdapter, Atk.SelectionImplementor, Atk.TextImplementor
	{
		TextImplementorHelper textExpert = null;
		ISelectionProvider selectionProvider = null;
		SelectionProviderUserHelper	selectionHelper;
		
		public ComboBoxOptions (IRawElementProviderSimple provider) : base (provider)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");

			if ((provider as IRawElementProviderFragment) == null)
				throw new ArgumentException ("Provider for ParentMenu should be IRawElementProviderFragment");

			selectionProvider = (ISelectionProvider)provider.GetPatternProvider(SelectionPatternIdentifiers.Pattern.Id);
			if (selectionProvider == null)
				throw new ArgumentException ("The List inside the ComboBox should always implement ISelectionProvider");

			selectionHelper = new SelectionProviderUserHelper (provider as IRawElementProviderFragment, selectionProvider);
			
			string name = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			textExpert = new TextImplementorHelper (name, this);

			//FIXME: take in account ComboBox style changes at runtime
			if (ParentIsSimple ())
				Role = Atk.Role.TreeTable;
			else
				Role = Atk.Role.Menu;
		}

		public bool ParentIsSimple ()
		{
			//FIXME: change this not to use Provider API when we fix the FIXME in Adapter ctor. (just use Parent.IsSimple())
			IRawElementProviderSimple parentProvider =
			  ((IRawElementProviderFragment) Provider).Navigate (NavigateDirection.Parent);
			return ComboBox.IsSimple (parentProvider);
		}

		#region SelectionImplementor implementation //FIXME: consider making ComboBoxOptions inherit from List
		
		public int SelectionCount {
			get { return selectionHelper.SelectionCount; }
		}
		
		public virtual bool AddSelection (int i)
		{
			return selectionHelper.AddSelection (i);
			//return ((ComboBoxItem)RefAccessibleChild (i)).DoAction (0);
		}

		public virtual bool ClearSelection ()
		{
			return selectionHelper.ClearSelection ();
			//in the past, we did this because ComboBox doesn't support this: return (SelectionCount == 0);
		}

		public Atk.Object RefSelection (int i)
		{
			return selectionHelper.RefSelection (i);
		}

		public bool IsChildSelected (int i)
		{
			return selectionHelper.IsChildSelected (i);
		}

		public bool RemoveSelection (int i)
		{
			return selectionHelper.RemoveSelection (i);
			//in the past, we did this because ComboBox doesn't support this: return false;
		}

		public bool SelectAllSelection ()
		{
			return selectionHelper.SelectAllSelection ();
			//in the past, we did this because ComboBox doesn't support this: return false;
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
			return textExpert.GetRunAttributes (offset, out startOffset, out endOffset);
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
			textExpert.GetRangeExtents (startOffset, endOffset, coordType, out rect);
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

		public override void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			//TODO
		}

		int selectedChild = -1;
		
		internal void RecursiveDeselect (Adapter keepSelected)
		{
			lock (syncRoot) {
				for (int i = 0; i < NAccessibleChildren; i++) {
					Atk.Object child = RefAccessibleChild (i);

					if (child == null || ((Adapter)child) == keepSelected)  {
						selectedChild = i;
						continue;
					}
					
					ComboBoxItem item = child as ComboBoxItem;
					if (item != null)
						item.Deselect ();
				}
			}

			if (Parent is ComboBoxOptions)
				((ComboBoxOptions)Parent).RecursiveDeselect (keepSelected);
			else if (Parent is ComboBox)
				((ComboBox)Parent).RaiseSelectionChanged (keepSelected.Name);
		}

		internal void RaiseExpandedCollapsed ()
		{
			NotifyStateChange (Atk.StateType.Showing);
			NotifyStateChange (Atk.StateType.Visible);
		}

		internal override void RemoveChild (Atk.Object childToRemove)
		{
			Console.WriteLine ("______________RemoveChild in ComboBoxOptions");
			int childIndex = children.IndexOf (childToRemove);
			bool cancelSelection = false;
			if (IsChildSelected (childIndex))
				cancelSelection = true; else Console.WriteLine ("______________RemoveChild in ComboBoxOptions NO");
			base.RemoveChild (childToRemove);
			if (children.Count <= 0 || cancelSelection)
				((ComboBox)Parent).RaiseSelectionChanged (null);
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == AutomationElementIdentifiers.NameProperty)
				return;
			base.RaiseAutomationPropertyChangedEvent (e);
		}
	}
}
