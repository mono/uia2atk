#!/usr/bin/env python
# -*- coding: utf-8 -*-

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        May 23 2008
# Description: Test accessibility of checkbutton widget 
#              Use the checkbuttonframe.py wrapper script
#              Test the sample/checkButton.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of checkbutton widget
"""

# imports
import sys
from strongwind import *
from treeview import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the treeview sample application
try:
  app = launchTreeView(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
cbFrame = app.treeViewFrame

# find a shorter way instead of app.checkbuttonFrame.CHECK_BUTTON_ONE
cbFrame.column0.click()
# need a short delay when checking and unchecking the check boxes
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.column0,"click");
cbFrame.checkRoleName(cbFrame.column0,"table column header")

cbFrame.column0.click()
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.column0, "unclick");

cbFrame.parent0.activate()
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.parent0, "activate");
cbFrame.checkRoleName(cbFrame.parent0,"table cell")

cbFrame.parent0.activate()
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.parent0, "unactivate");

cbFrame.parent1.select()
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.parent1, "activate");


print "INFO:  Log written to: %s" % config.OUTPUT_DIR


cbFrame.keyCombo('<Alt>F4')

