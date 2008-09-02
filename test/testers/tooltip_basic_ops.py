#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/02/2008
# Description: Test accessibility of tooltip widget 
#              Use the tooltipframe.py wrapper script
#              Test the samples/tooltip.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of tooltip widget
"""

# imports
import sys
import os

from strongwind import *
from tooltip import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the tooltip sample application
try:
  app = launchToolTip(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
ttFrame = app.toolTipFrame

#move mouse to button to rise tooltip
ttFrame.mousePoint(ttFrame.button)
sleep(config.SHORT_DELAY)
ttFrame.assertTooltip("show button\'s tooltip")

#move mouse to checkbox to rise tooltip
ttFrame.mousePoint(ttFrame.checkbox)
sleep(config.SHORT_DELAY)
ttFrame.assertTooltip("my favorite fruit")

#close application frame window
ttFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
