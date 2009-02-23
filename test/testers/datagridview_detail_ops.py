#!/usr/bin/env python

##
# Written by:  Mario Carrion <mcarrion@novell.com>
# Date:        02/23/2009
# Description: Test accessibility of DataGridView widget 
#              Use the datagridviewframe.py wrapper script
#              Test the samples/datagridview.py script
##

# The docstring below  is used in the generated log file
"""
Test accessibility of DataGridView widget
"""

# imports
import sys
import os

from strongwind import *
from datagridview import *
from helpers import *
from eventlistener import *
from sys import argv
from os import path
import pyatspi

app_path = None 
try:
	app_path = argv[1]
except IndexError:
	pass #expected

# open the datagridview sample application
try:
	app = launchDataGridView(app_path)
except IOError, msg:
	print "ERROR:  %s" % msg
	exit(2)

# make sure we got the app back
if app is None:
	exit(4)

# Alias to make things shorter
dtgvFrame = app.dataGridViewFrame

# Check DataGridView TableColumnHeader's actions list

# BNC #478840 - Uncomment following 2 lines 
##actionsCheck(dtgvFrame.checkbox_column, "TableColumnHeader")
##actionsCheck(dtgvFrame.textbox_column,  "TableColumnHeader")

statesCheck(dtgvFrame.checkbox_column, "TableColumnHeader")
statesCheck(dtgvFrame.textbox_column,  "TableColumnHeader")

# BCN #478856 - Uncomment following 3 lines
##statesCheck(dtgvFrame.tablecells[0], "TreeViewTableCell", add_states=["focused", "selected"])
##statesCheck(dtgvFrame.tablecells[1], "TreeViewTableCell")
##statesCheck(dtgvFrame.tablecells[2], "TreeViewTableCell")

checkbox = dtgvFrame.findAllCheckBoxes(None)[0]
statesCheck(checkbox, "CheckBox", add_states=[ \
    "selectable", "selected", "checked", "focused" \
])

reg = EventListener(event_names='object:state-changed')
reg.start()

checkbox.click()
statesCheck(checkbox, "CheckBox", add_states=[ \
    "selectable", "selected", "focused" \
])

sleep(config.SHORT_DELAY)
assert reg.containsEvent(checkbox, 'object:state-changed:checked')

reg.clearQueuedEvents()

checkbox.click()
statesCheck(checkbox, "CheckBox", add_states=[ \
    "selectable", "selected", "checked", "focused" \
])

sleep(config.SHORT_DELAY)
assert reg.containsEvent(checkbox, 'object:state-changed:checked')

reg.stop()

# Testing Edits Text value
dtgvFrame.assertEditsText(dtgvFrame.edits)

# Testing Edits States
# BCN #478856 - Uncomment following 5 lines
##for index in range(6):
##	if index % 2 == 0: # Not Editable 
##		statesCheck(dtgvFrame.edits[index], "TreeViewTableCell", invalid_states=["editable"])
##	else: # Editable
##		statesCheck(dtgvFrame.edits[index], "TreeViewTableCell", add_states=["editable"])

# Selection tests
dtgvFrame.assertClearSelection(dtgvFrame.treetable)
sleep(config.SHORT_DELAY)
dtgvFrame.assertSelectionChild(dtgvFrame.treetable, 0)
sleep(config.SHORT_DELAY)
# BCN #478891 - Uncomment following line
# statesCheck(dtgvFrame.checkboxes[0], "CheckBox", add_states=["selected", "checked", "selectable"], invalid_states=["enabled", "sensitive"])

#check table's table implementation
dtgvFrame.assertTable(dtgvFrame.treetable, row=6, col=2)
sleep(config.SHORT_DELAY)

#close application frame window
dtgvFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
