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
using Mono.UIAutomation.Services;
using System.Windows.Automation.Provider;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;


namespace UiaAtkBridge
{
	public class AutomationBridge : IAutomationBridge
	{
#region Private Fields
		
		private bool applicationStarted = false;
		private Monitor appMonitor = null;
		private long initTime = DateTime.Now.Ticks;
		static private AmbiDictionary<IntPtr, IRawElementProviderSimple> pointerProviderMapping;
		static private AmbiDictionary<IRawElementProviderSimple, Atk.Object> providerAdapterMapping;
		
		private static int windowProviders;
		
#endregion

#region Public Constructor
		
		public AutomationBridge ()
		{
			pointerProviderMapping =
				new AmbiDictionary<IntPtr,IRawElementProviderSimple> ();
			providerAdapterMapping =
				new AmbiDictionary<IRawElementProviderSimple, Atk.Object>();

			windowProviders = 0;
		}

		public static void Quit ()
		{
			Monitor.Instance.Dispose ();
		}
		
#endregion
		
#region Public Methods

		static object adapterLookup = new object ();
		static bool alreadyInLookup = false;
		const uint INFINITE = uint.MaxValue;
		
		public static Adapter GetAdapterForProviderLazy (IRawElementProviderSimple provider)
		{
			return GetAdapterForProvider (provider, 0);
		}

		public static Adapter GetAdapterForProviderSemiLazy (IRawElementProviderSimple provider)
		{
			IRawElementProviderFragment navigator = (IRawElementProviderFragment)provider;
			Queue <IRawElementProviderFragment> parents = new Queue<IRawElementProviderFragment> ();
			Adapter adapter = GetAdapterForProviderLazy (navigator);

			while ((adapter == null) && (navigator != null)) {
				navigator = navigator.Navigate (NavigateDirection.Parent);
				parents.Enqueue (navigator);
				adapter = GetAdapterForProviderLazy (navigator);
			}

			if (parents.Count == 0)
				return adapter;
			ParentAdapter parentAdapter = adapter as ParentAdapter;
			if (parentAdapter == null)
				return null;

			parents.Dequeue ();
			parentAdapter.RequestChildren ();
			while (parents.Count > 0) {
				parentAdapter = (ParentAdapter)GetAdapterForProviderLazy (parents.Dequeue ());
				parentAdapter.RequestChildren ();
			}
			return GetAdapterForProviderLazy (provider);
		}
		
		[Obsolete("Use GetAdapterForProviderLazy as it's more efficient")]
		public static Adapter GetAdapterForProvider (IRawElementProviderSimple provider)
		{
			return GetAdapterForProvider (provider, INFINITE);
		}
		
