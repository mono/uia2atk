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
# check menu item's action
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
# ensure that menu items do not have editable text
##############################
mmFrame.assertUneditableText(mmFrame.menuitem_file, "test")
sleep(config.SHORT_DELAY)
mmFrame.assertText(mmFrame.menuitem_file, "File")

mmFrame.assertUneditableText(mmFrame.menuitem_file_new, "test")
sleep(config.SHORT_DELAY)
mmFrame.assertText(mmFrame.menuitem_file_new, "New")

mmFrame.assertUneditableText(mmFrame.menuitem_file_new_doc, "test")
sleep(config.SHORT_DELAY)
mmFrame.assertText(mmFrame.menuitem_file_new_doc, "Document")

##############################
# check the states of the MainMenu and all its children
##############################
statesCheck(mmFrame.mainmenu, "MainMenu")
# BUG485515: MainMenu: extraneous "focusable" state on Menu and MenuItem
#statesCheck(mmFrame.menuitem_file, "Menu")
# BUG486335: extraneous "showing" state of menu item when it is not showing
#statesCheck(mmFrame.menuitem_file_new, "Menu", invalid_states=["showing"], \
#                                               add_states=["focusable"]))
#statesCheck(mmFrame.menuitem_file_new_doc, "MenuItem", \
#                                               invalid_states=["showing")
#statesCheck(mmFrame.menuitem_file_open, "MenuItem", \
#                                               invalid_states=["showing"])
#statesCheck(mmFrame.menuitem_file_exit, "MenuItem", \
#                                               invalid_states=["showing")
# BUG485515: MainMenu: extraneous "focusable" state on Menu and MenuItem
#statesCheck(mmFrame.menuitem_edit, "Menu")
# BUG486335: extraneous "showing" state of menu item when it is not showing
#statesCheck(mmFrame.menuitem_edit_undo, "MenuItem", \
#                                               invalid_states=["showing"])
#statesCheck(mmFrame.menuitem_edit_redo, "MenuItem", \
#                                               invalid_states=["showing"])
# BUG485515: MainMenu: extraneous "focusable" state on Menu and MenuItem
#statesCheck(mmFrame.menuitem_help, "Menu")
# BUG486335: extraneous "showing" state of menu item when it is not showing
#statesCheck(mmFrame.menuitem_help_about, "MenuItem", \
#                                               invalid_states=["showing"])

##############################
# check MainMenu(menu bar)'s AtkSelection
##############################
mmFrame.selectChild(mmFrame.mainmenu, 0)
sleep(config.SHORT_DELAY)
# BUG476362: menu item's "selected" state is missing when you select a menu item
#statesCheck(mmFrame.menuitem_file, "Menu", add_states=["selected", "focused"])
# BUG485515: MainMenu: extraneous "focusable" state on Menu and MenuItem
#statesCheck(mmFrame.menuitem_edit, "Menu")

mmFrame.selectChild(mmFrame.mainmenu, 1)
sleep(config.SHORT_DELAY)
# BUG485515: MainMenu: extraneous "focusable" state on Menu and MenuItem
#statesCheck(mmFrame.menuitem_file, "Menu")
# BUG476362: menu item's "selected" state is missing when you select a menu item
#statesCheck(mmFrame.menuitem_edit, "Menu", add_states=["selected", "focused"])

##############################
# check menu and menu item's AtkComponent
##############################
mmFrame.menuitem_file.mouseClick()
sleep(config.SHORT_DELAY)
# BUG476878: the component of its children are incorrect
# BUG476362: menu item's "selected" state is missing when you select a menu item
#statesCheck(mmFrame.menuitem_file, "Menu", add_states=["selected", "focused"])

mmFrame.menuitem_edit.mouseClick()
sleep(config.SHORT_DELAY)
# BUG476878: the component of its children are incorrect
# BUG476362: menu item's "selected" state is missing when you select a menu item
#statesCheck(mmFrame.menuitem_edit, "Menu", add_states=["selected", "focused"])

# check menu Selection
mmFrame.selectChild(mmFrame.menuitem_file, 0)
sleep(config.SHORT_DELAY)
# BUG476362: menu item's "selected" state is missing when you select a menu item
# BUG485515: MainMenu: extraneous "focusable" state on Menu and MenuItem
#statesCheck(mmFrame.menuitem_file_new, "Menu", \
#                                       add_states=["selected", "focusable"])

mmFrame.selectChild(mmFrame.menuitem_edit, 1)
sleep(config.SHORT_DELAY)
# BUG476362: menu item's "selected" state is missing when you select a menu item
#statesCheck(mmFrame.menuitem_edit_redo, "MenuItem", add_states=["selected"])

mmFrame.selectChild(mmFrame.menuitem_file_new, 0)
sleep(config.SHORT_DELAY)
# BUG476362: menu item's "selected" state is missing when you select a menu item
#statesCheck(mmFrame.menuitem_file_new_doc, "MenuItem", add_states=["selected"])

# check menu item
mmFrame.menuitem_file.click(log=True)
sleep(config.SHORT_DELAY)
# BUG476362: menu item's "selected" state is missing when you select a menu item
# BUG485515: MainMenu: extraneous "focusable" state on Menu and MenuItem
#statesCheck(mmFrame.menuitem_file, "Menu", \
#                               add_states=["selected", "focused", "focusable"])

mmFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG476362: menu item's "selected" state is missing when you select a menu item
#statesCheck(mmFrame.menuitem_file_new, "Menu", add_states=["selected", "focused"])

mmFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG476362: menu item's "selected" state is missing when you select a menu item
#statesCheck(mmFrame.menuitem_file_new_doc, "MenuItem", add_states=["selected", "focused"])
# BUG485515: MainMenu: extraneous "focusable" state on Menu and MenuItem
#statesCheck(mmFrame.menuitem_file_new, "Menu")

mmFrame.menuitem_file_new_doc.mouseClick()
sleep(config.SHORT_DELAY)
# BUG476362: menu item's "selected" state is missing when you select a menu item
#mmFrame.assertText(mmFrame.label, "You are clicking Document")
#statesCheck(mmFrame.menuitem_file_new_doc, "MenuItem", add_states=["selected"])

mmFrame.menuitem_edit_undo.mouseClick()
sleep(config.SHORT_DELAY)
# BUG476362: menu item's "selected" state is missing when you select a menu item
#mmFrame.assertText(mmFrame.label, "You are clicking Undo")
#statesCheck(mmFrame.menuitem_file_new_doc, "MenuItem", add_states=["selected"])

##############################
# End
##############################
# close application frame window
mmFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
