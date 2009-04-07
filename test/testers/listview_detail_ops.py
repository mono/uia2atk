#!/usr/bin/env python

###################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        12/16/2008
# Description: Test accessibility of listview widget 
#              Use the listviewframe.py wrapper script
#              Test the samples/listview_detail.py script
###################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of listview_detail widget
"""

# imports
import sys
import os

from strongwind import *
from listview_detail import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the listview_detail sample application
try:
  app = launchListView(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
lvFrame = app.listViewFrame

# check ListView TableColumnHeader's actions
actionsCheck(lvFrame.column_a, "TableColumnHeader")
actionsCheck(lvFrame.column_b, "TableColumnHeader")

# check ListView checkbox's actions
for index in range(6):
    actionsCheck(lvFrame.checkbox[index], "CheckBox")

# check TreeTable 's states
statesCheck(lvFrame.treetable, "TreeTable", add_states=["focused"])

# check TableColumnHeader's states
statesCheck(lvFrame.column_a, "TableColumnHeader")
statesCheck(lvFrame.column_b, "TableColumnHeader")

# check checkbox's states
for index in range(6):
    statesCheck(lvFrame.checkbox[index], "ListViewCheckBox")
  
# check table cell's states, focus on Item0 default
statesCheck(lvFrame.tablecells['Item0'], "ListViewTableCell", add_states=["focused"])
statesCheck(lvFrame.tablecells['0'], "ListViewTableCell")

# mouse click ColumnHeader doesn't rise "focused" because HasKeyboardFocus is False
lvFrame.column_a.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.column_a, "TableColumnHeader")

lvFrame.column_b.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.column_b, "TableColumnHeader")

# mouse click Item0 checkbox to rise "checked"
lvFrame.checkbox[0].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.checkbox[0], "ListViewCheckBox", add_states=["checked"])

# mouse click again to uncheck
lvFrame.checkbox[0].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.checkbox[0], "ListViewCheckBox")

# do click action for Item1 checkbox to rise "checked"
lvFrame.click(lvFrame.checkbox[1])
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.checkbox[1], "ListViewCheckBox", add_states=["checked"])

# do click action again to uncheck
lvFrame.click(lvFrame.checkbox[1])
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.checkbox[1], "ListViewCheckBox")

# mouse click items under Column A, both items in the same row be selected, 
# because FullRowSelect is True, items under Column A should rise focused
##items under Column A can rise focused but accerciser doesn't show BUG468271 Comment#6
lvFrame.tablecells['Item0'].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.tablecells['Item0'], "ListViewTableCell", add_states=["selected", "focused"])
statesCheck(lvFrame.tablecells['0'], "ListViewTableCell", add_states=["selected"])

# keyUp/Down navigate in tree table list, table cell under first column 
# should rise focused, both table cells in the same row should rise selected
lvFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.tablecells['Item1'], "ListViewTableCell", add_states=["selected", "focused"])
statesCheck(lvFrame.tablecells['1'], "ListViewTableCell", add_states=["selected"])
# table cells in first line get rid of focused and selected
statesCheck(lvFrame.tablecells['Item0'], "ListViewTableCell")
statesCheck(lvFrame.tablecells['0'], "ListViewTableCell")

# check TreeTable selection implementation
lvFrame.assertSelectionChild(lvFrame.treetable, 4)
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.tablecells['Item0'], "ListViewTableCell", add_states=["selected"])
statesCheck(lvFrame.tablecells['0'], "ListViewTableCell", add_states=["selected"])

lvFrame.assertSelectionChild(lvFrame.treetable, 10)
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.tablecells['Item2'], "ListViewTableCell", add_states=["selected"])
statesCheck(lvFrame.tablecells['2'], "ListViewTableCell", add_states=["selected"])

# clear selection to get rid of "selected" state, but Item1 still is focused
lvFrame.assertClearSelection(lvFrame.treetable)
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.tablecells['Item1'], "ListViewTableCell", add_states=["focused"])
statesCheck(lvFrame.tablecells['1'], "ListViewTableCell")

# check text implementation for TableCells and CheckBoxs
lvFrame.assertText(lvFrame.tablecells)

lvFrame.assertText(lvFrame.checkbox)

# check table implementation for TreeTable
lvFrame.assertTable(lvFrame.treetable, row=6, col=2)

# check TableCells' order after click ColumnHeader
##columnheader do click action doesn't work BUG476304
lvFrame.click(lvFrame.column_a)
sleep(config.SHORT_DELAY)
#lvFrame.assertOrder(itemone="Item5")

lvFrame.click(lvFrame.column_b)
sleep(config.SHORT_DELAY)
#lvFrame.assertOrder(itemone="Item0")

# check TableCells' order after mouse click TableCells, also check 
# TableCells' position
lvFrame.column_a.mouseClick()
sleep(config.SHORT_DELAY)
lvFrame.assertOrder(itemone="Item5")

lvFrame.column_b.mouseClick()
sleep(config.SHORT_DELAY)
lvFrame.assertOrder(itemone="Item0")

# check TableCells' Text is uneditable
lvFrame.enterTextValue(lvFrame.tablecells['Item0'], "aaaa", oldtext="Item0")

lvFrame.enterTextValue(lvFrame.tablecells['0'], "10", oldtext="0")

##"tree table" has extraneous "table cell" BUG459054
'''
if lvFrame.alltablecell[0].name != "Default Group":
    print "Expected Results: There is no 'Default Group' to be showing"
    pass
else:
    print "TreeTable still has 'Default Group' TableCell"
    exit(4)
'''  

# close application frame window
lvFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
