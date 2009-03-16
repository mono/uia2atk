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
##states test 
#################

statesCheck(dgFrame.edit_cells[0], "TableCell", add_states=["editable"])

statesCheck(dgFrame.read_cells[0], "TableCell")

#####################################################################################
##AtkAction test, mouse click, key navigate to change label and text
#####################################################################################

#AtkAction, mouse click, key up to change label
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
dgFrame.assertLabel("row:2 col:3 Value:Read2")

###################################
##EditableText test
###################################

#TextBox_Read is uneditable, text still remain 'Read1'
dgFrame.enterTextValue(dgFrame.read_cells[1], "uneditable", oldtext="Read1")

#TextBox_Edit is editable, change text to 'editable'
dgFrame.enterTextValue(dgFrame.read_cells[2], "editable")

#close application frame window
dgFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
