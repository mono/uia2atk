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
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.Unix;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Navigation;
using Mono.UIAutomation.Bridge;

namespace Mono.UIAutomation.Winforms
{

	internal class ListItemProvider : FragmentRootControlProvider, ISelectableItem
	{

		#region Constructors

		public ListItemProvider (FragmentRootControlProvider rootProvider,
		                         IListProvider provider, 
		                         Control control,
		                         object objectItem)
			: base (control)
		{
			listProvider = provider;
			this.rootProvider = rootProvider;
			this.objectItem = objectItem;
		}

		#endregion

		#region ISelectionItemProvider realization

		public virtual Control ContainerControl {
			get { return ListProvider.Control; }
		}

		public IRawElementProviderSimple SelectionContainer { 
			get { return ListProvider; }
		}
		
		public bool Selected { 
			get { return ListProvider.IsItemSelected (this); }
		}

		public void Select ()
		{
			ListProvider.SelectItem (this);
		}
		
		public void Unselect ()
		{
			ListProvider.UnselectItem (this);
		}

		#endregion
		
		#region IRawElementProviderSimple Members
		
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.ListItem.Id;
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return ListProvider.GetPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id);
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id
			         || propertyId == AutomationElementIdentifiers.NameProperty.Id
			         || propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
				return ListProvider.GetItemPropertyValue (this, propertyId);
			else if (propertyId == AutomationElementIdentifiers.NativeWindowHandleProperty.Id)
				return null;
			else
				return base.GetProviderPropertyValue (propertyId);
		}

		protected override Rect BoundingRectangleProperty {
			get {
				var rect = ListProvider.GetItemPropertyValue (
					this, AutomationElementIdentifiers.BoundingRectangleProperty.Id);
				return rect != null ? (Rect) rect : new Rect(Double.NaN, Double.NaN, Double.NaN, Double.NaN);
			}
		}

		#endregion
		
		#region Public Methods

		public override void Initialize ()
		{
			//Behaviors
			SetBehavior (SelectionItemPatternIdentifiers.Pattern,
			             listProvider.GetListItemBehaviorRealization (SelectionItemPatternIdentifiers.Pattern,
			                                                          this));

			Action updateScrollItemPattern = () => {
				SetBehavior (ScrollItemPatternIdentifiers.Pattern,
					listProvider.GetListItemBehaviorRealization (
						ScrollItemPatternIdentifiers.Pattern,
						this));
			};
			updateScrollItemPattern ();
			listProvider.ScrollPatternSupportChanged += updateScrollItemPattern;

			SetBehavior (TogglePatternIdentifiers.Pattern,
			             listProvider.GetListItemBehaviorRealization (TogglePatternIdentifiers.Pattern,
			                                                          this));
			SetBehavior (ExpandCollapsePatternIdentifiers.Pattern,
			             listProvider.GetListItemBehaviorRealization (ExpandCollapsePatternIdentifiers.Pattern,
			                                                          this));
			SetBehavior (ValuePatternIdentifiers.Pattern,
			             listProvider.GetListItemBehaviorRealization (ValuePatternIdentifiers.Pattern,
			                                                          this));
			SetBehavior (GridItemPatternIdentifiers.Pattern,
			             listProvider.GetListItemBehaviorRealization (GridItemPatternIdentifiers.Pattern,
			                                                          this));
			SetBehavior (InvokePatternIdentifiers.Pattern,
			             listProvider.GetListItemBehaviorRealization (InvokePatternIdentifiers.Pattern,
			                                                          this));
			//When ListItem is ControlType.DataItem
			SetBehavior (TableItemPatternIdentifiers.Pattern,
			             listProvider.GetListItemBehaviorRealization (TableItemPatternIdentifiers.Pattern,
			                                                          this));
			//When supports an image (Internal Pattern)
			SetBehavior (EmbeddedImagePatternIdentifiers.Pattern,
			             listProvider.GetListItemBehaviorRealization (EmbeddedImagePatternIdentifiers.Pattern,
			                                                          this));
			//When supports clipboard (Internal Pattern)
			SetBehavior (ClipboardPatternIdentifiers.Pattern,
			             listProvider.GetListItemBehaviorRealization (ClipboardPatternIdentifiers.Pattern,
			                                                          this));
			
			// Default Events
			SetEvent (ProviderEventType.AutomationElementControlTypeProperty,
			          new AutomationControlTypePropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementIsPatternAvailableProperty,
			          new AutomationIsPatternAvailablePropertyEvent (this));

			// Specific Events
			SetEvent (ProviderEventType.AutomationElementIsKeyboardFocusableProperty,
			          listProvider.GetListItemEventRealization (ProviderEventType.AutomationElementIsKeyboardFocusableProperty,
			                                                    this));
			SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
			          listProvider.GetListItemEventRealization (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
			                                                    this));
			SetEvent (ProviderEventType.AutomationFocusChangedEvent,
			          listProvider.GetListItemEventRealization (ProviderEventType.AutomationFocusChangedEvent,
			                                                    this));
			SetEvent (ProviderEventType.AutomationElementIsOffscreenProperty,
			          listProvider.GetListItemEventRealization (ProviderEventType.AutomationElementIsOffscreenProperty,
			                                                    this));
			SetEvent (ProviderEventType.AutomationElementBoundingRectangleProperty,
			          listProvider.GetListItemEventRealization (ProviderEventType.AutomationElementBoundingRectangleProperty,
			                                                    this));
		}

		protected override void InitializeChildControlStructure ()
		{
			// This way we don't use our Control to define events twice
		}

		public void UpdateBehavior (AutomationPattern pattern)
		{			
			SetBehavior (pattern,
			             listProvider.GetListItemBehaviorRealization (pattern,
			                                                          this));
		}
		
		public override void SetFocus ()
		{
			((IRawElementProviderFragment) listProvider).SetFocus ();
			listProvider.FocusItem (ObjectItem);
		}

		#endregion

		#region Public Properties

		public object ObjectItem {
			get { return objectItem; }
		}

		public int Index {
			get { return ListProvider.IndexOfObjectItem (ObjectItem); }
		}

		public IListProvider ListProvider {
			get { return listProvider; }
		}

		#endregion	

		#region IRawElementProviderFragment Interface 

		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { return rootProvider; }
		}

		#endregion
		
		#region Private Fields

		private FragmentRootControlProvider rootProvider;
		private IListProvider listProvider;
		private object objectItem;
		
		#endregion
		
	}
}
