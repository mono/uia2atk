#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/12/2009
# Description: Test accessibility of openfiledialog widget 
#              Use the openfiledialogframe.py wrapper script
#              Test the samples/openfiledialog.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of openfiledialog widget 

"""

# imports
import sys
import os

from strongwind import *
from savefiledialog import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the savefiledialog sample application
try:
  app = launchSaveFileDialog(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
ofdFrame = app.saveFileDialogFrame

# click "Click me" button to open the "Save As" dialog, and then find all the
# accessibles of that dialog
ofdFrame.opendialog_button.click(log=True)
sleep(config.MEDIUM_DELAY)
ofdFrame.findAllSaveDialogWidgets()

# check the default states of all of the accessibles on the "Save As" dialog
ofdFrame.checkDefaultSaveDialogStates()

# click the button to make a new directory, and then find all of the
# accessibles of that dialog
ofdFrame.new_dir_toolbar_button.click(log=True)
sleep(config.MEDIUM_DELAY)
ofdFrame.findAllNewFolderOrFileAccessibles()
# check the default states of all of the accessibles on the "New Folder Or
# File" dialog
ofdFrame.checkDefaultNewFolderOrFileDialogStates()

# close the "New Folder Or File" dialog
ofdFrame.new_folder_cancel.click(log=True)
ofdFrame.new_folder_dialog.assertClosed()

# click popUpButton personal to rise focused, there is no "Personal" folder on treetable
ofdFrame.personal_popup.click(log=True)
sleep(config.SHORT_DELAY)
# ensure that the click action had the desired affect on the GUI by making sure
# the "Personal" menu item is the selected child of the "Save in" combo box.
# We need to refind the "Save In" combo box accessibles again first, because
# they reload (and change) when the popup is clicked.
ofdFrame.findSaveInComboBoxAccessibles()
ofdFrame.assertSelectedChild(ofdFrame.save_in_combobox,
                             ofdFrame.personal_menuitem)
# BUG475082 OpenFileDialog: PopupButton has wrong states -- Everything fixed
# except +/- "focused" state.
#statesCheck(ofdFrame.personal_menuitem,
#            "MenuItem",
#            add_states=["focusable", "focused", "selected"])

# keyUp move focus to desktop_popup
ofdFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
ofdFrame.findSaveInComboBoxAccessibles()
# the selection of the "Save In" combo box shouldn't actually change
ofdFrame.assertSelectedChild(ofdFrame.save_in_combobox,
                             ofdFrame.personal_menuitem)
# BUG475082 OpenFileDialog: PopupButton has wrong states -- Everything fixed
# except +/- "focused" state.
#statesCheck(ofdFrame.personal_menuitem,
#            "MenuItem",
#            add_states=["focusable", "focused", "selected"])

# mouseClick move focus to mycomputer_popup
ofdFrame.mycomputer_popup.mouseClick()
sleep(config.SHORT_DELAY)
ofdFrame.findSaveInComboBoxAccessibles()
ofdFrame.assertSelectedChild(ofdFrame.save_in_combobox,
                             ofdFrame.mycomputer_menuitem)
# BUG475082 OpenFileDialog: PopupButton has wrong states -- Everything fixed
# except +/- "focused" state.
#statesCheck(ofdFrame.personal_menuitem,
#            "MenuItem",
#            add_states=["focusable", "focused", "selected"])

ofdFrame.cancel_button.click(log=True)
# this requires a MEDIUM_DELAY because it can take a while to close
sleep(config.MEDIUM_DELAY)
ofdFrame.savedialog.assertClosed()

# invoke action to save file
ofdFrame.opendialog_button.click(log=True)
sleep(config.MEDIUM_DELAY)
ofdFrame.findAllSaveDialogWidgets()
ofdFrame.a_blank_file_cell.invoke(log=True)
sleep(config.SHORT_DELAY)
ofdFrame.findSaveConfirmationDialogAccessibles()
# now close the "Save" confirmation dialog
ofdFrame.save_confirmation_cancel_button.click(log=True)
sleep(config.SHORT_DELAY)

# test view style dropdown menu_items from toolbar button on the top
ofdFrame.dropDownMenuItemTests()

# TODO: Test context menu when clicking in the TreeTable

# TODO: Test icon view button on the toolbar and test TreeTable with different
# icons
ofdFrame.checkImageSize()

# test accessibles of the save_in_combobox
ofdFrame.saveInComboBoxAccessiblesTest()

# close savedialog window
ofdFrame.cancel_button.click(log=True)
# this requires a MEDIUM_DELAY because it can take a while to close
sleep(config.MEDIUM_DELAY)

#close application frame window
ofdFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
