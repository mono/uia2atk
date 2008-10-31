#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/31/2008
# Description: Test accessibility of toolstrip widget 
#              Use the toolstripframe.py wrapper script
#              Test the samples/toolstrip.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of toolstrip widget
"""

# imports
import sys
import os

from strongwind import *
from toolstrip import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the toolstrip sample application
try:
  app = launchToolStrip(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tsFrame = app.toolStripFrame

#assert if pyatspi can get toolbar role for toolstrip
tsFrame.assertToolStrip()

#check toolstrip's states
statesCheck(tsFrame.toolstrip, "ToolBar")

#close main window
tsFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
