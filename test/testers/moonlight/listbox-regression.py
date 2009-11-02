#!/usr/bin/env python

##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/10/20
# Description: Test accessibility of listbox widget
#              Use the listboxframe.py wrapper script
#              Test the Moonlight ListBox sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of listbox widget
"""

# imports
from pyatspi import *
from strongwind import *
from listbox import *
from helpers import *

from sys import argv
from os import path

app_path = None
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the listbox sample application
try:
    app = launchListBox(app_path)
except IOError, msg:
    print  "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
lbFrame = app.listBoxFrame

################
# Check Actions
################
for list_item in lbFrame.list_items:
    actionsCheck(list_item, 'TableCell')

#######################
# Check default States
#######################
statesCheck(lbFrame.label, 'Label')
statesCheck(lbFrame.list_box, 'TreeTable')
for list_item in lbFrame.list_items:
    statesCheck(list_item, 'TableCell')

########################################
# Test ATK_TEXT implement for list_item
########################################
for list_item in lbFrame.list_items:
    assertText(list_item, str(list_item))

#################
# Key navigation
#################
lbFrame.click()
sleep(config.SHORT_DELAY)
assertText(lbFrame.label, 'You selected no item.')
statesCheck(lbFrame.list_items[0], 'TableCell', add_states=['focused']

lbFrame.keyCombo('Down')
sleep(config.SHORT_DELAY)
lbFrame.keyCombo('Down')
sleep(config.SHORT_DELAY)
lbFrame.keyCombo('Down')
sleep(config.SHORT_DELAY)
assertText(lbFrame.label, 'You selected Item 4.')
statesCheck(lbFrame.list_items[3], 'TableCell', add_states=['focused', 'seleceted'])
# 'focused' and 'selected' state should move from Item1 to Item4
statesCheck(lbFrame.list_items[0], 'TableCell')

########################
# Selection action test
########################
lbFrame.select(lbFrame.list_items[0])
statesCheck(lbFrame.list_items[0], 'TableCell', add_states=['focused', 'selected'])
# re-check 'Item 4' states
statesCheck(lbFrame.list_items[3], 'TableCell')
lbFrame.clearSelection(lbFrame.list_box)
sleep(config.SHORT_DELAY)
statesCheck(lbFrame.list_box, "TreeTable", add_states=["focused"])
statesCheck(lbFrame.list_items[0], "TableCell", add_states=["focused"])

print 'INFO:  Log written to: %s' % config.OUTPUT_DIR

# close application frame window
quit(lbFrame)
