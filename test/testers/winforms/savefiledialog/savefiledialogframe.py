
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/17/2009
# Description: savefiledialog.py wrapper script
#              Used by the savefiledialog-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from helpers import *
from strongwind import *
from savefiledialog import *
from sys import path

class SearchFolderError(Exception):
    'Raised when search an extra item method fails'
    pass

# class to represent the main window.
class SaveFileDialogFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON1 = "Click me"
    NUM_TOOLBARS = 2
    NUM_SMALL_TOOLBAR_BUTTONS = 4
    NUM_COMBOBOXES = 3
    NUM_POPUPMENUITEMS = 5

    def __init__(self, accessible):
        super(SaveFileDialogFrame, self).__init__(accessible)
        self.opendialog_button = self.findPushButton(self.BUTTON1)
 
    # assert if all widgets on Open dialog are showing
    def findAllSaveDialogWidgets(self):
        """
        Search for all widgets on "Save As" dialog
        """
        procedurelogger.action("search for all widgets from SaveDialog windows")
        procedurelogger.expectedResult("All of the widgets are found successfully")
        # click button will invoke dialog page
        self.savedialog = self.app.findDialog("Save As")

        # there are 3 labels in dialog
        self.lookin_label = self.savedialog.findLabel("Save in:")
        self.filename_label = self.savedialog.findLabel("File name:")
        self.savetype_label = self.savedialog.findLabel("Save as type:")

        # there are 2 push button in dialog
        self.save_button = self.savedialog.findPushButton("Save")
        self.cancel_button = self.savedialog.findPushButton("Cancel")

        # find the save dialog toolbars and ensure that we find the expected
        # number of toolbars
        self.toolbars = self.savedialog.findAllToolBars(None)
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

        # find the combo box accessibles that are direct descendants of the
        # "Save As" dialog, and then assert that we found the expected number
        self.comboboxes = self.savedialog.findAllComboBoxs(None)
        assert len(self.comboboxes) == self.NUM_COMBOBOXES, \
                     "Found %s combo box accessibles, expected %s" %\
                     (len(self.comboboxes), self.NUM_COMBOBOXES)

        # give intuitive names to each of the combo boxes
        self.file_type_combobox = self.comboboxes[0]
        self.filename_combobox = self.comboboxes[1]
        self.save_in_combobox = self.comboboxes[2]

        # find the menu of the save_in_combobox
        self.findSaveInComboBoxAccessibles()        

        # find the menu of the filename_combobox
        self.filename_menu = \
                      self.filename_combobox.findMenu(None, checkShowing=False)
        # the menu items can differ greatly, we'll just find anything that is
        # there
        self.filename_menu_items = \
                 self.filename_menu.findAllMenuItems(None, checkShowing=False)

        # find the menu of the file type combobox
        self.file_type_menu = \
                     self.file_type_combobox.findMenu(None, checkShowing=False)
        # Find all of the menu items of the file type combo box.  Some day,
        # the number of these might differ too, so we'll just take what we
        # get.
        self.file_type_menu_items = \
                 self.file_type_menu.findAllMenuItems(None, checkShowing=False)
        # the "All files (*.*) menu item should always exist, at least.  We
        # want to find this one because it should always be the default
        # selected item in the combo box
        self.all_files_menu_item = \
                            self.file_type_menu.findMenuItem("All files (*.*)")
        # we should at least get one menu item
        assert len(self.file_type_menu_items) > 0, \
                    "Found %s file type menu items, expected at least 1" % \
                                                 len(self.file_type_menu_items)

        # There is a tree table, which has all of the file icons in it.  The
        # file icons are represented as table cells of the tree table.  The
        # "Save As" dialog should start in the samples directory, so we
        # can find some of the table cells based on sample names.  There is
        # some difficult here because different table cells may be showing
        # on different machines because of the SaveDialog size.  The table
        # cells we find below should take this into consideration.
        self.treetable = self.savedialog.findTreeTable(None)
        # all of these table cells should be showing on any given machine
        self.a_blank_file_cell = self.treetable.findTableCell("a_blank_file")
        self.apple_red_png_cell = self.treetable.findTableCell("apple-red.png")
        self.attribute_test_rtf_cell = \
                             self.treetable.findTableCell("attribute-test.rtf")
        self.button_label_linklabel_py_cell = \
                      self.treetable.findTableCell("button_label_linklabel.py")
        # this table cell should not be showing on any given machine
        self.winradiobutton_py_cell = \
                          self.treetable.findTableCell("winradiobutton.py",
                                                       checkShowing=False)

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

    def checkDefaultSaveDialogStates(self):
        '''
        Check the states of the accessibles that are available from the default
        SaveDialog
        '''
        procedurelogger.action("Check the states of the default Savedialog accessibles")
        procedurelogger.expectedResult("All the states are what is expected")
        # check the states of the actual dialog
        statesCheck(self.savedialog, "Dialog", add_states=["active", "modal"])

        # check states of the labels in dialog
        statesCheck(self.lookin_label, "Label")
        statesCheck(self.filename_label, "Label")
        statesCheck(self.savetype_label, "Label")

        # check states of the push buttons in dialog
        statesCheck(self.save_button, "Button")
        statesCheck(self.cancel_button, "Button")

        # check the states of the two tool bars
        statesCheck(self.small_toolbar, "ToolBar")
        statesCheck(self.popuptoolbar, "ToolBar")

        # check the state of the two tool bars' children
        # the back button is disabled by default
        statesCheck(self.back_toolbar_button,
                    "Button",
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
        statesCheck(self.filename_combobox, "ComboBox")
        statesCheck(self.save_in_combobox, "ComboBox")

        # check the states of the dir menu
        statesCheck(self.save_in_menu,
                    "Menu",
                    invalid_states=["visible", "showing"])
        # we can't really check the menu items of the save_in_menu at this
        # point because we can't guarantee what will be showing

        # check states of the menu of the filename_combobox
        statesCheck(self.filename_menu,
                    "Menu",
                    invalid_states=["visible", "showing"])
        # we can't really check the menu items of the save_in_menu at this
        # point because we can't guarantee what will be showing

        # check the states of the menu of the file type combobox
        statesCheck(self.file_type_menu,
                    "Menu",
                    invalid_states=["visible", "showing"])
        # check the states of all of the menu items of the file type combo box
        # menu
        for menu_item in self.file_type_menu_items:
            # the all_files_menu_item is selected by default
            if menu_item is self.all_files_menu_item:
                statesCheck(menu_item, "MenuItem", add_states=["selected"])
            else:
                # BUG506745 combo box receives "focused" state, but do not have
                # "focusable" state
                #statesCheck(menu_item, "MenuItem", invalid_states=["showing"])
                pass # delete this once the above bug is resolved

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

    def saveInComboBoxAccessiblesTest(self):
        '''
        test all of the accessibles of save_in_combobox
        '''
        # find all of the accessibles
        self.findSaveInComboBoxAccessibles()
        # check save_in_combobox and children' states,
        # menu and menuitems are invisible and isn't showing
        # BUG506745: receives "focused" state, but do not have "focusable" state
        #statesCheck(self.save_in_combobox, "ComboBox", add_states=["focusable"])
        statesCheck(self.save_in_menu, "Menu", \
                                     invalid_states=["showing", "visible"])
        # BUG506744: receive "focused" state, but do not have "focusable" state, 
        # affect all states tests below
        #statesCheck(self.recently_used_menuitem, "MenuItem", \
        #                              invalid_states=["showing"])
        # samples_menuitem is selected and focused by default
        #samples_menuitem = self.save_in_menu.findMenuItem("samples")
        # BUG510048: selected index of ComboBox erroneously has "focused" state
        #statesCheck(samples_menuitem, "MenuItem", add_states=["selected"])

        # click menuitem under save_in_menu to check its AtkAction, move focus 
        # and selection to recentlyused_menuitem
        self.recently_used_menuitem.click(log=True)
        sleep(config.SHORT_DELAY)
        # find accessible again because menu is refreshed
        self.findSaveInComboBoxAccessibles()
        self.assertSelectedChild(self.save_in_combobox,
                             self.recently_used_menuitem)
        # BUG510048: selected index of ComboBox erroneously has "focused" state
        #statesCheck(self.recently_used_menuitem, "MenuItem", \
        #                                             add_states=["selected"])
        #statesCheck(samples_menuitem, "MenuItem")

        # use keyDown move focus and selection to desktop_menuitem
        self.recently_used_menuitem.keyCombo("Down", grabFocus=True)
        sleep(config.SHORT_DELAY)
        # find accessible again because menu is refreshed
        self.findSaveInComboBoxAccessibles()
        # BUG510048: selected index of ComboBox erroneously has "focused" state
        #statesCheck(self.desktop_menuitem, "MenuItem", add_states=["selected"])
        #statesCheck(self.recently_used_menuitem, "MenuItem")

        # use mouseClick expand combobox
        self.save_in_combobox.mouseClick()
        sleep(config.SHORT_DELAY)
        # find accessible again because menu is refreshed
        self.findSaveInComboBoxAccessibles()
        # BUG506745: receives "focused" state, but do not have "focusable" state
        #statesCheck(self.save_in_combobox, "ComboBox", \
        #                                 add_states=["focused", "focusable"])
        # first menu_item is selected so menu also raise selected state because
        # Selection is implemented for combobox
        statesCheck(self.save_in_menu, "Menu", add_states=["selected"])
        #statesCheck(self.desktop_menuitem, "MenuItem", \
        #                                add_states=["focused", "selected"])

        # mouse click to move focus and selection to mycomputer_menuitem
        self.mycomputer_menuitem.mouseClick()
        sleep(config.SHORT_DELAY)
        # find accessible again because menu is refreshed
        self.findSaveInComboBoxAccessibles()
        self.assertSelectedChild(self.save_in_combobox,
                            self.mycomputer_menuitem)
        # BUG510048: selected index of ComboBox erroneously has "focused" state
        #statesCheck(self.mycomputer_menuitem, "MenuItem", add_states=["selected"])
        #statesCheck(self.desktop_menuitem, "MenuItem")

    def findSaveInComboBoxAccessibles(self):
        ''' 
        find all of the accessibles of save_in_combobox.  This needs to be
        called after any action that causes the widgets (and thus the
        accesibles) of the "Save In" ComboBox to load or refresh.
        '''
        # find the menu of the save_in_combobox
        self.save_in_menu = \
                       self.save_in_combobox.findMenu(None, checkShowing=False)
        # there are many menuitems and they will differ between machines, so
        # we will just find the ones that should be common between machines.
        self.recently_used_menuitem = \
            self.save_in_menu.findMenuItem("Recently used", checkShowing=False)
        self.desktop_menuitem = \
                  self.save_in_menu.findMenuItem("Desktop", checkShowing=False)
        self.personal_menuitem = \
            self.save_in_menu.findMenuItem("Personal folder", \
                                            checkShowing=False)
        self.mycomputer_menuitem = \
              self.save_in_menu.findMenuItem("My Computer", checkShowing=False)
        self.mynetwork_menuitem = \
               self.save_in_menu.findMenuItem("My Network", checkShowing=False)
        # we'll also find all of them together
        self.save_in_menu_items = \
               self.save_in_combobox.findAllMenuItems(None, checkShowing=False)
        assert len(self.save_in_menu_items) > 5, "Found %s menu items of the directory combo box, expected at least 5" % len(self.save_in_menu_items)

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

    # assert activate table cell to enter folder or open file
    def enterFolderOrOpenFile(self, is_folder):
        """
        Expect an action to execute double click to enter folder or open file. 
        Doing invoke action for a folder named 'ANewFolder' to make sure it's
        an empty folder; Doing invoke action for a file to make sure it can be
        saved and OpenDialog is closed
        """
        if is_folder:
            procedurelogger.expectedResult('Enter to folder')
            try:
                table_cell = self.treetable.findTableCell(None)
                if len(table_cell) > 0:
                    raise SearchFolderError
            except SearchError:
                pass
        else:
            procedurelogger.expectedResult('file is saved')
            self.app.findDialog("Save").findPushButton("OK").click()
            sleep(config.SHORT_DELAY)
            self.savedialog.assertClosed()

    def findSaveConfirmationDialogAccessibles(self):
        '''
        Find all of the accessibles on the "Save" dialog that appears when
        attempting to save a file that already exists 
        '''
        self.save_confirmation_dialog = self.app.findDialog("Save") 
        self.save_confirmation_ok_button = \
                             self.save_confirmation_dialog.findPushButton("OK")
        self.save_confirmation_cancel_button = \
                         self.save_confirmation_dialog.findPushButton("Cancel")
        self.save_confirmation_icon = \
                                     self.save_confirmation_dialog.findIcon("")
        self.save_confirmation_label = self.save_confirmation_dialog.findLabel(re.compile("overwrite it\?$"))

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

        # ensure cells' image size in file view treetable are 16*16(list style)
        self.assertImageSize(self.a_blank_file_cell, expected_width=16, expected_height=16)
        self.assertImageSize(self.apple_red_png_cell, expected_width=16, expected_height=16)

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

    # close application main window after running test
    def quit(self):
        self.altF4()
