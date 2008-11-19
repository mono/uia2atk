#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/21/2008
# Description: Test accessibility of checkedlistbox widget 
#              Use the checkedlistboxframe.py wrapper script
#              Test the samples/checkedlistbox.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of checkedlistbox widget
"""

# imports
import sys
import os

from strongwind import *
from checkedlistbox import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the checkedlistbox sample application
try:
  app = launchCheckedListBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
clbFrame = app.checkedListBoxFrame

#check listitem's actions
actionsCheck(clbFrame.listitem[0], "ListItem")

#check list's states
statesCheck(clbFrame.listbox1, "List")
statesCheck(clbFrame.listbox2, "List")
#use Tab key to move focus, then check list's states
clbFrame.keyCombo("Tab")
statesCheck(clbFrame.listbox1, "List")
statesCheck(clbFrame.listbox2, "List")

#check default states for ListItem which CheckOnClick is True
statesCheck(clbFrame.listitem[0], "ListItem", add_states=["focused"])
#check default states for ListItem which CheckOnClick is False
statesCheck(clbFrame.listitem[20], "ListItem")

#mouse click listitems to change label to show which items are checked or 
#selected
clbFrame.listitem[0].mouseClick()
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.listitem[0], '0')

clbFrame.listitem[5].mouseClick()
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.listitem[5], '0 5')

clbFrame.listitem[20].mouseClick()
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.listitem[20], '20')

clbFrame.listitem[30].mouseClick()
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.listitem[30], '20 30')

#click listitem 1 to rise selected state
clbFrame.click(clbFrame.listitem[1])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[1], "ListItem", add_states=["selected"])

#click listitem 21 to rise selected state, because focus is moved to list2 so 
#click action also would rise focused state for listitem
clbFrame.click(clbFrame.listitem[21])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[21], "ListItem", add_states=["focused", "selected"])

#toggle listitem 2 to rise checked state, toggle action wouldn't rise selected state
clbFrame.toggle(clbFrame.listitem[2])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[2], "ListItem", add_states=["checked"])

#toggle listitem 22 to rise checked state
clbFrame.toggle(clbFrame.listitem[22])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[22], "ListItem", add_states=["checked"])

#mouse click listitem 3 to checked and selected
clbFrame.listitem[3].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[3], "ListItem", add_states=["focused", "selected", "checked"])

#mouse click listitem 25 to selected
clbFrame.listitem[25].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[25], "ListItem", add_states=["focused", "selected"])

#mouse click listitem 25 again to checked
clbFrame.listitem[25].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[25], "ListItem", add_states=["focused", "selected", "checked"])

#check list selection implementation
#select item by childIndex
clbFrame.assertSelectionChild(clbFrame.listbox1, 6)
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[6], "ListItem", add_states=["selected"])

clbFrame.assertSelectionChild(clbFrame.listbox2, 6)
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[26], "ListItem", add_states=["focused", "selected"])

#clear selection
clbFrame.assertClearSelection(clbFrame.listbox1)
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[6], "ListItem")

clbFrame.assertClearSelection(clbFrame.listbox2)
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[26], "ListItem", add_states=["focused"])

#toggle action would update the label text
clbFrame.toggle(clbFrame.listitem[10])
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.listitem[10], '0 2 3 5 10')

clbFrame.click(clbFrame.listitem[20])
clbFrame.toggle(clbFrame.listitem[20])
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.listitem[20], '2 5 20')

#check list's table implementation
clbFrame.assertTable(clbFrame.listbox1)
clbFrame.assertTable(clbFrame.listbox2, row=30, col=1)


#close application frame window
clbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
