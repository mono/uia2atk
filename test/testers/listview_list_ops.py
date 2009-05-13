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
import pyatspi
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
for i in range(5):
    actionsCheck(lvFrame.tablecell[i], "TableCell")

##############################
# check treetable's AtkAccessible
##############################
statesCheck(lvFrame.treetable, "TreeTable", add_states=["focused"])

##############################
# check tablecell's AtkAccessible
##############################
statesCheck(lvFrame.tablecell[0], "TableCell", add_states=["focused", "editable"])
statesCheck(lvFrame.tablecell[3], "TableCell", add_states=["editable"])
statesCheck(lvFrame.tablecell[4], "TableCell", add_states=["editable"])

##############################
# check tablecell's AtkAccessible while multi-items selected
##############################
# click tablecell0 to rise selected states, tablecell4 also with 
# selected states after Ctrl+click tablecell4 because MultiSelect is True
lvFrame.click(lvFrame.tablecell[0])
sleep(config.SHORT_DELAY)
# TODO: BUG487118 focused is missing and doesn't send event to change label 
# when perform click from accerciser
#lvFrame.assertText(lvFrame.label, "Items are: Item 0 ")
statesCheck(lvFrame.tablecell[0], "TableCell", add_states=["selected", "focused", "editable"])
statesCheck(lvFrame.tablecell[4], "TableCell", add_states=["editable"])

pyatspi.Registry.generateKeyboardEvent(37, None, pyatspi.KEY_PRESS)
sleep(config.SHORT_DELAY)
lvFrame.click(lvFrame.tablecell[4])
pyatspi.Registry.generateKeyboardEvent(37, None, pyatspi.KEY_RELEASE)
sleep(config.SHORT_DELAY)
# TODO: BUG487118 
#lvFrame.assertText(lvFrame.label, "Items are: Item 0 Item 4 ")
#statesCheck(lvFrame.tablecell[4], "TableCell", add_states=["selected", "focused",  "editable"])
statesCheck(lvFrame.tablecell[0], "TableCell", add_states=["selected", "focused", "editable"])

##############################
# check tablecell's AtkAccessible while single-item selected
##############################
# close MultiSelect, then select tablecells to check states, 
lvFrame.checkbox.click()
sleep(config.SHORT_DELAY)

lvFrame.click(lvFrame.tablecell[1])
sleep(config.SHORT_DELAY)
# TODO: BUG487118
#lvFrame.assertText(lvFrame.label, "Items are: Item 1 ")
#statesCheck(lvFrame.tablecell[1], "TableCell", add_states=["selected", "focused",  "editable"])

lvFrame.click(lvFrame.tablecell[2])
sleep(config.SHORT_DELAY)
# TODO: BUG487118
#lvFrame.assertText(lvFrame.label, "Items are: Item 2 ")
#statesCheck(lvFrame.tablecell[2], "TableCell", add_states=["selected", "focused",  "editable"])
statesCheck(lvFrame.tablecell[1], "TableCell", add_states=["editable"])

##############################
# check tablecell's AtkAccessible by mouseClick
##############################
# mouse click TableCell to rise focused and selected states
lvFrame.mouseClick(log=False)
lvFrame.tablecell[0].mouseClick()
sleep(config.SHORT_DELAY)
lvFrame.assertText(lvFrame.label, "Items are: Item 0 ")
statesCheck(lvFrame.tablecell[0], "TableCell", add_states=["selected", "focused", "editable"])

lvFrame.tablecell[4].mouseClick()
sleep(config.SHORT_DELAY)
lvFrame.assertText(lvFrame.label, "Items are: Item 4 ")
statesCheck(lvFrame.tablecell[4], "TableCell", add_states=["selected", "focused", "editable"])
statesCheck(lvFrame.tablecell[0], "TableCell", add_states=["editable"])

##############################
# check tablecell's AtkAccessible by keyboard
##############################
lvFrame.keyCombo("Up", grabFocus=False)
statesCheck(lvFrame.tablecell[3], "TableCell", add_states=["focused", "selected", "editable"])
statesCheck(lvFrame.tablecell[4], "TableCell", add_states=["editable"])

lvFrame.keyCombo("Down", grabFocus=False)
statesCheck(lvFrame.tablecell[4], "TableCell", add_states=["focused", "selected", "editable"])
statesCheck(lvFrame.tablecell[3], "TableCell", add_states=["editable"])

##############################
# check tablecell's AtkSelection
##############################
# single selected
lvFrame.selectChild(lvFrame.treetable, lvFrame.tablecell[2].getIndexInParent())
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.tablecell[2], "TableCell", add_states=["selected", "editable"])

# clear selection
lvFrame.clearSelection(lvFrame.treetable)
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.tablecell[2], "TableCell", add_states=["editable"])
statesCheck(lvFrame.treetable, "TreeTable", add_states=["focused"])

# multi selected
lvFrame.checkbox.click()
sleep(config.SHORT_DELAY)

lvFrame.selectChild(lvFrame.treetable, lvFrame.tablecell[0].getIndexInParent())
lvFrame.selectChild(lvFrame.treetable, lvFrame.tablecell[3].getIndexInParent())
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.tablecell[0], "TableCell", add_states=["selected", "editable"])
statesCheck(lvFrame.tablecell[3], "TableCell", add_states=["selected", "editable"])

# clear selection
lvFrame.clearSelection(lvFrame.treetable)
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.tablecell[0], "TableCell", add_states=["editable"])
statesCheck(lvFrame.tablecell[3], "TableCell", add_states=["editable"])
statesCheck(lvFrame.treetable, "TreeTable", add_states=["focused"])

##############################
# check tablecell's AtkText
##############################
# check tablecell's text implementation
lvFrame.assertText(lvFrame.tablecell[0], "Item 0")
lvFrame.assertText(lvFrame.tablecell[4], "Item 4")

##############################
# check tablecell's LabelEdit
##############################
lvFrame.tablecell[4].deleteText()
sleep(config.SHORT_DELAY)
lvFrame.tablecell[4].insertText("Item 99")
sleep(config.SHORT_DELAY)
lvFrame.assertText(lvFrame.tablecell[4], "Item 99")

lvFrame.changeText(lvFrame.tablecell[3], "Item 99")
sleep(config.SHORT_DELAY)
lvFrame.assertText(lvFrame.tablecell[3], "Item 99")

##############################
# check tablecell's AtkTable
##############################
# check treetable's table implementation
lvFrame.assertTable(lvFrame.treetable, 5, 1)

##############################
# End
##############################
# close application frame window
lvFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
