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
from helpers import *
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

################################################
##check balloon Alert windows
##search for label and icon from Alert
##click balloon to close Alert
################################################

#click button rise notifyicon frame window
niFrame.click(niFrame.balloon_button)
sleep(config.SHORT_DELAY)
##Alert with wrong name BUG476859
niFrame.balloonWidgets()

#check states
##missing "mobal" and has extra "resizable" BUG476862
statesCheck(niFrame.balloon_alert, "Alert")
##incorrect states BUG476906
statesCheck(niFrame.label, "Label")
##incorrect states BUG476871
statesCheck(niFrame.icon, "Icon")

#mouse click notifyicon frame window would close it
balloon.mouseClick()
sleep(config.SHORT_DELAY)
balloon.assertClosed()

#close application frame window
niFrame.quit()


print "INFO:  Log written to: %s" % config.OUTPUT_DIR
