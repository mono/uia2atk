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

###################################################
##search for all widgets from open dialog
###################################################

#click "Click me" button to show savedialog page, then check subwidgets
ofdFrame.click(ofdFrame.opendialog_button)
sleep(config.MEDIUM_DELAY)
ofdFrame.AssertWidgets()

###########################################################
##search for all widgets from new folder dialog
##add new folder
###########################################################

#click newdirtoolbarbutton to creat new folder, then check subwidgets
ofdFrame.click(ofdFrame.newdirtoolbarbutton)
sleep(config.SHORT_DELAY)
ofdFrame.NewFolderCheck()
#enter new folder name, then click Cancel button will not create it
ofdFrame.CreatFolder(ofdFrame.newfolder_cancel)

#click newdirtoolbarbutton to creat new folder, then check subwidgets
ofdFrame.click(ofdFrame.newdirtoolbarbutton)
sleep(config.SHORT_DELAY)
ofdFrame.NewFolderCheck()
#enter new folder name, then click OK button will create it
ofdFrame.CreatFolder(ofdFrame.newfolder_ok)

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
#to recentlyused_menuitem, there is no "ANewFolder" folder on treetable
##click menuitem doesn't change dir BUG475529
ofdFrame.ItemClick(ofdFrame.recentlyused_menuitem)
sleep(config.SHORT_DELAY)
ofdFrame.AssertMenuItemClick("ANewFolder")
statesCheck(ofdFrame.recentlyused_menuitem, "MenuItem", add_states=["focused", "selected"])
statesCheck(samples_menuitem, "MenuItem")

#use keyDown move focus and selection to desktop_menuitem
ofdFrame.recentlyused_menuitem.keyCombo("Down", grabFocus=True)
sleep(config.SHORT_DELAY)
statesCheck(ofdFrame.desktop_menuitem, "MenuItem", add_states=["focused", "selected"])
statesCheck(ofdFrame.recentlyused_menuitem, "MenuItem", invalid_states=["showing"])

#use mouseClick move focus and selection to mycomputer_menuitem, there is no 
#"My Computer" folder on treetable
##mouse click menuitem under dir_combobox crash app BUG474611
ofdFrame.dir_menu.mouseClick()
sleep(config.SHORT_DELAY)
ofdFrame.mycomputer_menuitem.mouseClick()
sleep(config.SHORT_DELAY)
ofdFrame.AssertMenuItemClick("My Computer")
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

#click popUpButton personal to rise focused, there is no "Personal" folder on treetable
ofdFrame.ItemClick(ofdFrame.personal_popup)
sleep(config.SHORT_DELAY)
ofdFrame.AssertMenuItemClick("Personal")
statesCheck(ofdFrame.personal_popup, "MenuItem", add_states=["focusable", "focused"])

#keyUp move focus to desktop_popup, there is no "Desktop" folder on treetable
ofdFrame.personal_popup.keyCombo("Up", grabFocus=True)
sleep(config.SHORT_DELAY)
ofdFrame.AssertMenuItemClick("Desktop")
statesCheck(ofdFrame.desktop_popup, "MenuItem", add_states=["focusable", "focused"])
statesCheck(ofdFrame.personal_popup, "MenuItem", add_states=["focusable"])

#mouseClick move focus to mycomputer_popup, there is no "My Computer" folder on treetable
ofdFrame.mycomputer_popup.mouseClick()
sleep(config.SHORT_DELAY)
ofdFrame.AssertMenuItemClick("My Computer")
statesCheck(ofdFrame.mycomputer_popup  , "MenuItem", add_states=["focusable", "focused"])
statesCheck(ofdFrame.desktop_popup, "MenuItem", add_states=["focusable"])

###############################################
##test activate action for Tabel Cell
###############################################

#activate action to double click README file
##missing activate action BUG476365
ofdFrame.click(ofdFrame.opendialog_button)
sleep(config.SHORT_DELAY)
ofdFrame.AssertWidgets()
sleep(config.SHORT_DELAY)
#double click to open "README" file
ofdFrame.assertActivate("README")

#activate action to double click ANewFolder folder
ofdFrame.click(ofdFrame.opendialog_button)
sleep(config.SHORT_DELAY)
ofdFrame.AssertWidgets()
sleep(config.SHORT_DELAY)
#double click to enter "ANewFolder" folder
ofdFrame.assertActivate("ANewFolder")


#close savedialog window
ofdFrame.click(ofdFrame.cancel_button)

#close application frame window
ofdFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
