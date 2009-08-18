#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/11/2008
# Description: Test accessibility of listview widget 
#              Use the listviewframe.py wrapper script
#              Test the samples/winforms/ListView_largeimage.py script
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

##########################
#check states and actions
##########################

#check ListView item's actions list
actionsCheck(lvliFrame.listitem[0], "CheckedListItem")

#check ListView's states list
statesCheck(lvliFrame.list, "TreeTable", add_states=["focused"])

#check ListItem0,1's default states
statesCheck(lvliFrame.listitem[0], "TableCell", add_states=["focused"])
statesCheck(lvliFrame.listitem[1], "TableCell")

#click listitem to rise selected states, listitem1 also with 
#selected states after click listitem3 because MultiSelect is True
lvliFrame.listitem[1].click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(lvliFrame.listitem[1], "TableCell", add_states=["selected"])

lvliFrame.listitem[3].click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(lvliFrame.listitem[3], "TableCell", add_states=["selected"])

statesCheck(lvliFrame.listitem[1], "TableCell", add_states=["selected"])

#do toggle action to check/uncheck listitem
lvliFrame.listitem[1].toggle(log=True)
sleep(config.SHORT_DELAY)
lvliFrame.assertLabel("Item1 Checked")
statesCheck(lvliFrame.listitem[1], "TableCell", \
                                    add_states=["checked", "selected"])

lvliFrame.listitem[1].toggle(log=True)
sleep(config.SHORT_DELAY)
lvliFrame.assertLabel("Item1 Unchecked")
statesCheck(lvliFrame.listitem[1], "TableCell", \
                                    add_states=["selected"])
#toggle listitem2 to checked but without focused and selected
lvliFrame.listitem[2].toggle(log=True)
sleep(config.SHORT_DELAY)
lvliFrame.assertLabel("Item2 Checked")
statesCheck(lvliFrame.listitem[2], "TableCell", \
                                    add_states=["checked"])

lvliFrame.listitem[2].toggle(log=True)
sleep(config.SHORT_DELAY)
lvliFrame.assertLabel("Item2 Unchecked")
statesCheck(lvliFrame.listitem[2], "TableCell")

#mouse click ListItem to rise focused and selected states
lvliFrame.mouseClick(log=False)
lvliFrame.listitem[0].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvliFrame.listitem[0], "TableCell", add_states=["focused", "selected"])

lvliFrame.listitem[5].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvliFrame.listitem[5], "TableCell", add_states=["focused", "selected"])
#listitem0 with default states after click listitem5
statesCheck(lvliFrame.listitem[0], "TableCell")

###################################
#test for selection implementation
###################################

#check list selection implementation
lvliFrame.selectionChild(lvliFrame.list, 2)
sleep(config.SHORT_DELAY)
statesCheck(lvliFrame.listitem[2], "TableCell", add_states=["selected"])

#clear selection
lvliFrame.clearSelection(lvliFrame.list)
sleep(config.SHORT_DELAY)
statesCheck(lvliFrame.listitem[2], "TableCell")
#check listbox state after clear selection
statesCheck(lvliFrame.list, "TreeTable", add_states=["focused"])

##############################
#test for text implementation
##############################

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