		private static Adapter GetAdapterForProvider (IRawElementProviderSimple provider, uint lazyLoadingLevel)
		{
			lock (adapterLookup) {
				if (alreadyInLookup)
					throw new Exception ("Cannot run nested GetAdapterForProvider lookups");
				alreadyInLookup = true;

				try {
					if (lazyLoadingLevel > 0) {
						Log.Warn ("Obsolete non-lazy-loading GetAdapterForProvider method called.");
						
						List <Atk.Object> alreadyRequestedChildren = new List <Atk.Object> ();
						List <IRawElementProviderSimple> initialProvs = new List <IRawElementProviderSimple> ();
						
						foreach (IRawElementProviderSimple providerReady in providerAdapterMapping.Keys) {
							initialProvs.Add (providerReady);
						}
						
						foreach (IRawElementProviderSimple providerReady in initialProvs) {
							Adapter adapter = providerAdapterMapping [providerReady] as Adapter;
							if (adapter != null)
								RequestChildren (adapter, alreadyRequestedChildren, lazyLoadingLevel);
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
		
		private static void RequestChildren (Atk.Object adapter, List<Atk.Object> alreadyRequestedChildren, uint level)
		{
			if (alreadyRequestedChildren.Contains (adapter) || level == 0)
				return;

			int nChildren = adapter.NAccessibleChildren;
			
			if (nChildren > 0) {
				for (int i = 0; i < nChildren; i++) {
					Atk.Object adapterN = adapter.RefAccessibleChild (i);
					
					if (i == 0)
						alreadyRequestedChildren.Add (adapter);
					RequestChildren (adapterN, alreadyRequestedChildren, level - 1);
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
			//Console.WriteLine (String.Format("RaiseStructureChangedEvent:{0}-{1}",
			//                                 e.StructureChangeType.ToString (),
			//                                 provider.GetType ().Name));
			if (e.StructureChangeType == StructureChangeType.ChildrenBulkAdded) {
				HandleBulkAdded (simpleProvider);
			} else if (e.StructureChangeType == StructureChangeType.ChildAdded) {
				if (!providerAdapterMapping.ContainsKey (simpleProvider))
					HandleElementAddition (simpleProvider);
			} else if (e.StructureChangeType == StructureChangeType.ChildRemoved) {
				// TODO: Handle proper documented args
				//       (see FragmentRootControlProvider)
				if (HandleTotalElementRemoval (simpleProvider))
					Monitor.Instance.Dispose ();
			} else if (e.StructureChangeType == StructureChangeType.ChildrenBulkRemoved) {
				HandleBulkRemoved (simpleProvider);
			} else if (e.StructureChangeType == StructureChangeType.ChildrenInvalidated) {
				// These often seem to be coupled with
				// add/removed/reordered events.
			}
			else
				Log.Warn ("StructureChangedEvent not handled: {0}", e.StructureChangeType);
			
			// TODO: Other structure changes
		}

		public void Initialize (IntPtr parentObject)
		{
			if (parentObject == IntPtr.Zero)
				if (appMonitor == null)
					appMonitor = Monitor.Instance;
			else
				new Monitor (parentObject);
		}

		public void Terminate ()
		{
			//TODO: Merge with Dispose?
		}
		
#endregion
		
#region Private Methods
		
		internal static ParentAdapter GetParentAdapter (IRawElementProviderSimple provider)
		{
			IRawElementProviderFragment fragment = (IRawElementProviderFragment)provider;
			IRawElementProviderFragment parentProvider;

			parentProvider = fragment.Navigate (NavigateDirection.Parent);

			if (parentProvider == null)
				return null;

			int parentControlTypeId = (int)
				parentProvider.GetPropertyValue (AEIds.ControlTypeProperty.Id);
			if (parentControlTypeId == ControlType.Header.Id ||
			    parentControlTypeId == ControlType.DataItem.Id ||
			    parentControlTypeId == ControlType.TreeItem.Id)
				return GetParentAdapter (parentProvider);

			// For a MenuItem provider, a Menu parent could indicate
			// the hidden menu generated when a parent MenuItem is
			// expanded. So if grandparent is MenuItem, treat its
			// Adapter as the ParentAdapter.
			int providerControlTypeId = (int)
				provider.GetPropertyValue (AEIds.ControlTypeProperty.Id);

			if (providerControlTypeId == ControlType.MenuItem.Id &&
			    parentControlTypeId == ControlType.Menu.Id) {
				IRawElementProviderFragment grandParentProvider =
					parentProvider.Navigate (NavigateDirection.Parent);
				if (grandParentProvider != null &&
				    ControlType.MenuItem.Id == (int) grandParentProvider.GetPropertyValue (AEIds.ControlTypeProperty.Id)) {
					Atk.Object grandParentAdapter = null;
					if (!providerAdapterMapping.TryGetValue (grandParentProvider, out grandParentAdapter))
						return null;
					return grandParentAdapter as ParentAdapter;
				}
			}

			if (!providerAdapterMapping.ContainsKey (parentProvider))
				HandleElementAddition (parentProvider);

			Atk.Object parent = null;
			if (!providerAdapterMapping.TryGetValue (parentProvider, out parent))
				return null;
			ParentAdapter ret = parent as ParentAdapter;
			if (ret == null) {
				//FIXME: we should throw an exception here if in DEBUG mode
				Log.Warn ("AutomationBridge: Could not cast {0} to ParentAdapter", parent);
				
			}
			return ret;
		}

		private static void HandleElementAddition (IRawElementProviderSimple simpleProvider)
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
			if (controlTypeId == ControlType.Window.Id) {
				HandleNewWindowControlType (simpleProvider);
				return;
			} else if (controlTypeId == ControlType.ToolTip.Id) {
 				HandleNewToolTipControlType (simpleProvider);
				return;
			} else if (controlTypeId == ControlType.Menu.Id) { //for ContextMenuStrip widget
				HandleNewMenuControlType (simpleProvider);
				return;
			}

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
				HandleNewMenuBarControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.MenuItem.Id) //for ToolStripMenuItem widget
				HandleNewMenuItemControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.DataGrid.Id) //for ToolStripMenuItem widget
				HandleNewDataGridControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.Table.Id)
				HandleNewTableControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.DataItem.Id)
				AddChildrenToParent (simpleProvider);
			else if (controlTypeId == ControlType.Pane.Id)
				HandleNewPane (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.SplitButton.Id)
				HandleNewSplitButton (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.Tab.Id)
				HandleNewTab (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.TabItem.Id)
				HandleNewTabItem (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.Tree.Id)
				HandleNewTree (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.TreeItem.Id)
				HandleNewTreeItem (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.Separator.Id)
				HandleNewSeparator (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.Slider.Id)
				HandleNewSliderControlType (simpleProvider, parentAdapter);
			else if (controlTypeId == ControlType.Calendar.Id)
				HandleNewCalendarControlType (simpleProvider, parentAdapter);
			// TODO: Other providers
			else if (controlTypeId != ControlType.Thumb.Id)
				Log.Warn ("AutomationBridge: Unhandled control: {0}",
				          ControlType.LookupById (controlTypeId).ProgrammaticName);
		}

		private static bool HandleElementRemoval (Atk.Object atkObj)
		{
			Adapter adapter = atkObj as Adapter;
			if (adapter != null && adapter.Provider != null)
				return HandleElementRemoval (adapter.Provider);
			return false;
		}

		internal static bool HandleTotalElementRemoval (IRawElementProviderSimple provider)
		{
			bool lastWindowProvider = false;
			
			int controlTypeId = (int)provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (controlTypeId == ControlType.DataItem.Id) {
				// No adapter created for it, but adapters
				// created for its children
				IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
				if (fragment == null)
					return false;
				for (IRawElementProviderFragment child = fragment.Navigate (NavigateDirection.FirstChild); 
				     child != null; 
				     child = child.Navigate (NavigateDirection.NextSibling))
					HandleTotalElementRemoval (child);
				return false;
			}

			Atk.Object obj;
			if (providerAdapterMapping.TryGetValue (provider, out obj) == false)
				return false;
			Adapter adapter = obj as Adapter;
			if (adapter == null)
				return false;

			ParentAdapter parentAdapter = adapter.Parent as ParentAdapter;
			if (parentAdapter != null)
				parentAdapter.PreRemoveChild (adapter);

			foreach (Atk.Object atkObj in GetAdaptersDescendantsFamily (adapter)) {
				if (HandleElementRemoval (atkObj))
					lastWindowProvider = true;
			}

			return lastWindowProvider;
		}
		
		private static bool HandleElementRemoval (IRawElementProviderSimple provider)
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

		private static List<Atk.Object> GetAdaptersDescendantsFamily (Atk.Object adapter) {
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
			int controlTypeId = (int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (!providerAdapterMapping.ContainsKey (provider)) {
				if (controlTypeId == ControlType.Header.Id || controlTypeId == ControlType.DataItem.Id || controlTypeId == ControlType.TreeItem.Id) {
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
			IRawElementProviderFragment child = fragment.Navigate(NavigateDirection.FirstChild);
			List<Atk.Object> keep;
			if (controlTypeId == ControlType.Tree.Id || controlTypeId == ControlType.DataGrid.Id)
				keep = BuildKeepListRecurse (child);
			else
				keep = BuildKeepListNoRecurse (child);
			Atk.Object obj = providerAdapterMapping [provider];
			int index = obj.NAccessibleChildren;
			while (--index >= 0) {
				Adapter childAdapter = obj.RefAccessibleChild (index) as Adapter;
				if (!keep.Contains(childAdapter) && !childAdapter.ManagesRemoval) {
					HandleElementRemoval (childAdapter.Provider);
				}
			}
		}

		private List<Atk.Object> BuildKeepListNoRecurse (IRawElementProviderFragment fragment)
		{
			List<Atk.Object> keep = new List<Atk.Object> ();
			while (fragment != null) {
				if (providerAdapterMapping.ContainsKey (fragment))
					keep.Add (providerAdapterMapping [fragment]);
				fragment = fragment.Navigate (NavigateDirection.NextSibling);
			}
			return keep;
		}

		private List<Atk.Object> BuildKeepListRecurse (IRawElementProviderFragment fragment)
		{
			List<Atk.Object> keep = new List<Atk.Object> ();
			IRawElementProviderFragment child;
			int depth = 0;
			while (fragment != null) {
				if (providerAdapterMapping.ContainsKey (fragment))
					keep.Add (providerAdapterMapping [fragment]);
				child = fragment.Navigate (NavigateDirection.FirstChild);
				if (child != null) {
					fragment = child;
					depth++;
				} else {
					IRawElementProviderFragment next = fragment.Navigate (NavigateDirection.NextSibling);
					if (next == null && depth > 0) {
						depth--;
						fragment = fragment.Navigate (NavigateDirection.Parent).Navigate (NavigateDirection.NextSibling);
					} else fragment = next;
				}
			}
			return keep;
		}

		private static void HandleNewWindowControlType (IRawElementProviderSimple provider)
		{
			Window newWindow = new Window (provider);

			IncludeNewAdapter (newWindow, TopLevelRootItem.Instance);

			IntPtr providerHandle = (IntPtr) provider.GetPropertyValue (AutomationElementIdentifiers.NativeWindowHandleProperty.Id);
			pointerProviderMapping [providerHandle] = provider;

			GLib.Signal.Emit (newWindow, "create");
			
			windowProviders++;
		}
		
		private static void HandleNewToolTipControlType (IRawElementProviderSimple provider)
		{
			ToolTip atkToolTip = new ToolTip (provider);

			IncludeNewAdapter (atkToolTip, TopLevelRootItem.Instance);
		}

		private static bool HandleNewControlWithNativeObject (IRawElementProviderSimple provider)
		{
			ParentAdapter parentObject = GetParentAdapter (provider);

			// TODO: Thread this better
			System.Threading.Thread.Sleep (500);
			Atk.Object nativeObj = (Atk.Object)
				provider.GetPropertyValue (AutomationElementIdentifiers.NativeAccessibilityObjectProperty.Id);
			
			if (nativeObj == null) {
				Log.Error ("UiaAtkBridge: Couldn't get an atk object for a WebBrowser");
				return false;
			}
			providerAdapterMapping [provider] = nativeObj;
			parentObject.AddOneChild (nativeObj);
			return true;
		}

		private static void HandleNewButtonControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			if (parentObject is UiaAtkBridge.ComboBox)
				return; //ComboBox will handle its children additions on its own
			
			if (parentObject is UiaAtkBridge.List)
				return; //Not replicating DomainUpDown buttons

			if (parentObject is UiaAtkBridge.Container && parentObject.Role == Atk.Role.ToolBar) {
				ToggleButton atkToggle = new ToggleButton (provider);
				IncludeNewAdapter (atkToggle, parentObject);
				return;
			}
			
			if ((parentObject == null) || (parentObject.Role == Atk.Role.ScrollBar))
				return;

			Button atkButton = new Button (provider);
			
			IncludeNewAdapter (atkButton, parentObject);
		}

		private static void HandleNewLabelControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			TextLabel atkLabel;

			if (parentObject != null &&
			    ((int) parentObject.Provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id)) == ControlType.StatusBar.Id)
				atkLabel = new TextImageLabel (provider);
			else
				atkLabel = new TextLabel (provider);

			IncludeNewAdapter (atkLabel, parentObject);
		}

		private static void HandleNewCheckBoxControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			CheckBoxButton atkCheck = new CheckBoxButton (provider);

			IncludeNewAdapter (atkCheck, parentObject);
		}
		
		private static void HandleNewListControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter atkList;
			if (parentObject is UiaAtkBridge.ComboBoxDropDown)
				atkList = new ComboBoxOptions (provider);
			else if (parentObject is UiaAtkBridge.ComboBox)
				atkList = new ComboBoxOptionsTable (provider);
			else if (provider is IGridProvider)
				atkList = new ListWithGrid ((IRawElementProviderFragmentRoot)provider);
			else
				atkList = new Tree ((IRawElementProviderFragmentRoot)provider);

			IncludeNewAdapter (atkList, parentObject);
		}

