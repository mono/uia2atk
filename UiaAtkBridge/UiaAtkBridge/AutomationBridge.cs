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
using System.Diagnostics;
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
		static private AmbiDictionary<IRawElementProviderSimple, Atk.Object> providerAdapterMapping;
		
		private int windowProviders;
		
#endregion

#region Public Constructor
		
		public AutomationBridge ()
		{
			bool newMonitor = false;

			pointerProviderMapping =
				new AmbiDictionary<IntPtr,IRawElementProviderSimple> ();
			providerAdapterMapping =
				new AmbiDictionary<IRawElementProviderSimple, Atk.Object>();

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
							Adapter adapter = providerAdapterMapping [providerReady] as Adapter;
							if (adapter != null)
								RequestChildren (adapter, alreadyRequestedChildren);
						}
						alreadyRequestedChildren = null;
					}
					
					Atk.Object obj;
					if (providerAdapterMapping.TryGetValue (provider, out obj))
						return obj as Adapter;
					return null;
				} finally {
					alreadyInLookup = false;
				}
			}
		}
		
		private static void RequestChildren (Atk.Object adapter, List<Atk.Object> alreadyRequestedChildren)
		{
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
				// FXIME: Implement to enable/disable bridge when ATs are/aren't running.
				return true;
			}
		}

 		public bool IsAccessibilityEnabled {
 			get {
				// FIXME: This is a temporary hack, we will replace it, proposed solutions:
				// - Use GConf API (we will need to fix threading issues).
				// - <Insert your fantastic idea here>
				string output = bool.FalseString;
				bool enabled = false;
				
				ProcessStartInfo
					processInfo = new ProcessStartInfo ("gconftool-2",
					                                    "-g /desktop/gnome/interface/accessibility");
				processInfo.UseShellExecute = false;
				processInfo.ErrorDialog = false;
				processInfo.CreateNoWindow = true;
				processInfo.RedirectStandardOutput = true;
				
				try {
					Process gconftool2 = Process.Start (processInfo);
					output = gconftool2.StandardOutput.ReadToEnd () ?? bool.FalseString;
					gconftool2.WaitForExit ();
					gconftool2.Close ();
				} catch (System.IO.FileNotFoundException) {}

				try {
					enabled = bool.Parse (output);
				} catch (FormatException) {}
				
				return enabled;
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
			keyStrings [0xff08] = "BackSpace";
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
			if (appMonitor == null)
				return;
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
				// cache the values when the key is pressed
				evnt.Keyval = downKeys [e.Keycode].Keyval;
				evnt.String = downKeys [e.Keycode].String;
				downKeys.Remove (e.Keycode);
			} else {
				evnt.Keyval = (uint)e.Keysym;
				evnt.String = (keyStrings.ContainsKey (evnt.Keyval)? keyStrings [evnt.Keyval]: e.Str);
				downKeys [e.Keycode] = new KeyDefinition ((uint)e.Keysym, evnt.String);
			}
			evnt.Length = evnt.String.Length;
			evnt.Timestamp = (uint)((DateTime.Now.Ticks - initTime) / 10000);
			e.SuppressKeyPress = appMonitor.HandleKey (evnt);
		}
		
		public void RaiseAutomationEvent (AutomationEvent eventId, object provider, AutomationEventArgs e)
		{
//			if (e!= null)
//				Console.WriteLine ("RaiseAutomationEvent{0}", e.EventId.ProgrammaticName);
			
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
			
			Adapter adapter = providerAdapterMapping [simpleProvider] as Adapter;
			if (adapter != null)
				adapter.RaiseAutomationEvent (eventId, e);
		}
		
		public void RaiseAutomationPropertyChangedEvent (object element, AutomationPropertyChangedEventArgs e)
		{
			if (element == null)
				throw new ArgumentNullException ("element");
			
//			Console.WriteLine ("RaiseAutomationPropertyChangedEvent{0}:{1}", 
//			  e.Property.ProgrammaticName, e == null ? "" : e.NewValue == null ? "" : e.NewValue.ToString());
			
			IRawElementProviderSimple simpleProvider =
				(IRawElementProviderSimple) element;
			// Create an adapter if we haven't already, so that
			// an AT will know about the control that has focus,
			// but don't do this if we're shutting down (ie,
			// providerAdapterMapping.Count == 0)
			if (e.Property == AutomationElementIdentifiers.HasKeyboardFocusProperty &&
			  !providerAdapterMapping.ContainsKey (simpleProvider) &&
			  providerAdapterMapping.Count > 0) {
				int controlTypeId = (int) simpleProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
				// Currently we don't instantiate adapters for
				// DataItems; their children become children
				// of the DataItem's parent
				if (controlTypeId == ControlType.DataItem.Id && simpleProvider is IRawElementProviderFragment) {
					IRawElementProviderFragment child = ((IRawElementProviderFragment)simpleProvider).Navigate (NavigateDirection.FirstChild);
					if (!(providerAdapterMapping.ContainsKey (child)))
						HandlePossiblePreExistingProvider (child);
					if (providerAdapterMapping.ContainsKey (child))
						((Adapter)providerAdapterMapping [child]).RaiseAutomationPropertyChangedEvent (e);
					return;
				} else
					HandlePossiblePreExistingProvider (simpleProvider);
			}

			if ((!providerAdapterMapping.ContainsKey (simpleProvider)) || windowProviders == 0)
				return;
			
			Adapter adapter = providerAdapterMapping [simpleProvider] as Adapter;
			if (adapter != null)
				adapter.RaiseAutomationPropertyChangedEvent (e);
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

		public void Initialize ()
		{
			if (appMonitor == null)
				appMonitor = new Monitor();
		}

		public void Terminate ()
		{
			//TODO: Something to terminate ?
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

			Atk.Object parent = null;
			if (!providerAdapterMapping.TryGetValue (parentProvider, out parent))
				return null;
			ParentAdapter ret = parent as ParentAdapter;
			if (ret == null)
				Console.WriteLine ("AutomationBridge: warning: Could not cast " + parent + " to ParentAdapter");
			return ret;
		}

		private void HandleElementAddition (IRawElementProviderSimple simpleProvider)
		{
			bool? hasNativeAtkObj = (bool?) simpleProvider.GetPropertyValue (AutomationElementIdentifiers.HasNativeAccessibilityObjectProperty.Id);
			if (hasNativeAtkObj == true &&
			    HandleNewControlWithNativeObject (simpleProvider))
				return;

			if (providerAdapterMapping.ContainsKey (simpleProvider))
				return;

			int controlTypeId = (int) simpleProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);

			// All controls that have TopLevelRootItem as the
			// parent go here
			if (controlTypeId == ControlType.Window.Id)
				HandleNewWindowControlType (simpleProvider);
 			else if (controlTypeId == ControlType.ToolTip.Id)
 				HandleNewToolTipControlType (simpleProvider);

			ParentAdapter parentAdapter = GetParentAdapter (simpleProvider);
			if (parentAdapter == null) {
				// We're likely disposing and were interrupted
				// by an event that is causing an element to be
				// added.  In the case where the parent hasn't
				// been created yet, we'll walk its hierarchy
				// when it is added later.
				return;
			}

			// All controls that need parents go here
			if (controlTypeId == ControlType.Button.Id)
				// TODO: Consider generalizing...
				HandleNewButtonControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.Text.Id)
				HandleNewLabelControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.CheckBox.Id)
				HandleNewCheckBoxControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.List.Id)
				HandleNewListControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.ListItem.Id)
				HandleNewListItemControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.ComboBox.Id)
				HandleNewComboBoxControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.StatusBar.Id)
				HandleNewStatusBarControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.ProgressBar.Id)
				HandleNewProgressBarControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.ScrollBar.Id)
				HandleNewScrollBarControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.Group.Id)
				HandleNewGroupControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.RadioButton.Id)
				HandleNewRadioButtonControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.Spinner.Id)
				HandleNewSpinnerControlType (simpleProvider, parentAdapter);
 			else if (controlTypeId == ControlType.Hyperlink.Id)
 				HandleNewHyperlinkControlType (simpleProvider, parentAdapter);
			else if ((controlTypeId == ControlType.Document.Id) || (controlTypeId == ControlType.Edit.Id))
				HandleNewDocumentOrEditControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.Image.Id)
				HandleNewImageControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.ToolBar.Id)
				HandleNewContainer (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.Header.Id)
				AddChildrenToParent (simpleProvider);
			else if (controlTypeId == ControlType.HeaderItem.Id)
				HandleNewHeaderItemControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.MenuBar.Id) //for MenuStrip widget
				// || (controlTypeId == ControlType.Menu.Id)) //for 1.x Menu widget it seems <- TODO
				HandleNewMenuBarControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.MenuItem.Id) //for ToolStripMenuItem widget
				HandleNewMenuItemControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.DataGrid.Id) //for ToolStripMenuItem widget
				HandleNewDataGridControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.DataItem.Id)
				AddChildrenToParent (simpleProvider);
			else if (controlTypeId == ControlType.Pane.Id)
				HandleNewContainer (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.SplitButton.Id)
				HandleNewSplitButton (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.Tab.Id)
				HandleNewTab (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.TabItem.Id)
				HandleNewTabItem (simpleProvider, parentAdapter);
			// TODO: Other providers
			else if (controlTypeId != ControlType.Thumb.Id)
				Console.WriteLine ("AutomationBridge: Unhandled control: " +
				                   ControlType.LookupById (controlTypeId).ProgrammaticName);
		}

		private bool HandleElementRemoval (Atk.Object atkObj)
		{
			IRawElementProviderSimple provider;
			if (atkObj is Adapter &&
			    providerAdapterMapping.TryGetKey ((Adapter)atkObj, out provider))
				return HandleElementRemoval (provider);
			return false;
		}

		private bool HandleTotalElementRemoval (IRawElementProviderSimple provider)
		{
			bool lastWindowProvider = false;
			
			Atk.Object obj;
			if (providerAdapterMapping.TryGetValue (provider, out obj) == false)
				return false;
			Adapter adapter = obj as Adapter;
			if (adapter == null)
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
			bool? hasNativeAtkObj = (bool?) provider.GetPropertyValue (AutomationElementIdentifiers.HasNativeAccessibilityObjectProperty.Id);
			if (hasNativeAtkObj == true) {
				Atk.Object nativeObj = (Atk.Object)
					provider.GetPropertyValue (AutomationElementIdentifiers.NativeAccessibilityObjectProperty.Id);
				ParentAdapter parent = nativeObj.Parent as ParentAdapter;
				if (parent != null)
					parent.RemoveChild (nativeObj);
				return false;
			}
			
			bool lastWindowProvider = false;
			
			Atk.Object obj;
			if (providerAdapterMapping.TryGetValue (provider, out obj) == false)
				return false;
			Adapter adapter = (Adapter)obj;

			int controlTypeId = (int)provider.GetPropertyValue (
				AutomationElementIdentifiers.ControlTypeProperty.Id);
			
			if (controlTypeId == ControlType.ToolTip.Id) {
				TopLevelRootItem.Instance.RemoveChild (adapter);
			} else if (controlTypeId == ControlType.Window.Id) {
				// We should do the following, but it would
				// reintroduce bug 427857.
				//GLib.Signal.Emit (adapter, "deactivate");
				//GLib.Signal.Emit (adapter, "destroy");
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
			List<Atk.Object> keep = new List<Atk.Object> ();
			IRawElementProviderFragment child = fragment.Navigate(NavigateDirection.FirstChild);
			while (child != null) {
				if (providerAdapterMapping.ContainsKey (child))
					keep.Add (providerAdapterMapping [child]);
				child = child.Navigate (NavigateDirection.NextSibling);
			}
			Atk.Object obj = providerAdapterMapping [provider];
			int index = obj.NAccessibleChildren;
			while (--index >= 0) {
				Adapter childAdapter = obj.RefAccessibleChild (index) as Adapter;
				if (!keep.Contains(childAdapter) && !childAdapter.ManagesRemoval) {
					HandleElementRemoval (childAdapter.Provider);
				}
			}
		}

		private void HandleNewWindowControlType (IRawElementProviderSimple provider)
		{
			Window newWindow = new Window (provider);

			IncludeNewAdapter (newWindow, TopLevelRootItem.Instance);

			IntPtr providerHandle = (IntPtr) provider.GetPropertyValue (AutomationElementIdentifiers.NativeWindowHandleProperty.Id);
			pointerProviderMapping [providerHandle] = provider;
			
			GLib.Signal.Emit (newWindow, "create");
			GLib.Signal.Emit (newWindow, "activate");
			
			windowProviders++;
		}
		
		private void HandleNewToolTipControlType (IRawElementProviderSimple provider)
		{
			ToolTip atkToolTip = new ToolTip (provider);

			IncludeNewAdapter (atkToolTip, TopLevelRootItem.Instance);
		}

		private bool HandleNewControlWithNativeObject (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);

			// TODO: Thread this better
			System.Threading.Thread.Sleep (500);
			Atk.Object nativeObj = (Atk.Object)
				provider.GetPropertyValue (AutomationElementIdentifiers.NativeAccessibilityObjectProperty.Id);
			
			if (nativeObj == null) {
				Console.WriteLine ("UiaAtkBridge: Couldn't get an atk object for a WebBrowser");
				return false;
			}
				providerAdapterMapping [provider] = nativeObj;
			parentObject.AddOneChild (nativeObj);
			return true;
		}

		private void HandleNewButtonControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			if (parentObject is UiaAtkBridge.ComboBox)
				return; //ComboBox will handle its children additions on its own
			
			if (parentObject is UiaAtkBridge.List)
				return; //Not replicating DomainUpDown buttons
			
			if ((parentObject == null) || (parentObject.Role == Atk.Role.ScrollBar))
				return;

			Button atkButton = new Button (provider);
			
			IncludeNewAdapter (atkButton, parentObject);
		}

		private void HandleNewLabelControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			TextLabel atkLabel;

			if (parentObject != null &&
			    ((int) parentObject.Provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)) == ControlType.StatusBar.Id)
				atkLabel = new TextImageLabel (provider);
			else
				atkLabel = new TextLabel (provider);

			IncludeNewAdapter (atkLabel, parentObject);
		}

		private void HandleNewCheckBoxControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			CheckBoxButton atkCheck = new CheckBoxButton (provider);

			IncludeNewAdapter (atkCheck, parentObject);
		}
		
		private void HandleNewListControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter atkList;
			if (parentObject is UiaAtkBridge.ComboBox) {
				if (parentObject is UiaAtkBridge.ComboBoxDropDown)
					atkList = new MenuItem (provider);
				else
					atkList = new ComboBoxTable (provider);
			}
			else if (provider is IGridProvider)
				atkList = new ListWithGrid ((IRawElementProviderFragmentRoot)provider);
			else
				atkList = new List ((IRawElementProviderFragmentRoot)provider);

			IncludeNewAdapter (atkList, parentObject);
		}

		private void HandleNewListItemControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter atkItem;
			if (parentObject is ComboBoxTable)
				atkItem = new ComboBoxItem (provider);
			else if (parentObject is MenuItem)
				atkItem = new MenuItem (provider);
			else
				atkItem = new ListItem (provider);
			
			IncludeNewAdapter (atkItem, parentObject);
		}

		private void HandleNewComboBoxControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			ComboBox atkCombo;
			if (UiaAtkBridge.ComboBox.IsSimple (provider))
				atkCombo = new ComboBox (provider);
			else
				atkCombo = new ComboBoxDropDown (provider);
			
			IncludeNewAdapter (atkCombo, parentObject);
		}
		
		private void HandleNewStatusBarControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			TextContainer atkStatus;
			//FIXME: probably we shouldn't split this in 2 classes
			//FIXME: not sure if this interface check is correct
			if (provider is IGridProvider)
				atkStatus = new TextContainerWithGrid (provider);
 			else 
				atkStatus = new TextContainer (provider);

			IncludeNewAdapter (atkStatus, parentObject);
		}

		private void HandleNewProgressBarControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			ProgressBar atkProgress = new ProgressBar (provider);
			
			IncludeNewAdapter (atkProgress, parentObject);
		}
		
		private void HandleNewScrollBarControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			ScrollBar atkScroll = new ScrollBar (provider);

			IncludeNewAdapter (atkScroll, parentObject);
		}
		
		private void HandleNewGroupControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter newAdapter = null;
			if (parentObject is DataGrid)
				newAdapter = new DataGridGroup (provider);
			else if (parentObject is List && provider is IRawElementProviderFragment)
				newAdapter = new ListGroup ((IRawElementProviderFragment)provider);
			else
				newAdapter = new Container (provider);

			IncludeNewAdapter (newAdapter, parentObject);
		}
		
		private void HandleNewRadioButtonControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			RadioButton atkRadio = new RadioButton (provider);

			IncludeNewAdapter (atkRadio, parentObject);
		}
		
		private void HandleNewSpinnerControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter atkSpinner;
			if (provider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id) != null)
				if (provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id) != null)
					atkSpinner = new ListWithEditableText ((IRawElementProviderFragmentRoot)provider);
				else
					atkSpinner = new List ((IRawElementProviderFragmentRoot)provider);
			else if (provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id) != null)
				atkSpinner = new SpinnerWithValue (provider);
 			else
				return;

			IncludeNewAdapter (atkSpinner, parentObject);
		}
		
		private void HandleNewDocumentOrEditControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter atkEditOrDoc = new TextBoxEntryView (provider);
			
			IncludeNewAdapter (atkEditOrDoc, parentObject);
		}

		private void HandleNewHyperlinkControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
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

		private void HandleNewImageControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter atkImage = new Image (provider);

			IncludeNewAdapter (atkImage, parentObject);
		}

		private void HandleNewContainer (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter atkContainer = new Container (provider);
			providerAdapterMapping [provider] = atkContainer;
			
			IncludeNewAdapter (atkContainer, parentObject);
		}

		private void HandleNewMenuBarControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			MenuBar newMenuBar = new MenuBar (provider);

			IncludeNewAdapter (newMenuBar, parentObject);
		}

		private void HandleNewMenuItemControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter newAdapter = new MenuItem (provider);
			IncludeNewAdapter (newAdapter, parentObject);
		}

		private void HandleNewSplitButton (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			SplitButton splitButton = new SplitButton (provider);
			
			IncludeNewAdapter (splitButton, parentObject);
		}

		private void HandleNewTab (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
			if (fragment == null) {
				Console.WriteLine ("UiaAtkBridge: warning: Tab must be a fragment; ignoring");
				return;
			}
				
			Tab tab = new Tab (fragment);
			
			IncludeNewAdapter (tab, parentObject);
		}

		private void HandleNewTabItem (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			TextContainer tabItem = new TextContainer (provider);
			
			IncludeNewAdapter (tabItem, parentObject);
		}

		internal static void IncludeNewAdapter (Adapter newAdapter, ParentAdapter parentAdapter)
		{
			if (newAdapter.Provider == null)
				throw new ArgumentException (String.Format ("{0} adapter should have a not null provider", newAdapter.GetType ().Name));

			//FIXME: figure out why we can't uncomment this:
//			if (providerAdapterMapping.ContainsKey (newAdapter.Provider))
//				return;

			if (parentAdapter == null) {
				// Either the parent hasn't been created yet
				// (and we'll get called later), or we're
				// currently being destroyed
				return;
			}

			providerAdapterMapping [newAdapter.Provider] = newAdapter;
			parentAdapter.AddOneChild (newAdapter);
		}
		
		private void HandleNewDataGridControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
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

		private void HandleNewHeaderItemControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			TextLabel atkLabel = new TextLabel (provider);

			IncludeNewAdapter (atkLabel, parentObject);
		}
		
		// This whole function is a hack to work around the
		// bridge not instantiating providers for controls which
		// existed prior to the provider being created.
		private void HandlePossiblePreExistingProvider (IRawElementProviderSimple provider)
		{
Console.WriteLine ("check: " + provider);
			IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
			if (fragment == null)
				return;
			IRawElementProviderFragment parent = fragment.Navigate (NavigateDirection.Parent);
Console.WriteLine ("parent: " + parent);
			if (parent == null || parent == provider)
				return;
			int controlTypeId = (int) parent.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (controlTypeId == ControlType.DataItem.Id)
				parent = parent.Navigate (NavigateDirection.Parent);
Console.WriteLine ("parent: " + parent);

			Atk.Object obj = null;
			if (!providerAdapterMapping.TryGetValue (parent, out obj))
				HandlePossiblePreExistingProvider (parent);
			if (!providerAdapterMapping.TryGetValue (parent, out obj))
				return;
			ParentAdapter parentAdapter = obj as ParentAdapter;
Console.WriteLine ("Done recursing: " + provider + ", parentAdapter " + obj);
			if (parentAdapter != null) {
				if (parentAdapter.RefStateSet().ContainsState (Atk.StateType.ManagesDescendants))
					HandleElementAddition (provider);
				else
					// Otherwise try to keep things in
					// order, so request all children
					parentAdapter.RequestChildren ();
			}
		}
#endregion
	}
}
