
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/11/2009
# Description: openfiledialog.py wrapper script
#              Used by the openfiledialog-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from openfiledialog import *
from helpers import *

# class to represent the main window.
class OpenFileDialogFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON1 = "OpenDialog"
    BUTTON2 = "EnableVisible"
    NUM_COMBOBOXS = 3
    NUM_FILETYPEMENUITEMS = 0
    NUM_TOOLBARS = 2
    NUM_SMALL_TOOLBAR_BUTTONS = 4
    NUM_POPUPMENUITEMS = 5

    def __init__(self, accessible):
        super(OpenFileDialogFrame, self).__init__(accessible)
        self.opendialog_button = self.findPushButton(self.BUTTON1)
        self.enable_button = self.findPushButton(self.BUTTON2)

    def findAllOpenDialogWidgets(self):
        """
        Search for all widgets on Open Dialog
        """
        procedurelogger.action("search for all widgets from Open Dialog")
        procedurelogger.expectedResult("All of the widgets are found successfully")

        # click button will invoke dialog page
        self.opendialog = self.app.findDialog("Open")

        # there are 3 labels in dialog
        self.look_in_label = self.opendialog.findLabel("Look in:")
        self.file_name_label = self.opendialog.findLabel("File name:")
        self.file_type_label = self.opendialog.findLabel("Files of type:")

        # there are 2 push button in dialog
        self.open_button = self.opendialog.findPushButton("Open")
        self.cancel_button = self.opendialog.findPushButton("Cancel")

        # there are 3 comboboxs
        self.comboboxs = self.opendialog.findAllComboBoxs(None)
        assert len(self.comboboxs) == self.NUM_COMBOBOXS, \
                     "Found %s combo box accessibles, expected %s" %\
                     (len(self.comboboxes), self.NUM_COMBOBOXES)
        # give intuitive names to each of the combo box
        self.file_type_combobox = self.comboboxs[0]
        self.file_name_combobox = self.comboboxs[1]
        self.look_in_combobox = self.comboboxs[2]

        # find the menu of the look_in_combobox
        self.findLookInComboBoxAccessibles() 

        # find the menu of the file_name_combobox
        self.file_name_menu = \
                      self.file_name_combobox.findMenu(None, checkShowing=False)
        # the menu items can differ greatly, we'll just find anything that is
        # there
        self.file_name_menu_items = \
                 self.file_name_menu.findAllMenuItems(None, checkShowing=False)

        # find the menu of the file_type_combobox
        self.file_type_menu = \
                     self.file_type_combobox.findMenu(None, checkShowing=False)

        # file_type_menu has zero descendant
        actual_child = self.file_type_menu.childCount
        assert actual_child == self.NUM_FILETYPEMENUITEMS, \
                 "file_type_menu actual have child: %s, expected child: %s" % \
                                     (actual_child, self.NUM_FILETYPEMENUITEMS)

        # There is a tree table, which has all of the file icons in it.The
        # "Open" dialog should start in the samples directory, so we
        # can find some of the table cells based on sample names. 
        self.treetable = self.opendialog.findTreeTable(None)
        # all of these table cells should be showing on any given machine
        self.a_blank_file_cell = self.treetable.findTableCell("a_blank_file")
        self.apple_red_png_cell = self.treetable.findTableCell("apple-red.png")
        self.attribute_test_rtf_cell = \
                             self.treetable.findTableCell("attribute-test.rtf")
        self.button_label_linklabel_py_cell = \
                      self.treetable.findTableCell("button_label_linklabel.py")
        # this table cell is off screen on any given machine because of the 
	# dialog size
        self.winradiobutton_py_cell = \
                          self.treetable.findTableCell("winradiobutton.py",
                                                       checkShowing=False)

        # find the open dialog toolbars and ensure that we find the expected
        # number of toolbars
        self.toolbars = self.opendialog.findAllToolBars(None)
        assert len(self.toolbars) == self.NUM_TOOLBARS, \
                                "Found %s tool bar accessibles, expected %s" %\
                                (len(self.toolbars), self.NUM_TOOLBARS)

        # give an intuitive name to the toolbar in the upper right of the
        # dialog
        self.small_toolbar = self.toolbars[0]

        # find all of the push buttons of the small_toolbar and ensure that
        # we find the expected number of push buttons
        self.small_toolbar_buttons = self.small_toolbar.findAllPushButtons(None)
        assert len(self.small_toolbar_buttons) == self.NUM_SMALL_TOOLBAR_BUTTONS, \
                 "Found %s push button bar accessibles, expected %s" %\
               (len(self.small_toolbar_buttons), self.NUM_SMALL_TOOLBAR_BUTTONS)
        # give intuitive names to each of the small toolbar buttons and find
        # the single toggle button (that is part of the toolbar button with a
        # drop down menu).
        self.back_toolbar_button = self.small_toolbar_buttons[0]
        self.up_toolbar_button = self.small_toolbar_buttons[1]
        self.new_dir_toolbar_button = self.small_toolbar_buttons[2]
        self.menu_toolbar_button = self.small_toolbar_buttons[3]
        self.menu_toggle_button = self.small_toolbar.findToggleButton(None)

        # give an intuitive name to the toolbar on the left side of the dialog
        self.popuptoolbar = self.toolbars[1]
        # find the menu item "popup" buttons on the left toolbar
        self.recently_used_popup = \
                    self.popuptoolbar.findMenuItem('Recently\nused')
        self.desktop_popup = self.popuptoolbar.findMenuItem("Desktop")
        self.personal_popup = self.popuptoolbar.findMenuItem("Personal")
        self.mycomputer_popup = self.popuptoolbar.findMenuItem("My Computer")
        self.mynetwork_popup = self.popuptoolbar.findMenuItem("My Network")

        # we also find all menuitems from popup toolbar
        self.popup_toolbar_menuitems = self.popuptoolbar.findAllMenuItems(None)
        assert len(self.popup_toolbar_menuitems) == self.NUM_POPUPMENUITEMS, \
                     "Found %s popup menuitem accessibles, expected %s" %\
                    (len(self.popup_toolbar_menuitems), self.NUM_POPUPMENUITEMS)

    def checkDefaultOpenDialogStates(self):
        '''
        Check the states of the accessibles that are available from the default
        OpenDialog
        '''
        procedurelogger.action("Check the states of the default OpenDialog accessibles")
        procedurelogger.expectedResult("All the states are what is expected")
        # check the states of the actual dialog
        statesCheck(self.opendialog, "Dialog", add_states=["active", "modal"])

        # check states of the labels in dialog
        statesCheck(self.look_in_label, "Label")
        statesCheck(self.file_name_label, "Label")
        statesCheck(self.file_type_label, "Label")

        # check states of the push buttons in dialog
        statesCheck(self.open_button, "Button")
        statesCheck(self.cancel_button, "Button")

        # check the states of the two tool bars
        statesCheck(self.small_toolbar, "ToolBar")
        statesCheck(self.popuptoolbar, "ToolBar")

        # check the state of the two tool bars' children
        # the back button is disabled by default
        statesCheck(self.back_toolbar_button, "Button",
                    invalid_states=["sensitive", "enabled"])
        statesCheck(self.up_toolbar_button, "Button")
        statesCheck(self.new_dir_toolbar_button, "Button")
        statesCheck(self.menu_toolbar_button, "Button")
        statesCheck(self.menu_toggle_button, "ToggleButton")
        statesCheck(self.recently_used_popup, "MenuItem")
        statesCheck(self.desktop_popup, "MenuItem")
        statesCheck(self.personal_popup, "MenuItem")
        statesCheck(self.mycomputer_popup, "MenuItem")
        statesCheck(self.mynetwork_popup, "MenuItem")

        # check the states of each of the combo boxes
        statesCheck(self.file_type_combobox, "ComboBox")
        statesCheck(self.file_name_combobox, "ComboBox")
        statesCheck(self.look_in_combobox, "ComboBox")

        # check the states of the dir menu
        statesCheck(self.look_in_menu,
                    "Menu",
                    invalid_states=["visible", "showing"])
        # we can't really check the menu items of the look_in_menu at this
        # point because we can't guarantee what will be showing

        # check states of the menu of the filename_combobox
        statesCheck(self.file_name_menu,
                    "Menu",
                    invalid_states=["visible", "showing"])
        # we can't really check the menu items of the look_in_menu at this
        # point because we can't guarantee what will be showing

        # check the states of the menu of the file type combobox
        statesCheck(self.file_type_menu,
                    "Menu",
                    invalid_states=["visible", "showing"])

        # check the states of the tree table
        statesCheck(self.treetable, "TreeTable")
        # all of these table cells should be showing on any given machine
        statesCheck(self.apple_red_png_cell,
                    "TableCell",
                    add_states=["editable"])
        statesCheck(self.attribute_test_rtf_cell,
                    "TableCell",
                    add_states=["editable"])
        statesCheck(self.button_label_linklabel_py_cell,
                    "TableCell",
                    add_states=["editable"])
        # this table cell should not be showing on any given machine
        statesCheck(self.winradiobutton_py_cell,
                    "TableCell",
                    invalid_states=["showing", "visible"],
                    add_states=["editable"])

    def lookInComboBoxAccessiblesTest(self):
        '''
        test all of the accessibles from look_in_combobox
        '''
        # find all of the accessibles
        self.findLookInComboBoxAccessibles()  
        # check look_in_combobox and children' states,
        # menu and menuitems are invisible and isn't showing
        # BUG506745: receives "focused" state, but do not have "focusable" state
        #statesCheck(self.look_in_combobox, "ComboBox", add_states=["focusable"])
        statesCheck(self.look_in_menu, "Menu", \
                                     invalid_states=["showing", "visible"])
        # BUG506744: receive "focused" state, but do not have "focusable" state, 
        # affect all states tests below
        #statesCheck(self.recently_used_menuitem, "MenuItem", \
        #                              invalid_states=["showing"])
        # samples_menuitem is selected and focused by default
        #samples_menuitem = self.look_in_menu.findMenuItem("samples")
        # BUG510048: selected index of ComboBox erroneously has "focused" state
        #statesCheck(samples_menuitem, "MenuItem", add_states=["selected"])

        # click menuitem under look_in_menu to check its AtkAction, move focus 
        # and selection to recentlyused_menuitem
        self.recently_used_menuitem.click(log=True)
        sleep(config.SHORT_DELAY)
        # find accessible again because menu is refreshed
        self.findLookInComboBoxAccessibles()
        self.assertSelectedChild(self.look_in_combobox,
                             self.recently_used_menuitem)
        # BUG510048: selected index of ComboBox erroneously has "focused" state
        #statesCheck(self.recently_used_menuitem, "MenuItem", \
        #                                             add_states=["selected"])
        #statesCheck(samples_menuitem, "MenuItem")

        # use keyDown move focus and selection to desktop_menuitem
        self.recently_used_menuitem.keyCombo("Down", grabFocus=True)
        sleep(config.SHORT_DELAY)
        # find accessible again because menu is refreshed
        self.findLookInComboBoxAccessibles()
        # BUG510048: selected index of ComboBox erroneously has "focused" state
        #statesCheck(self.desktop_menuitem, "MenuItem", add_states=["selected"])
        #statesCheck(self.recently_used_menuitem, "MenuItem")

        # use mouseClick expand combobox
        self.look_in_combobox.mouseClick()
        sleep(config.SHORT_DELAY)
        # find accessible again because menu is refreshed
        self.findLookInComboBoxAccessibles()
        # BUG506745: receives "focused" state, but do not have "focusable" state
        #statesCheck(self.look_in_combobox, "ComboBox", \
        #                                 add_states=["focused", "focusable"])
        statesCheck(self.look_in_menu, "Menu")
        #statesCheck(self.desktop_menuitem, "MenuItem", \
        #                                add_states=["focused", "selected"])

        # mouse click to move focus and selection to mycomputer_menuitem
        self.mycomputer_menuitem.mouseClick()
        sleep(config.SHORT_DELAY)
        # find accessible again because menu is refreshed
        self.findLookInComboBoxAccessibles()
        self.assertSelectedChild(self.look_in_combobox,
                            self.mycomputer_menuitem)
        # BUG510048: selected index of ComboBox erroneously has "focused" state
        #statesCheck(self.mycomputer_menuitem, "MenuItem", add_states=["selected"])
        #statesCheck(self.desktop_menuitem, "MenuItem")

    def findLookInComboBoxAccessibles(self):
        ''' 
        find all of the accessibles of look_in_combobox.  This needs to be
        called after any action that causes the widgets (and thus the
        accesibles) of the "Look In" ComboBox to load or refresh.
        '''
        # find the menu of the look_in_combobox
        self.look_in_menu = \
                       self.look_in_combobox.findMenu(None, checkShowing=False)
        # there are many menuitems and they will differ between machines, so
        # we will just find the ones that should be common between machines.
        self.recently_used_menuitem = \
            self.look_in_menu.findMenuItem("Recently used", checkShowing=False)
        self.desktop_menuitem = \
                  self.look_in_menu.findMenuItem("Desktop", checkShowing=False)
        self.personal_menuitem = \
            self.look_in_menu.findMenuItem("Personal folder", \
                                            checkShowing=False)
        self.mycomputer_menuitem = \
              self.look_in_menu.findMenuItem("My Computer", checkShowing=False)
        self.mynetwork_menuitem = \
               self.look_in_menu.findMenuItem("My Network", checkShowing=False)
        # we'll also find all of them together
        self.look_in_menu_items = \
               self.look_in_combobox.findAllMenuItems(None, checkShowing=False)
        assert len(self.look_in_menu_items) > 5, "Found %s menu items of the directory combo box, expected at least 5" % len(self.look_in_menu_items)

    def testVisibleWidgets(self, is_visible):
        """
        Make sure if Help button and ReadOnly checkbox are showing up
        """
        if is_visible:
            procedurelogger.expectedResult('"Help" button and "Open Readonly" checkbox widgets are showing up')
            self.help_button = self.opendialog.findPushButton("Help")
            self.readonly_checkbox = self.opendialog.findCheckBox("Open Readonly")

            # states check
            statesCheck(self.help_button, "Button")
            statesCheck(self.readonly_checkbox, "Button")
        else:
            procedurelogger.expectedResult('"Help" button and "Open Readonly" checkbox shouldn\'t showing up')
            try:
                self.opendialog.findPushButton("Help")
                self.opendialog.findCheckBox("Open Readonly")
            except SearchError:
                return
            assert False, '"Help" and "Open Readonly" shouldn\'t showing up'

    def findAllNewFolderOrFileAccessibles(self):
        '''
        Search for all widgets on "New Folder or File" dialog
        '''
        procedurelogger.action('search for all widgets from "New Folder or File" dialog')
        procedurelogger.expectedResult("All of the widgets are found successfully")
        self.new_folder_dialog = self.app.findDialog("New Folder or File")
        self.new_folder_panel = self.new_folder_dialog.findPanel("New Name")
        self.new_folder_label =self.new_folder_panel.findLabel("Enter Name:")
        self.new_folder_icon = self.new_folder_panel.findIcon(None)
        self.new_folder_text = self.new_folder_panel.findText(None)
        self.new_folder_ok = self.new_folder_dialog.findPushButton("OK")
        self.new_folder_cancel = \
                                self.new_folder_dialog.findPushButton("Cancel")

    def checkDefaultNewFolderOrFileDialogStates(self):
        statesCheck(self.new_folder_dialog,
                    "Dialog",
                    invalid_states=["resizable"],
                    add_states=["active", "modal"])
        statesCheck(self.new_folder_panel, "Panel")
        statesCheck(self.new_folder_label, "Label")
        statesCheck(self.new_folder_icon, "Icon")
        statesCheck(self.new_folder_text, "Text", add_states=["focused"])
        statesCheck(self.new_folder_ok, "Button")
        statesCheck(self.new_folder_cancel, "Button")

    def dropDownMenuItemTests(self):
        self.menu_toggle_button.click(log=True)
        sleep(config.MEDIUM_DELAY)

        self.findDropDownMenuItemAccessibles()
        
        # default states check for all menu items
        # BUG511179: all menu_items have checked state
        #statesCheck(self.smallicon_menuitem, "MenuItem")
        #statesCheck(self.tiles_menuitem, "MenuItem")
        #statesCheck(self.largeicon_menuitem, "MenuItem")
        statesCheck(self.list_menuitem, "MenuItem", add_states=["checked"])
        #statesCheck(self.details_menuitem, "MenuItem")

        # select Details view style
        # BUG511193: click menuitem may crash the application
        #self.details_menuitem.click(log=True)
        #sleep(config.SHORT_DELAY)
        # BUG511154: table cells are not accessible when Details is selected
        #self.treetable.findTableCell("a_blank_file")
        # BUG511157: TreeTable implement wrong name when Detail style is selected
        #assert self.treetable.name == "", \
        #                      "actual name is %s, expected name is %s" % \
        #                      (self.treetable.name, "")
        #statesCheck(self.details_menuitem, "MenuItem", add_states=["checked"])
        #statesCheck(self.list_menuitem, "MenuItem")        

    def findDropDownMenuItemAccessibles(self):
        """
        Find all of the accessibles on the view style window that appears when 
        menu_toggle_button is clicked
        """
        self.view_style_window = self.app.findWindow(None)
        procedurelogger.expectedResult("5 MenuItem under view style window are showing up")
        self.view_style_menu = self.view_style_window.findMenu(None)

        self.smallicon_menuitem = \
           self.view_style_menu.findMenuItem("Small Icon", checkShowing=False)
        self.tiles_menuitem = \
                self.view_style_menu.findMenuItem("Tiles", checkShowing=False)
        self.largeicon_menuitem = \
           self.view_style_menu.findMenuItem("Large Icon", checkShowing=False)
        self.list_menuitem = \
                 self.view_style_menu.findMenuItem("List", checkShowing=False)
        self.details_menuitem = \
              self.view_style_menu.findMenuItem("Details", checkShowing=False)

    def contextMenuAccessiblesTest(self):
        """
        Check all ContextMenu accessibles default states
        """
        # check default states
        statesCheck(self.window, "ContextMenu", add_states=["active"])
        statesCheck(self.view_menu, "Menu", add_states=["focusable"])
        statesCheck(self.new_menu, "Menu", add_states=["focusable"])
        
        statesCheck(self.show_hidden_menuitem, "MenuItem")
        # BUG514635:menu's descendant menu_item is missing 'focusable' state
        # BUG514647:some unchecked menu items have extraneous "checked" state   
        #statesCheck(self.view_smallicon_menuitem, "MenuItem", \
        #                   invalid_states=["showing"])
        #statesCheck(self.view_tiles_menuitem, "MenuItem", \
        #                   invalid_states=["showing"])
        #statesCheck(self.view_largeicon_menuitem, "MenuItem", \
        #                   invalid_states=["showing"])
        #statesCheck(self.view_list_menuitem, "MenuItem", \
        #                   invalid_states=["showing"], add_states=["checked"])
        #statesCheck(self.view_details_menuitem, "MenuItem", \
        #                   invalid_states=["showing"])
        #statesCheck(self.new_folder_menuitem, "MenuItem", \
        #                   invalid_states=["showing"])

    def findContextMenuAccessibles(self):
        """
        Find all of the accessibles on the ContextMenu when mouse 
        button3 is click against table_cells on TreeTable list
        """
        procedurelogger.expectedResult("All of the widgets are found successfully")
        self.window = self.app.findWindow(None)

        self.view_menu = self.window.findMenu("View")
        # view_menu has 5 menu_item descendants
        self.view_smallicon_menuitem = \
                   self.view_menu.findMenuItem("Small Icon", checkShowing=False)
        self.view_tiles_menuitem = \
                        self.view_menu.findMenuItem("Tiles", checkShowing=False)
        self.view_largeicon_menuitem = \
                   self.view_menu.findMenuItem("Large Icon", checkShowing=False)
        self.view_list_menuitem = \
                         self.view_menu.findMenuItem("List", checkShowing=False)
        self.view_details_menuitem = \
                      self.view_menu.findMenuItem("Details", checkShowing=False)
 
        self.new_menu = self.window.findMenu("New")
        # new_menu has 1 menu_item descendant
        self.new_folder_menuitem = \
                   self.new_menu.findMenuItem("New Folder", checkShowing=False)

        self.show_hidden_menuitem = self.window.findMenuItem("Show hidden files")

    def checkImageSize(self):
        """
        check accessible's image size by using assertImageSize method wraper
        """
        # ensure all small toolbar buttons' image size are 16*16
        for accessible in self.small_toolbar_buttons:
            self.assertImageSize(accessible, expected_width=16, expected_height=16)

        # ensure all popup toolbar menuitems' image size are 32*32
        # BUG511152: AtkImage is not implemented
        #for accessible in self.popup_toolbar_menuitems:
        #    self.assertImageSize(accessible, expected_width=32, expected_height=32)

        # ensure cells' image size in file view treetable are 24*24(list style)
        self.assertImageSize(self.a_blank_file_cell, expected_width=24, expected_height=24)
        self.assertImageSize(self.apple_red_png_cell, expected_width=24, expected_height=24)

    # assert the size of an image to test AtkImage implementation
    def assertImageSize(self, accessible, expected_width=0, expected_height=0):
        """make sure accessible's image size is expected """
        procedurelogger.action("assert %s's image size" % accessible)
        actual_width, actual_height = \
                              accessible._accessible.queryImage().getImageSize()

        procedurelogger.expectedResult('"%s" image size is %s x %s' %
                                 (accessible, expected_width, expected_height))

        assert actual_width == expected_width, "%s (%s), %s (%s)" %\
                                  ("expecself.popup_toolbar_menuitemsted width",
                                              expected_width,
                                             "does not match actual width",
                                              actual_width)
        assert actual_height == expected_height, "%s (%s), %s (%s)" %\
                                            ("expected height",
                                              expected_height,
                                             "does not match actual height",
                                              actual_height)

    def assertSelectedChild(self, accessible, expected_selected_child):
        '''
        assert that accessible's selected child matches selected_menu_item
        ''' 
        procedurelogger.expectedResult("%s is selected" % expected_selected_child)
        actual_selected_child = accessible.getSelectedChild(0)
        assert actual_selected_child._accessible == \
                            expected_selected_child._accessible, \
                            '"%s" was the selected child, expected "%s"' % \
                            (actual_selected_child, expected_selected_child)

    # close application main window after running test
    def quit(self):
        self.altF4()
