#!/usr/bin/env python

###########################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        3/2/2008
# Description: Test accessibility of datagridtextboxcolumn widget 
#              Use the datagridframe.py wrapper script
#              Test the samples/winforms/datagrid.py script
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
# BUG479801: ReadOnly cells shouldn't implement EditableText
#dgFrame.assertUnEditableText(dgFrame.read_cells[0], is_implemented=False, oldtext="Read0")
#dgFrame.assertUnEditableText(dgFrame.read_cells[1], is_implemented=False, oldtext="Read1")
#dgFrame.assertUnEditableText(dgFrame.read_cells[2], is_implemented=False, oldtext="Read2")

# Cells under TextBox_Edit is editable, change text to 'editable'
# BUG493865: EditableText of TableCells still remain (null) after doing deleteText
#dgFrame.assertEditableText(dgFrame.edit_cells[0], "editable")
#dgFrame.assertEditableText(dgFrame.edit_cells[1], "editable")
#dgFrame.assertEditableText(dgFrame.edit_cells[2], "editable")

# Cells under TextBox_Read is uneditable, text doesn't being changed
dgFrame.assertTypeText(dgFrame.read_cells[0], expected_text="Read0")
dgFrame.assertTypeText(dgFrame.read_cells[1], expected_text="Read1")
dgFrame.assertTypeText(dgFrame.read_cells[2], expected_text="Read2")

# Cells under TextBox_Edit is editable, change text to 'type something'
dgFrame.assertTypeText(dgFrame.edit_cells[0], expected_text="type something")
dgFrame.assertTypeText(dgFrame.edit_cells[1], expected_text="Edit1type something")
# BUG485466: navigate to last line cause crash, now we know that is our 
# sample's bug
#dgFrame.assertTypeText(dgFrame.edit_cells[2], "editable", expectedtext="editable")




# close application frame window
dgFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
