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

# -> Check DataGridView TableColumnHeader's actions and states list
for column in dtgvFrame.columns:
	# BUG478840 - DataGridView. Columns are missing "click" action
	##actionsCheck(column, "TableColumnHeader")
	statesCheck(column, "TableColumnHeader")
	
	# Mouse clicking column won't rise "focused" because HasKeyboardFocus is False
	column.mouseClick()
	sleep(config.SHORT_DELAY)
	statesCheck(column, "TableColumnHeader")
	
# -> CheckBox cells tests

# BUG478856 - Uncomment following lines
##statesCheck(dtgvFrame.tablecells[0], "TreeViewTableCell", add_states=["focused", "selected"])
##statesCheck(dtgvFrame.tablecells[1], "TreeViewTableCell")
##statesCheck(dtgvFrame.tablecells[2], "TreeViewTableCell")

##reg = EventListener(event_types='object:state-changed')
##reg.start()

checkbox = dtgvFrame.findAllCheckBoxes(None)[1]
checkbox.grabFocus()

# BUG478856 - focusable is missing
#assert reg.containsEvent(checkbox, 'object:state-changed:focused')

##reg.clearQueuedEvents()

##sleep(config.LONG_DELAY)
# BUG478856 - focusable is missing
##statesCheck(checkbox, "CheckBox", add_states=[ \
##    "selectable", "selected", "focused" \
##])

##checkbox.click()
##sleep(config.LONG_DELAY)
# BUG478856 - focusable is missing
##statesCheck(checkbox, "CheckBox", add_states=[ \
##    "selectable", "selected", "focused", "checked" \
##])

# assert reg.containsEvent(checkbox, 'object:state-changed:checked')

##reg.clearQueuedEvents()

checkbox.click()
# BUG478856 - focusable is missing
##sleep(config.LONG_DELAY)
##statesCheck(checkbox, "CheckBox", add_states=[ \
##   "selectable", "selected", "focused", \
##])

##sleep(config.LONG_DELAY)
##assert reg.containsEvent(checkbox, 'object:state-changed:checked')

##reg.stop()

# -> TextBox cell tests
dtgvFrame.assertEditsText(dtgvFrame.edits)

# BUG478856 - Uncomment following 4 lines
##dtgvFrame.edits[0].mouseClick()
##statesCheck(dtgvFrame.edits[0], "TextBox", add_states=[ "selectable", "selected", "focused" ])
##dtgvFrame.edits[1].mouseClick()
##statesCheck(dtgvFrame.edits[1], "TextBox", add_states=[ "selectable", "selected", "focused" ])

# -> Button cell tests

for button in dtgvFrame.buttons:
	actionsCheck(button,"Button")
	# BUG478856 "focusable"
	# statesCheck(button,"Button")
	
# Testing click
dtgvFrame.buttons[0].mouseClick()
sleep(config.SHORT_DELAY)
dtgvFrame.assertCellClickValue(0, 3)
# BUG478856 - focusable is missing
##statesCheck(dtgvFrame.buttons[0],"Button", add_states=["focused", "selectable", "selected"])
dtgvFrame.buttons[1].mouseClick()
sleep(config.SHORT_DELAY)
# BUG478856 - focusable is missing
##statesCheck(dtgvFrame.buttons[0],"Button", add_states=["selectable"])
dtgvFrame.assertCellClickValue(1, 3)
# BUG478856 - focusable is missing
# statesCheck(dtgvFrame.buttons[1],"Button", add_states=["focused", "selectable", "selected"])

# -> Link cell tests

# BUG478856  missing focusable
##for label in dtgvFrame.labels:
##	statesCheck(button,"Label", invalid_states=["multi line"], add_states=["selectable", "focusable"])

# -> ComboBox cell tests

# -> All cells: Selection tests
dtgvFrame.assertClearSelection(dtgvFrame.treetable)
sleep(config.SHORT_DELAY)
# Selecting first checkbox
dtgvFrame.assertSelectionChild(dtgvFrame.treetable, 0)
sleep(config.SHORT_DELAY)
# BUG478891 - DataGridView: Selected cells don't emit "selected" event.
##statesCheck(dtgvFrame.treetable[0], "CheckBox", add_states=["selected", "checked", "selectable"], invalid_states=["enabled", "sensitive"])

# Selecting first textbox
dtgvFrame.assertSelectionChild(dtgvFrame.treetable, 1)
# BUG479126 - DataGridView. TextBox cells don't implement SelectionItem
##statesCheck(dtgvFrame.treetable[1], "TableCell", add_states=["selected", "selectable"], invalid_states=["enabled", "sensitive"])

#check table's table implementation
dtgvFrame.assertTable(dtgvFrame.treetable, row=6, col=5)
sleep(config.SHORT_DELAY)

#close application frame window
dtgvFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

