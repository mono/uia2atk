#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/06/2008
# Description: Test accessibility of statusstrip widget 
#              Use the statusstripframe.py wrapper script
#              Test the samples/statusstrip.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of statusstrip widget
"""

# imports
import sys
import os

from strongwind import *
from statusstrip import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the statusstrip sample application
try:
  app = launchStatusStrip(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
ssFrame = app.statusStripFrame

#check for statusbar role
ssFrame.assertStatusBar()

#check statusbar's states
statesCheck(ssFrame.statusbar, "StatusBar")

#############################################################################
#test status and values of ToolStripProgressBar and ToolStripStatusLabel in 
#StatusStrip
#############################################################################
statesCheck(ssFrame.ProgressBar, "ProgressBar")
statesCheck(ssFrame.StripLabel, "Label")

#click StripButton the first time
ssFrame.click(ssFrame.StripButton)
sleep(config.SHORT_DELAY)
ssFrame.assertLabel(ssFrame.StripLabel, "10%")

#click StripButton the second time
ssFrame.click(ssFrame.StripButton)
sleep(config.SHORT_DELAY)
ssFrame.assertValue(ssFrame.ProgressBar, 20)

#click StripButton the third time
ssFrame.click(ssFrame.StripButton)
sleep(config.SHORT_DELAY)
ssFrame.assertLabel(ssFrame.StripLabel, "30%")

#click StripButton the fourth time
ssFrame.click(ssFrame.StripButton)
sleep(config.SHORT_DELAY)
ssFrame.assertValue(ssFrame.ProgressBar, 40)

###############################################################
#test ToolStripDropDownButton's states and actions in StatusStrip
###############################################################
actionCheck(ssFrame.DropDownButton, "Button")
actionCheck(ssFrame.DropDownButton_item1, "MenuItem")
actionCheck(ssFrame.DropDownButton_item2, "MenuItem")

statesCheck(ssFrame.DropDownButton, "Button")
statesCheck(ssFrame.DropDownButton_item1, "MenuItem")
statesCheck(ssFrame.DropDownButton_item2, "MenuItem")

ssFrame.click(ssFrame.DropDownButton_item1)
sleep(config.SHORT_DELAY)
ssFrame.assertTextColor(ssFrame.MainLabel, "Red")

ssFrame.click(ssFrame.DropDownButton_item2)
sleep(config.SHORT_DELAY)
ssFrame.assertTextColor(ssFrame.MainLabel, "Blue")

###############################################################
#test ToolStripSplitButton's states and actions in StatusStrip
###############################################################
actionCheck(ssFrame.SplitButton, "Button")
actionCheck(ssFrame.SplitButton_item1, "MenuItem")
actionCheck(ssFrame.SplitButton_item2, "MenuItem")

statesCheck(ssFrame.SplitButton, "Button")
statesCheck(ssFrame.SplitButton_item1, "MenuItem")
statesCheck(ssFrame.SplitButton_item2, "MenuItem")

ssFrame.click(ssFrame.SplitButton_item1)
sleep(config.SHORT_DELAY)
ssFrame.assertTextColor(ssFrame.StripLabel, "Blue")

ssFrame.click(ssFrame.SplitButton_item2)
sleep(config.SHORT_DELAY)
ssFrame.assertTextColor(ssFrame.StripLabel, "Red")


#close application frame window
sbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