		private static void HandleNewListItemControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter atkItem;
			if (parentObject is ComboBoxOptions)
				atkItem = new ComboBoxItem (provider);
			else if (parentObject is List)
				atkItem = new ListItem (provider);
			else
				atkItem = new TreeItem (provider);
			
			IncludeNewAdapter (atkItem, parentObject);
		}

		private static void HandleNewComboBoxControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			ComboBox atkCombo;
			if (UiaAtkBridge.ComboBox.IsSimple (provider))
				atkCombo = new ComboBox (provider);
			else
				atkCombo = new ComboBoxDropDown (provider);
			
			IncludeNewAdapter (atkCombo, parentObject);
		}
		
		private static void HandleNewStatusBarControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
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

		private static void HandleNewProgressBarControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			ProgressBar atkProgress = new ProgressBar (provider);
			
			IncludeNewAdapter (atkProgress, parentObject);
		}
		
		private static void HandleNewScrollBarControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			ScrollBar atkScroll = new ScrollBar (provider);

			IncludeNewAdapter (atkScroll, parentObject);
		}
		
		private static void HandleNewGroupControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter newAdapter = null;
			if (parentObject is DataGrid)
				newAdapter = new DataGridGroup (provider);
			else if (parentObject is Tree && provider is IRawElementProviderFragment)
				newAdapter = new ListGroup ((IRawElementProviderFragment)provider);
			else
				newAdapter = new Container (provider);

			IncludeNewAdapter (newAdapter, parentObject);
		}
		
		private static void HandleNewRadioButtonControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			RadioButton atkRadio = new RadioButton (provider);

			IncludeNewAdapter (atkRadio, parentObject);
		}
		
		private static void HandleNewSpinnerControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
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
		
		private static void HandleNewDocumentOrEditControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter atkEditOrDoc = new TextBoxEntryView (provider);
			
			IncludeNewAdapter (atkEditOrDoc, parentObject);
		}

		private static void HandleNewHyperlinkControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
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

		private static void HandleNewImageControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter atkImage = new Image (provider);

			IncludeNewAdapter (atkImage, parentObject);
		}

		private static void HandleNewContainer (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter atkContainer = new Container (provider);
			providerAdapterMapping [provider] = atkContainer;
			
			IncludeNewAdapter (atkContainer, parentObject);
		}

		private static void HandleNewPane (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter atkItem = null;
			// Try to figure out whether this is a Splitter, a
			// SplitContainer, or something else
			if (provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id) != null) {
				System.Windows.Rect bounds = (System.Windows.Rect) 
					provider.GetPropertyValue (
					  AutomationElementIdentifiers.BoundingRectangleProperty.Id);
				// hack -- try to distinguish a Splitter from a SplitContainer
				if (parentObject is Window &&
					((bounds.Height < 10 && bounds.Width > 50) ||
					 (bounds.Width < 10 && bounds.Height > 50)))
					atkItem = new Splitter (provider);
				else
					atkItem = new SplitContainer (provider);
			}
			if (atkItem == null)
				atkItem = new Container (provider);
			providerAdapterMapping [provider] = atkItem;
			
			IncludeNewAdapter (atkItem, parentObject);
		}

		private static void HandleNewMenuBarControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			MenuBar newMenuBar = new MenuBar (provider);
			IncludeNewAdapter (newMenuBar, parentObject);
		}

		private static void HandleNewMenuControlType (IRawElementProviderSimple provider)
		{
			// Test for hidden menu that is the child of an expanded
			// MenuItem, in which case children should be processed.
			IRawElementProviderFragment providerFrag =
				provider as IRawElementProviderFragment;
			if (providerFrag != null) {
				var parentFrag =
					providerFrag.Navigate (NavigateDirection.Parent);
				if (parentFrag != null &&
				    ControlType.MenuItem.Id.Equals (parentFrag.GetPropertyValue (AEIds.ControlTypeProperty.Id))) {
					Atk.Object parentObject = null;
					if (!providerAdapterMapping.TryGetValue (parentFrag, out parentObject))
						return;
					ParentAdapter parentAdapter = parentObject as ParentAdapter;
					parentAdapter.RequestChildren ();
					return;
				}
			}

			// Handle context menu
			var fakeWindow = new Window ();
			TopLevelRootItem.Instance.AddOneChild (fakeWindow);
			IncludeNewAdapter (new ContextMenu (provider), fakeWindow);
		}

		private static void HandleNewMenuItemControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter newAdapter = null;
			ParentAdapter wrapperPanel = null;
			if (parentObject is Container && parentObject.Role == Atk.Role.ToolBar) {
				wrapperPanel = new Container (provider);
				parentObject.AddOneChild (wrapperPanel);
				parentObject = wrapperPanel;

				//toolbarbutton is keyboard-focusable while toolstripdropdownbutton is not
				bool isKeyboardFocusable = (bool) 
					provider.GetPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id);
				if (isKeyboardFocusable)
					newAdapter = new Button (provider);
			}
			if (newAdapter == null) {
				var child = ((IRawElementProviderFragment)provider).Navigate (NavigateDirection.FirstChild);
				newAdapter = (child == null) ? new MenuItem (provider) : new ParentMenu (provider);
			}
			IncludeNewAdapter (newAdapter, parentObject);
		}

		private static void HandleNewSplitButton (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			SplitButton splitButton = new SplitButton (provider);
			
			IncludeNewAdapter (splitButton, parentObject);
		}

		private static void HandleNewTab (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
			if (fragment == null) {
				Log.Warn ("UiaAtkBridge: Tab must be a fragment; ignoring");
				return;
			}
				
			Tab tab = new Tab (fragment);
			
			IncludeNewAdapter (tab, parentObject);
		}

		private static void HandleNewTabItem (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			TextContainer tabItem = new TextContainer (provider);
			
			IncludeNewAdapter (tabItem, parentObject);
		}

		private static void HandleNewTree (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
			if (fragment == null) {
				Log.Warn ("UiaAtkBridge: Tree must be a fragment; ignoring");
				return;
			}
				
			Tree tree = new Tree (fragment);
			IncludeNewAdapter (tree, parentObject);
		}

		private static void HandleNewTreeItem (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			TreeItem treeItem = new TreeItem (provider);
			
			IncludeNewAdapter (treeItem, parentObject);

			// TODO: Perhaps avoid doing this
			IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
			if (fragment == null)
				return;
			for (IRawElementProviderFragment child = fragment.Navigate (NavigateDirection.FirstChild); child != null; child = child.Navigate (NavigateDirection.NextSibling))
				HandleElementAddition (child);
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

			Log.Debug ("AutomationBridge: Creating new {0} adapter for {1}",
			           newAdapter.GetType (), newAdapter.Provider.GetType ());

			providerAdapterMapping [newAdapter.Provider] = newAdapter;
			parentAdapter.AddOneChild (newAdapter);
		}
		
		private static void HandleNewDataGridControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
			if (fragment == null)
				return;
			Adapter newAdapter = new DataGrid (fragment);

			IncludeNewAdapter (newAdapter, parentObject);
		}

		private static void HandleNewTableControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
			if (fragment == null)
				return;
			Adapter newAdapter = new Table (fragment);

			IncludeNewAdapter (newAdapter, parentObject);
		}

		private static void HandleNewSeparator (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			IncludeNewAdapter (new Separator (provider), parentObject);
		}
		
		internal static void AddChildrenToParent (IRawElementProviderSimple provider)
		{
			IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
			if (fragment == null)
				return;
			IRawElementProviderFragment child 
				= fragment.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				if (!providerAdapterMapping.ContainsKey (child))
					AutomationInteropProvider.RaiseStructureChangedEvent (child, 
					  new StructureChangedEventArgs (StructureChangeType.ChildAdded,
					                                 child.GetRuntimeId ()));
				child = child.Navigate (NavigateDirection.NextSibling);
			}
		}

		private static void HandleNewHeaderItemControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			HeaderItem headerItem = new HeaderItem (provider);

			IncludeNewAdapter (headerItem, parentObject);
		}
		
		private static void HandleNewSliderControlType (IRawElementProviderSimple provider, ParentAdapter parentObject)
		{
			Adapter atkSlider = new Slider (provider);

			IncludeNewAdapter (atkSlider, parentObject);
		}

		private static void HandleNewCalendarControlType (IRawElementProviderSimple provider,
		                                           ParentAdapter parentObject)
		{
			IncludeNewAdapter (new Container (provider), parentObject);
		}
		
		// This whole function is a hack to work around the
		// bridge not instantiating providers for controls which
		// existed prior to the provider being created.
		private void HandlePossiblePreExistingProvider (IRawElementProviderSimple provider)
		{
			IRawElementProviderFragment fragment = provider as IRawElementProviderFragment;
			if (fragment == null)
				return;
			IRawElementProviderFragment parent = fragment.Navigate (NavigateDirection.Parent);
			if (parent == null || parent == provider)
				return;
			int controlTypeId = (int) parent.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (controlTypeId == ControlType.DataItem.Id)
				parent = parent.Navigate (NavigateDirection.Parent);

			Atk.Object obj = null;
			if (!providerAdapterMapping.TryGetValue (parent, out obj))
				HandlePossiblePreExistingProvider (parent);
			if (!providerAdapterMapping.TryGetValue (parent, out obj))
				return;
			ParentAdapter parentAdapter = obj as ParentAdapter;
			if (parentAdapter != null) {
				if (parentAdapter.RefStateSet().ContainsState (Atk.StateType.ManagesDescendants))
					HandleElementAddition (provider);
				else
					// Otherwise try to keep things in
					// order, so request all children
					parentAdapter.RequestChildren ();
			}
		}

		internal static T PerformTransformation <T> (Adapter adapter, T newAdapter) where T : Adapter
		{
			if (adapter.Provider != newAdapter.Provider)
				throw new InvalidOperationException ("The provider of both adapters must be the same");
			if (adapter.NAccessibleChildren > 0)
				throw new NotImplementedException ("Transformation of adapters with children has not been implemented yet");
			
			ParentAdapter parent = (ParentAdapter)adapter.Parent;
			parent.RemoveChild (adapter);
			providerAdapterMapping [newAdapter.Provider] = newAdapter;
			parent.AddOneChild (newAdapter);
			return newAdapter;
		}
#endregion
	}
}
