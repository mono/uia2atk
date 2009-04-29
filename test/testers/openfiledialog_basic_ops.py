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

###############################################################################
# click EnableVisible button to show openfiledialog page, then check HelpButon 
# and ReadOnlyCheckBox are showing up
###############################################################################

ofdFrame.click(ofdFrame.enable_button)
sleep(config.MEDIUM_DELAY)
ofdFrame.assertNormalElements()
ofdFrame.assertVisibleElements()

# close opendialog window
ofdFrame.click(ofdFrame.cancel_button)
sleep(config.MEDIUM_DELAY)
ofdFrame.opendialog.assertClosed()

############################################################################
# click OpenDialog button to show openfiledialog page, then check all elements 
# are showing up
############################################################################

ofdFrame.click(ofdFrame.opendialog_button)
sleep(config.MEDIUM_DELAY)
##BUG490105:accessible position and size of ToggleButton is incorrect 
ofdFrame.assertNormalElements()
ofdFrame.assertPopUpButtonElements()
ofdFrame.assertSmallToolBarButtonElements()
ofdFrame.assertDirComboBoxElements()

###########################################################
# search for all widgets from "New Folder or File" dialog,
# click Cancel button won't create new folder,
# click OK button will create new folder
###########################################################

ofdFrame.creatNewFolderTest("Cancel")

ofdFrame.creatNewFolderTest("Ok")

#####################
# dirComboBox test
#####################

# check dirComboBox and children' states, dir_menu and menuitems are invisible 
# and isn't showing
statesCheck(ofdFrame.dir_combobox, "ComboBox")
statesCheck(ofdFrame.dir_menu, "Menu", invalid_states=["showing", "visible"])
statesCheck(ofdFrame.recentlyused_menuitem, "MenuItem", invalid_states=["showing"])

# click dirComboBox to expand menu, then check states again
ofdFrame.press(ofdFrame.dir_combobox)
sleep(config.SHORT_DELAY)
statesCheck(ofdFrame.dir_combobox, "ComboBox")
statesCheck(ofdFrame.dir_menu, "Menu")
# recentlyused_menuitem on the top of the list, so it is un-showing 
statesCheck(ofdFrame.recentlyused_menuitem, "MenuItem", invalid_states=["showing"])
# mynetwork_menuitem is shown and visible
statesCheck(ofdFrame.mynetwork_menuitem, "MenuItem")
# samples_menuitem is selected and focused by default
samples_menuitem = ofdFrame.dir_menu.findMenuItem("samples")
statesCheck(samples_menuitem, "MenuItem", add_states=["focused", "selected"])

# click menuitem under dir_menu to check its AtkAction, move focus and selection
# to recentlyused_menuitem, there is no "ANewFolder" folder on listview
ofdFrame.click(ofdFrame.recentlyused_menuitem, log=True)
sleep(config.SHORT_DELAY)
ofdFrame.assertDirChange("ANewFolder")
# search for elements under dirCombobox again
ofdFrame.assertDirComboBoxElements()

statesCheck(ofdFrame.recentlyused_menuitem, "MenuItem", add_states=["focused", "selected"])

# use keyDown move focus and selection to desktop_menuitem
## BUG499139: SWF has not provide keyUp/Down navigation
#ofdFrame.recentlyused_menuitem.keyCombo("Down", grabFocus=True)
#sleep(config.SHORT_DELAY)
#statesCheck(ofdFrame.desktop_menuitem, "MenuItem", add_states=["focused", "selected"])
#statesCheck(ofdFrame.recentlyused_menuitem, "MenuItem", invalid_states=["showing"])

# move focus and selection to mycomputer_menuitem, there is no "My Computer" folder on listview
ofdFrame.click(ofdFrame.mycomputer_menuitem, log=True)
sleep(config.SHORT_DELAY)
#ofdFrame.mouseClick()
#sleep(config.SHORT_DELAY)
ofdFrame.assertDirChange("My Computer")
ofdFrame.assertDirComboBoxElements()

statesCheck(ofdFrame.mycomputer_menuitem, "MenuItem", add_states=["focused", "selected"])

# regression test that click first and last dir menuitem doesn't crash app due to bug474611
ofdFrame.click(ofdFrame.recentlyused_menuitem, log=True)
sleep(config.SHORT_DELAY)
ofdFrame.mouseClick()
sleep(config.SHORT_DELAY)
ofdFrame.assertDirComboBoxElements()

ofdFrame.click(ofdFrame.mynetwork_menuitem, log=True)
sleep(config.SHORT_DELAY)
ofdFrame.assertDirComboBoxElements()

# click dirCombobox to contract menu_item list
ofdFrame.dir_combobox.mouseClick(log=False)

########################################
# popUpButtonPanel and popUpButton test
########################################
##states test blocks by BUG490572 and BUG475082

#statesCheck(ofdFrame.popuptoolbar, "ToolBar")
#statesCheck(ofdFrame.recentlyused_popup, "MenuItem", add_states=["focusable"])
#statesCheck(ofdFrame.mynetwork_popup, "MenuItem", add_states=["focusable"])

# click popUpButton personal to rise focused, there is no "Personal" folder on listview
ofdFrame.click(ofdFrame.personal_popup, log=True)
sleep(config.SHORT_DELAY)
ofdFrame.assertDirChange("Personal")
#statesCheck(ofdFrame.personal_popup, "MenuItem", add_states=["focusable", "focused"])

# keyUp move focus to desktop_popup, there is no "Desktop" folder on listview
ofdFrame.personal_popup.keyCombo("Up", grabFocus=True)
sleep(config.SHORT_DELAY)
ofdFrame.assertDirChange("Desktop")
#statesCheck(ofdFrame.desktop_popup, "MenuItem", add_states=["focusable", "focused"])
#statesCheck(ofdFrame.personal_popup, "MenuItem", add_states=["focusable"])

# mouseClick move focus to mycomputer_popup, there is no "My Computer" folder on listview
ofdFrame.mycomputer_popup.mouseClick()
sleep(config.SHORT_DELAY)
ofdFrame.assertDirChange("My Computer")
#statesCheck(ofdFrame.mycomputer_popup  , "MenuItem", add_states=["focusable", "focused"])
#statesCheck(ofdFrame.desktop_popup, "MenuItem", add_states=["focusable"])

# close opendialog window
ofdFrame.click(ofdFrame.cancel_button)
sleep(config.SHORT_DELAY)
ofdFrame.opendialog.assertClosed()

# close application frame window
ofdFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
