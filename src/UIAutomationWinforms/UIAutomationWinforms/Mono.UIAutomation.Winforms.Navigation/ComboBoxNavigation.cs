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
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms;

namespace Mono.UIAutomation.Winforms.Navigation
{
	
	//Navigation tree has the following leafs:
	// 1. TextBox - TextBox Provider (may not visible)
	// 2. ComboBoxItem - ComboBoxItemProvider (0 to n) 
	// 3. Button - ButtonProvider (may not be visible)
	internal class ComboBoxNavigation : SimpleNavigation
	{

#region Constructor
		
		public ComboBoxNavigation (ComboBoxProvider provider)
			: base (provider)
		{
			this.provider = provider;

			navigation = new NavigationChain ();
			listbox_navigation = new ComboBoxListBoxNavigation (provider);

			((ComboBox) provider.Control).DropDownStyleChanged 
				+= new EventHandler (OnDropDownStyleChanged);
			UpdateNavigation ((ComboBox) provider.Control, provider);
		}

#endregion
		
#region Public Methods
		
		public override void FinalizeProvider ()
		{
			((ComboBox) provider.Control).DropDownStyleChanged 
				-= new EventHandler (OnDropDownStyleChanged);
			
			base.FinalizeProvider (); 
		}
		
#endregion
		
#region INavigable Interface
		
		public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
		{
			if (direction == NavigateDirection.FirstChild) {
				INavigation first = navigation.GetFirstLink ();
				return first != null ? (IRawElementProviderFragment) first.Provider : null;
			} else if (direction == NavigateDirection.LastChild) {
				INavigation last = navigation.GetLastLink ();
				return last != null ? (IRawElementProviderFragment) last.Provider : null;
			} else
				return base.Navigate (direction);
		}

#endregion
		
#region Private Methods

		private void OnDropDownStyleChanged (object sender, EventArgs args)
		{
			UpdateNavigation ((ComboBox) sender, provider);
		}		
		
		//TODO: Should this method generated StructureChanged events?
		//UISpy reports events, however doesn't report ChildRemoved only
		//ChildAdded. More tests needed.
		private void UpdateNavigation (ComboBox combobox, ComboBoxProvider provider)
		{
			if (combobox.DropDownStyle == ComboBoxStyle.Simple) {
				if (button_navigation != null) {
					//TODO: UISpy doesn't report this structure, however, according
					//to MSDN we should do so
					Helper.RaiseStructureChangedEvent (StructureChangeType.ChildRemoved,
					                                   (IRawElementProviderFragment) button_navigation.Provider);
					button_navigation.FinalizeProvider ();
					button_navigation = null;
				}
				//if (textbox_navigation == null)
				//	textbox_navigation = new ComboBoxTextBoxNavigation (provider);
			} else if (combobox.DropDownStyle == ComboBoxStyle.DropDown) {
				if (button_navigation == null) {
					button_navigation = new ComboBoxButtonNavigation (provider);
					//TODO: UISpy doesn't report this structure, however, according
					//to MSDN we should do so
					Helper.RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
					                                   (IRawElementProviderFragment) button_navigation.Provider);
				}
				//if (textbox_navigation != null) {
				//	DISPOSE textbox
				//	textbox_navigation = null;
				//}
			}

			navigation.Clear ();
			//if (textbox_navigation != null)
			//	navigation.AddLink (textbox_navigation);
			navigation.AddLink (listbox_navigation);
			if (button_navigation != null)
				navigation.AddLink (button_navigation);
		}
		
#endregion

#region Private members

		private NavigationChain navigation;
		private ComboBoxProvider provider;
		private ComboBoxButtonNavigation button_navigation;
		private ComboBoxListBoxNavigation listbox_navigation;
		//private ComboBoxTextBoxNavigation textbox_navigation;
		
#endregion
		
#region Button Navigation Class
		
		class ComboBoxButtonNavigation : SimpleNavigation
		{
			public ComboBoxButtonNavigation  (ComboBoxProvider provider)
				: base (provider)
			{
				this.provider = provider;
			}
			
