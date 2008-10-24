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

using System.Collections.Generic;

using NUnit.Framework;

namespace UiaAtkBridgeTest
{
	
	public abstract class AtkTester {
		
		public abstract Atk.Object GetAccessible (
		  BasicWidgetType type, string name, bool real);
		
		public abstract Atk.Object GetAccessible (
		  BasicWidgetType type, string[] name, bool real);

		public abstract Atk.Object GetAccessibleThatEmbedsAnImage (
		  BasicWidgetType type, string name, bool real);

		public abstract I CastToAtkInterface <I> (Atk.Object accessible) where I : class;
		
		protected void InterfaceComponent (BasicWidgetType type, Atk.Component implementor)
		{
			Assert.AreEqual (1, implementor.Alpha, "Component.Alpha");

			if (type == BasicWidgetType.Window) {
				Assert.AreEqual (Atk.Layer.Window, implementor.Layer, "Component.Layer(Window)");
				Assert.AreEqual (-1, implementor.MdiZorder, "Component.MdiZorder(Window)");
			} else if (type == BasicWidgetType.ParentMenu) {
				Assert.AreEqual (Atk.Layer.Popup, implementor.Layer, "Component.Layer(Menu)");
			} else {
				Assert.AreEqual (Atk.Layer.Widget, implementor.Layer, "Component.Layer(notWindow)");
				//FIXME: still don't know why this is failing in the GailTester, accerciser is lying me?
				//Assert.AreEqual (0, implementor.MdiZorder, "Component.MdiZorder(notWindow)");
			}
		}
		
		protected abstract int ValidNumberOfActionsForAButton { get; }
		protected abstract int ValidNChildrenForASimpleStatusBar { get; }
		protected abstract int ValidNChildrenForAScrollBar { get; }
		
