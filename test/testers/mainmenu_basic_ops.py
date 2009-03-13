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
actionsCheck(mmFrame.mainmenu, "Menu")
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
# check states of mainmenu
statesCheck(mmFrame.mainmenu, "MainMenu")
statesCheck(mmFrame.menuitem_file, "Menu", add_states=["focusable"])
statesCheck(mmFrame.menuitem_file_new, "Menu", add_states=["focusable"])
statesCheck(mmFrame.menuitem_file_new_doc, "MenuItem", add_states=["focusable"])
statesCheck(mmFrame.menuitem_file_open, "MenuItem", add_states=["focusable"])
statesCheck(mmFrame.menuitem_file_exit, "MenuItem", add_states=["focusable"])
statesCheck(mmFrame.menuitem_edit, "Menu", add_states=["focusable"])
statesCheck(mmFrame.menuitem_edit_undo, "MenuItem", add_states=["focusable"])
statesCheck(mmFrame.menuitem_edit_redo, "MenuItem", add_states=["focusable"])
statesCheck(mmFrame.menuitem_help, "Menu", add_states=["focusable"])
statesCheck(mmFrame.menuitem_help_about, "MenuItem", add_states=["focusable"])

##############################
# check MainMenu's AtkSelection
##############################
mmFrame.assertSelectChild(mmFrame.mainmenu, 0)
# BUG476362
statesCheck(mmFrame.menuitem_file, "MenuItem", add_states=["focusable", "selected"])
statesCheck(mmFrame.menuitem_edit, "MenuItem", add_states=["focusable"])

mmFrame.assertSelectChild(mmFrame.mainmenu, 1)
statesCheck(mmFrame.menuitem_file, "MenuItem", add_states=["focusable"])
# BUG476362
statesCheck(mmFrame.menuitem_edit, "MenuItem", add_states=["focusable", "selected"])

##############################
# check MainMenu and its children's AtkComponent
##############################
# BUG476878
mmFrame.menuitem_file.mouseClick()
sleep(config.SHORT_DELAY)
mmFrame.assertText(mmFrame.label, "You are clicking &File")

mmFrame.menuitem_edit.mouseClick()
sleep(config.SHORT_DELAY)
mmFrame.assertText(mmFrame.label, "You are clicking &Edit")

##############################
# End
##############################
# close application frame window
mmFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
