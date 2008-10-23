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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Collections.Generic;
using Mono.UIAutomation.Bridge;

using System.Windows.Automation;
using System.Windows.Automation.Provider;


namespace UiaAtkBridge
{
	public class AutomationBridge : IAutomationBridge
	{
#region Private Fields
		
		private bool applicationStarted = false;
		private Monitor appMonitor = null;
		private long initTime = DateTime.Now.Ticks;
		private AmbiDictionary<IntPtr, IRawElementProviderSimple>
			pointerProviderMapping;
		static private AmbiDictionary<IRawElementProviderSimple, Adapter>
			providerAdapterMapping;
		
		private int windowProviders;
		
#endregion

#region Public Constructor
		
		public AutomationBridge ()
		{
			bool newMonitor = false;
			if (appMonitor == null) {
				//Console.WriteLine ("about to create monitor");
				appMonitor = new Monitor();
				//Console.WriteLine ("just made monitor");
			}
			pointerProviderMapping =
				new AmbiDictionary<IntPtr,IRawElementProviderSimple> ();
			providerAdapterMapping =
				new AmbiDictionary<IRawElementProviderSimple, Adapter>();

			windowProviders = 0;
		}
		
#endregion
		
#region Public Methods

		static object adapterLookup = new object ();
		static bool alreadyInLookup = false;
		
		public static Adapter GetAdapterForProviderLazy (IRawElementProviderSimple provider)
		{
			return GetAdapterForProvider (provider, false);
		}
		
		[Obsolete("Use GetAdapterForProviderLazy as it's more efficient")]
		public static Adapter GetAdapterForProvider (IRawElementProviderSimple provider)
		{
			return GetAdapterForProvider (provider, true);
		}
		
		private static Adapter GetAdapterForProvider (IRawElementProviderSimple provider, bool avoidLazyLoading)
		{
			lock (adapterLookup) {
				if (alreadyInLookup)
					throw new Exception ("Cannot run nested GetAdapterForProvider lookups");
				alreadyInLookup = true;

				try {
					if (avoidLazyLoading) {
						Console.WriteLine ("WARNING: obsolete non-lazy-loading GetAdapterForProvider method called.");
						
						List <Atk.Object> alreadyRequestedChildren = new List <Atk.Object> ();
						List <IRawElementProviderSimple> initialProvs = new List <IRawElementProviderSimple> ();
						
						foreach (IRawElementProviderSimple providerReady in providerAdapterMapping.Keys) {
							initialProvs.Add (providerReady);
						}
						
						foreach (IRawElementProviderSimple providerReady in initialProvs) {
							RequestChildren (providerAdapterMapping [providerReady], alreadyRequestedChildren);
						}
						alreadyRequestedChildren = null;
					}
					
					Adapter adapter;
					if (providerAdapterMapping.TryGetValue (provider, out adapter))
						return adapter;
					return null;
				}
				finally {
					alreadyInLookup = false;
				}
			}
		}
		
		private static void RequestChildren (Atk.Object adapter, List<Atk.Object> alreadyRequestedChildren) {
			if (alreadyRequestedChildren.Contains (adapter))
				return;

			int nChildren = adapter.NAccessibleChildren;
			
			if (nChildren > 0) {
				for (int i = 0; i < nChildren; i++) {
					Atk.Object adapterN = adapter.RefAccessibleChild (i);
					
					if (i == 0)
						alreadyRequestedChildren.Add (adapter);
					RequestChildren (adapterN, alreadyRequestedChildren);
				}
			}
			else
				alreadyRequestedChildren.Add (adapter);
		}
		
#endregion
		
#region IAutomationBridge Members
		
		public bool ClientsAreListening {
			get {
				// TODO: How with ATK?
				return true;
			}
		}
		
		public object HostProviderFromHandle (IntPtr hwnd)
		{
			if (!pointerProviderMapping.ContainsKey (hwnd))
				return null;
			return pointerProviderMapping [hwnd];
		}

		struct KeyDefinition
		{
			public uint Keyval;
			public String String;
			public KeyDefinition (uint Keyval, String String)
			{
				this.Keyval = Keyval;
				this.String = String;
			}
		}

		static Dictionary<int, KeyDefinition> downKeys = new Dictionary<int, KeyDefinition>();
		static Dictionary<uint, String> keyStrings;

