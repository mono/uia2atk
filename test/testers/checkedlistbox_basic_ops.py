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

########Check Action
#check listitem's actions
actionsCheck(clbFrame.listitem[0], "ListItem")
actionsCheck(clbFrame.listitem[20], "ListItem")

########Check States after doing Click, Taggle, mouseClick, keyCombo
#check list's states
statesCheck(clbFrame.listbox1, "List")
statesCheck(clbFrame.listbox2, "List")

#check default states for ListItem 0 which in "CheckOnClick is True" list
statesCheck(clbFrame.listitem[0], "ListItem")
#check default states for ListItem 20 which in "CheckOnClick is False" list
#with focused
statesCheck(clbFrame.listitem[20], "ListItem", add_states=["focused"])

#use keyCombo with "Tab" to change the focus from listitem 20 to listitem 0
clbFrame.keyCombo("Tab", grabFocus = False)
statesCheck(clbFrame.listitem[0], "ListItem", add_states=["focused"])

statesCheck(clbFrame.listitem[20], "ListItem")

#click listitem 1 to rise selected state
clbFrame.click(clbFrame.listitem[1])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[1], "ListItem", add_states=["focused", "selected"])

#use keyCombo with "Tab" and "Space" key to focus and check listitem20
clbFrame.keyCombo("Tab", grabFocus = False)
clbFrame.keyCombo("space", grabFocus = False)
statesCheck(clbFrame.listitem[20], "ListItem", add_states=["focused", "checked"])
#press "space" again to uncheck but still focused
clbFrame.keyCombo("space", grabFocus = False)
statesCheck(clbFrame.listitem[20], "ListItem", add_states=["focused"])

#click listitem 21 to rise selected state, because focus has moved to list2 so 
#click action also would rise focused state for listitem
clbFrame.click(clbFrame.listitem[21])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[21], "ListItem", add_states=["focused", "selected"])

#toggle to check/uncheck listitem2 which wouldn't rise selected state
clbFrame.toggle(clbFrame.listitem[2])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[2], "ListItem", add_states=["checked"])

clbFrame.toggle(clbFrame.listitem[2])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[2], "ListItem")

#toggle to check/uncheck listitem22
clbFrame.toggle(clbFrame.listitem[22])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[22], "ListItem", add_states=["checked"])

clbFrame.toggle(clbFrame.listitem[22])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[22], "ListItem")

#mouse click listitem 3 to focused, checked and selected
clbFrame.mouseClick(log=False)
clbFrame.listitem[3].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[3], "ListItem", add_states=["focused", "selected", "checked"])
#mouse click listitem 3 again to uncheck it, but still with focused and selected
clbFrame.listitem[3].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[3], "ListItem", add_states=["focused", "selected"])

#mouse click listitem 23 to selected
clbFrame.listitem[23].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[23], "ListItem", add_states=["focused", "selected"])
#mouse click listitem 23 again to checked, still with focused and selected
clbFrame.listitem[23].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[23], "ListItem", add_states=["focused", "selected", "checked"])
#mouse click listitem 23 again to uncheck it, still with focused and selected
clbFrame.listitem[23].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[23], "ListItem", add_states=["focused", "selected"])

########check list selection implementation
#select item by childIndex to rise selected state
clbFrame.assertSelectionChild(clbFrame.listbox1, 0)
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[0], "ListItem", add_states=["selected"])

clbFrame.assertSelectionChild(clbFrame.listbox2, 0)
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[20], "ListItem", add_states=["focused", "selected"])

#clear selection to get rid of selected state
clbFrame.assertClearSelection(clbFrame.listbox1)
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[0], "ListItem")

clbFrame.assertClearSelection(clbFrame.listbox2)
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[20], "ListItem", add_states=["focused"])

########assert label's text to check event by mouseClick and Toggle action
#check listitem 2 to change label to "Item 2 : Checked"
clbFrame.listitem[2].mouseClick()
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(2, '2')
#multi check listitem 5 to change label to "Item 2 5 : Checked"
clbFrame.listitem[5].mouseClick()
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(5, '2 5')
#selecte listitem 20 by click one time to change label to "Item : Checked"
clbFrame.listitem[20].mouseClick()
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(20, '20')
#selecte listitem 22 by click one time to change label to "Item : Checked"
clbFrame.listitem[22].mouseClick()
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(22, '22')
#check listitem 20 by click second time to change label to "Item 22 : Checked"
clbFrame.listitem[22].mouseClick()
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(22, '22')

#toggle action would update the label text to "Item 0 2 5 : Checked"
clbFrame.toggle(clbFrame.listitem[0])
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(0, '0 2 5')
#toggle again to delete 0 from label text to be "Item 2 5 : Checked"
clbFrame.toggle(clbFrame.listitem[0])
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(0, '2 5')

#listitem in list2 should be selected by click then toggle action can check it
clbFrame.click(clbFrame.listitem[20])
clbFrame.toggle(clbFrame.listitem[20])
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(20, '20 22')
#toggle again to delete 20 from label text to be "Item 22 : Checked"
clbFrame.toggle(clbFrame.listitem[20])
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(20, '22')


#close application frame window
clbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
