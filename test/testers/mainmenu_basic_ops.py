#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/18/2008
# Description: main test script of mainmenu
#              ../samples/mainmenu.py is the test sample script
#              mainmenu/* is the wrapper of mainmenu test sample script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of MainMenu widget
"""

# imports
from mainmenu import *
from helpers import *
from states import *
from sys import argv

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass #expected

# open the mainmenu sample application
try:
    app = launchMainMenu(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
mmFrame = app.mainMenuFrame

##############################
# check MainMenu and its children's AtkAccessible
##############################
actionsCheck(mmFrame.menuitem_file, "Menu")
actionsCheck(mmFrame.menuitem_file_new, "Menu")
actionsCheck(mmFrame.menuitem_file_new_doc, "MenuItem")
actionsCheck(mmFrame.menuitem_file_open, "MenuItem")
actionsCheck(mmFrame.menuitem_file_exit, "MenuItem")
actionsCheck(mmFrame.menuitem_edit, "Menu")
actionsCheck(mmFrame.menuitem_edit_undo, "MenuItem")
actionsCheck(mmFrame.menuitem_edit_redo, "MenuItem")
actionsCheck(mmFrame.menuitem_help, "Menu")
actionsCheck(mmFrame.menuitem_help_about, "MenuItem")

##############################
# check MainMenu and its children's AtkAccessible
##############################
mmFrame.inputText(mmFrame.menuitem_file, "test")
sleep(config.SHORT_DELAY)
mmFrame.assertText(mmFrame.menuitem_file, "File")

mmFrame.inputText(mmFrame.menuitem_file_new, "test")
sleep(config.SHORT_DELAY)
mmFrame.assertText(mmFrame.menuitem_file_new, "New")

mmFrame.inputText(mmFrame.menuitem_file_new_doc, "test")
sleep(config.SHORT_DELAY)
mmFrame.assertText(mmFrame.menuitem_file_new_doc, "Document")

##############################
# check MainMenu and its children's AtkAccessible
##############################
# check states of mainmenu
# TODO: BUG485515
#statesCheck(mmFrame.mainmenu, "MainMenu")
#statesCheck(mmFrame.menuitem_file, "Menu")
#statesCheck(mmFrame.menuitem_file_new, "Menu")
#statesCheck(mmFrame.menuitem_file_new_doc, "MenuItem")
#statesCheck(mmFrame.menuitem_file_open, "MenuItem")
#statesCheck(mmFrame.menuitem_file_exit, "MenuItem")
#statesCheck(mmFrame.menuitem_edit, "Menu")
#statesCheck(mmFrame.menuitem_edit_undo, "MenuItem")
#statesCheck(mmFrame.menuitem_edit_redo, "MenuItem")
#statesCheck(mmFrame.menuitem_help, "Menu")
#statesCheck(mmFrame.menuitem_help_about, "MenuItem")

##############################
# check MainMenu(menu bar)'s AtkSelection
##############################
mmFrame.selectChild(mmFrame.mainmenu, 0)
sleep(config.SHORT_DELAY)
# TODO: BUG476362, BUG476362, BUG485524
#statesCheck(mmFrame.menuitem_file, "Menu", add_states=["selected", "focused"])
#statesCheck(mmFrame.menuitem_edit, "Menu")

mmFrame.selectChild(mmFrame.mainmenu, 1)
sleep(config.SHORT_DELAY)
# TODO: BUG476362, BUG476362, BUG485524
#statesCheck(mmFrame.menuitem_file, "Menu")
#statesCheck(mmFrame.menuitem_edit, "Menu", add_states=["selected", "focusable"])

##############################
# check menu and menu item's AtkComponent
##############################
# check menu
# BUG476878
# TODO: BUG476362, BUG485524
mmFrame.menuitem_file.mouseClick()
sleep(config.SHORT_DELAY)
#statesCheck(mmFrame.menuitem_file, "Menu", add_states=["selected", "focused"])

mmFrame.menuitem_edit.mouseClick()
sleep(config.SHORT_DELAY)
#statesCheck(mmFrame.menuitem_edit, "Menu", add_states=["selected", "focusable"])

# TODO: BUG476362, BUG476362, BUG485524
# check menu Selection
mmFrame.selectChild(mmFrame.menuitem_file, 0)
sleep(config.SHORT_DELAY)
#statesCheck(mmFrame.menuitem_file_new, "MenuItem", add_states=["selected"])

mmFrame.selectChild(mmFrame.menuitem_edit, 1)
sleep(config.SHORT_DELAY)
#statesCheck(mmFrame.menuitem_edit_redo, "MenuItem", add_states=["selected"])

mmFrame.selectChild(mmFrame.menuitem_file_new, 0)
sleep(config.SHORT_DELAY)
#statesCheck(mmFrame.menuitem_file_new_doc, "MenuItem", add_states=["selected"])

# check menu item
mmFrame.click(mmFrame.menuitem_file)
sleep(config.SHORT_DELAY)
#statesCheck(mmFrame.menuitem_file, "Menu", add_states=["selected", "focused"])

mmFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
#statesCheck(mmFrame.menuitem_file_new, "MenuItem", add_states=["selected", "focused"])

mmFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
#statesCheck(mmFrame.menuitem_file_new_doc, "MenuItem", add_states=["selected", "focused"])
#statesCheck(mmFrame.menuitem_file_new, "MenuItem")

mmFrame.menuitem_file_new_doc.mouseClick()
sleep(config.SHORT_DELAY)
# TODO: BUG476362
#mmFrame.assertText(mmFrame.label, "You are clicking Document")
# focused state is remove when you have clicked on a menu item
#statesCheck(mmFrame.menuitem_file_new_doc, "MenuItem", add_states=["selected"])

mmFrame.menuitem_edit_undo.mouseClick()
sleep(config.SHORT_DELAY)
# TODO: BUG476362
#mmFrame.assertText(mmFrame.label, "You are clicking Undo")
#statesCheck(mmFrame.menuitem_file_new_doc, "MenuItem", add_states=["selected"])

##############################
# End
##############################
# close application frame window
mmFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
