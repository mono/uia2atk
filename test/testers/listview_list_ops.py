#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
#              Ray Wang <rawang@novell.com>
# Date:        12/13/2008
# Description: main test script of listview_list
#              ../samples/listview_list.py is the test sample script
#              listview_list/* are the wrappers of listview_list test sample
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of listview_list widget
"""

# imports
from listview_list import *
from helpers import *
from actions import *
from states import *
from sys import argv

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the listview_list sample application
try:
  app = launchListView(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
lvFrame = app.listViewFrame

##############################
# check listitem's AtkAction
##############################
actionsCheck(lvFrame.listitem[0], "ListItem")

##############################
# check list's AtkAccessible
##############################
statesCheck(lvFrame.list, "List", add_states=["focused"])

##############################
# check listitem's AtkAccessible
##############################
statesCheck(lvFrame.listitem[0], "ListItem", add_states=["focused"])
statesCheck(lvFrame.listitem[4], "ListItem")

##############################
# check listitem's AtkAccessible while multi-items selected
##############################
#click listitem to rise selected states, listitem1 also with 
#selected states after click listitem3 because MultiSelect is True
lvFrame.click(lvFrame.listitem[0])
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.listitem[0], "ListItem", add_states=["selected", "focused"])
statesCheck(lvFrame.listitem[4], "ListItem")

lvFrame.click(lvFrame.listitem[4])
sleep(config.SHORT_DELAY)
# FIXME: multi-click error
#statesCheck(lvFrame.listitem[4], "ListItem", add_states=["selected", "focused"])
#statesCheck(lvFrame.listitem[0], "ListItem", add_states=["selected"])

##############################
# check listitem's AtkAccessible while single-item selected
##############################
#close MultiSelect, then select listitems to check states, 
lvFrame.checkbox.click()
sleep(config.SHORT_DELAY)
lvFrame.click(lvFrame.listitem[1])
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.listitem[1], "ListItem", add_states=["selected", "focused"])
statesCheck(lvFrame.listitem[2], "ListItem")

lvFrame.click(lvFrame.listitem[2])
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.listitem[2], "ListItem", add_states=["selected", "focused"])
statesCheck(lvFrame.listitem[1], "ListItem")

##############################
# check listitem's AtkAccessible by mouseClick
##############################
#mouse click ListItem to rise focused and selected states
lvFrame.mouseClick(log=False)
lvFrame.listitem[0].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.listitem[0], "ListItem", add_states=["selected", "focused"])
statesCheck(lvFrame.listitem[3], "ListItem")

lvFrame.listitem[4].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.listitem[3], "ListItem", add_states=["selected", "focused"])
statesCheck(lvFrame.listitem[0], "ListItem")

##############################
# check listitem's AtkAccessible by keyboard
##############################
lvFrame.keyCombo("Up", grabFocus=False)
statesCheck(lvFrame.listitem[2], "ListItem", add_states=["focused", "selected"])

lvFrame.keyCombo("Down", grabFocus=False)
statesCheck(lvFrame.listitem[3], "ListItem", add_states=["focused", "selected"])

##############################
# check listitem's AtkSelection
##############################
#check list selection implementation
lvFrame.assertSelectionChild(lvFrame.list, 2)
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.listitem[2], "ListItem", add_states=["selected"])

#clear selection
lvFrame.assertClearSelection(lvFrame.list)
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.listitem[2], "ListItem")
#listitem3 still focused
statesCheck(lvFrame.listitem[3], "ListItem", add_states=["focused"])
#check listbox state after clear selection
statesCheck(lvFrame.list, "List")

##############################
# check listitem's AtkText
##############################
#check listitem's text implementation
lvFrame.assertText(lvFrame.listitem[0], "Item 0")
lvFrame.assertText(lvFrame.listitem[4], "Item 4")

##############################
# check listitem's LabelEdit
##############################
lvFrame.inputText(lvFrame.listitem[3], "Item 99")
lvFrame.assertText(lvFrame.listitem[3], "Item 99")

##############################
# check listitem's AtkTable
##############################
# check list's table implementation
lvFrame.assertTable(lvFrame.list, 4, 2)

##############################
# End
##############################
#close application frame window
lvFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
