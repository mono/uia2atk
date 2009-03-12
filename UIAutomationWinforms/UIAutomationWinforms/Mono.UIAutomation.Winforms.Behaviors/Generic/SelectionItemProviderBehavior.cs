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
// Copyright (c) 2008,2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Behaviors.Generic
{
	//NOTE: 
	//     About exceptions thrown: http://msdn.microsoft.com/en-us/library/ms749016.aspx
	internal abstract class SelectionItemProviderBehavior<T>
		: ProviderBehavior, ISelectionItemProvider
			where T : FragmentControlProvider, ISelectableItem
	{
		
		#region Constructors

		protected SelectionItemProviderBehavior (T provider)
			: base (provider)
		{
			itemProvider = provider;
		}
		
		#endregion
		
		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern { 
			get { return SelectionItemPatternIdentifiers.Pattern; }
		}		

		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementSelectedEvent, 
			                   null);
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementAddedEvent, 
			                   null);
			Provider.SetEvent (ProviderEventType.SelectionItemPatternElementRemovedEvent, 
			                   null);
			Provider.SetEvent (ProviderEventType.SelectionItemPatternIsSelectedProperty, 
			                   null);
			Provider.SetEvent (ProviderEventType.SelectionItemPatternSelectionContainerProperty,
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == SelectionItemPatternIdentifiers.IsSelectedProperty.Id)
				return IsSelected;
			else if (propertyId == SelectionItemPatternIdentifiers.SelectionContainerProperty.Id)
				return SelectionContainer;
			else
				return null;
		}
		
		#endregion

		#region ISelectionItemProvider Interface
		
		public void AddToSelection ()
		{
			if (IsSelected)
				return;

			ISelectionProvider selectionProvider = SelectionProvider;
			
			if (!selectionProvider.CanSelectMultiple) {
				if (selectionProvider.GetSelection ().Length > 0)
					throw new InvalidOperationException ();
				else
					Select ();
			} else
				Select ();
		}

		public void RemoveFromSelection ()
		{
			if (!IsSelected)
				return;

			ISelectionProvider selectionProvider = SelectionProvider;
			int selectedItemsCount = selectionProvider.GetSelection ().Length;

			if (!selectionProvider.CanSelectMultiple
			    && selectionProvider.IsSelectionRequired
			    && selectedItemsCount > 0) 
				throw new InvalidOperationException ();	
			else if (selectionProvider.CanSelectMultiple 
			         && selectionProvider.IsSelectionRequired
			         && selectedItemsCount == 1)
				throw new InvalidOperationException ();	
			else
				PerformUnselect ();
		}

		public void Select ()
		{
			if (IsSelected)
				return;
			
			if (itemProvider.ContainerControl.InvokeRequired) {
				itemProvider.ContainerControl.BeginInvoke (new MethodInvoker (Select));
				return;
			}
			itemProvider.Select ();
		}

		public bool IsSelected {
			get { return itemProvider.Selected; }
		}

		public IRawElementProviderSimple SelectionContainer {
			get { return itemProvider.SelectionContainer; }
		}

		#endregion

		#region Protected Properties

		protected T ItemProvider {
			get { return itemProvider; }
		}

		#endregion
		
		#region Private Fields

		private ISelectionProvider SelectionProvider {
			get {
				ISelectionProvider selectionProvider
					= itemProvider.SelectionContainer.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id)
						 as ISelectionProvider;
				if (selectionProvider == null)
					throw new ArgumentException (string.Format ("itemProvider.SelectionContainer MUST implement SelectionPattern: {0}",
					                                            itemProvider.GetType ()));

				return selectionProvider;
			}
		}

		private void PerformUnselect ()
		{
			if (itemProvider.ContainerControl.InvokeRequired) {
				itemProvider.ContainerControl.BeginInvoke (new MethodInvoker (PerformUnselect));
				return;
			}
			itemProvider.Unselect ();
		}
		
		private T itemProvider;
		
		#endregion
		
	}
}
