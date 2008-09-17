#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/16/2008
# Description: Test accessibility of notifyicon widget 
#              Use the notifyiconframe.py wrapper script
#              Test the samples/novifyicon.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of novifyicon widget
"""

# imports
import sys
import os

from strongwind import *
from notifyicon import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the notifyicon sample application
try:
  app = launchNotifyIcon(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
niFrame = app.notifyIconFrame

#check Button's actions list
niFrame.actionsCheck(niFrame.button)

#click button rise notifyicon frame window
niFrame.click(niFrame.button)
sleep(config.SHORT_DELAY)
niFrame.assertNotifyIcon()

#check notifyicon frame's states list
niFrame.statesCheck(niFrame.notifyicon)

#close application frame window
niFrame.quit()


print "INFO:  Log written to: %s" % config.OUTPUT_DIR