		public void CreateKeyStrings ()
		{
			keyStrings = new Dictionary<uint, String>();
			keyStrings [0xfd1d] = "Print_Screen";
			keyStrings [0xff08] = "Backspace";
			keyStrings [0xff09] = "Tab";
			keyStrings [0xff0b] = "Clear";
			keyStrings [0xff0d] = "Return";
			keyStrings [0xff10] = "Line_feed";
			keyStrings [0xff13] = "Pause";
			keyStrings [0xff14] = "Scroll_Lock";
			keyStrings [0xff1b] = "Escape";
			keyStrings [0xff21] = "Kanji";
			keyStrings [0xff2d] = "Kana_Lock";
			keyStrings [0xff31] = "Hangul";
			keyStrings [0xff50] = "Home";
			keyStrings [0xff51] = "Left";
			keyStrings [0xff52] = "Up";
			keyStrings [0xff53] = "Right";
			keyStrings [0xff54] = "Down";
			keyStrings [0xff55] = "Page_Up";
			keyStrings [0xff56] = "Page_Down";
			keyStrings [0xff57] = "End";
			keyStrings [0xff60] = "Select";
			keyStrings [0xff61] = "Print";
			keyStrings [0xff62] = "Execute";
			keyStrings [0xff63] = "Insert";
			keyStrings [0xff67] = "Menu";
			keyStrings [0xff67] = "Menu";
			keyStrings [0xff6a] = "Help";
			keyStrings [0xff7f] = "Num_Lock";
			keyStrings [0xff95] = "KP_Home";
			keyStrings [0xff96] = "KP_Left";
			keyStrings [0xff97] = "KP_Up";
			keyStrings [0xff98] = "KP_Right";
			keyStrings [0xff99] = "KP_Down";
			keyStrings [0xff9a] = "KP_Page_Up";
			keyStrings [0xff9b] = "KP_Page_Down";
			keyStrings [0xff9c] = "KP_Begin";
			keyStrings [0xff9d] = "KP_End";
			keyStrings [0xff9e] = "KP_Insert";
			keyStrings [0xff9f] = "KP_Delete";
			keyStrings [0xffbe] = "F1";
			keyStrings [0xffbf] = "F2";
			keyStrings [0xffc0] = "F3";
			keyStrings [0xffc1] = "F4";
			keyStrings [0xffc2] = "F5";
			keyStrings [0xffc3] = "F6";
			keyStrings [0xffc4] = "F7";
			keyStrings [0xffc5] = "F8";
			keyStrings [0xffc6] = "F9";
			keyStrings [0xffc7] = "F10";
			keyStrings [0xffc8] = "F11";
			keyStrings [0xffc9] = "F12";
			keyStrings [0xffca] = "F13";
			keyStrings [0xffcb] = "F14";
			keyStrings [0xffcc] = "F15";
			keyStrings [0xffcd] = "F16";
			keyStrings [0xffce] = "F17";
			keyStrings [0xffcf] = "F18";
			keyStrings [0xffd0] = "F19";
			keyStrings [0xffd1] = "F20";
			keyStrings [0xffd2] = "F21";
			keyStrings [0xffd3] = "F22";
			keyStrings [0xffd4] = "F23";
			keyStrings [0xffd5] = "F24";
			keyStrings [0xffe1] = "Shift_L";
			keyStrings [0xffe2] = "Shift_R";
			keyStrings [0xffe3] = "Control_L";
			keyStrings [0xffe4] = "Control_R";
			keyStrings [0xffe5] = "Caps_Lock";
			keyStrings [0xffe9] = "Alt_L";
			keyStrings [0xffea] = "Alt_R";
			keyStrings [0xffeb] = "Super_L";
			keyStrings [0xffff] = "Delete";
		}