		protected void InterfaceActionFor3RadioButtons (Atk.Action actionable1, Atk.Object accessible1,
		                                              Atk.Action actionable2, Atk.Object accessible2,
		                                              Atk.Action actionable3, Atk.Object accessible3)
		{
			RunInGuiThread ( delegate () {
				Assert.IsTrue (actionable1.DoAction (0), "IAF3RB::DoAction#1");
			});
			System.Threading.Thread.Sleep (3000);
			RunInGuiThread ( delegate () {
				Assert.IsTrue (actionable3.DoAction (0), "IAF3RB::DoAction#1");
			});
			System.Threading.Thread.Sleep (3000);
			RunInGuiThread ( delegate () {
				Assert.IsTrue (accessible1.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #1");
				Assert.IsFalse (accessible2.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #2");
				Assert.IsTrue (accessible3.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #3");
			});
			
			EventMonitor.Start ();

			RunInGuiThread ( delegate () {
				Assert.IsTrue (actionable2.DoAction (0), "IAF3RB::DoAction#1");
			});
			System.Threading.Thread.Sleep (3000);
				
			// FIXME: should we detect "object:state-changed:focused" events here?? it seems gail doesn't fire them if we use this atk API
			EventCollection events = EventMonitor.Pause ();
			string evType = "object:state-changed:checked";
			EventCollection radioEvs = events.FindByRole (Atk.Role.RadioButton).FindByType (evType);
			string eventsInXml = String.Format (" events in XML: {0}", Environment.NewLine + events.OriginalGrossXml);
			Assert.AreEqual (2, radioEvs.Count, "bad number of events expected!" + eventsInXml);
			Assert.IsFalse (radioEvs [0].SourceName == radioEvs [1].SourceName, "events should come from different widgets;" + eventsInXml);
			Assert.AreEqual (1, radioEvs.FindWithDetail1 ("1").Count, "one should be Checked;" + eventsInXml);
			Assert.AreEqual (1, radioEvs.FindWithDetail1 ("0").Count, "one should be Checked;" + eventsInXml);

			RunInGuiThread ( delegate () {
				Assert.IsFalse (accessible1.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #4");
				Assert.IsTrue (accessible2.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #5");
				Assert.IsTrue (accessible3.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #6");

				Assert.IsTrue (actionable1.DoAction (0), "IAF3RB::DoAction#2");
			});
			
			System.Threading.Thread.Sleep (3000);
			
			RunInGuiThread ( delegate () {
				Assert.IsTrue (accessible1.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #7");
				Assert.IsFalse (accessible2.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #8");
				Assert.IsTrue (accessible3.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #9");
	
				Assert.IsTrue (actionable1.DoAction (0), "IAF3RB::DoAction#3");
			});

			System.Threading.Thread.Sleep (3000);
			
			RunInGuiThread ( delegate () {
				Assert.IsTrue (accessible1.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #10");
				Assert.IsFalse (accessible2.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #11");
				Assert.IsTrue (accessible3.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #12");
				
				Assert.IsTrue (actionable3.DoAction (0), "IAF3RB::DoAction#4");
			});
			
			System.Threading.Thread.Sleep (3000);
			
			RunInGuiThread ( delegate () {
				Assert.IsTrue (accessible1.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #13");
				Assert.IsFalse (accessible2.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #14");
				Assert.IsTrue (accessible3.RefStateSet ().ContainsState (Atk.StateType.Checked), "IAF3RB::Checked #15");
			});
		}
		
		protected void InterfaceAction (BasicWidgetType type, Atk.Action implementor, Atk.Object accessible)
		{
			int validNumberOfActions = ValidNumberOfActionsForAButton;
			if ((type == BasicWidgetType.TextBoxEntry) ||
			    (type == BasicWidgetType.ComboBoxDropDownList) || 
			    (type == BasicWidgetType.ListItem) || 
			    (type == BasicWidgetType.ParentMenu))
				validNumberOfActions = 1;
			else if (type == BasicWidgetType.CheckedListItem)
				validNumberOfActions = 2;
			
			Assert.AreEqual (validNumberOfActions, implementor.NActions, "NActions");
			
			if (type == BasicWidgetType.ComboBoxDropDownList)  {
				Assert.AreEqual ("press", implementor.GetName (0), "GetName press");
			} else if (type == BasicWidgetType.TextBoxEntry) {
				Assert.AreEqual ("activate", implementor.GetName (0), "GetName activate");
			} else { //Button and Checkbox and RadioButton, and ParentMenu
				Assert.AreEqual ("click", implementor.GetName (0), "GetName click");
				if ((ValidNumberOfActionsForAButton > 1) && (type != BasicWidgetType.ParentMenu)) {
					Assert.AreEqual ("press", implementor.GetName (1), "GetName press");
					Assert.AreEqual ("release", implementor.GetName (2), "GetName release");
				}
				else if (type == BasicWidgetType.CheckedListItem)
					Assert.AreEqual ("toggle", implementor.GetName (1), "GetName toggle");
			}
			
			bool actionPerformed = true;
			//this only applies if the CheckBox is not real (in Gail) :-?
//			if (type == BasicWidgetType.CheckBox)
//				actionPerformed = false;
			
			Atk.StateSet state = accessible.RefStateSet();
			Assert.IsFalse (state.IsEmpty, "RefStateSet.IsEmpty");
			Assert.IsTrue (state.ContainsState (Atk.StateType.Enabled), "RefStateSet.Enabled #1");
			//a radio button is checked by default
			if (type != BasicWidgetType.RadioButton)
				Assert.IsFalse (state.ContainsState (Atk.StateType.Checked), "RefStateSet.!Checked #1");
			

			if (type == BasicWidgetType.ListItem || type == BasicWidgetType.CheckedListItem)
				Assert.IsFalse (state.ContainsState (Atk.StateType.Selected), "RefStateSet.Selected");

			if (type != BasicWidgetType.ComboBoxDropDownList) {
				// only valid actions should work
				for (int i = 0; i < validNumberOfActions; i++) 
					Assert.AreEqual (actionPerformed, implementor.DoAction (i), "DoAction(" + i + ")");
				if ((validNumberOfActions > 1) // does not apply in UIA because 1 doaction==1click==checked
				                                         // (in GAIL click+press+release==2clicks==unchecked)
				     && type == BasicWidgetType.CheckBox)
					//one more, to leave it checked
					Assert.AreEqual (actionPerformed, implementor.DoAction (0), "DoAction_Corrective");
			}
			else
			{
				Assert.AreEqual (actionPerformed, implementor.DoAction (0), "DoAction Combo#1");
				Assert.AreEqual (1, implementor.NActions, "NActions doesn't change");
				Assert.AreEqual ("press", implementor.GetName (0), "Action[0] doesn't change");
				Assert.AreEqual (false, implementor.DoAction (0), "DoAction Combo#2");
			}
			// it takes a bit before the State is propagated!
			System.Threading.Thread.Sleep (4000);
			
			state = accessible.RefStateSet ();
			Assert.IsTrue (state.ContainsState (Atk.StateType.Enabled), "RefStateSet.Enabled #2");
			if ((type == BasicWidgetType.CheckBox) || (type == BasicWidgetType.RadioButton) || (type == BasicWidgetType.CheckedListItem))
				Assert.IsTrue (state.ContainsState (Atk.StateType.Checked), "RefStateSet.Checked");
			else
				Assert.IsFalse (state.ContainsState (Atk.StateType.Checked), "RefStateSet.!Checked #2");
			if ((type == BasicWidgetType.ListItem) || (type == BasicWidgetType.CheckedListItem))
				Assert.IsTrue (state.ContainsState (Atk.StateType.Selected), "RefStateSet.Selected");
			
			
			//still need to figure out why this is null in gail
//				Assert.IsNull (implementor.GetLocalizedName (0));
//				Assert.IsNull (implementor.GetLocalizedName (1));
//				Assert.IsNull (implementor.GetLocalizedName (2));
			
			for (int i = 0; i < ValidNumberOfActionsForAButton; i++) 
				Assert.IsNull (implementor.GetDescription (i), "GetDescription null");
			
			//out of range
			Assert.IsFalse (implementor.DoAction (-1), "DoAction OOR#1");
			Assert.IsFalse (implementor.DoAction (validNumberOfActions), "DoAction OOR#2");
			Assert.IsNull (implementor.GetName (-1), "GetName OOR#1");
			Assert.IsNull (implementor.GetName (validNumberOfActions), "GetName OOR#2");
			Assert.IsNull (implementor.GetDescription (-1), "GetDescription OOR#1");
			Assert.IsNull (implementor.GetDescription (validNumberOfActions), "GetDescription OOR#2");
			Assert.IsNull (implementor.GetLocalizedName (-1), "GetLocalizedName OOR#1");
			Assert.IsNull (implementor.GetLocalizedName (validNumberOfActions), "GetLocalizedName OOR#2");
			
			string descrip = "Some big ugly description";
			for (int i = 0; i < validNumberOfActions; i++) {
				Assert.IsTrue (implementor.SetDescription(i, descrip), "SetDescription");
				Assert.AreEqual (descrip, implementor.GetDescription (i), "GetDescription");
				descrip += ".";
			}
			Assert.IsFalse (implementor.SetDescription(validNumberOfActions, descrip), "SetDescription OOR");
			Assert.IsNull (implementor.GetDescription (validNumberOfActions), "GetDescription OOR#3");
			
			// With no keybinding set, everything should return null
			Assert.IsNull (implementor.GetKeybinding (0), "GetKeyBinding#1");
			Assert.IsNull (implementor.GetKeybinding (1), "GetKeyBinding#2");
			Assert.IsNull (implementor.GetKeybinding (2), "GetKeyBinding#3");
			
			//out of range items too
			Assert.IsNull (implementor.GetKeybinding (-1), "GetKeyBinding OOR#1");
			Assert.IsNull (implementor.GetKeybinding (3), "GetKeyBinding OOR#2");
		}
		
		protected void InterfaceSelection (Atk.Selection implementor, string[] names, Atk.Object accessible, BasicWidgetType type)
		{
			if (names == null)
				throw new ArgumentNullException ("names");
			
			if (names.Length < 2)
				throw new ArgumentException ("For testing purposes, use 2 or more items", "names");
			
			string accessibleName = null;
			if (type == BasicWidgetType.ParentMenu)
				accessibleName = names [0];
			else if (type == BasicWidgetType.ListBox || type == BasicWidgetType.CheckedListBox)
				// Not sure if this is right; setting so I can test other things -MPG
				accessibleName = String.Empty;
			
			Assert.AreEqual (accessibleName, accessible.Name, "AtkObj Name");
			if (type == BasicWidgetType.TabControl)
				Assert.IsFalse (implementor.ClearSelection(), "ClearSelection");
			else {
				Assert.IsTrue (implementor.ClearSelection(), "ClearSelection");
				for (int i = 0; i < names.Length; i++)
						Assert.IsFalse (implementor.IsChildSelected (i), "isChildSelected(" + i + ")");
				Assert.AreEqual (0, implementor.SelectionCount, "SelectionCount == 0");
			}
			
			for (int i = 0; i < names.Length; i++) {
				Assert.IsTrue (implementor.AddSelection (i), "AddSelection(" + i + ")");

				string accName = names [i];
				if (type == BasicWidgetType.ParentMenu)
					accName = names[0];
				else if (type == BasicWidgetType.ListBox || type == BasicWidgetType.CheckedListBox)
					accName = String.Empty;
				else if (type == BasicWidgetType.TabControl)
					accName = null;
				Assert.AreEqual (accName, accessible.Name, "AtkObj Name #" + i);
				
				Atk.Object refSelObj = implementor.RefSelection (0);
				if (type != BasicWidgetType.ParentMenu) {
					Assert.IsNotNull (refSelObj, "refSel should not be null");
					if (type != BasicWidgetType.ListBox && type != BasicWidgetType.CheckedListBox && type != BasicWidgetType.TabControl)
						Assert.AreEqual (accessible.Name, refSelObj.Name, "AtkObj NameRefSel#" + i);
					Assert.AreEqual (1, implementor.SelectionCount, "SelectionCount == 1");
					Assert.IsTrue (implementor.IsChildSelected (i), "childSelected(" + i + ")");
					Assert.IsTrue (refSelObj.RefStateSet ().ContainsState (Atk.StateType.Selectable), "Selected child should have State.Selectable");
					Assert.IsTrue (refSelObj.RefStateSet ().ContainsState (Atk.StateType.Selected), "Selected child(" + i + ") should have State.Selected");
				} else {
					//first child in a menu -> tearoff menuitem (can't be selected)
					if (i == 0) {
						Assert.IsNull (refSelObj, "refSel should be null, a menu cannot select a subitem[" + i + "]");
						Assert.AreEqual (0, implementor.SelectionCount, "SelectionCount == 0");
						Assert.IsFalse (implementor.IsChildSelected (i), "childSelected(" + i + ")");
					} else {
						Assert.IsNotNull (refSelObj, "refSel should not be null");
						Assert.AreEqual (names [i], refSelObj.Name, "AtkObj NameRefSel#" + i);
						Assert.AreEqual (1, implementor.SelectionCount, "SelectionCount == 1");
						Assert.IsTrue (implementor.IsChildSelected (i), "childSelected(" + i + ")");
					}
					
				}

				if (i == 1 && (type == BasicWidgetType.ListBox || type == BasicWidgetType.CheckedListBox))
					Assert.IsFalse (accessible.RefAccessibleChild(0).RefStateSet().ContainsState (Atk.StateType.Selected), "Unselected child should not have State.Selected");
				int refSelPos = i;
				if (refSelPos == 0)
					refSelPos = -1;
				Assert.IsNull (implementor.RefSelection (refSelPos), "RefSelection OOR#-" + i);
			}

			Assert.IsNotNull (implementor.RefSelection (0), "RefSel!=null");

			string lastName = accessible.Name;

			if (type == BasicWidgetType.ComboBoxDropDownList || 
			    type == BasicWidgetType.ListBox || 
			    type == BasicWidgetType.CheckedListBox ||
			    type == BasicWidgetType.TabControl) {
				//strangely, OOR selections return true (valid)
				Assert.IsTrue (implementor.AddSelection (-1), "AddSelection OOR#1");
			} else {
				Assert.IsFalse (implementor.AddSelection (-1), "AddSelection OOR#1");
			}
			//strangely, OOR upper selection returns true for menu and combobox
			Assert.IsTrue (implementor.AddSelection (names.Length), "AddSelection OOR#2");
			
			Assert.AreEqual (lastName, accessible.Name, "OOR selections shouldn't affect name");
			
			Assert.IsNull (implementor.RefSelection (-1), "RefSelection OOR#1");
			Assert.IsNull (implementor.RefSelection (names.Length), "RefSelection OOR#2");
			
			if (type != BasicWidgetType.TabControl) {
				Assert.IsTrue (implementor.ClearSelection (), "ClearSelection");
			Assert.IsNull (implementor.RefSelection (0), "RefSel after CS");
			}

			//this is a normal combobox (not multiline) (TODO: research multiline comboboxes?)
			Assert.IsFalse (implementor.SelectAllSelection ());
			
			if (type != BasicWidgetType.ListBox && type != BasicWidgetType.CheckedListBox && type != BasicWidgetType.TabControl)
				Assert.IsNull (implementor.RefSelection (0), "RefSel after SAS");
			
			Assert.IsTrue (names.Length > 0, "Please use a names variable that is not empty");
			Assert.IsTrue (implementor.AddSelection (0), "AddSelection->0");
			if (type != BasicWidgetType.ParentMenu) {
				Assert.IsNotNull (implementor.RefSelection (0), "RefSel!=null after AS0");

				if (type != BasicWidgetType.ListBox && type != BasicWidgetType.CheckedListBox && type != BasicWidgetType.TabControl) {
					Assert.IsTrue (implementor.RemoveSelection (names.Length), "RemoveSelection OOR#>n");
					Assert.IsTrue (implementor.RemoveSelection (-1), "RemoveSelection OOR#<0");
				}
			}
			else {
				Assert.IsNull (implementor.RefSelection (0), "RefSel==null after AS0");

				Assert.IsFalse (implementor.RemoveSelection (names.Length), "RemoveSelection OOR#>n");
				Assert.IsFalse (implementor.RemoveSelection (-1), "RemoveSelection OOR#<0");
			}

			if (type != BasicWidgetType.ListBox && type != BasicWidgetType.CheckedListBox && type != BasicWidgetType.TabControl) {
				Assert.IsTrue (implementor.RemoveSelection (0), "RemoveSelection");
				Assert.IsNull (implementor.RefSelection (0), "RefSel after RemoveSel");
			}
		}
		
		private void TextMatchesValue (BasicWidgetType type, Atk.Value atkValue, Atk.Text atkText)
		{
			if (atkText == null)
				return;
			string text = GetCurrentValue (atkValue).ToString ("F2");
			Assert.AreEqual (text, atkText.GetText (0, -1), "GetText");
			Assert.AreEqual (text.Length, atkText.CharacterCount, "CharacterCount");
			Assert.AreEqual (text [0], atkText.GetCharacterAtOffset (0), "GetCharacterAtOffset");
		}

		protected void InterfaceValue (BasicWidgetType type, Atk.Value atkValue, Atk.Text atkText)
		{
			Assert.IsNotNull (atkValue, "InterfaceValue value not null");
			Assert.AreEqual (  0, GetMinimumValue(atkValue), "InterfaceValue MinimumValue");
			if (type == BasicWidgetType.HScrollBar || type == BasicWidgetType.VScrollBar)
				Assert.IsTrue (GetMaximumValue(atkValue) > 0, "InterfaceValue MaximumValue > 0");
			else
				Assert.AreEqual (100, GetMaximumValue(atkValue), "InterfaceValue MaximumValue");
			if (type == BasicWidgetType.Spinner)
				Assert.AreEqual (50, GetCurrentValue(atkValue), "InterfaceValue CurrentValue #1");
			TextMatchesValue (type, atkValue, atkText);
		}

		protected void InterfaceValue (BasicWidgetType type, Atk.Value atkValue)
		{
			InterfaceValue (type, atkValue, null);
		}

		protected void InterfaceImage (BasicWidgetType type, Atk.Image implementor, Atk.Component component, 
		                               Atk.Image withoutImageImplementor)
		{
			Assert.IsNull (implementor.ImageDescription, "ImageDescription == null initially");
			Assert.IsNull (withoutImageImplementor.ImageDescription, "wii.ImageDescription == null initially");
			string myDesc = "Hola mundo";
			Assert.IsTrue (implementor.SetImageDescription (myDesc), "Setting the img desc should return success");
			if (type != BasicWidgetType.PictureBox)
				Assert.IsFalse (withoutImageImplementor.SetImageDescription (myDesc), "Setting the img desc should return false");
			Assert.AreEqual (myDesc, implementor.ImageDescription, "The img desc should have been overriden correctly");
			if (type != BasicWidgetType.PictureBox)
				Assert.IsNull (withoutImageImplementor.ImageDescription, "wii.ImageDescription == null always");
			int ia, ib, ca, cb;
			implementor.GetImagePosition (out ia, out ib, Atk.CoordType.Screen);
			component.GetPosition (out ca, out cb, Atk.CoordType.Screen);
			Assert.IsTrue (ia > 0, "x of the image must be > 0; obtained " + ia);
			Assert.IsTrue (ib > 0, "y of the image must be > 0; obtained " + ib);
			Assert.IsTrue (ia >= ca, "x of the image must be >= x from the widget; obtained " + ia + "<" + ca);
			Assert.IsTrue (ib >= cb, "y of the image must be >= y from the widget; obtained " + ia + "<" + cb);

			withoutImageImplementor.GetImagePosition (out ia, out ib, Atk.CoordType.Screen);
			Assert.AreEqual (int.MinValue, ia, "x of the image must be int.MinValue; obtained " + ia);
			Assert.AreEqual (int.MinValue, ib, "y of the image must be int.MinValue; obtained " + ib);
			
			implementor.GetImageSize (out ia, out ib);
			component.GetSize (out ca, out cb);
			Assert.IsTrue (ia > 0, "width of the image must be > 0; obtained " + ia);
			Assert.IsTrue (ib > 0, "height of the image must be > 0; obtained " + ib);
			Assert.IsTrue (ia <= ca, "width of the image must be <= width from the widget; obtained " + ia + ">" + ca);
			Assert.IsTrue (ib <= cb, "height of the image must be <= height from the widget; obtained " + ib + ">" + cb);

			withoutImageImplementor.GetImageSize (out ia, out ib);
			Assert.AreEqual (-1, ia, "width of the image must be int.MinValue; obtained " + ia);
			Assert.AreEqual (-1, ib, "height of the image must be int.MinValue; obtained " + ib);
		}

		protected void StatesComboBox (Atk.Object accessible)
		{
			States (accessible,
			  Atk.StateType.Enabled,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);
		}

		protected string childrenRoles (Atk.Object accessible)
		{
			string res = String.Empty;
			for (int i = 0; i < accessible.NAccessibleChildren; i++)
				res += accessible.RefAccessibleChild (i).Role.ToString () + ",";
			if (res == String.Empty)
				return "<no children>";
			return res;
		}
		
		protected void PropertyRole (BasicWidgetType type, Atk.Object accessible)
		{
			Atk.Role role = Atk.Role.Unknown;
			switch (type) {
			case BasicWidgetType.Label:
				role = Atk.Role.Label;
				break;
			case BasicWidgetType.NormalButton:
				role = Atk.Role.PushButton;
				break;
			case BasicWidgetType.Window:
				role = Atk.Role.Frame;
				break;
			case BasicWidgetType.CheckBox:
				role = Atk.Role.CheckBox;
				break;
			case BasicWidgetType.ComboBoxDropDownEntry:
			case BasicWidgetType.ComboBoxDropDownList:
			case BasicWidgetType.ComboBoxSimple:
				role = Atk.Role.ComboBox;
				break;
			case BasicWidgetType.RadioButton:
				role = Atk.Role.RadioButton;
				break;
			case BasicWidgetType.TextBoxEntry:
				role = Atk.Role.Text;
				break;
			case BasicWidgetType.StatusBar:
				role = Atk.Role.Statusbar;
				break;
			case BasicWidgetType.ParentMenu:
				role = Atk.Role.Menu;
				break;
			case BasicWidgetType.HScrollBar:
			case BasicWidgetType.VScrollBar:
				role = Atk.Role.ScrollBar;
				break;
			case BasicWidgetType.ProgressBar:
				role = Atk.Role.ProgressBar;
				break;
			case BasicWidgetType.ListBox:
			case BasicWidgetType.CheckedListBox:
				role = Atk.Role.List;
				break;
			case BasicWidgetType.Spinner:
				role = Atk.Role.SpinButton;
				break;
			case BasicWidgetType.TabControl:
				role = Atk.Role.PageTabList;
				break;
			case BasicWidgetType.TabPage:
				role = Atk.Role.PageTab;
				break;
			case BasicWidgetType.GroupBox:
				role = Atk.Role.Panel;
				break;
			case BasicWidgetType.ListView:
				role = Atk.Role.TreeTable;
				break;
			case BasicWidgetType.PictureBox:
				role = Atk.Role.Icon;
				break;
			default:
				throw new NotImplementedException ();
			}
			Assert.AreEqual (role, accessible.Role, "Atk.Role");
		}

		protected Atk.Object InterfaceText (BasicWidgetType type)
		{
			return InterfaceTextAux (type, false, null);
		}

		protected Atk.Object InterfaceText (BasicWidgetType type, bool onlySingleLine)
		{
			return InterfaceTextAux (type, onlySingleLine, null);
		}

		protected void InterfaceText (BasicWidgetType type, Atk.Object accessible)
		{
			InterfaceTextAux (type, false, accessible);
		}
		
		protected void InterfaceText (BasicWidgetType type, bool onlySingleLine, Atk.Object accessible)
		{
			InterfaceTextAux (type, onlySingleLine, accessible);
		}
		
		private Atk.Object InterfaceTextAux (BasicWidgetType type, bool onlySingleLine, Atk.Object accessible)
		{
			int startOffset, endOffset;
			string expected;
			string name = simpleTestText;

			Atk.Text atkText = null;

			RunInGuiThread (delegate () {
				if (accessible == null)
					accessible = GetAccessible (type, name, true);
				atkText = CastToAtkInterface <Atk.Text> (accessible);
			});
			
			int caret = 0;
			if (type == BasicWidgetType.TextBoxView)
				caret = name.Length;
			
			Assert.AreEqual (caret, atkText.CaretOffset, "CaretOffset SL");
			Assert.AreEqual (name.Length, atkText.CharacterCount, "CharacterCount SL");
			Assert.AreEqual (name [0], atkText.GetCharacterAtOffset (0), "GetCharacterAtOffset SL");
			Assert.AreEqual (name, atkText.GetText (0, name.Length), "GetText SL");

			int highCaretOffset = 15;
			//any value (beware, this may change when this is fixed: http://bugzilla.gnome.org/show_bug.cgi?id=556453 )
			Assert.AreEqual (!Misc.HasReadOnlyText (type), atkText.SetCaretOffset (-1), "SetCaretOffset#1 SL");
			Assert.AreEqual (!Misc.HasReadOnlyText (type), atkText.SetCaretOffset (0), "SetCaretOffset#2 SL");
			Assert.AreEqual (!Misc.HasReadOnlyText (type), atkText.SetCaretOffset (1), "SetCaretOffset#3 SL");
			Assert.AreEqual (!Misc.HasReadOnlyText (type), atkText.SetCaretOffset (highCaretOffset), "SetCaretOffset#4 SL");
			
			// don't do this until bug#393565 is fixed:
			//Assert.AreEqual (typeof(Atk.TextAttribute), atkText.DefaultAttributes[0].GetType());

			int nSelections = -1;
			if ((type == BasicWidgetType.Label) || 
			    (type == BasicWidgetType.TextBoxEntry) ||
				(type == BasicWidgetType.TextBoxView))
				nSelections = 0;
			
			Assert.AreEqual (nSelections, atkText.NSelections, "NSelections#1 SL");

			int caretOffset = 0;
			if (!Misc.HasReadOnlyText (type))
				caretOffset = highCaretOffset;
			
			// you cannot select a label AFAIK so, all zeroes returned!
			Assert.AreEqual (null, atkText.GetSelection (0, out startOffset, out endOffset), "GetSelection#1 SL");
			Assert.AreEqual (caretOffset, startOffset, "GetSelection#2 SL");
			Assert.AreEqual (caretOffset, endOffset, "GetSelection#3 SL");
			Assert.AreEqual (null, atkText.GetSelection (1, out startOffset, out endOffset), "GetSelection#4 SL");
			Assert.AreEqual (caretOffset, startOffset, "GetSelection#5 SL");
			Assert.AreEqual (caretOffset, endOffset, "GetSelection#6 SL");
			//yes, it's wierd that we get valid offsets when calling with an OOR int, blame gail! I'm lazy to file a bug
			Assert.AreEqual (null, atkText.GetSelection (-1, out startOffset, out endOffset), "GetSelection#7 SL");
			Assert.AreEqual (caretOffset, startOffset, "GetSelection#8 SL");
			Assert.AreEqual (caretOffset, endOffset, "GetSelection#9 SL");
			
			// you cannot select a label AFAIK so, false always returned!
			Assert.AreEqual (false, atkText.SetSelection (0, 1, 2), "SetSelection#1 SL");
			// test GetSelection *after* SetSelection
			Assert.AreEqual (null, atkText.GetSelection (0, out startOffset, out endOffset), "GetSelection#10 SL");
			Assert.AreEqual (caretOffset, startOffset, "GetSelection#11 SL");
			Assert.AreEqual (caretOffset, endOffset, "GetSelection#12 SL");
			//test crazy numbers for SetSelection
			Assert.AreEqual (false, atkText.SetSelection (-3, 10, -2), "SetSelection#2 SL");
			Assert.AreEqual (null, atkText.GetSelection (0, out startOffset, out endOffset), "GetSelection#13 SL");
			Assert.AreEqual (caretOffset, startOffset, "GetSelection#14 SL");
			Assert.AreEqual (caretOffset, endOffset, "GetSelection#15 SL");
			
			//did NSelections changed?
			Assert.AreEqual (false, atkText.SetSelection (1, 2, 3), "SetSelection#3 SL");
			Assert.AreEqual (nSelections, atkText.NSelections, "NSelections#2 SL");
			Assert.AreEqual (false, atkText.RemoveSelection (0), "RemoveSelection#1 SL");
			Assert.AreEqual (nSelections, atkText.NSelections, "NSelections#3 SL");
			Assert.AreEqual (false, atkText.RemoveSelection (1), "RemoveSelection#2 SL");
			Assert.AreEqual (nSelections, atkText.NSelections, "NSelections#4 SL");
			Assert.AreEqual (false, atkText.RemoveSelection (-1), "RemoveSelection#3 SL");
			Assert.AreEqual (nSelections, atkText.NSelections, "NSelections#5 SL");


			//IMPORTANT NOTE about GetText*Offset methods [GetTextAtOffset(),GetTextAfterOffset(),GetTextBeforeOffset()]:
			//in Gail, they all return null if GetText() has not been called yet, however we may
			//prefer not to follow this wierd behaviour in the bridge
			
			//GetTextAtOffset
			expected = " test";
			Assert.AreEqual (expected, 
				atkText.GetTextAtOffset (12, Atk.TextBoundary.WordEnd, out startOffset, out endOffset),
				"GetTextAtOffset,WordEnd SL");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,WordEnd,so SL");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,WordEnd,eo SL");

			int startCaretOffset = highCaretOffset;
			int endCaretOffset = highCaretOffset;
			if (Misc.HasReadOnlyText (type)) {
				startCaretOffset = name.IndexOf (expected);
				endCaretOffset = name.IndexOf (expected) + expected.Length;
			}
			
			//test selections after obtaining text with a different API than GetText
			Assert.AreEqual (nSelections, atkText.NSelections, "NSelections#6 SL");
			//NSelections == 0, however we have one selection, WTF?:
			Assert.AreEqual (null, atkText.GetSelection (0, out startOffset, out endOffset), "GetSelection#16 SL");
			
			Assert.AreEqual (startCaretOffset, startOffset, "GetSelection#17 SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetSelection#18.. SL");
			Assert.AreEqual (null, atkText.GetSelection (1, out startOffset, out endOffset), "GetSelection#19 SL");
			Assert.AreEqual (startCaretOffset, startOffset, "GetSelection#20 SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetSelection#21 SL");
			Assert.AreEqual (null, atkText.GetSelection (30, out startOffset, out endOffset), "GetSelection#22 SL");
			Assert.AreEqual (startCaretOffset, startOffset, "GetSelection#23 SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetSelection#24 SL");
			Assert.AreEqual (null, atkText.GetSelection (-1, out startOffset, out endOffset), "GetSelection#25 SL");
			Assert.AreEqual (startCaretOffset, startOffset, "GetSelection#26 SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetSelection#27 SL");
			
			Assert.AreEqual (false, atkText.SetSelection (0, 0, 0), "SetSelection#3 SL");
			Assert.AreEqual (null, atkText.GetSelection (0, out startOffset, out endOffset), "GetSelection#28 SL");
			Assert.AreEqual (startCaretOffset, startCaretOffset, "GetSelection#29 SL");
			Assert.AreEqual (endCaretOffset, endOffset, "GetSelection#30 SL");
			
			
			expected = "test ";
			Assert.AreEqual (expected, 
				atkText.GetTextAtOffset (12, Atk.TextBoundary.WordStart, out startOffset, out endOffset),
				"GetTextAtOffset,WordStart SL");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,WordStart,so SL");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,WordStart,eo SL");
			
			expected = simpleTestText;
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (12, Atk.TextBoundary.LineEnd, out startOffset, out endOffset),
				"GetTextAtOffset,LineEnd SL");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,LineEnd,so SL");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,LineEnd,eo SL");

			expected = simpleTestText;
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (12, Atk.TextBoundary.LineStart, out startOffset, out endOffset),
				"GetTextAtOffset,LineStart SL");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,LineStart,so SL");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,LineStart,eo SL");
			
			expected = simpleTestText;
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (18, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextAtOffset,SentenceEnd SL");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,SentenceEnd,so SL");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,SentenceEnd,eo SL");
			
			expected = simpleTestText;
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (18, Atk.TextBoundary.SentenceStart, out startOffset, out endOffset),
				"GetTextAtOffset,SentenceStart SL");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,SentenceStart,so SL");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,SentenceStart,eo SL");
			
			Assert.AreEqual ("t",
				atkText.GetTextAtOffset (18, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char1 SL");
			Assert.AreEqual (18, startOffset, "GetTextAtOffset,Char1,so SL");
			Assert.AreEqual (19, endOffset, "GetTextAtOffset,Char1,eo SL");
			Assert.AreEqual (".",
				atkText.GetTextAtOffset (23, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char2 SL");
			Assert.AreEqual (23, startOffset, "GetTextAtOffset,Char2,so SL");
			Assert.AreEqual (24, endOffset, "GetTextAtOffset,Char2,eo SL");
			Assert.AreEqual ("e",
				atkText.GetTextAtOffset (name.Length - 2, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char3 SL");
			Assert.AreEqual (name.Length - 2, startOffset, "GetTextAtOffset,Char3,so SL");
			Assert.AreEqual (name.Length - 1, endOffset, "GetTextAtOffset,Char3,eo SL");
			Assert.AreEqual (".",
				atkText.GetTextAtOffset (name.Length - 1, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char4 SL");
			Assert.AreEqual (name.Length - 1, startOffset, "GetTextAtOffset,Char4,so SL");
			Assert.AreEqual (name.Length, endOffset, "GetTextAtOffset,Char4,eo SL");
			Assert.AreEqual (String.Empty,
				atkText.GetTextAtOffset (name.Length, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char5 SL");
			Assert.AreEqual (name.Length, startOffset, "GetTextAtOffset,Char5,so SL");
			Assert.AreEqual (name.Length, endOffset, "GetTextAtOffset,Char5,eo SL");
			Assert.AreEqual (String.Empty,
				atkText.GetTextAtOffset (-1, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char6 SL");
			Assert.AreEqual (name.Length, startOffset, "GetTextAtOffset,Char6,so SL");
			Assert.AreEqual (name.Length, endOffset, "GetTextAtOffset,Char6,eo SL");


			//GetTextAfterOffset
			expected = " sentence";
			Assert.AreEqual (expected, 
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.WordEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,WordEnd SL");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,WordEnd,so SL");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,WordEnd,eo SL");
			
			expected = "sentence.";
			Assert.AreEqual (expected, 
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.WordStart, out startOffset, out endOffset),
				"GetTextAfterOffset,WordStart SL");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,WordStart,so SL");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,WordStart,eo SL");

			if (onlySingleLine)
				return accessible;
			
			name = "This is a test sentence.\r\nSecond line. Other phrase.\nThird line?";

			RunInGuiThread (delegate () {
				accessible = GetAccessible (type, name, true);
				atkText = CastToAtkInterface <Atk.Text> (accessible);
			});

			System.Threading.Thread.Sleep (2000);

			Assert.AreEqual (name.Length, atkText.CharacterCount, "CharacterCount");
			Assert.AreEqual (name [0], atkText.GetCharacterAtOffset (0), "GetCharacterAtOffset");
			Assert.AreEqual (name, atkText.GetText (0, name.Length), "GetText");

			//IMPORTANT NOTE about GetText*Offset methods [GetTextAtOffset(),GetTextAfterOffset(),GetTextBeforeOffset()]:
			//in Gail, they all return null if GetText() has not been called yet, however we may
			//prefer not to follow this wierd behaviour in the bridge
			
			//GetTextAtOffset
			expected = " test";
			Assert.AreEqual (expected, 
				atkText.GetTextAtOffset (12, Atk.TextBoundary.WordEnd, out startOffset, out endOffset),
				"GetTextAtOffset,WordEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,WordEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,WordEnd,eo");
			
			int indexStartOffset = name.IndexOf (expected);
			int indexEndOffset = name.IndexOf (expected) + expected.Length;

			if (!Misc.HasReadOnlyText (type)) {
				indexStartOffset = name.Length;
				indexEndOffset = name.Length;
			}
			
			//test selections after obtaining text with a different API than GetText
			Assert.AreEqual (nSelections, atkText.NSelections, "NSelections#6");
			//NSelections == 0, however we have one selection, WTF?:
			Assert.AreEqual (null, atkText.GetSelection (0, out startOffset, out endOffset), "GetSelection#16");
			Assert.AreEqual (indexStartOffset, startOffset, "GetSelection#17");
			Assert.AreEqual (indexEndOffset, endOffset, "GetSelection#18");
			Assert.AreEqual (null, atkText.GetSelection (1, out startOffset, out endOffset), "GetSelection#19");
			Assert.AreEqual (indexStartOffset, startOffset, "GetSelection#20");
			Assert.AreEqual (indexEndOffset, endOffset, "GetSelection#21");
			Assert.AreEqual (null, atkText.GetSelection (30, out startOffset, out endOffset), "GetSelection#22");
			Assert.AreEqual (indexStartOffset, startOffset, "GetSelection#23");
			Assert.AreEqual (indexEndOffset, endOffset, "GetSelection#24");
			Assert.AreEqual (null, atkText.GetSelection (-1, out startOffset, out endOffset), "GetSelection#25");
			Assert.AreEqual (indexStartOffset, startOffset, "GetSelection#26");
			Assert.AreEqual (indexEndOffset, endOffset, "GetSelection#27");
			
			Assert.AreEqual (false, atkText.SetSelection (0, 0, 0), "SetSelection#3");
			Assert.AreEqual (null, atkText.GetSelection (0, out startOffset, out endOffset), "GetSelection#28");
			Assert.AreEqual (indexStartOffset, startOffset, "GetSelection#29");
			Assert.AreEqual (indexEndOffset, endOffset, "GetSelection#30");
			
			
			expected = "test ";
			Assert.AreEqual (expected, 
				atkText.GetTextAtOffset (12, Atk.TextBoundary.WordStart, out startOffset, out endOffset),
				"GetTextAtOffset,WordStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,WordStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,WordStart,eo");
			

			expected = "This is a test sentence.";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (12, Atk.TextBoundary.LineEnd, out startOffset, out endOffset),
				"GetTextAtOffset,LineEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,LineEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,LineEnd,eo");

			expected = "This is a test sentence.\r\n";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (12, Atk.TextBoundary.LineStart, out startOffset, out endOffset),
				"GetTextAtOffset,LineStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,LineStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,LineStart,eo");
			
			expected = "This is a test sentence.";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (18, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextAtOffset,SentenceEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,SentenceEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,SentenceEnd,eo");
			
			expected = "This is a test sentence.\r\n";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (18, Atk.TextBoundary.SentenceStart, out startOffset, out endOffset),
				"GetTextAtOffset,SentenceStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,SentenceStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,SentenceStart,eo");
			
			Assert.AreEqual ("t",
				atkText.GetTextAtOffset (18, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char1");
			Assert.AreEqual (18, startOffset, "GetTextAtOffset,Char1,so");
			Assert.AreEqual (19, endOffset, "GetTextAtOffset,Char1,eo");
			Assert.AreEqual (".",
				atkText.GetTextAtOffset (23, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char2");
			Assert.AreEqual (23, startOffset, "GetTextAtOffset,Char2,so");
			Assert.AreEqual (24, endOffset, "GetTextAtOffset,Char2,eo");
			Assert.AreEqual ("e",
				atkText.GetTextAtOffset (name.Length - 2, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char3");
			Assert.AreEqual (name.Length - 2, startOffset, "GetTextAtOffset,Char3,so");
			Assert.AreEqual (name.Length - 1, endOffset, "GetTextAtOffset,Char3,eo");
			Assert.AreEqual ("?",
				atkText.GetTextAtOffset (name.Length - 1, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char4");
			Assert.AreEqual (name.Length - 1, startOffset, "GetTextAtOffset,Char4,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextAtOffset,Char4,eo");
			Assert.AreEqual (String.Empty,
				atkText.GetTextAtOffset (name.Length, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char5");
			Assert.AreEqual (name.Length, startOffset, "GetTextAtOffset,Char5,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextAtOffset,Char5,eo");
			Assert.AreEqual (String.Empty,
				atkText.GetTextAtOffset (-1, Atk.TextBoundary.Char, out startOffset, out endOffset), "GetTextAtOffset,Char6");
			Assert.AreEqual (name.Length, startOffset, "GetTextAtOffset,Char6,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextAtOffset,Char6,eo");

			//GetTextAfterOffset
			expected = " sentence";
			Assert.AreEqual (expected, 
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.WordEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,WordEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,WordEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,WordEnd,eo");
			
			expected = "sentence.\r\n";
			Assert.AreEqual (expected, 
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.WordStart, out startOffset, out endOffset),
				"GetTextAfterOffset,WordStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,WordStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,WordStart,eo");
			
			expected = "\r\nSecond line. Other phrase.";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.LineEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,LineEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,LineEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,LineEnd,eo");

			expected = "Second line. Other phrase.\n";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (12, Atk.TextBoundary.LineStart, out startOffset, out endOffset),
				"GetTextAfterOffset,LineStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,LineStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,LineStart,eo");
			
			expected = "\r\nSecond line.";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (18, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,SentenceEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,SentenceEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,SentenceEnd,eo");

			expected = " Other phrase.";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (24, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,SentenceEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,SentenceEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,SentenceEnd,eo");
			
			expected = "Second line. ";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (18, Atk.TextBoundary.SentenceStart, out startOffset, out endOffset),
				"GetTextAfterOffset,SentenceStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,SentenceStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,SentenceStart,eo");
			
			Assert.AreEqual ("e",
				atkText.GetTextAfterOffset (18, Atk.TextBoundary.Char, out startOffset, out endOffset),
				"GetTextAfterOffset,Char");
			Assert.AreEqual (19, startOffset, "GetTextAfterOffset,Char,so");
			Assert.AreEqual (20, endOffset, "GetTextAfterOffset,Char,eo");
			Assert.AreEqual ("\r",
				atkText.GetTextAfterOffset (23, Atk.TextBoundary.Char, out startOffset, out endOffset),
				"GetTextAfterOffset,Char");
			Assert.AreEqual (24, startOffset, "GetTextAfterOffset,Char,so");
			Assert.AreEqual (25, endOffset, "GetTextAfterOffset,Char,eo");
			Assert.AreEqual ("?",
				atkText.GetTextAfterOffset (name.Length - 2, Atk.TextBoundary.Char, out startOffset, out endOffset),
				"GetTextAfterOffset,Char");
			Assert.AreEqual (name.Length - 1, startOffset, "GetTextAfterOffset,Char,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextAfterOffset,Char,eo");
			Assert.AreEqual (String.Empty,
				atkText.GetTextAfterOffset (name.Length - 1, Atk.TextBoundary.Char, out startOffset, out endOffset),
				"GetTextAfterOffset,Char");
			Assert.AreEqual (name.Length, startOffset, "GetTextAfterOffset,Char,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextAfterOffset,Char,eo");
			Assert.AreEqual (String.Empty,
				atkText.GetTextAfterOffset (name.Length, Atk.TextBoundary.Char, out startOffset, out endOffset),
				"GetTextAfterOffset,Char");
			Assert.AreEqual (name.Length, startOffset, "GetTextAfterOffset,Char,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextAfterOffset,Char,eo");
			Assert.AreEqual (null,
				atkText.GetTextAfterOffset (-1, Atk.TextBoundary.Char, out startOffset, out endOffset),
				"GetTextAfterOffset,Char");
			Assert.AreEqual (name.Length, startOffset, "GetTextAfterOffset,Char,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextAfterOffset,Char,eo");
			
			
			//GetTextBeforeOffset
			expected = " a";
			Assert.AreEqual (expected, 
				atkText.GetTextBeforeOffset (12, Atk.TextBoundary.WordEnd, out startOffset, out endOffset),
				"GetTextBeforeOffset,WordEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextBeforeOffset,WordEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,WordEnd,eo");
			
			expected = "a ";
			Assert.AreEqual (expected, 
				atkText.GetTextBeforeOffset (12, Atk.TextBoundary.WordStart, out startOffset, out endOffset),
				"GetTextBeforeOffset,WordStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextBeforeOffset,WordStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,WordStart,eo");
			
			expected = String.Empty;
			
			Assert.AreEqual (expected,
				atkText.GetTextBeforeOffset (12, Atk.TextBoundary.LineEnd, out startOffset, out endOffset),
				"GetTextBeforeOffset,LineEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextBeforeOffset,LineEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,LineEnd,eo");

			Assert.AreEqual (expected,
				atkText.GetTextBeforeOffset (12, Atk.TextBoundary.LineStart, out startOffset, out endOffset),
				"GetTextBeforeOffset,LineStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextBeforeOffset,LineStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,LineStart,eo");
			
			Assert.AreEqual (expected,
				atkText.GetTextBeforeOffset (18, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextBeforeOffset,SentenceEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextBeforeOffset,SentenceEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,SentenceEnd,eo");
			
			Assert.AreEqual (expected,
				atkText.GetTextBeforeOffset (18, Atk.TextBoundary.SentenceStart, out startOffset, out endOffset),
				"GetTextBeforeOffset,SentenceStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextBeforeOffset,SentenceStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextBeforeOffset,SentenceStart,eo");
			
			Assert.AreEqual ("n",
				atkText.GetTextBeforeOffset (18, Atk.TextBoundary.Char, out startOffset, out endOffset),
				"GetTextBeforeOffset,Char");
			Assert.AreEqual (17, startOffset, "GetTextBeforeOffset,Char,so");
			Assert.AreEqual (18, endOffset, "GetTextBeforeOffset,Char,eo");
			Assert.AreEqual ("e",
				atkText.GetTextBeforeOffset (23, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (22, startOffset, "GetTextBeforeOffset,Char,so");
			Assert.AreEqual (23, endOffset, "GetTextBeforeOffset,Char,eo");
			Assert.AreEqual ("n",
				atkText.GetTextBeforeOffset (name.Length - 2, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (name.Length - 3, startOffset, "GetTextBeforeOffset,Char,so");
			Assert.AreEqual (name.Length - 2, endOffset, "GetTextBeforeOffset,Char,eo");
			Assert.AreEqual ("e",
				atkText.GetTextBeforeOffset (name.Length - 1, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (name.Length - 2, startOffset, "GetTextBeforeOffset,Char,so");
			Assert.AreEqual (name.Length - 1, endOffset, "GetTextBeforeOffset,Char,eo");
			Assert.AreEqual ("?",
				atkText.GetTextBeforeOffset (name.Length, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (name.Length - 1, startOffset, "GetTextBeforeOffset,Char,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextBeforeOffset,Char,eo");
			Assert.AreEqual (null,
				atkText.GetTextBeforeOffset (-1, Atk.TextBoundary.Char, out startOffset, out endOffset));
			Assert.AreEqual (name.Length - 1, startOffset, "GetTextBeforeOffset,Char,so");
			Assert.AreEqual (name.Length, endOffset, "GetTextBeforeOffset,Char,eo");
			
			
			
			
			name = "Tell me; here a sentence\r\nwith EOL but without dot, and other phrase... Heh!";

			accessible = GetAccessible (type, name, true);
			atkText = CastToAtkInterface <Atk.Text> (accessible);
			Assert.AreEqual (name, atkText.GetText(0, name.Length), "GetText#2");
			
			expected = "\r\nwith EOL but without dot, and other phrase...";
			Assert.AreEqual (expected,
				atkText.GetTextAfterOffset (3, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextAfterOffset,SentenceEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAfterOffset,SentenceEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAfterOffset,SentenceEnd,eo");
			
			expected = "Tell me; here a sentence\r\n";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (4, Atk.TextBoundary.SentenceStart, out startOffset, out endOffset),
				"GetTextAtOffset,SentenceStart");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,SentenceStart,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,SentenceStart,eo");
			
			expected = "Tell me; here a sentence";
			Assert.AreEqual (expected,
				atkText.GetTextAtOffset (4, Atk.TextBoundary.SentenceEnd, out startOffset, out endOffset),
				"GetTextAtOffset,SentenceEnd");
			Assert.AreEqual (name.IndexOf (expected), startOffset, "GetTextAtOffset,SentenceEnd,so");
			Assert.AreEqual (name.IndexOf (expected) + expected.Length, endOffset, "GetTextAtOffset,SentenceEnd,eo");
			
			
			return accessible;
		}

		protected string simpleTestText = "This is a test sentence.";

		protected void Parent (BasicWidgetType type, Atk.Object accessible)
		{
			Atk.Object parent = accessible.Parent;
			Assert.IsNotNull (parent, "parent not null");
			if (type == BasicWidgetType.Window)
				Assert.AreEqual (parent.Role, Atk.Role.Application, "Parent of a frame should be an application");
			int count = parent.NAccessibleChildren;
			for (int i = 0; i < count; i++)
				if (parent.RefAccessibleChild (i) == accessible)
					return;
			Assert.Fail ("Object should be child of parent");
		}

		protected void States (Atk.Object accessible, params Atk.StateType [] expected)
		{
			Dictionary<Atk.StateType, bool> expectedStates = new Dictionary<Atk.StateType, bool> ();;
			foreach (Atk.StateType state in expected)
				expectedStates [state] = true;
			Atk.StateSet stateSet = accessible.RefStateSet ();
			foreach (Atk.StateType state in Enum.GetValues (typeof (Atk.StateType)))
				if (expectedStates.ContainsKey (state))
					Assert.IsTrue (stateSet.ContainsState (state), "Missing state: " + state);
				else
					Assert.IsFalse (stateSet.ContainsState (state), "Superfluous state: " + state);
		}

		private double GetMinimumValue (Atk.Value value)
		{
			GLib.Value gv = new GLib.Value (0);
			value.GetMinimumValue (ref gv);
			return (double)gv.Val;
		}

		private double GetMaximumValue (Atk.Value value)
		{
			GLib.Value gv = new GLib.Value (0);
			value.GetMaximumValue (ref gv);
			return (double)gv.Val;
		}

		private double GetCurrentValue (Atk.Value value)
		{
			GLib.Value gv = new GLib.Value (0);
			value.GetCurrentValue (ref gv);
			return (double)gv.Val;
		}

		public abstract void RunInGuiThread (System.Action d);
	}
}
