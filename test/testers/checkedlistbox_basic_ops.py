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

##########################
##Check Action
##########################

# check listitem's actions
actionsCheck(clbFrame.listitem[0], "CheckedListItem")
actionsCheck(clbFrame.listitem[20], "CheckedListItem")

#############################################################
##Do Click, Taggle, mouseClick, keyCombo actions,
##then check states
#############################################################

# check list's states, list1 with focused
statesCheck(clbFrame.listbox1, "List", add_states=["focused"])
statesCheck(clbFrame.listbox2, "List")

# check default states for ListItem 0 which in "CheckOnClick is True" list
statesCheck(clbFrame.listitem[0], "CheckBoxListItem", add_states=["focused"])
# check default states for ListItem 20 which in "CheckOnClick is False" list
statesCheck(clbFrame.listitem[20], "CheckBoxListItem")

# use keyTab to change the focus from listbox1 to listbox2
clbFrame.keyCombo("Tab", grabFocus = False)
# BUG515457: treetable remain 'focused' state after focus is moved
#statesCheck(clbFrame.listbox1, "List")
statesCheck(clbFrame.listbox2, "List", add_states=["focused"])

# use keySpace to check listitem20
clbFrame.keyCombo("space", grabFocus = False)
statesCheck(clbFrame.listitem[20], "ListItem", add_states=["checked", "focused"])
# both listitem[20] and listbox2 have focused state that is similar to gtk.treeview
statesCheck(clbFrame.listbox2, "List", add_states=["focused"])
# press "space" again to uncheck but still focused
clbFrame.keyCombo("space", grabFocus = False)
statesCheck(clbFrame.listitem[20], "ListItem", add_states=["focused"])
# BUG515457: treetable remain 'focused' state after focus is moved
#statesCheck(clbFrame.listbox1, "List")

# do click for listitem 1 to rise selected state
clbFrame.click(clbFrame.listitem[1])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[1], "ListItem", add_states=["selected"])

# do click for listitem 21 to rise selected state
clbFrame.click(clbFrame.listitem[21])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[21], "ListItem", add_states=["focused", "selected"])

# do toggle to check/uncheck listitem2 which wouldn't rise selected state
clbFrame.toggle(clbFrame.listitem[2])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[2], "ListItem", add_states=["checked"])

clbFrame.toggle(clbFrame.listitem[2])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[2], "ListItem")

# do toggle to check/uncheck listitem22
clbFrame.toggle(clbFrame.listitem[22])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[22], "ListItem", add_states=["checked"])

clbFrame.toggle(clbFrame.listitem[22])
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[22], "ListItem")

# mouse click listitem 3 to rise checked and selected, focus to listbox1
clbFrame.mouseClick(log=False)
clbFrame.listitem[3].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[3], "ListItem", add_states=["focused", "selected", "checked"])
statesCheck(clbFrame.listbox1, "List", add_states=["focused"])
# mouse click listitem 3 again to uncheck it, but listbox1 still with focused
clbFrame.listitem[3].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[3], "ListItem", add_states=["focused","selected"])
statesCheck(clbFrame.listbox1, "List", add_states=["focused"])

# mouse click listitem 23 to selected, focus to listbox2
clbFrame.listitem[23].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[23], "ListItem", add_states=["focused", "selected"])
statesCheck(clbFrame.listbox2, "List", add_states=["focused"])
# mouse click listitem 23 again to checked
clbFrame.listitem[23].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[23], "ListItem", add_states=["focused", "selected", "checked"])
# mouse click listitem 23 again to uncheck it, listbox2 still with focused
clbFrame.listitem[23].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[23], "ListItem", add_states=["focused", "selected"])
statesCheck(clbFrame.listbox2, "List", add_states=["focused"])

###############################################
##check list selection implementation
###############################################
clbFrame.selectChildAndCheckStates(clbFrame.listbox1,
                                   clbFrame.listitem[0].getIndexInParent(),
                                   add_states=["selected"])