			public override IRawElementProviderSimple Provider {
				get { 
					if (button_provider == null) {
						button_provider = new ComboBoxButtonProvider ((ComboBox) provider.Control);
						button_provider.Navigation = this;
					}

					return button_provider;
				}
			}
			
			public override void FinalizeProvider ()
			{
				if (button_provider != null)
					button_provider.Terminate ();
			}

			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				if (direction == NavigateDirection.Parent)
					return provider;
				else if (direction == NavigateDirection.PreviousSibling)
					return GetPreviousSiblingProvider ();
				else
					return null;
			}
			
			private ComboBoxProvider provider;
			private ComboBoxButtonProvider button_provider;
			
		}

#endregion

#region ListBox Navigation Class
		
		//TODO: This class should allow navigating 2 ScrollBars and n items.		
		class ComboBoxListBoxNavigation : SimpleNavigation
		{
			public ComboBoxListBoxNavigation (ComboBoxProvider provider)
				: base (provider)
			{
				this.provider = provider;
				
				InitializeItemChangeEvents ();
			}
			
			public override IRawElementProviderSimple Provider {
				get {  return GetListBoxProvider (); }
			}
			
			public override void FinalizeProvider ()
			{
				if (listbox_provider != null)
					listbox_provider.Terminate ();
			}
			
			public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
			{
				if (direction == NavigateDirection.Parent)
					return provider;
				else if (direction == NavigateDirection.FirstChild)
					return GetListBoxProvider ().GetItemProvider (0);
				else if (direction == NavigateDirection.LastChild)
					return GetListBoxProvider ().GetItemProvider (GetListBoxProvider ().ItemsCount - 1);
				else if (direction == NavigateDirection.NextSibling)
					return GetNextSiblingProvider ();
				else if (direction == NavigateDirection.PreviousSibling)
					return GetPreviousSiblingProvider ();
				else
					return null;
			}
			
			private ComboBoxListBoxProvider GetListBoxProvider ()
			{
				if (listbox_provider == null) {
					listbox_provider = new ComboBoxListBoxProvider ((ComboBox) provider.Control,
					                                                provider);
					listbox_provider.Navigation = this;
				}
				return listbox_provider; 
			}
			
			private void InitializeItemChangeEvents ()
			{
				ComboBox combobox = (ComboBox) provider.Control;
				Type type = typeof (ComboBox.ObjectCollection);

				//ChildAdded hook up
				EventInfo childAddedEvent = type.GetEvent ("ChildAdded",
				                                           BindingFlags.Instance 
				                                           | BindingFlags.NonPublic);
				Type delegateType = childAddedEvent.EventHandlerType;
				MethodInfo childAddedMethod = childAddedEvent.GetAddMethod (true);				
				Delegate delegateAdded = Delegate.CreateDelegate (delegateType, 
				                                                  this, "OnChildAdded", 
				                                                  false);
				childAddedMethod.Invoke (combobox.Items, 
				                         new object[] { delegateAdded });

				//ChildRemoved hook up
				EventInfo childRemovedEvent = type.GetEvent ("ChildRemoved",
				                                             BindingFlags.Instance 
				                                             | BindingFlags.NonPublic);
				MethodInfo childRemovedMethod = childRemovedEvent .GetAddMethod (true);				
				Delegate delegateRemoved = Delegate.CreateDelegate (delegateType, 
				                                                    this, "OnChildRemoved", 
				                                                    false);
				childAddedMethod.Invoke (combobox.Items,
				                         new object[] { delegateRemoved });

				//TODO: Is this OK? CPU-intensive?
				for (int index = 0; index < combobox.Items.Count; index++)
					OnChildAdded (combobox, index);
			}
			
			private void OnChildAdded (object sender, int index)
			{
				ListItemProvider item = provider.GetItemProvider (index);
				
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
				                                   item);
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated,
				                                   provider);
			}
			
			private void OnChildRemoved (object sender, int index)
			{
				ListItemProvider item = provider.RemoveItemAt (index);
					
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildRemoved,
				                                   item);
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated,
				                                   provider);
			}

			private ComboBoxProvider provider;
			private ComboBoxListBoxProvider listbox_provider;
		}	
		
#endregion

	}
}
