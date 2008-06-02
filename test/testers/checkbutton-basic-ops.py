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
from strongwind import *
from checkbutton import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the checkbutton sample application
try:
  app = launchCheckButton(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
cbFrame = app.checkbuttonFrame

# find a shorter way instead of app.checkbuttonFrame.CHECK_BUTTON_ONE
cbFrame.findCheckBox(cbFrame.CHECK_BUTTON_ONE).click()
cbFrame.assertResult(cbFrame.CHECK_BUTTON_ONE,"checked");

# need a short delay when checking and unchecking the check boxes
sleep(config.SHORT_DELAY)

cbFrame.findCheckBox(cbFrame.CHECK_BUTTON_ONE).click()
cbFrame.assertResult(cbFrame.CHECK_BUTTON_ONE, "unchecked");

sleep(config.SHORT_DELAY)

cbFrame.findCheckBox(cbFrame.CHECK_BUTTON_TWO).click()
cbFrame.assertResult(cbFrame.CHECK_BUTTON_TWO, "checked");

sleep(config.SHORT_DELAY)

cbFrame.findCheckBox(cbFrame.CHECK_BUTTON_TWO).click()
cbFrame.assertResult(cbFrame.CHECK_BUTTON_TWO, "unchecked");

cbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
