#!/usr/bin/env python

#########################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        2/26/2008
# Description: Test accessibility of datagrid control 
#              Use the datagridframe.py wrapper script
#              Test the samples/datagrid.py script
#########################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of datagrid control
"""

# imports
import sys
import os

from strongwind import *
from datagrid import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the datagrid sample application
try:
  app = launchDataGrid(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
dgFrame = app.dataGridFrame

####################
# check default Text
####################
dgFrame.assertDefaultText()

###############
# check Actions
###############

# check DataGrid TableColumnHeaders' actions
for column_header in dgFrame.column_headers:
    actionsCheck(column_header, "TableColumnHeader")

# check DataGrid TableCells' actions
for table_cell in dgFrame.table_cells:
    actionsCheck(table_cell, "TableCell")

##############
# check States
##############

# check TreeTable's states
statesCheck(dgFrame.treetable, "TreeTable", add_states=['focused'])

# check TableColumnHeaders' states
for column_header in dgFrame.column_headers:
    statesCheck(column_header, "TableColumnHeader")

# focus on first TableCell(row0, col0) default
# BUG480831: cells under BoolColumn shouldn't implement Atk.EditableText
#statesCheck(dgFrame.null_cell, "TableCell", add_states=["focused"])

# mouse click columnheader doesn't raise "focused" because HasKeyboardFocus is
# False
dgFrame.bool_column.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.bool_column, "TableColumnHeader")

dgFrame.combobox_column.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.combobox_column, "TableColumnHeader")

############################################################
# AtkAction, keyCombo, mouseClick to change label and states
############################################################

# mouse click TableCell 'Edit0' to raise "focused"
dgFrame.edit_cells[0].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.edit_cells[0], "TableCell", add_states=["focused", "editable"])
dgFrame.assertLabel("row:0 col:1 Value:Edit0")

# keyRight move focus to 'Read0'
dgFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.read_cells[0], "TableCell", add_states=["focused"])
statesCheck(dgFrame.edit_cells[0], "TableCell", add_states=["editable"])
dgFrame.assertLabel("row:0 col:2 Value:Read0")

# keyDown move focus to 'Read1'
dgFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.read_cells[1], "TableCell", add_states=["focused"])
statesCheck(dgFrame.read_cells[0], "TableCell")
dgFrame.assertLabel("row:1 col:2 Value:Read1")

# click TableCell 'Edit2' to raise "selected" for cells which in the same row
dgFrame.click(dgFrame.edit_cells[2])
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.edit_cells[2], "TableCell", add_states=["selected", "editable"])
# BUG480831: bool_cells and dropdown_combobox shouldn't have editable state
#statesCheck(dgFrame.combobox_cells[2], "TableCell", add_states=["selected"])
#statesCheck(dgFrame.false_cell, "TableCell", add_states=["selected"])
statesCheck(dgFrame.read_cells[2], "TableCell", add_states=["selected"])
#read_cells[1] get rid of selected but still focused
statesCheck(dgFrame.read_cells[1], "TableCell", add_states=["focused"])
dgFrame.assertLabel("row:1 col:2 Value:Read1")

# click 'Box1' to raise "selected" for cells which in the same row
dgFrame.click(dgFrame.combobox_cells[1])
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.edit_cells[1], "TableCell", add_states=["selected", "editable"])
# BUG480831: bool_cells and dropdown_combobox shouldn't have editable state
#statesCheck(dgFrame.combobox_cells[1], "TableCell", add_states=["selected"])
#statesCheck(dgFrame.true_cell, "TableCell", add_states=["selected"])
statesCheck(dgFrame.read_cells[1], "TableCell", add_states=["selected", "focused"])
# edit_cells[2] still selected
statesCheck(dgFrame.edit_cells[2], "TableCell", add_states=["selected", "editable"])
dgFrame.assertLabel("row:1 col:2 Value:Read1")

###################
# test AtkSelection
###################

# select ColumnHeader shouldn't raise selected
dgFrame.selectChild(dgFrame.treetable, 0)
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.bool_column, "TableColumnHeader")

# select index5 'Edit0' to raise selected for row1 TableCells
dgFrame.selectChild(dgFrame.treetable, 5)
sleep(config.SHORT_DELAY)
# BUG480831: and bool_cells and dropdown_combobox shouldn't have editable state
#statesCheck(dgFrame.null_cell, "TableCell", add_states=["selected"])
statesCheck(dgFrame.edit_cells[0], "TableCell", add_states=["selected", "editable"])
statesCheck(dgFrame.read_cells[0], "TableCell", add_states=["selected"])
#statesCheck(dgFrame.combobox_cells[0], "TableCell", add_states=["selected"])

# clear selection to get rid of "selected" state from row1 TableCells
dgFrame.clearSelection(dgFrame.treetable)
sleep(config.SHORT_DELAY)
# BUG480831: bool_cells and dropdown_combobox shouldn't have editable state
#statesCheck(dgFrame.null_cell, "TableCell")
statesCheck(dgFrame.edit_cells[0], "TableCell", add_states=["editable"])
statesCheck(dgFrame.read_cells[0], "TableCell")
#statesCheck(dgFrame.combobox_cells[0], "TableCell")

###############
# test AtkTable
###############

# check table's table implementation
dgFrame.assertRowAndColumnCount(dgFrame.treetable, row=3, col=4)

###############################################
# test AtkAction for ColumnHeader to sort order
###############################################

# check item's order after click column header
# BUG493176: TableCell doesn't exchange position
#dgFrame.edittext_column.click(log=True)
#sleep(config.SHORT_DELAY)
#dgFrame.assertSortedOrder(is_reversed=True)

#dgFrame.readtext_column.click(log=True)
#sleep(config.SHORT_DELAY)
#dgFrame.assertSortedOrder(is_reversed=False)

# check item's order after mouse click column header, also the test can check 
# column header's position
#dgFrame.edittext_column.mouseClick()
#sleep(config.SHORT_DELAY)
#dgFrame.assertSortedOrder(is_reversed=True)

#dgFrame.combobox_column.mouseClick()
#sleep(config.SHORT_DELAY)
#dgFrame.assertSortedOrder(is_reversed=False)

# close application frame window
dgFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
