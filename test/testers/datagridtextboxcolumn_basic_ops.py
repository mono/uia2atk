#!/usr/bin/env python

###########################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        3/2/2008
# Description: Test accessibility of datagridtextboxcolumn widget 
#              Use the datagridframe.py wrapper script
#              Test the samples/datagrid.py script
###########################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of datagridtextboxcolumn widget
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

#################
# states test 
#################

statesCheck(dgFrame.edit_cells[0], "TableCell", add_states=["editable"])
## BUG479796: ReadOnly cell still has editable state
#statesCheck(dgFrame.read_cells[0], "TableCell")

####################################################################
# mouse click, key navigate to change label and text
####################################################################

# mouse click, key press to change label
dgFrame.edit_cells[1].mouseClick()
sleep(config.SHORT_DELAY)
dgFrame.assertLabel("row:1 col:1 Value:Edit1")

dgFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
dgFrame.assertLabel("row:0 col:1 Value:Edit0")

dgFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
dgFrame.assertLabel("row:0 col:2 Value:Read0")

dgFrame.read_cells[2].mouseClick()
sleep(config.SHORT_DELAY)
dgFrame.assertLabel("row:2 col:2 Value:Read2")

#########################################
# test AtkText by insertText and typeText
#########################################

# Cells under TextBox_Read is uneditable, text doesn't being changed
## BUG479801: ReadOnly cells shouldn't implement EditableText
#dgFrame.assertInsertText(dgFrame.read_cells[0], "uneditable", oldtext="Read0")
#dgFrame.assertInsertText(dgFrame.read_cells[1], "uneditable", oldtext="Read1")
#dgFrame.assertInsertText(dgFrame.read_cells[2], "uneditable", oldtext="Read2")

# Cells under TextBox_Edit is editable, change text to 'editable'
## BUG493865: EditableText of TableCells still remain (null) after doing deleteText
#dgFrame.assertInsertText(dgFrame.edit_cells[0], "editable")
#dgFrame.assertInsertText(dgFrame.edit_cells[1], "editable")
#dgFrame.assertInsertText(dgFrame.edit_cells[2], "editable")

# Cells under TextBox_Read is uneditable, text doesn't being changed
dgFrame.assertTypeText(dgFrame.read_cells[0], "uneditable", expectedtext="Read0")
dgFrame.assertTypeText(dgFrame.read_cells[1], "uneditable", expectedtext="Read1")
dgFrame.assertTypeText(dgFrame.read_cells[2], "uneditable", expectedtext="Read2")

# Cells under TextBox_Edit is editable, change text to 'editable'
dgFrame.assertTypeText(dgFrame.edit_cells[0], "editable", expectedtext="editable")
dgFrame.assertTypeText(dgFrame.edit_cells[1], "editable", expectedtext="Edit1editable")
## BUG485466: navigate to last line cause crash
#dgFrame.assertTypeText(dgFrame.edit_cells[2], "editable", expectedtext="editable")




# close application frame window
dgFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