clbFrame.selectChildAndCheckStates(clbFrame.listbox1,
                                   clbFrame.listitem[3].getIndexInParent(),
                                   add_states=["selected"])

clbFrame.selectChildAndCheckStates(clbFrame.listbox2,
                                   clbFrame.listitem[20].getIndexInParent(),
                                   add_states=["focused", "selected"])

clbFrame.selectChildAndCheckStates(clbFrame.listbox2,
                                   clbFrame.listitem[22].getIndexInParent(),
                                   add_states=["focused", "selected"])

# clear selection to get rid of selected state, listbox2 with focused
clbFrame.clearAccessibleSelection(clbFrame.listbox1)
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[0], "ListItem")
# BUG515457: treetable remain 'focused' state after focus is moved
#statesCheck(clbFrame.listbox1, "List")

clbFrame.clearAccessibleSelection(clbFrame.listbox2)
sleep(config.SHORT_DELAY)
statesCheck(clbFrame.listitem[20], "ListItem")
statesCheck(clbFrame.listbox2, "List", add_states=["focused"])

#####################################################################
##mouseClick and Toggle listitem to change label's text
#####################################################################

# one time mouseClick listitems in listbox1 would change label's text
# check listitem 2 to change label to "Item 2 Checked"
clbFrame.listitem[2].mouseClick()
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.label1, "Item 2 Checked")
# multi check listitem 4 to change label to "Item 2 4 Checked"
clbFrame.listitem[4].mouseClick()
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.label1, "Item 2 4 Checked")
# make sure they both have the correct states after being clicked
statesCheck(clbFrame.listitem[2], "ListItem", add_states=["checked"])
statesCheck(clbFrame.listitem[4],
            "ListItem",
            add_states=["checked", "focused", "selected"])

# two times mouseClick listitems in listbox2 would change label's text
# selecte listitem 20 by mouseClick in one time doesn't change label text
clbFrame.listitem[20].mouseClick()
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.label2, newlabel="Item 23 Unchecked")
# Check listitem 20 by mouseClick in second time would change label text
clbFrame.listitem[20].mouseClick()
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.label2, newlabel="Item 20 Checked")
# Uncheck listitem 20 by mouseClick again would change label text
clbFrame.listitem[20].mouseClick()
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.label2, newlabel="Item 20 Unchecked")
# make sure they all have the correct states after being clicked
statesCheck(clbFrame.listitem[23], "ListItem")
statesCheck(clbFrame.listitem[20],
            "ListItem",
            add_states=["selected", "focused"])

# in listbox1 toggle action would rise checked but doesn't change label 
# but click action can change label because using SelectedIndexChanged event
clbFrame.toggle(clbFrame.listitem[0])
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.label1, "Item 2 4 Checked")

clbFrame.click(clbFrame.listitem[0])
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.label1, "Item 0 2 4 Checked")
# toggle again to unchecked listitem2.  The label will not change because the
# toggle action does not change it.
clbFrame.toggle(clbFrame.listitem[2])
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.label1, "Item 0 2 4 Checked")

# make sure they all have the correct states after being toggled
statesCheck(clbFrame.listitem[0],
            "ListItem",
            add_states=["checked", "selected"])
statesCheck(clbFrame.listitem[2], "ListItem")
statesCheck(clbFrame.listitem[4], "ListItem", add_states=["checked"])

# click action can change label
clbFrame.click(clbFrame.listitem[2])
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.label1, "Item 0 4 Checked")

# in listbox2 "toggle" action would check listitem and change label, 
# "click" action would selecte listitem but doesn't change label because using
# ItemCheck event
clbFrame.click(clbFrame.listitem[21])
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.label2, newlabel="Item 20 Unchecked")

clbFrame.toggle(clbFrame.listitem[21])
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.label2, newlabel="Item 21 Checked")

clbFrame.toggle(clbFrame.listitem[21])
sleep(config.SHORT_DELAY)
clbFrame.assertLabel(clbFrame.label2, newlabel="Item 21 Unchecked")

# close application frame window
clbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
