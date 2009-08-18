#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/12/2009
# Description: Test accessibility of openfiledialog widget 
#              Use the openfiledialogframe.py wrapper script
#              Test the samples/winforms/openfiledialog.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of openfiledialog widget 

"""

# imports
import sys
import os

from strongwind import *
from openfiledialog import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the openfiledialog sample application
try:
  app = launchOpenFileDialog(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
ofdFrame = app.openFileDialogFrame

# click "EnableVisible" button to show open dialog page, then check HelpButon 
# and ReadOnlyCheckBox are showing up
ofdFrame.enable_button.click(log=True)
sleep(config.MEDIUM_DELAY)
ofdFrame.findAllOpenDialogWidgets()
ofdFrame.testVisibleWidgets(is_visible=True)

# close opendialog window
ofdFrame.cancel_button.click(log=True)
sleep(config.MEDIUM_DELAY)
ofdFrame.opendialog.assertClosed()

# click "OpenDialog" button to open the "Open" dialog, and then find all the
# accessibles of that dialog
ofdFrame.opendialog_button.click(log=True)
sleep(config.MEDIUM_DELAY)
ofdFrame.findAllOpenDialogWidgets()
ofdFrame.testVisibleWidgets(is_visible=False)

# check the default states of all of the accessibles on the "Open" dialog
ofdFrame.checkDefaultOpenDialogStates()

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

# test view style dropdown menu_items from toolbar button on the top
ofdFrame.dropDownMenuItemTests()

# Test context menu when clicking in the TreeTable
ofdFrame.treetable.mouseClick(button=3)
sleep(config.SHORT_DELAY)
ofdFrame.findContextMenuAccessibles()
ofdFrame.contextMenuAccessiblesTest()

ofdFrame.smallicon_menuitem.click(log=True)
sleep(config.SHORT_DELAY)
# BUG514635:menu's descendant menu_item is missing 'focusable' state
#statesCheck(ofdFrame.smallicon_menuitem, "MenuItem", \
#                           invalid_states=["showing"], add_states=["checked"])

# test accessible's Image by check image size
ofdFrame.checkImageSize()

# test accessibles of the look_in_combobox
ofdFrame.lookInComboBoxAccessiblesTest()

# click popUpButton personal to rise focused, there is no "Personal" folder on treetable
ofdFrame.personal_popup.click(log=True)
sleep(config.SHORT_DELAY)
# ensure that the click action had the desired affect on the GUI by making sure
# the "Personal" menu item is the selected child of the "Look In" combo box.
# We need to refind the "Look In" combo box accessibles again first, because
# they reload (and change) when the popup is clicked.
ofdFrame.findLookInComboBoxAccessibles()
ofdFrame.assertSelectedChild(ofdFrame.look_in_combobox,
                             ofdFrame.personal_menuitem)
# BUG475082 FileDialog: PopupButton has wrong states -- Everything fixed
# except +/- "focused" state.
#statesCheck(ofdFrame.personal_menuitem,
#            "MenuItem",
#            add_states=["focusable", "focused", "selected"])

# keyUp move focus to desktop_popup
ofdFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
ofdFrame.findLookInComboBoxAccessibles()
# the selection of the "Look In" combo box shouldn't actually change
ofdFrame.assertSelectedChild(ofdFrame.look_in_combobox,
                             ofdFrame.desktop_menuitem)
# BUG475082 FileDialog: PopupButton has wrong states -- Everything fixed
# except +/- "focused" state.
#statesCheck(ofdFrame.personal_menuitem,
#            "MenuItem",
#            add_states=["focusable", "focused", "selected"])

# mouseClick move focus to mycomputer_popup
ofdFrame.mycomputer_popup.mouseClick()
sleep(config.SHORT_DELAY)
ofdFrame.findLookInComboBoxAccessibles()
ofdFrame.assertSelectedChild(ofdFrame.look_in_combobox,
                             ofdFrame.mycomputer_menuitem)
# BUG475082 FileDialog: PopupButton has wrong states -- Everything fixed
# except +/- "focused" state.
#statesCheck(ofdFrame.personal_menuitem,
#            "MenuItem",
#            add_states=["focusable", "focused", "selected"])

# close savedialog window
ofdFrame.cancel_button.click(log=True)
# this requires a MEDIUM_DELAY because it can take a while to close
sleep(config.MEDIUM_DELAY)

# invoke 'Open' dialog again
ofdFrame.opendialog_button.click(log=True)
sleep(config.MEDIUM_DELAY)
ofdFrame.findAllOpenDialogWidgets()

# invoke action to open file, open dialog is closed
ofdFrame.a_blank_file_cell.invoke(log=True)
sleep(config.MEDIUM_DELAY)
ofdFrame.opendialog.assertClosed()

#close application frame window
ofdFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
