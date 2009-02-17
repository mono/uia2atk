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
##search for help button and readonly checkbox from open dialog
###############################################################################

#click EnableVisible button to show openfiledialog page , then check subwidgets, 
#HelpButon and ReadOnlyCheckBox are showing up
ofdFrame.click(ofdFrame.enable_button)
sleep(config.MEDIUM_DELAY)
ofdFrame.assertWidgets()
ofdFrame.assertVisibleWidget()

#close opendialog window
ofdFrame.click(ofdFrame.cancel_button)

###################################################
##search for all widgets from open dialog
###################################################

#click OpenDialog button to show openfiledialog page, then check subwidgets
ofdFrame.click(ofdFrame.opendialog_button)
sleep(config.MEDIUM_DELAY)
ofdFrame.assertWidgets()

###########################################################
##search for all widgets from new folder dialog
##check add now folder action
###########################################################

#click newdirtoolbarbutton to creat new folder, then check subwidgets
ofdFrame.itemClick(ofdFrame.newdirtoolbarbutton)
sleep(config.SHORT_DELAY)
ofdFrame.newFolderCheck()
#enter new folder name, then click Cancel button will not create it
ofdFrame.creatFolder(ofdFrame.newfolder_cancel)

#click newdirtoolbarbutton to creat new folder, then check subwidgets
ofdFrame.itemClick(ofdFrame.newdirtoolbarbutton)
sleep(config.SHORT_DELAY)
#enter new folder name, then click OK button will create it
ofdFrame.creatFolder(ofdFrame.newfolder_ok)

#######################################################
##dirComboBox test
##check states for items under dirCombobox
##check Action for dirCombobox and menuitems
#######################################################

#check dirComboBox and children' states, dir_menu and menuitems are invisible and isn't showing
statesCheck(ofdFrame.dir_combobox, "ComboBox")
statesCheck(ofdFrame.dir_menu, "Menu", invalid_states=["showing", "visible"])
statesCheck(ofdFrame.recentlyused_menuitem, "MenuItem", invalid_states=["showing"])

#click dirComboBox to expand menu, then check states again
ofdFrame.press(ofdFrame.dir_combobox)
sleep(config.SHORT_DELAY)
statesCheck(ofdFrame.dir_combobox, "ComboBox")
statesCheck(ofdFrame.dir_menu, "Menu")
#recentlyused_menuitem on the top of the list, so it is un-showing 
statesCheck(ofdFrame.recentlyused_menuitem, "MenuItem", invalid_states=["showing"])
#mynetwork_menuitem is shown and visible
statesCheck(ofdFrame.mynetwork_menuitem, "MenuItem")
#samples_menuitem is selected and focused by default
samples_menuitem = ofdFrame.dir_menu.findMenuItem("samples")
statesCheck(samples_menuitem, "MenuItem", add_states=["focused", "selected"])

#click menuitem under dir_menu to check its AtkAction, move focus and selection
#to recentlyused_menuitem, there is no "ANewFolder" folder on listview
##click menuitem doesn't change dir BUG475529
ofdFrame.itemClick(ofdFrame.recentlyused_menuitem)
sleep(config.SHORT_DELAY)
ofdFrame.assertMenuItemClick("ANewFolder")
statesCheck(ofdFrame.recentlyused_menuitem, "MenuItem", add_states=["focused", "selected"])
statesCheck(samples_menuitem, "MenuItem")

#use keyDown move focus and selection to desktop_menuitem
ofdFrame.recentlyused_menuitem.keyCombo("Down", grabFocus=True)
sleep(config.SHORT_DELAY)
statesCheck(ofdFrame.desktop_menuitem, "MenuItem", add_states=["focused", "selected"])
statesCheck(ofdFrame.recentlyused_menuitem, "MenuItem", invalid_states=["showing"])

#use mouseClick move focus and selection to mycomputer_menuitem, there is no 
#"My Computer" folder on listview
##mouse click menuitem under dir_combobox crash app BUG474611
ofdFrame.dir_menu.mouseClick()
sleep(config.SHORT_DELAY)
ofdFrame.mycomputer_menuitem.mouseClick()
sleep(config.SHORT_DELAY)
ofdFrame.assertMenuItemClick("My Computer")
statesCheck(ofdFrame.mycomputer_menuitem, "MenuItem", add_states=["focused", "selected"])
statesCheck(ofdFrame.desktop_menuitem, "MenuItem")

#############################################################
##popUpButtonPanel and popUpButton test
##test states
##test Action and navigation for popUpButton
#############################################################

##incorrect states BUG475082
statesCheck(ofdFrame.popuptoolbar, "ToolBar")
statesCheck(ofdFrame.recentlyused_popup, "MenuItem", add_states=["focusable"])
statesCheck(ofdFrame.mynetwork_popup, "MenuItem", add_states=["focusable"])

#click popUpButton personal to rise focused, there is no "Personal" folder on listview
ofdFrame.itemClick(ofdFrame.personal_popup)
sleep(config.SHORT_DELAY)
ofdFrame.assertMenuItemClick("Personal")
statesCheck(ofdFrame.personal_popup, "MenuItem", add_states=["focusable", "focused"])

#keyUp move focus to desktop_popup, there is no "Desktop" folder on listview
ofdFrame.personal_popup.keyCombo("Up", grabFocus=True)
sleep(config.SHORT_DELAY)
ofdFrame.assertMenuItemClick("Desktop")
statesCheck(ofdFrame.desktop_popup, "MenuItem", add_states=["focusable", "focused"])
statesCheck(ofdFrame.personal_popup, "MenuItem", add_states=["focusable"])

#mouseClick move focus to mycomputer_popup, there is no "My Computer" folder on listview
ofdFrame.mycomputer_popup.mouseClick()
sleep(config.SHORT_DELAY)
ofdFrame.assertMenuItemClick("My Computer")
statesCheck(ofdFrame.mycomputer_popup  , "MenuItem", add_states=["focusable", "focused"])
statesCheck(ofdFrame.desktop_popup, "MenuItem", add_states=["focusable"])

#close opendialog window
ofdFrame.click(ofdFrame.cancel_button)

#close application frame window
ofdFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
