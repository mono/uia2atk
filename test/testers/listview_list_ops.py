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
# check tablecell's AtkAction
##############################
actionsCheck(lvFrame.tablecell[0], "TableCell")

##############################
# check treetable's AtkAccessible
##############################
statesCheck(lvFrame.treetable, "TreeTable", add_states=["focused"])

##############################
# check tablecell's AtkAccessible
##############################
statesCheck(lvFrame.tablecell[0], "TableCell", add_states=["focused"])
statesCheck(lvFrame.tablecell[4], "TableCell")

##############################
# check tablecell's AtkAccessible while multi-items selected
##############################
#click tablecell to rise selected states, tablecell1 also with 
#selected states after click tablecell3 because MultiSelect is True
lvFrame.click(lvFrame.tablecell[0])
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.tablecell[0], "TableCell", add_states=["selected", "focused"])
statesCheck(lvFrame.tablecell[4], "TableCell")

lvFrame.click(lvFrame.tablecell[4])
sleep(config.SHORT_DELAY)
# FIXME: multi-click error
# TODO: BUG
#statesCheck(lvFrame.tablecell[4], "TableCell", add_states=["selected", "focused"])
statesCheck(lvFrame.tablecell[0], "TableCell", add_states=["selected", "focused"])
statesCheck(lvFrame.tablecell[4], "TableCell", add_states=["selected"])

##############################
# check tablecell's AtkAccessible while single-item selected
##############################
#close MultiSelect, then select tablecells to check states, 
lvFrame.checkbox.click()
sleep(config.SHORT_DELAY)

lvFrame.click(lvFrame.tablecell[1])
sleep(config.SHORT_DELAY)
# TODO: BUG, when multiselect is enabled, both tablecells should have "selected" state 
#statesCheck(lvFrame.tablecell[1], "TableCell", add_states=["selected", "focused"])
statesCheck(lvFrame.tablecell[1], "TableCell", add_states=["selected"])
statesCheck(lvFrame.tablecell[2], "TableCell")

lvFrame.click(lvFrame.tablecell[2])
sleep(config.SHORT_DELAY)
# TODO: BUG, when multiselect is enabled, both tablecells should have "selected" state 
#statesCheck(lvFrame.tablecell[2], "TableCell", add_states=["selected", "focused"])
statesCheck(lvFrame.tablecell[2], "TableCell", add_states=["selected"])
statesCheck(lvFrame.tablecell[1], "TableCell")

##############################
# check tablecell's AtkAccessible by mouseClick
##############################
#mouse click TableCell to rise focused and selected states
lvFrame.mouseClick(log=False)
lvFrame.tablecell[0].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.tablecell[0], "TableCell", add_states=["selected", "focused"])
statesCheck(lvFrame.tablecell[3], "TableCell")

lvFrame.tablecell[3].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.tablecell[3], "TableCell", add_states=["selected", "focused"])
statesCheck(lvFrame.tablecell[0], "TableCell")

##############################
# check tablecell's AtkAccessible by keyboard
##############################
lvFrame.keyCombo("Up", grabFocus=False)
statesCheck(lvFrame.tablecell[2], "TableCell", add_states=["focused", "selected"])

lvFrame.keyCombo("Down", grabFocus=False)
statesCheck(lvFrame.tablecell[3], "TableCell", add_states=["focused", "selected"])

##############################
# check tablecell's AtkSelection
##############################
#check treetable selection implementation
lvFrame.assertSelectionChild(lvFrame.treetable, 2)
sleep(config.SHORT_DELAY)
# TODO: BUG476065
#statesCheck(lvFrame.tablecell[2], "TableCell", add_states=["selected"])

#clear selection
lvFrame.assertClearSelection(lvFrame.treetable)
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.tablecell[2], "TableCell")
#tablecell3 still focused
statesCheck(lvFrame.tablecell[3], "TableCell", add_states=["focused"])
#check treetable state after clear selection
# TODO: BUG480218
#statesCheck(lvFrame.treetable, "TreeTable")

##############################
# check tablecell's AtkText
##############################
#check tablecell's text implementation
lvFrame.assertText(lvFrame.tablecell[0], "Item 0")
lvFrame.assertText(lvFrame.tablecell[4], "Item 4")

##############################
# check tablecell's LabelEdit
##############################
# TODO: should edit list items' text from GUI

# TODO: BUG481456 could not edit in "Text(Editable)"
#lvFrame.inputText(lvFrame.tablecell[3], "Item 99")
#lvFrame.assertText(lvFrame.tablecell[3], "Item 99")

##############################
# check tablecell's AtkTable
##############################
# check treetable's table implementation
lvFrame.assertTable(lvFrame.treetable, 5, 1)

##############################
# End
##############################
#close application frame window
lvFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
