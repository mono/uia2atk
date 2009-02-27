#!/usr/bin/env python

#########################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        2/26/2008
# Description: Test accessibility of datagrid widget 
#              Use the datagridframe.py wrapper script
#              Test the samples/datagrid.py script
#########################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of datagrid widget
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

#####################################################
##check default Text
#####################################################
dgFrame.assertDefaultText("ColumnHeader")

dgFrame.assertDefaultText("BoolColumn")

dgFrame.assertDefaultText("TextBox_Edit")

dgFrame.assertDefaultText("TextBox_Read")

dgFrame.assertDefaultText("ComboBox")

######################################################
##check Actions
##check States
##AtkAction test to change label and states
##AtkComponent test by mouse click
##key navigate to change focus
######################################################

#check DataGrid TableColumnHeader's actions list
actionsCheck(dgFrame.bool_column, "TableColumnHeader")
actionsCheck(dgFrame.readtext_column, "TableColumnHeader")
actionsCheck(dgFrame.edittext_column, "TableColumnHeader")
actionsCheck(dgFrame.combobox_column, "TableColumnHeader")

#check DataGridBoolColumn TableCell actions list
actionsCheck(dgFrame.nullbool_cell, "TableCell")
actionsCheck(dgFrame.truebool_cell, "TableCell")
actionsCheck(dgFrame.falsebool_cell, "TableCell")

#check TreeTable 's states
statesCheck(dgFrame.treetable, "TreeTable")

#check TableColumnHeader's states
statesCheck(dgFrame.bool_column, "TableColumnHeader")
statesCheck(dgFrame.readtext_column, "TableColumnHeader")
statesCheck(dgFrame.edittext_column, "TableColumnHeader")
statesCheck(dgFrame.combobox_column, "TableColumnHeader")

#focus on first TableCell(row0, col0) default
statesCheck(dgFrame.nullbool_cell, "TableCell", add_states=["focused"])

#mouse click bool_column doesn't raise "focused" because HasKeyboardFocus is False
dgFrame.bool_column.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.bool_column, "TableColumnHeader")

#mouse click combobox_column doesn't raise "focused" because HasKeyboardFocus is False
dgFrame.combobox_column.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.combobox_column, "TableColumnHeader")

#mouse click TableCell 'Edit0' to raise "focused"
dgFrame.edit_cells[0].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.edit_cells[0], "TableCell", add_states=["focused"])
dgFrame.assertLabel("row:0 col:1 Value:Edit0")

#keyRight move focus to 'Read0'
dgFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.read_cells[0], "TableCell", add_states=["focused"])
statesCheck(dgFrame.edit_cells[0], "TableCell")
dgFrame.assertLabel("row:0 col:2 Value:Read0")

#keyDown move focus to 'Read1'
dgFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.read_cells[1], "TableCell", add_states=["focused"])
statesCheck(dgFrame.read_cells[0], "TableCell")
dgFrame.assertLabel("row:1 col:1 Value:Read1")

#do click action for TableCell 'Edit2' to raise "checked"
dgFrame.click(dgFrame.edit_cells[2])
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.edit_cells[2], "TableCell", add_states=["focused"])
statesCheck(dgFrame.read_cells[1], "TableCell")
dgFrame.assertLabel("row:2 col:1 Value:Edit2")

#do click move focus to 'Box1' 
dgFrame.click(dgFrame.combobox_cells[1])
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.combobox_cells[1], "TableCell", add_states=["focused"])
statesCheck(dgFrame.edit_cells[2], "TableCell")
dgFrame.assertLabel("row:0 col:3 Value:Box1")

########################
##test AtkSelection
########################

#select index0 'BoolColumn to raise selected
dgFrame.assertSelectionChild(dgFrame.treetable, 0)
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.bool_column, "TableColumnHeader", add_states = ["selected"])

#select index5 'Edit0' to raise selected for row1 TableCells
dgFrame.assertSelectionChild(dgFrame.treetable, 3)
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.nullbool_cell, "TableCell", add_states=["selected"])
statesCheck(dgFrame.edit_cells[0], "TableCell", add_states=["selected"])
statesCheck(dgFrame.read_cells[0], "TableCell", add_states=["selected"])
statesCheck(dgFrame.combobox_cells[0], "TableCell", add_states=["selected"])

#clear selection to get rid of "selected" state from row1 TableCells
dgFrame.assertClearSelection(dgFrame.treetable)
sleep(config.SHORT_DELAY)
statesCheck(dgFrame.nullbool_cell, "TableCell")
statesCheck(dgFrame.edit_cells[0], "TableCell")
statesCheck(dgFrame.read_cells[0], "TableCell")
statesCheck(dgFrame.combobox_cells[0], "TableCell")

#####################################################
##test AtkTable
#####################################################

#check table's table implementation
dgFrame.assertTable(dgFrame.treetable, row=3, col=4)

###########################################################
##test AtkAction for ColumnHeader to sort order
###########################################################

#check item's order after click column header
dgFrame.click(dgFrame.bool_column)
sleep(config.SHORT_DELAY)
dgFrame.assertOrder(itemone="Read2")

dgFrame.click(dgFrame.readtext_column)
sleep(config.SHORT_DELAY)
dgFrame.assertOrder(itemone="Read0")

#check item's order after mouse click column header, also the test can check 
#column header's position
dgFrame.edittext_column.mouseClick()
sleep(config.SHORT_DELAY)
dgFrame.assertOrder(itemone="Read2")

dgFrame.combobox_column.mouseClick()
sleep(config.SHORT_DELAY)
dgFrame.assertOrder(itemone="Read0")

#close application frame window
dgFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
