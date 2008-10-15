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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ListItem;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{

	internal class ListItemProvider : FragmentRootControlProvider
	{

		#region Constructors

		public ListItemProvider (FragmentRootControlProvider rootProvider,
		                         ListProvider provider, 
		                         Control control) 
			: base (control)
		{
			listProvider = provider;
			this.rootProvider = rootProvider;	
		}

		#endregion
		
		#region IRawElementProviderSimple Members
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.ListItem.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "list item";
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return nameProperty;
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return ListProvider.GetPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id);
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id
			         || propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
				return ListProvider.GetItemPropertyValue (this, propertyId);
			else
				return base.GetPropertyValue (propertyId);
		}

		#endregion
		
		#region Public Methods
		
		public void Initialize ()
		{
			SetBehavior (SelectionItemPatternIdentifiers.Pattern,
			             listProvider.GetListItemBehaviorRealization (SelectionItemPatternIdentifiers.Pattern,
			                                                          this));
			SetBehavior (ScrollItemPatternIdentifiers.Pattern,
			             listProvider.GetListItemBehaviorRealization (ScrollItemPatternIdentifiers.Pattern,
			                                                          this));
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
			
			InitializeEvents ();
		}

		public override void InitializeEvents ()
		{
			base.InitializeEvents (); 
			
			nameProperty = (string) ListProvider.GetItemPropertyValue (this, 
			                                                           AutomationElementIdentifiers.NameProperty.Id);
			
			SetEvent (ProviderEventType.AutomationElementIsKeyboardFocusableProperty,
			          listProvider.GetListItemHasKeyboardFocusEvent (this));
			
			//FIXME: Implement this
			SetEvent (ProviderEventType.AutomationFocusChangedEvent,
			          null);
		}
		
		#endregion

		#region Public Properties

		public int Index {
			get { return ListProvider.IndexOfItem (this); }
		}	

		public ListProvider ListProvider {
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
		private ListProvider listProvider;
		private string nameProperty;
		
		#endregion
		
	}
}
