#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        11/19/2008
# Description: main test script of menustrip
#              ../samples/menustrip.py is the test sample script
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
# check menu item's action
##############################
actionsCheck(msFrame.menuitem_file, "Menu")
actionsCheck(msFrame.menuitem_file_new, "Menu")
actionsCheck(msFrame.menuitem_file_new_doc, "MenuItem")
actionsCheck(msFrame.menuitem_file_open, "MenuItem")
actionsCheck(msFrame.menuitem_edit, "Menu")
actionsCheck(msFrame.menuitem_edit_copy, "MenuItem")
actionsCheck(msFrame.menuitem_edit_paste, "MenuItem")

##############################
# check menu item's Text
##############################
msFrame.inputText(msFrame.menuitem_file, "test")
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.menuitem_file, "File")

msFrame.inputText(msFrame.menuitem_file_new, "test")
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.menuitem_file_new, "New")

msFrame.inputText(msFrame.menuitem_file_new_doc, "test")
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.menuitem_file_new_doc, "Document")

##############################
# check MenuStrip and its children's AtkAccessible
##############################
# check states of menustrip
statesCheck(msFrame.menustrip, "MenuStrip")
statesCheck(msFrame.menuitem_file, "Menu")
statesCheck(msFrame.menuitem_file_new, "Menu", invalid_states=["showing"])
statesCheck(msFrame.menuitem_file_new_doc, "MenuItem", invalid_states=["showing"])
statesCheck(msFrame.menuitem_file_open, "MenuItem", invalid_states=["showing"])
statesCheck(msFrame.menuitem_edit, "Menu")
statesCheck(msFrame.menuitem_edit_copy, "MenuItem", invalid_states=["showing"])
statesCheck(msFrame.menuitem_edit_paste, "MenuItem", invalid_states=["showing"])

##############################
# check MenuStrip's AtkSelection
##############################
msFrame.selectChild(msFrame.menustrip, 0)
sleep(config.SHORT_DELAY)
# BUG476362, 485524
#statesCheck(msFrame.menuitem_file, "Menu", add_states=["selected", "focused"])
statesCheck(msFrame.menuitem_edit, "Menu")

msFrame.selectChild(msFrame.menustrip, 1)
sleep(config.SHORT_DELAY)
# BUG476362, 485524
statesCheck(msFrame.menuitem_file, "Menu")
#statesCheck(msFrame.menuitem_edit, "Menu", add_states=["selected", "focused"])

##############################
# check MenuStrip and its children's AtkComponent
##############################
# TODO: BUG476878
msFrame.menustrip.mouseClick()
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.label, "You are clicking ")

# check menu Selection
msFrame.selectChild(msFrame.menuitem_file, 0)
sleep(config.SHORT_DELAY)
#statesCheck(msFrame.menuitem_file_new, "MenuItem", add_states=["selected"])
msFrame.clearSelection(msFrame.menuitem_file_new)
sleep(config.SHORT_DELAY)

msFrame.selectChild(msFrame.menuitem_edit, 1)
sleep(config.SHORT_DELAY)
#statesCheck(msFrame.menuitem_edit_paste, "MenuItem", add_states=["selected"])
msFrame.clearSelection(msFrame.menuitem_file_new)
sleep(config.SHORT_DELAY)

msFrame.selectChild(msFrame.menuitem_file_new, 0)
sleep(config.SHORT_DELAY)
#statesCheck(msFrame.menuitem_file_new_doc, "MenuItem", add_states=["selected"])
msFrame.clearSelection(msFrame.menuitem_file_new)
sleep(config.SHORT_DELAY)

msFrame.menuitem_file.mouseClick()
sleep(config.SHORT_DELAY)
#statesCheck(msFrame.menuitem_file, "Menu", add_states=["selected", "focused"])

msFrame.menuitem_edit.mouseClick()
sleep(config.SHORT_DELAY)
#statesCheck(msFrame.menuitem_file, "Menu", add_states=["selected", "focused"])

# check menu item
msFrame.click(msFrame.menuitem_file)
sleep(config.SHORT_DELAY)
#statesCheck(msFrame.menuitem_file, "Menu", add_states=["selected", "focused"])

msFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
#statesCheck(msFrame.menuitem_file_new, "Menu", add_states=["selected", "focused"])

msFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
#statesCheck(msFrame.menuitem_file_new_doc, "MenuItem", add_states=["selected", "focused"])
#statesCheck(msFrame.menuitem_file_new, "Menu")

msFrame.menuitem_file_new_doc.mouseClick()
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.label, "You are clicking Document")
# focused state is remove when you have clicked on a menu item
#statesCheck(msFrame.menuitem_file_new_doc, "MenuItem", add_states=["selected"])

msFrame.click(msFrame.menuitem_edit)
sleep(config.SHORT_DELAY)
msFrame.menuitem_edit_copy.mouseClick()
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.label, "You are clicking Copy")
#statesCheck(msFrame.menuitem_file_new_doc, "MenuItem", add_states=["selected"])

##############################
# check menu item's Image
##############################
# TODO: BUG486290
#msFrame.assertImage(msFrame.menuitem_file_new_doc, 16, 16)

##############################
# End
##############################
# close application frame window
msFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
