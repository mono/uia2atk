#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/11/2008
# Description: Test accessibility of listview widget 
#              Use the listviewframe.py wrapper script
#              Test the samples/ListView_largeimage.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of listview_largeimage widget
"""

# imports
import sys
import os

from strongwind import *
from listview_largeimage import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the listview_largeimage sample application
try:
  app = launchListView(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
lvliFrame = app.listViewLargeImageFrame

#################################################################
#check states and actions
#################################################################

#check ListView item's actions list
actionsCheck(lvliFrame.listitem[0], "ListItem")

#check ListView's states list
statesCheck(lvliFrame.list, "List", add_states=["focused"])

#check ListItem0,1's default states
statesCheck(lvliFrame.listitem[0], "ListItem")
statesCheck(lvliFrame.listitem[1], "ListItem")

#click listitem to rise selected and focused states, listitem1 also with 
#selected states after click listitem3 because MultiSelect is True
lvliFrame.click(lvliFrame.listitem[1])
sleep(config.SHORT_DELAY)
statesCheck(lvliFrame.listitem[1], "ListItem", add_states=["focused", "selected"])

lvliFrame.click(lvliFrame.listitem[3])
sleep(config.SHORT_DELAY)
statesCheck(lvliFrame.listitem[3], "ListItem", add_states=["focused", "selected"])

statesCheck(lvliFrame.listitem[1], "ListItem", add_states=["selected"])

#do toggle action to check/uncheck listitem
lvliFrame.toggle(lvliFrame.listitem[1])
sleep(config.SHORT_DELAY)
statesCheck(lvliFrame.listitem[1], "ListItem", \
                                    add_states=["checked", "focused", "selected"])

lvliFrame.toggle(lvliFrame.listitem[1])
sleep(config.SHORT_DELAY)
statesCheck(lvliFrame.listitem[1], "ListItem", \
                                    add_states=["focused", "selected"])

#mouse click ListItem to rise focused and selected states
lvliFrame.mouseClick(log=False)
lvliFrame.listitem[0].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvliFrame.listitem[0], "ListItem", add_states=["focused", "selected"])

lvliFrame.listitem[5].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvliFrame.listitem[5], "ListItem", add_states=["focused", "selected"])
#listitem0 with default states after click listitem5
statesCheck(lvliFrame.listitem[0], "ListItem")

####################################################################
#test toggle action to change label
####################################################################
#first time toggle to check item2
lvliFrame.toggle(lvliFrame.listitem[2])
sleep(config.SHORT_DELAY)
lvliFrame.assertLabel(lvliFrame.listitem[2], 'Item2')
#toggle again to uncheck item2
lvliFrame.toggle(lvliFrame.listitem[2])
sleep(config.SHORT_DELAY)
lvliFrame.assertLabel(lvliFrame.listitem[2], 'Item2')

###################################################################
#test for selection implementation
###################################################################

#check list selection implementation
lvliFrame.assertSelectionChild(lvliFrame.list, 2)
sleep(config.SHORT_DELAY)
statesCheck(lvliFrame.listitem[2], "ListItem", add_states=["focused", "selected"])

#clear selection
lvliFrame.assertClearSelection(lvliFrame.list)
sleep(config.SHORT_DELAY)
statesCheck(lvliFrame.listitem[2], "ListItem")
#listbox rise focused state after clear selection
statesCheck(lvliFrame.list, "List", add_states=["focused"])

#################################################################
#test for text implementation
#################################################################

lvliFrame.assertText(lvliFrame.listitem[0], "Item0")
lvliFrame.assertText(lvliFrame.listitem[5], "Item5")

###################################################################
#test for image implementation
###################################################################
for i in range(5):
    lvliFrame.assertImageSize(lvliFrame.listitem[i])


#close application frame window
lvliFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
