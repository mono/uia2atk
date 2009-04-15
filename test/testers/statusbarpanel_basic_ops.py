#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/10/2008
# Description: Test accessibility of statusbarpanel widget 
#              Use the statusbarpanelframe.py wrapper script
#              Test the samples/statusbarpanel.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of statusbarpanel widget
"""

# imports
import sys
import os
import time

from strongwind import *
from statusbarpanel import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the statusbarpanel sample application
try:
  app = launchStatusBarPanel(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
sbpFrame = app.statusBarPanelFrame

# check statusbarpanel's states
statesCheck(sbpFrame.panel1, "StatusBarPanel")
statesCheck(sbpFrame.panel2, "StatusBarPanel")
statesCheck(sbpFrame.panel3, "StatusBarPanel")

# click button1 to change statusbarpanel's text value
sbpFrame.click(sbpFrame.button1)
sleep(config.SHORT_DELAY)
sbpFrame.assertText(sbpFrame.panel1, "You have click 1 times")

# click button1 to change statusbarpanel's text value again
sbpFrame.click(sbpFrame.button1)
sleep(config.SHORT_DELAY)
sbpFrame.assertText(sbpFrame.panel1, "You have click 2 times")

# make sure the text is uneditable
sbpFrame.assertUnimplementedEditableText(sbpFrame.panel1)

sbpFrame.assertUnimplementedEditableText(sbpFrame.panel2)

sbpFrame.assertUnimplementedEditableText(sbpFrame.panel3)

# check image size to make sure Image is implemented
## BUG455927 is reopened that icons have incorrect image size (-1x-1)
#sbpFrame.assertImageSize(sbpFrame.panel1, width=16, height=16)
#sbpFrame.assertImageSize(sbpFrame.panel2, width=48, height=48)
#sbpFrame.assertImageSize(sbpFrame.panel3, width=32, height=32)

# close application frame window
sbpFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
