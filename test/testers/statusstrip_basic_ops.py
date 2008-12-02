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
Test accessibility of statusstrip widget which include toolstriplabel, 
toolstripdropdownbutton, toolstripsplitbutton and toolstripprogressbar.
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

#check statusbar's states
statesCheck(ssFrame.statusstrip, "StatusBar")

#############################################################################
#test status and value of ToolStripProgressBar and ToolStripStatusLabel in 
#StatusStrip
#############################################################################
statesCheck(ssFrame.ProgressBar, "ProgressBar")
statesCheck(ssFrame.StripLabel, "Label")

#click StripButton the first time
ssFrame.click(ssFrame.button)
sleep(config.SHORT_DELAY)
ssFrame.assertLabel(ssFrame.StripLabel, "It is 20% of 100%")
ssFrame.assertProgressBarValue(ssFrame.ProgressBar, 20)

#click StripButton the second time
ssFrame.click(ssFrame.button)
sleep(config.SHORT_DELAY)
ssFrame.assertLabel(ssFrame.StripLabel, "It is 40% of 100%")
ssFrame.assertProgressBarValue(ssFrame.ProgressBar, 40)

###############################################################
#test ToolStripDropDownButton's states and actions in StatusStrip
###############################################################
actionsCheck(ssFrame.DropDownButton_item1, "MenuItem")
actionsCheck(ssFrame.DropDownButton_item2, "MenuItem")

statesCheck(ssFrame.DropDownButton_item1, "MenuItem")
statesCheck(ssFrame.DropDownButton_item2, "MenuItem")

ssFrame.click(ssFrame.DropDownButton_item1)
sleep(config.SHORT_DELAY)
ssFrame.assertLabel(ssFrame.MainLabel, "You selected Red")

ssFrame.click(ssFrame.DropDownButton_item2)
sleep(config.SHORT_DELAY)
ssFrame.assertLabel(ssFrame.MainLabel, "You selected Blue")

###############################################################
#test ToolStripSplitButton's states and actions in StatusStrip
###############################################################
actionsCheck(ssFrame.SplitButton_item1, "MenuItem")
actionsCheck(ssFrame.SplitButton_item2, "MenuItem")

statesCheck(ssFrame.SplitButton_item1, "MenuItem")
statesCheck(ssFrame.SplitButton_item2, "MenuItem")

ssFrame.click(ssFrame.SplitButton_item1)
sleep(config.SHORT_DELAY)
ssFrame.assertLabel(ssFrame.StripLabel, "You selected Blue Color")

ssFrame.click(ssFrame.SplitButton_item2)
sleep(config.SHORT_DELAY)
ssFrame.assertLabel(ssFrame.StripLabel, "You selected Red Color")


#close application frame window
ssFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
