
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

    def __init__(self, accessible):
        super(SaveFileDialogFrame, self).__init__(accessible)
        self.opendialog_button = self.findPushButton(self.BUTTON1)
 
    # assert if all widgets on Open dialog are showing
    def AssertWidgets(self):
        """
        Search for all widgets on Open Dialog
        """
        procedurelogger.action("search for all widgets from SaveDialog windows")

        # click button will invoke dialog page
        self.savedialog = self.app.findDialog("Save As")
        # there are 3 labels in dialog
        procedurelogger.expectedResult("3 Labels are showing up")
        self.lookin_label = self.savedialog.findLabel("Save in:")
        self.filename_label = self.savedialog.findLabel("File name:")
        self.savetype_label = self.savedialog.findLabel("Save as type:")
        # there are 2 push button in dialog
        procedurelogger.expectedResult("2 PushButtons are showing up")
        self.save_button = self.savedialog.findPushButton("Save")
        self.cancel_button = self.savedialog.findPushButton("Cancel")
        # there are 5 popupbutton in popupbuttonpanel on the left side
        procedurelogger.expectedResult("5 PopUpButton are showing up")
        self.toolbars = self.savedialog.findAllToolBars(None)
        self.popuptoolbar = self.toolbars[1]
        self.recentlyused_popup = self.popuptoolbar.findMenuItem(re.compile('^Recently'))
        self.desktop_popup = self.popuptoolbar.findMenuItem("Desktop")
        self.personal_popup = self.popuptoolbar.findMenuItem("Personal")
        self.mycomputer_popup = self.popuptoolbar.findMenuItem("My Computer")
        self.mynetwork_popup = self.popuptoolbar.findMenuItem("My Network")
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
        '''
        #this should be used if BUG481357 is fixed 
        procedurelogger.expectedResult("5 MenuItem under menutogglebutton are showing up")
        self.smallicon_menuitem = self.menutogglebutton.findMenuItem("Small Icon", checkShowing=False)
        self.tiles_menuitem = self.menutogglebutton.findMenuItem("Tiles", checkShowing=False)
        self.largeicon_menuitem = self.menutogglebutton.findMenuItem("Large Icon", checkShowing=False)
        self.list_menuitem = self.menutogglebutton.findMenuItem("List", checkShowing=False)
        self.details_menuitem = self.menutogglebutton.findMenuItem("Details", checkShowing=False)
        '''
        # there are 2 normal combobox on bottom and 1 dirComboBox on top
        procedurelogger.expectedResult("2 normal combobox on bottom and 1 dirComboBox on top are showing up")
        self.comboboxs = self.savedialog.findAllComboBoxs(None)
        self.filename_combobox = self.comboboxs[1]
        self.filetype_combobox = self.comboboxs[0]
        self.dir_combobox = self.comboboxs[2]
        # there are 5 menuitems under dir_combobox
        procedurelogger.expectedResult("5 MenuItems under dir_combobox are showing up")
        self.dir_menu = self.dir_combobox.findMenu(None, checkShowing=False)
        self.recentlyused_menuitem = self.dir_menu.findMenuItem("Recently used", checkShowing=False)
        self.desktop_menuitem = self.dir_menu.findMenuItem("Desktop", checkShowing=False)
        self.personal_menuitem = self.dir_menu.findMenuItem("Personal", checkShowing=False)
        self.mycomputer_menuitem = self.dir_menu.findMenuItem("My Computer", checkShowing=False)
        self.mynetwork_menuitem = self.dir_menu.findMenuItem("My Network", checkShowing=False)
        # There are 1 TreeTable with some TableCells
        procedurelogger.expectedResult("1 TreeTable with many TableCells are showing up")
        self.treetable = self.savedialog.findTreeTable(None)
        self.tablecells = self.treetable.findAllTableCells(None)

        assert len(self.tablecells) != 0, "no table cells under treetable"

    # assert if all widgets on creat new folder dialog are showing
    def creatNewFolderTest(self, is_created):
        """
        Click button to invoke New Folder or File dialog, to make sure all 
	widgets on this dialog are showing up. 

        Enter folder name, click Ok button will create new folder, click Cancel 
	button won't create new folder

        """
        # Action
        self.newdirtoolbarbutton.click(log=True)
        sleep(config.SHORT_DELAY)

        # Expected Result
        procedurelogger.expectedResult("All widgets in NewFolderOrFile dialog is showing up")
        self.newfolderdialog = self.app.findDialog("New Folder or File")
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
            self.treetable.findTableCell("ANewFolder")

        else:
            self.newfolder_cancel.click()
            sleep(config.SHORT_DELAY)

            procedurelogger.expectedResult("never create new folder")
            try:
                new_menuitem = self.treetable.findTableCell("ANewFolder")
                if new_menuitem:
                    raise SearchFolderError
            except SearchError:
                pass

    def assertDirChanged(self, foldername):
        """
        Doing click action against MenuItems under dirCombobox to make sure 
	directory is changed to the selected MenuItem by checking the 
	expected foldername doesn't showing in the new directory
        """
        procedurelogger.expectedResult("Directory is changed, you can't find %s folder" % foldername)
        try:
            foldername_menuitem = self.treetable.findTableCell(foldername)
            if foldername_menuitem:
               raise SearchFolderError
        except SearchError:
            pass

    # assert activate table cell to enter folder or open file
    def enterFolderOrOpenFile(self, is_folder):
        """
        Expect an action to execute double click to enter folder or open file. 
	Doing invoke action for a folder named 'ANewFolder' to make sure it's
	an empty folder;
        Doing invoke action for a file to make sure it can be saved and 
	OpenDialog is closed
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

   
    # close application main window after running test
    def quit(self):
        # delete ANewFolder
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        os.rmdir("%s/samples/ANewFolder" % uiaqa_path)
        os.rm("%s/samples/ANewFile" % uiaqa_path)

        sleep(config.SHORT_DELAY)
        # close form
        self.altF4()
