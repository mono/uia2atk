#!/usr/bin/env python

##
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        3/2/2008
# Description: Test accessibility of datagridboolcolumn widget 
#              Use the datagridframe.py wrapper script
#              Test the samples/datagrid.py script
##

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

#####################################################################################
##AtkAction test, mouse click, key navigate to change label and text
#####################################################################################

#do click action for BoolColumn cells to check its Text
dgFrame.click(dgFrame.false_cell)
sleep(config.SHORT_DELAY)
dgFrame.assertText(dgFrame.false_cell, "True")

dgFrame.click(dgFrame.true_cell)
sleep(config.SHORT_DELAY)
dgFrame.assertText(dgFrame.true_cell, "(null)")

dgFrame.click(dgFrame.null_cell)
sleep(config.SHORT_DELAY)
dgFrame.assertText(dgFrame.null_cell, "False")

#mouse click and key press
dgFrame.false_cell.mouseClick()
sleep(config.SHORT_DELAY)
dgFrame.assertLabel("row:2 col:0 Value:True")

dgFrame.keyCombo("Space", grabFocus=False)
sleep(config.SHORT_DELAY)
dgFrame.assertText(dgFrame.false_cell, "(null)")

#key up and press
dgFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
dgFrame.assertLabel("row:1 col:0 Value:")

dgFrame.keyCombo("Space", grabFocus=False)
sleep(config.SHORT_DELAY)
dgFrame.assertText(dgFrame.true_cell, "False")

#key up and press
dgFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
dgFrame.assertLabel("row:0 col:0 Value:False")

dgFrame.keyCombo("Space", grabFocus=False)
sleep(config.SHORT_DELAY)
dgFrame.assertText(dgFrame.null_cell, "True")

##########################
##Text is uneditable
##########################

dgFrame.enterTextValue(dgFrame.true_cell, "True", oldtext="False")

dgFrame.enterTextValue(dgFrame.null_cell, "(null)", oldtext="True")

dgFrame.enterTextValue(dgFrame.false_cell, "False", oldtext="(null)")

#close application frame window
dgFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
