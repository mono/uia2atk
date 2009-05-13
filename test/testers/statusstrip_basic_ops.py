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
This test is only focus on testing the components of the statusstrip which are
toolstriplabel and toolstripprogressbar. the other please see themselves' test

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

# check statusbar's states
statesCheck(ssFrame.statusstrip, "StatusBar")

##############################################################################
# test states and value for ToolStripProgressBar and ToolStripStatusLabel to 
# make sure children on StatusStrip also have correct states and event changed
##############################################################################

statesCheck(ssFrame.ProgressBar, "ProgressBar")
statesCheck(ssFrame.StripLabel, "Label")

# click button1 to change toolstriplabel and toolstripprogressbar
ssFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
ssFrame.assertLabel(ssFrame.StripLabel, "It is 20% of 100%")
ssFrame.assertProgressBarValue(ssFrame.ProgressBar, 20)
statesCheck(ssFrame.statusstrip, "StatusBar")

# click button1 again to change toolstriplabel and toolstripprogressbar
ssFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
ssFrame.assertLabel(ssFrame.StripLabel, "It is 40% of 100%")
ssFrame.assertProgressBarValue(ssFrame.ProgressBar, 40)
statesCheck(ssFrame.statusstrip, "StatusBar")

# close application frame window
ssFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
