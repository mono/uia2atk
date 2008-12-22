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
Test accessibility of statusstrip widget which can include toolstriplabel,
toolstripdropdownbutton, toolstripsplitbutton and toolstripprogressbar.
we just give general test for label and progressbar which under statusstrip,
the complete test of each of them will be running for each single control test
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

#########################################################################################
#test states and value of ToolStripProgressBar and 
#ToolStripStatusLabel to make sure StatusStrip with "status bar" role
#have children which also with correct states and event changed
#########################################################################################
statesCheck(ssFrame.ProgressBar, "ProgressBar")
statesCheck(ssFrame.StripLabel, "Label")

#click button1 the first time to change toolstriplabel and toolstripprogressbar
ssFrame.click(ssFrame.button)
sleep(config.SHORT_DELAY)
ssFrame.assertLabel(ssFrame.StripLabel, "It is 20% of 100%")
ssFrame.assertProgressBarValue(ssFrame.ProgressBar, 20)

#click button1 the second time to change toolstriplabel and toolstripprogressbar
ssFrame.click(ssFrame.button)
sleep(config.SHORT_DELAY)
ssFrame.assertLabel(ssFrame.StripLabel, "It is 40% of 100%")
ssFrame.assertProgressBarValue(ssFrame.ProgressBar, 40)

#close application frame window
ssFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
