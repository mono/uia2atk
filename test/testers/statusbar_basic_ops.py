#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/15/2008
# Description: Test accessibility of statusbar widget 
#              Use the statusbarframe.py wrapper script
#              Test the samples/statusbar.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of statusbar widget
"""

# imports
import sys
import os

from strongwind import *
from statusbar import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the statusbar sample application
try:
  app = launchStatusBar(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
sbFrame = app.statusBarFrame

#check statusbar's states list
sbFrame.statesCheck()

#check for statusbar role
sbFrame.assertStatusBar()

#close application frame window
sbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
