#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/12/2008
# Description: Test accessibility of listview widget 
#              Use the listviewframe.py wrapper script
#              Test the samples/winforms/ListView_smallimage.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of listview_smallimage widget
"""

# imports
import sys
import os

from strongwind import *
from listview_smallimage import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass #expected

# open the listview_smallimage sample application
try:
    app = launchListView(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
lvsiFrame = app.listViewSmallImageFrame

##########################
# check states and actions
##########################

# check ListView item's actions list
actionsCheck(lvsiFrame.listitem[0], "CheckedListItem")

# check ListView's states list
statesCheck(lvsiFrame.list, "TreeTable", add_states=["focused"])

# check ListItem0,1's default states
statesCheck(lvsiFrame.listitem[0], "TableCell", add_states=["focused"])
statesCheck(lvsiFrame.listitem[1], "TableCell")

# click listitem to rise selected states, listitem1 also with 
# selected states after click listitem3 because MultiSelect is True
lvsiFrame.listitem[1].click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(lvsiFrame.listitem[1], "TableCell", add_states=["selected"])

lvsiFrame.listitem[3].click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(lvsiFrame.listitem[3], "TableCell", add_states=["selected"])

statesCheck(lvsiFrame.listitem[1], "TableCell", add_states=["selected"])

# do toggle action to check/uncheck listitem
lvsiFrame.listitem[1].toggle(log=True)
sleep(config.SHORT_DELAY)
lvsiFrame.assertLabel("Item1 Checked")
statesCheck(lvsiFrame.listitem[1], "TableCell", \
                                    add_states=["checked", "selected"])

lvsiFrame.listitem[1].toggle(log=True)
sleep(config.SHORT_DELAY)
lvsiFrame.assertLabel("Item1 Unchecked")
statesCheck(lvsiFrame.listitem[1], "TableCell", \
                                    add_states=["selected"])
# toggle listitem2 to checked but without focused and selected
lvsiFrame.listitem[2].toggle(log=True)
sleep(config.SHORT_DELAY)
lvsiFrame.assertLabel("Item2 Checked")
statesCheck(lvsiFrame.listitem[2], "TableCell", \
                                    add_states=["checked"])

lvsiFrame.listitem[2].toggle(log=True)
sleep(config.SHORT_DELAY)
lvsiFrame.assertLabel("Item2 Unchecked")
statesCheck(lvsiFrame.listitem[2], "TableCell")

# mouse click ListItem to rise focused and selected states
lvsiFrame.mouseClick(log=False)
lvsiFrame.listitem[0].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvsiFrame.listitem[0], "TableCell", add_states=["focused", "selected"])

lvsiFrame.listitem[5].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvsiFrame.listitem[5], "TableCell", add_states=["focused", "selected"])
# listitem0 with default states after click listitem5
statesCheck(lvsiFrame.listitem[0], "TableCell")

###################################
# test for selection implementation
###################################

# check list selection implementation
lvsiFrame.selectionChild(lvsiFrame.list, 2)
sleep(config.SHORT_DELAY)
statesCheck(lvsiFrame.listitem[2], "TableCell", add_states=["selected"])

# clear selection
lvsiFrame.clearSelection(lvsiFrame.list)
sleep(config.SHORT_DELAY)
statesCheck(lvsiFrame.listitem[2], "TableCell")
# check listbox state after clear selection
statesCheck(lvsiFrame.list, "TreeTable", add_states=["focused"])

##############################
# test for text implementation
##############################

lvsiFrame.assertText(lvsiFrame.listitem[0], "Item0")
lvsiFrame.assertText(lvsiFrame.listitem[5], "Item5")

###############################
# test for image implementation
###############################
for i in range(5):
    lvsiFrame.assertImageSize(lvsiFrame.listitem[i])


# close application frame window
lvsiFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
