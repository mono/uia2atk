#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/18/2008
# Description: main test script of listbox
#              ../samples/listbox.py is the test sample script
#              listbox/* is the wrapper of listbox test sample script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of listbox widget
"""
# imports
from listbox import *
from helpers import *
from states import *
from actions import *
from sys import argv

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass #expected

# open the treeview sample application
try:
    app = launchListBox(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
lbFrame = app.listBoxFrame

############################
# check TableCell's actions
############################
actionsCheck(lbFrame.tablecell[0], "TableCell")
actionsCheck(lbFrame.tablecell[1], "TableCell")
actionsCheck(lbFrame.tablecell[19], "TableCell")

############################
# check TreeTable's states
############################
# BUG480218: Both treetable and tablecell[0] should has focused state
#statesCheck(lbFrame.treetable, "TreeTable", add_states=["focused"])

############################
# check TableCell's states
############################

statesCheck(lbFrame.tablecell[0], "TableCell", add_states=["focused", "selected"])
statesCheck(lbFrame.tablecell[1], "TableCell")
statesCheck(lbFrame.tablecell[19], "TableCell")

## BUG450704: Incorrect list item is selected, line63-64 may avoid the block
lbFrame.mouseClick()
sleep(config.SHORT_DELAY)

lbFrame.tablecell[10].mouseClick()
sleep(config.SHORT_DELAY)
lbFrame.assertLabel('10')
statesCheck(lbFrame.tablecell[10], "TableCell", add_states=["focused", "selected"])

lbFrame.tablecell[15].mouseClick()
sleep(config.SHORT_DELAY)
lbFrame.assertLabel('15')
statesCheck(lbFrame.tablecell[15], "TableCell", add_states=["focused", "selected"])

#########################################################
# check TableCell's AtkAction, TableCells use Click event
#########################################################
lbFrame.tablecell[0].click(log=True)
sleep(config.SHORT_DELAY)
## BUG496786: Click action doesn't send event to change lable
#lbFrame.assertLabel('0')
statesCheck(lbFrame.tablecell[0], "TableCell", add_states=["focused", "selected"])
statesCheck(lbFrame.tablecell[15], "TableCell")

lbFrame.tablecell[9].click(log=True)
sleep(config.SHORT_DELAY)
#lbFrame.assertLabel('9')
statesCheck(lbFrame.tablecell[9], "TableCell", add_states=["focused", "selected"])
statesCheck(lbFrame.tablecell[0], "TableCell")

########################################################################
# check TableCell's AtkAction, TableCells use SelectedIndexChanged event
########################################################################
lbFrame.checkbox.click(log=True)
sleep(config.SHORT_DELAY)

lbFrame.tablecell[0].click(log=True)
sleep(config.SHORT_DELAY)
lbFrame.assertLabel('0')
statesCheck(lbFrame.tablecell[0], "TableCell", add_states=["focused", "selected"])
statesCheck(lbFrame.tablecell[19], "TableCell")

lbFrame.tablecell[9].click(log=True)
sleep(config.SHORT_DELAY)
lbFrame.assertLabel('9')
statesCheck(lbFrame.tablecell[9], "TableCell", add_states=["focused", "selected"])
statesCheck(lbFrame.tablecell[0], "TableCell")

lbFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
lbFrame.assertLabel('10')
statesCheck(lbFrame.tablecell[10], "TableCell", add_states=["focused", "selected"])

#######################################################
# check AtkSelection by select child and clear selected
#######################################################

lbFrame.selectChildAndCheckStates(lbFrame.treetable, lbFrame.tablecell[0].getIndexInParent(), add_states=["focused", "selected"])
statesCheck(lbFrame.tablecell[1], "TableCell")

lbFrame.assertClearSelection(lbFrame.treetable)
sleep(config.SHORT_DELAY)
statesCheck(lbFrame.treetable, "TreeTable", add_states=["focused"])
statesCheck(lbFrame.tablecell[0], "TableCell", add_states=["focused"])

lbFrame.selectChildAndCheckStates(lbFrame.treetable, lbFrame.tablecell[19].getIndexInParent(), add_states=["focused", "selected"])
## BUG496764: missing visible state
#statesCheck(lbFrame.tablecell[0], "TableCell", invalid_states=["showing"])

lbFrame.assertClearSelection(lbFrame.treetable)
sleep(config.SHORT_DELAY)
statesCheck(lbFrame.treetable, "TreeTable", add_states=["focused"])
statesCheck(lbFrame.tablecell[19], "TableCell", add_states=["focused"])

############################
# check TableCell's AtkText
############################
#check tablecell's Text
lbFrame.assertItemsText()

############################
# End
############################
#close application frame window
lbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
