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
		private AmbiDictionary<IntPtr, IRawElementProviderSimple> pointerProviderMapping;
		static private AmbiDictionary<IRawElementProviderSimple, Adapter> providerAdapterMapping;
		
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
						Console.Error.WriteLine ("WARNING: obsolete non-lazy-loading GetAdapterForProvider method called.");
						
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
			Console.WriteLine ("RaiseAutomationPropertyChangedEvent");
			if (element == null)
				throw new ArgumentNullException ("element");
			
			IRawElementProviderSimple simpleProvider =
				(IRawElementProviderSimple) element;
			// Create an adapter if we haven't already, so that
			// an AT will know about the control that has focus,
			// but don't do this if we're shutting down (ie,
			// providerAdapterMapping.Count == 0)
			if (e.Property == AutomationElementIdentifiers.HasKeyboardFocusProperty && !providerAdapterMapping.ContainsKey (simpleProvider) && providerAdapterMapping.Count > 0) {
				HandleElementAddition (simpleProvider);
				int controlTypeId = (int) simpleProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
				if (controlTypeId == ControlType.DataItem.Id && simpleProvider is IRawElementProviderFragment) {
					IRawElementProviderFragment child = ((IRawElementProviderFragment)simpleProvider).Navigate (NavigateDirection.FirstChild);
					providerAdapterMapping [child].RaiseAutomationPropertyChangedEvent (e);
					return;
				}
			}

			if ((!providerAdapterMapping.ContainsKey (simpleProvider)) || windowProviders == 0)
				return;
			
			providerAdapterMapping [simpleProvider].RaiseAutomationPropertyChangedEvent (e);
		}
		
		public void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			IRawElementProviderSimple simpleProvider = (IRawElementProviderSimple) provider;
			if (e.StructureChangeType == StructureChangeType.ChildrenBulkAdded) {
				HandleBulkAdded (simpleProvider);
			} else if (e.StructureChangeType == StructureChangeType.ChildAdded) {
				if (!providerAdapterMapping.ContainsKey (simpleProvider))
					HandleElementAddition (simpleProvider);
			} else if (e.StructureChangeType == StructureChangeType.ChildRemoved) {
				// TODO: Handle proper documented args
				//       (see FragmentRootControlProvider)
				if (HandleTotalElementRemoval (simpleProvider))
					appMonitor.Quit ();
			} else if (e.StructureChangeType == StructureChangeType.ChildrenBulkRemoved) {
				HandleBulkRemoved (simpleProvider);
			} else if (e.StructureChangeType == StructureChangeType.ChildrenInvalidated) {
				HandleBulkRemoved (simpleProvider);
				HandleBulkAdded (simpleProvider);
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
			
			int controlTypeId = (int) parentProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (controlTypeId == ControlType.Header.Id || controlTypeId == ControlType.DataItem.Id)
				return GetParentAdapter (parentProvider);

			if (!providerAdapterMapping.ContainsKey (parentProvider))
				HandleElementAddition (parentProvider);

			Adapter parent = null;
			if (!providerAdapterMapping.TryGetValue (parentProvider, out parent))
				return null;
			ParentAdapter ret = parent as ParentAdapter;
			if (ret == null)
				Console.WriteLine ("AutomationBridge: warning: Could not cast " + parent + " to ParentAdapter");
			return ret;
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
				HandleNewContainer (simpleProvider);
			else if (controlTypeId == ControlType.Header.Id)
				AddChildrenToParent (simpleProvider);
			else if (controlTypeId == ControlType.HeaderItem.Id)
				HandleNewHeaderItemControlType (simpleProvider);
			else if (controlTypeId == ControlType.MenuBar.Id) //for MenuStrip widget
				// || (controlTypeId == ControlType.Menu.Id)) //for 1.x Menu widget it seems <- TODO
				HandleNewMenuBarControlType (simpleProvider);
			else if (controlTypeId == ControlType.MenuItem.Id) //for ToolStripMenuItem widget
				HandleNewMenuItemControlType (simpleProvider);
			else if (controlTypeId == ControlType.DataGrid.Id) //for ToolStripMenuItem widget
				HandleNewDataGridControlType (simpleProvider);
			else if (controlTypeId == ControlType.DataItem.Id)
				AddChildrenToParent (simpleProvider);
			// TODO: Other providers
			else if (controlTypeId != ControlType.Thumb.Id)
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
		
		
		private void HandleBulkAdded (IRawElementProviderSimple provider)
		{
			if (!providerAdapterMapping.ContainsKey (provider)) {
				int controlTypeId = (int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
				if (controlTypeId == ControlType.Header.Id || controlTypeId == ControlType.DataItem.Id) {
					IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
					if (fragment != null) {
						IRawElementProviderFragment parent = fragment.Navigate (NavigateDirection.Parent);
						HandleBulkAdded (parent);
					}
				}
				return;
			}
			ParentAdapter adapter = providerAdapterMapping [provider] as ParentAdapter;
			if (adapter != null)
				adapter.UpdateChildren ();
		}

		private void HandleBulkRemoved (IRawElementProviderSimple provider)
		{
			IRawElementProviderFragment fragment;
			if (!providerAdapterMapping.ContainsKey (provider)) {
				int controlTypeId = (int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
				if (controlTypeId == ControlType.Header.Id || controlTypeId == ControlType.DataItem.Id) {
					fragment = provider as IRawElementProviderFragment;
					if (fragment != null) {
						IRawElementProviderFragment parent = fragment.Navigate (NavigateDirection.Parent);
						HandleBulkRemoved (parent);
					}
				}
				return;
			}
			if ((fragment = provider as IRawElementProviderFragment) == null)
				return;
			List<Adapter> keep = new List<Adapter> ();
			IRawElementProviderFragment child = fragment.Navigate(NavigateDirection.FirstChild);
			while (child != null) {
				if (providerAdapterMapping.ContainsKey (child))
					keep.Add (providerAdapterMapping [child]);
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

			IncludeNewAdapter (newWindow, TopLevelRootItem.Instance);
			
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
			
			IncludeNewAdapter (atkButton, parentObject);
		}
		
		private void HandleNewLabelControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			TextLabel atkLabel = new TextLabel (provider);

			IncludeNewAdapter (atkLabel, parentObject);
		}
		
		private void HandleNewCheckBoxControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			CheckBoxButton atkCheck = new CheckBoxButton (provider);

			IncludeNewAdapter (atkCheck, parentObject);
		}
		
		private void HandleNewListControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			Adapter atkList;
			if (parentObject is UiaAtkBridge.ComboBox)
				atkList = new MenuItem (provider);
			else if (provider is IGridProvider)
				atkList = new ListWithGrid ((IRawElementProviderFragmentRoot)provider);
			else
				atkList = new List ((IRawElementProviderFragmentRoot)provider);

			IncludeNewAdapter (atkList, parentObject);
		}

		private void HandleNewListItemControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);

			Adapter atkItem;
			if (parentObject is MenuItem)
				atkItem = new MenuItem (provider);
			else
				atkItem = new ListItem (provider);
			
			IncludeNewAdapter (atkItem, parentObject);
		}

		private void HandleNewComboBoxControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);

			ComboBox atkCombo = new ComboBox ((IRawElementProviderFragmentRoot)provider);
			
			IncludeNewAdapter (atkCombo, parentObject);
		}
		
		private void HandleNewStatusBarControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			TextContainer atkStatus;
			//FIXME: probably we shouldn't split this in 2 classes
			//FIXME: not sure if this interface check is correct
			if (provider is IGridProvider)
				atkStatus = new TextContainerWithGrid (provider);
 			else 
				atkStatus = new TextContainer (provider);

			IncludeNewAdapter (atkStatus, parentObject);
		}

		private void HandleNewProgressBarControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			ProgressBar atkProgress = new ProgressBar (provider);
			
			IncludeNewAdapter (atkProgress, parentObject);
		}
		
		private void HandleNewScrollBarControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			ScrollBar atkScroll = new ScrollBar (provider);

			IncludeNewAdapter (atkScroll, parentObject);
		}
		
		private void HandleNewGroupControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			Adapter newAdapter = null;
			if (parentObject is DataGrid)
				newAdapter = new DataGridGroup (provider);
			else if (parentObject is List && provider is IRawElementProviderFragment)
				newAdapter = new ListGroup ((IRawElementProviderFragment)provider);
			else
				newAdapter = new Container (provider);

			IncludeNewAdapter (newAdapter, parentObject);
		}
		
		private void HandleNewRadioButtonControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			RadioButton atkRadio = new RadioButton (provider);

			IncludeNewAdapter (atkRadio, parentObject);
		}
		
		private void HandleNewSpinnerControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			Adapter atkSpinner;
			if (provider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id) != null)
				atkSpinner = new List ((IRawElementProviderFragmentRoot)provider);
 			else
				atkSpinner = new Spinner (provider);

			IncludeNewAdapter (atkSpinner, parentObject);
		}
		
		private void HandleNewDocumentOrEditControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			Adapter atkEditOrDoc = new TextBoxEntryView (provider);
			
			IncludeNewAdapter (atkEditOrDoc, parentObject);
		}
		
		private void HandleNewToolTipControlType (IRawElementProviderSimple provider)
		{
			ToolTip atkToolTip = new ToolTip (provider);

			IncludeNewAdapter (atkToolTip, TopLevelRootItem.Instance);
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

			IncludeNewAdapter (atkHyperlink, parentObject);
		}

		private void HandleNewImageControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			Adapter atkImage = new Image (provider);

			IncludeNewAdapter (atkImage, parentObject);
		}

		private void HandleNewContainer (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			Adapter atkContainer = new Container (provider);
			providerAdapterMapping [provider] = atkContainer;
			
			IncludeNewAdapter (atkContainer, parentObject);
		}

		private void HandleNewMenuBarControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			MenuBar newMenuBar = new MenuBar (provider);

			IncludeNewAdapter (newMenuBar, parentObject);
		}

		private void HandleNewMenuItemControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			if (parentObject == null)
				return; //doesn't matter, MenuItem will discover its own children during its ctor call

			Adapter newAdapter = new MenuItem (provider);
			IncludeNewAdapter (newAdapter, parentObject);
		}

		internal static void IncludeNewAdapter (Adapter newAdapter, ParentAdapter parentAdapter)
		{
			if (newAdapter.Provider == null)
				throw new ArgumentException (String.Format ("{0} adapter should have a not null provider", newAdapter.GetType ().Name));
//			if (providerAdapterMapping.ContainsKey (newAdapter.Provider))
//				return;
			
			providerAdapterMapping [newAdapter.Provider] = newAdapter;
			parentAdapter.AddOneChild (newAdapter);
		}
		
		private void HandleNewDataGridControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);

			Adapter newAdapter = new DataGrid (provider);

			IncludeNewAdapter (newAdapter, parentObject);
		}

		private void AddChildrenToParent (IRawElementProviderSimple provider)
		{
			IRawElementProviderFragment root = provider as IRawElementProviderFragment;
			if (root == null)
				return;
			IRawElementProviderFragment child 
				= root.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				AutomationInteropProvider.RaiseStructureChangedEvent (child, 
				                                                      new StructureChangedEventArgs (StructureChangeType.ChildAdded,
				                                                                                     child.GetRuntimeId ()));				
				child = child.Navigate (NavigateDirection.NextSibling);
			}
		}

		private void HandleNewHeaderItemControlType (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);
			
			TextLabel atkLabel = new TextLabel (provider);

			IncludeNewAdapter (atkLabel, parentObject);
		}
		
#endregion
	}
}
