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
from helpers import *
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

#check for statusbar role
sbFrame.assertStatusBar()

#check statusbar's states
statesCheck(sbFrame.statusbar, "StatusBar")

#click button2 to change statusbar's text value
sbFrame.click(sbFrame.button2)
sleep(config.SHORT_DELAY)
sbFrame.assertText(sbFrame.statusbar, "Change texts 1 times")

#click button2 to change statusbar's text value again
sbFrame.click(sbFrame.button2)
sleep(config.SHORT_DELAY)
sbFrame.assertText(sbFrame.statusbar, "Change texts 2 times")

#close application frame window
sbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