		void HandleKeyEvent (AutomationEvent eventId, KeyEventArgs e)
		{
			Atk.KeyEventStruct evnt;
			if (keyStrings == null)
				CreateKeyStrings ();
			evnt.Type = (e.Down? 0: 1);
			evnt.State = 0;
			if (e.Shift)
				evnt.State |= 1;
			if (e.Control)
				evnt.State |= 4;
			if (e.Alt)
				evnt.State |= 8;
			evnt.Keycode = (ushort)e.Keycode;
			if (evnt.Type == 1 && downKeys.ContainsKey (e.Keycode)) {
				// For some reason, we don't get keysym and
				// string values for KeyUp events, so we
				// cache the values when th ekey is pressed
				evnt.Keyval = downKeys [e.Keycode].Keyval;
				evnt.String = downKeys [e.Keycode].String;
				downKeys.Remove (e.Keycode);
			} else {
				evnt.Keyval = (uint)e.Keysym;
				evnt.String = (keyStrings.ContainsKey (evnt.Keyval)? keyStrings [evnt.Keyval]: e.Str);
				downKeys [e.Keycode] = new KeyDefinition ((uint)e.Keysym, evnt.String);
			}
			evnt.Length = evnt.String.Length;
			// TODO: Fill in timestamp
			evnt.Timestamp = (uint)((DateTime.Now.Ticks - initTime) / 10000);
			e.SuppressKeyPress = appMonitor.HandleKey (evnt);
		}
		
		public void RaiseAutomationEvent (AutomationEvent eventId, object provider, AutomationEventArgs e)
		{
			// TODO: Find better way to pass PreRun event on to bridge
			//        (nullx3 is a magic value)
			//        (once bridge events are working, should be able to happen upon construction, right?)
			if (eventId == null && provider == null && e == null) {
				if (!applicationStarted && appMonitor != null)
					appMonitor.ApplicationStarts ();
				return;
			}
			
			if (eventId == AutomationElementIdentifiers.KeyEvent) {
				HandleKeyEvent (eventId, (KeyEventArgs)e);
				return;
			}

			IRawElementProviderSimple simpleProvider =
				(IRawElementProviderSimple) provider;
			if (!providerAdapterMapping.ContainsKey (simpleProvider))
				return;
			
			providerAdapterMapping [simpleProvider].RaiseAutomationEvent (eventId, e);
		}
		
		public void RaiseAutomationPropertyChangedEvent (object element, AutomationPropertyChangedEventArgs e)
		{
			if (element == null)
				return;
			
			IRawElementProviderSimple simpleProvider =
				(IRawElementProviderSimple) element;
			// Create an adapter if we haven't already, so that
			// an AT will know about the control that has focus,
			// but don't do this if we're shutting down (ie,
			// providerAdapterMapping.Count == 0)
			if (e.Property == AutomationElementIdentifiers.HasKeyboardFocusProperty && !providerAdapterMapping.ContainsKey (simpleProvider) && providerAdapterMapping.Count > 0)
				HandleElementAddition (simpleProvider);
			if ((!providerAdapterMapping.ContainsKey (simpleProvider)) || windowProviders == 0)
				return;
			
			providerAdapterMapping [simpleProvider].RaiseAutomationPropertyChangedEvent (e);
		}
		
		public void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			IRawElementProviderSimple simpleProvider = (IRawElementProviderSimple) provider;
			// TODO: Handle ChildrenBulkAdded
			if (e.StructureChangeType == StructureChangeType.ChildAdded) {
				if (!providerAdapterMapping.ContainsKey (simpleProvider))
					HandleElementAddition (simpleProvider);
			} else if (e.StructureChangeType == StructureChangeType.ChildRemoved) {
				// TODO: Handle proper documented args
				//       (see FragmentRootControlProvider)
				if (HandleTotalElementRemoval (simpleProvider))
					appMonitor.Quit ();
			} else if (e.StructureChangeType == StructureChangeType.ChildrenBulkRemoved) {
				HandleBulkRemoved (simpleProvider);
			}
			else
				Console.WriteLine ("StructureChangedEvent not handled:" + e.StructureChangeType.ToString ());
			
			// TODO: Other structure changes
		}
		
#endregion
		
#region Private Methods
		
		private ParentAdapter GetParentAdapter (IRawElementProviderSimple provider)
		{
			IRawElementProviderFragment fragment = (IRawElementProviderFragment)provider;
			IRawElementProviderSimple parentProvider;

			parentProvider = fragment.Navigate (NavigateDirection.Parent);

			if (parentProvider == null)
				return null;
			
			if (!providerAdapterMapping.ContainsKey (parentProvider))
				HandleElementAddition (parentProvider);

			Adapter parent = null;
			if (!providerAdapterMapping.TryGetValue (parentProvider, out parent))
				return null;
			return (ParentAdapter)parent;
		}

