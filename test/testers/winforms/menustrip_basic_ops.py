#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        11/19/2008
# Description: main test script of menustrip
#              ../samples/winforms/menustrip.py is the test sample script
#              menustrip/* is the wrapper of menustrip test sample script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of MenuStrip widget
"""

# imports
from menustrip import *
from helpers import *
from states import *
from sys import argv
import sys

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the menustrip sample application
try:
  app = launchMenuStrip(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
msFrame = app.menuStripFrame

##############################
# check Text of menu items
##############################
actionsCheck(msFrame.menuitem_file, "Menu")
actionsCheck(msFrame.menuitem_file_new, "Menu")
actionsCheck(msFrame.menuitem_file_new_doc, "MenuItem")
actionsCheck(msFrame.menuitem_file_open, "MenuItem")
actionsCheck(msFrame.menuitem_edit, "Menu")
actionsCheck(msFrame.menuitem_edit_copy, "MenuItem")
actionsCheck(msFrame.menuitem_edit_paste, "MenuItem")

##############################
# ensure that menu items do not have editable text
##############################
msFrame.assertUneditableText(msFrame.menuitem_file, "test")
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.menuitem_file, "File")

msFrame.assertUneditableText(msFrame.menuitem_file_new, "test")
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.menuitem_file_new, "New")

msFrame.assertUneditableText(msFrame.menuitem_file_new_doc, "test")
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.menuitem_file_new_doc, "Document")

##############################
# check the states of the MenuStrip and all its children
##############################
# check states of menustrip
statesCheck(msFrame.menustrip, "MenuStrip")
statesCheck(msFrame.menuitem_file, "Menu")
# BUG486335 - MenuItem, ToolStripMenuItem: extraneous "showing" state of menu
# item when it is not showing
# according to bnc485515, comment4, we have "File" menu without "focusable", 
# and "New" menu with "focusable"
#statesCheck(msFrame.menuitem_file_new, "Menu", invalid_states=["showing"], \
#                                               add_states=["focusable"])
#statesCheck(msFrame.menuitem_file_new_doc, "MenuItem", invalid_states=["showing"])
#statesCheck(msFrame.menuitem_file_open, "MenuItem", invalid_states=["showing"])
#statesCheck(msFrame.menuitem_edit_copy, "MenuItem", invalid_states=["showing"])
#statesCheck(msFrame.menuitem_edit_paste, "MenuItem", invalid_states=["showing"])
statesCheck(msFrame.menuitem_edit, "Menu")

##############################
# check MenuStrip's AtkSelection
##############################
msFrame.selectChild(msFrame.menustrip, 0)
sleep(config.SHORT_DELAY)
# depending on the resolution of some of the bugs in this test, we may
# not want the "focused" state to be added here.
statesCheck(msFrame.menuitem_file, "Menu", add_states=["selected", "focused"])
statesCheck(msFrame.menuitem_edit, "Menu")

msFrame.selectChild(msFrame.menustrip, 1)
sleep(config.SHORT_DELAY)
statesCheck(msFrame.menuitem_file, "Menu")
# depending on the resolution of some of the bugs in this test, we may
# not want the "focused" state to be added here.
statesCheck(msFrame.menuitem_edit, "Menu", add_states=["selected", "focused"])

msFrame.selectChild(msFrame.menuitem_edit, 1)
sleep(config.SHORT_DELAY)
statesCheck(msFrame.menuitem_edit_copy, "MenuItem")
# BUG503725 - Menu, Menu item loses "focusable" state when that item becomes focused
#statesCheck(msFrame.menuitem_edit_paste, "MenuItem", add_states=["selected", "focused"])

# Press the "Down" key to select/focus on the "Copy" menu item
msFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG507623 "select" state doesn't go away when selected w/ selectChild
#statesCheck(msFrame.menuitem_edit_paste, "MenuItem")
# BUG503725 - Menu, Menu item loses "focusable" state when that item becomes focused
#statesCheck(msFrame.menuitem_edit_copy, "MenuItem", add_states=["selected", "focused"])

# Press Enter while on the "Copy" menu item and check the states
msFrame.keyCombo("Enter", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG486335 - MenuItem, ToolStripMenuItem: extraneous "showing" state of menu
# item when it is not showing
#statesCheck(msFrame.menuitem_edit_paste,
#            "MenuItem",
#            invalid_states=["showing"])
#statesCheck(msFrame.menuitem_edit_copy,
#            "MenuItem",
#            invalid_states=["showing"],
#            add_states=["selected"])

msFrame.menustrip.mouseClick()
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.label, "You are clicking ")

# check menu Selection
msFrame.selectChild(msFrame.menuitem_file, 0)
sleep(config.SHORT_DELAY)
statesCheck(msFrame.menuitem_file_new, "Menu", add_states=["selected", "focusable"])

msFrame.selectChild(msFrame.menuitem_edit, 1)
sleep(config.SHORT_DELAY)
# BUG503725 - Menu, Menu item loses "focusable" state when that item becomes focused
# BUG508055 - MenuStrip: menu item gets "focused" state from selectChild when
# menu is collapsed
#statesCheck(msFrame.menuitem_edit_paste, "MenuItem", add_states=["selected", "focused"])

msFrame.selectChild(msFrame.menuitem_file_new, 0)
sleep(config.SHORT_DELAY)
# BUG503725 - Menu, Menu item loses "focusable" state when that item becomes focused
# BUG508055 - MenuStrip: menu item gets "focused" state from selectChild when
#statesCheck(msFrame.menuitem_file_new_doc, "MenuItem", add_states=["selected"])

# tests using keyboard navigation and the accessible action interface 
# TODO: it would be nice to add some mouseClick tests here once we figure
# out why pyatspi.generateMouseEvent is unreliable (see BUG503973)

# check menu item
msFrame.menuitem_file.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(msFrame.menuitem_file, "Menu", add_states=["selected", "focused"])

msFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG503725 - Menu, Menu item loses "focusable" state when that item becomes focused
#statesCheck(msFrame.menuitem_file_new, "Menu", add_states=["selected", "focused", "focusable"])

msFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG503725 - Menu, Menu item loses "focusable" state when that item becomes focused
#statesCheck(msFrame.menuitem_file_new_doc, "MenuItem", add_states=["selected", "focused"])
# Bug 508257 - MenuStrip: submenu retains "focused" state when a submenu item
# has focus
#statesCheck(msFrame.menuitem_file_new, "Menu", add_states=["selected", "focusable"])

msFrame.keyCombo("Enter", grabFocus=False)
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.label, "You are clicking Document")
# "selected" and "focused" states should be remove when you have clicked on a
# menu item
# BUG506959 "selected" state of a menu item persists when the menu item is
# clicked
#statesCheck(msFrame.menuitem_file_new_doc, "MenuItem")

msFrame.menuitem_edit.click(log=True)
sleep(config.SHORT_DELAY)
msFrame.menuitem_edit_copy.mouseClick()
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.label, "You are clicking Copy")
# BUG506959 "selected" state of a menu item persists when the menu item is
# clicked
#statesCheck(msFrame.menuitem_file_new_doc, "MenuItem")
#statesCheck(msFrame.menuitem_edit_copy, "MenuItem")

#############################
# check menu_items Action
#############################
# click New menu that is first layer of File menu
msFrame.menuitem_file_new.click(log=True)
sleep(config.SHORT_DELAY)
# BUG508998: click action doesn't work for menuitems if its top level menu is 
# contracted
#msFrame.assertText(msFrame.label, "You are clicking New")
# click action won't update New menu's states
# BUG506959 "selected" state of a menu item persists when the menu item is
# clicked
#statesCheck(msFrame.menuitem_file_new, "MenuItem")

# click Document menu_item that is second layer of File menu
msFrame.menuitem_file_new_doc.click(log=True)
sleep(config.SHORT_DELAY)
# BUG508998: click action doesn't work for menuitems if its top level menu is 
# contracted
#msFrame.assertText(msFrame.label, "You are clicking Document")
# click action won't update Document menu_item's states
# BUG506959 "selected" state of a menu item persists when the menu item is
# clicked
#statesCheck(msFrame.menuitem_file_new_doc, "MenuItem")

##############################
# check Image of menu items
##############################
# BUG486290 - MenuStrip: menu item's image is not implemented
#msFrame.assertImage(msFrame.menuitem_file_new_doc, 16, 16)

##############################
# End
##############################
# close application frame window
msFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
