#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/03/2009
# Description: Test accessibility of toolstripseparator widget 
#              Use the toolstripseparatorframe.py wrapper script
#              Test the samples/winforms/toolstripseparator.py script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of toolstripseparator widget
"""

# imports
import sys
import os

from strongwind import *
from toolstripseparator import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the toolstripseparator sample application
try:
  app = launchToolStripSeparator(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tssFrame = app.toolStripSeparatorFrame

# check separators' states
# the first sepatator is not enabled and sensitived
statesCheck(tssFrame.separators[0], "ToolStripSeparator", \
                                      invalid_states=["enabled", "sensitive"])

statesCheck(tssFrame.separators[1], "ToolStripSeparator")

statesCheck(tssFrame.separators[2], "ToolStripSeparator")

# close main window
tssFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