		private void HandleElementAddition (IRawElementProviderSimple simpleProvider)
		{
			if (providerAdapterMapping.ContainsKey (simpleProvider))
				return;

			int controlTypeId = (int) simpleProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

			if (controlTypeId == ControlType.Window.Id)
				HandleNewWindowControlType (simpleProvider);
			else if (controlTypeId == ControlType.Button.Id)
				// TODO: Consider generalizing...
				HandleNewButtonControlType (simpleProvider);
			else if (controlTypeId == ControlType.Text.Id)
				HandleNewLabelControlType (simpleProvider);
			else if (controlTypeId == ControlType.CheckBox.Id)
				HandleNewCheckBoxControlType (simpleProvider);
			else if (controlTypeId == ControlType.List.Id)
				HandleNewListControlType (simpleProvider);
			else if (controlTypeId == ControlType.ListItem.Id)
				HandleNewListItemControlType (simpleProvider);
			else if (controlTypeId == ControlType.ComboBox.Id)
				HandleNewComboBoxControlType (simpleProvider);
			else if (controlTypeId == ControlType.StatusBar.Id)
				HandleNewStatusBarControlType (simpleProvider);
			else if (controlTypeId == ControlType.ProgressBar.Id)
				HandleNewProgressBarControlType (simpleProvider);
			else if (controlTypeId == ControlType.ScrollBar.Id)
				HandleNewScrollBarControlType (simpleProvider);
			else if (controlTypeId == ControlType.Group.Id)
				HandleNewGroupControlType (simpleProvider);
			else if (controlTypeId == ControlType.RadioButton.Id){
				HandleNewRadioButtonControlType (simpleProvider);}
			else if (controlTypeId == ControlType.Spinner.Id)
				HandleNewSpinnerControlType (simpleProvider);
 			else if (controlTypeId == ControlType.ToolTip.Id)
 				HandleNewToolTipControlType (simpleProvider);
 			else if (controlTypeId == ControlType.Hyperlink.Id)
 				HandleNewHyperlinkControlType (simpleProvider);
			else if ((controlTypeId == ControlType.Document.Id) || (controlTypeId == ControlType.Edit.Id))
				HandleNewDocumentOrEditControlType (simpleProvider);
			else if (controlTypeId == ControlType.Image.Id)
				HandleNewImageControlType (simpleProvider);
			else if (controlTypeId == ControlType.ToolBar.Id)
				HandleNewToolBarControlType (simpleProvider);
			else if (controlTypeId == ControlType.MenuBar.Id) //for MenuStrip widget
				// || (controlTypeId == ControlType.Menu.Id)) //for 1.x Menu widget it seems <- TODO
				HandleNewMenuBarControlType (simpleProvider);
			else if (controlTypeId == ControlType.MenuItem.Id) //for ToolStripMenuItem widget
				HandleNewMenuItemControlType (simpleProvider);
			// TODO: Other providers
			else
				Console.WriteLine ("AutomationBridge: Unhandled control: " +
				                   ControlType.LookupById (controlTypeId).ProgrammaticName);
		}

		private bool HandleElementRemoval (Atk.Object atkObj)
		{
			IRawElementProviderSimple provider;
			if (providerAdapterMapping.TryGetKey ((Adapter)atkObj, out provider) == false)
				return false;
			return HandleElementRemoval (provider);
		}

		private bool HandleTotalElementRemoval (IRawElementProviderSimple provider)
		{
			bool lastWindowProvider = false;
			
			Adapter adapter;
			if (providerAdapterMapping.TryGetValue (provider, out adapter) == false)
				return false;

			foreach (Atk.Object atkObj in GetAdaptersDescendantsFamily (adapter)){
				if (HandleElementRemoval (atkObj))
					lastWindowProvider = true;
			}

			return lastWindowProvider;
		}

		private List<Atk.Object> GetAdaptersDescendantsFamily (Atk.Object adapter) {
			List <Atk.Object> list = new List <Atk.Object> ();
			int nchild = adapter.NAccessibleChildren;
			if (nchild > 0) {
				for (int i = 0; i < nchild; i++) {
					list.AddRange (GetAdaptersDescendantsFamily (adapter.RefAccessibleChild (i)));
				}
			}
			list.Add (adapter);
			return list;
		}
		
