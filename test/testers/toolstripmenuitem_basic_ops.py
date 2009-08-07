#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/31/2008
# Description: Test accessibility of toolstripmenuitem widget 
#              Use the toolstripmenuitemframe.py wrapper script
#              Test the samples/toolstripmenuitem.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of toolstripmenuitem widget
"""

# imports
import sys
import os

from strongwind import *
from toolstripmenuitem import *
from helpers import *
from states import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the toolstripmenuitem sample application
try:
  app = launchToolStripMenuItem(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tsmiFrame = app.toolStripMenuItemFrame

# check actions
actionsCheck(tsmiFrame.view_menu, "Menu")
actionsCheck(tsmiFrame.help_menu, "Menu")
actionsCheck(tsmiFrame.file_menu, "Menu")
actionsCheck(tsmiFrame.edit_menu, "Menu")
actionsCheck(tsmiFrame.create_menu_item, "MenuItem")
actionsCheck(tsmiFrame.write_menu_item, "MenuItem")
actionsCheck(tsmiFrame.financial_menu_item, "MenuItem")
actionsCheck(tsmiFrame.medical_menu_item, "MenuItem")
actionsCheck(tsmiFrame.new_menu_item, "MenuItem")
actionsCheck(tsmiFrame.open_menu_item, "MenuItem")
actionsCheck(tsmiFrame.copy_this_menu_item, "MenuItem")
actionsCheck(tsmiFrame.paste_that_menu_item, "MenuItem")

# check states
# We are unsure about whether these should have "focusable" states or not
# The resolution of BUG485515 might clear this up
#statesCheck(tsmiFrame.view_menu, "Menu")
#statesCheck(tsmiFrame.help_menu, "Menu")
statesCheck(tsmiFrame.file_menu, "Menu")
statesCheck(tsmiFrame.edit_menu, "Menu")
# BUG486335: MenuItem, ToolStripMenuItem: extraneous "showing" state of 
# menu item when it is not showing
#statesCheck(tsmiFrame.create_menu_item, "MenuItem", invalid_states=["showing"])
#statesCheck(tsmiFrame.write_menu_item, "MenuItem", invalid_states=["showing"])
#statesCheck(tsmiFrame.financial_menu_item, "MenuItem", invalid_states=["showing"])
#statesCheck(tsmiFrame.medical_menu_item, "MenuItem", invalid_states=["showing"])
#statesCheck(tsmiFrame.new_menu_item, "MenuItem", invalid_states=["showing"])
#statesCheck(tsmiFrame.open_menu_item, "MenuItem", invalid_states=["showing"])
#statesCheck(tsmiFrame.copy_this_menu_item, "MenuItem", invalid_states=["showing"])
#statesCheck(tsmiFrame.paste_that_menu_item, "MenuItem", invalid_states=["showing"])

# use the action interface to open the "view" menu and click on "create"
tsmiFrame.view_menu.click(log=True)
sleep(config.SHORT_DELAY)
tsmiFrame.create_menu_item.click(log=True)
sleep(config.SHORT_DELAY)
tsmiFrame.assertMessageBoxAppeared("Create Clicked")
# press enter to close the dialog
tsmiFrame.keyCombo("Enter", grabFocus=False)

# make sure the text area is blank like we expect
tsmiFrame.assertText("")

# use accelerator to access menu
tsmiFrame.keyCombo("<Alt>V", grabFocus=False)
sleep(config.SHORT_DELAY)
tsmiFrame.keyCombo("Enter", grabFocus=False)
sleep(config.SHORT_DELAY)
tsmiFrame.assertMessageBoxAppeared("Create Clicked")
# press enter to close the dialog
tsmiFrame.keyCombo("Enter", grabFocus=False)

# make sure the text area is blank like we expect
tsmiFrame.assertText("")

tsmiFrame.keyCombo("<Alt>V", grabFocus=False)
sleep(config.SHORT_DELAY)
tsmiFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
tsmiFrame.keyCombo("Enter", grabFocus=False)
sleep(config.SHORT_DELAY)
tsmiFrame.assertText("Write Clicked\n")
sleep(config.SHORT_DELAY)

tsmiFrame.clearTextArea()
sleep(config.SHORT_DELAY)

tsmiFrame.help_menu.click(log=True)
sleep(config.SHORT_DELAY)
tsmiFrame.financial_menu_item.click(log=True)
sleep(config.SHORT_DELAY)
tsmiFrame.assertText("Have some money\n")
sleep(config.SHORT_DELAY)

tsmiFrame.clearTextArea()
sleep(config.SHORT_DELAY)

tsmiFrame.keyCombo("<Alt>H", grabFocus=False)
sleep(config.SHORT_DELAY)
tsmiFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
tsmiFrame.keyCombo("Enter", grabFocus=False)
sleep(config.SHORT_DELAY)
tsmiFrame.assertText("Here is a bandage\n")
sleep(config.SHORT_DELAY)

tsmiFrame.clearTextArea()
sleep(config.SHORT_DELAY)

# click on the "Copy This" menu item without opening its parent menu
tsmiFrame.copy_this_menu_item.click(log=True)
# Bug 501074 - Cannot perform action on accessible that is not visible
# tsmiFrame.assertText("Copy Clicked\n")

# clear out the text area by using the keyboard to navigate to "New"
tsmiFrame.keyCombo("<Alt>F", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG503725 - Menu item loses "focusable" state when that item becomes focused
#statesCheck(tsmiFrame.new_menu_item,
#            "MenuItem",
#            add_states=[FOCUSED, SELECTED])
tsmiFrame.keyCombo("Enter", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG503735 Selected state of a menu item persists when the menu item is
# clicked.  This bug causes the following statesCheck to have an extraneous
# "selected" state
#statesCheck(tsmiFrame.new_menu_item, "MenuItem")
#tsmiFrame.assertText("")

# select the File -> Open menu item and check the states along the way
tsmiFrame.keyCombo("<Alt>F", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG503725 - Menu item loses "focusable" state when that item becomes focused
#statesCheck(tsmiFrame.new_menu_item,
#            "MenuItem",
#            add_states=[FOCUSED, SELECTED])
tsmiFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG503735 Selected state of a menu item persists when the menu item is
# clicked.  Enter was pressed for the "New" menu item above, so it still has
# the selected state here.
# statesCheck(tsmiFrame.new_menu_item, "MenuItem")
# BUG503725 Menu item loses "focusable" state when that item becomes focused
#statesCheck(tsmiFrame.open_menu_item,
#            "MenuItem",
#            add_states=[FOCUSED, SELECTED])
tsmiFrame.keyCombo("Enter", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG503735 Selected state of a menu item persists when the menu item is
# clicked.  This bug causes the following statesCheck to have an extraneous
# "selected" state
#statesCheck(tsmiFrame.open_menu_item, "MenuItem")
# close the winow that opens
tsmiFrame.keyCombo("Enter", grabFocus=False)

# use mouseClicks to click on "Paste That", then check the text area text
# BUG503973 pyatspi.generateMouseEvent does not open menu when application
# window is active
#tsmiFrame.edit_menu.mouseClick()
#sleep(config.SHORT_DELAY)
#tsmiFrame.paste_that_menu_item.mouseClick()
#sleep(config.SHORT_DELAY)
# BUG486335 MenuItem, ToolStripMenuItem: extraneous "showing" state of
# menu item when it is not showing.  This means that calling assertEditMenuOpen
# will always succeed while this bug is open
#tsmiFrame.assertEditMenuOpen()
# BUG503973 
#tsmiFrame.assertText("Paste Clicked\n")

sleep(config.SHORT_DELAY)
tsmiFrame.clearTextArea()
tsmiFrame.keyCombo("<Alt>E", grabFocus=False)
sleep(config.SHORT_DELAY)

# use the keyboard to navigate to "Paste That" and then press Enter, then
# check the text area text
# BUG503725 - Menu item loses "focusable" state when that item becomes focused
#statesCheck(tsmiFrame.copy_this_menu_item,
#            "MenuItem",
#            add_states=[FOCUSED, SELECTED])
tsmiFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
# the copy menu should go back to the default states
statesCheck(tsmiFrame.copy_this_menu_item, "MenuItem")
# BUG503725 - Menu item loses "focusable" state when that item becomes focused
#statesCheck(tsmiFrame.paste_this_menu_item,
#            "MenuItem",
#            add_states=[FOCUSED, SELECTED])
tsmiFrame.keyCombo("Enter", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG503735 Selected state of a menu item persists when the menu item is
# clicked.  This bug causes the following statesCheck to have an extraneous
# "selected" state
#statesCheck(tsmiFrame.paste_this_menu_item, "MenuItem")
tsmiFrame.assertText("Paste Clicked\n")

# TODO: when some of these bugs are cleared up (especially BUG503973 or 
# BUG503663), we should write some more tests for the "View" and "Help" 
# menus on the ToolStrip

#close main window
tsmiFrame.quit()
