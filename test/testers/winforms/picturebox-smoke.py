#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/15/2008
# Description: Test accessibility of picturebox widget 
#              Use the pictureboxframe.py wrapper script
#              Test the samples/winforms/picturebox.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of picturebox widget
"""

# imports
import sys
import os

from strongwind import *
from picturebox import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the picturebox sample application
try:
  app = launchPictureBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
pbFrame = app.pictureBoxFrame

#check the actions of button in picturebox
actionsCheck(pbFrame.button, "Button")

#check the states of button and label in picturebox
statesCheck(pbFrame.button, "Button", add_states=["focused"])
statesCheck(pbFrame.label, "Label")

#click button changing to universi.jpg
pbFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
pbFrame.assertName(pbFrame.label, "show universe300x400.jpg")

#click button changing to desktop-blue_soccer.jpg
pbFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
pbFrame.assertName(pbFrame.label, "show desktop-blue_soccer400x500.jpg")

#close application frame window
pbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