		private bool HandleElementRemoval (IRawElementProviderSimple provider)
		{
			bool lastWindowProvider = false;
			
			Adapter adapter;
			if (providerAdapterMapping.TryGetValue (provider, out adapter) == false)
				return false;

			int controlTypeId = (int)provider.GetPropertyValue (
				AutomationElementIdentifiers.ControlTypeProperty.Id);
			
			if (controlTypeId == ControlType.ToolTip.Id) {
				TopLevelRootItem.Instance.RemoveChild (adapter);
			} else if (controlTypeId == ControlType.Window.Id) {
				TopLevelRootItem.Instance.RemoveChild (adapter);
				windowProviders--;
				if (windowProviders == 0)
					lastWindowProvider = true;
		 	} else {
				ParentAdapter parent = adapter.Parent as ParentAdapter;
				if (parent != null)
					parent.RemoveChild (adapter);
			}

			providerAdapterMapping.Remove (provider);
			pointerProviderMapping.Remove (provider);

			return lastWindowProvider;
		}
		
		
		private void HandleBulkRemoved (IRawElementProviderSimple provider)
		{
			if (!providerAdapterMapping.ContainsKey (provider)) {
				//Console.WriteLine ("Got a ChildrenBulkRemove for a " + provider + " but no adapter");
				return;
			}
			IRawElementProviderFragment fragment;
			if ((fragment = provider as IRawElementProviderFragment) == null)
				return;
			List<Adapter> keep = new List<Adapter> ();
			IRawElementProviderFragment child = fragment.Navigate(NavigateDirection.FirstChild);
			while (child != null) {
				if (providerAdapterMapping.ContainsKey (child))
					keep.Add (providerAdapterMapping[child]);
				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Adapter adapter = providerAdapterMapping [provider];
			int index = adapter.NAccessibleChildren;
			while (--index >= 0) {
				Adapter childAdapter = adapter.RefAccessibleChild (index) as Adapter;
				if (!keep.Contains(childAdapter))
					HandleElementRemoval (childAdapter.Provider);
			}
		}

		private void HandleNewWindowControlType (IRawElementProviderSimple provider)
		{
			Window newWindow = new Window (provider);
			providerAdapterMapping [provider] = newWindow;
			
			TopLevelRootItem.Instance.AddOneChild (newWindow);
			
			IntPtr providerHandle = (IntPtr) provider.GetPropertyValue (AutomationElementIdentifiers.NativeWindowHandleProperty.Id);
			pointerProviderMapping [providerHandle] = provider;
			
			windowProviders++;
		}
		
		private void HandleNewButtonControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			if (parentObject is UiaAtkBridge.ComboBox)
				return; //ComboBox will handle its children additions on its own
			
			if ((parentObject == null) || (parentObject.Role == Atk.Role.ScrollBar))
				return;

			Button atkButton = new Button (provider);
			providerAdapterMapping [provider] = atkButton;
			
			parentObject.AddOneChild (atkButton);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkButton);
		}
		
		private void HandleNewLabelControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			TextLabel atkLabel = new TextLabel (provider);
			providerAdapterMapping [provider] = atkLabel;
			
			parentObject.AddOneChild (atkLabel);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkLabel);
		}
		
		private void HandleNewCheckBoxControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			CheckBoxButton atkCheck = new CheckBoxButton (provider);
			providerAdapterMapping [provider] = atkCheck;
			
			parentObject.AddOneChild (atkCheck);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkCheck);
		}
		
		private void HandleNewListControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			if (parentObject is UiaAtkBridge.ComboBox)
				return; //ComboBox will handle its children additions on its own
			
			List atkList = new List ((IRawElementProviderFragmentRoot)provider);
			providerAdapterMapping [provider] = atkList;
			
			parentObject.AddOneChild (atkList);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkList);
		}

		private void HandleNewListItemControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			if (parentObject == null)
				return; //it's possible that the parent List hasn't got Atk.Object counterpart
			
			ListItem atkItem = new ListItem (provider);
			
			providerAdapterMapping [provider] = atkItem;
			
			parentObject.AddOneChild (atkItem);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkItem);
		}

		private void HandleNewComboBoxControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);

			ComboBox atkCombo = new ComboBox ((IRawElementProviderFragmentRoot)provider);
			providerAdapterMapping [provider] = atkCombo;
			
			parentObject.AddOneChild (atkCombo);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkCombo);
		}
		
		private void HandleNewStatusBarControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			StatusBar atkStatus;
			if (provider is IGridProvider)
				atkStatus = new StatusBarWithGrid (provider);
 			else atkStatus = new StatusBar (provider);
			providerAdapterMapping [provider] = atkStatus;
			
			parentObject.AddOneChild (atkStatus);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkStatus);
		}
		
		private void HandleNewProgressBarControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			ProgressBar atkProgress;
			if (provider is IGridProvider)
				atkProgress = new ProgressBar (provider);
 			else atkProgress = new ProgressBar (provider);
			providerAdapterMapping [provider] = atkProgress;
			
			parentObject.AddOneChild (atkProgress);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkProgress);
		}
		
		private void HandleNewScrollBarControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject =
				GetParentAdapter (provider);
			
			ScrollBar atkScroll = new ScrollBar (provider);
			providerAdapterMapping [provider] = atkScroll;
			
			parentObject.AddOneChild (atkScroll);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkScroll);
		}
		
		private void HandleNewGroupControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject =
				GetParentAdapter (provider);
			
			Pane atkPane = new Pane (provider);
			providerAdapterMapping [provider] = atkPane;
			
			parentObject.AddOneChild (atkPane);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkPane);
		}
		
		private void HandleNewRadioButtonControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			RadioButton atkRadio = new RadioButton (provider);
			providerAdapterMapping [provider] = atkRadio;
			
			parentObject.AddOneChild (atkRadio);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkRadio);
		}
		
		private void HandleNewSpinnerControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			Adapter atkSpinner;
			if (provider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id) != null)
				atkSpinner = new List ((IRawElementProviderFragmentRoot)provider);
 			else
				atkSpinner = new Spinner (provider);
			providerAdapterMapping [provider] = atkSpinner;
			
			parentObject.AddOneChild (atkSpinner);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkSpinner);
		}
		
		private void HandleNewDocumentOrEditControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			if (parentObject is UiaAtkBridge.ComboBox)
				return; //ComboBox will handle its children additions on its own
			
			Adapter atkEditOrDoc = new TextBoxEntryView (provider);
			
			providerAdapterMapping [provider] = atkEditOrDoc;
			
			parentObject.AddOneChild (atkEditOrDoc);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkEditOrDoc);
		}
		
		private void HandleNewToolTipControlType (IRawElementProviderSimple provider)
		{
			ToolTip atkToolTip = new ToolTip (provider);
			providerAdapterMapping [provider] = atkToolTip;

			TopLevelRootItem.Instance.AddOneChild (atkToolTip);
		}

		private void HandleNewHyperlinkControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			Adapter atkHyperlink;
			IInvokeProvider invokeProvider = (IInvokeProvider)provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			if (invokeProvider is IHypertext)
				atkHyperlink = new Hyperlink (provider);
			else
				// We don't have the extension needed to
				// properly support a hyperlink
				atkHyperlink = new TextLabel (provider);
			providerAdapterMapping [provider] = atkHyperlink;
			
			parentObject.AddOneChild (atkHyperlink);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkHyperlink);
		}

		private void HandleNewImageControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			Adapter atkImage = new Image (provider);
			providerAdapterMapping [provider] = atkImage;
			
			parentObject.AddOneChild (atkImage);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkImage);
		}

		private void HandleNewToolBarControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			Adapter atkToolBar = new ToolBar (provider);
			providerAdapterMapping [provider] = atkToolBar;
			
			parentObject.AddOneChild (atkToolBar);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              atkToolBar);
		}

		private void HandleNewMenuBarControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			MenuBar newMenuBar = new MenuBar (provider);
			providerAdapterMapping [provider] = newMenuBar;
			
			parentObject.AddOneChild (newMenuBar);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              newMenuBar);
		}

		private void HandleNewMenuItemControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);

			Adapter newAdapter;
			if (HasChildren (provider))
				newAdapter = new ParentMenu (provider);
			else
				newAdapter = new ChildMenuItem (provider);
			providerAdapterMapping [provider] = newAdapter;
			
			parentObject.AddOneChild (newAdapter);
			parentObject.AddRelationship (Atk.RelationType.Embeds,
			                              newAdapter);
		}

		private bool HasChildren (IRawElementProviderSimple provider)
		{
			if (!(provider is IRawElementProviderFragment))
				return false;

			IRawElementProviderFragment iter = (IRawElementProviderFragment)provider;
			iter = iter.Navigate (NavigateDirection.FirstChild);
			if (iter != null)
				return true;
			return false;
		}
		
#endregion
	}
}
