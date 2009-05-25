
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

class SearchFolderError(Exception):
    'Raised when search an extra item method fails'
    pass

# class to represent the main window.
class OpenFileDialogFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON1 = "OpenDialog"
    BUTTON2 = "EnableVisible"

    def __init__(self, accessible):
        super(OpenFileDialogFrame, self).__init__(accessible)
        self.opendialog_button = self.findPushButton(self.BUTTON1)
        self.enable_button = self.findPushButton(self.BUTTON2)

    def assertNormalElements(self):
        """
        Search for Label, normal PushButton, Treeview,  ComboBox elements on 
        Open Dialog
        """
        self.opendialog = self.app.findDialog("Open")

        # there are 3 labels in dialog
        procedurelogger.expectedResult("3 Labels are showing up")
        self.lookin_label = self.opendialog.findLabel("Look in:")
        self.filename_label = self.opendialog.findLabel("File name:")
        self.filesoftype_label = self.opendialog.findLabel("Files of type:")

        # there are 2 push button in dialog
        procedurelogger.expectedResult("2 PushButtons are showing up")
        self.open_button = self.opendialog.findPushButton("Open")
        self.cancel_button = self.opendialog.findPushButton("Cancel")

        # there are 2 normal combobox on bottom and 1 dirComboBox on top
        procedurelogger.expectedResult("2 normal combobox on bottom and 1 dirComboBox on top are showing up")
        self.comboboxs = self.opendialog.findAllComboBoxs(None)
        self.filename_combobox = self.comboboxs[0]
        self.filetype_combobox = self.comboboxs[1]
        self.dir_combobox = self.comboboxs[2]

        # There are 1 TreeTable with some TableCells
        procedurelogger.expectedResult("1 TreeTable with many TableCells are showing up")
        self.listview = self.opendialog.findTreeTable(None)
        self.listitems = self.listview.findAllTableCells(None)

        assert len(self.listitems) != 0, "no list items under listview"

    def assertPopUpButtonElements(self):
        """Search for PopUpButton elements on PopUpToolBar"""
        # there are 5 popupbutton in popupbuttonpanel on the left side
        procedurelogger.expectedResult("5 PopUpButton are showing up")
        self.toolbars = self.opendialog.findAllToolBars(None)
        self.popuptoolbar = self.toolbars[1]
        self.recentlyused_popup = self.popuptoolbar.findMenuItem(re.compile('^Recently'))
        self.desktop_popup = self.popuptoolbar.findMenuItem("Desktop")
        self.personal_popup = self.popuptoolbar.findMenuItem("Personal")
        self.mycomputer_popup = self.popuptoolbar.findMenuItem("My Computer")
        self.mynetwork_popup = self.popuptoolbar.findMenuItem("My Network")

    def assertSmallToolBarButtonElements(self):
        """Search for SmallToolBarButton elements on SmallToolBar"""
        # toolbar with 4 small toolbarbuttons on the top right side
        procedurelogger.expectedResult("4 PushButton in small toolbar are showing up")
        self.smalltoolbar = self.toolbars[0]
        self.toolbarbuttons = self.smalltoolbar.findAllPushButtons(None)
        self.backtoolbarbutton = self.toolbarbuttons[0]
        self.uptoolbarbutton = self.toolbarbuttons[1]
        self.newdirtoolbarbutton = self.toolbarbuttons[2]
        self.menutoolbarbutton = self.toolbarbuttons[3]
        self.menutogglebutton = self.smalltoolbar.findToggleButton(None)

        # there are 5 menuitems under menutogglebutton
        ##BUG490105:accessible position and size of ToggleButton is incorrect 
        '''
        self.menutogglebutton.mouseClick(log=False)
        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("5 MenuItem are showing up after click menutogglebutton")
        self.window = self.app.findWindow(None)
        self.smallicon_menuitem = self.window.findMenuItem("Small Icon")
        self.tiles_menuitem = self.window.findMenuItem("Tiles")
        self.largeicon_menuitem = self.window.findMenuItem("Large Icon")
        self.list_menuitem = self.window.findMenuItem("List")
        self.details_menuitem = self.window.findMenuItem("Details")
        self.menutogglebutton.mouseClick()
        '''
   
    def assertDirComboBoxElements(self):
        """Search for DirComboBox elements on DirComboBox"""
        # there are 5 menuitems under dir_combobox
        procedurelogger.expectedResult("5 MenuItems under dir_combobox are showing up")
        self.dir_menu = self.dir_combobox.findMenu(None, checkShowing=False)
        self.recentlyused_menuitem = self.dir_menu.findMenuItem("Recently used", checkShowing=False)
        self.desktop_menuitem = self.dir_menu.findMenuItem("Desktop", checkShowing=False)
        self.personal_menuitem = self.dir_menu.findMenuItem("Personal", checkShowing=False)
        self.mycomputer_menuitem = self.dir_menu.findMenuItem("My Computer", checkShowing=False)
        self.mynetwork_menuitem = self.dir_menu.findMenuItem("My Network", checkShowing=False)

    def assertVisibleElements(self):
        """
        To make sure Help button and ReadOnly checkbox are showing up

        """
        procedurelogger.expectedResult('"Help" button and "Open Readonly" checkbox widgets are showing up')
        self.help_button = self.opendialog.findPushButton("Help")
        self.readonly_checkbox = self.opendialog.findCheckBox("Open Readonly")

    def creatNewFolderTest(self, is_created):
        """
        Invoke New Folder or File dialog, to make sure all widgets on this 
        dialog are showing up. 

        Enter folder name, if accessible is Ok button will create new folder, 
        else if accessible is Cancel button won't create new folder

        """
        # Action
        self.newdirtoolbarbutton.click(log=True)
        sleep(config.SHORT_DELAY)

        # Expected Result
        self.newfolderdialog = self.app.findDialog("New Folder or File")

        procedurelogger.expectedResult("All widgets in NewFolderOrFile dialog is showing up")
        self.newfolder_panel = self.newfolderdialog.findPanel("New Name")
        self.newfolder_label =self.newfolder_panel.findLabel("Enter Name:")
        self.newfolder_icon = self.newfolder_panel.findIcon(None)
        self.newfolder_text = self.newfolder_panel.findText(None)
        self.newfolder_ok = self.newfolderdialog.findPushButton("OK")
        self.newfolder_cancel = self.newfolderdialog.findPushButton("Cancel")

        # Action
        # enter folder name to textbox, click OK button will create new folder, 
        # click Cancel button won't create new folder
        self.newfolder_text.enterText("ANewFolder")
        sleep(config.SHORT_DELAY)

        # Expected result
        if is_created:
            self.newfolder_ok.click()
            sleep(config.SHORT_DELAY)

            procedurelogger.expectedResult("create new folder")
            self.listview.findTableCell("ANewFolder")

        else:
            self.newfolder_cancel.click()
            sleep(config.SHORT_DELAY)

            procedurelogger.expectedResult("never create new folder")
            try:
                new_menuitem = self.listview.findTableCell("ANewFolder")
                if new_menuitem:
                    raise SearchFolderError
            except SearchError:
                pass

    def assertDirChange(self, foldername):
        """
        Doing click action against MenuItems under dirCombobox to make sure 
	directory is changed to the selected MenuItem by checking the 
	expected foldername doesn't showing in the new directory

        """
        procedurelogger.expectedResult("Directory is changed, you can't find %s folder" % foldername)
        try:
            foldername_menuitem = self.listview.findTableCell(foldername)
            if foldername_menuitem:
               raise SearchFolderError
        except SearchError:
            pass
   
    # close application main window after running test
    def quit(self):
        # delete ANewFolder
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        os.rmdir("%s/samples/ANewFolder" % uiaqa_path)

        sleep(config.SHORT_DELAY)
        # close form
        self.altF4()
