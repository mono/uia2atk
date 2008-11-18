#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/03/2008
# Description: Test accessibility of listview widget 
#              Use the listviewframe.py wrapper script
#              Test the samples/ListView_list.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of listview_list widget
"""

# imports
import sys
import os

from strongwind import *
from listview_list import *
from helpers import *
from sys import argv
from os import path

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

#check ListView item's actions list
actionsCheck(lvFrame.listitem[0], "List")

#check ListView's states list
statesCheck(lvFrame.list, "List")

#check ListItem0,1's default states
statesCheck(lvFrame.listitem[0], "ListItem", add_states=["focused"])
statesCheck(lvFrame.listitem[1], "ListItem")

#click listitem to rise selected states, listitem1 also with 
#selected states after click listitem3 because MultiSelect is True
lvFrame.click(lvFrame.listitem[1])
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.listitem[1], "ListItem", add_states=["selected"])

lvFrame.click(lvFrame.listitem[3])
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.listitem[3], "ListItem", add_states=["selected"])

statesCheck(lvFrame.listitem[1], "ListItem", add_states=["selected"])

#close MultiSelect, then select listitems to check states, 
#listitem1 disappear selected state
lvFrame.checkbox.click()
sleep(config.SHORT_DELAY)
lvFrame.click(lvFrame.listitem[2])
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.listitem[2], "ListItem", add_states=["selected"])

lvFrame.click(lvFrame.listitem[4])
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.listitem[4], "ListItem", add_states=["selected"])

statesCheck(lvFrame.listitem[1], "ListItem")

#mouse click ListItem to rise focused and selected states
lvFrame.mouseClick(log=False)
lvFrame.listitem[0].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.listitem[0], "ListItem", add_states=["focused", "selected"])

lvFrame.listitem[4].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.listitem[4], "ListItem", add_states=["focused", "selected"])
#listitem0 with default states after click listitem5
statesCheck(lvFrame.listitem[0], "ListItem")

lvFrame.keyCombo("Up", grabFocus=False)
statesCheck(lvFrame.listitem[3], "ListItem", add_states=["focused", "selected"])

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

#check listitem's text implementation
lvFrame.assertText(lvFrame.listitem[0], "Item 0")
lvFrame.assertText(lvFrame.listitem[4], "Item 4")

#check list's table implementation
lvFrame.assertTable(lvFrame.list)

#close application frame window
lvFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
