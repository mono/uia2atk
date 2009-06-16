#!/usr/bin/env python

##############################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        3/2/2008
# Description: Test accessibility of datagridboolcolumn widget 
#              Use the datagridframe.py wrapper script
#              Test the samples/datagrid.py script
##############################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of datagridboolcolumn widget
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

###################################################################
# press key Space to change focused cell's text, key Return move to 
# next row, Label is changed
###################################################################

dgFrame.keyCombo("space", grabFocus=False)
sleep(config.SHORT_DELAY)
dgFrame.keyCombo("Return", grabFocus=False)
sleep(config.SHORT_DELAY)
dgFrame.assertText(dgFrame.null_cell, "False")
dgFrame.assertLabel("row:1 col:0 Value:True")

dgFrame.keyCombo("space", grabFocus=False)
sleep(config.SHORT_DELAY)
dgFrame.keyCombo("Return", grabFocus=False)
sleep(config.SHORT_DELAY)
dgFrame.assertText(dgFrame.true_cell, "(null)")
dgFrame.assertLabel("row:2 col:0 Value:False")

dgFrame.keyCombo("space", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG506286: navigate to last line cause crash
#dgFrame.keyCombo("Return", grabFocus=False)
dgFrame.null_cell.mouseClick()
sleep(config.SHORT_DELAY)
dgFrame.assertText(dgFrame.false_cell, "True")
dgFrame.assertLabel("row:0 col:0 Value:False")

###################################
# mouseClick to test cells position
###################################
dgFrame.true_cell.mouseClick()
sleep(config.SHORT_DELAY)
dgFrame.assertLabel("row:1 col:0 Value:None")

dgFrame.false_cell.mouseClick()
sleep(config.SHORT_DELAY)
dgFrame.assertLabel("row:2 col:0 Value:True")

##########################
# Text is uneditable
##########################
# BUG480831:cells under BoolColumn should not implement Atk.EditableText, 

#dgFrame.assertUnEditableText(dgFrame.true_cell, is_implemented=False, expected_text="(null)")

#dgFrame.assertUnEditableText(dgFrame.null_cell, is_implemented=False, expected_text="False")

#dgFrame.assertUnEditableText(dgFrame.false_cell, is_implemented=False, expected_text="True")

# close application frame window
dgFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
